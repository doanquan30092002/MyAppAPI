using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            return Ok(new { message = result });
        }
    }
}
