using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Common.Message;
using MyApp.Application.Common.Response;
using MyApp.Application.CQRS.GetListBidders.Queries;

namespace MyApp.Api.Controllers.GetListBiddersController
{
    [Route("api")]
    [ApiController]
    public class GetListBiddersController(IMediator _mediator) : ControllerBase
    {
        [HttpGet]
        [Route("GetListBidders/{auctionId}")]
        public async Task<ActionResult<ApiResponse<GetListBiddersResponse>>> GetListBidders(
            [FromRoute] Guid auctionId
        )
        {
            try
            {
                var request = new GetListBiddersRequest { AuctionId = auctionId };

                var response = await _mediator.Send(request);
                if (response == null || !response.ListBidders.Any())
                {
                    return NotFound(
                        new ApiResponse<GetListBiddersResponse>
                        {
                            Code = 404,
                            Message = Message.NOT_FOUND,
                            Data = null,
                        }
                    );
                }

                return Ok(
                    new ApiResponse<GetListBiddersResponse>
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
                    new ApiResponse<GetListBiddersResponse>
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
