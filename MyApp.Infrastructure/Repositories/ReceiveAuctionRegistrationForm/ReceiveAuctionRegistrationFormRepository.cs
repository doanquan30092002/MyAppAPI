using Microsoft.EntityFrameworkCore;
using MyApp.Application.Interfaces.ReceiveAuctionRegistrationForm;
using MyApp.Core.Entities;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories.ReceiveAuctionRegistrationForm
{
    public class ReceiveAuctionRegistrationFormRepository
        : IReceiveAuctionRegistrationFormRepository
    {
        private readonly AppDbContext _context;

        public ReceiveAuctionRegistrationFormRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> GetAuctionNameByAuctionDocumentsIdAsync(Guid auctionDocumentsId)
        {
            if (auctionDocumentsId == null)
            {
                return string.Empty;
            }

            var auctionName = await _context
                .AuctionDocuments.Where(ad => ad.AuctionDocumentsId == auctionDocumentsId)
                .Include(ad => ad.AuctionAsset)
                .ThenInclude(aa => aa.Auction)
                .Select(ad => ad.AuctionAsset.Auction.AuctionName)
                .FirstOrDefaultAsync();

            return auctionName ?? string.Empty;
        }

        public async Task<List<Guid>> GetUserIdByAuctionDocumentId(Guid auctionDocumentsId)
        {
            var userId = await _context
                .AuctionDocuments.Where(ad => ad.AuctionDocumentsId == auctionDocumentsId)
                .Select(ad => ad.UserId)
                .ToListAsync();
            return userId;
        }

        public async Task SaveNotificationAsync(List<Guid> userIds, string message)
        {
            if (userIds == null || !userIds.Any() || string.IsNullOrWhiteSpace(message))
                return;

            var notifications = userIds
                .Select(userId => new Notification
                {
                    NotificationId = Guid.NewGuid(),
                    UserId = userId,
                    Message = message,
                    NotificationType = 0, // default type, adjust as needed
                    SentAt = DateTime.UtcNow,
                    IsRead = false,
                    UpdatedAt = DateTime.UtcNow,
                    UrlAction = "/notifications",
                })
                .ToList();
            try
            {
                await _context.Notifications.AddRangeAsync(notifications);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return; // failed to save notifications
            }
        }

        public async Task<bool> UpdateStatusTicketSigned(
            Guid auctionDocumentsId,
            int statusTicket,
            string? note
        )
        {
            var auctionDocuments = await _context.AuctionDocuments.FirstOrDefaultAsync(ad =>
                ad.AuctionDocumentsId == auctionDocumentsId
            );
            if (auctionDocuments == null)
            {
                return false;
            }
            if (!string.IsNullOrEmpty(note))
            {
                auctionDocuments.Note = note;
            }
            auctionDocuments.StatusTicket = statusTicket;
            auctionDocuments.UpdateAtTicket = DateTime.Now;
            try
            {
                _context.AuctionDocuments.Update(auctionDocuments);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
