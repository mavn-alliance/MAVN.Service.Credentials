using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using Lykke.Service.Credentials.Client;
using Lykke.Service.Credentials.Client.Api;
using Lykke.Service.Credentials.Client.Enums;
using Lykke.Service.Credentials.Client.Models.Requests;
using Lykke.Service.Credentials.Client.Models.Responses;
using Lykke.Service.Credentials.Domain.Exceptions;
using Lykke.Service.Credentials.Domain.Services;
using Lykke.Service.Credentials.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.Credentials.Controllers
{
    [Route("api/admins")]
    [ModelStateValidationActionFilter]
    public class AdminsController : ControllerBase, IAdminsApi
    {
        private readonly IAdminCredentialsService _adminCredentialsService;

        public AdminsController(IAdminCredentialsService adminCredentialsService)
        {
            _adminCredentialsService = adminCredentialsService;
        }

        /// <summary>
        /// Creates administrator credentials.
        /// </summary>
        /// <param name="request">The administrator credentials.</param>
        /// <remarks>
        /// Error codes:
        /// - **LoginAlreadyExists**
        /// </remarks>
        /// <returns>
        /// 200 - The result of administrator credentials creation.
        /// </returns>
        [HttpPost]
        [ProducesResponseType(typeof(CredentialsCreateResponse), (int) HttpStatusCode.OK)]
        public async Task<CredentialsCreateResponse> CreateAsync([FromBody] AdminCredentialsCreateRequest request)
        {
            try
            {
                await _adminCredentialsService.CreateAsync(request.AdminId, request.Login, request.Password);
            }
            catch (AdminCredentialsAlreadyExistsException)
            {
                return new CredentialsCreateResponse {Error = CredentialsError.LoginAlreadyExists};
            }

            return new CredentialsCreateResponse {Error = CredentialsError.None};
        }

        /// <summary>
        /// Validates administrator credentials.
        /// </summary>
        /// <param name="request">The administrator credentials.</param>
        /// <remarks>
        /// Error codes:
        /// - **LoginNotFound**
        /// - **PasswordMismatch**
        /// </remarks>
        /// <returns>
        /// 200 - The result of administrator credentials validation.
        /// </returns>
        [HttpPost("validate")]
        [ProducesResponseType(typeof(AdminCredentialsValidationResponse), (int) HttpStatusCode.OK)]
        public async Task<AdminCredentialsValidationResponse> ValidateAsync(
            [FromBody] CredentialsValidationRequest request)
        {
            bool isValid;
            string adminId;

            try
            {
                (isValid, adminId) = await _adminCredentialsService.ValidateAsync(request.Login, request.Password);
            }
            catch (AdminCredentialsNotFoundException)
            {
                return new AdminCredentialsValidationResponse {Error = CredentialsError.LoginNotFound};
            }

            if (!isValid)
                return new AdminCredentialsValidationResponse {Error = CredentialsError.PasswordMismatch};

            return new AdminCredentialsValidationResponse {AdminId = adminId, Error = CredentialsError.None};
        }

        /// <summary>
        /// Generates password reset identifier for an administrator.
        /// </summary>
        /// <param name="adminId">The administrator unique identifier.</param>
        /// <remarks>
        /// Error codes:
        /// - **ReachedMaximumRequestForPeriod**
        /// </remarks>
        /// <returns>
        /// 200 - The result of password resetting that contains a new identifier and error code.
        /// </returns>
        [HttpPost("resetIdentifier")]
        [ProducesResponseType(typeof(PasswordResetResponseModel), (int) HttpStatusCode.OK)]
        public async Task<PasswordResetResponseModel> GenerateResetIdentifierAsync(
            [Required] [FromQuery] string adminId)
        {
            string identifier;

            try
            {
                identifier = await _adminCredentialsService.GenerateResetIdentifierAsync(adminId);
            }
            catch (IdentifierRequestsExceededException)
            {
                return new PasswordResetResponseModel {ErrorCode = PasswordResetError.ReachedMaximumRequestForPeriod};
            }

            return new PasswordResetResponseModel {Identifier = identifier, ErrorCode = PasswordResetError.None};
        }

        /// <summary>
        /// Validates the password reset identifier.
        /// </summary>
        /// <param name="request">The administrator password reset request.</param>
        /// <remarks>
        /// Error codes:
        /// - **IdentifierDoesNotExist**
        /// - **ProvidedIdentifierHasExpired**
        /// </remarks>
        /// <returns>
        /// 200 - The result of the administrator password reset identifier validation.
        /// </returns>
        [HttpPost("resetIdentifier/validate")]
        [ProducesResponseType(typeof(ResetIdentifierValidationResponse), (int) HttpStatusCode.OK)]
        public async Task<ResetIdentifierValidationResponse> ValidateResetIdentifierAsync(
            [FromBody] ResetIdentifierValidationRequest request)
        {
            bool isValid;

            try
            {
                isValid = await _adminCredentialsService.ValidateResetIdentifierAsync(request.ResetIdentifier);
            }
            catch (IdentifierDoesNotExistException)
            {
                return new ResetIdentifierValidationResponse {Error = ValidateIdentifierError.IdentifierDoesNotExist};
            }

            if (!isValid)
            {
                return new ResetIdentifierValidationResponse
                {
                    Error = ValidateIdentifierError.ProvidedIdentifierHasExpired
                };
            }

            return new ResetIdentifierValidationResponse {Error = ValidateIdentifierError.None};
        }

        /// <summary>
        /// Resets the administrator password.
        /// </summary>
        /// <param name="request">The administrator credentials.</param>
        /// <remarks>
        /// Error codes:
        /// - **LoginDoesNotExist**
        /// - **IdentifierDoesNotExist**
        /// - **ProvidedIdentifierHasExpired**
        /// - **IdentifierMismatch**
        /// </remarks>
        /// <returns>
        /// 200 - The result of the administrator password reset.
        /// </returns>
        [HttpPost("resetPassword")]
        [ProducesResponseType(typeof(PasswordResetErrorResponse), (int) HttpStatusCode.OK)]
        public async Task<PasswordResetErrorResponse> ResetPasswordAsync([FromBody] AdminPasswordResetRequest request)
        {
            try
            {
                await _adminCredentialsService.ResetPasswordAsync(request.Login, request.Password,
                    request.ResetIdentifier);
            }
            catch (AdminCredentialsNotFoundException)
            {
                return new PasswordResetErrorResponse {Error = PasswordResetError.LoginDoesNotExist};
            }
            catch (IdentifierDoesNotExistException)
            {
                return new PasswordResetErrorResponse {Error = PasswordResetError.IdentifierDoesNotExist};
            }
            catch (IdentifierHasExpiredException)
            {
                return new PasswordResetErrorResponse {Error = PasswordResetError.ProvidedIdentifierHasExpired};
            }
            catch (IdentifierMismatchException)
            {
                return new PasswordResetErrorResponse {Error = PasswordResetError.IdentifierMismatch};
            }

            return new PasswordResetErrorResponse {Error = PasswordResetError.None};
        }

        /// <summary>
        /// Updates the administrator password.
        /// </summary>
        /// <param name="request">The administrator credentials.</param>
        /// <remarks>
        /// Error codes:
        /// - **LoginNotFound**
        /// </remarks>
        /// <returns>
        /// 200 - The result of administrator password changes.
        /// </returns>
        [HttpPut]
        [ProducesResponseType(typeof(CredentialsUpdateResponse), (int) HttpStatusCode.OK)]
        public async Task<CredentialsUpdateResponse> ChangePasswordAsync(
            [FromBody] AdminCredentialsUpdateRequest request)
        {
            try
            {
                await _adminCredentialsService.ChangePasswordAsync(request.Login, request.Password);
            }
            catch (AdminCredentialsNotFoundException)
            {
                return new CredentialsUpdateResponse {Error = CredentialsError.LoginNotFound};
            }

            return new CredentialsUpdateResponse {Error = CredentialsError.None};
        }

        /// <summary>
        /// Removes administrator credentials.
        /// </summary>
        /// <param name="login">The administrator login.</param>
        /// <returns>
        /// 204 - The administrator credentials successfully removed.
        /// </returns>
        [HttpDelete]
        [ProducesResponseType(typeof(CredentialsUpdateResponse), (int) HttpStatusCode.NoContent)]
        public Task RemoveAsync([FromQuery] string login)
        {
            return _adminCredentialsService.RemoveAsync(login);
        }
    }
}
