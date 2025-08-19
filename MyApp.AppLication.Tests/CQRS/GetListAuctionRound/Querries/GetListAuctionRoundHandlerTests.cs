using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MyApp.Application.CQRS.GetListAuctionRound.Querries;
using MyApp.Application.Interfaces.IGetListAuctionRoundRepository;
using MyApp.Core.Entities;
using NUnit.Framework;

namespace MyApp.Application.CQRS.GetListAuctionRound.Querries.Tests
{
    [TestFixture]
    public class GetListAuctionRoundHandlerTests
    {
        // Fake repository to simulate IGetListAuctionRoundRepository behavior without Moq
        private class FakeGetListAuctionRoundRepository : IGetListAuctionRoundRepository
        {
            private readonly List<AuctionRound> _auctionRounds;
            private readonly bool _returnEmpty;

            public FakeGetListAuctionRoundRepository(
                List<AuctionRound> auctionRounds = null,
                bool returnEmpty = false
            )
            {
                _auctionRounds = auctionRounds ?? new List<AuctionRound>();
                _returnEmpty = returnEmpty;
            }

            public Task<List<AuctionRound>> GetAuctionRoundsByAuctionIdAsync(Guid auctionId)
            {
                return Task.FromResult(_returnEmpty ? new List<AuctionRound>() : _auctionRounds);
            }
        }

        private GetListAuctionRoundHandler _handler;
        private GetListAuctionRoundRequest _request;
        private FakeGetListAuctionRoundRepository _repository;

        [SetUp]
        public void SetUp()
        {
            _request = new GetListAuctionRoundRequest { AuctionId = Guid.NewGuid() };
            _repository = new FakeGetListAuctionRoundRepository();
            _handler = new GetListAuctionRoundHandler(_repository);
        }

        [Test]
        public void Constructor_NullRepository_ThrowsArgumentNullException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() =>
                new GetListAuctionRoundHandler(null)
            );
            Assert.That(ex.ParamName, Is.EqualTo("getListAuctionRoundRepository"));
        }

        [Test]
        public async Task Handle_AuctionRoundsExist_ReturnsPopulatedGetListAuctionRoundResponse()
        {
            // Arrange
            var auctionRounds = new List<AuctionRound>
            {
                new AuctionRound
                {
                    AuctionRoundId = Guid.NewGuid(),
                    AuctionId = _request.AuctionId,
                    RoundNumber = 1,
                    Status = 2,
                },
                new AuctionRound
                {
                    AuctionRoundId = Guid.NewGuid(),
                    AuctionId = _request.AuctionId,
                    RoundNumber = 2,
                    Status = 1,
                },
            };
            _repository = new FakeGetListAuctionRoundRepository(auctionRounds);
            _handler = new GetListAuctionRoundHandler(_repository);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<GetListAuctionRoundResponse>(result);
            Assert.IsNotNull(result.AuctionRounds);
            Assert.That(result.AuctionRounds.Count, Is.EqualTo(auctionRounds.Count));
            for (int i = 0; i < auctionRounds.Count; i++)
            {
                Assert.That(
                    result.AuctionRounds[i].AuctionRoundId,
                    Is.EqualTo(auctionRounds[i].AuctionRoundId)
                );
                Assert.That(
                    result.AuctionRounds[i].AuctionId,
                    Is.EqualTo(auctionRounds[i].AuctionId)
                );
                Assert.That(
                    result.AuctionRounds[i].RoundNumber,
                    Is.EqualTo(auctionRounds[i].RoundNumber)
                );
                Assert.That(result.AuctionRounds[i].Status, Is.EqualTo(auctionRounds[i].Status));
            }
        }

        [Test]
        public async Task Handle_NoAuctionRounds_ReturnsEmptyGetListAuctionRoundResponse()
        {
            // Arrange
            _repository = new FakeGetListAuctionRoundRepository(returnEmpty: true);
            _handler = new GetListAuctionRoundHandler(_repository);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<GetListAuctionRoundResponse>(result);
            Assert.IsNotNull(result.AuctionRounds);
            Assert.That(result.AuctionRounds.Count, Is.EqualTo(0));
        }
    }
}
