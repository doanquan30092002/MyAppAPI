namespace MyApp.Application.Interfaces.UpdateAccount.Service
{
    public interface IOTPService_1
    {
        Task<bool> SendOtpAsync(string to); // true nếu gửi thành công
        Task<(bool Success, string Message)> VerifyOtpAsync(string to, string code);
    }
}
