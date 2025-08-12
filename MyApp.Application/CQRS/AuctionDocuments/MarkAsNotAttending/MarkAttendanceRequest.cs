using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace MyApp.Application.CQRS.AuctionDocuments.MarkAsNotAttending
{
    public class MarkAttendanceRequest : IRequest<bool>
    {
        [Required(ErrorMessage = "Danh sách ID hồ sơ không được để trống.")]
        [MinLength(1, ErrorMessage = "Phải có ít nhất một ID hồ sơ.")]
        public List<Guid> AuctionDocumentIds { get; set; }

        [Required(ErrorMessage = "Trạng thái tham gia là bắt buộc.")]
        public bool IsAttended { get; set; }
    }
}
