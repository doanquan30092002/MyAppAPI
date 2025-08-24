using Microsoft.EntityFrameworkCore;
using MyApp.Application.Interfaces.AssginAuctioneerAndPublicAuction;
using MyApp.Core.Entities;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories.AssginAuctioneerAndPublicAuction
{
    public class AssginAuctioneerAndPublicAuctionRepository
        : IAssginAuctioneerAndPublicAuctionRepository
    {
        private readonly AppDbContext _context;

        public AssginAuctioneerAndPublicAuctionRepository(AppDbContext context)
        {
            this._context = context;
        }

        public async Task<(bool, string, string)> AssignAuctioneerToAuctionAndPublicAuctionAsync(
            Guid auctionId,
            Guid auctioneerId,
            string userId,
            string staffInCharges
        )
        {
            var auction = await _context.Auctions.FirstOrDefaultAsync(x =>
                x.AuctionId == auctionId
            );

            try
            {
                auction.Auctioneer = auctioneerId;
                auction.Status = 1; // public auction status
                auction.UpdatedAt = DateTime.UtcNow;
                auction.UpdatedBy = Guid.Parse(userId);
                auction.StaffInCharge = staffInCharges;
                _context.Auctions.Update(auction);
                await _context.SaveChangesAsync();
                return (true, auction.AuctionEndDate.ToString(), auction.AuctionName.ToString()); // successfully assigned auctioneer to auction
            }
            catch (Exception)
            {
                return (false, null, null); // failed to assign auctioneer to auction
            }
        }

        public async Task<bool> CheckAuctioneerAssignedToAnotherAuctionAsync(
            Guid auctioneerId,
            Guid auctionId
        )
        {
            //get auction by auctioneerId and check if it is not null
            var auction = await _context.Auctions.FirstOrDefaultAsync(x =>
                x.AuctionId == auctionId
            );
            var check = await _context.Auctions.AnyAsync(x =>
                x.Auctioneer == auctioneerId
                && x.AuctionId != auctionId
                && x.AuctionStartDate == auction.AuctionStartDate
                && x.AuctionEndDate == auction.AuctionEndDate
            );
            if (check)
            {
                return true; // auctioneer is assigned to another auction
            }
            return false; // auctioneer is not assigned to another auction
        }

        public async Task<bool> CheckStatusAuctionIsWaitingAsync(Guid auctionId)
        {
            var auction = await _context.Auctions.FirstOrDefaultAsync(x =>
                x.AuctionId == auctionId
            );
            if (auction.Status == 4)
            {
                return true; // auction is waiting
            }
            return false; // auction is not waiting
        }

        public async Task<List<Guid>> GetAllUserIdRoleCustomer()
        {
            const int customerRoleId = 2;
            return await _context
                .Accounts.Where(a => a.RoleId == customerRoleId && a.IsActive)
                .Select(a => a.UserId)
                .Distinct()
                .ToListAsync();
        }

        public async Task<string> GetEmailFromAuctioneer(Guid auctioneer)
        {
            var emailAuctioneer = await _context
                .Accounts.Where(a => a.UserId == auctioneer)
                .Select(a => a.Email)
                .FirstOrDefaultAsync();

            return emailAuctioneer;
        }

        public async Task<List<string>> GetEmailFromStaffInCharges(List<string> staffInCharges)
        {
            List<string> emailStaffs = new List<string>();
            foreach (var staffId in staffInCharges)
            {
                var email = await _context
                    .Accounts.Where(a => a.UserId.ToString() == staffId)
                    .Select(a => a.Email)
                    .FirstOrDefaultAsync();
                if (email != null)
                {
                    emailStaffs.Add(email);
                }
            }
            return emailStaffs;
        }

        public async Task<bool> SaveNotificationAsync(List<Guid> userIds, string message)
        {
            if (userIds == null || !userIds.Any() || string.IsNullOrWhiteSpace(message))
                return false;

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
                return true; // successfully saved notifications
            }
            catch (Exception)
            {
                return false; // failed to save notifications
            }
        }
    }
}
