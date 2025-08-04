using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Common.Response;
using MyApp.Application.CQRS.ExportAuctionBook.Queries;

namespace MyApp.Api.Controllers.AuctionBookController
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuctionBookController(IMediator _mediator) : ControllerBase
    {
        [Authorize(Roles = "Staff")]
        [HttpPost("export-book")]
        public async Task<IActionResult> ExportAuctionBook(
            [FromForm] GetAuctionBookByAuctionIdCommand command
        )
        {
            var fileBytes = await _mediator.Send(command);

            var base64 = Convert.ToBase64String(fileBytes);
            var fileName = $"SoDangKyDauGia_{command.AuctionId}.docx";

            var response = new ApiResponse<object>
            {
                Code = 200,
                Message = "Xuất file Word thành công",
                Data = new
                {
                    fileName,
                    contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    base64,
                },
            };

            return Ok(response);

            //return File(
            //    fileBytes,
            //    "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            //    "SoDangKyDauGia.docx"
            //);
        }
    }
}
