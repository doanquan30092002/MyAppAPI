using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Common.Message;
using MyApp.Application.CQRS.ListAuctionRegisted;

namespace MyApp.Api.Controllers.ListAuctionRegisted
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Customer")]
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
                new
                {
                    Code = response.TotalAuctionRegisted != 0 ? 200 : 404,
                    Message = response.TotalAuctionRegisted != 0
                        ? Message.GET_LIST_AUCTION_REGISTED_SUCCESS
                        : Message.GET_LIST_AUCTION_REGISTED_NOT_EXSIT,
                    Data = response.TotalAuctionRegisted != 0 ? response : null,
                }
            );
        }
    }
}
