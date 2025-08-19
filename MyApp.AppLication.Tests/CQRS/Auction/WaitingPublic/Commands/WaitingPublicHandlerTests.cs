using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Hangfire;
using Moq;
using MyApp.Application.CQRS.Auction.WaitingPublic.Commands;
using MyApp.Application.Interfaces.IAuctionRepository;
using MyApp.Application.JobBackgroud.AuctionJob;
using NUnit.Framework;

namespace MyApp.Application.CQRS.Auction.WaitingPublic.Commands.Tests
{
    [TestFixture]
    public class WaitingPublicHandlerTests
    {
        private Mock<IAuctionRepository> _auctionRepoMock;
        private Mock<IBackgroundJobClient> _backgroundJobMock;
        private WaitingPublicHandler _handler;

        [SetUp]
        public void Setup()
        {
            _auctionRepoMock = new Mock<IAuctionRepository>();
            _backgroundJobMock = new Mock<IBackgroundJobClient>();
            _handler = new WaitingPublicHandler(_auctionRepoMock.Object, _backgroundJobMock.Object);
        }

        [Test]
        public async Task Handle_WhenWaitingPublicFails_ReturnsFalse()
        {
            _auctionRepoMock.Setup(x => x.WaitingPublicAsync(It.IsAny<Guid>())).ReturnsAsync(false);

            var result = await _handler.Handle(
                new WaitingPublicCommand { AuctionId = Guid.NewGuid() },
                default
            );

            Assert.IsFalse(result);
        }

        [Test]
        public async Task Handle_WhenAuctionIsNull_ReturnsFalse()
        {
            _auctionRepoMock.Setup(x => x.WaitingPublicAsync(It.IsAny<Guid>())).ReturnsAsync(true);
            _auctionRepoMock
                .Setup(x => x.FindAuctionByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((MyApp.Core.Entities.Auction)null);

            var result = await _handler.Handle(
                new WaitingPublicCommand { AuctionId = Guid.NewGuid() },
                default
            );

            Assert.IsFalse(result);
        }

        [Test]
        public async Task Handle_WhenRegisterOpenDateInPast_ReturnsFalse()
        {
            _auctionRepoMock.Setup(x => x.WaitingPublicAsync(It.IsAny<Guid>())).ReturnsAsync(true);
            _auctionRepoMock
                .Setup(x => x.FindAuctionByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(
                    new MyApp.Core.Entities.Auction
                    {
                        RegisterOpenDate = DateTime.Now.AddMinutes(-1),
                    }
                );

            var result = await _handler.Handle(
                new WaitingPublicCommand { AuctionId = Guid.NewGuid() },
                default
            );

            Assert.IsFalse(result);
        }

        [Test]
        public async Task Handle_WhenSuccess_ReturnsTrueAndSchedulesJob()
        {
            var auctionId = Guid.NewGuid();
            var registerOpenDate = DateTime.Now.AddMinutes(5);

            _auctionRepoMock.Setup(x => x.WaitingPublicAsync(auctionId)).ReturnsAsync(true);
            _auctionRepoMock
                .Setup(x => x.FindAuctionByIdAsync(auctionId))
                .ReturnsAsync(
                    new MyApp.Core.Entities.Auction { RegisterOpenDate = registerOpenDate }
                );

            var result = await _handler.Handle(
                new WaitingPublicCommand { AuctionId = auctionId },
                default
            );

            Assert.IsTrue(result);
            _backgroundJobMock.Verify(
                x =>
                    x.Schedule<SetAuctionStatus>(
                        It.IsAny<Expression<Func<SetAuctionStatus, Task>>>(),
                        registerOpenDate
                    ),
                Times.Once
            );
        }
    }
}
