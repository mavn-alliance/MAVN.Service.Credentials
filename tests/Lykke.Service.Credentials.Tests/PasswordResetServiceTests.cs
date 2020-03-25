using System;
using System.Threading.Tasks;
using Lykke.Logs;
using Lykke.Logs.Loggers.LykkeConsole;
using Lykke.Service.Credentials.MsSqlRepositories.Entities;
using Lykke.Service.Credentials.Domain.Enums;
using Lykke.Service.Credentials.Domain.Helpers;
using Lykke.Service.Credentials.Domain.Models;
using Lykke.Service.Credentials.Domain.Repositories;
using Lykke.Service.Credentials.Domain.Services;
using Lykke.Service.Credentials.DomainServices;
using Moq;
using StackExchange.Redis;
using Xunit;

namespace Lykke.Service.Credentials.Tests
{
    public class PasswordResetServiceTests
    {
        private const string IdentifierMock = "IdentifierMockXXXXXX";
        private readonly int _maxAllowedRequestsNumber = 2;
        private readonly string _instanceName = "TestInstanceName";
        private readonly TimeSpan _monitoredPeriod = new TimeSpan(0, 0, 5, 0);
        private readonly TimeSpan _identifierTimeSpan = new TimeSpan(1, 0, 0, 0);
        private readonly int _identifierLength = 18;
        private readonly IConnectionMultiplexer _mockMultiplexer;

