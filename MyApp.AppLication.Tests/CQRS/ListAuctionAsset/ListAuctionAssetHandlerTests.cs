using Moq;
using MyApp.Application.Interfaces.ListAuctionAsset;

namespace MyApp.Application.CQRS.ListAuctionAsset.Tests
{
    [TestFixture()]
    public class ListAuctionAssetHandlerTests
    {
        private Mock<IListAuctionAssetRepository> _repositoryMock;
        private ListAuctionAssetHandler _handler;

        // Giá trị cố định để dùng trong test
        private static readonly Guid FixedAuctionAssetsId = Guid.Parse(
            "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"
        );
        private static readonly Guid FixedCreatedBy = Guid.Parse(
            "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"
        );
        private static readonly Guid FixedUpdatedBy = Guid.Parse(
            "cccccccc-cccc-cccc-cccc-cccccccccccc"
        );
        private static readonly Guid FixedAuctionId = Guid.Parse(
            "dddddddd-dddd-dddd-dddd-dddddddddddd"
        );
        private static readonly DateTime FixedCreatedAt = new DateTime(
            2023,
            1,
            1,
            10,
            0,
            0,
            DateTimeKind.Utc
        );
        private static readonly DateTime FixedUpdatedAt = new DateTime(
            2023,
            2,
            1,
            10,
            0,
            0,
            DateTimeKind.Utc
        );
        private static readonly DateTime FixedAuctionStart = new DateTime(
            2023,
            3,
            1,
            0,
            0,
            0,
            DateTimeKind.Utc
        );
        private static readonly DateTime FixedAuctionEnd = new DateTime(
            2023,
            3,
            10,
            0,
            0,
            0,
            DateTimeKind.Utc
        );

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IListAuctionAssetRepository>();
            _handler = new ListAuctionAssetHandler(_repositoryMock.Object);
        }

        [Test]
        public async Task Handle_WhenDataExists_ShouldReturnCorrectResponse()
        {
            // Arrange
            var request = new ListAuctionAssetRequest
            {
                PageNumber = 1,
                PageSize = 5,
                Search = new SearchAuctionAsset
                {
                    CategoryId = 2,
                    TagName = "Xe máy",
                    AuctionStartDate = FixedAuctionStart,
                    AuctionEndDate = FixedAuctionEnd,
                    AuctionStatus = 1,
                },
            };

            var fakeAsset = new AuctionAssetResponse
            {
                AuctionAssetsId = FixedAuctionAssetsId,
                TagName = "Xe máy Honda",
                StartingPrice = 1500m,
                Unit = "Chiếc",
                Deposit = 200m,
                RegistrationFee = 50m,
                Description = "Xe máy mới 100%",
                CreatedAt = FixedCreatedAt,
                CreatedBy = FixedCreatedBy,
                UpdatedAt = FixedUpdatedAt,
                UpdatedBy = FixedUpdatedBy,
                AuctionId = FixedAuctionId,
                AuctionName = "Phiên đấu giá xe",
                CategoryName = "Xe cộ",
            };

            var fakeAssets = new List<AuctionAssetResponse> { fakeAsset };
            var fakeCategoryCounts = new Dictionary<string, int> { { "Xe cộ", 1 } };

            _repositoryMock
                .Setup(r =>
                    r.GetAllAuctionAssets(request.PageNumber, request.PageSize, request.Search)
                )
                .ReturnsAsync(fakeAssets);

            _repositoryMock
                .Setup(r => r.GetTotalAuctionAssetsCount())
                .ReturnsAsync(fakeAssets.Count);

            _repositoryMock.Setup(r => r.GetCategoryCounts()).ReturnsAsync(fakeCategoryCounts);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.PageNumber, Is.EqualTo(1));
            Assert.That(result.PageSize, Is.EqualTo(5));
            Assert.That(result.TotalAuctionAsset, Is.EqualTo(1));

            Assert.That(result.AuctionAssetResponses, Has.Count.EqualTo(1));

            var actualAsset = result.AuctionAssetResponses[0];
            Assert.That(actualAsset.AuctionAssetsId, Is.EqualTo(FixedAuctionAssetsId));
            Assert.That(actualAsset.TagName, Is.EqualTo("Xe máy Honda"));
            Assert.That(actualAsset.StartingPrice, Is.EqualTo(1500m));
            Assert.That(actualAsset.Unit, Is.EqualTo("Chiếc"));
            Assert.That(actualAsset.Deposit, Is.EqualTo(200m));
            Assert.That(actualAsset.RegistrationFee, Is.EqualTo(50m));
            Assert.That(actualAsset.Description, Is.EqualTo("Xe máy mới 100%"));
            Assert.That(actualAsset.CreatedAt, Is.EqualTo(FixedCreatedAt));
            Assert.That(actualAsset.CreatedBy, Is.EqualTo(FixedCreatedBy));
            Assert.That(actualAsset.UpdatedAt, Is.EqualTo(FixedUpdatedAt));
            Assert.That(actualAsset.UpdatedBy, Is.EqualTo(FixedUpdatedBy));
            Assert.That(actualAsset.AuctionId, Is.EqualTo(FixedAuctionId));
            Assert.That(actualAsset.AuctionName, Is.EqualTo("Phiên đấu giá xe"));
            Assert.That(actualAsset.CategoryName, Is.EqualTo("Xe cộ"));

            Assert.That(result.CategoryCounts.ContainsKey("Xe cộ"), Is.True);
            Assert.That(result.CategoryCounts["Xe cộ"], Is.EqualTo(1));
        }

        [Test]
        public async Task Handle_WhenNoData_ShouldReturnEmptyListAndZeroCounts()
        {
            // Arrange
            var request = new ListAuctionAssetRequest
            {
                PageNumber = 2,
                PageSize = 10,
                Search = new SearchAuctionAsset
                {
                    CategoryId = 5,
                    TagName = "Bất động sản",
                    AuctionStartDate = FixedAuctionStart,
                    AuctionEndDate = FixedAuctionEnd,
                    AuctionStatus = 0,
                },
            };

            var fakeAssets = new List<AuctionAssetResponse>();
            var fakeCategoryCounts = new Dictionary<string, int>();

            _repositoryMock
                .Setup(r =>
                    r.GetAllAuctionAssets(request.PageNumber, request.PageSize, request.Search)
                )
                .ReturnsAsync(fakeAssets);

            _repositoryMock.Setup(r => r.GetTotalAuctionAssetsCount()).ReturnsAsync(0);

            _repositoryMock.Setup(r => r.GetCategoryCounts()).ReturnsAsync(fakeCategoryCounts);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.PageNumber, Is.EqualTo(2));
            Assert.That(result.PageSize, Is.EqualTo(10));
            Assert.That(result.TotalAuctionAsset, Is.EqualTo(0));
            Assert.That(result.AuctionAssetResponses, Is.Empty);
            Assert.That(result.CategoryCounts, Is.Empty);
        }
    }
}
