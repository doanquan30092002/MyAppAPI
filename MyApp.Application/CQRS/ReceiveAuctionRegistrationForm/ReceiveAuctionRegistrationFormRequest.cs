using MediatR;

namespace MyApp.Application.CQRS.ReceiveAuctionRegistrationForm
{
    public class ReceiveAuctionRegistrationFormRequest : IRequest<bool>
    {
        public Guid AuctionDocumentsId { get; set; }
        public int StatusTicket { get; set; }
        public string? Note { get; set; } = null;
    }
}
