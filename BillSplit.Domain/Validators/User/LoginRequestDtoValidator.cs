using BillSplit.Contracts.Authorization;
using BillSplit.Contracts.User;
using FluentValidation;

namespace BillSplit.Domain.Validators.User;

public class LoginRequestDtoValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestDtoValidator()
    {
        RuleFor(u => u.Email).EmailAddress();
        RuleFor(u => u.Password).NotNull().NotEmpty();
    }
}
