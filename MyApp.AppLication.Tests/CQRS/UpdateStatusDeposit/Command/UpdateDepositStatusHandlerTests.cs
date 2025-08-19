using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MyApp.Application.CQRS.UpdateStatusDeposit.Command;
using MyApp.Application.Interfaces.IUpdateDepositStatus;
using NUnit.Framework;

namespace MyApp.Application.CQRS.UpdateStatusDeposit.Command.Tests
{
    [TestFixture]
    public class UpdateDepositStatusHandlerTests
    {
        private UpdateDepositStatusHandler _handler;
        private FakeUpdateDepositStatusRepository _repo;
        private FakeMediator _mediator;
        private UpdateDepositStatusRequest _request;

        [SetUp]
        public void SetUp()
        {
            _repo = new FakeUpdateDepositStatusRepository();
            _mediator = new FakeMediator();
            _handler = new UpdateDepositStatusHandler(_repo, _mediator);

            _request = new UpdateDepositStatusRequest
            {
                AuctionId = Guid.NewGuid(),
                AuctionDocumentsId = Guid.NewGuid(),
                Note = "Test",
            };
        }

        [Test]
        public async Task Handle_StatusUpdateFalse_ReturnsFalse()
        {
            // Arrange
            _repo.ShouldReturnStatus = false;

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.StatusUpdate);
        }

        [Test]
        public async Task Handle_StatusUpdateTrue_ReturnsTrue()
        {
            // Arrange
            _repo.ShouldReturnStatus = true;

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.StatusUpdate);
        }
    }

    // ======= Fakes =======

    // Fake repository thay cho IUpdateDepositStatus
    public class FakeUpdateDepositStatusRepository : IUpdateDepositStatus
    {
        public bool ShouldReturnStatus { get; set; } = false;

        public Task<UpdateDepositStatusResponse> UpdateDepositStatus(
            UpdateDepositStatusRequest updateDepositStatusRequest,
            CancellationToken cancellationToken
        )
        {
            return Task.FromResult(
                new UpdateDepositStatusResponse { StatusUpdate = ShouldReturnStatus }
            );
        }
    }

    // FakeMediator đầy đủ chữ ký cho MediatR 12
    public class FakeMediator : IMediator
    {
        // IRequest<TResponse>
        public Task<TResponse> Send<TResponse>(
            IRequest<TResponse> request,
            CancellationToken cancellationToken = default
        ) => Task.FromResult(default(TResponse)!);

        // IRequest (không có TResponse)
        public Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default)
            where TRequest : IRequest => Task.CompletedTask;

        // object-based send
        public Task<object?> Send(object request, CancellationToken cancellationToken = default) =>
            Task.FromResult<object?>(null);

        // Publish
        public Task Publish(object notification, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task Publish<TNotification>(
            TNotification notification,
            CancellationToken cancellationToken = default
        )
            where TNotification : INotification => Task.CompletedTask;

        // Streaming APIs
        public IAsyncEnumerable<TResponse> CreateStream<TResponse>(
            IStreamRequest<TResponse> request,
            CancellationToken cancellationToken = default
        )
        {
            return GetEmptyAsyncEnumerable<TResponse>();
        }

        public IAsyncEnumerable<object?> CreateStream(
            object request,
            CancellationToken cancellationToken = default
        )
        {
            return GetEmptyAsyncEnumerable<object?>();
        }

        private async IAsyncEnumerable<T> GetEmptyAsyncEnumerable<T>()
        {
            yield break;
        }
    }
}
