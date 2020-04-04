using FluentValidation;
using JetBrains.Annotations;
using MAVN.Service.Credentials.Client.Models.Requests;
using MAVN.Service.Credentials.Domain.Services;

namespace MAVN.Service.Credentials.Models.Validation
{
    [UsedImplicitly]
    public class AdminPasswordResetRequestValidator : AbstractValidator<AdminPasswordResetRequest>
    {
        public AdminPasswordResetRequestValidator(IPasswordValidator passwordValidator)
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(o => o.ResetIdentifier)
                .NotEmpty()
                .WithMessage("Reset identifier required.");

            RuleFor(o => o.Login)
                .NotEmpty()
                .WithMessage("Email required.")
                .EmailAddress()
                .WithMessage("Email invalid.");

            RuleFor(o => o.Password)
                .NotEmpty()
                .WithMessage("Password required.")
                .Must(passwordValidator.IsValidPassword)
                .WithMessage(passwordValidator.BuildValidationMessage());
        }
    }
}
