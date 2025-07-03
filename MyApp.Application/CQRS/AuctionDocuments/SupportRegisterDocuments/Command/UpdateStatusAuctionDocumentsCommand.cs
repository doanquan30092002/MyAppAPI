using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace MyApp.Application.CQRS.AuctionDocuments.SupportRegisterDocuments.Command
{
    public class UpdateStatusAuctionDocumentsCommand : IRequest<bool>
    {
        /// <summary>
        /// Id của hồ sơ đấu giá (bắt buộc)
        /// </summary>
        [Required(ErrorMessage = "Id hồ sơ đấu giá là bắt buộc.")]
        public Guid AuctionDocumentId { get; set; }

        /// <summary>
        /// Trạng thái vé (bắt buộc)
        /// </summary>
        [Required(ErrorMessage = "Trạng thái hồ là bắt buộc.")]
        public int StatusTicket { get; set; }

        /// <summary>
        /// Trạng thái đặt cọc (bắt buộc)
        /// </summary>
        [Required(ErrorMessage = "Trạng thái đặt cọc là bắt buộc.")]
        public int StatusDeposit { get; set; }
    }
}
