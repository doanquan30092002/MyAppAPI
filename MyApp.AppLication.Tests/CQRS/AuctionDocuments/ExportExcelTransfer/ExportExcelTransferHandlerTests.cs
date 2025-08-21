using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using MyApp.Application.CQRS.AuctionDocuments.ExportExcelTransfer;
using MyApp.Application.Interfaces.IAuctionRepository;
using MyApp.Application.Interfaces.IExcelRepository;
using NUnit.Framework;

namespace MyApp.Application.CQRS.AuctionDocuments.ExportExcelTransfer.Tests
{
    [TestFixture]
    public class ExportExcelTransferHandlerTests
    {
        private Mock<IAuctionRepository> _auctionRepoMock;
        private Mock<IExcelRepository> _excelRepoMock;
        private ExportExcelTransferHandler _handler;

        [SetUp]
        public void Setup()
        {
            _auctionRepoMock = new Mock<IAuctionRepository>();
            _excelRepoMock = new Mock<IExcelRepository>();
            _handler = new ExportExcelTransferHandler(
                _excelRepoMock.Object,
                _auctionRepoMock.Object
            );
        }

        [Test]
        public void Handle_ShouldThrowKeyNotFound_WhenAuctionNotFound()
        {
            // Arrange
            var request = new ExportExcelTransferCommand { AuctionId = Guid.NewGuid() };
            _auctionRepoMock
                .Setup(x => x.FindAuctionByIdAsync(request.AuctionId))
                .ReturnsAsync((MyApp.Core.Entities.Auction)null);

            // Act + Assert
            Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                await _handler.Handle(request, CancellationToken.None)
            );
        }

        [Test]
        public void Handle_ShouldThrowInvalidOperation_WhenAuctionStatusNotCompletedOrCanceled()
        {
            // Arrange
            var request = new ExportExcelTransferCommand { AuctionId = Guid.NewGuid() };
            var auction = new MyApp.Core.Entities.Auction
            {
                AuctionId = request.AuctionId,
                Status = 1,
            }; // công khai
            _auctionRepoMock
                .Setup(x => x.FindAuctionByIdAsync(request.AuctionId))
                .ReturnsAsync(auction);

            // Act + Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _handler.Handle(request, CancellationToken.None)
            );
        }

        [Test]
        public async Task Handle_ShouldReturnExcelData_WhenAuctionCompleted()
        {
            // Arrange
            var request = new ExportExcelTransferCommand { AuctionId = Guid.NewGuid() };
            var auction = new MyApp.Core.Entities.Auction
            {
                AuctionId = request.AuctionId,
                Status = 2,
            }; // hoàn thành
            var expectedData = new byte[] { 1, 2, 3 };

            _auctionRepoMock
                .Setup(x => x.FindAuctionByIdAsync(request.AuctionId))
                .ReturnsAsync(auction);

            _excelRepoMock
                .Setup(x => x.ExportRefundDocumentsExcelAsync(request.AuctionId))
                .ReturnsAsync(expectedData);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.AreEqual(expectedData, result);
        }

        [Test]
        public async Task Handle_ShouldReturnExcelData_WhenAuctionCanceled()
        {
            // Arrange
            var request = new ExportExcelTransferCommand { AuctionId = Guid.NewGuid() };
            var auction = new MyApp.Core.Entities.Auction
            {
                AuctionId = request.AuctionId,
                Status = 3,
            }; // hủy
            var expectedData = new byte[] { 9, 9, 9 };

            _auctionRepoMock
                .Setup(x => x.FindAuctionByIdAsync(request.AuctionId))
                .ReturnsAsync(auction);

            _excelRepoMock
                .Setup(x => x.ExportRefundDocumentsExcelAsync(request.AuctionId))
                .ReturnsAsync(expectedData);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.AreEqual(expectedData, result);
        }
    }
}
