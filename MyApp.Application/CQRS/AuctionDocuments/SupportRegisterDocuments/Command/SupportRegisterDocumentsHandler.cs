using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using MyApp.Application.Interfaces.IJwtHelper;
using MyApp.Application.Interfaces.ISupportRegisterDocuments;
using MyApp.Core.DTOs.AuctionDocumentsDTO;

namespace MyApp.Application.CQRS.AuctionDocuments.SupportRegisterDocuments.Command
{
    public class SupportRegisterDocumentsHandler
        : IRequestHandler<SupportRegisterDocumentsCommand, bool>
    {
        private readonly ISupportRegisterDocuments _supportRegisterDocuments;
        private readonly IJwtHelper _jwtHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SupportRegisterDocumentsHandler(
            ISupportRegisterDocuments supportRegisterDocuments,
            IJwtHelper jwtHelper,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _supportRegisterDocuments = supportRegisterDocuments;
            _jwtHelper = jwtHelper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> Handle(
            SupportRegisterDocumentsCommand request,
            CancellationToken cancellationToken
        )
        {
            Guid? createdByUserId = null;

            var createdByUserStr = _httpContextAccessor
                .HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)
                ?.Value;

            if (Guid.TryParse(createdByUserStr, out var parsedGuid))
            {
                createdByUserId = parsedGuid;
            }

            if (createdByUserId == null)
                throw new UnauthorizedAccessException("Không thể lấy UserId từ người dùng.");

            var userId = await _supportRegisterDocuments.GetUserIdByCitizenIdentificationAsync(
                request.CitizenIdentification
            );
            if (userId == Guid.Empty)
                throw new UnauthorizedAccessException(
                    "Không tìm thấy người dùng với số căn cước công dân này!"
                );
            var auction = await _supportRegisterDocuments.GetAuctionByIdAsync(request.AuctionId);
            if (auction == null)
                throw new ValidationException("Phiên đấu giá không tồn tại.");

            var invalidIds = await _supportRegisterDocuments.GetInvalidAuctionAssetIdsAsync(
                request.AuctionAssetsIds
            );
            if (invalidIds.Count > 0)
                throw new ValidationException(
                    $"Các tài sản đấu giá sau không tồn tại: {string.Join(", ", invalidIds)}"
                );

            var now = DateTime.Now;
            if (now < auction.RegisterOpenDate || now > auction.RegisterEndDate)
                throw new ValidationException(
                    "Thời gian đăng ký không hợp lệ. Vui lòng đăng ký trong khoảng thời gian cho phép."
                );

            var dto = new SupportRegisterDocumentsRequest
            {
                UserId = userId,
                AuctionAssetsIds = request.AuctionAssetsIds,
                BankAccount = request.BankAccount ?? "",
                BankAccountNumber = request.BankAccountNumber ?? "",
                BankBranch = request.BankBranch ?? "",
                AuctionId = request.AuctionId,
            };

            return await _supportRegisterDocuments.RegisterAsync(dto, createdByUserId.Value);
        }
    }
}
