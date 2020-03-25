using System.Threading.Tasks;
using Common.PasswordTools;
using Lykke.Logs;
using Lykke.Service.Credentials.Domain.Enums;
using Lykke.Service.Credentials.Domain.Models;
using Lykke.Service.Credentials.Domain.Repositories;
using Lykke.Service.Credentials.Domain.Services;
using Lykke.Service.Credentials.DomainServices;
using Lykke.Service.Credentials.MsSqlRepositories.Entities;
using Moq;
using Xunit;

namespace Lykke.Service.Credentials.Tests
{
    public class CustomerCredentialsServiceTests
    {
        private const string FakeCustomerId = "CustomerId";
        private const string ValidPIN = "1234";
        private const string InvalidPin = "1234a";
        private const int PinLength = 4;

        private readonly Mock<ICustomerCredentialsRepository> _customerCredentialsRepoMock = new Mock<ICustomerCredentialsRepository>();
        private readonly Mock<ISettingsService> _settingsServiceMock = new Mock<ISettingsService>();

        [Theory]
        [InlineData("")]
        [InlineData("123")]
        [InlineData("123a")]
        [InlineData("12345")]
        [InlineData("1234 ")]
        public async Task SetPinAsync_PinCodeIsNotValid_InvalidPinErrorReturned(string pinCode)
        {
            _settingsServiceMock.Setup(x => x.PinCodeLength)
                .Returns(4);

            var sut = CreateSutInstance();

            var result = await sut.SetPinAsync(FakeCustomerId, pinCode);

            Assert.Equal(PinCodeErrorCodes.InvalidPin, result);
        }

        [Fact]
        public async Task SetPinAsync_NoCredentialsForThisCustomerId_CustomerDoesNotExistErrorReturned()
        {
            _settingsServiceMock.Setup(x => x.PinCodeLength)
                .Returns(4);

            _customerCredentialsRepoMock.Setup(x => x.GetByCustomerIdAsync(FakeCustomerId))
                .ReturnsAsync((IHashedCustomerCredentials) null);

            var sut = CreateSutInstance();

            var result = await sut.SetPinAsync(FakeCustomerId, ValidPIN);

            Assert.Equal(PinCodeErrorCodes.CustomerDoesNotExist, result);
        }

        [Fact]
        public async Task SetPinAsync_PinAlreadySet_PinAlreadySetErrorReturned()
        {
            _settingsServiceMock.Setup(x => x.PinCodeLength)
                .Returns(4);

            _customerCredentialsRepoMock.Setup(x => x.GetByCustomerIdAsync(FakeCustomerId))
                .ReturnsAsync(new CustomerCredentialsEntity());

            _customerCredentialsRepoMock.Setup(x => x.GetPinByCustomerIdAsync(FakeCustomerId))
                .ReturnsAsync(new PinCodeEntity());

            var sut = CreateSutInstance();

            var result = await sut.SetPinAsync(FakeCustomerId, ValidPIN);

            Assert.Equal(PinCodeErrorCodes.PinAlreadySet, result);
        }

        [Fact]
        public async Task SetPinAsync_PinSetSuccessfully()
        {
            _settingsServiceMock.Setup(x => x.PinCodeLength)
                .Returns(4);

            _customerCredentialsRepoMock.Setup(x => x.GetByCustomerIdAsync(FakeCustomerId))
                .ReturnsAsync(new CustomerCredentialsEntity());

            _customerCredentialsRepoMock.Setup(x => x.GetPinByCustomerIdAsync(FakeCustomerId))
                .ReturnsAsync((PinCodeEntity)null);

            var sut = CreateSutInstance();

            var result = await sut.SetPinAsync(FakeCustomerId, ValidPIN);

            Assert.Equal(PinCodeErrorCodes.None, result);
        }

        [Fact]
        public async Task ValidatePinAsync_CustomerHasNoPin_PinIsNotSetErrorReturned()
        {
            _customerCredentialsRepoMock.Setup(x => x.GetPinByCustomerIdAsync(FakeCustomerId))
                .ReturnsAsync((PinCodeEntity)null);

            var sut = CreateSutInstance();

            var result = await sut.ValidatePinAsync(FakeCustomerId, ValidPIN);

            Assert.Equal(PinCodeErrorCodes.PinIsNotSet, result);
        }

