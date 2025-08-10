using Moq;
using MyApp.Application.Interfaces.ListCustomer;

namespace MyApp.Application.CQRS.ListCustomer.Tests
{
    [TestFixture()]
    public class ListCustomerHandlerTests
    {
        private Mock<IListCustomerRepository> _repositoryMock;
        private ListCustomerHandler _handler;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IListCustomerRepository>();
            _handler = new ListCustomerHandler(_repositoryMock.Object);
        }

        [Test]
        public async Task Handle_WhenCustomerListIsNull_ShouldReturnEmptyList()
        {
            // Arrange
            var request = new ListCustomerRequest
            {
                PageNumber = 1,
                PageSize = 10,
                Search = "Nguyễn",
            };

            _repositoryMock
                .Setup(r => r.ListCustomer(request.PageNumber, request.PageSize, request.Search))
                .ReturnsAsync((List<CustomerInfo>?)null);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.That(result.TotalCount, Is.EqualTo(0));
            Assert.That(result.CustomerInfos, Is.Empty);
            Assert.That(result.PageNumber, Is.EqualTo(1));
            Assert.That(result.PageSize, Is.EqualTo(10));
        }

        [Test]
        public async Task Handle_WhenCustomerListExists_ShouldReturnCorrectData()
        {
            // Arrange
            var request = new ListCustomerRequest
            {
                PageNumber = 1,
                PageSize = 5,
                Search = "Trần",
            };

            var fakeCustomerId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var fakeUserId = Guid.Parse("22222222-2222-2222-2222-222222222222");
            var fakeCreatedBy = Guid.Parse("33333333-3333-3333-3333-333333333333");

            var fakeCustomers = new List<CustomerInfo>
            {
                new CustomerInfo
                {
                    AccountId = fakeCustomerId,
                    PhoneNumber = "0909123456",
                    Email = "tranvanan@example.com",
                    IsActive = true,
                    RoleId = 2,
                    RoleName = "Khách hàng",
                    UserId = fakeUserId,
                    CitizenIdentification = "012345678912",
                    Name = "Trần Văn An",
                    BirthDay = new DateTime(1990, 5, 20),
                    Nationality = "Việt Nam",
                    Gender = true,
                    ValidDate = new DateTime(2030, 5, 20),
                    OriginLocation = "Hà Nội",
                    RecentLocation = "TP. Hồ Chí Minh",
                    IssueDate = new DateTime(2015, 6, 1),
                    IssueBy = "Công an Hà Nội",
                    CreatedAt = new DateTime(2025, 1, 1),
                    CreatedBy = fakeCreatedBy,
                    UpdatedAt = new DateTime(2025, 1, 1),
                    UpdatedBy = fakeUserId,
                },
            };

            _repositoryMock
                .Setup(r => r.ListCustomer(request.PageNumber, request.PageSize, request.Search))
                .ReturnsAsync(fakeCustomers);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.That(result.TotalCount, Is.EqualTo(1));
            Assert.That(result.CustomerInfos, Has.Count.EqualTo(1));

            var actual = result.CustomerInfos[0];

            Assert.That(actual.AccountId, Is.EqualTo(fakeCustomerId));
            Assert.That(actual.PhoneNumber, Is.EqualTo("0909123456"));
            Assert.That(actual.Email, Is.EqualTo("tranvanan@example.com"));
            Assert.That(actual.IsActive, Is.True);
            Assert.That(actual.RoleId, Is.EqualTo(2));
            Assert.That(actual.RoleName, Is.EqualTo("Khách hàng"));
            Assert.That(actual.UserId, Is.EqualTo(fakeUserId));
            Assert.That(actual.CitizenIdentification, Is.EqualTo("012345678912"));
            Assert.That(actual.Name, Is.EqualTo("Trần Văn An"));
            Assert.That(actual.BirthDay, Is.EqualTo(new DateTime(1990, 5, 20)));
            Assert.That(actual.Nationality, Is.EqualTo("Việt Nam"));
            Assert.That(actual.Gender, Is.True);
            Assert.That(actual.ValidDate, Is.EqualTo(new DateTime(2030, 5, 20)));
            Assert.That(actual.OriginLocation, Is.EqualTo("Hà Nội"));
            Assert.That(actual.RecentLocation, Is.EqualTo("TP. Hồ Chí Minh"));
            Assert.That(actual.IssueDate, Is.EqualTo(new DateTime(2015, 6, 1)));
            Assert.That(actual.IssueBy, Is.EqualTo("Công an Hà Nội"));
            Assert.That(actual.CreatedAt, Is.EqualTo(new DateTime(2025, 1, 1)));
            Assert.That(actual.CreatedBy, Is.EqualTo(fakeCreatedBy));
            Assert.That(actual.UpdatedAt, Is.EqualTo(new DateTime(2025, 1, 1)));
            Assert.That(actual.UpdatedBy, Is.EqualTo(fakeUserId));
        }
    }
}
