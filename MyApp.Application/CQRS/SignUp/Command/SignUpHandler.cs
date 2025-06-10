using System.Data;
using MediatR;
using MyApp.Application.Common.Message;
using MyApp.Application.Interfaces.ISignUpRepository;

namespace MyApp.Application.CQRS.SignUp.Command
{
    public class SignUpHandler : IRequestHandler<SignUpRequest, SignUpResponse>
    {
        private readonly ISignUpRepository _signUpRepository;

        public SignUpHandler(ISignUpRepository signUpRepository)
        {
            _signUpRepository =
                signUpRepository ?? throw new ArgumentNullException(nameof(signUpRepository));
        }

        public async Task<SignUpResponse> Handle(
            SignUpRequest request,
            CancellationToken cancellationToken
        )
        {
            // Check if email already exists
            var isEmailExists = await _signUpRepository.CheckEmailExists(request.Email);
            if (isEmailExists)
            {
                return new SignUpResponse { Message = Message.EMAIL_EXITS };
            }

            // Check if citizen identification number exists
            var isUserExists = await _signUpRepository.CheckUserExits(request);
            if (isUserExists)
            {
                return new SignUpResponse { Message = Message.CITIZENS_ID_EXITS };
            }

            // Begin transaction
            using var transaction =
                await _signUpRepository.BeginTransactionAsync(cancellationToken) as IDbTransaction;
            if (transaction == null)
            {
                throw new InvalidOperationException(Message.CREATE_FAIL);
            }

            try
            {
                // Insert user
                var isUserInserted = await _signUpRepository.InsertUser(request);
                if (!isUserInserted)
                {
                    transaction.Rollback();
                    return new SignUpResponse { Message = Message.CREATE_FAIL };
                }

                // Insert account
                var isAccountInserted = await _signUpRepository.InsertAccount(request);
                if (!isAccountInserted)
                {
                    transaction.Rollback();
                    return new SignUpResponse { Message = Message.CREATE_FAIL };
                }

                // Commit transaction
                transaction.Commit();
                return new SignUpResponse { Message = Message.CREATE_SUCCESS };
            }
            catch (Exception)
            {
                transaction.Rollback();
                return new SignUpResponse { Message = Message.CREATE_FAIL };
            }
        }
    }
}
