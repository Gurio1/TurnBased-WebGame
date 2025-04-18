using FastEndpoints;
using FluentValidation;

namespace Game.Features.Identity.IsEmailUnique;

public class IsEmailUniqueRequestValidator : Validator<IsEmailUniqueRequest>
{
    public IsEmailUniqueRequestValidator() =>
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email address is required")
            .EmailAddress()
            .WithMessage("Invalid email");
}
