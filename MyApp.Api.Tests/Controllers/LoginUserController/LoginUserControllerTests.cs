using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyApp.Application.Common.Message;
using MyApp.Application.Common.Response;
using MyApp.Application.CQRS.LoginUser.Queries;

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
            _controller = new LoginUserController(_mediatorMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext(),
                },
            };
        }

        [Test]
        public async Task Login_ReturnsTokenAndLoginSuccess()
        {
            // Arrange
            var loginRequest = new LoginUserRequest
            {
                Email = "test@example.com",
                Password = "password",
            };
            var responseDto = new LoginUserResponseDTO
            {
                Token = "valid_token",
                Message = Message.LOGIN_SUCCESS,
                Id = Guid.NewGuid(),
                Email = loginRequest.Email,
                Name = "Test User",
                RoleName = "Admin",
            };

            _mediatorMock
                .Setup(m => m.Send(loginRequest, It.IsAny<CancellationToken>()))
                .ReturnsAsync(responseDto);

            // Act
            var result = await _controller.Login(loginRequest);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var apiResponse = okResult.Value as ApiResponse<LoginUserResponse>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual(200, apiResponse.Code);
            Assert.AreEqual(Message.LOGIN_SUCCESS, apiResponse.Message);
            Assert.AreEqual("Test User", apiResponse.Data.Name);
            Assert.AreEqual("Admin", apiResponse.Data.RoleName);
            Assert.IsFalse(apiResponse.Data.IsExpired);
        }

        [Test]
        public async Task Login_ExpiredCitizenIdentification_ReturnsTokenWithWarning()
        {
            // Arrange
            var loginRequest = new LoginUserRequest
            {
                Email = "expired@example.com",
                Password = "password",
            };
            var responseDto = new LoginUserResponseDTO
            {
                Token = "expired_token",
                Message = Message.EXPIRED_CITIZEN_IDENTIFICATION,
                Id = Guid.NewGuid(),
                Email = loginRequest.Email,
                Name = "Expired User",
                RoleName = "User",
            };

            _mediatorMock
                .Setup(m => m.Send(loginRequest, It.IsAny<CancellationToken>()))
                .ReturnsAsync(responseDto);

            // Act
            var result = await _controller.Login(loginRequest);

            // Assert
            var okResult = result.Result as OkObjectResult;
            var apiResponse = okResult.Value as ApiResponse<LoginUserResponse>;
            Assert.AreEqual(200, apiResponse.Code);
            Assert.AreEqual(Message.EXPIRED_CITIZEN_IDENTIFICATION, apiResponse.Message);
            Assert.IsTrue(apiResponse.Data.IsExpired);
        }

        [Test]
        public async Task Login_Failure_Returns400()
        {
            // Arrange
            var loginRequest = new LoginUserRequest
            {
                Email = "wrong@example.com",
                Password = "wrong",
            };
            var responseDto = new LoginUserResponseDTO
            {
                Token = null,
                Message = Message.LOGIN_WRONG,
            };

            _mediatorMock
                .Setup(m => m.Send(loginRequest, It.IsAny<CancellationToken>()))
                .ReturnsAsync(responseDto);

            // Act
            var result = await _controller.Login(loginRequest);

            // Assert
            var okResult = result.Result as OkObjectResult;
            var apiResponse = okResult.Value as ApiResponse<LoginUserResponse>;
            Assert.AreEqual(400, apiResponse.Code);
            Assert.AreEqual(Message.LOGIN_WRONG, apiResponse.Message);
            Assert.IsNull(apiResponse.Data.Email);
        }
    }
}
