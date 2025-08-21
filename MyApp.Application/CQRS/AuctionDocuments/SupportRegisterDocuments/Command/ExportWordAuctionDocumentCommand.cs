using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.Runtime.Internal;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace MyApp.Application.CQRS.AuctionDocuments.SupportRegisterDocuments.Command
{
    public class ExportWordAuctionDocumentCommand : IRequest<byte[]>
    {
        /// <summary>
        /// Id tài liệu đấu giá (bắt buộc)
        /// </summary>
        [Required(ErrorMessage = "Mã hồ sơ là bắt buộc.")]
        public Guid AuctionDocumentId { get; set; }

        /// <summary>
        /// File template Word do người dùng upload (tùy chọn)
        /// </summary>
        public IFormFile TemplateFile { get; set; }
    }
}
