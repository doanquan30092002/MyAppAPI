using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Core.Entities
{
    public class Role
    {
        public string Id { get; set; }
        public string RoleName { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }
    }
}
