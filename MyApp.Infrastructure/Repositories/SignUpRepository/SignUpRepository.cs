using System.Data;
using Microsoft.EntityFrameworkCore.Storage;
using MyApp.Application.Common.Sha256Hasher;
using MyApp.Application.CQRS.SignUp.SignUpUser.Command;
using MyApp.Application.Interfaces.ISignUpRepository;
using MyApp.Core.Entities;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories.SignUpRepository
{
    public class SignUpRepository : ISignUpRepository
    {
        private readonly AppDbContext context;

        public SignUpRepository(AppDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Task<bool> CheckUserExits(SignUpRequest signUpRequest)
        {
            var userExists = context.Users.Any(u =>
                u.CitizenIdentification == signUpRequest.CitizenIdentification
            );

            if (userExists)
            {
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
        }

        Guid userId = Guid.NewGuid();

        //Inssert Account
        public async Task<bool> InsertAccount(SignUpRequest signUpRequest)
        {
            try
            {
                var account = new Account
                {
                    AccountId = Guid.NewGuid(),
                    PhoneNumber = signUpRequest.PhoneNumber,
                    Email = signUpRequest.Email,
                    Password = Sha256Hasher.ComputeSha256Hash(signUpRequest.Password),
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    CreatedBy = userId,
                    UpdatedBy = userId,
                    IsActive = true,
                    UserId = userId,
                    RoleId = signUpRequest.RoleId,
                };

                context.Accounts.Add(account);
                await context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> InsertUser(SignUpRequest signUpRequest)
        {
            try
            {
                var user = new User
                {
                    Id = userId,
                    CitizenIdentification = signUpRequest.CitizenIdentification,
                    Name = signUpRequest.Name,
                    BirthDay = signUpRequest.BirthDay,
                    Nationality = signUpRequest.Nationality,
                    Gender = signUpRequest.Gender,
                    ValidDate = signUpRequest.ValidDate,
                    OriginLocation = signUpRequest.OriginLocation,
                    RecentLocation = signUpRequest.RecentLocation,
                    IssueDate = signUpRequest.IssueDate,
                    IssueBy = signUpRequest.IssueBy,
                    CreatedAt = DateTime.Now,
                    CreatedBy = userId,
                    UpdatedAt = DateTime.Now,
                    UpdatedBy = userId,
                };

                context.Users.Add(user);
                await context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public Task<bool> CheckEmailExists(string email)
        {
            var emailExists = context.Accounts.Any(u => u.Email == email);
            if (emailExists)
            {
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
        }

        public Task<bool> CheckPhoneNumberExits(string phoneNumber)
        {
            var phoneNumberExists = context.Accounts.Any(u => u.PhoneNumber == phoneNumber);
            if (phoneNumberExists)
            {
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
        }

        public async Task<IDbTransaction> BeginTransactionAsync(CancellationToken cancellationToken)
        {
            var dbContextTransaction = await context.Database.BeginTransactionAsync(
                cancellationToken
            );
            return dbContextTransaction.GetDbTransaction();
        }
    }
}
