using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MyApp.Application.Interfaces.ReceiveAuctionRegistrationForm;

namespace MyApp.Application.CQRS.ReceiveAuctionRegistrationForm
{
    public class ReceiveAuctionRegistrationFormHandler
        : IRequestHandler<ReceiveAuctionRegistrationFormRequest, bool>
    {
        IReceiveAuctionRegistrationFormRepository _repository;

        public ReceiveAuctionRegistrationFormHandler(
            IReceiveAuctionRegistrationFormRepository repository
        )
        {
            _repository = repository;
        }

        public async Task<bool> Handle(
            ReceiveAuctionRegistrationFormRequest request,
            CancellationToken cancellationToken
        )
        {
            var response = await _repository.UpdateStatusTicketSigned(request.AuctionDocumentsId);
            return response;
        }
    }
}
