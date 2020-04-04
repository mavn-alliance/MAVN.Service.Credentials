using FluentValidation;
using JetBrains.Annotations;
using MAVN.Service.Credentials.Client.Models.Requests;
using MAVN.Service.Credentials.Domain.Services;

namespace MAVN.Service.Credentials.Models.Validation
{
    [UsedImplicitly]
    public class PasswordResetRequestValidator : AbstractValidator<PasswordResetRequest>
    {
        public PasswordResetRequestValidator(IPasswordValidator passwordValidator)
        {
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
