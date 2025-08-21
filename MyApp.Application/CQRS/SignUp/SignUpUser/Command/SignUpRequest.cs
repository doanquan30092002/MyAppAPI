using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MyApp.Core.Entities;

namespace MyApp.Application.CQRS.SignUp.SignUpUser.Command
{
    public class SignUpRequest : IRequest<SignUpResponse>
    {
        public string CitizenIdentification { get; set; }
        public string Name { get; set; }
        public DateTime BirthDay { get; set; }
        public string Nationality { get; set; }
        public bool Gender { get; set; }
        public DateTime ValidDate { get; set; }
        public string OriginLocation { get; set; }
        public string RecentLocation { get; set; }
        public DateTime IssueDate { get; set; }
        public string IssueBy { get; set; }

        [RegularExpression(
            @"^0(3[2-9]|5[6|8|9]|7[06-9]|8[1-6|8|9]|9[0-9])[0-9]{7}$",
            ErrorMessage = "Số điện thoại không hợp lệ"
        )]
        public string PhoneNumber { get; set; }

        [RegularExpression(
            @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            ErrorMessage = "Email không hợp lệ"
        )]
        public string Email { get; set; }

        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*[^a-zA-Z0-9]).{8,}$",
            ErrorMessage = "Mật khẩu không hợp lệ"
        )]
        public string Password { get; set; }

        public int RoleId { get; set; }
    }
}
