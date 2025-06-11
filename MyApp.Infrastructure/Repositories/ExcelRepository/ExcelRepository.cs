using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.Interfaces.IExcelRepository;
using MyApp.Core.Entities;
using MyApp.Infrastructure.Data;
using OfficeOpenXml;

namespace MyApp.Infrastructure.Repositories.ExcelRepository
{
    public class ExcelRepository : IExcelRepository
    {
        private readonly AppDbContext _context;

        public ExcelRepository(AppDbContext context)
        {
            _context = context;
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
    }
}
