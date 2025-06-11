using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.Interfaces.IAuctionCategoriesRepository;
using MyApp.Core.Entities;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories.AuctionCategoriesRepository
{
    public class AuctionCategoriesRepository : IAuctionCategoriesRepository
    {
        private readonly AppDbContext _context;

        public AuctionCategoriesRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AuctionCategory?> FindByIdAsync(int categoryId)
        {
            return await _context.AuctionCategories.FirstOrDefaultAsync(x =>
                x.CategoryId == categoryId
            );
        }

        public async Task<List<AuctionCategory>> GetAllCategoriesAsync()
        {
            return await _context.AuctionCategories.ToListAsync();
        }
    }
}
