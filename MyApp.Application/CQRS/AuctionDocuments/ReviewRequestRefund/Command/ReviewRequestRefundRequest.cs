using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace MyApp.Application.CQRS.AuctionDocuments.ReviewRequestRefund.Command
{
    public class ReviewRequestRefundRequest : IRequest<bool>
    {
        [Required(ErrorMessage = "Danh sách ID hồ sơ không được để trống.")]
        [MinLength(1, ErrorMessage = "Phải có ít nhất một ID hồ sơ.")]
        public List<Guid> AuctionDocumentIds { get; set; }

        public string? NoteReviewRefund { get; set; }

        [Required(ErrorMessage = "Trạng thái hoàn tiền là bắt buộc.")]
        [Range(
            2,
            3,
            ErrorMessage = "Trạng thái hoàn tiền chỉ được phép là 2 (chấp nhận) hoặc 3 (từ chối)."
        )]
        public int StatusRefund { get; set; }
    }
}
