using MyApp.Application.CQRS.User.Queries.Authenticate;
using MyApp.Core.Entities;

namespace MyApp.Application.Interfaces
{
    public interface ITokenRepository
    {
        string CreateJWTToken(User user, List<string> roles);
    }
}