        public PasswordResetServiceTests()
        {
            var mockMultiplexer = new Mock<IConnectionMultiplexer>();

            mockMultiplexer.Setup(_ => _.IsConnected).Returns(false);

            var mockDatabase = new Mock<IDatabase>();

            mockMultiplexer
                .Setup(_ => _.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                .Returns(mockDatabase.Object);

            _mockMultiplexer = mockMultiplexer.Object;
        }

        [Fact]
        public async Task FirstRequestForPasswordResetIdentifier_EverythingValid_SuccessfullyGenerated()
        {
            var base34Util = new Mock<IBase34Util>();
            var customerCredentialsService = new Mock<ICustomerCredentialsService>();

            base34Util.Setup(x => x.GenerateBase(It.IsAny<string>()))
                .Returns(IdentifierMock);

            var passwordResetRepository = new Mock<IPasswordResetRepository>();

            passwordResetRepository
                .Setup(x => x.CreateOrUpdateIdentifierAsync(It.IsAny<string>(),
                    IdentifierMock.Substring(0, _identifierLength),
                    It.IsAny<TimeSpan>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            PasswordResetService passwordResetService;
            using (var logFactory = LogFactory.Create().AddUnbufferedConsole())
            {
                passwordResetService = new PasswordResetService(_mockMultiplexer, _instanceName,
                    _maxAllowedRequestsNumber,
                    _monitoredPeriod, _identifierTimeSpan, _identifierLength, passwordResetRepository.Object,
                     customerCredentialsService.Object, base34Util.Object, logFactory);
            }

            string expectedCustomerId = "NewCustomerTestId";

            var actual = await passwordResetService.CreateOrUpdateIdentifierAsync(expectedCustomerId);

            passwordResetRepository.Verify();

            Assert.Equal(PasswordResetErrorCodes.None, actual.ErrorCode);
            Assert.Equal(IdentifierMock.Substring(0, _identifierLength), actual.Identifier);
        }


        [Fact]
        public async Task SecondRequestForPasswordResetIdentifier_EverythingValid_NewCodeIsGenerated()
        {
            var base34Util = new Mock<IBase34Util>();
            var customerCredentialsService = new Mock<ICustomerCredentialsService>();

            base34Util.Setup(x => x.GenerateBase(It.IsAny<string>()))
                .Returns(IdentifierMock);

            var passwordResetRepository = new Mock<IPasswordResetRepository>();

            passwordResetRepository
                .Setup(x => x.CreateOrUpdateIdentifierAsync(It.IsAny<string>(),
                    IdentifierMock.Substring(0, _identifierLength),
                    It.IsAny<TimeSpan>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            PasswordResetService passwordResetService;
            using (var logFactory = LogFactory.Create().AddUnbufferedConsole())
            {
                passwordResetService = new PasswordResetService(_mockMultiplexer, _instanceName,
                    _maxAllowedRequestsNumber,
                    _monitoredPeriod, _identifierTimeSpan, _identifierLength, passwordResetRepository.Object,
                    customerCredentialsService.Object, base34Util.Object, logFactory);
            }

            string expectedCustomerId = "NewCustomerTestId";

            var actual = await passwordResetService.CreateOrUpdateIdentifierAsync(expectedCustomerId);

            passwordResetRepository.Verify(
                e => e.CreateOrUpdateIdentifierAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()),
                Times.Exactly(1));

            Assert.Equal(PasswordResetErrorCodes.None, actual.ErrorCode);
            Assert.Equal(IdentifierMock.Substring(0, _identifierLength), actual.Identifier);

            var actual2 = await passwordResetService.CreateOrUpdateIdentifierAsync(expectedCustomerId);

            passwordResetRepository.Verify(
                e => e.CreateOrUpdateIdentifierAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()),
                Times.Exactly(2));

            Assert.Equal(PasswordResetErrorCodes.None, actual2.ErrorCode);
            Assert.Equal(IdentifierMock.Substring(0, _identifierLength), actual2.Identifier);
        }

        [Fact]
        public async Task SendPasswordResetRequest_EverythingIsValid_PasswordIsChanged()
        {
            var customerEmailMock = "Test@email.here";

            var base34Util = new Mock<IBase34Util>();
            var customerCredentialsService = new Mock<ICustomerCredentialsService>();

            customerCredentialsService
                .Setup(x => x.GetAsync(It.IsAny<string>()))
                .Returns(Task.FromResult((IHashedCustomerCredentials) new CustomerCredentialsEntity
                    {Login = customerEmailMock}));

            customerCredentialsService
                .Setup(x => x.UpdatePasswordAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var passwordResetRepository = new Mock<IPasswordResetRepository>();

            passwordResetRepository
                .Setup(x => x.GetIdentifierAsync(It.IsAny<string>()))
                .Returns(Task.FromResult((IResetIdentifier) new PasswordResetEntity {Identifier = IdentifierMock, ExpiresAt = DateTime.Now.AddDays(1)}))
                .Verifiable();

            PasswordResetService passwordResetService;
            using (var logFactory = LogFactory.Create().AddUnbufferedConsole())
            {
                passwordResetService = new PasswordResetService(_mockMultiplexer, _instanceName,
                    _maxAllowedRequestsNumber,
                    _monitoredPeriod, _identifierTimeSpan, _identifierLength, passwordResetRepository.Object,
                    customerCredentialsService.Object, base34Util.Object, logFactory);
            }

            var actual =
                await passwordResetService.PasswordResetAsync(customerEmailMock, IdentifierMock, "newPassword");

            passwordResetRepository.Verify();

            Assert.Equal(PasswordResetErrorCodes.None, actual);
        }

        [Fact]
        public async Task SendPasswordResetRequest_IdentifierHasExpired_ErrorIsReturned()
        {
            var customerEmailMock = "Test@email.here";

            var base34Util = new Mock<IBase34Util>();
            var customerCredentialsService = new Mock<ICustomerCredentialsService>();

            customerCredentialsService
                .Setup(x => x.GetAsync(It.IsAny<string>()))
                .Returns(Task.FromResult((IHashedCustomerCredentials)new CustomerCredentialsEntity
                { Login = customerEmailMock }));

            customerCredentialsService
                .Setup(x => x.UpdatePasswordAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var passwordResetRepository = new Mock<IPasswordResetRepository>();
            var expectedIdentifier = "MockedIdentifier";
            passwordResetRepository
                .Setup(x => x.GetIdentifierAsync(It.IsAny<string>()))
                .Returns(Task.FromResult((IResetIdentifier)new PasswordResetEntity { Identifier = expectedIdentifier, ExpiresAt = DateTime.Now.AddDays(-1) }))
                .Verifiable();

            PasswordResetService passwordResetService;
            using (var logFactory = LogFactory.Create().AddUnbufferedConsole())
            {
                passwordResetService = new PasswordResetService(_mockMultiplexer, _instanceName,
                    _maxAllowedRequestsNumber,
                    _monitoredPeriod, _identifierTimeSpan, _identifierLength, passwordResetRepository.Object,
                    customerCredentialsService.Object, base34Util.Object, logFactory);
            }

            var actual =
                await passwordResetService.PasswordResetAsync(customerEmailMock, IdentifierMock, "newPassword");

            passwordResetRepository.Verify();

            Assert.Equal(PasswordResetErrorCodes.ProvidedIdentifierHasExpired, actual);
        }


        [Fact]
        public async Task SendPasswordResetRequest_IdentifiersDoesNotMatch_ErrorIsReturned()
        {
            var customerEmailMock = "Test@email.here";

            var base34Util = new Mock<IBase34Util>();
            var customerCredentialsService = new Mock<ICustomerCredentialsService>();

            customerCredentialsService
                .Setup(x => x.GetAsync(It.IsAny<string>()))
                .Returns(Task.FromResult((IHashedCustomerCredentials)new CustomerCredentialsEntity
                { Login = customerEmailMock }));

            customerCredentialsService
                .Setup(x => x.UpdatePasswordAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var passwordResetRepository = new Mock<IPasswordResetRepository>();
            var expectedIdentifier = "MockedIdentifier";
            passwordResetRepository
                .Setup(x => x.GetIdentifierAsync(It.IsAny<string>()))
                .Returns(Task.FromResult((IResetIdentifier)new PasswordResetEntity { Identifier = expectedIdentifier, ExpiresAt = DateTime.Now.AddDays(1) }))
                .Verifiable();

            PasswordResetService passwordResetService;
            using (var logFactory = LogFactory.Create().AddUnbufferedConsole())
            {
                passwordResetService = new PasswordResetService(_mockMultiplexer, _instanceName,
                    _maxAllowedRequestsNumber,
                    _monitoredPeriod, _identifierTimeSpan, _identifierLength, passwordResetRepository.Object,
                    customerCredentialsService.Object, base34Util.Object, logFactory);
            }

            var actual =
                await passwordResetService.PasswordResetAsync(customerEmailMock, IdentifierMock, "newPassword");

            passwordResetRepository.Verify();

            Assert.Equal(PasswordResetErrorCodes.IdentifierMismatch, actual);
        }
        [Fact]
        public async Task SendPasswordResetRequest_CustomerHasNoIdentifier_ErrorIsReturned()
        {
            var customerEmailMock = "Test@email.here";

            var base34Util = new Mock<IBase34Util>();
            var customerCredentialsService = new Mock<ICustomerCredentialsService>();

            customerCredentialsService
                .Setup(x => x.GetAsync(It.IsAny<string>()))
                .Returns(Task.FromResult((IHashedCustomerCredentials)new CustomerCredentialsEntity
                { Login = customerEmailMock }));

            customerCredentialsService
                .Setup(x => x.UpdatePasswordAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var passwordResetRepository = new Mock<IPasswordResetRepository>();
         ;
            passwordResetRepository
                .Setup(x => x.GetIdentifierAsync(It.IsAny<string>()))
                .Returns(Task.FromResult((IResetIdentifier)null))
                .Verifiable();

            PasswordResetService passwordResetService;
            using (var logFactory = LogFactory.Create().AddUnbufferedConsole())
            {
                passwordResetService = new PasswordResetService(_mockMultiplexer, _instanceName,
                    _maxAllowedRequestsNumber,
                    _monitoredPeriod, _identifierTimeSpan, _identifierLength, passwordResetRepository.Object,
                    customerCredentialsService.Object, base34Util.Object, logFactory);
            }

            var actual =
                await passwordResetService.PasswordResetAsync(customerEmailMock, IdentifierMock, "newPassword");

            passwordResetRepository.Verify();

            Assert.Equal(PasswordResetErrorCodes.ThereIsNoIdentifierForThisCustomer, actual);
        }

        [Fact]
        public async Task SendPasswordResetRequest_CustomerDoesNotExist_ErrorIsReturned()
        {
            var customerEmailMock = "Test@email.here";

            var base34Util = new Mock<IBase34Util>();
            var customerCredentialsService = new Mock<ICustomerCredentialsService>();

            customerCredentialsService
                .Setup(x => x.GetAsync(It.IsAny<string>()))
                .Returns(Task.FromResult((IHashedCustomerCredentials)null)); 

            var passwordResetRepository = new Mock<IPasswordResetRepository>();


            PasswordResetService passwordResetService;
            using (var logFactory = LogFactory.Create().AddUnbufferedConsole())
            {
                passwordResetService = new PasswordResetService(_mockMultiplexer, _instanceName,
                    _maxAllowedRequestsNumber,
                    _monitoredPeriod, _identifierTimeSpan, _identifierLength, passwordResetRepository.Object,
                    customerCredentialsService.Object, base34Util.Object, logFactory);
            }

            var actual =
                await passwordResetService.PasswordResetAsync(customerEmailMock, IdentifierMock, "newPassword");

          

            Assert.Equal(PasswordResetErrorCodes.CustomerDoesNotExist, actual);
        }

        [Fact]
        public async Task ValidateIdentifierRequest_IdentifierDoesNotExist_ErrorIsReturned()
        {
            var base34Util = new Mock<IBase34Util>();
            var customerCredentialsService = new Mock<ICustomerCredentialsService>();

            var passwordResetRepository = new Mock<IPasswordResetRepository>();
            passwordResetRepository.Setup(x => x.GetByIdentifierAsync(IdentifierMock))
                .ReturnsAsync((IResetIdentifier) null);


            PasswordResetService passwordResetService;
            using (var logFactory = LogFactory.Create().AddUnbufferedConsole())
            {
                passwordResetService = new PasswordResetService(_mockMultiplexer, _instanceName,
                    _maxAllowedRequestsNumber,
                    _monitoredPeriod, _identifierTimeSpan, _identifierLength, passwordResetRepository.Object,
                    customerCredentialsService.Object, base34Util.Object, logFactory);
            }

            var actual = await passwordResetService.ValidateResetIdentifier(IdentifierMock);

            Assert.Equal(ValidateIdentifierErrorCodes.IdentifierDoesNotExist, actual);
        }

        [Fact]
        public async Task ValidateIdentifierRequest_IdentifierHasExpired_ErrorIsReturned()
        {
            var base34Util = new Mock<IBase34Util>();
            var customerCredentialsService = new Mock<ICustomerCredentialsService>();

            var passwordResetRepository = new Mock<IPasswordResetRepository>();
            passwordResetRepository.Setup(x => x.GetByIdentifierAsync(IdentifierMock))
                .ReturnsAsync(new PasswordResetEntity{ExpiresAt = DateTime.UtcNow.AddYears(-1)});


            PasswordResetService passwordResetService;
            using (var logFactory = LogFactory.Create().AddUnbufferedConsole())
            {
                passwordResetService = new PasswordResetService(_mockMultiplexer, _instanceName,
                    _maxAllowedRequestsNumber,
                    _monitoredPeriod, _identifierTimeSpan, _identifierLength, passwordResetRepository.Object,
                    customerCredentialsService.Object, base34Util.Object, logFactory);
            }

            var actual = await passwordResetService.ValidateResetIdentifier(IdentifierMock);

            Assert.Equal(ValidateIdentifierErrorCodes.ProvidedIdentifierHasExpired, actual);
        }

        [Fact]
        public async Task ValidateIdentifierRequest_IdentifierIsValid_NoErrorIsReturned()
        {
            var base34Util = new Mock<IBase34Util>();
            var customerCredentialsService = new Mock<ICustomerCredentialsService>();

            var passwordResetRepository = new Mock<IPasswordResetRepository>();
            passwordResetRepository.Setup(x => x.GetByIdentifierAsync(IdentifierMock))
                .ReturnsAsync(new PasswordResetEntity { ExpiresAt = DateTime.UtcNow.AddYears(1) });


            PasswordResetService passwordResetService;
            using (var logFactory = LogFactory.Create().AddUnbufferedConsole())
            {
                passwordResetService = new PasswordResetService(_mockMultiplexer, _instanceName,
                    _maxAllowedRequestsNumber,
                    _monitoredPeriod, _identifierTimeSpan, _identifierLength, passwordResetRepository.Object,
                    customerCredentialsService.Object, base34Util.Object, logFactory);
            }

            var actual = await passwordResetService.ValidateResetIdentifier(IdentifierMock);

            Assert.Equal(ValidateIdentifierErrorCodes.None, actual);
        }
    }
}
