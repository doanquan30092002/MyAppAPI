using MyApp.Application.CQRS.UpdateExpiredProfile.Command;
using MyApp.Core.Entities;

namespace MyApp.Application.Interfaces.UpdateExpiredProfile
{
    public interface IUpdateExpiredProfileRepository
    {
        Task<UserResponse> GetUserByIdAsync(string userId);
        Task UpdateUserAsync(UserResponse user);
    }
}
