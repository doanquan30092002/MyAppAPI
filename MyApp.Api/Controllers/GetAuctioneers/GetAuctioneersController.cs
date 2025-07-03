using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Common.Message;
using MyApp.Application.CQRS.GetAuctioneers.Queries;

namespace MyApp.Api.Controllers.GetAuctioneers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GetAuctioneersController(IMediator _mediator) : ControllerBase
    {
        [HttpGet]
        [Route("Get-Auctioneers")]
        public async Task<ActionResult> GetAuctioneers()
        {
            GetAuctioneersRequest request = new GetAuctioneersRequest();
            var response = await _mediator.Send(request);
            if (response == null || response.Count == 0)
            {
                return Ok(new { Code = 404, Message = Message.NOT_FOUND_AUCTIONEER });
            }
            return Ok(
                new
                {
                    Code = 200,
                    Message = Message.GET_AUCTIONEER_SUCCESS,
                    Data = response,
                }
            );
        }
    }
}
