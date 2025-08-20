using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using MyApp.Application.CQRS.AuctionDocuments.SupportRegisterDocuments.Queries;
using MyApp.Application.Interfaces.ISupportRegisterDocuments;
using MyApp.Core.DTOs.UserDTO;
using NUnit.Framework;

namespace MyApp.Application.CQRS.AuctionDocuments.SupportRegisterDocuments.Queries.Tests
{
    [TestFixture]
    public class GetUserByCitizenIdentificationHandlerTests
    {
        private Mock<ISupportRegisterDocuments> _mockSupportRegisterDocuments;
        private GetUserByCitizenIdentificationHandler _handler;

        [SetUp]
        public void Setup()
        {
            _mockSupportRegisterDocuments = new Mock<ISupportRegisterDocuments>();
            _handler = new GetUserByCitizenIdentificationHandler(
                _mockSupportRegisterDocuments.Object
            );
        }

        [Test]
        public async Task Handle_CitizenIdentificationIsNullOrEmpty_ReturnsNull()
        {
            // Arrange
            var requestEmpty = new GetUserByCitizenIdentificationRequest("");
            var requestNull = new GetUserByCitizenIdentificationRequest(null!);

            // Act
            var resultEmpty = await _handler.Handle(requestEmpty, CancellationToken.None);
            var resultNull = await _handler.Handle(requestNull, CancellationToken.None);

            // Assert
            Assert.IsNull(resultEmpty, "CitizenIdentification rỗng nên trả về null");
            Assert.IsNull(resultNull, "CitizenIdentification null nên trả về null");
        }

        [Test]
        public async Task Handle_ValidCitizenIdentification_ReturnsUser()
        {
            // Arrange
            var citizenId = "123456789";
            var request = new GetUserByCitizenIdentificationRequest(citizenId);

            var expectedUser = new GetUserByCitizenIdentificationResponse
            {
                Id = Guid.NewGuid(),
                CitizenIdentification = citizenId,
                Name = "Nguyen Van A",
                PhoneNumber = "0123456789",
            };

            _mockSupportRegisterDocuments
                .Setup(s => s.GetUserByCitizenIdentificationAsync(citizenId))
                .ReturnsAsync(expectedUser);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result, "Trả về user hợp lệ");
            Assert.AreEqual(expectedUser.Id, result!.Id);
            Assert.AreEqual(expectedUser.CitizenIdentification, result.CitizenIdentification);
            Assert.AreEqual(expectedUser.Name, result.Name);
            Assert.AreEqual(expectedUser.PhoneNumber, result.PhoneNumber);
        }
    }
}
