using FluentValidation;

namespace MoExpenseTracker.Features.V0.Auth;

class AuthSignValidation : AbstractValidator<SignupDto>
{
    public AuthSignValidation()
    {
        RuleFor(dto => dto.Name)
            .NotEmpty()
            .MinimumLength(5)
            .MaximumLength(255);

        RuleFor(dto => dto.Email)
           .NotEmpty()
           .EmailAddress();

        RuleFor(dto => dto.Password)
           .NotEmpty()
           .MinimumLength(6)
           .MaximumLength(255);
    }
}

class AuthLoginValidation : AbstractValidator<LoginDto>
{
    public AuthLoginValidation()
    {
        RuleFor(dto => dto.Email)
           .NotEmpty()
           .EmailAddress();

        RuleFor(dto => dto.Password)
           .NotEmpty()
           .MinimumLength(6)
           .MaximumLength(255);
    }
}