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
        public const string ACCOUNT_NOT_EXSIT = "Tài khoản không tồn tại.";

        #endregion
        #region UpdateExpiredProfile
        public const string CITIZEN_IDENTIFICATION_NOT_MATCH =
            "Số Căn Cước Công Dân không trùng với số đã đăng ký ban đầu";
        public const string UPDATE_PROFILE_SUCCESS = "Cập nhật thông tin cá nhân thành công";
        public const string USER_DOES_NOT_EXSIT = "Người dùng không tồn tại.";
        public const string LOGIN_INFO_NOT_FOUND = "Không tìm thấy thông tin đăng nhập.";

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
        public const string CUSTOMER_REGISTED_AUCTION =
            "Có khách hàng vừa tham gia đấu giá phiên: {0}";

        #endregion
        #region AssginAuctioneerAndPublicAuction
        public const string AUCTIONEER_ASSIGNED_ANOTHER_AUCTION =
            "Đấu giá viên đã được phân công cho một phiên đấu giá khác cùng ngày. Vui lòng chọn đấu giá viên khác";
        public const string ASSGIN_AUCTIONEER_AND_PUBLIC_AUCTION_SUCCESS =
            "Phiên đấu giá đã được công khai";
        public const string NOT_FOUND_AUCTIONEER = "Không tìm thấy thông tin người đấu giá";
        public const string GET_AUCTIONEER_SUCCESS = "Lấy thông tin người đấu giá thành công";
        public const string NEW_AUCTION_TO_CUSTOMER = "Có phiên đấu giá mới: {0}";
        public const string AUCTION_NOT_WAITING =
            "Phiên đấu giá không ở trạng thái chờ duyệt. Vui lòng kiểm tra lại";
        #endregion
        #region DetailAuctionDocument
        public const string GET_AUCTION_DOCUMENT_FAIL = "Không tồn tại thông tin này";
        public const string GET_AUCTION_DOCUMENT_SUCCESS = "Lấy thông tin thành công";
        #endregion
        #region ReceiveAuctionRegistrationForm
        public const string RECEIVE_AUCTION_REGISTRATION_FORM_SUCCESS =
            "Nhận hồ sơ đăng ký tham gia đấu giá thành công";
        public const string RECEIVE_AUCTION_REGISTRATION_FORM_FAIL =
            "Nhận hồ sơ đăng ký tham gia đấu giá thất bại";
        public const string RECEIVED_FORM_SUCCESS =
            "Công ty đã nhận được hồ sơ đăng ký tham gia đấu gia phiên: {0}";
        #endregion
        #region GenarateNumbericalOrder
        public const string GENARATE_NUMBERICAL_ORDER_SUCCESS = "Tạo số thứ tự thành công";
        public const string GENARATE_NUMBERICAL_ORDER_FAIL = "Tạo số thứ tự thất bại";
        #endregion
        #region GetListAuctionsDocument
        public const string GET_LIST_AUCTION_DOCUMENT_FAIL =
            "Đã xảy ra lỗi khi truy xuất danh sách hồ sơ đấu giá.";
        public const string NOT_FOUND_LIST_AUCTION_DOCUMENT =
            "Không tìm thấy danh sách hồ sơ đăng ký.";
        public const string GET_LIST_AUCTION_SUCESS = "Lấy danh sách hồ sơ thành công.";
        #endregion
        #region UpdateDepositStatus
        public const string UPDATE_DEPOSIT_STATUS_SUCESS = "Cập nhật trạng thái cọc thành công";
        public const string UPDATE_DEPOSIT_STATUS_FAIL = "Cập nhật trạng thái cọc thất bại";
        #endregion
        #region GetListAuctionRegisted
        public const string GET_LIST_AUCTION_REGISTED_SUCCESS =
            "Lấy danh sách đấu giá đã đăng ký thành công";
        public const string GET_LIST_AUCTION_REGISTED_NOT_EXIST = "Không có đấu giá nào đã đăng ký";
        #endregion
        #region AuctionDocumentRegisted
        public const string GET_AUCTION_DOCUMENT_REGISTED_SUCCESS =
            "Lấy thông tin hồ sơ đăng ký thành công";
        public const string GET_AUCTION_DOCUMENT_REGISTED_NOT_EXIST = "Không có hồ sơ đăng ký nào";
        #endregion
        #region UserRegisteredAuction
        public const string GET_USER_REGISTERED_AUCTION_SUCCESS =
            "Lấy thông tin người dùng đã đăng ký thành công";
        public const string USER_NOT_REGISTERED_OR_INELIGIBLE_PARTICIPATE =
            "Người dùng chưa đăng ký hoặc không đủ điều kiện tham gia đấu giá";
        public const string CITIZEN_NOT_EXIST = "Căn cước công dân không tồn tại trong hệ thống";
        #endregion
        #region CreateAuctionRound
        public const string CREATE_AUCTION_ROUND_SUCCESS = "Tạo vòng đấu giá thành công";
        public const string CREATE_AUCTION_ROUND_FAIL = "Tạo vòng đấu giá thất bại";
        #endregion
        #region CreateAuctionRound
        public const string NOT_FOUND_ROUND_BY_AUCTION_ID =
            "Không tồn tại vòng trả giá với id được nhập";
        #endregion
        #region SaveListPrices
        public const string SAVE_LIST_PRICES_SUCCESS = "Lưu danh sách giá thành công";
        public const string SAVE_LIST_PRICES_FAIL = "Lưu danh sách giá thất bại";
        #endregion
        #region Blog
        // Create blog
        public const string CREATE_BLOG_SUCCESS = "Tạo blog thành công";
        public const string CREATE_BLOG_FAIL = "Tạo blog thất bại";

        // Get blog detail
        public const string GET_BLOG_SUCCESS = "Lấy blog thành công";
        public const string GET_BLOG_NOT_FOUND = "Lấy blog thất bại";

        // Update blog
        public const string UPDATE_BLOG_SUCCESS = "Cập nhật blog thành công";
        public const string UPDATE_BLOG_FAIL = "Cập nhật blog thất bại";

        // Change status blog
        public const string UPDATE_STATUS_BLOG_SUCCESS = "Cập nhật blog thành công";
        public const string UPDATE_STATUS_BLOG_FAIL = "Cập nhật blog thất bại";

        //  Get list blogs
        public const string GET_BLOGS_SUCCESS = "Lấy danh sách blog thành công";
        public const string GET_BLOGS_NOT_FOUND = "Không tìm thấy blog nào";

        // Delete blog
        public const string DELETE_BLOG_SUCCESS = "Xoá blog thành công";
        public const string DELETE_BLOG_FAIL = "Xoá blog thất bại";
        #endregion
        #region GetListEnteredPrices
        public const string NOT_FOUND_LIST_ENTERED_PRICES =
            "Không tìm thấy danh sách giá được nhập";
        #endregion
        #region GetAuction
        public const string NOT_FOUND_AUCTION = "Không tìm thấy phiên đấu giá";
        #endregion
        #region UpdateWinnerFlag
        public const string NOT_FOUND_ROUND_PRICES_ID_TO_UPDATE = "Không tìm thấy id cần update";
        #endregion
        #region ChangeStatusAuctionRound
        public const string CHANGE_STATUS_AUCTION_ROUND_SUCCESS =
            "Thay đổi trạng thái vòng đấu giá thành công";
        public const string CHANGE_STATUS_AUCTION_ROUND_FAIL =
            "Thay đổi trạng thái vòng đấu giá thất bại";
        #endregion
        #region GetListUserWinner
        public const string NOT_FOUND_LIST_USER_WINNER =
            "Không tìm thấy danh sách người thắng cuộc";
        #endregion
        #region EmployeeManager
        public const string GET_LIST_EMPLOYEE_ACCOUNT_SUCCESS =
            "Lấy danh sách tài khoản nhân viên thành công";
        public const string GET_LIST_EMPLOYEE_ACCOUNT_NOT_FOUND =
            "Không tìm thấy danh sách tài khoản nhân viên";
        public const string CHANGE_STATUS_EMPLOYEE_ACCOUNT_SUCCESS =
            "Thay đổi trạng thái tài khoản nhân viên thành công";
        public const string CHANGE_STATUS_EMPLOYEE_ACCOUNT_FAIL =
            "Thay đổi trạng thái tài khoản nhân viên thất bại";
        public const string CHANGE_PERMISSTION_EMPLOYEE_ACCOUNT_FAIL =
            "Thay đổi quyền hạn tài khoản nhân viên thất bại";
        public const string CHANGE_PERMISSTION_EMPLOYEE_ACCOUNT_SUCCESS =
            "Thay đổi quyền hạn tài khoản nhân viên thành công";
        #endregion
    }
}
