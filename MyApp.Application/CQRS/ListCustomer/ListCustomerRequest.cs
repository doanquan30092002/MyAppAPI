using MediatR;

namespace MyApp.Application.CQRS.ListCustomer
{
    public class ListCustomerRequest : IRequest<ListCustomerResponse>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Search { get; set; }
    }
}
