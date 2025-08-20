using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using MyApp.Application.Common.CurrentUserService;
using MyApp.Application.CQRS.AuctionDocuments.SupportRegisterDocuments.Command;
using MyApp.Application.Interfaces.ISupportRegisterDocuments;
using MyApp.Core.DTOs.AuctionDocumentsDTO;
using NUnit.Framework;

namespace MyApp.Application.CQRS.AuctionDocuments.SupportRegisterDocuments.Command.Tests
{
    [TestFixture]
    public class SupportRegisterDocumentsHandlerTests
    {
        private Mock<ISupportRegisterDocuments> _supportRepoMock;
        private Mock<ICurrentUserService> _currentUserMock;
        private SupportRegisterDocumentsHandler _handler;
        private SupportRegisterDocumentsCommand _command;
        private Guid _validAuctionId;
        private Guid _validUserId;

        [SetUp]
        public void Setup()
        {
            _supportRepoMock = new Mock<ISupportRegisterDocuments>();
            _currentUserMock = new Mock<ICurrentUserService>();
            _handler = new SupportRegisterDocumentsHandler(
                _supportRepoMock.Object,
                _currentUserMock.Object
            );

            _validAuctionId = Guid.NewGuid();
            _validUserId = Guid.NewGuid();

            _command = new SupportRegisterDocumentsCommand
            {
                CitizenIdentification = "0123456789",
                AuctionAssetsIds = new List<Guid> { Guid.NewGuid() },
                BankAccount = "VCB",
                BankAccountNumber = "123456",
                BankBranch = "HN",
                AuctionId = _validAuctionId,
            };
        }

        [Test]
        public void Handle_ShouldThrow_WhenUserIdInvalid()
        {
            // Arrange
            _currentUserMock.Setup(x => x.GetUserId()).Returns(""); // null or invalid

            // Act & Assert
            var ex = Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _handler.Handle(_command, CancellationToken.None)
            );
            Assert.That(ex.Message, Is.EqualTo("Không thể lấy UserId từ người dùng."));
        }

