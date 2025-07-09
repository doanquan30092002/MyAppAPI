using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Common.Message;
using MyApp.Application.CQRS.GenarateNumbericalOrder;
using MyApp.Application.CQRS.UpdateAccount.Command.SendUpdateOtp;

namespace MyApp.Api.Controllers.GenarateNumbericalOrder
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Staff")]
    public class GenarateNumbericalOrderController(IMediator _mediator) : ControllerBase
    {
        [HttpPost]
        [Route("Genarate-Numberical-Order")]
        public async Task<ActionResult> GenarateNumbericalOrder(
            [FromBody] GenarateNumbericalOrderRequest request
        )
        {
            var response = await _mediator.Send(request);
            return Ok(
                new
                {
                    Code = response ? 200 : 400,
                    Message = response
                        ? Message.GENARATE_NUMBERICAL_ORDER_SUCCESS
                        : Message.GENARATE_NUMBERICAL_ORDER_FAIL,
                }
            );
        }
    }
}
