using AmazonBestSellers.Application.DTOs.Auth;
using FluentValidation;

namespace AmazonBestSellers.Application.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .Length(5, 50)
            .Matches("^[a-zA-Z0-9]{5,50}$");

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8).WithMessage("Password must be at least 8 characters");
    }
}
