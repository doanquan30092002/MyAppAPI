using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Common.Response;
using MyApp.Application.CQRS.AuctionDocuments.SupportRegisterDocuments.Command;
using MyApp.Application.CQRS.AuctionDocuments.SupportRegisterDocuments.Queries;

namespace MyApp.Api.Controllers.AuctionDocumentsController
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuctionDocumentsController(IMediator _mediator) : ControllerBase
    {
        [Authorize(Roles = "Staff")]
        [HttpPost("support-register")]
        public async Task<IActionResult> SupportRegisterDocuments(
            [FromBody] SupportRegisterDocumentsCommand command
        )
        {
            var result = await _mediator.Send(command);

            return Ok(
                new ApiResponse<string>
                {
                    Code = 200,
                    Message = "Đăng ký hồ sơ đấu giá thành công",
                    Data = null,
                }
            );
        }

        /// <summary>
        /// API tra cứu người dùng theo số CCCD/CMND.
        /// </summary>
        [Authorize(Roles = "Staff")]
        [HttpGet("user-by-citizen-identification")]
        public async Task<IActionResult> GetUserByCitizenIdentification(
            [FromQuery] string citizenIdentification
        )
        {
            var user = await _mediator.Send(
                new GetUserByCitizenIdentificationRequest(citizenIdentification)
            );
            if (user == null)
            {
                return NotFound(
                    new ApiResponse<object>
                    {
                        Code = 404,
                        Message = "Không tìm thấy người dùng với số CCCD/CMND này.",
                        Data = null,
                    }
                );
            }

            return Ok(
                new ApiResponse<object>
                {
                    Code = 200,
                    Message = "Lấy thông tin người dùng thành công.",
                    Data = user,
                }
            );
        }

        /// <summary>
        /// Cập nhật trạng thái vé và đặt cọc của hồ sơ đấu giá
        /// </summary>
        /// <param name="request">Thông tin cập nhật trạng thái</param>
        /// <returns>Trả về true nếu thành công</returns>
        [HttpPut("update-status")]
        public async Task<IActionResult> UpdateStatus(
            [FromBody] UpdateStatusAuctionDocumentsCommand request
        )
        {
            var result = await _mediator.Send(request);
            return Ok(
                new ApiResponse<object>
                {
                    Code = 200,
                    Message = "Cập nhật trạng thái thành công.",
                    Data = null,
                }
            );
        }
    }
}
