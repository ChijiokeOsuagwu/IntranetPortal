using IntranetPortal.Base.Models.GlobalSettingsModels;
using IntranetPortal.Base.Models.LmsModels;
using IntranetPortal.Base.Repositories.ErmRepositories;
using IntranetPortal.Base.Repositories.GlobalSettingsRepositories;
using IntranetPortal.Base.Repositories.LmsRepositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Services
{
    public class LmsService : ILmsService
    {
        private readonly ILeaveProfileRepository _leaveProfileRepository;
        private readonly ILeaveTypesRepository _leaveTypesRepository;
        private readonly IPublicHolidayRepository _publicHolidayRepository;
        private readonly ILeaveProfileDetailRepository _leaveProfileDetailRepository;
        private readonly IEmployeeLeaveRepository _employeeLeaveRepository;
        private readonly IEmployeesRepository _employeesRepository;

        public LmsService(ILeaveTypesRepository leaveTypesRepository, ILeaveProfileRepository leaveProfileRepository,
            IPublicHolidayRepository publicHolidayRepository, ILeaveProfileDetailRepository leaveProfileDetailRepository,
            IEmployeeLeaveRepository employeeLeaveRepository, IEmployeesRepository employeesRepository)
        {
            _leaveProfileRepository = leaveProfileRepository;
            _leaveTypesRepository = leaveTypesRepository;
            _publicHolidayRepository = publicHolidayRepository;
            _leaveProfileDetailRepository = leaveProfileDetailRepository;
            _employeeLeaveRepository = employeeLeaveRepository;
            _employeesRepository = employeesRepository;
        }

        #region Leave Types Service Methods
        public async Task<List<LeaveType>> GetLeaveTypes()
        {
            return await _leaveTypesRepository.GetAllAsync();
        }
        public async Task<LeaveType> GetLeaveType(string LeaveTypeCode)
        {
            LeaveType leaveType = new LeaveType();
            if (!string.IsNullOrWhiteSpace(LeaveTypeCode))
            {
                leaveType = await _leaveTypesRepository.GetByCodeAsync(LeaveTypeCode);
            }
            return leaveType;
        }
        public async Task<LeaveType> GetLeaveTypeByName(string Name)
        {
            LeaveType leaveType = new LeaveType();
            if (!string.IsNullOrWhiteSpace(Name))
            {
                leaveType = await _leaveTypesRepository.GetByNameAsync(Name);
            }
            return leaveType;
        }

        public async Task<bool> CreateLeaveType(LeaveType leaveType)
        {
            bool IsCreated = false;
            if (leaveType != null)
            {
                var sameCodeLeaveType = await _leaveTypesRepository.GetByCodeAsync(leaveType.Code);
                if (sameCodeLeaveType == null || string.IsNullOrWhiteSpace(sameCodeLeaveType.Name))
                {
                    var sameNameLeaveType = await _leaveTypesRepository.GetByNameAsync(leaveType.Name);
                    if (sameNameLeaveType == null || string.IsNullOrWhiteSpace(sameNameLeaveType.Name))
                    {
                        IsCreated = await _leaveTypesRepository.AddAsync(leaveType);
                    }
                    else { throw new Exception("A Leave Type with the same Name already exists."); }
                }
                else { throw new Exception("A Leave Type with the same Code already exists."); }
            }
            else { throw new Exception("Leave Type cannot be null."); }

            return IsCreated;
        }

        public async Task<bool> UpdateLeaveType(LeaveType leaveType)
        {
            bool IsUpdated;
            if (leaveType != null)
            {
                var sameNameLeaveType = await _leaveTypesRepository.GetByNameAsync(leaveType.Name);
                if (sameNameLeaveType == null || string.IsNullOrWhiteSpace(sameNameLeaveType.Code) || sameNameLeaveType.Code == leaveType.Code)
                {
                    IsUpdated = await _leaveTypesRepository.EditAsync(leaveType);
                }
                else { throw new Exception("A Leave Type with the same Name already exists."); }
            }
            else { throw new Exception("Required parameter Leave Type must not be null."); }

            return IsUpdated;
        }

        public async Task<bool> DeleteLeaveType(string code)
        {
            bool IsDeleted = false;
            if (!string.IsNullOrWhiteSpace(code))
            {
                IsDeleted = await _leaveTypesRepository.DeleteAsync(code);
            }
            else { throw new Exception("Required parameter Code cannot be null."); }
            return IsDeleted;
        }

        #endregion

        #region Leave Profiles Service Methods
        public async Task<List<LeaveProfile>> GetLeaveProfiles()
        {
            return await _leaveProfileRepository.GetAllAsync();
        }
        public async Task<LeaveProfile> GetLeaveProfile(int Id)
        {
            LeaveProfile leaveProfile = new LeaveProfile();
            if (Id > 0)
            {
                leaveProfile = await _leaveProfileRepository.GetByIdAsync(Id);
            }
            return leaveProfile;
        }
        public async Task<LeaveProfile> GetLeaveProfile(string Name)
        {
            LeaveProfile leaveProfile = new LeaveProfile();
            if (!string.IsNullOrWhiteSpace(Name))
            {
                leaveProfile = await _leaveProfileRepository.GetByNameAsync(Name);
            }
            return leaveProfile;
        }
        public async Task<bool> CreateLeaveProfile(LeaveProfile leaveProfile)
        {
            bool IsSuccessful;
            if (leaveProfile != null)
            {
                var sameNameLeaveProfile = await _leaveProfileRepository.GetByNameAsync(leaveProfile.Name);
                if (sameNameLeaveProfile == null || sameNameLeaveProfile.Id < 1)
                {
                    IsSuccessful = await _leaveProfileRepository.AddAsync(leaveProfile);
                }
                else { throw new Exception("A Leave Profile with the same Name already exists."); }
            }
            else { throw new Exception("Require parameter [Leave Profile] cannot be null."); }

            return IsSuccessful;
        }
        public async Task<bool> UpdateLeaveProfile(LeaveProfile leaveProfile)
        {
            bool IsUpdated;
            if (leaveProfile != null)
            {
                var sameNameLeaveProfile = await _leaveProfileRepository.GetByNameAsync(leaveProfile.Name);
                if (sameNameLeaveProfile == null || sameNameLeaveProfile.Id < 1 || sameNameLeaveProfile.Id == leaveProfile.Id)
                {
                    IsUpdated = await _leaveProfileRepository.EditAsync(leaveProfile);
                }
                else { throw new Exception("A Leave Profile with the same Name already exists."); }
            }
            else { throw new Exception("Required parameter Leave Profile cannot be null."); }

            return IsUpdated;
        }
        public async Task<bool> DeleteLeaveProfile(int Id)
        {
            bool IsDeleted;
            if (Id > 0)
            {
                var entities = await _leaveProfileDetailRepository.GetByProfileIdAsync(Id);
                if(entities == null || entities.Count < 1)
                {
                    var employees = await _employeesRepository.GetEmployeesByLeaveProfileIdAsync(Id);
                    if(employees == null || employees.Count < 1)
                    {
                        IsDeleted = await _leaveProfileRepository.DeleteAsync(Id);
                    }
                    else { throw new Exception("This Leave Profile cannot be deleted because it is linked to some employee records."); }
                }
                else { throw new Exception("This Leave Profile cannot be deleted because it contains some profile options records."); }
            }
            else { throw new Exception("Required parameter ID cannot be null."); }
            return IsDeleted;
        }
        #endregion

        #region Public Holiday Service Methods
        public async Task<List<PublicHoliday>> GetPublicHolidays(int year)
        {
            List<PublicHoliday> holidays = new List<PublicHoliday>();
            if (year > 0)
            {
                holidays = await _publicHolidayRepository.GetByYearAsync(year);
            }
            return holidays;
        }
        public async Task<PublicHoliday> GetPublicHoliday(int Id)
        {
            PublicHoliday holiday = new PublicHoliday();
            if (Id > 0)
            {
                holiday = await _publicHolidayRepository.GetByIdAsync(Id);
            }
            return holiday;
        }
        public async Task<bool> CreatePublicHoliday(PublicHoliday holiday)
        {
            bool IsCreated;
            if (holiday != null && holiday.StartDate != null && holiday.EndDate != null)
            {
                List<PublicHoliday> conflictingHolidays = await _publicHolidayRepository.GetByDateRangeAsync(holiday.StartDate, holiday.EndDate);
                if (conflictingHolidays == null || conflictingHolidays.Count < 1)
                {
                    IsCreated = await _publicHolidayRepository.AddAsync(holiday);
                }
                else { throw new Exception("Another public holiday falls within the selected dates."); }
            }
            else { throw new Exception("Required parameter Public Holiday cannot be null."); }

            return IsCreated;
        }
        public async Task<bool> UpdatePublicHoliday(PublicHoliday holiday)
        {
            bool IsUpdated;
            if (holiday != null && holiday.StartDate != null && holiday.EndDate != null)
            {
                List<PublicHoliday> conflictingHolidays = await _publicHolidayRepository.GetByDateRangeAsync(holiday.StartDate, holiday.EndDate);
                if (conflictingHolidays != null && conflictingHolidays.Count > 0)
                {
                    foreach (var item in conflictingHolidays)
                    {
                        if (item.Id != holiday.Id) { throw new Exception("Sorry, another public holiday falls within the selected dates."); }
                    }
                }
                IsUpdated = await _publicHolidayRepository.EditAsync(holiday);
            }
            else { throw new Exception("Required parameter Public Holiday cannot be null."); }

            return IsUpdated;
        }
        public async Task<bool> DeletePublicHoliday(int Id)
        {
            bool IsDeleted;
            if (Id > 0)
            {
                IsDeleted = await _publicHolidayRepository.DeleteAsync(Id);
            }
            else { throw new Exception("Required parameter ID cannot be null."); }
            return IsDeleted;
        }
        #endregion

        #region Leave Profile Details Service Methods
        public async Task<List<LeaveProfileDetail>> GetLeaveProfileDetails(int LeaveProfileId)
        {
            List<LeaveProfileDetail> leaveProfileDetails = new List<LeaveProfileDetail>();
            if (LeaveProfileId > 0)
            {
                leaveProfileDetails = await _leaveProfileDetailRepository.GetByProfileIdAsync(LeaveProfileId);
            }
            return leaveProfileDetails;
        }
        public async Task<List<LeaveProfileDetail>> GetLeaveProfileDetails(int LeaveProfileId, string LeaveTypeCode)
        {
            List<LeaveProfileDetail> leaveProfileDetails = new List<LeaveProfileDetail>();
            if (LeaveProfileId > 0 && !string.IsNullOrWhiteSpace(LeaveTypeCode))
            {
                leaveProfileDetails = await _leaveProfileDetailRepository.GetByProfileIdnLeaveTypeAsync(LeaveProfileId, LeaveTypeCode);
            }
            return leaveProfileDetails;
        }
        public async Task<LeaveProfileDetail> GetLeaveProfileDetail(int Id)
        {
            LeaveProfileDetail leaveProfileDetail = new LeaveProfileDetail();
            if (Id > 0)
            {
                var entities = await _leaveProfileDetailRepository.GetByIdAsync(Id);
                if (entities != null && entities.Count == 1)
                {
                    leaveProfileDetail = entities[0];
                }
            }
            return leaveProfileDetail;
        }
        public async Task<bool> CreateLeaveProfileDetail(LeaveProfileDetail leaveProfileDetail)
        {
            bool IsSuccessful;
            if (leaveProfileDetail != null)
            {
                var existingLeaveProfileDetails = await _leaveProfileDetailRepository.GetByProfileIdnLeaveTypeAsync(leaveProfileDetail.ProfileId, leaveProfileDetail.LeaveTypeCode);
                if (existingLeaveProfileDetails != null && existingLeaveProfileDetails.Count > 0)
                { throw new Exception("Duplicate Entry. This Leave Type has already been set up for this Profile."); }
                else
                {
                    IsSuccessful = await _leaveProfileDetailRepository.AddAsync(leaveProfileDetail);
                }
            }
            else { throw new Exception("Required parameter [Leave Profile Detail] cannot be null."); }

            return IsSuccessful;
        }
        public async Task<bool> UpdateLeaveProfileDetail(LeaveProfileDetail d)
        {
            bool IsUpdated;
            if (d != null)
            {
                IsUpdated = await _leaveProfileDetailRepository.EditAsync(d);
            }
            else { throw new Exception("Required parameter [Leave Profile Detail] cannot be null."); }

            return IsUpdated;
        }
        public async Task<bool> DeleteLeaveProfileDetail(int Id)
        {
            bool IsDeleted;
            if (Id > 0)
            {
                IsDeleted = await _leaveProfileDetailRepository.DeleteAsync(Id);
            }
            else { throw new Exception("Required parameter ID cannot be null."); }
            return IsDeleted;
        }
        #endregion

        #region Leave Plan  Service Methods
        public async Task<List<EmployeeLeave>> GetLeavePlans(string EmployeeId, int LeaveYear)
        {
            List<EmployeeLeave> leaveList = new List<EmployeeLeave>();
            if (!string.IsNullOrWhiteSpace(EmployeeId) && LeaveYear > 0)
            {
                leaveList = await _employeeLeaveRepository.GetByEmployeeIdnYearAsync(EmployeeId, LeaveYear, true);
            }
            return leaveList;
        }
        public async Task<EmployeeLeave> GetLeavePlan(long Id)
        {
            EmployeeLeave p = new EmployeeLeave();
            if (Id > 0)
            {
                p = await _employeeLeaveRepository.GetByIdAsync(Id);
            }
            return p;
        }
        public async Task<bool> CreateLeavePlan(EmployeeLeave e)
        {
            bool IsSuccessful;
            if (e != null)
            {
                IsSuccessful = await _employeeLeaveRepository.AddAsync(e);
            }
            else { throw new Exception("Required parameter [Leave Plan] cannot be null."); }

            return IsSuccessful;
        }

        #endregion

        #region Leave Helper Service Methods
        public DateTime GenerateLeaveEndDate(DateTime StartDate, int DurationTypeId, int Duration)
        {
            DateTime endDate;
            switch (DurationTypeId)
            {
                case 1:
                    endDate = StartDate.AddDays(Duration - 1);
                    break;
                case 2:
                    endDate = StartDate.AddDays((Duration * 7) - 1);
                    break;
                case 3:
                    endDate = StartDate.AddMonths(Duration).AddDays(-1);
                    break;
                case 4:
                    endDate = StartDate.AddYears(Duration).AddDays(-1);
                    break;
                case 0:
                    if (Duration == 0) { endDate = StartDate.AddDays(-1); }
                    else
                    {
                        DateTime newEndDate = StartDate;
                        for (int i = 0; i < Duration; i++)
                        {
                            if (newEndDate.DayOfWeek == DayOfWeek.Saturday)
                            {
                                newEndDate = newEndDate.AddDays(2);
                            }
                            else if (newEndDate.DayOfWeek == DayOfWeek.Sunday)
                            {
                                newEndDate = newEndDate.AddDays(1);
                            }
                            else
                            {
                                newEndDate = newEndDate.AddDays(1);
                            }
                        }

                        if (newEndDate.DayOfWeek == DayOfWeek.Saturday)
                        {
                            newEndDate = newEndDate.AddDays(2);
                        }
                        else if (newEndDate.DayOfWeek == DayOfWeek.Sunday)
                        {
                            newEndDate = newEndDate.AddDays(1);
                        }
                        endDate = newEndDate;

                        int noOfPublicHolidays = _publicHolidayRepository.GetByDateRangeAsync(StartDate, endDate).Result.Count;
                        if(noOfPublicHolidays > 0)
                        {
                            DateTime finalEndDate = endDate;
                            for(int i=0; i < noOfPublicHolidays; i++)
                            {
                                if (finalEndDate.DayOfWeek == DayOfWeek.Saturday)
                                {
                                    finalEndDate = finalEndDate.AddDays(2);
                                }
                                else if (finalEndDate.DayOfWeek == DayOfWeek.Sunday)
                                {
                                    finalEndDate = finalEndDate.AddDays(1);
                                }
                                else
                                {
                                    finalEndDate = finalEndDate.AddDays(1);
                                }
                            }
                            endDate = finalEndDate;
                        }
                    }
                    break;
                default:
                    endDate = StartDate;
                    break;
            }
            return endDate;
        }
        #endregion
    }
}
