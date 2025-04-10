using System;
using MyApp.Core.DTOs;

namespace MyApp.Application.CQRS.TestFilterSortPage.Queries.GetAllUser
{
    public class UserResponse
    {
        List<UserDTO> userRes { get; set; }
    }
}
