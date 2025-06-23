using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<ActionResult<UpdateExpiredProfileResponse>> UpdateExpiredProfile(
            [FromBody] UpdateExpiredProfileRequest request
        )
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }
    }
}
