using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.Common.Message;
using MyApp.Application.Common.Sha256Hasher;
using MyApp.Application.CQRS.UpdateAccountAndProfile.Command;
using MyApp.Application.Interfaces.UpdateAccountRepository;
using MyApp.Core.Entities;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories.UpdateAccountRepository
{
    public class UpdateAccountRepository : IUpdateAccountRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public UpdateAccountRepository(AppDbContext context, IMapper mapper)
        {
            this._context = context;
            _mapper = mapper;
        }

        public async Task<UpdateAccountResponse> UpdateAccountInfo(
            string? userId,
            string? emailNew,
            string? passwordOld,
            string? passwordNew,
            string? phoneNumberNew
        )
        {
            if (
                string.IsNullOrEmpty(emailNew)
                && string.IsNullOrEmpty(passwordOld)
                && string.IsNullOrEmpty(passwordNew)
                && string.IsNullOrEmpty(phoneNumberNew)
            )
            {
                return new UpdateAccountResponse
                {
                    Code = 400,
                    Message = Message.NO_FIELDS_PROVIDED,
                };
            }
            var account = await _context.Accounts.FirstOrDefaultAsync(x =>
                x.User.Id.ToString() == userId
            );

            //Trả về true nếu email mới hợp lệ (không trùng người khác).
            //Trả về false nếu email mới đã bị người khác dùng.
            //Tự động bỏ qua kiểm tra nếu không có email được nhập.
            if (!await IsEmailValidAsync(account.AccountId, emailNew))
            {
                return new UpdateAccountResponse { Code = 400, Message = Message.EMAIL_EXITS };
            }

            if (!await IsPhoneValidAsync(account.AccountId, phoneNumberNew))
            {
                return new UpdateAccountResponse
                {
                    Code = 400,
                    Message = Message.PHONE_NUMBER_EXITS,
                };
            }

            var passwordValidationResult = ValidateAndUpdatePassword(
                account,
                passwordOld,
                passwordNew
            );
            if (passwordValidationResult != null)
            {
                return passwordValidationResult;
            }

            UpdateOptionalFields(account, emailNew, phoneNumberNew);

            try
            {
                _context.Accounts.Update(account);
                await _context.SaveChangesAsync();

                return new UpdateAccountResponse
                {
                    Code = 200,
                    Message = Message.UPDATE_ACCOUNT_SUCCESS,
                };
            }
            catch (Exception)
            {
                return new UpdateAccountResponse { Code = 500, Message = Message.SYSTEM_ERROR };
            }
        }

        private async Task<bool> IsEmailValidAsync(Guid accountId, string? email)
        {
            if (string.IsNullOrEmpty(email))
                return true;
            return !await _context.Accounts.AnyAsync(x => x.Email == email);
        }

        private async Task<bool> IsPhoneValidAsync(Guid accountId, string? phone)
        {
            if (string.IsNullOrEmpty(phone))
                return true;
            return !await _context.Accounts.AnyAsync(x => x.PhoneNumber == phone);
        }

        private UpdateAccountResponse? ValidateAndUpdatePassword(
            Account account,
            string? oldPassword,
            string? newPassword
        )
        {
            bool isChangingPassword =
                !string.IsNullOrEmpty(oldPassword) || !string.IsNullOrEmpty(newPassword);

            if (!isChangingPassword)
                return null;

            if (string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword))
            {
                return new UpdateAccountResponse
                {
                    Code = 400,
                    Message = Message.PASSWORD_OLD_OR_NEW_EMPTY,
                };
            }

            string hashedOld = Sha256Hasher.ComputeSha256Hash(oldPassword);
            if (hashedOld != account.Password)
            {
                return new UpdateAccountResponse
                {
                    Code = 400,
                    Message = Message.PASSWORD_OLD_NOT_EQUAL,
                };
            }

            account.Password = Sha256Hasher.ComputeSha256Hash(newPassword);
            return null;
        }

        private void UpdateOptionalFields(Account account, string? email, string? phone)
        {
            if (!string.IsNullOrEmpty(email))
            {
                account.Email = email;
            }

            if (!string.IsNullOrEmpty(phone))
            {
                account.PhoneNumber = phone;
            }
        }
    }
}
