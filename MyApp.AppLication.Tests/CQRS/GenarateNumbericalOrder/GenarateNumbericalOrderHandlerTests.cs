using Moq;
using MyApp.Application.Interfaces.GenarateNumbericalOrder;

namespace MyApp.Application.CQRS.GenarateNumbericalOrder.Tests
{
    [TestFixture()]
    public class GenarateNumbericalOrderHandlerTests
    {
        private Mock<IGenarateNumbericalOrderRepository> _repositoryMock;
        private GenarateNumbericalOrderHandler _handler;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IGenarateNumbericalOrderRepository>();
            _handler = new GenarateNumbericalOrderHandler(_repositoryMock.Object);
        }

        [Test]
        public async Task Handle_ShouldReturnTrue_WhenRepositoryReturnsTrue()
        {
            // Arrange
            var auctionId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var request = new GenarateNumbericalOrderRequest { AuctionId = auctionId };

            _repositoryMock
                .Setup(x => x.GenerateNumbericalOrderAsync(auctionId))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task Handle_ShouldReturnFalse_WhenRepositoryReturnsFalse()
        {
            // Arrange
            var auctionId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var request = new GenarateNumbericalOrderRequest { AuctionId = auctionId };

            _repositoryMock
                .Setup(x => x.GenerateNumbericalOrderAsync(auctionId))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsFalse(result);
        }
    }
}
