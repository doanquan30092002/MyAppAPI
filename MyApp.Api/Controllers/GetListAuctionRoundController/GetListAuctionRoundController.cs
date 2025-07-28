using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Common.Message;
using MyApp.Application.Common.Response;
using MyApp.Application.CQRS.GetListAuctionRound.Querries;

namespace MyApp.Api.Controllers
{
    [ApiController]
    [Route("api/ListAuctionRound")]
    public class AuctionRoundController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuctionRoundController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{auction_id}")]
        [AllowAnonymous]
        public async Task<
            ActionResult<ApiResponse<GetListAuctionRoundResponse>>
        > GetAuctionRoundsByAuctionId([FromRoute] Guid auction_id)
        {
            try
            {
                var request = new GetListAuctionRoundRequest { AuctionId = auction_id };
                var response = await _mediator.Send(request);

                if (response.AuctionRounds == null || response.AuctionRounds.Count == 0)
                {
                    return NotFound(
                        new ApiResponse<GetListAuctionRoundResponse>
                        {
                            Code = 404,
                            Message = Message.NOT_FOUND_ROUND_BY_AUCTION_ID,
                            Data = null,
                        }
                    );
                }

                return Ok(
                    new ApiResponse<GetListAuctionRoundResponse>
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
                    new ApiResponse<GetListAuctionRoundResponse>
                    {
                        Code = 500,
                        Message = Message.HANDLER_ERROR + ex.Message,
                        Data = null,
                    }
                );
            }
        }
    }
}
