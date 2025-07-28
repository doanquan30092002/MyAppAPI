using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.CQRS.SaveListPrices.Command;
using MyApp.Application.Interfaces.ISaveListPricesRepository;
using MyApp.Core.Entities;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories.SaveListPricesRepository
{
    public class SaveListPricesRepository : ISaveListPricesRepository
    {
        private readonly AppDbContext context;

        public SaveListPricesRepository(AppDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> InserListPrices(SaveListPricesRequest saveListPricesRequest)
        {
            try
            {
                var auctionRoundPrices = saveListPricesRequest
                    .resultDTOs.Select(dto => new AuctionRoundPrices
                    {
                        AuctionRoundPriceId = Guid.NewGuid(),
                        AuctionRoundId = saveListPricesRequest.AuctionRoundId,
                        UserName = dto.UserName,
                        CitizenIdentification = dto.CitizenIdentification,
                        RecentLocation = dto.RecentLocation,
                        TagName = dto.TagName,
                        AuctionPrice = dto.AuctionPrice,
                        CreatedBy = dto.CreatedBy,
                        CreatedAt = DateTime.UtcNow,
                        FlagWinner = false,
                    })
                    .ToList();

                await context.AuctionRoundPrices.AddRangeAsync(auctionRoundPrices);
                var result = await context.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
