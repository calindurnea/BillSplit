using BillSplit.Contracts.BillGroup;
using BillSplit.Contracts.User;
using BillSplit.Domain.Exceptions;
using BillSplit.Domain.Models;
using BillSplit.Persistence.Repositories.Abstractions;
using BillSplit.Services.Abstractions.Interfaces;

namespace BillSplit.Services.Services;

public class BillGroupService : IBillGroupService
{
    private readonly IBillGroupRepository _billGroupRepository;

    public BillGroupService(IBillGroupRepository billGroupRepository)
    {
        _billGroupRepository = billGroupRepository ?? throw new ArgumentNullException(nameof(billGroupRepository));
    }

    public async Task<BillGroupDto> Get(UserClaims user, long id, CancellationToken cancellationToken = default)
    {
        var billGroup = await _billGroupRepository.Get(id, cancellationToken);

        if (billGroup is null)
        {
            throw new NotFoundException(nameof(BillGroup), id);
        }

        if (!UserHasAccess(user, billGroup))
        {
            throw new UnauthorizedAccessException();
        }

        return new BillGroupDto(billGroup.Id, billGroup.Name);
    }

    public async Task<IEnumerable<UserBillGroupDto>> GetByUserId(long userId, CancellationToken cancellationToken = default)
    {
        var billGroups = await _billGroupRepository.GetByUserId(userId, cancellationToken);

        return billGroups.Select(billGroup =>
            new UserBillGroupDto(
                billGroup.Id,
                billGroup.Name,
                billGroup.Bills.Sum(bill => bill.Amount),
                billGroup.Bills.Where(bill => bill.CreatedBy == userId).Sum(bill => bill.Amount)));
    }

    public async Task<long> Create(UserClaims user, CreateBillGroupDto createBillGroup, CancellationToken cancellationToken = default)
    {
        var billGroup = await _billGroupRepository.Create(new BillGroup(createBillGroup.Name, user.Id), cancellationToken);

        return billGroup.Id;
    }

    private static bool UserHasAccess(UserClaims user, BillGroup? billGroup)
    {
        return billGroup.CreatedBy == user.Id ||
               billGroup.UserBillGroups.Any(x => x.UserId == user.Id);
    }
}