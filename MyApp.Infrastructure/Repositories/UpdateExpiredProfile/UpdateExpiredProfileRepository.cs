using MyApp.Application.Common.Message;
using MyApp.Application.CQRS.UpdateExpiredProfile.Command;
using MyApp.Application.Interfaces.UpdateExpiredProfile;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories.UpdateExpiredProfile
{
    public class UpdateExpiredProfileRepository : IUpdateExpiredProfileRepository
    {
        private readonly AppDbContext _context;

        public UpdateExpiredProfileRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UpdateExpiredProfileResponse> UpdateExpiredProfileAsync(
            string userId,
            UpdateExpiredProfileRequest updateExpiredProfileRequest
        )
        {
            var user = _context.Users.FirstOrDefault(x => x.Id.ToString() == userId);
            bool isMatchCitizenIdentification = CheckIsMatchCitizenIdentification(
                updateExpiredProfileRequest.CitizenIdentification,
                user.CitizenIdentification
            );

            if (isMatchCitizenIdentification)
            {
                try
                {
                    user.Name = updateExpiredProfileRequest.Name;
                    user.BirthDay = updateExpiredProfileRequest.BirthDay;
                    user.Nationality = updateExpiredProfileRequest.Nationality;
                    user.Gender = updateExpiredProfileRequest.Gender;
                    user.ValidDate = updateExpiredProfileRequest.ValidDate;
                    user.OriginLocation = updateExpiredProfileRequest.OriginLocation;
                    user.RecentLocation = updateExpiredProfileRequest.RecentLocation;
                    user.IssueDate = updateExpiredProfileRequest.IssueDate;
                    user.IssueBy = updateExpiredProfileRequest.IssueBy;
                    user.UpdatedAt = DateTime.UtcNow;
                    user.UpdatedBy = Guid.Parse(userId);
                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();
                    return new UpdateExpiredProfileResponse
                    {
                        Code = 200,
                        Message = Message.UPDATE_PROFILE_SUCCESS,
                    };
                }
                catch (Exception)
                {
                    return new UpdateExpiredProfileResponse
                    {
                        Code = 500,
                        Message = Message.SYSTEM_ERROR,
                    };
                }
            }
            else
            {
                return new UpdateExpiredProfileResponse
                {
                    Code = 400,
                    Message = Message.CITIZEN_IDENTIFICATION_NOT_MATCH,
                };
            }
        }

        private bool CheckIsMatchCitizenIdentification(
            string citizenIdentificationRequest,
            string citizenIdentificationExsit
        )
        {
            return citizenIdentificationRequest == citizenIdentificationExsit == true;
        }
    }
}
