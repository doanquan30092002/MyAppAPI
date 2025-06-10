using System.Data;
using MediatR;
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
                return new SignUpResponse { Message = "The email address is already registered." };
            }

            // Check if citizen identification number exists
            var isUserExists = await _signUpRepository.CheckUserExits(request);
            if (isUserExists)
            {
                return new SignUpResponse
                {
                    Message = "The citizen identification number has been registered.",
                };
            }

            // Begin transaction
            using var transaction =
                await _signUpRepository.BeginTransactionAsync(cancellationToken) as IDbTransaction;
            if (transaction == null)
            {
                throw new InvalidOperationException("Failed to create a database transaction.");
            }

            try
            {
                // Insert user
                var isUserInserted = await _signUpRepository.InsertUser(request);
                if (!isUserInserted)
                {
                    transaction.Rollback();
                    return new SignUpResponse { Message = "Failed to create user." };
                }

                // Insert account
                var isAccountInserted = await _signUpRepository.InsertAccount(request);
                if (!isAccountInserted)
                {
                    transaction.Rollback();
                    return new SignUpResponse
                    {
                        Message = "Failed to create account. User creation has been rolled back.",
                    };
                }

                // Commit transaction
                transaction.Commit();
                return new SignUpResponse { Message = "User and account created successfully." };
            }
            catch (Exception)
            {
                transaction.Rollback(); // Use synchronous Rollback for IDisposable
                return new SignUpResponse
                {
                    Message = "An error occurred during signup. All changes have been rolled back.",
                };
            }
        }
    }
}
