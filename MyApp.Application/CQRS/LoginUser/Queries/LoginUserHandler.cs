using MediatR;
using MyApp.Application.Common.Message;
using MyApp.Application.Common.Sha256Hasher;
using MyApp.Application.Interfaces.ILoginUserRepository;

namespace MyApp.Application.CQRS.LoginUser.Queries
{
    public class LoginUserHandler : IRequestHandler<LoginUserRequest, LoginUserResponseDTO>
    {
        private readonly ILoginUserRepository _loginUserRepository;
        private readonly ITokenRepository _tokenRepository;

        public LoginUserHandler(
            ILoginUserRepository loginUserRepository,
            ITokenRepository tokenRepository
        )
        {
            _loginUserRepository = loginUserRepository;
            _tokenRepository = tokenRepository;
        }

        public async Task<LoginUserResponseDTO> Handle(
            LoginUserRequest request,
            CancellationToken cancellationToken
        )
        {
            var hashedPassword = Sha256Hasher.ComputeSha256Hash(request.Password);
            var account = await _loginUserRepository.GetAccountLogin(request.Email, hashedPassword);

            if (account == null)
                return new LoginUserResponseDTO { Token = null, Message = Message.LOGIN_WRONG };

            if (!account.IsActive)
                return new LoginUserResponseDTO { Token = null, Message = Message.ACCOUNT_LOCKED };

            var user = await _loginUserRepository.GetUserByEmail(account.Email);
            var role = await _loginUserRepository.GetRoleNameByEmail(account.Email);

            return new LoginUserResponseDTO
            {
                Token = _tokenRepository.CreateJWTToken(user, role),
                Message =
                    user.ValidDate > DateTime.UtcNow.Date
                        ? Message.LOGIN_SUCCESS
                        : Message.EXPIRED_CITIZEN_IDENTIFICATION,
                Id = user.Id,
                Email = account.Email,
                Name = user.Name,
                RoleName = role,
            };
        }
    }
}
