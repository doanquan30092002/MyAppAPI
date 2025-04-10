using AutoMapper;
using MyApp.Core.DTOs;
using MyApp.Core.Entities;

namespace MyApp.Core.Mapper
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDTO>();
        }
    }
}
