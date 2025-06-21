using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Common.Response;
using MyApp.Application.Common.Utils;
using MyApp.Application.CQRS.PaymentDeposit.GetInforPaymentDeposit.Queries;
using MyApp.Application.CQRS.PaymentDeposit.WebHookPaymentDeposit;
using MyApp.Core.DTOs.AuctionDocumentsDTO;

namespace MyApp.Api.Controllers.PaymentDepositController
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentDepositController(IMediator _mediator) : ControllerBase
    {
        /// <summary>
        /// Lấy thông tin thanh toán đặt cọc theo AuctionDocumentsId
        /// </summary>
        [Authorize(Roles = "Customer")]
        [HttpGet("get-infor-payment-deposit/{auctionDocumentsId}")]
        public async Task<ActionResult<ApiResponse<InforPaymentDepositResponse>>> GetInfo(
            Guid auctionDocumentsId
        )
        {
            var result = await _mediator.Send(
                new GetInforPaymentDepositRequest { AuctionDocumentsId = auctionDocumentsId }
            );

            var response = new ApiResponse<InforPaymentDepositResponse>
            {
                Code = 200,
                Message = "Lấy thông tin thành công.",
                Data = result,
            };

            return Ok(response);
        }

        [HttpPost("webhook-sepay-deposit")]
        public async Task<IActionResult> WebhookSePay([FromBody] SePayWebhookRequest request)
        {
            Guid auctionDocumentId = AuctionDocumentHelper.ExtractAuctionDocumentId(
                request.Content
            );
            bool statusDeposit = request.TransferType == "in" && request.TransferAmount > 0;

            var command = new WebHookPaymentDepositCommand
            {
                AuctionDocumentsId = auctionDocumentId,
                Status = statusDeposit,
                Amount = request.TransferAmount,
            };

            bool result = await _mediator.Send(command);

            var response = new ApiResponse<object>
            {
                Code = result ? 200 : 400,
                Message = result ? "Webhook received and processed" : "Failed to process webhook",
            };

            return Ok(response);
        }
    }
}
