using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Common.Message;
using MyApp.Application.Common.Response;
using MyApp.Application.CQRS.DetailAuctionAsset;

namespace MyApp.Api.Controllers.DetailAuctionAsset
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DetailAuctionAssetController(IMediator _mediator) : ControllerBase
    {
        [HttpGet]
        [Route("Detail-Auction-Asset")]
        public async Task<ActionResult> DetailAuctionAsset(
            [FromQuery] DetailAuctionAssetRequest request
        )
        {
            var response = await _mediator.Send(request);
            if (response.AuctionAssetResponse == null)
            {
                return Ok(
                    new ApiResponse<DetailAuctionAssetResponse>
                    {
                        Code = 404,
                        Message = Message.SEARCH_AUCTION_ASSET_NOT_FOUND,
                        Data = null,
                    }
                );
            }
            return Ok(
                new ApiResponse<DetailAuctionAssetResponse>
                {
                    Code = 200,
                    Message = Message.SEARCH_AUCTION_ASSET_SUCCESS,
                    Data = response,
                }
            );
        }
    }
}
