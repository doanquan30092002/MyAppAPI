using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Common.Response;
using MyApp.Application.CQRS.Auction.AddAuction.Commands;
using MyApp.Application.CQRS.Auction.UpdateAuction.Commands;
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
    }
}
