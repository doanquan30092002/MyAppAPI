using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Common.Response;
using MyApp.Application.CQRS.LoginUser.Queries;

namespace MyApp.Api.Controllers.LoginUserController
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginUserController(IMediator _mediator) : ControllerBase
    {
        [HttpPost]
        [Route("Login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginUserResponse>> Login(
            [FromBody] LoginUserRequest loginRequest
        )
        {
            var response = await _mediator.Send(loginRequest);
            // Nếu đăng nhập thành công và có token
            if (!string.IsNullOrEmpty(response.Token))
            {
                // Gửi cookie
                Response.Cookies.Append(
                    "access_token",
                    response.Token,
                    new CookieOptions
                    {
                        HttpOnly = true, // Ngăn JS truy cập → chống XSS
                        Secure = true, // Chỉ gửi qua HTTPS
                        SameSite = SameSiteMode.Strict, // Tránh CSRF
                        Expires = DateTimeOffset.UtcNow.AddDays(1), // Thời gian sống của cookie
                    }
                );
            }
            return Ok(
                new ApiResponse<LoginUserResponse>
                {
                    Code = string.IsNullOrEmpty(response.Token) ? 400 : 200,
                    Message = response.Message,
                    Data = response.Token != null ? response : null,
                }
            );
        }
    }
}
