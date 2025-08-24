using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MyApp.Core.DTOs.AuctionDocumentsDTO;
using TemplateEngine.Docx;

namespace MyApp.Application.Common.Services.ExportWord.ExportAuctionBook
{
    public class AuctionBookExporter : IAuctionBookExporter
    {
        public async Task<byte[]> ExportToWordAsync(
            List<AuctionDocumentDto> data,
            IFormFile templateFile
        )
        {
            try
            {
                using var outputStream = new MemoryStream();
                using var templateStream = new MemoryStream();

                if (templateFile != null)
                {
                    await templateFile.CopyToAsync(templateStream);
                }
                else
                {
                    // Lấy thư mục chạy (bin\Debug\... của Api project)
                    var baseDir = AppContext.BaseDirectory;

                    // Đi ngược lên tới thư mục project MyApp.Api
                    var apiProjectDir = Directory.GetParent(baseDir)!.Parent!.Parent!.Parent!;

                    // Lấy solution folder (cha của MyApp.Api)
                    var solutionDir = apiProjectDir.Parent!.FullName;

                    var defaultTemplatePath = Path.Combine(
                        solutionDir,
                        "MyApp.Application",
                        "Common",
                        "Services",
                        "ExportWord",
                        "ExportAuctionBook",
                        "mau-so-dang-ky.docx"
                    );

                    if (!File.Exists(defaultTemplatePath))
                        throw new FileNotFoundException(
                            "Không tìm thấy file mẫu mặc định",
                            defaultTemplatePath
                        );

                    var bytes = await File.ReadAllBytesAsync(defaultTemplatePath);
                    await templateStream.WriteAsync(bytes, 0, bytes.Length);
                }

                templateStream.Position = 0;

                using var document = new TemplateProcessor(templateStream).SetRemoveContentControls(
                    true
                );

                var tableContent = new TableContent("DangKyTable");

                int stt = 1;
                foreach (var item in data)
                {
                    tableContent.AddRow(
                        new FieldContent("STT", stt.ToString()),
                        new FieldContent("Ngay", item.CreateAtTicket.ToString("dd/MM/yyyy")),
                        new FieldContent("TaiSan", item.AssetName),
                        new FieldContent("HoTen", item.UserName),
                        new FieldContent("DiaChi", item.RecentLocation),
                        new FieldContent("CMND", item.CitizenIdentification),
                        new FieldContent("DienThoai", item.PhoneNumber),
                        new FieldContent("TienCoc", item.Deposit.ToString("N0")),
                        new FieldContent("KetQua", item.Result ?? "")
                    );
                    stt++;
                }

                var content = new Content(tableContent);
                document.FillContent(content);
                document.SaveChanges();

                templateStream.Position = 0;
                await templateStream.CopyToAsync(outputStream);

                return outputStream.ToArray();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Đã xảy ra lỗi khi xuất file Word.", ex);
            }
        }
    }
}
