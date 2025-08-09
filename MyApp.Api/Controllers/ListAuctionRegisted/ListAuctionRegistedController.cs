using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Common.Message;
using MyApp.Application.Common.Response;
using MyApp.Application.CQRS.ListAuctionRegisted;

namespace MyApp.Api.Controllers.ListAuctionRegisted
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ListAuctionRegistedController(IMediator _mediator) : ControllerBase
    {
        [HttpPost]
        [Route("List-Auction-Registed")]
        public async Task<ActionResult> ListAuctionRegisted(
            [FromBody] AuctionRegistedRequest request
        )
        {
            var response = await _mediator.Send(request);
            return Ok(
                new ApiResponse<AuctionRegistedResponse>
                {
                    Code = response.TotalAuctionRegisted != 0 ? 200 : 404,
                    Message =
                        response.TotalAuctionRegisted != 0
                            ? Message.GET_LIST_AUCTION_REGISTED_SUCCESS
                            : Message.GET_LIST_AUCTION_REGISTED_NOT_EXIST,
                    Data = response.TotalAuctionRegisted != 0 ? response : null,
                }
            );
        }
    }
}
