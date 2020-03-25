using FluentValidation;
using Lykke.Service.Credentials.Client.Models.Requests;

namespace Lykke.Service.Credentials.Models.Validation
{
    public class PartnerCredentialsValidationRequestValidator : AbstractValidator<PartnerCredentialsValidationRequest>
    {
        public PartnerCredentialsValidationRequestValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(x => x.ClientId)
                .NotNull()
                .NotEmpty()
                .WithMessage("Email address is required");

            RuleFor(x => x.ClientSecret)
                .NotNull()
                .NotEmpty()
                .WithMessage("Password is a required field");
        }
    }
}
