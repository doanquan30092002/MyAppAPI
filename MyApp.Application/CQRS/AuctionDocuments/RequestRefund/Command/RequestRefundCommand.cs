using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace MyApp.Application.CQRS.AuctionDocuments.RequestRefund.Command
{
    public class RequestRefundCommand : IRequest<bool>
    {
        [Required(ErrorMessage = "Danh sách hồ sơ không được để trống.")]
        [MinLength(1, ErrorMessage = "Phải có ít nhất một ID hồ sơ.")]
        public List<Guid> AuctionDocumentIds { get; set; }

        [Required(ErrorMessage = "Lý do hoàn tiền là bắt buộc.")]
        public string RefundReason { get; set; }

        [Required(ErrorMessage = "Vui lòng đính kèm minh chứng hoàn tiền.")]
        public IFormFile RefundProof { get; set; }
    }
}
