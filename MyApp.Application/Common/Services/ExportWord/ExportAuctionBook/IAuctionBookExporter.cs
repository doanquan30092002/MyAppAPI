using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MyApp.Core.DTOs.AuctionDocumentsDTO;

namespace MyApp.Application.Common.Services.ExportWord.ExportAuctionBook
{
    public interface IAuctionBookExporter
    {
        /// <summary>
        /// Xuất dữ liệu đăng ký đấu giá tài sản ra file Word.
        /// </summary>
        /// <param name="data">Danh sách đăng ký đấu giá</param>
        /// <returns>Mảng byte chứa nội dung file Word</returns>
        Task<byte[]> ExportToWordAsync(List<AuctionDocumentDto> data, IFormFile templateFile);
    }
}
