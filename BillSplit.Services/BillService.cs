using BillSplit.Contracts.Bill;
using BillSplit.Contracts.BillAllocation;
using BillSplit.Contracts.User;
using BillSplit.Domain.Exceptions;
using BillSplit.Domain.Models;
using BillSplit.Persistence.Repositories.Abstractions;
using BillSplit.Services.Abstractions.Interfaces;
using BillSplit.Services.Extensions;
using Microsoft.AspNetCore.Components.Forms;

namespace BillSplit.Services;

internal class BillService : IBillService
{
    private readonly IBillRepository _billRepository;
    private readonly IUserService _userService;
    private readonly IUserBillGroupRepository _userBillGroupRepository;
    private readonly IBillAllocationRepository _billAllocationRepository;
    private readonly IBillGroupRepository _billGroupRepository;

    public BillService(
        IBillRepository billRepository,
        IUserService userService,
        IUserBillGroupRepository userBillGroupRepository,
        IBillAllocationRepository billAllocationRepository,
        IBillGroupRepository billGroupRepository)
    {
        _billRepository = billRepository ?? throw new ArgumentNullException(nameof(billRepository));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _userBillGroupRepository = userBillGroupRepository ?? throw new ArgumentNullException(nameof(userBillGroupRepository));
        _billAllocationRepository = billAllocationRepository ?? throw new ArgumentNullException(nameof(billAllocationRepository));
        _billGroupRepository = billGroupRepository ?? throw new ArgumentNullException(nameof(billGroupRepository));
    }

    public async Task<BillDto> GetBill(UserClaims user, long id, CancellationToken cancellationToken = default)
    {
        var bill = (await _billRepository.Get(id, cancellationToken)).ThrowIfNull(id);

        var userBillGroups = (await _userBillGroupRepository.GetUserBillGroupIds(user.Id, cancellationToken)).ThrowIfNull();

        if (!userBillGroups.Contains(bill.BillGroupId))
        {
            throw new NotFoundException(nameof(Bill));
        }

        var billAllocations = (await _billAllocationRepository.GetBillAllocations(bill.Id, cancellationToken)).ToList();

        var createdByUser = await _userService.GetUsers(bill.CreatedBy, cancellationToken);
        var paidByUser = await _userService.GetUsers(bill.PaidBy, cancellationToken);
        var billAllocationUsers = await _userService.GetUsers(billAllocations.Select(x => x.UserId), cancellationToken);

        return new BillDto(
            bill.Id,
            bill.PaidBy,
            paidByUser.Name,
            bill.Amount,
            bill.Comment,
            createdByUser.Name,
            bill.CreatedDate,
            billAllocations.Select(x =>
                new BillAllocationDto(
                    x.Id,
                    x.UserId,
                    billAllocationUsers.First(allocationUser => allocationUser.Id == x.UserId).Name,
                    x.Amount,
                    x.PaidAmount)));
    }

    public async Task<long> CreateBill(UserClaims user, CreateBillDto createBill, CancellationToken cancellationToken = default)
    {
        (await _billGroupRepository.Get(cancellationToken, true, createBill.BillGroupId)).ThrowIfNull(createBill.BillGroupId);

        var billGroupUserIds = await _userBillGroupRepository.GetBillGroupUserIds(createBill.BillGroupId, cancellationToken);
        var userIdsToValidate = new List<long> { user.Id, createBill.PaidById };
        userIdsToValidate.AddRange(createBill.BillAllocations.Select(x => x.UserId));

        if (userIdsToValidate.Any(x => !billGroupUserIds.Contains(x)))
        {
            throw new NotFoundException("Not all users are part of the group");
        }

        if (createBill.Amount != createBill.BillAllocations.Sum(x => x.Amount))
        {
            throw new InvalidBillAllocationSetup("The bill amount must equal to the sum of the allocations");
        }

        var bill = await _billRepository.Create(
            new Bill(
                createBill.Amount,
                createBill.Comment,
                user.Id,
                createBill.BillGroupId,
                createBill.PaidById,
                createBill.BillAllocations.Select(x => new BillAllocation(x.UserId, x.Amount, user.Id)).ToList()),
            cancellationToken);

        return bill.Id;
    }
}