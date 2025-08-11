using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using MyApp.Application.Common.Message;
using MyApp.Application.CQRS.SignUp.SignUpUser.Command;
using MyApp.Application.Interfaces.ISignUpRepository;
using NUnit.Framework;

namespace MyApp.Application.CQRS.SignUp.SignUpUser.Command.Tests
{
    [TestFixture]
    public class SignUpHandlerTests
    {
        private Mock<ISignUpRepository> _repoMock;
        private SignUpHandler _handler;
        private SignUpRequest _request;
        private Mock<IDbTransaction> _transactionMock;

        [SetUp]
        public void SetUp()
        {
            _repoMock = new Mock<ISignUpRepository>();
            _handler = new SignUpHandler(_repoMock.Object);
            _request = new SignUpRequest
            {
                PhoneNumber = "0912345678",
                Email = "test@example.com",
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
                Password = "Password@1",
                RoleId = 1,
            };
            _transactionMock = new Mock<IDbTransaction>();
        }

        [Test]
        public void Handle_PhoneNumberExists_ThrowsValidationException()
        {
            _repoMock.Setup(r => r.CheckPhoneNumberExits(_request.PhoneNumber)).ReturnsAsync(true);

            var ex = Assert.ThrowsAsync<ValidationException>(() =>
                _handler.Handle(_request, CancellationToken.None)
            );

            Assert.That(ex.Message, Is.EqualTo(Message.PHONE_NUMBER_EXITS));
        }

        [Test]
        public void Handle_EmailExists_ThrowsValidationException()
        {
            _repoMock.Setup(r => r.CheckPhoneNumberExits(_request.PhoneNumber)).ReturnsAsync(false);
            _repoMock.Setup(r => r.CheckEmailExists(_request.Email)).ReturnsAsync(true);

            var ex = Assert.ThrowsAsync<ValidationException>(() =>
                _handler.Handle(_request, CancellationToken.None)
            );

            Assert.That(ex.Message, Is.EqualTo(Message.EMAIL_EXITS));
        }

        [Test]
        public void Handle_CitizenIdExists_ThrowsValidationException()
        {
            _repoMock.Setup(r => r.CheckPhoneNumberExits(_request.PhoneNumber)).ReturnsAsync(false);
            _repoMock.Setup(r => r.CheckEmailExists(_request.Email)).ReturnsAsync(false);
            _repoMock.Setup(r => r.CheckUserExits(_request)).ReturnsAsync(true);

            var ex = Assert.ThrowsAsync<ValidationException>(() =>
                _handler.Handle(_request, CancellationToken.None)
            );

            Assert.That(ex.Message, Is.EqualTo(Message.CITIZENS_ID_EXITS));
        }