        [Test]
        public void Handle_ShouldThrow_WhenCitizenIdNotFound()
        {
            // Arrange
            _currentUserMock.Setup(x => x.GetUserId()).Returns(Guid.NewGuid().ToString());
            _supportRepoMock
                .Setup(x => x.GetUserIdByCitizenIdentificationAsync(_command.CitizenIdentification))
                .ReturnsAsync(Guid.Empty);

            // Act & Assert
            var ex = Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _handler.Handle(_command, CancellationToken.None)
            );
            Assert.That(
                ex.Message,
                Is.EqualTo("Không tìm thấy người dùng với số căn cước công dân này!")
            );
        }

        [Test]
        public void Handle_ShouldThrow_WhenAuctionNotFound()
        {
            // Arrange
            _currentUserMock.Setup(x => x.GetUserId()).Returns(Guid.NewGuid().ToString());
            _supportRepoMock
                .Setup(x => x.GetUserIdByCitizenIdentificationAsync(It.IsAny<string>()))
                .ReturnsAsync(_validUserId);
            _supportRepoMock
                .Setup(x => x.GetAuctionByIdAsync(_command.AuctionId))
                .ReturnsAsync((MyApp.Core.Entities.Auction)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<ValidationException>(() =>
                _handler.Handle(_command, CancellationToken.None)
            );
            Assert.That(ex.Message, Is.EqualTo("Phiên đấu giá không tồn tại."));
        }

        [Test]
        public void Handle_ShouldThrow_WhenInvalidAuctionAssets()
        {
            // Arrange
            _currentUserMock.Setup(x => x.GetUserId()).Returns(Guid.NewGuid().ToString());
            _supportRepoMock
                .Setup(x => x.GetUserIdByCitizenIdentificationAsync(It.IsAny<string>()))
                .ReturnsAsync(_validUserId);
            _supportRepoMock
                .Setup(x => x.GetAuctionByIdAsync(_command.AuctionId))
                .ReturnsAsync(
                    new MyApp.Core.Entities.Auction
                    {
                        AuctionId = _validAuctionId,
                        RegisterOpenDate = DateTime.Now.AddHours(-1),
                        RegisterEndDate = DateTime.Now.AddHours(1),
                    }
                );
            _supportRepoMock
                .Setup(x => x.GetInvalidAuctionAssetIdsAsync(_command.AuctionAssetsIds))
                .ReturnsAsync(new List<Guid> { Guid.NewGuid() });

            // Act & Assert
            var ex = Assert.ThrowsAsync<ValidationException>(() =>
                _handler.Handle(_command, CancellationToken.None)
            );
            StringAssert.StartsWith("Các tài sản đấu giá sau không tồn tại", ex.Message);
        }

        [Test]
        public void Handle_ShouldThrow_WhenRegisterTimeInvalid()
        {
            // Arrange
            _currentUserMock.Setup(x => x.GetUserId()).Returns(Guid.NewGuid().ToString());
            _supportRepoMock
                .Setup(x => x.GetUserIdByCitizenIdentificationAsync(It.IsAny<string>()))
                .ReturnsAsync(_validUserId);
            _supportRepoMock
                .Setup(x => x.GetAuctionByIdAsync(_command.AuctionId))
                .ReturnsAsync(
                    new MyApp.Core.Entities.Auction
                    {
                        AuctionId = _validAuctionId,
                        RegisterOpenDate = DateTime.Now.AddDays(1), // chưa mở
                        RegisterEndDate = DateTime.Now.AddDays(2),
                    }
                );
            _supportRepoMock
                .Setup(x => x.GetInvalidAuctionAssetIdsAsync(_command.AuctionAssetsIds))
                .ReturnsAsync(new List<Guid>());

            // Act & Assert
            var ex = Assert.ThrowsAsync<ValidationException>(() =>
                _handler.Handle(_command, CancellationToken.None)
            );
            Assert.That(
                ex.Message,
                Is.EqualTo(
                    "Thời gian đăng ký không hợp lệ. Vui lòng đăng ký trong khoảng thời gian cho phép."
                )
            );
        }

        [Test]
        public async Task Handle_ShouldReturnTrue_WhenRegisterSuccess()
        {
            // Arrange
            _currentUserMock.Setup(x => x.GetUserId()).Returns(Guid.NewGuid().ToString());
            _supportRepoMock
                .Setup(x => x.GetUserIdByCitizenIdentificationAsync(It.IsAny<string>()))
                .ReturnsAsync(_validUserId);
            _supportRepoMock
                .Setup(x => x.GetAuctionByIdAsync(_command.AuctionId))
                .ReturnsAsync(
                    new MyApp.Core.Entities.Auction
                    {
                        AuctionId = _validAuctionId,
                        RegisterOpenDate = DateTime.Now.AddHours(-1),
                        RegisterEndDate = DateTime.Now.AddHours(1),
                    }
                );
            _supportRepoMock
                .Setup(x => x.GetInvalidAuctionAssetIdsAsync(_command.AuctionAssetsIds))
                .ReturnsAsync(new List<Guid>());
            _supportRepoMock
                .Setup(x =>
                    x.RegisterAsync(It.IsAny<SupportRegisterDocumentsRequest>(), It.IsAny<Guid>())
                )
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(_command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task Handle_ShouldReturnFalse_WhenRegisterFails()
        {
            // Arrange
            _currentUserMock.Setup(x => x.GetUserId()).Returns(Guid.NewGuid().ToString());
            _supportRepoMock
                .Setup(x => x.GetUserIdByCitizenIdentificationAsync(It.IsAny<string>()))
                .ReturnsAsync(_validUserId);
            _supportRepoMock
                .Setup(x => x.GetAuctionByIdAsync(_command.AuctionId))
                .ReturnsAsync(
                    new MyApp.Core.Entities.Auction
                    {
                        AuctionId = _validAuctionId,
                        RegisterOpenDate = DateTime.Now.AddHours(-1),
                        RegisterEndDate = DateTime.Now.AddHours(1),
                    }
                );
            _supportRepoMock
                .Setup(x => x.GetInvalidAuctionAssetIdsAsync(_command.AuctionAssetsIds))
                .ReturnsAsync(new List<Guid>());
            _supportRepoMock
                .Setup(x =>
                    x.RegisterAsync(It.IsAny<SupportRegisterDocumentsRequest>(), It.IsAny<Guid>())
                )
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(_command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result);
        }
    }
}
