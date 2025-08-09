using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Common.Message;
using MyApp.Application.CQRS.ReceiveAuctionRegistrationForm;

namespace MyApp.Api.Controllers.ReceiveAuctionRegistrationForm
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Staff")]
    public class ReceiveAuctionRegistrationFormController(IMediator _mediator) : ControllerBase
    {
        [HttpPost]
        [Route("Receive-Auction-Registration-Form")]
        public async Task<ActionResult> ReceiveAuctionRegistrationForm(
            [FromBody] ReceiveAuctionRegistrationFormRequest request
        )
        {
            var response = await _mediator.Send(request);
            return Ok(
                new
                {
                    Code = response ? 200 : 400,
                    Message = response
                        ? Message.RECEIVE_AUCTION_REGISTRATION_FORM_SUCCESS
                        : Message.RECEIVE_AUCTION_REGISTRATION_FORM_FAIL,
                }
            );
        }
    }
}
