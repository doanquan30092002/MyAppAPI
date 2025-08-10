using Moq;
using MyApp.Application.Common.Message;
using MyApp.Application.Interfaces.UserRegisteredAuction;
using MyApp.Core.Entities;

namespace MyApp.Application.CQRS.UserRegisteredAuction.Tests
{
    [TestFixture()]
    public class UserRegisteredAuctionHandlerTests
    {
        private Mock<IUserRegisteredAuctionRepository> _repoMock;
        private UserRegisteredAuctionHandler _handler;

        private User _fakeUser;
        private List<AuctionAsset> _fakeAssets;

        // 🔹 Dùng Guid cố định để test luôn cho ra kết quả như nhau
        private readonly Guid _fixedUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        private readonly Guid _fixedAuctionId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        private readonly Guid _fixedAuctionRoundId = Guid.Parse(
            "33333333-3333-3333-3333-333333333333"
        );
        private readonly Guid _fixedAssetId1 = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        private readonly Guid _fixedAssetId2 = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

        [SetUp]
        public void Setup()
        {
            _repoMock = new Mock<IUserRegisteredAuctionRepository>();
            _handler = new UserRegisteredAuctionHandler(_repoMock.Object);

            _fakeUser = new User
            {
                Id = _fixedUserId,
                Name = "Quan",
                CitizenIdentification = "123456789012",
                RecentLocation = "Hanoi",
            };

            _fakeAssets = new List<AuctionAsset>
            {
                new AuctionAsset
                {
                    AuctionAssetsId = _fixedAssetId1,
                    TagName = "B1",
                    StartingPrice = 1000,
                },
                new AuctionAsset
                {
                    AuctionAssetsId = _fixedAssetId2,
                    TagName = "B2",
                    StartingPrice = 2000,
                },
            };
        }

        [Test]
        public async Task Handle_ShouldReturn404_WhenUserDoesNotExist()
        {
            _repoMock
                .Setup(r => r.GetUserByCitizenIdAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);

            var request = new UserRegisteredAuctionRequest
            {
                CitizenIdentification = "123456789012",
                AuctionId = _fixedAuctionId,
            };

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.AreEqual(404, result.Code);
            Assert.AreEqual(Message.CITIZEN_NOT_EXIST, result.Message);
        }

        [Test]
        public async Task Handle_ShouldReturn404_WhenNoAuctionAssets()
        {
            _repoMock
                .Setup(r => r.GetUserByCitizenIdAsync(It.IsAny<string>()))
                .ReturnsAsync(_fakeUser);

            _repoMock
                .Setup(r => r.GetValidAuctionAssetsAsync(_fakeUser.Id, _fixedAuctionId))
                .ReturnsAsync(new List<AuctionAsset>());

            var request = new UserRegisteredAuctionRequest
            {
                CitizenIdentification = _fakeUser.CitizenIdentification,
                AuctionId = _fixedAuctionId,
            };

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.AreEqual(404, result.Code);
            Assert.AreEqual(Message.USER_NOT_REGISTERED_OR_INELIGIBLE_PARTICIPATE, result.Message);
        }

        [Test]
        public async Task Handle_ShouldReturn200_WhenAuctionRoundIdIsNull()
        {
            _repoMock
                .Setup(r => r.GetUserByCitizenIdAsync(It.IsAny<string>()))
                .ReturnsAsync(_fakeUser);

            _repoMock
                .Setup(r => r.GetValidAuctionAssetsAsync(_fakeUser.Id, _fixedAuctionId))
                .ReturnsAsync(_fakeAssets);

            var request = new UserRegisteredAuctionRequest
            {
                CitizenIdentification = _fakeUser.CitizenIdentification,
                AuctionId = _fixedAuctionId,
                AuctionRoundId = null,
            };

            var result = await _handler.Handle(request, CancellationToken.None);

            // Level 1: DTO
            Assert.AreEqual(200, result.Code);
            Assert.AreEqual(Message.GET_USER_REGISTERED_AUCTION_SUCCESS, result.Message);
            Assert.IsNotNull(result.Data);

            // Level 2: UserRegisteredAuctionResponse
            var data = result.Data!;
            Assert.AreEqual(_fakeUser.Id, data.Id);
            Assert.AreEqual(_fakeUser.CitizenIdentification, data.CitizenIdentification);
            Assert.AreEqual(_fakeUser.Name, data.Name);
            Assert.AreEqual(_fakeUser.RecentLocation, data.RecentLocation);
            Assert.AreEqual(2, data.AuctionAssets.Count);

            // Level 3: AuctionAsset
            Assert.AreEqual(_fakeAssets[0].AuctionAssetsId, data.AuctionAssets[0].AuctionAssetsId);
            Assert.AreEqual(_fakeAssets[0].TagName, data.AuctionAssets[0].TagName);
            Assert.AreEqual(_fakeAssets[0].StartingPrice, data.AuctionAssets[0].StartingPrice);
        }

        [Test]
        public async Task Handle_ShouldReturn404_WhenNoNextRoundTagNames()
        {
            _repoMock
                .Setup(r => r.GetUserByCitizenIdAsync(It.IsAny<string>()))
                .ReturnsAsync(_fakeUser);

            _repoMock
                .Setup(r => r.GetValidAuctionAssetsAsync(_fakeUser.Id, _fixedAuctionId))
                .ReturnsAsync(_fakeAssets);

            _repoMock
                .Setup(r =>
                    r.GetNextRoundTagNamesForUserAsync(
                        _fixedAuctionRoundId,
                        _fakeUser.CitizenIdentification
                    )
                )
                .ReturnsAsync(new List<(string, decimal)>());

            var request = new UserRegisteredAuctionRequest
            {
                CitizenIdentification = _fakeUser.CitizenIdentification,
                AuctionId = _fixedAuctionId,
                AuctionRoundId = _fixedAuctionRoundId,
            };

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.AreEqual(404, result.Code);
            Assert.AreEqual(Message.USER_NOT_REGISTERED_OR_INELIGIBLE_PARTICIPATE, result.Message);
        }

        [Test]
        public async Task Handle_ShouldReturn200_WhenHasNextRoundTagNames()
        {
            _repoMock
                .Setup(r => r.GetUserByCitizenIdAsync(It.IsAny<string>()))
                .ReturnsAsync(_fakeUser);

            _repoMock
                .Setup(r => r.GetValidAuctionAssetsAsync(_fakeUser.Id, _fixedAuctionId))
                .ReturnsAsync(_fakeAssets);

            var nextRoundData = new List<(string, decimal)> { ("B1", 9999m) };

            _repoMock
                .Setup(r =>
                    r.GetNextRoundTagNamesForUserAsync(
                        _fixedAuctionRoundId,
                        _fakeUser.CitizenIdentification
                    )
                )
                .ReturnsAsync(nextRoundData);

            var request = new UserRegisteredAuctionRequest
            {
                CitizenIdentification = _fakeUser.CitizenIdentification,
                AuctionId = _fixedAuctionId,
                AuctionRoundId = _fixedAuctionRoundId,
            };

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.AreEqual(200, result.Code);
            Assert.AreEqual(Message.GET_USER_REGISTERED_AUCTION_SUCCESS, result.Message);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(1, result.Data.AuctionAssets.Count);
            Assert.AreEqual(9999m, result.Data.AuctionAssets[0].StartingPrice);
        }
    }
}
