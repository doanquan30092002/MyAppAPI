using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.Common.Sha256Hasher;
using MyApp.Application.CQRS.ForgotPassword.Enums;
using MyApp.Application.Interfaces.IForgetPasswordRepository;
using MyApp.Core.Entities;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories.ForgetPassRepository
{
    public class ForgetPassRepository : IForgetPasswordRepository
    {
        private readonly AppDbContext _dbContext;

        public ForgetPassRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Account?> FindByContactAsync(string contact, OTPChannel type)
        {
            if (string.IsNullOrWhiteSpace(contact))
                return null;

            if (type == OTPChannel.Email)
            {
                return await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Email == contact);
            }
            else if (type == OTPChannel.SMS)
            {
                return await _dbContext.Accounts.FirstOrDefaultAsync(a => a.PhoneNumber == contact);
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> UpdatePasswordAsync(
            string contact,
            OTPChannel channel,
            string newPassword
        )
        {
            if (string.IsNullOrWhiteSpace(contact) || string.IsNullOrWhiteSpace(newPassword))
                return false;

            Account? account = null;

            if (channel == OTPChannel.Email)
            {
                account = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Email == contact);
            }
            else if (channel == OTPChannel.SMS)
            {
                account = await _dbContext.Accounts.FirstOrDefaultAsync(a =>
                    a.PhoneNumber == contact
                );
            }

            if (account == null)
                return false;

            account.Password = Sha256Hasher.ComputeSha256Hash(newPassword);

            _dbContext.Accounts.Update(account);
            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}
