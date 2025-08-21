using System;
using System.Threading;
using System.Threading.Tasks;
using MyApp.Application.CQRS.GetUserInfo.Queries;
using MyApp.Application.Interfaces.IGetUserInfoRepository;
using NUnit.Framework;

namespace MyApp.Application.CQRS.GetUserInfo.Queries.Tests
{
    [TestFixture]
    public class GetUserInfoHandlerTests
    {
        // Fake repository to simulate IGetUserInfoRepository behavior without Moq
        private class FakeGetUserInfoRepository : IGetUserInfoRepository
        {
            private readonly GetUserInfoResponse _response;
            private readonly bool _returnNull;

            public FakeGetUserInfoRepository(
                GetUserInfoResponse response = null,
                bool returnNull = false
            )
            {
                _response = response;
                _returnNull = returnNull;
            }

            public Task<GetUserInfoResponse> GetUserInfoAsync(
                Guid userId,
                CancellationToken cancellationToken
            )
            {
                return Task.FromResult(_returnNull ? null : _response);
            }
        }

        private GetUserInfoHandler _handler;
        private GetUserInfoQuery _request;
        private FakeGetUserInfoRepository _repository;

        [SetUp]
        public void SetUp()
        {
            _request = new GetUserInfoQuery { UserId = Guid.NewGuid() };
            _repository = new FakeGetUserInfoRepository();
            _handler = new GetUserInfoHandler(_repository);
        }

        [Test]
        public void Constructor_NullRepository_ThrowsArgumentNullException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => new GetUserInfoHandler(null));
            Assert.That(ex.ParamName, Is.EqualTo("getUserInfoRepository"));
        }

        [Test]
        public async Task Handle_UserExists_ReturnsPopulatedGetUserInfoResponse()
        {
            // Arrange
            var expectedResponse = new GetUserInfoResponse
            {
                CitizenIdentification = "123456789",
                Name = "Test User",
                BirthDay = DateTime.Now.AddYears(-20),
                Nationality = "VN",
                Gender = true,
                ValidDate = DateTime.Now,
                OriginLocation = "HN",
                RecentLocation = "HCM",
                IssueDate = DateTime.Now.AddYears(-1),
                IssueBy = "CA",
                PhoneNumber = "0912345678",
                Email = "test@example.com",
                RoleName = "User",
            };
            _repository = new FakeGetUserInfoRepository(expectedResponse);
            _handler = new GetUserInfoHandler(_repository);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<GetUserInfoResponse>(result);
            Assert.That(
                result.CitizenIdentification,
                Is.EqualTo(expectedResponse.CitizenIdentification)
            );
            Assert.That(result.Name, Is.EqualTo(expectedResponse.Name));
            Assert.That(result.BirthDay, Is.EqualTo(expectedResponse.BirthDay));
            Assert.That(result.Nationality, Is.EqualTo(expectedResponse.Nationality));
            Assert.That(result.Gender, Is.EqualTo(expectedResponse.Gender));
            Assert.That(result.ValidDate, Is.EqualTo(expectedResponse.ValidDate));
            Assert.That(result.OriginLocation, Is.EqualTo(expectedResponse.OriginLocation));
            Assert.That(result.RecentLocation, Is.EqualTo(expectedResponse.RecentLocation));
            Assert.That(result.IssueDate, Is.EqualTo(expectedResponse.IssueDate));
            Assert.That(result.IssueBy, Is.EqualTo(expectedResponse.IssueBy));
            Assert.That(result.PhoneNumber, Is.EqualTo(expectedResponse.PhoneNumber));
            Assert.That(result.Email, Is.EqualTo(expectedResponse.Email));
            Assert.That(result.RoleName, Is.EqualTo(expectedResponse.RoleName));
        }

        [Test]
        public async Task Handle_UserDoesNotExist_ReturnsEmptyGetUserInfoResponse()
        {
            // Arrange
            _repository = new FakeGetUserInfoRepository(returnNull: true);
            _handler = new GetUserInfoHandler(_repository);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<GetUserInfoResponse>(result);
            Assert.IsNull(result.CitizenIdentification);
            Assert.IsNull(result.Name);
            Assert.That(result.BirthDay, Is.EqualTo(default(DateTime)));
            Assert.IsNull(result.Nationality);
            Assert.That(result.Gender, Is.False);
            Assert.That(result.ValidDate, Is.EqualTo(default(DateTime)));
            Assert.IsNull(result.OriginLocation);
            Assert.IsNull(result.RecentLocation);
            Assert.That(result.IssueDate, Is.EqualTo(default(DateTime)));
            Assert.IsNull(result.IssueBy);
            Assert.IsNull(result.PhoneNumber);
            Assert.IsNull(result.Email);
            Assert.IsNull(result.RoleName);
        }
    }
}
