using System.Threading.Tasks;
using JetBrains.Annotations;
using MAVN.Service.Credentials.Client.Models.Requests;
using MAVN.Service.Credentials.Client.Models.Responses;
using Refit;

namespace MAVN.Service.Credentials.Client.Api
{
    /// <summary>
    /// Provides methods to work with administrator credentials.
    /// </summary>
    [PublicAPI]
    public interface IAdminsApi
    {
        /// <summary>
        /// Creates administrator credentials.
        /// </summary>
        /// <param name="request">The administrator credentials.</param>
        /// <returns>
        /// The result of administrator credentials creation.
        /// </returns>
        [Post("/api/admins")]
        Task<CredentialsCreateResponse> CreateAsync([Body] AdminCredentialsCreateRequest request);

        /// <summary>
        /// Validates administrator credentials.
        /// </summary>
        /// <param name="request">The administrator credentials.</param>
        /// <returns>
        /// The result of administrator credentials validation.
        /// </returns>
        [Post("/api/admins/validate")]
        Task<AdminCredentialsValidationResponse> ValidateAsync([Body] CredentialsValidationRequest request);

        /// <summary>
        /// Generates password reset identifier for an administrator.
        /// </summary>
        /// <param name="adminId">The administrator unique identifier.</param>
        /// <returns>
        /// The result of password resetting that contains a new identifier and error code.
        /// </returns>
        [Post("/api/admins/resetIdentifier")]
        Task<PasswordResetResponseModel> GenerateResetIdentifierAsync(string adminId);

        /// <summary>
        /// Validates the password reset identifier.
        /// </summary>
        /// <param name="request">The administrator password reset request</param>
        /// <returns>
        /// The result of the administrator password reset identifier validation.
        /// </returns>
        [Post("/api/admins/resetIdentifier/validate")]
        Task<ResetIdentifierValidationResponse> ValidateResetIdentifierAsync(
            [Body] ResetIdentifierValidationRequest request);

        /// <summary>
        /// Resets the administrator password.
        /// </summary>
        /// <param name="request">The administrator credentials.</param>
        /// <returns>
        /// The result of the administrator password reset.
        /// </returns>
        [Post("/api/admins/resetPassword")]
        Task<PasswordResetErrorResponse> ResetPasswordAsync([Body] AdminPasswordResetRequest request);

        /// <summary>
        /// Updates the administrator password.
        /// </summary>
        /// <param name="request">The administrator credentials.</param>
        /// <returns>
        /// The result of administrator password changes.
        /// </returns>
        [Put("/api/admins")]
        Task<CredentialsUpdateResponse> ChangePasswordAsync([Body] AdminCredentialsUpdateRequest request);

        /// <summary>
        /// Removes administrator credentials.
        /// </summary>
        /// <param name="login">The administrator login.</param>
        [Delete("/api/admins")]
        Task RemoveAsync(string login);
    }
}
