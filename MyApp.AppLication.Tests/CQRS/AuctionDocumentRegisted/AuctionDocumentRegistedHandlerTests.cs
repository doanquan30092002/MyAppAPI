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
        private AuctionDocumentRegistedHandler _handler;

        // Dùng Guid cố định cho tất cả test
        private static readonly Guid FixedUserId = Guid.Parse(
            "11111111-1111-1111-1111-111111111111"
        );
        private static readonly Guid FixedAuctionId = Guid.Parse(
            "22222222-2222-2222-2222-222222222222"
        );
        private static readonly Guid FixedDocumentId = Guid.Parse(
            "33333333-3333-3333-3333-333333333333"
        );

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IAuctionDocumentRegistedRepository>();
            _handler = new AuctionDocumentRegistedHandler(_repositoryMock.Object);
        }

        [Test]
        public async Task Handle_ShouldReturnNull_WhenRepositoryReturnsNull()
        {
            // Arrange
            var request = new AuctionDocumentRegistedRequest
            {
                UserId = FixedUserId,
                AuctionId = FixedAuctionId,
            };

            _repositoryMock
                .Setup(r => r.GetAuctionDocumentRegistedByAuctionId(FixedUserId, FixedAuctionId))
                .ReturnsAsync((List<AuctionDocumentRegistedResponse>?)null);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsNull(result);
            _repositoryMock.Verify(
                r => r.GetAuctionDocumentRegistedByAuctionId(FixedUserId, FixedAuctionId),
                Times.Once
            );
        }

        [Test]
        public async Task Handle_ShouldReturnEmptyList_WhenRepositoryReturnsEmptyList()
        {
            // Arrange
            var request = new AuctionDocumentRegistedRequest
            {
                UserId = FixedUserId,
                AuctionId = FixedAuctionId,
            };

            var emptyList = new List<AuctionDocumentRegistedResponse>();

            _repositoryMock
                .Setup(r => r.GetAuctionDocumentRegistedByAuctionId(FixedUserId, FixedAuctionId))
                .ReturnsAsync(emptyList);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public async Task Handle_ShouldReturnList_WhenRepositoryReturnsList()
        {
            // Arrange
            var request = new AuctionDocumentRegistedRequest
            {
                UserId = FixedUserId,
                AuctionId = FixedAuctionId,
            };

            var responseList = new List<AuctionDocumentRegistedResponse>
            {
                new AuctionDocumentRegistedResponse
                {
                    AuctionDocumentsId = FixedDocumentId,
                    CitizenIdentification = "0123456789",
                    Deposit = 1500000m,
                    Name = "Nguyễn Văn A",
                    Note = "Ghi chú kiểm tra",
                    NumericalOrder = 5,
                    RegistrationFee = 500000m,
                    StatusDeposit = 1,
                    StatusTicket = 2,
                    TagName = "Phiên đấu giá A",
                    BankAccount = "Ngân hàng ABC",
                    BankAccountNumber = "123456789012",
                    BankBranch = "Chi nhánh Hà Nội",
                    IsAttended = true,
                    RefundProof = "Ảnh xác nhận",
                    RefundReason = "Hoàn tiền do hủy phiên",
                    StatusRefund = 0,
                },
            };

            _repositoryMock
                .Setup(r => r.GetAuctionDocumentRegistedByAuctionId(FixedUserId, FixedAuctionId))
                .ReturnsAsync(responseList);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);

            var item = result[0];
            Assert.AreEqual(FixedDocumentId, item.AuctionDocumentsId);
            Assert.AreEqual("0123456789", item.CitizenIdentification);
            Assert.AreEqual(1500000m, item.Deposit);
            Assert.AreEqual("Nguyễn Văn A", item.Name);
            Assert.AreEqual("Ghi chú kiểm tra", item.Note);
            Assert.AreEqual(5, item.NumericalOrder);
            Assert.AreEqual(500000m, item.RegistrationFee);
            Assert.AreEqual(1, item.StatusDeposit);
            Assert.AreEqual(2, item.StatusTicket);
            Assert.AreEqual("Phiên đấu giá A", item.TagName);
            Assert.AreEqual("Ngân hàng ABC", item.BankAccount);
            Assert.AreEqual("123456789012", item.BankAccountNumber);
            Assert.AreEqual("Chi nhánh Hà Nội", item.BankBranch);
            Assert.IsTrue(item.IsAttended);
            Assert.AreEqual("Ảnh xác nhận", item.RefundProof);
            Assert.AreEqual("Hoàn tiền do hủy phiên", item.RefundReason);
            Assert.AreEqual(0, item.StatusRefund);
        }
    }
}
