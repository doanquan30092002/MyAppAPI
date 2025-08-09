using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Common.Message;
using MyApp.Application.Common.Response;
using MyApp.Application.CQRS.EmployeeManager.AssignPermissionUser;
using MyApp.Application.CQRS.EmployeeManager.ChangeStatusEmployeeAccount;
using MyApp.Application.CQRS.EmployeeManager.ListEmployeeAccount;

namespace MyApp.Api.Controllers.EmployeeManager
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeeManagerController(IMediator _mediator) : ControllerBase
    {
        [HttpGet]
        [Route("List-Employee-Account")]
        public async Task<ActionResult> ListEmployeeAccount(
            [FromQuery] ListEmployeeAccountRequest request
        )
        {
            var response = await _mediator.Send(request);
            if (response.EmployeeAccounts.Count == 0 || !response.EmployeeAccounts.Any())
            {
                return Ok(
                    new ApiResponse<ListEmployeeAccountResponse>
                    {
                        Code = 404,
                        Message = Message.GET_LIST_EMPLOYEE_ACCOUNT_NOT_FOUND,
                        Data = null,
                    }
                );
            }
            return Ok(
                new ApiResponse<ListEmployeeAccountResponse>
                {
                    Code = 200,
                    Message = Message.GET_LIST_EMPLOYEE_ACCOUNT_SUCCESS,
                    Data = response,
                }
            );
        }

        [HttpPost]
        [Route("Change-Status-Employee-Account")]
        public async Task<ActionResult> ChangeStatusEmployeeAccount(
            [FromBody] ChangeStatusEmployeeAccountRequest request
        )
        {
            var response = await _mediator.Send(request);
            if (!response)
            {
                return Ok(
                    new ApiResponse<bool>
                    {
                        Code = 400,
                        Message = Message.CHANGE_STATUS_EMPLOYEE_ACCOUNT_FAIL,
                        Data = false,
                    }
                );
            }

            return Ok(
                new ApiResponse<bool>
                {
                    Code = 200,
                    Message = Message.CHANGE_STATUS_EMPLOYEE_ACCOUNT_SUCCESS,
                    Data = true,
                }
            );
        }

        [HttpPost]
        [Route("Assign-Permission-User")]
        public async Task<ActionResult> AssignPermissionUser(
            [FromBody] AssignPermissionUserRequest request
        )
        {
            var response = await _mediator.Send(request);
            if (!response)
            {
                return Ok(
                    new ApiResponse<bool>
                    {
                        Code = 400,
                        Message = Message.CHANGE_PERMISSTION_EMPLOYEE_ACCOUNT_FAIL,
                        Data = false,
                    }
                );
            }

            return Ok(
                new ApiResponse<bool>
                {
                    Code = 200,
                    Message = Message.CHANGE_PERMISSTION_EMPLOYEE_ACCOUNT_SUCCESS,
                    Data = true,
                }
            );
        }
    }
}
