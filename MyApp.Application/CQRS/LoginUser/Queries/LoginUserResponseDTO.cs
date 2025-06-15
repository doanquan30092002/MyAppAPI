using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Application.CQRS.LoginUser.Queries
{
    public class LoginUserResponseDTO
    {
        public string Token { get; set; }
        public string Message { get; set; }
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string RoleName { get; set; }
    }
}
