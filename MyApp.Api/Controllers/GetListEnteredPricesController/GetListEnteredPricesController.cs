using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Common.Message;
using MyApp.Application.Common.Response;
using MyApp.Application.CQRS.GetListEnteredPrices.Queries;
using MyApp.Application.CQRS.GetListEnteredPrices.Querries;

namespace MyApp.Api.Controllers.GetListEnteredPricesController
{
    [Route("api")]
    [ApiController]
    public class GetListEnteredPricesController(IMediator _mediator) : ControllerBase
    {
        [HttpGet]
        [Route("ListEnteredPrices/{auctionRoundId}")]
        [AllowAnonymous]
        public async Task<
            ActionResult<ApiResponse<GetListEnteredPricesResponse>>
        > GetListEnteredPrices([FromRoute] Guid auctionRoundId)
        {
            try
            {
                var request = new GetListEnteredPricesRequest { AuctionRoundId = auctionRoundId };
                var response = await _mediator.Send(request);

                if (
                    response == null
                    || response.ListAuctionRoundPrices == null
                    || !response.ListAuctionRoundPrices.Any()
                )
                {
                    return NotFound(
                        new ApiResponse<GetListEnteredPricesResponse>
                        {
                            Code = 404,
                            Message = Message.NOT_FOUND_LIST_ENTERED_PRICES,
                            Data = null,
                        }
                    );
                }

                return Ok(
                    new ApiResponse<GetListEnteredPricesResponse>
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
                    new ApiResponse<GetListEnteredPricesResponse>
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
