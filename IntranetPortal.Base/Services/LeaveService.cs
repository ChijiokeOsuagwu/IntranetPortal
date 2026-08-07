using IntranetPortal.Base.Models.BaseModels;
using IntranetPortal.Base.Models.EmployeeRecordModels;
using IntranetPortal.Base.Models.GlobalSettingsModels;
using IntranetPortal.Base.Models.LeaveModels;
using IntranetPortal.Base.Repositories.ErmRepositories;
using IntranetPortal.Base.Repositories.GlobalSettingsRepositories;
using IntranetPortal.Base.Repositories.LeaveRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<LeaveProfile> GetLeaveProfileByCode(string ProfileCode)
        {
            LeaveProfile leaveProfile = new LeaveProfile();
            if (!string.IsNullOrWhiteSpace(ProfileCode))
            {
                leaveProfile = await _leaveRepository.GetLeaveProfileByCodeAsync(ProfileCode);
            }
            return leaveProfile;
        }
        public async Task<LeaveProfile> GetLeaveProfileByName(string ProfileName)
        {
            LeaveProfile leaveProfile = new LeaveProfile();
            if (!string.IsNullOrWhiteSpace(ProfileName))
            {
                leaveProfile = await _leaveRepository.GetLeaveProfileByNameAsync(ProfileName);
            }
            return leaveProfile;
        }
        public async Task<bool> CreateLeaveProfile(LeaveProfile leaveProfile)
        {
            bool IsSuccessful;
            if (leaveProfile != null)
            {
                var sameNameLeaveProfile = await _leaveRepository.GetLeaveProfileByNameAsync(leaveProfile.Name);
                if (sameNameLeaveProfile == null || string.IsNullOrWhiteSpace(sameNameLeaveProfile.Code))
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
                if (sameNameLeaveProfile == null || string.IsNullOrWhiteSpace(sameNameLeaveProfile.Code) || sameNameLeaveProfile.Code == leaveProfile.Code)
                {
                    IsUpdated = await _leaveRepository.EditLeaveProfileAsync(leaveProfile);
                }
                else { throw new Exception("A Leave Profile with the same Name already exists."); }
            }
            else { throw new Exception("Required parameter Leave Profile cannot be null."); }

            return IsUpdated;
        }
        public async Task<bool> DeleteLeaveProfile(string ProfileCode)
        {
            bool IsDeleted = false;
            if (!string.IsNullOrWhiteSpace(ProfileCode))
            {
                LeaveProfile leaveProfile = await _leaveRepository.GetLeaveProfileByCodeAsync(ProfileCode);
                if (leaveProfile == null) { throw new Exception("Required parameter ID cannot be null."); }

                var entities = await _leaveRepository.GetLeaveProfileDetailsByProfileCodeAsync(ProfileCode);
                if (entities == null || entities.Count < 1) { throw new Exception("This Leave Profile cannot be deleted because it contains some profile options records."); }
                var employees = await _employeesRepository.GetEmployeesByLeaveProfileCodeAsync(leaveProfile.Code);
                if (employees != null && employees.Count > 0) { throw new Exception("This Leave Profile cannot be deleted because it is linked to some employee records."); }
                IsDeleted = await _leaveRepository.DeleteLeaveProfileAsync(ProfileCode);
            }
            return IsDeleted;
        }
        #endregion

        #region Leave Profile Details Service Methods
        public async Task<List<LeaveProfileDetail>> GetLeaveProfileDetails(string LeaveProfileCode)
        {
            List<LeaveProfileDetail> leaveProfileDetails = new List<LeaveProfileDetail>();
            if (!string.IsNullOrWhiteSpace(LeaveProfileCode))
            {
                leaveProfileDetails = await _leaveRepository.GetLeaveProfileDetailsByProfileCodeAsync(LeaveProfileCode);
            }
            return leaveProfileDetails;
        }
        public async Task<List<LeaveProfileDetail>> GetLeaveProfileDetails(string LeaveProfileCode, string LeaveTypeCode)
        {
            List<LeaveProfileDetail> leaveProfileDetails = new List<LeaveProfileDetail>();
            if (!string.IsNullOrWhiteSpace(LeaveProfileCode) && !string.IsNullOrWhiteSpace(LeaveTypeCode))
            {
                leaveProfileDetails = await _leaveRepository.GetLeaveProfileDetailsByProfileCodenLeaveTypeAsync(LeaveProfileCode, LeaveTypeCode);
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
                var existingLeaveProfileDetails = await _leaveRepository.GetLeaveProfileDetailsByProfileCodenLeaveTypeAsync(leaveProfileDetail.ProfileCode, leaveProfileDetail.LeaveTypeCode);
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
        public async Task<List<LeavePlan>> SearchLeavePlansAsync(int LeaveYear, int LeaveMonth, string EmployeeName = null, int? LocationId = null, int? UnitId = null, string EmployeeId = null)
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
                        leavePlanList = await _leaveRepository.GetLeavePlansByEmployeeNameAsync(EmployeeName, LeaveYear);
                    }
                }
                else
                {
                    leavePlanList = await _leaveRepository.GetLeavePlansByEmployeeNameAsync(EmployeeName);
                }
            }
            else if (!string.IsNullOrWhiteSpace(EmployeeId))
            {
                if (LeaveYear > 0)
                {
                    if (LeaveMonth > 0)
                    {
                        leavePlanList = await _leaveRepository.GetLeavePlansByEmployeeIdAsync(EmployeeId, LeaveYear, LeaveMonth);
                    }
                    else
                    {
                        leavePlanList = await _leaveRepository.GetLeavePlansByEmployeeIdAsync(EmployeeName, LeaveYear);
                    }
                }
                else
                {
                    leavePlanList = await _leaveRepository.GetLeavePlansByEmployeeIdAsync(EmployeeName);
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
        public async Task<List<LeavePlan>> SearchMyTeamsLeavePlansAsync(string TeamLeadId, int LeaveYear, int LeaveMonth, string EmployeeId = null)
        {
            List<LeavePlan> leavePlanList = new List<LeavePlan>();
            if (!string.IsNullOrWhiteSpace(EmployeeId))
            {
                if (LeaveYear > 0)
                {
                    if (LeaveMonth > 0)
                    {
                        var eym_entities = await _leaveRepository.GetLeavePlansByEmployeeIdAsync(EmployeeId, LeaveYear, LeaveMonth);

                        if (eym_entities != null && eym_entities.Count > 0)
                        {
                            leavePlanList = eym_entities;
                        }
                    }
                    else
                    {
                        var ey_entities = await _leaveRepository.GetLeavePlansByEmployeeIdAsync(EmployeeId, LeaveYear);
                        if (ey_entities != null && ey_entities.Count > 0)
                        {
                            leavePlanList = ey_entities;
                        }

                    }
                }
                else
                {
                    var e_entities = await _leaveRepository.GetLeavePlansByEmployeeIdAsync(EmployeeId);
                    if (e_entities != null && e_entities.Count > 0)
                    {
                        leavePlanList = e_entities;
                    }

                }
            }
            else
            {
                if (LeaveYear > 0)
                {
                    if (LeaveMonth > 0)
                    {
                        var tym_entities = await _leaveRepository.GetLeavePlansByReportingLineIdAsync(TeamLeadId, LeaveYear, LeaveMonth);
                        if (tym_entities != null && tym_entities.Count > 0)
                        {
                            leavePlanList = tym_entities;
                        }

                    }
                    else
                    {
                        var ty_entities = await _leaveRepository.GetLeavePlansByReportingLineIdAsync(TeamLeadId, LeaveYear);
                        if (ty_entities != null && ty_entities.Count > 0)
                        {
                            leavePlanList = ty_entities;
                        }

                    }
                }
                else
                {
                    var t_entities = await _leaveRepository.GetLeavePlansByReportingLineIdAsync(EmployeeId);
                    if (t_entities != null && t_entities.Count > 0)
                    {
                        leavePlanList = t_entities;
                    }

                }
            }
            return leavePlanList;
        }

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

                        if (!string.Equals(p.LeaveReason, oldLeavePlan.LeaveReason, StringComparison.OrdinalIgnoreCase))
                        {
                            sb.AppendLine($"Updated Reason from: [{oldLeavePlan.LeaveReason}] to [{p.LeaveReason}].");
                        }


                        if (p.LeavePlanStartDate != oldLeavePlan.LeavePlanStartDate)
                        {
                            sb.AppendLine($"Updated Start Date from: [{oldLeavePlan.LeavePlanStartDate.Value.ToLongDateString()}] to [{p.LeavePlanStartDate.Value.ToLongDateString()}].");
                        }

                        if (p.LeavePlanEndDate != oldLeavePlan.LeavePlanEndDate)
                        {
                            sb.AppendLine($"Updated End Date from: [{oldLeavePlan.LeavePlanEndDate.Value.ToLongDateString()}] to [{p.LeavePlanEndDate.Value.ToLongDateString()}].");
                        }

                        if (p.LeavePlanResumptionDate != oldLeavePlan.LeavePlanResumptionDate)
                        {
                            sb.AppendLine($"Updated Resumption Date from: [{oldLeavePlan.LeavePlanResumptionDate.Value.ToLongDateString()}] to [{p.LeavePlanResumptionDate.Value.ToLongDateString()}].");
                        }

                        if (p.LeavePlanDuration != oldLeavePlan.LeavePlanDuration)
                        {
                            sb.AppendLine($"Updated Duration from: [{oldLeavePlan.LeavePlanDurationDescription}] to [{p.LeavePlanDurationDescription}].");
                        }


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
        public async Task<bool> UpdateLeavePlanFlagAsync(long LeavePlanId, bool IsFlagged, string FlagReason = null, string FlaggedBy = null)
        {
            bool IsSuccessful;
            IsSuccessful = await _leaveRepository.EditLeavePlanReturnStatusAsync(LeavePlanId, IsFlagged);
            if (IsSuccessful)
            {
                if (IsFlagged)
                {
                    LeaveNote note = new LeaveNote();
                    note.LeavePlanId = LeavePlanId;
                    note.NoteContent = FlagReason;
                    note.FromEmployeeName = FlaggedBy;
                    note.TimeAdded = DateTime.Now;
                    await _leaveRepository.AddNoteAsync(note);
                }

                //====== Add Activity History =======//
                LeaveActivityLog history = new LeaveActivityLog();
                if (IsFlagged)
                {
                    history.ActivityDescription = $"Leave Plan was flagged by {FlaggedBy} on {DateTime.UtcNow.ToLongDateString()} at exactly {DateTime.UtcNow.ToLongTimeString()} GMT.";
                }
                else
                {
                    history.ActivityDescription = $"Leave Plan was unflagged by {FlaggedBy} on {DateTime.UtcNow.ToLongDateString()} at exactly {DateTime.UtcNow.ToLongTimeString()} GMT.";
                }

                history.ActivityTime = DateTime.UtcNow;
                history.LeavePlanId = LeavePlanId;
                await _leaveRepository.AddLeaveActivityLogAsync(history);

            }
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
                switch (r.RequestedDurationTypeId)
                {
                    case 0:
                        r.RequestedDurationDescription = $"{r.RequestedDuration} Working Day(s)";
                        break;
                    case 1:
                        r.RequestedDurationDescription = $"{r.RequestedDuration} Day(s)";
                        break;
                    case 2:
                        r.RequestedDurationDescription = $"{r.RequestedDuration} Week(s)";
                        break;
                    case 3:
                        r.RequestedDurationDescription = $"{r.RequestedDuration} Month(s)";
                        break;
                    case 4:
                        r.RequestedDurationDescription = $"{r.RequestedDuration} Year(s)";
                        break;
                    default:
                        break;
                }
                LeaveRequestId = await _leaveRepository.AddLeaveRequestAsync(r);
                if (LeaveRequestId > 0)
                {
                    r.LeaveAllowance.LeaveRequestId = LeaveRequestId;
                    if (r.RequestLeaveAllowance)
                    {
                        var requestedLeaveAllowances = await _leaveRepository.GetLeaveAllowanceByEmployeeIdnLeaveYearAsync(r.LeaveEmployeeId, r.LeaveYear);
                        if (requestedLeaveAllowances == null || requestedLeaveAllowances.Count < 1)
                        {
                            if (await _leaveRepository.AddLeaveAllowanceAsync(r.LeaveAllowance) < 1)
                            {
                                await _leaveRepository.DeleteLeaveRequestAsync(LeaveRequestId);
                            }
                        }
                    }

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
            long _leaveRequestId = r.LeaveRequestId;

            if (r != null)
            {
                if (r.LeaveRequestStatusId > 1) { r.LeaveRequestStatusId = 0; }
                switch (r.RequestedDurationTypeId)
                {
                    case 0:
                        r.RequestedDurationDescription = $"{r.RequestedDuration} Working Day(s)";
                        break;
                    case 1:
                        r.RequestedDurationDescription = $"{r.RequestedDuration} Day(s)";
                        break;
                    case 2:
                        r.RequestedDurationDescription = $"{r.RequestedDuration} Week(s)";
                        break;
                    case 3:
                        r.RequestedDurationDescription = $"{r.RequestedDuration} Month(s)";
                        break;
                    case 4:
                        r.RequestedDurationDescription = $"{r.RequestedDuration} Year(s)";
                        break;
                    default:
                        break;
                }

                IsSuccessful = await _leaveRepository.EditLeaveRequestAsync(r);
                if (IsSuccessful)
                {
                    if (r.RequestLeaveAllowance)
                    {
                        var requestedLeaveAllowances = await _leaveRepository.GetLeaveAllowanceByEmployeeIdnLeaveYearAsync(r.LeaveEmployeeId, r.LeaveYear);
                        if (requestedLeaveAllowances == null || requestedLeaveAllowances.Count < 1)
                        {
                            await _leaveRepository.AddLeaveAllowanceAsync(r.LeaveAllowance);
                        }
                    }

                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine($"Leave Request was updated by {r.LeaveEmployeeName} on {DateTime.Now.ToLongDateString()} at {DateTime.Now.ToLongTimeString()}");

                    LeaveRequest oldLeaveRequest = await _leaveRepository.GetLeaveRequestByIdAsync(_leaveRequestId);
                    if (oldLeaveRequest != null)
                    {
                        if (!string.Equals(r.LeaveReason, oldLeaveRequest.LeaveReason, StringComparison.OrdinalIgnoreCase))
                        {
                            sb.AppendLine($"Updated Reason from: [{oldLeaveRequest.LeaveReason}] to [{r.LeaveReason}].");
                        }

                        if (r.RequestedStartDate != oldLeaveRequest.RequestedStartDate)
                        {
                            sb.AppendLine($"Updated Start Date from: [{oldLeaveRequest.RequestedStartDate.ToLongDateString()}] to [{r.RequestedStartDate.ToLongDateString()}].");
                        }

                        if (r.RequestedEndDate != oldLeaveRequest.RequestedEndDate)
                        {
                            sb.AppendLine($"Updated End Date from: [{oldLeaveRequest.RequestedEndDate.ToLongDateString()}] to [{r.RequestedEndDate.ToLongDateString()}].");
                        }

                        if (r.RequestedResumptionDate != oldLeaveRequest.RequestedResumptionDate)
                        {
                            sb.AppendLine($"Updated Resumption Date from: [{oldLeaveRequest.RequestedResumptionDate.Value.ToLongDateString()}] to [{r.RequestedResumptionDate.Value.ToLongDateString()}].");
                        }

                        if (r.RequestedDuration != oldLeaveRequest.RequestedDuration)
                        {
                            sb.AppendLine($"Updated Duration from: [{oldLeaveRequest.RequestedDurationDescription}] to [{r.RequestedDurationDescription}].");
                        }

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
        public async Task<bool> HrConfirmLeaveRequestAsync(long LeaveRequestId, string ConfirmedBy, DateTime ConfirmedTime)
        {
            LeaveRequest request = new LeaveRequest();
            LeaveTransaction transaction = new LeaveTransaction();
            request = await _leaveRepository.GetLeaveRequestByIdAsync(LeaveRequestId);
            if (request == null) { throw new Exception("No record was found for this Leave."); }
            transaction.LeaveDepartmentId = request.DepartmentId;
            transaction.LeaveDepartmentName = request.DepartmentName;
            transaction.LeaveEmployeeId = request.LeaveEmployeeId;
            transaction.LeaveEmployeeName = request.LeaveEmployeeName;
            transaction.LeaveLocationId = request.LocationId;
            transaction.LeaveRequestId = request.LeaveRequestId;
            transaction.LeaveTypeCode = request.LeaveTypeCode;
            transaction.LeaveUnitId = request.UnitId;
            transaction.LeaveYear = request.LeaveYear;
            transaction.NumberOfDaysUsed = request.RequestedDuration;
            transaction.TransactionDate = DateTime.UtcNow;
            transaction.TransactionDescription = $"Leave Request Approved and Confirmed by HR ({ConfirmedBy}). Starting on {request.RequestedStartDate.ToLongDateString()} and Ending on: {request.RequestedEndDate.ToLongDateString()}. To resume work on {request.RequestedResumptionDate.Value.ToLongDateString()}. ";
            transaction.TransactionRecordedBy = ConfirmedBy;

            bool IsSuccessful = await _leaveRepository.UpdateLeaveRequestHrConfirmedAsync(LeaveRequestId, ConfirmedBy, ConfirmedTime);
            if (IsSuccessful)
            {
                long newLeaveTransactionId = await _leaveRepository.AddLeaveTransactionAsync(transaction);
                if (newLeaveTransactionId > 0)
                {
                    LeaveRollingBalance newRollingBalance = new LeaveRollingBalance();
                    var currentBalances = await _leaveRepository.GetLeaveRollingBalanceByEmployeeIdAsync(request.LeaveEmployeeId, request.LeaveYear, request.LeaveTypeCode) ?? throw new Exception("Invalid Value Error: No Balances was found for this employee for the current Leave Year. ");
                    newRollingBalance = currentBalances;

                    newRollingBalance.LeaveDaysUsed = currentBalances.LeaveDaysUsed + request.RequestedDuration;
                    newRollingBalance.LeaveBalanceDate = DateTime.UtcNow;
                    newRollingBalance.LeaveTransactionId = newLeaveTransactionId;

                    if (await _leaveRepository.UpdateLeaveRollingBalanceAsync(newRollingBalance))
                    {
                        string activityDescription = $"Leave Request was Confirmed by the HR Department. The confirmation was done by {ConfirmedBy} on {DateTime.Now.ToLongDateString()} at {DateTime.Now.ToLongTimeString()}";
                        //====== Add Activity History =======//
                        LeaveActivityLog history = new LeaveActivityLog();
                        history.ActivityDescription = activityDescription;
                        history.ActivityTime = DateTime.Now;
                        history.LeaveRequestId = LeaveRequestId;
                        await _leaveRepository.AddLeaveActivityLogAsync(history);
                    }
                }
            }
            return IsSuccessful;
        }
        public async Task<bool> CloseLeaveRequestAsync(LeaveRequest r, string LeaveRequestClosedBy)
        {
            bool IsUpdated;
            if (r == null) { throw new ArgumentNullException("Leave Request"); }

            if (string.IsNullOrWhiteSpace(r.ActualLeaveDurationDescription))
            {
                switch (r.ActualLeaveDurationTypeId)
                {
                    case 0:
                        r.ActualLeaveDurationDescription = $"{r.ActualLeaveDuration} Working Day(s)";
                        break;
                    case 1:
                        r.ActualLeaveDurationDescription = $"{r.ActualLeaveDuration} Day(s)";
                        break;
                    case 2:
                        r.ActualLeaveDurationDescription = $"{r.ActualLeaveDuration} Week(s)";
                        break;
                    case 3:
                        r.ActualLeaveDurationDescription = $"{r.ActualLeaveDuration} Month(s)";
                        break;
                    case 4:
                        r.ActualLeaveDurationDescription = $"{r.ActualLeaveDuration} Year(s)";
                        break;
                    default:
                        break;
                }
            }
            IsUpdated = await _leaveRepository.UpdateLeaveRequestToClosedAsync(r, LeaveRequestClosedBy);
            if (IsUpdated)
            {
                //====== Add Activity History =======//
                LeaveActivityLog log = new LeaveActivityLog
                {
                    ActivityDescription = $"Leave Request was closed by {LeaveRequestClosedBy} on {DateTime.UtcNow.ToLongDateString()} at {DateTime.UtcNow.ToLongTimeString()}",
                    ActivityTime = DateTime.UtcNow,
                    LeaveRequestId = r.LeaveRequestId,
                };
                await _leaveRepository.AddLeaveActivityLogAsync(log);
            }
            return true;
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
        public async Task<List<LeaveRequest>> SearchLeaveRequestsAsync(int LeaveYear, int LeaveMonth, string EmployeeName = null, int? LocationId = null, int? UnitId = null)
        {
            List<LeaveRequest> leaveRequestList = new List<LeaveRequest>();
            if (!string.IsNullOrWhiteSpace(EmployeeName))
            {
                if (LeaveYear > 0)
                {
                    if (LeaveMonth > 0)
                    {
                        leaveRequestList = await _leaveRepository.GetLeaveRequestsByEmployeeNameAsync(EmployeeName, LeaveYear, LeaveMonth);
                    }
                    else
                    {
                        leaveRequestList = await _leaveRepository.GetLeaveRequestsByEmployeeNameAsync(EmployeeName, LeaveYear, LeaveMonth);
                    }
                }
                else
                {
                    leaveRequestList = await _leaveRepository.GetLeaveRequestsByEmployeeNameAsync(EmployeeName);
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
                                leaveRequestList = await _leaveRepository.GetLeaveRequestsByLocationIdnUnitIdAsync(LocationId.Value, UnitId.Value, LeaveYear, LeaveMonth);
                            }
                            else
                            {
                                leaveRequestList = await _leaveRepository.GetLeaveRequestsByLocationIdnUnitIdAsync(LocationId.Value, UnitId.Value, LeaveYear);
                            }
                        }
                    }
                    else
                    {
                        if (LeaveYear > 0)
                        {
                            if (LeaveMonth > 0)
                            {
                                leaveRequestList = await _leaveRepository.GetLeaveRequestsByLocationIdAsync(LocationId.Value, LeaveYear, LeaveMonth);
                            }
                            else
                            {
                                leaveRequestList = await _leaveRepository.GetLeaveRequestsByLocationIdAsync(LocationId.Value, LeaveYear);
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
                                leaveRequestList = await _leaveRepository.GetLeaveRequestsByUnitIdAsync(UnitId.Value, LeaveYear, LeaveMonth);
                            }
                            else
                            {
                                leaveRequestList = await _leaveRepository.GetLeaveRequestsByUnitIdAsync(UnitId.Value, LeaveYear);
                            }
                        }
                    }
                    else
                    {
                        if (LeaveYear > 0)
                        {
                            if (LeaveMonth > 0)
                            {
                                leaveRequestList = await _leaveRepository.GetLeaveRequestsByLeaveYearnLeaveMonthAsync(LeaveYear, LeaveMonth);
                            }
                            else
                            {
                                leaveRequestList = await _leaveRepository.GetLeaveRequestsByLeaveYearAsync(LeaveYear);
                            }
                        }
                    }
                }
            }
            return leaveRequestList;
        }
        public async Task<List<LeaveRequest>> SearchMyTeamsLeaveRequestsAsync(string TeamLeadId, int LeaveYear, int LeaveMonth, string EmployeeId = null, int? LeaveStatus = null)
        {
            List<LeaveRequest> leaveRequestList = new List<LeaveRequest>();
            if (!string.IsNullOrWhiteSpace(EmployeeId))
            {
                if (LeaveYear > 0)
                {
                    if (LeaveMonth > 0)
                    {
                        if (LeaveStatus != null)
                        {
                            var eyms_entities = await _leaveRepository.GetLeaveRequestsByEmployeeIdnStatusAsync(EmployeeId, LeaveYear, LeaveMonth, LeaveStatus.Value);
                            if (eyms_entities != null)
                            {
                                leaveRequestList = eyms_entities;
                            }
                        }
                        else
                        {
                            var eym_entities = await _leaveRepository.GetLeaveRequestsByEmployeeIdAsync(EmployeeId, LeaveYear, LeaveMonth);
                            if (eym_entities != null)
                            {
                                leaveRequestList = eym_entities;
                            }
                        }
                    }
                    else
                    {
                        if (LeaveStatus != null)
                        {
                            var eys_entities = await _leaveRepository.GetLeaveRequestsByEmployeeIdnStatusAsync(EmployeeId, LeaveYear, LeaveStatus.Value);
                            if (eys_entities != null)
                            {
                                leaveRequestList = eys_entities;
                            }
                        }
                        else
                        {
                            var ey_entities = await _leaveRepository.GetLeaveRequestsByEmployeeIdAsync(EmployeeId, LeaveYear);
                            if (ey_entities != null)
                            {
                                leaveRequestList = ey_entities;
                            }
                        }
                    }
                }
                else
                {
                    if (LeaveStatus != null)
                    {
                        var es_entities = await _leaveRepository.GetLeaveRequestsByEmployeeIdnStatusAsync(EmployeeId, LeaveStatus.Value);
                        if (es_entities != null)
                        {
                            leaveRequestList = es_entities;
                        }
                    }
                    else
                    {
                        var e_entities = await _leaveRepository.GetLeaveRequestsByEmployeeIdAsync(EmployeeId);
                        if (e_entities != null)
                        {
                            leaveRequestList = e_entities;
                        }
                    }
                }
            }
            else
            {
                if (LeaveYear > 0)
                {
                    if (LeaveMonth > 0)
                    {
                        if (LeaveStatus != null)
                        {
                            var tyms_entities = await _leaveRepository.GetLeaveRequestsByReportingLineIdnStatusAsync(TeamLeadId, LeaveYear, LeaveMonth, LeaveStatus.Value);
                            if (tyms_entities != null)
                            {
                                leaveRequestList = tyms_entities;
                            }
                        }
                        else
                        {
                            var tym_entities = await _leaveRepository.GetLeaveRequestsByReportingLineIdAsync(TeamLeadId, LeaveYear, LeaveMonth);
                            if (tym_entities != null)
                            {
                                leaveRequestList = tym_entities;
                            }
                        }
                    }
                    else
                    {
                        if (LeaveStatus != null)
                        {
                            var tys_entities = await _leaveRepository.GetLeaveRequestsByReportingLineIdnStatusAsync(TeamLeadId, LeaveYear, LeaveStatus.Value);
                            if (tys_entities != null)
                            {
                                leaveRequestList = tys_entities;
                            }
                        }
                        else
                        {
                            var ty_entities = await _leaveRepository.GetLeaveRequestsByReportingLineIdAsync(TeamLeadId, LeaveYear);
                            if (ty_entities != null)
                            {
                                leaveRequestList = ty_entities;
                            }
                        }
                    }
                }
                else
                {

                }
            }
            return leaveRequestList;
        }
        public async Task<List<LeaveRequest>> SearchApprovedLeaveRequestsAsync(int LeaveYear, int LeaveMonth = 0, int? LocationId = null, int? UnitId = null)
        {
            List<LeaveRequest> leaveRequestList = new List<LeaveRequest>();
            if (LocationId != null && LocationId > 0)
            {
                if (UnitId != null && UnitId > 0)
                {
                    if (LeaveMonth > 0)
                    {
                        leaveRequestList = await _leaveRepository.GetApprovedLeaveRequestsByLeaveYearnLeaveMonthnLocationIdnUnitIdAsync(LeaveYear, LeaveMonth, LocationId.Value, UnitId.Value);
                    }
                    else
                    {
                        leaveRequestList = await _leaveRepository.GetApprovedLeaveRequestsByLeaveYearnLocationIdnUnitIdAsync(LeaveYear, LocationId.Value, UnitId.Value);
                    }
                }
                else
                {
                    if (LeaveMonth > 0)
                    {
                        leaveRequestList = await _leaveRepository.GetApprovedLeaveRequestsByLeaveYearnLeaveMonthnLocationIdAsync(LeaveYear, LeaveMonth, LocationId.Value);
                    }
                    else
                    {
                        leaveRequestList = await _leaveRepository.GetApprovedLeaveRequestsByLeaveYearnLocationIdAsync(LeaveYear, LocationId.Value);
                    }
                }
            }
            else
            {
                if (UnitId != null && UnitId > 0)
                {
                    if (LeaveMonth > 0)
                    {
                        leaveRequestList = await _leaveRepository.GetApprovedLeaveRequestsByLeaveYearnLeaveMonthnUnitIdAsync(LeaveYear, LeaveMonth, UnitId.Value);
                    }
                    else
                    {
                        leaveRequestList = await _leaveRepository.GetApprovedLeaveRequestsByLeaveYearnUnitIdAsync(LeaveYear, UnitId.Value);
                    }
                }
                else
                {
                    if (LeaveMonth > 0)
                    {
                        leaveRequestList = await _leaveRepository.GetApprovedLeaveRequestsByLeaveYearnLeaveMonthAsync(LeaveYear, LeaveMonth);
                    }
                    else
                    {
                        leaveRequestList = await _leaveRepository.GetApprovedLeaveRequestsByLeaveYearAsync(LeaveYear);
                    }
                }
            }
            return leaveRequestList;
        }
        public async Task<List<LeaveRequest>> GetLeaveRequestsDueResumptionAsync(int ResumptionYear, int ResumptionMonth, int? LocationId = null, int? UnitId = null)
        {
            List<LeaveRequest> leaveRequestList = new List<LeaveRequest>();


            if (LocationId > 0)
            {
                if (UnitId > 0)
                {
                    leaveRequestList = await _leaveRepository.GetLeaveRequestsDueResumptionByResumptionYearnResumptionMonthnLocationIdnUnitIdAsync(ResumptionYear, ResumptionMonth, LocationId.Value, UnitId.Value);
                }
                else
                {
                    leaveRequestList = await _leaveRepository.GetLeaveRequestsDueResumptionByResumptionYearnResumptionMonthnLocationIdAsync(ResumptionYear, ResumptionMonth, LocationId.Value);
                }
            }
            else
            {
                if (UnitId > 0)
                {
                    leaveRequestList = await _leaveRepository.GetLeaveRequestsDueResumptionByResumptionYearnResumptionMonthnUnitIdAsync(ResumptionYear, ResumptionMonth, UnitId.Value);
                }
                else
                {
                    if (ResumptionMonth > 0)
                    {
                        leaveRequestList = await _leaveRepository.GetLeaveRequestsDueResumptionByResumptionYearnResumptionMonthAsync(ResumptionYear, ResumptionMonth);
                    }
                    else
                    {
                        leaveRequestList = await _leaveRepository.GetLeaveRequestsDueResumptionByResumptionYearAsync(ResumptionYear);
                    }
                }
            }




            return leaveRequestList;
        }

        #endregion

        #endregion

        #region Leave Balances Service Methods
        public async Task<LeaveRollingBalance> GetRefreshedLeaveBalancesAsync(string LeaveTypeCode, int LeaveYear, string EmployeeId = null, string EmployeeName = null)
        {
            if (string.IsNullOrWhiteSpace(LeaveTypeCode)) { throw new Exception("Required parameter Leave Type Code has an invalid value."); }
            if (LeaveYear < 2020) { throw new Exception("Required parameter Leave Year has an invalid value."); }
            if (string.IsNullOrWhiteSpace(EmployeeId) && string.IsNullOrWhiteSpace(EmployeeName)) { throw new Exception("Required parameter Employee ID and Employee Name both have invalid values."); }

            Employee leaveEmployee = new Employee();
            if (!string.IsNullOrWhiteSpace(EmployeeId))
            {
                leaveEmployee = await _employeesRepository.GetEmployeeByIdAsync(EmployeeId);
            }
            else
            {
                leaveEmployee = await _employeesRepository.GetEmployeeByNameAsync(EmployeeName);
            }

            LeaveRollingBalance previousYearsBalances = new LeaveRollingBalance();
            previousYearsBalances.LeaveYear = LeaveYear - 1;

            LeaveRollingBalance currentYearsBalances = new LeaveRollingBalance();
            currentYearsBalances.LeaveYear = LeaveYear;
            currentYearsBalances.LeaveTypeCode = previousYearsBalances.LeaveTypeCode = LeaveTypeCode;

            LeaveRollingBalance existingBalances = await _leaveRepository.GetLeaveRollingBalanceByEmployeeIdAsync(leaveEmployee.EmployeeID, LeaveYear, LeaveTypeCode);
            if (existingBalances == null || existingBalances.AnnualProfileLeaveDays < 1)
            {
                //Get Leave Details eg Duration, CanBeCarriedOver and CarryOverExpiryMonth
                LeaveProfileDetail leaveProfileDetail = await _leaveRepository.GetLeaveProfileDetailByEmployeeNamenLeaveTypeAsync(EmployeeName, LeaveTypeCode);
                if (leaveProfileDetail == null) { throw new Exception("No Leave Profile was found for this employee. Please ensure this employee is linked to a Leave Profile."); }
                currentYearsBalances.PreviousBalanceCanBeCarriedOver = leaveProfileDetail.CanBeCarriedOver;
                currentYearsBalances.PreviousBalanceExpiryMonth = leaveProfileDetail.CarryOverEndMonth ?? 0;

                switch (leaveProfileDetail.DurationTypeId)
                {
                    case 0:
                    case 1:
                        currentYearsBalances.AnnualProfileLeaveDays = leaveProfileDetail.Duration;
                        break;
                    case 2:
                        currentYearsBalances.AnnualProfileLeaveDays = leaveProfileDetail.Duration * 7;
                        break;
                    case 3:
                        currentYearsBalances.AnnualProfileLeaveDays = leaveProfileDetail.Duration * 30;
                        break;
                    case 4:
                        currentYearsBalances.AnnualProfileLeaveDays = leaveProfileDetail.Duration * 364;
                        break;
                    default:
                        break;
                }

                if (currentYearsBalances.PreviousBalanceCanBeCarriedOver && currentYearsBalances.PreviousBalanceExpiryMonth > DateTime.Today.Month)
                {
                    //Get the Calculated values of Leave Balance for the Previous Leave Year.
                    var previousBalanceEntity = await _leaveRepository.GetLeaveRollingBalanceByEmployeeIdAsync(EmployeeId, previousYearsBalances.LeaveYear, LeaveTypeCode);
                    if (previousBalanceEntity != null && previousBalanceEntity.TotalOutstandingLeaveDaysAfterExpiry > 0)
                    {
                        currentYearsBalances.PreviousYearsLeaveBalance = previousBalanceEntity.TotalOutstandingLeaveDaysAfterExpiry;
                    }

                    //Create a Transaction Object for the Opening Balance for the current Leave Year And Save to the Database
                    LeaveTransaction openingBalanceTransaction = new LeaveTransaction();
                    openingBalanceTransaction.LeaveDepartmentId = leaveEmployee.DepartmentID ?? 0;
                    openingBalanceTransaction.LeaveEmployeeId = leaveEmployee.EmployeeID;
                    openingBalanceTransaction.LeaveLocationId = leaveEmployee.LocationID ?? 0;
                    openingBalanceTransaction.LeaveTypeCode = LeaveTypeCode;
                    openingBalanceTransaction.LeaveUnitId = leaveEmployee.UnitID ?? 0;
                    openingBalanceTransaction.LeaveYear = LeaveYear;
                    openingBalanceTransaction.NumberOfDaysGiven = 0;
                    openingBalanceTransaction.NumberOfDaysUsed = 0;
                    openingBalanceTransaction.NumberOfDaysDeducted = 0;
                    openingBalanceTransaction.OpeningBalance = Convert.ToInt32(currentYearsBalances.AnnualProfileLeaveDays);
                    openingBalanceTransaction.PreviousBalance = Convert.ToInt32(currentYearsBalances.PreviousYearsLeaveBalance);
                    openingBalanceTransaction.TransactionDescription = $"Annual Leave Days Opening Balance for the Year {LeaveYear} added. ";
                    openingBalanceTransaction.TransactionDate = DateTime.UtcNow;
                    openingBalanceTransaction.TransactionRecordedBy = "System Service";
                    long newOpeningTransactionId = await _leaveRepository.AddLeaveTransactionAsync(openingBalanceTransaction);
                    if (newOpeningTransactionId > 0)
                    {
                        currentYearsBalances.TotalOutstandingLeaveDaysBeforeExpiry = openingBalanceTransaction.OpeningBalance + openingBalanceTransaction.PreviousBalance;
                        currentYearsBalances.TotalOutstandingLeaveDaysAfterExpiry = openingBalanceTransaction.OpeningBalance;

                        LeaveRollingBalance newRollingBalance = new LeaveRollingBalance();
                        newRollingBalance.AnnualProfileLeaveDays = openingBalanceTransaction.OpeningBalance;
                        newRollingBalance.LeaveBalanceDate = DateTime.UtcNow;
                        newRollingBalance.LeaveDaysAdded = openingBalanceTransaction.NumberOfDaysGiven;
                        newRollingBalance.LeaveDaysDeducted = openingBalanceTransaction.NumberOfDaysDeducted;
                        newRollingBalance.LeaveDaysUsed = openingBalanceTransaction.NumberOfDaysUsed;
                        newRollingBalance.LeaveDepartmentId = openingBalanceTransaction.LeaveDepartmentId;
                        newRollingBalance.LeaveDepartmentName = openingBalanceTransaction.LeaveDepartmentName;
                        newRollingBalance.LeaveEmployeeId = openingBalanceTransaction.LeaveEmployeeId;
                        newRollingBalance.LeaveEmployeeName = openingBalanceTransaction.LeaveEmployeeName;
                        newRollingBalance.LeaveLocationId = openingBalanceTransaction.LeaveLocationId;
                        newRollingBalance.LeaveLocationName = openingBalanceTransaction.LeaveLocationName;
                        newRollingBalance.LeaveTransactionId = newOpeningTransactionId;
                        newRollingBalance.LeaveTypeCode = openingBalanceTransaction.LeaveTypeCode;
                        newRollingBalance.LeaveTypeName = openingBalanceTransaction.LeaveTypeName;
                        newRollingBalance.LeaveUnitId = openingBalanceTransaction.LeaveUnitId;
                        newRollingBalance.LeaveUnitName = openingBalanceTransaction.LeaveUnitName;
                        newRollingBalance.LeaveYear = openingBalanceTransaction.LeaveYear;
                        newRollingBalance.PreviousBalanceCanBeCarriedOver = currentYearsBalances.PreviousBalanceCanBeCarriedOver;
                        newRollingBalance.PreviousBalanceExpiryMonth = currentYearsBalances.PreviousBalanceExpiryMonth;
                        newRollingBalance.PreviousYearsLeaveBalance = openingBalanceTransaction.PreviousBalance;
                        newRollingBalance.TotalOutstandingLeaveDaysBeforeExpiry = currentYearsBalances.TotalOutstandingLeaveDaysBeforeExpiry;
                        newRollingBalance.TotalOutstandingLeaveDaysAfterExpiry = currentYearsBalances.TotalOutstandingLeaveDaysAfterExpiry;

                        long newRollingBalanceId = await _leaveRepository.AddLeaveRollingBalanceAsync(newRollingBalance);
                        if (newRollingBalanceId < 1)
                        {
                            await _leaveRepository.DeleteLeaveTransactionAsync(newOpeningTransactionId);
                        }
                        else
                        {
                            currentYearsBalances = newRollingBalance;
                        }
                    }
                }
                else
                {
                    //Create a Transaction Object for the Opening Balance for the current Leave Year And Save to the Database
                    LeaveTransaction openingBalanceTransaction = new LeaveTransaction();
                    openingBalanceTransaction.LeaveDepartmentId = leaveEmployee.DepartmentID ?? 0;
                    openingBalanceTransaction.LeaveEmployeeId = leaveEmployee.EmployeeID;
                    openingBalanceTransaction.LeaveLocationId = leaveEmployee.LocationID ?? 0;
                    openingBalanceTransaction.LeaveTypeCode = LeaveTypeCode;
                    openingBalanceTransaction.LeaveUnitId = leaveEmployee.UnitID ?? 0;
                    openingBalanceTransaction.LeaveYear = LeaveYear;
                    openingBalanceTransaction.NumberOfDaysGiven = 0;
                    openingBalanceTransaction.NumberOfDaysUsed = 0;
                    openingBalanceTransaction.NumberOfDaysDeducted = 0;
                    openingBalanceTransaction.OpeningBalance = Convert.ToInt32(currentYearsBalances.AnnualProfileLeaveDays);
                    openingBalanceTransaction.PreviousBalance = 0;
                    openingBalanceTransaction.TransactionDescription = $"Annual Leave Days Opening Balance for the Year {LeaveYear} added. ";
                    openingBalanceTransaction.TransactionDate = DateTime.UtcNow;
                    openingBalanceTransaction.TransactionRecordedBy = "System Service";
                    long newOpeningTransactionId = await _leaveRepository.AddLeaveTransactionAsync(openingBalanceTransaction);
                    if (newOpeningTransactionId > 0)
                    {
                        currentYearsBalances.TotalOutstandingLeaveDaysBeforeExpiry = openingBalanceTransaction.OpeningBalance + openingBalanceTransaction.PreviousBalance;
                        currentYearsBalances.TotalOutstandingLeaveDaysAfterExpiry = openingBalanceTransaction.OpeningBalance;

                        LeaveRollingBalance newRollingBalance = new LeaveRollingBalance();
                        newRollingBalance.AnnualProfileLeaveDays = openingBalanceTransaction.OpeningBalance;
                        newRollingBalance.LeaveBalanceDate = DateTime.UtcNow;
                        newRollingBalance.LeaveDaysAdded = openingBalanceTransaction.NumberOfDaysGiven;
                        newRollingBalance.LeaveDaysDeducted = openingBalanceTransaction.NumberOfDaysDeducted;
                        newRollingBalance.LeaveDaysUsed = openingBalanceTransaction.NumberOfDaysUsed;
                        newRollingBalance.LeaveDepartmentId = openingBalanceTransaction.LeaveDepartmentId;
                        newRollingBalance.LeaveDepartmentName = openingBalanceTransaction.LeaveDepartmentName;
                        newRollingBalance.LeaveEmployeeId = openingBalanceTransaction.LeaveEmployeeId;
                        newRollingBalance.LeaveEmployeeName = openingBalanceTransaction.LeaveEmployeeName;
                        newRollingBalance.LeaveLocationId = openingBalanceTransaction.LeaveLocationId;
                        newRollingBalance.LeaveLocationName = openingBalanceTransaction.LeaveLocationName;
                        newRollingBalance.LeaveTransactionId = newOpeningTransactionId;
                        newRollingBalance.LeaveTypeCode = openingBalanceTransaction.LeaveTypeCode;
                        newRollingBalance.LeaveTypeName = openingBalanceTransaction.LeaveTypeName;
                        newRollingBalance.LeaveUnitId = openingBalanceTransaction.LeaveUnitId;
                        newRollingBalance.LeaveUnitName = openingBalanceTransaction.LeaveUnitName;
                        newRollingBalance.LeaveYear = openingBalanceTransaction.LeaveYear;
                        newRollingBalance.PreviousBalanceCanBeCarriedOver = currentYearsBalances.PreviousBalanceCanBeCarriedOver;
                        newRollingBalance.PreviousBalanceExpiryMonth = currentYearsBalances.PreviousBalanceExpiryMonth;
                        newRollingBalance.PreviousYearsLeaveBalance = openingBalanceTransaction.PreviousBalance;
                        newRollingBalance.TotalOutstandingLeaveDaysBeforeExpiry = currentYearsBalances.TotalOutstandingLeaveDaysBeforeExpiry;
                        newRollingBalance.TotalOutstandingLeaveDaysAfterExpiry = currentYearsBalances.TotalOutstandingLeaveDaysAfterExpiry;

                        long newRollingBalanceId = await _leaveRepository.AddLeaveRollingBalanceAsync(newRollingBalance);
                        if (newRollingBalanceId < 1)
                        {
                            await _leaveRepository.DeleteLeaveTransactionAsync(newOpeningTransactionId);
                        }
                        else
                        {
                            currentYearsBalances = newRollingBalance;
                        }

                    }
                }
            }
            else
            {
                currentYearsBalances = existingBalances;
            }
            return currentYearsBalances;
        }

        public async Task<bool> RefreshAllEmployeesAnnualLeaveBalances(int LeaveYear)
        {
            IEnumerable<EmployeeRoll> entities = await _employeesRepository.GetEmployeeRollsByAllAsync(DateTime.UtcNow);
            List<EmployeeRoll> listEmployeeRolls = entities.ToList();
            if (listEmployeeRolls != null && listEmployeeRolls.Count > 0)
            {
                int noOfEmployees = listEmployeeRolls.Count;
                int rowCount = 0;
                foreach (var employee in listEmployeeRolls)
                {
                    await RefreshEmployeeBalances("ANL", LeaveYear, employee.EmployeeID);
                    rowCount++;
                }
                if (rowCount == noOfEmployees) { return true; }
            }
            return false;
        }

        private async Task<bool> RefreshEmployeeBalances(string LeaveTypeCode, int LeaveYear, string EmployeeId)
        {
            if (string.IsNullOrWhiteSpace(LeaveTypeCode)) { throw new Exception("Required parameter Leave Type Code has an invalid value."); }
            if (LeaveYear < 2020) { throw new Exception("Required parameter Leave Year has an invalid value."); }
            if (string.IsNullOrWhiteSpace(EmployeeId)) { throw new Exception("Required parameter Employee ID have invalid values."); }

            Employee leaveEmployee = new Employee();
            if (!string.IsNullOrWhiteSpace(EmployeeId))
            {
                leaveEmployee = await _employeesRepository.GetEmployeeByIdAsync(EmployeeId);
            }


            LeaveRollingBalance previousYearsBalances = new LeaveRollingBalance();
            previousYearsBalances.LeaveYear = LeaveYear - 1;

            LeaveRollingBalance currentYearsBalances = new LeaveRollingBalance();
            currentYearsBalances.LeaveYear = LeaveYear;
            currentYearsBalances.LeaveTypeCode = previousYearsBalances.LeaveTypeCode = LeaveTypeCode;

            LeaveRollingBalance existingBalances = await _leaveRepository.GetLeaveRollingBalanceByEmployeeIdAsync(leaveEmployee.EmployeeID, LeaveYear, LeaveTypeCode);
            if (existingBalances == null || existingBalances.AnnualProfileLeaveDays < 1)
            {
                //Get Leave Details eg Duration, CanBeCarriedOver and CarryOverExpiryMonth
                LeaveProfileDetail leaveProfileDetail = await _leaveRepository.GetLeaveProfileDetailByEmployeeNamenLeaveTypeAsync(leaveEmployee.FullName, LeaveTypeCode);
                if (leaveProfileDetail == null) { throw new Exception("No Leave Profile was found for this employee. Please ensure a Link Profile has been set up for this employee."); }
                currentYearsBalances.PreviousBalanceCanBeCarriedOver = leaveProfileDetail.CanBeCarriedOver;
                currentYearsBalances.PreviousBalanceExpiryMonth = leaveProfileDetail.CarryOverEndMonth ?? 0;

                switch (leaveProfileDetail.DurationTypeId)
                {
                    case 0:
                    case 1:
                        currentYearsBalances.AnnualProfileLeaveDays = leaveProfileDetail.Duration;
                        break;
                    case 2:
                        currentYearsBalances.AnnualProfileLeaveDays = leaveProfileDetail.Duration * 7;
                        break;
                    case 3:
                        currentYearsBalances.AnnualProfileLeaveDays = leaveProfileDetail.Duration * 30;
                        break;
                    case 4:
                        currentYearsBalances.AnnualProfileLeaveDays = leaveProfileDetail.Duration * 364;
                        break;
                    default:
                        break;
                }

                if (currentYearsBalances.PreviousBalanceCanBeCarriedOver && currentYearsBalances.PreviousBalanceExpiryMonth > DateTime.Today.Month)
                {
                    //Get the Calculated values of Leave Balance for the Previous Leave Year.
                    var previousBalanceEntity = await _leaveRepository.GetLeaveRollingBalanceByEmployeeIdAsync(EmployeeId, previousYearsBalances.LeaveYear, LeaveTypeCode);
                    if (previousBalanceEntity != null && previousBalanceEntity.TotalOutstandingLeaveDaysAfterExpiry > 0)
                    {
                        currentYearsBalances.PreviousYearsLeaveBalance = previousBalanceEntity.TotalOutstandingLeaveDaysAfterExpiry;
                    }

                    //Create a Transaction Object for the Opening Balance for the current Leave Year And Save to the Database
                    LeaveTransaction openingBalanceTransaction = new LeaveTransaction();
                    openingBalanceTransaction.LeaveDepartmentId = leaveEmployee.DepartmentID ?? 0;
                    openingBalanceTransaction.LeaveEmployeeId = leaveEmployee.EmployeeID;
                    openingBalanceTransaction.LeaveLocationId = leaveEmployee.LocationID ?? 0;
                    openingBalanceTransaction.LeaveTypeCode = LeaveTypeCode;
                    openingBalanceTransaction.LeaveUnitId = leaveEmployee.UnitID ?? 0;
                    openingBalanceTransaction.LeaveYear = LeaveYear;
                    openingBalanceTransaction.NumberOfDaysGiven = 0;
                    openingBalanceTransaction.NumberOfDaysUsed = 0;
                    openingBalanceTransaction.NumberOfDaysDeducted = 0;
                    openingBalanceTransaction.OpeningBalance = Convert.ToInt32(currentYearsBalances.AnnualProfileLeaveDays);
                    openingBalanceTransaction.PreviousBalance = Convert.ToInt32(currentYearsBalances.PreviousYearsLeaveBalance);
                    openingBalanceTransaction.TransactionDescription = $"Annual Leave Days Opening Balance for the Year {LeaveYear} added. ";
                    openingBalanceTransaction.TransactionDate = DateTime.UtcNow;
                    openingBalanceTransaction.TransactionRecordedBy = "System Service";
                    long newOpeningTransactionId = await _leaveRepository.AddLeaveTransactionAsync(openingBalanceTransaction);
                    if (newOpeningTransactionId > 0)
                    {
                        currentYearsBalances.TotalOutstandingLeaveDaysBeforeExpiry = openingBalanceTransaction.OpeningBalance + openingBalanceTransaction.PreviousBalance;
                        currentYearsBalances.TotalOutstandingLeaveDaysAfterExpiry = openingBalanceTransaction.OpeningBalance;

                        LeaveRollingBalance newRollingBalance = new LeaveRollingBalance();
                        newRollingBalance.AnnualProfileLeaveDays = openingBalanceTransaction.OpeningBalance;
                        newRollingBalance.LeaveBalanceDate = DateTime.UtcNow;
                        newRollingBalance.LeaveDaysAdded = openingBalanceTransaction.NumberOfDaysGiven;
                        newRollingBalance.LeaveDaysDeducted = openingBalanceTransaction.NumberOfDaysDeducted;
                        newRollingBalance.LeaveDaysUsed = openingBalanceTransaction.NumberOfDaysUsed;
                        newRollingBalance.LeaveDepartmentId = openingBalanceTransaction.LeaveDepartmentId;
                        newRollingBalance.LeaveDepartmentName = openingBalanceTransaction.LeaveDepartmentName;
                        newRollingBalance.LeaveEmployeeId = openingBalanceTransaction.LeaveEmployeeId;
                        newRollingBalance.LeaveEmployeeName = openingBalanceTransaction.LeaveEmployeeName;
                        newRollingBalance.LeaveLocationId = openingBalanceTransaction.LeaveLocationId;
                        newRollingBalance.LeaveLocationName = openingBalanceTransaction.LeaveLocationName;
                        newRollingBalance.LeaveTransactionId = newOpeningTransactionId;
                        newRollingBalance.LeaveTypeCode = openingBalanceTransaction.LeaveTypeCode;
                        newRollingBalance.LeaveTypeName = openingBalanceTransaction.LeaveTypeName;
                        newRollingBalance.LeaveUnitId = openingBalanceTransaction.LeaveUnitId;
                        newRollingBalance.LeaveUnitName = openingBalanceTransaction.LeaveUnitName;
                        newRollingBalance.LeaveYear = openingBalanceTransaction.LeaveYear;
                        newRollingBalance.PreviousBalanceCanBeCarriedOver = currentYearsBalances.PreviousBalanceCanBeCarriedOver;
                        newRollingBalance.PreviousBalanceExpiryMonth = currentYearsBalances.PreviousBalanceExpiryMonth;
                        newRollingBalance.PreviousYearsLeaveBalance = openingBalanceTransaction.PreviousBalance;
                        newRollingBalance.TotalOutstandingLeaveDaysBeforeExpiry = currentYearsBalances.TotalOutstandingLeaveDaysBeforeExpiry;
                        newRollingBalance.TotalOutstandingLeaveDaysAfterExpiry = currentYearsBalances.TotalOutstandingLeaveDaysAfterExpiry;

                        long newRollingBalanceId = await _leaveRepository.AddLeaveRollingBalanceAsync(newRollingBalance);
                        if (newRollingBalanceId < 1)
                        {
                            await _leaveRepository.DeleteLeaveTransactionAsync(newOpeningTransactionId);
                        }
                        else
                        {
                            currentYearsBalances = newRollingBalance;
                        }
                    }
                }
                else
                {
                    //Create a Transaction Object for the Opening Balance for the current Leave Year And Save to the Database
                    LeaveTransaction openingBalanceTransaction = new LeaveTransaction();
                    openingBalanceTransaction.LeaveDepartmentId = leaveEmployee.DepartmentID ?? 0;
                    openingBalanceTransaction.LeaveEmployeeId = leaveEmployee.EmployeeID;
                    openingBalanceTransaction.LeaveLocationId = leaveEmployee.LocationID ?? 0;
                    openingBalanceTransaction.LeaveTypeCode = LeaveTypeCode;
                    openingBalanceTransaction.LeaveUnitId = leaveEmployee.UnitID ?? 0;
                    openingBalanceTransaction.LeaveYear = LeaveYear;
                    openingBalanceTransaction.NumberOfDaysGiven = 0;
                    openingBalanceTransaction.NumberOfDaysUsed = 0;
                    openingBalanceTransaction.NumberOfDaysDeducted = 0;
                    openingBalanceTransaction.OpeningBalance = Convert.ToInt32(currentYearsBalances.AnnualProfileLeaveDays);
                    openingBalanceTransaction.PreviousBalance = 0;
                    openingBalanceTransaction.TransactionDescription = $"Annual Leave Days Opening Balance for the Year {LeaveYear} added. ";
                    openingBalanceTransaction.TransactionDate = DateTime.UtcNow;
                    openingBalanceTransaction.TransactionRecordedBy = "System Service";
                    long newOpeningTransactionId = await _leaveRepository.AddLeaveTransactionAsync(openingBalanceTransaction);
                    if (newOpeningTransactionId > 0)
                    {
                        currentYearsBalances.TotalOutstandingLeaveDaysBeforeExpiry = openingBalanceTransaction.OpeningBalance + openingBalanceTransaction.PreviousBalance;
                        currentYearsBalances.TotalOutstandingLeaveDaysAfterExpiry = openingBalanceTransaction.OpeningBalance;

                        LeaveRollingBalance newRollingBalance = new LeaveRollingBalance();
                        newRollingBalance.AnnualProfileLeaveDays = openingBalanceTransaction.OpeningBalance;
                        newRollingBalance.LeaveBalanceDate = DateTime.UtcNow;
                        newRollingBalance.LeaveDaysAdded = openingBalanceTransaction.NumberOfDaysGiven;
                        newRollingBalance.LeaveDaysDeducted = openingBalanceTransaction.NumberOfDaysDeducted;
                        newRollingBalance.LeaveDaysUsed = openingBalanceTransaction.NumberOfDaysUsed;
                        newRollingBalance.LeaveDepartmentId = openingBalanceTransaction.LeaveDepartmentId;
                        newRollingBalance.LeaveDepartmentName = openingBalanceTransaction.LeaveDepartmentName;
                        newRollingBalance.LeaveEmployeeId = openingBalanceTransaction.LeaveEmployeeId;
                        newRollingBalance.LeaveEmployeeName = openingBalanceTransaction.LeaveEmployeeName;
                        newRollingBalance.LeaveLocationId = openingBalanceTransaction.LeaveLocationId;
                        newRollingBalance.LeaveLocationName = openingBalanceTransaction.LeaveLocationName;
                        newRollingBalance.LeaveTransactionId = newOpeningTransactionId;
                        newRollingBalance.LeaveTypeCode = openingBalanceTransaction.LeaveTypeCode;
                        newRollingBalance.LeaveTypeName = openingBalanceTransaction.LeaveTypeName;
                        newRollingBalance.LeaveUnitId = openingBalanceTransaction.LeaveUnitId;
                        newRollingBalance.LeaveUnitName = openingBalanceTransaction.LeaveUnitName;
                        newRollingBalance.LeaveYear = openingBalanceTransaction.LeaveYear;
                        newRollingBalance.PreviousBalanceCanBeCarriedOver = currentYearsBalances.PreviousBalanceCanBeCarriedOver;
                        newRollingBalance.PreviousBalanceExpiryMonth = currentYearsBalances.PreviousBalanceExpiryMonth;
                        newRollingBalance.PreviousYearsLeaveBalance = openingBalanceTransaction.PreviousBalance;
                        newRollingBalance.TotalOutstandingLeaveDaysBeforeExpiry = currentYearsBalances.TotalOutstandingLeaveDaysBeforeExpiry;
                        newRollingBalance.TotalOutstandingLeaveDaysAfterExpiry = currentYearsBalances.TotalOutstandingLeaveDaysAfterExpiry;

                        long newRollingBalanceId = await _leaveRepository.AddLeaveRollingBalanceAsync(newRollingBalance);
                        if (newRollingBalanceId < 1)
                        {
                            await _leaveRepository.DeleteLeaveTransactionAsync(newOpeningTransactionId);
                        }
                        else
                        {
                            currentYearsBalances = newRollingBalance;
                        }

                    }
                }
            }
            else
            {
                currentYearsBalances = existingBalances;
            }
            if (currentYearsBalances != null && currentYearsBalances.AnnualProfileLeaveDays > 0)
            {
                return true;
            }
            else { return false; }
        }

        #endregion

        #region Leave Submission Service Methods
        public async Task<bool> SubmitLeaveAsync(LeaveSubmission e)
        {
            bool IsSubmitted = false;

            //string DocumentType = string.Empty;
            if (e.LeavePlanId > 0) { e.DocumentType = "Leave Plan"; }
            else if (e.LeaveRequestId > 0) { e.DocumentType = "Leave Application"; }
            else { e.DocumentType = "Unknown"; }
            if (e != null)
            {
                //if (e.LeaveRequestId > 0)
                //{
                //    var existingSubmissions = await _leaveRepository.GetLeaveSubmissionsByRequestIdnRolenPurposeAsync(e.LeaveRequestId.Value, e.ToEmployeeRole, e.Purpose);
                //    if (existingSubmissions != null && existingSubmissions.Count > 0) { throw new Exception($"Double Submission! You have already submitted this Request for {e.ToEmployeeRole} {e.Purpose}."); }
                //}

                IsSubmitted = await _leaveRepository.AddLeaveSubmissionAsync(e);
                if (IsSubmitted)
                {
                    LeaveActivityLog history = new LeaveActivityLog();
                    history.ActivityDescription = $"{e.DocumentType} was submitted to {e.ToEmployeeName} by {e.FromEmployeeName} on {DateTime.Now.ToLongDateString()} at {DateTime.Now.ToLongTimeString()}";
                    history.ActivityTime = DateTime.UtcNow;
                    history.LeavePlanId = e.LeavePlanId;
                    history.LeaveRequestId = e.LeaveRequestId;
                    await _leaveRepository.AddLeaveActivityLogAsync(history);
                }
            }
            else { throw new Exception("Required parameter [Leave Submission] has invalid value."); }
            return IsSubmitted;
        }
        public async Task<bool> DeleteLeaveSubmissionAsync(long LeaveSubmissionId)
        {
            return await _leaveRepository.DeleteSubmissionAsync(LeaveSubmissionId);
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

        public async Task<List<LeaveSubmission>> GetLeaveSubmissionsByApproverRoleAsync(string approverRole, int? submittedYear = null)
        {
            List<LeaveSubmission> leaveSubmissions = new List<LeaveSubmission>();
            if (submittedYear == null || submittedYear < 2020) { submittedYear = DateTime.Now.Year; }
            var entities = await _leaveRepository.GetLeaveSubmissionsByRolenYearSubmittedAsync(approverRole, submittedYear.Value);
            if (entities != null && entities.Count > 0)
            {
                leaveSubmissions = entities;
            }
            return leaveSubmissions;
        }

        #endregion

        #region Leave Approval Service Methods
        public async Task<bool> ApproveLeaveAsync(LeaveApproval a, LeaveSubmission s, DocumentType t)
        {
            bool IsUpdated = false;
            int newStatusId = 0;
            string newStatusDescription = "";
            if (a == null) { throw new Exception("Required parameter [Leave Approval] has invalid value."); }

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

            long approvalId = await _leaveRepository.AddLeaveApprovalAsync(a);
            if (approvalId < 1) { throw new Exception("Sorry an error was encountered while attempting to add the approval record."); }

            if (t == DocumentType.LeavePlan)
            {
                if (await _leaveRepository.UpdateSubmissionActionStatusAsync(s.LeaveSubmissionId, a.TimeApproved))
                {
                    //====== Add Leave Activity Log =======//
                    LeaveActivityLog history = new LeaveActivityLog();
                    history.ActivityDescription = $"Leave Plan was approved by {a.ApproverName} on {DateTime.UtcNow.ToLongDateString()} at {DateTime.UtcNow.ToLongTimeString()}.";
                    history.ActivityTime = DateTime.UtcNow;
                    history.LeavePlanId = a.LeavePlanId;
                    await _leaveRepository.AddLeaveActivityLogAsync(history);

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
                }
                return true;
            }
            else if (t == DocumentType.LeaveRequest)
            {
                //Update Leave Request Status to Pending
                IsUpdated = await _leaveRepository.UpdateLeaveRequestStatusAsync(a.LeaveRequestId.Value, newStatusId);
                if (!IsUpdated)
                {
                    await _leaveRepository.DeleteApprovalAsync(approvalId);
                    throw new Exception("An error was encountered while attempting to update Leave Plan status.");
                }

                if (a.IsApproved)
                {
                    switch (s.ToEmployeeRole)
                    {
                        case "Line Manager":
                            await _leaveRepository.UpdateLeaveRequestApprovalStatusAsync(a.LeaveRequestId.Value, Enums.ApprovalType.LineManager);
                            break;
                        case "Head of Department":
                            await _leaveRepository.UpdateLeaveRequestApprovalStatusAsync(a.LeaveRequestId.Value, Enums.ApprovalType.HeadofDepartment);
                            break;
                        case "Station Manager":
                            await _leaveRepository.UpdateLeaveRequestApprovalStatusAsync(a.LeaveRequestId.Value, Enums.ApprovalType.StationManager);
                            break;
                        case "HR Department":
                            await _leaveRepository.UpdateLeaveRequestApprovalStatusAsync(a.LeaveRequestId.Value, Enums.ApprovalType.HrDepartment);
                            break;
                        case "Executive Management":
                            await _leaveRepository.UpdateLeaveRequestApprovalStatusAsync(a.LeaveRequestId.Value, Enums.ApprovalType.ExecutiveManagement);
                            break;
                        default:
                            break;
                    }
                }

                if (await _leaveRepository.UpdateSubmissionActionStatusAsync(s.LeaveSubmissionId, a.TimeApproved))
                {
                    //====== Add Leave Activity Log =======//
                    LeaveActivityLog history = new LeaveActivityLog();
                    history.ActivityDescription = $"Leave Request was approved by {a.ApproverName} on {DateTime.UtcNow.ToLongDateString()} at {DateTime.UtcNow.ToLongTimeString()}.";
                    history.ActivityTime = DateTime.UtcNow;
                    history.LeaveRequestId = a.LeaveRequestId;
                    await _leaveRepository.AddLeaveActivityLogAsync(history);

                    LeaveNote note = new LeaveNote();
                    //====== Add Leave Note =======//
                    if (!string.IsNullOrWhiteSpace(a.ApproverComments))
                    {
                        note.NoteContent = a.ApproverComments;
                        note.LeaveRequestId = a.LeaveRequestId;
                        note.TimeAdded = DateTime.Now;
                        note.FromEmployeeName = a.ApproverName;
                        await _leaveRepository.AddNoteAsync(note);
                    }
                }
                return true;
            }
            else { throw new Exception("Error: Unknown document type. The document type was not specified."); }
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
        public async Task<bool> DeclineLeaveAsync(LeaveApproval a, LeaveSubmission s, DocumentType t)
        {
            bool IsUpdated = false;
            int newStatusId = 0;
            string newStatusDescription = "";
            if (a == null) { throw new Exception("Required parameter [Leave Approval] has invalid value."); }
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

            if (t == DocumentType.LeavePlan)
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
            else if (t == DocumentType.LeaveRequest)
            {
                //Update Leave Request Status to Pending
                IsUpdated = await _leaveRepository.UpdateLeaveRequestStatusAsync(a.LeaveRequestId.Value, newStatusId);
                if (IsUpdated)
                {
                    await _leaveRepository.UpdateSubmissionActionStatusAsync(s.LeaveSubmissionId, a.TimeApproved);

                    LeaveNote note = new LeaveNote();
                    //====== Add Leave Note =======//
                    if (!string.IsNullOrWhiteSpace(a.ApproverComments))
                    {
                        note.NoteContent = a.ApproverComments;
                        note.LeaveRequestId = a.LeaveRequestId;
                        note.TimeAdded = DateTime.Now;
                        note.FromEmployeeName = a.ApproverName;
                        await _leaveRepository.AddNoteAsync(note);
                    }

                    //====== Add Leave Activity Log =======//
                    LeaveActivityLog history = new LeaveActivityLog();
                    history.ActivityDescription = $"Leave Request was declined by {a.ApproverName} on {DateTime.UtcNow.ToLongDateString()} at {DateTime.UtcNow.ToLongTimeString()}.";
                    history.ActivityTime = DateTime.UtcNow;
                    history.LeaveRequestId = a.LeaveRequestId;
                    await _leaveRepository.AddLeaveActivityLogAsync(history);

                    return true;
                }
                else
                {
                    throw new Exception("An error was encountered while attempting to update Leave Plan status.");
                }
            }
            else
            {
                throw new Exception("Error: Unknown document type. The document type was not specified. ");
            }
        }

        #endregion

        #region Leave Documents Service Methods
        public async Task<bool> AddLeaveDocumentAsync(LeaveDocument document)
        {
            if (document == null) { throw new Exception("Required parameter [Leave Document] has invalid value."); }

            long documentId = await _leaveRepository.AddLeaveDocumentAsync(document);
            if (documentId < 1) { throw new Exception("Sorry an error was encountered while attempting to add this document."); }

            //====== Add Leave Activity Log =======//
            LeaveActivityLog history = new LeaveActivityLog();
            history.ActivityDescription = $"Uploaded a document with title [{document.DocumentTitle}] on {DateTime.UtcNow.ToLongDateString()} at {DateTime.UtcNow.ToLongTimeString()}.";
            history.ActivityTime = DateTime.UtcNow;
            history.LeaveRequestId = document.LeaveRequestId;
            await _leaveRepository.AddLeaveActivityLogAsync(history);

            return true;
        }
        public async Task<bool> DeleteLeaveDocumentAsync(long LeaveDocumentId)
        {
            if (LeaveDocumentId < 1) { throw new Exception("Required parameter [Leave Document ID] has invalid value."); }

            LeaveDocument document = await _leaveRepository.GetLeaveDocumentByIdAsync(LeaveDocumentId);

            bool IsDeleted = await _leaveRepository.DeleteLeaveDocumentAsync(LeaveDocumentId);
            if (!IsDeleted) { throw new Exception("Sorry an error was encountered while attempting to delete this document."); }

            //====== Add Leave Activity Log =======//
            LeaveActivityLog history = new LeaveActivityLog();
            history.ActivityDescription = $"Document with title [{document.DocumentTitle}] on {DateTime.UtcNow.ToLongDateString()} at {DateTime.UtcNow.ToLongTimeString()}.";
            history.ActivityTime = DateTime.UtcNow;
            history.LeaveRequestId = document.LeaveRequestId;
            await _leaveRepository.AddLeaveActivityLogAsync(history);

            return true;
        }

        public async Task<List<LeaveDocument>> GetLeaveDocumentsAsync(long LeaveRequestId)
        {
            List<LeaveDocument> documentsList = new List<LeaveDocument>();
            if (LeaveRequestId > 0)
            {
                var entities = await _leaveRepository.GetLeaveDocumentsByLeaveRequestIdAsync(LeaveRequestId);
                if (entities != null && entities.Count > 0)
                {
                    documentsList = entities;
                }
            }
            return documentsList;
        }
        public async Task<LeaveDocument> GetLeaveDocumentAsync(long LeaveDocumentId)
        {
            LeaveDocument document = new LeaveDocument();
            if (LeaveDocumentId > 0)
            {
                var entity = await _leaveRepository.GetLeaveDocumentByIdAsync(LeaveDocumentId);
                if (entity != null)
                {
                    document = entity;
                }
            }
            return document;
        }


        #endregion

        #region Leave Resumption Service Methods
        public async Task<bool> SubmitLeaveResumptionNoticeAsync(LeaveResumption u, string SendToEmployeeName, string SendToEmployeeRole)
        {
            if (u == null) { throw new ArgumentNullException(nameof(u)); }
            long leaveResumptionId = 0;
            try
            {
                leaveResumptionId = await _leaveRepository.AddLeaveResumptionAsync(u);
                if (leaveResumptionId > 0)
                {
                    if (await _leaveRepository.UpdateLeaveRequestAdjustmentRequestAsync(u.LeaveRequestId, true))
                    {
                        LeaveSubmission leaveSubmission = new LeaveSubmission();
                        leaveSubmission.LeaveRequestId = u.LeaveRequestId;
                        leaveSubmission.FromEmployeeName = u.LeaveEmployeeName;
                        leaveSubmission.Purpose = "Confirm Resumption";
                        leaveSubmission.ToEmployeeName = SendToEmployeeName;
                        leaveSubmission.ToEmployeeRole = SendToEmployeeRole;
                        leaveSubmission.DocumentType = "Resumption Notice";
                        leaveSubmission.Message = string.Empty;

                        if (await _leaveRepository.UpdateLeaveRequestStatusAsync(u.LeaveRequestId, (int)LeaveStatusEnum.ResumptionNotice))
                        {
                            if (await _leaveRepository.AddLeaveSubmissionAsync(leaveSubmission))
                            {
                                //====== Add Activity History =======//
                                LeaveActivityLog history = new LeaveActivityLog();
                                history.ActivityDescription = $"Resumption Notice was sent to {SendToEmployeeName} on {DateTime.UtcNow.ToLongDateString()} at exactly {DateTime.UtcNow.ToLongTimeString()} WAT.";
                                history.ActivityTime = DateTime.Now;
                                history.LeaveRequestId = u.LeaveRequestId;
                                await _leaveRepository.AddLeaveActivityLogAsync(history);
                            }
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await _leaveRepository.DeleteLeaveResumptionAsync(leaveResumptionId);
                throw new Exception(ex.Message, ex.InnerException);
            }
            return false;
        }
        public async Task<bool> ConfirmLeaveResumptionAsync(LeaveResumption u, long LeaveSubmissionId)
        {
            if (u == null) { throw new ArgumentNullException(nameof(u)); }
            bool IsUpdated = false;
            try
            {
                IsUpdated = await _leaveRepository.UpdateLeaveResumptionByLineManagerAsync(u.LeaveResumptionId, u.LineManagerName, u.ResumptionDateByLineManager.Value, u.NoOfExtraDaysByLineManager, u.NoOfUnusedDaysByLineManager, u.ReasonByLineManager, u.LineManagerApprovesAdjustment);
                if (IsUpdated)
                {
                    if (await _leaveRepository.UpdateSubmissionActionStatusAsync(LeaveSubmissionId, DateTime.UtcNow))
                    {
                        //====== Add Activity History =======//
                        LeaveActivityLog history = new LeaveActivityLog();
                        history.ActivityDescription = $"Resumption confirmation was submitted by {u.LineManagerName} on {DateTime.UtcNow.ToLongDateString()} at exactly {DateTime.UtcNow.ToLongTimeString()} WAT.";
                        history.ActivityTime = DateTime.Now;
                        history.LeaveRequestId = u.LeaveRequestId;
                        await _leaveRepository.AddLeaveActivityLogAsync(history);
                        return true;
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
            return false;
        }
        public async Task<LeaveResumption> GetLeaveResumptionAsync(long LeaveRequestId, long LeaveResumptionId)
        {
            LeaveResumption leaveResumption = new LeaveResumption();
            if (LeaveRequestId > 0)
            {
                leaveResumption = await _leaveRepository.GetLeaveResumptionByLeaveRequestIdAsync(LeaveRequestId);
            }
            else if (LeaveResumptionId > 0)
            {
                leaveResumption = await _leaveRepository.GetLeaveResumptionByLeaveResumptionIdAsync(LeaveResumptionId);
            }
            return leaveResumption;
        }
        #endregion

        #region Leave Adjustments Service Methods
        public async Task<List<LeaveAdjustment>> GetLeaveAdjustmentsAsync(long LeaveRequestId)
        {
            List<LeaveAdjustment> adjustments = new List<LeaveAdjustment>();
            if (LeaveRequestId > 0)
            {
                adjustments = await _leaveRepository.GetLeaveAdjustmentsByLeaveRequestIdAsync(LeaveRequestId);
            }
            return adjustments;
        }
        public async Task<LeaveAdjustment> GetLeaveAdjustmentAsync(long LeaveAdjustmentId)
        {
            LeaveAdjustment adjustment = new LeaveAdjustment();
            if (LeaveAdjustmentId > 0)
            {
                adjustment = await _leaveRepository.GetLeaveAdjustmentByIdAsync(LeaveAdjustmentId);
            }
            return adjustment;
        }
        public async Task<bool> AddLeaveAdjustmentAsync(LeaveAdjustment adjustment)
        {
            if (adjustment == null) { throw new Exception("Leave Adjustment has an invalid value."); }
            LeaveTransaction leaveTransaction = new LeaveTransaction();
            leaveTransaction.LeaveDepartmentId = adjustment.LeaveDepartmentId;
            leaveTransaction.LeaveEmployeeId = adjustment.LeaveEmployeeId;
            leaveTransaction.LeaveLocationId = adjustment.LeaveLocationId;
            leaveTransaction.LeaveRequestId = adjustment.LeaveRequestId;
            leaveTransaction.LeaveTypeCode = adjustment.LeaveTypeCode;
            leaveTransaction.LeaveUnitId = adjustment.LeaveUnitId;
            leaveTransaction.LeaveYear = adjustment.LeaveYear;

            leaveTransaction.TransactionDate = adjustment.AdjustmentDate;
            leaveTransaction.TransactionDescription = "Leave Adjustment";
            leaveTransaction.TransactionRecordedBy = adjustment.AdjustmentAddedBy;
            if (adjustment.AdjustmentType == "Addition") { leaveTransaction.NumberOfDaysGiven = adjustment.NumberOfDays; }
            else if (adjustment.AdjustmentType == "Deduction") { leaveTransaction.NumberOfDaysDeducted = adjustment.NumberOfDays; }

            adjustment.LeaveAdjustmentId = await _leaveRepository.AddLeaveAdjustmentAsync(adjustment);
            if (adjustment.LeaveAdjustmentId < 1) { throw new Exception("An error was encountered. Leave Adjustment could not be added."); }
            leaveTransaction.LeaveAdjustmentId = adjustment.LeaveAdjustmentId;

            leaveTransaction.LeaveTransactionId = await _leaveRepository.AddLeaveTransactionAsync(leaveTransaction);
            if (leaveTransaction.LeaveTransactionId < 1)
            {
                await _leaveRepository.DeleteLeaveAdjustmentAsync(adjustment.LeaveAdjustmentId);
                throw new Exception("An error was encountered. Leave Transaction could not be added. ");
            }

            LeaveRollingBalance previousLeaveRollingBalance = new LeaveRollingBalance();
            LeaveRollingBalance currentRollingBalance = new LeaveRollingBalance();
            previousLeaveRollingBalance = await _leaveRepository.GetLeaveRollingBalanceByEmployeeIdAsync(adjustment.LeaveEmployeeId, adjustment.LeaveYear, adjustment.LeaveTypeCode);
            if (previousLeaveRollingBalance == null)
            {
                await _leaveRepository.DeleteLeaveTransactionAsync(leaveTransaction.LeaveTransactionId);
                await _leaveRepository.DeleteLeaveAdjustmentAsync(adjustment.LeaveAdjustmentId);
                return false;
            }
            else
            {
                currentRollingBalance = previousLeaveRollingBalance;
                currentRollingBalance.LeaveBalanceDate = DateTime.UtcNow;
                currentRollingBalance.LeaveTransactionId = leaveTransaction.LeaveTransactionId;

                if (adjustment.AdjustmentType == "Addition")
                {
                    currentRollingBalance.LeaveDaysAdded = leaveTransaction.NumberOfDaysGiven + previousLeaveRollingBalance.LeaveDaysAdded;
                    currentRollingBalance.TotalOutstandingLeaveDaysBeforeExpiry = leaveTransaction.NumberOfDaysGiven + previousLeaveRollingBalance.TotalOutstandingLeaveDaysBeforeExpiry;
                    currentRollingBalance.TotalOutstandingLeaveDaysAfterExpiry = leaveTransaction.NumberOfDaysGiven + previousLeaveRollingBalance.TotalOutstandingLeaveDaysAfterExpiry;
                }
                else if (adjustment.AdjustmentType == "Deduction")
                {
                    currentRollingBalance.LeaveDaysDeducted = previousLeaveRollingBalance.LeaveDaysDeducted + leaveTransaction.NumberOfDaysDeducted;
                    currentRollingBalance.TotalOutstandingLeaveDaysBeforeExpiry = previousLeaveRollingBalance.TotalOutstandingLeaveDaysBeforeExpiry - leaveTransaction.NumberOfDaysDeducted;
                    currentRollingBalance.TotalOutstandingLeaveDaysAfterExpiry = previousLeaveRollingBalance.TotalOutstandingLeaveDaysAfterExpiry - leaveTransaction.NumberOfDaysDeducted;
                }

                bool balanceIsUpdated = await _leaveRepository.UpdateLeaveRollingBalanceAsync(currentRollingBalance);
                if (!balanceIsUpdated)
                {
                    await _leaveRepository.DeleteLeaveTransactionAsync(leaveTransaction.LeaveTransactionId);
                    await _leaveRepository.DeleteLeaveAdjustmentAsync(adjustment.LeaveAdjustmentId);
                    return false;
                }

                if (await _leaveRepository.UpdateLeaveRequestStatusAsync(adjustment.LeaveRequestId, (int)LeaveStatusEnum.PendingClosure))
                {
                    //====== Add Activity History =======//
                    LeaveActivityLog log = new LeaveActivityLog();
                    log.ActivityDescription = $"A Leave Adjustment was added by {adjustment.AdjustmentAddedBy} on {DateTime.UtcNow.ToLongDateString()} at {DateTime.UtcNow.ToLongTimeString()}";
                    log.ActivityTime = DateTime.UtcNow;
                    log.LeaveRequestId = adjustment.LeaveRequestId;
                    await _leaveRepository.AddLeaveActivityLogAsync(log);
                }
                return true;
            }
        }
        public async Task<bool> DeleteLeaveAdjustmentAsync(long LeaveAdjustmentId, string DeletedBy)
        {
            LeaveAdjustment leaveAdjustment = new LeaveAdjustment();
            if (LeaveAdjustmentId < 1) { throw new Exception("Leave Adjustment ID has an invalid value."); }
            leaveAdjustment.LeaveAdjustmentId = LeaveAdjustmentId;

            var adjustmentEntity = await _leaveRepository.GetLeaveAdjustmentByIdAsync(leaveAdjustment.LeaveAdjustmentId = LeaveAdjustmentId);
            if (adjustmentEntity != null)
            {
                leaveAdjustment = adjustmentEntity;
                var transactionEntity = await _leaveRepository.GetLeaveTransactionByAdjustmentIdAsync(leaveAdjustment.LeaveAdjustmentId);
                if (transactionEntity != null)
                {
                    var rollingBalanceEntity = await _leaveRepository.GetLeaveRollingBalanceByEmployeeIdAsync(leaveAdjustment.LeaveEmployeeId, leaveAdjustment.LeaveYear, leaveAdjustment.LeaveTypeCode);
                    if (rollingBalanceEntity != null)
                    {
                        if(leaveAdjustment.AdjustmentType == "Addition")
                        {
                            LeaveRollingBalance newRollingBalance = rollingBalanceEntity;
                            newRollingBalance.LeaveDaysAdded = rollingBalanceEntity.LeaveDaysAdded - leaveAdjustment.NumberOfDays;
                            newRollingBalance.LeaveBalanceDate = DateTime.UtcNow;
                            newRollingBalance.LeaveTransactionId = null;

                            if (await _leaveRepository.UpdateLeaveRollingBalanceAsync(newRollingBalance))
                            {
                                if (await _leaveRepository.DeleteLeaveTransactionByLeaveAdjustmentIdAsync(leaveAdjustment.LeaveAdjustmentId))
                                {
                                    if (await _leaveRepository.DeleteLeaveAdjustmentAsync(leaveAdjustment.LeaveAdjustmentId))
                                    {
                                        //====== Add Activity History =======//
                                        LeaveActivityLog log = new LeaveActivityLog();
                                        log.ActivityDescription = $"A previous Leave Adjustment was reversed by {DeletedBy} on {DateTime.UtcNow.ToLongDateString()} at {DateTime.UtcNow.ToLongTimeString()}";
                                        log.ActivityTime = DateTime.UtcNow;
                                        log.LeaveRequestId = leaveAdjustment.LeaveRequestId;
                                        await _leaveRepository.AddLeaveActivityLogAsync(log);
                                    }
                                    return true;
                                }
                            }
                        }
                        else if(leaveAdjustment.AdjustmentType == "Deduction")
                        {
                            LeaveRollingBalance newRollingBalance = rollingBalanceEntity;
                            newRollingBalance.LeaveDaysDeducted = rollingBalanceEntity.LeaveDaysDeducted + leaveAdjustment.NumberOfDays;
                            newRollingBalance.LeaveBalanceDate = DateTime.UtcNow;
                            newRollingBalance.LeaveTransactionId = null;

                            if (await _leaveRepository.UpdateLeaveRollingBalanceAsync(newRollingBalance))
                            {
                                if (await _leaveRepository.DeleteLeaveTransactionByLeaveAdjustmentIdAsync(leaveAdjustment.LeaveAdjustmentId))
                                {
                                    if (await _leaveRepository.DeleteLeaveAdjustmentAsync(leaveAdjustment.LeaveAdjustmentId))
                                    {
                                        //====== Add Activity History =======//
                                        LeaveActivityLog log = new LeaveActivityLog();
                                        log.ActivityDescription = $"A previous Leave Adjustment was reversed by {DeletedBy} on {DateTime.UtcNow.ToLongDateString()} at {DateTime.UtcNow.ToLongTimeString()}";
                                        log.ActivityTime = DateTime.UtcNow;
                                        log.LeaveRequestId = leaveAdjustment.LeaveRequestId;
                                        await _leaveRepository.AddLeaveActivityLogAsync(log);
                                    }
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
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

        public int GetLeaveDuration(DateTime StartDate, DateTime ResumptionDate)
        {
            int _leaveDuration = 0;
            DateTime _leaveDay = StartDate.Date;
            while (_leaveDay < ResumptionDate.Date)
            {
                if ((_leaveDay.DayOfWeek != DayOfWeek.Saturday) && (_leaveDay.DayOfWeek != DayOfWeek.Sunday))
                {
                    _leaveDuration++;
                }
                _leaveDay = _leaveDay.AddDays(1);
            }
            return _leaveDuration;
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

        #region Leave Reports Service Methods
        public async Task<List<LeavePlanCompliance>> GetLeavePlanComplianceAsync(int LeaveYear, ReportParameter parameter)
        {
            List<LeavePlanCompliance> leavePlanComplianceList = new List<LeavePlanCompliance>();
            switch (parameter)
            {
                case ReportParameter.Unit:
                    leavePlanComplianceList = await _leaveRepository.GetLeavePlanComplianceByUnitsAsync(LeaveYear);
                    break;
                case ReportParameter.Department:
                    leavePlanComplianceList = await _leaveRepository.GetLeavePlanComplianceByDepartmentsAsync(LeaveYear);
                    break;
                case ReportParameter.Location:
                    leavePlanComplianceList = await _leaveRepository.GetLeavePlanComplianceByLocationsAsync(LeaveYear);
                    break;
            }

            return leavePlanComplianceList;
        }
        public async Task<List<LeaveRequestCompliance>> GetLeaveRequestComplianceAsync(int LeaveYear, ReportParameter parameter)
        {
            List<LeaveRequestCompliance> leaveRequestComplianceList = new List<LeaveRequestCompliance>();
            switch (parameter)
            {
                case ReportParameter.Unit:
                    leaveRequestComplianceList = await _leaveRepository.GetLeaveRequestComplianceByUnitsAsync(LeaveYear);
                    break;
                case ReportParameter.Department:
                    leaveRequestComplianceList = await _leaveRepository.GetLeaveRequestComplianceByDepartmentsAsync(LeaveYear);
                    break;
                case ReportParameter.Location:
                    leaveRequestComplianceList = await _leaveRepository.GetLeaveRequestComplianceByLocationsAsync(LeaveYear);
                    break;
            }

            return leaveRequestComplianceList;
        }


        public async Task<List<AnnualLeaveSummary>> SearchAnnualLeaveSummaryAsync(int LeaveYear, int UnitId = 0, int DepartmentId = 0, int LocationId = 0, string EmployeeName = null)
        {
            List<AnnualLeaveSummary> annualLeaveSummaryList = new List<AnnualLeaveSummary>();
            if (LeaveYear < 1) { throw new Exception("Error: Required parameter Leave Year has an invalid value."); }
            if (!string.IsNullOrWhiteSpace(EmployeeName))
            {
                annualLeaveSummaryList = await _leaveRepository.GetAnnualLeaveSummaryByEmployeeNameAsync(LeaveYear, EmployeeName);
            }
            else if (UnitId > 0)
            {
                if (LocationId > 0)
                {
                    annualLeaveSummaryList = await _leaveRepository.GetAnnualLeaveSummaryByLocationIdnUnitIdAsync(LeaveYear, LocationId, UnitId);
                }
                else
                {
                    annualLeaveSummaryList = await _leaveRepository.GetAnnualLeaveSummaryByUnitIdAsync(LeaveYear, UnitId);
                }
            }
            else if (DepartmentId > 0)
            {
                annualLeaveSummaryList = await _leaveRepository.GetAnnualLeaveSummaryByDepartmentIdAsync(LeaveYear, DepartmentId);
            }
            else if (LocationId > 0 && UnitId < 1)
            {
                annualLeaveSummaryList = await _leaveRepository.GetAnnualLeaveSummaryByLocationIdAsync(LeaveYear, LocationId);
            }

            return annualLeaveSummaryList;
        }

        #endregion

    }
}
