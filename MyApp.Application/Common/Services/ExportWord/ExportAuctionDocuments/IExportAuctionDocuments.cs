using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MyApp.Core.Entities;

namespace MyApp.Application.Common.Services.ExportWord.ExportAuctionDocuments
{
    public interface IExportAuctionDocuments
    {
        Task<(string FileName, MemoryStream Stream)> ExportAuctionDocumentToWordFile(
            AuctionDocuments document,
            IFormFile templateFile
        );
    }
}
