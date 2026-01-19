using FluentValidation;
using Identity.Core.DTOs.Users;

namespace Identity.Core.DTOs.Auth;

public sealed class UpdateUserProfileDtoValidator : AbstractValidator<UpdateUserProfileDto>
{
    public UpdateUserProfileDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MinimumLength(2).MaximumLength(100);
    }
}
