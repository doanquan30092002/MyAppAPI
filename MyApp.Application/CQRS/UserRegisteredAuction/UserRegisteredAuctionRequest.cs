using System.ComponentModel.DataAnnotations;
using MediatR;

namespace MyApp.Application.CQRS.UserRegisteredAuction
{
    public class UserRegisteredAuctionRequest : IRequest<UserRegisteredAuctionResponseDTO>
    {
        [RegularExpression(
            @"^\d{12}$",
            ErrorMessage = "Số căn cước công dân phải gồm đúng 12 chữ số."
        )]
        public string CitizenIdentification { get; set; }
        public Guid AuctionId { get; set; }
        public Guid? AuctionRoundId { get; set; }
    }
}
