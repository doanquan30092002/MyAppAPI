using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Common.Message;
using MyApp.Application.CQRS.UserRegisteredAuction;

namespace MyApp.Api.Controllers.UserRegisteredAuction
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Staff,Auctioneer")]
    public class UserRegisteredAuctionController(IMediator _mediator) : ControllerBase
    {
        [HttpPost]
        [Route("User-Registered-Auction")]
        public async Task<ActionResult> UserRegisteredAuction(
            [FromBody] UserRegisteredAuctionRequest request
        )
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }
    }
}
