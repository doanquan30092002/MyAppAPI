using System.Threading.Tasks;
using MyApp.Application.CQRS.GetListUserWinner.Querries;

namespace MyApp.Application.Interfaces.IListUserWinnerRepository
{
    public interface IListUserWinnerRepository
    {
        Task<GetListUserWinnerResponse> GetListUserWinnerAsync(
            GetListUserWinnerRequest getListUserWinnerRequest
        );
    }
}
