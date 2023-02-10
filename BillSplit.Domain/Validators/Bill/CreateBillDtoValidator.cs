using BillSplit.Contracts.Bill;
using BillSplit.Contracts.BillAllocation;
using BillSplit.Domain.Validators.BillAllocation;
using FluentValidation;

namespace BillSplit.Domain.Validators.Bill;

public class CreateBillDtoValidator : AbstractValidator<CreateBillDto>
{
    public CreateBillDtoValidator()
    {
        RuleFor(_ => _.BillGroupId).GreaterThan(0);
        RuleFor(_ => _.PaidById).GreaterThan(0);
            
        RuleFor(_ => _.Amount).GreaterThan(0)
            .Must((args, amount) => EqualSumOfAllocations(args.BillAllocations, amount))
            .WithMessage("The bill amount must equal to the sum of the allocations");
        RuleFor(_ => _.Comment).MaximumLength(512);
            
        RuleFor(_ => _.BillAllocations)
            .Must((args, billAllocations) => HaveUniqueUserIds(billAllocations))
            .WithMessage("A user can have maximum one bill allocation");
        RuleForEach(_ => _.BillAllocations).SetValidator(new CreateBillAllocationDtoValidator());
    }

    private static bool HaveUniqueUserIds(IEnumerable<CreateBillAllocationDto> billAllocations)
    {
        var ids = billAllocations.Select(x => x.UserId).ToList();
        return ids.Count == ids.Distinct().Count();
    }

    private static bool EqualSumOfAllocations(IEnumerable<CreateBillAllocationDto> billAllocations, decimal amount)
    {
        return amount == billAllocations.Sum(x => x.Amount);
    }
}