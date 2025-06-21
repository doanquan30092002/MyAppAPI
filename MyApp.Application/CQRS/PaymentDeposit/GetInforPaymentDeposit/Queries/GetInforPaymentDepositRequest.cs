using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MyApp.Core.DTOs.AuctionDocumentsDTO;

namespace MyApp.Application.CQRS.PaymentDeposit.GetInforPaymentDeposit.Queries
{
    public class GetInforPaymentDepositRequest : IRequest<InforPaymentDepositResponse>
    {
        [Required(ErrorMessage = "ID hồ sơ là bắt buộc")]
        public Guid AuctionDocumentsId { get; set; }
    }
}
