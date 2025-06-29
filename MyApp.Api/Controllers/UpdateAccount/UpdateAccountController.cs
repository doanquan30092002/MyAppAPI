using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Common.Message;
using MyApp.Application.CQRS.UpdateAccount.Command.SendUpdateOtp;
using MyApp.Application.CQRS.UpdateAccount.Command.VerifyAndUpdate;

namespace MyApp.Api.Controllers.EditAccountAndProfile
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UpdateAccountController(IMediator _mediator) : ControllerBase
    {
        [HttpPost]
        [Route("Send-Update-Otp")]
        public async Task<ActionResult> SendUpdateOtp([FromBody] UpdateAccountRequest request)
        {
            var response = await _mediator.Send(request);
            return Ok(
                new
                {
                    Success = response ? 200 : 400,
                    Message = response ? Message.SEND_OTP_SUCCESS : Message.SEND_OTP_FAIL,
                }
            );
        }

        [HttpPost]
        [Route("Verify-Otp-And-Update")]
        public async Task<ActionResult<UpdateAccountResponse>> VerifyAndUpdate(
            [FromBody] VerifyAndUpdateRequest request
        )
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }
    }
}
