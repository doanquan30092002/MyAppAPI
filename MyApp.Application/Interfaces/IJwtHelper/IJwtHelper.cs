using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MyApp.Application.Interfaces.IJwtHelper
{
    public interface IJwtHelper
    {
        Guid? GetUserIdFromToken(string jwtToken);

        Guid? GetUserIdFromHttpContext(HttpContext httpContext);
    }
}
