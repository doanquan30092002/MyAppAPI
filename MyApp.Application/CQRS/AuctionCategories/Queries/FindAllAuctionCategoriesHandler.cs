using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MyApp.Application.Interfaces.IAuctionCategoriesRepository;
using MyApp.Core.Entities;

namespace MyApp.Application.CQRS.AuctionCategories.Queries
{
    public class FindAllAuctionCategoriesQuery : IRequest<List<AuctionCategory>> { }

    public class FindAllAuctionCategoriesHandler
        : IRequestHandler<FindAllAuctionCategoriesQuery, List<AuctionCategory>>
    {
        private readonly IAuctionCategoriesRepository _auctionCategoriesRepository;

        public FindAllAuctionCategoriesHandler(
            IAuctionCategoriesRepository auctionCategoriesRepository
        )
        {
            _auctionCategoriesRepository = auctionCategoriesRepository;
        }

        public async Task<List<AuctionCategory>> Handle(
            FindAllAuctionCategoriesQuery request,
            CancellationToken cancellationToken
        )
        {
            return await _auctionCategoriesRepository.GetAllCategoriesAsync();
        }
    }
}
