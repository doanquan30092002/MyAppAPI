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
    }
}
