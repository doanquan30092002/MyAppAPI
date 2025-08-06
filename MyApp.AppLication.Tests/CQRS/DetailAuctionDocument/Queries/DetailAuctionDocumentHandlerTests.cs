using Moq;
using MyApp.Application.Interfaces.DetailAuctionDocument;

namespace MyApp.Application.CQRS.DetailAuctionDocument.Queries.Tests
{
    [TestFixture()]
    public class DetailAuctionDocumentHandlerTests
    {
        private Mock<IDetailAuctionDocumentRepository> _repositoryMock;
        private DetailAuctionDocumentHandler _handler;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IDetailAuctionDocumentRepository>();
            _handler = new DetailAuctionDocumentHandler(_repositoryMock.Object);
        }

        [Test]
        public async Task Handle_WithValidId_ReturnsExpectedResponse()
        {
            // Arrange
            var auctionDocumentsId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var userId = Guid.Parse("22222222-2222-2222-2222-222222222222");
            var assetId = Guid.Parse("33333333-3333-3333-3333-333333333333");
            var createByTicket = Guid.Parse("22222222-2222-2222-2222-222222222222");
            var createAtTicket = new DateTime(2025, 08, 01, 00, 00, 00, DateTimeKind.Utc);
            var updateAtTicket = new DateTime(2025, 08, 02, 00, 00, 00, DateTimeKind.Utc);
            var createAtDeposit = new DateTime(2054, 08, 01, 00, 00, 00, DateTimeKind.Utc);

            var expectedResponse = new DetailAuctionDocumentResponse
            {
                AuctionDocumentsId = auctionDocumentsId,
                UserId = userId,
                AuctionAssetId = assetId,
                BankAccount = "ACB",
                BankAccountNumber = "9876543210",
                BankBranch = "Chi nhánh Đà Nẵng",
                CreateByTicket = createByTicket,
                CreateAtTicket = createAtTicket,
                UpdateAtTicket = updateAtTicket,
                CreateAtDeposit = createAtDeposit,
                StatusTicket = 1,
                StatusDeposit = 2,
                NumericalOrder = 3,
                Note = "Ghi chú kiểm thử",
            };

            _repositoryMock
                .Setup(x => x.GetDetailAuctionDocumentByAuctionDocumentsIdAsync(auctionDocumentsId))
                .ReturnsAsync(expectedResponse);

            var request = new DetailAuctionDocumentRequest
            {
                AuctionDocumentsId = auctionDocumentsId,
            };

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResponse.AuctionDocumentsId, result.AuctionDocumentsId);
            Assert.AreEqual(expectedResponse.UserId, result.UserId);
            Assert.AreEqual(expectedResponse.AuctionAssetId, result.AuctionAssetId);
            Assert.AreEqual(expectedResponse.BankAccount, result.BankAccount);
            Assert.AreEqual(expectedResponse.BankAccountNumber, result.BankAccountNumber);
            Assert.AreEqual(expectedResponse.BankBranch, result.BankBranch);
            Assert.AreEqual(expectedResponse.CreateByTicket, result.CreateByTicket);
            Assert.AreEqual(expectedResponse.CreateAtTicket, result.CreateAtTicket);
            Assert.AreEqual(expectedResponse.UpdateAtTicket, result.UpdateAtTicket);
            Assert.AreEqual(expectedResponse.CreateAtDeposit, result.CreateAtDeposit);
            Assert.AreEqual(expectedResponse.StatusTicket, result.StatusTicket);
            Assert.AreEqual(expectedResponse.StatusDeposit, result.StatusDeposit);
            Assert.AreEqual(expectedResponse.NumericalOrder, result.NumericalOrder);
            Assert.AreEqual(expectedResponse.Note, result.Note);
        }

        [Test]
        public async Task Handle_WithInvalidId_ReturnsNull()
        {
            // Arrange
            var auctionDocumentsId = Guid.Parse("11111111-1111-1111-1111-111111111111");

            _repositoryMock
                .Setup(x => x.GetDetailAuctionDocumentByAuctionDocumentsIdAsync(auctionDocumentsId))
                .ReturnsAsync((DetailAuctionDocumentResponse?)null);

            var request = new DetailAuctionDocumentRequest
            {
                AuctionDocumentsId = auctionDocumentsId,
            };

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsNull(result);
        }
    }
}
