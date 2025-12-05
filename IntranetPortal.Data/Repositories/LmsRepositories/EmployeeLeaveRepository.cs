using IntranetPortal.Base.Models.LmsModels;
using IntranetPortal.Base.Repositories.LmsRepositories;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Data.Repositories.LmsRepositories
{
    public class EmployeeLeaveRepository : IEmployeeLeaveRepository
    {
        public IConfiguration _config { get; }
        public EmployeeLeaveRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        #region Employee Leave Write Action Methods
        //===  Leave Write Action Methods =======//
        public async Task<long> AddAsync(EmployeeLeave e)
        {
            long newLeaveId = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.lms_lvs_infs(emp_id, unit_id, dept_id, loc_id, ");
            sb.Append("lvs_yr, lvs_typ_cd, lvs_rsn, lvs_sts, is_pln, prp_lvs_sdt, ");
            sb.Append("prp_lvs_edt, prp_lvs_dur, prp_lvs_dur_typ, prp_dur_ds) ");
            sb.Append("VALUES(@emp_id, @unit_id, @dept_id, @loc_id, @lvs_yr, ");
            sb.Append("@lvs_typ_cd, @lvs_rsn, @lvs_sts, @is_pln, @prp_lvs_sdt, ");
            sb.Append("@prp_lvs_edt, @prp_lvs_dur, @prp_lvs_dur_typ, @prp_dur_ds) ");
            sb.Append("RETURNING lvs_inf_id;");
            string query = sb.ToString();

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var emp_id = cmd.Parameters.Add("emp_id", NpgsqlDbType.Text);
                    var unit_id = cmd.Parameters.Add("@unit_id", NpgsqlDbType.Integer);
                    var dept_id = cmd.Parameters.Add("@dept_id", NpgsqlDbType.Integer);
                    var loc_id = cmd.Parameters.Add("@loc_id", NpgsqlDbType.Integer);

                    var lvs_yr = cmd.Parameters.Add("@lvs_yr", NpgsqlDbType.Integer);
                    var lvs_typ_cd = cmd.Parameters.Add("@lvs_typ_cd", NpgsqlDbType.Text);
                    var lvs_rsn = cmd.Parameters.Add("@lvs_rsn", NpgsqlDbType.Text);
                    var lvs_sts = cmd.Parameters.Add("@lvs_sts", NpgsqlDbType.Text);
                    var is_pln = cmd.Parameters.Add("@is_pln", NpgsqlDbType.Boolean);

                    var prp_lvs_sdt = cmd.Parameters.Add("@prp_lvs_sdt", NpgsqlDbType.TimestampTz);
                    var prp_lvs_edt = cmd.Parameters.Add("@prp_lvs_edt", NpgsqlDbType.TimestampTz);
                    var prp_lvs_dur = cmd.Parameters.Add("@prp_lvs_dur", NpgsqlDbType.Integer);
                    var prp_lvs_dur_typ = cmd.Parameters.Add("@prp_lvs_dur_typ", NpgsqlDbType.Integer);
                    var prp_dur_ds = cmd.Parameters.Add("@prp_dur_ds", NpgsqlDbType.Integer);

                    cmd.Prepare();

                    emp_id.Value = e.EmployeeId;
                    unit_id.Value = e.UnitId;
                    dept_id.Value = e.DepartmentId;
                    loc_id.Value = e.LocationId;

                    lvs_yr.Value = e.LeaveYear;
                    lvs_typ_cd.Value = e.LeaveTypeCode;
                    lvs_rsn.Value = e.LeaveReason ?? (object)DBNull.Value;
                    lvs_sts.Value = e.LeaveStatus;
                    is_pln.Value = e.IsPlan;

                    prp_lvs_sdt.Value = e.ProposedLeaveStartDate;
                    prp_lvs_edt.Value = e.ProposedLeaveEndDate;
                    prp_lvs_dur.Value = e.ProposedLeaveDuration;
                    prp_lvs_dur_typ.Value = e.ProposedDurationTypeId;
                    prp_dur_ds.Value = e.ProposedDurationDescription;


                    var obj = await cmd.ExecuteScalarAsync();
                    newLeaveId = (long)obj;
                    await conn.CloseAsync();
                }
            }
            return newLeaveId;
        }
        public async Task<bool> DeleteAsync(long id)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("DELETE FROM public.lms_lvs_aprvs WHERE (lvs_inf_id = @lvs_inf_id); ");
            sb.Append("DELETE FROM public.lms_lvs_docs WHERE (lvs_inf_id = @lvs_inf_id); ");
            sb.Append("DELETE FROM public.lms_lvs_logs WHERE (lvs_inf_id = @lvs_inf_id); ");
            sb.Append("DELETE FROM public.lms_lvs_msgs WHERE (lvs_inf_id = @lvs_inf_id); ");
            sb.Append("DELETE FROM public.lms_lvs_sbms WHERE (lvs_inf_id = @lvs_inf_id); ");
            sb.Append("DELETE FROM public.lms_lvs_trnx WHERE (lvs_inf_id = @lvs_inf_id); ");
            sb.Append("DELETE FROM public.lms_lvs_infs WHERE (lvs_inf_id = @lvs_inf_id);");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Delete data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_inf_id = cmd.Parameters.Add("@lvs_inf_id", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    lvs_inf_id.Value = id;
                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            return rows > 0;
        }
        public async Task<bool> EditAsync(EmployeeLeave e)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.lms_lvs_infs SET emp_id=@emp_id, unit_id=@unit_id, ");
            sb.Append("dept_id=@dept_id, loc_id=loc_id, lvs_yr=@lvs_yr, lvs_typ_cd=@lvs_typ_cd, ");
            sb.Append("lvs_rsn=@lvs_rsn, lvs_sts=@lvs_sts, is_pln=@is_pln, prp_lvs_sdt=@prp_lvs_sdt, ");
            sb.Append("prp_lvs_edt=@prp_lvs_edt, prp_lvs_dur=@prp_lvs_dur, prp_dur_ds=@prp_dur_ds ");
            sb.Append("WHERE (lvs_inf_id=@lvs_inf_id); ");

            string query = sb.ToString();

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_inf_id = cmd.Parameters.Add("@lvs_inf_id", NpgsqlDbType.Bigint);
                    var emp_id = cmd.Parameters.Add("emp_id", NpgsqlDbType.Text);
                    var unit_id = cmd.Parameters.Add("@unit_id", NpgsqlDbType.Integer);
                    var dept_id = cmd.Parameters.Add("@dept_id", NpgsqlDbType.Integer);
                    var loc_id = cmd.Parameters.Add("@loc_id", NpgsqlDbType.Integer);

                    var lvs_yr = cmd.Parameters.Add("@lvs_yr", NpgsqlDbType.Integer);
                    var lvs_typ_cd = cmd.Parameters.Add("@lvs_typ_cd", NpgsqlDbType.Text);
                    var lvs_rsn = cmd.Parameters.Add("@lvs_rsn", NpgsqlDbType.Text);
                    var lvs_sts = cmd.Parameters.Add("@lvs_sts", NpgsqlDbType.Text);
                    var is_pln = cmd.Parameters.Add("@is_pln", NpgsqlDbType.Boolean);

                    var prp_lvs_sdt = cmd.Parameters.Add("@prp_lvs_sdt", NpgsqlDbType.TimestampTz);
                    var prp_lvs_edt = cmd.Parameters.Add("@prp_lvs_edt", NpgsqlDbType.TimestampTz);
                    var prp_lvs_dur = cmd.Parameters.Add("@prp_lvs_dur", NpgsqlDbType.Integer);
                    var prp_lvs_dur_typ = cmd.Parameters.Add("@prp_lvs_dur_typ", NpgsqlDbType.Integer);
                    var prp_dur_ds = cmd.Parameters.Add("@prp_dur_ds", NpgsqlDbType.Integer);

                    cmd.Prepare();

                    lvs_inf_id.Value = e.Id;
                    emp_id.Value = e.EmployeeId;
                    unit_id.Value = e.UnitId;
                    dept_id.Value = e.DepartmentId;
                    loc_id.Value = e.LocationId;

                    lvs_yr.Value = e.LeaveYear;
                    lvs_typ_cd.Value = e.LeaveTypeCode;
                    lvs_rsn.Value = e.LeaveReason ?? (object)DBNull.Value;
                    lvs_sts.Value = e.LeaveStatus;
                    is_pln.Value = e.IsPlan;

                    prp_lvs_sdt.Value = e.ProposedLeaveStartDate;
                    prp_lvs_edt.Value = e.ProposedLeaveEndDate;
                    prp_lvs_dur.Value = e.ProposedLeaveDuration;
                    prp_lvs_dur_typ.Value = e.ProposedDurationTypeId;
                    prp_dur_ds.Value = e.ProposedDurationDescription;

                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            return rows > 0;
        }
        public async Task<bool> UpdateStatusAsync(long leaveId, string newStatus)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.lms_lvs_infs SET lvs_sts=@lvs_sts ");
            sb.Append("WHERE (lvs_inf_id=@lvs_inf_id); ");
            string query = sb.ToString();

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_inf_id = cmd.Parameters.Add("@lvs_inf_id", NpgsqlDbType.Bigint);
                    var lvs_sts = cmd.Parameters.Add("@lvs_sts", NpgsqlDbType.Text);
                    cmd.Prepare();
                    lvs_inf_id.Value = leaveId;
                    lvs_sts.Value = newStatus;
                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            return rows > 0;
        }
        public async Task<bool> UpdateApprovalStatusAsync(long leaveId, string newStatus, string approvalType)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.lms_lvs_infs SET lvs_sts=@lvs_sts, ");
            switch (approvalType)
            {
                case "LM":
                    sb.Append("is_lm_aprv=true ");
                    break;
                case "HD":
                    sb.Append("is_hd_aprv=true ");
                    break;
                case "HR":
                    sb.Append("is_hr_aprv=true ");
                    break;
                case "SM":
                    sb.Append("is_sm_aprv=true ");
                    break;
                case "XM":
                    sb.Append("is_xm_aprv=true ");
                    break;
                default:
                    break;
            }
            sb.Append("WHERE (lvs_inf_id=@lvs_inf_id); ");
            string query = sb.ToString();

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_inf_id = cmd.Parameters.Add("@lvs_inf_id", NpgsqlDbType.Bigint);
                    var lvs_sts = cmd.Parameters.Add("@lvs_sts", NpgsqlDbType.Text);
                    cmd.Prepare();
                    lvs_inf_id.Value = leaveId;
                    lvs_sts.Value = newStatus;
                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            return rows > 0;
        }
        #endregion

        #region Employee Leave Read Action Methods

        #region Employee Leaves By Employee ID
        //=== Employee Leaves By Employee ID Read Action Methods ========//
        public async Task<List<EmployeeLeave>> GetByEmployeeIdAsync(string employeeId, bool isPlan)
        {
            List<EmployeeLeave> leaveList = new List<EmployeeLeave>();
            string query = string.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT v.lvs_inf_id, v.emp_id, v.unit_id, v.dept_id, ");
            sb.Append("v.loc_id, v.lvs_yr, v.lvs_typ_cd, v.lvs_rsn, v.lvs_sts, ");
            sb.Append("v.is_pln, v.prp_lvs_sdt, v.prp_lvs_edt, v.prp_lvs_dur, ");
            sb.Append("v.act_lvs_dur, v.prp_dur_ds, v.apv_lvs_sdt, v.apv_lvs_edt,  ");
            sb.Append("v.apv_lvs_dur, v.apv_dur_ds, v.act_lvs_sdt, v.act_lvs_edt, "); 
            sb.Append("v.act_dur_ds, v.lm_rsmptn_dt, v.lm_confm_dt, v.lm_confm_by, ");
            sb.Append("v.hr_rsmptn_dt, v.hr_confm_dt, v.hr_confm_by, v.rqs_cls_dt, ");
            sb.Append("v.is_lm_aprv, v.is_hd_aprv, v.is_hr_aprv, v.is_xm_aprv, ");
            sb.Append("v.is_sm_aprv, v.prp_lvs_dur_typ, v.apv_lvs_dur_typ, v.act_lvs_dur_typ,  ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = v.emp_id) ");
            sb.Append("as emp_nm, (SELECT lvs_typ_nm FROM public.lms_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = v.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = v.unit_id) ");
            sb.Append("as unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= v.dept_id) as dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = v.loc_id) as loc_nm ");
            sb.Append("FROM public.lms_lvs_infs v ");
            sb.Append("WHERE (v.emp_id=@emp_id) AND v.is_pln = @is_pln) ");
            sb.Append("ORDER BY v.prp_lvs_sdt; ");

            query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                    var is_pln = cmd.Parameters.Add("@is_pln", NpgsqlDbType.Boolean);
                    await cmd.PrepareAsync();
                    emp_id.Value = employeeId;
                    is_pln.Value = isPlan;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        leaveList.Add(new EmployeeLeave()
                        {
                            Id = reader["lvs_inf_id"] == DBNull.Value ? 0L : (long)reader["lvs_inf_id"],
                            EmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
                            EmployeeFullName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),

                            UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                            UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
                            DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                            DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
                            LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                            LocationName = reader["loc_nm"] == DBNull.Value ? string.Empty : reader["loc_nm"].ToString(),

                            LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"],
                            LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString(),
                            LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
                            LeaveReason = reader["lvs_rsn"] == DBNull.Value ? string.Empty : reader["lvs_rsn"].ToString(),
                            LeaveStatus = reader["lvs_sts"] == DBNull.Value ? string.Empty : reader["lvs_sts"].ToString(),
                            IsPlan = reader["is_pln"] == DBNull.Value ? true : (bool)reader["is_pln"],

                            ProposedLeaveStartDate = reader["prp_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_sdt"],
                            ProposedLeaveEndDate = reader["prp_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_edt"],
                            ProposedLeaveDuration = reader["prp_lvs_dur"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur"],
                            ProposedDurationTypeId = reader["prp_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur_typ"],
                            ProposedDurationDescription = reader["prp_dur_ds"] == DBNull.Value ? string.Empty : reader["prp_dur_ds"].ToString(),

                            ApprovedLeaveStartDate = reader["apv_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_sdt"],
                            ApprovedLeaveEndDate = reader["apv_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_edt"],
                            ApprovedLeaveDuration = reader["apv_lvs_dur"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur"],
                            ApprovedDurationTypeId = reader["apv_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur_typ"],
                            ApprovedDurationDescription = reader["apv_dur_ds"] == DBNull.Value ? string.Empty : reader["apv_dur_ds"].ToString(),

                            ActualLeaveStartDate = reader["act_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_sdt"],
                            ActualLeaveEndDate = reader["act_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_edt"],
                            ActualLeaveDuration = reader["act_lvs_dur"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur"],
                            ActualDurationTypeId = reader["act_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur_typ"],
                            ActualDurationDescription = reader["act_dur_ds"] == DBNull.Value ? string.Empty : reader["act_dur_ds"].ToString(),

                            RequestCloseDate = reader["rqs_cls_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rqs_cls_dt"],
                           
                            LineManagersResumptionDate = reader["lm_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_rsmptn_dt"],
                            LineManagerConfirmResumptionDate = reader["lm_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_confm_dt"],
                            LineManagerConfirmResumptionBy = reader["lm_confm_by"] == DBNull.Value ? string.Empty : reader["lm_confm_by"].ToString(),
                            
                            HrResumptionDate = reader["hr_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_rsmptn_dt"],
                            HrConfirmResumptionDate = reader["hr_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_confm_dt"],
                            HrConfirmResumptionBy = reader["hr_confm_by"] == DBNull.Value ? string.Empty : reader["hr_confm_by"].ToString(),

                            ApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
                            ApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],
                            ApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
                            ApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
                            ApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveList;
        }
        public async Task<List<EmployeeLeave>> GetByEmployeeIdnYearAsync(string employeeId, int year, bool isPlan)
        {
            List<EmployeeLeave> leaveList = new List<EmployeeLeave>();
            string query = string.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT v.lvs_inf_id, v.emp_id, v.unit_id, v.dept_id, ");
            sb.Append("v.loc_id, v.lvs_yr, v.lvs_typ_cd, v.lvs_rsn, v.lvs_sts, ");
            sb.Append("v.is_pln, v.prp_lvs_sdt, v.prp_lvs_edt, v.prp_lvs_dur, ");
            sb.Append("v.act_lvs_dur, v.prp_dur_ds, v.apv_lvs_sdt, v.apv_lvs_edt,  ");
            sb.Append("v.apv_lvs_dur, v.apv_dur_ds, v.act_lvs_sdt, v.act_lvs_edt, ");
            sb.Append("v.act_dur_ds, v.lm_rsmptn_dt, v.lm_confm_dt, v.lm_confm_by, ");
            sb.Append("v.hr_rsmptn_dt, v.hr_confm_dt, v.hr_confm_by, v.rqs_cls_dt, ");
            sb.Append("v.is_lm_aprv, v.is_hd_aprv, v.is_hr_aprv, v.is_xm_aprv, ");
            sb.Append("v.is_sm_aprv, v.prp_lvs_dur_typ, v.apv_lvs_dur_typ, v.act_lvs_dur_typ,  ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = v.emp_id) ");
            sb.Append("as emp_nm, (SELECT lvs_typ_nm FROM public.lms_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = v.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = v.unit_id) ");
            sb.Append("as unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= v.dept_id) as dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = v.loc_id) as loc_nm ");
            sb.Append("FROM public.lms_lvs_infs v ");
            sb.Append("WHERE (v.emp_id = @emp_id) AND (v.is_pln = @is_pln) ");
            sb.Append("AND (v.lvs_yr = @lvs_yr) ORDER BY v.prp_lvs_sdt; ");

            query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                    var lvs_yr = cmd.Parameters.Add("@lvs_yr", NpgsqlDbType.Integer);
                    var is_pln = cmd.Parameters.Add("@is_pln", NpgsqlDbType.Boolean);
                    await cmd.PrepareAsync();
                    emp_id.Value = employeeId;
                    lvs_yr.Value = year;
                    is_pln.Value = isPlan;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        leaveList.Add(new EmployeeLeave()
                        {
                            Id = reader["lvs_inf_id"] == DBNull.Value ? 0L : (long)reader["lvs_inf_id"],
                            EmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
                            EmployeeFullName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),

                            UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                            UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
                            DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                            DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
                            LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                            LocationName = reader["loc_nm"] == DBNull.Value ? string.Empty : reader["loc_nm"].ToString(),

                            LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"],
                            LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString(),
                            LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
                            LeaveReason = reader["lvs_rsn"] == DBNull.Value ? string.Empty : reader["lvs_rsn"].ToString(),
                            LeaveStatus = reader["lvs_sts"] == DBNull.Value ? string.Empty : reader["lvs_sts"].ToString(),
                            IsPlan = reader["is_pln"] == DBNull.Value ? true : (bool)reader["is_pln"],

                            ProposedLeaveStartDate = reader["prp_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_sdt"],
                            ProposedLeaveEndDate = reader["prp_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_edt"],
                            ProposedLeaveDuration = reader["prp_lvs_dur"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur"],
                            ProposedDurationTypeId = reader["prp_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur_typ"],
                            ProposedDurationDescription = reader["prp_dur_ds"] == DBNull.Value ? string.Empty : reader["prp_dur_ds"].ToString(),

                            ApprovedLeaveStartDate = reader["apv_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_sdt"],
                            ApprovedLeaveEndDate = reader["apv_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_edt"],
                            ApprovedLeaveDuration = reader["apv_lvs_dur"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur"],
                            ApprovedDurationTypeId = reader["apv_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur_typ"],
                            ApprovedDurationDescription = reader["apv_dur_ds"] == DBNull.Value ? string.Empty : reader["apv_dur_ds"].ToString(),

                            ActualLeaveStartDate = reader["act_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_sdt"],
                            ActualLeaveEndDate = reader["act_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_edt"],
                            ActualLeaveDuration = reader["act_lvs_dur"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur"],
                            ActualDurationTypeId = reader["act_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur_typ"],
                            ActualDurationDescription = reader["act_dur_ds"] == DBNull.Value ? string.Empty : reader["act_dur_ds"].ToString(),

                            RequestCloseDate = reader["rqs_cls_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rqs_cls_dt"],

                            LineManagersResumptionDate = reader["lm_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_rsmptn_dt"],
                            LineManagerConfirmResumptionDate = reader["lm_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_confm_dt"],
                            LineManagerConfirmResumptionBy = reader["lm_confm_by"] == DBNull.Value ? string.Empty : reader["lm_confm_by"].ToString(),

                            HrResumptionDate = reader["hr_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_rsmptn_dt"],
                            HrConfirmResumptionDate = reader["hr_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_confm_dt"],
                            HrConfirmResumptionBy = reader["hr_confm_by"] == DBNull.Value ? string.Empty : reader["hr_confm_by"].ToString(),

                            ApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
                            ApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],
                            ApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
                            ApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
                            ApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveList;
        }
        public async Task<List<EmployeeLeave>> GetByEmployeeIdnYearnMonthAsync(string employeeId, int year, int month, bool isPlan)
        {
            List<EmployeeLeave> leaveList = new List<EmployeeLeave>();
            string query = string.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT v.lvs_inf_id, v.emp_id, v.unit_id, v.dept_id, ");
            sb.Append("v.loc_id, v.lvs_yr, v.lvs_typ_cd, v.lvs_rsn, v.lvs_sts, ");
            sb.Append("v.is_pln, v.prp_lvs_sdt, v.prp_lvs_edt, v.prp_lvs_dur, ");
            sb.Append("v.act_lvs_dur, v.prp_dur_ds, v.apv_lvs_sdt, v.apv_lvs_edt,  ");
            sb.Append("v.apv_lvs_dur, v.apv_dur_ds, v.act_lvs_sdt, v.act_lvs_edt, ");
            sb.Append("v.act_dur_ds, v.lm_rsmptn_dt, v.lm_confm_dt, v.lm_confm_by, ");
            sb.Append("v.hr_rsmptn_dt, v.hr_confm_dt, v.hr_confm_by, v.rqs_cls_dt, ");
            sb.Append("v.is_lm_aprv, v.is_hd_aprv, v.is_hr_aprv, v.is_xm_aprv, ");
            sb.Append("v.is_sm_aprv, v.prp_lvs_dur_typ, v.apv_lvs_dur_typ, v.act_lvs_dur_typ,  ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = v.emp_id) ");
            sb.Append("as emp_nm, (SELECT lvs_typ_nm FROM public.lms_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = v.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = v.unit_id) ");
            sb.Append("as unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= v.dept_id) as dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = v.loc_id) as loc_nm ");
            sb.Append("FROM public.lms_lvs_infs v ");
            sb.Append("WHERE (v.emp_id = @emp_id) ");
            sb.Append("AND (DATE_PART('Year', v.prp_lvs_sdt) = @lvs_yr) ");
            sb.Append("AND (DATE_PART('Month', v.prp_lvs_sdt) = @lvs_month) ");
            sb.Append("AND (v.is_pln = @is_pln) ORDER BY v.prp_lvs_sdt; ");
            query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                    var lvs_yr = cmd.Parameters.Add("@lvs_yr", NpgsqlDbType.Integer);
                    var lvs_month = cmd.Parameters.Add("@lvs_month", NpgsqlDbType.Integer);
                    var is_pln = cmd.Parameters.Add("@is_pln", NpgsqlDbType.Boolean);
                    await cmd.PrepareAsync();
                    emp_id.Value = employeeId;
                    lvs_yr.Value = year;
                    lvs_month.Value = month;
                    is_pln.Value = isPlan;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        leaveList.Add(new EmployeeLeave()
                        {
                            Id = reader["lvs_inf_id"] == DBNull.Value ? 0L : (long)reader["lvs_inf_id"],
                            EmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
                            EmployeeFullName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),

                            UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                            UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
                            DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                            DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
                            LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                            LocationName = reader["loc_nm"] == DBNull.Value ? string.Empty : reader["loc_nm"].ToString(),

                            LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"],
                            LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString(),
                            LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
                            LeaveReason = reader["lvs_rsn"] == DBNull.Value ? string.Empty : reader["lvs_rsn"].ToString(),
                            LeaveStatus = reader["lvs_sts"] == DBNull.Value ? string.Empty : reader["lvs_sts"].ToString(),
                            IsPlan = reader["is_pln"] == DBNull.Value ? true : (bool)reader["is_pln"],

                            ProposedLeaveStartDate = reader["prp_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_sdt"],
                            ProposedLeaveEndDate = reader["prp_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_edt"],
                            ProposedLeaveDuration = reader["prp_lvs_dur"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur"],
                            ProposedDurationTypeId = reader["prp_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur_typ"],
                            ProposedDurationDescription = reader["prp_dur_ds"] == DBNull.Value ? string.Empty : reader["prp_dur_ds"].ToString(),

                            ApprovedLeaveStartDate = reader["apv_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_sdt"],
                            ApprovedLeaveEndDate = reader["apv_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_edt"],
                            ApprovedLeaveDuration = reader["apv_lvs_dur"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur"],
                            ApprovedDurationTypeId = reader["apv_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur_typ"],
                            ApprovedDurationDescription = reader["apv_dur_ds"] == DBNull.Value ? string.Empty : reader["apv_dur_ds"].ToString(),

                            ActualLeaveStartDate = reader["act_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_sdt"],
                            ActualLeaveEndDate = reader["act_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_edt"],
                            ActualLeaveDuration = reader["act_lvs_dur"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur"],
                            ActualDurationTypeId = reader["act_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur_typ"],
                            ActualDurationDescription = reader["act_dur_ds"] == DBNull.Value ? string.Empty : reader["act_dur_ds"].ToString(),

                            RequestCloseDate = reader["rqs_cls_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rqs_cls_dt"],

                            LineManagersResumptionDate = reader["lm_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_rsmptn_dt"],
                            LineManagerConfirmResumptionDate = reader["lm_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_confm_dt"],
                            LineManagerConfirmResumptionBy = reader["lm_confm_by"] == DBNull.Value ? string.Empty : reader["lm_confm_by"].ToString(),

                            HrResumptionDate = reader["hr_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_rsmptn_dt"],
                            HrConfirmResumptionDate = reader["hr_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_confm_dt"],
                            HrConfirmResumptionBy = reader["hr_confm_by"] == DBNull.Value ? string.Empty : reader["hr_confm_by"].ToString(),

                            ApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
                            ApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],
                            ApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
                            ApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
                            ApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveList;
        }
        public async Task<List<EmployeeLeave>> GetByEmployeeIdnYearnMonthnStatusAsync(string employeeId, int year, int month, string leaveStatus, bool isPlan)
        {
            List<EmployeeLeave> leaveList = new List<EmployeeLeave>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT v.lvs_inf_id, v.emp_id, v.unit_id, v.dept_id, ");
            sb.Append("v.loc_id, v.lvs_yr, v.lvs_typ_cd, v.lvs_rsn, v.lvs_sts, ");
            sb.Append("v.is_pln, v.prp_lvs_sdt, v.prp_lvs_edt, v.prp_lvs_dur, ");
            sb.Append("v.act_lvs_dur, v.prp_dur_ds, v.apv_lvs_sdt, v.apv_lvs_edt,  ");
            sb.Append("v.apv_lvs_dur, v.apv_dur_ds, v.act_lvs_sdt, v.act_lvs_edt, ");
            sb.Append("v.act_dur_ds, v.lm_rsmptn_dt, v.lm_confm_dt, v.lm_confm_by, ");
            sb.Append("v.hr_rsmptn_dt, v.hr_confm_dt, v.hr_confm_by, v.rqs_cls_dt, ");
            sb.Append("v.is_lm_aprv, v.is_hd_aprv, v.is_hr_aprv, v.is_xm_aprv, ");
            sb.Append("v.is_sm_aprv, v.prp_lvs_dur_typ, v.apv_lvs_dur_typ, v.act_lvs_dur_typ,  ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = v.emp_id) ");
            sb.Append("as emp_nm, (SELECT lvs_typ_nm FROM public.lms_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = v.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = v.unit_id) ");
            sb.Append("as unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= v.dept_id) as dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = v.loc_id) as loc_nm ");
            sb.Append("FROM public.lms_lvs_infs v ");
            sb.Append("WHERE (v.emp_id = @emp_id) ");
            sb.Append("AND (DATE_PART('Year', v.prp_lvs_sdt) = @lvs_yr) ");
            sb.Append("AND (DATE_PART('Month', v.prp_lvs_sdt) = @lvs_month) ");
            sb.Append("AND (v.lvs_sts = @lvs_sts) ");
            sb.Append("AND (v.is_pln = @is_pln) ORDER BY v.prp_lvs_sdt; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                    var lvs_yr = cmd.Parameters.Add("@lvs_yr", NpgsqlDbType.Integer);
                    var lvs_month = cmd.Parameters.Add("@lvs_month", NpgsqlDbType.Integer);
                    var lvs_sts = cmd.Parameters.Add("@lvs_sts", NpgsqlDbType.Text);
                    var is_pln = cmd.Parameters.Add("@is_pln", NpgsqlDbType.Boolean);
                    await cmd.PrepareAsync();
                    emp_id.Value = employeeId;
                    lvs_yr.Value = year;
                    lvs_month.Value = month;
                    lvs_sts.Value = leaveStatus;
                    is_pln.Value = isPlan;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        leaveList.Add(new EmployeeLeave()
                        {
                            Id = reader["lvs_inf_id"] == DBNull.Value ? 0L : (long)reader["lvs_inf_id"],
                            EmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
                            EmployeeFullName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),

                            UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                            UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
                            DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                            DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
                            LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                            LocationName = reader["loc_nm"] == DBNull.Value ? string.Empty : reader["loc_nm"].ToString(),

                            LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"],
                            LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString(),
                            LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
                            LeaveReason = reader["lvs_rsn"] == DBNull.Value ? string.Empty : reader["lvs_rsn"].ToString(),
                            LeaveStatus = reader["lvs_sts"] == DBNull.Value ? string.Empty : reader["lvs_sts"].ToString(),
                            IsPlan = reader["is_pln"] == DBNull.Value ? true : (bool)reader["is_pln"],

                            ProposedLeaveStartDate = reader["prp_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_sdt"],
                            ProposedLeaveEndDate = reader["prp_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_edt"],
                            ProposedLeaveDuration = reader["prp_lvs_dur"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur"],
                            ProposedDurationTypeId = reader["prp_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur_typ"],
                            ProposedDurationDescription = reader["prp_dur_ds"] == DBNull.Value ? string.Empty : reader["prp_dur_ds"].ToString(),

                            ApprovedLeaveStartDate = reader["apv_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_sdt"],
                            ApprovedLeaveEndDate = reader["apv_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_edt"],
                            ApprovedLeaveDuration = reader["apv_lvs_dur"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur"],
                            ApprovedDurationTypeId = reader["apv_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur_typ"],
                            ApprovedDurationDescription = reader["apv_dur_ds"] == DBNull.Value ? string.Empty : reader["apv_dur_ds"].ToString(),

                            ActualLeaveStartDate = reader["act_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_sdt"],
                            ActualLeaveEndDate = reader["act_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_edt"],
                            ActualLeaveDuration = reader["act_lvs_dur"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur"],
                            ActualDurationTypeId = reader["act_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur_typ"],
                            ActualDurationDescription = reader["act_dur_ds"] == DBNull.Value ? string.Empty : reader["act_dur_ds"].ToString(),

                            RequestCloseDate = reader["rqs_cls_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rqs_cls_dt"],

                            LineManagersResumptionDate = reader["lm_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_rsmptn_dt"],
                            LineManagerConfirmResumptionDate = reader["lm_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_confm_dt"],
                            LineManagerConfirmResumptionBy = reader["lm_confm_by"] == DBNull.Value ? string.Empty : reader["lm_confm_by"].ToString(),

                            HrResumptionDate = reader["hr_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_rsmptn_dt"],
                            HrConfirmResumptionDate = reader["hr_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_confm_dt"],
                            HrConfirmResumptionBy = reader["hr_confm_by"] == DBNull.Value ? string.Empty : reader["hr_confm_by"].ToString(),

                            ApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
                            ApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],
                            ApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
                            ApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
                            ApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveList;
        }
        public async Task<List<EmployeeLeave>> GetByEmployeeIdnStatusAsync(string employeeId, string leaveStatus, bool isPlan)
        {
            List<EmployeeLeave> leaveList = new List<EmployeeLeave>();
            string query = string.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT v.lvs_inf_id, v.emp_id, v.unit_id, v.dept_id, ");
            sb.Append("v.loc_id, v.lvs_yr, v.lvs_typ_cd, v.lvs_rsn, v.lvs_sts, ");
            sb.Append("v.is_pln, v.prp_lvs_sdt, v.prp_lvs_edt, v.prp_lvs_dur, ");
            sb.Append("v.act_lvs_dur, v.prp_dur_ds, v.apv_lvs_sdt, v.apv_lvs_edt,  ");
            sb.Append("v.apv_lvs_dur, v.apv_dur_ds, v.act_lvs_sdt, v.act_lvs_edt, ");
            sb.Append("v.act_dur_ds, v.lm_rsmptn_dt, v.lm_confm_dt, v.lm_confm_by, ");
            sb.Append("v.hr_rsmptn_dt, v.hr_confm_dt, v.hr_confm_by, v.rqs_cls_dt, ");
            sb.Append("v.is_lm_aprv, v.is_hd_aprv, v.is_hr_aprv, v.is_xm_aprv, ");
            sb.Append("v.is_sm_aprv, v.prp_lvs_dur_typ, v.apv_lvs_dur_typ, v.act_lvs_dur_typ,  ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = v.emp_id) ");
            sb.Append("as emp_nm, (SELECT lvs_typ_nm FROM public.lms_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = v.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = v.unit_id) ");
            sb.Append("as unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= v.dept_id) as dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = v.loc_id) as loc_nm ");
            sb.Append("FROM public.lms_lvs_infs v ");
            sb.Append("WHERE (v.emp_id = @emp_id) ");
            sb.Append("AND (v.lvs_sts = @lvs_sts) ");
            sb.Append("AND (v.is_pln = @is_pln) ORDER BY v.prp_lvs_sdt; ");
            query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                    var is_pln = cmd.Parameters.Add("@is_pln", NpgsqlDbType.Boolean);
                    await cmd.PrepareAsync();
                    emp_id.Value = employeeId;
                    is_pln.Value = isPlan;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        leaveList.Add(new EmployeeLeave()
                        {
                            Id = reader["lvs_inf_id"] == DBNull.Value ? 0L : (long)reader["lvs_inf_id"],
                            EmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
                            EmployeeFullName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),

                            UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                            UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
                            DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                            DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
                            LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                            LocationName = reader["loc_nm"] == DBNull.Value ? string.Empty : reader["loc_nm"].ToString(),

                            LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"],
                            LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString(),
                            LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
                            LeaveReason = reader["lvs_rsn"] == DBNull.Value ? string.Empty : reader["lvs_rsn"].ToString(),
                            LeaveStatus = reader["lvs_sts"] == DBNull.Value ? string.Empty : reader["lvs_sts"].ToString(),
                            IsPlan = reader["is_pln"] == DBNull.Value ? true : (bool)reader["is_pln"],

                            ProposedLeaveStartDate = reader["prp_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_sdt"],
                            ProposedLeaveEndDate = reader["prp_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_edt"],
                            ProposedLeaveDuration = reader["prp_lvs_dur"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur"],
                            ProposedDurationTypeId = reader["prp_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur_typ"],
                            ProposedDurationDescription = reader["prp_dur_ds"] == DBNull.Value ? string.Empty : reader["prp_dur_ds"].ToString(),

                            ApprovedLeaveStartDate = reader["apv_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_sdt"],
                            ApprovedLeaveEndDate = reader["apv_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_edt"],
                            ApprovedLeaveDuration = reader["apv_lvs_dur"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur"],
                            ApprovedDurationTypeId = reader["apv_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur_typ"],
                            ApprovedDurationDescription = reader["apv_dur_ds"] == DBNull.Value ? string.Empty : reader["apv_dur_ds"].ToString(),

                            ActualLeaveStartDate = reader["act_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_sdt"],
                            ActualLeaveEndDate = reader["act_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_edt"],
                            ActualLeaveDuration = reader["act_lvs_dur"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur"],
                            ActualDurationTypeId = reader["act_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur_typ"],
                            ActualDurationDescription = reader["act_dur_ds"] == DBNull.Value ? string.Empty : reader["act_dur_ds"].ToString(),

                            RequestCloseDate = reader["rqs_cls_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rqs_cls_dt"],

                            LineManagersResumptionDate = reader["lm_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_rsmptn_dt"],
                            LineManagerConfirmResumptionDate = reader["lm_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_confm_dt"],
                            LineManagerConfirmResumptionBy = reader["lm_confm_by"] == DBNull.Value ? string.Empty : reader["lm_confm_by"].ToString(),

                            HrResumptionDate = reader["hr_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_rsmptn_dt"],
                            HrConfirmResumptionDate = reader["hr_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_confm_dt"],
                            HrConfirmResumptionBy = reader["hr_confm_by"] == DBNull.Value ? string.Empty : reader["hr_confm_by"].ToString(),

                            ApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
                            ApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],
                            ApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
                            ApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
                            ApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveList;
        }
        public async Task<List<EmployeeLeave>> GetByEmployeeIdnYearnStatusAsync(string employeeId, int year, string leaveStatus, bool isPlan)
        {
            List<EmployeeLeave> leaveList = new List<EmployeeLeave>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT v.lvs_inf_id, v.emp_id, v.unit_id, v.dept_id, ");
            sb.Append("v.loc_id, v.lvs_yr, v.lvs_typ_cd, v.lvs_rsn, v.lvs_sts, ");
            sb.Append("v.is_pln, v.prp_lvs_sdt, v.prp_lvs_edt, v.prp_lvs_dur, ");
            sb.Append("v.act_lvs_dur, v.prp_dur_ds, v.apv_lvs_sdt, v.apv_lvs_edt,  ");
            sb.Append("v.apv_lvs_dur, v.apv_dur_ds, v.act_lvs_sdt, v.act_lvs_edt, ");
            sb.Append("v.act_dur_ds, v.lm_rsmptn_dt, v.lm_confm_dt, v.lm_confm_by, ");
            sb.Append("v.hr_rsmptn_dt, v.hr_confm_dt, v.hr_confm_by, v.rqs_cls_dt, ");
            sb.Append("v.is_lm_aprv, v.is_hd_aprv, v.is_hr_aprv, v.is_xm_aprv, ");
            sb.Append("v.is_sm_aprv, v.prp_lvs_dur_typ, v.apv_lvs_dur_typ, v.act_lvs_dur_typ,  ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = v.emp_id) ");
            sb.Append("as emp_nm, (SELECT lvs_typ_nm FROM public.lms_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = v.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = v.unit_id) ");
            sb.Append("as unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= v.dept_id) as dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = v.loc_id) as loc_nm ");
            sb.Append("FROM public.lms_lvs_infs v ");
            sb.Append("WHERE (v.emp_id = @emp_id) AND (v.lvs_yr = @lvs_yr) ");
            sb.Append("AND (v.lvs_sts = @lvs_sts) AND (v.is_pln = @is_pln) ");
            sb.Append("ORDER BY v.prp_lvs_sdt; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                    var lvs_yr = cmd.Parameters.Add("@lvs_yr", NpgsqlDbType.Integer);
                    var is_pln = cmd.Parameters.Add("@is_pln", NpgsqlDbType.Boolean);
                    await cmd.PrepareAsync();
                    emp_id.Value = employeeId;
                    lvs_yr.Value = year;
                    is_pln.Value = isPlan;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        leaveList.Add(new EmployeeLeave()
                        {
                            Id = reader["lvs_inf_id"] == DBNull.Value ? 0L : (long)reader["lvs_inf_id"],
                            EmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
                            EmployeeFullName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),

                            UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                            UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
                            DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                            DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
                            LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                            LocationName = reader["loc_nm"] == DBNull.Value ? string.Empty : reader["loc_nm"].ToString(),

                            LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"],
                            LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString(),
                            LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
                            LeaveReason = reader["lvs_rsn"] == DBNull.Value ? string.Empty : reader["lvs_rsn"].ToString(),
                            LeaveStatus = reader["lvs_sts"] == DBNull.Value ? string.Empty : reader["lvs_sts"].ToString(),
                            IsPlan = reader["is_pln"] == DBNull.Value ? true : (bool)reader["is_pln"],

                            ProposedLeaveStartDate = reader["prp_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_sdt"],
                            ProposedLeaveEndDate = reader["prp_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_edt"],
                            ProposedLeaveDuration = reader["prp_lvs_dur"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur"],
                            ProposedDurationTypeId = reader["prp_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur_typ"],
                            ProposedDurationDescription = reader["prp_dur_ds"] == DBNull.Value ? string.Empty : reader["prp_dur_ds"].ToString(),

                            ApprovedLeaveStartDate = reader["apv_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_sdt"],
                            ApprovedLeaveEndDate = reader["apv_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_edt"],
                            ApprovedLeaveDuration = reader["apv_lvs_dur"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur"],
                            ApprovedDurationTypeId = reader["apv_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur_typ"],
                            ApprovedDurationDescription = reader["apv_dur_ds"] == DBNull.Value ? string.Empty : reader["apv_dur_ds"].ToString(),

                            ActualLeaveStartDate = reader["act_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_sdt"],
                            ActualLeaveEndDate = reader["act_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_edt"],
                            ActualLeaveDuration = reader["act_lvs_dur"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur"],
                            ActualDurationTypeId = reader["act_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur_typ"],
                            ActualDurationDescription = reader["act_dur_ds"] == DBNull.Value ? string.Empty : reader["act_dur_ds"].ToString(),

                            RequestCloseDate = reader["rqs_cls_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rqs_cls_dt"],

                            LineManagersResumptionDate = reader["lm_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_rsmptn_dt"],
                            LineManagerConfirmResumptionDate = reader["lm_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_confm_dt"],
                            LineManagerConfirmResumptionBy = reader["lm_confm_by"] == DBNull.Value ? string.Empty : reader["lm_confm_by"].ToString(),

                            HrResumptionDate = reader["hr_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_rsmptn_dt"],
                            HrConfirmResumptionDate = reader["hr_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_confm_dt"],
                            HrConfirmResumptionBy = reader["hr_confm_by"] == DBNull.Value ? string.Empty : reader["hr_confm_by"].ToString(),

                            ApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
                            ApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],
                            ApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
                            ApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
                            ApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveList;
        }
        #endregion

        #region Employee Leaves By Employee Name
        //=== Employee Leaves By Employee Name Read Action Methods ========//
        public async Task<List<EmployeeLeave>> GetByEmployeeNamenYearAsync(string employeeName, int year, bool isPlan)
        {
            List<EmployeeLeave> leaveList = new List<EmployeeLeave>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT v.lvs_inf_id, v.emp_id, v.unit_id, v.dept_id, ");
            sb.Append("v.loc_id, v.lvs_yr, v.lvs_typ_cd, v.lvs_rsn, v.lvs_sts, ");
            sb.Append("v.is_pln, v.prp_lvs_sdt, v.prp_lvs_edt, v.prp_lvs_dur, ");
            sb.Append("v.act_lvs_dur, v.prp_dur_ds, v.apv_lvs_sdt, v.apv_lvs_edt,  ");
            sb.Append("v.apv_lvs_dur, v.apv_dur_ds, v.act_lvs_sdt, v.act_lvs_edt, ");
            sb.Append("v.act_dur_ds, v.lm_rsmptn_dt, v.lm_confm_dt, v.lm_confm_by, ");
            sb.Append("v.hr_rsmptn_dt, v.hr_confm_dt, v.hr_confm_by, v.rqs_cls_dt, ");
            sb.Append("v.is_lm_aprv, v.is_hd_aprv, v.is_hr_aprv, v.is_xm_aprv, ");
            sb.Append("v.is_sm_aprv, v.prp_lvs_dur_typ, v.apv_lvs_dur_typ, v.act_lvs_dur_typ,  ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = v.emp_id) ");
            sb.Append("as emp_nm, (SELECT lvs_typ_nm FROM public.lms_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = v.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = v.unit_id) ");
            sb.Append("as unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= v.dept_id) as dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = v.loc_id) as loc_nm ");
            sb.Append("FROM public.lms_lvs_infs v ");
            sb.Append("WHERE (v.is_pln = @is_pln) AND (v.lvs_yr = @lvs_yr) ");
            sb.Append("AND v.emp_id IN (SELECT p.id FROM public.gst_prsns p ");
            sb.Append("WHERE p.fullname = @emp_nm)) ");
            sb.Append("ORDER BY v.prp_lvs_sdt; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var emp_nm = cmd.Parameters.Add("@emp_nm", NpgsqlDbType.Text);
                    var lvs_yr = cmd.Parameters.Add("@lvs_yr", NpgsqlDbType.Integer);
                    var is_pln = cmd.Parameters.Add("@is_pln", NpgsqlDbType.Boolean);
                    await cmd.PrepareAsync();
                    emp_nm.Value = employeeName;
                    lvs_yr.Value = year;
                    is_pln.Value = isPlan;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        leaveList.Add(new EmployeeLeave()
                        {
                            Id = reader["lvs_inf_id"] == DBNull.Value ? 0L : (long)reader["lvs_inf_id"],
                            EmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
                            EmployeeFullName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),

                            UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                            UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
                            DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                            DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
                            LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                            LocationName = reader["loc_nm"] == DBNull.Value ? string.Empty : reader["loc_nm"].ToString(),

                            LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"],
                            LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString(),
                            LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
                            LeaveReason = reader["lvs_rsn"] == DBNull.Value ? string.Empty : reader["lvs_rsn"].ToString(),
                            LeaveStatus = reader["lvs_sts"] == DBNull.Value ? string.Empty : reader["lvs_sts"].ToString(),
                            IsPlan = reader["is_pln"] == DBNull.Value ? true : (bool)reader["is_pln"],

                            ProposedLeaveStartDate = reader["prp_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_sdt"],
                            ProposedLeaveEndDate = reader["prp_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_edt"],
                            ProposedLeaveDuration = reader["prp_lvs_dur"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur"],
                            ProposedDurationTypeId = reader["prp_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur_typ"],
                            ProposedDurationDescription = reader["prp_dur_ds"] == DBNull.Value ? string.Empty : reader["prp_dur_ds"].ToString(),

                            ApprovedLeaveStartDate = reader["apv_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_sdt"],
                            ApprovedLeaveEndDate = reader["apv_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_edt"],
                            ApprovedLeaveDuration = reader["apv_lvs_dur"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur"],
                            ApprovedDurationTypeId = reader["apv_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur_typ"],
                            ApprovedDurationDescription = reader["apv_dur_ds"] == DBNull.Value ? string.Empty : reader["apv_dur_ds"].ToString(),

                            ActualLeaveStartDate = reader["act_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_sdt"],
                            ActualLeaveEndDate = reader["act_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_edt"],
                            ActualLeaveDuration = reader["act_lvs_dur"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur"],
                            ActualDurationTypeId = reader["act_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur_typ"],
                            ActualDurationDescription = reader["act_dur_ds"] == DBNull.Value ? string.Empty : reader["act_dur_ds"].ToString(),

                            RequestCloseDate = reader["rqs_cls_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rqs_cls_dt"],

                            LineManagersResumptionDate = reader["lm_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_rsmptn_dt"],
                            LineManagerConfirmResumptionDate = reader["lm_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_confm_dt"],
                            LineManagerConfirmResumptionBy = reader["lm_confm_by"] == DBNull.Value ? string.Empty : reader["lm_confm_by"].ToString(),

                            HrResumptionDate = reader["hr_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_rsmptn_dt"],
                            HrConfirmResumptionDate = reader["hr_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_confm_dt"],
                            HrConfirmResumptionBy = reader["hr_confm_by"] == DBNull.Value ? string.Empty : reader["hr_confm_by"].ToString(),

                            ApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
                            ApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],
                            ApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
                            ApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
                            ApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveList;
        }
        public async Task<List<EmployeeLeave>> GetByEmployeeNamenYearnStatusAsync(string employeeName, int year, string leaveStatus, bool isPlan)
        {
            List<EmployeeLeave> leaveList = new List<EmployeeLeave>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT v.lvs_inf_id, v.emp_id, v.unit_id, v.dept_id, ");
            sb.Append("v.loc_id, v.lvs_yr, v.lvs_typ_cd, v.lvs_rsn, v.lvs_sts, ");
            sb.Append("v.is_pln, v.prp_lvs_sdt, v.prp_lvs_edt, v.prp_lvs_dur, ");
            sb.Append("v.act_lvs_dur, v.prp_dur_ds, v.apv_lvs_sdt, v.apv_lvs_edt,  ");
            sb.Append("v.apv_lvs_dur, v.apv_dur_ds, v.act_lvs_sdt, v.act_lvs_edt, ");
            sb.Append("v.act_dur_ds, v.lm_rsmptn_dt, v.lm_confm_dt, v.lm_confm_by, ");
            sb.Append("v.hr_rsmptn_dt, v.hr_confm_dt, v.hr_confm_by, v.rqs_cls_dt, ");
            sb.Append("v.is_lm_aprv, v.is_hd_aprv, v.is_hr_aprv, v.is_xm_aprv, ");
            sb.Append("v.is_sm_aprv, v.prp_lvs_dur_typ, v.apv_lvs_dur_typ, v.act_lvs_dur_typ,  ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = v.emp_id) ");
            sb.Append("as emp_nm, (SELECT lvs_typ_nm FROM public.lms_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = v.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = v.unit_id) ");
            sb.Append("as unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= v.dept_id) as dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = v.loc_id) as loc_nm ");
            sb.Append("FROM public.lms_lvs_infs v ");
            sb.Append("WHERE (v.is_pln = @is_pln) AND (v.lvs_yr = @lvs_yr) ");
            sb.Append("AND (v.lvs_sts = @lvs_sts) ");
            sb.Append("AND v.emp_id IN (SELECT p.id FROM public.gst_prsns p ");
            sb.Append("WHERE p.fullname = @emp_nm)) ");
            sb.Append("ORDER BY v.prp_lvs_sdt; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var emp_nm = cmd.Parameters.Add("@emp_nm", NpgsqlDbType.Text);
                    var lvs_yr = cmd.Parameters.Add("@lvs_yr", NpgsqlDbType.Integer);
                    var is_pln = cmd.Parameters.Add("@is_pln", NpgsqlDbType.Boolean);
                    await cmd.PrepareAsync();
                    emp_nm.Value = employeeName;
                    lvs_yr.Value = year;
                    is_pln.Value = isPlan;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        leaveList.Add(new EmployeeLeave()
                        {
                            Id = reader["lvs_inf_id"] == DBNull.Value ? 0L : (long)reader["lvs_inf_id"],
                            EmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
                            EmployeeFullName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),

                            UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                            UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
                            DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                            DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
                            LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                            LocationName = reader["loc_nm"] == DBNull.Value ? string.Empty : reader["loc_nm"].ToString(),

                            LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"],
                            LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString(),
                            LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
                            LeaveReason = reader["lvs_rsn"] == DBNull.Value ? string.Empty : reader["lvs_rsn"].ToString(),
                            LeaveStatus = reader["lvs_sts"] == DBNull.Value ? string.Empty : reader["lvs_sts"].ToString(),
                            IsPlan = reader["is_pln"] == DBNull.Value ? true : (bool)reader["is_pln"],

                            ProposedLeaveStartDate = reader["prp_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_sdt"],
                            ProposedLeaveEndDate = reader["prp_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_edt"],
                            ProposedLeaveDuration = reader["prp_lvs_dur"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur"],
                            ProposedDurationTypeId = reader["prp_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur_typ"],
                            ProposedDurationDescription = reader["prp_dur_ds"] == DBNull.Value ? string.Empty : reader["prp_dur_ds"].ToString(),

                            ApprovedLeaveStartDate = reader["apv_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_sdt"],
                            ApprovedLeaveEndDate = reader["apv_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_edt"],
                            ApprovedLeaveDuration = reader["apv_lvs_dur"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur"],
                            ApprovedDurationTypeId = reader["apv_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur_typ"],
                            ApprovedDurationDescription = reader["apv_dur_ds"] == DBNull.Value ? string.Empty : reader["apv_dur_ds"].ToString(),

                            ActualLeaveStartDate = reader["act_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_sdt"],
                            ActualLeaveEndDate = reader["act_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_edt"],
                            ActualLeaveDuration = reader["act_lvs_dur"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur"],
                            ActualDurationTypeId = reader["act_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur_typ"],
                            ActualDurationDescription = reader["act_dur_ds"] == DBNull.Value ? string.Empty : reader["act_dur_ds"].ToString(),

                            RequestCloseDate = reader["rqs_cls_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rqs_cls_dt"],

                            LineManagersResumptionDate = reader["lm_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_rsmptn_dt"],
                            LineManagerConfirmResumptionDate = reader["lm_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_confm_dt"],
                            LineManagerConfirmResumptionBy = reader["lm_confm_by"] == DBNull.Value ? string.Empty : reader["lm_confm_by"].ToString(),

                            HrResumptionDate = reader["hr_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_rsmptn_dt"],
                            HrConfirmResumptionDate = reader["hr_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_confm_dt"],
                            HrConfirmResumptionBy = reader["hr_confm_by"] == DBNull.Value ? string.Empty : reader["hr_confm_by"].ToString(),

                            ApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
                            ApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],
                            ApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
                            ApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
                            ApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveList;
        }
        public async Task<List<EmployeeLeave>> GetByEmployeeNamenYearnMonthnStatusAsync(string employeeName, int year, int month, string leaveStatus, bool isPlan)
        {
            List<EmployeeLeave> leaveList = new List<EmployeeLeave>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT v.lvs_inf_id, v.emp_id, v.unit_id, v.dept_id, ");
            sb.Append("v.loc_id, v.lvs_yr, v.lvs_typ_cd, v.lvs_rsn, v.lvs_sts, ");
            sb.Append("v.is_pln, v.prp_lvs_sdt, v.prp_lvs_edt, v.prp_lvs_dur, ");
            sb.Append("v.act_lvs_dur, v.prp_dur_ds, v.apv_lvs_sdt, v.apv_lvs_edt,  ");
            sb.Append("v.apv_lvs_dur, v.apv_dur_ds, v.act_lvs_sdt, v.act_lvs_edt, ");
            sb.Append("v.act_dur_ds, v.lm_rsmptn_dt, v.lm_confm_dt, v.lm_confm_by, ");
            sb.Append("v.hr_rsmptn_dt, v.hr_confm_dt, v.hr_confm_by, v.rqs_cls_dt, ");
            sb.Append("v.is_lm_aprv, v.is_hd_aprv, v.is_hr_aprv, v.is_xm_aprv, ");
            sb.Append("v.is_sm_aprv, v.prp_lvs_dur_typ, v.apv_lvs_dur_typ, v.act_lvs_dur_typ,  ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = v.emp_id) ");
            sb.Append("as emp_nm, (SELECT lvs_typ_nm FROM public.lms_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = v.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = v.unit_id) ");
            sb.Append("as unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= v.dept_id) as dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = v.loc_id) as loc_nm ");
            sb.Append("FROM public.lms_lvs_infs v ");
            sb.Append("WHERE (v.is_pln = @is_pln) AND (v.lvs_sts = @lvs_sts) ");
            sb.Append("AND DATE_PART('Year', v.prp_lvs_sdt) = @lvs_year ");
            sb.Append("AND DATE_PART('Month', v.prp_lvs_sdt) = @lvs_month ");
            sb.Append("AND v.emp_id IN (SELECT p.id FROM public.gst_prsns p ");
            sb.Append("WHERE p.fullname = @emp_nm)) ");
            sb.Append("ORDER BY v.prp_lvs_sdt; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var emp_nm = cmd.Parameters.Add("@emp_nm", NpgsqlDbType.Text);
                    var lvs_year = cmd.Parameters.Add("@lvs_year", NpgsqlDbType.Integer);
                    var lvs_month = cmd.Parameters.Add("@lvs_month", NpgsqlDbType.Integer);
                    var lvs_sts = cmd.Parameters.Add("@lvs_sts", NpgsqlDbType.Text);
                    var is_pln = cmd.Parameters.Add("@is_pln", NpgsqlDbType.Boolean);
                    await cmd.PrepareAsync();
                    emp_nm.Value = employeeName;
                    lvs_year.Value = year;
                    lvs_month.Value = month;
                    lvs_sts.Value = leaveStatus;
                    is_pln.Value = isPlan;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        leaveList.Add(new EmployeeLeave()
                        {
                            Id = reader["lvs_inf_id"] == DBNull.Value ? 0L : (long)reader["lvs_inf_id"],
                            EmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
                            EmployeeFullName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),

                            UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                            UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
                            DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                            DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
                            LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                            LocationName = reader["loc_nm"] == DBNull.Value ? string.Empty : reader["loc_nm"].ToString(),

                            LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"],
                            LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString(),
                            LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
                            LeaveReason = reader["lvs_rsn"] == DBNull.Value ? string.Empty : reader["lvs_rsn"].ToString(),
                            LeaveStatus = reader["lvs_sts"] == DBNull.Value ? string.Empty : reader["lvs_sts"].ToString(),
                            IsPlan = reader["is_pln"] == DBNull.Value ? true : (bool)reader["is_pln"],

                            ProposedLeaveStartDate = reader["prp_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_sdt"],
                            ProposedLeaveEndDate = reader["prp_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_edt"],
                            ProposedLeaveDuration = reader["prp_lvs_dur"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur"],
                            ProposedDurationTypeId = reader["prp_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur_typ"],
                            ProposedDurationDescription = reader["prp_dur_ds"] == DBNull.Value ? string.Empty : reader["prp_dur_ds"].ToString(),

                            ApprovedLeaveStartDate = reader["apv_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_sdt"],
                            ApprovedLeaveEndDate = reader["apv_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_edt"],
                            ApprovedLeaveDuration = reader["apv_lvs_dur"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur"],
                            ApprovedDurationTypeId = reader["apv_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur_typ"],
                            ApprovedDurationDescription = reader["apv_dur_ds"] == DBNull.Value ? string.Empty : reader["apv_dur_ds"].ToString(),

                            ActualLeaveStartDate = reader["act_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_sdt"],
                            ActualLeaveEndDate = reader["act_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_edt"],
                            ActualLeaveDuration = reader["act_lvs_dur"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur"],
                            ActualDurationTypeId = reader["act_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur_typ"],
                            ActualDurationDescription = reader["act_dur_ds"] == DBNull.Value ? string.Empty : reader["act_dur_ds"].ToString(),

                            RequestCloseDate = reader["rqs_cls_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rqs_cls_dt"],

                            LineManagersResumptionDate = reader["lm_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_rsmptn_dt"],
                            LineManagerConfirmResumptionDate = reader["lm_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_confm_dt"],
                            LineManagerConfirmResumptionBy = reader["lm_confm_by"] == DBNull.Value ? string.Empty : reader["lm_confm_by"].ToString(),

                            HrResumptionDate = reader["hr_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_rsmptn_dt"],
                            HrConfirmResumptionDate = reader["hr_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_confm_dt"],
                            HrConfirmResumptionBy = reader["hr_confm_by"] == DBNull.Value ? string.Empty : reader["hr_confm_by"].ToString(),

                            ApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
                            ApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],
                            ApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
                            ApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
                            ApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveList;
        }
        public async Task<List<EmployeeLeave>> GetByEmployeeNamenYearnMonthAsync(string employeeName, int year, int month, bool isPlan)
        {
            List<EmployeeLeave> leaveList = new List<EmployeeLeave>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT v.lvs_inf_id, v.emp_id, v.unit_id, v.dept_id, ");
            sb.Append("v.loc_id, v.lvs_yr, v.lvs_typ_cd, v.lvs_rsn, v.lvs_sts, ");
            sb.Append("v.is_pln, v.prp_lvs_sdt, v.prp_lvs_edt, v.prp_lvs_dur, ");
            sb.Append("v.act_lvs_dur, v.prp_dur_ds, v.apv_lvs_sdt, v.apv_lvs_edt,  ");
            sb.Append("v.apv_lvs_dur, v.apv_dur_ds, v.act_lvs_sdt, v.act_lvs_edt, ");
            sb.Append("v.act_dur_ds, v.lm_rsmptn_dt, v.lm_confm_dt, v.lm_confm_by, ");
            sb.Append("v.hr_rsmptn_dt, v.hr_confm_dt, v.hr_confm_by, v.rqs_cls_dt, ");
            sb.Append("v.is_lm_aprv, v.is_hd_aprv, v.is_hr_aprv, v.is_xm_aprv, ");
            sb.Append("v.is_sm_aprv, v.prp_lvs_dur_typ, v.apv_lvs_dur_typ, v.act_lvs_dur_typ,  ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = v.emp_id) ");
            sb.Append("as emp_nm, (SELECT lvs_typ_nm FROM public.lms_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = v.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = v.unit_id) ");
            sb.Append("as unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= v.dept_id) as dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = v.loc_id) as loc_nm ");
            sb.Append("FROM public.lms_lvs_infs v ");
            sb.Append("WHERE (v.is_pln = @is_pln) ");
            sb.Append("AND DATE_PART('Year', v.prp_lvs_sdt) = @lvs_year ");
            sb.Append("AND DATE_PART('Month', v.prp_lvs_sdt) = @lvs_month ");
            sb.Append("AND v.emp_id IN (SELECT p.id FROM public.gst_prsns p ");
            sb.Append("WHERE p.fullname = @emp_nm)) ");
            sb.Append("ORDER BY v.prp_lvs_sdt; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var emp_nm = cmd.Parameters.Add("@emp_nm", NpgsqlDbType.Text);
                    var lvs_year = cmd.Parameters.Add("@lvs_year", NpgsqlDbType.Integer);
                    var lvs_month = cmd.Parameters.Add("@lvs_month", NpgsqlDbType.Integer);
                    var is_pln = cmd.Parameters.Add("@is_pln", NpgsqlDbType.Boolean);
                    await cmd.PrepareAsync();
                    emp_nm.Value = employeeName;
                    lvs_year.Value = year;
                    lvs_month.Value = month;
                    is_pln.Value = isPlan;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        leaveList.Add(new EmployeeLeave()
                        {
                            Id = reader["lvs_inf_id"] == DBNull.Value ? 0L : (long)reader["lvs_inf_id"],
                            EmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
                            EmployeeFullName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),

                            UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                            UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
                            DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                            DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
                            LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                            LocationName = reader["loc_nm"] == DBNull.Value ? string.Empty : reader["loc_nm"].ToString(),

                            LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"],
                            LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString(),
                            LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
                            LeaveReason = reader["lvs_rsn"] == DBNull.Value ? string.Empty : reader["lvs_rsn"].ToString(),
                            LeaveStatus = reader["lvs_sts"] == DBNull.Value ? string.Empty : reader["lvs_sts"].ToString(),
                            IsPlan = reader["is_pln"] == DBNull.Value ? true : (bool)reader["is_pln"],

                            ProposedLeaveStartDate = reader["prp_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_sdt"],
                            ProposedLeaveEndDate = reader["prp_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_edt"],
                            ProposedLeaveDuration = reader["prp_lvs_dur"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur"],
                            ProposedDurationTypeId = reader["prp_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur_typ"],
                            ProposedDurationDescription = reader["prp_dur_ds"] == DBNull.Value ? string.Empty : reader["prp_dur_ds"].ToString(),

                            ApprovedLeaveStartDate = reader["apv_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_sdt"],
                            ApprovedLeaveEndDate = reader["apv_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_edt"],
                            ApprovedLeaveDuration = reader["apv_lvs_dur"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur"],
                            ApprovedDurationTypeId = reader["apv_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur_typ"],
                            ApprovedDurationDescription = reader["apv_dur_ds"] == DBNull.Value ? string.Empty : reader["apv_dur_ds"].ToString(),

                            ActualLeaveStartDate = reader["act_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_sdt"],
                            ActualLeaveEndDate = reader["act_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_edt"],
                            ActualLeaveDuration = reader["act_lvs_dur"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur"],
                            ActualDurationTypeId = reader["act_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur_typ"],
                            ActualDurationDescription = reader["act_dur_ds"] == DBNull.Value ? string.Empty : reader["act_dur_ds"].ToString(),

                            RequestCloseDate = reader["rqs_cls_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rqs_cls_dt"],

                            LineManagersResumptionDate = reader["lm_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_rsmptn_dt"],
                            LineManagerConfirmResumptionDate = reader["lm_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_confm_dt"],
                            LineManagerConfirmResumptionBy = reader["lm_confm_by"] == DBNull.Value ? string.Empty : reader["lm_confm_by"].ToString(),

                            HrResumptionDate = reader["hr_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_rsmptn_dt"],
                            HrConfirmResumptionDate = reader["hr_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_confm_dt"],
                            HrConfirmResumptionBy = reader["hr_confm_by"] == DBNull.Value ? string.Empty : reader["hr_confm_by"].ToString(),

                            ApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
                            ApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],
                            ApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
                            ApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
                            ApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveList;
        }
        public async Task<List<EmployeeLeave>> GetByEmployeeNamenStatusAsync(string employeeName, string leaveStatus, bool isPlan)
        {
            List<EmployeeLeave> leaveList = new List<EmployeeLeave>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT v.lvs_inf_id, v.emp_id, v.unit_id, v.dept_id, ");
            sb.Append("v.loc_id, v.lvs_yr, v.lvs_typ_cd, v.lvs_rsn, v.lvs_sts, ");
            sb.Append("v.is_pln, v.prp_lvs_sdt, v.prp_lvs_edt, v.prp_lvs_dur, ");
            sb.Append("v.act_lvs_dur, v.prp_dur_ds, v.apv_lvs_sdt, v.apv_lvs_edt,  ");
            sb.Append("v.apv_lvs_dur, v.apv_dur_ds, v.act_lvs_sdt, v.act_lvs_edt, ");
            sb.Append("v.act_dur_ds, v.lm_rsmptn_dt, v.lm_confm_dt, v.lm_confm_by, ");
            sb.Append("v.hr_rsmptn_dt, v.hr_confm_dt, v.hr_confm_by, v.rqs_cls_dt, ");
            sb.Append("v.is_lm_aprv, v.is_hd_aprv, v.is_hr_aprv, v.is_xm_aprv, ");
            sb.Append("v.is_sm_aprv, v.prp_lvs_dur_typ, v.apv_lvs_dur_typ, v.act_lvs_dur_typ,  ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = v.emp_id) ");
            sb.Append("as emp_nm, (SELECT lvs_typ_nm FROM public.lms_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = v.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = v.unit_id) ");
            sb.Append("as unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= v.dept_id) as dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = v.loc_id) as loc_nm ");
            sb.Append("FROM public.lms_lvs_infs v ");
            sb.Append("WHERE (v.is_pln = @is_pln) AND (v.lvs_sts = @lvs_sts) ");
            sb.Append("AND v.emp_id IN (SELECT p.id FROM public.gst_prsns p ");
            sb.Append("WHERE p.fullname = @emp_nm) ");
            sb.Append("ORDER BY v.prp_lvs_sdt; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var emp_nm = cmd.Parameters.Add("@emp_nm", NpgsqlDbType.Text);
                    var is_pln = cmd.Parameters.Add("@is_pln", NpgsqlDbType.Boolean);
                    var lvs_sts = cmd.Parameters.Add("@lvs_sts", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    emp_nm.Value = employeeName;
                    is_pln.Value = isPlan;
                    lvs_sts.Value = leaveStatus;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        leaveList.Add(new EmployeeLeave()
                        {
                            Id = reader["lvs_inf_id"] == DBNull.Value ? 0L : (long)reader["lvs_inf_id"],
                            EmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
                            EmployeeFullName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),

                            UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                            UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
                            DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                            DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
                            LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                            LocationName = reader["loc_nm"] == DBNull.Value ? string.Empty : reader["loc_nm"].ToString(),

                            LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"],
                            LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString(),
                            LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
                            LeaveReason = reader["lvs_rsn"] == DBNull.Value ? string.Empty : reader["lvs_rsn"].ToString(),
                            LeaveStatus = reader["lvs_sts"] == DBNull.Value ? string.Empty : reader["lvs_sts"].ToString(),
                            IsPlan = reader["is_pln"] == DBNull.Value ? true : (bool)reader["is_pln"],

                            ProposedLeaveStartDate = reader["prp_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_sdt"],
                            ProposedLeaveEndDate = reader["prp_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_edt"],
                            ProposedLeaveDuration = reader["prp_lvs_dur"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur"],
                            ProposedDurationTypeId = reader["prp_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur_typ"],
                            ProposedDurationDescription = reader["prp_dur_ds"] == DBNull.Value ? string.Empty : reader["prp_dur_ds"].ToString(),

                            ApprovedLeaveStartDate = reader["apv_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_sdt"],
                            ApprovedLeaveEndDate = reader["apv_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_edt"],
                            ApprovedLeaveDuration = reader["apv_lvs_dur"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur"],
                            ApprovedDurationTypeId = reader["apv_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur_typ"],
                            ApprovedDurationDescription = reader["apv_dur_ds"] == DBNull.Value ? string.Empty : reader["apv_dur_ds"].ToString(),

                            ActualLeaveStartDate = reader["act_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_sdt"],
                            ActualLeaveEndDate = reader["act_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_edt"],
                            ActualLeaveDuration = reader["act_lvs_dur"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur"],
                            ActualDurationTypeId = reader["act_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur_typ"],
                            ActualDurationDescription = reader["act_dur_ds"] == DBNull.Value ? string.Empty : reader["act_dur_ds"].ToString(),

                            RequestCloseDate = reader["rqs_cls_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rqs_cls_dt"],

                            LineManagersResumptionDate = reader["lm_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_rsmptn_dt"],
                            LineManagerConfirmResumptionDate = reader["lm_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_confm_dt"],
                            LineManagerConfirmResumptionBy = reader["lm_confm_by"] == DBNull.Value ? string.Empty : reader["lm_confm_by"].ToString(),

                            HrResumptionDate = reader["hr_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_rsmptn_dt"],
                            HrConfirmResumptionDate = reader["hr_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_confm_dt"],
                            HrConfirmResumptionBy = reader["hr_confm_by"] == DBNull.Value ? string.Empty : reader["hr_confm_by"].ToString(),

                            ApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
                            ApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],
                            ApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
                            ApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
                            ApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveList;
        }
        public async Task<List<EmployeeLeave>> GetByEmployeeNameAsync(string employeeName, bool isPlan)
        {
            List<EmployeeLeave> leaveList = new List<EmployeeLeave>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT v.lvs_inf_id, v.emp_id, v.unit_id, v.dept_id, ");
            sb.Append("v.loc_id, v.lvs_yr, v.lvs_typ_cd, v.lvs_rsn, v.lvs_sts, ");
            sb.Append("v.is_pln, v.prp_lvs_sdt, v.prp_lvs_edt, v.prp_lvs_dur, ");
            sb.Append("v.act_lvs_dur, v.prp_dur_ds, v.apv_lvs_sdt, v.apv_lvs_edt,  ");
            sb.Append("v.apv_lvs_dur, v.apv_dur_ds, v.act_lvs_sdt, v.act_lvs_edt, ");
            sb.Append("v.act_dur_ds, v.lm_rsmptn_dt, v.lm_confm_dt, v.lm_confm_by, ");
            sb.Append("v.hr_rsmptn_dt, v.hr_confm_dt, v.hr_confm_by, v.rqs_cls_dt, ");
            sb.Append("v.is_lm_aprv, v.is_hd_aprv, v.is_hr_aprv, v.is_xm_aprv, ");
            sb.Append("v.is_sm_aprv, v.prp_lvs_dur_typ, v.apv_lvs_dur_typ, v.act_lvs_dur_typ,  ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = v.emp_id) ");
            sb.Append("as emp_nm, (SELECT lvs_typ_nm FROM public.lms_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = v.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = v.unit_id) ");
            sb.Append("as unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= v.dept_id) as dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = v.loc_id) as loc_nm ");
            sb.Append("FROM public.lms_lvs_infs v ");
            sb.Append("WHERE (v.is_pln = @is_pln) ");
            sb.Append("AND v.emp_id IN (SELECT p.id FROM public.gst_prsns p ");
            sb.Append("WHERE p.fullname = @emp_nm) ");
            sb.Append("ORDER BY v.prp_lvs_sdt; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var emp_nm = cmd.Parameters.Add("@emp_nm", NpgsqlDbType.Text);
                    var is_pln = cmd.Parameters.Add("@is_pln", NpgsqlDbType.Boolean);
                    await cmd.PrepareAsync();
                    emp_nm.Value = employeeName;
                    is_pln.Value = isPlan;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        leaveList.Add(new EmployeeLeave()
                        {
                            Id = reader["lvs_inf_id"] == DBNull.Value ? 0L : (long)reader["lvs_inf_id"],
                            EmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
                            EmployeeFullName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),

                            UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                            UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
                            DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                            DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
                            LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                            LocationName = reader["loc_nm"] == DBNull.Value ? string.Empty : reader["loc_nm"].ToString(),

                            LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"],
                            LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString(),
                            LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
                            LeaveReason = reader["lvs_rsn"] == DBNull.Value ? string.Empty : reader["lvs_rsn"].ToString(),
                            LeaveStatus = reader["lvs_sts"] == DBNull.Value ? string.Empty : reader["lvs_sts"].ToString(),
                            IsPlan = reader["is_pln"] == DBNull.Value ? true : (bool)reader["is_pln"],

                            ProposedLeaveStartDate = reader["prp_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_sdt"],
                            ProposedLeaveEndDate = reader["prp_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_edt"],
                            ProposedLeaveDuration = reader["prp_lvs_dur"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur"],
                            ProposedDurationTypeId = reader["prp_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur_typ"],
                            ProposedDurationDescription = reader["prp_dur_ds"] == DBNull.Value ? string.Empty : reader["prp_dur_ds"].ToString(),

                            ApprovedLeaveStartDate = reader["apv_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_sdt"],
                            ApprovedLeaveEndDate = reader["apv_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_edt"],
                            ApprovedLeaveDuration = reader["apv_lvs_dur"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur"],
                            ApprovedDurationTypeId = reader["apv_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur_typ"],
                            ApprovedDurationDescription = reader["apv_dur_ds"] == DBNull.Value ? string.Empty : reader["apv_dur_ds"].ToString(),

                            ActualLeaveStartDate = reader["act_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_sdt"],
                            ActualLeaveEndDate = reader["act_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_edt"],
                            ActualLeaveDuration = reader["act_lvs_dur"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur"],
                            ActualDurationTypeId = reader["act_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur_typ"],
                            ActualDurationDescription = reader["act_dur_ds"] == DBNull.Value ? string.Empty : reader["act_dur_ds"].ToString(),

                            RequestCloseDate = reader["rqs_cls_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rqs_cls_dt"],

                            LineManagersResumptionDate = reader["lm_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_rsmptn_dt"],
                            LineManagerConfirmResumptionDate = reader["lm_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_confm_dt"],
                            LineManagerConfirmResumptionBy = reader["lm_confm_by"] == DBNull.Value ? string.Empty : reader["lm_confm_by"].ToString(),

                            HrResumptionDate = reader["hr_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_rsmptn_dt"],
                            HrConfirmResumptionDate = reader["hr_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_confm_dt"],
                            HrConfirmResumptionBy = reader["hr_confm_by"] == DBNull.Value ? string.Empty : reader["hr_confm_by"].ToString(),

                            ApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
                            ApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],
                            ApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
                            ApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
                            ApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveList;
        }
        #endregion

        #region Employee Leaves for Reports & Team Members
        //===== Employee Leaves for Reports & Team Members =========//
        public async Task<List<EmployeeLeave>> GetByReportingLineIdAsync(string teamLeadId, bool isPlan)
        {
            List<EmployeeLeave> leaveList = new List<EmployeeLeave>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT v.lvs_inf_id, v.emp_id, v.unit_id, v.dept_id, ");
            sb.Append("v.loc_id, v.lvs_yr, v.lvs_typ_cd, v.lvs_rsn, v.lvs_sts, ");
            sb.Append("v.is_pln, v.prp_lvs_sdt, v.prp_lvs_edt, v.prp_lvs_dur, ");
            sb.Append("v.act_lvs_dur, v.prp_dur_ds, v.apv_lvs_sdt, v.apv_lvs_edt,  ");
            sb.Append("v.apv_lvs_dur, v.apv_dur_ds, v.act_lvs_sdt, v.act_lvs_edt, ");
            sb.Append("v.act_dur_ds, v.lm_rsmptn_dt, v.lm_confm_dt, v.lm_confm_by, ");
            sb.Append("v.hr_rsmptn_dt, v.hr_confm_dt, v.hr_confm_by, v.rqs_cls_dt, ");
            sb.Append("v.is_lm_aprv, v.is_hd_aprv, v.is_hr_aprv, v.is_xm_aprv, ");
            sb.Append("v.is_sm_aprv, v.prp_lvs_dur_typ, v.apv_lvs_dur_typ, v.act_lvs_dur_typ,  ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = v.emp_id) ");
            sb.Append("as emp_nm, (SELECT lvs_typ_nm FROM public.lms_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = v.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = v.unit_id) ");
            sb.Append("as unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= v.dept_id) as dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = v.loc_id) as loc_nm ");
            sb.Append("FROM public.lms_lvs_infs v ");
            sb.Append("WHERE (v.is_pln = @is_pln) ");
            sb.Append("AND v.emp_id IN (SELECT r.emp_id FROM public.erm_emp_rpts r  ");
            sb.Append("WHERE r.rpt_emp_id = @rpt_emp_id) ");
            sb.Append("ORDER BY v.prp_lvs_sdt; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var rpt_emp_id = cmd.Parameters.Add("@rpt_emp_id", NpgsqlDbType.Text);
                    var is_pln = cmd.Parameters.Add("@is_pln", NpgsqlDbType.Boolean);
                    await cmd.PrepareAsync();
                    rpt_emp_id.Value = teamLeadId;
                    is_pln.Value = isPlan;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        leaveList.Add(new EmployeeLeave()
                        {
                            Id = reader["lvs_inf_id"] == DBNull.Value ? 0L : (long)reader["lvs_inf_id"],
                            EmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
                            EmployeeFullName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),

                            UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                            UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
                            DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                            DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
                            LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                            LocationName = reader["loc_nm"] == DBNull.Value ? string.Empty : reader["loc_nm"].ToString(),

                            LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"],
                            LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString(),
                            LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
                            LeaveReason = reader["lvs_rsn"] == DBNull.Value ? string.Empty : reader["lvs_rsn"].ToString(),
                            LeaveStatus = reader["lvs_sts"] == DBNull.Value ? string.Empty : reader["lvs_sts"].ToString(),
                            IsPlan = reader["is_pln"] == DBNull.Value ? true : (bool)reader["is_pln"],

                            ProposedLeaveStartDate = reader["prp_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_sdt"],
                            ProposedLeaveEndDate = reader["prp_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_edt"],
                            ProposedLeaveDuration = reader["prp_lvs_dur"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur"],
                            ProposedDurationTypeId = reader["prp_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur_typ"],
                            ProposedDurationDescription = reader["prp_dur_ds"] == DBNull.Value ? string.Empty : reader["prp_dur_ds"].ToString(),

                            ApprovedLeaveStartDate = reader["apv_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_sdt"],
                            ApprovedLeaveEndDate = reader["apv_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_edt"],
                            ApprovedLeaveDuration = reader["apv_lvs_dur"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur"],
                            ApprovedDurationTypeId = reader["apv_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur_typ"],
                            ApprovedDurationDescription = reader["apv_dur_ds"] == DBNull.Value ? string.Empty : reader["apv_dur_ds"].ToString(),

                            ActualLeaveStartDate = reader["act_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_sdt"],
                            ActualLeaveEndDate = reader["act_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_edt"],
                            ActualLeaveDuration = reader["act_lvs_dur"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur"],
                            ActualDurationTypeId = reader["act_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur_typ"],
                            ActualDurationDescription = reader["act_dur_ds"] == DBNull.Value ? string.Empty : reader["act_dur_ds"].ToString(),

                            RequestCloseDate = reader["rqs_cls_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rqs_cls_dt"],

                            LineManagersResumptionDate = reader["lm_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_rsmptn_dt"],
                            LineManagerConfirmResumptionDate = reader["lm_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_confm_dt"],
                            LineManagerConfirmResumptionBy = reader["lm_confm_by"] == DBNull.Value ? string.Empty : reader["lm_confm_by"].ToString(),

                            HrResumptionDate = reader["hr_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_rsmptn_dt"],
                            HrConfirmResumptionDate = reader["hr_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_confm_dt"],
                            HrConfirmResumptionBy = reader["hr_confm_by"] == DBNull.Value ? string.Empty : reader["hr_confm_by"].ToString(),

                            ApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
                            ApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],
                            ApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
                            ApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
                            ApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveList;
        }
        public async Task<List<EmployeeLeave>> GetByReportingLineIdnYearAsync(string teamLeadId, int year, bool isPlan)
        {
            List<EmployeeLeave> leaveList = new List<EmployeeLeave>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT v.lvs_inf_id, v.emp_id, v.unit_id, v.dept_id, ");
            sb.Append("v.loc_id, v.lvs_yr, v.lvs_typ_cd, v.lvs_rsn, v.lvs_sts, ");
            sb.Append("v.is_pln, v.prp_lvs_sdt, v.prp_lvs_edt, v.prp_lvs_dur, ");
            sb.Append("v.act_lvs_dur, v.prp_dur_ds, v.apv_lvs_sdt, v.apv_lvs_edt,  ");
            sb.Append("v.apv_lvs_dur, v.apv_dur_ds, v.act_lvs_sdt, v.act_lvs_edt, ");
            sb.Append("v.act_dur_ds, v.lm_rsmptn_dt, v.lm_confm_dt, v.lm_confm_by, ");
            sb.Append("v.hr_rsmptn_dt, v.hr_confm_dt, v.hr_confm_by, v.rqs_cls_dt, ");
            sb.Append("v.is_lm_aprv, v.is_hd_aprv, v.is_hr_aprv, v.is_xm_aprv, ");
            sb.Append("v.is_sm_aprv, v.prp_lvs_dur_typ, v.apv_lvs_dur_typ, v.act_lvs_dur_typ,  ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = v.emp_id) ");
            sb.Append("as emp_nm, (SELECT lvs_typ_nm FROM public.lms_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = v.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = v.unit_id) ");
            sb.Append("as unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= v.dept_id) as dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = v.loc_id) as loc_nm ");
            sb.Append("FROM public.lms_lvs_infs v ");
            sb.Append("WHERE v.lvs_yr = @lvs_yr AND v.is_pln = @is_pln ");
            sb.Append("AND v.emp_id IN (SELECT r.emp_id FROM public.erm_emp_rpts r  ");
            sb.Append("WHERE r.rpt_emp_id = @rpt_emp_id) ");
            sb.Append("ORDER BY v.prp_lvs_sdt; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var rpt_emp_id = cmd.Parameters.Add("@rpt_emp_id", NpgsqlDbType.Text);
                    var lvs_yr = cmd.Parameters.Add("@lvs_yr", NpgsqlDbType.Integer);
                    var is_pln = cmd.Parameters.Add("@is_pln", NpgsqlDbType.Boolean);
                    await cmd.PrepareAsync();
                    rpt_emp_id.Value = teamLeadId;
                    lvs_yr.Value = year;
                    is_pln.Value = isPlan;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        leaveList.Add(new EmployeeLeave()
                        {
                            Id = reader["lvs_inf_id"] == DBNull.Value ? 0L : (long)reader["lvs_inf_id"],
                            EmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
                            EmployeeFullName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),

                            UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                            UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
                            DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                            DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
                            LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                            LocationName = reader["loc_nm"] == DBNull.Value ? string.Empty : reader["loc_nm"].ToString(),

                            LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"],
                            LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString(),
                            LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
                            LeaveReason = reader["lvs_rsn"] == DBNull.Value ? string.Empty : reader["lvs_rsn"].ToString(),
                            LeaveStatus = reader["lvs_sts"] == DBNull.Value ? string.Empty : reader["lvs_sts"].ToString(),
                            IsPlan = reader["is_pln"] == DBNull.Value ? true : (bool)reader["is_pln"],

                            ProposedLeaveStartDate = reader["prp_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_sdt"],
                            ProposedLeaveEndDate = reader["prp_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_edt"],
                            ProposedLeaveDuration = reader["prp_lvs_dur"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur"],
                            ProposedDurationTypeId = reader["prp_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur_typ"],
                            ProposedDurationDescription = reader["prp_dur_ds"] == DBNull.Value ? string.Empty : reader["prp_dur_ds"].ToString(),

                            ApprovedLeaveStartDate = reader["apv_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_sdt"],
                            ApprovedLeaveEndDate = reader["apv_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_edt"],
                            ApprovedLeaveDuration = reader["apv_lvs_dur"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur"],
                            ApprovedDurationTypeId = reader["apv_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur_typ"],
                            ApprovedDurationDescription = reader["apv_dur_ds"] == DBNull.Value ? string.Empty : reader["apv_dur_ds"].ToString(),

                            ActualLeaveStartDate = reader["act_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_sdt"],
                            ActualLeaveEndDate = reader["act_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_edt"],
                            ActualLeaveDuration = reader["act_lvs_dur"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur"],
                            ActualDurationTypeId = reader["act_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur_typ"],
                            ActualDurationDescription = reader["act_dur_ds"] == DBNull.Value ? string.Empty : reader["act_dur_ds"].ToString(),

                            RequestCloseDate = reader["rqs_cls_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rqs_cls_dt"],

                            LineManagersResumptionDate = reader["lm_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_rsmptn_dt"],
                            LineManagerConfirmResumptionDate = reader["lm_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_confm_dt"],
                            LineManagerConfirmResumptionBy = reader["lm_confm_by"] == DBNull.Value ? string.Empty : reader["lm_confm_by"].ToString(),

                            HrResumptionDate = reader["hr_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_rsmptn_dt"],
                            HrConfirmResumptionDate = reader["hr_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_confm_dt"],
                            HrConfirmResumptionBy = reader["hr_confm_by"] == DBNull.Value ? string.Empty : reader["hr_confm_by"].ToString(),

                            ApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
                            ApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],
                            ApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
                            ApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
                            ApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveList;
        }
        public async Task<List<EmployeeLeave>> GetByReportingLineIdnYearnStatusAsync(string teamLeadId, int year, string leaveStatus, bool isPlan)
        {
            List<EmployeeLeave> leaveList = new List<EmployeeLeave>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT v.lvs_inf_id, v.emp_id, v.unit_id, v.dept_id, ");
            sb.Append("v.loc_id, v.lvs_yr, v.lvs_typ_cd, v.lvs_rsn, v.lvs_sts, ");
            sb.Append("v.is_pln, v.prp_lvs_sdt, v.prp_lvs_edt, v.prp_lvs_dur, ");
            sb.Append("v.act_lvs_dur, v.prp_dur_ds, v.apv_lvs_sdt, v.apv_lvs_edt,  ");
            sb.Append("v.apv_lvs_dur, v.apv_dur_ds, v.act_lvs_sdt, v.act_lvs_edt, ");
            sb.Append("v.act_dur_ds, v.lm_rsmptn_dt, v.lm_confm_dt, v.lm_confm_by, ");
            sb.Append("v.hr_rsmptn_dt, v.hr_confm_dt, v.hr_confm_by, v.rqs_cls_dt, ");
            sb.Append("v.is_lm_aprv, v.is_hd_aprv, v.is_hr_aprv, v.is_xm_aprv, ");
            sb.Append("v.is_sm_aprv, v.prp_lvs_dur_typ, v.apv_lvs_dur_typ, v.act_lvs_dur_typ,  ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = v.emp_id) ");
            sb.Append("as emp_nm, (SELECT lvs_typ_nm FROM public.lms_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = v.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = v.unit_id) ");
            sb.Append("as unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= v.dept_id) as dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = v.loc_id) as loc_nm ");
            sb.Append("FROM public.lms_lvs_infs v ");
            sb.Append("WHERE v.lvs_yr = @lvs_yr AND v.is_pln = @is_pln ");
            sb.Append("AND v.lvs_sts = @lvs_sts ");
            sb.Append("AND v.emp_id IN (SELECT r.emp_id FROM public.erm_emp_rpts r  ");
            sb.Append("WHERE r.rpt_emp_id = @rpt_emp_id) ");
            sb.Append("ORDER BY v.prp_lvs_sdt; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var rpt_emp_id = cmd.Parameters.Add("@rpt_emp_id", NpgsqlDbType.Text);
                    var lvs_yr = cmd.Parameters.Add("@lvs_yr", NpgsqlDbType.Integer);
                    var lvs_sts = cmd.Parameters.Add("@lvs_sts", NpgsqlDbType.Text);
                    var is_pln = cmd.Parameters.Add("@is_pln", NpgsqlDbType.Boolean);
                    await cmd.PrepareAsync();
                    rpt_emp_id.Value = teamLeadId;
                    lvs_yr.Value = year;
                    lvs_sts.Value = leaveStatus;
                    is_pln.Value = isPlan;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        leaveList.Add(new EmployeeLeave()
                        {
                            Id = reader["lvs_inf_id"] == DBNull.Value ? 0L : (long)reader["lvs_inf_id"],
                            EmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
                            EmployeeFullName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),

                            UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                            UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
                            DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                            DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
                            LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                            LocationName = reader["loc_nm"] == DBNull.Value ? string.Empty : reader["loc_nm"].ToString(),

                            LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"],
                            LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString(),
                            LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
                            LeaveReason = reader["lvs_rsn"] == DBNull.Value ? string.Empty : reader["lvs_rsn"].ToString(),
                            LeaveStatus = reader["lvs_sts"] == DBNull.Value ? string.Empty : reader["lvs_sts"].ToString(),
                            IsPlan = reader["is_pln"] == DBNull.Value ? true : (bool)reader["is_pln"],

                            ProposedLeaveStartDate = reader["prp_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_sdt"],
                            ProposedLeaveEndDate = reader["prp_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_edt"],
                            ProposedLeaveDuration = reader["prp_lvs_dur"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur"],
                            ProposedDurationTypeId = reader["prp_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur_typ"],
                            ProposedDurationDescription = reader["prp_dur_ds"] == DBNull.Value ? string.Empty : reader["prp_dur_ds"].ToString(),

                            ApprovedLeaveStartDate = reader["apv_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_sdt"],
                            ApprovedLeaveEndDate = reader["apv_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_edt"],
                            ApprovedLeaveDuration = reader["apv_lvs_dur"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur"],
                            ApprovedDurationTypeId = reader["apv_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur_typ"],
                            ApprovedDurationDescription = reader["apv_dur_ds"] == DBNull.Value ? string.Empty : reader["apv_dur_ds"].ToString(),

                            ActualLeaveStartDate = reader["act_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_sdt"],
                            ActualLeaveEndDate = reader["act_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_edt"],
                            ActualLeaveDuration = reader["act_lvs_dur"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur"],
                            ActualDurationTypeId = reader["act_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur_typ"],
                            ActualDurationDescription = reader["act_dur_ds"] == DBNull.Value ? string.Empty : reader["act_dur_ds"].ToString(),

                            RequestCloseDate = reader["rqs_cls_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rqs_cls_dt"],

                            LineManagersResumptionDate = reader["lm_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_rsmptn_dt"],
                            LineManagerConfirmResumptionDate = reader["lm_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_confm_dt"],
                            LineManagerConfirmResumptionBy = reader["lm_confm_by"] == DBNull.Value ? string.Empty : reader["lm_confm_by"].ToString(),

                            HrResumptionDate = reader["hr_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_rsmptn_dt"],
                            HrConfirmResumptionDate = reader["hr_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_confm_dt"],
                            HrConfirmResumptionBy = reader["hr_confm_by"] == DBNull.Value ? string.Empty : reader["hr_confm_by"].ToString(),

                            ApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
                            ApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],
                            ApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
                            ApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
                            ApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveList;
        }
        public async Task<List<EmployeeLeave>> GetByReportingLineIdnYearnMonthAsync(string teamLeadId, int year, int month, bool isPlan)
        {
            List<EmployeeLeave> leaveList = new List<EmployeeLeave>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT v.lvs_inf_id, v.emp_id, v.unit_id, v.dept_id, ");
            sb.Append("v.loc_id, v.lvs_yr, v.lvs_typ_cd, v.lvs_rsn, v.lvs_sts, ");
            sb.Append("v.is_pln, v.prp_lvs_sdt, v.prp_lvs_edt, v.prp_lvs_dur, ");
            sb.Append("v.act_lvs_dur, v.prp_dur_ds, v.apv_lvs_sdt, v.apv_lvs_edt,  ");
            sb.Append("v.apv_lvs_dur, v.apv_dur_ds, v.act_lvs_sdt, v.act_lvs_edt, ");
            sb.Append("v.act_dur_ds, v.lm_rsmptn_dt, v.lm_confm_dt, v.lm_confm_by, ");
            sb.Append("v.hr_rsmptn_dt, v.hr_confm_dt, v.hr_confm_by, v.rqs_cls_dt, ");
            sb.Append("v.is_lm_aprv, v.is_hd_aprv, v.is_hr_aprv, v.is_xm_aprv, ");
            sb.Append("v.is_sm_aprv, v.prp_lvs_dur_typ, v.apv_lvs_dur_typ, v.act_lvs_dur_typ,  ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = v.emp_id) ");
            sb.Append("as emp_nm, (SELECT lvs_typ_nm FROM public.lms_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = v.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = v.unit_id) ");
            sb.Append("as unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= v.dept_id) as dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = v.loc_id) as loc_nm ");
            sb.Append("FROM public.lms_lvs_infs v ");
            sb.Append("WHERE v.is_pln = @is_pln ");
            sb.Append("AND DATE_PART('Year', v.prp_lvs_sdt) = @lvs_yr ");
            sb.Append("AND DATE_PART('Month', v.prp_lvs_sdt) = @lvs_month ");
            sb.Append("AND v.emp_id IN (SELECT r.emp_id FROM public.erm_emp_rpts r  ");
            sb.Append("WHERE r.rpt_emp_id = @rpt_emp_id) ");
            sb.Append("ORDER BY v.prp_lvs_sdt; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var rpt_emp_id = cmd.Parameters.Add("@rpt_emp_id", NpgsqlDbType.Text);
                    var lvs_yr = cmd.Parameters.Add("@lvs_yr", NpgsqlDbType.Integer);
                    var lvs_month = cmd.Parameters.Add("@lvs_month", NpgsqlDbType.Integer);
                    var is_pln = cmd.Parameters.Add("@is_pln", NpgsqlDbType.Boolean);
                    await cmd.PrepareAsync();
                    rpt_emp_id.Value = teamLeadId;
                    lvs_yr.Value = year;
                    lvs_month.Value = month;
                    is_pln.Value = isPlan;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        leaveList.Add(new EmployeeLeave()
                        {
                            Id = reader["lvs_inf_id"] == DBNull.Value ? 0L : (long)reader["lvs_inf_id"],
                            EmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
                            EmployeeFullName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),

                            UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                            UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
                            DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                            DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
                            LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                            LocationName = reader["loc_nm"] == DBNull.Value ? string.Empty : reader["loc_nm"].ToString(),

                            LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"],
                            LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString(),
                            LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
                            LeaveReason = reader["lvs_rsn"] == DBNull.Value ? string.Empty : reader["lvs_rsn"].ToString(),
                            LeaveStatus = reader["lvs_sts"] == DBNull.Value ? string.Empty : reader["lvs_sts"].ToString(),
                            IsPlan = reader["is_pln"] == DBNull.Value ? true : (bool)reader["is_pln"],

                            ProposedLeaveStartDate = reader["prp_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_sdt"],
                            ProposedLeaveEndDate = reader["prp_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_edt"],
                            ProposedLeaveDuration = reader["prp_lvs_dur"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur"],
                            ProposedDurationTypeId = reader["prp_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur_typ"],
                            ProposedDurationDescription = reader["prp_dur_ds"] == DBNull.Value ? string.Empty : reader["prp_dur_ds"].ToString(),

                            ApprovedLeaveStartDate = reader["apv_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_sdt"],
                            ApprovedLeaveEndDate = reader["apv_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_edt"],
                            ApprovedLeaveDuration = reader["apv_lvs_dur"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur"],
                            ApprovedDurationTypeId = reader["apv_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur_typ"],
                            ApprovedDurationDescription = reader["apv_dur_ds"] == DBNull.Value ? string.Empty : reader["apv_dur_ds"].ToString(),

                            ActualLeaveStartDate = reader["act_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_sdt"],
                            ActualLeaveEndDate = reader["act_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_edt"],
                            ActualLeaveDuration = reader["act_lvs_dur"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur"],
                            ActualDurationTypeId = reader["act_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur_typ"],
                            ActualDurationDescription = reader["act_dur_ds"] == DBNull.Value ? string.Empty : reader["act_dur_ds"].ToString(),

                            RequestCloseDate = reader["rqs_cls_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rqs_cls_dt"],

                            LineManagersResumptionDate = reader["lm_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_rsmptn_dt"],
                            LineManagerConfirmResumptionDate = reader["lm_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_confm_dt"],
                            LineManagerConfirmResumptionBy = reader["lm_confm_by"] == DBNull.Value ? string.Empty : reader["lm_confm_by"].ToString(),

                            HrResumptionDate = reader["hr_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_rsmptn_dt"],
                            HrConfirmResumptionDate = reader["hr_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_confm_dt"],
                            HrConfirmResumptionBy = reader["hr_confm_by"] == DBNull.Value ? string.Empty : reader["hr_confm_by"].ToString(),

                            ApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
                            ApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],
                            ApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
                            ApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
                            ApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveList;
        }
        public async Task<List<EmployeeLeave>> GetByReportingLineIdnYearnMonthnStatusAsync(string teamLeadId, int year, int month, string leaveStatus, bool isPlan)
        {
            List<EmployeeLeave> leaveList = new List<EmployeeLeave>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT v.lvs_inf_id, v.emp_id, v.unit_id, v.dept_id, ");
            sb.Append("v.loc_id, v.lvs_yr, v.lvs_typ_cd, v.lvs_rsn, v.lvs_sts, ");
            sb.Append("v.is_pln, v.prp_lvs_sdt, v.prp_lvs_edt, v.prp_lvs_dur, ");
            sb.Append("v.act_lvs_dur, v.prp_dur_ds, v.apv_lvs_sdt, v.apv_lvs_edt,  ");
            sb.Append("v.apv_lvs_dur, v.apv_dur_ds, v.act_lvs_sdt, v.act_lvs_edt, ");
            sb.Append("v.act_dur_ds, v.lm_rsmptn_dt, v.lm_confm_dt, v.lm_confm_by, ");
            sb.Append("v.hr_rsmptn_dt, v.hr_confm_dt, v.hr_confm_by, v.rqs_cls_dt, ");
            sb.Append("v.is_lm_aprv, v.is_hd_aprv, v.is_hr_aprv, v.is_xm_aprv, ");
            sb.Append("v.is_sm_aprv, v.prp_lvs_dur_typ, v.apv_lvs_dur_typ, v.act_lvs_dur_typ,  ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = v.emp_id) ");
            sb.Append("as emp_nm, (SELECT lvs_typ_nm FROM public.lms_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = v.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = v.unit_id) ");
            sb.Append("as unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= v.dept_id) as dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = v.loc_id) as loc_nm ");
            sb.Append("FROM public.lms_lvs_infs v ");
            sb.Append("WHERE v.is_pln = @is_pln AND v.lvs_sts = @lvs_sts ");
            sb.Append("AND DATE_PART('Year', v.prp_lvs_sdt) = @lvs_yr ");
            sb.Append("AND DATE_PART('Month', v.prp_lvs_sdt) = @lvs_month ");
            sb.Append("AND v.emp_id IN (SELECT r.emp_id FROM public.erm_emp_rpts r  ");
            sb.Append("WHERE r.rpt_emp_id = @rpt_emp_id) ");
            sb.Append("ORDER BY v.prp_lvs_sdt; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var rpt_emp_id = cmd.Parameters.Add("@rpt_emp_id", NpgsqlDbType.Text);
                    var lvs_yr = cmd.Parameters.Add("@lvs_yr", NpgsqlDbType.Integer);
                    var lvs_month = cmd.Parameters.Add("@lvs_month", NpgsqlDbType.Integer);
                    var lvs_sts = cmd.Parameters.Add("@lvs_sts", NpgsqlDbType.Text);
                    var is_pln = cmd.Parameters.Add("@is_pln", NpgsqlDbType.Boolean);
                    await cmd.PrepareAsync();
                    rpt_emp_id.Value = teamLeadId;
                    lvs_yr.Value = year;
                    lvs_month.Value = month;
                    lvs_sts.Value = leaveStatus;
                    is_pln.Value = isPlan;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        leaveList.Add(new EmployeeLeave()
                        {
                            Id = reader["lvs_inf_id"] == DBNull.Value ? 0L : (long)reader["lvs_inf_id"],
                            EmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
                            EmployeeFullName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),

                            UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                            UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
                            DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                            DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
                            LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                            LocationName = reader["loc_nm"] == DBNull.Value ? string.Empty : reader["loc_nm"].ToString(),

                            LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"],
                            LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString(),
                            LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
                            LeaveReason = reader["lvs_rsn"] == DBNull.Value ? string.Empty : reader["lvs_rsn"].ToString(),
                            LeaveStatus = reader["lvs_sts"] == DBNull.Value ? string.Empty : reader["lvs_sts"].ToString(),
                            IsPlan = reader["is_pln"] == DBNull.Value ? true : (bool)reader["is_pln"],

                            ProposedLeaveStartDate = reader["prp_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_sdt"],
                            ProposedLeaveEndDate = reader["prp_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_edt"],
                            ProposedLeaveDuration = reader["prp_lvs_dur"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur"],
                            ProposedDurationTypeId = reader["prp_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur_typ"],
                            ProposedDurationDescription = reader["prp_dur_ds"] == DBNull.Value ? string.Empty : reader["prp_dur_ds"].ToString(),

                            ApprovedLeaveStartDate = reader["apv_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_sdt"],
                            ApprovedLeaveEndDate = reader["apv_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_edt"],
                            ApprovedLeaveDuration = reader["apv_lvs_dur"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur"],
                            ApprovedDurationTypeId = reader["apv_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur_typ"],
                            ApprovedDurationDescription = reader["apv_dur_ds"] == DBNull.Value ? string.Empty : reader["apv_dur_ds"].ToString(),

                            ActualLeaveStartDate = reader["act_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_sdt"],
                            ActualLeaveEndDate = reader["act_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_edt"],
                            ActualLeaveDuration = reader["act_lvs_dur"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur"],
                            ActualDurationTypeId = reader["act_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur_typ"],
                            ActualDurationDescription = reader["act_dur_ds"] == DBNull.Value ? string.Empty : reader["act_dur_ds"].ToString(),

                            RequestCloseDate = reader["rqs_cls_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rqs_cls_dt"],

                            LineManagersResumptionDate = reader["lm_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_rsmptn_dt"],
                            LineManagerConfirmResumptionDate = reader["lm_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_confm_dt"],
                            LineManagerConfirmResumptionBy = reader["lm_confm_by"] == DBNull.Value ? string.Empty : reader["lm_confm_by"].ToString(),

                            HrResumptionDate = reader["hr_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_rsmptn_dt"],
                            HrConfirmResumptionDate = reader["hr_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_confm_dt"],
                            HrConfirmResumptionBy = reader["hr_confm_by"] == DBNull.Value ? string.Empty : reader["hr_confm_by"].ToString(),

                            ApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
                            ApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],
                            ApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
                            ApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
                            ApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveList;
        }
        public async Task<List<EmployeeLeave>> GetByReportingLineIdnStatusAsync(string teamLeadId, string leaveStatus, bool isPlan)
        {
            List<EmployeeLeave> leaveList = new List<EmployeeLeave>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT v.lvs_inf_id, v.emp_id, v.unit_id, v.dept_id, ");
            sb.Append("v.loc_id, v.lvs_yr, v.lvs_typ_cd, v.lvs_rsn, v.lvs_sts, ");
            sb.Append("v.is_pln, v.prp_lvs_sdt, v.prp_lvs_edt, v.prp_lvs_dur, ");
            sb.Append("v.act_lvs_dur, v.prp_dur_ds, v.apv_lvs_sdt, v.apv_lvs_edt,  ");
            sb.Append("v.apv_lvs_dur, v.apv_dur_ds, v.act_lvs_sdt, v.act_lvs_edt, ");
            sb.Append("v.act_dur_ds, v.lm_rsmptn_dt, v.lm_confm_dt, v.lm_confm_by, ");
            sb.Append("v.hr_rsmptn_dt, v.hr_confm_dt, v.hr_confm_by, v.rqs_cls_dt, ");
            sb.Append("v.is_lm_aprv, v.is_hd_aprv, v.is_hr_aprv, v.is_xm_aprv, ");
            sb.Append("v.is_sm_aprv, v.prp_lvs_dur_typ, v.apv_lvs_dur_typ, v.act_lvs_dur_typ,  ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = v.emp_id) ");
            sb.Append("as emp_nm, (SELECT lvs_typ_nm FROM public.lms_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = v.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = v.unit_id) ");
            sb.Append("as unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= v.dept_id) as dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = v.loc_id) as loc_nm ");
            sb.Append("FROM public.lms_lvs_infs v ");
            sb.Append("WHERE v.is_pln = @is_pln AND v.lvs_sts = @lvs_sts ");
            sb.Append("AND v.emp_id IN (SELECT r.emp_id FROM public.erm_emp_rpts r  ");
            sb.Append("WHERE r.rpt_emp_id = @rpt_emp_id) ");
            sb.Append("ORDER BY v.prp_lvs_sdt; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var rpt_emp_id = cmd.Parameters.Add("@rpt_emp_id", NpgsqlDbType.Text);
                    var lvs_sts = cmd.Parameters.Add("@lvs_sts", NpgsqlDbType.Text);
                    var is_pln = cmd.Parameters.Add("@is_pln", NpgsqlDbType.Boolean);
                    await cmd.PrepareAsync();
                    rpt_emp_id.Value = teamLeadId;
                    lvs_sts.Value = leaveStatus;
                    is_pln.Value = isPlan;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        leaveList.Add(new EmployeeLeave()
                        {
                            Id = reader["lvs_inf_id"] == DBNull.Value ? 0L : (long)reader["lvs_inf_id"],
                            EmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
                            EmployeeFullName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),

                            UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                            UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
                            DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                            DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
                            LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                            LocationName = reader["loc_nm"] == DBNull.Value ? string.Empty : reader["loc_nm"].ToString(),

                            LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"],
                            LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString(),
                            LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
                            LeaveReason = reader["lvs_rsn"] == DBNull.Value ? string.Empty : reader["lvs_rsn"].ToString(),
                            LeaveStatus = reader["lvs_sts"] == DBNull.Value ? string.Empty : reader["lvs_sts"].ToString(),
                            IsPlan = reader["is_pln"] == DBNull.Value ? true : (bool)reader["is_pln"],

                            ProposedLeaveStartDate = reader["prp_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_sdt"],
                            ProposedLeaveEndDate = reader["prp_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_edt"],
                            ProposedLeaveDuration = reader["prp_lvs_dur"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur"],
                            ProposedDurationTypeId = reader["prp_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur_typ"],
                            ProposedDurationDescription = reader["prp_dur_ds"] == DBNull.Value ? string.Empty : reader["prp_dur_ds"].ToString(),

                            ApprovedLeaveStartDate = reader["apv_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_sdt"],
                            ApprovedLeaveEndDate = reader["apv_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_edt"],
                            ApprovedLeaveDuration = reader["apv_lvs_dur"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur"],
                            ApprovedDurationTypeId = reader["apv_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur_typ"],
                            ApprovedDurationDescription = reader["apv_dur_ds"] == DBNull.Value ? string.Empty : reader["apv_dur_ds"].ToString(),

                            ActualLeaveStartDate = reader["act_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_sdt"],
                            ActualLeaveEndDate = reader["act_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_edt"],
                            ActualLeaveDuration = reader["act_lvs_dur"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur"],
                            ActualDurationTypeId = reader["act_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur_typ"],
                            ActualDurationDescription = reader["act_dur_ds"] == DBNull.Value ? string.Empty : reader["act_dur_ds"].ToString(),

                            RequestCloseDate = reader["rqs_cls_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rqs_cls_dt"],

                            LineManagersResumptionDate = reader["lm_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_rsmptn_dt"],
                            LineManagerConfirmResumptionDate = reader["lm_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_confm_dt"],
                            LineManagerConfirmResumptionBy = reader["lm_confm_by"] == DBNull.Value ? string.Empty : reader["lm_confm_by"].ToString(),

                            HrResumptionDate = reader["hr_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_rsmptn_dt"],
                            HrConfirmResumptionDate = reader["hr_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_confm_dt"],
                            HrConfirmResumptionBy = reader["hr_confm_by"] == DBNull.Value ? string.Empty : reader["hr_confm_by"].ToString(),

                            ApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
                            ApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],
                            ApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
                            ApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
                            ApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveList;
        }
        #endregion

        #region All Employee Leaves By Location, Department, Unit
        //=== All Employee Leaves By Location, Department, Unit etc ========//
        public async Task<EmployeeLeave> GetByIdAsync(long id)
        {
            EmployeeLeave e = new EmployeeLeave();
            string query = string.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT v.lvs_inf_id, v.emp_id, v.unit_id, v.dept_id, ");
            sb.Append("v.loc_id, v.lvs_yr, v.lvs_typ_cd, v.lvs_rsn, v.lvs_sts, ");
            sb.Append("v.is_pln, v.prp_lvs_sdt, v.prp_lvs_edt, v.prp_lvs_dur, ");
            sb.Append("v.act_lvs_dur, v.prp_dur_ds, v.apv_lvs_sdt, v.apv_lvs_edt,  ");
            sb.Append("v.apv_lvs_dur, v.apv_dur_ds, v.act_lvs_sdt, v.act_lvs_edt, ");
            sb.Append("v.act_dur_ds, v.lm_rsmptn_dt, v.lm_confm_dt, v.lm_confm_by, ");
            sb.Append("v.hr_rsmptn_dt, v.hr_confm_dt, v.hr_confm_by, v.rqs_cls_dt, ");
            sb.Append("v.is_lm_aprv, v.is_hd_aprv, v.is_hr_aprv, v.is_xm_aprv, ");
            sb.Append("v.is_sm_aprv, v.prp_lvs_dur_typ, v.apv_lvs_dur_typ, v.act_lvs_dur_typ,  ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = v.emp_id) ");
            sb.Append("as emp_nm, (SELECT lvs_typ_nm FROM public.lms_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = v.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = v.unit_id) ");
            sb.Append("as unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= v.dept_id) as dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = v.loc_id) as loc_nm ");
            sb.Append("FROM public.lms_lvs_infs v ");
            sb.Append("WHERE (v.lvs_inf_id = @lvs_inf_id); ");

            query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_inf_id = cmd.Parameters.Add("@lvs_inf_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    lvs_inf_id.Value = id;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        e.Id = reader["lvs_inf_id"] == DBNull.Value ? 0L : (long)reader["lvs_inf_id"];
                        e.EmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString();
                        e.EmployeeFullName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString();

                        e.UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"];
                        e.UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString();
                        e.DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"];
                        e.DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString();
                        e.LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"];
                        e.LocationName = reader["loc_nm"] == DBNull.Value ? string.Empty : reader["loc_nm"].ToString();

                        e.LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"];
                        e.LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString();
                        e.LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString();
                        e.LeaveReason = reader["lvs_rsn"] == DBNull.Value ? string.Empty : reader["lvs_rsn"].ToString();
                        e.LeaveStatus = reader["lvs_sts"] == DBNull.Value ? string.Empty : reader["lvs_sts"].ToString();
                        e.IsPlan = reader["is_pln"] == DBNull.Value ? true : (bool)reader["is_pln"];

                        e.ProposedLeaveStartDate = reader["prp_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_sdt"];
                        e.ProposedLeaveEndDate = reader["prp_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_edt"];
                        e.ProposedLeaveDuration = reader["prp_lvs_dur"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur"];
                        e.ProposedDurationTypeId = reader["prp_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur_typ"];
                        e.ProposedDurationDescription = reader["prp_dur_ds"] == DBNull.Value ? string.Empty : reader["prp_dur_ds"].ToString();

                        e.ApprovedLeaveStartDate = reader["apv_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_sdt"];
                        e.ApprovedLeaveEndDate = reader["apv_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_edt"];
                        e.ApprovedLeaveDuration = reader["apv_lvs_dur"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur"];
                        e.ApprovedDurationTypeId = reader["apv_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur_typ"];
                        e.ApprovedDurationDescription = reader["apv_dur_ds"] == DBNull.Value ? string.Empty : reader["apv_dur_ds"].ToString();

                        e.ActualLeaveStartDate = reader["act_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_sdt"];
                        e.ActualLeaveEndDate = reader["act_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_edt"];
                        e.ActualLeaveDuration = reader["act_lvs_dur"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur"];
                        e.ActualDurationTypeId = reader["act_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur_typ"];
                        e.ActualDurationDescription = reader["act_dur_ds"] == DBNull.Value ? string.Empty : reader["act_dur_ds"].ToString();

                        e.RequestCloseDate = reader["rqs_cls_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rqs_cls_dt"];

                        e.LineManagersResumptionDate = reader["lm_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_rsmptn_dt"];
                        e.LineManagerConfirmResumptionDate = reader["lm_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_confm_dt"];
                        e.LineManagerConfirmResumptionBy = reader["lm_confm_by"] == DBNull.Value ? string.Empty : reader["lm_confm_by"].ToString();

                        e.HrResumptionDate = reader["hr_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_rsmptn_dt"];
                        e.HrConfirmResumptionDate = reader["hr_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_confm_dt"];
                        e.HrConfirmResumptionBy = reader["hr_confm_by"] == DBNull.Value ? string.Empty : reader["hr_confm_by"].ToString();

                        e.ApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"];
                        e.ApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"];
                        e.ApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"];
                        e.ApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"];
                        e.ApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"];
                    }
                }
                await conn.CloseAsync();
            }
            return e;
        }
        public async Task<List<EmployeeLeave>> GetAllAsync(bool isPlan)
        {
            List<EmployeeLeave> leaveList = new List<EmployeeLeave>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT v.lvs_inf_id, v.emp_id, v.unit_id, v.dept_id, ");
            sb.Append("v.loc_id, v.lvs_yr, v.lvs_typ_cd, v.lvs_rsn, v.lvs_sts, ");
            sb.Append("v.is_pln, v.prp_lvs_sdt, v.prp_lvs_edt, v.prp_lvs_dur, ");
            sb.Append("v.act_lvs_dur, v.prp_dur_ds, v.apv_lvs_sdt, v.apv_lvs_edt,  ");
            sb.Append("v.apv_lvs_dur, v.apv_dur_ds, v.act_lvs_sdt, v.act_lvs_edt, ");
            sb.Append("v.act_dur_ds, v.lm_rsmptn_dt, v.lm_confm_dt, v.lm_confm_by, ");
            sb.Append("v.hr_rsmptn_dt, v.hr_confm_dt, v.hr_confm_by, v.rqs_cls_dt, ");
            sb.Append("v.is_lm_aprv, v.is_hd_aprv, v.is_hr_aprv, v.is_xm_aprv, ");
            sb.Append("v.is_sm_aprv, v.prp_lvs_dur_typ, v.apv_lvs_dur_typ, v.act_lvs_dur_typ,  ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = v.emp_id) ");
            sb.Append("as emp_nm, (SELECT lvs_typ_nm FROM public.lms_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = v.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = v.unit_id) ");
            sb.Append("as unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= v.dept_id) as dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = v.loc_id) as loc_nm ");
            sb.Append("FROM public.lms_lvs_infs v ");
            sb.Append("WHERE (v.is_pln = @is_pln) ");
            sb.Append("ORDER BY v.prp_lvs_sdt; ");
            
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var is_pln = cmd.Parameters.Add("@is_pln", NpgsqlDbType.Boolean);
                    await cmd.PrepareAsync();
                    is_pln.Value = isPlan;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        leaveList.Add(new EmployeeLeave()
                        {
                            Id = reader["lvs_inf_id"] == DBNull.Value ? 0L : (long)reader["lvs_inf_id"],
                            EmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
                            EmployeeFullName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),

                            UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                            UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
                            DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                            DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
                            LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                            LocationName = reader["loc_nm"] == DBNull.Value ? string.Empty : reader["loc_nm"].ToString(),

                            LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"],
                            LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString(),
                            LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
                            LeaveReason = reader["lvs_rsn"] == DBNull.Value ? string.Empty : reader["lvs_rsn"].ToString(),
                            LeaveStatus = reader["lvs_sts"] == DBNull.Value ? string.Empty : reader["lvs_sts"].ToString(),
                            IsPlan = reader["is_pln"] == DBNull.Value ? true : (bool)reader["is_pln"],

                            ProposedLeaveStartDate = reader["prp_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_sdt"],
                            ProposedLeaveEndDate = reader["prp_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_edt"],
                            ProposedLeaveDuration = reader["prp_lvs_dur"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur"],
                            ProposedDurationTypeId = reader["prp_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur_typ"],
                            ProposedDurationDescription = reader["prp_dur_ds"] == DBNull.Value ? string.Empty : reader["prp_dur_ds"].ToString(),

                            ApprovedLeaveStartDate = reader["apv_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_sdt"],
                            ApprovedLeaveEndDate = reader["apv_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_edt"],
                            ApprovedLeaveDuration = reader["apv_lvs_dur"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur"],
                            ApprovedDurationTypeId = reader["apv_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur_typ"],
                            ApprovedDurationDescription = reader["apv_dur_ds"] == DBNull.Value ? string.Empty : reader["apv_dur_ds"].ToString(),

                            ActualLeaveStartDate = reader["act_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_sdt"],
                            ActualLeaveEndDate = reader["act_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_edt"],
                            ActualLeaveDuration = reader["act_lvs_dur"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur"],
                            ActualDurationTypeId = reader["act_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur_typ"],
                            ActualDurationDescription = reader["act_dur_ds"] == DBNull.Value ? string.Empty : reader["act_dur_ds"].ToString(),

                            RequestCloseDate = reader["rqs_cls_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rqs_cls_dt"],

                            LineManagersResumptionDate = reader["lm_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_rsmptn_dt"],
                            LineManagerConfirmResumptionDate = reader["lm_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_confm_dt"],
                            LineManagerConfirmResumptionBy = reader["lm_confm_by"] == DBNull.Value ? string.Empty : reader["lm_confm_by"].ToString(),

                            HrResumptionDate = reader["hr_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_rsmptn_dt"],
                            HrConfirmResumptionDate = reader["hr_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_confm_dt"],
                            HrConfirmResumptionBy = reader["hr_confm_by"] == DBNull.Value ? string.Empty : reader["hr_confm_by"].ToString(),

                            ApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
                            ApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],
                            ApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
                            ApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
                            ApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveList;
        }
        public async Task<List<EmployeeLeave>> GetByYearAsync(int year, bool isPlan)
        {
            List<EmployeeLeave> leaveList = new List<EmployeeLeave>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT v.lvs_inf_id, v.emp_id, v.unit_id, v.dept_id, ");
            sb.Append("v.loc_id, v.lvs_yr, v.lvs_typ_cd, v.lvs_rsn, v.lvs_sts, ");
            sb.Append("v.is_pln, v.prp_lvs_sdt, v.prp_lvs_edt, v.prp_lvs_dur, ");
            sb.Append("v.act_lvs_dur, v.prp_dur_ds, v.apv_lvs_sdt, v.apv_lvs_edt,  ");
            sb.Append("v.apv_lvs_dur, v.apv_dur_ds, v.act_lvs_sdt, v.act_lvs_edt, ");
            sb.Append("v.act_dur_ds, v.lm_rsmptn_dt, v.lm_confm_dt, v.lm_confm_by, ");
            sb.Append("v.hr_rsmptn_dt, v.hr_confm_dt, v.hr_confm_by, v.rqs_cls_dt, ");
            sb.Append("v.is_lm_aprv, v.is_hd_aprv, v.is_hr_aprv, v.is_xm_aprv, ");
            sb.Append("v.is_sm_aprv, v.prp_lvs_dur_typ, v.apv_lvs_dur_typ, v.act_lvs_dur_typ,  ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = v.emp_id) ");
            sb.Append("as emp_nm, (SELECT lvs_typ_nm FROM public.lms_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = v.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = v.unit_id) ");
            sb.Append("as unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= v.dept_id) as dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = v.loc_id) as loc_nm ");
            sb.Append("FROM public.lms_lvs_infs v ");
            sb.Append("WHERE (v.lvs_yr = @lvs_yr) AND (v.is_pln = @is_pln) ");
            sb.Append("ORDER BY v.prp_lvs_sdt; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_yr = cmd.Parameters.Add("@lvs_yr", NpgsqlDbType.Integer);
                    var is_pln = cmd.Parameters.Add("@is_pln", NpgsqlDbType.Boolean);
                    await cmd.PrepareAsync();
                    lvs_yr.Value = year;
                    is_pln.Value = isPlan;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        leaveList.Add(new EmployeeLeave()
                        {
                            Id = reader["lvs_inf_id"] == DBNull.Value ? 0L : (long)reader["lvs_inf_id"],
                            EmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
                            EmployeeFullName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),

                            UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                            UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
                            DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                            DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
                            LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                            LocationName = reader["loc_nm"] == DBNull.Value ? string.Empty : reader["loc_nm"].ToString(),

                            LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"],
                            LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString(),
                            LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
                            LeaveReason = reader["lvs_rsn"] == DBNull.Value ? string.Empty : reader["lvs_rsn"].ToString(),
                            LeaveStatus = reader["lvs_sts"] == DBNull.Value ? string.Empty : reader["lvs_sts"].ToString(),
                            IsPlan = reader["is_pln"] == DBNull.Value ? true : (bool)reader["is_pln"],

                            ProposedLeaveStartDate = reader["prp_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_sdt"],
                            ProposedLeaveEndDate = reader["prp_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_edt"],
                            ProposedLeaveDuration = reader["prp_lvs_dur"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur"],
                            ProposedDurationTypeId = reader["prp_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur_typ"],
                            ProposedDurationDescription = reader["prp_dur_ds"] == DBNull.Value ? string.Empty : reader["prp_dur_ds"].ToString(),

                            ApprovedLeaveStartDate = reader["apv_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_sdt"],
                            ApprovedLeaveEndDate = reader["apv_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_edt"],
                            ApprovedLeaveDuration = reader["apv_lvs_dur"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur"],
                            ApprovedDurationTypeId = reader["apv_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur_typ"],
                            ApprovedDurationDescription = reader["apv_dur_ds"] == DBNull.Value ? string.Empty : reader["apv_dur_ds"].ToString(),

                            ActualLeaveStartDate = reader["act_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_sdt"],
                            ActualLeaveEndDate = reader["act_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_edt"],
                            ActualLeaveDuration = reader["act_lvs_dur"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur"],
                            ActualDurationTypeId = reader["act_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur_typ"],
                            ActualDurationDescription = reader["act_dur_ds"] == DBNull.Value ? string.Empty : reader["act_dur_ds"].ToString(),

                            RequestCloseDate = reader["rqs_cls_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rqs_cls_dt"],

                            LineManagersResumptionDate = reader["lm_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_rsmptn_dt"],
                            LineManagerConfirmResumptionDate = reader["lm_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_confm_dt"],
                            LineManagerConfirmResumptionBy = reader["lm_confm_by"] == DBNull.Value ? string.Empty : reader["lm_confm_by"].ToString(),

                            HrResumptionDate = reader["hr_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_rsmptn_dt"],
                            HrConfirmResumptionDate = reader["hr_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_confm_dt"],
                            HrConfirmResumptionBy = reader["hr_confm_by"] == DBNull.Value ? string.Empty : reader["hr_confm_by"].ToString(),

                            ApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
                            ApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],
                            ApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
                            ApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
                            ApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveList;
        }
        public async Task<List<EmployeeLeave>> GetByYearnStatusAsync(int year, string leaveStatus, bool isPlan)
        {
            List<EmployeeLeave> leaveList = new List<EmployeeLeave>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT v.lvs_inf_id, v.emp_id, v.unit_id, v.dept_id, ");
            sb.Append("v.loc_id, v.lvs_yr, v.lvs_typ_cd, v.lvs_rsn, v.lvs_sts, ");
            sb.Append("v.is_pln, v.prp_lvs_sdt, v.prp_lvs_edt, v.prp_lvs_dur, ");
            sb.Append("v.act_lvs_dur, v.prp_dur_ds, v.apv_lvs_sdt, v.apv_lvs_edt,  ");
            sb.Append("v.apv_lvs_dur, v.apv_dur_ds, v.act_lvs_sdt, v.act_lvs_edt, ");
            sb.Append("v.act_dur_ds, v.lm_rsmptn_dt, v.lm_confm_dt, v.lm_confm_by, ");
            sb.Append("v.hr_rsmptn_dt, v.hr_confm_dt, v.hr_confm_by, v.rqs_cls_dt, ");
            sb.Append("v.is_lm_aprv, v.is_hd_aprv, v.is_hr_aprv, v.is_xm_aprv, ");
            sb.Append("v.is_sm_aprv, v.prp_lvs_dur_typ, v.apv_lvs_dur_typ, v.act_lvs_dur_typ,  ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = v.emp_id) ");
            sb.Append("as emp_nm, (SELECT lvs_typ_nm FROM public.lms_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = v.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = v.unit_id) ");
            sb.Append("as unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= v.dept_id) as dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = v.loc_id) as loc_nm ");
            sb.Append("FROM public.lms_lvs_infs v ");
            sb.Append("WHERE (v.lvs_yr = @lvs_yr) AND (v.is_pln = @is_pln) ");
            sb.Append("AND v.lvs_sts = @lvs_sts ");
            sb.Append("ORDER BY v.prp_lvs_sdt; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_yr = cmd.Parameters.Add("@lvs_yr", NpgsqlDbType.Integer);
                    var lvs_sts = cmd.Parameters.Add("@lvs_sts", NpgsqlDbType.Text);
                    var is_pln = cmd.Parameters.Add("@is_pln", NpgsqlDbType.Boolean);
                    await cmd.PrepareAsync();
                    lvs_yr.Value = year;
                    lvs_sts.Value = leaveStatus;
                    is_pln.Value = isPlan;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        leaveList.Add(new EmployeeLeave()
                        {
                            Id = reader["lvs_inf_id"] == DBNull.Value ? 0L : (long)reader["lvs_inf_id"],
                            EmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
                            EmployeeFullName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),

                            UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                            UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
                            DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                            DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
                            LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                            LocationName = reader["loc_nm"] == DBNull.Value ? string.Empty : reader["loc_nm"].ToString(),

                            LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"],
                            LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString(),
                            LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
                            LeaveReason = reader["lvs_rsn"] == DBNull.Value ? string.Empty : reader["lvs_rsn"].ToString(),
                            LeaveStatus = reader["lvs_sts"] == DBNull.Value ? string.Empty : reader["lvs_sts"].ToString(),
                            IsPlan = reader["is_pln"] == DBNull.Value ? true : (bool)reader["is_pln"],

                            ProposedLeaveStartDate = reader["prp_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_sdt"],
                            ProposedLeaveEndDate = reader["prp_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_edt"],
                            ProposedLeaveDuration = reader["prp_lvs_dur"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur"],
                            ProposedDurationTypeId = reader["prp_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur_typ"],
                            ProposedDurationDescription = reader["prp_dur_ds"] == DBNull.Value ? string.Empty : reader["prp_dur_ds"].ToString(),

                            ApprovedLeaveStartDate = reader["apv_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_sdt"],
                            ApprovedLeaveEndDate = reader["apv_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_edt"],
                            ApprovedLeaveDuration = reader["apv_lvs_dur"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur"],
                            ApprovedDurationTypeId = reader["apv_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur_typ"],
                            ApprovedDurationDescription = reader["apv_dur_ds"] == DBNull.Value ? string.Empty : reader["apv_dur_ds"].ToString(),

                            ActualLeaveStartDate = reader["act_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_sdt"],
                            ActualLeaveEndDate = reader["act_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_edt"],
                            ActualLeaveDuration = reader["act_lvs_dur"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur"],
                            ActualDurationTypeId = reader["act_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur_typ"],
                            ActualDurationDescription = reader["act_dur_ds"] == DBNull.Value ? string.Empty : reader["act_dur_ds"].ToString(),

                            RequestCloseDate = reader["rqs_cls_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rqs_cls_dt"],

                            LineManagersResumptionDate = reader["lm_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_rsmptn_dt"],
                            LineManagerConfirmResumptionDate = reader["lm_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_confm_dt"],
                            LineManagerConfirmResumptionBy = reader["lm_confm_by"] == DBNull.Value ? string.Empty : reader["lm_confm_by"].ToString(),

                            HrResumptionDate = reader["hr_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_rsmptn_dt"],
                            HrConfirmResumptionDate = reader["hr_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_confm_dt"],
                            HrConfirmResumptionBy = reader["hr_confm_by"] == DBNull.Value ? string.Empty : reader["hr_confm_by"].ToString(),

                            ApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
                            ApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],
                            ApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
                            ApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
                            ApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveList;
        }
        public async Task<List<EmployeeLeave>> GetByYearnMonthAsync(int year, int month, bool isPlan)
        {
            List<EmployeeLeave> leaveList = new List<EmployeeLeave>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT v.lvs_inf_id, v.emp_id, v.unit_id, v.dept_id, ");
            sb.Append("v.loc_id, v.lvs_yr, v.lvs_typ_cd, v.lvs_rsn, v.lvs_sts, ");
            sb.Append("v.is_pln, v.prp_lvs_sdt, v.prp_lvs_edt, v.prp_lvs_dur, ");
            sb.Append("v.act_lvs_dur, v.prp_dur_ds, v.apv_lvs_sdt, v.apv_lvs_edt,  ");
            sb.Append("v.apv_lvs_dur, v.apv_dur_ds, v.act_lvs_sdt, v.act_lvs_edt, ");
            sb.Append("v.act_dur_ds, v.lm_rsmptn_dt, v.lm_confm_dt, v.lm_confm_by, ");
            sb.Append("v.hr_rsmptn_dt, v.hr_confm_dt, v.hr_confm_by, v.rqs_cls_dt, ");
            sb.Append("v.is_lm_aprv, v.is_hd_aprv, v.is_hr_aprv, v.is_xm_aprv, ");
            sb.Append("v.is_sm_aprv, v.prp_lvs_dur_typ, v.apv_lvs_dur_typ, v.act_lvs_dur_typ,  ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = v.emp_id) ");
            sb.Append("as emp_nm, (SELECT lvs_typ_nm FROM public.lms_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = v.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = v.unit_id) ");
            sb.Append("as unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= v.dept_id) as dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = v.loc_id) as loc_nm ");
            sb.Append("FROM public.lms_lvs_infs v ");
            sb.Append("WHERE (v.is_pln = @is_pln) ");
            sb.Append("AND DATE_PART('Year', v.prp_lvs_sdt) = @lvs_yr ");
            sb.Append("AND DATE_PART('Month', v.prp_lvs_sdt) = @lvs_month ");
            sb.Append("ORDER BY v.prp_lvs_sdt; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_yr = cmd.Parameters.Add("@lvs_yr", NpgsqlDbType.Integer);
                    var lvs_month = cmd.Parameters.Add("@lvs_month", NpgsqlDbType.Integer);
                    var is_pln = cmd.Parameters.Add("@is_pln", NpgsqlDbType.Boolean);
                    await cmd.PrepareAsync();
                    lvs_yr.Value = year;
                    lvs_month.Value = month;
                    is_pln.Value = isPlan;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        leaveList.Add(new EmployeeLeave()
                        {
                            Id = reader["lvs_inf_id"] == DBNull.Value ? 0L : (long)reader["lvs_inf_id"],
                            EmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
                            EmployeeFullName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),

                            UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                            UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
                            DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                            DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
                            LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                            LocationName = reader["loc_nm"] == DBNull.Value ? string.Empty : reader["loc_nm"].ToString(),

                            LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"],
                            LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString(),
                            LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
                            LeaveReason = reader["lvs_rsn"] == DBNull.Value ? string.Empty : reader["lvs_rsn"].ToString(),
                            LeaveStatus = reader["lvs_sts"] == DBNull.Value ? string.Empty : reader["lvs_sts"].ToString(),
                            IsPlan = reader["is_pln"] == DBNull.Value ? true : (bool)reader["is_pln"],

                            ProposedLeaveStartDate = reader["prp_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_sdt"],
                            ProposedLeaveEndDate = reader["prp_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_edt"],
                            ProposedLeaveDuration = reader["prp_lvs_dur"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur"],
                            ProposedDurationTypeId = reader["prp_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur_typ"],
                            ProposedDurationDescription = reader["prp_dur_ds"] == DBNull.Value ? string.Empty : reader["prp_dur_ds"].ToString(),

                            ApprovedLeaveStartDate = reader["apv_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_sdt"],
                            ApprovedLeaveEndDate = reader["apv_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_edt"],
                            ApprovedLeaveDuration = reader["apv_lvs_dur"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur"],
                            ApprovedDurationTypeId = reader["apv_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur_typ"],
                            ApprovedDurationDescription = reader["apv_dur_ds"] == DBNull.Value ? string.Empty : reader["apv_dur_ds"].ToString(),

                            ActualLeaveStartDate = reader["act_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_sdt"],
                            ActualLeaveEndDate = reader["act_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_edt"],
                            ActualLeaveDuration = reader["act_lvs_dur"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur"],
                            ActualDurationTypeId = reader["act_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur_typ"],
                            ActualDurationDescription = reader["act_dur_ds"] == DBNull.Value ? string.Empty : reader["act_dur_ds"].ToString(),

                            RequestCloseDate = reader["rqs_cls_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rqs_cls_dt"],

                            LineManagersResumptionDate = reader["lm_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_rsmptn_dt"],
                            LineManagerConfirmResumptionDate = reader["lm_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_confm_dt"],
                            LineManagerConfirmResumptionBy = reader["lm_confm_by"] == DBNull.Value ? string.Empty : reader["lm_confm_by"].ToString(),

                            HrResumptionDate = reader["hr_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_rsmptn_dt"],
                            HrConfirmResumptionDate = reader["hr_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_confm_dt"],
                            HrConfirmResumptionBy = reader["hr_confm_by"] == DBNull.Value ? string.Empty : reader["hr_confm_by"].ToString(),

                            ApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
                            ApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],
                            ApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
                            ApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
                            ApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveList;
        }
        public async Task<List<EmployeeLeave>> GetByYearnMonthnStatusAsync(int year, int month, string leaveStatus, bool isPlan)
        {
            List<EmployeeLeave> leaveList = new List<EmployeeLeave>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT v.lvs_inf_id, v.emp_id, v.unit_id, v.dept_id, ");
            sb.Append("v.loc_id, v.lvs_yr, v.lvs_typ_cd, v.lvs_rsn, v.lvs_sts, ");
            sb.Append("v.is_pln, v.prp_lvs_sdt, v.prp_lvs_edt, v.prp_lvs_dur, ");
            sb.Append("v.act_lvs_dur, v.prp_dur_ds, v.apv_lvs_sdt, v.apv_lvs_edt,  ");
            sb.Append("v.apv_lvs_dur, v.apv_dur_ds, v.act_lvs_sdt, v.act_lvs_edt, ");
            sb.Append("v.act_dur_ds, v.lm_rsmptn_dt, v.lm_confm_dt, v.lm_confm_by, ");
            sb.Append("v.hr_rsmptn_dt, v.hr_confm_dt, v.hr_confm_by, v.rqs_cls_dt, ");
            sb.Append("v.is_lm_aprv, v.is_hd_aprv, v.is_hr_aprv, v.is_xm_aprv, ");
            sb.Append("v.is_sm_aprv, v.prp_lvs_dur_typ, v.apv_lvs_dur_typ, v.act_lvs_dur_typ,  ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = v.emp_id) ");
            sb.Append("as emp_nm, (SELECT lvs_typ_nm FROM public.lms_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = v.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = v.unit_id) ");
            sb.Append("as unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= v.dept_id) as dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = v.loc_id) as loc_nm ");
            sb.Append("FROM public.lms_lvs_infs v ");
            sb.Append("WHERE (v.is_pln = @is_pln) AND (v.lvs_sts = @lvs_sts) ");
            sb.Append("AND DATE_PART('Year', v.prp_lvs_sdt) = @lvs_yr ");
            sb.Append("AND DATE_PART('Month', v.prp_lvs_sdt) = @lvs_month ");
            sb.Append("ORDER BY v.prp_lvs_sdt; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_yr = cmd.Parameters.Add("@lvs_yr", NpgsqlDbType.Integer);
                    var lvs_month = cmd.Parameters.Add("@lvs_month", NpgsqlDbType.Integer);
                    var lvs_sts = cmd.Parameters.Add("@lvs_sts", NpgsqlDbType.Text);
                    var is_pln = cmd.Parameters.Add("@is_pln", NpgsqlDbType.Boolean);
                    await cmd.PrepareAsync();
                    lvs_yr.Value = year;
                    lvs_month.Value = month;
                    lvs_sts.Value = leaveStatus;
                    is_pln.Value = isPlan;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        leaveList.Add(new EmployeeLeave()
                        {
                            Id = reader["lvs_inf_id"] == DBNull.Value ? 0L : (long)reader["lvs_inf_id"],
                            EmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
                            EmployeeFullName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),

                            UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                            UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
                            DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                            DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
                            LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                            LocationName = reader["loc_nm"] == DBNull.Value ? string.Empty : reader["loc_nm"].ToString(),

                            LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"],
                            LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString(),
                            LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
                            LeaveReason = reader["lvs_rsn"] == DBNull.Value ? string.Empty : reader["lvs_rsn"].ToString(),
                            LeaveStatus = reader["lvs_sts"] == DBNull.Value ? string.Empty : reader["lvs_sts"].ToString(),
                            IsPlan = reader["is_pln"] == DBNull.Value ? true : (bool)reader["is_pln"],

                            ProposedLeaveStartDate = reader["prp_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_sdt"],
                            ProposedLeaveEndDate = reader["prp_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_edt"],
                            ProposedLeaveDuration = reader["prp_lvs_dur"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur"],
                            ProposedDurationTypeId = reader["prp_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur_typ"],
                            ProposedDurationDescription = reader["prp_dur_ds"] == DBNull.Value ? string.Empty : reader["prp_dur_ds"].ToString(),

                            ApprovedLeaveStartDate = reader["apv_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_sdt"],
                            ApprovedLeaveEndDate = reader["apv_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_edt"],
                            ApprovedLeaveDuration = reader["apv_lvs_dur"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur"],
                            ApprovedDurationTypeId = reader["apv_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur_typ"],
                            ApprovedDurationDescription = reader["apv_dur_ds"] == DBNull.Value ? string.Empty : reader["apv_dur_ds"].ToString(),

                            ActualLeaveStartDate = reader["act_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_sdt"],
                            ActualLeaveEndDate = reader["act_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_edt"],
                            ActualLeaveDuration = reader["act_lvs_dur"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur"],
                            ActualDurationTypeId = reader["act_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur_typ"],
                            ActualDurationDescription = reader["act_dur_ds"] == DBNull.Value ? string.Empty : reader["act_dur_ds"].ToString(),

                            RequestCloseDate = reader["rqs_cls_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rqs_cls_dt"],

                            LineManagersResumptionDate = reader["lm_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_rsmptn_dt"],
                            LineManagerConfirmResumptionDate = reader["lm_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_confm_dt"],
                            LineManagerConfirmResumptionBy = reader["lm_confm_by"] == DBNull.Value ? string.Empty : reader["lm_confm_by"].ToString(),

                            HrResumptionDate = reader["hr_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_rsmptn_dt"],
                            HrConfirmResumptionDate = reader["hr_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_confm_dt"],
                            HrConfirmResumptionBy = reader["hr_confm_by"] == DBNull.Value ? string.Empty : reader["hr_confm_by"].ToString(),

                            ApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
                            ApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],
                            ApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
                            ApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
                            ApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveList;
        }
        public async Task<List<EmployeeLeave>> GetByStatusAsync(string leaveStatus, bool isPlan)
        {
            List<EmployeeLeave> leaveList = new List<EmployeeLeave>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT v.lvs_inf_id, v.emp_id, v.unit_id, v.dept_id, ");
            sb.Append("v.loc_id, v.lvs_yr, v.lvs_typ_cd, v.lvs_rsn, v.lvs_sts, ");
            sb.Append("v.is_pln, v.prp_lvs_sdt, v.prp_lvs_edt, v.prp_lvs_dur, ");
            sb.Append("v.act_lvs_dur, v.prp_dur_ds, v.apv_lvs_sdt, v.apv_lvs_edt,  ");
            sb.Append("v.apv_lvs_dur, v.apv_dur_ds, v.act_lvs_sdt, v.act_lvs_edt, ");
            sb.Append("v.act_dur_ds, v.lm_rsmptn_dt, v.lm_confm_dt, v.lm_confm_by, ");
            sb.Append("v.hr_rsmptn_dt, v.hr_confm_dt, v.hr_confm_by, v.rqs_cls_dt, ");
            sb.Append("v.is_lm_aprv, v.is_hd_aprv, v.is_hr_aprv, v.is_xm_aprv, ");
            sb.Append("v.is_sm_aprv, v.prp_lvs_dur_typ, v.apv_lvs_dur_typ, v.act_lvs_dur_typ,  ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = v.emp_id) ");
            sb.Append("as emp_nm, (SELECT lvs_typ_nm FROM public.lms_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = v.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = v.unit_id) ");
            sb.Append("as unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= v.dept_id) as dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = v.loc_id) as loc_nm ");
            sb.Append("FROM public.lms_lvs_infs v ");
            sb.Append("WHERE (v.is_pln = @is_pln) AND (v.lvs_sts = @lvs_sts) ");
            sb.Append("ORDER BY v.prp_lvs_sdt; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_sts = cmd.Parameters.Add("@lvs_sts", NpgsqlDbType.Text);
                    var is_pln = cmd.Parameters.Add("@is_pln", NpgsqlDbType.Boolean);
                    await cmd.PrepareAsync();
                    lvs_sts.Value = leaveStatus;
                    is_pln.Value = isPlan;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        leaveList.Add(new EmployeeLeave()
                        {
                            Id = reader["lvs_inf_id"] == DBNull.Value ? 0L : (long)reader["lvs_inf_id"],
                            EmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
                            EmployeeFullName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),

                            UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                            UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
                            DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                            DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
                            LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                            LocationName = reader["loc_nm"] == DBNull.Value ? string.Empty : reader["loc_nm"].ToString(),

                            LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"],
                            LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString(),
                            LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
                            LeaveReason = reader["lvs_rsn"] == DBNull.Value ? string.Empty : reader["lvs_rsn"].ToString(),
                            LeaveStatus = reader["lvs_sts"] == DBNull.Value ? string.Empty : reader["lvs_sts"].ToString(),
                            IsPlan = reader["is_pln"] == DBNull.Value ? true : (bool)reader["is_pln"],

                            ProposedLeaveStartDate = reader["prp_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_sdt"],
                            ProposedLeaveEndDate = reader["prp_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_edt"],
                            ProposedLeaveDuration = reader["prp_lvs_dur"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur"],
                            ProposedDurationTypeId = reader["prp_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur_typ"],
                            ProposedDurationDescription = reader["prp_dur_ds"] == DBNull.Value ? string.Empty : reader["prp_dur_ds"].ToString(),

                            ApprovedLeaveStartDate = reader["apv_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_sdt"],
                            ApprovedLeaveEndDate = reader["apv_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_edt"],
                            ApprovedLeaveDuration = reader["apv_lvs_dur"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur"],
                            ApprovedDurationTypeId = reader["apv_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur_typ"],
                            ApprovedDurationDescription = reader["apv_dur_ds"] == DBNull.Value ? string.Empty : reader["apv_dur_ds"].ToString(),

                            ActualLeaveStartDate = reader["act_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_sdt"],
                            ActualLeaveEndDate = reader["act_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_edt"],
                            ActualLeaveDuration = reader["act_lvs_dur"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur"],
                            ActualDurationTypeId = reader["act_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur_typ"],
                            ActualDurationDescription = reader["act_dur_ds"] == DBNull.Value ? string.Empty : reader["act_dur_ds"].ToString(),

                            RequestCloseDate = reader["rqs_cls_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rqs_cls_dt"],

                            LineManagersResumptionDate = reader["lm_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_rsmptn_dt"],
                            LineManagerConfirmResumptionDate = reader["lm_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_confm_dt"],
                            LineManagerConfirmResumptionBy = reader["lm_confm_by"] == DBNull.Value ? string.Empty : reader["lm_confm_by"].ToString(),

                            HrResumptionDate = reader["hr_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_rsmptn_dt"],
                            HrConfirmResumptionDate = reader["hr_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_confm_dt"],
                            HrConfirmResumptionBy = reader["hr_confm_by"] == DBNull.Value ? string.Empty : reader["hr_confm_by"].ToString(),

                            ApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
                            ApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],
                            ApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
                            ApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
                            ApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveList;
        }
        #endregion
        #endregion

        #region Leave Submission Action Methods
        //==== Leave Submission Read Action Methods
        public async Task<List<LeaveSubmission>> GetSubmissionsByToEmployeeIdAsync(string toEmployeeId)
        {
            List<LeaveSubmission> submissionList = new List<LeaveSubmission>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.lvs_sbm_id, s.lvs_inf_id, s.frm_emp_id, s.to_emp_id, ");
            sb.Append("s.sbm_purps, s.sbm_dt, s.sbm_msg, s.is_xtn, s.dt_xtn, s.to_emp_rl, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns p1 ");
            sb.Append("WHERE p1.id = s.frm_emp_id) as frm_emp_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns p2 ");
            sb.Append("WHERE p2.id = s.to_emp_id) as to_emp_nm ");
            sb.Append("FROM public.lms_lvs_sbms s ");
            sb.Append("WHERE s.to_emp_id = @to_emp_id ");
            sb.Append("ORDER BY s.lvs_sbm_id DESC; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var to_emp_id = cmd.Parameters.Add("@to_emp_id", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    to_emp_id.Value = toEmployeeId;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        submissionList.Add(new LeaveSubmission()
                        {
                            Id = reader["lvs_sbm_id"] == DBNull.Value ? 0 : (long)reader["lvs_sbm_id"],
                            LeaveId = reader["lvs_inf_id"] == DBNull.Value ? 0 : (long)reader["lvs_inf_id"],
                            ToEmployeeId = reader["to_emp_id"] == DBNull.Value ? string.Empty : reader["to_emp_id"].ToString(),
                            ToEmployeeName = reader["to_emp_nm"] == DBNull.Value ? string.Empty : reader["to_emp_nm"].ToString(),
                            FromEmployeeId = reader["frm_emp_id"] == DBNull.Value ? string.Empty : reader["frm_emp_id"].ToString(),
                            FromEmployeeName = reader["frm_emp_nm"] == DBNull.Value ? string.Empty : reader["frm_emp_nm"].ToString(),
                            TimeSubmitted = reader["sbm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["sbm_dt"],
                            Purpose = reader["sbm_purps"] == DBNull.Value ? string.Empty : reader["sbm_purps"].ToString(),
                            Message = reader["sbm_msg"] == DBNull.Value ? string.Empty : reader["sbm_msg"].ToString(),
                            IsActioned = reader["is_xtn"] == DBNull.Value ? false : (bool)reader["is_xtn"],
                            TimeActioned = reader["dt_xtn"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dt_xtn"],
                            ToEmployeeRole = reader["to_emp_rl"] == DBNull.Value ? string.Empty : reader["to_emp_rl"].ToString(),
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return submissionList;
        }
        public async Task<List<LeaveSubmission>> GetSubmissionsByYearSubmittedAsync(int yearSubmitted)
        {
            List<LeaveSubmission> submissionList = new List<LeaveSubmission>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.lvs_sbm_id, s.lvs_inf_id, s.frm_emp_id, s.to_emp_id, ");
            sb.Append("s.sbm_purps, s.sbm_dt, s.sbm_msg, s.is_xtn, s.dt_xtn, s.to_emp_rl, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns p1 ");
            sb.Append("WHERE p1.id = s.frm_emp_id) as frm_emp_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns p2 ");
            sb.Append("WHERE p2.id = s.to_emp_id) as to_emp_nm ");
            sb.Append("FROM public.lms_lvs_sbms s ");
            sb.Append("WHERE DATE_PART('Year', s.sbm_dt) = @yr ");
            sb.Append("ORDER BY s.lvs_sbm_id DESC; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var yr = cmd.Parameters.Add("@yr", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    yr.Value = yearSubmitted;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        submissionList.Add(new LeaveSubmission()
                        {
                            Id = reader["lvs_sbm_id"] == DBNull.Value ? 0 : (long)reader["lvs_sbm_id"],
                            LeaveId = reader["lvs_inf_id"] == DBNull.Value ? 0 : (long)reader["lvs_inf_id"],
                            ToEmployeeId = reader["to_emp_id"] == DBNull.Value ? string.Empty : reader["to_emp_id"].ToString(),
                            ToEmployeeName = reader["to_emp_nm"] == DBNull.Value ? string.Empty : reader["to_emp_nm"].ToString(),
                            FromEmployeeId = reader["frm_emp_id"] == DBNull.Value ? string.Empty : reader["frm_emp_id"].ToString(),
                            FromEmployeeName = reader["frm_emp_nm"] == DBNull.Value ? string.Empty : reader["frm_emp_nm"].ToString(),
                            TimeSubmitted = reader["sbm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["sbm_dt"],
                            Purpose = reader["sbm_purps"] == DBNull.Value ? string.Empty : reader["sbm_purps"].ToString(),
                            Message = reader["sbm_msg"] == DBNull.Value ? string.Empty : reader["sbm_msg"].ToString(),
                            IsActioned = reader["is_xtn"] == DBNull.Value ? false : (bool)reader["is_xtn"],
                            TimeActioned = reader["dt_xtn"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dt_xtn"],
                            ToEmployeeRole = reader["to_emp_rl"] == DBNull.Value ? string.Empty : reader["to_emp_rl"].ToString(),
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return submissionList;
        }
        public async Task<List<LeaveSubmission>> GetSubmissionsByYearnMonthSubmittedAsync(int yearSubmitted, int monthSubmitted)
        {
            List<LeaveSubmission> submissionList = new List<LeaveSubmission>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.lvs_sbm_id, s.lvs_inf_id, s.frm_emp_id, s.to_emp_id, ");
            sb.Append("s.sbm_purps, s.sbm_dt, s.sbm_msg, s.is_xtn, s.dt_xtn, s.to_emp_rl, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns p1 ");
            sb.Append("WHERE p1.id = s.frm_emp_id) as frm_emp_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns p2 ");
            sb.Append("WHERE p2.id = s.to_emp_id) as to_emp_nm ");
            sb.Append("FROM public.lms_lvs_sbms s ");
            sb.Append("WHERE DATE_PART('Year', s.sbm_dt) = @yr ");
            sb.Append("AND DATE_PART('Month', s.sbm_dt) = @mn ");
            sb.Append("ORDER BY s.lvs_sbm_id DESC; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var yr = cmd.Parameters.Add("@yr", NpgsqlDbType.Integer);
                    var mn = cmd.Parameters.Add("@mn", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    yr.Value = yearSubmitted;
                    mn.Value = monthSubmitted;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        submissionList.Add(new LeaveSubmission()
                        {
                            Id = reader["lvs_sbm_id"] == DBNull.Value ? 0 : (long)reader["lvs_sbm_id"],
                            LeaveId = reader["lvs_inf_id"] == DBNull.Value ? 0 : (long)reader["lvs_inf_id"],
                            ToEmployeeId = reader["to_emp_id"] == DBNull.Value ? string.Empty : reader["to_emp_id"].ToString(),
                            ToEmployeeName = reader["to_emp_nm"] == DBNull.Value ? string.Empty : reader["to_emp_nm"].ToString(),
                            FromEmployeeId = reader["frm_emp_id"] == DBNull.Value ? string.Empty : reader["frm_emp_id"].ToString(),
                            FromEmployeeName = reader["frm_emp_nm"] == DBNull.Value ? string.Empty : reader["frm_emp_nm"].ToString(),
                            TimeSubmitted = reader["sbm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["sbm_dt"],
                            Purpose = reader["sbm_purps"] == DBNull.Value ? string.Empty : reader["sbm_purps"].ToString(),
                            Message = reader["sbm_msg"] == DBNull.Value ? string.Empty : reader["sbm_msg"].ToString(),
                            IsActioned = reader["is_xtn"] == DBNull.Value ? false : (bool)reader["is_xtn"],
                            TimeActioned = reader["dt_xtn"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dt_xtn"],
                            ToEmployeeRole = reader["to_emp_rl"] == DBNull.Value ? string.Empty : reader["to_emp_rl"].ToString(),
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return submissionList;
        }
        public async Task<List<LeaveSubmission>> GetSubmissionsByToEmployeeIdnYearSubmittedAsync(string toEmployeeId, int yearSubmitted)
        {
            List<LeaveSubmission> submissionList = new List<LeaveSubmission>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.lvs_sbm_id, s.lvs_inf_id, s.frm_emp_id, s.to_emp_id, ");
            sb.Append("s.sbm_purps, s.sbm_dt, s.sbm_msg, s.is_xtn, s.dt_xtn, s.to_emp-rl, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns p1 ");
            sb.Append("WHERE p1.id = s.frm_emp_id) as frm_emp_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns p2 ");
            sb.Append("WHERE p2.id = s.to_emp_id) as to_emp_nm ");
            sb.Append("FROM public.lms_lvs_sbms s ");
            sb.Append("WHERE s.to_emp_id = @to_emp_id ");
            sb.Append("AND DATE_PART('Year', s.sbm_dt) = @yr ");
            sb.Append("ORDER BY s.lvs_sbm_id DESC; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var to_emp_id = cmd.Parameters.Add("@to_emp_id", NpgsqlDbType.Text);
                    var yr = cmd.Parameters.Add("@yr", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    to_emp_id.Value = toEmployeeId;
                    yr.Value = yearSubmitted;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        submissionList.Add(new LeaveSubmission()
                        {
                            Id = reader["lvs_sbm_id"] == DBNull.Value ? 0 : (long)reader["lvs_sbm_id"],
                            LeaveId = reader["lvs_inf_id"] == DBNull.Value ? 0 : (long)reader["lvs_inf_id"],
                            ToEmployeeId = reader["to_emp_id"] == DBNull.Value ? string.Empty : reader["to_emp_id"].ToString(),
                            ToEmployeeName = reader["to_emp_nm"] == DBNull.Value ? string.Empty : reader["to_emp_nm"].ToString(),
                            FromEmployeeId = reader["frm_emp_id"] == DBNull.Value ? string.Empty : reader["frm_emp_id"].ToString(),
                            FromEmployeeName = reader["frm_emp_nm"] == DBNull.Value ? string.Empty : reader["frm_emp_nm"].ToString(),
                            TimeSubmitted = reader["sbm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["sbm_dt"],
                            Purpose = reader["sbm_purps"] == DBNull.Value ? string.Empty : reader["sbm_purps"].ToString(),
                            Message = reader["sbm_msg"] == DBNull.Value ? string.Empty : reader["sbm_msg"].ToString(),
                            IsActioned = reader["is_xtn"] == DBNull.Value ? false : (bool)reader["is_xtn"],
                            TimeActioned = reader["dt_xtn"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dt_xtn"],
                            ToEmployeeRole = reader["to_emp_rl"] == DBNull.Value ? string.Empty : reader["to_emp_rl"].ToString(),
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return submissionList;
        }
        public async Task<List<LeaveSubmission>> GetSubmissionsByToEmployeeIdnYearnMonthSubmittedAsync(string toEmployeeId, int yearSubmitted, int monthSubmitted)
        {
            List<LeaveSubmission> submissionList = new List<LeaveSubmission>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.lvs_sbm_id, s.lvs_inf_id, s.frm_emp_id, s.to_emp_id, ");
            sb.Append("s.sbm_purps, s.sbm_dt, s.sbm_msg, s.is_xtn, s.dt_xtn, s.to_emp_rl, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns p1 ");
            sb.Append("WHERE p1.id = s.frm_emp_id) as frm_emp_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns p2 ");
            sb.Append("WHERE p2.id = s.to_emp_id) as to_emp_nm ");
            sb.Append("FROM public.lms_lvs_sbms s ");
            sb.Append("WHERE s.to_emp_id = @to_emp_id ");
            sb.Append("AND DATE_PART('Year', s.sbm_dt) = @yr ");
            sb.Append("AND DATE_PART('Month', s.sbm_dt) = @mn ");
            sb.Append("ORDER BY s.lvs_sbm_id DESC; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var to_emp_id = cmd.Parameters.Add("@to_emp_id", NpgsqlDbType.Text);
                    var yr = cmd.Parameters.Add("@yr", NpgsqlDbType.Integer);
                    var mn = cmd.Parameters.Add("@mn", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    to_emp_id.Value = toEmployeeId;
                    yr.Value = yearSubmitted;
                    mn.Value = monthSubmitted;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        submissionList.Add(new LeaveSubmission()
                        {
                            Id = reader["lvs_sbm_id"] == DBNull.Value ? 0 : (long)reader["lvs_sbm_id"],
                            LeaveId = reader["lvs_inf_id"] == DBNull.Value ? 0 : (long)reader["lvs_inf_id"],
                            ToEmployeeId = reader["to_emp_id"] == DBNull.Value ? string.Empty : reader["to_emp_id"].ToString(),
                            ToEmployeeName = reader["to_emp_nm"] == DBNull.Value ? string.Empty : reader["to_emp_nm"].ToString(),
                            FromEmployeeId = reader["frm_emp_id"] == DBNull.Value ? string.Empty : reader["frm_emp_id"].ToString(),
                            FromEmployeeName = reader["frm_emp_nm"] == DBNull.Value ? string.Empty : reader["frm_emp_nm"].ToString(),
                            TimeSubmitted = reader["sbm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["sbm_dt"],
                            Purpose = reader["sbm_purps"] == DBNull.Value ? string.Empty : reader["sbm_purps"].ToString(),
                            Message = reader["sbm_msg"] == DBNull.Value ? string.Empty : reader["sbm_msg"].ToString(),
                            IsActioned = reader["is_xtn"] == DBNull.Value ? false : (bool)reader["is_xtn"],
                            TimeActioned = reader["dt_xtn"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dt_xtn"],
                            ToEmployeeRole = reader["to_emp_rl"] == DBNull.Value ? string.Empty : reader["to_emp_rl"].ToString(),
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return submissionList;
        }

        //==== Leave Submission Write Action Methods
        public async Task<bool> AddSubmissionAsync(LeaveSubmission e)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.lms_lvs_sbms(lvs_inf_id, frm_emp_id, ");
            sb.Append("to_emp_id, sbm_purps, sbm_dt, sbm_msg, is_xtn, dt_xtn, ");
            sb.Append("to_emp_rl) VALUES (@lvs_inf_id, @frm_emp_id, @to_emp_id, ");
            sb.Append("@sbm_purps, @sbm_dt, @sbm_msg, @is_xtn, @dt_xtn, @to_emp_rl); ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_inf_id = cmd.Parameters.Add("@lvs_inf_id", NpgsqlDbType.Bigint);
                    var frm_emp_id = cmd.Parameters.Add("@frm_emp_id", NpgsqlDbType.Text);
                    var to_emp_id = cmd.Parameters.Add("@to_emp_id", NpgsqlDbType.Text);
                    var sbm_purps = cmd.Parameters.Add("@sbm_purps", NpgsqlDbType.Text);
                    var sbm_dt = cmd.Parameters.Add("@sbm_dt", NpgsqlDbType.TimestampTz);
                    var sbm_msg = cmd.Parameters.Add("@sbm_msg", NpgsqlDbType.Text);
                    var is_xtn = cmd.Parameters.Add("@is_xtn", NpgsqlDbType.Boolean);
                    var dt_xtn = cmd.Parameters.Add("@dt_xtn", NpgsqlDbType.TimestampTz);
                    var to_emp_rl = cmd.Parameters.Add("@to_emp_rl", NpgsqlDbType.Text);
                    cmd.Prepare();
                    lvs_inf_id.Value = e.LeaveId;
                    frm_emp_id.Value = e.FromEmployeeId;
                    to_emp_id.Value = e.ToEmployeeId;
                    sbm_purps.Value = e.Purpose;
                    sbm_dt.Value = e.TimeSubmitted;
                    sbm_msg.Value = e.Message ?? (object)DBNull.Value;
                    is_xtn.Value = e.IsActioned;
                    dt_xtn.Value = e.TimeActioned ?? (object)DBNull.Value;
                    to_emp_rl.Value = e.ToEmployeeRole;
                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            return rows > 0;
        }
        public async Task<bool> DeleteSubmissionAsync(long id)
        {
            int rows = 0;
            string query = "DELETE FROM public.lms_lvs_sbms WHERE (lvs_sbm_id = @lvs_sbm_id);";
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Delete data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_sbm_id = cmd.Parameters.Add("@lvs_sbm_id", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    lvs_sbm_id.Value = id;
                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            return rows > 0;
        }
        public async Task<bool> EditSubmissionActionStatusAsync(long submissionId, DateTime? timeActioned)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.lms_lvs_sbms SET is_xtn=true, dt_xtn=@dt_xtn ");
            sb.Append("WHERE lvs_sbm_id = @lvs_sbm_id; ");
            string query = sb.ToString();

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_sbm_id = cmd.Parameters.Add("@lvs_sbm_id", NpgsqlDbType.Bigint);
                    var dt_xtn = cmd.Parameters.Add("@dt_xtn", NpgsqlDbType.TimestampTz);
                    cmd.Prepare();
                    lvs_sbm_id.Value = submissionId;
                    dt_xtn.Value = timeActioned;
                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            return rows > 0;
        }

        #endregion

        #region Leave Approval Action Methods
        public async Task<long> AddApprovalAsync(LeaveApproval e)
        {
            long rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.lms_lvs_aprvs(lvs_inf_id, aprv_emp_nm, ");
            sb.Append("lvs_emp_nm, is_aprvd, lvs_aprv_dt, lvs_aprv_rmk, lvs_aprv_as) ");
            sb.Append("VALUES (@lvs_inf_id, @aprv_emp_nm, @lvs_emp_nm, @is_aprvd, ");
            sb.Append("@lvs_aprv_dt, @lvs_aprv_rmk, @lvs_aprv_as) ");
            sb.Append("RETURNING lvs_aprv_id;");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_inf_id = cmd.Parameters.Add("@lvs_inf_id", NpgsqlDbType.Bigint);
                    var aprv_emp_nm = cmd.Parameters.Add("@aprv_emp_nm", NpgsqlDbType.Text);
                    var lvs_emp_nm = cmd.Parameters.Add("@lvs_emp_nm", NpgsqlDbType.Text);
                    var is_aprvd = cmd.Parameters.Add("@is_aprvd", NpgsqlDbType.Boolean);
                    var lvs_aprv_dt = cmd.Parameters.Add("@lvs_aprv_dt", NpgsqlDbType.TimestampTz);
                    var lvs_aprv_rmk = cmd.Parameters.Add("@lvs_aprv_rmk", NpgsqlDbType.Text);
                    var lvs_aprv_as = cmd.Parameters.Add("@lvs_aprv_as", NpgsqlDbType.Text);
                    cmd.Prepare();
                    lvs_inf_id.Value = e.LeaveId;
                    aprv_emp_nm.Value = e.ApproverName;
                    lvs_emp_nm.Value = e.ApplicantName;
                    is_aprvd.Value = e.IsApproved;
                    lvs_aprv_dt.Value = e.TimeApproved ?? DateTime.Now;
                    lvs_aprv_rmk.Value = e.ApproverComments ?? string.Empty;
                    lvs_aprv_as.Value = e.ApproverRole;

                    var obj = await cmd.ExecuteScalarAsync();
                    rows = (long)obj;
                    await conn.CloseAsync();
                }
            }
            return rows;
        }
        public async Task<bool> DeleteApprovalAsync(long approvalId)
        {
            int rows = 0;
            string query = "DELETE FROM public.lms_lvs_aprvs WHERE (lvs_aprv_id = @lvs_aprv_id);";
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Delete data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_aprv_id = cmd.Parameters.Add("@lvs_aprv_id", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    lvs_aprv_id.Value = approvalId;
                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            return rows > 0;
        }
        public async Task<List<LeaveApproval>> GetApprovalsByLeaveIdAsync(long leaveId)
        {
            List<LeaveApproval> approvalsList = new List<LeaveApproval>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT lvs_aprv_id, lvs_inf_id, aprv_emp_nm, lvs_emp_nm, ");
            sb.Append("is_aprvd, lvs_aprv_dt, lvs_aprv_rmk, lvs_aprv_as	");
            sb.Append("FROM public.lms_lvs_aprvs ");
            sb.Append("WHERE lvs_inf_id = @lvs_inf_id ");
            sb.Append("ORDER BY lvs_aprv_id; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_inf_id = cmd.Parameters.Add("@lvs_inf_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    lvs_inf_id.Value = leaveId;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        approvalsList.Add(new LeaveApproval()
                        {
                            ApprovalId = reader["lvs_aprv_id"] == DBNull.Value ? 0L : (long)reader["lvs_aprv_id"],
                            LeaveId = reader["lvs_inf_id"] == DBNull.Value ? 0L : (long)reader["lvs_inf_id"],
                            ApproverName = reader["aprv_emp_nm"] == DBNull.Value ? string.Empty : reader["aprv_emp_nm"].ToString(),
                            ApplicantName = reader["lvs_emp_nm"] == DBNull.Value ? string.Empty : reader["lvs_emp_nm"].ToString(),
                            IsApproved = reader["is_aprvd"] == DBNull.Value ? false : (bool)reader["is_aprvd"],
                            TimeApproved = reader["lvs_aprv_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lvs_aprv_dt"],
                            ApproverComments = reader["lvs_aprv_rmk"] == DBNull.Value ? string.Empty : reader["lvs_aprv_rmk"].ToString(),
                            ApproverRole = reader["lvs_aprv_as"] == DBNull.Value ? string.Empty : reader["lvs_aprv_as"].ToString(),
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return approvalsList;
        }
        public async Task<LeaveApproval> GetApprovalByIdAsync(long approvalId)
        {
            List<LeaveApproval> approvalsList = new List<LeaveApproval>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT lvs_aprv_id, lvs_inf_id, aprv_emp_nm, lvs_emp_nm, ");
            sb.Append("is_aprvd, lvs_aprv_dt, lvs_aprv_rmk, lvs_aprv_as	");
            sb.Append("FROM public.lms_lvs_aprvs ");
            sb.Append("WHERE lvs_aprv_id = @lvs_aprv_id; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_aprv_id = cmd.Parameters.Add("@lvs_aprv_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    lvs_aprv_id.Value = approvalId;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        approvalsList.Add(new LeaveApproval()
                        {
                            ApprovalId = reader["lvs_aprv_id"] == DBNull.Value ? 0L : (long)reader["lvs_aprv_id"],
                            LeaveId = reader["lvs_inf_id"] == DBNull.Value ? 0L : (long)reader["lvs_inf_id"],
                            ApproverName = reader["aprv_emp_nm"] == DBNull.Value ? string.Empty : reader["aprv_emp_nm"].ToString(),
                            ApplicantName = reader["lvs_emp_nm"] == DBNull.Value ? string.Empty : reader["lvs_emp_nm"].ToString(),
                            IsApproved = reader["is_aprvd"] == DBNull.Value ? false : (bool)reader["is_aprvd"],
                            TimeApproved = reader["lvs_aprv_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lvs_aprv_dt"],
                            ApproverComments = reader["lvs_aprv_rmk"] == DBNull.Value ? string.Empty : reader["lvs_aprv_rmk"].ToString(),
                            ApproverRole = reader["lvs_aprv_as"] == DBNull.Value ? string.Empty : reader["lvs_aprv_as"].ToString(),
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return approvalsList[0];
        }

        #endregion

        #region Leave Notes Log Action Methods
        public async Task<bool> AddNoteAsync(LeaveNote e)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.lms_lvs_msgs ");
            sb.Append("(lvs_inf_id, frm_emp_nm, msg_ds, msg_dt) ");
            sb.Append("VALUES (@lvs_inf_id, @frm_emp_nm, @msg_ds, @msg_dt); ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_inf_id = cmd.Parameters.Add("@lvs_inf_id", NpgsqlDbType.Bigint);
                    var frm_emp_nm = cmd.Parameters.Add("@frm_emp_nm", NpgsqlDbType.Text);
                    var msg_ds = cmd.Parameters.Add("@msg_ds", NpgsqlDbType.Text);
                    var msg_dt = cmd.Parameters.Add("@msg_dt", NpgsqlDbType.TimestampTz);
                    cmd.Prepare();
                    lvs_inf_id.Value = e.LeaveId;
                    frm_emp_nm.Value = e.FromEmployeeName;
                    msg_ds.Value = e.NoteContent ?? (object)DBNull.Value; 
                    msg_dt.Value = DateTime.Now;
                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            return rows > 0;
        }
        public async Task<List<LeaveNote>> GetNotesByLeaveIdAsync(long leaveId)
        {
            List<LeaveNote> notesList = new List<LeaveNote>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT lvs_msg_id, lvs_inf_id, ");
            sb.Append("frm_emp_nm, msg_ds, msg_dt  ");
            sb.Append("FROM public.lms_lvs_msgs ");
            sb.Append("WHERE lvs_inf_id=@lvs_inf_id ");
            sb.Append("ORDER BY lvs_msg_id DESC; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_inf_id = cmd.Parameters.Add("@lvs_inf_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    lvs_inf_id.Value = leaveId;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        notesList.Add(new LeaveNote()
                        {
                            Id = reader["lvs_msg_id"] == DBNull.Value ? 0L : (long)reader["lvs_msg_id"],
                            LeaveId = reader["lvs_inf_id"] == DBNull.Value ? 0L : (long)reader["lvs_inf_id"],
                            FromEmployeeName = reader["frm_emp_nm"] == DBNull.Value ? string.Empty : reader["frm_emp_nm"].ToString(),
                            NoteContent = reader["msg_ds"] == DBNull.Value ? string.Empty : reader["msg_ds"].ToString(),
                            TimeAdded = reader["msg_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["msg_dt"],
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return notesList;
        }
        #endregion

        #region Leave Activity Log Action Methods
        public async Task<bool> AddLogAsync(LeaveActivityLog e)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.lms_lvs_logs(act_ds, act_dt, lvs_inf_id) ");
            sb.Append("VALUES (@act_ds, @act_dt, @lvs_inf_id); ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_inf_id = cmd.Parameters.Add("@lvs_inf_id", NpgsqlDbType.Bigint);
                    var act_ds = cmd.Parameters.Add("@act_ds", NpgsqlDbType.Text);
                    var act_dt = cmd.Parameters.Add("@act_dt", NpgsqlDbType.TimestampTz);
                    cmd.Prepare();
                    lvs_inf_id.Value = e.LeaveId;
                    act_ds.Value = e.ActivityDescription ?? (object)DBNull.Value;
                    act_dt.Value = e.ActivityTime ?? DateTime.Now;
                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            return rows > 0;
        }
        public async Task<List<LeaveActivityLog>> GetLogByLeaveIdAsync(long leaveId)
        {
            List<LeaveActivityLog> activityLogList = new List<LeaveActivityLog>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT log_id, act_ds, act_dt, lvs_inf_id ");
            sb.Append("FROM public.lms_lvs_logs ");
            sb.Append("WHERE lvs_inf_id=@lvs_inf_id ");
            sb.Append("ORDER BY log_id DESC; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_inf_id = cmd.Parameters.Add("@lvs_inf_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    lvs_inf_id.Value = leaveId;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        activityLogList.Add(new LeaveActivityLog()
                        {
                            Id = reader["log_id"] == DBNull.Value ? 0L : (long)reader["log_id"],
                            LeaveId = reader["lvs_inf_id"] == DBNull.Value ? 0L : (long)reader["lvs_inf_id"],
                            ActivityDescription = reader["act_ds"] == DBNull.Value ? string.Empty : reader["act_ds"].ToString(),
                            ActivityTime = reader["act_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["act_dt"],
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return activityLogList;
        }
        #endregion

        #region Leave Documents Action Methods
        public async Task<bool> AddDocumentAsync(LeaveDocument e)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.lms_lvs_docs(lvs_inf_id, ");
            sb.Append("doc_ttl, doc_ds, doc_fmt, doc_lnk, upl_dt, upl_by) ");
            sb.Append("VALUES (@lvs_inf_id, @doc_ttl, @doc_ds, ");
            sb.Append("@doc_fmt, @doc_lnk, @upl_dt, @upl_by);");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_inf_id = cmd.Parameters.Add("@lvs_inf_id", NpgsqlDbType.Bigint);
                    var doc_ttl = cmd.Parameters.Add("@doc_ttl", NpgsqlDbType.Text);
                    var doc_ds = cmd.Parameters.Add("@doc_ds", NpgsqlDbType.Text);
                    var doc_fmt = cmd.Parameters.Add("@doc_fmt", NpgsqlDbType.Text);
                    var doc_lnk = cmd.Parameters.Add("@doc_lnk", NpgsqlDbType.Text);
                    var upl_dt = cmd.Parameters.Add("@upl_dt", NpgsqlDbType.TimestampTz);
                    var upl_by = cmd.Parameters.Add("@upl_by", NpgsqlDbType.Text);
                    cmd.Prepare();
                    lvs_inf_id.Value = e.LeaveId;
                    doc_ttl.Value = e.DocumentTitle;
                    doc_ds.Value = e.DocumentDescription ?? (object)DBNull.Value;
                    doc_fmt.Value = e.DocumentFormat ?? (object)DBNull.Value;
                    doc_lnk.Value = e.DocumentLink ?? (object)DBNull.Value;
                    upl_dt.Value = e.UploadedTime ?? DateTime.Now;
                    upl_by.Value = e.UploadedBy ?? (object)DBNull.Value;
                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            return rows > 0;
        }
        public async Task<bool> DeleteDocumentAsync(long id)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("DELETE FROM public.lms_lvs_docs ");
            sb.Append("WHERE lvs_doc_id =  @lvs_doc_id;");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_doc_id = cmd.Parameters.Add("@lvs_doc_id", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    lvs_doc_id.Value = id;
                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            return rows > 0;
        }
        public async Task<LeaveDocument> GetDocumentByIdAsync(long id)
        {
            List<LeaveDocument> documentList = new List<LeaveDocument>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT lvs_doc_id, lvs_inf_id, doc_ttl, ");
            sb.Append("doc_ds, doc_fmt, doc_lnk, upl_dt, upl_by ");
            sb.Append("FROM public.lms_lvs_docs ");
            sb.Append("WHERE lvs_doc_id = @lvs_doc_id; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_doc_id = cmd.Parameters.Add("@lvs_doc_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    lvs_doc_id.Value = id;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        documentList.Add(new LeaveDocument()
                        {
                            DocumentId = reader["lvs_doc_id"] == DBNull.Value ? 0L : (long)reader["lvs_doc_id"],
                            LeaveId = reader["lvs_inf_id"] == DBNull.Value ? 0L : (long)reader["lvs_inf_id"],
                            DocumentTitle = reader["doc_ttl"] == DBNull.Value ? string.Empty : reader["doc_ttl"].ToString(),
                            DocumentDescription = reader["doc_ds"] == DBNull.Value ? string.Empty : reader["doc_ds"].ToString(),
                            DocumentFormat = reader["doc_fmt"] == DBNull.Value ? string.Empty : reader["doc_fmt"].ToString(),
                            DocumentLink = reader["doc_lnk"] == DBNull.Value ? string.Empty : reader["doc_lnk"].ToString(),
                            UploadedTime = reader["upl_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["upl_dt"],
                            UploadedBy = reader["upl_by"] == DBNull.Value ? string.Empty : reader["upl_by"].ToString(),
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return documentList[0];
        }

        public async Task<List<LeaveDocument>> GetDocumentsByLeaveIdAsync(long leaveId)
        {
            List<LeaveDocument> documentList = new List<LeaveDocument>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT lvs_doc_id, lvs_inf_id, doc_ttl, ");
            sb.Append("doc_ds, doc_fmt, doc_lnk, upl_dt, upl_by ");
            sb.Append("FROM public.lms_lvs_docs ");
            sb.Append("ORDER BY lvs_doc_id DESC; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_inf_id = cmd.Parameters.Add("@lvs_inf_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    lvs_inf_id.Value = leaveId;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        documentList.Add(new LeaveDocument()
                        {
                            DocumentId = reader["lvs_doc_id"] == DBNull.Value ? 0L : (long)reader["lvs_doc_id"],
                            LeaveId = reader["lvs_inf_id"] == DBNull.Value ? 0L : (long)reader["lvs_inf_id"],
                            DocumentTitle = reader["doc_ttl"] == DBNull.Value ? string.Empty : reader["doc_ttl"].ToString(),
                            DocumentDescription = reader["doc_ds"] == DBNull.Value ? string.Empty : reader["doc_ds"].ToString(),
                            DocumentFormat = reader["doc_fmt"] == DBNull.Value ? string.Empty : reader["doc_fmt"].ToString(),
                            DocumentLink = reader["doc_lnk"] == DBNull.Value ? string.Empty : reader["doc_lnk"].ToString(),
                            UploadedTime = reader["upl_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["upl_dt"],
                            UploadedBy = reader["upl_by"] == DBNull.Value ? string.Empty : reader["upl_by"].ToString(),
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return documentList;
        }
        
        
        #endregion
    }
}
