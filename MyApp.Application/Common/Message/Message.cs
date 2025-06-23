namespace MyApp.Application.Common.Message
{
    public class Message
    {
        #region Common
        public const string HANDLER_FAILED = "Xử lý thất bại";
        public const string HANDLER_SUCCESS = "Xử lý thành công";
        public const string HANDLER_ERROR = "Xử lý xảy ra lỗi";
        public const string SYSTEM_ERROR = "Hệ thống đang bị lỗi. Xin thử lại sau!";
        #endregion

        #region SignUp
        public const string EMAIL_EXITS = "Email đã được sử dụng";
        public const string PHONE_NUMBER_EXITS = "Số diện thoại đã được sử dụng";
        public const string CITIZENS_ID_EXITS = "Số Căn cước công dân đã được sử dụng";
        public const string CREATE_FAIL = "Có lỗi không thể đăng ký tài khoản";
        public const string CREATE_SUCCESS = "Đăng ký tài khoản thành công";
        public const string VALIDATION_FAILED = "Xác thực thất bại";
        #endregion

        #region GetUserInfo
        public const string GET_USER_INFO_SUCCESS = "Lấy thông tin người dùng thành công";
        public const string GET_USER_INFO_FAIL = "Lấy thông tin người dùng thất bại";
        #endregion

        #region Login
        public const string LOGIN_SUCCESS = "Đăng nhập thành công";
        public const string ACCOUNT_LOCKED = "Tài khoản của bạn đã bị khóa";
        public const string LOGIN_WRONG = "Sai tài khoản hoặc mật khẩu";
        public const string EXPIRED_CITIZEN_IDENTIFICATION =
            "Căn cước công dân hết hạn. Vui lòng cập nhật!";
        #endregion

        #region Logout
        public const string LOGOUT_SUCCESS = "Đăng xuất thành công.";
        #endregion

        #region SearchUserAttendance
        public const string FOUND_NUMERICAL_ORDER = "Tìm thấy số thứ tự";
        public const string NOT_FOUND_NUMERICAL_ORDER = "Không tìm thấy số thứ tự";
        public const string AUCTION_NOT_EXIST = "Không tồn tại phiên đấu giá này";
        #endregion
        #region UpdateAccount
        public const string UPDATE_ACCOUNT_SUCCESS = "Cập nhật tài khoản thành công";
        public const string PASSWORD_OLD_NOT_EQUAL = "Mật khẩu cũ không khớp với mật khẩu hiện tại";
        public const string PASSWORD_OLD_OR_NEW_EMPTY = "Hãy nhập đủ mật khẩu cũ và mật khẩu mới";
        public const string NO_FIELDS_PROVIDED = "Không có thông tin nào được cập nhật";
        #endregion
        #region UpdateExpiredProfile
        public const string CITIZEN_IDENTIFICATION_NOT_MATCH =
            "Số Căn Cước Công Dân không trùng với số đã đăng ký ban đầu";
        public const string UPDATE_PROFILE_SUCCESS = "Cập nhật thông tin cá nhân thành công";
        #endregion
    }
}
