using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Common.Message;
using MyApp.Application.Common.Response;
using MyApp.Application.CQRS.Auction.GetListAuctionById.Querries;

namespace MyApp.Api.Controllers.GetAuctionByIdController
{
    public class GetAuctionByIdController(IMediator _mediator) : ControllerBase
    {
        [HttpGet]
        [Route("api/Detail/{auction_id}")]
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
                        Message = Message.HANDLER_SUCCESS,
                        Data = response,
                    }
                );
            }
            catch (Exception ex)
            {
                return new ApiResponse<GetAuctionByIdResponse>
                {
                    Code = 500,
                    Message = Message.HANDLER_ERROR + ex.Message,
                    Data = null,
                };
            }
        }
    }
}
