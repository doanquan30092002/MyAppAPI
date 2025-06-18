using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using MyApp.Application.Interfaces.IJwtHelper;
using MyApp.Application.Interfaces.ISupportRegisterDocuments;
using MyApp.Core.DTOs.AuctionDocumentsDTO;

namespace MyApp.Application.CQRS.AuctionDocuments.SupportRegisterDocuments.Command
{
    public class UpdateStatusAuctionDocumentsHandler
        : IRequestHandler<UpdateStatusAuctionDocumentsCommand, bool>
    {
        private readonly ISupportRegisterDocuments _supportRegisterDocuments;
        private readonly IJwtHelper _jwtHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UpdateStatusAuctionDocumentsHandler(
            ISupportRegisterDocuments supportRegisterDocuments,
            IJwtHelper jwtHelper,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _supportRegisterDocuments = supportRegisterDocuments;
            _jwtHelper = jwtHelper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> Handle(
            UpdateStatusAuctionDocumentsCommand request,
            CancellationToken cancellationToken
        )
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var updatedByUserId = _jwtHelper.GetUserIdFromHttpContext(httpContext);

            if (updatedByUserId == null || updatedByUserId == Guid.Empty)
                throw new UnauthorizedAccessException("Không thể lấy UserId từ người dùng.");

            var updateRequest = new UpdateStatusAuctionDocumentRequest
            {
                StatusTicket = request.StatusTicket,
                StatusDeposit = request.StatusDeposit,
            };

            var result = await _supportRegisterDocuments.UpdateAuctionDocumentStatusAsync(
                request.AuctionDocumentId,
                updateRequest,
                updatedByUserId.Value
            );

            return result;
        }
    }
}
