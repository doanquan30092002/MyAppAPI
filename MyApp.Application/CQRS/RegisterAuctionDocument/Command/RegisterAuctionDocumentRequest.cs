using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace MyApp.Application.CQRS.RegisterAuctionDocument.Command
{
    public class RegisterAuctionDocumentRequest : IRequest<RegisterAuctionDocumentResponse>
    {
        public string AuctionAssetsId { get; set; }
        public string? BankAccount { get; set; }
        public string? BankAccountNumber { get; set; }
        public string? BankBranch { get; set; }
    }
}
