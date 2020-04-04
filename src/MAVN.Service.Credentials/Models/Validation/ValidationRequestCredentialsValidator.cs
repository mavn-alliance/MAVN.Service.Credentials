using FluentValidation;
using MAVN.Service.Credentials.Client.Models.Requests;

namespace MAVN.Service.Credentials.Models.Validation
{
    public class ValidationRequestCredentialsValidator : AbstractValidator<CredentialsValidationRequest>
    {
        public ValidationRequestCredentialsValidator()
        {
            RuleFor(x => x.Login)
                .NotEmpty()
                .WithMessage("Email address is required.")
                .EmailAddress()
                .WithMessage("A valid email address is required.");
            RuleFor(x => x.Password)
                .NotNull()
                .WithMessage("Password is required field")
                .NotEmpty()
                .WithMessage("Password is a required field");
        }
    }
}
