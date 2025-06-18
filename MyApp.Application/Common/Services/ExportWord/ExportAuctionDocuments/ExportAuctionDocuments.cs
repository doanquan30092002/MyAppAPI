using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MyApp.Application.Interfaces.ISupportRegisterDocuments;
using MyApp.Core.Entities;
using TemplateEngine.Docx;

namespace MyApp.Application.Common.Services.ExportWord.ExportAuctionDocuments
{
    public class ExportAuctionDocuments : IExportAuctionDocuments
    {
        private readonly ISupportRegisterDocuments _supportRegisterDocuments;

        public ExportAuctionDocuments(ISupportRegisterDocuments supportRegisterDocuments)
        {
            _supportRegisterDocuments = supportRegisterDocuments;
        }

        public async Task<(string FileName, MemoryStream Stream)> ExportAuctionDocumentToWordFile(
            AuctionDocuments document,
            IFormFile templateFile
        )
        {
            byte[] templateBytes;
            using (var templateStream = templateFile.OpenReadStream())
            using (var ms = new MemoryStream())
            {
                await templateStream.CopyToAsync(ms);
                templateBytes = ms.ToArray();
            }

            var user = document.User;
            var asset = document.AuctionAsset;
            var auction = asset?.Auction;
            string? phone =
                await _supportRegisterDocuments.GetPhoneNumberByCitizenIdentificationAsync(
                    user?.CitizenIdentification ?? ""
                );

            using var inputStream = new MemoryStream(
                templateBytes,
                0,
                templateBytes.Length,
                writable: true
            );
            var tempStream = new MemoryStream();

            using (
                var processor = new TemplateProcessor(inputStream).SetRemoveContentControls(true)
            )
            {
                var content = new Content(
                    new FieldContent("FULL_NAME", user?.Name ?? ""),
                    new FieldContent("DOB", user?.BirthDay.ToString("dd/MM/yyyy") ?? ""),
                    new FieldContent("ID_NUMBER", user?.CitizenIdentification ?? ""),
                    new FieldContent("ID_DATE", user?.IssueDate.ToString("dd/MM/yyyy") ?? ""),
                    new FieldContent("PLACE_ID", user?.IssueBy ?? ""),
                    new FieldContent("PHONE", phone ?? ""),
                    new FieldContent("ADDRESS", user?.RecentLocation ?? ""),
                    new FieldContent("AUCTION_INFO", auction?.AuctionName ?? ""),
                    new FieldContent("ASSETS_INFO", asset?.Description ?? ""),
                    new FieldContent(
                        "PRICE",
                        asset?.StartingPrice.ToString("N0", new CultureInfo("vi-VN")) ?? ""
                    ),
                    new FieldContent("BANK_ACCOUNT", document.BankAccount ?? ""),
                    new FieldContent("BANK_ACCOUNT_NUMBER", document.BankAccountNumber ?? ""),
                    new FieldContent("BANK_BRANCH", document.BankBranch ?? ""),
                    new FieldContent(
                        "DATE",
                        $"ngày {DateTime.Now:dd} tháng {DateTime.Now:MM} năm {DateTime.Now:yyyy}"
                    )
                );
                processor.FillContent(content);
                processor.SaveChanges();
            }

            inputStream.Position = 0;
            await inputStream.CopyToAsync(tempStream);
            tempStream.Position = 0;

            string fileName = $"{user?.Name ?? "Unknown"}_{Guid.NewGuid():N}.docx";
            return (fileName, tempStream);
        }
    }
}
