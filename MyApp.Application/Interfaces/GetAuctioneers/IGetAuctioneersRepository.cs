using MyApp.Application.CQRS.GetAuctioneers.Queries;

namespace MyApp.Application.Interfaces.GetAuctioneers
{
    public interface IGetAuctioneersRepository
    {
        public Task<List<GetAuctioneersResponse>> GetAuctioneersAsync();
    }
}
