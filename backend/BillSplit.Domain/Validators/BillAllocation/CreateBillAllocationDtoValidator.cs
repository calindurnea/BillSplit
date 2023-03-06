using BillSplit.Contracts.BillAllocation;
using FluentValidation;

namespace BillSplit.Domain.Validators.BillAllocation;

public class CreateBillAllocationDtoValidator : AbstractValidator<CreateBillAllocationDto>
{
    public CreateBillAllocationDtoValidator()
    {
        RuleFor(_ => _.UserId).GreaterThan(0);
        RuleFor(_ => _.Amount).GreaterThanOrEqualTo(0);
    }
}