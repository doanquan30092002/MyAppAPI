using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

                    // 2. Xác định dòng cuối có dữ liệu thật
                    int lastRowWithData = worksheet
                        .Cells.Where(c => !string.IsNullOrWhiteSpace(c.Text))
                        .Select(c => c.Start.Row)
                        .DefaultIfEmpty(1) // Nếu không có dữ liệu thì trả về 1 (chỉ có header)
                        .Max();

                    // 3. Check data rows
                    for (int row = 2; row <= lastRowWithData; row++)
                    {
                        // Bỏ qua nếu toàn bộ hàng trống
                        bool isRowEmpty = true;
                        for (int col = 1; col <= requiredHeaders.Length; col++)
                        {
                            if (!string.IsNullOrWhiteSpace(worksheet.Cells[row, col].Text))
                            {
                                isRowEmpty = false;
                                break;
                            }
                        }
                        if (isRowEmpty)
                            continue;

                        // Cột 1: Tag_Name (string, bắt buộc có)
                        if (string.IsNullOrWhiteSpace(worksheet.Cells[row, 1].Text))
                            return false;

                        // Cột 2: starting_price (phải là số)
                        if (!IsValidPrice(worksheet.Cells[row, 2], min: 0))
                            return false;

                        // Cột 3: Unit (string, bắt buộc có)
                        if (string.IsNullOrWhiteSpace(worksheet.Cells[row, 3].Text))
                            return false;

                        // Cột 4: Deposit (phải là số)
                        if (!IsValidPrice(worksheet.Cells[row, 4], min: 0))
                            return false;

                        // Cột 5: Registration_fee (phải là số)
                        if (!IsValidPrice(worksheet.Cells[row, 5], min: 0))
                            return false;

                        // Cột 6: Description (string, cho phép rỗng) => không check
                    }
                }
            }
            return true;
        }

        private bool IsValidPrice(ExcelRange cell, decimal min = 0, decimal? max = null)
        {
            if (cell.Value == null)
                return false;

            // Trường hợp giá trị là số thực tế
            if (cell.Value is double || cell.Value is decimal || cell.Value is int)
            {
                decimal price = Convert.ToDecimal(cell.Value);
                if (price < min)
                    return false;
                if (max.HasValue && price > max.Value)
                    return false;
                return true;
            }

            // Trường hợp là chuỗi, cần parse
            var text = cell.Text?.Trim();
            if (string.IsNullOrWhiteSpace(text))
                return false;

            // Xử lý chuỗi chứa dấu phẩy hoặc chấm
            text = text.Replace(",", "").Replace(" ", "");
            if (decimal.TryParse(text, out var parsedPrice))
            {
                if (parsedPrice < min)
                    return false;
                if (max.HasValue && parsedPrice > max.Value)
                    return false;
                return true;
            }

            return false;
        }

        public async Task SaveAssetsFromExcelAsync(Guid auctionId, IFormFile excelFile, Guid userId)
        {
            var assets = new List<AuctionAssets>();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var stream = new MemoryStream())
            {
                await excelFile.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    if (worksheet == null)
                        throw new ValidationException("File Excel không có sheet nào.");

                    int lastRowWithData = worksheet
                        .Cells.Where(c => !string.IsNullOrWhiteSpace(c.Text))
                        .Select(c => c.Start.Row)
                        .DefaultIfEmpty(1)
                        .Max();

                    for (int row = 2; row <= lastRowWithData; row++)
                    {
                        var asset = new AuctionAssets
                        {
                            AuctionAssetsId = Guid.NewGuid(),
                            TagName = worksheet.Cells[row, 1].Text?.Trim(),
                            StartingPrice = ParseDecimalCell(
                                worksheet.Cells[row, 2],
                                "Giá khởi điểm",
                                row
                            ),
                            Unit = worksheet.Cells[row, 3].Text?.Trim(),
                            Deposit = ParseDecimalCell(
                                worksheet.Cells[row, 4],
                                "Tiền đặt cọc",
                                row
                            ),
                            RegistrationFee = ParseDecimalCell(
                                worksheet.Cells[row, 5],
                                "Phí đăng ký",
                                row
                            ),
                            Description = worksheet.Cells[row, 6].Text?.Trim(),
                            CreatedAt = DateTime.Now,
                            CreatedBy = userId,
                            UpdatedAt = DateTime.Now,
                            UpdatedBy = userId,
                            AuctionId = auctionId,
                        };

                        assets.Add(asset);
                    }
                }
            }

            if (assets.Count > 0)
                await _context.AuctionAssets.AddRangeAsync(assets);
        }

        private decimal ParseDecimalCell(ExcelRange cell, string columnName, int rowNumber)
        {
            if (cell?.Value == null || string.IsNullOrWhiteSpace(cell.Text))
                throw new FormatException($"Ô {columnName} ở dòng {rowNumber} đang trống.");

            // Nếu ô thực sự là số (nhờ VBA format)
            if (cell.Value is double || cell.Value is int || cell.Value is decimal)
            {
                decimal num = Convert.ToDecimal(cell.Value);
                if (num < 0)
                    throw new FormatException($"Ô {columnName} ở dòng {rowNumber} không được âm.");
                return num;
            }

            // Nếu vẫn là chuỗi (trong trường hợp không format)
            string raw = cell.Text.Trim();
            raw = raw.Replace(",", "").Replace(" ", ""); // bỏ dấu phân cách nghìn
            if (
                decimal.TryParse(
                    raw,
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out var parsed
                )
            )
            {
                if (parsed < 0)
                    throw new FormatException($"Ô {columnName} ở dòng {rowNumber} không được âm.");
                return parsed;
            }

            throw new FormatException(
                $"Ô {columnName} ở dòng {rowNumber} không phải là số hợp lệ."
            );
        }

        public async Task<byte[]> ExportRefundDocumentsExcelAsync(Guid auctionId)
        {
            var documents = await _auctionRepository.GetPaidOrDepositedDocumentsByAuctionIdAsync(
                auctionId
            );

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                // Tách danh sách
                var bankRefunds = documents
                    .Where(doc =>
                        !string.IsNullOrWhiteSpace(doc.BankAccount)
                        && !string.IsNullOrWhiteSpace(doc.BankAccountNumber)
                    )
                    .ToList();

                var cashRefunds = documents.Except(bankRefunds).ToList();

                // Ghi từng sheet
                WriteRefundSheet(package, bankRefunds, "HoanTien_ChuyenKhoan");
                WriteRefundSheet(package, cashRefunds, "HoanTien_TienMat");

                return await Task.FromResult(package.GetAsByteArray());
            }
        }

        private void WriteRefundSheet(
            ExcelPackage package,
            List<AuctionDocuments> documents,
            string sheetName
        )
        {
            var worksheet = package.Workbook.Worksheets.Add(sheetName);

            // Header
            worksheet.Cells[1, 1].Value = "STT";
            worksheet.Cells[1, 2].Value = "Mã hồ sơ";
            worksheet.Cells[1, 3].Value = "Tên khách hàng";
            worksheet.Cells[1, 4].Value = "Số CCCD";
            worksheet.Cells[1, 5].Value = "Tài khoản ngân hàng";
            worksheet.Cells[1, 6].Value = "Số tài khoản";
            worksheet.Cells[1, 7].Value = "Chi nhánh";
            worksheet.Cells[1, 8].Value = "Mã tài sản";
            worksheet.Cells[1, 9].Value = "Tên tài sản";
            worksheet.Cells[1, 10].Value = "Tiền đặt cọc";
            worksheet.Cells[1, 11].Value = "Phí đăng ký";
            worksheet.Cells[1, 12].Value = "Trạng thái phiếu hồ sơ";
            worksheet.Cells[1, 13].Value = "Trạng thái tiền cọc";
            worksheet.Cells[1, 14].Value = "Trạng thái điểm danh";
            worksheet.Cells[1, 15].Value = "Trạng thái hoàn cọc";

            int row = 2,
                stt = 1;
            foreach (var doc in documents)
            {
                worksheet.Cells[row, 1].Value = stt++;
                worksheet.Cells[row, 2].Value =
                    doc.AuctionDocumentsId != Guid.Empty ? doc.AuctionDocumentsId.ToString() : "";
                worksheet.Cells[row, 3].Value = doc.User?.Name ?? "";
                worksheet.Cells[row, 4].Value = doc.User?.CitizenIdentification ?? "";
                worksheet.Cells[row, 5].Value = doc.BankAccount;
                worksheet.Cells[row, 6].Value = doc.BankAccountNumber;
                worksheet.Cells[row, 7].Value = doc.BankBranch;
                worksheet.Cells[row, 8].Value = doc.AuctionAsset?.TagName ?? "";
                worksheet.Cells[row, 9].Value = doc.AuctionAsset?.Description ?? "";

                worksheet.Cells[row, 10].Value = doc.AuctionAsset?.Deposit ?? 0;
                worksheet.Cells[row, 10].Style.Numberformat.Format = "#,##0";

                worksheet.Cells[row, 11].Value = doc.AuctionAsset?.RegistrationFee ?? 0;
                worksheet.Cells[row, 11].Style.Numberformat.Format = "#,##0";

                worksheet.Cells[row, 12].Value = GetStatusTicketText(doc.StatusTicket);
                worksheet.Cells[row, 13].Value = GetStatusDepositText(doc.StatusDeposit);
                worksheet.Cells[row, 14].Value = GetIsAttendedText(doc.IsAttended);
                worksheet.Cells[row, 15].Value = GetStatusRefundText(doc.StatusRefund);

                row++;
            }

            // Format header bold
            using (var range = worksheet.Cells[1, 1, 1, 15])
            {
                range.Style.Font.Bold = true;
            }

            worksheet.Cells.AutoFitColumns();

            // Dropdown cho trạng thái (cột 12, 13, 14, 15)
            string[] statusTicketOptions =
            {
                "Chưa chuyển tiền",
                "Đã chuyển tiền",
                "Đã ký phiếu",
                "Đã hoàn",
                "Không xác định",
            };
            string[] statusDepositOptions =
            {
                "Chưa cọc",
                "Đã cọc",
                "Đã hoàn tiền",
                "Đã hoàn",
                "Không xác định",
            };
            string[] attendedOptions = { "Không điểm danh", "Đã điểm danh", "Không xác định" };
            string[] refundOptions =
            {
                "Không xác định",
                "Đã yêu cầu hoàn tiền cọc",
                "Chấp nhận hoàn cọc",
                "Từ chối hoàn cọc",
                "Không xác định",
            };

            for (int r = 2; r < row; r++)
            {
                var statusTicketValidation = worksheet.DataValidations.AddListValidation($"L{r}");
                foreach (var opt in statusTicketOptions)
                    statusTicketValidation.Formula.Values.Add(opt);

                var statusDepositValidation = worksheet.DataValidations.AddListValidation($"M{r}");
                foreach (var opt in statusDepositOptions)
                    statusDepositValidation.Formula.Values.Add(opt);

                var attendedValidation = worksheet.DataValidations.AddListValidation($"N{r}");
                foreach (var opt in attendedOptions)
                    attendedValidation.Formula.Values.Add(opt);

                var refundValidation = worksheet.DataValidations.AddListValidation($"O{r}");
                foreach (var opt in refundOptions)
                    refundValidation.Formula.Values.Add(opt);
            }

            // Lock toàn bộ rồi mở khoá các cột trạng thái
            worksheet.Protection.IsProtected = true;
            worksheet.Protection.SetPassword("1234");

            for (int r = 2; r < row; r++)
            {
                worksheet.Cells[r, 12].Style.Locked = false; // phiếu hồ sơ
                worksheet.Cells[r, 13].Style.Locked = false; // tiền cọc
                worksheet.Cells[r, 14].Style.Locked = false; // điểm danh
                worksheet.Cells[r, 15].Style.Locked = false; // hoàn cọc
            }

            worksheet.Cells[1, 12, 1, 15].Style.Locked = false;
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

        private string GetIsAttendedText(bool? isAttended)
        {
            return isAttended switch
            {
                true => "Đã điểm danh",
                false => "Không điểm danh",
                _ => "Không xác định",
            };
        }

        private string GetStatusRefundText(int? statusRefund)
        {
            return statusRefund switch
            {
                1 => "Đã yêu cầu hoàn tiền cọc",
                2 => "Chấp nhận hoàn cọc",
                3 => "Từ chối hoàn cọc",
                null => "Không xác định",
                _ => "Không xác định",
            };
        }
    }
}
