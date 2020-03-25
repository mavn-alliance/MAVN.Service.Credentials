using FluentValidation;
using JetBrains.Annotations;
using Lykke.Service.Credentials.Client.Models.Requests;

namespace Lykke.Service.Credentials.Models.Validation
{
    [UsedImplicitly]
    public class GenerateClientSecretRequestValidator : AbstractValidator<GenerateClientSecretRequest>
    {
        public GenerateClientSecretRequestValidator()
        {
            RuleFor(x => x.Length)
                .InclusiveBetween(8, 100)
                .WithMessage("A valid Length between 8 and 100 (inclusive) is required.");

        }
    }
}