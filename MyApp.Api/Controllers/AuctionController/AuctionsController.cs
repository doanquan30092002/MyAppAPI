using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Common.Response;
using MyApp.Application.CQRS.Auction.AddAuction.Commands;
using MyApp.Application.CQRS.Auction.CancelAuction.Commands;
using MyApp.Application.CQRS.Auction.MarkAuctionAsSuccessful.Command;
using MyApp.Application.CQRS.Auction.RejectAuction;
using MyApp.Application.CQRS.Auction.UpdateAuction.Commands;
using MyApp.Application.CQRS.Auction.WaitingPublic.Commands;
using MyApp.Core.DTOs.AuctionDTO;

namespace MyApp.Api.Controllers.AuctionController
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuctionsController(IMediator _mediator) : ControllerBase
    {
        [Authorize(Roles = "Staff")]
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateAuction([FromForm] AddAuctionCommand command)
        {
            var auctionId = await _mediator.Send(command);
            var response = new ApiResponse<AuctionResponse>
            {
                Code = 200,
                Message = "Tạo phiên đấu giá thành công",
                Data = new AuctionResponse { AuctionId = auctionId },
            };
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPut]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateAuction([FromForm] UpdateAuctionCommand command)
        {
            var updatedAuctionId = await _mediator.Send(command);

            var response = new ApiResponse<AuctionResponse>
            {
                Code = 200,
                Message = "Cập nhật phiên đấu giá thành công",
                Data = new AuctionResponse { AuctionId = updatedAuctionId },
            };
            return Ok(response);
        }

        [Authorize(Roles = "Staff")]
        [HttpPut("cancel")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CancelAuction([FromForm] CancelAuctionCommand command)
        {
            var result = await _mediator.Send(command);

            var response = new ApiResponse<object>
            {
                Code = result ? 200 : 400,
                Message = result ? "Hủy phiên đấu giá thành công" : "Hủy phiên đấu giá thất bại",
                Data = null,
            };
            return Ok(response);
        }

        [Authorize(Roles = "Staff")]
        [HttpPut("waiting-public/{auctionId}")]
        public async Task<IActionResult> WaitingPublic(Guid auctionId)
        {
            var command = new WaitingPublicCommand { AuctionId = auctionId };

            var result = await _mediator.Send(command);

            var response = new ApiResponse<Object>
            {
                Code = 200,
                Message = "Chuyển trạng thái phiên đấu giá sang 'Chờ công bố' thành công",
                Data = new { AuctionId = auctionId, Status = result },
            };

            return Ok(response);
        }

        [Authorize(Roles = "Manager")]
        [HttpPut("reject-auction")]
        public async Task<IActionResult> RejectAuction([FromBody] RejectAuction rejectAuction)
        {
            var result = await _mediator.Send(rejectAuction);

            var response = new ApiResponse<Object>
            {
                Code = 200,
                Message = "Từ chối phiên đấu giá thành công",
                Data = new { AuctionId = rejectAuction.AuctionId, Status = result },
            };

            return Ok(response);
        }

        [Authorize(Roles = "Auctioneer")]
        [HttpPut("mark-successful")]
        public async Task<IActionResult> MarkAuctionAsSuccessful(
            [FromBody] MarkAuctionAsSuccessfulCommand command
        )
        {
            var result = await _mediator.Send(command);

            var response = new ApiResponse<object>
            {
                Code = 200,
                Message = "Đánh dấu phiên đấu giá thành công",

                Data = new { AuctionId = command.AuctionId, Status = result },
            };

            return Ok(response);
        }
    }
}
