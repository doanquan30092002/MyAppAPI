using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Moq;
using MyApp.Application.Common.CurrentUserService;
using MyApp.Application.Interfaces.IAuctionCategoriesRepository;
using MyApp.Application.Interfaces.IAuctionRepository;
using MyApp.Application.Interfaces.IExcelRepository;
using MyApp.Application.Interfaces.IJwtHelper;
using MyApp.Application.Interfaces.IUnitOfWork;
using MyApp.Core.Entities;
using NUnit.Framework;

namespace MyApp.Application.CQRS.Auction.AddAuction.Commands.Tests
{
    [TestFixture]
    public class AddAuctionCommandHandlerTests
    {
        private Mock<IAuctionRepository> _auctionRepoMock;
        private Mock<IExcelRepository> _excelRepoMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<ICurrentUserService> _currentUserMock;
        private Mock<IAuctionCategoriesRepository> _categoryRepoMock;
        private AddAuctionCommandHandler _handler;

        [SetUp]
        public void Setup()
        {
            _auctionRepoMock = new Mock<IAuctionRepository>();
            _excelRepoMock = new Mock<IExcelRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _currentUserMock = new Mock<ICurrentUserService>();
            _categoryRepoMock = new Mock<IAuctionCategoriesRepository>();

            _handler = new AddAuctionCommandHandler(
                _auctionRepoMock.Object,
                _excelRepoMock.Object,
                _unitOfWorkMock.Object,
                _currentUserMock.Object,
                _categoryRepoMock.Object
            );
        }

        private AddAuctionCommand CreateValidCommand(IFormFile? file = null)
        {
            return new AddAuctionCommand
            {
                AuctionName = "Test Auction",
                AuctionDescription = "Desc",
                RegisterOpenDate = DateTime.UtcNow,
                RegisterEndDate = DateTime.UtcNow.AddDays(1),
                AuctionStartDate = DateTime.UtcNow.AddDays(2),
                AuctionEndDate = DateTime.UtcNow.AddDays(3),
                NumberRoundMax = 1,
                CategoryId = 1,
                AuctionAssetFile = file,
            };
        }

        private IFormFile CreateFakeFile()
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.WriteLine(
                "Tên nhãn (Tag_Name),Giá khởi điểm (starting_price),Đơn vị (Unit),Tiền đặt cọc (Deposit),Phí đăng ký (Registration_fee),Mô tả (Description)"
            );
            writer.Flush();
            stream.Position = 0;
            return new FormFile(stream, 0, stream.Length, "file", "file.xlsx");
        }

        [Test]
        public void Handle_UserIdInvalid_ThrowsUnauthorizedAccessException()
        {
            _currentUserMock.Setup(x => x.GetUserId()).Returns<string?>(null);

            var command = CreateValidCommand();

            Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _handler.Handle(command, CancellationToken.None)
            );
        }

        [Test]
        public void Handle_CategoryNotFound_ThrowsValidationException()
        {
            _currentUserMock.Setup(x => x.GetUserId()).Returns(Guid.NewGuid().ToString());
            _categoryRepoMock
                .Setup(x => x.FindByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((AuctionCategory?)null);

            var command = CreateValidCommand();

            Assert.ThrowsAsync<ValidationException>(() =>
                _handler.Handle(command, CancellationToken.None)
            );
        }

        [Test]
        public void Handle_InvalidExcel_ThrowsValidationException()
        {
            _currentUserMock.Setup(x => x.GetUserId()).Returns(Guid.NewGuid().ToString());
            _categoryRepoMock
                .Setup(x => x.FindByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new AuctionCategory());
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(1);
            _excelRepoMock.Setup(x => x.CheckExcelFormatAsync(fileMock.Object)).ReturnsAsync(false);

            var command = CreateValidCommand(fileMock.Object);

            Assert.ThrowsAsync<ValidationException>(() =>
                _handler.Handle(command, CancellationToken.None)
            );
        }

        [Test]
        public async Task Handle_ValidCommandWithoutExcel_Success()
        {
            var userId = Guid.NewGuid();
            _currentUserMock.Setup(x => x.GetUserId()).Returns(userId.ToString());
            _categoryRepoMock
                .Setup(x => x.FindByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new AuctionCategory());
            _auctionRepoMock
                .Setup(x => x.AddAuctionAsync(It.IsAny<AddAuctionCommand>(), userId))
                .ReturnsAsync(Guid.NewGuid());

            var command = CreateValidCommand();
            var result = await _handler.Handle(command, CancellationToken.None);

            _unitOfWorkMock.Verify(x => x.BeginTransaction(), Times.Once);
            _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
            Assert.IsInstanceOf<Guid>(result);
        }

        [Test]
        public async Task Handle_ValidCommandWithExcel_Success()
        {
            var userId = Guid.NewGuid();
            _currentUserMock.Setup(x => x.GetUserId()).Returns(userId.ToString());
            _categoryRepoMock
                .Setup(x => x.FindByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new AuctionCategory());

            var file = CreateFakeFile();
            _excelRepoMock.Setup(x => x.CheckExcelFormatAsync(file)).ReturnsAsync(true);

            var auctionId = Guid.NewGuid();
            _auctionRepoMock
                .Setup(x => x.AddAuctionAsync(It.IsAny<AddAuctionCommand>(), userId))
                .ReturnsAsync(auctionId);

            var command = CreateValidCommand(file);
            var result = await _handler.Handle(command, CancellationToken.None);

            _excelRepoMock.Verify(
                x => x.SaveAssetsFromExcelAsync(auctionId, file, userId),
                Times.Once
            );
            _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
            Assert.AreEqual(auctionId, result);
        }

        [Test]
        public void Handle_ExceptionDuringSave_RollsBack()
        {
            var userId = Guid.NewGuid();
            _currentUserMock.Setup(x => x.GetUserId()).Returns(userId.ToString());
            _categoryRepoMock
                .Setup(x => x.FindByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new AuctionCategory());
            _auctionRepoMock
                .Setup(x => x.AddAuctionAsync(It.IsAny<AddAuctionCommand>(), userId))
                .ThrowsAsync(new Exception("DB error"));

            var command = CreateValidCommand();

            Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
            _unitOfWorkMock.Verify(x => x.RollbackAsync(), Times.Once);
        }
    }
}
