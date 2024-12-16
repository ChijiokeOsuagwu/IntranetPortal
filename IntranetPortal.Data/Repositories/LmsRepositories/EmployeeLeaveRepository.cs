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

        public async Task<List<EmployeeLeave>> GetByEmployeeIdnYearAsync(string employeeId, int year, bool getPlan)
        {
            List<EmployeeLeave> leaveList = new List<EmployeeLeave>();
            string query = string.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT v.lvs_inf_id, v.emp_id, v.lvs_yr, v.lvs_typ_cd, ");
            sb.Append("v.lvs_rsn, v.lvs_sts, v.lvs_sdt, v.lvs_edt, v.lvs_dur, ");
            sb.Append("v.unit_id, v.dept_id, v.loc_id, v.is_pln, v.dur_typ, ");
            sb.Append("CASE WHEN v.dur_typ = 0 THEN 'Working Day(s)' ");
            sb.Append("WHEN v.dur_typ = 1 THEN 'Day(s)' ");
            sb.Append("WHEN v.dur_typ = 2 THEN 'Week(s)' ");
            sb.Append("WHEN v.dur_typ = 3 THEN 'Month(s)' ");
            sb.Append("WHEN v.dur_typ = 4 THEN 'Year(s)' END as dur_typ_ds, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = v.emp_id) ");
            sb.Append("as emp_nm, (SELECT lvs_typ_nm FROM public.lms_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = v.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = v.unit_id) ");
            sb.Append("as unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= v.dept_id) as dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = v.loc_id) as loc_nm, v.rsmptn_dt, ");
            sb.Append("v.cls_rqs_dt, v.lm_rsmptn_dt, v.lm_confm_dt, ");
            sb.Append("v.lm_confm_by, v.hr_confm_dt, v.hr_confm_by ");
            sb.Append("FROM public.lms_lvs_infs v ");
            sb.Append("WHERE (v.emp_id = @emp_id AND v.lvs_yr = @lvs_yr ");
            sb.Append("AND v.is_pln = @is_pln) ORDER BY v.lvs_sdt; ");
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
                    is_pln.Value = getPlan;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        leaveList.Add(new EmployeeLeave()
                        {
                            Id = reader["lvs_inf_id"] == DBNull.Value ? 0 : (long)reader["lvs_inf_id"],
                            EmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
                            EmployeeFullName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),
                            LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"],
                            LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString(),
                            LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
                            LeaveReason = reader["lvs_rsn"] == DBNull.Value ? string.Empty : reader["lvs_rsn"].ToString(),
                            LeaveStatus = reader["lvs_sts"] == DBNull.Value ? string.Empty : reader["lvs_sts"].ToString(),
                            LeaveStartDate = reader["lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["lvs_sdt"],
                            LeaveEndDate = reader["lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["lvs_edt"],
                            Duration = reader["lvs_dur"] == DBNull.Value ? 0 : (int)reader["lvs_dur"],
                            DurationTypeId = reader["dur_typ"] == DBNull.Value ? 0 : (int)reader["dur_typ"],
                            DurationTypeDescription = reader["dur_typ_ds"] == DBNull.Value ? string.Empty : reader["dur_typ_ds"].ToString(),
                            UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                            UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
                            DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                            DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
                            LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                            LocationName = reader["loc_nm"] == DBNull.Value ? string.Empty : reader["loc_nm"].ToString(),
                            IsPlan = reader["is_pln"] == DBNull.Value ? true : (bool)reader["is_pln"],

                            ResumptionDate = reader["rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rsmptn_dt"],
                            CloseRequestDate = reader["cls_rqs_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["cls_rqs_dt"],
                            LineManagersResumptionDate = reader["lm_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_rsmptn_dt"],
                            LineManagerConfirmResumptionDate = reader["lm_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_confm_dt"],
                            LineManagerConfirmResumptionBy = reader["lm_confm_by"] == DBNull.Value ? string.Empty : reader["lm_confm_by"].ToString(),
                            HrConfirmResumptionDate = reader["hr_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_confm_dt"],
                            HrConfirmResumptionBy = reader["hr_confm_by"] == DBNull.Value ? string.Empty : reader["hr_confm_by"].ToString(),
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveList;
        }
        public async Task<EmployeeLeave> GetByIdAsync(long id)
        {
            EmployeeLeave e = new EmployeeLeave();
            string query = string.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT v.lvs_inf_id, v.emp_id, v.lvs_yr, v.lvs_typ_cd, ");
            sb.Append("v.lvs_rsn, v.lvs_sts, v.lvs_sdt, v.lvs_edt, v.lvs_dur, ");
            sb.Append("v.unit_id, v.dept_id, v.loc_id, v.is_pln, v.dur_typ, ");
            sb.Append("CASE WHEN v.dur_typ = 0 THEN 'Working Day(s)' ");
            sb.Append("WHEN v.dur_typ = 1 THEN 'Day(s)' ");
            sb.Append("WHEN v.dur_typ = 2 THEN 'Week(s)' ");
            sb.Append("WHEN v.dur_typ = 3 THEN 'Month(s)' ");
            sb.Append("WHEN v.dur_typ = 4 THEN 'Year(s)' END as dur_typ_ds, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = v.emp_id) ");
            sb.Append("as emp_nm, (SELECT lvs_typ_nm FROM public.lms_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = v.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = v.unit_id) ");
            sb.Append("as unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= v.dept_id) as dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = v.loc_id) as loc_nm, v.rsmptn_dt, ");
            sb.Append("v.cls_rqs_dt, v.lm_rsmptn_dt, v.lm_confm_dt, ");
            sb.Append("v.lm_confm_by, v.hr_confm_dt, v.hr_confm_by ");
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
                        e.Id = reader["lvs_inf_id"] == DBNull.Value ? 0 : (long)reader["lvs_inf_id"];
                        e.EmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString();
                        e.EmployeeFullName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString();
                        e.LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"];
                        e.LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString();
                        e.LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString();
                        e.LeaveReason = reader["lvs_rsn"] == DBNull.Value ? string.Empty : reader["lvs_rsn"].ToString();
                        e.LeaveStatus = reader["lvs_sts"] == DBNull.Value ? string.Empty : reader["lvs_sts"].ToString();
                        e.LeaveStartDate = reader["lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["lvs_sdt"];
                        e.LeaveEndDate = reader["lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["lvs_edt"];
                        e.Duration = reader["lvs_dur"] == DBNull.Value ? 0 : (int)reader["lvs_dur"];
                        e.DurationTypeId = reader["dur_typ"] == DBNull.Value ? 0 : (int)reader["dur_typ"];
                        e.DurationTypeDescription = reader["dur_typ_ds"] == DBNull.Value ? string.Empty : reader["dur_typ_ds"].ToString();
                        e.UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"];
                        e.UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString();
                        e.DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"];
                        e.DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString();
                        e.LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"];
                        e.LocationName = reader["loc_nm"] == DBNull.Value ? string.Empty : reader["loc_nm"].ToString();
                        e.IsPlan = reader["is_pln"] == DBNull.Value ? true : (bool)reader["is_pln"];
                        
                        e.ResumptionDate = reader["rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rsmptn_dt"];
                        e.CloseRequestDate = reader["cls_rqs_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["cls_rqs_dt"];
                        e.LineManagersResumptionDate = reader["lm_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_rsmptn_dt"];
                        e.LineManagerConfirmResumptionDate = reader["lm_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_confm_dt"];
                        e.LineManagerConfirmResumptionBy = reader["lm_confm_by"] == DBNull.Value ? string.Empty : reader["lm_confm_by"].ToString();
                        e.HrConfirmResumptionDate = reader["hr_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_confm_dt"];
                        e.HrConfirmResumptionBy = reader["hr_confm_by"] == DBNull.Value ? string.Empty : reader["hr_confm_by"].ToString();
                    }
                }
                await conn.CloseAsync();
            }
            return e;
        }
        public async Task<bool> AddAsync(EmployeeLeave e)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.lms_lvs_infs(emp_id, lvs_yr, ");
            sb.Append("lvs_typ_cd, lvs_rsn, lvs_sts, lvs_sdt, lvs_edt, ");
            sb.Append("lvs_dur, unit_id, dept_id, loc_id, is_pln, dur_typ) ");
            sb.Append("VALUES (@emp_id, @lvs_yr, @lvs_typ_cd, @lvs_rsn, ");
            sb.Append("@lvs_sts, @lvs_sdt, @lvs_edt, @lvs_dur, @unit_id, ");
            sb.Append("@dept_id, @loc_id, @is_pln, @dur_typ);");
            string query = sb.ToString();

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var emp_id = cmd.Parameters.Add("emp_id", NpgsqlDbType.Text);
                    var lvs_yr = cmd.Parameters.Add("@lvs_yr", NpgsqlDbType.Integer);
                    var lvs_typ_cd = cmd.Parameters.Add("@lvs_typ_cd", NpgsqlDbType.Text);
                    var lvs_rsn = cmd.Parameters.Add("@lvs_rsn", NpgsqlDbType.Text);
                    var lvs_sts = cmd.Parameters.Add("@lvs_sts", NpgsqlDbType.Text);
                    var lvs_sdt = cmd.Parameters.Add("@lvs_sdt", NpgsqlDbType.TimestampTz);
                    var lvs_edt = cmd.Parameters.Add("@lvs_edt", NpgsqlDbType.TimestampTz);
                    var lvs_dur = cmd.Parameters.Add("@lvs_dur", NpgsqlDbType.Integer);
                    var dur_typ = cmd.Parameters.Add("@dur_typ", NpgsqlDbType.Integer);
                    var unit_id = cmd.Parameters.Add("@unit_id", NpgsqlDbType.Integer);
                    var dept_id = cmd.Parameters.Add("@dept_id", NpgsqlDbType.Integer);
                    var loc_id = cmd.Parameters.Add("@loc_id", NpgsqlDbType.Integer);
                    var is_pln = cmd.Parameters.Add("@is_pln", NpgsqlDbType.Boolean);
                    cmd.Prepare();
                    emp_id.Value = e.EmployeeId;
                    lvs_yr.Value = e.LeaveYear;
                    lvs_typ_cd.Value = e.LeaveTypeCode;
                    lvs_rsn.Value = e.LeaveReason;
                    lvs_sts.Value = e.LeaveStatus;
                    lvs_sdt.Value = e.LeaveStartDate;
                    lvs_edt.Value = e.LeaveEndDate;
                    lvs_dur.Value = e.Duration;
                    dur_typ.Value = e.DurationTypeId;
                    unit_id.Value = e.UnitId;
                    dept_id.Value = e.DepartmentId;
                    loc_id.Value = e.LocationId;
                    is_pln.Value = e.IsPlan;
                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            return rows > 0;
        }
        public async Task<bool> DeleteAsync(long id)
        {
            int rows = 0;
            string query = "DELETE FROM public.lms_lvs_infs WHERE (lvs_inf_id = @lvs_inf_id);";
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
            sb.Append("UPDATE public.lms_lvs_infs SET emp_id=@emp_id, ");
            sb.Append("lvs_yr=@lvs_yr, lvs_typ_cd=@lvs_typ_cd, lvs_rsn=@lvs_rsn, ");
            sb.Append("lvs_sts=@lvs_sts, lvs_sdt=@lvs_sdt, lvs_edt=@lvs_edt, ");
            sb.Append("lvs_dur=@lvs_dur, unit_id=@unit_id, dept_id=@dept_id, ");
            sb.Append("loc_id=@loc_id, is_pln=@is_pln, dur_typ=@dur_typ ");
            sb.Append("WHERE (lvs_inf_id=@lvs_inf_id); ");
            string query = sb.ToString();

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var emp_id = cmd.Parameters.Add("emp_id", NpgsqlDbType.Text);
                    var lvs_yr = cmd.Parameters.Add("@lvs_yr", NpgsqlDbType.Integer);
                    var lvs_typ_cd = cmd.Parameters.Add("@lvs_typ_cd", NpgsqlDbType.Text);
                    var lvs_rsn = cmd.Parameters.Add("@lvs_rsn", NpgsqlDbType.Text);
                    var lvs_sts = cmd.Parameters.Add("@lvs_sts", NpgsqlDbType.Text);
                    var lvs_sdt = cmd.Parameters.Add("@lvs_sdt", NpgsqlDbType.TimestampTz);
                    var lvs_edt = cmd.Parameters.Add("@lvs_edt", NpgsqlDbType.TimestampTz);
                    var lvs_dur = cmd.Parameters.Add("@lvs_dur", NpgsqlDbType.Integer);
                    var dur_typ = cmd.Parameters.Add("@dur_typ", NpgsqlDbType.Integer);
                    var unit_id = cmd.Parameters.Add("@unit_id", NpgsqlDbType.Integer);
                    var dept_id = cmd.Parameters.Add("@dept_id", NpgsqlDbType.Integer);
                    var loc_id = cmd.Parameters.Add("@loc_id", NpgsqlDbType.Integer);
                    var is_pln = cmd.Parameters.Add("@is_pln", NpgsqlDbType.Boolean);
                    cmd.Prepare();
                    emp_id.Value = e.EmployeeId;
                    lvs_yr.Value = e.LeaveYear;
                    lvs_typ_cd.Value = e.LeaveTypeCode;
                    lvs_rsn.Value = e.LeaveReason;
                    lvs_sts.Value = e.LeaveStatus;
                    lvs_sdt.Value = e.LeaveStartDate;
                    lvs_edt.Value = e.LeaveEndDate;
                    lvs_dur.Value = e.Duration;
                    dur_typ.Value = e.DurationTypeId;
                    unit_id.Value = e.UnitId;
                    dept_id.Value = e.DepartmentId;
                    loc_id.Value = e.LocationId;
                    is_pln.Value = e.IsPlan;
                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            return rows > 0;
        }
    }
}
