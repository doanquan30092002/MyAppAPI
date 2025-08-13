using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Common.Message;
using MyApp.Application.Common.Response;
using MyApp.Application.CQRS.GetStatisticOverview.Queries;

namespace MyApp.Api.Controllers.GetStatisticOverviewController
{
    [Route("api")]
    [ApiController]
    public class GetStatisticOverviewController(IMediator _mediator) : ControllerBase
    {
        [HttpGet]
        [Route("GetStatisticOverview")]
        [AllowAnonymous]
        public async Task<
            ActionResult<ApiResponse<GetStatisticOverviewResponse>>
        > GetStatisticOverview([FromQuery] GetStatisticOverviewRequest request)
        {
            try
            {
                var response = await _mediator.Send(request);

                if (response == null)
                {
                    return NotFound(
                        new ApiResponse<GetStatisticOverviewResponse>
                        {
                            Code = 404,
                            Message = Message.NOT_FOUND,
                            Data = null,
                        }
                    );
                }

                return Ok(
                    new ApiResponse<GetStatisticOverviewResponse>
                    {
                        Code = 200,
                        Message = Message.HANDLER_SUCCESS,
                        Data = response,
                    }
                );
            }
            catch (Exception ex)
            {
                // You may log ex here for debugging purposes
                return StatusCode(
                    500,
                    new ApiResponse<GetStatisticOverviewResponse>
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
