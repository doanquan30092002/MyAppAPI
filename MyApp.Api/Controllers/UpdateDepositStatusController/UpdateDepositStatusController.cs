using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Common.Message;
using MyApp.Application.Common.Response;
using MyApp.Application.CQRS.Auction.GetListAuction.Querries;
using MyApp.Application.CQRS.UpdateStatusDeposit.Command;

namespace MyApp.Api.Controllers.UpdateDepositStatusController
{
    [Route("api")]
    [ApiController]
    public class UpdateDepositStatusController(IMediator _mediator) : ControllerBase
    {
        [HttpPost]
        [Route("UpdateDeposit/{auction_id}/{auction_document_id}")]
        public async Task<
            ActionResult<ApiResponse<UpdateDepositStatusResponse>>
        > UpdateDepositStatus(
            [FromRoute] Guid auction_document_id,
            [FromRoute] Guid auction_id,
            [FromBody] UpdateDepositStatusRequest request
        )
        {
            // Gán auction_document_id từ path variable vào request
            request.AuctionDocumentsId = auction_document_id;
            request.AuctionId = auction_id;
            try
            {
                // Gửi yêu cầu qua MediatR
                var response = await _mediator.Send(request);

                return Ok(
                    new ApiResponse<UpdateDepositStatusResponse>
                    {
                        Code = 200,
                        Message = Message.UPDATE_DEPOSIT_STATUS_SUCESS,
                        Data = response,
                    }
                );
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new ApiResponse<UpdateDepositStatusResponse>
                    {
                        Code = 500,
                        Message = Message.UPDATE_DEPOSIT_STATUS_FAIL,
                        Data = null,
                    }
                );
            }
        }
    }
}
