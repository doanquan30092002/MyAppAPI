using MyApp.Application.CQRS.ListCustomer;

namespace MyApp.Application.Interfaces.ListCustomer
{
    public interface IListCustomerRepository
    {
        Task<List<CustomerInfo>?> ListCustomer(int pageNumber, int pageSize, string? search);
    }
}
