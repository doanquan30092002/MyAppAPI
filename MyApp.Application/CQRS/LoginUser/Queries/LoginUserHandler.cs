using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
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
            var account = await loginUserRepository.GetAccountLogin(request);
            if (account != null)
            {
                var role = await loginUserRepository.GetRoleNameByPhoneNumber(account.PhoneNumber);
                var user = await loginUserRepository.GetUserByPhoneNumber(account.PhoneNumber);
                var result = new LoginUserResponse
                {
                    Token = tokenRepository.CreateJWTToken(user, role),
                };

                return result;
            }
            return null;
        }
    }
}
