using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Common.Response;
using MyApp.Application.CQRS.Notifications.GetNofiticationByUserId.Queries;
using MyApp.Application.CQRS.Notifications.GetNotificationById.Queries;
using MyApp.Application.CQRS.Notifications.GetNotificationByUserId.Queries;
using MyApp.Application.CQRS.Notifications.HasUnread.Command;
using MyApp.Application.CQRS.Notifications.MarkAllAsRead;
using MyApp.Application.CQRS.Notifications.MarkAsRead.Commands;

namespace MyApp.Api.Controllers.NotificationsController
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController(IMediator _mediator) : ControllerBase
    {
        /// <summary>
        /// Lấy danh sách notification theo UserId (hỗ trợ phân trang)
        /// </summary>
        /// <param name="userId">UserId (Guid)</param>
        /// <param name="pageIndex">Số trang (bắt đầu từ 1)</param>
        /// <param name="pageSize">Số lượng bản ghi mỗi trang</param>
        /// <returns>ApiResponse với dữ liệu notifications</returns>
        [HttpGet]
        public async Task<IActionResult> GetByUserId(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10
        )
        {
            var request = new GetNotificationsByUserIdRequest
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
            };

            var result = await _mediator.Send(request);

            return Ok(
                new ApiResponse<GetNotificationsByUserIdResponse>
                {
                    Code = 200,
                    Message = "Lấy danh sách thông báo thành công",
                    Data = result,
                }
            );
        }

        /// <summary>
        /// Lấy chi tiết notification theo Id (chỉ user sở hữu mới xem)
        /// </summary>
        [HttpGet("{notificationId:guid}")]
        public async Task<IActionResult> GetById(Guid notificationId)
        {
            var request = new GetNotificationsByIdRequest { NotificationId = notificationId };

            var result = await _mediator.Send(request);

            return Ok(
                new ApiResponse<NotificationDto>
                {
                    Code = 200,
                    Message = "Lấy thông báo thành công",
                    Data = result,
                }
            );
        }

        /// <summary>
        /// Đánh dấu đã đọc 1 notification
        /// </summary>
        [HttpPut("{notificationId:guid}/mark-as-read")]
        public async Task<IActionResult> MarkAsRead(Guid notificationId)
        {
            var command = new MarkAsReadRequest(notificationId);
            var result = await _mediator.Send(command);

            if (!result)
                return NotFound(
                    new ApiResponse<object>
                    {
                        Code = 404,
                        Message = "Không tìm thấy thông báo hoặc bạn không có quyền",
                        Data = null,
                    }
                );

            return Ok(
                new ApiResponse<object>
                {
                    Code = 200,
                    Message = "Đã đánh dấu đã đọc",
                    Data = null,
                }
            );
        }

        [HttpPut("mark-all-as-read")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var command = new MarkAllAsReadRequest();
            var updatedCount = await _mediator.Send(command);

            return Ok(
                new ApiResponse<object>
                {
                    Code = 200,
                    Message = $"Đã đánh dấu {updatedCount} thông báo đã đọc",
                    Data = null,
                }
            );
        }

        /// <summary>
        /// Kiểm tra user hiện tại có thông báo chưa đọc không
        /// </summary>
        [HttpGet("has-unread")]
        public async Task<IActionResult> HasUnread()
        {
            var query = new HasUnreadCommand();
            var hasUnread = await _mediator.Send(query);

            return Ok(
                new ApiResponse<HasUnreadNotificationResponse>
                {
                    Code = 200,
                    Message = "Kiểm tra thành công",
                    Data = new HasUnreadNotificationResponse { HasUnread = hasUnread },
                }
            );
        }
    }
}
