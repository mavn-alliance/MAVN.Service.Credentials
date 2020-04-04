using System.Threading.Tasks;
using JetBrains.Annotations;
using MAVN.Service.Credentials.Client.Models.Requests;
using MAVN.Service.Credentials.Client.Models.Responses;
using Refit;

namespace MAVN.Service.Credentials.Client.Api
{
    /// <summary>
    /// Provides methods to work with partners credentials.
    /// </summary>
    [PublicAPI]
    public interface IPartnersApi
    {
        /// <summary>
        /// Creates partner credentials.
        /// </summary>
        /// <param name="request">Partner credentials.</param>
        /// <returns>The result of partner credentials creation.</returns>
        [Post("/api/partners")]
        Task<CredentialsCreateResponse> CreateAsync([Body] PartnerCredentialsCreateRequest request);

        /// <summary>
        /// Removes partners credentials.
        /// </summary>
        /// <param name="clientId">Partner client id.</param>
        [Delete("/api/partners")]
        Task RemoveAsync(string clientId);

        /// <summary>
        /// Updates the partner credentials.
        /// </summary>
        /// <param name="request">Partner credentials.</param>
        /// <returns>
        /// The result of partner credentials update changes.
        /// </returns>
        [Put("/api/partners")]
        Task<CredentialsUpdateResponse> UpdateAsync([Body] PartnerCredentialsUpdateRequest request);

        /// <summary>
        /// Validates partner credentials.
        /// </summary>
        /// <param name="request">Partner credentials.</param>
        /// <returns>
        /// The result of partner credentials validation.
        /// </returns>
        [Post("/api/partners/validate")]
        Task<PartnerCredentialsValidationResponse> ValidateAsync([Body] PartnerCredentialsValidationRequest request);
    }
}
