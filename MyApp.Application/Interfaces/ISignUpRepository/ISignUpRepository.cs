using System.Data;
using MyApp.Application.CQRS.SignUp.SignUpUser.Command;

namespace MyApp.Application.Interfaces.ISignUpRepository
{
    public interface ISignUpRepository
    {
        public Task<bool> InsertUser(SignUpRequest signUpRequest);

        public Task<bool> InsertAccount(SignUpRequest signUpRequest);

        public Task<bool> CheckUserExits(SignUpRequest signUpRequest);

        public Task<bool> CheckEmailExists(string email);
        Task<IDbTransaction> BeginTransactionAsync(CancellationToken cancellationToken);
        Task<bool> CheckPhoneNumberExits(string phoneNumber);
    }
}
