using System;
using System.Net;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using MAVN.Service.Credentials.Client;
using MAVN.Service.Credentials.Client.Api;
using MAVN.Service.Credentials.Client.Models.Requests;
using MAVN.Service.Credentials.Client.Models.Responses;
using MAVN.Service.Credentials.Domain.Exceptions;
using MAVN.Service.Credentials.Domain.Services;
using MAVN.Service.Credentials.Services;
using Microsoft.AspNetCore.Mvc;

namespace MAVN.Service.Credentials.Controllers
{
    [Route("api/partners")]
    [ModelStateValidationActionFilter]
    public class PartnersController : ControllerBase, IPartnersApi
    {
        private readonly IPartnerCredentialsService _partnerCredentialsService;
        private readonly ILog _log;

        public PartnersController(IPartnerCredentialsService partnerCredentialsService, ILogFactory logFactory)
        {
            _partnerCredentialsService = partnerCredentialsService;
            _log = logFactory.CreateLog(this);
        }

        /// <summary>
        /// Creates partner credentials.
        /// </summary>
        /// <param name="request">Partner credentials.</param>
        /// <returns>200 - The result of partner credentials creation.</returns>
        /// <remarks>
        /// Error codes:
        /// - **LoginAlreadyExists**
        /// </remarks>
        [HttpPost]
        [ProducesResponseType(typeof(CredentialsCreateResponse), (int) HttpStatusCode.OK)]
        public async Task<CredentialsCreateResponse> CreateAsync([FromBody] PartnerCredentialsCreateRequest request)
        {
            try
            {
                await _partnerCredentialsService.CreateAsync(request.ClientId, request.ClientSecret, request.PartnerId);
            }
            catch (PartnerCredentialsAlreadyExistsException)
            {
                _log.Info("Partner credentials already exists", new { request.ClientId });

                return new CredentialsCreateResponse {Error = CredentialsError.LoginAlreadyExists};
            }

            return new CredentialsCreateResponse {Error = CredentialsError.None};
        }

        /// <summary>
        /// Removes partners credentials.
        /// </summary>
        /// <param name="clientId">Partner client id.</param>
        [HttpDelete]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        public async Task RemoveAsync([FromQuery] string clientId)
        {
            await _partnerCredentialsService.RemoveAsync(clientId);
        }

        /// <summary>
        /// Updates the partner credentials.
        /// </summary>
        /// <param name="request">Partner credentials.</param>
        /// <returns>
        /// 200 - The result of partner credentials update changes.
        /// </returns>
        /// <remarks>
        /// Error codes:
        /// - **LoginNotFound**
        /// </remarks>
        [HttpPut]
        [ProducesResponseType(typeof(CredentialsUpdateResponse), (int) HttpStatusCode.OK)]
        public async Task<CredentialsUpdateResponse> UpdateAsync([FromBody] PartnerCredentialsUpdateRequest request)
        {
            try
            {
                await _partnerCredentialsService.UpdateAsync(request.ClientId, request.ClientSecret, request.PartnerId);
            }
            catch (PartnerCredentialsNotFoundException)
            {
                _log.Info("Partner credentials not found", new { request.ClientId });

                return new CredentialsUpdateResponse {Error = CredentialsError.LoginNotFound};
            }

            return new CredentialsUpdateResponse {Error = CredentialsError.None};
        }

        /// <summary>
        /// Validates partner credentials.
        /// </summary>
        /// <param name="request">Partner credentials.</param>
        /// <returns>
        /// 200 - The result of partner credentials validation.
        /// </returns>
        /// <remarks>
        /// Error codes:
        /// - **LoginNotFound**
        /// - **PasswordMismatch**
        /// </remarks>
        [HttpPost("validate")]
        [ProducesResponseType(typeof(PartnerCredentialsValidationResponse), (int) HttpStatusCode.OK)]
        public async Task<PartnerCredentialsValidationResponse> ValidateAsync(
            [FromBody] PartnerCredentialsValidationRequest request)
        {
            bool isValid;
            string partnerId;

            try
            {
                (isValid, partnerId) = await _partnerCredentialsService.ValidateAsync(request.ClientId, request.ClientSecret);
            }
            catch (PartnerCredentialsNotFoundException)
            {
                _log.Info("Partner credentials not found", new { request.ClientId });

                return new PartnerCredentialsValidationResponse {Error = CredentialsError.LoginNotFound};
            }

            return !isValid
                ? new PartnerCredentialsValidationResponse {Error = CredentialsError.PasswordMismatch}
                : new PartnerCredentialsValidationResponse {Error = CredentialsError.None, PartnerId = partnerId};
        }
    }
}
