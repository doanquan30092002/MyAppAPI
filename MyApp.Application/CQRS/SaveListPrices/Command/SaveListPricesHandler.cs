using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using MyApp.Application.Common.Message;
using MyApp.Application.Common.Services.AuctionRoundPriceHub;
using MyApp.Application.CQRS.SaveListPrices.Command;
using MyApp.Application.Interfaces.IGetListEnteredPricesRepository;
using MyApp.Application.Interfaces.ISaveListPricesRepository;
using MyApp.Core.Entities;

namespace MyApp.Application.CQRS.SaveListPrices.Command
{
    public class SaveListPricesHandler
        : IRequestHandler<SaveListPricesRequest, SaveListPricesResponse>
    {
        private readonly ISaveListPricesRepository _saveListPricesRepository;
        private readonly IHubContext<AuctionRoundPriceHub> _hubContext;
        private readonly IGetListEnteredPricesRepository _getListEnteredPricesRepository;

        public SaveListPricesHandler(
            ISaveListPricesRepository saveListPricesRepository,
            IHubContext<AuctionRoundPriceHub> hubContext,
            IGetListEnteredPricesRepository getListEnteredPricesRepository
        )
        {
            _saveListPricesRepository = saveListPricesRepository;
            _hubContext = hubContext;
            _getListEnteredPricesRepository = getListEnteredPricesRepository;
        }

        public async Task<SaveListPricesResponse> Handle(
            SaveListPricesRequest request,
            CancellationToken cancellationToken
        )
        {
            try
            {
                var isInserted = await _saveListPricesRepository.InserListPrices(request);
                if (!isInserted)
                {
                    throw new ValidationException(Message.SAVE_LIST_PRICES_FAIL);
                }

                var latestList = _getListEnteredPricesRepository.GetListEnteredPricesAsync(
                    request.AuctionRoundId
                );
                await _hubContext
                    .Clients.Group(request.AuctionRoundId.ToString())
                    .SendAsync("ReceiveLatestPrices", latestList);

                return new SaveListPricesResponse { };
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception)
            {
                throw new ValidationException(Message.SAVE_LIST_PRICES_FAIL);
            }
        }
    }
}
