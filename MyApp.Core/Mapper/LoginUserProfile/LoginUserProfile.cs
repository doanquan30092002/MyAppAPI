using AutoMapper;
using MyApp.Core.DTOs.LoginUserDTO;
using MyApp.Core.Entities;

namespace MyApp.Core.Mapper.LoginUserProfile
{
    public class LoginUserProfile : Profile
    {
        public LoginUserProfile()
        {
            CreateMap<Account, AccountDTO>();
            CreateMap<User, UserDTO>();
        }
    }
}
