using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<ActionResult<GetAuctionByIdResponse>> GetAuctionById(
            [FromRoute] Guid auction_id
        )
        {
            try
            {
                // Create a GetListAuctionRequest with only AuctionId set
                var request = new GetListAuctionRequest { AuctionId = auction_id };

                // Send the request to the mediator
                var response = await _mediator.Send(request);

                // Check if any auction was found
                if (response.Auctions == null || !response.Auctions.Any())
                {
                    return NotFound("Auction not found.");
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    $"An error occurred while retrieving the auction: {ex.Message}"
                );
            }
        }
    }
}
