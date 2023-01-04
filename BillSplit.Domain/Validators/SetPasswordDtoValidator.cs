using BillSplit.Contracts.User;
using FluentValidation;

namespace BillSplit.Domain.Validators;

public class SetPasswordDtoValidator : AbstractValidator<SetPasswordDto>
{
    public SetPasswordDtoValidator()
    {
        RuleFor(u => u.UserId).GreaterThan(0);
        RuleFor(u => u.Password).NotEmpty().MinimumLength(6);
        RuleFor(u => u.PasswordCheck).Equal(u => u.Password);
    }
}
