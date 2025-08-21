using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Moq;
using MyApp.Application.Common.Services.AuctionRoundPriceHub;
using MyApp.Application.CQRS.SaveListPrices.Command;
using MyApp.Application.Interfaces.IGetListEnteredPricesRepository;
using MyApp.Application.Interfaces.ISaveListPricesRepository;
using MyApp.Core.Entities;
using NUnit.Framework;

namespace MyApp.Application.CQRS.SaveListPrices.Command.Tests
{
    [TestFixture]
    public class SaveListPricesHandlerTests
    {
        private Mock<ISaveListPricesRepository> _fakeSaveRepo;
        private Mock<IGetListEnteredPricesRepository> _fakeGetRepo;
        private Mock<IHubContext<AuctionRoundPriceHub>> _fakeHubContext;
        private Mock<IHubClients> _fakeHubClients;
        private Mock<IClientProxy> _fakeClientProxy;

        private SaveListPricesHandler _handler;

        [SetUp]
        public void Setup()
        {
            _fakeSaveRepo = new Mock<ISaveListPricesRepository>();
            _fakeGetRepo = new Mock<IGetListEnteredPricesRepository>();
            _fakeHubContext = new Mock<IHubContext<AuctionRoundPriceHub>>();
            _fakeHubClients = new Mock<IHubClients>();
            _fakeClientProxy = new Mock<IClientProxy>();

            // Setup hub context giả
            _fakeHubClients
                .Setup(c => c.Group(It.IsAny<string>()))
                .Returns(_fakeClientProxy.Object);

            _fakeHubContext.Setup(h => h.Clients).Returns(_fakeHubClients.Object);

            _handler = new SaveListPricesHandler(
                _fakeSaveRepo.Object,
                _fakeHubContext.Object,
                _fakeGetRepo.Object
            );
        }

        [Test]
        public async Task Handle_ShouldReturnResponse_WhenSaveSuccess()
        {
            // Arrange
            var request = new SaveListPricesRequest
            {
                AuctionRoundId = Guid.NewGuid(),
                resultDTOs = new List<ResultDTO>
                {
                    new ResultDTO
                    {
                        UserName = "testUser",
                        AuctionPrice = 1000,
                        CreatedBy = Guid.NewGuid(),
                    },
                },
            };

            _fakeSaveRepo
                .Setup(r => r.InserListPrices(It.IsAny<SaveListPricesRequest>()))
                .ReturnsAsync(true);

            // Fix: Adjust the type of the returned value to match the expected type
            _fakeGetRepo
                .Setup(r => r.GetListEnteredPricesAsync(It.IsAny<Guid>()))
                .ReturnsAsync(
                    new List<AuctionRoundPrices>
                    {
                        new AuctionRoundPrices
                        { /* Initialize properties as needed */
                        },
                    }
                );

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<SaveListPricesResponse>(result);

            _fakeSaveRepo.Verify(
                r => r.InserListPrices(It.IsAny<SaveListPricesRequest>()),
                Times.Once
            );
            _fakeHubClients.Verify(c => c.Group(It.IsAny<string>()), Times.Once);
            _fakeClientProxy.Verify(
                p => p.SendCoreAsync("ReceiveLatestPrices", It.IsAny<object[]>(), default),
                Times.Once
            );
        }

        [Test]
        public void Handle_ShouldThrowValidationException_WhenSaveFails()
        {
            // Arrange
            var request = new SaveListPricesRequest
            {
                AuctionRoundId = Guid.NewGuid(),
                resultDTOs = new List<ResultDTO>(),
            };

            _fakeSaveRepo
                .Setup(r => r.InserListPrices(It.IsAny<SaveListPricesRequest>()))
                .ReturnsAsync(false);

            // Act & Assert
            Assert.ThrowsAsync<ValidationException>(async () =>
                await _handler.Handle(request, CancellationToken.None)
            );
        }

        [Test]
        public void Handle_ShouldThrowValidationException_WhenRepositoryThrowsException()
        {
            // Arrange
            var request = new SaveListPricesRequest
            {
                AuctionRoundId = Guid.NewGuid(),
                resultDTOs = new List<ResultDTO>(),
            };

            _fakeSaveRepo
                .Setup(r => r.InserListPrices(It.IsAny<SaveListPricesRequest>()))
                .ThrowsAsync(new Exception("DB Error"));

            // Act & Assert
            Assert.ThrowsAsync<ValidationException>(async () =>
                await _handler.Handle(request, CancellationToken.None)
            );
        }
    }
}
