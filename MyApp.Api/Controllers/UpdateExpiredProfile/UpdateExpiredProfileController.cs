using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Common.Response;
using MyApp.Application.CQRS.UpdateExpiredProfile.Command;

namespace MyApp.Api.Controllers.UpdateExpiredProfile
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UpdateExpiredProfileController(IMediator _mediator) : ControllerBase
    {
        [HttpPost]
        [Route("Update-Expired-Profile")]
        public async Task<ActionResult> UpdateExpiredProfile(
            [FromBody] UpdateExpiredProfileRequest request
        )
        {
            var response = await _mediator.Send(request);
            return Ok(
                new ApiResponse<UpdateExpiredProfileResponse>
                {
                    Code = response.Code,
                    Message = response.Message,
                }
            );
        }
    }
}
