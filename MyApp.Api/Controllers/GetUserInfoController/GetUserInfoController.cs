using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Common.Message;
using MyApp.Application.Common.Response;
using MyApp.Application.CQRS.GetUserInfo.Queries;

namespace MyApp.Api.Controllers.GetUserInfoController
{
    public class GetUserInfoController(IMediator _mediator) : ControllerBase
    {
        [HttpGet]
        [Route("api/UserInfo/{user_id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<GetUserInfoResponse>>> GetUserInfo(
            [FromRoute] Guid user_id
        )
        {
            try
            {
                var request = new GetUserInfoQuery { UserId = user_id };
                var response = await _mediator.Send(request);
                if (response == null)
                {
                    return new ApiResponse<GetUserInfoResponse>
                    {
                        Code = 404,
                        Message = Message.HANDLER_FAILED,
                        Data = null,
                    };
                }

                return Ok(
                    new ApiResponse<GetUserInfoResponse>
                    {
                        Code = 200,
                        Message = Message.GET_USER_INFO_SUCCESS,
                        Data = response,
                    }
                );
            }
            catch (Exception ex)
            {
                return new ApiResponse<GetUserInfoResponse>
                {
                    Code = 500,
                    Message = Message.HANDLER_ERROR + ex.Message,
                    Data = null,
                };
            }
        }
    }
}
