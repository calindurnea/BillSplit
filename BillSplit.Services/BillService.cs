using BillSplit.Contracts.Bill;
using BillSplit.Contracts.BillAllocation;
using BillSplit.Contracts.User;
using BillSplit.Domain.Exceptions;
using BillSplit.Domain.Models;
using BillSplit.Persistence.Repositories.Abstractions;
using BillSplit.Services.Abstractions.Interfaces;
using BillSplit.Services.Extensions;

namespace BillSplit.Services
{
    internal class BillService : IBillService
    {
        private readonly IBillRepository _billRepository;
        private readonly IUserService _userService;
        private readonly IUserBillGroupRepository _userBillGroupRepository;
        private readonly IBillAllocationRepository _billAllocationRepository;

        public BillService(
            IBillRepository billRepository,
            IUserService userService,
            IUserBillGroupRepository userBillGroupRepository,
            IBillAllocationRepository billAllocationRepository)
        {
            _billRepository = billRepository ?? throw new ArgumentNullException(nameof(billRepository));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _userBillGroupRepository = userBillGroupRepository ?? throw new ArgumentNullException(nameof(userBillGroupRepository));
            _billAllocationRepository = billAllocationRepository ?? throw new ArgumentNullException(nameof(billAllocationRepository));
        }

        public async Task<BillDto> Get(UserClaims user, long id, CancellationToken cancellationToken = default)
        {
            var bill = (await _billRepository.Get(id, cancellationToken)).ThrowIfNull(id);

            var userBillGroups = (await _userBillGroupRepository.GetUserBillGroupIds(user.Id, cancellationToken)).ThrowIfNull();

            if (!userBillGroups.Contains(bill.BillGroupId))
            {
                throw new NotFoundException(nameof(Bill));
            }

            var billAllocations = (await _billAllocationRepository.GetBillAllocations(bill.Id, cancellationToken)).ToList();

            var createdByUser = _userService.Get(bill.CreatedBy, cancellationToken);
            var paidByUser = _userService.Get(bill.PaidBy, cancellationToken);
            var billAllocationUsers = _userService.Get(billAllocations.Select(x => x.UserId), cancellationToken);

            var tasks = new List<Task>();
            tasks.AddRange(new Task[] { createdByUser, paidByUser, billAllocationUsers });

            await Task.WhenAll(tasks);

            return new BillDto(
                bill.Id,
                bill.PaidBy,
                paidByUser.Result.Name,
                bill.Amount,
                bill.Comment,
                createdByUser.Result.Name,
                bill.CreatedDate,
                billAllocations.Select(x =>
                    new BillAllocationDto(
                        x.Id,
                        x.UserId,
                        billAllocationUsers.Result.First(user => user.Id == x.UserId).Name,
                        x.Amount)));
        }
    }
}