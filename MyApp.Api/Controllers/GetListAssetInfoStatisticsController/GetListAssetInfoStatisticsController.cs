using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Common.Response;
using MyApp.Application.CQRS.GetListAssetInfoStatistic.Queries;

namespace MyApp.Api.Controllers.GetListAssetInfoStatisticsController
{
    [Route("api")]
    [ApiController]
    public class GetListAssetInfoStatisticsController(IMediator _mediator) : ControllerBase
    {
        [HttpGet]
        [Route("AuctionAssetStatistics/{auction_assets_id}")]
        [AllowAnonymous]
        public async Task<
            ActionResult<ApiResponse<GetListAssetInfoStatisticsResponse>>
        > GetAuctionAssetStatistics(
            [FromRoute] Guid auction_assets_id,
            [FromQuery] GetListAssetInfostatisticsRequest getListAssetInfostatisticsRequest
        )
        {
            try
            {
                getListAssetInfostatisticsRequest.AuctionAssetsId = auction_assets_id;

                var response = await _mediator.Send(getListAssetInfostatisticsRequest);
                if (response == null)
                {
                    return NotFound(
                        new ApiResponse<GetListAssetInfoStatisticsResponse>
                        {
                            Code = 404,
                            Message = "Auction asset not found.",
                            Data = null,
                        }
                    );
                }

                return Ok(
                    new ApiResponse<GetListAssetInfoStatisticsResponse>
                    {
                        Code = 200,
                        Message = "Successfully retrieved auction asset statistics.",
                        Data = response,
                    }
                );
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new ApiResponse<GetListAssetInfoStatisticsResponse>
                    {
                        Code = 500,
                        Message = "An error occurred while processing the request.",
                        Data = null,
                    }
                );
            }
        }
    }
}
