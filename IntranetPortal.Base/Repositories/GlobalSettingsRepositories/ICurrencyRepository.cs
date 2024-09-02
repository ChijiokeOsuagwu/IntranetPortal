using IntranetPortal.Base.Models.GlobalSettingsModels;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.GlobalSettingsRepositories
{
    public interface ICurrencyRepository
    {
        IConfiguration _config { get; }

        Task<Currency> GetByCodeAsync(string code);
        Task<IList<Currency>> GetCurrenciesAsync();
    }
}