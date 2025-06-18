using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Common.Response;
using MyApp.Application.CQRS.SignUp.GetRole.Queries;
using MyApp.Core.Entities;

namespace MyApp.Api.Controllers.SignUpController
{
    [Route("api/")]
    [ApiController]
    public class GetRoleController(IMediator _mediator) : ControllerBase
    {
        [HttpGet]
        [Route("GetRoles")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<List<Role>>>> GetRoles()
        {
            var roles = await _mediator.Send(new GetRoleQuery());
            if (roles == null || !roles.Any())
            {
                return NotFound(
                    new ApiResponse<List<Role>>
                    {
                        Code = 404,
                        Message = "No roles found.",
                        Data = null,
                    }
                );
            }
            return Ok(
                new ApiResponse<List<Role>>
                {
                    Code = 200,
                    Message = "Roles retrieved successfully.",
                    Data = roles,
                }
            );
        }
    }
}
