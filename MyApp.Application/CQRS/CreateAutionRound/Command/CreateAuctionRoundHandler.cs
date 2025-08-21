using System.ComponentModel.DataAnnotations;
using System.Data;
using MediatR;
using MyApp.Application.Common.Message;
using MyApp.Application.Interfaces.ICreateAuctionRoundRepository;

namespace MyApp.Application.CQRS.CreateAutionRound.Command
{
    public class CreateAuctionRoundHandler
        : IRequestHandler<CreateAuctionRoundRequest, CreateAuctionRoundResponse>
    {
        private readonly ICreateAuctionRoundRepository _createAuctionRoundRepository;

        public CreateAuctionRoundHandler(ICreateAuctionRoundRepository createAuctionRoundRepository)
        {
            _createAuctionRoundRepository = createAuctionRoundRepository;
        }

        public async Task<CreateAuctionRoundResponse> Handle(
            CreateAuctionRoundRequest request,
            CancellationToken cancellationToken
        )
        {
            var isSuccess = await _createAuctionRoundRepository.InsertAuctionRound(request);

            if (!isSuccess)
            {
                throw new ValidationException(Message.CREATE_FAIL);
            }

            return new CreateAuctionRoundResponse { };
        }
    }
}
