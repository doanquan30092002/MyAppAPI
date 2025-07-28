using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Common.Message;
using MyApp.Application.Common.Response;
using MyApp.Application.CQRS.ChangeStatusAuctionRound;

namespace MyApp.Api.Controllers.ChangeStatusAuctionRound
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Auctioneer")]
    public class ChangeStatusAuctionRoundController(IMediator _mediator) : ControllerBase
    {
        [HttpPost]
        [Route("Change-Status-Auction-Round")]
        public async Task<ActionResult> ChangeStatusAuctionRound(
            [FromBody] ChangeStatusAuctionRoundRequest request
        )
        {
            var response = await _mediator.Send(request);
            return Ok(
                new ApiResponse<bool>
                {
                    Code = response ? 200 : 500,
                    Message = response
                        ? Message.CHANGE_STATUS_AUCTION_ROUND_SUCCESS
                        : Message.CHANGE_STATUS_AUCTION_ROUND_FAIL,
                    Data = response,
                }
            );
        }
    }
}
