using MediatR;
using MyApp.Application.Interfaces.ListCustomer;

namespace MyApp.Application.CQRS.ListCustomer
{
    public class ListCustomerHandler : IRequestHandler<ListCustomerRequest, ListCustomerResponse>
    {
        private readonly IListCustomerRepository _repository;

        public ListCustomerHandler(IListCustomerRepository repository)
        {
            _repository = repository;
        }

        public async Task<ListCustomerResponse> Handle(
            ListCustomerRequest request,
            CancellationToken cancellationToken
        )
        {
            List<CustomerInfo>? customers = await _repository.ListCustomer(
                request.PageNumber,
                request.PageSize,
                request.Search
            );
            if (customers == null)
            {
                return new ListCustomerResponse
                {
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalCount = 0,
                    CustomerInfos = new List<CustomerInfo>(),
                };
            }
            return new ListCustomerResponse
            {
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = customers.Count,
                CustomerInfos = customers,
            };
        }
    }
}
