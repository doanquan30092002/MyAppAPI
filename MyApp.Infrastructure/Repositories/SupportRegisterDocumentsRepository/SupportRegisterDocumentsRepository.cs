using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.Interfaces.ISupportRegisterDocuments;
using MyApp.Core.DTOs.AuctionDocumentsDTO;
using MyApp.Core.Entities;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories.SupportRegisterDocuments
{
    public class SupportRegisterDocumentsRepository : ISupportRegisterDocuments
    {
        private readonly AppDbContext _dbContext;

        public SupportRegisterDocumentsRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Auction?> GetAuctionByIdAsync(Guid auctionId)
        {
            return await _dbContext
                .Auctions.AsNoTracking()
                .FirstOrDefaultAsync(x => x.AuctionId == auctionId);
        }

        public async Task<List<Guid>> GetInvalidAuctionAssetIdsAsync(List<Guid> auctionAssetIds)
        {
            if (auctionAssetIds == null || !auctionAssetIds.Any())
                return new List<Guid>();

            var validIds = await _dbContext
                .AuctionAssets.Where(a => auctionAssetIds.Contains(a.AuctionAssetsId))
                .Select(a => a.AuctionAssetsId)
                .ToListAsync();

            var invalidIds = auctionAssetIds.Except(validIds).ToList();
            return invalidIds;
        }

        public async Task<User?> GetUserByCitizenIdentificationAsync(string citizenIdentification)
        {
            if (string.IsNullOrWhiteSpace(citizenIdentification))
                return null;

            return await _dbContext
                .Users.AsNoTracking()
                .FirstOrDefaultAsync(u => u.CitizenIdentification == citizenIdentification);
        }

        public async Task<Guid> GetUserIdByCitizenIdentificationAsync(string citizenIdentification)
        {
            if (string.IsNullOrWhiteSpace(citizenIdentification))
                return Guid.Empty;

            var user = await _dbContext
                .Users.AsNoTracking()
                .FirstOrDefaultAsync(u => u.CitizenIdentification == citizenIdentification);

            return user?.Id ?? Guid.Empty;
        }

        public async Task<bool> RegisterAsync(
            SupportRegisterDocumentsRequest request,
            Guid createdByUserId
        )
        {
            if (
                request == null
                || request.AuctionAssetsIds == null
                || request.AuctionAssetsIds.Count == 0
            )
                return false;

            var assetIds = request.AuctionAssetsIds?.ToList() ?? new List<Guid>();

            var alreadyRegisteredAssets = await _dbContext
                .AuctionDocuments.Include(x => x.AuctionAsset)
                .Where(x =>
                    x.UserId == request.UserId
                    && assetIds.Contains(x.AuctionAssetId)
                    && x.AuctionAsset.AuctionId == request.AuctionId
                )
                .Select(x => x.AuctionAssetId)
                .ToListAsync();

            if (alreadyRegisteredAssets.Any())
            {
                var conflictedIds = string.Join(", ", alreadyRegisteredAssets);
                throw new InvalidOperationException(
                    $"Người dùng đã đăng kí tài sản này trong cùng phiên: {conflictedIds}"
                );
            }

            var existingDocumentInSession = await _dbContext
                .AuctionDocuments.Include(x => x.AuctionAsset)
                .Where(x => x.UserId == request.UserId)
                .Where(x => x.AuctionAsset.AuctionId == request.AuctionId)
                .OrderBy(x => x.NumericalOrder)
                .FirstOrDefaultAsync();

            int numericalOrder;

            if (
                existingDocumentInSession != null
                && existingDocumentInSession.NumericalOrder.HasValue
            )
            {
                numericalOrder = existingDocumentInSession.NumericalOrder.Value;
            }
            else
            {
                var maxOrder = (
                    await _dbContext
                        .AuctionDocuments.Include(x => x.AuctionAsset)
                        .Where(x => x.AuctionAsset.AuctionId == request.AuctionId)
                        .Select(x => x.NumericalOrder ?? 0)
                        .ToListAsync()
                )
                    .DefaultIfEmpty(0)
                    .Max();

                numericalOrder = maxOrder + 1;
            }

            var documents = new List<AuctionDocuments>();
            foreach (var assetId in assetIds)
            {
                var doc = new AuctionDocuments
                {
                    AuctionDocumentsId = Guid.NewGuid(),
                    UserId = request.UserId,
                    AuctionAssetId = assetId,
                    BankAccount = request.BankAccount,
                    BankAccountNumber = request.BankAccountNumber,
                    BankBranch = request.BankBranch,
                    CreateByTicket = createdByUserId,
                    CreateAtTicket = DateTime.Now,
                    UpdateAtTicket = DateTime.Now,
                    CreateAtDeposit = DateTime.Now,
                    StatusTicket = false,
                    StatusDeposit = false,
                    StatusRefundDeposit = false,
                    NumericalOrder = numericalOrder,
                };
                documents.Add(doc);
            }

            await _dbContext.AuctionDocuments.AddRangeAsync(documents);
            var result = await _dbContext.SaveChangesAsync();

            return result > 0;
        }

        public async Task<bool> UpdateAuctionDocumentStatusAsync(
            Guid auctionDocumentId,
            UpdateStatusAuctionDocumentRequest request,
            Guid updatedBy
        )
        {
            var auctionDocument = await _dbContext.AuctionDocuments.FirstOrDefaultAsync(x =>
                x.AuctionDocumentsId == auctionDocumentId
            );

            if (auctionDocument == null)
                throw new KeyNotFoundException(
                    $"Không tìm thấy hồ sơ đấu giá với Id: {auctionDocumentId}"
                );

            auctionDocument.UpdateAtTicket = DateTime.Now;
            auctionDocument.StatusTicket = request.StatusTicket;
            auctionDocument.StatusDeposit = request.StatusDeposit;

            _dbContext.AuctionDocuments.Update(auctionDocument);
            var result = await _dbContext.SaveChangesAsync();

            return result > 0;
        }
    }
}
