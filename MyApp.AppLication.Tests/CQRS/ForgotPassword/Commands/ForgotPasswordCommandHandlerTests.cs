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
using MyApp.Application.Interfaces.IForgetPasswordRepository;
using MyApp.Core.Entities;
using NUnit.Framework;

namespace MyApp.Application.CQRS.ForgotPassword.Commands.Tests
{
    [TestFixture]
    public class ForgotPasswordCommandHandlerTests
    {
        private Mock<IForgetPasswordRepository> _mockRepo;
        private Mock<IOTPService> _mockEmailOtpService;
        private Mock<IOTPService> _mockSmsOtpService;

        private ForgotPasswordCommandHandler _handler;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IForgetPasswordRepository>();
            _mockEmailOtpService = new Mock<IOTPService>();
            _mockSmsOtpService = new Mock<IOTPService>();

            _mockEmailOtpService.Setup(s => s.Channel).Returns(OTPChannel.Email);
            _mockSmsOtpService.Setup(s => s.Channel).Returns(OTPChannel.SMS);

            var services = new List<IOTPService>
            {
                _mockEmailOtpService.Object,
                _mockSmsOtpService.Object,
            };
            _handler = new ForgotPasswordCommandHandler(services, _mockRepo.Object);
        }

        [Test]
        public async Task Handle_EmailChannel_Success()
        {
            // Arrange
            var command = new ForgotPasswordCommand
            {
                Contact = "test@example.com",
                Channel = OTPChannel.Email,
            };
            var account = new Account { AccountId = Guid.NewGuid() };
            _mockRepo
                .Setup(r => r.FindByContactAsync(command.Contact, command.Channel))
                .ReturnsAsync(account);
            _mockEmailOtpService
                .Setup(s => s.SendOtpAsync(command.Contact, null))
                .ReturnsAsync("OTP_SENT");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.AreEqual("OTP_SENT", result);
        }

        [Test]
        public async Task Handle_SmsChannel_Success()
        {
            var command = new ForgotPasswordCommand
            {
                Contact = "+84123456789",
                Channel = OTPChannel.SMS,
            };
            var account = new Account { AccountId = Guid.NewGuid() };
            _mockRepo
                .Setup(r => r.FindByContactAsync(command.Contact, command.Channel))
                .ReturnsAsync(account);
            _mockSmsOtpService
                .Setup(s => s.SendOtpAsync(command.Contact, null))
                .ReturnsAsync("OTP_SENT");

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.AreEqual("OTP_SENT", result);
        }

        [Test]
        public void Handle_AccountNotFound_ThrowsArgumentException()
        {
            var command = new ForgotPasswordCommand
            {
                Contact = "unknown@example.com",
                Channel = OTPChannel.Email,
            };
            _mockRepo
                .Setup(r => r.FindByContactAsync(command.Contact, command.Channel))
                .ReturnsAsync((Account)null);

            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _handler.Handle(command, CancellationToken.None)
            );

            Assert.AreEqual("Tài khoản không tồn tại với thông tin liên hệ này.", ex.Message);
        }

        [Test]
        public void Handle_ChannelNotSupported_ThrowsNotSupportedException()
        {
            var command = new ForgotPasswordCommand
            {
                Contact = "test@example.com",
                Channel = (OTPChannel)99,
            };
            var account = new Account { AccountId = Guid.NewGuid() };
            _mockRepo
                .Setup(r => r.FindByContactAsync(command.Contact, command.Channel))
                .ReturnsAsync(account);

            var ex = Assert.ThrowsAsync<NotSupportedException>(async () =>
                await _handler.Handle(command, CancellationToken.None)
            );

            Assert.IsTrue(ex.Message.Contains("Kênh gửi OTP"));
        }

        [Test]
        public void Validate_InvalidContact_ReturnsValidationError()
        {
            var command = new ForgotPasswordCommand
            {
                Contact = "invalid-contact",
                Channel = OTPChannel.Email,
            };
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(
                command,
                new ValidationContext(command),
                validationResults,
                true
            );

            Assert.IsFalse(isValid);
            Assert.IsTrue(
                validationResults.Exists(v => v.ErrorMessage.Contains("Email hoặc số điện thoại"))
            );
        }

        [Test]
        public void Validate_InvalidChannel_ReturnsValidationError()
        {
            var command = new ForgotPasswordCommand
            {
                Contact = "test@example.com",
                Channel = (OTPChannel)99,
            };

            // Lấy ValidationResult trực tiếp từ Validate()
            var validationResults = new List<ValidationResult>();
            var results = command.Validate(new ValidationContext(command));
            validationResults.AddRange(results);

            Assert.IsTrue(validationResults.Exists(v => v.ErrorMessage.Contains("kênh gửi OTP")));
        }
    }
}
