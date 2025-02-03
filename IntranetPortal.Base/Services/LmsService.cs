using IntranetPortal.Base.Models.BaseModels;
using IntranetPortal.Base.Models.EmployeeRecordModels;
using IntranetPortal.Base.Models.GlobalSettingsModels;
using IntranetPortal.Base.Models.LmsModels;
using IntranetPortal.Base.Models.PmsModels;
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
        public async Task<List<LeaveType>> GetLeaveTypes(bool ExcludeSystem = true)
        {
            if (ExcludeSystem)
            {
                return await _leaveTypesRepository.GetAllExcludingSystemAsync();
            }
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
                if (entities == null || entities.Count < 1)
                {
                    var employees = await _employeesRepository.GetEmployeesByLeaveProfileIdAsync(Id);
                    if (employees == null || employees.Count < 1)
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

        #region Employee Leave Write  Service Methods
        public async Task<long> CreateLeaveAsync(EmployeeLeave e)
        {
            long LeaveId;
            string DocumentType;
            if (e.IsPlan) { DocumentType = "Leave Plan"; } else { DocumentType = "Leave Request"; }

            if (e != null)
            {
                LeaveId = await _employeeLeaveRepository.AddAsync(e);
                if (LeaveId > 0)
                {
                    //====== Add Activity History =======//
                    LeaveActivityLog history = new LeaveActivityLog();
                    history.ActivityDescription = $"{DocumentType} was created by {e.EmployeeFullName} on {DateTime.UtcNow.ToLongDateString()} at {DateTime.UtcNow.ToLongTimeString()}";
                    history.ActivityTime = DateTime.UtcNow;
                    history.LeaveId = LeaveId;
                    await _employeeLeaveRepository.AddLogAsync(history);
                }
            }
            else { throw new Exception($"Required parameter [{DocumentType}] cannot be null."); }

            return LeaveId;
        }
        public async Task<bool> UpdateLeaveAsync(EmployeeLeave e)
        {
            bool IsSuccessful;
            string DocumentType;
            if (e.IsPlan) { DocumentType = "Leave Plan"; } else { DocumentType = "Leave Request"; }
            if (e != null)
            {
                IsSuccessful = await _employeeLeaveRepository.EditAsync(e);
                if (IsSuccessful)
                {
                    //====== Add Activity History =======//
                    LeaveActivityLog history = new LeaveActivityLog();
                    history.ActivityDescription = $"{DocumentType} was editted by {e.EmployeeFullName} on {DateTime.UtcNow.ToLongDateString()} at {DateTime.UtcNow.ToLongTimeString()}";
                    history.ActivityTime = DateTime.UtcNow;
                    history.LeaveId = e.Id;
                    await _employeeLeaveRepository.AddLogAsync(history);
                }
            }
            else { throw new Exception($"Required parameter [{DocumentType}] cannot be null."); }
            return IsSuccessful;
        }
        public async Task<bool> DeleteLeaveAsync(long id)
        {
            bool IsSuccessful;
            if (id > 0)
            {
                IsSuccessful = await _employeeLeaveRepository.DeleteAsync(id);
            }
            else { throw new Exception("Required parameter [Leave ID] is missing."); }
            return IsSuccessful;
        }
        public async Task<bool> UpdateLeaveStatusAsync(long LeaveId, string NewStatus)
        {
            bool IsSuccessful;
            if (LeaveId > 0 && !string.IsNullOrWhiteSpace(NewStatus))
            {
                IsSuccessful = await _employeeLeaveRepository.UpdateStatusAsync(LeaveId, NewStatus);
            }
            else { throw new Exception("Some required parameters has invalid values."); }
            return IsSuccessful;
        }

        #endregion

        #region Employee Leave Read Service Methods
        public async Task<List<EmployeeLeave>> GetEmployeeLeavesAsync(string EmployeeId, int LeaveYear, bool IsPlan)
        {
            List<EmployeeLeave> leaveList = new List<EmployeeLeave>();
            if (!string.IsNullOrWhiteSpace(EmployeeId) && LeaveYear > 0)
            {
                leaveList = await _employeeLeaveRepository.GetByEmployeeIdnYearAsync(EmployeeId, LeaveYear, IsPlan);
            }
            return leaveList;
        }
        public async Task<EmployeeLeave> GetEmployeeLeaveAsync(long Id)
        {
            EmployeeLeave p = new EmployeeLeave();
            if (Id > 0)
            {
                p = await _employeeLeaveRepository.GetByIdAsync(Id);
            }
            return p;
        }
        public async Task<List<EmployeeLeave>> SearchMyTeamsEmployeeLeavesAsync(string TeamLeadId, int LeaveYear, int LeaveMonth, string EmployeeId = null, string LeaveStatus = null, bool IsPlan = true)
        {
            List<EmployeeLeave> LeaveList = new List<EmployeeLeave>();
            if (string.IsNullOrWhiteSpace(EmployeeId))
            {
                if (LeaveYear > 0)
                {
                    if (LeaveMonth > 0)
                    {
                        if (!string.IsNullOrWhiteSpace(LeaveStatus))
                        {
                            var tyms_entities = await _employeeLeaveRepository.GetByReportingLineIdnYearnMonthnStatusAsync(TeamLeadId, LeaveYear, LeaveMonth, LeaveStatus, IsPlan);
                            if (tyms_entities != null && tyms_entities.Count > 0)
                            {
                                LeaveList = tyms_entities;
                            }
                        }
                        else
                        {
                            var tym_entities = await _employeeLeaveRepository.GetByReportingLineIdnYearnMonthAsync(TeamLeadId, LeaveYear, LeaveMonth, IsPlan);
                            if (tym_entities != null && tym_entities.Count > 0)
                            {
                                LeaveList = tym_entities;
                            }
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(LeaveStatus))
                        {
                            var tys_entities = await _employeeLeaveRepository.GetByReportingLineIdnYearnStatusAsync(TeamLeadId, LeaveYear, LeaveStatus, IsPlan);
                            if (tys_entities != null && tys_entities.Count > 0)
                            {
                                LeaveList = tys_entities;
                            }
                        }
                        else
                        {
                            var ty_entities = await _employeeLeaveRepository.GetByReportingLineIdnYearAsync(TeamLeadId, LeaveYear, IsPlan);
                            if (ty_entities != null && ty_entities.Count > 0)
                            {
                                LeaveList = ty_entities;
                            }
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(LeaveStatus))
                    {
                        var ts_entities = await _employeeLeaveRepository.GetByReportingLineIdnStatusAsync(TeamLeadId, LeaveStatus, IsPlan);
                        if (ts_entities != null && ts_entities.Count > 0)
                        {
                            LeaveList = ts_entities;
                        }
                    }
                    else
                    {
                        var t_entities = await _employeeLeaveRepository.GetByReportingLineIdAsync(TeamLeadId, IsPlan);
                        if (t_entities != null && t_entities.Count > 0)
                        {
                            LeaveList = t_entities;
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
                        if (!string.IsNullOrWhiteSpace(LeaveStatus))
                        {
                            var eyms_entities = await _employeeLeaveRepository.GetByReportingLineIdnYearnMonthnStatusAsync(TeamLeadId, LeaveYear, LeaveMonth, LeaveStatus, IsPlan);
                            if (eyms_entities != null && eyms_entities.Count > 0)
                            {
                                LeaveList = eyms_entities;
                            }
                        }
                        else
                        {
                            var eym_entities = await _employeeLeaveRepository.GetByReportingLineIdnYearnMonthAsync(TeamLeadId, LeaveYear, LeaveMonth, IsPlan);
                            if (eym_entities != null && eym_entities.Count > 0)
                            {
                                LeaveList = eym_entities;
                            }
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(LeaveStatus))
                        {
                            var eyms_entities = await _employeeLeaveRepository.GetByEmployeeIdnYearnMonthnStatusAsync(EmployeeId, LeaveYear, LeaveMonth, LeaveStatus, IsPlan);
                            if (eyms_entities != null && eyms_entities.Count > 0)
                            {
                                LeaveList = eyms_entities;
                            }
                        }
                        else
                        {
                            var eym_entities = await _employeeLeaveRepository.GetByEmployeeIdnYearnMonthAsync(TeamLeadId, LeaveYear, LeaveMonth, IsPlan);
                            if (eym_entities != null && eym_entities.Count > 0)
                            {
                                LeaveList = eym_entities;
                            }
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(LeaveStatus))
                    {
                        var es_entities = await _employeeLeaveRepository.GetByEmployeeIdnStatusAsync(EmployeeId, LeaveStatus, IsPlan);
                        if (es_entities != null && es_entities.Count > 0)
                        {
                            LeaveList = es_entities;
                        }
                    }
                    else
                    {
                        var e_entities = await _employeeLeaveRepository.GetByEmployeeIdAsync(EmployeeId, IsPlan);
                        if (e_entities != null && e_entities.Count > 0)
                        {
                            LeaveList = e_entities;
                        }
                    }
                }
            }
            return LeaveList;
        }
        public async Task<List<EmployeeLeave>> SearchAllEmployeeLeavesAsync(int LeaveYear, int LeaveMonth, string EmployeeName = null, string LeaveStatus = null, bool IsPlan = true)
        {
            List<EmployeeLeave> LeaveList = new List<EmployeeLeave>();
            if (string.IsNullOrWhiteSpace(EmployeeName))
            {
                if (LeaveYear > 0)
                {
                    if (LeaveMonth > 0)
                    {
                        if (!string.IsNullOrWhiteSpace(LeaveStatus))
                        {
                            var tyms_entities = await _employeeLeaveRepository.GetByYearnMonthnStatusAsync(LeaveYear, LeaveMonth, LeaveStatus, IsPlan);
                            if (tyms_entities != null && tyms_entities.Count > 0)
                            {
                                LeaveList = tyms_entities;
                            }
                        }
                        else
                        {
                            var tym_entities = await _employeeLeaveRepository.GetByYearnMonthAsync(LeaveYear, LeaveMonth, IsPlan);
                            if (tym_entities != null && tym_entities.Count > 0)
                            {
                                LeaveList = tym_entities;
                            }
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(LeaveStatus))
                        {
                            var tys_entities = await _employeeLeaveRepository.GetByYearnStatusAsync(LeaveYear, LeaveStatus, IsPlan);
                            if (tys_entities != null && tys_entities.Count > 0)
                            {
                                LeaveList = tys_entities;
                            }
                        }
                        else
                        {
                            var ty_entities = await _employeeLeaveRepository.GetByYearAsync(LeaveYear, IsPlan);
                            if (ty_entities != null && ty_entities.Count > 0)
                            {
                                LeaveList = ty_entities;
                            }
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(LeaveStatus))
                    {
                        var ts_entities = await _employeeLeaveRepository.GetByStatusAsync(LeaveStatus, IsPlan);
                        if (ts_entities != null && ts_entities.Count > 0)
                        {
                            LeaveList = ts_entities;
                        }
                    }
                    else
                    {
                        var t_entities = await _employeeLeaveRepository.GetAllAsync(IsPlan);
                        if (t_entities != null && t_entities.Count > 0)
                        {
                            LeaveList = t_entities;
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
                        if (!string.IsNullOrWhiteSpace(LeaveStatus))
                        {
                            var eyms_entities = await _employeeLeaveRepository.GetByEmployeeNamenYearnMonthnStatusAsync(EmployeeName, LeaveYear, LeaveMonth, LeaveStatus, IsPlan);
                            if (eyms_entities != null && eyms_entities.Count > 0)
                            {
                                LeaveList = eyms_entities;
                            }
                        }
                        else
                        {
                            var eym_entities = await _employeeLeaveRepository.GetByEmployeeNamenYearnMonthAsync(EmployeeName, LeaveYear, LeaveMonth, IsPlan);
                            if (eym_entities != null && eym_entities.Count > 0)
                            {
                                LeaveList = eym_entities;
                            }
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(LeaveStatus))
                        {
                            var eyms_entities = await _employeeLeaveRepository.GetByEmployeeNamenYearnStatusAsync(EmployeeName, LeaveYear, LeaveStatus, IsPlan);
                            if (eyms_entities != null && eyms_entities.Count > 0)
                            {
                                LeaveList = eyms_entities;
                            }
                        }
                        else
                        {
                            var eym_entities = await _employeeLeaveRepository.GetByEmployeeNamenYearAsync(EmployeeName, LeaveYear, IsPlan);
                            if (eym_entities != null && eym_entities.Count > 0)
                            {
                                LeaveList = eym_entities;
                            }
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(LeaveStatus))
                    {
                        var es_entities = await _employeeLeaveRepository.GetByEmployeeNamenStatusAsync(EmployeeName, LeaveStatus, IsPlan);
                        if (es_entities != null && es_entities.Count > 0)
                        {
                            LeaveList = es_entities;
                        }
                    }
                    else
                    {
                        var e_entities = await _employeeLeaveRepository.GetByEmployeeNameAsync(EmployeeName, IsPlan);
                        if (e_entities != null && e_entities.Count > 0)
                        {
                            LeaveList = e_entities;
                        }
                    }
                }
            }
            return LeaveList;
        }

        #endregion

        #region Leave Submission Service Methods
        public async Task<bool> SubmitLeaveAsync(LeaveSubmission e, string DocumentType)
        {
            bool IsUpdated = false;
            if (e != null)
            {
                //IsSubmitted = await _employeeLeaveRepository.AddSubmissionAsync(e);
                //if (IsSubmitted)
                //{
                //Update Leave Plan Status to Pending
                IsUpdated = await _employeeLeaveRepository.UpdateStatusAsync(e.LeaveId, LeaveStatus.Pending.ToString());
                //====== Add Appraisal Activity History =======//
                if (IsUpdated)
                {
                    Employee sender = await _employeesRepository.GetEmployeeByIdAsync(e.FromEmployeeId);
                    Employee approver = await _employeesRepository.GetEmployeeByIdAsync(e.ToEmployeeId);
                    LeaveActivityLog history = new LeaveActivityLog();
                    history.ActivityDescription = $"{DocumentType} was submitted to {approver.FullName} by {sender.FullName} on {DateTime.UtcNow.ToLongDateString()} at {DateTime.UtcNow.ToLongTimeString()}";
                    history.ActivityTime = DateTime.UtcNow;
                    history.LeaveId = e.LeaveId;
                    await _employeeLeaveRepository.AddLogAsync(history);
                }
                //}
            }
            else { throw new Exception("Required parameter [Leave Submission] has invalid value."); }
            return IsUpdated;
        }
        #endregion

        #region Leave Approval Service Methods
        public async Task<bool> ApproveLeaveAsync(LeaveApproval e, string DocumentType)
        {
            bool IsUpdated = false;
            if (e != null)
            {
                string ApprovalType = "";
                switch (e.ApproverRole)
                {
                    case "Line Manager":
                        ApprovalType = "LM";
                        break;
                    case "Head of Department":
                        ApprovalType = "HD";
                        break;
                    case "HR Representative":
                        ApprovalType = "HR";
                        break;
                    case "Station Manager":
                        ApprovalType = "SM";
                        break;
                    case "Executive Management":
                        ApprovalType = "XM";
                        break;
                    default:
                        break;
                }
                long approvalId = await _employeeLeaveRepository.AddApprovalAsync(e);
                if (approvalId > 0)
                {
                    //Update Leave Plan Status to Pending
                    IsUpdated = await _employeeLeaveRepository.UpdateApprovalStatusAsync(e.LeaveId, LeaveStatus.Approved.ToString(), ApprovalType);
                    if (IsUpdated)
                    {
                        //====== Add Leave Note =======//
                        LeaveNote note = new LeaveNote();
                        note.NoteContent = e.ApproverComments;
                        note.LeaveId = e.LeaveId;
                        note.TimeAdded = DateTime.Now;
                        note.FromEmployeeName = e.ApproverName;
                        if (await _employeeLeaveRepository.AddNoteAsync(note))
                        {
                            //====== Add Leave Activity Log =======//
                            LeaveActivityLog history = new LeaveActivityLog();
                            history.ActivityDescription = $"{DocumentType} was approved by {e.ApproverName} on {DateTime.UtcNow.ToLongDateString()} at {DateTime.UtcNow.ToLongTimeString()} WAT";
                            history.ActivityTime = DateTime.UtcNow;
                            history.LeaveId = e.LeaveId;
                            await _employeeLeaveRepository.AddLogAsync(history);
                        }
                        return true;
                    }
                    else
                    {
                        await _employeeLeaveRepository.DeleteApprovalAsync(approvalId);
                        throw new Exception($"An error was encountered while attempting to update {DocumentType} status.");
                    }
                }
                else { throw new Exception("Sorry an error was encountered while attempting to add the approval record."); }
            }
            else { throw new Exception("Required parameter [Leave Approval] has invalid value."); }

        }
        public async Task<List<LeaveApproval>> GetLeaveApprovalsAsync(long LeaveId)
        {
            List<LeaveApproval> approvalsList = new List<LeaveApproval>();
            if (LeaveId > 0)
            {
                var entities = await _employeeLeaveRepository.GetApprovalsByLeaveIdAsync(LeaveId);
                if (entities != null && entities.Count > 0)
                {
                    approvalsList = entities;
                }
            }
            return approvalsList;
        }

        #endregion

        #region Leave Notes Service Methods
        public async Task<List<LeaveNote>> GetLeaveNotesAsync(long LeaveId)
        {
            List<LeaveNote> notesList = new List<LeaveNote>();
            if (LeaveId > 0)
            {
                var entities = await _employeeLeaveRepository.GetNotesByLeaveIdAsync(LeaveId);
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
                IsAdded = await _employeeLeaveRepository.AddNoteAsync(e);
            }
            else { throw new Exception("Required parameter [Leave Note] has invalid value."); }
            return IsAdded;
        }
        #endregion

        #region Leave Activities Service Methods
        public async Task<List<LeaveActivityLog>> GetLeaveActivitiesAsync(long LeaveId)
        {
            List<LeaveActivityLog> activitiesList = new List<LeaveActivityLog>();
            if (LeaveId > 0)
            {
                var entities = await _employeeLeaveRepository.GetLogByLeaveIdAsync(LeaveId);
                if (entities != null && entities.Count > 0)
                {
                    activitiesList = entities;
                }
            }
            return activitiesList;
        }
        public async Task<bool> AddActivityLogAsync(LeaveActivityLog e)
        {
            bool IsAdded = false;
            if (e != null)
            {
                IsAdded = await _employeeLeaveRepository.AddLogAsync(e);
            }
            return IsAdded;
        }
        #endregion

        #region Leave Documents Service Methods
        public async Task<List<LeaveDocument>> GetLeaveDocumentsAsync(long LeaveId)
        {
            List<LeaveDocument> documentsList = new List<LeaveDocument>();
            if (LeaveId > 0)
            {
                var entities = await _employeeLeaveRepository.GetDocumentsByLeaveIdAsync(LeaveId);
                if (entities != null && entities.Count > 0)
                {
                    documentsList = entities;
                }
            }
            return documentsList;
        }
        public async Task<LeaveDocument> GetLeaveDocumentAsync(long DocumentId)
        {
           LeaveDocument document = new LeaveDocument();
            if (DocumentId > 0)
            {
                document = await _employeeLeaveRepository.GetDocumentByIdAsync(DocumentId);
            }
            return document;
        }
        public async Task<bool> AddLeaveDocumentAsync(LeaveDocument e)
        {
            bool IsAdded;
            if (e != null)
            {
                IsAdded = await _employeeLeaveRepository.AddDocumentAsync(e);
            }
            else { throw new Exception("Required parameter [Leave Document] has invalid value."); }
            return IsAdded;
        }
        public async Task<bool> DeleteLeaveDocumentAsync(long LeaveDocumentId)
        {
            bool IsAdded;
            if (LeaveDocumentId > 0)
            {
                IsAdded = await _employeeLeaveRepository.DeleteDocumentAsync(LeaveDocumentId);
            }
            else { throw new Exception("Required parameter [Document ID] has invalid value."); }
            return IsAdded;
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
                        for (int i = 0; i <= Duration; i++)
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
                        if (noOfPublicHolidays > 0)
                        {
                            DateTime finalEndDate = endDate;
                            for (int i = 0; i < noOfPublicHolidays; i++)
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
