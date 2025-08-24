namespace MyApp.Application.Common.Utils
{
    public interface ITemplateEmail
    {
        string BuildEmailHtml(string title, string bodyHtml);
    }

    public class TemplateEmail : ITemplateEmail
    {
        string BuildEmailHtml(string title, string bodyHtml)
        {
            return $@"<!doctype html>
<html lang=""vi"">
<head>
  <meta charset=""utf-8"">
  <meta name=""viewport"" content=""width=device-width,initial-scale=1"">
  <title>{title}</title>
</head>
<body style=""margin:0;padding:0;background:#f5f7fb;"">
  <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""background:#f5f7fb;padding:24px 0;"">
    <tr>
      <td align=""center"">
        <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""640"" 
               style=""width:640px;max-width:100%;background:#ffffff;border-radius:12px;
                      box-shadow:0 2px 8px rgba(16,24,40,.06);
                      font-family:Segoe UI,Roboto,Helvetica,Arial,sans-serif;
                      color:#101828;"">
          <tr>
            <td style=""padding:20px 28px;border-bottom:1px solid #eaecf0;"">
              <h2 style=""margin:0;font-size:18px;line-height:26px;color:#101828;"">{title}</h2>
            </td>
          </tr>
          <tr>
            <td style=""padding:20px 28px;font-size:14px;line-height:22px;color:#344054;"">
              {bodyHtml}
            </td>
          </tr>
          <tr>
            <td style=""padding:16px 28px;border-top:1px solid #eaecf0;font-size:12px;line-height:18px;color:#667085;"">
              <p style=""margin:0;"">Trân trọng,<br>Hệ thống đấu giá số Tuấn Linh</p>
            </td>
          </tr>
        </table>
      </td>
    </tr>
  </table>
</body>
</html>";
        }

        string ITemplateEmail.BuildEmailHtml(string title, string bodyHtml)
        {
            return BuildEmailHtml(title, bodyHtml);
        }
    }
}
