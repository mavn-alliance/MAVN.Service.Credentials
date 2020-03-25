using System.Threading.Tasks;
using Common.PasswordTools;
using Lykke.Logs;
using Lykke.Service.Credentials.Domain.Exceptions;
using Lykke.Service.Credentials.Domain.Models;
using Lykke.Service.Credentials.Domain.Repositories;
using Lykke.Service.Credentials.Domain.Services;
using Lykke.Service.Credentials.DomainServices;
using Moq;
using Xunit;

namespace Lykke.Service.Credentials.Tests
{
    public class PartnerCredentialsServiceTests
    {
        private const string ClientId = "ClientId";
        private const string ClientSecret = "ClientSecret";
        private const string PartnerId = "ClientPartnerId";
        private PartnerCredentials _partnerCredentials;

        private readonly IPartnerCredentialsService _partnerCredentialsService;

        private readonly Mock<IPartnerCredentialsRepository> _partnerCredentialsRepositoryMock =
            new Mock<IPartnerCredentialsRepository>();

        public PartnerCredentialsServiceTests()
        {
            _partnerCredentials = new PartnerCredentials {ClientId = ClientId, PartnerId = PartnerId};
            _partnerCredentials.SetPassword(ClientSecret);

            _partnerCredentialsRepositoryMock.Setup(x => x.GetByClientIdAsync(It.IsAny<string>()))
                .Returns((string clientId) =>Task.FromResult(_partnerCredentials));

            _partnerCredentialsRepositoryMock.Setup(x => x.GetByPartnerIdAsync(It.IsAny<string>()))
                .Returns((string partnerId) => Task.FromResult(_partnerCredentials));

            _partnerCredentialsService =
                new PartnerCredentialsService(_partnerCredentialsRepositoryMock.Object, EmptyLogFactory.Instance);
        }

        [Fact]
        public async Task
            When_Create_Async_Is_Executed_For_Non_Existing_Client_Then_New_Partner_Credentials_Are_Created()
        {
            _partnerCredentials = null;

            await _partnerCredentialsService.CreateAsync(ClientId, ClientSecret, PartnerId);

            _partnerCredentialsRepositoryMock.Verify(x => x.InsertAsync(It.IsAny<PartnerCredentials>()), Times.Once);
        }

        [Fact]
        public async Task
            When_Create_Async_Is_Executed_For_Existing_Client_Then_New_Partner_Credentials_Are_Not_Created()
        {
            await Assert.ThrowsAsync<PartnerCredentialsAlreadyExistsException>(async () =>
                await _partnerCredentialsService.CreateAsync(ClientId, ClientSecret, PartnerId));
        }

        [Fact]
        public async Task When_Remove_Async_Is_Executed_Then_Partner_Credentials_Are_Removed()
        {
            await _partnerCredentialsService.RemoveAsync(ClientId);

            _partnerCredentialsRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task
            When_Update_Async_Is_Executed_For_Existing_Partner_Credentials_Then_Partner_Credentials_Are_Updated()
        {
            await _partnerCredentialsService.UpdateAsync(ClientId, ClientSecret, PartnerId);

            _partnerCredentialsRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<PartnerCredentials>()), Times.Once);
        }

        [Fact]
        public async Task
            When_Update_Async_Is_Executed_For_Non_Existing_Partner_Credentials_Then_Error_Is_Thrown()
        {
            _partnerCredentials = null;

            await Assert.ThrowsAsync<PartnerCredentialsNotFoundException>(async () =>
                await _partnerCredentialsService.UpdateAsync(ClientId, ClientSecret, PartnerId));
        }

        [Fact]
        public async Task When_Validate_Async_Is_Executed_For_Valid_Client_Secret_Then_Validation_Will_Succeed()
        {
            var (isValid, partnerId) = await _partnerCredentialsService.ValidateAsync(ClientId, ClientSecret);

            Assert.True(isValid);
            Assert.NotNull(partnerId);
        }

        [Fact]
        public async Task When_Validate_Async_Is_Executed_For_Not_Valid_Client_Secret_Then_Validation_Will_Fail()
        {
            var (isValid, partnerId) = await _partnerCredentialsService.ValidateAsync(ClientId, "Non valid secret");

            Assert.False(isValid);
            Assert.NotNull(partnerId);
        }

        [Fact]
        public async Task When_Validate_Async_Is_Executed_For_Non_Existing_Partner_Credentials_Then_Error_Is_Thrown()
        {
            _partnerCredentials = null;

            await Assert.ThrowsAsync<PartnerCredentialsNotFoundException>(async () =>
                await _partnerCredentialsService.ValidateAsync(ClientId, "Non valid secret"));
        }
    }
}
