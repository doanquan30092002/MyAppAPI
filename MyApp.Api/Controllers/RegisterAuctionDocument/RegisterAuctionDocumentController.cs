using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Common.Response;
using MyApp.Application.Common.Utils;
using MyApp.Application.CQRS.RegisterAuctionDocument.Command;
using MyApp.Application.CQRS.RegisterAuctionDocument.UpdateStatusTicket;

namespace MyApp.Api.Controllers.RegisterAuctionDocument
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RegisterAuctionDocumentController(IMediator _mediator) : ControllerBase
    {
        [HttpPost]
        [Route("Register-Auction-Document")]
        public async Task<ActionResult<RegisterAuctionDocumentResponse>> RegisterAuctionDocument(
            [FromBody] RegisterAuctionDocumentRequest registerAuctionDocumentRequest
        )
        {
            var response = await _mediator.Send(registerAuctionDocumentRequest);
            if (response.Code != 200)
            {
                return Ok(new { Code = response.Code, Message = response.Message });
            }
            return Ok(
                new
                {
                    Code = response.Code,
                    Message = response.Message,
                    Data = new RegisterAuctionDocumentResponseDTO
                    {
                        QrUrl = response.QrUrl,
                        AccountNumber = response.AccountNumber,
                        BeneficiaryBank = response.BeneficiaryBank,
                        AmountTicket = response.AmountTicket,
                        Description = response.Description,
                    },
                }
            );
        }

        [HttpPost]
        [Route("Update-Status-Ticket")]
        public async Task<IActionResult> UpdateStatusTicket(
            [FromBody] SePayWebhook sePayWebhookRequest
        )
        {
            Guid auctionDocumentId = AuctionDocumentHelper.ExtractAuctionDocumentId(
                sePayWebhookRequest.Content
            );
            bool statusDeposit =
                sePayWebhookRequest.TransferType == "in" && sePayWebhookRequest.TransferAmount > 0;

            var request = new UpdateStatusTicketRequest { AuctionDocumentsId = auctionDocumentId };

            var result = await _mediator.Send(request);

            return Ok(
                new
                {
                    Code = result ? 200 : 400,
                    Message = result
                        ? "Cập nhật trạng thái phiếu đăng ký thành công"
                        : "Cập nhật trạng thái phiếu đăng ký thất bại",
                }
            );
        }
    }
}
