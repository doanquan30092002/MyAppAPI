using MyApp.Application.CQRS.UpdateExpiredProfile.Command;
using MyApp.Core.Entities;

namespace MyApp.Application.Interfaces.UpdateExpiredProfile
{
    public interface IUpdateExpiredProfileRepository
    {
        Task<UpdateExpiredProfileResponse> UpdateExpiredProfileAsync(
            string userId,
            UpdateExpiredProfileRequest updateExpiredProfileRequest
        );
    }
}
