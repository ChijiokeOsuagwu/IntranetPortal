using IntranetPortal.Base.Models.GlobalSettingsModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.GlobalSettingsRepositories
{
    public interface ICompanyRepository
    {
        Task<Company> GetCompanyByCodeAsync(string companyCode);
        Task<IList<Company>> GetCompaniesAsync();
    }
}
