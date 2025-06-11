using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MyApp.Application.Interfaces.IJwtHelper;

namespace MyApp.Application.Common.Services.JwtHelper
{
    public class JwtHelper : IJwtHelper
    {
        public Guid? GetUserIdFromHttpContext(HttpContext httpContext)
        {
            if (httpContext == null)
                return null;

            var authHeader = httpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                return null;

            var token = authHeader.Substring("Bearer ".Length);
            return GetUserIdFromToken(token);
        }

        public Guid? GetUserIdFromToken(string jwtToken)
        {
            if (string.IsNullOrEmpty(jwtToken))
                return null;

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(jwtToken);

                var userDataClaim = token.Claims.FirstOrDefault(c =>
                    c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/userdata"
                );

                if (userDataClaim != null)
                {
                    using var doc = JsonDocument.Parse(userDataClaim.Value);
                    if (doc.RootElement.TryGetProperty("Id", out var idElement))
                    {
                        if (Guid.TryParse(idElement.GetString(), out var userId))
                            return userId;
                    }
                }

                var userIdClaim = token.Claims.FirstOrDefault(c =>
                    c.Type == "sub" || c.Type == "Id" || c.Type == ClaimTypes.NameIdentifier
                );

                if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var fallbackUserId))
                    return fallbackUserId;

                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
