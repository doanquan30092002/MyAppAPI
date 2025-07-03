using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.CQRS.AssginAuctioneerAndPublicAuction.Command;

namespace MyApp.Api.Controllers.AssginAuctioneerAndPublicAuction
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Manager")]
    [Authorize]
    public class AssginAuctioneerAndPublicAuctionController(IMediator _mediator) : ControllerBase
    {
        [HttpPost]
        [Route("Assgin-Auctioneer-Public-Auction")]
        public async Task<
            ActionResult<AssginAuctioneerAndPublicAuctionResponse>
        > AssginAuctioneerAndPublicAuction(
            [FromBody] AssginAuctioneerAndPublicAuctionRequest request
        )
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }
    }
}
