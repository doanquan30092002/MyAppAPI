using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using MyApp.Application.CQRS.ForgotPassword.Commands;
using MyApp.Application.CQRS.ForgotPassword.Enums;
using MyApp.Application.CQRS.ForgotPassword.Service;
using MyApp.Application.Interfaces.IForgetPasswordRepository;
using NUnit.Framework;

namespace MyApp.Application.CQRS.ForgotPassword.Commands.Tests
{
    [TestFixture]
    public class ResetPasswordHandlerTests
    {
        private Mock<IOTPService> _otpServiceMock;
        private Mock<IForgetPasswordRepository> _forgetPasswordRepoMock;
        private ResetPasswordHandler _handler;

        [SetUp]
        public void Setup()
        {
            _otpServiceMock = new Mock<IOTPService>();
            _forgetPasswordRepoMock = new Mock<IForgetPasswordRepository>();
            _handler = new ResetPasswordHandler(
                _otpServiceMock.Object,
                _forgetPasswordRepoMock.Object
            );
        }

        [Test]
        public void Handle_ShouldThrowUnauthorizedAccessException_WhenResetGuidInvalid()
        {
            // Arrange
            var command = new ResetPasswordCommand
            {
                Contact = "user@example.com",
                ResetGuid = Guid.NewGuid().ToString(),
                NewPassword = "NewPass123!",
                Channel = OTPChannel.Email,
            };

            _otpServiceMock
                .Setup(x => x.VerifyResetGuid(command.Contact, command.ResetGuid))
                .Returns(false);

            // Act & Assert
            var ex = Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _handler.Handle(command, CancellationToken.None)
            );

            Assert.AreEqual("ResetGuid không hợp lệ hoặc đã hết hạn.", ex.Message);
        }

        [Test]
        public async Task Handle_ShouldUpdatePassword_WhenResetGuidValid()
        {
            // Arrange
            var command = new ResetPasswordCommand
            {
                Contact = "user@example.com",
                ResetGuid = Guid.NewGuid().ToString(),
                NewPassword = "NewPass123!",
                Channel = OTPChannel.Email,
            };

            _otpServiceMock
                .Setup(x => x.VerifyResetGuid(command.Contact, command.ResetGuid))
                .Returns(true);

            _forgetPasswordRepoMock
                .Setup(x =>
                    x.UpdatePasswordAsync(command.Contact, command.Channel, command.NewPassword)
                )
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result);
            _forgetPasswordRepoMock.Verify(
                x => x.UpdatePasswordAsync(command.Contact, command.Channel, command.NewPassword),
                Times.Once
            );
        }

        [Test]
        public async Task Handle_ShouldReturnFalse_WhenUpdatePasswordFails()
        {
            // Arrange
            var command = new ResetPasswordCommand
            {
                Contact = "user@example.com",
                ResetGuid = Guid.NewGuid().ToString(),
                NewPassword = "NewPass123!",
                Channel = OTPChannel.Email,
            };

            _otpServiceMock
                .Setup(x => x.VerifyResetGuid(command.Contact, command.ResetGuid))
                .Returns(true);

            _forgetPasswordRepoMock
                .Setup(x =>
                    x.UpdatePasswordAsync(command.Contact, command.Channel, command.NewPassword)
                )
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result);
        }
    }
}
