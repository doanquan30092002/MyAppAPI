using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace MyApp.Application.CQRS.UpdateExpiredProfile.Command
{
    public class UpdateExpiredProfileRequest : IRequest<UpdateExpiredProfileResponse>
    {
        public string CitizenIdentification { get; set; }
        public string Name { get; set; }
        public DateTime BirthDay { get; set; }
        public string Nationality { get; set; }
        public bool Gender { get; set; }
        public DateTime ValidDate { get; set; }
        public string OriginLocation { get; set; }
        public string RecentLocation { get; set; }
        public DateTime IssueDate { get; set; }
        public string IssueBy { get; set; }
    }
}
