using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Application.CQRS.UserRegisteredAuction
{
    public class UserRegisteredAuctionResponse
    {
        public Guid Id { get; set; }
        public string CitizenIdentification { get; set; }
        public string Name { get; set; }
        public string RecentLocation { get; set; }
        public List<AuctionAsset> AuctionAssets { get; set; } = new List<AuctionAsset>();
    }

    public class AuctionAsset
    {
        public Guid AuctionAssetsId { get; set; }
        public string TagName { get; set; }
    }

    public class UserRegisteredAuctionResponseDTO
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public UserRegisteredAuctionResponse? Data { get; set; }
    }
}
