using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using MyApp.Application.Common.CurrentUserService;
using MyApp.Application.Interfaces.IAuctionCategoriesRepository;
using MyApp.Application.Interfaces.IAuctionRepository;
using MyApp.Application.Interfaces.IExcelRepository;
using MyApp.Application.Interfaces.IJwtHelper;
using MyApp.Application.Interfaces.IUnitOfWork;

namespace MyApp.Application.CQRS.Auction.AddAuction.Commands
{
    public class AddAuctionCommandHandler : IRequestHandler<AddAuctionCommand, Guid>
    {
        private readonly IAuctionRepository _auctionRepository;
        private readonly IExcelRepository _excelRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuctionCategoriesRepository _auctionCategoriesRepository;

        public AddAuctionCommandHandler(
            IAuctionRepository auctionRepository,
            IExcelRepository excelRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IAuctionCategoriesRepository auctionCategoriesRepository
        )
        {
            _auctionRepository = auctionRepository;
            _excelRepository = excelRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _auctionCategoriesRepository = auctionCategoriesRepository;
        }

        public async Task<Guid> Handle(
            AddAuctionCommand request,
            CancellationToken cancellationToken
        )
        {
            var userIdStr = _currentUserService.GetUserId();

            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
                throw new UnauthorizedAccessException("Không thể lấy UserId từ người dùng.");

            var category = await _auctionCategoriesRepository.FindByIdAsync(request.CategoryId);
            if (category == null)
                throw new ValidationException("Loại tài sản đấu giá không tồn tại.");

            if (request.AuctionAssetFile != null && request.AuctionAssetFile.Length > 0)
            {
                var isValidExcel = await _excelRepository.CheckExcelFormatAsync(
                    request.AuctionAssetFile
                );
                if (!isValidExcel)
                    throw new ValidationException("File Excel không đúng định dạng yêu cầu.");
            }

            _unitOfWork.BeginTransaction();
            try
            {
                var auctionId = await _auctionRepository.AddAuctionAsync(request, userId);

                if (request.AuctionAssetFile != null && request.AuctionAssetFile.Length > 0)
                {
                    await _excelRepository.SaveAssetsFromExcelAsync(
                        auctionId,
                        request.AuctionAssetFile,
                        userId
                    );
                }

                await _unitOfWork.CommitAsync();
                return auctionId;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }
    }
}
