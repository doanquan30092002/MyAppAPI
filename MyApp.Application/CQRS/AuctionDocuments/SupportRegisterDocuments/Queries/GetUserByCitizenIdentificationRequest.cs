using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MyApp.Core.DTOs.UserDTO;
using MyApp.Core.Entities;

namespace MyApp.Application.CQRS.AuctionDocuments.SupportRegisterDocuments.Queries
{
    public class GetUserByCitizenIdentificationRequest
        : IRequest<GetUserByCitizenIdentificationResponse?>
    {
        public string CitizenIdentification { get; set; }

        public GetUserByCitizenIdentificationRequest(string citizenIdentification)
        {
            CitizenIdentification = citizenIdentification;
        }
    }
}
