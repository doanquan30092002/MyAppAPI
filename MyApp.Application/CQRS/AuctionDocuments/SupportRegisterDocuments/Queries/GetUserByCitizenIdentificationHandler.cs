using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.Runtime.Internal;
using MediatR;
using MyApp.Application.Interfaces.ISupportRegisterDocuments;
using MyApp.Core.Entities;

namespace MyApp.Application.CQRS.AuctionDocuments.SupportRegisterDocuments.Queries
{
    public class GetUserByCitizenIdentificationHandler
        : IRequestHandler<GetUserByCitizenIdentificationRequest, User?>
    {
        private readonly ISupportRegisterDocuments _supportRegisterDocuments;

        public GetUserByCitizenIdentificationHandler(
            ISupportRegisterDocuments supportRegisterDocuments
        )
        {
            _supportRegisterDocuments = supportRegisterDocuments;
        }

        public async Task<User?> Handle(
            GetUserByCitizenIdentificationRequest request,
            CancellationToken cancellationToken
        )
        {
            if (string.IsNullOrWhiteSpace(request.CitizenIdentification))
                return null;

            return await _supportRegisterDocuments.GetUserByCitizenIdentificationAsync(
                request.CitizenIdentification
            );
        }
    }
}
