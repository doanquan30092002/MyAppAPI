using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MediatR;
using MyApp.Application.CQRS.ForgotPassword.Enums;

namespace MyApp.Application.CQRS.ForgotPassword.Commands
{
    public class VerifyOtpAndChangePasswordCommand : IRequest<bool>, IValidatableObject
    {
        [Required(ErrorMessage = "Thông tin liên hệ là bắt buộc.")]
        public string Contact { get; set; }

        [Required(ErrorMessage = "Mã OTP là bắt buộc.")]
        public string OtpCode { get; set; }

        [Required(ErrorMessage = "Mật khẩu mới là bắt buộc.")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Kênh gữi OTP là bắt buộc.")]
        [Range(0, 1, ErrorMessage = "Kênh OTP không hỗ trợ")]
        public OTPChannel Channel { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Contact))
            {
                yield return new ValidationResult(
                    "Thông tin liên hệ là bắt buộc.",
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
                        "Thông tin lạc phải là Email hoặc số điện thoại.",
                        new[] { nameof(Contact) }
                    );
                }
            }

            // Validate OtpCode
            if (string.IsNullOrWhiteSpace(OtpCode))
            {
                yield return new ValidationResult("Mã OTP là bắt buộc", new[] { nameof(OtpCode) });
            }

            // Validate NewPassword
            if (string.IsNullOrWhiteSpace(NewPassword))
            {
                yield return new ValidationResult(
                    "Mất khẩu mới là bắt buộc",
                    new[] { nameof(NewPassword) }
                );
            }

            if (
                !Enum.IsDefined(typeof(OTPChannel), Channel)
                || (int)Channel < 0
                || (int)Channel > 1
            )
            {
                yield return new ValidationResult(
                    "Kênh gửi OTP phải là 0 (Email) hoặc 1 (SMS).",
                    new[] { nameof(Channel) }
                );
            }
        }
    }
}
