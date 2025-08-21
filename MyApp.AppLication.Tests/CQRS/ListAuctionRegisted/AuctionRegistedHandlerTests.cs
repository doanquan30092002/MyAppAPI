using Moq;
using MyApp.Application.Interfaces.ListAuctionRegisted;

namespace MyApp.Application.CQRS.ListAuctionRegisted.Tests
{
    [TestFixture()]
    public class AuctionRegistedHandlerTests
    {
        private Mock<IAuctionRegistedRepository> _repoMock;
        private AuctionRegistedHandler _handler;

        private Guid _userId;
        private Guid _auctionId;
        private Guid _assetId;

        [SetUp]
        public void SetUp()
        {
            _repoMock = new Mock<IAuctionRegistedRepository>();
            _handler = new AuctionRegistedHandler(_repoMock.Object);

            _userId = Guid.Parse("68faa0b5-2e6b-4792-8ceb-8a5e951a3e1e");
            _auctionId = Guid.Parse("A6BB60B6-C387-4764-99AB-B01FBAB2B13D");
            _assetId = Guid.Parse("259BD93F-6967-42E7-AF05-AB1BEE026F75");
        }

        [Test]
        public async Task Handle_ShouldReturnEmpty_WhenNoAssets()
        {
            var request = new AuctionRegistedRequest
            {
                PageNumber = 1,
                PageSize = 10,
                UserId = _userId,
            };

            _repoMock
                .Setup(r => r.GetRegisteredAssetIdsAsync(_userId))
                .ReturnsAsync(new List<Guid>());

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.That(result.TotalAuctionRegisted, Is.EqualTo(0));
            Assert.That(result.AuctionResponse, Is.Null);
        }

