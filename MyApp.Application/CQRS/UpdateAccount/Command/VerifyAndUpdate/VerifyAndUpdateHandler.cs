using MediatR;
using Microsoft.Extensions.Caching.Memory;
using MyApp.Application.Common.CurrentUserService;
using MyApp.Application.Common.Message;
using MyApp.Application.Common.Sha256Hasher;
using MyApp.Application.CQRS.UpdateAccount.Command.SendUpdateOtp;
using MyApp.Application.Interfaces.UpdateAccount.Repository;
using MyApp.Application.Interfaces.UpdateAccount.Service;

namespace MyApp.Application.CQRS.UpdateAccount.Command.VerifyAndUpdate
{
    public class VerifyAndUpdateHandler
        : IRequestHandler<VerifyAndUpdateRequest, UpdateAccountResponse>
    {
        private readonly IUpdateAccountRepository _updateAccountRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IOTPService_1 _otpService;
        private readonly IMemoryCache _cache;

        public VerifyAndUpdateHandler(
            IUpdateAccountRepository updateAccountRepository,
            ICurrentUserService currentUserService,
            IOTPService_1 otpService,
            IMemoryCache cache
        )
        {
            _updateAccountRepository = updateAccountRepository;
            _currentUserService = currentUserService;
            _otpService = otpService;
            _cache = cache;
        }

        public async Task<UpdateAccountResponse> Handle(
            VerifyAndUpdateRequest request,
            CancellationToken cancellationToken
        )
        {
            var userId = _currentUserService.GetUserId();

            if (string.IsNullOrEmpty(userId))
            {
                return new UpdateAccountResponse { Code = 401, Message = "Unauthorized" };
            }

            var email = await _updateAccountRepository.GetEmailByUserIdAsync(userId);

            var isVerify = await _otpService.VerifyOtpAsync(email, request.OtpCode);
            if (!isVerify.Success)
            {
                return new UpdateAccountResponse { Code = 400, Message = isVerify.Message };
            }

            if (
                !_cache.TryGetValue(
                    $"update_pending_{email}",
                    out UpdateAccountRequest updateRequest
                )
            )
            {
                return new UpdateAccountResponse
                {
                    Code = 400,
                    Message = Message.NO_FIELDS_PROVIDED,
                };
            }

            // Bắt đầu xử lý logic
            if (
                string.IsNullOrEmpty(updateRequest.Email)
                && string.IsNullOrEmpty(updateRequest.PasswordOld)
                && string.IsNullOrEmpty(updateRequest.PasswordNew)
                && string.IsNullOrEmpty(updateRequest.PhoneNumber)
            )
            {
                return new UpdateAccountResponse
                {
                    Code = 400,
                    Message = Message.NO_FIELDS_PROVIDED,
                };
            }

            var account = await _updateAccountRepository.GetAccountByUserIdAsync(userId);
            if (account == null)
            {
                return new UpdateAccountResponse
                {
                    Code = 404,
                    Message = Message.ACCOUNT_NOT_EXSIT,
                };
            }

            if (!string.IsNullOrEmpty(updateRequest.Email))
            {
                var emailUsed = await _updateAccountRepository.IsEmailUsedByOtherAsync(
                    account.AccountId,
                    updateRequest.Email
                );
                if (emailUsed)
                {
                    return new UpdateAccountResponse { Code = 400, Message = Message.EMAIL_EXITS };
                }

                account.Email = updateRequest.Email;
            }

            if (!string.IsNullOrEmpty(updateRequest.PhoneNumber))
            {
                var phoneUsed = await _updateAccountRepository.IsPhoneUsedByOtherAsync(
                    account.AccountId,
                    updateRequest.PhoneNumber
                );
                if (phoneUsed)
                {
                    return new UpdateAccountResponse
                    {
                        Code = 400,
                        Message = Message.PHONE_NUMBER_EXITS,
                    };
                }

                account.PhoneNumber = updateRequest.PhoneNumber;
            }

            // Password
            if (
                !string.IsNullOrEmpty(updateRequest.PasswordOld)
                || !string.IsNullOrEmpty(updateRequest.PasswordNew)
            )
            {
                if (
                    string.IsNullOrEmpty(updateRequest.PasswordOld)
                    || string.IsNullOrEmpty(updateRequest.PasswordNew)
                )
                {
                    return new UpdateAccountResponse
                    {
                        Code = 400,
                        Message = Message.PASSWORD_OLD_OR_NEW_EMPTY,
                    };
                }

                string hashedOld = Sha256Hasher.ComputeSha256Hash(updateRequest.PasswordOld);
                if (hashedOld != account.Password)
                {
                    return new UpdateAccountResponse
                    {
                        Code = 400,
                        Message = Message.PASSWORD_OLD_NOT_EQUAL,
                    };
                }

                account.Password = Sha256Hasher.ComputeSha256Hash(updateRequest.PasswordNew);
            }

            try
            {
                await _updateAccountRepository.UpdateAccountAsync(account);
                _cache.Remove($"update_pending_{email}");

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
    }
}