        [Fact]
        public async Task ValidatePinAsync_PinDoesNotMatch_PinCodeMismatchErrorReturned()
        {
            var response = new PinCodeEntity();
            response.SetPassword(ValidPIN);
            _customerCredentialsRepoMock.Setup(x => x.GetPinByCustomerIdAsync(FakeCustomerId))
                .ReturnsAsync(response);

            var sut = CreateSutInstance();

            var result = await sut.ValidatePinAsync(FakeCustomerId, InvalidPin);

            Assert.Equal(PinCodeErrorCodes.PinCodeMismatch, result);
        }

        [Fact]
        public async Task ValidatePinAsync_PinIsCorrect_NoErrors()
        {
            var response = new PinCodeEntity();
            response.SetPassword(ValidPIN);
            _customerCredentialsRepoMock.Setup(x => x.GetPinByCustomerIdAsync(FakeCustomerId))
                .ReturnsAsync(response);

            var sut = CreateSutInstance();

            var result = await sut.ValidatePinAsync(FakeCustomerId, ValidPIN);

            Assert.Equal(PinCodeErrorCodes.None, result);
        }

        [Theory]
        [InlineData("")]
        [InlineData("123")]
        [InlineData("123a")]
        [InlineData("12345")]
        [InlineData("1234 ")]
        public async Task UpdatePinAsync_PinCodeIsNotValid_InvalidPinErrorReturned(string pinCode)
        {
            _settingsServiceMock.Setup(x => x.PinCodeLength)
                .Returns(4);

            var sut = CreateSutInstance();

            var result = await sut.UpdatePinAsync(FakeCustomerId, pinCode);

            Assert.Equal(PinCodeErrorCodes.InvalidPin, result);
        }

        [Fact]
        public async Task UpdatePinAsync_NoCredentialsForThisCustomerId_CustomerDoesNotExistErrorReturned()
        {
            _settingsServiceMock.Setup(x => x.PinCodeLength)
                .Returns(4);

            _customerCredentialsRepoMock.Setup(x => x.GetByCustomerIdAsync(FakeCustomerId))
                .ReturnsAsync((IHashedCustomerCredentials)null);

            var sut = CreateSutInstance();

            var result = await sut.UpdatePinAsync(FakeCustomerId, ValidPIN);

            Assert.Equal(PinCodeErrorCodes.CustomerDoesNotExist, result);
        }

        [Fact]
        public async Task UpdatePinAsync_PinIsNotSet_PinIsNotSetErrorReturned()
        {
            _settingsServiceMock.Setup(x => x.PinCodeLength)
                .Returns(4);

            _customerCredentialsRepoMock.Setup(x => x.GetByCustomerIdAsync(FakeCustomerId))
                .ReturnsAsync(new CustomerCredentialsEntity());

            _customerCredentialsRepoMock.Setup(x => x.GetPinByCustomerIdAsync(FakeCustomerId))
                .ReturnsAsync((PinCodeEntity)null);

            var sut = CreateSutInstance();

            var result = await sut.UpdatePinAsync(FakeCustomerId, ValidPIN);

            Assert.Equal(PinCodeErrorCodes.PinIsNotSet, result);
        }

        [Fact]
        public async Task UpdatePinAsync_PinSetSuccessfully()
        {
            _settingsServiceMock.Setup(x => x.PinCodeLength)
                .Returns(4);

            _customerCredentialsRepoMock.Setup(x => x.GetByCustomerIdAsync(FakeCustomerId))
                .ReturnsAsync(new CustomerCredentialsEntity());

            _customerCredentialsRepoMock.Setup(x => x.GetPinByCustomerIdAsync(FakeCustomerId))
                .ReturnsAsync(new PinCodeEntity());

            var sut = CreateSutInstance();

            var result = await sut.UpdatePinAsync(FakeCustomerId, ValidPIN);

            Assert.Equal(PinCodeErrorCodes.None, result);
        }

        private ICustomerCredentialsService CreateSutInstance()
        {
            return new CustomerCredentialsService(_customerCredentialsRepoMock.Object, _settingsServiceMock.Object, EmptyLogFactory.Instance);
        }
    }
}
