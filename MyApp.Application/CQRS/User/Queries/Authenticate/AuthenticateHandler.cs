using MediatR;
using MyApp.Application.Interfaces;

namespace MyApp.Application.CQRS.User.Queries.Authenticate
{
    public class AuthenticateHandler : IRequestHandler<LoginRequest, LoginResponse>
    {
        private readonly IUserRepository userRepository;
        private readonly ITokenRepository tokenRepository;

        public AuthenticateHandler(IUserRepository userRepository, ITokenRepository tokenRepository)
        {
            this.userRepository = userRepository;
            this.tokenRepository = tokenRepository;
        }

        public async Task<LoginResponse> Handle(
            LoginRequest request,
            CancellationToken cancellationToken
        )
        {
            // check user exist
            var user = await userRepository.CheckUserLogin(request);
            if (user != null)
            {
                var roles = await userRepository.GetRoleNamesByUsername(user.Username);
                var result = new LoginResponse
                {
                    Token = tokenRepository.CreateJWTToken(user, roles.ToList()),
                };

                return result;
            }
            return null;
        }
    }
}
