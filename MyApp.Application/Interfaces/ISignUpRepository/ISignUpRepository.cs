using System.Data;
using MyApp.Application.CQRS.SignUp.Command;

namespace MyApp.Application.Interfaces.ISignUpRepository
{
    public interface ISignUpRepository
    {
        public Task<bool> InsertUser(SignUpRequest signUpRequest);

        public Task<bool> InsertAccount(SignUpRequest signUpRequest);

        public Task<bool> CheckUserExits(SignUpRequest signUpRequest);

        public Task<bool> CheckEmailExists(string email);
        Task<IDbTransaction> BeginTransactionAsync(CancellationToken cancellationToken);
    }
}
