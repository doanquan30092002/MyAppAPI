using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Common.Message;

namespace MyApp.Api.Controllers.Logout
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogoutController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            Response.Cookies.Delete(
                "access_token",
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Path = "/", // Rất quan trọng nếu bạn đã set Path khi tạo
                }
            );

            return Ok(new { Code = 200, Message = Message.LOGOUT_SUCCESS });
        }
    }
}