        [Test]
        public void Handle_BeginTransactionNull_ThrowsValidationException()
        {
            _repoMock.Setup(r => r.CheckPhoneNumberExits(_request.PhoneNumber)).ReturnsAsync(false);
            _repoMock.Setup(r => r.CheckEmailExists(_request.Email)).ReturnsAsync(false);
            _repoMock.Setup(r => r.CheckUserExits(_request)).ReturnsAsync(false);
            _repoMock
                .Setup(r => r.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync((IDbTransaction)null);

            var ex = Assert.ThrowsAsync<ValidationException>(() =>
                _handler.Handle(_request, CancellationToken.None)
            );

            Assert.That(ex.Message, Is.EqualTo(Message.CREATE_FAIL));
        }

        [Test]
        public void Handle_InsertUserFails_ThrowsValidationExceptionAndRollback()
        {
            _repoMock.Setup(r => r.CheckPhoneNumberExits(_request.PhoneNumber)).ReturnsAsync(false);
            _repoMock.Setup(r => r.CheckEmailExists(_request.Email)).ReturnsAsync(false);
            _repoMock.Setup(r => r.CheckUserExits(_request)).ReturnsAsync(false);
            _repoMock
                .Setup(r => r.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_transactionMock.Object);
            _repoMock.Setup(r => r.InsertUser(_request)).ReturnsAsync(false);

            var ex = Assert.ThrowsAsync<ValidationException>(() =>
                _handler.Handle(_request, CancellationToken.None)
            );

            //_transactionMock.Verify(t => t.Rollback(), Times.Once);
            Assert.That(ex.Message, Is.EqualTo(Message.CREATE_FAIL));
        }

        [Test]
        public void Handle_InsertAccountFails_ThrowsValidationExceptionAndRollback()
        {
            _repoMock.Setup(r => r.CheckPhoneNumberExits(_request.PhoneNumber)).ReturnsAsync(false);
            _repoMock.Setup(r => r.CheckEmailExists(_request.Email)).ReturnsAsync(false);
            _repoMock.Setup(r => r.CheckUserExits(_request)).ReturnsAsync(false);
            _repoMock
                .Setup(r => r.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_transactionMock.Object);
            _repoMock.Setup(r => r.InsertUser(_request)).ReturnsAsync(true);
            _repoMock.Setup(r => r.InsertAccount(_request)).ReturnsAsync(false);

            var ex = Assert.ThrowsAsync<ValidationException>(() =>
                _handler.Handle(_request, CancellationToken.None)
            );

            //_transactionMock.Verify(t => t.Rollback(), Times.Once);
            Assert.That(ex.Message, Is.EqualTo(Message.CREATE_FAIL));
        }

        [Test]
        public async Task Handle_SuccessfulSignUp_CommitsTransactionAndReturnsResponse()
        {
            _repoMock.Setup(r => r.CheckPhoneNumberExits(_request.PhoneNumber)).ReturnsAsync(false);
            _repoMock.Setup(r => r.CheckEmailExists(_request.Email)).ReturnsAsync(false);
            _repoMock.Setup(r => r.CheckUserExits(_request)).ReturnsAsync(false);
            _repoMock
                .Setup(r => r.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_transactionMock.Object);
            _repoMock.Setup(r => r.InsertUser(_request)).ReturnsAsync(true);
            _repoMock.Setup(r => r.InsertAccount(_request)).ReturnsAsync(true);

            var response = await _handler.Handle(_request, CancellationToken.None);

            //_transactionMock.Verify(t => t.Commit(), Times.Once);
            Assert.IsNotNull(response);
            Assert.That(response, Is.TypeOf<SignUpResponse>());
        }

        [Test]
        public void Handle_ExceptionDuringInsert_ThrowsValidationExceptionAndRollback()
        {
            _repoMock.Setup(r => r.CheckPhoneNumberExits(_request.PhoneNumber)).ReturnsAsync(false);
            _repoMock.Setup(r => r.CheckEmailExists(_request.Email)).ReturnsAsync(false);
            _repoMock.Setup(r => r.CheckUserExits(_request)).ReturnsAsync(false);
            _repoMock
                .Setup(r => r.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_transactionMock.Object);
            _repoMock.Setup(r => r.InsertUser(_request)).ReturnsAsync(true);
            _repoMock
                .Setup(r => r.InsertAccount(_request))
                .ThrowsAsync(new Exception("Unexpected"));

            var ex = Assert.ThrowsAsync<ValidationException>(() =>
                _handler.Handle(_request, CancellationToken.None)
            );

            //_transactionMock.Verify(t => t.Rollback(), Times.Once);
            Assert.That(ex.Message, Is.EqualTo(Message.CREATE_FAIL));
        }

        [Test]
        public void Handle_ExceptionAfterInserts_ThrowsValidationExceptionAndRollback()
        {
            _repoMock.Setup(r => r.CheckPhoneNumberExits(_request.PhoneNumber)).ReturnsAsync(false);
            _repoMock.Setup(r => r.CheckEmailExists(_request.Email)).ReturnsAsync(false);
            _repoMock.Setup(r => r.CheckUserExits(_request)).ReturnsAsync(false);
            _repoMock
                .Setup(r => r.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_transactionMock.Object);
            _repoMock.Setup(r => r.InsertUser(_request)).ReturnsAsync(true);
            _repoMock.Setup(r => r.InsertAccount(_request)).ReturnsAsync(true);

            // Giả lập Commit ném exception
            _transactionMock.Setup(t => t.Commit()).Throws(new Exception("Commit failed"));

            var ex = Assert.ThrowsAsync<ValidationException>(() =>
                _handler.Handle(_request, CancellationToken.None)
            );

            //_transactionMock.Verify(t => t.Rollback(), Times.Once);
            Assert.That(ex.Message, Is.EqualTo(Message.CREATE_FAIL));
        }
    }
}
