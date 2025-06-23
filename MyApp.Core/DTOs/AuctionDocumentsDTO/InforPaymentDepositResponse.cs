using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Core.DTOs.AuctionDocumentsDTO
{
    public class InforPaymentDepositResponse
    {
        public string QrUrl { get; set; }

        public InforBankTuanLinh InforBankTuanLinh { get; set; }

        public decimal Money { get; set; }

        public string Description { get; set; }
    }
}
