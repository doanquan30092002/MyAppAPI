using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyApp.Core.Entities;

namespace MyApp.Core.DTOs.UserDTO
{
    public class GetUserByCitizenIdentificationResponse : User
    {
        public string? PhoneNumber { get; set; }
    }
}
