using System;
using System.Threading.Tasks;
using Common.PasswordTools;
using Lykke.Logs;
using Lykke.Service.Credentials.Domain.Exceptions;
using Lykke.Service.Credentials.Domain.Models;
using Lykke.Service.Credentials.Domain.Repositories;
using Lykke.Service.Credentials.Domain.Services;
using Moq;
using Xunit;

namespace Lykke.Service.Credentials.DomainServices.Tests
{
    public class AdminCredentialsServiceTests
    {
        private const string Login = "Login";
        private const string Password = "Password";
        private const string ResetPasswordIdentifier = "Reset Password Identifier";

        private readonly Mock<IAdminCredentialsRepository> _adminCredentialsRepositoryMock =
            new Mock<IAdminCredentialsRepository>();

        private readonly Mock<IPasswordResetRepository> _passwordResetRepositoryMock =
            new Mock<IPasswordResetRepository>();

        private readonly Mock<IResetIdentifierService> _resetIdentifierServiceMock =
            new Mock<IResetIdentifierService>();

        private readonly IAdminCredentialsService _service;

        private AdminCredentials _adminCredentials;

        private ResetIdentifier _resetIdentifier;

        public AdminCredentialsServiceTests()
        {
            _adminCredentials = new AdminCredentials {AdminId = Guid.NewGuid().ToString(), Login = Login};
            _adminCredentials.SetPassword(Password);

            var date = DateTime.UtcNow;
            _resetIdentifier = new ResetIdentifier("AdminId", ResetPasswordIdentifier, date, date.AddMinutes(1));

            _adminCredentialsRepositoryMock.Setup(o => o.GetByLoginAsync(It.IsAny<string>()))
                .Returns((string login) => Task.FromResult(_adminCredentials));

            _passwordResetRepositoryMock.Setup(o => o.GetByIdentifierAsync(It.IsAny<string>()))
                .Returns((string resetIdentifier) => Task.FromResult<IResetIdentifier>(_resetIdentifier));

            _service = new AdminCredentialsService(
                _adminCredentialsRepositoryMock.Object,
                _passwordResetRepositoryMock.Object,
                _resetIdentifierServiceMock.Object,
                EmptyLogFactory.Instance);
        }

        [Fact]
        public async Task Insert_New_Admin_Credentials_If_Not_Exists()
        {
            // arrange

            _adminCredentials = null;

            // act

            await _service.CreateAsync(Guid.NewGuid().ToString(), Login, Password);

            // assert

            _adminCredentialsRepositoryMock.Verify(o => o.InsertAsync(It.IsAny<AdminCredentials>()), Times.Once);
        }

        [Fact]
        public async Task Do_Not_Create_Admin_Credentials_If_Already_Exists()
        {
            // act

            var task = _service.CreateAsync(Guid.NewGuid().ToString(), Login, Password);

            // assert

            await Assert.ThrowsAsync<AdminCredentialsAlreadyExistsException>(async () => await task);
        }

        [Fact]
        public async Task Validation_Returns_True_If_Password_Equal()
        {
            // act

            var result = await _service.ValidateAsync(Login, Password);

            // assert

            Assert.True(result.isValid);
        }

        [Fact]
        public async Task Validation_Returns_False_If_Password_Not_Equal()
        {
            // act

            var result = await _service.ValidateAsync(Login, "Wrong Password");

            // assert

            Assert.False(result.isValid);
        }

        [Fact]
        public async Task Validation_Throws_Error_If_Admin_Credentials_Does_Not_Exist()
        {
            // arrange

            _adminCredentials = null;

            // act

            var task = _service.ValidateAsync(Login, Password);

            // assert

            await Assert.ThrowsAsync<AdminCredentialsNotFoundException>(async () => await task);
        }

        [Fact]
        public async Task Generate_Reset_Identifier_If_The_Number_Of_Attempts_Is_Not_Exceeded()
        {
            // arrange

            var expectedResult = (identifier: ResetPasswordIdentifier, TimeSpan.Zero);

            _resetIdentifierServiceMock.Setup(o => o.GenerateAsync(It.IsAny<string>()))
                .Returns((string key) => Task.FromResult(expectedResult));

            // act

            var actualResult = await _service.GenerateResetIdentifierAsync(Guid.NewGuid().ToString());

            // assert

            Assert.Equal(expectedResult.identifier, actualResult);
        }

