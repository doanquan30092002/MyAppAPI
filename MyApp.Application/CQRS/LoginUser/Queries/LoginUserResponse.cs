using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Application.CQRS.LoginUser.Queries
{
    public class LoginUserResponse
    {
        public string Token { get; set; }
        public string Message { get; set; }
    }
}
