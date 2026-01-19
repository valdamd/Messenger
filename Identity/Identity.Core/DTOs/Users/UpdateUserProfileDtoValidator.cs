using FluentValidation;
using Identity.Core.DTOs.Auth;

namespace Identity.Core.DTOs.Users;

public sealed class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
{
    public RegisterUserDtoValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
        RuleFor(x => x.Name).NotEmpty().MinimumLength(2);
    }
}
