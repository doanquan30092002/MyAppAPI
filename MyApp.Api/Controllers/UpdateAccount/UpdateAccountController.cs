using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.CQRS.UpdateAccountAndProfile.Command;

namespace MyApp.Api.Controllers.EditAccountAndProfile
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UpdateAccountController(IMediator _mediator) : ControllerBase
    {
        [HttpPost]
        [Route("Update-Account")]
        public async Task<ActionResult<UpdateAccountResponse>> UpdateAccount(
            [FromBody] UpdateAccountRequest request
        )
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }
    }
}
