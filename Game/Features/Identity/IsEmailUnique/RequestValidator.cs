using FastEndpoints;
using FluentValidation;

namespace Game.Features.Identity.IsEmailUnique;

public class RequestValidator : Validator<Request>
{
    public RequestValidator() =>
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email address is required")
            .EmailAddress()
            .WithMessage("Invalid email");
}
