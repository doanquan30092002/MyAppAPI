using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Common.Message;
using MyApp.Application.Common.Response;
using MyApp.Application.CQRS.GetBusinessOverview.Queries;

namespace MyApp.Api.Controllers.GetBusinessOverview
{
    [Route("api")]
    [ApiController]
    public class GetBusinessOverviewController : ControllerBase
    {
        private readonly IMediator _mediator;

        public GetBusinessOverviewController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet]
        [Route("GetBusinessOverview")]
        [AllowAnonymous]
        public async Task<
            ActionResult<ApiResponse<GetBusinessOverviewResponse>>
        > GetBusinessOverview([FromQuery] GetBusinessOverviewRequest getBusinessOverviewRequest)
        {
            try
            {
                var response = await _mediator.Send(getBusinessOverviewRequest);
                if (response == null)
                {
                    return NotFound(
                        new ApiResponse<GetBusinessOverviewResponse>
                        {
                            Code = 404,
                            Message = Message.NOT_FOUND,
                            Data = null,
                        }
                    );
                }

                return Ok(
                    new ApiResponse<GetBusinessOverviewResponse>
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
                    new ApiResponse<GetBusinessOverviewResponse>
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
