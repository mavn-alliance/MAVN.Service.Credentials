using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Lykke.Logs;
using Lykke.Logs.Loggers.LykkeConsole;
using MAVN.Service.Credentials.Client.Models.Requests;
using MAVN.Service.Credentials.Domain.Models;
using MAVN.Service.Credentials.Domain.Repositories;
using MAVN.Service.Credentials.Domain.Services;
using MAVN.Service.Credentials.DomainServices;
using MAVN.Service.Credentials.Models.Validation;
using Moq;
using Xunit;

namespace MAVN.Service.Credentials.Tests
{
    public class CustomerCredentialsValidationTests
    {
        [Fact]
        public async Task UserTriesToRegister_WithValidEmail_SuccessfullyCreated()
        {
            var customerCredentialsRepository = new Mock<ICustomerCredentialsRepository>();
            var settingsService = new Mock<ISettingsService>();

            customerCredentialsRepository
                .Setup(x => x.CreateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask)
                .Verifiable();
            CustomerCredentialsService customerCredentialsService;

            using (var logFactory = LogFactory.Create().AddUnbufferedConsole())
            {
                customerCredentialsService =
                    new CustomerCredentialsService(customerCredentialsRepository.Object, settingsService.Object, logFactory);
            }

            string customerId = "MockedId";
            string login = "validmail@mail.com";
            string password = "passwordMock";


            await customerCredentialsService.CreateAsync(customerId, login, password);

            customerCredentialsRepository.Verify();
        }

        #region Create

        [Fact]
        public void CreateRequestIsMade_CorrectCredentials_ValidationPassSuccessfully()
        {
            var validatorService = new PasswordValidator(CreateDefaultValidationRules());
            var request = new CredentialsCreateRequest
            {
                CustomerId = "mockId",
                Login = "validmail@mail.bg",
                Password = "passwordMock1@"
            };

            var validator = new CreateRequestCredentialsValidator(validatorService);
            var result = validator.Validate(request);

            Assert.True(result.IsValid);
        }

        [Fact]
        public void CreateRequestIsMade_InvalidEmail_ValidationShouldFail()
        {
            var validatorService = new PasswordValidator(CreateDefaultValidationRules());
            var request = new CredentialsCreateRequest
            {
                CustomerId = "mockId",
                Login = "invalid.bg",
                Password = "passwordMock"
            };

            var validator = new CreateRequestCredentialsValidator(validatorService);
            var result = validator.Validate(request);

            Assert.False(result.IsValid);
        }

        [Fact]
        public void CreateRequestIsMade_InvalidCustomerId_ValidationShouldFail()
        {
            var validatorService = new PasswordValidator(CreateDefaultValidationRules());
            var request = new CredentialsCreateRequest
            {
                CustomerId = "",
                Login = "valid@mail.bg",
                Password = "passwordMock"
            };

            var validator = new CreateRequestCredentialsValidator(validatorService);
            var result = validator.Validate(request);

            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData("short")]
        [InlineData("tolooooooooooooooooooooooooooooooooooooooooooooooooooong")]
        [InlineData("WithoutNumber@")]
        [InlineData("WithoutSpecialSymbol1")]
        [InlineData("withoutuppercase1@")]
        [InlineData("WITHOUTLOWERCASE1@")]
        [InlineData("WithNotAllowedSymbol1<")]
        [InlineData("WithNotAllowedWhitespace1@")]
        [InlineData(null)]
        public void CreateRequestIsMade_InvalidPassword_ValidationShouldFail(string inputPassword)
        {
            var validatorService = new PasswordValidator(CreateDefaultValidationRules());
            var request = new CredentialsCreateRequest
            {
                CustomerId = "customerId",
                Login = "valid@mail.bg",
                Password = inputPassword
            };

            var validator = new CreateRequestCredentialsValidator(validatorService);
            var result = validator.Validate(request);

            Assert.False(result.IsValid);
        }


        #endregion

        #region Update

        [Fact]
        public void UpdateRequestIsMade_CorrectCredentials_ValidationShouldFail()
        {
            var validatorService = new PasswordValidator(CreateDefaultValidationRules());
            var request = new CredentialsUpdateRequest
            {
                CustomerId = "mockId",
                Login = "validmail@mail.bg",
                Password = "passwordMock1@"
            };

            var validator = new UpdateRequestCredentialsValidator(validatorService);
            var result = validator.Validate(request);

            Assert.True(result.IsValid);
        }

