using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Core.DTOs.AuctionDocumentsDTO
{
    public class SupportRegisterDocumentsRequest
    {
        public Guid UserId { get; set; }

        public List<Guid> AuctionAssetsIds { get; set; }

        public string BankAccount { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankBranch { get; set; }
        public Guid AuctionId { get; set; }
    }
}
