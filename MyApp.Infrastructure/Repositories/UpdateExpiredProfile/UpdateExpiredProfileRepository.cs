using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.CQRS.UpdateExpiredProfile.Command;
using MyApp.Application.Interfaces.UpdateExpiredProfile;
using MyApp.Core.Entities;
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

        public async Task<UserResponse> GetUserByIdAsync(string userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id.ToString() == userId);
            return new UserResponse
            {
                Id = user.Id,
                Name = user.Name,
                CitizenIdentification = user.CitizenIdentification,
                BirthDay = user.BirthDay,
                Nationality = user.Nationality,
                Gender = user.Gender,
                ValidDate = user.ValidDate,
                OriginLocation = user.OriginLocation,
                RecentLocation = user.RecentLocation,
                IssueDate = user.IssueDate,
                IssueBy = user.IssueBy,
                UpdatedAt = user.UpdatedAt,
                UpdatedBy = user.UpdatedBy,
            };
        }

        public async Task UpdateUserAsync(UserResponse userNew)
        {
            var userExist = await _context.Users.FirstOrDefaultAsync(x => x.Id == userNew.Id);
            userExist.Name = userNew.Name;
            userExist.BirthDay = userNew.BirthDay;
            userExist.Nationality = userNew.Nationality;
            userExist.Gender = userNew.Gender;
            userExist.ValidDate = userNew.ValidDate;
            userExist.OriginLocation = userNew.OriginLocation;
            userExist.RecentLocation = userNew.RecentLocation;
            userExist.IssueDate = userNew.IssueDate;
            userExist.IssueBy = userNew.IssueBy;
            userExist.UpdatedAt = userNew.UpdatedAt;
            userExist.UpdatedBy = userNew.UpdatedBy;

            _context.Users.Update(userExist);
            await _context.SaveChangesAsync();
        }
    }
}
