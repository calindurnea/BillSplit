using BillSplit.Contracts.Bill;
using BillSplit.Contracts.BillAllocation;
using BillSplit.Contracts.BillGroup;
using BillSplit.Contracts.User;
using BillSplit.Domain.Exceptions;
using BillSplit.Domain.Models;
using BillSplit.Persistence.Repositories.Abstractions;
using BillSplit.Services.Abstractions.Interfaces;
using BillSplit.Services.Extensions;

namespace BillSplit.Services;

public class BillGroupService : IBillGroupService
{
    private readonly IBillGroupRepository _billGroupRepository;
    private readonly IUserBillGroupRepository _userBillGroupRepository;
    private readonly IUserService _userService;
    private readonly IBillRepository _billRepository;
    private readonly IBillAllocationRepository _billAllocationRepository;

    public BillGroupService(
        IBillGroupRepository billGroupRepository,
        IUserService userService,
        IUserBillGroupRepository userBillGroupRepository,
        IBillRepository billRepository,
        IBillAllocationRepository billAllocationRepository)
    {
        _billGroupRepository = billGroupRepository ?? throw new ArgumentNullException(nameof(billGroupRepository));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _userBillGroupRepository = userBillGroupRepository ?? throw new ArgumentNullException(nameof(userBillGroupRepository));
        _billRepository = billRepository ?? throw new ArgumentNullException(nameof(billRepository));
        _billAllocationRepository = billAllocationRepository ?? throw new ArgumentNullException(nameof(billAllocationRepository));
    }

    public async Task<BillGroupDto> Get(UserClaims user, long id, CancellationToken cancellationToken = default)
    {
        var billGroup = (await _billGroupRepository.Get(cancellationToken, true, id)).ThrowIfNull(id).First();

        if (!UserHasAccess(user, billGroup))
        {
            throw new UnauthorizedAccessException();
        }

        var bills = (await _billRepository.GetGroupBills(billGroup.Id, cancellationToken)).ToList();

        var billsCreatedBy = _userService.Get(bills.Select(x => x.CreatedBy), cancellationToken);
        var billsPaidBy = _userService.Get(bills.Select(x => x.PaidBy), cancellationToken);
        var billsAllocations = _billAllocationRepository.GetBillsAllocations(bills.Select(x => x.Id), cancellationToken);

        var tasks = new List<Task>();
        tasks.AddRange(new Task[] { billsCreatedBy, billsPaidBy, billsAllocations });

        await Task.WhenAll(tasks);

        var billsAllocationsUsers = await _userService.Get(billsAllocations.Result.Select(x => x.UserId), cancellationToken);

        return new BillGroupDto(
            billGroup.Id,
            billGroup.Name,
            bills.Select(bill => new BillDto(
                bill.Id,
                bill.PaidBy,
                billsPaidBy.Result.First(x => x.Id == bill.CreatedBy).Name,
                bill.Amount,
                bill.Comment,
                billsCreatedBy.Result.First(x => x.Id == bill.CreatedBy).Name,
                bill.CreatedDate,
                billsAllocations.Result.Where(allocation => allocation.BillId == bill.Id)
                    .Select(allocation => new BillAllocationDto(
                        allocation.Id,
                        allocation.UserId,
                        billsAllocationsUsers.First(x => x.Id == allocation.UserId).Name,
                        allocation.Amount)))));
    }

    public async Task<IEnumerable<UserBillGroupDto>> Get(UserClaims user, CancellationToken cancellationToken = default)
    {
        var userBillGroupIds = (await _userBillGroupRepository.GetUserBillGroupIds(user.Id, cancellationToken)).ThrowIfNull(user.Id);
        var billGroups = (await _billGroupRepository.Get(cancellationToken, true, userBillGroupIds.ToArray())).ThrowIfNull(userBillGroupIds.ToArray());

        return billGroups.Select(billGroup =>
            new UserBillGroupDto(
                billGroup.Id,
                billGroup.Name,
                TotalAmount: billGroup.Bills.Sum(bill => bill.Amount),
                CurrentUserAmount: GetUserAmount(user.Id, billGroup)));
    }

