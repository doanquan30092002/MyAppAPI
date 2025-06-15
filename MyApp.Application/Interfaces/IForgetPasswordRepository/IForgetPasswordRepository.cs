using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyApp.Application.CQRS.ForgotPassword.Enums;
using MyApp.Core.Entities;

namespace MyApp.Application.Interfaces.IForgetPasswordRepository
{
    public interface IForgetPasswordRepository
    {
        Task<Account?> FindByContactAsync(string contact, OTPChannel type);
        Task<bool> UpdatePasswordAsync(string contact, OTPChannel channel, string newPassword);
    }
}
