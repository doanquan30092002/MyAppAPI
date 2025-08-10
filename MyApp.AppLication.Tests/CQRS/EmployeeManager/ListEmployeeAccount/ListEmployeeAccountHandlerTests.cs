using Moq;
using MyApp.Application.Interfaces.EmployeeManager;

namespace MyApp.Application.CQRS.EmployeeManager.ListEmployeeAccount.Tests
{
    [TestFixture()]
    public class ListEmployeeAccountHandlerTests
    {
        private Mock<IEmployeeManagerRepository> _repoMock;
        private ListEmployeeAccountHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _repoMock = new Mock<IEmployeeManagerRepository>();
            _handler = new ListEmployeeAccountHandler(_repoMock.Object);
        }

        [Test]
        public async Task Handle_WhenDataExists_ReturnsExpectedResponse()
        {
            // Arrange
            var request = new ListEmployeeAccountRequest
            {
                PageNumber = 1,
                PageSize = 10,
                Seach = "nguyen",
                RoleId = 3,
            };

            var expectedAccount = new EmployeeAccountResponse
            {
                AccountId = new Guid("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"),
                PhoneNumber = "0987654321",
                Email = "nguyenvana@example.com",
                IsActive = true,
                RoleId = 3,
                RoleName = "Nhân viên",
                UserId = new Guid("11111111-2222-3333-4444-555555555555"),
                CitizenIdentification = "012345678901",
                Name = "Nguyễn Văn A",
                BirthDay = new DateTime(1990, 5, 20),
                Nationality = "Việt Nam",
                Gender = true,
                ValidDate = new DateTime(2030, 5, 20),
                OriginLocation = "Hà Nội",
                RecentLocation = "TP Hồ Chí Minh",
                IssueDate = new DateTime(2023, 5, 20),
                IssueBy = "Cục Cảnh sát QLHC",
                CreatedAt = new DateTime(2020, 5, 20),
                CreatedBy = new Guid("66666666-7777-8888-9999-aaaaaaaaaaaa"),
                UpdatedAt = new DateTime(2024, 5, 20),
                UpdatedBy = new Guid("bbbbbbbb-cccc-dddd-eeee-ffffffffffff"),
            };

            var fakeAccounts = new List<EmployeeAccountResponse> { expectedAccount };

            _repoMock
                .Setup(r => r.ListEmployeeAccount(1, 10, "nguyen", 3))
                .ReturnsAsync(fakeAccounts);

            _repoMock.Setup(r => r.GetEmployeeAccountTotal()).ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.PageNumber, Is.EqualTo(1));
            Assert.That(result.PageSize, Is.EqualTo(10));
            Assert.That(result.TotalCount, Is.EqualTo(1));
            Assert.That(result.EmployeeAccounts.Count, Is.EqualTo(1));

            var actual = result.EmployeeAccounts[0];
            Assert.That(actual.AccountId, Is.EqualTo(expectedAccount.AccountId));
            Assert.That(actual.PhoneNumber, Is.EqualTo(expectedAccount.PhoneNumber));
            Assert.That(actual.Email, Is.EqualTo(expectedAccount.Email));
            Assert.That(actual.IsActive, Is.EqualTo(expectedAccount.IsActive));
            Assert.That(actual.RoleId, Is.EqualTo(expectedAccount.RoleId));
            Assert.That(actual.RoleName, Is.EqualTo(expectedAccount.RoleName));
            Assert.That(actual.UserId, Is.EqualTo(expectedAccount.UserId));
            Assert.That(
                actual.CitizenIdentification,
                Is.EqualTo(expectedAccount.CitizenIdentification)
            );
            Assert.That(actual.Name, Is.EqualTo(expectedAccount.Name));
            Assert.That(actual.BirthDay, Is.EqualTo(expectedAccount.BirthDay));
            Assert.That(actual.Nationality, Is.EqualTo(expectedAccount.Nationality));
            Assert.That(actual.Gender, Is.EqualTo(expectedAccount.Gender));
            Assert.That(actual.ValidDate, Is.EqualTo(expectedAccount.ValidDate));
            Assert.That(actual.OriginLocation, Is.EqualTo(expectedAccount.OriginLocation));
            Assert.That(actual.RecentLocation, Is.EqualTo(expectedAccount.RecentLocation));
            Assert.That(actual.IssueDate, Is.EqualTo(expectedAccount.IssueDate));
            Assert.That(actual.IssueBy, Is.EqualTo(expectedAccount.IssueBy));
            Assert.That(actual.CreatedAt, Is.EqualTo(expectedAccount.CreatedAt));
            Assert.That(actual.CreatedBy, Is.EqualTo(expectedAccount.CreatedBy));
            Assert.That(actual.UpdatedAt, Is.EqualTo(expectedAccount.UpdatedAt));
            Assert.That(actual.UpdatedBy, Is.EqualTo(expectedAccount.UpdatedBy));

            _repoMock.Verify(r => r.ListEmployeeAccount(1, 10, "nguyen", 3), Times.Once);
            _repoMock.Verify(r => r.GetEmployeeAccountTotal(), Times.Once);
        }

        [Test]
        public async Task Handle_WhenNoData_ReturnsEmptyListAndTotalZero()
        {
            // Arrange
            var request = new ListEmployeeAccountRequest
            {
                PageNumber = 2,
                PageSize = 5,
                Seach = null,
                RoleId = null,
            };

            _repoMock
                .Setup(r => r.ListEmployeeAccount(2, 5, null, null))
                .ReturnsAsync(new List<EmployeeAccountResponse>());

            _repoMock.Setup(r => r.GetEmployeeAccountTotal()).ReturnsAsync(0);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.That(result.EmployeeAccounts, Is.Empty);
            Assert.That(result.TotalCount, Is.EqualTo(0));
            Assert.That(result.PageNumber, Is.EqualTo(2));
            Assert.That(result.PageSize, Is.EqualTo(5));

            _repoMock.Verify(r => r.ListEmployeeAccount(2, 5, null, null), Times.Once);
            _repoMock.Verify(r => r.GetEmployeeAccountTotal(), Times.Once);
        }
    }
}
