using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Moq;
using MyApp.Application.Common.Message;
using MyApp.Application.Interfaces.RegisterAuctionDocument.Repository;

namespace MyApp.Application.CQRS.RegisterAuctionDocument.Command.Tests
{
    [TestFixture()]
    public class RegisterAuctionDocumentHandlerTests
    {
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private Mock<IRegisterAuctionDocumentRepository> _repositoryMock;
        private RegisterAuctionDocumentHandler _handler;
        private readonly string _userId = "33333333-3333-3333-3333-333333333333";
        private readonly Guid _auctionDocumentId = Guid.Parse(
            "11111111-1111-1111-1111-111111111111"
        );

        [SetUp]
        public void Setup()
        {
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _repositoryMock = new Mock<IRegisterAuctionDocumentRepository>();

            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(
                new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, _userId) })
            );

            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

            _handler = new RegisterAuctionDocumentHandler(
                _httpContextAccessorMock.Object,
                _repositoryMock.Object
            );
        }

        [Test]
        public async Task Should_Create_New_Auction_Document_When_Not_Exists()
        {
            // Arrange
            var request = new RegisterAuctionDocumentRequest
            {
                AuctionAssetsId = "22222222-2222-2222-2222-222222222222",
                BankAccount = "Nguyễn Văn A",
                BankAccountNumber = "123456789",
                BankBranch = "Ngân hàng A",
            };

            _repositoryMock
                .Setup(r => r.CheckAuctionDocumentPaid(_userId, request.AuctionAssetsId))
                .ReturnsAsync((AuctionDocumentResponse)null);

            _repositoryMock
                .Setup(r =>
                    r.InsertAuctionDocumentAsync(
                        request.AuctionAssetsId,
                        _userId,
                        request.BankAccount,
                        request.BankAccountNumber,
                        request.BankBranch
                    )
                )
                .ReturnsAsync(_auctionDocumentId);

            var expectedResponse = new RegisterAuctionDocumentResponse
            {
                Code = 200,
                Message = "Tạo tài liệu đấu giá thành công",
                QrUrl = "http://qr.vn/abc",
                AuctionDocumentsId = _auctionDocumentId,
                AccountNumber = "123456789",
                BeneficiaryBank = "Ngân hàng A",
                AmountTicket = 100000,
                Description = "Thanh toán phí tham gia",
            };

            _repositoryMock
                .Setup(r => r.CreateQRForPayTicket(_auctionDocumentId))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert - kiểm tra từng field (không dùng Assert.Multiple)
            Assert.That(result.Code, Is.EqualTo(200));
            Assert.That(result.Message, Is.EqualTo("Tạo tài liệu đấu giá thành công"));
            Assert.That(result.QrUrl, Is.EqualTo("http://qr.vn/abc"));
            Assert.That(result.AuctionDocumentsId, Is.EqualTo(_auctionDocumentId));
            Assert.That(result.AccountNumber, Is.EqualTo("123456789"));
            Assert.That(result.BeneficiaryBank, Is.EqualTo("Ngân hàng A"));
            Assert.That(result.AmountTicket, Is.EqualTo(100000));
            Assert.That(result.Description, Is.EqualTo("Thanh toán phí tham gia"));
        }

        [Test]
        public async Task Should_Return_Fail_When_InsertAuctionDocument_Fails()
        {
            // Arrange
            var request = new RegisterAuctionDocumentRequest
            {
                AuctionAssetsId = "22222222-2222-2222-2222-222222222222",
                BankAccount = "Nguyễn Văn B",
                BankAccountNumber = "987654321",
                BankBranch = "Ngân hàng B",
            };

            _repositoryMock
                .Setup(r => r.CheckAuctionDocumentPaid(_userId, request.AuctionAssetsId))
                .ReturnsAsync((AuctionDocumentResponse)null);

            _repositoryMock
                .Setup(r =>
                    r.InsertAuctionDocumentAsync(
                        request.AuctionAssetsId,
                        _userId,
                        request.BankAccount,
                        request.BankAccountNumber,
                        request.BankBranch
                    )
                )
                .ReturnsAsync(Guid.Empty);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert - kiểm tra từng field
            Assert.That(result.Code, Is.EqualTo(400));
            Assert.That(result.Message, Is.EqualTo(Message.REGISTER_AUCTION_DOCUMENT_FAIL));
            Assert.That(result.QrUrl, Is.Null);
            Assert.That(result.AuctionDocumentsId, Is.EqualTo(Guid.Empty));
            Assert.That(result.AccountNumber, Is.Null);
            Assert.That(result.BeneficiaryBank, Is.Null);
            Assert.That(result.AmountTicket, Is.Null);
            Assert.That(result.Description, Is.Null);
        }

        [Test]
        public async Task Should_Return_Error_When_Auction_Document_Already_Paid()
        {
            // Arrange
            var request = new RegisterAuctionDocumentRequest
            {
                AuctionAssetsId = "22222222-2222-2222-2222-222222222222",
                BankAccount = "Nguyễn Văn C",
                BankAccountNumber = "111222333",
                BankBranch = "Ngân hàng C",
            };

            var existingDoc = new AuctionDocumentResponse
            {
                StatusTicket = Message.REGISTER_TICKET_PAID,
                AuctionDocumentsId = _auctionDocumentId,
            };

            _repositoryMock
                .Setup(r => r.CheckAuctionDocumentPaid(_userId, request.AuctionAssetsId))
                .ReturnsAsync(existingDoc);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert - kiểm tra từng field
            Assert.That(result.Code, Is.EqualTo(400));
            Assert.That(result.Message, Is.EqualTo(Message.AUCTION_DOCUMENT_EXIST));
            Assert.That(result.QrUrl, Is.Null);
            Assert.That(result.AuctionDocumentsId, Is.EqualTo(Guid.Empty));
            Assert.That(result.AccountNumber, Is.Null);
            Assert.That(result.BeneficiaryBank, Is.Null);
            Assert.That(result.AmountTicket, Is.Null);
            Assert.That(result.Description, Is.Null);
        }

        [Test]
        public async Task Should_Update_Bank_Info_When_Auction_Document_Not_Paid()
        {
            // Arrange
            var request = new RegisterAuctionDocumentRequest
            {
                AuctionAssetsId = "22222222-2222-2222-2222-222222222222",
                BankAccount = "Nguyễn Văn D",
                BankAccountNumber = "444555666",
                BankBranch = "Ngân hàng D",
            };

            var existingDoc = new AuctionDocumentResponse
            {
                StatusTicket = Message.REGISTER_TICKET_NOT_PAID,
                AuctionDocumentsId = _auctionDocumentId,
            };

            _repositoryMock
                .Setup(r => r.CheckAuctionDocumentPaid(_userId, request.AuctionAssetsId))
                .ReturnsAsync(existingDoc);

            _repositoryMock
                .Setup(r =>
                    r.UpdateInforBankFromUser(
                        _auctionDocumentId,
                        request.BankAccount,
                        request.BankAccountNumber,
                        request.BankBranch
                    )
                )
                .ReturnsAsync(true);

            var expectedResponse = new RegisterAuctionDocumentResponse
            {
                Code = 200,
                Message = "Cập nhật thông tin ngân hàng và tạo QR thành công",
                QrUrl = "http://qr.vn/xyz",
                AuctionDocumentsId = _auctionDocumentId,
                AccountNumber = "444555666",
                BeneficiaryBank = "Ngân hàng D",
                AmountTicket = 200000,
                Description = "Thanh toán phí tham gia",
            };

            _repositoryMock
                .Setup(r => r.CreateQRForPayTicket(_auctionDocumentId))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert - kiểm tra từng field
            Assert.That(result.Code, Is.EqualTo(200));
            Assert.That(
                result.Message,
                Is.EqualTo("Cập nhật thông tin ngân hàng và tạo QR thành công")
            );
            Assert.That(result.QrUrl, Is.EqualTo("http://qr.vn/xyz"));
            Assert.That(result.AuctionDocumentsId, Is.EqualTo(_auctionDocumentId));
            Assert.That(result.AccountNumber, Is.EqualTo("444555666"));
            Assert.That(result.BeneficiaryBank, Is.EqualTo("Ngân hàng D"));
            Assert.That(result.AmountTicket, Is.EqualTo(200000));
            Assert.That(result.Description, Is.EqualTo("Thanh toán phí tham gia"));
        }
    }
}
