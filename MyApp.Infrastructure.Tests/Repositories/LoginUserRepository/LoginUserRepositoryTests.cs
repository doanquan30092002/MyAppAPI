using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.Interfaces.ILoginUserRepository;
using MyApp.Core.DTOs.LoginUserDTO;
using MyApp.Core.Entities;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories.LoginUserRepository.Tests
{
    [TestFixture()]
    public class LoginUserRepositoryTests
    {
        private AppDbContext _context;
        private IMapper _mapper;
        private ILoginUserRepository _repository;

        [SetUp]
        public void Setup()
        {
            // In-memory database
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);

            // Seed test data
            var role = new Role { RoleId = 1, RoleName = "Admin" };

            var user = new User
            {
                Id = Guid.NewGuid(),
                CitizenIdentification = "123456789012",
                Name = "John Doe",
                BirthDay = new DateTime(1990, 1, 1),
                Nationality = "Vietnam",
                Gender = true,
                ValidDate = new DateTime(2030, 1, 1),
                OriginLocation = "Hanoi",
                RecentLocation = "HCMC",
                IssueDate = new DateTime(2020, 1, 1),
                IssueBy = "Police Dept",
                CreatedBy = Guid.NewGuid(),
                UpdatedBy = Guid.NewGuid(),
            };

            var account = new Account
            {
                AccountId = Guid.NewGuid(),
                PhoneNumber = "0123456789",
                Email = "test@example.com",
                Password = "123456",
                CreatedBy = Guid.NewGuid(),
                UpdatedBy = Guid.NewGuid(),
                IsActive = true,
                Role = role,
                RoleId = role.RoleId,
                User = user,
                UserId = user.Id,
            };

            _context.Roles.Add(role);
            _context.Users.Add(user);
            _context.Accounts.Add(account);
            _context.SaveChanges();

            // AutoMapper config
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Account, AccountDTO>();
                cfg.CreateMap<User, UserDTO>();
            });

            _mapper = config.CreateMapper();

            _repository = new LoginUserRepository(_context, _mapper);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetAccountLogin_ShouldReturnAccount_WhenValidCredentials()
        {
            var result = await _repository.GetAccountLogin("test@example.com", "123456");

            Assert.IsNotNull(result);
            Assert.AreEqual("test@example.com", result.Email);
        }

        [Test]
        public async Task GetAccountLogin_ShouldReturnNull_WhenInvalidCredentials()
        {
            var result = await _repository.GetAccountLogin("test@example.com", "wrong");

            Assert.IsNull(result);
        }

        [Test]
        public async Task GetRoleNameByEmail_ShouldReturnCorrectRole()
        {
            var role = await _repository.GetRoleNameByEmail("test@example.com");

            Assert.AreEqual("Admin", role);
        }

        [Test]
        public async Task GetRoleNameByEmail_ShouldReturnNull_WhenEmailNotFound()
        {
            var role = await _repository.GetRoleNameByEmail("notfound@example.com");

            Assert.IsNull(role);
        }

        [Test]
        public async Task GetUserByEmail_ShouldReturnCorrectUser()
        {
            var user = await _repository.GetUserByEmail("test@example.com");

            Assert.IsNotNull(user);
            Assert.AreEqual("John Doe", user.Name);
        }

        [Test]
        public async Task GetUserByEmail_ShouldReturnNull_WhenEmailNotFound()
        {
            var user = await _repository.GetUserByEmail("notfound@example.com");

            Assert.IsNull(user);
        }
    }
}
