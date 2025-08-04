using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Common.Message;
using MyApp.Application.Common.Response;
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

            if (response == null || response.Count == 0)
            {
                return NotFound(
                    new ApiResponse<List<AuctionDocumentRegistedResponse>>
                    {
                        Code = 404,
                        Message = Message.GET_AUCTION_DOCUMENT_REGISTED_NOT_EXSIT,
                        Data = null,
                    }
                );
            }

            return Ok(
                new ApiResponse<List<AuctionDocumentRegistedResponse>>
                {
                    Code = 200,
                    Message = Message.GET_AUCTION_DOCUMENT_REGISTED_SUCCESS,
                    Data = response,
                }
            );
        }
    }
}
