using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using MyApp.Application.Common.Services.SendMessage;
using MyApp.Application.CQRS.SendMessageToUser.Command;
using NUnit.Framework;

namespace MyApp.Application.CQRS.SendMessageToUser.Command.Tests
{
    [TestFixture]
    public class SendMessageTests
    {
        // ------------------- Validate -------------------
        [Test]
        public void Validate_ShouldFail_WhenContactIsEmpty()
        {
            var cmd = new SendMessageCommand
            {
                Contact = "",
                Channel = SendMessageChannel.Email,
                Message = "Hello",
            };

            var results = new List<ValidationResult>();
            Validator.TryValidateObject(cmd, new ValidationContext(cmd), results, true);

            Assert.That(
                results,
                Has.Some.Matches<ValidationResult>(r =>
                    r.ErrorMessage.Contains("Thông tin liên lạc là bắt buộc")
                )
            );
        }

        [Test]
        public void Validate_ShouldFail_WhenContactIsInvalid()
        {
            var cmd = new SendMessageCommand
            {
                Contact = "invalid-contact",
                Channel = SendMessageChannel.Email,
                Message = "Hello",
            };

            var results = cmd.Validate(new ValidationContext(cmd));

            Assert.That(
                results,
                Has.Some.Matches<ValidationResult>(r =>
                    r.ErrorMessage.Contains("Thông tin liên lạc phải là Email")
                )
            );
        }

        [Test]
        public void Validate_ShouldFail_WhenChannelInvalid()
        {
            var cmd = new SendMessageCommand
            {
                Contact = "user@example.com",
                Channel = (SendMessageChannel)99, // invalid
                Message = "Hello",
            };

            var results = cmd.Validate(new ValidationContext(cmd));

            Assert.That(
                results,
                Has.Some.Matches<ValidationResult>(r =>
                    r.ErrorMessage.Contains("kênh gửi tin nhắn")
                )
            );
        }

        [Test]
        public void Validate_ShouldPass_WhenValidEmail()
        {
            var cmd = new SendMessageCommand
            {
                Contact = "user@example.com",
                Channel = SendMessageChannel.Email,
                Message = "Hello",
            };

            var results = cmd.Validate(new ValidationContext(cmd));

            Assert.IsEmpty(results); // No errors
        }

        [Test]
        public void Validate_ShouldPass_WhenValidPhone()
        {
            var cmd = new SendMessageCommand
            {
                Contact = "+84123456789",
                Channel = SendMessageChannel.SMS,
                Message = "Hi",
            };

            var results = cmd.Validate(new ValidationContext(cmd));

            Assert.IsEmpty(results); // No errors
        }

        // ------------------- Handler -------------------
        [Test]
        public void Handle_ShouldThrow_WhenChannelNotSupported()
        {
            var handler = new SendMessageHandler(new List<ISendMessage>()); // empty
            var cmd = new SendMessageCommand
            {
                Contact = "user@example.com",
                Channel = SendMessageChannel.Email,
                Message = "Hi",
            };

            Assert.ThrowsAsync<NotSupportedException>(() =>
                handler.Handle(cmd, CancellationToken.None)
            );
        }

        [Test]
        public async Task Handle_ShouldSendEmail_WithSubject()
        {
            var mockSender = new Mock<ISendMessage>();
            mockSender.SetupGet(x => x.Channel).Returns(SendMessageChannel.Email);
            mockSender
                .Setup(x =>
                    x.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null)
                )
                .ReturnsAsync(true);

            var handler = new SendMessageHandler(new[] { mockSender.Object });

            var cmd = new SendMessageCommand
            {
                Contact = "user@example.com",
                Channel = SendMessageChannel.Email,
                Message = "Email message",
            };

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.IsTrue(result);
            mockSender.Verify(
                x =>
                    x.SendAsync("user@example.com", "Thông báo từ hệ thống", "Email message", null),
                Times.Once
            );
        }

        [Test]
        public async Task Handle_ShouldSendSms_WithEmptySubject()
        {
            var mockSender = new Mock<ISendMessage>();
            mockSender.SetupGet(x => x.Channel).Returns(SendMessageChannel.SMS);
            mockSender
                .Setup(x =>
                    x.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null)
                )
                .ReturnsAsync(false);

            var handler = new SendMessageHandler(new[] { mockSender.Object });

            var cmd = new SendMessageCommand
            {
                Contact = "+84123456789",
                Channel = SendMessageChannel.SMS,
                Message = "SMS message",
            };

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.IsFalse(result);
            mockSender.Verify(
                x => x.SendAsync("+84123456789", "", "SMS message", null),
                Times.Once
            );
        }
    }
}
