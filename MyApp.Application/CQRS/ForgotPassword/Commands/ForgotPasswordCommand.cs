using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MyApp.Application.CQRS.ForgotPassword.Enums;

namespace MyApp.Application.CQRS.ForgotPassword.Commands
{
    public class ForgotPasswordCommand : IRequest<string>
    {
        public string Contact { get; set; }
        public OTPChannel Channel { get; set; }
    }
}
