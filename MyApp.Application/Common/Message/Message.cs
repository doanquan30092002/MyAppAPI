namespace MyApp.Application.Common.Message
{
    public class Message
    {
        //SignUp
        public const string EMAIL_EXITS = "Email đã được sử dụng";
        public const string PHONE_NUMBER_EXITS = "Số diện thoại đã được sử dụng";
        public const string CITIZENS_ID_EXITS = "Số Căn cước công dân đã được sử dụng";
        public const string CREATE_FAIL = "Có lỗi không thể đăng ký tài khoản";
        public const string CREATE_SUCCESS = "Đăng ký tài khoản thành công";
        public const string VALIDATION_FAILED = "Xác thực thất bại";

        #region Login
        public const string LOGIN_SUCCESS = "Đăng nhập thành công";
        public const string ACCOUNT_LOCKED = "Tài khoản của bạn đã bị khóa";
        public const string LOGIN_WRONG = "Sai tài khoản hoặc mật khẩu";
        #endregion
        #region SearchUserAttendance
        public const string FOUND_NUMERICAL_ORDER = "Tìm thấy số thứ tự";
        public const string NOT_FOUND_NUMERICAL_ORDER = "Không tìm thấy số thứ tự";
        public const string AUCTION_NOT_EXIST = "Không tồn tại phiên đấu giá này";
        #endregion
    }
}
