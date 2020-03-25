using FluentValidation;
using JetBrains.Annotations;
using Lykke.Service.Credentials.Client.Models.Requests;
using Lykke.Service.Credentials.Domain.Services;

namespace Lykke.Service.Credentials.Models.Validation
{
    [UsedImplicitly]
    public class UpdateRequestCredentialsValidator : AbstractValidator<CredentialsUpdateRequest>
    {
        public UpdateRequestCredentialsValidator(IPasswordValidator passwordValidator)
        {
            RuleFor(x => x.Login)
                .NotEmpty()
                .WithMessage("Email address is required.")
                .EmailAddress()
                .WithMessage("A valid email address is required.");
            RuleFor(x => x.CustomerId)
                .NotNull()
                .WithMessage("CustomerId is required.")
                .NotEmpty()
                .WithMessage("CustomerId cannot be empty.");
            RuleFor(x => x.Password)
                .NotNull()
                .WithMessage("Password is a required field")
                .NotEmpty()
                .WithMessage("Password is a required field");
            RuleFor(x => x.Password)
                .Must(passwordValidator.IsValidPassword)
                .When(x => !string.IsNullOrEmpty(x.Password))
                .WithMessage(passwordValidator.BuildValidationMessage());
        }
    }
}
