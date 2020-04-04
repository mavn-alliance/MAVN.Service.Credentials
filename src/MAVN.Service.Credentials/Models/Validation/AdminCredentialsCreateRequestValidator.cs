using FluentValidation;
using JetBrains.Annotations;
using MAVN.Service.Credentials.Client.Models.Requests;
using MAVN.Service.Credentials.Domain.Services;

namespace MAVN.Service.Credentials.Models.Validation
{
    [UsedImplicitly]
    public class AdminCredentialsCreateRequestValidator : AbstractValidator<AdminCredentialsCreateRequest>
    {
        public AdminCredentialsCreateRequestValidator(IPasswordValidator passwordValidator)
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(o => o.AdminId)
                .NotEmpty()
                .WithMessage("Admin id required.");

            RuleFor(o => o.Login)
                .NotEmpty()
                .WithMessage("Email required.")
                .EmailAddress()
                .WithMessage("Email invalid.");

            RuleFor(o => o.Password)
                .NotEmpty()
                .WithMessage("Password required")
                .Must(passwordValidator.IsValidPassword)
                .WithMessage(passwordValidator.BuildValidationMessage());
        }
    }
}
