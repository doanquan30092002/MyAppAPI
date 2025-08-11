using Moq;
using MyApp.Application.Common.CurrentUserService;
using MyApp.Application.Common.Message;
using MyApp.Application.Interfaces.RegisterAuctionDocument.Repository;

namespace MyApp.Application.CQRS.RegisterAuctionDocument.Command.Tests
{
    [TestFixture()]
    public class RegisterAuctionDocumentHandlerTests
    {
        private Mock<ICurrentUserService> _currentUserServiceMock;
        private Mock<IRegisterAuctionDocumentRepository> _repositoryMock;
        private RegisterAuctionDocumentHandler _handler;
        private readonly string _userId = "33333333-3333-3333-3333-333333333333";
        private readonly Guid _auctionDocumentId = Guid.Parse(
            "11111111-1111-1111-1111-111111111111"
        );

        [SetUp]
        public void Setup()
        {
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _repositoryMock = new Mock<IRegisterAuctionDocumentRepository>();

            _currentUserServiceMock.Setup(s => s.GetUserId()).Returns(_userId);

            _handler = new RegisterAuctionDocumentHandler(
                _currentUserServiceMock.Object,
                _repositoryMock.Object
            );
        }

        [Test]
        public async Task Should_Return_Unauthorized_When_UserId_Is_Null()
        {
            // Arrange
            _currentUserServiceMock.Setup(s => s.GetUserId()).Returns((string)null);
            var request = new RegisterAuctionDocumentRequest();

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.That(result.Code, Is.EqualTo(401));
            Assert.That(result.Message, Is.EqualTo(Message.UNAUTHORIZED));
        }

        [Test]
        public async Task Should_Create_New_Auction_Document_When_Not_Exists()
        {
            var request = new RegisterAuctionDocumentRequest
            {
                AuctionAssetsId = "2222",
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
                Message = "Tạo thành công",
                QrUrl = "http://qr.vn/abc",
                AuctionDocumentsId = _auctionDocumentId,
                AccountNumber = "123456789",
                BeneficiaryBank = "Ngân hàng A",
                AmountTicket = 100000,
                Description = "Thanh toán",
            };

            _repositoryMock
                .Setup(r => r.CreateQRForPayTicket(_auctionDocumentId))
                .ReturnsAsync(expectedResponse);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.That(result.Code, Is.EqualTo(200));
            Assert.That(result.Message, Is.EqualTo("Tạo thành công"));
        }

        [Test]
        public async Task Should_Return_Fail_When_InsertAuctionDocument_Fails()
        {
            var request = new RegisterAuctionDocumentRequest
            {
                AuctionAssetsId = "2222",
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

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.That(result.Code, Is.EqualTo(400));
            Assert.That(result.Message, Is.EqualTo(Message.REGISTER_AUCTION_DOCUMENT_FAIL));
        }

        [Test]
        public async Task Should_Return_Error_When_Auction_Document_Already_Paid()
        {
            var request = new RegisterAuctionDocumentRequest { AuctionAssetsId = "2222" };

            var existingDoc = new AuctionDocumentResponse
            {
                StatusTicket = Message.REGISTER_TICKET_PAID,
                AuctionDocumentsId = _auctionDocumentId,
            };

            _repositoryMock
                .Setup(r => r.CheckAuctionDocumentPaid(_userId, request.AuctionAssetsId))
                .ReturnsAsync(existingDoc);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.That(result.Code, Is.EqualTo(400));
            Assert.That(result.Message, Is.EqualTo(Message.AUCTION_DOCUMENT_EXIST));
        }

        [Test]
        public async Task Should_Update_Bank_Info_When_Auction_Document_Not_Paid_And_Update_Success()
        {
            var request = new RegisterAuctionDocumentRequest
            {
                AuctionAssetsId = "2222",
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
                Message = "Update thành công",
                QrUrl = "http://qr.vn/xyz",
            };

            _repositoryMock
                .Setup(r => r.CreateQRForPayTicket(_auctionDocumentId))
                .ReturnsAsync(expectedResponse);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.That(result.Code, Is.EqualTo(200));
            Assert.That(result.Message, Is.EqualTo("Update thành công"));
        }

        [Test]
        public async Task Should_Update_Bank_Info_When_Auction_Document_Not_Paid_And_Update_Fails()
        {
            var request = new RegisterAuctionDocumentRequest
            {
                AuctionAssetsId = "2222",
                BankAccount = "Nguyễn Văn E",
                BankAccountNumber = "000111222",
                BankBranch = "Ngân hàng E",
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
                .ReturnsAsync(false);

            var expectedResponse = new RegisterAuctionDocumentResponse
            {
                Code = 200,
                Message = "QR vẫn tạo được dù update fail",
                QrUrl = "http://qr.vn/zzz",
            };

            _repositoryMock
                .Setup(r => r.CreateQRForPayTicket(_auctionDocumentId))
                .ReturnsAsync(expectedResponse);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.That(result.Code, Is.EqualTo(200));
            Assert.That(result.Message, Is.EqualTo("QR vẫn tạo được dù update fail"));
        }
    }
}
