using FluentValidation;
using JetBrains.Annotations;
using MAVN.Service.Credentials.Client.Models.Requests;

namespace MAVN.Service.Credentials.Models.Validation
{
    [UsedImplicitly]
    public class GenerateClientIdRequestValidator : AbstractValidator<GenerateClientIdRequest>
    {
        public GenerateClientIdRequestValidator()
        {
            RuleFor(x => x.Length)
                .InclusiveBetween(2, 100)
                .WithMessage("A valid Length between 2 and 100 (inclusive) is required.");

        }
    }
}