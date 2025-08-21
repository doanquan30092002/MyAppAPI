using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Common.Services.ExportWord.ExportAuctionBook;
using MyApp.Application.Common.Services.NotificationHub;

namespace MyApp.Api.Controllers.TestRealTimeController
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignalRTestController : ControllerBase
    {
        private readonly INotificationSender _notificationSender;
        private readonly IAuctionBookExporter _bookExporter;

        public SignalRTestController(
            INotificationSender notificationSender,
            IAuctionBookExporter bookExporter
        )
        {
            _notificationSender = notificationSender;
            _bookExporter = bookExporter;
        }

        // POST: api/signalrtest/send
        [HttpPost("send")]
        public async Task<IActionResult> Send([FromBody] SignalRTestRequest request)
        {
            if (request == null || request.UserIds == null || request.UserIds.Count == 0)
                return BadRequest("UserIds is required.");

            await _notificationSender.SendToUsersAsync(
                request.UserIds,
                new
                {
                    Title = request.Title,
                    Content = request.Content,
                    Time = DateTime.UtcNow,
                }
            );

            return Ok(new { Success = true });
        }
    }

    public class SignalRTestRequest
    {
        public List<Guid> UserIds { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
}