    private static decimal GetUserAmount(long userId, BillGroup billGroup)
    {
        return billGroup.Bills
            .SelectMany(x => x.BillAllocations)
            .Where(x => x.UserId == userId)
            .Sum(x => x.Amount);
    }

    public async Task<long> Create(UserClaims user, CreateBillGroupDto createBillGroup, CancellationToken cancellationToken = default)
    {
        await ValidateAllUsersExist(createBillGroup.UserIds, cancellationToken);

        var billGroup = new BillGroup(createBillGroup.Name, user.Id);
        foreach (var userId in createBillGroup.UserIds)
        {
            billGroup.UserBillGroups.Add(new UserBillGroup(userId, user.Id));
        }

        billGroup = await _billGroupRepository.Create(billGroup, cancellationToken);

        return billGroup.Id;
    }

    public async Task UpdateName(UserClaims user, long id, UpdateBillGroupNameDto updateBillGroupName, CancellationToken cancellationToken = default)
    {
        var billGroup = await GetBillGroupIfAccessible(user, id, false, cancellationToken);

        billGroup.Name = updateBillGroupName.Name;

        await _billGroupRepository.Update(billGroup, cancellationToken);
    }

    public async Task RemoveUser(UserClaims user, long id, long userId, CancellationToken cancellationToken = default)
    {
        var billGroup = await GetBillGroupIfAccessible(user, id, false, cancellationToken);

        var userBillGroup = await _userBillGroupRepository.Get(userId, billGroup.Id, false, cancellationToken);

        if (userBillGroup is null)
        {
            return;
        }

        var userBillGroupAllocations = await _billAllocationRepository.GetUserBillGroupAllocations(userId, billGroup.Id, cancellationToken);

        if (userBillGroupAllocations.Any(x => x.Amount > 0))
        {
            throw new UserBillAllocationException("This user cannot be removed because of unsettled bill allocations");
        }

        userBillGroup.IsDeleted = true;
        userBillGroup.DeletedBy = user.Id;
        await _userBillGroupRepository.Update(userBillGroup, cancellationToken);
    }

    public async Task AddUser(UserClaims user, long id, long userId, CancellationToken cancellationToken = default)
    {
        var billGroup = await GetBillGroupIfAccessible(user, id, false, cancellationToken);

        var userBillGroup = await _userBillGroupRepository.Get(userId, billGroup.Id, true, cancellationToken);

        if (userBillGroup is not null)
        {
            return;
        }

        billGroup.UserBillGroups.Add(new UserBillGroup(userId, user.Id));
        await _billGroupRepository.Update(billGroup, cancellationToken);
    }

    private async Task<BillGroup> GetBillGroupIfAccessible(UserClaims user, long billGroupId, bool withNoTracking = true, CancellationToken cancellationToken = default)
    {
        var billGroup = (await _billGroupRepository.Get(cancellationToken, withNoTracking, billGroupId)).ThrowIfNull(billGroupId).First();

        var userBillGroupIds = (await _userBillGroupRepository.GetUserBillGroupIds(user.Id, cancellationToken)).ThrowIfNull();

        if (!userBillGroupIds.Contains(billGroupId))
        {
            throw new ForbiddenException(billGroupId);
        }

        return billGroup;
    }

    private async Task ValidateAllUsersExist(IEnumerable<long> userIds, CancellationToken cancellationToken = default)
    {
        await _userService.Get(userIds, cancellationToken);
    }

    private static bool UserHasAccess(UserClaims user, BillGroup? billGroup)
    {
        return billGroup.CreatedBy == user.Id ||
               billGroup.UserBillGroups.Any(x => x.UserId == user.Id);
    }
}