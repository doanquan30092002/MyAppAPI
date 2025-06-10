using MediatR;
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
            // check user exist
            var account = await loginUserRepository.GetAccountLogin(
                request.Email,
                Sha256Hasher.ComputeSha256Hash(request.Password)
            );
            if (account != null)
            {
                var role = await loginUserRepository.GetRoleNameByEmail(account.Email);
                var user = await loginUserRepository.GetUserByEmail(account.Email);
                var result = new LoginUserResponse
                {
                    Token = tokenRepository.CreateJWTToken(user, role),
                    Message = "Đăng nhập thành công",
                };

                return result;
            }
            return new LoginUserResponse
            {
                Token = null,
                Message = "Đăng nhập sai tài khoản hoặc mật khẩu",
            };
        }
    }
}
