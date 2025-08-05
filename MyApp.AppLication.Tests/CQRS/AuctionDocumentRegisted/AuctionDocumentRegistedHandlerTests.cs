using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Moq;
using MyApp.Application.Interfaces.AuctionDocumentRegisted;

namespace MyApp.Application.CQRS.AuctionDocumentRegisted.Tests
{
    [TestFixture()]
    public class AuctionDocumentRegistedHandlerTests
    {
        private Mock<IAuctionDocumentRegistedRepository> _repositoryMock;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private AuctionDocumentRegistedHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _repositoryMock = new Mock<IAuctionDocumentRegistedRepository>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

            _handler = new AuctionDocumentRegistedHandler(
                _repositoryMock.Object,
                _httpContextAccessorMock.Object
            );
        }

        [Test]
        public async Task Handle_WithValidUserIdAndAuctionId_ReturnsAuctionDocuments()
        {
            // Arrange
            var userId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa").ToString();
            var auctionId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) };
            var identity = new ClaimsIdentity(claims, "Test");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            var httpContext = new DefaultHttpContext { User = claimsPrincipal };

            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

            var expectedDocuments = new List<AuctionDocumentRegistedResponse>
            {
                new AuctionDocumentRegistedResponse
                {
                    AuctionDocumentsId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                    CitizenIdentification = "0123456789",
                    Deposit = 5000000,
                    Name = "Nguyen Van A",
                    Note = "Đã nộp",
                    NumericalOrder = 1,
                    RegistrationFee = 100000,
                    StatusDeposit = 1,
                    StatusTicket = 0,
                    TagName = "Tag A",
                    BankAccount = "VCB",
                    BankAccountNumber = "123456789",
                    BankBranch = "Hà Nội",
                },
            };

            _repositoryMock
                .Setup(x => x.GetAuctionDocumentRegistedByAuctionId(userId, auctionId))
                .ReturnsAsync(expectedDocuments);

            var request = new AuctionDocumentRegistedRequest { AuctionId = auctionId };

            // Act
            var result = await _handler.Handle(request, default);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            var doc = result[0];
            Assert.AreEqual(
                Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                doc.AuctionDocumentsId
            );
            Assert.AreEqual("0123456789", doc.CitizenIdentification);
            Assert.AreEqual(5000000, doc.Deposit);
            Assert.AreEqual("Nguyen Van A", doc.Name);
            Assert.AreEqual("Đã nộp", doc.Note);
            Assert.AreEqual(1, doc.NumericalOrder);
            Assert.AreEqual(100000, doc.RegistrationFee);
            Assert.AreEqual(1, doc.StatusDeposit);
            Assert.AreEqual(0, doc.StatusTicket);
            Assert.AreEqual("Tag A", doc.TagName);
            Assert.AreEqual("VCB", doc.BankAccount);
            Assert.AreEqual("123456789", doc.BankAccountNumber);
            Assert.AreEqual("Hà Nội", doc.BankBranch);
        }

        [Test]
        public async Task Handle_WithMissingUserId_ReturnsNull()
        {
            // Arrange
            var auctionId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
            var request = new AuctionDocumentRegistedRequest { AuctionId = auctionId };

            var httpContext = new DefaultHttpContext(); // Không có user
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

            // Act
            var result = await _handler.Handle(request, default);

            // Assert
            _repositoryMock.Verify(
                x => x.GetAuctionDocumentRegistedByAuctionId(It.IsAny<string>(), It.IsAny<Guid>()),
                Times.Never
            );

            Assert.IsNull(result);
        }
    }
}
