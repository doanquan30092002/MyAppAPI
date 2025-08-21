using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MediatR;
using MyApp.Application.Common.Services.SendMessage;
using MyApp.Application.CQRS.ForgotPassword.Enums;

namespace MyApp.Application.CQRS.SendMessageToUser.Command
{
    public class SendMessageCommand : IRequest<bool>, IValidatableObject
    {
        [Required(ErrorMessage = "Thông tin liên lạc là bắt buộc.")]
        public string Contact { get; set; }

        [Range(0, 1, ErrorMessage = "Kênh gửi tin nhắn không hỗ trợ.")]
        public SendMessageChannel Channel { get; set; }

        [Required(ErrorMessage = "Thông điệp gửi đi là bắt buộc.")]
        public string Message { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Contact))
            {
                yield return new ValidationResult(
                    "Thông tin liên lạc là bắt buộc",
                    new[] { nameof(Contact) }
                );
            }
            else
            {
                var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

                var phoneRegex = new Regex(@"^\+?\d{9,15}$");

                if (!emailRegex.IsMatch(Contact) && !phoneRegex.IsMatch(Contact))
                {
                    yield return new ValidationResult(
                        "Thông tin liên lạc phải là Email hoặc số điện thoại",
                        new[] { nameof(Contact) }
                    );
                }
            }

            if (
                !Enum.IsDefined(typeof(SendMessageChannel), Channel)
                || (int)Channel < 0
                || (int)Channel > 1
            )
            {
                yield return new ValidationResult(
                    "kênh gửi tin nhắn phải trong 0 (Email) hoặc 1 (SMS).",
                    new[] { nameof(Channel) }
                );
            }
        }
    }
}
