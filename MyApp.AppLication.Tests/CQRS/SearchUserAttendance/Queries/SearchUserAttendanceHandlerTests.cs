using Moq;
using MyApp.Application.Common.Message;
using MyApp.Application.Interfaces.SearchUserAttendance;

namespace MyApp.Application.CQRS.SearchUserAttendance.Queries.Tests
{
    [TestFixture()]
    public class SearchUserAttendanceHandlerTests
    {
        private Mock<ISearchUserAttendanceRepository> _attendanceRepositoryMock;
        private SearchUserAttendanceHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _attendanceRepositoryMock = new Mock<ISearchUserAttendanceRepository>();
            _handler = new SearchUserAttendanceHandler(_attendanceRepositoryMock.Object);
        }

        [Test]
        public async Task Handle_AuctionDoesNotExist_ReturnsAuctionNotExistMessage()
        {
            // Arrange
            var request = new SearchUserAttendanceRequest
            {
                AuctionId = Guid.Parse("68FAA0B5-2E6B-4792-8CEB-8A5E951A3E1A"),
                CitizenIdentification = "123456789",
            };

            _attendanceRepositoryMock
                .Setup(r => r.GetAuctionNameAsync(request.AuctionId))
                .ReturnsAsync((string)null);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.That(result.Message, Is.EqualTo(Message.AUCTION_NOT_EXIST));
            Assert.That(result.AuctionName, Is.Null);
            Assert.That(result.NumericalOrder, Is.Null);
        }

        [Test]
        public async Task Handle_NumericalOrderNotFound_ReturnsNotFoundMessage()
        {
            // Arrange
            var request = new SearchUserAttendanceRequest
            {
                AuctionId = Guid.Parse("68FAA0B5-2E6B-4792-8CEB-8A5E951A3E1A"),
                CitizenIdentification = "123456789",
            };

            _attendanceRepositoryMock
                .Setup(r => r.GetAuctionNameAsync(request.AuctionId))
                .ReturnsAsync("Auction ABC");

            _attendanceRepositoryMock
                .Setup(r =>
                    r.GetNumericalOrderAsync(request.AuctionId, request.CitizenIdentification)
                )
                .ReturnsAsync((int?)null);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.That(result.Message, Is.EqualTo(Message.NOT_FOUND_NUMERICAL_ORDER));
            Assert.That(result.AuctionName, Is.EqualTo("Auction ABC"));
            Assert.That(result.NumericalOrder, Is.Null);
        }

        [Test]
        public async Task Handle_NumericalOrderFound_ReturnsSuccessMessage()
        {
            // Arrange
            var request = new SearchUserAttendanceRequest
            {
                AuctionId = Guid.Parse("68FAA0B5-2E6B-4792-8CEB-8A5E951A3E1A"),
                CitizenIdentification = "123456789",
            };

            _attendanceRepositoryMock
                .Setup(r => r.GetAuctionNameAsync(request.AuctionId))
                .ReturnsAsync("Auction ABC");

            _attendanceRepositoryMock
                .Setup(r =>
                    r.GetNumericalOrderAsync(request.AuctionId, request.CitizenIdentification)
                )
                .ReturnsAsync(5);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.That(result.Message, Is.EqualTo(Message.FOUND_NUMERICAL_ORDER));
            Assert.That(result.AuctionName, Is.EqualTo("Auction ABC"));
            Assert.That(result.NumericalOrder, Is.EqualTo(5));
        }
    }
}
