using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using MyApp.Application.Common.CurrentUserService;
using MyApp.Application.CQRS.Auction.GetListAuction.Querries;
using MyApp.Application.CQRS.Auction.GetListAution.Querries;
using MyApp.Application.Interfaces.IGetListRepository;
using NUnit.Framework;

namespace MyApp.Application.CQRS.Auction.GetListAuction.Querries.Tests
{
    [TestFixture]
    public class GetListAuctionHandlerTests
    {
        private Mock<IGetListRepository> _repoMock;
        private Mock<ICurrentUserService> _currentUserMock;
        private GetListAuctionHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _repoMock = new Mock<IGetListRepository>(MockBehavior.Strict);
            _currentUserMock = new Mock<ICurrentUserService>(MockBehavior.Strict);
            _handler = new GetListAuctionHandler(_repoMock.Object, _currentUserMock.Object);
        }

        private static GetListAuctionResponse FakeResponse(int count = 1)
        {
            var list = new List<ListAuctionDTO>();
            for (int i = 0; i < count; i++)
            {
                list.Add(
                    new ListAuctionDTO
                    {
                        AuctionId = Guid.NewGuid(),
                        AuctionName = $"Auction {i + 1}",
                        CategoryId = 1,
                        Status = 1,
                        RegisterOpenDate = DateTime.UtcNow.AddDays(-2),
                        RegisterEndDate = DateTime.UtcNow.AddDays(2),
                        AuctionStartDate = DateTime.UtcNow.AddDays(3),
                        AuctionEndDate = DateTime.UtcNow.AddDays(4),
                        CreatedByUserName = "creator",
                        UpdateByUserName = "updater",
                        AuctioneerBy = "auctioneer",
                    }
                );
            }

            return new GetListAuctionResponse { TotalCount = count, Auctions = list };
        }

        [Test]
        public async Task Handle_WhenRoleIsAuctioneer_WithUserId_ShouldCallRepositoryWithUserId()
        {
            // Arrange
            var request = new GetListAuctionRequest { AuctionName = "ABC" };
            var userId = Guid.NewGuid().ToString();

            _currentUserMock.Setup(x => x.GetUserId()).Returns(userId);
            _currentUserMock.Setup(x => x.GetRole()).Returns("Auctioneer");

            _repoMock
                .Setup(r =>
                    r.GetListAuctionsAsync(It.IsAny<GetListAuctionRequest>(), It.IsAny<string?>())
                )
                .ReturnsAsync(FakeResponse(2));

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.TotalCount, Is.EqualTo(2));

            _repoMock.Verify(
                r =>
                    r.GetListAuctionsAsync(
                        It.Is<GetListAuctionRequest>(req => req == request),
                        It.Is<string?>(uid => uid == userId)
                    ),
                Times.Once
            );

