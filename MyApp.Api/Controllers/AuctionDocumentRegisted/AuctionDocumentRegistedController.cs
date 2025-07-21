using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Common.Message;
using MyApp.Application.CQRS.AuctionDocumentRegisted;

namespace MyApp.Api.Controllers.AuctionDocumentRegisted
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Customer")]
    public class AuctionDocumentRegistedController(IMediator _mediator) : ControllerBase
    {
        [HttpPost]
        [Route("Auction-Document-Registed")]
        public async Task<ActionResult> AuctionDocumentRegisted(
            [FromBody] AuctionDocumentRegistedRequest request
        )
        {
            var response = await _mediator.Send(request);
            return Ok(
                new
                {
                    Code = response != null ? 200 : 404,
                    Message = response != null
                        ? Message.GET_AUCTION_DOCUMENT_REGISTED_SUCCESS
                        : Message.GET_AUCTION_DOCUMENT_REGISTED_NOT_EXIST,
                    Data = response != null ? response : null,
                }
            );
        }
    }
}
