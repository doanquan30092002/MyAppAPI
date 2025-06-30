using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MyApp.Application.CQRS.UpdateAccount.Command.SendUpdateOtp;

namespace MyApp.Application.CQRS.UpdateAccount.Command.VerifyAndUpdate
{
    public class VerifyAndUpdateRequest : IRequest<UpdateAccountResponse>
    {
        public string OtpCode { get; set; }
    }
}
