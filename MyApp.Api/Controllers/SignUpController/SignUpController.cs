using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.CQRS.SignUp.Command;

namespace MyApp.Api.Controllers.SignUpController
{
    [Route("api/")]
    [ApiController]
    public class SignUpController(IMediator _mediator) : ControllerBase
    {
        [HttpPost]
        [Route("SignUp")]
        [AllowAnonymous]
        public async Task<ActionResult<SignUpResponse>> Login(
            [FromBody] SignUpRequest signupRequest
        )
        {
            var response = await _mediator.Send(signupRequest);
            return Ok(response);
        }
    }
}
