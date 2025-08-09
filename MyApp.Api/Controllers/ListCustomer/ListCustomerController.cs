using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Common.Message;
using MyApp.Application.Common.Response;
using MyApp.Application.CQRS.ListCustomer;

namespace MyApp.Api.Controllers.ListCustomer
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListCustomerController(IMediator _mediator) : ControllerBase
    {
        [HttpGet]
        [Route("List-Customer")]
        [Authorize]
        public async Task<ActionResult> ListCustomer([FromQuery] ListCustomerRequest request)
        {
            var response = await _mediator.Send(request);
            if (response.TotalCount == 0)
            {
                return Ok(
                    new ApiResponse<ListCustomerResponse>
                    {
                        Code = 404,
                        Message = Message.GET_LIST_CUSTOMER_NOT_FOUND,
                        Data = null,
                    }
                );
            }
            return Ok(
                new ApiResponse<ListCustomerResponse>
                {
                    Code = 200,
                    Message = Message.GET_LIST_CUSTOMER_SUCCESS,
                    Data = response,
                }
            );
        }
    }
}
