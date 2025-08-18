using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using MyApp.Application.CQRS.AuctionCategories.Queries;
using MyApp.Application.Interfaces.IAuctionCategoriesRepository;
using MyApp.Core.Entities;
using NUnit.Framework;

namespace MyApp.Application.CQRS.AuctionCategories.Queries.Tests
{
    [TestFixture]
    public class FindAllAuctionCategoriesHandlerTests
    {
        private Mock<IAuctionCategoriesRepository> _mockRepo;
        private FindAllAuctionCategoriesHandler _handler;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IAuctionCategoriesRepository>();
            _handler = new FindAllAuctionCategoriesHandler(_mockRepo.Object);
        }

        [Test]
        public async Task Handle_ShouldReturnListOfCategories_WhenCategoriesExist()
        {
            // Arrange
            var categories = new List<AuctionCategory>
            {
                new AuctionCategory { CategoryId = 1, CategoryName = "Category 1" },
                new AuctionCategory { CategoryId = 2, CategoryName = "Category 2" },
            };

            _mockRepo.Setup(r => r.GetAllCategoriesAsync()).ReturnsAsync(categories);

            var query = new FindAllAuctionCategoriesQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Category 1", result[0].CategoryName);
            Assert.AreEqual("Category 2", result[1].CategoryName);
        }

        [Test]
        public async Task Handle_ShouldReturnEmptyList_WhenNoCategoriesExist()
        {
            // Arrange
            _mockRepo
                .Setup(r => r.GetAllCategoriesAsync())
                .ReturnsAsync(new List<AuctionCategory>());

            var query = new FindAllAuctionCategoriesQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }
    }
}
