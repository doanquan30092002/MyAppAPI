using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.Runtime.Internal;
using MediatR;

namespace MyApp.Application.CQRS.AuctionDocuments.ConfirmReufund
{
    public class ConfirmRefundCommand : IRequest<bool>
    {
        [Required(ErrorMessage = "Danh sách mã hồ sơ không được để trống")]
        public List<Guid> AuctionDocumentIds { get; set; }
    }
}
