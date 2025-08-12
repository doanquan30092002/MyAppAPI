using System.Security.Claims;
using Hangfire;
using Hangfire.MemoryStorage;
using Moq;
using MyApp.Application.Common.CurrentUserService;
using MyApp.Application.Common.Message;
using MyApp.Application.Common.Services.NotificationHub;
using MyApp.Application.Interfaces.AssginAuctioneerAndPublicAuction;
using MyApp.Application.JobBackgroud.AuctionJob;

namespace MyApp.Application.CQRS.AssginAuctioneerAndPublicAuction.Command.Tests
{
    [TestFixture]
    public class AssginAuctioneerAndPublicAuctionHandlerTests
    {
        private Mock<IAssginAuctioneerAndPublicAuctionRepository> _repoMock;
        private Mock<ISetAuctionUpdateableFalse> _setAuctionJobMock;
        private Mock<INotificationSender> _notificationSenderMock;
        private Mock<ICurrentUserService> _currentUserServiceMock;
        private AssginAuctioneerAndPublicAuctionHandler _handler;
        private Guid _auctionId;
        private Guid _auctioneerId;

        [SetUp]
        public void Setup()
        {
            GlobalConfiguration.Configuration.UseMemoryStorage();

            _repoMock = new Mock<IAssginAuctioneerAndPublicAuctionRepository>();
            _setAuctionJobMock = new Mock<ISetAuctionUpdateableFalse>();
            _notificationSenderMock = new Mock<INotificationSender>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();

            _auctionId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            _auctioneerId = Guid.Parse("22222222-2222-2222-2222-222222222222");

            // Setup mặc định cho GetUserId()
            _currentUserServiceMock.Setup(s => s.GetUserId()).Returns(Guid.NewGuid().ToString());

            _handler = new AssginAuctioneerAndPublicAuctionHandler(
                _repoMock.Object,
                _currentUserServiceMock.Object,
                _setAuctionJobMock.Object,
                _notificationSenderMock.Object
            );
        }

        [Test]
        public async Task Handle_AuctionNotWaiting_Returns400()
        {
            _repoMock
                .Setup(r => r.CheckStatusAuctionIsWaitingAsync(_auctionId))
                .ReturnsAsync(false);

            var result = await _handler.Handle(
                new AssginAuctioneerAndPublicAuctionRequest
                {
                    AuctionId = _auctionId,
                    Auctioneer = _auctioneerId,
                },
                CancellationToken.None
            );

            Assert.AreEqual(400, result.Code);
            Assert.AreEqual(Message.AUCTION_NOT_WAITING, result.Message);
        }

        [Test]
        public async Task Handle_AuctioneerAssignedToAnother_Returns400()
        {
            _repoMock.Setup(r => r.CheckStatusAuctionIsWaitingAsync(_auctionId)).ReturnsAsync(true);
            _repoMock
                .Setup(r =>
                    r.CheckAuctioneerAssignedToAnotherAuctionAsync(_auctioneerId, _auctionId)
                )
                .ReturnsAsync(true);

            var result = await _handler.Handle(
                new AssginAuctioneerAndPublicAuctionRequest
                {
                    AuctionId = _auctionId,
                    Auctioneer = _auctioneerId,
                },
                CancellationToken.None
            );

            Assert.AreEqual(400, result.Code);
            Assert.AreEqual(Message.AUCTIONEER_ASSIGNED_ANOTHER_AUCTION, result.Message);
        }

