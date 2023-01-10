using BillSplit.Contracts.User;
using FluentValidation;

namespace BillSplit.Domain.Validators.User;

public class UpsertUserDtoValidator : AbstractValidator<UpsertUserDto>
{
    public UpsertUserDtoValidator()
    {
        RuleFor(u => u.Email).EmailAddress();
        RuleFor(u => u.Name).NotEmpty().MaximumLength(350);
        // RuleFor(u => u.PhoneNumber).ExclusiveBetween(11111111, 99999999).WithMessage("Phone number must be a valid Danish number");
    }
}
