using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.CQRS.Auction.GetListAution.Querries;
using MyApp.Application.CQRS.LoginUser.Queries;

namespace MyApp.Api.Controllers.GetListAuctionController
{
    public class GetListAuctionController(IMediator _mediator) : ControllerBase
    {
        [HttpGet]
        [Route("List")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginUserResponse>> getListAuction(
            [FromQuery] GetListAuctionRequest getListAuctionRequest
        )
        {
            var response = await _mediator.Send(getListAuctionRequest);
            return Ok(response);
        }
    }
}
