using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace MyApp.Application.CQRS.AuctionDocuments.SupportRegisterDocuments.Command
{
    public class SupportRegisterDocumentsCommand : IRequest<bool>
    {
        /// <summary>
        /// Số căn cước công dân (CMND/CCCD) của người đăng ký
        /// </summary>
        [Required(ErrorMessage = "Số căn cước công dân là bắt buộc.")]
        public string CitizenIdentification { get; set; }

        /// <summary>
        /// Danh sách Id các tài sản đấu giá mà người dùng muốn đăng ký
        /// </summary>
        [Required(ErrorMessage = "Danh sách tài sản đấu giá là bắt buộc.")]
        public List<Guid> AuctionAssetsIds { get; set; }

        /// <summary>
        /// Tên chủ tài khoản ngân hàng
        /// </summary>
        [Required(ErrorMessage = "Tên chủ tài khoản là bắt buộc.")]
        public string BankAccount { get; set; }

        /// <summary>
        /// Số tài khoản ngân hàng
        /// </summary>
        [Required(ErrorMessage = "Số tài khoản ngân hàng là bắt buộc.")]
        public string BankAccountNumber { get; set; }

        /// <summary>
        /// Chi nhánh ngân hàng
        /// </summary>
        [Required(ErrorMessage = "Chi nhánh ngân hàng là bắt buộc.")]
        public string BankBranch { get; set; }

        [Required(ErrorMessage = "Phiên đấu giá là bắt buộc.")]
        public Guid AuctionId { get; set; }
    }
}
