using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Http;
using MyApp.Application.Common.CurrentUserService;
using MyApp.Application.Interfaces.IActionAssetsRepository;
using MyApp.Application.Interfaces.IAuctionCategoriesRepository;
using MyApp.Application.Interfaces.IAuctionRepository;
using MyApp.Application.Interfaces.IExcelRepository;
using MyApp.Application.Interfaces.IUnitOfWork;

namespace MyApp.Application.CQRS.Auction.UpdateAuction.Commands
{
    public class UpdateAuctionHandler : IRequestHandler<UpdateAuctionCommand, Guid>
    {
        private readonly IAuctionRepository _auctionRepository;
        private readonly IAuctionCategoriesRepository _categoriesRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IExcelRepository _excelRepository;
        private readonly IAuctionAssetsRepository _auctionAssetRepository;

        public UpdateAuctionHandler(
            IAuctionRepository auctionRepository,
            IAuctionCategoriesRepository categoriesRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IExcelRepository excelRepository,
            IAuctionAssetsRepository auctionAssetRepository
        )
        {
            _auctionRepository = auctionRepository;
            _categoriesRepository = categoriesRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _excelRepository = excelRepository;
            _auctionAssetRepository = auctionAssetRepository;
        }

        public async Task<Guid> Handle(
            UpdateAuctionCommand request,
            CancellationToken cancellationToken
        )
        {
            var userIdStr = _currentUserService.GetUserId();

            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
                throw new UnauthorizedAccessException("Không thể lấy UserId từ người dùng.");

            var auction = await _auctionRepository.FindAuctionByIdAsync(request.AuctionId);
            if (auction == null)
                throw new ValidationException("Phiên đấu giá không tồn tại.");

            if (auction.Updateable == false)
                throw new ValidationException("Phiên đấu giá đã khóa do đã hoàn thành.");

            if (request.CategoryId != auction.CategoryId)
            {
                var category = await _categoriesRepository.FindByIdAsync(request.CategoryId);
                if (category == null)
                    throw new ValidationException("Loại tài sản đấu giá không tồn tại.");
            }

            _unitOfWork.BeginTransaction();

            try
            {
                bool updateSuccess = await _auctionRepository.UpdateAuctionAsync(request, userId);

                if (!updateSuccess)
                {
                    await _unitOfWork.RollbackAsync();
                    throw new InvalidOperationException("Cập nhật phiên đấu giá thất bại.");
                }

                if (request.AuctionAssetFile != null && request.AuctionAssetFile.Length > 0)
                {
                    await _auctionAssetRepository.DeleteByAuctionIdAsync(request.AuctionId);
                    await _excelRepository.SaveAssetsFromExcelAsync(
                        request.AuctionId,
                        request.AuctionAssetFile,
                        userId
                    );
                }

                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }

            return request.AuctionId;
        }
    }
}
