namespace MyApp.Application.Interfaces.ChangeStatusAuctionRound
{
    public interface IChangeStatusAuctionRoundRepository
    {
        Task<bool> ChangeStatusAuctionRoundAsync(Guid auctionRoundId, int status);
    }
}
