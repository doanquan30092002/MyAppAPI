namespace MyApp.Application.Interfaces.SearchUserAttendance
{
    public interface ISearchUserAttendanceRepository
    {
        Task<int?> GetNumericalOrderAsync(Guid auctionId, string citizenIdentification);
        Task<string?> GetAuctionNameAsync(Guid auctionId);
    }
}
