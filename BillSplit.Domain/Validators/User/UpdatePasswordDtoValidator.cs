using BillSplit.Contracts.Authorization;
using BillSplit.Contracts.User;
using FluentValidation;

namespace BillSplit.Domain.Validators.User;

public class UpdatePasswordDtoValidator : AbstractValidator<UpdatePasswordDto>
{
    public UpdatePasswordDtoValidator()
    {
        RuleFor(u => u.Password).NotEmpty().MinimumLength(6);
        RuleFor(u => u.NewPassword).NotEqual(u => u.Password);
        RuleFor(u => u.NewPassword).Equal(u => u.NewPasswordCheck);
    }
}
