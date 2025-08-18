using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using MyApp.Application.Common.CurrentUserService;
using MyApp.Application.CQRS.AuctionDocuments.FindHighestPriceAndFlag.Queries;
using MyApp.Application.Interfaces.IFindHighestPriceAndFlag;
using NUnit.Framework;

namespace MyApp.Application.CQRS.AuctionDocuments.FindHighestPriceAndFlag.Queries.Tests
{
    [TestFixture]
    public class FindHighestPriceAndFlagHandlerTests
    {
        private Mock<IFindHighestPriceAndFlag> _mockService;
        private Mock<ICurrentUserService> _mockCurrentUser;
        private FindHighestPriceAndFlagHandler _handler;

        [SetUp]
        public void Setup()
        {
            _mockService = new Mock<IFindHighestPriceAndFlag>();
            _mockCurrentUser = new Mock<ICurrentUserService>();
            _handler = new FindHighestPriceAndFlagHandler(
                _mockService.Object,
                _mockCurrentUser.Object
            );
        }

        [Test]
        public async Task Handle_ShouldReturnResponse_WhenUserIdIsValid()
        {
            // Arrange
            var auctionId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var expectedResponse = new FindHighestPriceAndFlagResponse
            {
                Data = new System.Collections.Generic.Dictionary<
                    Guid,
                    System.Collections.Generic.List<PriceFlagDto>
                >(),
            };

            _mockCurrentUser.Setup(x => x.GetUserId()).Returns(userId.ToString());
            _mockService
                .Setup(x => x.FindHighestPriceAndFlag(auctionId, userId))
                .ReturnsAsync(expectedResponse);

            var request = new FindHighestPriceAndFlagRequest { AuctionId = auctionId };

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResponse, result);
        }

        [Test]
        public void Handle_ShouldThrowUnauthorizedAccessException_WhenUserIdIsNullOrInvalid()
        {
            // Arrange
            var auctionId = Guid.NewGuid();

            // Case 1: UserId null
            _mockCurrentUser.Setup(x => x.GetUserId()).Returns((string?)null);
            var request1 = new FindHighestPriceAndFlagRequest { AuctionId = auctionId };

            // Act & Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                await _handler.Handle(request1, CancellationToken.None)
            );

            // Case 2: UserId không parse được
            _mockCurrentUser.Setup(x => x.GetUserId()).Returns("invalid-guid");
            var request2 = new FindHighestPriceAndFlagRequest { AuctionId = auctionId };

            Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                await _handler.Handle(request2, CancellationToken.None)
            );
        }
    }
}
