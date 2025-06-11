using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.CQRS.SearchUserAttendance.Queries;

namespace MyApp.Api.Controllers.SearchUserAttendanceController
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchUserAttendanceController(IMediator _mediator) : ControllerBase
    {
        [HttpPost]
        [Route("Search-User-Attendance")]
        [AllowAnonymous]
        public async Task<ActionResult<SearchUserAttendanceResponse>> SearchUserAttendance(
            [FromQuery] Guid auctionId,
            [FromBody] string citizenIdentification
        )
        {
            if (auctionId == Guid.Empty || string.IsNullOrWhiteSpace(citizenIdentification))
            {
                return BadRequest("Invalid auction ID or citizen identification.");
            }
            var searchUserAttendanceRequest = new SearchUserAttendanceRequest
            {
                AuctionId = auctionId,
                CitizenIdentification = citizenIdentification,
            };
            var response = await _mediator.Send(searchUserAttendanceRequest);
            if (
                response.Message.Equals("Không tìm thấy số thứ tự.")
                || response.Message.Equals("Không tồn tại phiên đấu giá này.")
            )
            {
                return NotFound(response);
            }
            return Ok(response);
        }
    }
}
