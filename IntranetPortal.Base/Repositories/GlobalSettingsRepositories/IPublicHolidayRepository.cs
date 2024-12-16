using IntranetPortal.Base.Models.GlobalSettingsModels;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.GlobalSettingsRepositories
{
    public interface IPublicHolidayRepository
    {
        Task<bool> AddAsync(PublicHoliday publicHoliday);
        Task<bool> DeleteAsync(int id);
        Task<bool> EditAsync(PublicHoliday publicHoliday);
        Task<List<PublicHoliday>> GetAllAsync();
        Task<PublicHoliday> GetByIdAsync(int id);
        Task<List<PublicHoliday>> GetByDateRangeAsync(DateTime StartDate, DateTime EndDate);
        Task<List<PublicHoliday>> GetByYearAsync(int year);


        Task<IList<HolidayType>> GetPublicHolidayTypesAsync();
    }
}