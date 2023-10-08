using BillSplit.Contracts.Bill;
using BillSplit.Contracts.BillAllocation;
using BillSplit.Contracts.User;
using BillSplit.Domain.Exceptions;
using BillSplit.Domain.Models;
using BillSplit.Persistence.Repositories.Abstractions;
using BillSplit.Services.Abstractions.Interfaces;
using BillSplit.Services.Extensions;

namespace BillSplit.Services;

internal sealed class BillService : IBillService
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
        var bill = (await _billRepository.GetBill(id, true, true, cancellationToken)).ThrowIfNull(id);

        var userBillGroups = (await _userBillGroupRepository.GetUserBillGroupIds(user.Id, cancellationToken)).ThrowIfNull();

        if (!userBillGroups.Contains(bill.BillGroupId))
        {
            throw new NotFoundException(nameof(Bill));
        }

        var createdByUser = await _userService.GetUser(bill.CreatedBy);
        var paidByUser = await _userService.GetUser(bill.PaidBy);
        var billAllocationUsers = await _userService.GetUsers(bill.BillAllocations.Select(x => x.UserId).ToHashSet(), cancellationToken);

        return new BillDto(
            bill.Id,
            bill.PaidBy,
            paidByUser.Name,
            bill.Amount,
            bill.Comment,
            createdByUser.Name,
            bill.CreatedDate,
            bill.BillAllocations.Select(x =>
                new BillAllocationDto(
                    x.Id,
                    x.UserId,
                    billAllocationUsers.First(allocationUser => allocationUser.Id == x.UserId).Name,
                    x.Amount,
                    x.PaidAmount)));
    }

    public async Task<long> UpsertBill(UserClaims user, UpsertBillDto upsertBill, CancellationToken cancellationToken = default)
    {
        (await _billGroupRepository.GetBillGroups(cancellationToken, true, upsertBill.BillGroupId)).ThrowIfNull(upsertBill.BillGroupId);

        var billGroupUserIds = await _userBillGroupRepository.GetBillGroupUserIds(upsertBill.BillGroupId, cancellationToken);
        var userIdsToValidate = new List<long> { user.Id, upsertBill.PaidById };
        userIdsToValidate.AddRange(upsertBill.BillAllocations.Select(x => x.UserId));

        if (userIdsToValidate.Any(x => !billGroupUserIds.Contains(x)))
        {
            throw new NotFoundException("Not all users are part of the group");
        }

        if (upsertBill.Amount != upsertBill.BillAllocations.Sum(x => x.Amount))
        {
            throw new InvalidBillAllocationSetupException("The bill amount must equal to the sum of the allocations");
        }

        if (upsertBill.Id is not null)
        {
            return await UpdateBill(user, upsertBill.Id.Value, upsertBill, cancellationToken);
        }

        var bill = await _billRepository.CreateBill(new Bill(
            upsertBill.Amount,
            upsertBill.Comment,
            user.Id,
            upsertBill.BillGroupId,
            upsertBill.PaidById,
            upsertBill.BillAllocations.Select(x =>
                new BillAllocation(x.UserId, x.Amount, user.Id)).ToList()), cancellationToken);

        return bill.Id;
    }

    private async Task<long> UpdateBill(UserClaims user, long billId, UpsertBillDto upsertBill, CancellationToken cancellationToken)
    {
        var bill = (await _billRepository.GetBill(billId, false, false, cancellationToken)).ThrowIfNull(billId);

        bill.Amount = upsertBill.Amount;
        bill.Comment = upsertBill.Comment;
        bill.PaidBy = upsertBill.PaidById;
        bill.UpdatedBy = user.Id;

        foreach (var billAllocation in bill.BillAllocations)
        {
            billAllocation.IsDeleted = true;
            billAllocation.DeletedBy = user.Id;
            billAllocation.DeletedDate = DateTime.UtcNow;
        }

        foreach (var billAllocation in upsertBill.BillAllocations)
        {
            bill.BillAllocations.Add(new BillAllocation(billAllocation.UserId, billAllocation.Amount, user.Id));
        }

        return bill.Id;
    }

    public async Task Delete(UserClaims user, long id, CancellationToken cancellationToken = default)
    {
        var bill = await _billRepository.GetBill(id, true, false, cancellationToken);

        if (bill is null)
        {
            return;
        }

        var billGroupUserIds = (await _userBillGroupRepository.GetBillGroupUserIds(bill.BillGroupId, cancellationToken)).ThrowIfNull(bill.BillGroupId);

        if (!billGroupUserIds.Contains(user.Id))
        {
            throw new ForbiddenException(id);
        }

        bill.IsDeleted = true;
        bill.DeletedBy = user.Id;
        bill.DeletedDate = DateTime.UtcNow;

        foreach (var billAllocation in bill.BillAllocations)
        {
            billAllocation.IsDeleted = true;
            billAllocation.DeletedBy = user.Id;
            billAllocation.DeletedDate = DateTime.UtcNow;
        }

        await _billRepository.UpdateBill(bill, cancellationToken);
    }
}