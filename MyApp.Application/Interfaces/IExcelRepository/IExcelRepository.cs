using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MyApp.Application.Interfaces.IExcelRepository
{
    public interface IExcelRepository
    {
        /// <summary>
        /// Lưu các asset vào database từ file excel truyền vào.
        /// </summary>
        /// <param name="auctionId">Id của auction</param>
        /// <param name="excelFile">File Excel theo format yêu cầu</param>
        Task SaveAssetsFromExcelAsync(Guid auctionId, IFormFile excelFile, Guid userId);

        /// <summary>
        /// Kiểm tra định dạng file Excel có hợp lệ theo chuẩn yêu cầu hay không.
        /// </summary>
        /// <param name="excelFile">File Excel cần kiểm tra</param>
        /// <returns>True nếu hợp lệ, False nếu không hợp lệ</returns>
        Task<bool> CheckExcelFormatAsync(IFormFile excelFile);

        /// <summary>
        /// Xuất file Excel danh sách hồ sơ đấu giá cần hoàn tiền khi hủy phiên đấu giá.
        /// </summary>
        /// <param name="auctionId">Id của phiên đấu giá</param>
        /// <returns>File Excel dưới dạng mảng byte[]</returns>
        Task<byte[]> ExportRefundDocumentsExcelAsync(Guid auctionId);
    }
}
