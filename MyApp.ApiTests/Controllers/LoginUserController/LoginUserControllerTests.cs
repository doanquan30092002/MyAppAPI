using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.CQRS.LoginUser.Queries;
using NUnit.Framework;

namespace MyApp.Api.Controllers.LoginUserController.Tests
{
    [TestFixture()]
    public class LoginUserControllerTests
    {
        private Mock<IMediator> _mediatorMock;
        private LoginUserController _controller;

        [SetUp]
        public void SetUp()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new LoginUserController(_mediatorMock.Object);

            var httpContext = new DefaultHttpContext();
            _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        }

        [Test]
        public async Task Login_Returns200AndSetCookie_WhenTokenIsValid()
        {
            var request = new LoginUserRequest
            {
                Email = "test@example.com",
                Password = "password123",
            };

            var expectedResponse = new LoginUserResponseDTO
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                Name = "Test User",
                RoleName = "User",
                Message = "Login successful",
                Token = "token123",
            };

            _mediatorMock
                .Setup(x => x.Send(It.IsAny<LoginUserRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            var result = await _controller.Login(request) as OkObjectResult;

            Assert.IsNotNull(result);
            dynamic data = result.Value;
            Assert.AreEqual(200, (int)data.Code);
            Assert.IsFalse((bool)data.Data.IsExpired);
            Assert.IsTrue(
                _controller
                    .Response.Headers["Set-Cookie"]
                    .ToString()
                    .Contains("access_token=token123")
            );
        }

        [Test]
        public async Task Login_Returns400_WhenTokenMissingAndExpiredMessage()
        {
            var request = new LoginUserRequest { Email = "expired@example.com", Password = "pass" };

            var expiredResponse = new LoginUserResponseDTO
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                Name = "Expired",
                RoleName = "User",
                Token = "",
                Message = Message.EXPIRED_CITIZEN_IDENTIFICATION,
            };

            _mediatorMock
                .Setup(x => x.Send(It.IsAny<LoginUserRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expiredResponse);

            var result = await _controller.Login(request) as OkObjectResult;

            Assert.IsNotNull(result);
            dynamic data = result.Value;
            Assert.AreEqual(400, (int)data.Code);
            Assert.IsTrue((bool)data.Data.IsExpired);
        }

        [Test]
        public async Task Login_Returns400_WhenTokenMissingButMessageNotExpired()
        {
            var request = new LoginUserRequest
            {
                Email = "wrong@example.com",
                Password = "wrongpass",
            };

            var response = new LoginUserResponseDTO
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                Name = "Wrong",
                RoleName = "User",
                Token = null,
                Message = "Invalid credentials",
            };

            _mediatorMock
                .Setup(x => x.Send(It.IsAny<LoginUserRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var result = await _controller.Login(request) as OkObjectResult;

            Assert.IsNotNull(result);
            dynamic data = result.Value;
            Assert.AreEqual(400, (int)data.Code);
            Assert.IsFalse((bool)data.Data.IsExpired);
        }
    }
}
