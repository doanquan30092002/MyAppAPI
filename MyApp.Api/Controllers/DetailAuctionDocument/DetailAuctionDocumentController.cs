using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Common.Message;
using MyApp.Application.CQRS.DetailAuctionDocument.Queries;
using MyApp.Application.CQRS.UpdateExpiredProfile.Command;

namespace MyApp.Api.Controllers.DetailAuctionDocument
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetailAuctionDocumentController(IMediator _mediator) : ControllerBase
    {
        [HttpGet]
        [Route("Detail-Auction-Document")]
        public async Task<ActionResult> UpdateExpiredProfile(
            [FromQuery] DetailAuctionDocumentRequest request
        )
        {
            var response = await _mediator.Send(request);
            if (response == null)
            {
                return Ok(new { Code = 404, Message = Message.GET_AUCTION_DOCUMENT_FAIL });
            }
            return Ok(
                new
                {
                    Code = 200,
                    Message = Message.GET_AUCTION_DOCUMENT_SUCCESS,
                    Data = response,
                }
            );
        }
    }
}
