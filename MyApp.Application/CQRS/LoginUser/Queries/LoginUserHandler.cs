using MediatR;
using MyApp.Application.Common.Message;
using MyApp.Application.Common.Sha256Hasher;
using MyApp.Application.Interfaces.ILoginUserRepository;

namespace MyApp.Application.CQRS.LoginUser.Queries
{
    public class LoginUserHandler : IRequestHandler<LoginUserRequest, LoginUserResponseDTO>
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

        public async Task<LoginUserResponseDTO> Handle(
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
                //check account is active
                if (account.IsActive)
                {
                    var user = await loginUserRepository.GetUserByEmail(account.Email);

                    var role = await loginUserRepository.GetRoleNameByEmail(account.Email);

                    var result = new LoginUserResponseDTO
                    {
                        Token = tokenRepository.CreateJWTToken(user, role),
                        // check valid date
                        Message =
                            user.ValidDate > DateTime.UtcNow.Date
                                ? Message.LOGIN_SUCCESS
                                : Message.EXPIRED_CITIZEN_IDENTIFICATION,
                        Id = user.Id,
                        Email = account.Email,
                        Name = user.Name,
                        RoleName = role,
                    };

                    return result;
                }
                else
                {
                    return new LoginUserResponseDTO
                    {
                        Token = null,
                        Message = Message.ACCOUNT_LOCKED,
                    };
                }
            }
            return new LoginUserResponseDTO { Token = null, Message = Message.LOGIN_WRONG };
        }
    }
}
