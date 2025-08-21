using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Moq;
using MyApp.Application.Common.CurrentUserService;
using MyApp.Application.CQRS.Auction.UpdateAuction.Commands;
using MyApp.Application.Interfaces.IActionAssetsRepository;
using MyApp.Application.Interfaces.IAuctionCategoriesRepository;
using MyApp.Application.Interfaces.IAuctionRepository;
using MyApp.Application.Interfaces.IExcelRepository;
using MyApp.Application.Interfaces.IUnitOfWork;
using MyApp.Core.Entities;
using NUnit.Framework;

namespace MyApp.AppLication.Tests.CQRS.Auction.UpdateAuction.Commands
{
    [TestFixture]
    public class UpdateAuctionHandlerTests
    {
        private Mock<IAuctionRepository> _auctionRepoMock;
        private Mock<IAuctionCategoriesRepository> _categoryRepoMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<ICurrentUserService> _currentUserMock;
        private Mock<IExcelRepository> _excelRepoMock;
        private Mock<IAuctionAssetsRepository> _auctionAssetRepoMock;

        private UpdateAuctionHandler _handler;

        [SetUp]
        public void Setup()
        {
            _auctionRepoMock = new Mock<IAuctionRepository>();
            _categoryRepoMock = new Mock<IAuctionCategoriesRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _currentUserMock = new Mock<ICurrentUserService>();
            _excelRepoMock = new Mock<IExcelRepository>();
            _auctionAssetRepoMock = new Mock<IAuctionAssetsRepository>();

            _handler = new UpdateAuctionHandler(
                _auctionRepoMock.Object,
                _categoryRepoMock.Object,
                _unitOfWorkMock.Object,
                _currentUserMock.Object,
                _excelRepoMock.Object,
                _auctionAssetRepoMock.Object
            );
        }

        [Test]
        public async Task Handle_ShouldUpdateAuctionSuccessfully_WhenValidRequest()
        {
            // Arrange
            var auctionId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var command = new UpdateAuctionCommand
            {
                AuctionId = auctionId,
                CategoryId = 1,
                AuctionAssetFile = null,
            };

            _currentUserMock.Setup(x => x.GetUserId()).Returns(userId.ToString());
            _auctionRepoMock
                .Setup(x => x.FindAuctionByIdAsync(auctionId))
                .ReturnsAsync(
                    new MyApp.Core.Entities.Auction
                    {
                        AuctionId = auctionId,
                        Updateable = true,
                        CategoryId = 1,
                    }
                );
            _auctionRepoMock.Setup(x => x.UpdateAuctionAsync(command, userId)).ReturnsAsync(true);

            _unitOfWorkMock.Setup(x => x.BeginTransaction());
            _unitOfWorkMock.Setup(x => x.CommitAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.AreEqual(auctionId, result);
            _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
        }

        [Test]
        public void Handle_ShouldThrowUnauthorizedAccessException_WhenUserIdIsInvalid()
        {
            // Arrange
            _currentUserMock.Setup(x => x.GetUserId()).Returns((string)null);
            var command = new UpdateAuctionCommand { AuctionId = Guid.NewGuid(), CategoryId = 1 };

            // Act & Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _handler.Handle(command, CancellationToken.None)
            );
        }

        [Test]
        public void Handle_ShouldThrowValidationException_WhenAuctionNotFound()
        {
            var auctionId = Guid.NewGuid();
            _currentUserMock.Setup(x => x.GetUserId()).Returns(Guid.NewGuid().ToString());
            _auctionRepoMock
                .Setup(x => x.FindAuctionByIdAsync(auctionId))
                .ReturnsAsync((MyApp.Core.Entities.Auction)null);

            var command = new UpdateAuctionCommand { AuctionId = auctionId, CategoryId = 1 };

            Assert.ThrowsAsync<ValidationException>(() =>
                _handler.Handle(command, CancellationToken.None)
            );
        }

