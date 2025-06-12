using MediatR;
using MyApp.Application.Common.Message;
using MyApp.Application.Common.Sha256Hasher;
using MyApp.Application.Interfaces.ILoginUserRepository;

namespace MyApp.Application.CQRS.LoginUser.Queries
{
    public class LoginUserHandler : IRequestHandler<LoginUserRequest, LoginUserResponse>
    {
        private readonly ILoginUserRepository loginUserRepository;
        private readonly ITokenRepository tokenRepository;

        public LoginUserHandler(
            ILoginUserRepository userRepository,
            ITokenRepository tokenRepository
        )
        {
            this.loginUserRepository = userRepository;
            this.tokenRepository = tokenRepository;
        }

        public async Task<LoginUserResponse> Handle(
            LoginUserRequest request,
            CancellationToken cancellationToken
        )
        {
            // check account exist
            var account = await loginUserRepository.GetAccountLogin(
                request.Email,
                Sha256Hasher.ComputeSha256Hash(request.Password)
            );
            if (account != null)
            {
                if (account.IsActive)
                {
                    var role = await loginUserRepository.GetRoleNameByEmail(account.Email);
                    var user = await loginUserRepository.GetUserByEmail(account.Email);
                    var result = new LoginUserResponse
                    {
                        Token = tokenRepository.CreateJWTToken(user, role),
                        Message = Message.LOGIN_SUCCESS,
                    };

                    return result;
                }
                else
                {
                    return new LoginUserResponse { Token = null, Message = Message.ACCOUNT_LOCKED };
                }
            }
            return new LoginUserResponse { Token = null, Message = Message.LOGIN_WRONG };
        }
    }
}
