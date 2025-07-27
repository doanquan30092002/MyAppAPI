using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Common.Message;
using MyApp.Application.Common.Response;
using MyApp.Application.CQRS.UpdateFlagWinner.Command;

namespace MyApp.Api.Controllers.UpdateWinnerFlagController
{
    [Route("api")]
    [ApiController]
    [Authorize]
    public class UpdateWinnerFlagController(IMediator _mediator) : ControllerBase
    {
        [HttpPost]
        [Route("UpdateWinnerFlag")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<UpdateWinnerFlagResponse>>> UpdateWinnerFlag(
            [FromBody] UpdateWinnerFlagRequest request
        )
        {
            try
            {
                var response = await _mediator.Send(request);

                if (response == null || !response.StatusUpdate)
                {
                    return NotFound(
                        new ApiResponse<UpdateWinnerFlagResponse>
                        {
                            Code = 404,
                            Message = Message.NOT_FOUND_ROUND_PRICES_ID_TO_UPDATE,
                            Data = null,
                        }
                    );
                }

                return Ok(
                    new ApiResponse<UpdateWinnerFlagResponse>
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
                    new ApiResponse<UpdateWinnerFlagResponse>
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
