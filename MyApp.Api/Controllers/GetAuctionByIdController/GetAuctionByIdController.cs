using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Common.Message;
using MyApp.Application.Common.Response;
using MyApp.Application.CQRS.Auction.GetListAuction.Querries;
using MyApp.Application.CQRS.Auction.GetListAuctionById.Querries;
using MyApp.Application.CQRS.Auction.GetListAution.Querries;
using MyApp.Application.CQRS.SignUp.Command;

namespace MyApp.Api.Controllers.GetAuctionByIdController
{
    public class GetAuctionByIdController(IMediator _mediator) : ControllerBase
    {
        [HttpGet]
        [Route("Detail/{auction_id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<GetAuctionByIdResponse>>> GetAuctionById(
            [FromRoute] Guid auction_id
        )
        {
            try
            {
                var request = new GetAuctionByIdRequest { AuctionId = auction_id };
                var response = await _mediator.Send(request);

                if (response == null)
                {
                    return NotFound(
                        new ApiResponse<GetAuctionByIdResponse>
                        {
                            Code = 400,
                            Message = "Auction not found.",
                            Data = null,
                        }
                    );
                }

                return Ok(
                    new ApiResponse<GetAuctionByIdResponse>
                    {
                        Code = 200,
                        Message = "Auction retrieved successfully.",
                        Data = response,
                    }
                );
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new ApiResponse<GetAuctionByIdResponse>
                    {
                        Code = 500,
                        Message = $"An error occurred while retrieving the auction: {ex.Message}",
                        Data = null,
                    }
                );
            }
        }
    }
}
