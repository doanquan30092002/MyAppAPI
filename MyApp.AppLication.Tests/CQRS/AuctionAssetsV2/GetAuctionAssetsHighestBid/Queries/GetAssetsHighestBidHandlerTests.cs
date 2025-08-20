using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using MyApp.Application.CQRS.AuctionAssetsV2.GetAuctionAssetsHighestBid.Queries;
using MyApp.Application.Interfaces.IActionAssetsRepository;
using MyApp.Core.DTOs.AuctionAssetsDTO;
using NUnit.Framework;

namespace MyApp.Application.CQRS.AuctionAssetsV2.GetAuctionAssetsHighestBid.Queries.Tests
{
    [TestFixture]
    public class GetAssetsHighestBidHandlerTests
    {
        private Mock<IAuctionAssetsRepository> _mockRepo;
        private GetAssetsHighestBidHandler _handler;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IAuctionAssetsRepository>();
            _handler = new GetAssetsHighestBidHandler(_mockRepo.Object);
        }

        [Test]
        public async Task Handle_ReturnsPagedResult_Success()
        {
            // Arrange
            var auctionId = Guid.NewGuid();
            var assets = new List<AuctionAssetsWithHighestBidResponse>
            {
                new AuctionAssetsWithHighestBidResponse
                {
                    AuctionAssetsId = Guid.NewGuid(),
                    TagName = "Tag1",
                    StartingPrice = 100,
                    HighestBid = new HighestBidInfo
                    {
                        Price = 120,
                        CitizenIdentification = "123456789",
                        Name = "Nguyen Van A",
                    },
                },
            };
            int pageSize = 10;
            int totalItems = assets.Count;
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var pagedResult = new PagedResult<AuctionAssetsWithHighestBidResponse>
            {
                Items = assets,
                TotalItems = totalItems,
                PageNumber = 1,
                PageSize = pageSize,
                TotalPages = totalPages,
            };

            _mockRepo
                .Setup(r =>
                    r.GetAuctionAssetsWithHighestBidByAuctionIdAsync(auctionId, null, 1, 10)
                )
                .ReturnsAsync(pagedResult);

            var request = new GetAssetsHighestBidRequest
            {
                AuctionId = auctionId,
                PageNumber = 1,
                PageSize = 10,
            };

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(totalItems, result.TotalItems);
            Assert.AreEqual(totalPages, result.TotalPages);
            Assert.AreEqual(assets.Count, result.Items.Count);
            Assert.AreEqual("Tag1", result.Items[0].TagName);
            Assert.AreEqual(120, result.Items[0].HighestBid.Price);
        }

        [Test]
        public async Task Handle_WithTagName_FilterApplied()
        {
            // Arrange
            var auctionId = Guid.NewGuid();
            var tagName = "SpecialTag";
            var assets = new List<AuctionAssetsWithHighestBidResponse>
            {
                new AuctionAssetsWithHighestBidResponse
                {
                    AuctionAssetsId = Guid.NewGuid(),
                    TagName = tagName,
                    StartingPrice = 200,
                    HighestBid = new HighestBidInfo
                    {
                        Price = 250,
                        CitizenIdentification = "987654321",
                        Name = "Le Thi B",
                    },
                },
            };
            int pageSize = 10;
            int totalItems = assets.Count;
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var pagedResult = new PagedResult<AuctionAssetsWithHighestBidResponse>
            {
                Items = assets,
                TotalItems = totalItems,
                PageNumber = 1,
                PageSize = pageSize,
                TotalPages = totalPages,
            };

            _mockRepo
                .Setup(r =>
                    r.GetAuctionAssetsWithHighestBidByAuctionIdAsync(auctionId, tagName, 1, 10)
                )
                .ReturnsAsync(pagedResult);

            var request = new GetAssetsHighestBidRequest
            {
                AuctionId = auctionId,
                TagName = tagName,
                PageNumber = 1,
                PageSize = 10,
            };

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(tagName, result.Items[0].TagName);
            Assert.AreEqual(250, result.Items[0].HighestBid.Price);
        }

        [Test]
        public async Task Handle_EmptyResult_ReturnsEmptyPagedResult()
        {
            // Arrange
            var auctionId = Guid.NewGuid();

            var emptyPagedResult = new PagedResult<AuctionAssetsWithHighestBidResponse>
            {
                Items = new List<AuctionAssetsWithHighestBidResponse>(),
                TotalItems = 0,
                PageNumber = 1,
                PageSize = 10,
                TotalPages = 0,
            };

            _mockRepo
                .Setup(r =>
                    r.GetAuctionAssetsWithHighestBidByAuctionIdAsync(auctionId, null, 1, 10)
                )
                .ReturnsAsync(emptyPagedResult);

            var request = new GetAssetsHighestBidRequest
            {
                AuctionId = auctionId,
                PageNumber = 1,
                PageSize = 10,
            };

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.TotalItems);
            Assert.AreEqual(0, result.TotalPages);
            Assert.AreEqual(0, result.Items.Count);
        }
    }
}
