using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.Runtime.Internal;
using MediatR;
using Microsoft.AspNetCore.Http;
using MyApp.Core.DTOs.AuctionDocumentsDTO;

namespace MyApp.Application.CQRS.ExportAuctionBook.Queries
{
    public class GetAuctionBookByAuctionIdCommand : IRequest<byte[]>
    {
        [Required(ErrorMessage = "AuctionId là bắt buộc.")]
        public Guid AuctionId { get; set; }

        //[Required(ErrorMessage = "Mẫu xuất file là bắt buộc.")]
        public IFormFile? TemplateFile { get; set; }
    }
}
