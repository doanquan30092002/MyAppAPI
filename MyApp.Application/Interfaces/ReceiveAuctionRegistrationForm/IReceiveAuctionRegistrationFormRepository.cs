using MyApp.Application.CQRS.ReceiveAuctionRegistrationForm;

namespace MyApp.Application.Interfaces.ReceiveAuctionRegistrationForm
{
    public interface IReceiveAuctionRegistrationFormRepository
    {
        Task<bool> UpdateStatusTicketSigned(Guid auctionDocumentsId);
    }
}
