using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyApp.Application.CQRS.ForgotPassword.Enums;

namespace MyApp.Application.Common.Services.SendMessage
{
    public interface ISendMessage
    {
        SendMessageChannel Channel { get; }

        Task<bool> SendAsync(string to, string subject, string content);
    }
}
