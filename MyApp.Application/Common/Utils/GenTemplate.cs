using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Application.Common.Utils
{
    public static class GenTemplate
    {
        public static string GenerateOtpEmailTemplate(string otp, double expireMinutes)
        {
            return $@"
<!DOCTYPE html>
<html lang=""vi"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <style>
        body {{
            font-family: 'Arial', sans-serif;
            background-color: #f4f4f4;
            margin: 0;
            padding: 0;
        }}
        .container {{
            max-width: 600px;
            margin: 20px auto;
            background-color: #ffffff;
            border-radius: 8px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
            overflow: hidden;
        }}
        .header {{
            background-color: #4a90e2;
            padding: 20px;
            text-align: center;
            color: white;
        }}
        .header h1 {{
            margin: 0;
            font-size: 24px;
        }}
        .content {{
            padding: 30px;
            text-align: center;
        }}
        .otp-code {{
            font-size: 32px;
            font-weight: bold;
            color: #4a90e2;
            margin: 20px 0;
            padding: 10px;
            background-color: #f8f9fa;
            border-radius: 4px;
            display: inline-block;
        }}
        .content p {{
            color: #666;
            line-height: 1.6;
            margin: 10px 0;
        }}
        .footer {{
            background-color: #f8f9fa;
            padding: 20px;
            text-align: center;
            color: #999;
            font-size: 14px;
        }}
        .footer a {{
            color: #4a90e2;
            text-decoration: none;
        }}
        @media only screen and (max-width: 600px) {{
            .container {{
                margin: 10px;
            }}
            .content {{
                padding: 20px;
            }}
            .otp-code {{
                font-size: 28px;
            }}
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>Xác Thực OTP</h1>
        </div>
        <div class=""content"">
            <p>Chào bạn,</p>
            <p>Đây là mã OTP của bạn để xác thực tài khoản:</p>
            <div class=""otp-code"">{otp}</div>
            <p>Mã này có hiệu lực trong {expireMinutes} phút. Vui lòng không chia sẻ mã này với bất kỳ ai.</p>
            <p>Nếu bạn không yêu cầu mã này, vui lòng bỏ qua email này.</p>
        </div>
        <div class=""footer"">
            <p>© {DateTime.Now.Year} Tuan Tinh Auction Digital.</p>
            <p><a href=""https://myapp.com/support"">Liên hệ hỗ trợ</a></p>
        </div>
    </div>
</body>
</html>";
        }

        public static string GenerateCancelAuctionTemplate(string auctionName, string cancelReason)
        {
            return $@"
<!DOCTYPE html>
<html lang=""vi"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <style>
        body {{
            font-family: 'Arial', sans-serif;
            background-color: #f4f4f4;
            margin: 0;
            padding: 0;
        }}
        .container {{
            max-width: 600px;
            margin: 20px auto;
            background-color: #ffffff;
            border-radius: 8px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
            overflow: hidden;
        }}
        .header {{
            background-color: #e74c3c;
            padding: 20px;
            text-align: center;
            color: white;
        }}
        .header h1 {{
            margin: 0;
            font-size: 22px;
        }}
        .content {{
            padding: 30px;
            color: #333;
        }}
        .content h2 {{
            font-size: 20px;
            margin-bottom: 10px;
            color: #e74c3c;
        }}
        .reason-box {{
            background-color: #fcebea;
            border-left: 5px solid #e74c3c;
            padding: 15px;
            margin: 20px 0;
            border-radius: 4px;
            color: #c0392b;
        }}
        .note-box {{
            background-color: #eef9f1;
            border-left: 5px solid #27ae60;
            padding: 15px;
            margin: 20px 0;
            border-radius: 4px;
            color: #2e7d32;
            line-height: 1.6;
        }}
        .content p {{
            line-height: 1.6;
            margin: 10px 0;
        }}
        .footer {{
            background-color: #f8f9fa;
            padding: 20px;
            text-align: center;
            color: #999;
            font-size: 14px;
        }}
        .footer a {{
            color: #4a90e2;
            text-decoration: none;
        }}
        @media only screen and (max-width: 600px) {{
            .container {{
                margin: 10px;
            }}
            .content {{
                padding: 20px;
            }}
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>Thông Báo Hủy Phiên Đấu Giá</h1>
        </div>
        <div class=""content"">
            <p>Kính gửi Quý khách,</p>
            <p>Chúng tôi xin thông báo rằng phiên đấu giá <b>{auctionName}</b> đã bị hủy.</p>
            
            <h2>Lý do hủy:</h2>
            <div class=""reason-box"">{cancelReason}</div>
            
            <div class=""note-box"">
                Trong trường hợp phiên đấu giá bị hủy, chúng tôi sẽ hoàn trả hồ sơ và tiền đặt cọc 
                trong vòng <b>03 ngày làm việc</b> kể từ ngày phiên đấu giá kết thúc. 
                <br/><br/>
                ✅ Nếu Quý khách có cung cấp thông tin tài khoản ngân hàng trong <b>Phiếu đăng ký tham gia đấu giá</b>, 
                chúng tôi sẽ chuyển khoản trực tiếp.  
                <br/>
                ⚠️ Nếu không có thông tin chuyển khoản, vui lòng liên hệ với chúng tôi để nhận lại bằng tiền mặt tại văn phòng.
            </div>
            
            <p>Chúng tôi thành thật xin lỗi vì sự bất tiện này và mong Quý khách thông cảm.</p>
            <p>Nếu cần hỗ trợ thêm, vui lòng liên hệ với chúng tôi.</p>
        </div>
        <div class=""footer"">
            <p>© {DateTime.Now.Year} Tuan Tinh Auction Digital.</p>
        </div>
    </div>
</body>
</html>";
        }
    }
}
