using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Common;
using Common.Log;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Common.Log;
using Lykke.Service.Credentials.Client;
using Lykke.Service.Credentials.Client.Api;
using Lykke.Service.Credentials.Client.Enums;
using Lykke.Service.Credentials.Client.Models.Requests;
using Lykke.Service.Credentials.Client.Models.Responses;
using Lykke.Service.Credentials.Domain.Services;
using Lykke.Service.Credentials.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.Credentials.Controllers
{
    [Route("api/credentials")]
    [ModelStateValidationActionFilter]
    public class CredentialsController : Controller, ICredentialsApi
    {
        private readonly ICustomerCredentialsService _customerCredentialsService;
        private readonly ICredentialsGeneratorService _credentialsGeneratorService;
        private readonly IPasswordResetService _passwordResetService;
        private readonly IMapper _mapper;
        private readonly ILog _log;

        public CredentialsController(
            ICustomerCredentialsService customerCredentialsService,
            ICredentialsGeneratorService credentialsGeneratorService,
            IPasswordResetService passwordResetService,
            IMapper mapper,
            ILogFactory logFactory)
        {
            _customerCredentialsService = customerCredentialsService;
            _credentialsGeneratorService = credentialsGeneratorService;
            _passwordResetService = passwordResetService;
            _mapper = mapper;
            _log = logFactory.CreateLog(this);
        }

        /// <summary>
        /// Validates customer credentials.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <returns><see cref="CredentialsValidationResponse"/></returns>
        [HttpPost("validate")]
        [ProducesResponseType(typeof(CredentialsValidationResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.BadRequest)]
        public async Task<CredentialsValidationResponse> ValidateCredentialsAsync(
            [FromBody] CredentialsValidationRequest request)
        {
            var credentials = await _customerCredentialsService.GetAsync(request.Login);

            if (credentials == null)
            {
                _log.Info("Login not found", request.Login.SanitizeEmail());
                return new CredentialsValidationResponse {Error = CredentialsError.LoginNotFound};
            }

            var isValid = _customerCredentialsService.Validate(credentials, request.Password);
            if (isValid)
                return new CredentialsValidationResponse {CustomerId = credentials.CustomerId};

            return new CredentialsValidationResponse {Error = CredentialsError.PasswordMismatch};
        }

        /// <summary>
        /// Creates customer credenetials.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <returns><see cref="CredentialsCreateResponse"/></returns>
        [HttpPost]
        [ProducesResponseType(typeof(CredentialsCreateResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.BadRequest)]
        public async Task<CredentialsCreateResponse> CreateAsync([FromBody] CredentialsCreateRequest request)
        {
            var credentials = await _customerCredentialsService.GetAsync(request.Login);

            if (credentials != null)
            {
                _log.Info("Credentials already exists", request.Login.SanitizeEmail());

                return new CredentialsCreateResponse {Error = CredentialsError.LoginAlreadyExists};
            }

            await _customerCredentialsService.CreateAsync(
                request.CustomerId,
                request.Login,
                request.Password);

            return new CredentialsCreateResponse();
        }

        /// <summary>
        /// Updates customer credentials.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <returns><see cref="CredentialsUpdateResponse"/></returns>
        [HttpPut]
        [ProducesResponseType(typeof(CredentialsUpdateResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.BadRequest)]
        public async Task<CredentialsUpdateResponse> ChangePasswordAsync([FromBody] CredentialsUpdateRequest request)
        {
            var credentials = await _customerCredentialsService.GetAsync(request.Login);

            if (credentials == null)
            {
                _log.Info("Customer credentials do not exist", request.Login.SanitizeEmail());

                return new CredentialsUpdateResponse {Error = CredentialsError.LoginNotFound};
            }

            await _customerCredentialsService.UpdatePasswordAsync(
                request.Login,
                request.Password);

            return new CredentialsUpdateResponse();
        }

        /// <summary>
        /// Updates customer login.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <returns><see cref="CredentialsUpdateResponse"/></returns>
        [HttpPut("login")]
        [ProducesResponseType(typeof(CredentialsUpdateResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<CredentialsUpdateResponse> UpdateLoginAsync([FromBody] LoginUpdateRequest request)
        {
            var credentials = await _customerCredentialsService.GetAsync(request.OldLogin);
            if (credentials == null)
            {
                _log.Info("Old email doesn't exist", request.OldLogin.SanitizeEmail());

                return new CredentialsUpdateResponse { Error = CredentialsError.LoginNotFound };
            }

            credentials = await _customerCredentialsService.GetAsync(request.NewLogin);
            if (credentials != null)
            {
                _log.Info("New email already exists", request.NewLogin.SanitizeEmail());

                return new CredentialsUpdateResponse { Error = CredentialsError.LoginAlreadyExists };
            }

            var updated = await _customerCredentialsService.UpdateLoginAsync(request.OldLogin, request.NewLogin);

            return updated
                ? new CredentialsUpdateResponse()
                : new CredentialsUpdateResponse { Error = CredentialsError.LoginNotFound };
        }

        /// <summary>
        /// Removes customer credentials.
        /// </summary>
        /// <param name="login">Login.</param>
        [HttpDelete("{login}")]
        [ProducesResponseType(typeof(void), (int) HttpStatusCode.OK)]
        public async Task RemoveAsync([Required] [FromRoute] string login)
        {
            await _customerCredentialsService.RemoveAsync(login);
        }

        /// <summary>
        /// Generates Password reset Identifier for the Customer
        /// </summary>
        /// <param name="customerId">Internal Customer Id.</param>
        /// /// <returns><see cref="PasswordResetResponseModel"/></returns>
        [HttpGet("resetIdentifier/{customerId}")]
        [ProducesResponseType(typeof(PasswordResetResponseModel), (int) HttpStatusCode.OK)]
        public async Task<PasswordResetResponseModel> GenerateResetIdentifierAsync(
            [Required] [FromRoute] string customerId)
        {
            var result = await _passwordResetService.CreateOrUpdateIdentifierAsync(customerId);
            return _mapper.Map<PasswordResetResponseModel>(result);
        }

        /// <summary>
        /// Resets the Password if the provided Identifier matches
        /// </summary>
        /// <param name="request" cref="PasswordResetRequest">Contains information about the Password Reset request</param>
        /// /// <returns><see cref="PasswordResetErrorResponse"/></returns>
        [HttpPost("password-reset")]
        [ProducesResponseType(typeof(PasswordResetErrorResponse), (int)HttpStatusCode.OK)]
        public async Task<PasswordResetErrorResponse> PasswordResetAsync([FromBody] PasswordResetRequest request)
        {
            var result = await _passwordResetService.PasswordResetAsync(request.CustomerEmail, request.ResetIdentifier,
                request.Password);

            return _mapper.Map<PasswordResetErrorResponse>(result);
        }

        /// <summary>
        /// Validates the provided identifier
        /// </summary>
        /// <param name="request" cref="ResetIdentifierValidationRequest">Contains information about the request</param>
        /// /// <returns><see cref="ResetIdentifierValidationResponse" /></returns>
        [HttpPost("reset-validation")]
        [ProducesResponseType(typeof(PasswordResetErrorResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<ResetIdentifierValidationResponse> ValidateIdentifierAsync([FromBody] ResetIdentifierValidationRequest request)
        {
            var result = await _passwordResetService.ValidateResetIdentifier(request.ResetIdentifier);

            return _mapper.Map<ResetIdentifierValidationResponse>(result);
        }
        
        /// <summary>
        /// Creates an Id for a Customer
        /// </summary>
        /// <param name="request" cref="GenerateClientIdRequest">Contains information about the request</param>
        /// <response code="200">string.</response>
        [HttpPost("clientId")]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.OK)]
        public Task<string> GenerateClientIdAsync([FromBody] GenerateClientIdRequest request)
        {
            return Task.FromResult(_credentialsGeneratorService.GenerateClientId(request.Length));
        }

        /// <summary>
        /// Generates a random Secret of a given length
        /// </summary>
        /// <param name="request" cref="GenerateClientSecretRequest">Contains information about the request</param>
        /// <response code="200">string.</response>
        [HttpPost("clientSecret")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        public Task<string> GenerateClientSecretAsync([FromBody] GenerateClientSecretRequest request)
        {
            return Task.FromResult(_credentialsGeneratorService.GeneratePassword(request.Length));
        }

        /// <summary>
        /// Set pin code for customer
        /// </summary>
        /// <param name="request" cref="SetPinRequest">Contains information about the request</param>
        /// /// <returns><see cref="SetPinResponse" /></returns>
        [HttpPost("pin")]
        [ProducesResponseType(typeof(SetPinResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<SetPinResponse> SetPinAsync([FromBody]SetPinRequest request)
        {
            var result = await _customerCredentialsService.SetPinAsync(request.CustomerId, request.PinCode);

            return new SetPinResponse { Error = (PinCodeErrorCodes)result};
        }

        /// <summary>
        /// Update pin code for customer
        /// </summary>
        /// <param name="request" cref="SetPinRequest">Contains information about the request</param>
        /// /// <returns><see cref="SetPinResponse" /></returns>
        [HttpPut("pin")]
        [ProducesResponseType(typeof(SetPinResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<SetPinResponse> UpdatePinAsync([FromBody]SetPinRequest request)
        {
            var result = await _customerCredentialsService.UpdatePinAsync(request.CustomerId, request.PinCode);

            return new SetPinResponse { Error = (PinCodeErrorCodes)result };
        }

        /// <summary>
        /// Validates customer pin code.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <returns><see cref="ValidatePinResponse"/></returns>
        [HttpPost("pin/validate")]
        [ProducesResponseType(typeof(ValidatePinResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<ValidatePinResponse> ValidatePinAsync([FromBody] ValidatePinRequest request)
        {
            var result = await _customerCredentialsService.ValidatePinAsync(request.CustomerId, request.PinCode);

            return new ValidatePinResponse { Error = (PinCodeErrorCodes)result };
        }

        /// <summary>
        /// Check if pin code is set for this customer
        /// </summary>
        /// <param name="customerId">Request.</param>
        /// <returns><see cref="HasPinResponse"/></returns>
        [HttpGet("pin/has-pin")]
        [ProducesResponseType(typeof(HasPinResponse), (int)HttpStatusCode.OK)]
        public async Task<HasPinResponse> HasPinAsync([FromQuery] string customerId)
        {
            var hasPin = await _customerCredentialsService.IsPinSetAsync(customerId);

            return new HasPinResponse{HasPin = hasPin};
        }
    }
}
