using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using MyApp.Application.CQRS.ForgotPassword.Commands;
using MyApp.Application.CQRS.ForgotPassword.Enums;
using MyApp.Application.CQRS.ForgotPassword.Service;
using NUnit.Framework;

namespace MyApp.Application.CQRS.ForgotPassword.Commands.Tests
{
    [TestFixture]
    public class VerifyOtpHandlerTests
    {
        [Test]
        public void Validate_ShouldFail_WhenContactIsNull()
        {
            var cmd = new VerifyOtpCommand
            {
                Contact = null,
                OtpCode = "123456",
                Channel = OTPChannel.Email,
            };

            var results = new List<ValidationResult>();
            Validator.TryValidateObject(cmd, new ValidationContext(cmd), results, true);

            Assert.IsTrue(results.Exists(r => r.ErrorMessage.Contains("Thông tin liên hệ")));
        }

        [Test]
        public void Validate_ShouldFail_WhenContactIsInvalid()
        {
            var cmd = new VerifyOtpCommand
            {
                Contact = "invalid_contact",
                OtpCode = "123456",
                Channel = OTPChannel.Email,
            };

            var results = new List<ValidationResult>();
            Validator.TryValidateObject(cmd, new ValidationContext(cmd), results, true);

            Assert.IsTrue(results.Exists(r => r.ErrorMessage.Contains("Email hoặc số điện thoại")));
        }

        [Test]
        public void Validate_ShouldPass_WhenContactIsEmail()
        {
            var cmd = new VerifyOtpCommand
            {
                Contact = "test@example.com",
                OtpCode = "123456",
                Channel = OTPChannel.Email,
            };

            var results = new List<ValidationResult>();
            Validator.TryValidateObject(cmd, new ValidationContext(cmd), results, true);

            Assert.IsEmpty(results);
        }

        [Test]
        public void Validate_ShouldPass_WhenContactIsPhone()
        {
            var cmd = new VerifyOtpCommand
            {
                Contact = "+84987654321",
                OtpCode = "123456",
                Channel = OTPChannel.SMS,
            };

            var results = new List<ValidationResult>();
            Validator.TryValidateObject(cmd, new ValidationContext(cmd), results, true);

            Assert.IsEmpty(results);
        }

        [Test]
        public void Validate_ShouldFail_WhenOtpCodeIsNull()
        {
            var cmd = new VerifyOtpCommand
            {
                Contact = "test@example.com",
                OtpCode = null,
                Channel = OTPChannel.Email,
            };

            var results = new List<ValidationResult>();
            Validator.TryValidateObject(cmd, new ValidationContext(cmd), results, true);

            Assert.IsTrue(results.Exists(r => r.ErrorMessage.Contains("Mã OTP")));
        }

        [Test]
        public void Validate_ShouldFail_WhenChannelInvalid()
        {
            var cmd = new VerifyOtpCommand
            {
                Contact = "test@example.com",
                OtpCode = "123456",
                Channel = (OTPChannel)999, // invalid
            };

            var results = new List<ValidationResult>();
            Validator.TryValidateObject(cmd, new ValidationContext(cmd), results, true);

            Assert.IsTrue(results.Exists(r => r.ErrorMessage.Contains("Kênh gửi OTP")));
        }

        [Test]
        public async Task Handle_ShouldReturnGuid_WhenOtpIsValid()
        {
            // Arrange
            var mockOtpService = new Mock<IOTPService>();
            mockOtpService
                .Setup(s => s.VerifyOtpAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync("reset-guid-123");

            var handler = new VerifyOtpHandler(mockOtpService.Object, null);

            var cmd = new VerifyOtpCommand
            {
                Contact = "test@example.com",
                OtpCode = "123456",
                Channel = OTPChannel.Email,
            };

            // Act
            var result = await handler.Handle(cmd, CancellationToken.None);

            // Assert
            Assert.AreEqual("reset-guid-123", result);
            mockOtpService.Verify(s => s.VerifyOtpAsync("test@example.com", "123456"), Times.Once);
        }
    }
}
