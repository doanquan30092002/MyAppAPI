using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Common.Response;
using MyApp.Application.CQRS.ForgotPassword.Commands;

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

        [HttpPost("verify-otp-and-change-password")]
        public async Task<IActionResult> VerifyOtpAndChangePassword(
            [FromBody] VerifyOtpAndChangePasswordCommand command
        )
        {
            var result = await _mediator.Send(command);

            if (!result)
            {
                return BadRequest(
                    new ApiResponse<string>
                    {
                        Code = 400,
                        Message = "OTP không hợp lệ hoặc đổi mật khẩu thất bại",
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
