using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace MyApp.Application.CQRS.UpdateAccountAndProfile.Command
{
    public class UpdateAccountRequest : IRequest<UpdateAccountResponse>
    {
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*[^a-zA-Z0-9]).{8,}$",
            ErrorMessage = "Mật khẩu cũ không hợp lệ"
        )]
        public string? PasswordOld { get; set; }

        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*[^a-zA-Z0-9]).{8,}$",
            ErrorMessage = "Mật khẩu mới không hợp lệ"
        )]
        public string? PasswordNew { get; set; }

        [RegularExpression(
            @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            ErrorMessage = "Email không hợp lệ"
        )]
        public string? Email { get; set; }

        [RegularExpression(
            @"^0(3[2-9]|5[6|8|9]|7[06-9]|8[1-6|8|9]|9[0-9])[0-9]{7}$",
            ErrorMessage = "Số điện thoại không hợp lệ"
        )]
        public string? PhoneNumber { get; set; }
    }
}
