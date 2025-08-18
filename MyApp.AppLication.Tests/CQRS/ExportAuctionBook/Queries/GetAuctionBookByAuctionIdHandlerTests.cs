using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Moq;
using MyApp.Application.Common.Services.ExportWord.ExportAuctionBook;
using MyApp.Application.CQRS.AuctionDocuments.FindHighestPriceAndFlag.Queries;
using MyApp.Application.CQRS.ExportAuctionBook.Queries;
using MyApp.Application.Interfaces.IAuctionDocuments;
using MyApp.Application.Interfaces.IFindHighestPriceAndFlag;
using MyApp.Core.DTOs.AuctionDocumentsDTO;
using NUnit.Framework;

namespace MyApp.Application.CQRS.ExportAuctionBook.Queries.Tests
{
    [TestFixture]
    public class GetAuctionBookByAuctionIdHandlerTests
    {
        private Mock<IAuctionDocuments> _auctionDocumentsMock;
        private Mock<IFindHighestPriceAndFlag> _findHighestPriceAndFlagMock;
        private Mock<IAuctionBookExporter> _auctionBookExporterMock;
        private GetAuctionBookByAuctionIdHandler _handler;

        [SetUp]
        public void Setup()
        {
            _auctionDocumentsMock = new Mock<IAuctionDocuments>();
            _findHighestPriceAndFlagMock = new Mock<IFindHighestPriceAndFlag>();
            _auctionBookExporterMock = new Mock<IAuctionBookExporter>();

            _handler = new GetAuctionBookByAuctionIdHandler(
                _auctionDocumentsMock.Object,
                _findHighestPriceAndFlagMock.Object,
                _auctionBookExporterMock.Object
            );
        }

        [Test]
        public async Task Handle_ShouldSetResult_TrungDauGia_WhenFlagTrue()
        {
            var auctionId = Guid.NewGuid();
            var docId = Guid.NewGuid();
            var command = new GetAuctionBookByAuctionIdCommand
            {
                AuctionId = auctionId,
                TemplateFile = Mock.Of<IFormFile>(),
            };

            var docs = new List<AuctionDocumentDto>
            {
                new AuctionDocumentDto { AuctionDocumentsId = docId },
            };

            var priceFlagsMap = new Dictionary<Guid, List<PriceFlagDto>>
            {
                {
                    docId,
                    new List<PriceFlagDto>
                    {
                        new PriceFlagDto { Flag = true, Price = 1000m },
                    }
                },
            };

            _auctionDocumentsMock
                .Setup(x => x.GetAllDocumentsByAuctionIdAsync(auctionId))
                .ReturnsAsync(docs);
            _findHighestPriceAndFlagMock
                .Setup(x => x.GetAllHighestPriceAndFlagByAuctionId(auctionId))
                .ReturnsAsync(new FindHighestPriceAndFlagResponse { Data = priceFlagsMap });
            _auctionBookExporterMock
                .Setup(x => x.ExportToWordAsync(docs, command.TemplateFile))
                .ReturnsAsync(new byte[] { 1, 2, 3 });

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.AreEqual("Trúng đấu giá", docs[0].Result);
            Assert.AreEqual(new byte[] { 1, 2, 3 }, result);
        }

        [Test]
        public async Task Handle_ShouldSetResult_KhongTrungDauGia_WhenFlagFalse()
        {
            var auctionId = Guid.NewGuid();
            var docId = Guid.NewGuid();
            var command = new GetAuctionBookByAuctionIdCommand
            {
                AuctionId = auctionId,
                TemplateFile = Mock.Of<IFormFile>(),
            };

            var docs = new List<AuctionDocumentDto>
            {
                new AuctionDocumentDto { AuctionDocumentsId = docId },
            };

            var priceFlagsMap = new Dictionary<Guid, List<PriceFlagDto>>
            {
                {
                    docId,
                    new List<PriceFlagDto>
                    {
                        new PriceFlagDto { Flag = false, Price = 500m },
                    }
                },
            };

            _auctionDocumentsMock
                .Setup(x => x.GetAllDocumentsByAuctionIdAsync(auctionId))
                .ReturnsAsync(docs);
            _findHighestPriceAndFlagMock
                .Setup(x => x.GetAllHighestPriceAndFlagByAuctionId(auctionId))
                .ReturnsAsync(new FindHighestPriceAndFlagResponse { Data = priceFlagsMap });
            _auctionBookExporterMock
                .Setup(x => x.ExportToWordAsync(docs, command.TemplateFile))
                .ReturnsAsync(new byte[] { 4, 5, 6 });

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.AreEqual("Không trúng đấu giá", docs[0].Result);
            Assert.AreEqual(new byte[] { 4, 5, 6 }, result);
        }

        [Test]
        public async Task Handle_ShouldSetResult_KhongTrungDauGia_WhenNoPriceFlags()
        {
            var auctionId = Guid.NewGuid();
            var docId = Guid.NewGuid();
            var command = new GetAuctionBookByAuctionIdCommand
            {
                AuctionId = auctionId,
                TemplateFile = Mock.Of<IFormFile>(),
            };

            var docs = new List<AuctionDocumentDto>
            {
                new AuctionDocumentDto { AuctionDocumentsId = docId },
            };

            var priceFlagsMap = new Dictionary<Guid, List<PriceFlagDto>>(); // empty map

            _auctionDocumentsMock
                .Setup(x => x.GetAllDocumentsByAuctionIdAsync(auctionId))
                .ReturnsAsync(docs);
            _findHighestPriceAndFlagMock
                .Setup(x => x.GetAllHighestPriceAndFlagByAuctionId(auctionId))
                .ReturnsAsync(new FindHighestPriceAndFlagResponse { Data = priceFlagsMap });
            _auctionBookExporterMock
                .Setup(x => x.ExportToWordAsync(docs, command.TemplateFile))
                .ReturnsAsync(new byte[] { 7, 8, 9 });

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.AreEqual("Không trúng đấu giá", docs[0].Result);
            Assert.AreEqual(new byte[] { 7, 8, 9 }, result);
        }

        [Test]
        public async Task Handle_ShouldSetResult_KhongTrungDauGia_WhenPriceFlagsFirstNull()
        {
            var auctionId = Guid.NewGuid();
            var docId = Guid.NewGuid();
            var command = new GetAuctionBookByAuctionIdCommand
            {
                AuctionId = auctionId,
                TemplateFile = Mock.Of<IFormFile>(),
            };

            var docs = new List<AuctionDocumentDto>
            {
                new AuctionDocumentDto { AuctionDocumentsId = docId },
            };

            var priceFlagsMap = new Dictionary<Guid, List<PriceFlagDto>>
            {
                {
                    docId,
                    new List<PriceFlagDto> { null! }
                }, // simulate null element
            };

            _auctionDocumentsMock
                .Setup(x => x.GetAllDocumentsByAuctionIdAsync(auctionId))
                .ReturnsAsync(docs);
            _findHighestPriceAndFlagMock
                .Setup(x => x.GetAllHighestPriceAndFlagByAuctionId(auctionId))
                .ReturnsAsync(new FindHighestPriceAndFlagResponse { Data = priceFlagsMap });
            _auctionBookExporterMock
                .Setup(x => x.ExportToWordAsync(docs, command.TemplateFile))
                .ReturnsAsync(new byte[] { 10, 11, 12 });

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.AreEqual("Không trúng đấu giá", docs[0].Result);
            Assert.AreEqual(new byte[] { 10, 11, 12 }, result);
        }
    }
}
