using FastEndpoints;
using FluentValidation;

namespace Game.Features.Identity.CreateUser;

public class CreateRequestValidator : Validator<CreateRequest>
{
    public CreateRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email address is required")
            .EmailAddress()
            .WithMessage("Invalid email");


        RuleFor(x => x.Password)
            .MinimumLength(8)
            .WithMessage("Password cant be less than 8 symbols")
            .NotEmpty()
            .WithMessage("Password required")
            .Equal(x => x.ConfirmedPassword)
            .WithMessage("Password and Confirmed password should be equal")
            .Matches(@"\d").WithMessage("Password must contain at least one number.")
            .Matches(@"[!@#$%^&*()_+\[\]{}:;<>,.?/~\\-]").WithMessage("Password must contain at least one special symbol.");
    }
}
