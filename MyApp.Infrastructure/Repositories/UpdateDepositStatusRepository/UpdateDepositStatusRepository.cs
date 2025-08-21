using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyApp.Application.CQRS.UpdateStatusDeposit.Command;
using MyApp.Application.Interfaces.IUpdateDepositStatus;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories.UpdateDepositStatusRepository
{
    public class UpdateDepositStatusRepository : IUpdateDepositStatus
    {
        private readonly AppDbContext context;

        public UpdateDepositStatusRepository(AppDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Task<UpdateDepositStatusResponse> UpdateDepositStatus(
            UpdateDepositStatusRequest updateDepositStatusRequest,
            CancellationToken cancellationToken
        )
        {
            if (updateDepositStatusRequest.AuctionDocumentsId == Guid.Empty)
            {
                return Task.FromResult(new UpdateDepositStatusResponse { StatusUpdate = false });
            }

            var auctionDocument = context.AuctionDocuments.FirstOrDefault(ad =>
                ad.AuctionDocumentsId == updateDepositStatusRequest.AuctionDocumentsId
            );

            if (auctionDocument == null)
            {
                return Task.FromResult(new UpdateDepositStatusResponse { StatusUpdate = false });
            }

            if (auctionDocument.StatusDeposit != 0)
            {
                return Task.FromResult(new UpdateDepositStatusResponse { StatusUpdate = false });
            }

            auctionDocument.StatusDeposit = 1;
            auctionDocument.IsAttended = true;

            if (!string.IsNullOrEmpty(updateDepositStatusRequest.Note))
            {
                auctionDocument.Note = updateDepositStatusRequest.Note;
            }

            auctionDocument.UpdateAtTicket = new DateTime(
                2025,
                7,
                10,
                2,
                43,
                0,
                DateTimeKind.Utc
            ).ToLocalTime();

            try
            {
                context.SaveChanges();

                return Task.FromResult(new UpdateDepositStatusResponse { StatusUpdate = true });
            }
            catch (Exception)
            {
                return Task.FromResult(new UpdateDepositStatusResponse { StatusUpdate = false });
            }
        }
    }
}
