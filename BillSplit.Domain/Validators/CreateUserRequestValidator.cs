using BillSplit.Contracts.User;
using FluentValidation;

namespace BillSplit.Domain.Validators;

public class CreateUserRequestValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserRequestValidator()
    {
        RuleFor(u => u.Email).EmailAddress();
        RuleFor(u => u.Name).NotEmpty().MaximumLength(350);
        RuleFor(u => u.Password).NotEmpty().MinimumLength(8);
        RuleFor(u => u.PhoneNumber).ExclusiveBetween(11111111, 99999999).WithMessage("Phone number must be a valid Danish number");
    }
}
