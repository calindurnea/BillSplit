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

internal sealed class BillGroupService : IBillGroupService
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

    public async Task<BillGroupDto> GetBillGroups(UserClaims user, long id, CancellationToken cancellationToken = default)
    {
        var billGroup = (await _billGroupRepository.GetBillGroups(cancellationToken, true, id)).FirstOrDefault().ThrowIfNull(id);

        if (!UserHasAccess(user, billGroup))
        {
            throw new UnauthorizedAccessException();
        }

        var bills = (await _billRepository.GetGroupBills(billGroup.Id, true, cancellationToken)).ToList();

        var billsCreatedBy = _userService.GetUsers(bills.Select(x => x.CreatedBy).ToHashSet(), cancellationToken);
        var billsPaidBy = _userService.GetUsers(bills.Select(x => x.PaidBy).ToHashSet(), cancellationToken);

        var tasks = new List<Task>();
        tasks.AddRange(new Task[] { billsCreatedBy, billsPaidBy });

        await Task.WhenAll(tasks);

        var userIds = bills
            .SelectMany(x => x.BillAllocations)
            .Select(x => x.UserId)
            .ToHashSet();

        var billsAllocationsUsers = await _userService.GetUsers(userIds, cancellationToken);

        return new BillGroupDto(
            billGroup.Id,
            billGroup.Name,
            bills.Select(bill => new BillDto(
                bill.Id,
                bill.PaidBy,
                billsPaidBy.Result.First(x => x.Id == bill.PaidBy).Name,
                bill.Amount,
                bill.Comment,
                billsCreatedBy.Result.First(x => x.Id == bill.CreatedBy).Name,
                bill.CreatedDate,
                bill.BillAllocations.Where(allocation => allocation.BillId == bill.Id)
                    .Select(allocation => new BillAllocationDto(
                        allocation.Id,
                        allocation.UserId,
                        billsAllocationsUsers.First(x => x.Id == allocation.UserId).Name,
                        allocation.Amount,
                        allocation.PaidAmount)))));
    }

    public async Task<IEnumerable<UserBillGroupDto>> GetBillGroups(UserClaims user, CancellationToken cancellationToken = default)
    {
        var userBillGroupIds = (await _userBillGroupRepository.GetUserBillGroupIds(user.Id, cancellationToken)).ThrowIfNull(user.Id).ToList();
        var billGroups = (await _billGroupRepository.GetBillGroups(cancellationToken, true, userBillGroupIds.ToArray()))
            .ThrowIfNull(userBillGroupIds.ToArray())
            .ToList();

        var bills = new List<Bill>();
        foreach (var billGroup in billGroups)
        {
            bills.AddRange(await _billRepository.GetGroupBills(billGroup.Id, true, cancellationToken));
        }

        return billGroups.Select(billGroup =>
        {
            var billGroupBills = bills.Where(bill => bill.BillGroupId == billGroup.Id).ToList();
            return new UserBillGroupDto(
                billGroup.Id,
                billGroup.Name,
                TotalAmount: billGroupBills.Sum(bill => bill.Amount),
                CurrentUserAmount: GetUserAmount(user.Id, billGroupBills.SelectMany(bill => bill.BillAllocations).ToList()));
        });
    }

    private static decimal GetUserAmount(long userId, IReadOnlyCollection<BillAllocation> billAllocations)
    {
        var userOwedAllocations = billAllocations
            .Where(billAllocation => billAllocation.Bill.PaidBy == userId && billAllocation.UserId != userId)
            .Sum(x => x.Amount);

        var userOwingAllocations = billAllocations
            .Where(billAllocation => billAllocation.Bill.PaidBy != userId && billAllocation.UserId == userId)
            .Sum(x => x.Amount - x.PaidAmount);

        return userOwedAllocations - userOwingAllocations;
    }

    public async Task<long> CreateBillGroup(UserClaims user, CreateBillGroupDto createBillGroup, CancellationToken cancellationToken = default)
    {
        await ValidateAllUsersExist(createBillGroup.UserIds, cancellationToken);

        var billGroup = new BillGroup(createBillGroup.Name, user.Id);
        billGroup.UserBillGroups.Add(new UserBillGroup(user.Id, user.Id));

        foreach (var userId in createBillGroup.UserIds)
        {
            billGroup.UserBillGroups.Add(new UserBillGroup(userId, user.Id));
        }

        billGroup = await _billGroupRepository.CreateBillGroup(billGroup, cancellationToken);

        return billGroup.Id;
    }

    public async Task UpdateBillGroupName(UserClaims user, long id, UpdateBillGroupNameDto updateBillGroupName, CancellationToken cancellationToken = default)
    {
        var billGroup = await GetBillGroupIfAccessible(user, id, false, cancellationToken);

        billGroup.Name = updateBillGroupName.Name;
        billGroup.UpdatedBy = user.Id;

        await _billGroupRepository.UpdateBillGroup(billGroup, cancellationToken);
    }

    public async Task RemoveBillGroupUser(UserClaims user, long id, long userId, CancellationToken cancellationToken = default)
    {
        var billGroup = await GetBillGroupIfAccessible(user, id, false, cancellationToken);

        var userBillGroup = await _userBillGroupRepository.GetUserBillGroup(userId, billGroup.Id, false, cancellationToken);

        if (userBillGroup is null)
        {
            return;
        }

        var userBillGroupAllocations = await _billAllocationRepository.GetUserBillGroupAllocations(userId, billGroup.Id, cancellationToken);

        if (userBillGroupAllocations.Any(x => x.Amount > x.PaidAmount))
        {
            throw new UnsettledBillAllocationsException("This user cannot be removed because of unsettled bill allocations");
        }

        userBillGroup.IsDeleted = true;
        userBillGroup.DeletedBy = user.Id;
        userBillGroup.DeletedDate = DateTime.UtcNow;

        await _userBillGroupRepository.UpdateUserBillGroup(userBillGroup, cancellationToken);
    }

    public async Task AddBillGroupUser(UserClaims user, long id, long userId, CancellationToken cancellationToken = default)
    {
        var billGroup = await GetBillGroupIfAccessible(user, id, false, cancellationToken);

        var userBillGroup = await _userBillGroupRepository.GetUserBillGroup(userId, billGroup.Id, true, cancellationToken);

        if (userBillGroup is not null)
        {
            return;
        }

        billGroup.UserBillGroups.Add(new UserBillGroup(userId, user.Id));

        await _billGroupRepository.UpdateBillGroup(billGroup, cancellationToken);
    }

    public async Task DeleteBillGroup(UserClaims user, long id, CancellationToken cancellationToken = default)
    {
        BillGroup billGroup;
        try
        {
            billGroup = await GetBillGroupIfAccessible(user, id, false, cancellationToken);
        }
        catch (NotFoundException)
        {
            return;
        }

        var billGroupAllocations = await _billAllocationRepository.GetBillGroupAllocations(id, cancellationToken);

        if (billGroupAllocations.Any(x => x.Amount > x.PaidAmount))
        {
            throw new UnsettledBillAllocationsException("This group cannot be deleted because of unsettled bill allocations");
        }

        billGroup.IsDeleted = true;
        billGroup.DeletedBy = user.Id;
        billGroup.DeletedDate = DateTime.UtcNow;

        await _billGroupRepository.UpdateBillGroup(billGroup, cancellationToken);
    }

    private async Task<BillGroup> GetBillGroupIfAccessible(UserClaims user, long billGroupId, bool withNoTracking = true, CancellationToken cancellationToken = default)
    {
        var billGroup = (await _billGroupRepository.GetBillGroups(cancellationToken, withNoTracking, billGroupId)).FirstOrDefault().ThrowIfNull(billGroupId);

        var userBillGroupIds = (await _userBillGroupRepository.GetUserBillGroupIds(user.Id, cancellationToken)).ThrowIfNull();

        if (!userBillGroupIds.Contains(billGroupId))
        {
            throw new ForbiddenException(billGroupId);
        }

        return billGroup;
    }

    private async Task ValidateAllUsersExist(IEnumerable<long> userIds, CancellationToken cancellationToken = default)
    {
        await _userService.GetUsers(userIds.ToHashSet(), cancellationToken);
    }

    private static bool UserHasAccess(UserClaims user, BillGroup billGroup)
    {
        return billGroup.CreatedBy == user.Id ||
               billGroup.UserBillGroups.Any(x => x.UserId == user.Id);
    }
}