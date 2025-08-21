using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using MyApp.Core.DTOs.LoginUserDTO;

namespace MyApp.Infrastructure.Repositories.LoginUserRepository.Tests
{
    [TestFixture()]
    public class TokenRepositoryTests
    {
        private TokenRepository _tokenRepository;

        [SetUp]
        public void Setup()
        {
            var inMemorySettings = new Dictionary<string, string>
            {
                { "Jwt:Key", "ThisIsASecretKeyForTestingOnly123456" },
                { "Jwt:Issuer", "TestIssuer" },
                { "Jwt:Audience", "TestAudience" },
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _tokenRepository = new TokenRepository(configuration);
        }

        [Test]
        public void CreateJWTToken_ShouldReturnValidToken()
        {
            // Arrange
            var user = new UserDTO { Id = Guid.NewGuid(), Name = "Test User" };
            var role = "Admin";

            // Act
            var token = _tokenRepository.CreateJWTToken(user, role);

            // Assert
            Assert.IsNotNull(token, "Token should not be null");

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            Assert.AreEqual("TestIssuer", jwtToken.Issuer);
            Assert.AreEqual("TestAudience", jwtToken.Audiences.First());
            Assert.AreEqual(
                user.Id.ToString(),
                jwtToken.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value
            );
            Assert.AreEqual(user.Name, jwtToken.Claims.First(c => c.Type == ClaimTypes.Name).Value);
            Assert.AreEqual(role, jwtToken.Claims.First(c => c.Type == ClaimTypes.Role).Value);
        }
    }
}
