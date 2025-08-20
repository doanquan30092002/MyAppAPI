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
    public class VerifyOtpCommand : IRequest<string>, IValidatableObject
    {
        [Required(ErrorMessage = "Thông tin liên hệ là bắt buộc.")]
        public string Contact { get; set; }

        [Required(ErrorMessage = "Mã OTP là bắt buộc.")]
        public string OtpCode { get; set; }

        [Required(ErrorMessage = "Kênh gửi OTP là bắt buộc.")]
        public OTPChannel Channel { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Contact
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
                        "Thông tin liên hệ phải là Email hoặc số điện thoại.",
                        new[] { nameof(Contact) }
                    );
                }
            }

            // OTP
            if (string.IsNullOrWhiteSpace(OtpCode))
            {
                yield return new ValidationResult("Mã OTP là bắt buộc.", new[] { nameof(OtpCode) });
            }

            // Channel
            if (!Enum.IsDefined(typeof(OTPChannel), Channel))
            {
                yield return new ValidationResult(
                    "Kênh gửi OTP không hợp lệ.",
                    new[] { nameof(Channel) }
                );
            }
        }
    }
}