        [Test]
        public async Task Handle_ShouldReturnEmpty_WhenNoAuctions()
        {
            var request = new AuctionRegistedRequest
            {
                PageNumber = 1,
                PageSize = 10,
                UserId = _userId,
            };

            _repoMock
                .Setup(r => r.GetRegisteredAssetIdsAsync(_userId))
                .ReturnsAsync(new List<Guid> { _assetId });

            _repoMock
                .Setup(r => r.GetRegisteredAuctionIdsAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(new List<Guid>());

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.That(result.TotalAuctionRegisted, Is.EqualTo(0));
            Assert.That(result.AuctionResponse, Is.Null);
        }

        [Test]
        public async Task Handle_ShouldReturnEmpty_WhenSearchFiltersOutAll()
        {
            var request = new AuctionRegistedRequest
            {
                PageNumber = 1,
                PageSize = 10,
                UserId = _userId,
                Search = new SearchAuctionRegisted
                {
                    AuctionName = "Không tồn tại",
                    AuctionStartDate = new DateTime(2025, 5, 1),
                    AuctionEndDate = new DateTime(2025, 4, 30), // end date < start date
                },
            };

            _repoMock
                .Setup(r => r.GetRegisteredAssetIdsAsync(_userId))
                .ReturnsAsync(new List<Guid> { _assetId });

            _repoMock
                .Setup(r => r.GetRegisteredAuctionIdsAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(new List<Guid> { _auctionId });

            _repoMock
                .Setup(r => r.GetAuctionsByIdsAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(
                    new List<AuctionResponse>
                    {
                        new AuctionResponse
                        {
                            AuctionId = _auctionId,
                            AuctionName = "Đấu giá vàng",
                            CategoryName = "Vàng",
                            AuctionDescription = "Phiên đấu giá vàng",
                            AuctionRules = "Không hoàn tiền cọc",
                            AuctionPlanningMap = null,
                            RegisterOpenDate = new DateTime(2025, 4, 20),
                            RegisterEndDate = new DateTime(2025, 4, 25),
                            AuctionStartDate = new DateTime(2025, 5, 1),
                            AuctionEndDate = new DateTime(2025, 5, 5),
                            NumberRoundMax = 3,
                            Status = 1,
                        },
                    }
                );

            _repoMock
                .Setup(r =>
                    r.GetAuctionAssetsByAuctionIdAsync(It.IsAny<Guid>(), It.IsAny<List<Guid>>())
                )
                .ReturnsAsync(new List<AuctionAsset>());

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.That(result.TotalAuctionRegisted, Is.EqualTo(0));
            Assert.That(result.AuctionResponse, Is.Empty);
        }

        [Test]
        public async Task Handle_ShouldReturnPagedResults_WhenMultipleAuctions()
        {
            var request = new AuctionRegistedRequest
            {
                PageNumber = 2,
                PageSize = 1,
                UserId = _userId,
            };

            _repoMock
                .Setup(r => r.GetRegisteredAssetIdsAsync(_userId))
                .ReturnsAsync(new List<Guid> { _assetId });

            _repoMock
                .Setup(r => r.GetRegisteredAuctionIdsAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(new List<Guid> { _auctionId, Guid.NewGuid() });

            var auctionList = new List<AuctionResponse>
            {
                new AuctionResponse
                {
                    AuctionId = _auctionId,
                    AuctionName = "Đấu giá 1",
                    CategoryName = "Bất động sản",
                    AuctionDescription = "Mô tả 1",
                    AuctionRules = "Luật 1",
                    AuctionPlanningMap = "Map1",
                    RegisterOpenDate = new DateTime(2025, 3, 1),
                    RegisterEndDate = new DateTime(2025, 3, 5),
                    AuctionStartDate = new DateTime(2025, 3, 10),
                    AuctionEndDate = new DateTime(2025, 3, 15),
                    NumberRoundMax = 3,
                    Status = 1,
                },
                new AuctionResponse
                {
                    AuctionId = Guid.NewGuid(),
                    AuctionName = "Đấu giá 2",
                    CategoryName = "Ô tô",
                    AuctionDescription = "Mô tả 2",
                    AuctionRules = "Luật 2",
                    AuctionPlanningMap = "Map2",
                    RegisterOpenDate = new DateTime(2025, 4, 1),
                    RegisterEndDate = new DateTime(2025, 4, 5),
                    AuctionStartDate = new DateTime(2025, 4, 10),
                    AuctionEndDate = new DateTime(2025, 4, 15),
                    NumberRoundMax = 4,
                    Status = 2,
                },
            };

            _repoMock
                .Setup(r => r.GetAuctionsByIdsAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(auctionList);

            _repoMock
                .Setup(r =>
                    r.GetAuctionAssetsByAuctionIdAsync(It.IsAny<Guid>(), It.IsAny<List<Guid>>())
                )
                .ReturnsAsync(
                    new List<AuctionAsset>
                    {
                        new AuctionAsset
                        {
                            AuctionAssetsId = _assetId,
                            TagName = "Tài sản 1",
                            StartingPrice = 1000,
                            Unit = "VNĐ",
                            Deposit = 100,
                            RegistrationFee = 50,
                            Description = "TS1",
                            AuctionId = _auctionId,
                        },
                    }
                );

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.That(result.PageNumber, Is.EqualTo(2));
            Assert.That(result.PageSize, Is.EqualTo(1));
            Assert.That(result.TotalAuctionRegisted, Is.EqualTo(2));
            Assert.That(result.AuctionResponse.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task Handle_ShouldReturnFullData_WhenSearchMatches()
        {
            var request = new AuctionRegistedRequest
            {
                PageNumber = 1,
                PageSize = 5,
                UserId = _userId,
                Search = new SearchAuctionRegisted
                {
                    AuctionName = "Đấu giá vàng",
                    AuctionStartDate = new DateTime(2025, 1, 15),
                    AuctionEndDate = new DateTime(2025, 1, 20),
                },
            };

            _repoMock
                .Setup(r => r.GetRegisteredAssetIdsAsync(_userId))
                .ReturnsAsync(new List<Guid> { _assetId });

            _repoMock
                .Setup(r => r.GetRegisteredAuctionIdsAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(new List<Guid> { _auctionId });

            _repoMock
                .Setup(r => r.GetAuctionsByIdsAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(
                    new List<AuctionResponse>
                    {
                        new AuctionResponse
                        {
                            AuctionId = _auctionId,
                            AuctionName = "Đấu giá vàng",
                            CategoryName = "Vàng",
                            AuctionDescription = "Phiên đấu giá vàng miếng",
                            AuctionRules = "Không hoàn tiền cọc",
                            AuctionPlanningMap = "http://example.com/map.png",
                            RegisterOpenDate = new DateTime(2025, 1, 1),
                            RegisterEndDate = new DateTime(2025, 1, 10),
                            AuctionStartDate = new DateTime(2025, 1, 15),
                            AuctionEndDate = new DateTime(2025, 1, 20),
                            NumberRoundMax = 3,
                            Status = 1,
                        },
                    }
                );

            _repoMock
                .Setup(r => r.GetAuctionAssetsByAuctionIdAsync(_auctionId, It.IsAny<List<Guid>>()))
                .ReturnsAsync(
                    new List<AuctionAsset>
                    {
                        new AuctionAsset
                        {
                            AuctionAssetsId = _assetId,
                            TagName = "Lô vàng 1kg",
                            StartingPrice = 100000000m,
                            Unit = "VNĐ",
                            Deposit = 5000000m,
                            RegistrationFee = 200000m,
                            Description = "Vàng 9999",
                            AuctionId = _auctionId,
                        },
                    }
                );

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.That(result.PageNumber, Is.EqualTo(1));
            Assert.That(result.PageSize, Is.EqualTo(5));
            Assert.That(result.TotalAuctionRegisted, Is.EqualTo(1));

            var auction = result.AuctionResponse.First();
            Assert.That(auction.AuctionId, Is.EqualTo(_auctionId));
            Assert.That(auction.AuctionName, Is.EqualTo("Đấu giá vàng"));
            Assert.That(auction.CategoryName, Is.EqualTo("Vàng"));
            Assert.That(auction.AuctionDescription, Is.EqualTo("Phiên đấu giá vàng miếng"));
            Assert.That(auction.AuctionRules, Is.EqualTo("Không hoàn tiền cọc"));
            Assert.That(auction.AuctionPlanningMap, Is.EqualTo("http://example.com/map.png"));
            Assert.That(auction.RegisterOpenDate, Is.EqualTo(new DateTime(2025, 1, 1)));
            Assert.That(auction.RegisterEndDate, Is.EqualTo(new DateTime(2025, 1, 10)));
            Assert.That(auction.AuctionStartDate, Is.EqualTo(new DateTime(2025, 1, 15)));
            Assert.That(auction.AuctionEndDate, Is.EqualTo(new DateTime(2025, 1, 20)));
            Assert.That(auction.NumberRoundMax, Is.EqualTo(3));
            Assert.That(auction.Status, Is.EqualTo(1));

            var asset = auction.AuctionAssets.First();
            Assert.That(asset.AuctionAssetsId, Is.EqualTo(_assetId));
            Assert.That(asset.TagName, Is.EqualTo("Lô vàng 1kg"));
            Assert.That(asset.StartingPrice, Is.EqualTo(100000000m));
            Assert.That(asset.Unit, Is.EqualTo("VNĐ"));
            Assert.That(asset.Deposit, Is.EqualTo(5000000m));
            Assert.That(asset.RegistrationFee, Is.EqualTo(200000m));
            Assert.That(asset.Description, Is.EqualTo("Vàng 9999"));
            Assert.That(asset.AuctionId, Is.EqualTo(_auctionId));
        }
    }
}
