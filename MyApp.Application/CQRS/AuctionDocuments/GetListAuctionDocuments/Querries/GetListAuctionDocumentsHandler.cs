using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MyApp.Application.CQRS.Auction.GetListAuction.Querries;
using MyApp.Application.CQRS.Auction.GetListAution.Querries;
using MyApp.Application.Interfaces.IGetListDocumentsRepository;
using MyApp.Application.Interfaces.IGetListRepository;

namespace MyApp.Application.CQRS.AuctionDocuments.GetListAuctionDocuments.Querries
{
    public class GetListAuctionDocumentsHandler
        : IRequestHandler<GetListAuctionDocumentsRequest, GetListAuctionDocumentsResponse>
    {
        private readonly IGetListDocumentsRepository _getListDocumentsRepository;

        public GetListAuctionDocumentsHandler(
            IGetListDocumentsRepository getListDocumentsRepository
        )
        {
            _getListDocumentsRepository =
                getListDocumentsRepository
                ?? throw new ArgumentNullException(nameof(getListDocumentsRepository));
        }

        public Task<GetListAuctionDocumentsResponse> Handle(
            GetListAuctionDocumentsRequest request,
            CancellationToken cancellationToken
        )
        {
            return _getListDocumentsRepository.GetListAuctionDocumentsAsync(request);
        }
    }
}
