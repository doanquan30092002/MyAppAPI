using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Common.Response;
using MyApp.Application.CQRS.AuctionDocuments.SupportRegisterDocuments.Command;

namespace MyApp.Api.Controllers.AuctionDocumentsController
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuctionDocumentsController(IMediator _mediator) : ControllerBase
    {
        [Authorize(Roles = "Staff")]
        [HttpPost("support-register")]
        public async Task<IActionResult> SupportRegisterDocuments(
            [FromBody] SupportRegisterDocumentsCommand command
        )
        {
            var result = await _mediator.Send(command);

            return Ok(
                new ApiResponse<string>
                {
                    Code = 200,
                    Message = "Đăng ký hồ sơ đấu giá thành công",
                    Data = null,
                }
            );
        }
    }
}
