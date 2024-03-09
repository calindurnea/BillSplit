using BillSplit.Contracts.BillGroup;
using FluentValidation;

namespace BillSplit.Domain.Validators;

public class CreateBillGroupDtoValidator : AbstractValidator<CreateBillGroupDto>
{
    public CreateBillGroupDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(350);
    }
}