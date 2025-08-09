using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Common.Message;
using MyApp.Application.Common.Response;
using MyApp.Application.CQRS.ListAuctionAsset;

namespace MyApp.Api.Controllers.ListAuctionAsset
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ListAuctionAssetController(IMediator _mediator) : ControllerBase
    {
        [HttpGet]
        [Route("List-Auction-Asset")]
        public async Task<ActionResult> ListAuctionAsset(
            [FromQuery] ListAuctionAssetRequest request
        )
        {
            var response = await _mediator.Send(request);
            if (!response.AuctionAssetResponses.Any())
            {
                return Ok(
                    new ApiResponse<ListAuctionAssetResponse>
                    {
                        Code = 404,
                        Message = Message.SEARCH_NOT_FOUND,
                        Data = new ListAuctionAssetResponse
                        {
                            PageNumber = request.PageNumber,
                            PageSize = request.PageSize,
                            TotalAuctionAsset = response.TotalAuctionAsset,
                            AuctionAssetResponses = new List<AuctionAssetResponse>(),
                            CategoryCounts = response.CategoryCounts,
                        },
                    }
                );
            }
            return Ok(
                new ApiResponse<ListAuctionAssetResponse>
                {
                    Code = response.TotalAuctionAsset > 0 ? 200 : 404,
                    Message =
                        response.TotalAuctionAsset > 0
                            ? Message.SEARCH_SUCCESS
                            : Message.SEARCH_NOT_FOUND,
                    Data = response.TotalAuctionAsset > 0 ? response : null,
                }
            );
        }
    }
}
