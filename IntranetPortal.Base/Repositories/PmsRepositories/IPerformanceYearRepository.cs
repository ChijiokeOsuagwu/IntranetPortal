using IntranetPortal.Base.Models.PmsModels;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.PmsRepositories
{
    public interface IPerformanceYearRepository
    {
        IConfiguration _config { get; }

        Task<bool> AddAsync(PerformanceYear performanceYear);
        Task<bool> DeleteAsync(int performanceYearId);
        Task<IList<PerformanceYear>> GetAllAsync();
        Task<IList<PerformanceYear>> GetByIdAsync(int performanceYearId);
        Task<IList<PerformanceYear>> GetByNameAsync(string performanceYearName);
        Task<IList<PerformanceYear>> GetByOverlappingDatesAsync(DateTime startDate, DateTime endDate);
        Task<bool> UpdateAsync(PerformanceYear performanceYear);
    }
}