        [Fact]
        public async Task Generate_Reset_Identifier_Throws_Error_Number_Of_Attempts_Are_Exceeded()
        {
            // arrange

            _resetIdentifierServiceMock.Setup(o => o.GenerateAsync(It.IsAny<string>()))
                .Throws<IdentifierRequestsExceededException>();

            // act

            var task = _service.GenerateResetIdentifierAsync(Guid.NewGuid().ToString());

            // assert

            await Assert.ThrowsAsync<IdentifierRequestsExceededException>(async () => await task);
        }

        [Fact]
        public async Task Validate_Reset_Identifier_Returns_True_If_Not_Expired()
        {
            // act

            var actualResult = await _service.ValidateResetIdentifierAsync(Guid.NewGuid().ToString());

            // assert

            Assert.True(actualResult);
        }

        [Fact]
        public async Task Validate_Reset_Identifier_Returns_False_If_Expired()
        {
            // arrange

            _resetIdentifier.ExpiresAt = DateTime.UtcNow.AddMinutes(-1);

            // act

            var actualResult = await _service.ValidateResetIdentifierAsync(Guid.NewGuid().ToString());

            // assert

            Assert.False(actualResult);
        }

        [Fact]
        public async Task Validate_Reset_Identifier_Throws_Error_If_Does_Not_Exist()
        {
            // arrange

            _resetIdentifier = null;

            // act

            var task = _service.ValidateResetIdentifierAsync(Guid.NewGuid().ToString());

            // assert

            await Assert.ThrowsAsync<IdentifierDoesNotExistException>(async () => await task);
        }

        [Fact]
        public async Task Reset_Password_Updates_Admin_Credentials()
        {
            // act

            await _service.ResetPasswordAsync(Login, Password, ResetPasswordIdentifier);

            // assert

            _adminCredentialsRepositoryMock.Verify(o => o.UpdateAsync(It.IsAny<AdminCredentials>()), Times.Once);
        }

        [Fact]
        public async Task Reset_Password_Resets_Number_Of_Attempts()
        {
            // act

            await _service.ResetPasswordAsync(Login, Password, ResetPasswordIdentifier);

            // assert

            _resetIdentifierServiceMock.Verify(o => o.ClearAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task Reset_Password_Removes_Identifier()
        {
            // act

            await _service.ResetPasswordAsync(Login, Password, ResetPasswordIdentifier);

            // assert

            _passwordResetRepositoryMock.Verify(o => o.RemoveAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task Reset_Password_Throws_Error_If_Admin_Credentials_Does_Not_Exist()
        {
            // arrange

            _adminCredentials = null;

            // act

            var task = _service.ResetPasswordAsync(Login, Password, ResetPasswordIdentifier);

            // assert

            await Assert.ThrowsAsync<AdminCredentialsNotFoundException>(async () => await task);
        }

        [Fact]
        public async Task Reset_Password_Throws_Error_If_Reset_Password_Identifier_Does_Not_Exist()
        {
            // arrange

            _resetIdentifier = null;

            // act

            var task = _service.ResetPasswordAsync(Login, Password, ResetPasswordIdentifier);

            // assert

            await Assert.ThrowsAsync<IdentifierDoesNotExistException>(async () => await task);
        }

        [Fact]
        public async Task Reset_Password_Throws_Error_If_Reset_Password_Identifier_Expired()
        {
            // arrange

            _resetIdentifier.ExpiresAt = DateTime.UtcNow.AddMinutes(-1);

            // act

            var task = _service.ResetPasswordAsync(Login, Password, ResetPasswordIdentifier);

            // assert

            await Assert.ThrowsAsync<IdentifierHasExpiredException>(async () => await task);
        }

        [Fact]
        public async Task Reset_Password_Throws_Error_If_Reset_Password_Identifier_Mismatch()
        {
            // act

            var task = _service.ResetPasswordAsync(Login, Password, "Wrong Identifier");

            // assert

            await Assert.ThrowsAsync<IdentifierMismatchException>(async () => await task);
        }

        [Fact]
        public async Task Change_Password_Updates_Admin_Credentials()
        {
            // act

            await _service.ChangePasswordAsync(Login, Password);

            // assert

            _adminCredentialsRepositoryMock.Verify(o => o.UpdateAsync(It.IsAny<AdminCredentials>()), Times.Once);
        }

        [Fact]
        public async Task Change_Password_Throws_Error_If_Admin_Credentials_Does_Not_Exist()
        {
            // arrange

            _adminCredentials = null;

            // act

            var task = _service.ChangePasswordAsync(Login, Password);

            // assert

            await Assert.ThrowsAsync<AdminCredentialsNotFoundException>(async () => await task);
        }
    }
}
