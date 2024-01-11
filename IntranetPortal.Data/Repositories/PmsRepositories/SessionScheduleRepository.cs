using IntranetPortal.Base.Enums;
using IntranetPortal.Base.Models.EmployeeRecordModels;
using IntranetPortal.Base.Models.PmsModels;
using IntranetPortal.Base.Repositories.PmsRepositories;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Data.Repositories.PmsRepositories
{
    public class SessionScheduleRepository : ISessionScheduleRepository
    {
        public IConfiguration _config { get; }
        public SessionScheduleRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        #region Session Schedule Read Action Methods
        public async Task<IList<SessionSchedule>> GetAllAsync(int reviewSessionId)
        {
            List<SessionSchedule> sessionSchedulesList = new List<SessionSchedule>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.sxn_schd_id, s.rvw_sxn_id, s.rvw_yr_id, s.schd_typ_id, ");
            sb.Append("s.schd_act_typ, s.schd_loc_id, s.schd_dpt_id, s.schd_unt_id, ");
            sb.Append("s.schd_emp_id, s.schd_str_dt, s.schd_ndt_dt, s.schd_ctt, s.schd_ctb, ");
            sb.Append("s.schd_mdt, s.schd_mdb, r.rvw_sxn_nm, y.pms_yr_nm, l.locname, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = s.schd_emp_id) as fullname, ");
            sb.Append("u.unitname, d.deptname, s.schd_isc, s.schd_isc_dt, ");
            sb.Append("s.schd_isc_by, CASE WHEN s.schd_str_dt > CURRENT_TIMESTAMP THEN 'Up Coming' ");
            sb.Append("WHEN CURRENT_TIMESTAMP BETWEEN s.schd_str_dt AND s.schd_ndt_dt THEN 'Current' ");
            sb.Append("WHEN CURRENT_TIMESTAMP > s.schd_ndt_dt THEN 'Elapsed' END schd_status, ");
            sb.Append("CASE s.schd_act_typ WHEN 0 THEN 'All Activities' ");
            sb.Append("WHEN 1 THEN 'Performance Contract Definition Only' ");
            sb.Append("WHEN 2 THEN 'Performance Evaluation Only' END schd_act_typ_ds, ");
            sb.Append("CASE s.schd_typ_id WHEN 0 THEN 'For Everyone' ");
            sb.Append("WHEN 1 THEN 'For a Location' WHEN 2 THEN 'For a Department' ");
            sb.Append("WHEN 3 THEN 'For a Unit' WHEN 4 THEN 'For an Individual' ");
            sb.Append("END schd_typ_ds ");
            sb.Append("FROM public.pmsschdls s ");
            sb.Append("INNER JOIN public.pmsrvwsxns r ON r.rvw_sxn_id = s.rvw_sxn_id ");
            sb.Append("INNER JOIN public.pmssttyrs y ON y.pms_yr_id = s.rvw_yr_id ");
            sb.Append("LEFT JOIN public.gst_locs l ON l.locqk = s.schd_loc_id ");
            sb.Append("LEFT JOIN public.gst_depts d ON d.deptqk = s.schd_dpt_id ");
            sb.Append("LEFT JOIN public.gst_units u ON u.unitqk = s.schd_unt_id ");
            //sb.Append("LEFT JOIN public.erm_emp_inf e ON e.emp_id = s.schd_emp_id ");
            sb.Append("WHERE (s.rvw_sxn_id = @rvw_sxn_id) ");
            sb.Append("ORDER BY s.sxn_schd_id DESC;");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_sxn_id = cmd.Parameters.Add("@rvw_sxn_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                rvw_sxn_id.Value = reviewSessionId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    sessionSchedulesList.Add(new SessionSchedule()
                    {
                        SessionScheduleId = reader["sxn_schd_id"] == DBNull.Value ? 0 : (int)reader["sxn_schd_id"],
                        ReviewSessionId = reader["rvw_sxn_id"] == DBNull.Value ? 0 : (int)reader["rvw_sxn_id"],
                        ReviewSessionName = reader["rvw_sxn_nm"] == DBNull.Value ? string.Empty : reader["rvw_sxn_nm"].ToString(),
                        ReviewYearId = reader["rvw_yr_id"] == DBNull.Value ? 0 : (int)reader["rvw_yr_id"],
                        ReviewYearName = reader["pms_yr_nm"] == DBNull.Value ? string.Empty : reader["pms_yr_nm"].ToString(),
                        ScheduleType = reader["schd_typ_id"] == DBNull.Value ? SessionScheduleType.All : (SessionScheduleType)reader["schd_typ_id"],
                        ScheduleTypeDescription = reader["schd_typ_ds"] == DBNull.Value ? string.Empty : reader["schd_typ_ds"].ToString(),
                        ActivityType = reader["schd_act_typ"] == DBNull.Value ? SessionActivityType.AllActivities : (SessionActivityType)reader["schd_act_typ"],
                        SessionActivityTypeDescription = reader["schd_act_typ_ds"] == DBNull.Value ? string.Empty : reader["schd_act_typ_ds"].ToString(),
                        ScheduleLocationId = reader["schd_loc_id"] == DBNull.Value ? 0 : (int)reader["schd_loc_id"],
                        ScheduleLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                        ScheduleDepartmentId = reader["schd_dpt_id"] == DBNull.Value ? 0 : (int)reader["schd_dpt_id"],
                        ScheduleDepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                        ScheduleUnitId = reader["schd_unt_id"] == DBNull.Value ? 0 : (int)reader["schd_unt_id"],
                        ScheduleUnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                        ScheduleEmployeeId = reader["schd_emp_id"] == DBNull.Value ? string.Empty : reader["schd_emp_id"].ToString(),
                        ScheduleEmployeeName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                        ScheduleStartTime = reader["schd_str_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["schd_str_dt"],
                        ScheduleEndTime = reader["schd_ndt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["schd_ndt_dt"],
                        ScheduleStatus = reader["schd_status"] == DBNull.Value ? string.Empty : reader["schd_status"].ToString(),
                        IsCancelled = reader["schd_isc"] == DBNull.Value ? false : (bool)reader["schd_isc"],
                        CancelledBy = reader["schd_isc_by"] == DBNull.Value ? string.Empty : reader["schd_isc_by"].ToString(),
                        CancelledTime = reader["schd_isc_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["schd_isc_dt"],
                        LastModifiedTime = reader["schd_mdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["schd_mdt"],
                        LastModifiedBy = reader["schd_mdb"] == DBNull.Value ? string.Empty : (reader["schd_mdb"]).ToString(),
                        CreatedTime = reader["schd_ctt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["schd_ctt"],
                        CreatedBy = reader["schd_ctb"] == DBNull.Value ? string.Empty : (reader["schd_ctb"]).ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return sessionSchedulesList;
        }

        public async Task<IList<SessionSchedule>> GetByEmployeeCardinalsAsync(int reviewSessionId, SessionActivityType activityType, EmployeeCardinal employeeCardinal)
        {
            List<SessionSchedule> sessionSchedulesList = new List<SessionSchedule>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.sxn_schd_id, s.rvw_sxn_id, s.rvw_yr_id, s.schd_typ_id, ");
            sb.Append("s.schd_act_typ, s.schd_loc_id, s.schd_dpt_id, s.schd_unt_id, ");
            sb.Append("s.schd_emp_id, s.schd_str_dt, s.schd_ndt_dt, s.schd_ctt, s.schd_ctb, ");
            sb.Append("s.schd_mdt, s.schd_mdb, r.rvw_sxn_nm, y.pms_yr_nm, l.locname, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = s.schd_emp_id) as fullname, ");
            sb.Append("u.unitname, d.deptname, s.schd_isc, s.schd_isc_dt, ");
            sb.Append("s.schd_isc_by, CASE WHEN s.schd_str_dt > CURRENT_TIMESTAMP THEN 'Up Coming' ");
            sb.Append("WHEN CURRENT_TIMESTAMP BETWEEN s.schd_str_dt AND s.schd_ndt_dt THEN 'Current' ");
            sb.Append("WHEN CURRENT_TIMESTAMP > s.schd_ndt_dt THEN 'Elapsed' END schd_status, ");
            sb.Append("CASE s.schd_act_typ WHEN 0 THEN 'All Activities' ");
            sb.Append("WHEN 1 THEN 'Performance Contract Definition Only' ");
            sb.Append("WHEN 2 THEN 'Performance Evaluation Only' END schd_act_typ_ds, ");
            sb.Append("CASE s.schd_typ_id WHEN 0 THEN 'For Everyone' ");
            sb.Append("WHEN 1 THEN 'For a Location' WHEN 2 THEN 'For a Department' ");
            sb.Append("WHEN 3 THEN 'For a Unit' WHEN 4 THEN 'For an Individual' ");
            sb.Append("END schd_typ_ds ");
            sb.Append("FROM public.pmsschdls s ");
            sb.Append("INNER JOIN public.pmsrvwsxns r ON r.rvw_sxn_id = s.rvw_sxn_id ");
            sb.Append("INNER JOIN public.pmssttyrs y ON y.pms_yr_id = s.rvw_yr_id ");
            sb.Append("LEFT JOIN public.gst_locs l ON l.locqk = s.schd_loc_id ");
            sb.Append("LEFT JOIN public.gst_depts d ON d.deptqk = s.schd_dpt_id ");
            sb.Append("LEFT JOIN public.gst_units u ON u.unitqk = s.schd_unt_id ");
            //sb.Append("LEFT JOIN public.erm_emp_inf e ON e.emp_id = s.schd_emp_id ");
            sb.Append("WHERE (@schd_date between s.schd_str_dt and s.schd_ndt_dt) AND (s.schd_isc = false) ");
            sb.Append("AND ((s.schd_typ_id = 0 AND (s.schd_act_typ = @schd_act_typ OR s.schd_act_typ = 0)) ");
            sb.Append("OR (s.schd_typ_id = 1 AND s.schd_loc_id = @schd_loc_id  AND (s.schd_act_typ = @schd_act_typ OR s.schd_act_typ = 0)) ");
            sb.Append("OR (s.schd_typ_id = 2 AND s.schd_dpt_id = @schd_dpt_id  AND (s.schd_act_typ = @schd_act_typ OR s.schd_act_typ = 0)) ");
            sb.Append("OR (s.schd_typ_id = 3 AND s.schd_unt_id = @schd_unt_id  AND (s.schd_act_typ = @schd_act_typ OR s.schd_act_typ = 0)) ");
            sb.Append("OR (s.schd_typ_id = 4 AND s.schd_emp_id = @schd_emp_id  AND (s.schd_act_typ = @schd_act_typ OR s.schd_act_typ = 0))) ");
            sb.Append("ORDER BY s.sxn_schd_id DESC;");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var schd_date = cmd.Parameters.Add("@schd_date", NpgsqlDbType.Timestamp);
                var schd_act_typ = cmd.Parameters.Add("@schd_act_typ", NpgsqlDbType.Integer);
                var schd_loc_id = cmd.Parameters.Add("@schd_loc_id", NpgsqlDbType.Integer);
                var schd_dpt_id = cmd.Parameters.Add("@schd_dpt_id", NpgsqlDbType.Integer);
                var schd_unt_id = cmd.Parameters.Add("@schd_unt_id", NpgsqlDbType.Integer);
                var schd_emp_id = cmd.Parameters.Add("@schd_emp_id", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                schd_date.Value = DateTime.Now;
                schd_act_typ.Value = activityType;
                schd_loc_id.Value = employeeCardinal.EmployeeLocationId;
                schd_dpt_id.Value = employeeCardinal.EmployeeDepartmentId;
                schd_unt_id.Value = employeeCardinal.EmployeeUnitId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    sessionSchedulesList.Add(new SessionSchedule()
                    {
                        SessionScheduleId = reader["sxn_schd_id"] == DBNull.Value ? 0 : (int)reader["sxn_schd_id"],
                        ReviewSessionId = reader["rvw_sxn_id"] == DBNull.Value ? 0 : (int)reader["rvw_sxn_id"],
                        ReviewSessionName = reader["rvw_sxn_nm"] == DBNull.Value ? string.Empty : reader["rvw_sxn_nm"].ToString(),
                        ReviewYearId = reader["rvw_yr_id"] == DBNull.Value ? 0 : (int)reader["rvw_yr_id"],
                        ReviewYearName = reader["pms_yr_nm"] == DBNull.Value ? string.Empty : reader["pms_yr_nm"].ToString(),
                        ScheduleType = reader["schd_typ_id"] == DBNull.Value ? SessionScheduleType.All : (SessionScheduleType)reader["schd_typ_id"],
                        ScheduleTypeDescription = reader["schd_typ_ds"] == DBNull.Value ? string.Empty : reader["schd_typ_ds"].ToString(),
                        ActivityType = reader["schd_act_typ"] == DBNull.Value ? SessionActivityType.AllActivities : (SessionActivityType)reader["schd_act_typ"],
                        SessionActivityTypeDescription = reader["schd_act_typ_ds"] == DBNull.Value ? string.Empty : reader["schd_act_typ_ds"].ToString(),
                        ScheduleLocationId = reader["schd_loc_id"] == DBNull.Value ? 0 : (int)reader["schd_loc_id"],
                        ScheduleLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                        ScheduleDepartmentId = reader["schd_dpt_id"] == DBNull.Value ? 0 : (int)reader["schd_dpt_id"],
                        ScheduleDepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                        ScheduleUnitId = reader["schd_unt_id"] == DBNull.Value ? 0 : (int)reader["schd_unt_id"],
                        ScheduleUnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                        ScheduleEmployeeId = reader["schd_emp_id"] == DBNull.Value ? string.Empty : reader["schd_emp_id"].ToString(),
                        ScheduleEmployeeName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                        ScheduleStartTime = reader["schd_str_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["schd_str_dt"],
                        ScheduleEndTime = reader["schd_ndt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["schd_ndt_dt"],
                        ScheduleStatus = reader["schd_status"] == DBNull.Value ? string.Empty : reader["schd_status"].ToString(),
                        IsCancelled = reader["schd_isc"] == DBNull.Value ? false : (bool)reader["schd_isc"],
                        CancelledBy = reader["schd_isc_by"] == DBNull.Value ? string.Empty : reader["schd_isc_by"].ToString(),
                        CancelledTime = reader["schd_isc_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["schd_isc_dt"],
                        LastModifiedTime = reader["schd_mdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["schd_mdt"],
                        LastModifiedBy = reader["schd_mdb"] == DBNull.Value ? string.Empty : (reader["schd_mdb"]).ToString(),
                        CreatedTime = reader["schd_ctt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["schd_ctt"],
                        CreatedBy = reader["schd_ctb"] == DBNull.Value ? string.Empty : (reader["schd_ctb"]).ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return sessionSchedulesList;
        }

        public async Task<IList<SessionSchedule>> GetByIdAsync(int sessionScheduleId)
        {
            List<SessionSchedule> sessionSchedulesList = new List<SessionSchedule>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.sxn_schd_id, s.rvw_sxn_id, s.rvw_yr_id, s.schd_typ_id, ");
            sb.Append("s.schd_act_typ, s.schd_loc_id, s.schd_dpt_id, s.schd_unt_id, ");
            sb.Append("s.schd_emp_id, s.schd_str_dt, s.schd_ndt_dt, s.schd_ctt, s.schd_ctb, ");
            sb.Append("s.schd_mdt, s.schd_mdb, r.rvw_sxn_nm, y.pms_yr_nm, l.locname, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = s.schd_emp_id) as fullname, ");
            sb.Append("u.unitname, d.deptname, s.schd_isc, s.schd_isc_dt, ");
            sb.Append("s.schd_isc_by, CASE WHEN s.schd_str_dt > CURRENT_TIMESTAMP THEN 'Up Coming' ");
            sb.Append("WHEN CURRENT_TIMESTAMP BETWEEN s.schd_str_dt AND s.schd_ndt_dt THEN 'Current' ");
            sb.Append("WHEN CURRENT_TIMESTAMP > s.schd_ndt_dt THEN 'Elapsed' END schd_status, ");
            sb.Append("CASE s.schd_act_typ WHEN 0 THEN 'All Activities' ");
            sb.Append("WHEN 1 THEN 'Performance Contract Definition Only' ");
            sb.Append("WHEN 2 THEN 'Performance Evaluation Only' END schd_act_typ_ds, ");
            sb.Append("CASE s.schd_typ_id WHEN 0 THEN 'For Everyone' ");
            sb.Append("WHEN 1 THEN 'For a Location' WHEN 2 THEN 'For a Department' ");
            sb.Append("WHEN 3 THEN 'For a Unit' WHEN 4 THEN 'For an Individual' ");
            sb.Append("END schd_typ_ds ");
            sb.Append("FROM public.pmsschdls s ");
            sb.Append("INNER JOIN public.pmsrvwsxns r ON r.rvw_sxn_id = s.rvw_sxn_id ");
            sb.Append("INNER JOIN public.pmssttyrs y ON y.pms_yr_id = s.rvw_yr_id ");
            sb.Append("LEFT JOIN public.gst_locs l ON l.locqk = s.schd_loc_id ");
            sb.Append("LEFT JOIN public.gst_depts d ON d.deptqk = s.schd_dpt_id ");
            sb.Append("LEFT JOIN public.gst_units u ON u.unitqk = s.schd_unt_id ");
            sb.Append("WHERE (s.sxn_schd_id = @sxn_schd_id); ");
            //sb.Append("LEFT JOIN public.erm_emp_inf e ON e.emp_id = s.schd_emp_id ");
            //sb.Append("WHERE (s.rvw_sxn_id = @rvw_sxn_id) ");
            //sb.Append("ORDER BY s.sxn_schd_id DESC;");
           
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var sxn_schd_id = cmd.Parameters.Add("@sxn_schd_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                sxn_schd_id.Value = sessionScheduleId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    sessionSchedulesList.Add(new SessionSchedule()
                    {
                        SessionScheduleId = reader["sxn_schd_id"] == DBNull.Value ? 0 : (int)reader["sxn_schd_id"],
                        ReviewSessionId = reader["rvw_sxn_id"] == DBNull.Value ? 0 : (int)reader["rvw_sxn_id"],
                        ReviewSessionName = reader["rvw_sxn_nm"] == DBNull.Value ? string.Empty : reader["rvw_sxn_nm"].ToString(),
                        ReviewYearId = reader["rvw_yr_id"] == DBNull.Value ? 0 : (int)reader["rvw_yr_id"],
                        ReviewYearName = reader["pms_yr_nm"] == DBNull.Value ? string.Empty : reader["pms_yr_nm"].ToString(),
                        ScheduleType = reader["schd_typ_id"] == DBNull.Value ? SessionScheduleType.All : (SessionScheduleType)reader["schd_typ_id"],
                        ScheduleTypeDescription = reader["schd_typ_ds"] == DBNull.Value ? string.Empty : reader["schd_typ_ds"].ToString(),
                        ActivityType = reader["schd_act_typ"] == DBNull.Value ? SessionActivityType.AllActivities : (SessionActivityType)reader["schd_act_typ"],
                        SessionActivityTypeDescription = reader["schd_act_typ_ds"] == DBNull.Value ? string.Empty : reader["schd_act_typ_ds"].ToString(),
                        ScheduleLocationId = reader["schd_loc_id"] == DBNull.Value ? 0 : (int)reader["schd_loc_id"],
                        ScheduleLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                        ScheduleDepartmentId = reader["schd_dpt_id"] == DBNull.Value ? 0 : (int)reader["schd_dpt_id"],
                        ScheduleDepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                        ScheduleUnitId = reader["schd_unt_id"] == DBNull.Value ? 0 : (int)reader["schd_unt_id"],
                        ScheduleUnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                        ScheduleEmployeeId = reader["schd_emp_id"] == DBNull.Value ? string.Empty : reader["schd_emp_id"].ToString(),
                        ScheduleEmployeeName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                        ScheduleStartTime = reader["schd_str_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["schd_str_dt"],
                        ScheduleEndTime = reader["schd_ndt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["schd_ndt_dt"],
                        ScheduleStatus = reader["schd_status"] == DBNull.Value ? string.Empty : reader["schd_status"].ToString(),
                        IsCancelled = reader["schd_isc"] == DBNull.Value ? false : (bool)reader["schd_isc"],
                        CancelledBy = reader["schd_isc_by"] == DBNull.Value ? string.Empty : reader["schd_isc_by"].ToString(),
                        CancelledTime = reader["schd_isc_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["schd_isc_dt"],
                        LastModifiedTime = reader["schd_mdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["schd_mdt"],
                        LastModifiedBy = reader["schd_mdb"] == DBNull.Value ? string.Empty : (reader["schd_mdb"]).ToString(),
                        CreatedTime = reader["schd_ctt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["schd_ctt"],
                        CreatedBy = reader["schd_ctb"] == DBNull.Value ? string.Empty : (reader["schd_ctb"]).ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return sessionSchedulesList;
        }

        public async Task<IList<SessionSchedule>> GetByTypeAsync(int reviewSessionId, SessionScheduleType scheduleType)
        {
            List<SessionSchedule> sessionSchedulesList = new List<SessionSchedule>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.sxn_schd_id, s.rvw_sxn_id, s.rvw_yr_id, s.schd_typ_id, ");
            sb.Append("s.schd_act_typ, s.schd_loc_id, s.schd_dpt_id, s.schd_unt_id, ");
            sb.Append("s.schd_emp_id, s.schd_str_dt, s.schd_ndt_dt, s.schd_ctt, s.schd_ctb, ");
            sb.Append("s.schd_mdt, s.schd_mdb, r.rvw_sxn_nm, y.pms_yr_nm, l.locname, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = s.schd_emp_id) as fullname, ");
            sb.Append("u.unitname, d.deptname, s.schd_isc, s.schd_isc_dt, ");
            sb.Append("s.schd_isc_by, CASE WHEN s.schd_str_dt > CURRENT_TIMESTAMP THEN 'Up Coming' ");
            sb.Append("WHEN CURRENT_TIMESTAMP BETWEEN s.schd_str_dt AND s.schd_ndt_dt THEN 'Current' ");
            sb.Append("WHEN CURRENT_TIMESTAMP > s.schd_ndt_dt THEN 'Elapsed' END schd_status, ");
            sb.Append("CASE s.schd_act_typ WHEN 0 THEN 'All Activities' ");
            sb.Append("WHEN 1 THEN 'Performance Contract Definition Only' ");
            sb.Append("WHEN 2 THEN 'Performance Evaluation Only' END schd_act_typ_ds, ");
            sb.Append("CASE s.schd_typ_id WHEN 0 THEN 'For Everyone' ");
            sb.Append("WHEN 1 THEN 'For a Location' WHEN 2 THEN 'For a Department' ");
            sb.Append("WHEN 3 THEN 'For a Unit' WHEN 4 THEN 'For an Individual' ");
            sb.Append("END schd_typ_ds ");
            sb.Append("FROM public.pmsschdls s ");
            sb.Append("INNER JOIN public.pmsrvwsxns r ON r.rvw_sxn_id = s.rvw_sxn_id ");
            sb.Append("INNER JOIN public.pmssttyrs y ON y.pms_yr_id = s.rvw_yr_id ");
            sb.Append("LEFT JOIN public.gst_locs l ON l.locqk = s.schd_loc_id ");
            sb.Append("LEFT JOIN public.gst_depts d ON d.deptqk = s.schd_dpt_id ");
            sb.Append("LEFT JOIN public.gst_units u ON u.unitqk = s.schd_unt_id ");
            //sb.Append("LEFT JOIN public.erm_emp_inf e ON e.emp_id = s.schd_emp_id ");
            sb.Append("WHERE (s.rvw_sxn_id = @rvw_sxn_id) ");
            sb.Append("AND (s.schd_typ_id = @schd_typ_id) ");
            sb.Append("ORDER BY s.sxn_schd_id DESC;");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_sxn_id = cmd.Parameters.Add("@rvw_sxn_id", NpgsqlDbType.Integer);
                var schd_typ_id = cmd.Parameters.Add("@schd_typ_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                rvw_sxn_id.Value = reviewSessionId;
                schd_typ_id.Value = scheduleType;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    sessionSchedulesList.Add(new SessionSchedule()
                    {
                        SessionScheduleId = reader["sxn_schd_id"] == DBNull.Value ? 0 : (int)reader["sxn_schd_id"],
                        ReviewSessionId = reader["rvw_sxn_id"] == DBNull.Value ? 0 : (int)reader["rvw_sxn_id"],
                        ReviewSessionName = reader["rvw_sxn_nm"] == DBNull.Value ? string.Empty : reader["rvw_sxn_nm"].ToString(),
                        ReviewYearId = reader["rvw_yr_id"] == DBNull.Value ? 0 : (int)reader["rvw_yr_id"],
                        ReviewYearName = reader["pms_yr_nm"] == DBNull.Value ? string.Empty : reader["pms_yr_nm"].ToString(),
                        ScheduleType = reader["schd_typ_id"] == DBNull.Value ? SessionScheduleType.All : (SessionScheduleType)reader["schd_typ_id"],
                        ScheduleTypeDescription = reader["schd_typ_ds"] == DBNull.Value ? string.Empty : reader["schd_typ_ds"].ToString(),
                        ActivityType = reader["schd_act_typ"] == DBNull.Value ? SessionActivityType.AllActivities : (SessionActivityType)reader["schd_act_typ"],
                        SessionActivityTypeDescription = reader["schd_act_typ_ds"] == DBNull.Value ? string.Empty : reader["schd_act_typ_ds"].ToString(),
                        ScheduleLocationId = reader["schd_loc_id"] == DBNull.Value ? 0 : (int)reader["schd_loc_id"],
                        ScheduleLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                        ScheduleDepartmentId = reader["schd_dpt_id"] == DBNull.Value ? 0 : (int)reader["schd_dpt_id"],
                        ScheduleDepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                        ScheduleUnitId = reader["schd_unt_id"] == DBNull.Value ? 0 : (int)reader["schd_unt_id"],
                        ScheduleUnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                        ScheduleEmployeeId = reader["schd_emp_id"] == DBNull.Value ? string.Empty : reader["schd_emp_id"].ToString(),
                        ScheduleEmployeeName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                        ScheduleStartTime = reader["schd_str_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["schd_str_dt"],
                        ScheduleEndTime = reader["schd_ndt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["schd_ndt_dt"],
                        ScheduleStatus = reader["schd_status"] == DBNull.Value ? string.Empty : reader["schd_status"].ToString(),
                        IsCancelled = reader["schd_isc"] == DBNull.Value ? false : (bool)reader["schd_isc"],
                        CancelledBy = reader["schd_isc_by"] == DBNull.Value ? string.Empty : reader["schd_isc_by"].ToString(),
                        CancelledTime = reader["schd_isc_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["schd_isc_dt"],
                        LastModifiedTime = reader["schd_mdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["schd_mdt"],
                        LastModifiedBy = reader["schd_mdb"] == DBNull.Value ? string.Empty : (reader["schd_mdb"]).ToString(),
                        CreatedTime = reader["schd_ctt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["schd_ctt"],
                        CreatedBy = reader["schd_ctb"] == DBNull.Value ? string.Empty : (reader["schd_ctb"]).ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return sessionSchedulesList;
        }

        public async Task<IList<SessionSchedule>> GetByReviewSessionIdAsync(int reviewSessionId)
        {
            List<SessionSchedule> sessionSchedulesList = new List<SessionSchedule>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.sxn_schd_id, s.rvw_sxn_id, s.rvw_yr_id, s.schd_typ_id, ");
            sb.Append("s.schd_act_typ, s.schd_loc_id, s.schd_dpt_id, s.schd_unt_id, ");
            sb.Append("s.schd_emp_id, s.schd_str_dt, s.schd_ndt_dt, s.schd_ctt, s.schd_ctb, ");
            sb.Append("s.schd_mdt, s.schd_mdb, r.rvw_sxn_nm, y.pms_yr_nm, l.locname, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = s.schd_emp_id) as fullname, ");
            sb.Append("u.unitname, d.deptname, s.schd_isc, s.schd_isc_dt, ");
            sb.Append("s.schd_isc_by, CASE WHEN s.schd_str_dt > CURRENT_TIMESTAMP THEN 'Up Coming' ");
            sb.Append("WHEN CURRENT_TIMESTAMP BETWEEN s.schd_str_dt AND s.schd_ndt_dt THEN 'Current' ");
            sb.Append("WHEN CURRENT_TIMESTAMP > s.schd_ndt_dt THEN 'Elapsed' END schd_status, ");
            sb.Append("CASE s.schd_act_typ WHEN 0 THEN 'All Activities' ");
            sb.Append("WHEN 1 THEN 'Performance Contract Definition Only' ");
            sb.Append("WHEN 2 THEN 'Performance Evaluation Only' END schd_act_typ_ds, ");
            sb.Append("CASE s.schd_typ_id WHEN 0 THEN 'For Everyone' ");
            sb.Append("WHEN 1 THEN 'For a Location' WHEN 2 THEN 'For a Department' ");
            sb.Append("WHEN 3 THEN 'For a Unit' WHEN 4 THEN 'For an Individual' ");
            sb.Append("END schd_typ_ds ");
            sb.Append("FROM public.pmsschdls s ");
            sb.Append("INNER JOIN public.pmsrvwsxns r ON r.rvw_sxn_id = s.rvw_sxn_id ");
            sb.Append("INNER JOIN public.pmssttyrs y ON y.pms_yr_id = s.rvw_yr_id ");
            sb.Append("LEFT JOIN public.gst_locs l ON l.locqk = s.schd_loc_id ");
            sb.Append("LEFT JOIN public.gst_depts d ON d.deptqk = s.schd_dpt_id ");
            sb.Append("LEFT JOIN public.gst_units u ON u.unitqk = s.schd_unt_id ");
            //sb.Append("LEFT JOIN public.erm_emp_inf e ON e.emp_id = s.schd_emp_id ");
            sb.Append("WHERE (s.rvw_sxn_id = @rvw_sxn_id) ");
            sb.Append("ORDER BY s.sxn_schd_id DESC;");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_sxn_id = cmd.Parameters.Add("@rvw_sxn_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                rvw_sxn_id.Value = reviewSessionId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    sessionSchedulesList.Add(new SessionSchedule()
                    {
                        SessionScheduleId = reader["sxn_schd_id"] == DBNull.Value ? 0 : (int)reader["sxn_schd_id"],
                        ReviewSessionId = reader["rvw_sxn_id"] == DBNull.Value ? 0 : (int)reader["rvw_sxn_id"],
                        ReviewSessionName = reader["rvw_sxn_nm"] == DBNull.Value ? string.Empty : reader["rvw_sxn_nm"].ToString(),
                        ReviewYearId = reader["rvw_yr_id"] == DBNull.Value ? 0 : (int)reader["rvw_yr_id"],
                        ReviewYearName = reader["pms_yr_nm"] == DBNull.Value ? string.Empty : reader["pms_yr_nm"].ToString(),
                        ScheduleType = reader["schd_typ_id"] == DBNull.Value ? SessionScheduleType.All : (SessionScheduleType)reader["schd_typ_id"],
                        ScheduleTypeDescription = reader["schd_typ_ds"] == DBNull.Value ? string.Empty : reader["schd_typ_ds"].ToString(),
                        ActivityType = reader["schd_act_typ"] == DBNull.Value ? SessionActivityType.AllActivities : (SessionActivityType)reader["schd_act_typ"],
                        SessionActivityTypeDescription = reader["schd_act_typ_ds"] == DBNull.Value ? string.Empty : reader["schd_act_typ_ds"].ToString(),
                        ScheduleLocationId = reader["schd_loc_id"] == DBNull.Value ? 0 : (int)reader["schd_loc_id"],
                        ScheduleLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                        ScheduleDepartmentId = reader["schd_dpt_id"] == DBNull.Value ? 0 : (int)reader["schd_dpt_id"],
                        ScheduleDepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                        ScheduleUnitId = reader["schd_unt_id"] == DBNull.Value ? 0 : (int)reader["schd_unt_id"],
                        ScheduleUnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                        ScheduleEmployeeId = reader["schd_emp_id"] == DBNull.Value ? string.Empty : reader["schd_emp_id"].ToString(),
                        ScheduleEmployeeName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                        ScheduleStartTime = reader["schd_str_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["schd_str_dt"],
                        ScheduleEndTime = reader["schd_ndt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["schd_ndt_dt"],
                        ScheduleStatus = reader["schd_status"] == DBNull.Value ? string.Empty : reader["schd_status"].ToString(),
                        IsCancelled = reader["schd_isc"] == DBNull.Value ? false : (bool)reader["schd_isc"],
                        CancelledBy = reader["schd_isc_by"] == DBNull.Value ? string.Empty : reader["schd_isc_by"].ToString(),
                        CancelledTime = reader["schd_isc_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["schd_isc_dt"],
                        LastModifiedTime = reader["schd_mdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["schd_mdt"],
                        LastModifiedBy = reader["schd_mdb"] == DBNull.Value ? string.Empty : (reader["schd_mdb"]).ToString(),
                        CreatedTime = reader["schd_ctt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["schd_ctt"],
                        CreatedBy = reader["schd_ctb"] == DBNull.Value ? string.Empty : (reader["schd_ctb"]).ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return sessionSchedulesList;
        }

        public async Task<IList<SessionSchedule>> GetByReviewSessionIdAsync(int reviewSessionId, SessionActivityType activityType)
        {
            List<SessionSchedule> sessionSchedulesList = new List<SessionSchedule>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.sxn_schd_id, s.rvw_sxn_id, s.rvw_yr_id, s.schd_typ_id, ");
            sb.Append("s.schd_act_typ, s.schd_loc_id, s.schd_dpt_id, s.schd_unt_id, ");
            sb.Append("s.schd_emp_id, s.schd_str_dt, s.schd_ndt_dt, s.schd_ctt, s.schd_ctb, ");
            sb.Append("s.schd_mdt, s.schd_mdb, r.rvw_sxn_nm, y.pms_yr_nm, l.locname, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = s.schd_emp_id) as fullname, ");
            sb.Append("u.unitname, d.deptname, s.schd_isc, s.schd_isc_dt, ");
            sb.Append("s.schd_isc_by, CASE WHEN s.schd_str_dt > CURRENT_TIMESTAMP THEN 'Up Coming' ");
            sb.Append("WHEN CURRENT_TIMESTAMP BETWEEN s.schd_str_dt AND s.schd_ndt_dt THEN 'Current' ");
            sb.Append("WHEN CURRENT_TIMESTAMP > s.schd_ndt_dt THEN 'Elapsed' END schd_status, ");
            sb.Append("CASE s.schd_act_typ WHEN 0 THEN 'All Activities' ");
            sb.Append("WHEN 1 THEN 'Performance Contract Definition Only' ");
            sb.Append("WHEN 2 THEN 'Performance Evaluation Only' END schd_act_typ_ds, ");
            sb.Append("CASE s.schd_typ_id WHEN 0 THEN 'For Everyone' ");
            sb.Append("WHEN 1 THEN 'For a Location' WHEN 2 THEN 'For a Department' ");
            sb.Append("WHEN 3 THEN 'For a Unit' WHEN 4 THEN 'For an Individual' ");
            sb.Append("END schd_typ_ds ");
            sb.Append("FROM public.pmsschdls s ");
            sb.Append("INNER JOIN public.pmsrvwsxns r ON r.rvw_sxn_id = s.rvw_sxn_id ");
            sb.Append("INNER JOIN public.pmssttyrs y ON y.pms_yr_id = s.rvw_yr_id ");
            sb.Append("LEFT JOIN public.gst_locs l ON l.locqk = s.schd_loc_id ");
            sb.Append("LEFT JOIN public.gst_depts d ON d.deptqk = s.schd_dpt_id ");
            sb.Append("LEFT JOIN public.gst_units u ON u.unitqk = s.schd_unt_id ");
            //sb.Append("LEFT JOIN public.erm_emp_inf e ON e.emp_id = s.schd_emp_id ");
            sb.Append("WHERE (s.rvw_sxn_id = @rvw_sxn_id) ");
            sb.Append("AND (s.schd_act_typ = @schd_act_typ) ");
            sb.Append("ORDER BY s.sxn_schd_id DESC;");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_sxn_id = cmd.Parameters.Add("@rvw_sxn_id", NpgsqlDbType.Integer);
                var schd_act_typ = cmd.Parameters.Add("@schd_act_typ", NpgsqlDbType.Integer);
                var schd_date = cmd.Parameters.Add("@schd_date", NpgsqlDbType.Timestamp);
                await cmd.PrepareAsync();
                rvw_sxn_id.Value = reviewSessionId;
                schd_act_typ.Value = activityType;
                schd_date.Value = DateTime.Now;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    sessionSchedulesList.Add(new SessionSchedule()
                    {
                        SessionScheduleId = reader["sxn_schd_id"] == DBNull.Value ? 0 : (int)reader["sxn_schd_id"],
                        ReviewSessionId = reader["rvw_sxn_id"] == DBNull.Value ? 0 : (int)reader["rvw_sxn_id"],
                        ReviewSessionName = reader["rvw_sxn_nm"] == DBNull.Value ? string.Empty : reader["rvw_sxn_nm"].ToString(),
                        ReviewYearId = reader["rvw_yr_id"] == DBNull.Value ? 0 : (int)reader["rvw_yr_id"],
                        ReviewYearName = reader["pms_yr_nm"] == DBNull.Value ? string.Empty : reader["pms_yr_nm"].ToString(),
                        ScheduleType = reader["schd_typ_id"] == DBNull.Value ? SessionScheduleType.All : (SessionScheduleType)reader["schd_typ_id"],
                        ScheduleTypeDescription = reader["schd_typ_ds"] == DBNull.Value ? string.Empty : reader["schd_typ_ds"].ToString(),
                        ActivityType = reader["schd_act_typ"] == DBNull.Value ? SessionActivityType.AllActivities : (SessionActivityType)reader["schd_act_typ"],
                        SessionActivityTypeDescription = reader["schd_act_typ_ds"] == DBNull.Value ? string.Empty : reader["schd_act_typ_ds"].ToString(),
                        ScheduleLocationId = reader["schd_loc_id"] == DBNull.Value ? 0 : (int)reader["schd_loc_id"],
                        ScheduleLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                        ScheduleDepartmentId = reader["schd_dpt_id"] == DBNull.Value ? 0 : (int)reader["schd_dpt_id"],
                        ScheduleDepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                        ScheduleUnitId = reader["schd_unt_id"] == DBNull.Value ? 0 : (int)reader["schd_unt_id"],
                        ScheduleUnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                        ScheduleEmployeeId = reader["schd_emp_id"] == DBNull.Value ? string.Empty : reader["schd_emp_id"].ToString(),
                        ScheduleEmployeeName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                        ScheduleStartTime = reader["schd_str_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["schd_str_dt"],
                        ScheduleEndTime = reader["schd_ndt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["schd_ndt_dt"],
                        ScheduleStatus = reader["schd_status"] == DBNull.Value ? string.Empty : reader["schd_status"].ToString(),
                        IsCancelled = reader["schd_isc"] == DBNull.Value ? false : (bool)reader["schd_isc"],
                        CancelledBy = reader["schd_isc_by"] == DBNull.Value ? string.Empty : reader["schd_isc_by"].ToString(),
                        CancelledTime = reader["schd_isc_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["schd_isc_dt"],
                        LastModifiedTime = reader["schd_mdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["schd_mdt"],
                        LastModifiedBy = reader["schd_mdb"] == DBNull.Value ? string.Empty : (reader["schd_mdb"]).ToString(),
                        CreatedTime = reader["schd_ctt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["schd_ctt"],
                        CreatedBy = reader["schd_ctb"] == DBNull.Value ? string.Empty : (reader["schd_ctb"]).ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return sessionSchedulesList;
        }

        public async Task<IList<SessionSchedule>> GetByLocationIdAsync(int reviewSessionId, int locationId)
        {
            List<SessionSchedule> sessionSchedulesList = new List<SessionSchedule>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.sxn_schd_id, s.rvw_sxn_id, s.rvw_yr_id, s.schd_typ_id, ");
            sb.Append("s.schd_act_typ, s.schd_loc_id, s.schd_dpt_id, s.schd_unt_id, ");
            sb.Append("s.schd_emp_id, s.schd_str_dt, s.schd_ndt_dt, s.schd_ctt, s.schd_ctb, ");
            sb.Append("s.schd_mdt, s.schd_mdb, r.rvw_sxn_nm, y.pms_yr_nm, l.locname, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = s.schd_emp_id) as fullname, ");
            sb.Append("u.unitname, d.deptname, s.schd_isc, s.schd_isc_dt, ");
            sb.Append("s.schd_isc_by, CASE WHEN s.schd_str_dt > CURRENT_TIMESTAMP THEN 'Up Coming' ");
            sb.Append("WHEN CURRENT_TIMESTAMP BETWEEN s.schd_str_dt AND s.schd_ndt_dt THEN 'Current' ");
            sb.Append("WHEN CURRENT_TIMESTAMP > s.schd_ndt_dt THEN 'Elapsed' END schd_status, ");
            sb.Append("CASE s.schd_act_typ WHEN 0 THEN 'All Activities' ");
            sb.Append("WHEN 1 THEN 'Performance Contract Definition Only' ");
            sb.Append("WHEN 2 THEN 'Performance Evaluation Only' END schd_act_typ_ds, ");
            sb.Append("CASE s.schd_typ_id WHEN 0 THEN 'For Everyone' ");
            sb.Append("WHEN 1 THEN 'For a Location' WHEN 2 THEN 'For a Department' ");
            sb.Append("WHEN 3 THEN 'For a Unit' WHEN 4 THEN 'For an Individual' ");
            sb.Append("END schd_typ_ds ");
            sb.Append("FROM public.pmsschdls s ");
            sb.Append("INNER JOIN public.pmsrvwsxns r ON r.rvw_sxn_id = s.rvw_sxn_id ");
            sb.Append("INNER JOIN public.pmssttyrs y ON y.pms_yr_id = s.rvw_yr_id ");
            sb.Append("LEFT JOIN public.gst_locs l ON l.locqk = s.schd_loc_id ");
            sb.Append("LEFT JOIN public.gst_depts d ON d.deptqk = s.schd_dpt_id ");
            sb.Append("LEFT JOIN public.gst_units u ON u.unitqk = s.schd_unt_id ");
            //sb.Append("LEFT JOIN public.erm_emp_inf e ON e.emp_id = s.schd_emp_id ");
            sb.Append("WHERE (s.rvw_sxn_id = @rvw_sxn_id) ");
            sb.Append("AND (s.schd_loc_id = @schd_loc_id) ");
            sb.Append("ORDER BY s.sxn_schd_id DESC;");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_sxn_id = cmd.Parameters.Add("@rvw_sxn_id", NpgsqlDbType.Integer);
                var schd_loc_id = cmd.Parameters.Add("@schd_loc_id", NpgsqlDbType.Integer);
                var schd_date = cmd.Parameters.Add("@schd_date", NpgsqlDbType.Timestamp);
                await cmd.PrepareAsync();
                rvw_sxn_id.Value = reviewSessionId;
                schd_loc_id.Value = locationId;
                schd_date.Value = DateTime.Now;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    sessionSchedulesList.Add(new SessionSchedule()
                    {
                        SessionScheduleId = reader["sxn_schd_id"] == DBNull.Value ? 0 : (int)reader["sxn_schd_id"],
                        ReviewSessionId = reader["rvw_sxn_id"] == DBNull.Value ? 0 : (int)reader["rvw_sxn_id"],
                        ReviewSessionName = reader["rvw_sxn_nm"] == DBNull.Value ? string.Empty : reader["rvw_sxn_nm"].ToString(),
                        ReviewYearId = reader["rvw_yr_id"] == DBNull.Value ? 0 : (int)reader["rvw_yr_id"],
                        ReviewYearName = reader["pms_yr_nm"] == DBNull.Value ? string.Empty : reader["pms_yr_nm"].ToString(),
                        ScheduleType = reader["schd_typ_id"] == DBNull.Value ? SessionScheduleType.All : (SessionScheduleType)reader["schd_typ_id"],
                        ScheduleTypeDescription = reader["schd_typ_ds"] == DBNull.Value ? string.Empty : reader["schd_typ_ds"].ToString(),
                        ActivityType = reader["schd_act_typ"] == DBNull.Value ? SessionActivityType.AllActivities : (SessionActivityType)reader["schd_act_typ"],
                        SessionActivityTypeDescription = reader["schd_act_typ_ds"] == DBNull.Value ? string.Empty : reader["schd_act_typ_ds"].ToString(),
                        ScheduleLocationId = reader["schd_loc_id"] == DBNull.Value ? 0 : (int)reader["schd_loc_id"],
                        ScheduleLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                        ScheduleDepartmentId = reader["schd_dpt_id"] == DBNull.Value ? 0 : (int)reader["schd_dpt_id"],
                        ScheduleDepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                        ScheduleUnitId = reader["schd_unt_id"] == DBNull.Value ? 0 : (int)reader["schd_unt_id"],
                        ScheduleUnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                        ScheduleEmployeeId = reader["schd_emp_id"] == DBNull.Value ? string.Empty : reader["schd_emp_id"].ToString(),
                        ScheduleEmployeeName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                        ScheduleStartTime = reader["schd_str_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["schd_str_dt"],
                        ScheduleEndTime = reader["schd_ndt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["schd_ndt_dt"],
                        ScheduleStatus = reader["schd_status"] == DBNull.Value ? string.Empty : reader["schd_status"].ToString(),
                        IsCancelled = reader["schd_isc"] == DBNull.Value ? false : (bool)reader["schd_isc"],
                        CancelledBy = reader["schd_isc_by"] == DBNull.Value ? string.Empty : reader["schd_isc_by"].ToString(),
                        CancelledTime = reader["schd_isc_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["schd_isc_dt"],
                        LastModifiedTime = reader["schd_mdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["schd_mdt"],
                        LastModifiedBy = reader["schd_mdb"] == DBNull.Value ? string.Empty : (reader["schd_mdb"]).ToString(),
                        CreatedTime = reader["schd_ctt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["schd_ctt"],
                        CreatedBy = reader["schd_ctb"] == DBNull.Value ? string.Empty : (reader["schd_ctb"]).ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return sessionSchedulesList;
        }

        public async Task<IList<SessionSchedule>> GetByLocationIdAsync(int reviewSessionId, int locationId, SessionActivityType activityType)
        {
            List<SessionSchedule> sessionSchedulesList = new List<SessionSchedule>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.sxn_schd_id, s.rvw_sxn_id, s.rvw_yr_id, s.schd_typ_id, ");
            sb.Append("s.schd_act_typ, s.schd_loc_id, s.schd_dpt_id, s.schd_unt_id, ");
            sb.Append("s.schd_emp_id, s.schd_str_dt, s.schd_ndt_dt, s.schd_ctt, s.schd_ctb, ");
            sb.Append("s.schd_mdt, s.schd_mdb, r.rvw_sxn_nm, y.pms_yr_nm, l.locname, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = s.schd_emp_id) as fullname, ");
            sb.Append("u.unitname, d.deptname, s.schd_isc, s.schd_isc_dt, ");
            sb.Append("s.schd_isc_by, CASE WHEN s.schd_str_dt > CURRENT_TIMESTAMP THEN 'Up Coming' ");
            sb.Append("WHEN CURRENT_TIMESTAMP BETWEEN s.schd_str_dt AND s.schd_ndt_dt THEN 'Current' ");
            sb.Append("WHEN CURRENT_TIMESTAMP > s.schd_ndt_dt THEN 'Elapsed' END schd_status, ");
            sb.Append("CASE s.schd_act_typ WHEN 0 THEN 'All Activities' ");
            sb.Append("WHEN 1 THEN 'Performance Contract Definition Only' ");
            sb.Append("WHEN 2 THEN 'Performance Evaluation Only' END schd_act_typ_ds, ");
            sb.Append("CASE s.schd_typ_id WHEN 0 THEN 'For Everyone' ");
            sb.Append("WHEN 1 THEN 'For a Location' WHEN 2 THEN 'For a Department' ");
            sb.Append("WHEN 3 THEN 'For a Unit' WHEN 4 THEN 'For an Individual' ");
            sb.Append("END schd_typ_ds ");
            sb.Append("FROM public.pmsschdls s ");
            sb.Append("INNER JOIN public.pmsrvwsxns r ON r.rvw_sxn_id = s.rvw_sxn_id ");
            sb.Append("INNER JOIN public.pmssttyrs y ON y.pms_yr_id = s.rvw_yr_id ");
            sb.Append("LEFT JOIN public.gst_locs l ON l.locqk = s.schd_loc_id ");
            sb.Append("LEFT JOIN public.gst_depts d ON d.deptqk = s.schd_dpt_id ");
            sb.Append("LEFT JOIN public.gst_units u ON u.unitqk = s.schd_unt_id ");
            //sb.Append("LEFT JOIN public.erm_emp_inf e ON e.emp_id = s.schd_emp_id ");
            sb.Append("WHERE (s.rvw_sxn_id = @rvw_sxn_id) ");
            sb.Append("AND (s.schd_loc_id = @schd_loc_id) ");
            sb.Append("AND (s.schd_act_typ = @schd_act_typ) ");
            sb.Append("ORDER BY s.sxn_schd_id DESC;");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_sxn_id = cmd.Parameters.Add("@rvw_sxn_id", NpgsqlDbType.Integer);
                var schd_loc_id = cmd.Parameters.Add("@schd_loc_id", NpgsqlDbType.Integer);
                var schd_act_typ = cmd.Parameters.Add("@schd_act_typ", NpgsqlDbType.Integer);
                var schd_date = cmd.Parameters.Add("@schd_date", NpgsqlDbType.Timestamp);
                await cmd.PrepareAsync();
                rvw_sxn_id.Value = reviewSessionId;
                schd_loc_id.Value = locationId;
                schd_act_typ.Value = activityType;
                schd_date.Value = DateTime.Now;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    sessionSchedulesList.Add(new SessionSchedule()
                    {
                        SessionScheduleId = reader["sxn_schd_id"] == DBNull.Value ? 0 : (int)reader["sxn_schd_id"],
                        ReviewSessionId = reader["rvw_sxn_id"] == DBNull.Value ? 0 : (int)reader["rvw_sxn_id"],
                        ReviewSessionName = reader["rvw_sxn_nm"] == DBNull.Value ? string.Empty : reader["rvw_sxn_nm"].ToString(),
                        ReviewYearId = reader["rvw_yr_id"] == DBNull.Value ? 0 : (int)reader["rvw_yr_id"],
                        ReviewYearName = reader["pms_yr_nm"] == DBNull.Value ? string.Empty : reader["pms_yr_nm"].ToString(),
                        ScheduleType = reader["schd_typ_id"] == DBNull.Value ? SessionScheduleType.All : (SessionScheduleType)reader["schd_typ_id"],
                        ScheduleTypeDescription = reader["schd_typ_ds"] == DBNull.Value ? string.Empty : reader["schd_typ_ds"].ToString(),
                        ActivityType = reader["schd_act_typ"] == DBNull.Value ? SessionActivityType.AllActivities : (SessionActivityType)reader["schd_act_typ"],
                        SessionActivityTypeDescription = reader["schd_act_typ_ds"] == DBNull.Value ? string.Empty : reader["schd_act_typ_ds"].ToString(),
                        ScheduleLocationId = reader["schd_loc_id"] == DBNull.Value ? 0 : (int)reader["schd_loc_id"],
                        ScheduleLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                        ScheduleDepartmentId = reader["schd_dpt_id"] == DBNull.Value ? 0 : (int)reader["schd_dpt_id"],
                        ScheduleDepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                        ScheduleUnitId = reader["schd_unt_id"] == DBNull.Value ? 0 : (int)reader["schd_unt_id"],
                        ScheduleUnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                        ScheduleEmployeeId = reader["schd_emp_id"] == DBNull.Value ? string.Empty : reader["schd_emp_id"].ToString(),
                        ScheduleEmployeeName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                        ScheduleStartTime = reader["schd_str_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["schd_str_dt"],
                        ScheduleEndTime = reader["schd_ndt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["schd_ndt_dt"],
                        ScheduleStatus = reader["schd_status"] == DBNull.Value ? string.Empty : reader["schd_status"].ToString(),
                        IsCancelled = reader["schd_isc"] == DBNull.Value ? false : (bool)reader["schd_isc"],
                        CancelledBy = reader["schd_isc_by"] == DBNull.Value ? string.Empty : reader["schd_isc_by"].ToString(),
                        CancelledTime = reader["schd_isc_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["schd_isc_dt"],
                        LastModifiedTime = reader["schd_mdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["schd_mdt"],
                        LastModifiedBy = reader["schd_mdb"] == DBNull.Value ? string.Empty : (reader["schd_mdb"]).ToString(),
                        CreatedTime = reader["schd_ctt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["schd_ctt"],
                        CreatedBy = reader["schd_ctb"] == DBNull.Value ? string.Empty : (reader["schd_ctb"]).ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return sessionSchedulesList;
        }

        public async Task<IList<SessionSchedule>> GetByDepartmentCodeAsync(int reviewSessionId, int departmentId)
        {
            List<SessionSchedule> sessionSchedulesList = new List<SessionSchedule>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.sxn_schd_id, s.rvw_sxn_id, s.rvw_yr_id, s.schd_typ_id, ");
            sb.Append("s.schd_act_typ, s.schd_loc_id, s.schd_dpt_id, s.schd_unt_id, ");
            sb.Append("s.schd_emp_id, s.schd_str_dt, s.schd_ndt_dt, s.schd_ctt, s.schd_ctb, ");
            sb.Append("s.schd_mdt, s.schd_mdb, r.rvw_sxn_nm, y.pms_yr_nm, l.locname, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = s.schd_emp_id) as fullname, ");
            sb.Append("u.unitname, d.deptname, s.schd_isc, s.schd_isc_dt, ");
            sb.Append("s.schd_isc_by, CASE WHEN s.schd_str_dt > CURRENT_TIMESTAMP THEN 'Up Coming' ");
            sb.Append("WHEN CURRENT_TIMESTAMP BETWEEN s.schd_str_dt AND s.schd_ndt_dt THEN 'Current' ");
            sb.Append("WHEN CURRENT_TIMESTAMP > s.schd_ndt_dt THEN 'Elapsed' END schd_status, ");
            sb.Append("CASE s.schd_act_typ WHEN 0 THEN 'All Activities' ");
            sb.Append("WHEN 1 THEN 'Performance Contract Definition Only' ");
            sb.Append("WHEN 2 THEN 'Performance Evaluation Only' END schd_act_typ_ds, ");
            sb.Append("CASE s.schd_typ_id WHEN 0 THEN 'For Everyone' ");
            sb.Append("WHEN 1 THEN 'For a Location' WHEN 2 THEN 'For a Department' ");
            sb.Append("WHEN 3 THEN 'For a Unit' WHEN 4 THEN 'For an Individual' ");
            sb.Append("END schd_typ_ds ");
            sb.Append("FROM public.pmsschdls s ");
            sb.Append("INNER JOIN public.pmsrvwsxns r ON r.rvw_sxn_id = s.rvw_sxn_id ");
            sb.Append("INNER JOIN public.pmssttyrs y ON y.pms_yr_id = s.rvw_yr_id ");
            sb.Append("LEFT JOIN public.gst_locs l ON l.locqk = s.schd_loc_id ");
            sb.Append("LEFT JOIN public.gst_depts d ON d.deptqk = s.schd_dpt_id ");
            sb.Append("LEFT JOIN public.gst_units u ON u.unitqk = s.schd_unt_id ");
            //sb.Append("LEFT JOIN public.erm_emp_inf e ON e.emp_id = s.schd_emp_id ");
            sb.Append("WHERE (s.rvw_sxn_id = @rvw_sxn_id) ");
            sb.Append("AND (s.schd_dpt_id = @schd_dpt_id) ");
            sb.Append("ORDER BY s.sxn_schd_id DESC;");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_sxn_id = cmd.Parameters.Add("@rvw_sxn_id", NpgsqlDbType.Integer);
                var schd_dpt_id = cmd.Parameters.Add("@schd_dpt_id", NpgsqlDbType.Integer);
                var schd_date = cmd.Parameters.Add("@schd_date", NpgsqlDbType.Timestamp);
                await cmd.PrepareAsync();
                rvw_sxn_id.Value = reviewSessionId;
                schd_dpt_id.Value = departmentId;
                schd_date.Value = DateTime.Now;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    sessionSchedulesList.Add(new SessionSchedule()
                    {
                        SessionScheduleId = reader["sxn_schd_id"] == DBNull.Value ? 0 : (int)reader["sxn_schd_id"],
                        ReviewSessionId = reader["rvw_sxn_id"] == DBNull.Value ? 0 : (int)reader["rvw_sxn_id"],
                        ReviewSessionName = reader["rvw_sxn_nm"] == DBNull.Value ? string.Empty : reader["rvw_sxn_nm"].ToString(),
                        ReviewYearId = reader["rvw_yr_id"] == DBNull.Value ? 0 : (int)reader["rvw_yr_id"],
                        ReviewYearName = reader["pms_yr_nm"] == DBNull.Value ? string.Empty : reader["pms_yr_nm"].ToString(),
                        ScheduleType = reader["schd_typ_id"] == DBNull.Value ? SessionScheduleType.All : (SessionScheduleType)reader["schd_typ_id"],
                        ScheduleTypeDescription = reader["schd_typ_ds"] == DBNull.Value ? string.Empty : reader["schd_typ_ds"].ToString(),
                        ActivityType = reader["schd_act_typ"] == DBNull.Value ? SessionActivityType.AllActivities : (SessionActivityType)reader["schd_act_typ"],
                        SessionActivityTypeDescription = reader["schd_act_typ_ds"] == DBNull.Value ? string.Empty : reader["schd_act_typ_ds"].ToString(),
                        ScheduleLocationId = reader["schd_loc_id"] == DBNull.Value ? 0 : (int)reader["schd_loc_id"],
                        ScheduleLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                        ScheduleDepartmentId = reader["schd_dpt_id"] == DBNull.Value ? 0 : (int)reader["schd_dpt_id"],
                        ScheduleDepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                        ScheduleUnitId = reader["schd_unt_id"] == DBNull.Value ? 0 : (int)reader["schd_unt_id"],
                        ScheduleUnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                        ScheduleEmployeeId = reader["schd_emp_id"] == DBNull.Value ? string.Empty : reader["schd_emp_id"].ToString(),
                        ScheduleEmployeeName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                        ScheduleStartTime = reader["schd_str_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["schd_str_dt"],
                        ScheduleEndTime = reader["schd_ndt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["schd_ndt_dt"],
                        ScheduleStatus = reader["schd_status"] == DBNull.Value ? string.Empty : reader["schd_status"].ToString(),
                        IsCancelled = reader["schd_isc"] == DBNull.Value ? false : (bool)reader["schd_isc"],
                        CancelledBy = reader["schd_isc_by"] == DBNull.Value ? string.Empty : reader["schd_isc_by"].ToString(),
                        CancelledTime = reader["schd_isc_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["schd_isc_dt"],
                        LastModifiedTime = reader["schd_mdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["schd_mdt"],
                        LastModifiedBy = reader["schd_mdb"] == DBNull.Value ? string.Empty : (reader["schd_mdb"]).ToString(),
                        CreatedTime = reader["schd_ctt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["schd_ctt"],
                        CreatedBy = reader["schd_ctb"] == DBNull.Value ? string.Empty : (reader["schd_ctb"]).ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return sessionSchedulesList;
        }

        public async Task<IList<SessionSchedule>> GetByDepartmentCodeAsync(int reviewSessionId, int departmentId, SessionActivityType activityType)
        {
            List<SessionSchedule> sessionSchedulesList = new List<SessionSchedule>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.sxn_schd_id, s.rvw_sxn_id, s.rvw_yr_id, s.schd_typ_id, ");
            sb.Append("s.schd_act_typ, s.schd_loc_id, s.schd_dpt_id, s.schd_unt_id, ");
            sb.Append("s.schd_emp_id, s.schd_str_dt, s.schd_ndt_dt, s.schd_ctt, s.schd_ctb, ");
            sb.Append("s.schd_mdt, s.schd_mdb, r.rvw_sxn_nm, y.pms_yr_nm, l.locname, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = s.schd_emp_id) as fullname, ");
            sb.Append("u.unitname, d.deptname, s.schd_isc, s.schd_isc_dt, ");
            sb.Append("s.schd_isc_by, CASE WHEN s.schd_str_dt > CURRENT_TIMESTAMP THEN 'Up Coming' ");
            sb.Append("WHEN CURRENT_TIMESTAMP BETWEEN s.schd_str_dt AND s.schd_ndt_dt THEN 'Current' ");
            sb.Append("WHEN CURRENT_TIMESTAMP > s.schd_ndt_dt THEN 'Elapsed' END schd_status, ");
            sb.Append("CASE s.schd_act_typ WHEN 0 THEN 'All Activities' ");
            sb.Append("WHEN 1 THEN 'Performance Contract Definition Only' ");
            sb.Append("WHEN 2 THEN 'Performance Evaluation Only' END schd_act_typ_ds, ");
            sb.Append("CASE s.schd_typ_id WHEN 0 THEN 'For Everyone' ");
            sb.Append("WHEN 1 THEN 'For a Location' WHEN 2 THEN 'For a Department' ");
            sb.Append("WHEN 3 THEN 'For a Unit' WHEN 4 THEN 'For an Individual' ");
            sb.Append("END schd_typ_ds ");
            sb.Append("FROM public.pmsschdls s ");
            sb.Append("INNER JOIN public.pmsrvwsxns r ON r.rvw_sxn_id = s.rvw_sxn_id ");
            sb.Append("INNER JOIN public.pmssttyrs y ON y.pms_yr_id = s.rvw_yr_id ");
            sb.Append("LEFT JOIN public.gst_locs l ON l.locqk = s.schd_loc_id ");
            sb.Append("LEFT JOIN public.gst_depts d ON d.deptqk = s.schd_dpt_id ");
            sb.Append("LEFT JOIN public.gst_units u ON u.unitqk = s.schd_unt_id ");
            //sb.Append("LEFT JOIN public.erm_emp_inf e ON e.emp_id = s.schd_emp_id ");
            sb.Append("WHERE (s.rvw_sxn_id = @rvw_sxn_id) ");
            sb.Append("AND (LOWER(s.schd_dpt_id) = LOWER(@schd_dpt_id)) ");
            sb.Append("AND (s.schd_act_typ = @schd_act_typ) ");
            sb.Append("ORDER BY s.sxn_schd_id DESC;");

            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_sxn_id = cmd.Parameters.Add("@rvw_sxn_id", NpgsqlDbType.Integer);
                var schd_dpt_id = cmd.Parameters.Add("@schd_dpt_id", NpgsqlDbType.Integer);
                var schd_act_typ = cmd.Parameters.Add("@schd_act_typ", NpgsqlDbType.Integer);
                var schd_date = cmd.Parameters.Add("@schd_date", NpgsqlDbType.Timestamp);
                await cmd.PrepareAsync();
                rvw_sxn_id.Value = reviewSessionId;
                schd_dpt_id.Value = departmentId;
                schd_act_typ.Value = activityType;
                schd_date.Value = DateTime.Now;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    sessionSchedulesList.Add(new SessionSchedule()
                    {
                        SessionScheduleId = reader["sxn_schd_id"] == DBNull.Value ? 0 : (int)reader["sxn_schd_id"],
                        ReviewSessionId = reader["rvw_sxn_id"] == DBNull.Value ? 0 : (int)reader["rvw_sxn_id"],
                        ReviewSessionName = reader["rvw_sxn_nm"] == DBNull.Value ? string.Empty : reader["rvw_sxn_nm"].ToString(),
                        ReviewYearId = reader["rvw_yr_id"] == DBNull.Value ? 0 : (int)reader["rvw_yr_id"],
                        ReviewYearName = reader["pms_yr_nm"] == DBNull.Value ? string.Empty : reader["pms_yr_nm"].ToString(),
                        ScheduleType = reader["schd_typ_id"] == DBNull.Value ? SessionScheduleType.All : (SessionScheduleType)reader["schd_typ_id"],
                        ScheduleTypeDescription = reader["schd_typ_ds"] == DBNull.Value ? string.Empty : reader["schd_typ_ds"].ToString(),
                        ActivityType = reader["schd_act_typ"] == DBNull.Value ? SessionActivityType.AllActivities : (SessionActivityType)reader["schd_act_typ"],
                        SessionActivityTypeDescription = reader["schd_act_typ_ds"] == DBNull.Value ? string.Empty : reader["schd_act_typ_ds"].ToString(),
                        ScheduleLocationId = reader["schd_loc_id"] == DBNull.Value ? 0 : (int)reader["schd_loc_id"],
                        ScheduleLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                        ScheduleDepartmentId = reader["schd_dpt_id"] == DBNull.Value ? 0 : (int)reader["schd_dpt_id"],
                        ScheduleDepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                        ScheduleUnitId = reader["schd_unt_id"] == DBNull.Value ? 0 : (int)reader["schd_unt_id"],
                        ScheduleUnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                        ScheduleEmployeeId = reader["schd_emp_id"] == DBNull.Value ? string.Empty : reader["schd_emp_id"].ToString(),
                        ScheduleEmployeeName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                        ScheduleStartTime = reader["schd_str_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["schd_str_dt"],
                        ScheduleEndTime = reader["schd_ndt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["schd_ndt_dt"],
                        ScheduleStatus = reader["schd_status"] == DBNull.Value ? string.Empty : reader["schd_status"].ToString(),
                        IsCancelled = reader["schd_isc"] == DBNull.Value ? false : (bool)reader["schd_isc"],
                        CancelledBy = reader["schd_isc_by"] == DBNull.Value ? string.Empty : reader["schd_isc_by"].ToString(),
                        CancelledTime = reader["schd_isc_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["schd_isc_dt"],
                        LastModifiedTime = reader["schd_mdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["schd_mdt"],
                        LastModifiedBy = reader["schd_mdb"] == DBNull.Value ? string.Empty : (reader["schd_mdb"]).ToString(),
                        CreatedTime = reader["schd_ctt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["schd_ctt"],
                        CreatedBy = reader["schd_ctb"] == DBNull.Value ? string.Empty : (reader["schd_ctb"]).ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return sessionSchedulesList;
        }

        public async Task<IList<SessionSchedule>> GetByUnitCodeAsync(int reviewSessionId, int unitId)
        {
            List<SessionSchedule> sessionSchedulesList = new List<SessionSchedule>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.sxn_schd_id, s.rvw_sxn_id, s.rvw_yr_id, s.schd_typ_id, ");
            sb.Append("s.schd_act_typ, s.schd_loc_id, s.schd_dpt_id, s.schd_unt_id, ");
            sb.Append("s.schd_emp_id, s.schd_str_dt, s.schd_ndt_dt, s.schd_ctt, s.schd_ctb, ");
            sb.Append("s.schd_mdt, s.schd_mdb, r.rvw_sxn_nm, y.pms_yr_nm, l.locname, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = s.schd_emp_id) as fullname, ");
            sb.Append("u.unitname, d.deptname, s.schd_isc, s.schd_isc_dt, ");
            sb.Append("s.schd_isc_by, CASE WHEN s.schd_str_dt > CURRENT_TIMESTAMP THEN 'Up Coming' ");
            sb.Append("WHEN CURRENT_TIMESTAMP BETWEEN s.schd_str_dt AND s.schd_ndt_dt THEN 'Current' ");
            sb.Append("WHEN CURRENT_TIMESTAMP > s.schd_ndt_dt THEN 'Elapsed' END schd_status, ");
            sb.Append("CASE s.schd_act_typ WHEN 0 THEN 'All Activities' ");
            sb.Append("WHEN 1 THEN 'Performance Contract Definition Only' ");
            sb.Append("WHEN 2 THEN 'Performance Evaluation Only' END schd_act_typ_ds, ");
            sb.Append("CASE s.schd_typ_id WHEN 0 THEN 'For Everyone' ");
            sb.Append("WHEN 1 THEN 'For a Location' WHEN 2 THEN 'For a Department' ");
            sb.Append("WHEN 3 THEN 'For a Unit' WHEN 4 THEN 'For an Individual' ");
            sb.Append("END schd_typ_ds ");
            sb.Append("FROM public.pmsschdls s ");
            sb.Append("INNER JOIN public.pmsrvwsxns r ON r.rvw_sxn_id = s.rvw_sxn_id ");
            sb.Append("INNER JOIN public.pmssttyrs y ON y.pms_yr_id = s.rvw_yr_id ");
            sb.Append("LEFT JOIN public.gst_locs l ON l.locqk = s.schd_loc_id ");
            sb.Append("LEFT JOIN public.gst_depts d ON d.deptqk = s.schd_dpt_id ");
            sb.Append("LEFT JOIN public.gst_units u ON u.unitqk = s.schd_unt_id ");
            //sb.Append("LEFT JOIN public.erm_emp_inf e ON e.emp_id = s.schd_emp_id ");
            sb.Append("WHERE (s.rvw_sxn_id = @rvw_sxn_id) ");
            sb.Append("AND (s.schd_unt_id = @schd_unt_id) ");
            sb.Append("ORDER BY s.sxn_schd_id DESC;");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_sxn_id = cmd.Parameters.Add("@rvw_sxn_id", NpgsqlDbType.Integer);
                var schd_unt_id = cmd.Parameters.Add("@schd_unt_id", NpgsqlDbType.Integer);
                var schd_date = cmd.Parameters.Add("@schd_date", NpgsqlDbType.Timestamp);
                await cmd.PrepareAsync();
                rvw_sxn_id.Value = reviewSessionId;
                schd_unt_id.Value = unitId;
                schd_date.Value = DateTime.Now;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    sessionSchedulesList.Add(new SessionSchedule()
                    {
                        SessionScheduleId = reader["sxn_schd_id"] == DBNull.Value ? 0 : (int)reader["sxn_schd_id"],
                        ReviewSessionId = reader["rvw_sxn_id"] == DBNull.Value ? 0 : (int)reader["rvw_sxn_id"],
                        ReviewSessionName = reader["rvw_sxn_nm"] == DBNull.Value ? string.Empty : reader["rvw_sxn_nm"].ToString(),
                        ReviewYearId = reader["rvw_yr_id"] == DBNull.Value ? 0 : (int)reader["rvw_yr_id"],
                        ReviewYearName = reader["pms_yr_nm"] == DBNull.Value ? string.Empty : reader["pms_yr_nm"].ToString(),
                        ScheduleType = reader["schd_typ_id"] == DBNull.Value ? SessionScheduleType.All : (SessionScheduleType)reader["schd_typ_id"],
                        ScheduleTypeDescription = reader["schd_typ_ds"] == DBNull.Value ? string.Empty : reader["schd_typ_ds"].ToString(),
                        ActivityType = reader["schd_act_typ"] == DBNull.Value ? SessionActivityType.AllActivities : (SessionActivityType)reader["schd_act_typ"],
                        SessionActivityTypeDescription = reader["schd_act_typ_ds"] == DBNull.Value ? string.Empty : reader["schd_act_typ_ds"].ToString(),
                        ScheduleLocationId = reader["schd_loc_id"] == DBNull.Value ? 0 : (int)reader["schd_loc_id"],
                        ScheduleLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                        ScheduleDepartmentId = reader["schd_dpt_id"] == DBNull.Value ? 0 : (int)reader["schd_dpt_id"],
                        ScheduleDepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                        ScheduleUnitId = reader["schd_unt_id"] == DBNull.Value ? 0 : (int)reader["schd_unt_id"],
                        ScheduleUnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                        ScheduleEmployeeId = reader["schd_emp_id"] == DBNull.Value ? string.Empty : reader["schd_emp_id"].ToString(),
                        ScheduleEmployeeName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                        ScheduleStartTime = reader["schd_str_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["schd_str_dt"],
                        ScheduleEndTime = reader["schd_ndt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["schd_ndt_dt"],
                        ScheduleStatus = reader["schd_status"] == DBNull.Value ? string.Empty : reader["schd_status"].ToString(),
                        IsCancelled = reader["schd_isc"] == DBNull.Value ? false : (bool)reader["schd_isc"],
                        CancelledBy = reader["schd_isc_by"] == DBNull.Value ? string.Empty : reader["schd_isc_by"].ToString(),
                        CancelledTime = reader["schd_isc_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["schd_isc_dt"],
                        LastModifiedTime = reader["schd_mdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["schd_mdt"],
                        LastModifiedBy = reader["schd_mdb"] == DBNull.Value ? string.Empty : (reader["schd_mdb"]).ToString(),
                        CreatedTime = reader["schd_ctt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["schd_ctt"],
                        CreatedBy = reader["schd_ctb"] == DBNull.Value ? string.Empty : (reader["schd_ctb"]).ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return sessionSchedulesList;
        }

        public async Task<IList<SessionSchedule>> GetByUnitCodeAsync(int reviewSessionId, int unitId, SessionActivityType activityType)
        {
            List<SessionSchedule> sessionSchedulesList = new List<SessionSchedule>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.sxn_schd_id, s.rvw_sxn_id, s.rvw_yr_id, s.schd_typ_id, ");
            sb.Append("s.schd_act_typ, s.schd_loc_id, s.schd_dpt_id, s.schd_unt_id, ");
            sb.Append("s.schd_emp_id, s.schd_str_dt, s.schd_ndt_dt, s.schd_ctt, s.schd_ctb, ");
            sb.Append("s.schd_mdt, s.schd_mdb, r.rvw_sxn_nm, y.pms_yr_nm, l.locname, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = s.schd_emp_id) as fullname, ");
            sb.Append("u.unitname, d.deptname, s.schd_isc, s.schd_isc_dt, ");
            sb.Append("s.schd_isc_by, CASE WHEN s.schd_str_dt > CURRENT_TIMESTAMP THEN 'Up Coming' ");
            sb.Append("WHEN CURRENT_TIMESTAMP BETWEEN s.schd_str_dt AND s.schd_ndt_dt THEN 'Current' ");
            sb.Append("WHEN CURRENT_TIMESTAMP > s.schd_ndt_dt THEN 'Elapsed' END schd_status, ");
            sb.Append("CASE s.schd_act_typ WHEN 0 THEN 'All Activities' ");
            sb.Append("WHEN 1 THEN 'Performance Contract Definition Only' ");
            sb.Append("WHEN 2 THEN 'Performance Evaluation Only' END schd_act_typ_ds, ");
            sb.Append("CASE s.schd_typ_id WHEN 0 THEN 'For Everyone' ");
            sb.Append("WHEN 1 THEN 'For a Location' WHEN 2 THEN 'For a Department' ");
            sb.Append("WHEN 3 THEN 'For a Unit' WHEN 4 THEN 'For an Individual' ");
            sb.Append("END schd_typ_ds ");
            sb.Append("FROM public.pmsschdls s ");
            sb.Append("INNER JOIN public.pmsrvwsxns r ON r.rvw_sxn_id = s.rvw_sxn_id ");
            sb.Append("INNER JOIN public.pmssttyrs y ON y.pms_yr_id = s.rvw_yr_id ");
            sb.Append("LEFT JOIN public.gst_locs l ON l.locqk = s.schd_loc_id ");
            sb.Append("LEFT JOIN public.gst_depts d ON d.deptqk = s.schd_dpt_id ");
            sb.Append("LEFT JOIN public.gst_units u ON u.unitqk = s.schd_unt_id ");
            //sb.Append("LEFT JOIN public.erm_emp_inf e ON e.emp_id = s.schd_emp_id ");
            sb.Append("WHERE (s.rvw_sxn_id = @rvw_sxn_id) ");
            sb.Append("AND (LOWER(s.schd_unt_id) = LOWER(@schd_unt_id)) ");
            sb.Append("AND (s.schd_act_typ = @schd_act_typ) ");
            sb.Append("ORDER BY s.sxn_schd_id DESC;");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_sxn_id = cmd.Parameters.Add("@rvw_sxn_id", NpgsqlDbType.Integer);
                var schd_unt_id = cmd.Parameters.Add("@schd_unt_id", NpgsqlDbType.Integer);
                var schd_act_typ = cmd.Parameters.Add("@schd_act_typ", NpgsqlDbType.Integer);
                var schd_date = cmd.Parameters.Add("@schd_date", NpgsqlDbType.Timestamp);
                await cmd.PrepareAsync();
                rvw_sxn_id.Value = reviewSessionId;
                schd_unt_id.Value = unitId;
                schd_act_typ.Value = activityType;
                schd_date.Value = DateTime.Now;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    sessionSchedulesList.Add(new SessionSchedule()
                    {
                        SessionScheduleId = reader["sxn_schd_id"] == DBNull.Value ? 0 : (int)reader["sxn_schd_id"],
                        ReviewSessionId = reader["rvw_sxn_id"] == DBNull.Value ? 0 : (int)reader["rvw_sxn_id"],
                        ReviewSessionName = reader["rvw_sxn_nm"] == DBNull.Value ? string.Empty : reader["rvw_sxn_nm"].ToString(),
                        ReviewYearId = reader["rvw_yr_id"] == DBNull.Value ? 0 : (int)reader["rvw_yr_id"],
                        ReviewYearName = reader["pms_yr_nm"] == DBNull.Value ? string.Empty : reader["pms_yr_nm"].ToString(),
                        ScheduleType = reader["schd_typ_id"] == DBNull.Value ? SessionScheduleType.All : (SessionScheduleType)reader["schd_typ_id"],
                        ScheduleTypeDescription = reader["schd_typ_ds"] == DBNull.Value ? string.Empty : reader["schd_typ_ds"].ToString(),
                        ActivityType = reader["schd_act_typ"] == DBNull.Value ? SessionActivityType.AllActivities : (SessionActivityType)reader["schd_act_typ"],
                        SessionActivityTypeDescription = reader["schd_act_typ_ds"] == DBNull.Value ? string.Empty : reader["schd_act_typ_ds"].ToString(),
                        ScheduleLocationId = reader["schd_loc_id"] == DBNull.Value ? 0 : (int)reader["schd_loc_id"],
                        ScheduleLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                        ScheduleDepartmentId = reader["schd_dpt_id"] == DBNull.Value ? 0 : (int)reader["schd_dpt_id"],
                        ScheduleDepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                        ScheduleUnitId = reader["schd_unt_id"] == DBNull.Value ? 0 : (int)reader["schd_unt_id"],
                        ScheduleUnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                        ScheduleEmployeeId = reader["schd_emp_id"] == DBNull.Value ? string.Empty : reader["schd_emp_id"].ToString(),
                        ScheduleEmployeeName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                        ScheduleStartTime = reader["schd_str_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["schd_str_dt"],
                        ScheduleEndTime = reader["schd_ndt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["schd_ndt_dt"],
                        ScheduleStatus = reader["schd_status"] == DBNull.Value ? string.Empty : reader["schd_status"].ToString(),
                        IsCancelled = reader["schd_isc"] == DBNull.Value ? false : (bool)reader["schd_isc"],
                        CancelledBy = reader["schd_isc_by"] == DBNull.Value ? string.Empty : reader["schd_isc_by"].ToString(),
                        CancelledTime = reader["schd_isc_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["schd_isc_dt"],
                        LastModifiedTime = reader["schd_mdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["schd_mdt"],
                        LastModifiedBy = reader["schd_mdb"] == DBNull.Value ? string.Empty : (reader["schd_mdb"]).ToString(),
                        CreatedTime = reader["schd_ctt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["schd_ctt"],
                        CreatedBy = reader["schd_ctb"] == DBNull.Value ? string.Empty : (reader["schd_ctb"]).ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return sessionSchedulesList;
        }

        #endregion


        #region Employee Session Schedule Read Action Methods
        public async Task<List<SessionActivityType>> GetForAllAsync(int reviewSessionId)
        {
            List<SessionActivityType> sessionScheduledActivityList = new List<SessionActivityType>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT schd_act_typ ");
            sb.Append("FROM public.pmsschdls ");
            sb.Append("WHERE (rvw_sxn_id = @rvw_sxn_id) ");
            sb.Append("AND (schd_typ_id = 0) AND (schd_isc = false) ");
            sb.Append("AND (@schdl_dt BETWEEN schd_str_dt AND schd_ndt_dt); ");

            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_sxn_id = cmd.Parameters.Add("@rvw_sxn_id", NpgsqlDbType.Integer);
                var schdl_dt = cmd.Parameters.Add("@schdl_dt", NpgsqlDbType.TimestampTz);
                await cmd.PrepareAsync();
                rvw_sxn_id.Value = reviewSessionId;
                schdl_dt.Value = DateTime.UtcNow;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    sessionScheduledActivityList.Add(
                         reader["schd_act_typ"] == DBNull.Value ? SessionActivityType.AllActivities : (SessionActivityType)reader["schd_act_typ"]
                    );
                }
            }
            await conn.CloseAsync();
            return sessionScheduledActivityList;
        }

        public async Task<List<SessionActivityType>> GetForLocationAsync(int reviewSessionId, int locationId)
        {
            List<SessionActivityType> sessionScheduledActivityList = new List<SessionActivityType>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT schd_act_typ ");
            sb.Append("FROM public.pmsschdls ");
            sb.Append("WHERE (rvw_sxn_id = @rvw_sxn_id) ");
            sb.Append("AND (schd_typ_id = 1) AND (schd_isc = false) ");
            sb.Append("AND (schd_loc_id = @schd_loc_id) ");
            sb.Append("AND (@schdl_dt BETWEEN schd_str_dt AND schd_ndt_dt); ");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_sxn_id = cmd.Parameters.Add("@rvw_sxn_id", NpgsqlDbType.Integer);
                var schd_loc_id = cmd.Parameters.Add("@schd_loc_id", NpgsqlDbType.Integer);
                var schdl_dt = cmd.Parameters.Add("@schdl_dt", NpgsqlDbType.TimestampTz);
                await cmd.PrepareAsync();
                rvw_sxn_id.Value = reviewSessionId;
                schd_loc_id.Value = locationId;
                schdl_dt.Value = DateTime.UtcNow;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    sessionScheduledActivityList.Add(
                         reader["schd_act_typ"] == DBNull.Value ? SessionActivityType.AllActivities : (SessionActivityType)reader["schd_act_typ"]
                    );
                }
            }
            await conn.CloseAsync();
            return sessionScheduledActivityList;
        }

        public async Task<List<SessionActivityType>> GetForDepartmentAsync(int reviewSessionId, int departmentId)
        {
            List<SessionActivityType> sessionScheduledActivityList = new List<SessionActivityType>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT schd_act_typ ");
            sb.Append("FROM public.pmsschdls ");
            sb.Append("WHERE (rvw_sxn_id = @rvw_sxn_id) ");
            sb.Append("AND (schd_typ_id = 2) AND (schd_isc = false) ");
            sb.Append("AND (schd_dpt_id = @schd_dpt_id) ");
            sb.Append("AND (@schdl_dt BETWEEN schd_str_dt AND schd_ndt_dt); ");

            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_sxn_id = cmd.Parameters.Add("@rvw_sxn_id", NpgsqlDbType.Integer);
                var schd_dpt_id = cmd.Parameters.Add("@schd_dpt_id", NpgsqlDbType.Integer);
                var schdl_dt = cmd.Parameters.Add("@schdl_dt", NpgsqlDbType.TimestampTz);
                await cmd.PrepareAsync();
                rvw_sxn_id.Value = reviewSessionId;
                schd_dpt_id.Value = departmentId;
                schdl_dt.Value = DateTime.UtcNow;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    sessionScheduledActivityList.Add(
                         reader["schd_act_typ"] == DBNull.Value ? SessionActivityType.AllActivities : (SessionActivityType)reader["schd_act_typ"]
                    );
                }
            }
            await conn.CloseAsync();
            return sessionScheduledActivityList;
        }

        public async Task<List<SessionActivityType>> GetForUnitAsync(int reviewSessionId, int unitId)
        {
            List<SessionActivityType> sessionScheduledActivityList = new List<SessionActivityType>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT schd_act_typ ");
            sb.Append("FROM public.pmsschdls ");
            sb.Append("WHERE (rvw_sxn_id = @rvw_sxn_id) ");
            sb.Append("AND (schd_typ_id = 2) AND (schd_isc = false) ");
            sb.Append("AND (schd_unt_id = @schd_unt_id) ");
            sb.Append("AND (@schdl_dt BETWEEN schd_str_dt AND schd_ndt_dt); ");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_sxn_id = cmd.Parameters.Add("@rvw_sxn_id", NpgsqlDbType.Integer);
                var schd_unt_id = cmd.Parameters.Add("@schd_unt_id", NpgsqlDbType.Integer);
                var schdl_dt = cmd.Parameters.Add("@schdl_dt", NpgsqlDbType.TimestampTz);
                await cmd.PrepareAsync();
                rvw_sxn_id.Value = reviewSessionId;
                schd_unt_id.Value = unitId;
                schdl_dt.Value = DateTime.UtcNow;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    sessionScheduledActivityList.Add(
                         reader["schd_act_typ"] == DBNull.Value ? SessionActivityType.AllActivities : (SessionActivityType)reader["schd_act_typ"]
                    );
                }
            }
            await conn.CloseAsync();
            return sessionScheduledActivityList;
        }

        public async Task<List<SessionActivityType>> GetForEmployeeAsync(int reviewSessionId, string employeeId)
        {
            List<SessionActivityType> sessionScheduledActivityList = new List<SessionActivityType>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT schd_act_typ ");
            sb.Append("FROM public.pmsschdls ");
            sb.Append("WHERE (rvw_sxn_id = @rvw_sxn_id) ");
            sb.Append("AND (schd_typ_id = 2) AND (schd_isc = false) ");
            sb.Append("AND (schd_emp_id = @schd_emp_id) ");
            sb.Append("AND (@schdl_dt BETWEEN schd_str_dt AND schd_ndt_dt); ");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_sxn_id = cmd.Parameters.Add("@rvw_sxn_id", NpgsqlDbType.Integer);
                var schd_emp_id = cmd.Parameters.Add("@schd_emp_id", NpgsqlDbType.Text);
                var schdl_dt = cmd.Parameters.Add("@schdl_dt", NpgsqlDbType.TimestampTz);
                await cmd.PrepareAsync();
                rvw_sxn_id.Value = reviewSessionId;
                schd_emp_id.Value = employeeId;
                schdl_dt.Value = DateTime.UtcNow;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    sessionScheduledActivityList.Add(
                         reader["schd_act_typ"] == DBNull.Value ? SessionActivityType.AllActivities : (SessionActivityType)reader["schd_act_typ"]
                    );
                }
            }
            await conn.CloseAsync();
            return sessionScheduledActivityList;
        }

        #endregion

        #region Session Schedule Write Action Methods
        public async Task<bool> AddAsync(SessionSchedule sessionSchedule)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.pmsschdls(rvw_sxn_id, rvw_yr_id, schd_typ_id, ");
            sb.Append("schd_act_typ, schd_loc_id, schd_dpt_id, schd_unt_id, schd_emp_id, ");
            sb.Append("schd_str_dt, schd_ndt_dt, schd_ctt, schd_ctb, schd_mdt, schd_mdb, ");
            sb.Append("schd_isc, schd_isc_dt, schd_isc_by) VALUES (@rvw_sxn_id, @rvw_yr_id, ");
            sb.Append("@schd_typ_id, @schd_act_typ, @schd_loc_id, @schd_dpt_id, @schd_unt_id, ");
            sb.Append("@schd_emp_id, @schd_str_dt, @schd_ndt_dt, @schd_ctt, @schd_ctb, ");
            sb.Append("@schd_mdt, @schd_mdb, @schd_isc, @schd_isc_dt, @schd_isc_by);");
            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_sxn_id = cmd.Parameters.Add("@rvw_sxn_id", NpgsqlDbType.Integer);
                var rvw_yr_id = cmd.Parameters.Add("@rvw_yr_id", NpgsqlDbType.Integer);
                var schd_typ_id = cmd.Parameters.Add("@schd_typ_id", NpgsqlDbType.Integer);
                var schd_act_typ = cmd.Parameters.Add("@schd_act_typ", NpgsqlDbType.Integer);
                var schd_loc_id = cmd.Parameters.Add("@schd_loc_id", NpgsqlDbType.Integer);
                var schd_dpt_id = cmd.Parameters.Add("@schd_dpt_id", NpgsqlDbType.Integer);
                var schd_unt_id = cmd.Parameters.Add("@schd_unt_id", NpgsqlDbType.Integer);
                var schd_emp_id = cmd.Parameters.Add("@schd_emp_id", NpgsqlDbType.Text);
                var schd_str_dt = cmd.Parameters.Add("@schd_str_dt", NpgsqlDbType.Timestamp);
                var schd_ndt_dt = cmd.Parameters.Add("@schd_ndt_dt", NpgsqlDbType.Timestamp);
                var schd_ctt = cmd.Parameters.Add("@schd_ctt", NpgsqlDbType.Timestamp);
                var schd_ctb = cmd.Parameters.Add("@schd_ctb", NpgsqlDbType.Text);
                var schd_mdt = cmd.Parameters.Add("@schd_mdt", NpgsqlDbType.Timestamp);
                var schd_mdb = cmd.Parameters.Add("@schd_mdb", NpgsqlDbType.Text);
                var schd_isc = cmd.Parameters.Add("@schd_isc", NpgsqlDbType.Boolean);
                var schd_isc_dt = cmd.Parameters.Add("@schd_isc_dt", NpgsqlDbType.Timestamp);
                var schd_isc_by = cmd.Parameters.Add("@schd_isc_by", NpgsqlDbType.Text);

                cmd.Prepare();
                rvw_sxn_id.Value = sessionSchedule.ReviewSessionId;
                rvw_yr_id.Value = sessionSchedule.ReviewYearId;
                schd_typ_id.Value = (int)sessionSchedule.ScheduleType;
                schd_act_typ.Value = (int)sessionSchedule.ActivityType;
                schd_loc_id.Value = sessionSchedule.ScheduleLocationId ?? (object)DBNull.Value;
                schd_dpt_id.Value = sessionSchedule.ScheduleDepartmentId ?? (object)DBNull.Value;
                schd_unt_id.Value = sessionSchedule.ScheduleUnitId ?? (object)DBNull.Value;
                schd_emp_id.Value = sessionSchedule.ScheduleEmployeeId ?? (object)DBNull.Value;
                schd_str_dt.Value = sessionSchedule.ScheduleStartTime ?? DateTime.Now;
                schd_ndt_dt.Value = sessionSchedule.ScheduleEndTime ?? DateTime.Now.AddDays(1.0D);
                schd_ctt.Value = sessionSchedule.CreatedTime ?? (object)DBNull.Value;
                schd_ctb.Value = sessionSchedule.CreatedBy ?? (object)DBNull.Value;
                schd_mdt.Value = sessionSchedule.LastModifiedTime ?? (object)DBNull.Value;
                schd_mdb.Value = sessionSchedule.LastModifiedBy ?? (object)DBNull.Value;
                schd_isc.Value = sessionSchedule.IsCancelled;
                schd_isc_dt.Value = sessionSchedule.CancelledTime ?? (object)DBNull.Value;
                schd_isc_by.Value = sessionSchedule.CancelledBy ?? (object)DBNull.Value;

                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<bool> CancelAsync(int sessionScheduleId, string cancelledBy)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));

            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.pmsschdls SET schd_isc=true, ");
            sb.Append("schd_isc_dt=@schd_isc_dt, schd_isc_by=@schd_isc_by  ");
            sb.Append("WHERE (sxn_schd_id=@sxn_schd_id);");
            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var schd_isc_dt = cmd.Parameters.Add("@schd_isc_dt", NpgsqlDbType.Timestamp);
                var schd_isc_by = cmd.Parameters.Add("@schd_isc_by", NpgsqlDbType.Text);
                var sxn_schd_id = cmd.Parameters.Add("@sxn_schd_id", NpgsqlDbType.Integer);

                cmd.Prepare();

                schd_isc_dt.Value = DateTime.Now;
                schd_isc_by.Value = cancelledBy;
                sxn_schd_id.Value = sessionScheduleId;

                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int sessionScheduleId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = "DELETE FROM public.pmsschdls WHERE (sxn_schd_id = @sxn_schd_id);";
            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var sxn_schd_id = cmd.Parameters.Add("@sxn_schd_id", NpgsqlDbType.Integer);
                cmd.Prepare();
                sxn_schd_id.Value = sessionScheduleId;

                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        #endregion
    }
}
