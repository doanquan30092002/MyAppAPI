using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using MyApp.Application.CQRS.GetAuctioneers.Queries;
using MyApp.Application.Interfaces.GetAuctioneers;
using NUnit.Framework;

namespace MyApp.Application.CQRS.GetAuctioneers.Queries.Tests
{
    [TestFixture()]
    public class GetAuctioneersHanlderTests
    {
        private Mock<IGetAuctioneersRepository> _mockRepo;
        private GetAuctioneersHanlder _handler;

        [SetUp]
        public void SetUp()
        {
            _mockRepo = new Mock<IGetAuctioneersRepository>();
            _handler = new GetAuctioneersHanlder(_mockRepo.Object);
        }

        [Test]
        public async Task Handle_ShouldReturnAuctioneersList()
        {
            // Arrange
            var expectedList = new List<GetAuctioneersResponse>
            {
                new GetAuctioneersResponse { Id = Guid.NewGuid(), Name = "Alice" },
                new GetAuctioneersResponse { Id = Guid.NewGuid(), Name = "Bob" },
            };

            _mockRepo.Setup(repo => repo.GetAuctioneersAsync()).ReturnsAsync(expectedList);

            var request = new GetAuctioneersRequest();

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Equals(expectedList);
            _mockRepo.Verify(repo => repo.GetAuctioneersAsync(), Times.Once);
        }
    }
}
