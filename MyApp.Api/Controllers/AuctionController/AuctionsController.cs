using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Common.Response;
using MyApp.Application.CQRS.Auction.AddAuction.Commands;
using MyApp.Application.CQRS.Auction.UpdateAuction.Commands;

namespace MyApp.Api.Controllers.AuctionController
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuctionsController(IMediator _mediator) : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateAuction([FromForm] AddAuctionCommand command)
        {
            var auctionId = await _mediator.Send(command);
            var response = new ApiResponse<Guid>
            {
                Code = 200,
                Message = "Tạo phiên đấu giá thành công",
                Data = auctionId,
            };
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPut]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateAuction([FromForm] UpdateAuctionCommand command)
        {
            var updatedAuctionId = await _mediator.Send(command);

            var response = new ApiResponse<Guid>
            {
                Code = 200,
                Message = "Cập nhật phiên đấu giá thành công",
                Data = updatedAuctionId,
            };
            return Ok(response);
        }
    }
}
