using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.CQRS.ReceiveAuctionRegistrationForm;
using MyApp.Application.Interfaces.ReceiveAuctionRegistrationForm;
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

        public async Task<bool> UpdateStatusTicketSigned(Guid auctionDocumentsId)
        {
            var auctionDocuments = await _context.AuctionDocuments.FirstOrDefaultAsync(ad =>
                ad.AuctionDocumentsId == auctionDocumentsId
            );
            if (auctionDocuments == null)
            {
                return await Task.FromResult(false);
            }
            auctionDocuments.StatusTicket = 2;
            auctionDocuments.UpdateAtTicket = DateTime.Now;
            try
            {
                _context.AuctionDocuments.Update(auctionDocuments);
                await _context.SaveChangesAsync();
                return await Task.FromResult(true);
            }
            catch (Exception)
            {
                return await Task.FromResult(false);
            }
        }
    }
}
