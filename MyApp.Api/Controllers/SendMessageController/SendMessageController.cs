using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Common.Response;
using MyApp.Application.CQRS.SendMessageToUser.Command;

namespace MyApp.Api.Controllers.SendMessageController
{
    [Route("api/[controller]")]
    [ApiController]
    public class SendMessageController(IMediator _mediator) : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result)
            {
                return BadRequest(
                    new ApiResponse<string>
                    {
                        Code = 400,
                        Message = "Gửi tin nhắn thất bại",
                        Data = null,
                    }
                );
            }

            return Ok(
                new ApiResponse<string>
                {
                    Code = 200,
                    Message = "Gửi tin nhắn thành công",
                    Data = null,
                }
            );
        }
    }
}
