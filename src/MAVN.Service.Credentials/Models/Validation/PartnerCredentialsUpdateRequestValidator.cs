using FluentValidation;
using MAVN.Service.Credentials.Client.Models.Requests;
using MAVN.Service.Credentials.Domain.Services;

namespace MAVN.Service.Credentials.Models.Validation
{
    public class PartnerCredentialsUpdateRequestValidator : AbstractValidator<PartnerCredentialsUpdateRequest>
    {
        public PartnerCredentialsUpdateRequestValidator(IPasswordValidator passwordValidator)
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(o => o.ClientId)
                .NotNull()
                .NotEmpty()
                .WithMessage("Client id required.");

            RuleFor(o => o.ClientSecret)
                .NotNull()
                .NotEmpty()
                .WithMessage("Password required")
                .Must(passwordValidator.IsValidPassword)
                .WithMessage(passwordValidator.BuildValidationMessage());

            RuleFor(o => o.PartnerId)
                .NotNull()
                .NotEmpty()
                .WithMessage("Partner id required.");
        }
    }
}
