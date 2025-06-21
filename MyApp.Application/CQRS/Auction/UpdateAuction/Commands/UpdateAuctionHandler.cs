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
using MyApp.Application.Interfaces.IActionAssetsRepository;
using MyApp.Application.Interfaces.IAuctionCategoriesRepository;
using MyApp.Application.Interfaces.IAuctionRepository;
using MyApp.Application.Interfaces.IExcelRepository;
using MyApp.Application.Interfaces.IJwtHelper;
using MyApp.Application.Interfaces.IUnitOfWork;
using MyApp.Application.JobBackgroud.AuctionJob;

namespace MyApp.Application.CQRS.Auction.UpdateAuction.Commands
{
    public class UpdateAuctionHandler : IRequestHandler<UpdateAuctionCommand, Guid>
    {
        private readonly IAuctionRepository _auctionRepository;
        private readonly IAuctionCategoriesRepository _categoriesRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IJwtHelper _jwtHelper;
        private readonly IExcelRepository _excelRepository;
        private readonly IAuctionAssetsRepository _auctionAssetRepository;
        private readonly SetAuctionUpdateableFalse _setAuctionUpdateableFalse;

        public UpdateAuctionHandler(
            IAuctionRepository auctionRepository,
            IAuctionCategoriesRepository categoriesRepository,
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            IJwtHelper jwtHelper,
            IExcelRepository excelRepository,
            IAuctionAssetsRepository auctionAssetRepository,
            SetAuctionUpdateableFalse setAuctionUpdateableFalse
        )
        {
            _auctionRepository = auctionRepository;
            _categoriesRepository = categoriesRepository;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _jwtHelper = jwtHelper;
            _excelRepository = excelRepository;
            _auctionAssetRepository = auctionAssetRepository;
            _setAuctionUpdateableFalse = setAuctionUpdateableFalse;
        }

        public async Task<Guid> Handle(
            UpdateAuctionCommand request,
            CancellationToken cancellationToken
        )
        {
            Guid? userId = null;

            var userIdStr = _httpContextAccessor
                .HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)
                ?.Value;

            if (Guid.TryParse(userIdStr, out var parsedGuid))
            {
                userId = parsedGuid;
            }

            if (userId == null)
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
            UpdateAuctionResult updateResult;
            try
            {
                updateResult = await _auctionRepository.UpdateAuctionAsync(request, userId.Value);

                if (request.AuctionAssetFile != null && request.AuctionAssetFile.Length > 0)
                {
                    await _auctionAssetRepository.DeleteByAuctionIdAsync(request.AuctionId);
                    await _excelRepository.SaveAssetsFromExcelAsync(
                        request.AuctionId,
                        request.AuctionAssetFile,
                        userId.Value
                    );
                }

                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }

            if (updateResult.StatusChangedToTrue)
            {
                var delay = updateResult.AuctionEndDate - DateTime.Now;
                if (delay > TimeSpan.Zero)
                {
                    BackgroundJob.Schedule<SetAuctionUpdateableFalse>(
                        job => job.SetAuctionUpdateableFalseAsync(request.AuctionId),
                        delay
                    );
                }
                else
                {
                    await _setAuctionUpdateableFalse.SetAuctionUpdateableFalseAsync(
                        request.AuctionId
                    );
                }
            }

            return request.AuctionId;
        }
    }
}
