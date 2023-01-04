using BillSplit.Contracts.User;
using FluentValidation;

namespace BillSplit.Domain.Validators;

public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserDtoValidator()
    {
        RuleFor(u => u.Email).EmailAddress();
        RuleFor(u => u.Name).NotEmpty().MaximumLength(350);
        RuleFor(u => u.PhoneNumber).ExclusiveBetween(11111111, 99999999).WithMessage("Phone number must be a valid Danish number");
    }
}
