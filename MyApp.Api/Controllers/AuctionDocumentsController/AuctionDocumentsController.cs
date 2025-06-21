using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Common.Response;
using MyApp.Application.CQRS.AuctionDocuments.SupportRegisterDocuments.Command;
using MyApp.Application.CQRS.AuctionDocuments.SupportRegisterDocuments.Queries;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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
        [Authorize(Roles = "Staff")]
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

        /// <summary>
        /// Xuất hồ sơ đấu giá ra file Word theo AuctionId và số căn cước công dân, cho phép upload template.
        /// </summary>
        /// <param name="auctionId">Id phiên đấu giá</param>
        /// <param name="citizenIdentification">Số căn cước công dân/CMND</param>
        /// <param name="templateFile">File template Word (tùy chọn)</param>
        /// <returns>File Word xuất ra</returns>
        [HttpPost("export-word")]
        [Consumes("multipart/form-data")]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> ExportWordAuctionDocuments(
            [FromForm] ExportWordAuctionDocumentCommand command
        )
        {
            try
            {
                var fileBytes = await _mediator.Send(command);
                var fileName = $"ho-so-dau-gia-{command.AuctionDocumentId}.docx";
                var base64 = Convert.ToBase64String(fileBytes);

                var response = new ApiResponse<object>
                {
                    Code = 200,
                    Message = "Xuất file thành công",
                    Data = new
                    {
                        FileName = fileName,
                        ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                        Base64 = base64,
                    },
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<object>
                {
                    Code = 400,
                    Message = "Lỗi xuất file: Hãy kiểm tra định dạng file theo tiêu chuẩn format",
                    Data = null,
                };
                return StatusCode(500, errorResponse);
            }
        }
    }
}