        [Test]
        public async Task Handle_SaveNotificationFails_Returns500()
        {
            _repoMock.Setup(r => r.CheckStatusAuctionIsWaitingAsync(_auctionId)).ReturnsAsync(true);
            _repoMock
                .Setup(r =>
                    r.CheckAuctioneerAssignedToAnotherAuctionAsync(_auctioneerId, _auctionId)
                )
                .ReturnsAsync(false);
            _repoMock
                .Setup(r =>
                    r.AssignAuctioneerToAuctionAndPublicAuctionAsync(
                        _auctionId,
                        _auctioneerId,
                        It.IsAny<string>()
                    )
                )
                .ReturnsAsync((true, DateTime.Now.AddMinutes(5).ToString(), "AuctionName"));
            _repoMock
                .Setup(r => r.GetAllUserIdRoleCustomer())
                .ReturnsAsync(new List<Guid> { Guid.NewGuid() });
            _repoMock
                .Setup(r => r.SaveNotificationAsync(It.IsAny<List<Guid>>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            var result = await _handler.Handle(
                new AssginAuctioneerAndPublicAuctionRequest
                {
                    AuctionId = _auctionId,
                    Auctioneer = _auctioneerId,
                },
                CancellationToken.None
            );

            Assert.AreEqual(500, result.Code);
            Assert.AreEqual(Message.SYSTEM_ERROR, result.Message);
        }

        [Test]
        public async Task Handle_AssignFails_Returns500()
        {
            _repoMock.Setup(r => r.CheckStatusAuctionIsWaitingAsync(_auctionId)).ReturnsAsync(true);
            _repoMock
                .Setup(r =>
                    r.CheckAuctioneerAssignedToAnotherAuctionAsync(_auctioneerId, _auctionId)
                )
                .ReturnsAsync(false);
            _repoMock
                .Setup(r =>
                    r.AssignAuctioneerToAuctionAndPublicAuctionAsync(
                        _auctionId,
                        _auctioneerId,
                        It.IsAny<string>()
                    )
                )
                .ReturnsAsync((false, "", ""));

            var result = await _handler.Handle(
                new AssginAuctioneerAndPublicAuctionRequest
                {
                    AuctionId = _auctionId,
                    Auctioneer = _auctioneerId,
                },
                CancellationToken.None
            );

            Assert.AreEqual(500, result.Code);
            Assert.AreEqual(Message.SYSTEM_ERROR, result.Message);
        }

        [Test]
        public async Task Handle_AssignSuccessWithDelay_SchedulesJob()
        {
            _repoMock.Setup(r => r.CheckStatusAuctionIsWaitingAsync(_auctionId)).ReturnsAsync(true);
            _repoMock
                .Setup(r =>
                    r.CheckAuctioneerAssignedToAnotherAuctionAsync(_auctioneerId, _auctionId)
                )
                .ReturnsAsync(false);
            _repoMock
                .Setup(r =>
                    r.AssignAuctioneerToAuctionAndPublicAuctionAsync(
                        _auctionId,
                        _auctioneerId,
                        It.IsAny<string>()
                    )
                )
                .ReturnsAsync((true, DateTime.Now.AddMinutes(1).ToString(), "AuctionName"));
            _repoMock
                .Setup(r => r.GetAllUserIdRoleCustomer())
                .ReturnsAsync(new List<Guid> { Guid.NewGuid() });
            _repoMock
                .Setup(r => r.SaveNotificationAsync(It.IsAny<List<Guid>>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            var result = await _handler.Handle(
                new AssginAuctioneerAndPublicAuctionRequest
                {
                    AuctionId = _auctionId,
                    Auctioneer = _auctioneerId,
                },
                CancellationToken.None
            );

            Assert.AreEqual(200, result.Code);
            Assert.AreEqual(Message.ASSGIN_AUCTIONEER_AND_PUBLIC_AUCTION_SUCCESS, result.Message);
        }

        [Test]
        public async Task Handle_AssignSuccessWithoutDelay_CallsSetAuctionUpdateableFalse()
        {
            _repoMock.Setup(r => r.CheckStatusAuctionIsWaitingAsync(_auctionId)).ReturnsAsync(true);
            _repoMock
                .Setup(r =>
                    r.CheckAuctioneerAssignedToAnotherAuctionAsync(_auctioneerId, _auctionId)
                )
                .ReturnsAsync(false);
            _repoMock
                .Setup(r =>
                    r.AssignAuctioneerToAuctionAndPublicAuctionAsync(
                        _auctionId,
                        _auctioneerId,
                        It.IsAny<string>()
                    )
                )
                .ReturnsAsync((true, DateTime.Now.AddMinutes(-1).ToString(), "AuctionName"));
            _repoMock
                .Setup(r => r.GetAllUserIdRoleCustomer())
                .ReturnsAsync(new List<Guid> { Guid.NewGuid() });
            _repoMock
                .Setup(r => r.SaveNotificationAsync(It.IsAny<List<Guid>>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            var result = await _handler.Handle(
                new AssginAuctioneerAndPublicAuctionRequest
                {
                    AuctionId = _auctionId,
                    Auctioneer = _auctioneerId,
                },
                CancellationToken.None
            );

            _setAuctionJobMock.Verify(
                j => j.SetAuctionUpdateableFalseAsync(_auctionId),
                Times.Once
            );
            Assert.AreEqual(200, result.Code);
            Assert.AreEqual(Message.ASSGIN_AUCTIONEER_AND_PUBLIC_AUCTION_SUCCESS, result.Message);
        }

        [Test]
        public async Task Handle_UserIdIsNull_Returns401()
        {
            // Arrange
            _currentUserServiceMock.Setup(s => s.GetUserId()).Returns((string)null);

            var result = await _handler.Handle(
                new AssginAuctioneerAndPublicAuctionRequest
                {
                    AuctionId = _auctionId,
                    Auctioneer = _auctioneerId,
                },
                CancellationToken.None
            );

            Assert.AreEqual(401, result.Code);
            Assert.AreEqual(Message.UNAUTHORIZED, result.Message);
        }

        [Test]
        public void Handle_SendNotificationThrows_ExceptionBubblesUp()
        {
            _repoMock.Setup(r => r.CheckStatusAuctionIsWaitingAsync(_auctionId)).ReturnsAsync(true);
            _repoMock
                .Setup(r =>
                    r.CheckAuctioneerAssignedToAnotherAuctionAsync(_auctioneerId, _auctionId)
                )
                .ReturnsAsync(false);
            _repoMock
                .Setup(r =>
                    r.AssignAuctioneerToAuctionAndPublicAuctionAsync(
                        _auctionId,
                        _auctioneerId,
                        It.IsAny<string>()
                    )
                )
                .ReturnsAsync((true, DateTime.Now.AddMinutes(5).ToString(), "AuctionName"));
            _repoMock
                .Setup(r => r.GetAllUserIdRoleCustomer())
                .ReturnsAsync(new List<Guid> { Guid.NewGuid() });

            _notificationSenderMock
                .Setup(s => s.SendToUsersAsync(It.IsAny<List<Guid>>(), It.IsAny<object>()))
                .ThrowsAsync(new Exception("Notification error"));

            Assert.ThrowsAsync<Exception>(async () =>
            {
                await _handler.Handle(
                    new AssginAuctioneerAndPublicAuctionRequest
                    {
                        AuctionId = _auctionId,
                        Auctioneer = _auctioneerId,
                    },
                    CancellationToken.None
                );
            });
        }
    }
}
