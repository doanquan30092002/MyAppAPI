using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.Interfaces.IAuctionDocuments;
using MyApp.Core.DTOs.AuctionDocumentsDTO;
using MyApp.Core.Entities;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories.AuctionDocumentsRepository
{
    public class AuctionDocumentsRepository : IAuctionDocuments
    {
        private readonly AppDbContext _context;

        public AuctionDocumentsRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<AuctionDocumentDto>> GetAllDocumentsByAuctionIdAsync(Guid auctionId)
        {
            var query =
                from doc in _context.AuctionDocuments
                join asset in _context.AuctionAssets
                    on doc.AuctionAssetId equals asset.AuctionAssetsId
                join auction in _context.Auctions on asset.AuctionId equals auction.AuctionId
                join user in _context.Users on doc.UserId equals user.Id
                join acc in _context.Accounts on user.Id equals acc.UserId
                where auction.AuctionId == auctionId
                select new AuctionDocumentDto
                {
                    AuctionDocumentsId = doc.AuctionDocumentsId,
                    CreateAtTicket = doc.CreateAtTicket,
                    AssetName = asset.TagName,
                    UserName = user.Name,
                    RecentLocation = user.RecentLocation,
                    CitizenIdentification = user.CitizenIdentification,
                    PhoneNumber = acc.PhoneNumber,
                    Deposit = asset.Deposit,
                    Result = null,
                };

            return await query.ToListAsync();
        }
    }
}
