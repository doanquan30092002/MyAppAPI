using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Common.Message;
using MyApp.Application.Common.Response;
using MyApp.Application.CQRS.SearchUserAttendance.Queries;

namespace MyApp.Api.Controllers.SearchUserAttendanceController
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchUserAttendanceController(IMediator _mediator) : ControllerBase
    {
        [HttpGet]
        [Route("Search-User-Attendance")]
        [AllowAnonymous]
        public async Task<ActionResult<SearchUserAttendanceResponse>> SearchUserAttendance(
            [FromQuery] Guid auctionId,
            string citizenIdentification
        )
        {
            var searchUserAttendanceRequest = new SearchUserAttendanceRequest
            {
                AuctionId = auctionId,
                CitizenIdentification = citizenIdentification,
            };
            var response = await _mediator.Send(searchUserAttendanceRequest);
            if (
                response.Message.Equals(Message.NOT_FOUND_NUMERICAL_ORDER)
                || response.Message.Equals(Message.AUCTION_NOT_EXIST)
            )
            {
                return NotFound(
                    new ApiResponse<SearchUserAttendanceResponse>
                    {
                        Code = 404,
                        Message = response.Message,
                        Data = null,
                    }
                );
            }
            return Ok(
                new ApiResponse<SearchUserAttendanceResponse>
                {
                    Code = 200,
                    Message = response.Message,
                    Data = new SearchUserAttendanceResponse
                    {
                        AuctionName = response.AuctionName,
                        NumericalOrder = response.NumericalOrder,
                    },
                }
            );
        }
    }
}