        [Fact]
        public void UpdateRequestIsMade_InvalidEmail_ValidationShouldFail()
        {
            var validatorService = new PasswordValidator(CreateDefaultValidationRules());
            var request = new CredentialsUpdateRequest()
            {
                CustomerId = "mockId",
                Login = "invalid.bg",
                Password = "passwordMock"
            };

            var validator = new UpdateRequestCredentialsValidator(validatorService);
            var result = validator.Validate(request);

            Assert.False(result.IsValid);
        }

        [Fact]
        public void UpdateRequestIsMade_InvalidCustomerId_ValidationShouldFail()
        {
            var validatorService = new PasswordValidator(CreateDefaultValidationRules());
            var request = new CredentialsUpdateRequest()
            {
                CustomerId = "",
                Login = "valid@mail.bg",
                Password = "passwordMock"
            };

            var validator = new UpdateRequestCredentialsValidator(validatorService);
            var result = validator.Validate(request);

            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData("short")]
        [InlineData("tolooooooooooooooooooooooooooooooooooooooooooooooooooong")]
        [InlineData("WithoutNumber@")]
        [InlineData("WithoutSpecialSymbol1")]
        [InlineData("withoutuppercase1@")]
        [InlineData("WITHOUTLOWERCASE1@")]
        [InlineData("WithNotAllowedSymbol1<")]
        [InlineData("WithNotAllowedWhitespace1@")]
        [InlineData(null)]
        public void UpdateRequestIsMade_InvalidPassword_ValidationShouldFail(string inputPassword)
        {
            var validatorService = new PasswordValidator(CreateDefaultValidationRules());
            var request = new CredentialsUpdateRequest
            {
                CustomerId = "customerId",
                Login = "valid@mail.bg",
                Password = inputPassword
            };

            var validator = new UpdateRequestCredentialsValidator(validatorService);
            var result = validator.Validate(request);

            Assert.False(result.IsValid);
        }

        #endregion

        #region Validations

        [Fact]
        public void ValidationRequestIsMade_CorrectCredentials_ValidationShouldNotFail()
        {
            var validator = new ValidationRequestCredentialsValidator();

            validator.ShouldNotHaveValidationErrorFor(model => model.Login, "validmail@mail.com");
            validator.ShouldNotHaveValidationErrorFor(model => model.Password, "passwordMock");
        }

        [Fact]
        public void ValidationRequestIsMade_InvalidEmail_ValidationShouldFail()
        {
            var validator = new ValidationRequestCredentialsValidator();

            validator.ShouldHaveValidationErrorFor(model => model.Login, "invalid.mail");
        }
        #endregion

        [Theory]
        [InlineData("short")]
        [InlineData("tolooooooooooooooooooooooooooooooooooooooooooooooooooong")]
        [InlineData("WithoutNumber@")]
        [InlineData("WithoutSpecialSymbol1")]
        [InlineData("withoutuppercase1@")]
        [InlineData("WITHOUTLOWERCASE1@")]
        [InlineData("WithNotAllowedSymbol1<")]
        [InlineData("WithNotAllowedWhitespace1@")]
        [InlineData(null)]
        public void ResetRequestIsMade_InvalidPassword_ValidationShouldFail(string inputPassword)
        {
            var validatorService = new PasswordValidator(CreateDefaultValidationRules());
            var request = new PasswordResetRequest
            {
                Password = inputPassword
            };

            var validator = new PasswordResetRequestValidator(validatorService);
            var result = validator.Validate(request);

            Assert.False(result.IsValid);
        }

        private PasswordValidationRulesDto CreateDefaultValidationRules()
        {
            return new PasswordValidationRulesDto
            {
                AllowWhiteSpaces = false,
                MinLength = 6,
                MinUpperCase = 1,
                MinLowerCase = 1,
                MaxLength = 20,
                MinNumbers = 1,
                AllowedSpecialSymbols = "@#$%&",
                MinSpecialSymbols = 1
            };
        }
    }
}
