using BillSplit.Contracts.BillAllocation;
using FluentValidation;

namespace BillSplit.Domain.Validators;

public class CreateBillAllocationDtoValidator : AbstractValidator<CreateBillAllocationDto>
{
    public CreateBillAllocationDtoValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0);
        RuleFor(x => x.Amount).GreaterThanOrEqualTo(0);
    }
}