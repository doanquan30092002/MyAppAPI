using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Common.Message;
using MyApp.Application.Common.Response;
using MyApp.Application.CQRS.Auction.GetListAuction.Querries;
using MyApp.Application.CQRS.Auction.GetListAution.Querries;
using MyApp.Application.CQRS.GetAuctionRoundStatistics.Queries;

namespace MyApp.Api.Controllers.GetAuctionRoundStatisticsController
{
    [Route("api")]
    [ApiController]
    public class GetAuctionRoundStatisticsController(IMediator _mediator) : ControllerBase
    {
        [HttpGet]
        [Route("AuctionStatistics/{auction_id}")]
        [AllowAnonymous]
        public async Task<
            ActionResult<ApiResponse<GetAuctionRoundStatisticsResponse>>
        > GetAuctionRoundStatistics(
            [FromRoute] Guid auction_id,
            [FromQuery] GetAuctionRoundStatisticsRequest getAuctionRoundStatisticsRequest
        )
        {
            try
            {
                // Set the auction_id in the request object
                getAuctionRoundStatisticsRequest.AuctionId = auction_id;

                var response = await _mediator.Send(getAuctionRoundStatisticsRequest);
                if (response == null)
                {
                    return NotFound(
                        new ApiResponse<GetAuctionRoundStatisticsResponse>
                        {
                            Code = 404,
                            Message = Message.ID_AUCTION_NOT_FOUND,
                            Data = null,
                        }
                    );
                }

                return Ok(
                    new ApiResponse<GetAuctionRoundStatisticsResponse>
                    {
                        Code = 200,
                        Message = Message.HANDLER_SUCCESS,
                        Data = response,
                    }
                );
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new ApiResponse<GetAuctionRoundStatisticsResponse>
                    {
                        Code = 500,
                        Message = Message.HANDLER_ERROR,
                        Data = null,
                    }
                );
            }
        }
    }
}
