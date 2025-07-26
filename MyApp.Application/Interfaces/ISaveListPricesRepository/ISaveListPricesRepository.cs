using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyApp.Application.CQRS.SaveListPrices.Command;
using MyApp.Application.CQRS.SignUp.SignUpUser.Command;

namespace MyApp.Application.Interfaces.ISaveListPricesRepository
{
    public interface ISaveListPricesRepository
    {
        public Task<bool> InserListPrices(SaveListPricesRequest saveListPricesRequest);
    }
}
