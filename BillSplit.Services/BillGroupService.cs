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
    private readonly IUserService _userService;

    public BillGroupService(IBillGroupRepository billGroupRepository, IUserService userService)
    {
        _billGroupRepository = billGroupRepository ?? throw new ArgumentNullException(nameof(billGroupRepository));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    public async Task<BillGroupDto> Get(UserClaims user, long id, CancellationToken cancellationToken = default)
    {
        var billGroup = (await _billGroupRepository.Get(id, cancellationToken)).ThrowIfNull(id);

        if (!UserHasAccess(user, billGroup))
        {
            throw new UnauthorizedAccessException();
        }

        return new BillGroupDto(billGroup.Id, billGroup.Name, billGroup.Bills.Sum(bill => bill.Amount));
    }

    public async Task<IEnumerable<UserBillGroupDto>> Get(UserClaims user, CancellationToken cancellationToken = default)
    {
        var billGroups = await _billGroupRepository.GetByUserId(user.Id, cancellationToken);

        if (!billGroups.Any())
        {
            throw new NotFoundException(nameof(BillGroup));
        }
        
        return billGroups.Select(billGroup =>
            new UserBillGroupDto(
                billGroup.Id,
                billGroup.Name,
                billGroup.Bills.Sum(bill => bill.Amount),
                billGroup.Bills.Where(bill => bill.CreatedBy == user.Id).Sum(bill => bill.Amount)));
    }

    public async Task<long> Create(UserClaims currentUser, CreateBillGroupDto createBillGroup, CancellationToken cancellationToken = default)
    {
        await ValidateAllUsersExist(createBillGroup.UserIds, cancellationToken);
        
        var billGroup = new BillGroup(createBillGroup.Name, currentUser.Id);
        foreach (var userId in createBillGroup.UserIds)
        {
            billGroup.UserBillGroups.Add(new UserBillGroup(userId, currentUser.Id));
        }
        
        billGroup = await _billGroupRepository.Create(billGroup, cancellationToken);

        return billGroup.Id;
    }

    private async Task ValidateAllUsersExist(ISet<long> userIds, CancellationToken cancellationToken = default)
    {
        await _userService.Get(userIds, cancellationToken);
    }

    private static bool UserHasAccess(UserClaims user, BillGroup? billGroup)
    {
        return billGroup.CreatedBy == user.Id ||
               billGroup.UserBillGroups.Any(x => x.UserId == user.Id);
    }
}