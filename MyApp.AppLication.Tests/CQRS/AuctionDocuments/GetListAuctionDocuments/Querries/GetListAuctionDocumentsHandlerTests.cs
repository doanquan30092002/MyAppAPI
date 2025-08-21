using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MyApp.Application.CQRS.Auction.GetListAuction.Querries;
using MyApp.Application.CQRS.Auction.GetListAution.Querries;
using MyApp.Application.CQRS.AuctionDocuments.GetListAuctionDocuments.Querries;
using MyApp.Application.Interfaces.IGetListDocumentsRepository;
using NUnit.Framework;

namespace MyApp.Application.CQRS.AuctionDocuments.GetListAuctionDocuments.Querries.Tests
{
    [TestFixture]
    public class GetListAuctionDocumentsHandlerTests
    {
        // Fake repository to simulate IGetListDocumentsRepository behavior without Moq
        private class FakeGetListDocumentsRepository : IGetListDocumentsRepository
        {
            private readonly GetListAuctionDocumentsResponse _response;
            private readonly bool _throwException;

            public FakeGetListDocumentsRepository(
                GetListAuctionDocumentsResponse response = null,
                bool throwException = false
            )
            {
                _response = response ?? new GetListAuctionDocumentsResponse();
                _throwException = throwException;
            }

            public Task<GetListAuctionDocumentsResponse> GetListAuctionDocumentsAsync(
                GetListAuctionDocumentsRequest request
            )
            {
                if (_throwException)
                {
                    throw new Exception("Simulated repository error");
                }
                return Task.FromResult(_response);
            }
        }

        private GetListAuctionDocumentsHandler _handler;
        private GetListAuctionDocumentsRequest _request;
        private FakeGetListDocumentsRepository _repository;

        [SetUp]
        public void SetUp()
        {
            _request = new GetListAuctionDocumentsRequest
            {
                AuctionId = Guid.NewGuid(),
                CitizenIdentification = null,
                Name = null,
                TagName = null,
                StatusTicket = null,
                StatusDeposit = null,
                IsAttended = null,
                StatusRefund = null,
                SortBy = null,
                IsAscending = true,
                PageNumber = null,
                PageSize = null,
            };
            _repository = new FakeGetListDocumentsRepository();
            _handler = new GetListAuctionDocumentsHandler(_repository);
        }