            _currentUserMock.Verify(x => x.GetUserId(), Times.Once);
            _currentUserMock.Verify(x => x.GetRole(), Times.Once);
            _repoMock.VerifyNoOtherCalls();
            _currentUserMock.VerifyNoOtherCalls();
        }

        [Test]
        public async Task Handle_WhenRoleIsAuctioneer_ButUserIdIsNull_ShouldPassEmptyString()
        {
            // Arrange
            var request = new GetListAuctionRequest { Status = 1 };

            _currentUserMock.Setup(x => x.GetUserId()).Returns((string?)null); // handler -> string.Empty
            _currentUserMock.Setup(x => x.GetRole()).Returns("Auctioneer");

            _repoMock
                .Setup(r =>
                    r.GetListAuctionsAsync(It.IsAny<GetListAuctionRequest>(), It.IsAny<string?>())
                )
                .ReturnsAsync(FakeResponse(3));

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.TotalCount, Is.EqualTo(3));

            _repoMock.Verify(
                r =>
                    r.GetListAuctionsAsync(
                        It.Is<GetListAuctionRequest>(req => req == request),
                        It.Is<string?>(uid => uid == string.Empty)
                    ),
                Times.Once
            );

            _currentUserMock.Verify(x => x.GetUserId(), Times.Once);
            _currentUserMock.Verify(x => x.GetRole(), Times.Once);
            _repoMock.VerifyNoOtherCalls();
            _currentUserMock.VerifyNoOtherCalls();
        }

        [Test]
        public async Task Handle_WhenRoleIsAuctioneer_ButUserIdIsEmpty_ShouldPassEmptyString()
        {
            // Arrange
            var request = new GetListAuctionRequest { Status = 2 };

            _currentUserMock.Setup(x => x.GetUserId()).Returns(string.Empty); // vẫn empty
            _currentUserMock.Setup(x => x.GetRole()).Returns("Auctioneer");

            _repoMock
                .Setup(r =>
                    r.GetListAuctionsAsync(It.IsAny<GetListAuctionRequest>(), It.IsAny<string?>())
                )
                .ReturnsAsync(FakeResponse(4));

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.TotalCount, Is.EqualTo(4));

            _repoMock.Verify(
                r =>
                    r.GetListAuctionsAsync(
                        It.Is<GetListAuctionRequest>(req => req == request),
                        It.Is<string?>(uid => uid == string.Empty)
                    ),
                Times.Once
            );

            _currentUserMock.Verify(x => x.GetUserId(), Times.Once);
            _currentUserMock.Verify(x => x.GetRole(), Times.Once);
            _repoMock.VerifyNoOtherCalls();
            _currentUserMock.VerifyNoOtherCalls();
        }

        [Test]
        public async Task Handle_WhenRoleIsNotAuctioneer_ShouldCallRepositoryWithoutUserId()
        {
            // Arrange
            var request = new GetListAuctionRequest { CategoryId = 5 };

            _currentUserMock.Setup(x => x.GetUserId()).Returns(Guid.NewGuid().ToString());
            _currentUserMock.Setup(x => x.GetRole()).Returns("Admin");

            _repoMock
                .Setup(r =>
                    r.GetListAuctionsAsync(It.IsAny<GetListAuctionRequest>(), It.IsAny<string?>())
                )
                .ReturnsAsync(FakeResponse(1));

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.TotalCount, Is.EqualTo(1));

            _repoMock.Verify(
                r =>
                    r.GetListAuctionsAsync(
                        It.Is<GetListAuctionRequest>(req => req == request),
                        It.Is<string?>(uid => uid == null)
                    ),
                Times.Once
            );

            _currentUserMock.Verify(x => x.GetUserId(), Times.Once);
            _currentUserMock.Verify(x => x.GetRole(), Times.Once);
            _repoMock.VerifyNoOtherCalls();
            _currentUserMock.VerifyNoOtherCalls();
        }

        [Test]
        public async Task Handle_WhenRoleIsNull_ShouldCallRepositoryWithoutUserId()
        {
            // Arrange
            var request = new GetListAuctionRequest { PageNumber = 1 };

            _currentUserMock.Setup(x => x.GetUserId()).Returns(Guid.NewGuid().ToString());
            _currentUserMock.Setup(x => x.GetRole()).Returns((string?)null);

            _repoMock
                .Setup(r =>
                    r.GetListAuctionsAsync(It.IsAny<GetListAuctionRequest>(), It.IsAny<string?>())
                )
                .ReturnsAsync(FakeResponse(1));

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.TotalCount, Is.EqualTo(1));

            _repoMock.Verify(
                r =>
                    r.GetListAuctionsAsync(
                        It.Is<GetListAuctionRequest>(req => req == request),
                        It.Is<string?>(uid => uid == null)
                    ),
                Times.Once
            );

            _currentUserMock.Verify(x => x.GetUserId(), Times.Once);
            _currentUserMock.Verify(x => x.GetRole(), Times.Once);
            _repoMock.VerifyNoOtherCalls();
            _currentUserMock.VerifyNoOtherCalls();
        }

        [Test]
        public async Task Handle_WhenRoleLooksLikeAuctioneerDifferentCase_ShouldCallRepositoryWithoutUserId()
        {
            // Arrange
            var request = new GetListAuctionRequest { PageSize = 10 };

            _currentUserMock.Setup(x => x.GetUserId()).Returns(Guid.NewGuid().ToString());
            _currentUserMock.Setup(x => x.GetRole()).Returns("auctioneer"); // khác hoa/thường

            _repoMock
                .Setup(r =>
                    r.GetListAuctionsAsync(It.IsAny<GetListAuctionRequest>(), It.IsAny<string?>())
                )
                .ReturnsAsync(FakeResponse(1));

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.TotalCount, Is.EqualTo(1));

            _repoMock.Verify(
                r =>
                    r.GetListAuctionsAsync(
                        It.Is<GetListAuctionRequest>(req => req == request),
                        It.Is<string?>(uid => uid == null)
                    ),
                Times.Once
            );

            _currentUserMock.Verify(x => x.GetUserId(), Times.Once);
            _currentUserMock.Verify(x => x.GetRole(), Times.Once);
            _repoMock.VerifyNoOtherCalls();
            _currentUserMock.VerifyNoOtherCalls();
        }

        [Test]
        public async Task Handle_WhenRoleIsEmptyString_ShouldCallRepositoryWithoutUserId()
        {
            // Arrange
            var request = new GetListAuctionRequest { AuctionName = "EmptyRoleCase" };

            _currentUserMock.Setup(x => x.GetUserId()).Returns(Guid.NewGuid().ToString());
            _currentUserMock.Setup(x => x.GetRole()).Returns(string.Empty); // role != null nhưng không phải "Auctioneer"

            _repoMock
                .Setup(r =>
                    r.GetListAuctionsAsync(It.IsAny<GetListAuctionRequest>(), It.IsAny<string?>())
                )
                .ReturnsAsync(FakeResponse(5));

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.TotalCount, Is.EqualTo(5));

            _repoMock.Verify(
                r =>
                    r.GetListAuctionsAsync(
                        It.Is<GetListAuctionRequest>(req => req == request),
                        It.Is<string?>(uid => uid == null)
                    ),
                Times.Once
            );

            _currentUserMock.Verify(x => x.GetUserId(), Times.Once);
            _currentUserMock.Verify(x => x.GetRole(), Times.Once);
            _repoMock.VerifyNoOtherCalls();
            _currentUserMock.VerifyNoOtherCalls();
        }
    }
}
