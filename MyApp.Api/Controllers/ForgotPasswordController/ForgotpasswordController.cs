using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Common.Response;
using MyApp.Application.CQRS.ForgotPassword.Commands;
using MyApp.Core.DTOs.ResetPassDTO;

namespace MyApp.Api.Controllers.ForgotPasswordController
{
    [Route("api/[controller]")]
    [ApiController]
    public class ForgotpasswordController(IMediator _mediator) : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command)
        {
            var result = await _mediator.Send(command);
            var response = new ApiResponse<string> { Code = 200, Message = "Gửi OTP thành công" };
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpCommand command)
        {
            var resetGuid = await _mediator.Send(command);
            if (string.IsNullOrEmpty(resetGuid))
            {
                return BadRequest(
                    new ApiResponse<string>
                    {
                        Code = 400,
                        Message = "OTP không hợp lệ hoặc đã hết hạn",
                        Data = null,
                    }
                );
            }

            return Ok(
                new ApiResponse<ResetPasswordTokenResponse>
                {
                    Code = 200,
                    Message = "OTP hợp lệ, bạn có thể đặt lại mật khẩu",
                    Data = new ResetPasswordTokenResponse { ResetToken = resetGuid },
                }
            );
        }

        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
        {
            var result = await _mediator.Send(command);
            if (!result)
            {
                return BadRequest(
                    new ApiResponse<string>
                    {
                        Code = 400,
                        Message = "ResetGuid không hợp lệ hoặc đổi mật khẩu thất bại",
                        Data = null,
                    }
                );
            }

            return Ok(
                new ApiResponse<string>
                {
                    Code = 200,
                    Message = "Đổi mật khẩu thành công",
                    Data = null,
                }
            );
        }
    }
}
