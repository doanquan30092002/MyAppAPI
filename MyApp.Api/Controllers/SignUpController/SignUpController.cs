using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Common.Message;
using MyApp.Application.Common.Response;
using MyApp.Application.CQRS.SignUp.SignUpUser.Command;

namespace MyApp.Api.Controllers.SignUpController
{
    [Route("api/")]
    [ApiController]
    public class SignUpController(IMediator _mediator) : ControllerBase
    {
        [HttpPost]
        [Route("SignUp")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<SignUpResponse>>> Login(
            [FromBody] SignUpRequest signupRequest
        )
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(
                    new ApiResponse<SignUpResponse>
                    {
                        Code = 400,
                        Message = Message.VALIDATION_FAILED,
                        Data = null,
                    }
                );
            }

            var response = await _mediator.Send(signupRequest);
            return Ok(
                new ApiResponse<SignUpResponse>
                {
                    Code = 200,
                    Message = Message.CREATE_SUCCESS,
                    Data = response,
                }
            );
        }
    }
}
