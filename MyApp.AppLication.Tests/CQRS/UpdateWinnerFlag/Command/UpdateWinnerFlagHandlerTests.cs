using System;
using System.Threading;
using System.Threading.Tasks;
using MyApp.Application.CQRS.UpdateFlagWinner.Command;
using MyApp.Application.Interfaces.IUpdateWinnerFlagRepository;
using NUnit.Framework;

namespace MyApp.Application.CQRS.UpdateFlagWinner.Command.Tests
{
    public class FakeUpdateWinnerFlagRepository : IUpdateWinnerFlagRepository
    {
        private readonly Func<Guid, Task<bool>> _updateFunc;

        public FakeUpdateWinnerFlagRepository(Func<Guid, Task<bool>> updateFunc)
        {
            _updateFunc = updateFunc;
        }

        public Task<bool> UpdateWinnerFlagAsync(Guid auctionRoundPriceId)
        {
            return _updateFunc(auctionRoundPriceId);
        }
    }

    [TestFixture]
    public class UpdateWinnerFlagHandlerTests
    {
        [Test]
        public async Task Handle_ShouldReturnTrue_WhenUpdateIsSuccessful()
        {
            // Arrange
            var fakeRepo = new FakeUpdateWinnerFlagRepository(_ => Task.FromResult(true));
            var handler = new UpdateWinnerFlagHandler(fakeRepo);
            var request = new UpdateWinnerFlagRequest { AuctionRoundPriceId = Guid.NewGuid() };

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.StatusUpdate);
        }

        [Test]
        public async Task Handle_ShouldReturnFalse_WhenUpdateFails()
        {
            // Arrange
            var fakeRepo = new FakeUpdateWinnerFlagRepository(_ => Task.FromResult(false));
            var handler = new UpdateWinnerFlagHandler(fakeRepo);
            var request = new UpdateWinnerFlagRequest { AuctionRoundPriceId = Guid.NewGuid() };

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.StatusUpdate);
        }

        [Test]
        public async Task Handle_ShouldReturnFalse_WhenRepositoryThrowsException()
        {
            // Arrange
            var fakeRepo = new FakeUpdateWinnerFlagRepository(_ => throw new Exception("DB error"));
            var handler = new UpdateWinnerFlagHandler(fakeRepo);
            var request = new UpdateWinnerFlagRequest { AuctionRoundPriceId = Guid.NewGuid() };

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.StatusUpdate);
        }
    }
}
