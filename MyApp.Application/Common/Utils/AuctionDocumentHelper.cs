using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MyApp.Application.Common.Utils
{
    public static class AuctionDocumentHelper
    {
        /// <summary>
        /// Tìm và lấy GUID phía sau tiền tố "DH" trong nội dung content.
        /// </summary>
        /// <param name="content">Chuỗi content cần tách</param>
        /// <returns>Guid nếu tìm thấy, Guid.Empty nếu không hợp lệ hoặc không tìm thấy</returns>
        public static Guid ExtractAuctionDocumentId(string content)
        {
            if (string.IsNullOrEmpty(content))
                return Guid.Empty;

            // Pattern: DH + khoảng trắng hoặc không + GUID (dạng chuẩn)
            var match = Regex.Match(
                content,
                @"DH\s*([0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12})"
            );

            if (match.Success && match.Groups.Count > 1)
            {
                if (Guid.TryParse(match.Groups[1].Value, out Guid result))
                    return result;
            }

            return Guid.Empty;
        }

        public static Guid? ExtractAuctionDocumentId_1(string input)
        {
            const string prefix = "DH";

            // Tìm vị trí xuất hiện "DH"
            int index = input.IndexOf(prefix);
            if (index == -1 || input.Length < index + prefix.Length + 32)
            {
                // Không tìm thấy "DH" hoặc không đủ 32 ký tự phía sau
                return null;
            }

            // Lấy chuỗi 32 ký tự sau "DH"
            string hex = input.Substring(index + prefix.Length, 32);

            // Kiểm tra và parse
            if (Guid.TryParseExact(hex, "N", out Guid guid))
            {
                return guid;
            }

            return null;
        }
    }
}
