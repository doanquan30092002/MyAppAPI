using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyApp.Core.Entities;

namespace MyApp.Application.Interfaces.IAuctionCategoriesRepository
{
    public interface IAuctionCategoriesRepository
    {
        /// <summary>
        /// Lấy ra tất cả các category đấu giá.
        /// </summary>
        /// <returns>Danh sách các AuctionCategory</returns>
        Task<List<AuctionCategory>> GetAllCategoriesAsync();

        Task<AuctionCategory?> FindByIdAsync(int categoryId);
    }
}
