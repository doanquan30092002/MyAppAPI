namespace MyApp.Application.Interfaces.GenarateNumbericalOrder
{
    public interface IGenarateNumbericalOrderRepository
    {
        Task<bool> GenerateNumbericalOrderAsync(Guid auctionId);
    }
}
