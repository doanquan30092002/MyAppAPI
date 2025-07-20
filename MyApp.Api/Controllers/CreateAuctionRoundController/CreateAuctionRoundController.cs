using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Common.Message;
using MyApp.Application.Common.Response;
using MyApp.Application.CQRS.CreateAutionRound.Command;

namespace MyApp.Api.Controllers.CreateAuctionRoundController
{
    [Route("api")]
    [ApiController]
    public class CreateAuctionRoundController(IMediator _mediator) : ControllerBase
    {
        [HttpPost]
        [Route("CreateAuctionRound")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<CreateAuctionRoundResponse>>> Create(
            [FromBody] CreateAuctionRoundRequest request
        )
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(
                    new ApiResponse<CreateAuctionRoundResponse>
                    {
                        Code = 400,
                        Message = Message.CREATE_AUCTION_ROUND_FAIL,
                        Data = null,
                    }
                );
            }

            var response = await _mediator.Send(request);
            return Ok(
                new ApiResponse<CreateAuctionRoundResponse>
                {
                    Code = 200,
                    Message = Message.CREATE_AUCTION_ROUND_SUCCESS,
                    Data = response,
                }
            );
        }
    }
}
