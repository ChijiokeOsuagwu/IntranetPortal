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
    public class LeaveService : ILeaveService
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

        #region Leave Settings Service Methods

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
            bool IsDeleted = false;
            if (Id > 0)
            {
                LeaveProfile leaveProfile = await _leaveRepository.GetLeaveProfileByIdAsync(Id);
                if (leaveProfile == null) { throw new Exception("Required parameter ID cannot be null."); }

                var entities = await _leaveRepository.GetLeaveProfileDetailsByProfileIdAsync(Id);
                if (entities == null || entities.Count < 1) { throw new Exception("This Leave Profile cannot be deleted because it contains some profile options records."); }
                var employees = await _employeesRepository.GetEmployeesByLeaveProfileCodeAsync(leaveProfile.Code);
                if (employees != null && employees.Count > 0) { throw new Exception("This Leave Profile cannot be deleted because it is linked to some employee records."); }
                IsDeleted = await _leaveRepository.DeleteLeaveProfileAsync(Id);
            }
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

        #endregion

        #region Leave Plan Service Methods

        #region Leave Plan Read Service Methods
        public async Task<List<LeavePlan>> GetLeavePlansAsync(string EmployeeId, int LeaveYear)
        {
            List<LeavePlan> leaveList = new List<LeavePlan>();
            if (!string.IsNullOrWhiteSpace(EmployeeId) && LeaveYear > 0)
            {
                leaveList = await _leaveRepository.GetLeavePlansByEmployeeIdAsync(EmployeeId, LeaveYear);
            }
            return leaveList;
        }
        public async Task<LeavePlan> GetLeavePlanAsync(long LeavePlanId)
        {
            LeavePlan p = new LeavePlan();
            if (LeavePlanId > 0)
            {
                p = await _leaveRepository.GetLeavePlanByIdAsync(LeavePlanId);
            }
            return p;
        }
        public async Task<List<LeavePlan>> SearchLeavePlansAsync(int LeaveYear, int LeaveMonth, string EmployeeName = null, int? LocationId = null, int? UnitId = null)
        {
            List<LeavePlan> leavePlanList = new List<LeavePlan>();
            if (!string.IsNullOrWhiteSpace(EmployeeName))
            {
                if (LeaveYear > 0)
                {
                    if (LeaveMonth > 0)
                    {
                        leavePlanList = await _leaveRepository.GetLeavePlansByEmployeeNameAsync(EmployeeName, LeaveYear, LeaveMonth);
                    }
                    else
                    {
                        leavePlanList = await _leaveRepository.GetLeavePlansByEmployeeNameAsync(EmployeeName, LeaveYear, LeaveMonth);
                    }
                }
                else
                {
                    leavePlanList = await _leaveRepository.GetLeavePlansByEmployeeNameAsync(EmployeeName);
                }
            }
            else
            {
                if (LocationId != null && LocationId > 0)
                {
                    if (UnitId != null && UnitId > 0)
                    {
                        if (LeaveYear > 0)
                        {
                            if (LeaveMonth > 0)
                            {
                                leavePlanList = await _leaveRepository.GetLeavePlansByLocationIdnUnitIdAsync(LocationId.Value, UnitId.Value, LeaveYear, LeaveMonth);
                            }
                            else
                            {
                                leavePlanList = await _leaveRepository.GetLeavePlansByLocationIdnUnitIdAsync(LocationId.Value, UnitId.Value, LeaveYear);
                            }
                        }
                    }
                    else
                    {
                        if (LeaveYear > 0)
                        {
                            if (LeaveMonth > 0)
                            {
                                leavePlanList = await _leaveRepository.GetLeavePlansByLocationIdAsync(LocationId.Value, LeaveYear, LeaveMonth);
                            }
                            else
                            {
                                leavePlanList = await _leaveRepository.GetLeavePlansByLocationIdAsync(LocationId.Value, LeaveYear);
                            }
                        }
                    }
                }
                else
                {
                    if (UnitId != null && UnitId > 0)
                    {
                        if (LeaveYear > 0)
                        {
                            if (LeaveMonth > 0)
                            {
                                leavePlanList = await _leaveRepository.GetLeavePlansByUnitIdAsync(UnitId.Value, LeaveYear, LeaveMonth);
                            }
                            else
                            {
                                leavePlanList = await _leaveRepository.GetLeavePlansByUnitIdAsync(UnitId.Value, LeaveYear);
                            }
                        }
                    }
                    else
                    {
                        if (LeaveYear > 0)
                        {
                            if (LeaveMonth > 0)
                            {
                                leavePlanList = await _leaveRepository.GetLeavePlansByLeaveYearnLeaveMonthAsync(LeaveYear, LeaveMonth);
                            }
                            else
                            {
                                leavePlanList = await _leaveRepository.GetLeavePlansByLeaveYearAsync(LeaveYear);
                            }
                        }
                    }
                }
            }
            return leavePlanList;
        }


        //public async Task<List<EmployeeLeave>> SearchMyTeamsEmployeeLeavesAsync(string TeamLeadId, int LeaveYear, int LeaveMonth, string EmployeeId = null, string LeaveStatus = null, bool IsPlan = true)
        //{
        //    List<EmployeeLeave> LeaveList = new List<EmployeeLeave>();
        //    if (string.IsNullOrWhiteSpace(EmployeeId))
        //    {
        //        if (LeaveYear > 0)
        //        {
        //            if (LeaveMonth > 0)
        //            {
        //                if (!string.IsNullOrWhiteSpace(LeaveStatus))
        //                {
        //                    var tyms_entities = await _employeeLeaveRepository.GetByReportingLineIdnYearnMonthnStatusAsync(TeamLeadId, LeaveYear, LeaveMonth, LeaveStatus, IsPlan);
        //                    if (tyms_entities != null && tyms_entities.Count > 0)
        //                    {
        //                        LeaveList = tyms_entities;
        //                    }
        //                }
        //                else
        //                {
        //                    var tym_entities = await _employeeLeaveRepository.GetByReportingLineIdnYearnMonthAsync(TeamLeadId, LeaveYear, LeaveMonth, IsPlan);
        //                    if (tym_entities != null && tym_entities.Count > 0)
        //                    {
        //                        LeaveList = tym_entities;
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                if (!string.IsNullOrWhiteSpace(LeaveStatus))
        //                {
        //                    var tys_entities = await _employeeLeaveRepository.GetByReportingLineIdnYearnStatusAsync(TeamLeadId, LeaveYear, LeaveStatus, IsPlan);
        //                    if (tys_entities != null && tys_entities.Count > 0)
        //                    {
        //                        LeaveList = tys_entities;
        //                    }
        //                }
        //                else
        //                {
        //                    var ty_entities = await _employeeLeaveRepository.GetByReportingLineIdnYearAsync(TeamLeadId, LeaveYear, IsPlan);
        //                    if (ty_entities != null && ty_entities.Count > 0)
        //                    {
        //                        LeaveList = ty_entities;
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            if (!string.IsNullOrWhiteSpace(LeaveStatus))
        //            {
        //                var ts_entities = await _employeeLeaveRepository.GetByReportingLineIdnStatusAsync(TeamLeadId, LeaveStatus, IsPlan);
        //                if (ts_entities != null && ts_entities.Count > 0)
        //                {
        //                    LeaveList = ts_entities;
        //                }
        //            }
        //            else
        //            {
        //                var t_entities = await _employeeLeaveRepository.GetByReportingLineIdAsync(TeamLeadId, IsPlan);
        //                if (t_entities != null && t_entities.Count > 0)
        //                {
        //                    LeaveList = t_entities;
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        if (LeaveYear > 0)
        //        {
        //            if (LeaveMonth > 0)
        //            {
        //                if (!string.IsNullOrWhiteSpace(LeaveStatus))
        //                {
        //                    var eyms_entities = await _employeeLeaveRepository.GetByReportingLineIdnYearnMonthnStatusAsync(TeamLeadId, LeaveYear, LeaveMonth, LeaveStatus, IsPlan);
        //                    if (eyms_entities != null && eyms_entities.Count > 0)
        //                    {
        //                        LeaveList = eyms_entities;
        //                    }
        //                }
        //                else
        //                {
        //                    var eym_entities = await _employeeLeaveRepository.GetByReportingLineIdnYearnMonthAsync(TeamLeadId, LeaveYear, LeaveMonth, IsPlan);
        //                    if (eym_entities != null && eym_entities.Count > 0)
        //                    {
        //                        LeaveList = eym_entities;
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                if (!string.IsNullOrWhiteSpace(LeaveStatus))
        //                {
        //                    var eyms_entities = await _employeeLeaveRepository.GetByEmployeeIdnYearnMonthnStatusAsync(EmployeeId, LeaveYear, LeaveMonth, LeaveStatus, IsPlan);
        //                    if (eyms_entities != null && eyms_entities.Count > 0)
        //                    {
        //                        LeaveList = eyms_entities;
        //                    }
        //                }
        //                else
        //                {
        //                    var eym_entities = await _employeeLeaveRepository.GetByEmployeeIdnYearnMonthAsync(TeamLeadId, LeaveYear, LeaveMonth, IsPlan);
        //                    if (eym_entities != null && eym_entities.Count > 0)
        //                    {
        //                        LeaveList = eym_entities;
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            if (!string.IsNullOrWhiteSpace(LeaveStatus))
        //            {
        //                var es_entities = await _employeeLeaveRepository.GetByEmployeeIdnStatusAsync(EmployeeId, LeaveStatus, IsPlan);
        //                if (es_entities != null && es_entities.Count > 0)
        //                {
        //                    LeaveList = es_entities;
        //                }
        //            }
        //            else
        //            {
        //                var e_entities = await _employeeLeaveRepository.GetByEmployeeIdAsync(EmployeeId, IsPlan);
        //                if (e_entities != null && e_entities.Count > 0)
        //                {
        //                    LeaveList = e_entities;
        //                }
        //            }
        //        }
        //    }
        //    return LeaveList;
        //}
        //public async Task<List<EmployeeLeave>> SearchAllEmployeeLeavesAsync(int LeaveYear, int LeaveMonth, string EmployeeName = null, string LeaveStatus = null, bool IsPlan = true)
        //{
        //    List<EmployeeLeave> LeaveList = new List<EmployeeLeave>();
        //    if (string.IsNullOrWhiteSpace(EmployeeName))
        //    {
        //        if (LeaveYear > 0)
        //        {
        //            if (LeaveMonth > 0)
        //            {
        //                if (!string.IsNullOrWhiteSpace(LeaveStatus))
        //                {
        //                    var tyms_entities = await _employeeLeaveRepository.GetByYearnMonthnStatusAsync(LeaveYear, LeaveMonth, LeaveStatus, IsPlan);
        //                    if (tyms_entities != null && tyms_entities.Count > 0)
        //                    {
        //                        LeaveList = tyms_entities;
        //                    }
        //                }
        //                else
        //                {
        //                    var tym_entities = await _employeeLeaveRepository.GetByYearnMonthAsync(LeaveYear, LeaveMonth, IsPlan);
        //                    if (tym_entities != null && tym_entities.Count > 0)
        //                    {
        //                        LeaveList = tym_entities;
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                if (!string.IsNullOrWhiteSpace(LeaveStatus))
        //                {
        //                    var tys_entities = await _employeeLeaveRepository.GetByYearnStatusAsync(LeaveYear, LeaveStatus, IsPlan);
        //                    if (tys_entities != null && tys_entities.Count > 0)
        //                    {
        //                        LeaveList = tys_entities;
        //                    }
        //                }
        //                else
        //                {
        //                    var ty_entities = await _employeeLeaveRepository.GetByYearAsync(LeaveYear, IsPlan);
        //                    if (ty_entities != null && ty_entities.Count > 0)
        //                    {
        //                        LeaveList = ty_entities;
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            if (!string.IsNullOrWhiteSpace(LeaveStatus))
        //            {
        //                var ts_entities = await _employeeLeaveRepository.GetByStatusAsync(LeaveStatus, IsPlan);
        //                if (ts_entities != null && ts_entities.Count > 0)
        //                {
        //                    LeaveList = ts_entities;
        //                }
        //            }
        //            else
        //            {
        //                var t_entities = await _employeeLeaveRepository.GetAllAsync(IsPlan);
        //                if (t_entities != null && t_entities.Count > 0)
        //                {
        //                    LeaveList = t_entities;
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        if (LeaveYear > 0)
        //        {
        //            if (LeaveMonth > 0)
        //            {
        //                if (!string.IsNullOrWhiteSpace(LeaveStatus))
        //                {
        //                    var eyms_entities = await _employeeLeaveRepository.GetByEmployeeNamenYearnMonthnStatusAsync(EmployeeName, LeaveYear, LeaveMonth, LeaveStatus, IsPlan);
        //                    if (eyms_entities != null && eyms_entities.Count > 0)
        //                    {
        //                        LeaveList = eyms_entities;
        //                    }
        //                }
        //                else
        //                {
        //                    var eym_entities = await _employeeLeaveRepository.GetByEmployeeNamenYearnMonthAsync(EmployeeName, LeaveYear, LeaveMonth, IsPlan);
        //                    if (eym_entities != null && eym_entities.Count > 0)
        //                    {
        //                        LeaveList = eym_entities;
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                if (!string.IsNullOrWhiteSpace(LeaveStatus))
        //                {
        //                    var eyms_entities = await _employeeLeaveRepository.GetByEmployeeNamenYearnStatusAsync(EmployeeName, LeaveYear, LeaveStatus, IsPlan);
        //                    if (eyms_entities != null && eyms_entities.Count > 0)
        //                    {
        //                        LeaveList = eyms_entities;
        //                    }
        //                }
        //                else
        //                {
        //                    var eym_entities = await _employeeLeaveRepository.GetByEmployeeNamenYearAsync(EmployeeName, LeaveYear, IsPlan);
        //                    if (eym_entities != null && eym_entities.Count > 0)
        //                    {
        //                        LeaveList = eym_entities;
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            if (!string.IsNullOrWhiteSpace(LeaveStatus))
        //            {
        //                var es_entities = await _employeeLeaveRepository.GetByEmployeeNamenStatusAsync(EmployeeName, LeaveStatus, IsPlan);
        //                if (es_entities != null && es_entities.Count > 0)
        //                {
        //                    LeaveList = es_entities;
        //                }
        //            }
        //            else
        //            {
        //                var e_entities = await _employeeLeaveRepository.GetByEmployeeNameAsync(EmployeeName, IsPlan);
        //                if (e_entities != null && e_entities.Count > 0)
        //                {
        //                    LeaveList = e_entities;
        //                }
        //            }
        //        }
        //    }
        //    return LeaveList;
        //}

        #endregion

        #region Leave Plan Write Service Methods
        public async Task<long> CreateLeavePlanAsync(LeavePlan p)
        {
            long LeavePlanId;
            if (p != null)
            {
                LeavePlanId = await _leaveRepository.AddLeavePlanAsync(p);
                if (LeavePlanId > 0)
                {
                    //====== Add Activity History =======//
                    LeaveActivityLog log = new LeaveActivityLog();
                    log.ActivityDescription = $"Leave Plan was created by {p.LeaveEmployeeName} on {DateTime.UtcNow.ToLongDateString()} at {DateTime.UtcNow.ToLongTimeString()}";
                    log.ActivityTime = DateTime.UtcNow;
                    log.LeavePlanId = LeavePlanId;
                    await _leaveRepository.AddLeaveActivityLogAsync(log);
                }
            }
            else { throw new Exception($"Required parameter [Leave Plan] cannot be null."); }

            return LeavePlanId;
        }
        public async Task<bool> UpdateLeavePlanAsync(LeavePlan p)
        {
            bool IsSuccessful;
            if (p != null)
            {
                IsSuccessful = await _leaveRepository.EditLeavePlanAsync(p);
                if (IsSuccessful)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine($"Leave Plan was updated by {p.LeaveEmployeeName} on {DateTime.Now.ToLongDateString()} at {DateTime.Now.ToLongTimeString()}");

                    LeavePlan oldLeavePlan = await _leaveRepository.GetLeavePlanByIdAsync(p.LeavePlanId);
                    if (oldLeavePlan != null)
                    {
                        //if (!(string.IsNullOrWhiteSpace(p.LeaveReason) && string.IsNullOrWhiteSpace(oldLeavePlan.LeaveReason)))
                        //{
                        if (!string.Equals(p.LeaveReason, oldLeavePlan.LeaveReason, StringComparison.OrdinalIgnoreCase))
                        {
                            sb.AppendLine($"Updated Reason from: [{oldLeavePlan.LeaveReason}] to [{p.LeaveReason}].");
                        }
                        //}

                        ////if (!(p.LeavePlanStartDate == null && oldLeavePlan.LeavePlanStartDate == null))
                        ////{
                        if (p.LeavePlanStartDate != oldLeavePlan.LeavePlanStartDate)
                        {
                            sb.AppendLine($"Updated Start Date from: [{oldLeavePlan.LeavePlanStartDate.Value.ToLongDateString()}] to [{p.LeavePlanStartDate.Value.ToLongDateString()}].");
                        }
                        //}

                        //if (!(p.LeavePlanEndDate == null && oldLeavePlan.LeavePlanEndDate == null))
                        //{
                        if (p.LeavePlanEndDate != oldLeavePlan.LeavePlanEndDate)
                        {
                            sb.AppendLine($"Updated End Date from: [{oldLeavePlan.LeavePlanEndDate.Value.ToLongDateString()}] to [{p.LeavePlanEndDate.Value.ToLongDateString()}].");
                        }
                        //}

                        //if (!(p.LeavePlanResumptionDate == null && oldLeavePlan.LeavePlanResumptionDate == null))
                        //{
                        if (p.LeavePlanResumptionDate != oldLeavePlan.LeavePlanResumptionDate)
                        {
                            sb.AppendLine($"Updated Resumption Date from: [{oldLeavePlan.LeavePlanResumptionDate.Value.ToLongDateString()}] to [{p.LeavePlanResumptionDate.Value.ToLongDateString()}].");
                        }
                        //}

                        //if (!(p.LeavePlanDuration == 0 && oldLeavePlan.LeavePlanDuration == 0))
                        //{
                        if (p.LeavePlanDuration != oldLeavePlan.LeavePlanDuration)
                        {
                            sb.AppendLine($"Updated Duration from: [{oldLeavePlan.LeavePlanDurationDescription}] to [{p.LeavePlanDurationDescription}].");
                        }
                        //}

                        if (!string.Equals(p.LeaveTypeCode, oldLeavePlan.LeaveTypeCode, StringComparison.OrdinalIgnoreCase))
                        {
                            var leaveType = await _leaveRepository.GetLeaveTypeByCodeAsync(p.LeaveTypeCode);
                            if (leaveType != null) { p.LeaveTypeName = leaveType.Name; }
                            sb.AppendLine($"Updated Leave Type from: [{oldLeavePlan.LeaveTypeName}] to [{p.LeaveTypeName}].");
                        }

                        if (!string.Equals(p.LeavePlanDurationTypeDescription, oldLeavePlan.LeavePlanDurationTypeDescription, StringComparison.OrdinalIgnoreCase))
                        {
                            sb.AppendLine($"Updated Duration Type from: [{oldLeavePlan.LeavePlanDurationTypeDescription}] to [{p.LeavePlanDurationTypeDescription}].");
                        }

                        if (p.LeaveYear != oldLeavePlan.LeaveYear)
                        {
                            sb.AppendLine($"Updated Leave Year from: [{oldLeavePlan.LeaveYear}] to [{p.LeaveYear}].");
                        }
                    }
                    else
                    {
                        sb.AppendLine($"Reason was update to [{p.LeaveReason}].");
                        sb.AppendLine($"Start Date was updated to [{p.LeavePlanStartDate.Value.ToLongDateString()}].");
                        sb.AppendLine($"End Date was updated to [{p.LeavePlanEndDate.Value.ToLongDateString()}].");
                        sb.AppendLine($"Resumption Date was updated to [{p.LeavePlanResumptionDate.Value.ToLongDateString()}].");
                        sb.AppendLine($"Duration was updated to [{p.LeavePlanDurationDescription}].");
                        sb.AppendLine($"Leave Type was updated to [{p.LeaveTypeCode}].");
                        sb.AppendLine($"Duration Type was updated to [{p.LeavePlanDurationTypeDescription}].");
                        sb.AppendLine($"Leave Year was updated to [{p.LeaveYear}].");
                    }

                    //====== Add Activity History =======//
                    LeaveActivityLog history = new LeaveActivityLog();
                    history.ActivityDescription = sb.ToString();
                    history.ActivityTime = DateTime.Now;
                    history.LeavePlanId = p.LeavePlanId;
                    await _leaveRepository.AddLeaveActivityLogAsync(history);
                }
            }
            else { throw new Exception($"Required parameter [Leave Plan] cannot be null."); }
            return IsSuccessful;
        }
        public async Task<bool> DeleteLeavePlanAsync(long id)
        {
            bool IsSuccessful;
            if (id > 0)
            {
                IsSuccessful = await _leaveRepository.DeleteLeavePlanAsync(id);
            }
            else { throw new Exception("Required parameter [Leave Plan ID] is missing."); }
            return IsSuccessful;
        }

        #endregion

        #endregion

        #region Leave Request Service Methods
        #region Leave Request Write Service Methods
        public async Task<long> CreateLeaveRequestAsync(LeaveRequest r)
        {
            long LeaveRequestId;
            if (r != null)
            {
                LeaveRequestId = await _leaveRepository.AddLeaveRequestAsync(r);
                if (LeaveRequestId > 0)
                {
                    //====== Add Activity History =======//
                    LeaveActivityLog log = new LeaveActivityLog();
                    log.ActivityDescription = $"Leave Request was created by {r.LeaveEmployeeName} on {DateTime.UtcNow.ToLongDateString()} at {DateTime.UtcNow.ToLongTimeString()}";
                    log.ActivityTime = DateTime.UtcNow;
                    log.LeaveRequestId = LeaveRequestId;
                    await _leaveRepository.AddLeaveActivityLogAsync(log);
                }
            }
            else { throw new Exception($"Required parameter [Leave Request] cannot be null."); }

            return LeaveRequestId;
        }
        public async Task<bool> UpdateLeaveRequestAsync(LeaveRequest r)
        {
            bool IsSuccessful;
            if (r != null)
            {
                IsSuccessful = await _leaveRepository.EditLeaveRequestAsync(r);
                if (IsSuccessful)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine($"Leave Request was updated by {r.LeaveEmployeeName} on {DateTime.Now.ToLongDateString()} at {DateTime.Now.ToLongTimeString()}");

                    LeaveRequest oldLeaveRequest = await _leaveRepository.GetLeaveRequestByIdAsync(r.LeaveRequestId);
                    if (oldLeaveRequest != null)
                    {
                        //if (!(string.IsNullOrWhiteSpace(p.LeaveReason) && string.IsNullOrWhiteSpace(oldLeavePlan.LeaveReason)))
                        //{
                        if (!string.Equals(r.LeaveReason, oldLeaveRequest.LeaveReason, StringComparison.OrdinalIgnoreCase))
                        {
                            sb.AppendLine($"Updated Reason from: [{oldLeaveRequest.LeaveReason}] to [{r.LeaveReason}].");
                        }
                        //}

                        ////if (!(p.LeavePlanStartDate == null && oldLeavePlan.LeavePlanStartDate == null))
                        ////{
                        if (r.RequestedStartDate != oldLeaveRequest.RequestedStartDate)
                        {
                            sb.AppendLine($"Updated Start Date from: [{oldLeaveRequest.RequestedStartDate.ToLongDateString()}] to [{r.RequestedStartDate.ToLongDateString()}].");
                        }
                        //}

                        //if (!(p.LeavePlanEndDate == null && oldLeavePlan.LeavePlanEndDate == null))
                        //{
                        if (r.RequestedEndDate != oldLeaveRequest.RequestedEndDate)
                        {
                            sb.AppendLine($"Updated End Date from: [{oldLeaveRequest.RequestedEndDate.ToLongDateString()}] to [{r.RequestedEndDate.ToLongDateString()}].");
                        }
                        //}

                        //if (!(p.LeavePlanResumptionDate == null && oldLeavePlan.LeavePlanResumptionDate == null))
                        //{
                        if (r.RequestedResumptionDate != oldLeaveRequest.RequestedResumptionDate)
                        {
                            sb.AppendLine($"Updated Resumption Date from: [{oldLeaveRequest.RequestedResumptionDate.Value.ToLongDateString()}] to [{r.RequestedResumptionDate.Value.ToLongDateString()}].");
                        }
                        //}

                        //if (!(p.LeavePlanDuration == 0 && oldLeavePlan.LeavePlanDuration == 0))
                        //{
                        if (r.RequestedDuration != oldLeaveRequest.RequestedDuration)
                        {
                            sb.AppendLine($"Updated Duration from: [{oldLeaveRequest.RequestedDurationDescription}] to [{r.RequestedDurationDescription}].");
                        }
                        //}

                        if (!string.Equals(r.LeaveTypeCode, oldLeaveRequest.LeaveTypeCode, StringComparison.OrdinalIgnoreCase))
                        {
                            var leaveType = await _leaveRepository.GetLeaveTypeByCodeAsync(r.LeaveTypeCode);
                            if (leaveType != null) { r.LeaveTypeName = leaveType.Name; }
                            sb.AppendLine($"Updated Leave Type from: [{oldLeaveRequest.LeaveTypeName}] to [{r.LeaveTypeName}].");
                        }

                        if (r.LeaveYear != oldLeaveRequest.LeaveYear)
                        {
                            sb.AppendLine($"Updated Leave Year from: [{oldLeaveRequest.LeaveYear}] to [{r.LeaveYear}].");
                        }
                    }
                    else
                    {
                        sb.AppendLine($"Reason was update to [{r.LeaveReason}].");
                        sb.AppendLine($"Start Date was updated to [{r.RequestedStartDate.ToLongDateString()}].");
                        sb.AppendLine($"End Date was updated to [{r.RequestedEndDate.ToLongDateString()}].");
                        sb.AppendLine($"Resumption Date was updated to [{r.RequestedResumptionDate.Value.ToLongDateString()}].");
                        sb.AppendLine($"Duration was updated to [{r.RequestedDurationDescription}].");
                        sb.AppendLine($"Leave Type was updated to [{r.LeaveTypeCode}].");
                        sb.AppendLine($"Leave Year was updated to [{r.LeaveYear}].");
                    }

                    //====== Add Activity History =======//
                    LeaveActivityLog history = new LeaveActivityLog();
                    history.ActivityDescription = sb.ToString();
                    history.ActivityTime = DateTime.Now;
                    history.LeaveRequestId = r.LeaveRequestId;
                    await _leaveRepository.AddLeaveActivityLogAsync(history);
                }
            }
            else { throw new Exception($"Required parameter [Leave Request Entity] cannot be null."); }
            return IsSuccessful;
        }
        public async Task<bool> DeleteLeaveRequestAsync(long id)
        {
            bool IsSuccessful;
            if (id > 0)
            {
                IsSuccessful = await _leaveRepository.DeleteLeaveRequestAsync(id);
            }
            else { throw new Exception("Required parameter [Leave Request ID] is missing."); }
            return IsSuccessful;
        }

        #endregion
        #region Leave Request Read Service Methods
        public async Task<List<LeaveRequest>> GetLeaveRequestsAsync(string EmployeeId, int LeaveYear)
        {
            List<LeaveRequest> leaveList = new List<LeaveRequest>();
            if (!string.IsNullOrWhiteSpace(EmployeeId) && LeaveYear > 0)
            {
                leaveList = await _leaveRepository.GetLeaveRequestsByEmployeeIdAsync(EmployeeId, LeaveYear);
            }
            return leaveList;
        }
        public async Task<LeaveRequest> GetLeaveRequestAsync(long LeaveRequestId)
        {
            LeaveRequest p = new LeaveRequest();
            if (LeaveRequestId > 0)
            {
                p = await _leaveRepository.GetLeaveRequestByIdAsync(LeaveRequestId);
            }
            return p;
        }
        #endregion
        #endregion

        #region Leave Balances Service Methods
        public async Task<LeaveBalances> GetLeaveBalancesAsync(string LeaveTypeCode, int LeaveYear, string EmployeeId = null, string EmployeeName = null)
        {
            if (string.IsNullOrWhiteSpace(LeaveTypeCode)) { throw new Exception("Required parameter Leave Type Code has an invalid value."); }
            if (LeaveYear < 1) { throw new Exception("Required parameter Leave Year has an invalid value."); }
            if (string.IsNullOrWhiteSpace(EmployeeId) && string.IsNullOrWhiteSpace(EmployeeName)) { throw new Exception("Required parameter Employee ID and Employee Name both have invalid values."); }
            
            LeaveBalances balances = new LeaveBalances();
            balances.LeaveYear = LeaveYear;

            var entity = await _leaveRepository.GetLeaveTypeByCodeAsync(LeaveTypeCode);
            if(entity != null) 
            {
                balances.LeaveTypeName = entity.Name;
            }

            long _totalAnnualLeaveDays = 0;
            long _totalLeaveDaysDue = 0;
            long _totalLeaveDaysUsedInPreviousYear = 0;
            long _totalLeaveDaysUnusedInPreviousYear = 0;
            long _totalLeaveDaysUsedInCurrentYear = 0;
            long _totalLeaveDaysUnusedInCurrentYear = 0;
            LeaveProfileDetail _profileDetail;

            if (!string.IsNullOrWhiteSpace(EmployeeId))
            {
                int _previousLeaveYear = Convert.ToInt32(LeaveYear - 1);
                LeaveBalances _previousBalances = new LeaveBalances();
                _profileDetail = await _leaveRepository.GetLeaveProfileDetailByEmployeeIdnLeaveTypeAsync(EmployeeId, LeaveTypeCode);
                if (_profileDetail == null) { throw new Exception("No Leave Profile was found for this employee. Please ensure this employee is linked to a Leave Profile."); }
                switch (_profileDetail.DurationTypeId)
                {
                    case 0:
                    case 1:
                        _totalAnnualLeaveDays = _profileDetail.Duration;
                        break;
                    case 2:
                        _totalAnnualLeaveDays = _profileDetail.Duration * 7;
                        break;
                    case 3:
                        _totalAnnualLeaveDays = _profileDetail.Duration * 30;
                        break;
                    case 4:
                        _totalAnnualLeaveDays = _profileDetail.Duration * 364;
                        break;
                    default:
                        break;
                }
                _totalLeaveDaysUsedInPreviousYear = await _leaveRepository.GetLeaveDaysUsedByEmployeeIdnLeaveTypeCodenLeaveYearAsync(EmployeeId, LeaveTypeCode, _previousLeaveYear);
                _totalLeaveDaysUnusedInPreviousYear = _totalAnnualLeaveDays - _totalLeaveDaysUsedInPreviousYear;


                _totalLeaveDaysUsedInCurrentYear = await _leaveRepository.GetLeaveDaysUsedByEmployeeIdnLeaveTypeCodenLeaveYearAsync(EmployeeId, LeaveTypeCode, LeaveYear);
                _totalLeaveDaysUnusedInCurrentYear = _totalAnnualLeaveDays - _totalLeaveDaysUsedInCurrentYear;

                if (_profileDetail.CanBeCarriedOver && (_profileDetail.CarryOverEndMonth <= DateTime.Now.Month))
                {
                    _totalLeaveDaysDue = _totalAnnualLeaveDays + _totalLeaveDaysUnusedInPreviousYear;
                    balances.CarriedOverLeaveBalance = _totalLeaveDaysUnusedInPreviousYear;
                    balances.CurrentYearPrifileLeaveDays = _totalAnnualLeaveDays;
                    balances.TotalLeaveDaysUsed = _totalLeaveDaysUsedInCurrentYear;
                    balances.TotalOutstandingLeaveDays = _totalLeaveDaysDue - _totalLeaveDaysUsedInCurrentYear;
                }
                else
                {
                    _totalLeaveDaysDue = _totalAnnualLeaveDays;
                    balances.CurrentYearPrifileLeaveDays = _totalAnnualLeaveDays;
                    balances.TotalLeaveDaysUsed = _totalLeaveDaysUsedInCurrentYear;
                    balances.TotalOutstandingLeaveDays = _totalLeaveDaysDue - _totalLeaveDaysUsedInCurrentYear;
                }
            }
            else
            {
                int _previousLeaveYear = Convert.ToInt32(LeaveYear - 1);
                LeaveBalances _previousBalances = new LeaveBalances();
                _profileDetail = await _leaveRepository.GetLeaveProfileDetailByEmployeeNamenLeaveTypeAsync(EmployeeName, LeaveTypeCode);
                if (_profileDetail == null) { throw new Exception("No Leave Profile was found for this employee. Please ensure this employee is linked to a Leave Profile."); }
                switch (_profileDetail.DurationTypeId)
                {
                    case 0:
                    case 1:
                        _totalAnnualLeaveDays = _profileDetail.Duration;
                        break;
                    case 2:
                        _totalAnnualLeaveDays = _profileDetail.Duration * 7;
                        break;
                    case 3:
                        _totalAnnualLeaveDays = _profileDetail.Duration * 30;
                        break;
                    case 4:
                        _totalAnnualLeaveDays = _profileDetail.Duration * 364;
                        break;
                    default:
                        break;
                }
                _totalLeaveDaysUsedInPreviousYear = await _leaveRepository.GetLeaveDaysUsedByEmployeeIdnLeaveTypeCodenLeaveYearAsync(EmployeeId, LeaveTypeCode, _previousLeaveYear);
                _totalLeaveDaysUnusedInPreviousYear = _totalAnnualLeaveDays - _totalLeaveDaysUsedInPreviousYear;


                _totalLeaveDaysUsedInCurrentYear = await _leaveRepository.GetLeaveDaysUsedByEmployeeIdnLeaveTypeCodenLeaveYearAsync(EmployeeId, LeaveTypeCode, LeaveYear);
                _totalLeaveDaysUnusedInCurrentYear = _totalAnnualLeaveDays - _totalLeaveDaysUsedInCurrentYear;

                if (_profileDetail.CanBeCarriedOver && (_profileDetail.CarryOverEndMonth <= DateTime.Now.Month))
                {
                    _totalLeaveDaysDue = _totalAnnualLeaveDays + _totalLeaveDaysUnusedInPreviousYear;
                    balances.CarriedOverLeaveBalance = _totalLeaveDaysUnusedInPreviousYear;
                    balances.CurrentYearPrifileLeaveDays = _totalAnnualLeaveDays;
                    balances.TotalLeaveDaysUsed = _totalLeaveDaysUsedInCurrentYear;
                    balances.TotalOutstandingLeaveDays = _totalLeaveDaysDue - _totalLeaveDaysUsedInCurrentYear;
                }
                else
                {
                    _totalLeaveDaysDue = _totalAnnualLeaveDays;
                    balances.CurrentYearPrifileLeaveDays = _totalAnnualLeaveDays;
                    balances.TotalLeaveDaysUsed = _totalLeaveDaysUsedInCurrentYear;
                    balances.TotalOutstandingLeaveDays = _totalLeaveDaysDue - _totalLeaveDaysUsedInCurrentYear;
                }

            }
            return balances;
        }

        #endregion

        #region Leave Submission Service Methods
        public async Task<bool> SubmitLeaveAsync(LeaveSubmission e)
        {
            bool IsSubmitted = false;
            string DocumentType = string.Empty;
            if (e.LeavePlanId > 0) { DocumentType = "Leave Plan"; }
            else if (e.LeaveRequestId > 0) { DocumentType = "Leave Request"; }
            else { DocumentType = "Unknown"; }
            if (e != null)
            {
                IsSubmitted = await _leaveRepository.AddLeaveSubmissionAsync(e);
                if (IsSubmitted)
                {
                    LeaveActivityLog history = new LeaveActivityLog();
                    history.ActivityDescription = $"{DocumentType} was submitted to {e.ToEmployeeName} by {e.FromEmployeeName} on {DateTime.Now.ToLongDateString()} at {DateTime.Now.ToLongTimeString()}";
                    history.ActivityTime = DateTime.UtcNow;
                    history.LeavePlanId = e.LeavePlanId;
                    history.LeaveRequestId = e.LeaveRequestId;
                    await _leaveRepository.AddLeaveActivityLogAsync(history);
                }
            }
            else { throw new Exception("Required parameter [Leave Submission] has invalid value."); }
            return IsSubmitted;
        }
        public async Task<LeaveSubmission> GetLeaveSubmissionByIdAsync(long LeaveSubmissionId)
        {
            LeaveSubmission leaveSubmission = new LeaveSubmission();

            var entities = await _leaveRepository.GetLeaveSubmissionsByLeaveSubmissionIdAsync(LeaveSubmissionId);
            if (entities != null && entities.Count > 0)
            {
                leaveSubmission = entities[0];
            }
            return leaveSubmission;
        }

        public async Task<List<LeaveSubmission>> GetLeaveSubmissionsByApproverIdAsync(string approverName, int? submittedYear = null)
        {
            List<LeaveSubmission> leaveSubmissions = new List<LeaveSubmission>();
            if (submittedYear != null && submittedYear > 0)
            {
                var entities = await _leaveRepository.GetLeaveSubmissionsByYearSubmittedAsync(approverName, submittedYear.Value);
                if (entities != null && entities.Count > 0)
                {
                    leaveSubmissions = entities;
                }
            }
            else
            {
                var entities = await _leaveRepository.GetLeaveSubmissionsByToEmployeeNameAsync(approverName);
                if (entities != null && entities.Count > 0)
                {
                    leaveSubmissions = entities;
                }
            }
            return leaveSubmissions;
        }
        public async Task<bool> DeleteLeaveSubmissionAsync(long LeaveSubmissionId)
        {
            return await _leaveRepository.DeleteSubmissionAsync(LeaveSubmissionId);
        }

        #endregion

        #region Leave Approval Service Methods
        public async Task<bool> ApproveLeavePlanAsync(LeaveApproval a, LeaveSubmission s)
        {
            bool IsUpdated = false;
            int newStatusId = 0;
            string newStatusDescription = "";
            if (a != null)
            {
                if (a.IsApproved)
                {
                    newStatusId = (int)LeaveStatusEnum.Approved;
                    newStatusDescription = LeaveStatusEnum.Approved.ToString();
                }
                else
                {
                    newStatusId = (int)LeaveStatusEnum.Declined;
                    newStatusDescription = LeaveStatusEnum.Declined.ToString();
                }

                //string ApprovalType;
                //switch (a.ApproverRole)
                //{
                //    case "Line Manager":
                //        ApprovalType = "LM";
                //        break;
                //    case "Head of Department":
                //        ApprovalType = "HD";
                //        break;
                //    case "HR Representative":
                //        ApprovalType = "HR";
                //        break;
                //    case "Station Manager":
                //        ApprovalType = "SM";
                //        break;
                //    case "Executive Management":
                //        ApprovalType = "XM";
                //        break;
                //    default:
                //        break;
                //}

                long approvalId = await _leaveRepository.AddLeaveApprovalAsync(a);
                if (approvalId > 0)
                {
                    //Update Leave Plan Status to Pending
                    IsUpdated = await _leaveRepository.UpdateLeavePlanStatusAsync(a.LeavePlanId.Value, newStatusId);
                    if (IsUpdated)
                    {
                        await _leaveRepository.UpdateSubmissionActionStatusAsync(s.LeaveSubmissionId, a.TimeApproved);

                        LeaveNote note = new LeaveNote();
                        //====== Add Leave Note =======//
                        if (!string.IsNullOrWhiteSpace(a.ApproverComments))
                        {
                            note.NoteContent = a.ApproverComments;
                            note.LeavePlanId = a.LeavePlanId;
                            note.TimeAdded = DateTime.Now;
                            note.FromEmployeeName = a.ApproverName;
                            await _leaveRepository.AddNoteAsync(note);
                        }

                        //====== Add Leave Activity Log =======//
                        LeaveActivityLog history = new LeaveActivityLog();
                        history.ActivityDescription = $"Leave Plan was approved by {a.ApproverName} on {DateTime.UtcNow.ToLongDateString()} at {DateTime.UtcNow.ToLongTimeString()}.";
                        history.ActivityTime = DateTime.UtcNow;
                        history.LeavePlanId = a.LeavePlanId;
                        await _leaveRepository.AddLeaveActivityLogAsync(history);

                        return true;
                    }
                    else
                    {
                        await _leaveRepository.DeleteApprovalAsync(approvalId);
                        throw new Exception("An error was encountered while attempting to update Leave Plan status.");
                    }
                }
                else { throw new Exception("Sorry an error was encountered while attempting to add the approval record."); }
            }
            else { throw new Exception("Required parameter [Leave Approval] has invalid value."); }

        }
        public async Task<List<LeaveApproval>> GetLeaveApprovalsAsync(long? LeavePlanId = null, long? LeaveRequestId = null)
        {
            List<LeaveApproval> approvalsList = new List<LeaveApproval>();
            if (LeavePlanId > 0)
            {
                var entities = await _leaveRepository.GetLeaveApprovalsByLeavePlanIdAsync(LeavePlanId.Value);
                if (entities != null && entities.Count > 0)
                {
                    approvalsList = entities;
                }
            }
            else if (LeaveRequestId > 0)
            {
                var entities = await _leaveRepository.GetLeaveApprovalsByLeaveRequestIdAsync(LeaveRequestId.Value);
                if (entities != null && entities.Count > 0)
                {
                    approvalsList = entities;
                }
            }
            return approvalsList;
        }

        public async Task<bool> DeclineLeavePlanAsync(LeaveApproval a, LeaveSubmission s)
        {
            bool IsUpdated = false;
            int newStatusId = 0;
            string newStatusDescription = "";
            if (a != null)
            {
                if (a.IsApproved)
                {
                    newStatusId = (int)LeaveStatusEnum.Approved;
                    newStatusDescription = LeaveStatusEnum.Approved.ToString();
                }
                else
                {
                    newStatusId = (int)LeaveStatusEnum.Declined;
                    newStatusDescription = LeaveStatusEnum.Declined.ToString();
                }

                //Update Leave Plan Status to Pending
                IsUpdated = await _leaveRepository.UpdateLeavePlanStatusAsync(a.LeavePlanId.Value, newStatusId);
                if (IsUpdated)
                {
                    await _leaveRepository.UpdateSubmissionActionStatusAsync(s.LeaveSubmissionId, a.TimeApproved);

                    LeaveNote note = new LeaveNote();
                    //====== Add Leave Note =======//
                    if (!string.IsNullOrWhiteSpace(a.ApproverComments))
                    {
                        note.NoteContent = a.ApproverComments;
                        note.LeavePlanId = a.LeavePlanId;
                        note.TimeAdded = DateTime.Now;
                        note.FromEmployeeName = a.ApproverName;
                        await _leaveRepository.AddNoteAsync(note);
                    }

                    //====== Add Leave Activity Log =======//
                    LeaveActivityLog history = new LeaveActivityLog();
                    history.ActivityDescription = $"Leave Plan was declined by {a.ApproverName} on {DateTime.UtcNow.ToLongDateString()} at {DateTime.UtcNow.ToLongTimeString()}.";
                    history.ActivityTime = DateTime.UtcNow;
                    history.LeavePlanId = a.LeavePlanId;
                    await _leaveRepository.AddLeaveActivityLogAsync(history);

                    return true;
                }
                else
                {
                    throw new Exception("An error was encountered while attempting to update Leave Plan status.");
                }
            }
            else { throw new Exception("Required parameter [Leave Approval] has invalid value."); }

        }

        #endregion






        #region Leave Service Helper Methods
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
                    else if (Duration == 1) { endDate = StartDate; }
                    else
                    {
                        DateTime newEndDate = StartDate;
                        int counter = 1;
                        do
                        {
                            counter++;
                            newEndDate = newEndDate.AddDays(1);
                            if (newEndDate.DayOfWeek == DayOfWeek.Saturday)
                            {
                                newEndDate = newEndDate.AddDays(2);
                            }
                        } while (counter < Duration);


                        //for (int i = 1; i <= Duration; i++)
                        //{
                        //    if (newEndDate.DayOfWeek == DayOfWeek.Saturday)
                        //    {
                        //        newEndDate = newEndDate.AddDays(2);
                        //        //i++;
                        //    }
                        //    else
                        //    {
                        //        newEndDate = newEndDate.AddDays(1);
                        //    }
                        //}

                        if (newEndDate.DayOfWeek == DayOfWeek.Saturday)
                        {
                            endDate = newEndDate.AddDays(2);
                        }
                        else if (newEndDate.DayOfWeek == DayOfWeek.Sunday)
                        {
                            endDate = newEndDate.AddDays(1);
                        }
                        else
                        {
                            endDate = newEndDate;
                        }
                        int noOfPublicHolidays = _publicHolidayRepository.GetByDateRangeAsync(StartDate, endDate).Result.Count;
                        if (noOfPublicHolidays > 0)
                        {
                            DateTime finalEndDate = endDate;
                            for (int i = 1; i <= noOfPublicHolidays; i++)
                            {
                                if (finalEndDate.DayOfWeek == DayOfWeek.Saturday)
                                {
                                    finalEndDate = finalEndDate.AddDays(2);
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

        public int GetLeaveBalance(string EmployeeId, string LeaveTypeCode, int LeaveYear)
        {
            int leaveBalance = 0;
            //    int noOfCarriedOverLeaveDays = 0;
            //    int noOfUsedLeaveDays = 0;

            //    //1.Get Leave Profile Details for the selected Leave Type
            //    LeaveProfileDetail leaveProfileDetail = new LeaveProfileDetail();
            //    var profileDetailEntity = _leaveProfileDetailRepository.GetByEmployeeIdnLeaveTypeAsync(EmployeeId, LeaveTypeCode).Result;
            //    if (profileDetailEntity != null) { leaveProfileDetail = profileDetailEntity; }

            //    //2.Get Total Leave Days from Profile Details (P)
            //    int LeaveProfileDuration = leaveProfileDetail.Duration;
            //    int LeaveProfileDurationType = leaveProfileDetail.DurationTypeId;
            //    string LeaveDurationDescription = leaveProfileDetail.DurationDescription;

            //    //3.If Leave Type is CarryOver Enabled : Get LeaveCarryOver Days(V)
            //    if (leaveProfileDetail.CanBeCarriedOver && leaveProfileDetail.CarryOverEndMonth <= DateTime.Now.Month)
            //    {
            //        //Get Total Number of Days of Leave Type used up the previous year.
            //        int _leaveYear = LeaveYear - 1;

            //        LeaveDuration previousYearUsedLeaveDuration = _employeeLeaveRepository.GetUsedLeaveDurationByLeaveYearnEmployeeIdnLeaveTypeAsync(_leaveYear, EmployeeId, LeaveTypeCode).Result;
            //        if (previousYearUsedLeaveDuration != null)
            //        {
            //            noOfCarriedOverLeaveDays = LeaveProfileDuration - previousYearUsedLeaveDuration.Duration;
            //        }
            //    }

            //    //4.Get Total Leave Days Used already(U)
            //    LeaveDuration usedLeaveDuration = _employeeLeaveRepository.GetUsedLeaveDurationByLeaveYearnEmployeeIdnLeaveTypeAsync(LeaveYear, EmployeeId, LeaveTypeCode).Result;
            //    if (usedLeaveDuration != null)
            //    {
            //        noOfUsedLeaveDays = usedLeaveDuration.Duration;
            //    }

            //    //5.Calculate D = ((P + V) - U)
            //    leaveBalance = (leaveProfileDetail.Duration + noOfCarriedOverLeaveDays) - noOfUsedLeaveDays;

            //    //6.Return D
            return leaveBalance;
        }
        #endregion

        #region Leave Notes Service Methods
        public async Task<List<LeaveNote>> GetLeavePlanNotesAsync(long LeavePlanId)
        {
            List<LeaveNote> notesList = new List<LeaveNote>();
            if (LeavePlanId > 0)
            {
                var entities = await _leaveRepository.GetNotesByLeavePlanIdAsync(LeavePlanId);
                if (entities != null && entities.Count > 0)
                {
                    notesList = entities;
                }
            }
            return notesList;
        }
        public async Task<List<LeaveNote>> GetLeaveRequestNotesAsync(long LeaveRequestId)
        {
            List<LeaveNote> notesList = new List<LeaveNote>();
            if (LeaveRequestId > 0)
            {
                var entities = await _leaveRepository.GetNotesByLeaveRequestIdAsync(LeaveRequestId);
                if (entities != null && entities.Count > 0)
                {
                    notesList = entities;
                }
            }
            return notesList;
        }
        public async Task<bool> AddLeaveNoteAsync(LeaveNote e)
        {
            bool IsAdded;
            if (e != null)
            {
                IsAdded = await _leaveRepository.AddNoteAsync(e);
            }
            else { throw new Exception("Required parameter [Leave Note] has invalid value."); }
            return IsAdded;
        }
        #endregion

        #region Leave Activities Service Methods
        public async Task<List<LeaveActivityLog>> GetLeaveActivitiesAsync(long? LeavePlanId = null, long? LeaveRequestId = null)
        {
            List<LeaveActivityLog> activitiesList = new List<LeaveActivityLog>();
            if (LeavePlanId > 0)
            {
                var planActivities = await _leaveRepository.GetLeaveActivityLogByLeavePlanIdAsync(LeavePlanId.Value);
                if (planActivities != null && planActivities.Count > 0)
                {
                    activitiesList = planActivities;
                }
            }
            else if (LeaveRequestId > 0)
            {
                var requestActivities = await _leaveRepository.GetLeaveActivityLogByLeaveRequestIdAsync(LeaveRequestId.Value);
                if (requestActivities != null && requestActivities.Count > 0)
                {
                    activitiesList = requestActivities;
                }
            }
            return activitiesList;
        }
        //public async Task<bool> AddActivityLogAsync(LeaveActivityLog e)
        //{
        //    bool IsAdded = false;
        //    if (e != null)
        //    {
        //        IsAdded = await _employeeLeaveRepository.AddLogAsync(e);
        //    }
        //    return IsAdded;
        //}
        #endregion

    }
}
