using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace MyApp.Application.CQRS.SearchUserAttendance.Queries
{
    public class SearchUserAttendanceRequest : IRequest<SearchUserAttendanceResponseDTO>
    {
        public Guid AuctionId { get; set; }
        public string CitizenIdentification { get; set; }
    }
}
