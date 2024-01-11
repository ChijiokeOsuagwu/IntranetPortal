using IntranetPortal.Base.Enums;
using IntranetPortal.Base.Models.EmployeeRecordModels;
using IntranetPortal.Base.Models.PmsModels;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.PmsRepositories
{
    public interface ISessionScheduleRepository
    {
        IConfiguration _config { get; }

        Task<bool> AddAsync(SessionSchedule sessionSchedule);
        Task<bool> CancelAsync(int sessionScheduleId, string cancelledBy);
        Task<bool> DeleteAsync(int sessionScheduleId);
        Task<IList<SessionSchedule>> GetAllAsync(int reviewSessionId);
        Task<IList<SessionSchedule>> GetByDepartmentCodeAsync(int reviewSessionId, int departmentId);
        Task<IList<SessionSchedule>> GetByDepartmentCodeAsync(int reviewSessionId, int departmentId, SessionActivityType activityType);
        Task<IList<SessionSchedule>> GetByEmployeeCardinalsAsync(int reviewSessionId, SessionActivityType activityType, EmployeeCardinal employeeCardinal);
        Task<IList<SessionSchedule>> GetByIdAsync(int sessionScheduleId);
        Task<IList<SessionSchedule>> GetByLocationIdAsync(int reviewSessionId, int locationId);
        Task<IList<SessionSchedule>> GetByLocationIdAsync(int reviewSessionId, int locationId, SessionActivityType activityType);
        Task<IList<SessionSchedule>> GetByReviewSessionIdAsync(int reviewSessionId);
        Task<IList<SessionSchedule>> GetByReviewSessionIdAsync(int reviewSessionId, SessionActivityType activityType);
        Task<IList<SessionSchedule>> GetByTypeAsync(int reviewSessionId, SessionScheduleType scheduleType);
        Task<IList<SessionSchedule>> GetByUnitCodeAsync(int reviewSessionId, int unitId);
        Task<IList<SessionSchedule>> GetByUnitCodeAsync(int reviewSessionId, int unitId, SessionActivityType activityType);
        Task<List<SessionActivityType>> GetForAllAsync(int reviewSessionId);
        Task<List<SessionActivityType>> GetForDepartmentAsync(int reviewSessionId, int departmentId);
        Task<List<SessionActivityType>> GetForEmployeeAsync(int reviewSessionId, string employeeId);
        Task<List<SessionActivityType>> GetForLocationAsync(int reviewSessionId, int locationId);
        Task<List<SessionActivityType>> GetForUnitAsync(int reviewSessionId, int unitId);
    }
}