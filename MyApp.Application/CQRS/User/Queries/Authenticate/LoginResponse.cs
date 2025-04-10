using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Application.CQRS.User.Queries.Authenticate
{
    public class LoginResponse
    {
        public string Token { get; set; }
    }
}
