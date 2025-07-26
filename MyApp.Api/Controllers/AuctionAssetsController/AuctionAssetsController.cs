using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Common.Response;
using MyApp.Application.CQRS.AuctionAssetsV2.GetAuctionAssetsHighestBid.Queries;
using MyApp.Core.DTOs.AuctionAssetsDTO;

namespace MyApp.Api.Controllers.AuctionAssetsController
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuctionAssetsController(IMediator _mediator) : ControllerBase
    {
        [HttpGet("highest-bids/{auctionId}")]
        public async Task<IActionResult> GetAssetsWithHighestBid([FromRoute] Guid auctionId)
        {
            var query = new GetAssetsHighestBidRequest { AuctionId = auctionId };
            var result = await _mediator.Send(query);

            var response = new ApiResponse<List<AuctionAssetsWithHighestBidResponse>>
            {
                Code = 200,
                Message = "Lấy danh sách tài sản kèm giá cao nhất thành công",
                Data = result,
            };

            return Ok(response);
        }
    }
}
