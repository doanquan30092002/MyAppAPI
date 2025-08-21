using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyApp.Core.DTOs.LoginUserDTO;

namespace MyApp.Application.Interfaces.ILoginUserRepository
{
    public interface ITokenRepository
    {
        string CreateJWTToken(UserDTO user, string role);
    }
}
