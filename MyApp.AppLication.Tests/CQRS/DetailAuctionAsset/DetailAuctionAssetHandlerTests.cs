using Moq;
using MyApp.Application.Interfaces.DetailAuctionAsset;

namespace MyApp.Application.CQRS.DetailAuctionAsset.Tests
{
    [TestFixture()]
    public class DetailAuctionAssetHandlerTests
    {
        private Mock<IDetailAuctionAssetRepository> _repositoryMock;
        private DetailAuctionAssetHandler _handler;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IDetailAuctionAssetRepository>();
            _handler = new DetailAuctionAssetHandler(_repositoryMock.Object);
        }

        [Test]
        public async Task Handle_ShouldReturnFullData_WhenAuctionAssetExists()
        {
            // 🇻🇳 Chuẩn bị dữ liệu mô phỏng
            var auctionAssetsId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
            var fakeAuctionAsset = new AuctionAssetResponse
            {
                AuctionAssetsId = auctionAssetsId,
                TagName = "Xe máy SH 150i",
                StartingPrice = 100_000_000m,
                Unit = "Chiếc",
                Deposit = 10_000_000m,
                RegistrationFee = 500_000m,
                Description = "Xe mới 100%, chưa qua sử dụng",
                CreatedAt = new DateTime(2024, 12, 1, 10, 0, 0),
                AuctionName = "Phiên đấu giá tháng 12",
            };

            _repositoryMock
                .Setup(r => r.GetAuctionAssetByIdAsync(auctionAssetsId))
                .ReturnsAsync(fakeAuctionAsset);

            _repositoryMock
                .Setup(r => r.GetTotalAuctionDocumentsAsync(auctionAssetsId))
                .ReturnsAsync(5);

            _repositoryMock
                .Setup(r => r.GetTotalRegistrationFeeAsync(auctionAssetsId))
                .ReturnsAsync(2_500_000m);

            _repositoryMock
                .Setup(r => r.GetTotalDepositAsync(auctionAssetsId))
                .ReturnsAsync(50_000_000m);

            var request = new DetailAuctionAssetRequest { AuctionAssetsId = auctionAssetsId };

            // 🇻🇳 Thực thi Handler
            var result = await _handler.Handle(request, CancellationToken.None);

            // 🇻🇳 Kiểm tra đầy đủ các field trả về
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.AuctionAssetResponse);
            Assert.AreEqual(auctionAssetsId, result.AuctionAssetResponse.AuctionAssetsId);
            Assert.AreEqual("Xe máy SH 150i", result.AuctionAssetResponse.TagName);
            Assert.AreEqual(100_000_000m, result.AuctionAssetResponse.StartingPrice);
            Assert.AreEqual("Chiếc", result.AuctionAssetResponse.Unit);
            Assert.AreEqual(10_000_000m, result.AuctionAssetResponse.Deposit);
            Assert.AreEqual(500_000m, result.AuctionAssetResponse.RegistrationFee);
            Assert.AreEqual(
                "Xe mới 100%, chưa qua sử dụng",
                result.AuctionAssetResponse.Description
            );
            Assert.AreEqual(
                new DateTime(2024, 12, 1, 10, 0, 0),
                result.AuctionAssetResponse.CreatedAt
            );
            Assert.AreEqual("Phiên đấu giá tháng 12", result.AuctionAssetResponse.AuctionName);

            Assert.AreEqual(5, result.TotalAuctionDocument);
            Assert.AreEqual(2_500_000m, result.TotalRegistrationFee);
            Assert.AreEqual(50_000_000m, result.TotalDeposit);
        }

        [Test]
        public async Task Handle_ShouldReturnEmptyData_WhenAuctionAssetNotFound()
        {
            // 🇻🇳 Chuẩn bị dữ liệu mô phỏng: AuctionAsset không tồn tại
            var auctionAssetsId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

            _repositoryMock
                .Setup(r => r.GetAuctionAssetByIdAsync(auctionAssetsId))
                .ReturnsAsync((AuctionAssetResponse)null);

            var request = new DetailAuctionAssetRequest { AuctionAssetsId = auctionAssetsId };

            // 🇻🇳 Thực thi Handler
            var result = await _handler.Handle(request, CancellationToken.None);

            // 🇻🇳 Kiểm tra kết quả trả về rỗng
            Assert.IsNotNull(result);
            Assert.IsNull(result.AuctionAssetResponse);
            Assert.AreEqual(0, result.TotalAuctionDocument);
            Assert.AreEqual(0m, result.TotalRegistrationFee);
            Assert.AreEqual(0m, result.TotalDeposit);
        }
    }
}
