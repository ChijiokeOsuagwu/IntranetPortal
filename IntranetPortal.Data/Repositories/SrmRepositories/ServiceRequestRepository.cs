using IntranetPortal.Base.Models.SrmModels;
using IntranetPortal.Base.Repositories.SrmRepositories;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Data.Repositories.SrmRepositories
{
    public class ServiceRequestRepository : IServiceRequestRepository
    {
        public IConfiguration _config { get; }

        public ServiceRequestRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        #region Service Incidents Data Access Methods
        
        #region Write Action Methods
        public async Task<long> AddServiceIncidentAsync(ServiceIncident incident)
        {
            long inserted_row_id = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.srm_inc_inf(inc_desc, inc_imp, inc_dt, ");
            sb.Append("inc_emp_id, inc_rpt_by, inc_rpt_dt, inc_sts, inc_isfn, ");
            sb.Append("inc_sys_id, inc_loc_id, inc_unit_id, inc_svrt, is_assgnd, ");
            sb.Append("inc_dept_id, srv_cntr_id, inc_no, res_cnfmd, cnfmd_by, ");
            sb.Append("cnfmd_dt, assgnd_to, assgnd_dt) VALUES (@inc_desc, @inc_imp, ");
            sb.Append("@inc_dt, @inc_emp_id, @inc_rpt_by,  @inc_rpt_dt, @inc_sts, ");
            sb.Append("@inc_isfn, @inc_sys_id, @inc_loc_id, @inc_unit_id, ");
            sb.Append("@inc_svrt, @is_assgnd, @inc_dept_id, @srv_cntr_id, ");
            sb.Append("@inc_no, @res_cnfmd, @cnfmd_by, @cnfmd_dt, @assgnd_to, ");
            sb.Append("@assgnd_dt) RETURNING inc_id; "); 

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var inc_desc = cmd.Parameters.Add("@inc_desc", NpgsqlDbType.Text);
                    var inc_imp = cmd.Parameters.Add("@inc_imp", NpgsqlDbType.Text);
                    var inc_dt = cmd.Parameters.Add("@inc_dt", NpgsqlDbType.Timestamp);
                    var inc_emp_id = cmd.Parameters.Add("@inc_emp_id", NpgsqlDbType.Text);
                    var inc_rpt_by = cmd.Parameters.Add("@inc_rpt_by", NpgsqlDbType.Text);
                    var inc_rpt_dt = cmd.Parameters.Add("@inc_rpt_dt", NpgsqlDbType.Timestamp);
                    var inc_sts = cmd.Parameters.Add("@inc_sts", NpgsqlDbType.Text);
                    var inc_isfn = cmd.Parameters.Add("@inc_isfn", NpgsqlDbType.Boolean);
                    var inc_sys_id = cmd.Parameters.Add("@inc_sys_id", NpgsqlDbType.Integer);
                    var inc_loc_id = cmd.Parameters.Add("@inc_loc_id", NpgsqlDbType.Integer);
                    var inc_unit_id = cmd.Parameters.Add("@inc_unit_id", NpgsqlDbType.Integer);
                    var inc_svrt = cmd.Parameters.Add("@inc_svrt", NpgsqlDbType.Integer);
                    var is_assgnd = cmd.Parameters.Add("@is_assgnd", NpgsqlDbType.Boolean);
                    var inc_dept_id = cmd.Parameters.Add("@inc_dept_id", NpgsqlDbType.Integer);
                    var srv_cntr_id = cmd.Parameters.Add("@srv_cntr_id", NpgsqlDbType.Text);
                    var inc_no = cmd.Parameters.Add("@inc_no", NpgsqlDbType.Text);
                    var res_cnfmd = cmd.Parameters.Add("@res_cnfmd", NpgsqlDbType.Boolean);
                    var cnfmd_by = cmd.Parameters.Add("@cnfmd_by", NpgsqlDbType.Text);
                    var cnfmd_dt = cmd.Parameters.Add("@cnfmd_dt", NpgsqlDbType.Timestamp);
                    var assgnd_to = cmd.Parameters.Add("@assgnd_to", NpgsqlDbType.Text);
                    var assgnd_dt = cmd.Parameters.Add("@assgnd_dt", NpgsqlDbType.Timestamp);
                    cmd.Prepare();
                    inc_desc.Value = incident.Description;
                    inc_imp.Value = incident.Impact ?? (object)DBNull.Value;
                    inc_dt.Value = incident.IncidentTime ?? DateTime.Now;
                    inc_emp_id.Value = incident.IncidentEmployeeId;
                    inc_rpt_by.Value = incident.ReportedByEmployeeName;
                    inc_rpt_dt.Value = incident.ReportedTime ?? DateTime.Now;
                    inc_sts.Value = incident.IncidentStatus;
                    inc_isfn.Value = incident.IsFalseNegative;
                    inc_sys_id.Value = incident.ServiceSystemId ?? (object)DBNull.Value;
                    inc_loc_id.Value = incident.LocationId ?? (object)DBNull.Value;
                    inc_unit_id.Value = incident.UnitId ?? (object)DBNull.Value;
                    inc_svrt.Value = incident.Severity;
                    is_assgnd.Value = incident.IsAssigned;
                    inc_dept_id.Value = incident.DepartmentId;
                    srv_cntr_id.Value = incident.ServiceCenterId;
                    inc_no.Value = incident.Number;
                    res_cnfmd.Value = incident.ConfirmedResolved;
                    cnfmd_by.Value = incident.ConfirmedBy;
                    cnfmd_dt.Value = incident.ConfirmedTime ?? (object)DBNull.Value;
                    assgnd_to.Value = incident.AssignedToName ?? (object)DBNull.Value;
                    assgnd_dt.Value = incident.AssignedTime ?? (object)DBNull.Value;
                    var obj = await cmd.ExecuteScalarAsync();
                    inserted_row_id = (long)obj;
                }
                await conn.CloseAsync();
            }
            return inserted_row_id;
        }
        public async Task<bool> UpdateServiceIncidentAsync(ServiceIncident incident)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();

            sb.Append("UPDATE public.srm_inc_inf SET inc_desc=@inc_desc, ");
            sb.Append("inc_imp=@inc_imp, inc_dt=@inc_dt, inc_emp_id=@inc_emp_id, ");
            sb.Append("inc_isfn=@inc_isfn, inc_sys_id=@inc_sys_id, ");
            sb.Append("inc_loc_id=@inc_loc_id, inc_unit_id=@inc_unit_id, ");
            sb.Append("inc_svrt=@inc_svrt, inc_dept_id=@inc_dept_id,  ");
            sb.Append("srv_cntr_id=@srv_cntr_id ");
            sb.Append("WHERE (inc_id=@inc_id); ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var inc_desc = cmd.Parameters.Add("@inc_desc", NpgsqlDbType.Text);
                    var inc_imp = cmd.Parameters.Add("@inc_imp", NpgsqlDbType.Text);
                    var inc_dt = cmd.Parameters.Add("@inc_dt", NpgsqlDbType.Timestamp);
                    var inc_emp_id = cmd.Parameters.Add("@inc_emp_id", NpgsqlDbType.Text);
                    var inc_isfn = cmd.Parameters.Add("@inc_isfn", NpgsqlDbType.Boolean);
                    var inc_sys_id = cmd.Parameters.Add("@inc_sys_id", NpgsqlDbType.Integer);
                    var inc_loc_id = cmd.Parameters.Add("@inc_loc_id", NpgsqlDbType.Integer);
                    var inc_unit_id = cmd.Parameters.Add("@inc_unit_id", NpgsqlDbType.Integer);
                    var inc_svrt = cmd.Parameters.Add("@inc_svrt", NpgsqlDbType.Integer);
                    var inc_dept_id = cmd.Parameters.Add("@inc_dept_id", NpgsqlDbType.Integer);
                    var srv_cntr_id = cmd.Parameters.Add("@srv_cntr_id", NpgsqlDbType.Text);
                    var inc_id = cmd.Parameters.Add("@inc_id", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    inc_desc.Value = incident.Description;
                    inc_imp.Value = incident.Impact ?? (object)DBNull.Value;
                    inc_dt.Value = incident.IncidentTime ?? DateTime.Now;
                    inc_emp_id.Value = incident.IncidentEmployeeId;
                    inc_isfn.Value = incident.IsFalseNegative;
                    inc_sys_id.Value = incident.ServiceSystemId ?? (object)DBNull.Value;
                    inc_loc_id.Value = incident.LocationId ?? (object)DBNull.Value;
                    inc_unit_id.Value = incident.UnitId ?? (object)DBNull.Value;
                    inc_svrt.Value = incident.Severity;
                    inc_dept_id.Value = incident.DepartmentId ?? (object)DBNull.Value;
                    srv_cntr_id.Value = incident.ServiceCenterId ?? (object)DBNull.Value;
                    inc_id.Value = incident.Id;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        public async Task<bool> UpdateServiceIncidentStatusAsync(long serviceIncidentId, string newIncidentStatus)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();

            sb.Append("UPDATE public.srm_inc_inf SET inc_sts=@inc_sts ");
            sb.Append("WHERE (inc_id=@inc_id); ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var inc_sts = cmd.Parameters.Add("@inc_sts", NpgsqlDbType.Text);
                    var inc_id = cmd.Parameters.Add("@inc_id", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    inc_sts.Value = newIncidentStatus;
                    inc_id.Value = serviceIncidentId;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        public async Task<bool> UpdateServiceIncidentAssignmentAsync(long serviceIncidentId, string assignedToEmployeeName)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.srm_inc_inf SET is_assgnd='true', ");
            sb.Append("assgnd_to=@assgnd_to, assgnd_dt=@assgnd_dt ");
            sb.Append("WHERE (inc_id=@inc_id); ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var assgnd_to = cmd.Parameters.Add("@assgnd_to", NpgsqlDbType.Text);
                    var assgnd_dt = cmd.Parameters.Add("@assgnd_dt", NpgsqlDbType.Timestamp);
                    var inc_id = cmd.Parameters.Add("@inc_id", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    assgnd_to.Value = assignedToEmployeeName;
                    assgnd_dt.Value = DateTime.Now;
                    inc_id.Value = serviceIncidentId;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }


        public async Task<bool> DeleteServiceIncidentAsync(long serviceIncidentId)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("DELETE FROM public.srm_inc_hst WHERE (inc_id=@inc_id); ");
            sb.Append("DELETE FROM public.srm_inc_nts WHERE (inc_id=@inc_id); ");
            sb.Append("DELETE FROM public.srm_inc_res WHERE (inc_id=@inc_id); ");
            sb.Append("DELETE FROM public.srm_inc_inf WHERE (inc_id=@inc_id); ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var inc_id = cmd.Parameters.Add("@inc_id", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    inc_id.Value = serviceIncidentId;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        #endregion
        
        #region Read Action Methods
        public async Task<List<ServiceIncident>> GetServiceIncidentsByOwnerIdAsync(string ownerId, DateTime? startDate, DateTime? endDate)
        {
            List<ServiceIncident> incidentsList = new List<ServiceIncident>();
            if(startDate == null) { startDate = DateTime.Now.AddMonths(-6); }
            if(endDate == null) { endDate = DateTime.Now.AddDays(1); }

            string start_date = startDate.Value.ToString("yyyy-MM-dd");
            string end_date = endDate.Value.ToString("yyyy-MM-dd");

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT i.inc_id, i.inc_desc, i.inc_imp, i.inc_dt, i.inc_emp_id, i.inc_rpt_by, ");
            sb.Append("i.inc_rpt_dt, i.inc_sts, i.inc_isfn, i.inc_sys_id, i.inc_loc_id, i.inc_unit_id, ");
            sb.Append("i.inc_svrt, i.is_assgnd, i.inc_dept_id, i.inc_no, i.srv_cntr_id, i.res_cnfmd, ");
            sb.Append("i.cnfmd_by, i.cnfmd_dt, i.assgnd_to, i.assgnd_dt, t.tm_nm, s.inc_sys_nm, ");
            sb.Append("CASE i.inc_svrt WHEN 0 THEN 'Low' ");
            sb.Append("WHEN 1 THEN 'Medium' ");
            sb.Append("WHEN 2 THEN 'High' ");
            sb.Append("WHEN 3 THEN 'Critical' END AS inc_svrt_desc, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id =  i.inc_emp_id) as inc_emp_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = i.inc_unit_id) as inc_unit_nm, ");
            sb.Append("(SELECT deptname FROM public.gst_depts WHERE deptqk = i.inc_dept_id) as inc_dept_nm, ");
            sb.Append("(SELECT locname FROM public.gst_locs WHERE locqk = i.inc_loc_id) as inc_loc_nm ");
            sb.Append("FROM public.srm_inc_inf i ");
            sb.Append("LEFT OUTER JOIN public.srm_inc_sys s ON i.inc_sys_id = s.inc_sys_id ");
            sb.Append("LEFT OUTER JOIN public.gst_tms t ON i.srv_cntr_id = t.tm_id ");
            sb.Append("WHERE (i.inc_emp_id = @inc_emp_id) ");
            //sb.Append("AND (LOWER(t.tsk_itm_ds) LIKE '%'||LOWER(@kw)||'%') ");
            sb.Append("AND (i.inc_rpt_dt >= to_date(@sdt,'YYYY-MM-DD')) ");
            sb.Append("AND (i.inc_rpt_dt <= to_date(@edt,'YYYY-MM-DD')) ");
            sb.Append("ORDER BY i.inc_id DESC;");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var inc_emp_id = cmd.Parameters.Add("@inc_emp_id", NpgsqlDbType.Text);
                    var sdt = cmd.Parameters.Add("@sdt", NpgsqlDbType.Text);
                    var edt = cmd.Parameters.Add("@edt", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    inc_emp_id.Value = ownerId;
                    sdt.Value = start_date;
                    edt.Value = end_date;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            incidentsList.Add(new ServiceIncident
                            {
                                Id = reader["inc_id"] == DBNull.Value ? 0 : (long)reader["inc_id"],
                                Number = reader["inc_no"] == DBNull.Value ? "" : reader["inc_no"].ToString(),
                                Description = reader["inc_desc"] == DBNull.Value ? "" : reader["inc_desc"].ToString(),
                                Impact = reader["inc_imp"] == DBNull.Value ? "" : reader["inc_imp"].ToString(),
                                IncidentTime = reader["inc_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["inc_dt"],
                                IncidentEmployeeId = reader["inc_emp_id"] == DBNull.Value ? "" : reader["inc_emp_id"].ToString(),
                                IncidentEmployeeName = reader["inc_emp_nm"] == DBNull.Value ? "" : reader["inc_emp_nm"].ToString(),
                                ReportedByEmployeeName = reader["inc_rpt_by"] == DBNull.Value ? "" : reader["inc_rpt_by"].ToString(),
                                ReportedTime = reader["inc_rpt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["inc_rpt_dt"],
                                IncidentStatus = reader["inc_sts"] == DBNull.Value ? string.Empty : reader["inc_sts"].ToString(),
                                IsFalseNegative = reader["inc_isfn"] == DBNull.Value ? false : (bool)reader["inc_isfn"],
                                ServiceSystemId = reader["inc_sys_id"] == DBNull.Value ? 0 : (int)reader["inc_sys_id"],
                                ServiceSystemName = reader["inc_sys_nm"] == DBNull.Value ? "" : reader["inc_sys_nm"].ToString(),

                                LocationId = reader["inc_loc_id"] == DBNull.Value ? 0 : (int)reader["inc_loc_id"],
                                LocationName = reader["inc_loc_nm"] == DBNull.Value ? "" : reader["inc_loc_nm"].ToString(),
                                UnitId = reader["inc_unit_id"] == DBNull.Value ? 0 : (int)reader["inc_unit_id"],
                                UnitName = reader["inc_unit_nm"] == DBNull.Value ? "" : reader["inc_unit_nm"].ToString(),
                                DepartmentId = reader["inc_dept_id"] == DBNull.Value ? 0 : (int)reader["inc_dept_id"],
                                DepartmentName = reader["inc_dept_nm"] == DBNull.Value ? "" : reader["inc_dept_nm"].ToString(),
                                Severity = reader["inc_svrt"] == DBNull.Value ? 0 : (int)reader["inc_svrt"],
                                SeverityDescription = reader["inc_svrt_desc"] == DBNull.Value ? "" : reader["inc_svrt_desc"].ToString(),
                                
                                IsAssigned = reader["is_assgnd"] == DBNull.Value ? false : (bool)reader["is_assgnd"],
                                AssignedToName = reader["assgnd_to"] == DBNull.Value ? string.Empty : reader["assgnd_to"].ToString(),
                                AssignedTime = reader["assgnd_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["assgnd_dt"],

                                ServiceCenterId = reader["srv_cntr_id"] == DBNull.Value ? "" : reader["srv_cntr_id"].ToString(),
                                ServiceCenterName = reader["tm_nm"] == DBNull.Value ? string.Empty : reader["tm_nm"].ToString(),
                            
                                ConfirmedResolved = reader["res_cnfmd"] == DBNull.Value ? false : (bool)reader["res_cnfmd"],
                                ConfirmedBy = reader["cnfmd_by"] == DBNull.Value ? string.Empty : reader["cnfmd_by"].ToString(),
                                ConfirmedTime = reader["cnfmd_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["cnfmd_dt"],
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return incidentsList;
        }
        public async Task<List<ServiceIncident>> GetServiceIncidentsByTeamMemberIdAsync(string teamMemberId, DateTime? startDate, DateTime? endDate)
        {
            List<ServiceIncident> incidentsList = new List<ServiceIncident>();
            if (startDate == null) { startDate = DateTime.Now.AddMonths(-6); }
            if (endDate == null) { endDate = DateTime.Now.AddDays(1); }

            string start_date = startDate.Value.ToString("yyyy-MM-dd");
            string end_date = endDate.Value.ToString("yyyy-MM-dd");

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT i.inc_id, i.inc_desc, i.inc_imp, i.inc_dt, i.inc_emp_id, i.inc_rpt_by, ");
            sb.Append("i.inc_rpt_dt, i.inc_sts, i.inc_isfn, i.inc_sys_id, i.inc_loc_id, i.inc_unit_id, ");
            sb.Append("i.inc_svrt, i.is_assgnd, i.inc_dept_id, i.inc_no, i.srv_cntr_id, i.res_cnfmd, ");
            sb.Append("i.cnfmd_by, i.cnfmd_dt, i.assgnd_to, i.assgnd_dt, t.tm_nm, s.inc_sys_nm, ");
            sb.Append("CASE i.inc_svrt WHEN 0 THEN 'Low' ");
            sb.Append("WHEN 1 THEN 'Medium' ");
            sb.Append("WHEN 2 THEN 'High' ");
            sb.Append("WHEN 3 THEN 'Critical' END AS inc_svrt_desc, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id =  i.inc_emp_id) as inc_emp_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = i.inc_unit_id) as inc_unit_nm, ");
            sb.Append("(SELECT deptname FROM public.gst_depts WHERE deptqk = i.inc_dept_id) as inc_dept_nm, ");
            sb.Append("(SELECT locname FROM public.gst_locs WHERE locqk = i.inc_loc_id) as inc_loc_nm ");
            sb.Append("FROM public.srm_inc_inf i ");
            sb.Append("LEFT OUTER JOIN public.srm_inc_sys s ON i.inc_sys_id = s.inc_sys_id ");
            sb.Append("LEFT OUTER JOIN public.gst_tms t ON i.srv_cntr_id = t.tm_id ");

            sb.Append("WHERE (@team_member_id IN (SELECT mbr_id FROM public.gst_tmbrs ");
            sb.Append("WHERE tm_id = i.srv_cntr_id)) OR (i.srv_cntr_id IS NULL) ");

            sb.Append("AND (i.inc_rpt_dt >= to_date(@sdt,'YYYY-MM-DD')) ");
            sb.Append("AND (i.inc_rpt_dt <= to_date(@edt,'YYYY-MM-DD')) ");
            sb.Append("ORDER BY i.inc_id DESC;");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var team_member_id = cmd.Parameters.Add("@team_member_id", NpgsqlDbType.Text);
                    var sdt = cmd.Parameters.Add("@sdt", NpgsqlDbType.Text);
                    var edt = cmd.Parameters.Add("@edt", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    team_member_id.Value = teamMemberId;
                    sdt.Value = start_date;
                    edt.Value = end_date;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            incidentsList.Add(new ServiceIncident
                            {
                                Id = reader["inc_id"] == DBNull.Value ? 0 : (long)reader["inc_id"],
                                Number = reader["inc_no"] == DBNull.Value ? "" : reader["inc_no"].ToString(),
                                Description = reader["inc_desc"] == DBNull.Value ? "" : reader["inc_desc"].ToString(),
                                Impact = reader["inc_imp"] == DBNull.Value ? "" : reader["inc_imp"].ToString(),
                                IncidentTime = reader["inc_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["inc_dt"],
                                IncidentEmployeeId = reader["inc_emp_id"] == DBNull.Value ? "" : reader["inc_emp_id"].ToString(),
                                IncidentEmployeeName = reader["inc_emp_nm"] == DBNull.Value ? "" : reader["inc_emp_nm"].ToString(),
                                ReportedByEmployeeName = reader["inc_rpt_by"] == DBNull.Value ? "" : reader["inc_rpt_by"].ToString(),
                                ReportedTime = reader["inc_rpt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["inc_rpt_dt"],
                                IncidentStatus = reader["inc_sts"] == DBNull.Value ? string.Empty : reader["inc_sts"].ToString(),
                                IsFalseNegative = reader["inc_isfn"] == DBNull.Value ? false : (bool)reader["inc_isfn"],
                                ServiceSystemId = reader["inc_sys_id"] == DBNull.Value ? 0 : (int)reader["inc_sys_id"],
                                ServiceSystemName = reader["inc_sys_nm"] == DBNull.Value ? "" : reader["inc_sys_nm"].ToString(),

                                LocationId = reader["inc_loc_id"] == DBNull.Value ? 0 : (int)reader["inc_loc_id"],
                                LocationName = reader["inc_loc_nm"] == DBNull.Value ? "" : reader["inc_loc_nm"].ToString(),
                                UnitId = reader["inc_unit_id"] == DBNull.Value ? 0 : (int)reader["inc_unit_id"],
                                UnitName = reader["inc_unit_nm"] == DBNull.Value ? "" : reader["inc_unit_nm"].ToString(),
                                DepartmentId = reader["inc_dept_id"] == DBNull.Value ? 0 : (int)reader["inc_dept_id"],
                                DepartmentName = reader["inc_dept_nm"] == DBNull.Value ? "" : reader["inc_dept_nm"].ToString(),
                                Severity = reader["inc_svrt"] == DBNull.Value ? 0 : (int)reader["inc_svrt"],
                                SeverityDescription = reader["inc_svrt_desc"] == DBNull.Value ? "" : reader["inc_svrt_desc"].ToString(),
                                
                                IsAssigned = reader["is_assgnd"] == DBNull.Value ? false : (bool)reader["is_assgnd"],
                                AssignedToName = reader["assgnd_to"] == DBNull.Value ? string.Empty : reader["assgnd_to"].ToString(),
                                AssignedTime = reader["assgnd_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["assgnd_dt"],

                                ServiceCenterId = reader["srv_cntr_id"] == DBNull.Value ? "" : reader["srv_cntr_id"].ToString(),
                                ServiceCenterName = reader["tm_nm"] == DBNull.Value ? string.Empty : reader["tm_nm"].ToString(),

                                ConfirmedResolved = reader["res_cnfmd"] == DBNull.Value ? false : (bool)reader["res_cnfmd"],
                                ConfirmedBy = reader["cnfmd_by"] == DBNull.Value ? string.Empty : reader["cnfmd_by"].ToString(),
                                ConfirmedTime = reader["cnfmd_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["cnfmd_dt"],
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return incidentsList;
        }
        public async Task<List<ServiceIncident>> GetServiceIncidentByIdAsync(long serviceIncidentId)
        {
            List<ServiceIncident> incidentsList = new List<ServiceIncident>();

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT i.inc_id, i.inc_desc, i.inc_imp, i.inc_dt, i.inc_emp_id, i.inc_rpt_by, ");
            sb.Append("i.inc_rpt_dt, i.inc_sts, i.inc_isfn, i.inc_sys_id, i.inc_loc_id, i.inc_unit_id, ");
            sb.Append("i.inc_svrt, i.is_assgnd, i.inc_dept_id, i.inc_no, i.srv_cntr_id, i.res_cnfmd, ");
            sb.Append("i.cnfmd_by, i.cnfmd_dt, i.assgnd_to, i.assgnd_dt, t.tm_nm, s.inc_sys_nm, ");
            sb.Append("CASE i.inc_svrt WHEN 0 THEN 'Low' ");
            sb.Append("WHEN 1 THEN 'Medium' ");
            sb.Append("WHEN 2 THEN 'High' ");
            sb.Append("WHEN 3 THEN 'Critical' END AS inc_svrt_desc, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id =  i.inc_emp_id) as inc_emp_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = i.inc_unit_id) as inc_unit_nm, ");
            sb.Append("(SELECT deptname FROM public.gst_depts WHERE deptqk = i.inc_dept_id) as inc_dept_nm, ");
            sb.Append("(SELECT locname FROM public.gst_locs WHERE locqk = i.inc_loc_id) as inc_loc_nm ");
            sb.Append("FROM public.srm_inc_inf i ");
            sb.Append("LEFT OUTER JOIN public.srm_inc_sys s ON i.inc_sys_id = s.inc_sys_id ");
            sb.Append("LEFT OUTER JOIN public.gst_tms t ON i.srv_cntr_id = t.tm_id ");
            sb.Append("WHERE (i.inc_id = @inc_id) ");
            sb.Append("ORDER BY i.inc_id DESC;");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var inc_id = cmd.Parameters.Add("@inc_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    inc_id.Value = serviceIncidentId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            incidentsList.Add(new ServiceIncident
                            {
                                Id = reader["inc_id"] == DBNull.Value ? 0 : (long)reader["inc_id"],
                                Number = reader["inc_no"] == DBNull.Value ? "" : reader["inc_no"].ToString(),
                                Description = reader["inc_desc"] == DBNull.Value ? "" : reader["inc_desc"].ToString(),
                                Impact = reader["inc_imp"] == DBNull.Value ? "" : reader["inc_imp"].ToString(),
                                IncidentTime = reader["inc_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["inc_dt"],
                                IncidentEmployeeId = reader["inc_emp_id"] == DBNull.Value ? "" : reader["inc_emp_id"].ToString(),
                                IncidentEmployeeName = reader["inc_emp_nm"] == DBNull.Value ? "" : reader["inc_emp_nm"].ToString(),
                                ReportedByEmployeeName = reader["inc_rpt_by"] == DBNull.Value ? "" : reader["inc_rpt_by"].ToString(),
                                ReportedTime = reader["inc_rpt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["inc_rpt_dt"],
                                IncidentStatus = reader["inc_sts"] == DBNull.Value ? string.Empty : reader["inc_sts"].ToString(),
                                IsFalseNegative = reader["inc_isfn"] == DBNull.Value ? false : (bool)reader["inc_isfn"],
                                ServiceSystemId = reader["inc_sys_id"] == DBNull.Value ? 0 : (int)reader["inc_sys_id"],
                                ServiceSystemName = reader["inc_sys_nm"] == DBNull.Value ? "" : reader["inc_sys_nm"].ToString(),

                                LocationId = reader["inc_loc_id"] == DBNull.Value ? 0 : (int)reader["inc_loc_id"],
                                LocationName = reader["inc_loc_nm"] == DBNull.Value ? "" : reader["inc_loc_nm"].ToString(),
                                UnitId = reader["inc_unit_id"] == DBNull.Value ? 0 : (int)reader["inc_unit_id"],
                                UnitName = reader["inc_unit_nm"] == DBNull.Value ? "" : reader["inc_unit_nm"].ToString(),
                                DepartmentId = reader["inc_dept_id"] == DBNull.Value ? 0 : (int)reader["inc_dept_id"],
                                DepartmentName = reader["inc_dept_nm"] == DBNull.Value ? "" : reader["inc_dept_nm"].ToString(),
                                Severity = reader["inc_svrt"] == DBNull.Value ? 0 : (int)reader["inc_svrt"],
                                SeverityDescription = reader["inc_svrt_desc"] == DBNull.Value ? "" : reader["inc_svrt_desc"].ToString(),
                                
                                IsAssigned = reader["is_assgnd"] == DBNull.Value ? false : (bool)reader["is_assgnd"],
                                AssignedToName = reader["assgnd_to"] == DBNull.Value ? string.Empty : reader["assgnd_to"].ToString(),
                                AssignedTime = reader["assgnd_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["assgnd_dt"],

                                ServiceCenterId = reader["srv_cntr_id"] == DBNull.Value ? "" : reader["srv_cntr_id"].ToString(),
                                ServiceCenterName = reader["tm_nm"] == DBNull.Value ? string.Empty : reader["tm_nm"].ToString(),

                                ConfirmedResolved = reader["res_cnfmd"] == DBNull.Value ? false : (bool)reader["res_cnfmd"],
                                ConfirmedBy = reader["cnfmd_by"] == DBNull.Value ? string.Empty : reader["cnfmd_by"].ToString(),
                                ConfirmedTime = reader["cnfmd_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["cnfmd_dt"],
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return incidentsList;
        }
        #endregion

        #endregion

        #region Incident Resolution Write Action Methods
        public async Task<long> AddIncidentResolutionAsync(IncidentResolution resolution)
        {
            long inserted_row_id = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.srm_inc_res(inc_id, res_emp_id, ");
            sb.Append("res_dt, res_desc, res_iscf, res_cfby, res_cfdt, ");
            sb.Append("inc_typ_id, recd_by, recd_dt) VALUES ");
            sb.Append("(@inc_id, @res_emp_id, @res_dt, @res_desc, ");
            sb.Append("@res_iscf, @res_cfby, @res_cfdt, @inc_typ_id, ");
            sb.Append("@recd_by, @recd_dt) ");
            sb.Append("RETURNING inc_res_id; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var inc_id = cmd.Parameters.Add("@inc_id", NpgsqlDbType.Bigint);
                    var res_emp_id = cmd.Parameters.Add("@res_emp_id", NpgsqlDbType.Text);
                    var res_dt = cmd.Parameters.Add("@res_dt", NpgsqlDbType.Timestamp);
                    var res_desc = cmd.Parameters.Add("@res_desc", NpgsqlDbType.Text);
                    var res_iscf = cmd.Parameters.Add("@res_iscf", NpgsqlDbType.Boolean);
                    var res_cfby = cmd.Parameters.Add("@res_cfby", NpgsqlDbType.Text);
                    var res_cfdt = cmd.Parameters.Add("@res_cfdt", NpgsqlDbType.Timestamp);
                    var inc_typ_id = cmd.Parameters.Add("@inc_typ_id", NpgsqlDbType.Integer);
                    var recd_by = cmd.Parameters.Add("@recd_by", NpgsqlDbType.Text);
                    var recd_dt = cmd.Parameters.Add("@recd_dt", NpgsqlDbType.Timestamp);
                    cmd.Prepare();
                    inc_id.Value = resolution.IncidentId;
                    res_emp_id.Value = resolution.ResolvedByEmployeeId;
                    res_dt.Value = resolution.ResolvedTime;
                    res_desc.Value = resolution.ResolutionDescription;
                    res_iscf.Value = resolution.IsConfirmed;
                    res_cfby.Value = resolution.ConfirmedBy ?? (object)DBNull.Value; 
                    res_cfdt.Value = resolution.ConfirmedTime ?? (object)DBNull.Value;
                    inc_typ_id.Value = resolution.ServiceTypeId ?? (object)DBNull.Value;
                    recd_by.Value = resolution.RecordedByEmployeeName;
                    recd_dt.Value = DateTime.Now;
                    var obj = await cmd.ExecuteScalarAsync();
                    inserted_row_id = (long)obj;
                }
                await conn.CloseAsync();
            }
            return inserted_row_id;
        }
        public async Task<bool> UpdateIncidentResolutionAsync(IncidentResolution resolution)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.srm_inc_res SET res_emp_id=@res_emp_id,  ");
            sb.Append("res_dt=@res_dt, res_desc=@res_desc, inc_typ_id=@inc_typ_id, ");
            sb.Append("recd_by=@recd_by, recd_dt=@recd_dt ");
            sb.Append("WHERE (inc_res_id=@inc_res_id); ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var res_emp_id = cmd.Parameters.Add("@res_emp_id", NpgsqlDbType.Text);
                    var res_dt = cmd.Parameters.Add("@res_dt", NpgsqlDbType.Timestamp);
                    var res_desc = cmd.Parameters.Add("@res_desc", NpgsqlDbType.Text);
                    var inc_typ_id = cmd.Parameters.Add("@inc_typ_id", NpgsqlDbType.Integer);
                    var inc_res_id = cmd.Parameters.Add("@inc_res_id", NpgsqlDbType.Bigint);
                    var recd_dt = cmd.Parameters.Add("@recd_dt", NpgsqlDbType.Timestamp);
                    var recd_by = cmd.Parameters.Add("@recd_by", NpgsqlDbType.Text);
                    cmd.Prepare();
                    res_emp_id.Value = resolution.ResolvedByEmployeeId;
                    res_dt.Value = resolution.ResolvedTime ?? DateTime.Now;
                    res_desc.Value = resolution.ResolutionDescription;
                    inc_typ_id.Value = resolution.ServiceTypeId ?? (object)DBNull.Value;
                    inc_res_id.Value = resolution.Id;
                    recd_by.Value = resolution.RecordedByEmployeeName;
                    recd_dt.Value = DateTime.Now;

                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        public async Task<bool> UpdateIncidentResolutionConfirmationAsync(long incidentResolutionId, bool resolutionIsConfirmed, string resolutionConfirmedBy)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.srm_inc_res SET res_iscf=@res_iscf, ");
            sb.Append("res_cfby=@res_cfby, res_cfdt=@res_cfdt ");
            sb.Append("WHERE (inc_res_id=@inc_res_id); ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var res_iscf = cmd.Parameters.Add("@res_iscf", NpgsqlDbType.Boolean);
                    var res_cfdt = cmd.Parameters.Add("@res_cfdt", NpgsqlDbType.Timestamp);
                    var res_cfby = cmd.Parameters.Add("@res_cfby", NpgsqlDbType.Text);
                    var inc_res_id = cmd.Parameters.Add("@inc_res_id", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    res_iscf.Value = resolutionIsConfirmed;
                    res_cfdt.Value = DateTime.Now;
                    res_cfby.Value = resolutionConfirmedBy;
                    inc_res_id.Value = incidentResolutionId;

                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        public async Task<bool> DeleteIncidentResolutionAsync(long incidentResolutionId)
        {
            int rows = 0;
            string query = "DELETE FROM public.srm_inc_res WHERE (inc_res_id=@inc_res_id); ";
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var inc_res_id = cmd.Parameters.Add("@inc_res_id", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    inc_res_id.Value = incidentResolutionId;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }

        #endregion

        #region Incident Resolution Read Action Methods
        public async Task<List<IncidentResolution>> GetIncidentResolutionsByIncidentIdAsync(long incidentId)
        {
            List<IncidentResolution> incidentResolutionsList = new List<IncidentResolution>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT r.inc_res_id, r.inc_id, r.res_emp_id, r.res_dt, r.res_desc, ");
            sb.Append("r.res_iscf, r.res_cfby, r.res_cfdt, r.inc_typ_id, r.recd_by, ");
            sb.Append("r.recd_dt, i.inc_desc, i.inc_no, t.inc_typ_ds, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = r.res_emp_id) as res_emp_nm ");
            sb.Append("FROM public.srm_inc_res r ");
            sb.Append("INNER JOIN public.srm_inc_inf i ON r.inc_id = i.inc_id ");
            sb.Append("LEFT JOIN public.srm_inc_typ t ON r.inc_typ_id = t.inc_typ_id ");
            sb.Append("WHERE (r.inc_id = @inc_id) ORDER BY r.inc_res_id DESC; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var inc_id = cmd.Parameters.Add("@inc_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    inc_id.Value = incidentId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            incidentResolutionsList.Add(new IncidentResolution
                            {
                                Id = reader["inc_res_id"] == DBNull.Value ? 0 : (long)reader["inc_res_id"],
                                ResolvedTime = reader["res_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["res_dt"],
                                ResolutionDescription = reader["res_desc"] == DBNull.Value ? "" : reader["res_desc"].ToString(),
                                ResolvedByEmployeeId = reader["res_emp_id"] == DBNull.Value ? "" : reader["res_emp_id"].ToString(),
                                ResolvedByEmployeeName = reader["res_emp_nm"] == DBNull.Value ? "" : reader["res_emp_nm"].ToString(),
                                IncidentId = reader["inc_id"] == DBNull.Value ? 0 : (long)reader["inc_id"],
                                IncidentNumber = reader["inc_no"] == DBNull.Value ? "" : reader["inc_no"].ToString(),
                                IncidentDescription = reader["inc_desc"] == DBNull.Value ? "" : reader["inc_desc"].ToString(),

                                IsConfirmed = reader["res_iscf"] == DBNull.Value ? false : (bool)reader["res_iscf"],
                                ConfirmedBy = reader["res_cfby"] == DBNull.Value ? string.Empty : reader["res_cfby"].ToString(),
                                ConfirmedTime = reader["res_cfdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["res_cfdt"],

                                ServiceTypeName = reader["inc_typ_ds"] == DBNull.Value ? "" : reader["inc_typ_ds"].ToString(),
                                ServiceTypeId = reader["inc_typ_id"] == DBNull.Value ? 0 : (int)reader["inc_typ_id"],

                                RecordedByEmployeeName = reader["recd_by"] == DBNull.Value ? string.Empty : reader["recd_by"].ToString(),
                                RecordedTime = reader["recd_dt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["recd_dt"],
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return incidentResolutionsList;
        }
        public async Task<List<IncidentResolution>> GetIncidentResolutionsByIdAsync(long incidentResolutionId)
        {
            List<IncidentResolution> incidentResolutionsList = new List<IncidentResolution>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT r.inc_res_id, r.inc_id, r.res_emp_id, r.res_dt, r.res_desc, ");
            sb.Append("r.res_iscf, r.res_cfby, r.res_cfdt, r.inc_typ_id, r.recd_by, ");
            sb.Append("r.recd_dt, i.inc_desc, i.inc_no, t.inc_typ_ds, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = r.res_emp_id) as res_emp_nm ");
            sb.Append("FROM public.srm_inc_res r ");
            sb.Append("INNER JOIN public.srm_inc_inf i ON r.inc_id = i.inc_id ");
            sb.Append("LEFT JOIN public.srm_inc_typ t ON r.inc_typ_id = t.inc_typ_id ");
            sb.Append("WHERE (r.inc_res_id = @inc_res_id) ORDER BY r.inc_res_id; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var inc_res_id = cmd.Parameters.Add("@inc_res_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    inc_res_id.Value = incidentResolutionId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            incidentResolutionsList.Add(new IncidentResolution
                            {
                                Id = reader["inc_res_id"] == DBNull.Value ? 0 : (long)reader["inc_res_id"],
                                ResolvedTime = reader["res_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["res_dt"],
                                ResolutionDescription = reader["res_desc"] == DBNull.Value ? "" : reader["res_desc"].ToString(),
                                ResolvedByEmployeeId = reader["res_emp_id"] == DBNull.Value ? "" : reader["res_emp_id"].ToString(),
                                ResolvedByEmployeeName = reader["res_emp_nm"] == DBNull.Value ? "" : reader["res_emp_nm"].ToString(),
                                IncidentId = reader["inc_id"] == DBNull.Value ? 0 : (long)reader["inc_id"],
                                IncidentNumber = reader["inc_no"] == DBNull.Value ? "" : reader["inc_no"].ToString(),
                                IncidentDescription = reader["inc_desc"] == DBNull.Value ? "" : reader["inc_desc"].ToString(),

                                IsConfirmed = reader["res_iscf"] == DBNull.Value ? false : (bool)reader["res_iscf"],
                                ConfirmedBy = reader["res_cfby"] == DBNull.Value ? string.Empty : reader["res_cfby"].ToString(),
                                ConfirmedTime = reader["res_cfdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["res_cfdt"],

                                ServiceTypeName = reader["inc_typ_ds"] == DBNull.Value ? "" : reader["inc_typ_ds"].ToString(),
                                ServiceTypeId = reader["inc_typ_id"] == DBNull.Value ? 0 : (int)reader["inc_typ_id"],

                                RecordedByEmployeeName = reader["recd_by"] == DBNull.Value ? string.Empty : reader["recd_by"].ToString(),
                                RecordedTime = reader["recd_dt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["recd_dt"],
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return incidentResolutionsList;
        }


        #endregion

        #region Settings Data Access Methods
        #region Service Systems Action Methods
        public async Task<int> AddServiceSystemAsync(ServiceSystem system)
        {
            int inserted_row_id = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.srm_inc_sys(inc_sys_nm) ");
            sb.Append("VALUES (@inc_sys_nm) RETURNING inc_sys_id; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var inc_sys_nm = cmd.Parameters.Add("@inc_sys_nm", NpgsqlDbType.Text);
                    cmd.Prepare();
                    inc_sys_nm.Value = system.Name;
                    var obj = await cmd.ExecuteScalarAsync();
                    inserted_row_id = (int)obj;
                }
                await conn.CloseAsync();
            }
            return inserted_row_id;
        }
        public async Task<bool> UpdateServiceSystemAsync(ServiceSystem system)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.srm_inc_sys SET inc_sys_nm=@inc_sys_nm ");
            sb.Append("WHERE inc_sys_id=@inc_sys_id; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var inc_sys_id = cmd.Parameters.Add("@inc_sys_id", NpgsqlDbType.Integer);
                    var inc_sys_nm = cmd.Parameters.Add("@inc_sys_nm", NpgsqlDbType.Text);
                    cmd.Prepare();
                    inc_sys_id.Value = system.Id;
                    inc_sys_nm.Value = system.Name;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        public async Task<bool> DeleteServiceSystemAsync(int systemId)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("DELETE FROM public.srm_inc_sys ");
            sb.Append("WHERE inc_sys_id=@inc_sys_id; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var inc_sys_id = cmd.Parameters.Add("@inc_sys_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    inc_sys_id.Value = systemId;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }

        public async Task<List<ServiceSystem>> GetServiceSystemByIdAsync(int systemId)
        {
            List<ServiceSystem> serviceSystemList = new List<ServiceSystem>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT inc_sys_id, inc_sys_nm FROM public.srm_inc_sys ");
            sb.Append("WHERE inc_sys_id = @inc_sys_id; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var inc_sys_id = cmd.Parameters.Add("@inc_sys_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    inc_sys_id.Value = systemId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            serviceSystemList.Add(new ServiceSystem
                            {
                                Id = reader["inc_sys_id"] == DBNull.Value ? 0 : (int)reader["inc_sys_id"],
                                Name = reader["inc_sys_nm"] == DBNull.Value ? "" : reader["inc_sys_nm"].ToString(),
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return serviceSystemList;
        }
        public async Task<List<ServiceSystem>> GetServiceSystemsByNameAsync(string name)
        {
            List<ServiceSystem> serviceSystemList = new List<ServiceSystem>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT inc_sys_id, inc_sys_nm FROM public.srm_inc_sys ");
            sb.Append("WHERE LOWER(inc_sys_nm) = LOWER(@inc_sys_nm) ");
            sb.Append("ORDER BY inc_sys_nm DESC;");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var inc_sys_nm = cmd.Parameters.Add("@inc_sys_nm", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    inc_sys_nm.Value = name;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            serviceSystemList.Add(new ServiceSystem
                            {
                                Id = reader["inc_sys_id"] == DBNull.Value ? 0 : (int)reader["inc_sys_id"],
                                Name = reader["inc_sys_nm"] == DBNull.Value ? "" : reader["inc_sys_nm"].ToString(),
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return serviceSystemList;
        }
        public async Task<List<ServiceSystem>> GetServiceSystemsAsync()
        {
            List<ServiceSystem> serviceSystemList = new List<ServiceSystem>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT inc_sys_id, inc_sys_nm FROM public.srm_inc_sys ");
            sb.Append("ORDER BY inc_sys_nm;");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    await cmd.PrepareAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            serviceSystemList.Add(new ServiceSystem
                            {
                                Id = reader["inc_sys_id"] == DBNull.Value ? 0 : (int)reader["inc_sys_id"],
                                Name = reader["inc_sys_nm"] == DBNull.Value ? "" : reader["inc_sys_nm"].ToString(),
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return serviceSystemList;
        }

        #endregion

        #region Service Centers Action Methods
        public async Task<int> AddServiceCenterAsync(ServiceCenter center)
        {
            int inserted_row_id = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.srm_srv_cnt(srv_cnt_nm) ");
            sb.Append("VALUES (@srv_cnt_nm) RETURNING srv_cnt_id; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var srv_cnt_nm = cmd.Parameters.Add("@srv_cnt_nm", NpgsqlDbType.Text);
                    cmd.Prepare();
                    srv_cnt_nm.Value = center.Name;
                    var obj = await cmd.ExecuteScalarAsync();
                    inserted_row_id = (int)obj;
                }
                await conn.CloseAsync();
            }
            return inserted_row_id;
        }
        public async Task<bool> UpdateServiceCenterAsync(ServiceCenter center)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.srm_srv_cnt SET srv_cnt_nm=@srv_cnt_nm ");
            sb.Append("WHERE srv_cnt_id=@srv_cnt_id; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var srv_cnt_id = cmd.Parameters.Add("@srv_cnt_id", NpgsqlDbType.Integer);
                    var srv_cnt_nm = cmd.Parameters.Add("@srv_cnt_nm", NpgsqlDbType.Text);
                    cmd.Prepare();
                    srv_cnt_id.Value = center.Id;
                    srv_cnt_nm.Value = center.Name;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        public async Task<bool> DeleteServiceCenterAsync(int serviceCenterId)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("DELETE FROM public.srm_srv_cnt ");
            sb.Append("WHERE srv_cnt_id=@srv_cnt_id; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var srv_cnt_id = cmd.Parameters.Add("@srv_cnt_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    srv_cnt_id.Value = serviceCenterId;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }

        public async Task<List<ServiceCenter>> GetServiceCenterByIdAsync(int serviceCenterId)
        {
            List<ServiceCenter> serviceCenterList = new List<ServiceCenter>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT srv_cnt_id, srv_cnt_nm FROM public.srm_srv_cnt ");
            sb.Append("WHERE srv_cnt_id = @srv_cnt_id; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var srv_cnt_id = cmd.Parameters.Add("@srv_cnt_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    srv_cnt_id.Value = serviceCenterId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            serviceCenterList.Add(new ServiceCenter
                            {
                                Id = reader["srv_cnt_id"] == DBNull.Value ? 0 : (int)reader["srv_cnt_id"],
                                Name = reader["srv_cnt_nm"] == DBNull.Value ? "" : reader["srv_cnt_nm"].ToString(),
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return serviceCenterList;
        }
        public async Task<List<ServiceCenter>> GetServiceCentersByNameAsync(string serviceCenterName)
        {
            List<ServiceCenter> serviceCenterList = new List<ServiceCenter>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT srv_cnt_id, srv_cnt_nm FROM public.srm_srv_cnt ");
            sb.Append("WHERE LOWER(srv_cnt_nm) = LOWER(@srv_cnt_nm) ");
            sb.Append("ORDER BY srv_cnt_nm DESC;");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var srv_cnt_nm = cmd.Parameters.Add("@srv_cnt_nm", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    srv_cnt_nm.Value = serviceCenterName;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            serviceCenterList.Add(new ServiceCenter
                            {
                                Id = reader["srv_cnt_id"] == DBNull.Value ? 0 : (int)reader["srv_cnt_id"],
                                Name = reader["srv_cnt_nm"] == DBNull.Value ? "" : reader["srv_cnt_nm"].ToString(),
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return serviceCenterList;
        }
        public async Task<List<ServiceCenter>> GetServiceCentersAsync()
        {
            List<ServiceCenter> serviceCenterList = new List<ServiceCenter>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT srv_cnt_id, srv_cnt_nm FROM public.srm_srv_cnt ");
            sb.Append("ORDER BY srv_cnt_nm DESC;");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    await cmd.PrepareAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            serviceCenterList.Add(new ServiceCenter
                            {
                                Id = reader["srv_cnt_id"] == DBNull.Value ? 0 : (int)reader["srv_cnt_id"],
                                Name = reader["srv_cnt_nm"] == DBNull.Value ? "" : reader["srv_cnt_nm"].ToString(),
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return serviceCenterList;
        }

        #endregion

        #region Service Types Action Methods
        public async Task<long> AddServiceTypeAsync(ServiceType serviceType)
        {
            long inserted_row_id = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.srm_inc_typ(inc_typ_ds, inc_sys_id) ");
            sb.Append("VALUES (@inc_typ_ds, @inc_sys_id) RETURNING inc_typ_id; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var inc_typ_ds = cmd.Parameters.Add("@inc_typ_ds", NpgsqlDbType.Text);
                    var inc_sys_id = cmd.Parameters.Add("@inc_sys_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    inc_typ_ds.Value = serviceType.Name;
                    inc_sys_id.Value = serviceType.ServiceSystemId;
                    var obj = await cmd.ExecuteScalarAsync();
                    inserted_row_id = (long)obj;
                }
                await conn.CloseAsync();
            }
            return inserted_row_id;
        }
        public async Task<bool> UpdateServiceTypeAsync(ServiceType serviceType)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.srm_inc_typ SET inc_typ_ds=@inc_typ_ds, ");
            sb.Append("inc_sys_id=@inc_sys_id WHERE (inc_typ_id=@inc_typ_id); ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var inc_typ_ds = cmd.Parameters.Add("@inc_typ_ds", NpgsqlDbType.Text);
                    var inc_sys_id = cmd.Parameters.Add("@inc_sys_id", NpgsqlDbType.Integer);
                    var inc_typ_id = cmd.Parameters.Add("@inc_typ_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    inc_typ_ds.Value = serviceType.Name;
                    inc_sys_id.Value = serviceType.ServiceSystemId ?? (object)DBNull.Value;
                    inc_typ_id.Value = serviceType.Id;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        public async Task<bool> DeleteServiceTypeAsync(int serviceTypeId)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("DELETE FROM public.srm_inc_typ ");
            sb.Append("WHERE inc_typ_id=@inc_typ_id; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var inc_typ_id = cmd.Parameters.Add("@inc_typ_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    inc_typ_id.Value = serviceTypeId;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }

        public async Task<ServiceType> GetServiceTypeByIdAsync(int serviceTypeId)
        {
            List<ServiceType> serviceTypeList = new List<ServiceType>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT t.inc_typ_id, t.inc_typ_ds, t.inc_sys_id, ");
            sb.Append("s.inc_sys_nm FROM public.srm_inc_typ t ");
            sb.Append("INNER JOIN public.srm_inc_sys s ON s.inc_sys_id = t.inc_sys_id ");
            sb.Append("WHERE t.inc_typ_id = @inc_typ_id; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var inc_typ_id = cmd.Parameters.Add("@inc_sys_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    inc_typ_id.Value = serviceTypeId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            serviceTypeList.Add(new ServiceType
                            {
                                Id = reader["inc_typ_id"] == DBNull.Value ? 0 : (int)reader["inc_typ_id"],
                                Name = reader["inc_typ_ds"] == DBNull.Value ? "" : reader["inc_typ_ds"].ToString(),
                                ServiceSystemId = reader["inc_sys_id"] == DBNull.Value ? (int?)null : (int)reader["inc_sys_id"],
                                ServiceSystemName = reader["inc_sys_nm"] == DBNull.Value ? "" : reader["inc_sys_nm"].ToString(),

                            });
                        }
                }
                await conn.CloseAsync();
            }
            return serviceTypeList[0];
        }
        public async Task<ServiceType> GetServiceTypesByNameAsync(string serviceTypeName)
        {
            List<ServiceType> serviceTypeList = new List<ServiceType>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT t.inc_typ_id, t.inc_typ_ds, t.inc_sys_id, ");
            sb.Append("s.inc_sys_nm FROM public.srm_inc_typ t ");
            sb.Append("INNER JOIN public.srm_inc_sys s ON s.inc_sys_id = t.inc_sys_id ");
            sb.Append("WHERE LOWER(t.inc_typ_ds) = LOWER(@inc_typ_ds) ");
            sb.Append("ORDER BY t.inc_typ_ds DESC;");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var inc_typ_ds = cmd.Parameters.Add("@inc_typ_ds", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    inc_typ_ds.Value = serviceTypeName;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            serviceTypeList.Add(new ServiceType
                            {
                                Id = reader["inc_typ_id"] == DBNull.Value ? 0 : (int)reader["inc_typ_id"],
                                Name = reader["inc_typ_ds"] == DBNull.Value ? "" : reader["inc_typ_ds"].ToString(),
                                ServiceSystemId = reader["inc_sys_id"] == DBNull.Value ? (int?)null : (int)reader["inc_sys_id"],
                                ServiceSystemName = reader["inc_sys_nm"] == DBNull.Value ? "" : reader["inc_sys_nm"].ToString(),

                            });
                        }
                }
                await conn.CloseAsync();
            }
            return serviceTypeList[0];
        }
        public async Task<List<ServiceType>> GetServiceTypesAsync()
        {
            List<ServiceType> serviceTypeList = new List<ServiceType>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT t.inc_typ_id, t.inc_typ_ds, t.inc_sys_id, ");
            sb.Append("s.inc_sys_nm FROM public.srm_inc_typ t ");
            sb.Append("INNER JOIN public.srm_inc_sys s ON s.inc_sys_id = t.inc_sys_id ");
            sb.Append("ORDER BY t.inc_typ_ds DESC;");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    await cmd.PrepareAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            serviceTypeList.Add(new ServiceType
                            {
                                Id = reader["inc_typ_id"] == DBNull.Value ? 0 : (int)reader["inc_typ_id"],
                                Name = reader["inc_typ_ds"] == DBNull.Value ? "" : reader["inc_typ_ds"].ToString(),
                                ServiceSystemId = reader["inc_sys_id"] == DBNull.Value ? (int?)null : (int)reader["inc_sys_id"],
                                ServiceSystemName = reader["inc_sys_nm"] == DBNull.Value ? "" : reader["inc_sys_nm"].ToString(),

                            });
                        }
                }
                await conn.CloseAsync();
            }
            return serviceTypeList;

        }

        #endregion

        #region Service Request Note Action Methods
        public async Task<List<ServiceRequestNote>> GetServiceRequestNotesByIncidentIdAsync(long serviceIncidentId)
        {
            List<ServiceRequestNote> serviceRequestNotes = new List<ServiceRequestNote>();
            StringBuilder sb = new StringBuilder();
           
            sb.Append("SELECT nts_id, nts_tm, nts_ds, nts_by, ");
            sb.Append("inc_id, is_ccl, ccl_by, ccl_dt ");
            sb.Append("FROM public.srm_inc_nts ");
            sb.Append("WHERE (inc_id = @inc_id) ORDER BY nts_id DESC; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var inc_id = cmd.Parameters.Add("@inc_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    inc_id.Value = serviceIncidentId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            serviceRequestNotes.Add(new ServiceRequestNote
                            {
                                NoteId = reader["nts_id"] == DBNull.Value ? 0 : (long)reader["nts_id"],
                                NoteTime = reader["nts_tm"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["nts_tm"],
                                NoteContent = reader["nts_ds"] == DBNull.Value ? string.Empty : reader["nts_ds"].ToString(),
                                NoteWrittenBy = reader["nts_by"] == DBNull.Value ? string.Empty : reader["nts_by"].ToString(),
                                ServiceIncidentId = reader["inc_id"] == DBNull.Value ? 0 : (long)reader["inc_id"],
                                IsCancelled = reader["is_ccl"] == DBNull.Value ? false : (bool)reader["is_ccl"],
                                CancelledBy = reader["ccl_by"] == DBNull.Value ? "" : reader["ccl_by"].ToString(),
                                CancelledOn = reader["ccl_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ccl_dt"],
                            });
                        }
                }
            }
            return serviceRequestNotes;
        }
        public async Task<bool> AddNoteAsync(ServiceRequestNote n)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.srm_inc_nts(nts_tm, nts_ds, nts_by, ");
            sb.Append("inc_id, is_ccl, ccl_by, ccl_dt) Values (@nts_tm, @nts_ds, ");
            sb.Append("@nts_by, @inc_id, @is_ccl, @ccl_by, @ccl_dt); ");
            string query = sb.ToString();

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var nts_tm = cmd.Parameters.Add("@nts_tm", NpgsqlDbType.Timestamp);
                    var nts_ds = cmd.Parameters.Add("@nts_ds", NpgsqlDbType.Text);
                    var nts_by = cmd.Parameters.Add("@nts_by", NpgsqlDbType.Text);
                    var inc_id = cmd.Parameters.Add("@tsk_id", NpgsqlDbType.Bigint);
                    var is_ccl = cmd.Parameters.Add("@is_ccl", NpgsqlDbType.Boolean);
                    var ccl_by = cmd.Parameters.Add("@ccl_by", NpgsqlDbType.Text);
                    var ccl_dt = cmd.Parameters.Add("@ccl_dt", NpgsqlDbType.Timestamp);
                    cmd.Prepare();
                    nts_tm.Value = n.NoteTime;
                    nts_ds.Value = n.NoteContent;
                    nts_by.Value = n.NoteWrittenBy ?? (object)DBNull.Value;
                    inc_id.Value = n.ServiceIncidentId;
                    is_ccl.Value = false;
                    ccl_by.Value = n.CancelledBy ?? (object)DBNull.Value;
                    ccl_dt.Value = n.CancelledOn ?? (object)DBNull.Value;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        public async Task<bool> UpdateServiceRequestNoteToIsCancelledAsync(long serviceRequestNoteId, bool isCancelled, string cancelledBy)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.srm_inc_nts SET is_ccl=@is_ccl, ");
            sb.Append("ccl_by=@ccl_by, ccl_dt=@ccl_dt  ");
            sb.Append("WHERE (nts_id=@nts_id); ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var is_ccl = cmd.Parameters.Add("@is_ccl", NpgsqlDbType.Boolean);
                    var nts_id = cmd.Parameters.Add("@nts_id", NpgsqlDbType.Bigint);
                    var ccl_dt = cmd.Parameters.Add("@ccl_dt", NpgsqlDbType.Timestamp);
                    var ccl_by = cmd.Parameters.Add("@ccl_by", NpgsqlDbType.Text);
                    cmd.Prepare();
                    nts_id.Value = serviceRequestNoteId;
                    is_ccl.Value = isCancelled;
                    ccl_dt.Value = DateTime.Now;
                    ccl_by.Value = cancelledBy ?? (object)DBNull.Value;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        public async Task<bool> DeleteServiceRequestNoteAsync(long serviceRequestNoteId)
        {
            int rows = 0;
            string query = "DELETE FROM public.srm_inc_nts WHERE (nts_id=@nts_id); ";
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var nts_id = cmd.Parameters.Add("@nts_id", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    nts_id.Value = serviceRequestNoteId;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        #endregion

        #region Service Request Activity Log Action Methods
        public async Task<List<ServiceRequestActivity>> GetServiceRequestActivitysByServiceIncidentIdAsync(long serviceIncidentId)
        {
            List<ServiceRequestActivity> activityLogs = new List<ServiceRequestActivity>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT inc_hst_id, inc_hst_tm, inc_hst_ds, ");
            sb.Append("inc_hst_by, inc_id FROM public.srm_inc_hst ");
            sb.Append("WHERE (inc_id = @inc_id) ");
            sb.Append("ORDER BY inc_hst_id DESC; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var inc_id = cmd.Parameters.Add("@inc_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    inc_id.Value = serviceIncidentId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            activityLogs.Add(new ServiceRequestActivity
                            {
                                ActivityHistoryId = reader["inc_hst_id"] == DBNull.Value ? 0 : (long)reader["inc_hst_id"],
                                ServiceIncidentId = reader["inc_id"] == DBNull.Value ? 0 : (long)reader["inc_id"],
                                ActivityTime = reader["inc_hst_tm"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["inc_hst_tm"],
                                ActivityDescription = reader["inc_hst_ds"] == DBNull.Value ? string.Empty : reader["inc_hst_ds"].ToString(),
                                ActivityBy = reader["inc_hst_by"] == DBNull.Value ? string.Empty : reader["inc_hst_by"].ToString(),
                            });
                        }
                }
            }
            return activityLogs;
        }
        public async Task<bool> AddServiceRequestActivityAsync(ServiceRequestActivity log)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.srm_inc_hst(inc_hst_tm, inc_hst_ds, inc_hst_by, inc_id) ");
            sb.Append("VALUES (@inc_hst_tm, @inc_hst_ds, @inc_hst_by, @inc_id );");
            string query = sb.ToString();

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var inc_hst_tm = cmd.Parameters.Add("@inc_hst_tm", NpgsqlDbType.Timestamp);
                    var inc_hst_ds = cmd.Parameters.Add("@inc_hst_ds", NpgsqlDbType.Text);
                    var inc_hst_by = cmd.Parameters.Add("@inc_hst_by", NpgsqlDbType.Text);
                    var inc_id = cmd.Parameters.Add("@inc_id", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    inc_hst_tm.Value = log.ActivityTime;
                    inc_hst_ds.Value = log.ActivityDescription;
                    inc_hst_by.Value = log.ActivityBy;
                    inc_id.Value = log.ServiceIncidentId;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        public async Task<bool> DeleteServiceRequestActivityAsync(long activityLogId)
        {
            int rows = 0;
            string query = "DELETE FROM public.srm_inc_hst WHERE (inc_hst_id = @inc_hst_id);";
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var inc_hst_id = cmd.Parameters.Add("@inc_hst_id", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    inc_hst_id.Value = activityLogId;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        #endregion


        public async Task<List<string>> GetIncidentCodeNumbersByCreatedDateAsync(DateTime createdDate)
        {
            List<string> listOfCodeNumbers = new List<string>();
            string _newNumber = string.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT inc_no FROM public.srm_inc_inf ");
            sb.Append("WHERE date_part('year', inc_rpt_dt) = date_part('year', @inc_rpt_dt) ");
            sb.Append("ORDER BY inc_no DESC; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var inc_rpt_dt = cmd.Parameters.Add("@inc_rpt_dt", NpgsqlDbType.Timestamp);
                    await cmd.PrepareAsync();
                    inc_rpt_dt.Value = createdDate;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            _newNumber = reader["inc_no"] == DBNull.Value ? string.Empty : reader["inc_no"].ToString();
                            listOfCodeNumbers.Add(_newNumber);
                        }
                }
                await conn.CloseAsync();
            }
            return listOfCodeNumbers;
        }


        #endregion

    }
}
