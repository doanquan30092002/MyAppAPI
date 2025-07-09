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
        public const string SEND_OTP_SUCCESS = "OTP đã được gửi thành công";
        public const string SEND_OTP_FAIL = "Gửi OTP không thành công";
        public const string OTP_CORRECT = "OTP hợp lệ";
        public const string OTP_INCORRECT = "OTP không đúng";
        public const string OTP_EXPIRED_OR_NOT_EXIST = "OTP đã hết hạn hoặc không tồn tại";
        public const string EMAIL_SUBJECT = "Mã OTP xác thực";
        public const string EMAIL_BODY = "Mã OTP của bạn là: ";
        #endregion
        #region UpdateExpiredProfile
        public const string CITIZEN_IDENTIFICATION_NOT_MATCH =
            "Số Căn Cước Công Dân không trùng với số đã đăng ký ban đầu";
        public const string UPDATE_PROFILE_SUCCESS = "Cập nhật thông tin cá nhân thành công";
        #endregion
        #region RegisterAuctionDocument
        public const string CREATE_QR_SUCCESS = "Tạo QR thành công";

        // 0: chưa tồn tại hồ sơ đăng ký
        // 1: đã thanh toán hồ sơ đăng ký
        public const int REGISTER_TICKET_NOT_PAID = 0;
        public const int REGISTER_TICKET_PAID = 1;
        public const string REGISTER_AUCTION_DOCUMENT_FAIL = "Lỗi khi tạo phiếu đăng ký hồ sơ";
        public const string AUCTION_DOCUMENT_EXIST =
            "Bạn đã đăng ký mua hồ sơ tham gia đấu giá cho tài sản này. Vui lòng xem lịch sử mua hồ sơ";
        #endregion
        #region AssginAuctioneerAndPublicAuction
        public const string AUCTIONEER_ASSIGNED_ANOTHER_AUCTION =
            "Đấu giá viên đã được phân công cho một phiên đấu giá khác cùng ngày. Vui lòng chọn đấu giá viên khác";
        public const string ASSGIN_AUCTIONEER_AND_PUBLIC_AUCTION_SUCCESS =
            "Phiên đấu giá đã được công khai";
        public const string NOT_FOUND_AUCTIONEER = "Không tìm thấy thông tin người đấu giá";
        public const string GET_AUCTIONEER_SUCCESS = "Lấy thông tin người đấu giá thành công";
        #endregion
        #region DetailAuctionDocument
        public const string GET_AUCTION_DOCUMENT_FAIL = "Không tồn tại thông tin này";
        public const string GET_AUCTION_DOCUMENT_SUCCESS = "Lấy thông tin thành công";
        #endregion

        #region GetListAuctionsDocument
        public const string GET_LIST_AUCTION_DOCUMENT_FAIL =
            "Đã xảy ra lỗi khi truy xuất danh sách hồ sơ đấu giá.";
        public const string NOT_FOUND_LIST_AUCTION_DOCUMENT =
            "Không tìm thấy danh sách hồ sơ đăng ký.";
        public const string GET_LIST_AUCTION_SUCESS = "Lấy danh sách hồ sơ thành công.";
        #endregion
    }
}
