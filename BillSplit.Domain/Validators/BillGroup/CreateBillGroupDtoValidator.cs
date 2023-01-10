using BillSplit.Contracts.BillGroup;
using FluentValidation;

namespace BillSplit.Domain.Validators.BillGroup;

public class CreateBillGroupDtoValidator : AbstractValidator<CreateBillGroupDto>
{
    public CreateBillGroupDtoValidator()
    {
        RuleFor(_ => _.Name).NotEmpty().MaximumLength(350);
    }
}