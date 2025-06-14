using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using MyApp.Application.CQRS.Auction.AddAuction.Commands;

namespace MyApp.Application.CQRS.Auction.UpdateAuction.Commands
{
    public class UpdateAuctionCommand : IRequest<Guid>, IValidatableObject
    {
        [Required(ErrorMessage = "AuctionId là bắt buộc.")]
        public Guid AuctionId { get; set; }

        [Required(ErrorMessage = "Tên phiên đấu giá là bắt buộc.")]
        public string AuctionName { get; set; }

        [Required(ErrorMessage = "Mô tả là bắt buộc.")]
        public string AuctionDescription { get; set; }
        public IFormFile? AuctionRulesFile { get; set; }
        public IFormFile? AuctionPlanningMap { get; set; }

        [Required(ErrorMessage = "Ngày mở đăng ký là bắt buộc.")]
        public DateTime RegisterOpenDate { get; set; }
        public DateTime RegisterEndDate { get; set; }

        [Required(ErrorMessage = "Ngày bắt đầu đấu giá là bắt buộc.")]
        public DateTime AuctionStartDate { get; set; }

        [Required(ErrorMessage = "Ngày kết thúc đấu giá là bắt buộc.")]
        public DateTime AuctionEndDate { get; set; }
        public string? Auction_Map { get; set; }

        [Range(1, 5, ErrorMessage = "Số vòng tối đa phải từ 1 đến 5.")]
        public int NumberRoundMax { get; set; }
        public bool Status { get; set; } = false;
        public string? WinnerData { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "CategoryId là bắt buộc.")]
        public int CategoryId { get; set; }
        public IFormFile? AuctionAssetFile { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (AuctionStartDate >= AuctionEndDate)
            {
                yield return new ValidationResult(
                    "Ngày bắt đầu đấu giá phải trước ngày kết thúc đấu giá.",
                    new[] { "AuctionStartDate", "AuctionEndDate" }
                );
            }

            if ((AuctionStartDate - RegisterOpenDate).TotalDays < 7)
            {
                yield return new ValidationResult(
                    "Ngày mở đăng ký phải trước ngày bắt đầu đấu giá ít nhất 7 ngày.",
                    new[] { "RegisterOpenDate", "AuctionStartDate" }
                );
            }

            if (RegisterEndDate >= AuctionStartDate)
            {
                yield return new ValidationResult(
                    "Ngày kết thúc đăng ký phải trước ngày bắt đầu đấu giá.",
                    new[] { "RegisterEndDate", "AuctionStartDate" }
                );
            }
        }
    }
}
