using BillSplit.Contracts.Authorization;
using FluentValidation;

namespace BillSplit.Domain.Validators.User;

public class SetPasswordDtoValidator : AbstractValidator<SetInitialPasswordDto>
{
    public SetPasswordDtoValidator()
    {
        RuleFor(u => u.UserId).GreaterThan(0);
        RuleFor(u => u.Password).NotEmpty().MinimumLength(6);
        RuleFor(u => u.Password).Equal(u => u.PasswordCheck)
            .WithMessage("The new password does not match the confirm password");
    }
}
