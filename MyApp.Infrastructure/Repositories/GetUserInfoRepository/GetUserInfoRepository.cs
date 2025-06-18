using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.CQRS.GetUserInfo.Queries;
using MyApp.Application.Interfaces.IGetUserInfoRepository;
using MyApp.Application.Interfaces.ISignUpRepository;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories.GetUserInfoRepository
{
    public class GetUserInfoRepository : IGetUserInfoRepository
    {
        private readonly AppDbContext context;

        public GetUserInfoRepository(AppDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<GetUserInfoResponse> GetUserInfoAsync(
            Guid userId,
            CancellationToken cancellationToken
        )
        {
            var account = await context
                .Accounts.Include(a => a.User)
                .Include(a => a.Role)
                .FirstOrDefaultAsync(a => a.UserId == userId, cancellationToken);

            if (account == null || account.User == null)
            {
                return null;
            }

            return new GetUserInfoResponse
            {
                CitizenIdentification = account.User.CitizenIdentification,
                Name = account.User.Name,
                BirthDay = account.User.BirthDay,
                Nationality = account.User.Nationality,
                Gender = account.User.Gender,
                ValidDate = account.User.ValidDate,
                OriginLocation = account.User.OriginLocation,
                RecentLocation = account.User.RecentLocation,
                IssueDate = account.User.IssueDate,
                IssueBy = account.User.IssueBy,
                PhoneNumber = account.PhoneNumber,
                Email = account.Email,
                RoleName = account.Role?.RoleName,
            };
        }
    }
}
