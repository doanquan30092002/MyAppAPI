using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Common.Response;
using MyApp.Application.CQRS.Auction.GetListAuction.Querries;
using MyApp.Application.CQRS.Auction.GetListAution.Querries;

namespace MyApp.Api.Controllers.GetListAuctionDocumentsController
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetListAuctionDocumentsController(IMediator _mediator) : ControllerBase
    {
        [HttpGet]
        [Route("ListDocuments/{auction_id}")]
        [AllowAnonymous]
        public async Task<
            ActionResult<ApiResponse<GetListAuctionDocumentsResponse>>
        > GetListAuctionDocuments(
            [FromRoute] Guid auction_id,
            [FromQuery] GetListAuctionDocumentsRequest getListAuctionDocumentsRequest
        )
        {
            try
            {
                // Set the auction_id in the request object
                getListAuctionDocumentsRequest.AuctionId = auction_id;

                var response = await _mediator.Send(getListAuctionDocumentsRequest);
                if (response == null)
                {
                    return NotFound(
                        new ApiResponse<GetListAuctionDocumentsResponse>
                        {
                            Code = 404,
                            Message = "List of auction documents not found.",
                            Data = null,
                        }
                    );
                }

                return Ok(
                    new ApiResponse<GetListAuctionDocumentsResponse>
                    {
                        Code = 200,
                        Message = "Auction documents retrieved successfully.",
                        Data = response,
                    }
                );
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new ApiResponse<GetListAuctionDocumentsResponse>
                    {
                        Code = 500,
                        Message =
                            $"An error occurred while retrieving the auction documents: {ex.Message}",
                        Data = null,
                    }
                );
            }
        }
    }
}
