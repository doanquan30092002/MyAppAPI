using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Application.CQRS.UpdateAccountAndProfile.Command
{
    public class UpdateAccountResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
    }
}
