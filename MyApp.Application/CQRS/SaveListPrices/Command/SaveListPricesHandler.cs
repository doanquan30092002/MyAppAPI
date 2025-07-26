using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MyApp.Application.Common.Message;
using MyApp.Application.CQRS.SaveListPrices.Command;
using MyApp.Application.Interfaces.ISaveListPricesRepository;

namespace MyApp.Application.CQRS.SaveListPrices.Command
{
    public class SaveListPricesHandler
        : IRequestHandler<SaveListPricesRequest, SaveListPricesResponse>
    {
        private readonly ISaveListPricesRepository _saveListPricesRepository;

        public SaveListPricesHandler(ISaveListPricesRepository saveListPricesRepository)
        {
            _saveListPricesRepository =
                saveListPricesRepository
                ?? throw new ArgumentNullException(nameof(saveListPricesRepository));
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
