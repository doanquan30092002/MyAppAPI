using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MyApp.Application.CQRS.AuctionDocuments.RequestRefund.Helper
{
    public interface IRequestRefundHelper
    {
        Guid GetCurrentUserId();

        Task ValidateDocumentForRefundAsync(Guid documentId, Guid userId);

        Task<string> UploadRefundProofAsync(IFormFile refundProof);
    }
}