        [Test]
        public void Constructor_NullRepository_ThrowsArgumentNullException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() =>
                new GetListAuctionDocumentsHandler(null)
            );
            Assert.That(ex.ParamName, Is.EqualTo("getListDocumentsRepository"));
        }

        [Test]
        public async Task Handle_SuccessfulFetch_ReturnsPopulatedGetListAuctionDocumentsResponse()
        {
            // Arrange
            var auctionDocuments = new List<ListAuctionDocumentsDTO>
            {
                new ListAuctionDocumentsDTO
                {
                    AuctionDocumentsId = Guid.NewGuid(),
                    UserId = Guid.NewGuid(),
                    CitizenIdentification = "123456789",
                    Name = "Test User",
                    TagName = "Asset1",
                    Deposit = 1000m,
                    StatusDeposit = 1,
                    RegistrationFee = 500m,
                    StatusTicket = 1,
                    IsAttended = true,
                    StatusRefund = 2,
                    RefundReason = "Approved",
                    RefundProof = "proof1.pdf",
                    NumericalOrder = 1,
                    Note = "Note 1",
                },
                new ListAuctionDocumentsDTO
                {
                    AuctionDocumentsId = Guid.NewGuid(),
                    UserId = Guid.NewGuid(),
                    CitizenIdentification = "987654321",
                    Name = "Another User",
                    TagName = "Asset2",
                    Deposit = 2000m,
                    StatusDeposit = 2,
                    RegistrationFee = 600m,
                    StatusTicket = 2,
                    IsAttended = false,
                    StatusRefund = 3,
                    RefundReason = "Rejected",
                    RefundProof = "proof2.pdf",
                    NumericalOrder = 2,
                    Note = "Note 2",
                },
            };
            var documentsAssetList = new List<DocumentsAssetDto>
            {
                new DocumentsAssetDto { AssetId = Guid.NewGuid(), Quantity = 2 },
                new DocumentsAssetDto { AssetId = Guid.NewGuid(), Quantity = 1 },
            };
            var response = new GetListAuctionDocumentsResponse
            {
                TotalCount = 2,
                DocumentsAssetList = documentsAssetList,
                AuctionDocuments = auctionDocuments,
            };
            _repository = new FakeGetListDocumentsRepository(response);
            _handler = new GetListAuctionDocumentsHandler(_repository);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<GetListAuctionDocumentsResponse>(result);
            Assert.That(result.TotalCount, Is.EqualTo(response.TotalCount));
            Assert.That(
                result.DocumentsAssetList.Count,
                Is.EqualTo(response.DocumentsAssetList.Count)
            );
            for (int i = 0; i < response.DocumentsAssetList.Count; i++)
            {
                Assert.That(
                    result.DocumentsAssetList[i].AssetId,
                    Is.EqualTo(response.DocumentsAssetList[i].AssetId)
                );
                Assert.That(
                    result.DocumentsAssetList[i].Quantity,
                    Is.EqualTo(response.DocumentsAssetList[i].Quantity)
                );
            }
            Assert.That(result.AuctionDocuments.Count, Is.EqualTo(response.AuctionDocuments.Count));
            for (int i = 0; i < response.AuctionDocuments.Count; i++)
            {
                Assert.That(
                    result.AuctionDocuments[i].AuctionDocumentsId,
                    Is.EqualTo(response.AuctionDocuments[i].AuctionDocumentsId)
                );
                Assert.That(
                    result.AuctionDocuments[i].UserId,
                    Is.EqualTo(response.AuctionDocuments[i].UserId)
                );
                Assert.That(
                    result.AuctionDocuments[i].CitizenIdentification,
                    Is.EqualTo(response.AuctionDocuments[i].CitizenIdentification)
                );
                Assert.That(
                    result.AuctionDocuments[i].Name,
                    Is.EqualTo(response.AuctionDocuments[i].Name)
                );
                Assert.That(
                    result.AuctionDocuments[i].TagName,
                    Is.EqualTo(response.AuctionDocuments[i].TagName)
                );
                Assert.That(
                    result.AuctionDocuments[i].Deposit,
                    Is.EqualTo(response.AuctionDocuments[i].Deposit)
                );
                Assert.That(
                    result.AuctionDocuments[i].StatusDeposit,
                    Is.EqualTo(response.AuctionDocuments[i].StatusDeposit)
                );
                Assert.That(
                    result.AuctionDocuments[i].RegistrationFee,
                    Is.EqualTo(response.AuctionDocuments[i].RegistrationFee)
                );
                Assert.That(
                    result.AuctionDocuments[i].StatusTicket,
                    Is.EqualTo(response.AuctionDocuments[i].StatusTicket)
                );
                Assert.That(
                    result.AuctionDocuments[i].IsAttended,
                    Is.EqualTo(response.AuctionDocuments[i].IsAttended)
                );
                Assert.That(
                    result.AuctionDocuments[i].StatusRefund,
                    Is.EqualTo(response.AuctionDocuments[i].StatusRefund)
                );
                Assert.That(
                    result.AuctionDocuments[i].RefundReason,
                    Is.EqualTo(response.AuctionDocuments[i].RefundReason)
                );
                Assert.That(
                    result.AuctionDocuments[i].RefundProof,
                    Is.EqualTo(response.AuctionDocuments[i].RefundProof)
                );
                Assert.That(
                    result.AuctionDocuments[i].NumericalOrder,
                    Is.EqualTo(response.AuctionDocuments[i].NumericalOrder)
                );
                Assert.That(
                    result.AuctionDocuments[i].Note,
                    Is.EqualTo(response.AuctionDocuments[i].Note)
                );
            }
        }

        [Test]
        public async Task Handle_NoDocuments_ReturnsEmptyGetListAuctionDocumentsResponse()
        {
            // Arrange
            var response = new GetListAuctionDocumentsResponse
            {
                TotalCount = 0,
                DocumentsAssetList = new List<DocumentsAssetDto>(),
                AuctionDocuments = new List<ListAuctionDocumentsDTO>(),
            };
            _repository = new FakeGetListDocumentsRepository(response);
            _handler = new GetListAuctionDocumentsHandler(_repository);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<GetListAuctionDocumentsResponse>(result);
            Assert.That(result.TotalCount, Is.EqualTo(response.TotalCount));
            Assert.That(result.DocumentsAssetList.Count, Is.EqualTo(0));
            Assert.That(result.AuctionDocuments.Count, Is.EqualTo(0));
        }

        [Test]
        public void Handle_ExceptionThrown_ThrowsException()
        {
            // Arrange
            _repository = new FakeGetListDocumentsRepository(throwException: true);
            _handler = new GetListAuctionDocumentsHandler(_repository);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _handler.Handle(_request, CancellationToken.None)
            );
            Assert.That(ex.Message, Is.EqualTo("Simulated repository error"));
        }
    }
}