        [Test]
        public void Handle_ShouldThrowValidationException_WhenAuctionNotUpdateable()
        {
            var auctionId = Guid.NewGuid();
            _currentUserMock.Setup(x => x.GetUserId()).Returns(Guid.NewGuid().ToString());
            _auctionRepoMock
                .Setup(x => x.FindAuctionByIdAsync(auctionId))
                .ReturnsAsync(
                    new MyApp.Core.Entities.Auction
                    {
                        AuctionId = auctionId,
                        Updateable = false,
                        CategoryId = 1,
                    }
                );

            var command = new UpdateAuctionCommand { AuctionId = auctionId, CategoryId = 1 };

            Assert.ThrowsAsync<ValidationException>(() =>
                _handler.Handle(command, CancellationToken.None)
            );
        }

        [Test]
        public void Handle_ShouldThrowValidationException_WhenCategoryNotExist()
        {
            // Arrange
            var auctionId = Guid.NewGuid();
            _currentUserMock.Setup(x => x.GetUserId()).Returns(Guid.NewGuid().ToString());
            _auctionRepoMock
                .Setup(x => x.FindAuctionByIdAsync(auctionId))
                .ReturnsAsync(
                    new MyApp.Core.Entities.Auction
                    {
                        AuctionId = auctionId,
                        Updateable = true,
                        CategoryId = 1,
                    }
                );

            var command = new UpdateAuctionCommand { AuctionId = auctionId, CategoryId = 2 };

            _categoryRepoMock
                .Setup(x => x.FindByIdAsync(command.CategoryId))
                .ReturnsAsync((AuctionCategory)null);

            Assert.ThrowsAsync<ValidationException>(() =>
                _handler.Handle(command, CancellationToken.None)
            );
        }

        [Test]
        public async Task Handle_ShouldProcessExcelFile_WhenFileProvided()
        {
            var auctionId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(10);

            _currentUserMock.Setup(x => x.GetUserId()).Returns(userId.ToString());
            _auctionRepoMock
                .Setup(x => x.FindAuctionByIdAsync(auctionId))
                .ReturnsAsync(
                    new MyApp.Core.Entities.Auction
                    {
                        AuctionId = auctionId,
                        Updateable = true,
                        CategoryId = 1,
                    }
                );
            _auctionRepoMock
                .Setup(x => x.UpdateAuctionAsync(It.IsAny<UpdateAuctionCommand>(), userId))
                .ReturnsAsync(true);

            _unitOfWorkMock.Setup(x => x.BeginTransaction());
            _unitOfWorkMock.Setup(x => x.CommitAsync()).Returns(Task.CompletedTask);

            var command = new UpdateAuctionCommand
            {
                AuctionId = auctionId,
                CategoryId = 1,
                AuctionAssetFile = fileMock.Object,
            };

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.AreEqual(auctionId, result);
            _auctionAssetRepoMock.Verify(x => x.DeleteByAuctionIdAsync(auctionId), Times.Once);
            _excelRepoMock.Verify(
                x => x.SaveAssetsFromExcelAsync(auctionId, fileMock.Object, userId),
                Times.Once
            );
        }

        [Test]
        public void Handle_ShouldRollback_WhenUpdateFails()
        {
            var auctionId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            _currentUserMock.Setup(x => x.GetUserId()).Returns(userId.ToString());
            _auctionRepoMock
                .Setup(x => x.FindAuctionByIdAsync(auctionId))
                .ReturnsAsync(
                    new MyApp.Core.Entities.Auction
                    {
                        AuctionId = auctionId,
                        Updateable = true,
                        CategoryId = 1,
                    }
                );
            _auctionRepoMock
                .Setup(x => x.UpdateAuctionAsync(It.IsAny<UpdateAuctionCommand>(), userId))
                .ReturnsAsync(false);

            _unitOfWorkMock.Setup(x => x.BeginTransaction());
            _unitOfWorkMock.Setup(x => x.RollbackAsync()).Returns(Task.CompletedTask);

            var command = new UpdateAuctionCommand { AuctionId = auctionId, CategoryId = 1 };

            Assert.ThrowsAsync<InvalidOperationException>(() =>
                _handler.Handle(command, CancellationToken.None)
            );
            _unitOfWorkMock.Verify(x => x.RollbackAsync(), Times.Once);
        }
    }
}
