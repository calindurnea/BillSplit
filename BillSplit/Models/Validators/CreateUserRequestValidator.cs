using BillSplit.Models.Requests;
using FluentValidation;

namespace BillSplit.Models.Validators;

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(u => u.Email).EmailAddress();
        RuleFor(u => u.FirstName).NotEmpty().MaximumLength(150);
        RuleFor(u => u.LastName).NotEmpty().MaximumLength(150);
        RuleFor(u => u.PhoneNumber).ExclusiveBetween(11111111, 99999999).WithMessage("Phone number must be a valid Danish number");
    }
}
