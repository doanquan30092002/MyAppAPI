using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.CQRS.LoginUser.Queries;

namespace MyApp.Api.Controllers.LoginUserController
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginUserController(IMediator _mediator) : ControllerBase
    {
        [HttpPost]
        [Route("Login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginUserResponse>> Login(
            [FromBody] LoginUserRequest loginRequest
        )
        {
            var response = await _mediator.Send(loginRequest);
            return Ok(response);
        }
    }
}
