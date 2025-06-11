using System.Security.Cryptography;
using System.Text;

namespace MyApp.Application.Common.Sha256Hasher
{
    public class Sha256Hasher
    {
        /// <summary>
        /// Băm một chuỗi thành giá trị SHA-256.
        /// </summary>
        /// <param name="rawString">Chuỗi đầu vào.</param>
        /// <returns>Chuỗi thập lục phân của giá trị SHA-256.</returns>
        public static string ComputeSha256Hash(string rawString)
        {
            if (string.IsNullOrEmpty(rawString))
            {
                // Trả về chuỗi rỗng cho đầu vào rỗng, hoặc ném ngoại lệ nếu thích.
                return string.Empty;
            }

            // Tạo một đối tượng SHA256
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Băm dữ liệu đầu vào.
                // Chuyển chuỗi thành mảng byte bằng UTF8 encoding.
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawString));

                // Chuyển mảng byte của hash thành một chuỗi thập lục phân.
                // Mỗi byte được định dạng thành 2 chữ số thập lục phân ("x2").
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
