using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyApp.Core.DTOs.AuctionDocumentsDTO;
using MyApp.Core.Entities;

namespace MyApp.Application.Interfaces.IAuctionDocuments
{
    public interface IAuctionDocuments
    {
        Task<List<AuctionDocumentDto>> GetAllDocumentsByAuctionIdAsync(Guid auctionId);
    }
}
