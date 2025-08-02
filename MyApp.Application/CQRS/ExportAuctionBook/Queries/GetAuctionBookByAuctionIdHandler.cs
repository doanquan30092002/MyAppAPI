using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MyApp.Application.Common.Services.ExportWord.ExportAuctionBook;
using MyApp.Application.Interfaces.IAuctionDocuments;
using MyApp.Application.Interfaces.IFindHighestPriceAndFlag;
using MyApp.Core.DTOs.AuctionDocumentsDTO;

namespace MyApp.Application.CQRS.ExportAuctionBook.Queries
{
    public class GetAuctionBookByAuctionIdHandler
        : IRequestHandler<GetAuctionBookByAuctionIdCommand, byte[]>
    {
        private readonly IAuctionDocuments _auctionDocuments;

        private readonly IFindHighestPriceAndFlag _findHighestPriceAndFlag;

        private readonly IAuctionBookExporter _auctionBookExporter;

        public GetAuctionBookByAuctionIdHandler(
            IAuctionDocuments auctionDocuments,
            IFindHighestPriceAndFlag findHighestPriceAndFlag,
            IAuctionBookExporter auctionBookExporter
        )
        {
            _auctionDocuments = auctionDocuments;
            _findHighestPriceAndFlag = findHighestPriceAndFlag;
            _auctionBookExporter = auctionBookExporter;
        }

        public async Task<byte[]> Handle(
            GetAuctionBookByAuctionIdCommand request,
            CancellationToken cancellationToken
        )
        {
            var result = await _auctionDocuments.GetAllDocumentsByAuctionIdAsync(request.AuctionId);

            var response = await _findHighestPriceAndFlag.GetAllHighestPriceAndFlagByAuctionId(
                request.AuctionId
            );

            var priceFlagMap = response.Data;

            foreach (var doc in result)
            {
                if (
                    priceFlagMap.TryGetValue(doc.AuctionDocumentsId, out var priceFlags)
                    && priceFlags.Count > 0
                    && priceFlags[0] != null
                )
                {
                    doc.Result = priceFlags[0].Flag ? "Trúng đấu giá" : "Không trúng đấu giá";
                }
                else
                {
                    doc.Result = "Không trúng đấu giá";
                }
            }

            return await _auctionBookExporter.ExportToWordAsync(result, request.TemplateFile);
        }
    }
}
