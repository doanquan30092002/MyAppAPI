using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.Interfaces.IAuctionRepository;
using MyApp.Application.Interfaces.IExcelRepository;
using MyApp.Core.Entities;
using MyApp.Infrastructure.Data;
using OfficeOpenXml;

namespace MyApp.Infrastructure.Repositories.ExcelRepository
{
    public class ExcelRepository : IExcelRepository
    {
        private readonly AppDbContext _context;
        private readonly IAuctionRepository _auctionRepository;

        public ExcelRepository(AppDbContext context, IAuctionRepository auctionRepository)
        {
            _context = context;
            _auctionRepository = auctionRepository;
        }

        public async Task<bool> CheckExcelFormatAsync(IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length == 0)
                return false;

            string[] requiredHeaders = new[]
            {
                "Tên nhãn (Tag_Name)",
                "Giá khởi điểm (starting_price)",
                "Đơn vị (Unit)",
                "Tiền đặt cọc (Deposit)",
                "Phí đăng ký (Registration_fee)",
                "Mô tả (Description)",
            };

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var stream = new MemoryStream())
            {
                await excelFile.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    if (worksheet == null)
                        return false;

                    // 1. Check header
                    for (int col = 1; col <= requiredHeaders.Length; col++)
                    {
                        var headerText = worksheet.Cells[1, col].Text?.Trim();
                        if (headerText != requiredHeaders[col - 1])
                            return false;
                    }

                    int rowCount = worksheet.Dimension.Rows;

                    // 2. Check data rows
                    for (int row = 2; row <= rowCount; row++)
                    {
                        // Cột 1: Tag_Name (string, bắt buộc có)
                        if (string.IsNullOrWhiteSpace(worksheet.Cells[row, 1].Text))
                            return false;

                        // Cột 2: starting_price (phải là số)
                        if (!IsValidDecimal(worksheet.Cells[row, 2].Text))
                            return false;

                        // Cột 3: Unit (string, bắt buộc có)
                        if (string.IsNullOrWhiteSpace(worksheet.Cells[row, 3].Text))
                            return false;

                        // Cột 4: Deposit (phải là số)
                        if (!IsValidDecimal(worksheet.Cells[row, 4].Text))
                            return false;

                        // Cột 5: Registration_fee (phải là số)
                        if (!IsValidDecimal(worksheet.Cells[row, 5].Text))
                            return false;

                        // Cột 6: Description (string, cho phép rỗng)
                        // Không cần check
                    }
                }
            }
            return true;
        }

        private bool IsValidDecimal(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;
            var style =
                NumberStyles.Number | NumberStyles.AllowThousands | NumberStyles.AllowDecimalPoint;
            return decimal.TryParse(
                input.Replace(",", "").Replace(" ", ""),
                style,
                CultureInfo.InvariantCulture,
                out _
            );
        }

        public async Task SaveAssetsFromExcelAsync(Guid auctionId, IFormFile excelFile, Guid userId)
        {
            var assets = new List<AuctionAssets>();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var stream = new MemoryStream())
            {
                await excelFile.CopyToAsync(stream);
                using (var package = new OfficeOpenXml.ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    int rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var asset = new AuctionAssets
                        {
                            AuctionAssetsId = Guid.NewGuid(),
                            TagName = worksheet.Cells[row, 1].Text,
                            StartingPrice = decimal.Parse(
                                worksheet.Cells[row, 2].Text,
                                NumberStyles.Any,
                                CultureInfo.InvariantCulture
                            ),
                            Unit = worksheet.Cells[row, 3].Text,
                            Deposit = decimal.Parse(
                                worksheet.Cells[row, 4].Text,
                                NumberStyles.Any,
                                CultureInfo.InvariantCulture
                            ),
                            RegistrationFee = decimal.Parse(
                                worksheet.Cells[row, 5].Text,
                                NumberStyles.Any,
                                CultureInfo.InvariantCulture
                            ),
                            Description = worksheet.Cells[row, 6].Text,
                            CreatedAt = DateTime.UtcNow,
                            CreatedBy = userId,
                            UpdatedAt = DateTime.UtcNow,
                            UpdatedBy = userId,
                            AuctionId = auctionId,
                        };
                        assets.Add(asset);
                    }
                }
            }
            await _context.AuctionAssets.AddRangeAsync(assets);
        }

        public async Task<byte[]> ExportRefundDocumentsExcelAsync(Guid auctionId)
        {
            var documents = await _auctionRepository.GetPaidOrDepositedDocumentsByAuctionIdAsync(
                auctionId
            );

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("HoSoHoanTien");

                // Header
                worksheet.Cells[1, 1].Value = "STT";
                worksheet.Cells[1, 2].Value = "Tên khách hàng";
                worksheet.Cells[1, 3].Value = "Số CCCD";
                worksheet.Cells[1, 4].Value = "Tài khoản ngân hàng";
                worksheet.Cells[1, 5].Value = "Số tài khoản";
                worksheet.Cells[1, 6].Value = "Chi nhánh";
                worksheet.Cells[1, 7].Value = "Mã tài sản";
                worksheet.Cells[1, 8].Value = "Tên tài sản";
                worksheet.Cells[1, 9].Value = "Tiền đặt cọc";
                worksheet.Cells[1, 10].Value = "Phí đăng ký";
                worksheet.Cells[1, 11].Value = "Trạng thái phiếu hồ sơ";
                worksheet.Cells[1, 12].Value = "Trạng thái tiền cọc";

                int row = 2,
                    stt = 1;
                foreach (var doc in documents)
                {
                    worksheet.Cells[row, 1].Value = stt++;
                    worksheet.Cells[row, 2].Value = doc.User?.Name ?? "";
                    worksheet.Cells[row, 3].Value = doc.User?.CitizenIdentification ?? "";
                    worksheet.Cells[row, 4].Value = doc.BankAccount;
                    worksheet.Cells[row, 5].Value = doc.BankAccountNumber;
                    worksheet.Cells[row, 6].Value = doc.BankBranch;
                    worksheet.Cells[row, 7].Value = doc.AuctionAsset?.TagName ?? "";
                    worksheet.Cells[row, 8].Value = doc.AuctionAsset?.Description ?? "";

                    // Tiền có dấu phẩy phân cách hàng nghìn
                    worksheet.Cells[row, 9].Value = doc.AuctionAsset?.Deposit ?? 0;
                    worksheet.Cells[row, 9].Style.Numberformat.Format = "#,##0";

                    worksheet.Cells[row, 10].Value = doc.AuctionAsset?.RegistrationFee ?? 0;
                    worksheet.Cells[row, 10].Style.Numberformat.Format = "#,##0";

                    worksheet.Cells[row, 11].Value = GetStatusTicketText(doc.StatusTicket);
                    worksheet.Cells[row, 12].Value = GetStatusDepositText(doc.StatusDeposit);

                    row++;
                }

                // Format header bold
                using (var range = worksheet.Cells[1, 1, 1, 12])
                {
                    range.Style.Font.Bold = true;
                }
                worksheet.Cells.AutoFitColumns();

                // Dropdown cho "Trạng thái phiếu hồ sơ" (cột 11)
                string[] statusTicketOptions = new[]
                {
                    "Chưa chuyển tiền",
                    "Đã chuyển tiền",
                    "Đã ký phiếu",
                    "Đã hoàn",
                    "Không xác định",
                };
                // Dropdown cho "Trạng thái tiền cọc" (cột 12)
                string[] statusDepositOptions = new[]
                {
                    "Chưa cọc",
                    "Đã cọc",
                    "Đã hoàn tiền",
                    "Đã hoàn",
                    "Không xác định",
                };

                for (int r = 2; r < row; r++)
                {
                    // Dropdown cho cột 11
                    var statusTicketValidation = worksheet.DataValidations.AddListValidation(
                        $"K{r}"
                    );
                    foreach (var opt in statusTicketOptions)
                        statusTicketValidation.Formula.Values.Add(opt);

                    // Dropdown cho cột 12
                    var statusDepositValidation = worksheet.DataValidations.AddListValidation(
                        $"L{r}"
                    );
                    foreach (var opt in statusDepositOptions)
                        statusDepositValidation.Formula.Values.Add(opt);
                }

                return await Task.FromResult(package.GetAsByteArray());
            }
        }

        private string GetStatusTicketText(int status)
        {
            return status switch
            {
                0 => "Chưa chuyển tiền",
                1 => "Đã chuyển tiền",
                2 => "Đã ký phiếu",
                3 => "Đã hoàn",
                _ => "Không xác định",
            };
        }

        private string GetStatusDepositText(int status)
        {
            return status switch
            {
                0 => "Chưa cọc",
                1 => "Đã cọc",
                2 => "Đã hoàn tiền",
                3 => "Đã hoàn",
                _ => "Không xác định",
            };
        }
    }
}
