using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Common.Message;
using MyApp.Application.Common.Response;
using MyApp.Application.CQRS.GetListUserWinner.Queries;
using MyApp.Application.CQRS.GetListUserWinner.Querries;

namespace MyApp.Api.Controllers.GetListUserWinnerController
{
    [Route("api")]
    [ApiController]
    public class GetListUserWinnerController(IMediator _mediator) : ControllerBase
    {
        [HttpGet]
        [Route("ListUserWinner/{auctionId}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<GetListUserWinnerResponse>>> GetListUserWinner(
            [FromRoute] Guid auctionId
        )
        {
            try
            {
                var request = new GetListUserWinnerRequest { AuctionId = auctionId };
                var response = await _mediator.Send(request);

                if (
                    response == null
                    || response.auctionRoundPrices == null
                    || !response.auctionRoundPrices.Any()
                )
                {
                    return NotFound(
                        new ApiResponse<GetListUserWinnerResponse>
                        {
                            Code = 404,
                            Message = Message.NOT_FOUND_LIST_USER_WINNER,
                            Data = null,
                        }
                    );
                }

                return Ok(
                    new ApiResponse<GetListUserWinnerResponse>
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
                    new ApiResponse<GetListUserWinnerResponse>
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
