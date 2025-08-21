using System.ComponentModel.DataAnnotations;
using MediatR;

namespace MyApp.Application.CQRS.UserRegisteredAuction
{
    public class UserRegisteredAuctionRequest : IRequest<UserRegisteredAuctionResponseDTO>
    {
        public int NumericalOrder { get; set; }
        public Guid AuctionId { get; set; }
        public Guid? AuctionRoundId { get; set; }
    }
}
