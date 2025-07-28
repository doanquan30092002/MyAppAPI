using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MyApp.Application.CQRS.SignUp.SignUpUser.Command;

namespace MyApp.Application.CQRS.SaveListPrices.Command
{
    public class SaveListPricesRequest : IRequest<SaveListPricesResponse>
    {
        public Guid AuctionRoundId { get; set; }
        public List<ResultDTO> resultDTOs { get; set; }
    }

    public class ResultDTO
    {
        public string UserName { get; set; }
        public string CitizenIdentification { get; set; }
        public string RecentLocation { get; set; }
        public string TagName { get; set; }
        public decimal AuctionPrice { get; set; }
        public Guid CreatedBy { get; set; }
    }
}
