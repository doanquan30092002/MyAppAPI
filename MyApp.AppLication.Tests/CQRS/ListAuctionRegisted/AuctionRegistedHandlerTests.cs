using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Moq;
using MyApp.Application.Interfaces.ListAuctionRegisted;

namespace MyApp.Application.CQRS.ListAuctionRegisted.Tests
{
    [TestFixture()]
    public class AuctionRegistedHandlerTests
    {
        private Mock<IAuctionRegistedRepository> _repositoryMock;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private AuctionRegistedHandler _handler;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IAuctionRegistedRepository>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _handler = new AuctionRegistedHandler(
                _repositoryMock.Object,
                _httpContextAccessorMock.Object
            );
        }

        [Test]
        public async Task Handle_ReturnsExpectedResponse_WhenUserIdExists()
        {
            var userId = "b3f571aa-f81d-4d70-a451-291bcf149741";

            var fixedAuctionId = Guid.Parse("e51877c2-4dce-463a-a7a0-728264b09315");
            var fixedAssetId = Guid.Parse("b95e2c92-fec0-4cd3-8882-d9bfbad5ee95");
            var fixedDate = new DateTime(2025, 8, 1, 0, 0, 0, DateTimeKind.Utc);

            var request = new AuctionRegistedRequest
            {
                PageNumber = 1,
                PageSize = 10,
                Search = new SearchAuctionRegisted
                {
                    AuctionName = "Test Auction",
                    AuctionStartDate = fixedDate.AddDays(-5), // 2025-07-27
                    AuctionEndDate = fixedDate.AddDays(2), // 2025-08-03
                },
            };

            var expectedResponse = new AuctionRegistedResponse
            {
                PageNumber = 1,
                PageSize = 10,
                TotalAuctionRegisted = 1,
                AuctionResponse = new List<AuctionResponse>
                {
                    new AuctionResponse
                    {
                        AuctionId = fixedAuctionId,
                        AuctionName = "Test Auction",
                        CategoryName = "Land",
                        AuctionDescription = "Auctioning a piece of land",
                        AuctionRules = "No refunds",
                        AuctionPlanningMap = "http://map.com/image",
                        RegisterOpenDate = fixedDate.AddDays(-5),
                        RegisterEndDate = fixedDate.AddDays(-1),
                        AuctionStartDate = fixedDate,
                        AuctionEndDate = fixedDate,
                        NumberRoundMax = 3,
                        Status = 1,
                        AuctionAssets = new List<AuctionAsset>
                        {
                            new AuctionAsset
                            {
                                AuctionAssetsId = fixedAssetId,
                                TagName = "Plot A1",
                                StartingPrice = 50000,
                                Unit = "m2",
                                Deposit = 10000,
                                RegistrationFee = 200,
                                Description = "Prime land",
                                AuctionId = fixedAuctionId,
                            },
                        },
                    },
                },
            };

            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(c => c.User).Returns(principal);
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContextMock.Object);

            _repositoryMock
                .Setup(repo => repo.ListAuctionRegisted(userId, request))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _handler.Handle(request, default);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(1, result.PageNumber);
            Assert.AreEqual(10, result.PageSize);
            Assert.AreEqual(1, result.TotalAuctionRegisted);
            Assert.AreEqual(1, result.AuctionResponse.Count);

            var actualAuction = result.AuctionResponse[0];
            Assert.AreEqual(fixedAuctionId, actualAuction.AuctionId);
            Assert.AreEqual("Test Auction", actualAuction.AuctionName);
            Assert.AreEqual("Land", actualAuction.CategoryName);
            Assert.AreEqual("Auctioning a piece of land", actualAuction.AuctionDescription);
            Assert.AreEqual("No refunds", actualAuction.AuctionRules);
            Assert.AreEqual("http://map.com/image", actualAuction.AuctionPlanningMap);
            Assert.AreEqual(fixedDate.AddDays(-5), actualAuction.RegisterOpenDate);
            Assert.AreEqual(fixedDate.AddDays(-1), actualAuction.RegisterEndDate);
            Assert.AreEqual(fixedDate, actualAuction.AuctionStartDate);
            Assert.AreEqual(fixedDate, actualAuction.AuctionEndDate);
            Assert.AreEqual(3, actualAuction.NumberRoundMax);
            Assert.AreEqual(1, actualAuction.Status);
            Assert.AreEqual(1, actualAuction.AuctionAssets.Count);

            var actualAsset = actualAuction.AuctionAssets[0];
            Assert.AreEqual(fixedAssetId, actualAsset.AuctionAssetsId);
            Assert.AreEqual("Plot A1", actualAsset.TagName);
            Assert.AreEqual(50000, actualAsset.StartingPrice);
            Assert.AreEqual("m2", actualAsset.Unit);
            Assert.AreEqual(10000, actualAsset.Deposit);
            Assert.AreEqual(200, actualAsset.RegistrationFee);
            Assert.AreEqual("Prime land", actualAsset.Description);
            Assert.AreEqual(fixedAuctionId, actualAsset.AuctionId);
        }

        [Test]
        public void Handle_ThrowsException_WhenUserIdIsMissing()
        {
            // Arrange
            var request = new AuctionRegistedRequest { PageNumber = 1, PageSize = 10 };

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(c => c.User).Returns(new ClaimsPrincipal()); // Không có claim

            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(mockHttpContext.Object);

            // Act & Assert
            var ex = Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _handler.Handle(request, default)
            );
            Assert.That(ex.Message, Is.EqualTo("User not authenticated."));
        }
    }
}
