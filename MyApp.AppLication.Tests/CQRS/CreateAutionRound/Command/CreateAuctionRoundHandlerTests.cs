using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using MyApp.Application.CQRS.CreateAutionRound.Command;
using MyApp.Application.Interfaces.ICreateAuctionRoundRepository;
using NUnit.Framework;

namespace MyApp.Application.CQRS.CreateAutionRound.Command.Tests
{
    // Fake repository để test
    public class FakeCreateAuctionRoundRepository : ICreateAuctionRoundRepository
    {
        private readonly bool _shouldSucceed;

        public FakeCreateAuctionRoundRepository(bool shouldSucceed)
        {
            _shouldSucceed = shouldSucceed;
        }

        public Task<bool> InsertAuctionRound(CreateAuctionRoundRequest createAuctionRoundRequest)
        {
            return Task.FromResult(_shouldSucceed);
        }
    }

    [TestFixture]
    public class CreateAuctionRoundHandlerTests
    {
        [Test]
        public async Task Handle_ShouldReturnResponse_WhenInsertSucceeds()
        {
            // Arrange
            var fakeRepo = new FakeCreateAuctionRoundRepository(true);
            var handler = new CreateAuctionRoundHandler(fakeRepo);

            var request = new CreateAuctionRoundRequest
            {
                AuctionId = Guid.NewGuid(),
                CreatedBy = Guid.NewGuid(),
            };

            // Act
            var response = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsInstanceOf<CreateAuctionRoundResponse>(response);
        }

        [Test]
        public void Handle_ShouldThrowValidationException_WhenInsertFails()
        {
            // Arrange
            var fakeRepo = new FakeCreateAuctionRoundRepository(false);
            var handler = new CreateAuctionRoundHandler(fakeRepo);

            var request = new CreateAuctionRoundRequest
            {
                AuctionId = Guid.NewGuid(),
                CreatedBy = Guid.NewGuid(),
            };

            // Act + Assert
            Assert.ThrowsAsync<ValidationException>(async () =>
                await handler.Handle(request, CancellationToken.None)
            );
        }
    }
}
