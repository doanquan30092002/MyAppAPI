using System.ComponentModel.DataAnnotations;
using System.Data;
using MediatR;
using MyApp.Application.Common.Message;
using MyApp.Application.Interfaces.ISignUpRepository;

namespace MyApp.Application.CQRS.SignUp.SignUpUser.Command
{
    public class SignUpHandler : IRequestHandler<SignUpRequest, SignUpResponse>
    {
        private readonly ISignUpRepository _signUpRepository;

        public SignUpHandler(ISignUpRepository signUpRepository)
        {
            _signUpRepository = signUpRepository;
        }

        public async Task<SignUpResponse> Handle(
            SignUpRequest request,
            CancellationToken cancellationToken
        )
        {
            // Check if phone number already exists
            var isPhoneNumberExists = await _signUpRepository.CheckPhoneNumberExits(
                request.PhoneNumber
            );
            if (isPhoneNumberExists)
            {
                throw new ValidationException(Message.PHONE_NUMBER_EXITS);
            }

            // Check if email already exists
            var isEmailExists = await _signUpRepository.CheckEmailExists(request.Email);
            if (isEmailExists)
            {
                throw new ValidationException(Message.EMAIL_EXITS);
            }

            // Check if citizen identification number exists
            var isUserExists = await _signUpRepository.CheckUserExits(request);
            if (isUserExists)
            {
                throw new ValidationException(Message.CITIZENS_ID_EXITS);
            }

            // Begin transaction
            using var transaction =
                await _signUpRepository.BeginTransactionAsync(cancellationToken) as IDbTransaction;
            if (transaction == null)
            {
                throw new ValidationException(Message.CREATE_FAIL);
            }

            try
            {
                // Insert user
                var isUserInserted = await _signUpRepository.InsertUser(request);
                if (!isUserInserted)
                {
                    transaction.Rollback();
                    throw new ValidationException(Message.CREATE_FAIL);
                }

                // Insert account
                var isAccountInserted = await _signUpRepository.InsertAccount(request);
                if (!isAccountInserted)
                {
                    transaction.Rollback();
                    throw new ValidationException(Message.CREATE_FAIL);
                }

                // Commit transaction
                transaction.Commit();
                return new SignUpResponse { };
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw new ValidationException(Message.CREATE_FAIL);
            }
        }
    }
}
