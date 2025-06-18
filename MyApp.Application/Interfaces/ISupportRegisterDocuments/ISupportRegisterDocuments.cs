using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyApp.Core.DTOs.AuctionDocumentsDTO;
using MyApp.Core.Entities;

namespace MyApp.Application.Interfaces.ISupportRegisterDocuments
{
    public interface ISupportRegisterDocuments
    {
        /// <summary>
        /// Xử lý đăng ký hồ sơ đấu giá kèm thông tin tài khoản ngân hàng và danh sách tài sản.
        /// </summary>
        /// <param name="request">Dữ liệu đăng ký</param>
        /// <returns>Trả về true nếu thành công</returns>
        Task<bool> RegisterAsync(SupportRegisterDocumentsRequest request, Guid createdByUserId);

        /// <summary>
        /// Tìm UserId theo số căn cước công dân (CMND/CCCD).
        /// </summary>
        /// <param name="citizenIdentification">Chuỗi số CCCD/CMND</param>
        /// <returns>Guid UserId. Nếu không tìm thấy, trả về Guid.Empty</returns>
        Task<Guid> GetUserIdByCitizenIdentificationAsync(string citizenIdentification);

        /// <summary>
        /// Kiểm tra tính hợp lệ của danh sách AuctionAssetIds.
        /// </summary>
        /// <param name="auctionAssetIds">Danh sách Id tài sản đấu giá</param>
        /// <returns>Danh sách các Guid không hợp lệ (không tồn tại trong DB). Nếu tất cả hợp lệ, trả về danh sách rỗng.</returns>
        Task<List<Guid>> GetInvalidAuctionAssetIdsAsync(List<Guid> auctionAssetIds);

        /// <summary>
        /// Tìm thông tin phiên đấu giá theo AuctionId.
        /// </summary>
        /// <param name="auctionId">Guid của phiên đấu giá</param>
        /// <returns>Thông tin phiên đấu giá, nếu không tìm thấy trả về null</returns>
        Task<Auction?> GetAuctionByIdAsync(Guid auctionId);

        /// <summary>
        /// Tìm thông tin người dùng theo số căn cước công dân (CMND/CCCD).
        /// </summary>
        /// <param name="citizenIdentification">Chuỗi số CCCD/CMND</param>
        /// <returns>Thông tin người dùng. Nếu không tìm thấy, trả về null</returns>
        Task<User?> GetUserByCitizenIdentificationAsync(string citizenIdentification);

        /// <summary>
        /// Cập nhật trạng thái hồ sơ đấu giá.
        /// </summary>
        /// <param name="auctionDocumentId">Id của hồ sơ đấu giá cần cập nhật</param>
        /// <param name="request">Trạng thái mới cần cập nhật</param>
        /// <param name="updatedBy">Guid người thực hiện cập nhật</param>
        /// <returns>True nếu cập nhật thành công, false nếu thất bại</returns>
        Task<bool> UpdateAuctionDocumentStatusAsync(
            Guid auctionDocumentId,
            UpdateStatusAuctionDocumentRequest request,
            Guid updatedBy
        );
    }
}
