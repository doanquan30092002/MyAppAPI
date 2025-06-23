using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Application.CQRS.UpdateExpiredProfile.Command
{
    public class UpdateExpiredProfileResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
    }
}
