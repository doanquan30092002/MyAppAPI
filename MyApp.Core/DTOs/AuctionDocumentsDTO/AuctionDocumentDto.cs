using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Core.DTOs.AuctionDocumentsDTO
{
    public class AuctionDocumentDto
    {
        public Guid AuctionDocumentsId { get; set; }
        public DateTime CreateAtTicket { get; set; }
        public string AssetName { get; set; }
        public string UserName { get; set; }
        public string RecentLocation { get; set; }
        public string CitizenIdentification { get; set; }
        public string PhoneNumber { get; set; }
        public decimal Deposit { get; set; }
        public string? Result { get; set; }
    }
}
