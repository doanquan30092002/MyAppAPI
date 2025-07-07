using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Common.Response;
using MyApp.Application.CQRS.Auction.GetListAuction.Querries;
using MyApp.Application.CQRS.Auction.GetListAuctionById.Querries;
using MyApp.Application.CQRS.Auction.GetListAution.Querries;

namespace MyApp.Api.Controllers.GetListAuctionController
{
    public class GetListAuctionController(IMediator _mediator) : ControllerBase
    {
        [HttpGet]
        [Route("api/List")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<GetListAuctionResponse>>> getListAuction(
            [FromQuery] GetListAuctionRequest getListAuctionRequest
        )
        {
            try
            {
                var response = await _mediator.Send(getListAuctionRequest);
                if (response == null)
                {
                    return NotFound(
                        new ApiResponse<GetAuctionByIdResponse>
                        {
                            Code = 400,
                            Message = "Không có phiên đấu giá nào được tìm thấy",
                            Data = null,
                        }
                    );
                }

                return Ok(
                    new ApiResponse<GetListAuctionResponse>
                    {
                        Code = 200,
                        Message = "Lấy danh sách phiên đấu giá thành công",
                        Data = response,
                    }
                );
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new ApiResponse<GetAuctionByIdResponse>
                    {
                        Code = 500,
                        Message = " Lỗi trong quá trình lấy danh sách phiên đấu giá",
                        Data = null,
                    }
                );
            }
        }
    }
}
