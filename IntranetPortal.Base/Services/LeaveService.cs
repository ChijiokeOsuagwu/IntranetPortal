using IntranetPortal.Base.Models.GlobalSettingsModels;
using IntranetPortal.Base.Models.LeaveModels;
using IntranetPortal.Base.Repositories.ErmRepositories;
using IntranetPortal.Base.Repositories.GlobalSettingsRepositories;
using IntranetPortal.Base.Repositories.LeaveRepositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Services
{
    public class LeaveService: ILeaveService
    {
        private readonly ILeaveRepository _leaveRepository;
        private readonly IPublicHolidayRepository _publicHolidayRepository;
        private readonly IEmployeesRepository _employeesRepository;

        public LeaveService(ILeaveRepository leaveRepository, IPublicHolidayRepository publicHolidayRepository, IEmployeesRepository employeesRepository)
        {
            _leaveRepository = leaveRepository;
            _publicHolidayRepository = publicHolidayRepository;
            _employeesRepository = employeesRepository;
        }

        #region Leave Types Service Methods
        public async Task<List<LeaveType>> GetLeaveTypes(bool ExcludeSystem = true)
        {
            if (ExcludeSystem)
            {
                return await _leaveRepository.GetAllLeaveTypesExcludingSystemAsync();
            }
            return await _leaveRepository.GetAllLeaveTypesAsync();
        }
        public async Task<LeaveType> GetLeaveType(string LeaveTypeCode)
        {
            LeaveType leaveType = new LeaveType();
            if (!string.IsNullOrWhiteSpace(LeaveTypeCode))
            {
                leaveType = await _leaveRepository.GetLeaveTypeByCodeAsync(LeaveTypeCode);
            }
            return leaveType;
        }
        public async Task<LeaveType> GetLeaveTypeByName(string Name)
        {
            LeaveType leaveType = new LeaveType();
            if (!string.IsNullOrWhiteSpace(Name))
            {
                leaveType = await _leaveRepository.GetLeaveTypeByNameAsync(Name);
            }
            return leaveType;
        }
        public async Task<bool> CreateLeaveType(LeaveType leaveType)
        {
            bool IsCreated = false;
            if (leaveType != null)
            {
                var sameCodeLeaveType = await _leaveRepository.GetLeaveTypeByCodeAsync(leaveType.Code);
                if (sameCodeLeaveType == null || string.IsNullOrWhiteSpace(sameCodeLeaveType.Name))
                {
                    var sameNameLeaveType = await _leaveRepository.GetLeaveTypeByNameAsync(leaveType.Name);
                    if (sameNameLeaveType == null || string.IsNullOrWhiteSpace(sameNameLeaveType.Name))
                    {
                        IsCreated = await _leaveRepository.AddLeaveTypeAsync(leaveType);
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
                var sameNameLeaveType = await _leaveRepository.GetLeaveTypeByNameAsync(leaveType.Name);
                if (sameNameLeaveType == null || string.IsNullOrWhiteSpace(sameNameLeaveType.Code) || sameNameLeaveType.Code == leaveType.Code)
                {
                    IsUpdated = await _leaveRepository.EditLeaveTypeAsync(leaveType);
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
                IsDeleted = await _leaveRepository.DeleteLeaveTypeAsync(code);
            }
            else { throw new Exception("Required parameter Code cannot be null."); }
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

        #region Leave Profiles Service Methods
        public async Task<List<LeaveProfile>> GetLeaveProfiles()
        {
            return await _leaveRepository.GetAllLeaveProfilesAsync();
        }
        public async Task<LeaveProfile> GetLeaveProfile(int Id)
        {
            LeaveProfile leaveProfile = new LeaveProfile();
            if (Id > 0)
            {
                leaveProfile = await _leaveRepository.GetLeaveProfileByIdAsync(Id);
            }
            return leaveProfile;
        }
        public async Task<LeaveProfile> GetLeaveProfile(string Name)
        {
            LeaveProfile leaveProfile = new LeaveProfile();
            if (!string.IsNullOrWhiteSpace(Name))
            {
                leaveProfile = await _leaveRepository.GetLeaveProfileByNameAsync(Name);
            }
            return leaveProfile;
        }
        public async Task<bool> CreateLeaveProfile(LeaveProfile leaveProfile)
        {
            bool IsSuccessful;
            if (leaveProfile != null)
            {
                var sameNameLeaveProfile = await _leaveRepository.GetLeaveProfileByNameAsync(leaveProfile.Name);
                if (sameNameLeaveProfile == null || sameNameLeaveProfile.Id < 1)
                {
                    IsSuccessful = await _leaveRepository.AddLeaveProfileAsync(leaveProfile);
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
                var sameNameLeaveProfile = await _leaveRepository.GetLeaveProfileByNameAsync(leaveProfile.Name);
                if (sameNameLeaveProfile == null || sameNameLeaveProfile.Id < 1 || sameNameLeaveProfile.Id == leaveProfile.Id)
                {
                    IsUpdated = await _leaveRepository.EditLeaveProfileAsync(leaveProfile);
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
                var entities = await _leaveRepository.GetLeaveProfileDetailsByProfileIdAsync(Id);
                if (entities == null || entities.Count < 1)
                {
                    var employees = await _employeesRepository.GetEmployeesByLeaveProfileIdAsync(Id);
                    if (employees == null || employees.Count < 1)
                    {
                        IsDeleted = await _leaveRepository.DeleteLeaveProfileAsync(Id);
                    }
                    else { throw new Exception("This Leave Profile cannot be deleted because it is linked to some employee records."); }
                }
                else { throw new Exception("This Leave Profile cannot be deleted because it contains some profile options records."); }
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
                leaveProfileDetails = await _leaveRepository.GetLeaveProfileDetailsByProfileIdAsync(LeaveProfileId);
            }
            return leaveProfileDetails;
        }
        public async Task<List<LeaveProfileDetail>> GetLeaveProfileDetails(int LeaveProfileId, string LeaveTypeCode)
        {
            List<LeaveProfileDetail> leaveProfileDetails = new List<LeaveProfileDetail>();
            if (LeaveProfileId > 0 && !string.IsNullOrWhiteSpace(LeaveTypeCode))
            {
                leaveProfileDetails = await _leaveRepository.GetLeaveProfileDetailsByProfileIdnLeaveTypeAsync(LeaveProfileId, LeaveTypeCode);
            }
            return leaveProfileDetails;
        }
        public async Task<LeaveProfileDetail> GetLeaveProfileDetail(int Id)
        {
            LeaveProfileDetail leaveProfileDetail = new LeaveProfileDetail();
            if (Id > 0)
            {
                var entities = await _leaveRepository.GetLeaveProfileDetailByIdAsync(Id);
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
                var existingLeaveProfileDetails = await _leaveRepository.GetLeaveProfileDetailsByProfileIdnLeaveTypeAsync(leaveProfileDetail.ProfileId, leaveProfileDetail.LeaveTypeCode);
                if (existingLeaveProfileDetails != null && existingLeaveProfileDetails.Count > 0)
                { throw new Exception("Duplicate Entry. This Leave Type has already been set up for this Profile."); }
                else
                {
                    switch (leaveProfileDetail.DurationTypeId)
                    {
                        case 0:
                            leaveProfileDetail.DurationDescription = $"{leaveProfileDetail.Duration} Working Days";
                            break;
                        case 1:
                            leaveProfileDetail.DurationDescription = $"{leaveProfileDetail.Duration} Days";
                            break;
                        case 2:
                            leaveProfileDetail.DurationDescription = $"{leaveProfileDetail.Duration} Weeks";
                            break;
                        case 3:
                            leaveProfileDetail.DurationDescription = $"{leaveProfileDetail.Duration} Months";
                            break;
                        case 4:
                            leaveProfileDetail.DurationDescription = $"{leaveProfileDetail.Duration} Years";
                            break;
                        default:
                            break;
                    }
                    IsSuccessful = await _leaveRepository.AddLeaveProfileDetailAsync(leaveProfileDetail);
                }
            }
            else { throw new Exception("Required parameter [Leave Profile Detail] cannot be null."); }

            return IsSuccessful;
        }
        public async Task<bool> UpdateLeaveProfileDetail(LeaveProfileDetail d)
        {
            bool IsUpdated;
            if (d == null) { throw new Exception("Required parameter [Leave Profile Detail] cannot be null."); }
                switch (d.DurationTypeId)
                {
                    case 0:
                    d.DurationDescription = $"{d.Duration} Working Days";
                        break;
                    case 1:
                    d.DurationDescription = $"{d.Duration} Days";
                        break;
                    case 2:
                    d.DurationDescription = $"{d.Duration} Weeks";
                        break;
                    case 3:
                    d.DurationDescription = $"{d.Duration} Months";
                        break;
                    case 4:
                    d.DurationDescription = $"{d.Duration} Years";
                        break;
                    default:
                        break;
                }
                IsUpdated = await _leaveRepository.EditLeaveProfileDetailAsync(d);
            return IsUpdated;
        }
        public async Task<bool> DeleteLeaveProfileDetail(int Id)
        {
            bool IsDeleted;
            if (Id > 0)
            {
                IsDeleted = await _leaveRepository.DeleteLeaveProfileDetailAsync(Id);
            }
            else { throw new Exception("Required parameter ID cannot be null."); }
            return IsDeleted;
        }
        #endregion
    }
}
