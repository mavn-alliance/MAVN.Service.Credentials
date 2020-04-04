using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;
using Refit;
using System.Threading.Tasks;
using MAVN.Service.Credentials.Client.Models.Requests;
using MAVN.Service.Credentials.Client.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace MAVN.Service.Credentials.Client.Api
{
    /// <summary>
    /// Credentials client API interface.
    /// </summary>
    [PublicAPI]
    public interface ICredentialsApi
    {
        /// <summary>
        /// Validates customer credentials.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <returns><see cref="CredentialsValidationResponse"/></returns>
        [Post("/api/credentials/validate")]
        Task<CredentialsValidationResponse> ValidateCredentialsAsync([Body] CredentialsValidationRequest request);

        /// <summary>
        /// Create customer credentials
        /// </summary>
        /// <param name="request">Request.</param>
        /// <returns><see cref="CredentialsCreateResponse"/></returns>
        [Post("/api/credentials")]
        Task<CredentialsCreateResponse> CreateAsync([Body]CredentialsCreateRequest request);

        /// <summary>
        /// Update the customer password 
        /// </summary>
        /// <param name="request">Request.</param>
        /// <returns><see cref="CredentialsUpdateResponse"/></returns>
        [Put("/api/credentials")]
        Task<CredentialsUpdateResponse> ChangePasswordAsync([Body]CredentialsUpdateRequest request);

        /// <summary>
        /// Updates customer login.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <returns><see cref="CredentialsUpdateResponse"/></returns>
        [Put("/api/credentials/login")]
        Task<CredentialsUpdateResponse> UpdateLoginAsync([Body] LoginUpdateRequest request);

        /// <summary>
        /// Remove customer credentials
        /// </summary>
        /// <param name="login">Login.</param>
        [Delete("/api/credentials/{login}")]
        Task RemoveAsync([Required] [FromRoute]string login);

        /// <summary>
        /// Generates Password reset Identifier for the Customer
        /// </summary>
        /// <param name="customerId">Internal Customer Id.</param>
        /// /// <returns><see cref="PasswordResetResponseModel"/></returns>
        [Get("/api/credentials/resetIdentifier/{customerId}")]
        Task<PasswordResetResponseModel> GenerateResetIdentifierAsync([Required] [FromRoute]string customerId);

        /// <summary>
        /// Resets the Password if the provided Identifier matches
        /// </summary>
        /// <param name="request" cref="PasswordResetRequest">Contains information about the Password Reset request</param>
        /// /// <returns><see cref="PasswordResetErrorResponse"/></returns>
        [Post("/api/credentials/password-reset")]
        Task<PasswordResetErrorResponse> PasswordResetAsync([Body] PasswordResetRequest request);

        /// <summary>
        /// Validates the provided identifier
        /// </summary>
        /// <param name="request" cref="ResetIdentifierValidationRequest">Contains information about the request</param>
        /// /// <returns><see cref="ResetIdentifierValidationResponse"/></returns>
        [Post("/api/credentials/reset-validation")]
        Task<ResetIdentifierValidationResponse> ValidateIdentifierAsync(ResetIdentifierValidationRequest request);

        /// <summary>
        /// Creates an Id for a Customer
        /// </summary>
        /// <param name="request" cref="GenerateClientIdRequest">Contains information about the request</param>
        /// <response code="200">string.</response>
        [Post("/api/credentials/clientId")]
        Task<string> GenerateClientIdAsync(GenerateClientIdRequest request);

        /// <summary>
        /// Generates a random Secret of a given length
        /// </summary>
        /// <param name="request" cref="GenerateClientSecretRequest">Contains information about the request</param>
        /// <response code="200">string.</response>
        [Post("/api/credentials/clientSecret")]
        Task<string> GenerateClientSecretAsync(GenerateClientSecretRequest request);
        /// <summary>
        /// Set pin code for customer
        /// </summary>
        /// <param name="request" cref="SetPinRequest">Contains information about the request</param>
        /// /// <returns><see cref="SetPinResponse" /></returns>
        [Post("/api/credentials/pin")]
        Task<SetPinResponse> SetPinAsync([FromBody] SetPinRequest request);

        /// <summary>
        /// Validates customer pin code.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <returns><see cref="ValidatePinResponse"/></returns>
        [Post("/api/credentials/pin/validate")]
        Task<ValidatePinResponse> ValidatePinAsync([FromBody] ValidatePinRequest request);

        /// <summary>
        /// Check if pin code is set for this customer
        /// </summary>
        /// <param name="customerId">Request.</param>
        /// <returns><see cref="HasPinResponse"/></returns>
        [Get("/api/credentials/pin/has-pin")]
        Task<HasPinResponse> HasPinAsync([FromQuery] string customerId);

        /// <summary>
        /// Update pin code for customer
        /// </summary>
        /// <param name="request" cref="SetPinRequest">Contains information about the request</param>
        /// /// <returns><see cref="SetPinResponse" /></returns>
        [Put("/api/credentials/pin")]
        Task<SetPinResponse> UpdatePinAsync([FromBody] SetPinRequest request);
    }
}
