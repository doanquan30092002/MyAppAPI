using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Common.Message;
using MyApp.Application.Common.Response;
using MyApp.Application.CQRS.DetailAuctionDocument.Queries;

namespace MyApp.Api.Controllers.DetailAuctionDocument
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DetailAuctionDocumentController(IMediator _mediator) : ControllerBase
    {
        [HttpGet]
        [Route("Detail-Auction-Document")]
        public async Task<ActionResult> DetailAuctionDocument(
            [FromQuery] DetailAuctionDocumentRequest request
        )
        {
            var response = await _mediator.Send(request);
            if (response == null)
            {
                return Ok(new { Code = 404, Message = Message.GET_AUCTION_DOCUMENT_FAIL });
            }
            return Ok(
                new ApiResponse<DetailAuctionDocumentResponse>
                {
                    Code = 200,
                    Message = Message.GET_AUCTION_DOCUMENT_SUCCESS,
                    Data = response,
                }
            );
        }
    }
}
