using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MyApp.Core.Entities;

namespace MyApp.Application.CQRS.AuctionDocuments.SupportRegisterDocuments.Queries
{
    public class GetUserByCitizenIdentificationRequest : IRequest<User?>
    {
        public string CitizenIdentification { get; set; }

        public GetUserByCitizenIdentificationRequest(string citizenIdentification)
        {
            CitizenIdentification = citizenIdentification;
        }
    }
}
