using IntranetPortal.Base.Models.EmployeeRecordModels;
using IntranetPortal.Base.Repositories.ErmRepositories;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Data.Repositories.ErmRepositories
{
    public class EmployeeSeparationRepository : IEmployeeSeparationRepository
    {
        public IConfiguration _config { get; }
        public EmployeeSeparationRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        #region Employees Separations Write Action Methods

        public async Task<bool> AddAsync(EmployeeSeparation e)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.erm_emp_spxs(emp_id, spx_typ_id, ");
            sb.Append("spx_rsn_id, spx_rsn_desc, exp_last_wk_dt, ");
            sb.Append("ntc_srv_dt, cnb_rhd, rtnd_asst, ctd_dt, mdf_dt, ctd_by, ");
            sb.Append("mdf_by, notc_prd_mnth, outs_lv_dys, act_last_wk_dt, ");
            sb.Append("outs_wk_dys, unit_id, dept_id, loc_id) VALUES (" );
            sb.Append("@emp_id, @spx_typ_id, @spx_rsn_id, @spx_rsn_desc,");
            sb.Append("@exp_last_wk_dt, @ntc_srv_dt, @cnb_rhd, @rtnd_asst, ");
            sb.Append("@ctd_dt, @ctd_dt, @ctd_by, @ctd_by, @notc_prd_mnth, ");
            sb.Append("@outs_lv_dys, @act_last_wk_dt, @outs_wk_dys, ");
            sb.Append("@unit_id, @dept_id, @loc_id);");

            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                var spx_typ_id = cmd.Parameters.Add("@spx_typ_id", NpgsqlDbType.Integer);
                var spx_rsn_id = cmd.Parameters.Add("@spx_rsn_id", NpgsqlDbType.Integer);
                var spx_rsn_desc = cmd.Parameters.Add("@spx_rsn_desc", NpgsqlDbType.Text);
                var exp_last_wk_dt = cmd.Parameters.Add("@exp_last_wk_dt", NpgsqlDbType.Date);
                var ntc_srv_dt = cmd.Parameters.Add("@ntc_srv_dt", NpgsqlDbType.Date);
                var cnb_rhd = cmd.Parameters.Add("@cnb_rhd", NpgsqlDbType.Boolean);
                var rtnd_asst = cmd.Parameters.Add("@rtnd_asst", NpgsqlDbType.Boolean);
                var ctd_by = cmd.Parameters.Add("@ctd_by", NpgsqlDbType.Text);
                var ctd_dt = cmd.Parameters.Add("@ctd_dt", NpgsqlDbType.TimestampTz);
                var notc_prd_mnth = cmd.Parameters.Add("@notc_prd_mnth", NpgsqlDbType.Integer);
                var outs_lv_dys = cmd.Parameters.Add("@outs_lv_dys", NpgsqlDbType.Integer);
                var act_last_wk_dt = cmd.Parameters.Add("@act_last_wk_dt", NpgsqlDbType.Date);
                var outs_wk_dys = cmd.Parameters.Add("@outs_wk_dys", NpgsqlDbType.Integer);
                var loc_id = cmd.Parameters.Add("@loc_id", NpgsqlDbType.Integer);
                var dept_id = cmd.Parameters.Add("@dept_id", NpgsqlDbType.Integer);
                var unit_id = cmd.Parameters.Add("@unit_id", NpgsqlDbType.Integer);

                cmd.Prepare();

                emp_id.Value = e.EmployeeId;
                spx_typ_id.Value = e.SeparationTypeId;
                spx_rsn_id.Value = e.SeparationReasonId;
                spx_rsn_desc.Value = e.SeparationReasonExplanation ?? (object)DBNull.Value;
                exp_last_wk_dt.Value = e.ExpectedLastWorkedDate ?? (object)DBNull.Value;
                ntc_srv_dt.Value = e.NoticeServedDate ?? (object)DBNull.Value;
                cnb_rhd.Value = e.EligibleForRehire;
                rtnd_asst.Value = e.ReturnedAssignedAssets;
                ctd_by.Value = e.RecordCreatedBy ?? (object)DBNull.Value;
                ctd_dt.Value = e.RecordCreatedDate ?? (object)DBNull.Value;
                notc_prd_mnth.Value = e.NoticePeriodInMonths;
                outs_lv_dys.Value = e.OutstandingLeaveDays;
                act_last_wk_dt.Value = e.ActualLastWorkedDate ?? (object)DBNull.Value;
                outs_wk_dys.Value = e.OutstandingWorkDays;
                loc_id.Value = e.LocationId;
                dept_id.Value = e.DepartmentId;
                unit_id.Value = e.UnitId;

                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<bool> EditAsync(EmployeeSeparation e)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.erm_emp_spxs SET emp_id=@emp_id, ");
            sb.Append("spx_typ_id=@spx_typ_id, spx_rsn_id=@spx_rsn_id, ");
            sb.Append("spx_rsn_desc=@spx_rsn_desc, ");
            sb.Append("exp_last_wk_dt=@exp_last_wk_dt, ntc_srv_dt=@ntc_srv_dt, ");
            sb.Append("cnb_rhd=@cnb_rhd, rtnd_asst=@rtnd_asst, mdf_dt=@mdf_dt, ");
            sb.Append("mdf_by=@mdf_by, notc_prd_mnth=@notc_prd_mnth, ");
            sb.Append("outs_lv_dys=@outs_lv_dys, act_last_wk_dt=@act_last_wk_dt, ");
            sb.Append("outs_wk_dys=@outs_wk_dys, ");
            sb.Append("loc_id=@loc_id, dept_id=@dept_id, unit_id=@unit_id ");
            sb.Append("WHERE (emp_spx_id=@emp_spx_id); ");

            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var emp_spx_id = cmd.Parameters.Add("@emp_spx_id", NpgsqlDbType.Integer);
                var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                var spx_typ_id = cmd.Parameters.Add("@spx_typ_id", NpgsqlDbType.Integer);
                var spx_rsn_id = cmd.Parameters.Add("@spx_rsn_id", NpgsqlDbType.Integer);
                var spx_rsn_desc = cmd.Parameters.Add("@spx_rsn_desc", NpgsqlDbType.Text);
                var exp_last_wk_dt = cmd.Parameters.Add("@exp_last_wk_dt", NpgsqlDbType.Date);
                var ntc_srv_dt = cmd.Parameters.Add("@ntc_srv_dt", NpgsqlDbType.Date);
                var cnb_rhd = cmd.Parameters.Add("@cnb_rhd", NpgsqlDbType.Boolean);
                var rtnd_asst = cmd.Parameters.Add("@rtnd_asst", NpgsqlDbType.Boolean);
                var mdf_by = cmd.Parameters.Add("@mdf_by", NpgsqlDbType.Text);
                var mdf_dt = cmd.Parameters.Add("@mdf_dt", NpgsqlDbType.TimestampTz);
                var notc_prd_mnth = cmd.Parameters.Add("@notc_prd_mnth", NpgsqlDbType.Integer);
                var outs_lv_dys = cmd.Parameters.Add("@outs_lv_dys", NpgsqlDbType.Integer);
                var act_last_wk_dt = cmd.Parameters.Add("@act_last_wk_dt", NpgsqlDbType.Date);
                var outs_wk_dys = cmd.Parameters.Add("@outs_wk_dys", NpgsqlDbType.Integer);
                var unit_id = cmd.Parameters.Add("@unit_id", NpgsqlDbType.Integer);
                var dept_id = cmd.Parameters.Add("@dept_id", NpgsqlDbType.Integer);
                var loc_id = cmd.Parameters.Add("@loc_id", NpgsqlDbType.Integer);

                cmd.Prepare();

                emp_spx_id.Value = e.EmployeeSeparationId;
                emp_id.Value = e.EmployeeId;
                spx_typ_id.Value = e.SeparationTypeId;
                spx_rsn_id.Value = e.SeparationReasonId;
                spx_rsn_desc.Value = e.SeparationReasonExplanation ?? (object)DBNull.Value;
                exp_last_wk_dt.Value = e.ExpectedLastWorkedDate ?? (object)DBNull.Value;
                ntc_srv_dt.Value = e.NoticeServedDate ?? (object)DBNull.Value;
                cnb_rhd.Value = e.EligibleForRehire;
                rtnd_asst.Value = e.ReturnedAssignedAssets;
                mdf_by.Value = e.RecordCreatedBy ?? (object)DBNull.Value;
                mdf_dt.Value = e.RecordCreatedDate ?? (object)DBNull.Value;
                notc_prd_mnth.Value = e.NoticePeriodInMonths;
                outs_lv_dys.Value = e.OutstandingLeaveDays;
                act_last_wk_dt.Value = e.ActualLastWorkedDate ?? (object)DBNull.Value;
                outs_wk_dys.Value = e.OutstandingWorkDays;
                loc_id.Value = e.LocationId;
                dept_id.Value = e.DepartmentId;
                unit_id.Value = e.UnitId;

                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (id < 1) { return false; }
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("DELETE FROM public.erm_emp_spxs ");
            sb.Append("WHERE (emp_spx_id=@emp_spx_id); ");
            string query = sb.ToString();

            await conn.OpenAsync();
            //Delete data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var emp_spx_id = cmd.Parameters.Add("@emp_spx_id", NpgsqlDbType.Integer);
                cmd.Prepare();
                emp_spx_id.Value = id;

                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(string employeeId, DateTime exitDate)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("DELETE FROM public.erm_emp_spxs ");
            sb.Append("WHERE LOWER(emp_id) = LOWER(@emp_id) ");
            sb.Append("AND (spx_dt = @spx_dt);");
            string query = sb.ToString();

            await conn.OpenAsync();
            //Delete data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                var spx_dt = cmd.Parameters.Add("@spx_dt", NpgsqlDbType.Date);
                cmd.Prepare();
                emp_id.Value = employeeId;
                spx_dt.Value = exitDate;

                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        #endregion

        #region Employees Separations Read Action Methods

        public async Task<List<EmployeeSeparation>> GetByEmployeeIdAsync(string employeeId)
        {
            if (String.IsNullOrEmpty(employeeId)) { throw new ArgumentNullException(nameof(employeeId), "The required parameter [employeeId] is missing or has in invalid value."); }
            List<EmployeeSeparation> employeeSeparations = new List<EmployeeSeparation>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT s.emp_spx_id, s.emp_id, s.spx_typ_id, s.spx_rsn_id, ");
            sb.Append("s.spx_rsn_desc, s.exp_last_wk_dt, s.ntc_srv_dt, ");
            sb.Append("s.cnb_rhd, s.rtnd_asst, s.ctd_dt, s.mdf_dt, s.ctd_by, s.mdf_by, ");
            sb.Append("s.notc_prd_mnth, s.outs_lv_dys, s.act_last_wk_dt, s.outs_wk_dys, ");
            sb.Append("s.loc_id, s.dept_id, s.unit_id, t.spx_typ_ds, r.spx_rsn_ds, ");
            sb.Append("l.locname, d.deptname, u.unitname, ");

            sb.Append("(SELECT COALESCE(SUM(o.amt_blc), 0) ");
            sb.Append("FROM public.erm_spx_outstdns o ");
            sb.Append("WHERE (o.emp_spx_id = s.emp_spx_id) ");
            sb.Append("AND (o.outstdn_typ_id = 0)) amt_owd, ");

            sb.Append("(SELECT COALESCE(SUM(b.amt_blc), 0 ) ");
            sb.Append("FROM public.erm_spx_outstdns b ");
            sb.Append("WHERE (b.emp_spx_id = s.emp_spx_id) ");
            sb.Append("AND (b.outstdn_typ_id = 1)) amt_indbt, ");

            sb.Append("(SELECT COALESCE(COUNT(pymnt_id),0) ");
            sb.Append("FROM public.erm_spx_pymnts ");
            sb.Append("WHERE emp_spx_id = s.emp_spx_id) pymnt_no, ");
            
            sb.Append("(SELECT COALESCE(COUNT(outstdn_id),0) ");
            sb.Append("FROM public.erm_spx_outstdns ");
            sb.Append("WHERE emp_spx_id = s.emp_spx_id) outstdn_no, ");

            sb.Append("(SELECT fullname FROM public.gst_prsns  ");
            sb.Append("WHERE id = s.emp_id) as emp_nm FROM public.erm_emp_spxs s ");
            sb.Append("INNER JOIN public.erm_spx_typs t ON s.spx_typ_id = t.spx_typ_id ");
            sb.Append("LEFT OUTER JOIN public.gst_locs l ON s.loc_id = l.locqk ");
            sb.Append("LEFT OUTER JOIN public.gst_depts d ON s.dept_id = d.deptqk ");
            sb.Append("LEFT OUTER JOIN public.gst_units u ON s.unit_id = u.unitqk ");
            sb.Append("LEFT OUTER JOIN public.erm_spx_rsns r ON s.spx_rsn_id = r.spx_rsn_id ");
            sb.Append("WHERE LOWER(s.emp_id) = LOWER(@emp_id) ");
            sb.Append("ORDER BY emp_spx_id; ");

            query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                emp_id.Value = employeeId;
                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        employeeSeparations.Add(new EmployeeSeparation
                        {
                            EmployeeSeparationId = reader["emp_spx_id"] == DBNull.Value ? 0 : (int)reader["emp_spx_id"],
                            EmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : (reader["emp_id"]).ToString(),
                            EmployeeName = reader["emp_nm"] == DBNull.Value ? string.Empty : (reader["emp_nm"]).ToString(),
                            SeparationTypeId = reader["spx_typ_id"] == DBNull.Value ? 0 : (int)(reader["spx_typ_id"]),
                            SeparationTypeDescription = reader["spx_typ_ds"] == DBNull.Value ? string.Empty : reader["spx_typ_ds"].ToString(),
                            SeparationReasonId = reader["spx_rsn_id"] == DBNull.Value ? 0 : (int)(reader["spx_rsn_id"]),
                            SeparationReasonDescription = reader["spx_rsn_ds"] == DBNull.Value ? string.Empty : reader["spx_rsn_ds"].ToString(),
                            SeparationReasonExplanation = reader["spx_rsn_desc"] == DBNull.Value ? string.Empty : reader["spx_rsn_desc"].ToString(),
                            ExpectedLastWorkedDate = reader["exp_last_wk_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["exp_last_wk_dt"],
                            ActualLastWorkedDate = reader["act_last_wk_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["act_last_wk_dt"],
                            NoticeServedDate = reader["ntc_srv_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ntc_srv_dt"],
                            EligibleForRehire = reader["cnb_rhd"] == DBNull.Value ? false : (bool)reader["cnb_rhd"],
                            ReturnedAssignedAssets = reader["rtnd_asst"] == DBNull.Value ? false : (bool)reader["rtnd_asst"],
                            RecordModifiedBy = reader["mdf_by"] == DBNull.Value ? string.Empty : reader["mdf_by"].ToString(),
                            RecordModifiedDate = reader["mdf_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mdf_dt"],
                            RecordCreatedBy = reader["ctd_by"] == DBNull.Value ? string.Empty : reader["ctd_by"].ToString(),
                            RecordCreatedDate = reader["ctd_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ctd_dt"],
                            IsIndebted = (decimal)reader["amt_indbt"] == 0.00M ? false : true,
                            IsOwed = (decimal)reader["amt_owd"] == 0.00M ? false : true,

                            HasOutstandings = (long)reader["outstdn_no"] == 0 ? false : true,
                            HasPayments = (long)reader["pymnt_no"] == 0 ? false : true,

                            NoticePeriodInMonths = reader["notc_prd_mnth"] == DBNull.Value ? 0 : (int)reader["notc_prd_mnth"],
                            OutstandingLeaveDays = reader["outs_lv_dys"] == DBNull.Value ? 0 : (int)reader["outs_lv_dys"],
                            OutstandingWorkDays = reader["outs_wk_dys"] == DBNull.Value ? 0 : (int)reader["outs_wk_dys"],
                            UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                            UnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                            DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                            DepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                            LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                            LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),

                        });
                    }
            }
            await conn.CloseAsync();
            return employeeSeparations;
        }

        public async Task<List<EmployeeSeparation>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            string StartDate = startDate.ToString("yyyy-MM-dd");
            string EndDate = endDate.ToString("yyyy-MM-dd");

            List<EmployeeSeparation> employeeSeparations = new List<EmployeeSeparation>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT s.emp_spx_id, s.emp_id, s.spx_typ_id, s.spx_rsn_id, ");
            sb.Append("s.spx_rsn_desc, s.exp_last_wk_dt, s.ntc_srv_dt, ");
            sb.Append("s.cnb_rhd, s.rtnd_asst, s.ctd_dt, s.mdf_dt, s.ctd_by, s.mdf_by, ");
            sb.Append("s.notc_prd_mnth, s.outs_lv_dys, s.act_last_wk_dt, s.outs_wk_dys, ");
            sb.Append("s.loc_id, s.dept_id, s.unit_id, t.spx_typ_ds, r.spx_rsn_ds, ");
            sb.Append("l.locname, d.deptname, u.unitname, ");

            sb.Append("(SELECT COALESCE(SUM(o.amt_blc), 0) ");
            sb.Append("FROM public.erm_spx_outstdns o ");
            sb.Append("WHERE (o.emp_spx_id = s.emp_spx_id) ");
            sb.Append("AND (o.outstdn_typ_id = 0)) amt_owd, ");

            sb.Append("(SELECT COALESCE(SUM(b.amt_blc), 0 ) ");
            sb.Append("FROM public.erm_spx_outstdns b ");
            sb.Append("WHERE (b.emp_spx_id = s.emp_spx_id) ");
            sb.Append("AND (b.outstdn_typ_id = 1)) amt_indbt, ");

            sb.Append("(SELECT COALESCE(COUNT(pymnt_id),0) ");
            sb.Append("FROM public.erm_spx_pymnts ");
            sb.Append("WHERE emp_spx_id = s.emp_spx_id) pymnt_no, ");

            sb.Append("(SELECT COALESCE(COUNT(outstdn_id),0) ");
            sb.Append("FROM public.erm_spx_outstdns ");
            sb.Append("WHERE emp_spx_id = s.emp_spx_id) outstdn_no, ");

            sb.Append("(SELECT fullname FROM public.gst_prsns  ");
            sb.Append("WHERE id = s.emp_id) as emp_nm FROM public.erm_emp_spxs s ");
            sb.Append("INNER JOIN public.erm_spx_typs t ON s.spx_typ_id = t.spx_typ_id ");
            sb.Append("LEFT OUTER JOIN public.gst_locs l ON s.loc_id = l.locqk ");
            sb.Append("LEFT OUTER JOIN public.gst_depts d ON s.dept_id = d.deptqk ");
            sb.Append("LEFT OUTER JOIN public.gst_units u ON s.unit_id = u.unitqk ");
            sb.Append("LEFT OUTER JOIN public.erm_spx_rsns r ON s.spx_rsn_id = r.spx_rsn_id ");
            sb.Append("WHERE (s.act_last_wk_dt >= to_date(@start_date, 'YYYY-MM-DD')) ");
            sb.Append("AND (s.act_last_wk_dt <= to_date(@end_date, 'YYYY-MM-DD')) ");
            sb.Append("OR (s.act_last_wk_dt IS NULL) ");
            sb.Append("ORDER BY s.act_last_wk_dt DESC; ");

            query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var start_date = cmd.Parameters.Add("@start_date", NpgsqlDbType.Text);
                var end_date = cmd.Parameters.Add("@end_date", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                start_date.Value = StartDate;
                end_date.Value = EndDate;
                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        employeeSeparations.Add(new EmployeeSeparation
                        {
                            EmployeeSeparationId = reader["emp_spx_id"] == DBNull.Value ? 0 : (int)reader["emp_spx_id"],
                            EmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : (reader["emp_id"]).ToString(),
                            EmployeeName = reader["emp_nm"] == DBNull.Value ? string.Empty : (reader["emp_nm"]).ToString(),
                            SeparationTypeId = reader["spx_typ_id"] == DBNull.Value ? 0 : (int)(reader["spx_typ_id"]),
                            SeparationTypeDescription = reader["spx_typ_ds"] == DBNull.Value ? string.Empty : reader["spx_typ_ds"].ToString(),
                            SeparationReasonId = reader["spx_rsn_id"] == DBNull.Value ? 0 : (int)(reader["spx_rsn_id"]),
                            SeparationReasonDescription = reader["spx_rsn_ds"] == DBNull.Value ? string.Empty : reader["spx_rsn_ds"].ToString(),
                            SeparationReasonExplanation = reader["spx_rsn_desc"] == DBNull.Value ? string.Empty : reader["spx_rsn_desc"].ToString(),
                            ExpectedLastWorkedDate = reader["exp_last_wk_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["exp_last_wk_dt"],
                            ActualLastWorkedDate = reader["act_last_wk_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["act_last_wk_dt"],
                            NoticeServedDate = reader["ntc_srv_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ntc_srv_dt"],
                            EligibleForRehire = reader["cnb_rhd"] == DBNull.Value ? false : (bool)reader["cnb_rhd"],
                            ReturnedAssignedAssets = reader["rtnd_asst"] == DBNull.Value ? false : (bool)reader["rtnd_asst"],
                            RecordModifiedBy = reader["mdf_by"] == DBNull.Value ? string.Empty : reader["mdf_by"].ToString(),
                            RecordModifiedDate = reader["mdf_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mdf_dt"],
                            RecordCreatedBy = reader["ctd_by"] == DBNull.Value ? string.Empty : reader["ctd_by"].ToString(),
                            RecordCreatedDate = reader["ctd_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ctd_dt"],
                            
                            IsIndebted = (decimal)reader["amt_indbt"] == 0.00M ? false : true,
                            IsOwed = (decimal)reader["amt_owd"] == 0.00M ? false : true,
                            HasOutstandings = (long)reader["outstdn_no"] == 0 ? false : true,
                            HasPayments = (long)reader["pymnt_no"] == 0 ? false : true,

                            NoticePeriodInMonths = reader["notc_prd_mnth"] == DBNull.Value ? 0 : (int)reader["notc_prd_mnth"],
                            OutstandingLeaveDays = reader["outs_lv_dys"] == DBNull.Value ? 0 : (int)reader["outs_lv_dys"],
                            OutstandingWorkDays = reader["outs_wk_dys"] == DBNull.Value ? 0 : (int)reader["outs_wk_dys"],
                            UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                            UnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                            DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                            DepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                            LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                            LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                        });
                    }
            }
            await conn.CloseAsync();
            return employeeSeparations;
        }

        public async Task<EmployeeSeparation> GetByIdAsync(int id)
        {
            EmployeeSeparation e = new EmployeeSeparation();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT s.emp_spx_id, s.emp_id, s.spx_typ_id, s.spx_rsn_id, ");
            sb.Append("s.spx_rsn_desc, s.exp_last_wk_dt, s.ntc_srv_dt, ");
            sb.Append("s.cnb_rhd, s.rtnd_asst, s.ctd_dt, s.mdf_dt, s.ctd_by, s.mdf_by, ");
            sb.Append("s.notc_prd_mnth, s.outs_lv_dys, s.act_last_wk_dt, s.outs_wk_dys, ");
            sb.Append("s.loc_id, s.dept_id, s.unit_id, t.spx_typ_ds, r.spx_rsn_ds, ");
            sb.Append("l.locname, d.deptname, u.unitname, ");

            sb.Append("(SELECT COALESCE(SUM(o.amt_blc), 0) ");
            sb.Append("FROM public.erm_spx_outstdns o ");
            sb.Append("WHERE (o.emp_spx_id = s.emp_spx_id) ");
            sb.Append("AND (o.outstdn_typ_id = 0)) amt_owd, ");

            sb.Append("(SELECT COALESCE(SUM(b.amt_blc), 0 ) ");
            sb.Append("FROM public.erm_spx_outstdns b ");
            sb.Append("WHERE (b.emp_spx_id = s.emp_spx_id) ");
            sb.Append("AND (b.outstdn_typ_id = 1)) amt_indbt, ");

            sb.Append("(SELECT COALESCE(COUNT(pymnt_id),0) ");
            sb.Append("FROM public.erm_spx_pymnts ");
            sb.Append("WHERE emp_spx_id = s.emp_spx_id) pymnt_no, ");

            sb.Append("(SELECT COALESCE(COUNT(outstdn_id),0) ");
            sb.Append("FROM public.erm_spx_outstdns ");
            sb.Append("WHERE emp_spx_id = s.emp_spx_id) outstdn_no, ");

            sb.Append("(SELECT fullname FROM public.gst_prsns  ");
            sb.Append("WHERE id = s.emp_id) as emp_nm FROM public.erm_emp_spxs s ");
            sb.Append("LEFT OUTER JOIN public.erm_spx_typs t ON s.spx_typ_id = t.spx_typ_id ");
            sb.Append("LEFT OUTER JOIN public.gst_locs l ON s.loc_id = l.locqk ");
            sb.Append("LEFT OUTER JOIN public.gst_depts d ON s.dept_id = d.deptqk ");
            sb.Append("LEFT OUTER JOIN public.gst_units u ON s.unit_id = u.unitqk ");
            sb.Append("LEFT OUTER JOIN public.erm_spx_rsns r ON s.spx_rsn_id = r.spx_rsn_id ");
            sb.Append("WHERE s.emp_spx_id = @emp_spx_id; ");

            query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var emp_spx_id = cmd.Parameters.Add("@emp_spx_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                emp_spx_id.Value = id;
                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        e.EmployeeSeparationId = reader["emp_spx_id"] == DBNull.Value ? 0 : (int)reader["emp_spx_id"];
                        e.EmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : (reader["emp_id"]).ToString();
                        e.EmployeeName = reader["emp_nm"] == DBNull.Value ? string.Empty : (reader["emp_nm"]).ToString();
                        e.SeparationTypeId = reader["spx_typ_id"] == DBNull.Value ? 0 : (int)(reader["spx_typ_id"]);
                        e.SeparationTypeDescription = reader["spx_typ_ds"] == DBNull.Value ? string.Empty : reader["spx_typ_ds"].ToString();
                        e.SeparationReasonId = reader["spx_rsn_id"] == DBNull.Value ? 0 : (int)(reader["spx_rsn_id"]);
                        e.SeparationReasonDescription = reader["spx_rsn_ds"] == DBNull.Value ? string.Empty : reader["spx_rsn_ds"].ToString();
                        e.SeparationReasonExplanation = reader["spx_rsn_desc"] == DBNull.Value ? string.Empty : reader["spx_rsn_desc"].ToString();
                        e.ExpectedLastWorkedDate = reader["exp_last_wk_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["exp_last_wk_dt"];
                        e.ActualLastWorkedDate = reader["act_last_wk_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["act_last_wk_dt"];
                        e.NoticeServedDate = reader["ntc_srv_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ntc_srv_dt"];
                        e.EligibleForRehire = reader["cnb_rhd"] == DBNull.Value ? false : (bool)reader["cnb_rhd"];
                        e.ReturnedAssignedAssets = reader["rtnd_asst"] == DBNull.Value ? false : (bool)reader["rtnd_asst"];
                        e.RecordModifiedBy = reader["mdf_by"] == DBNull.Value ? string.Empty : reader["mdf_by"].ToString();
                        e.RecordModifiedDate = reader["mdf_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mdf_dt"];
                        e.RecordCreatedBy = reader["ctd_by"] == DBNull.Value ? string.Empty : reader["ctd_by"].ToString();
                        e.RecordCreatedDate = reader["ctd_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ctd_dt"];
                        
                        e.IsIndebted = (decimal)reader["amt_indbt"] == 0.00M ? false : true;
                        e.IsOwed = (decimal)reader["amt_owd"] == 0.00M ? false : true;
                        e.HasOutstandings = (long)reader["outstdn_no"] == 0 ? false : true;
                        e.HasPayments = (long)reader["pymnt_no"] == 0 ? false : true;

                        e.NoticePeriodInMonths = reader["notc_prd_mnth"] == DBNull.Value ? 0 : (int)reader["notc_prd_mnth"];
                        e.OutstandingLeaveDays = reader["outs_lv_dys"] == DBNull.Value ? 0 : (int)reader["outs_lv_dys"];
                        e.OutstandingWorkDays = reader["outs_wk_dys"] == DBNull.Value ? 0 : (int)reader["outs_wk_dys"];
                        e.UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"];
                        e.UnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString();
                        e.DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"];
                        e.DepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString();
                        e.LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"];
                        e.LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString();
                    }
            }
            await conn.CloseAsync();
            return e;
        }

        public async Task<decimal> GetBalanceOwedBySeparationIdAsync(int employeeSeparationId)
        {
            if (employeeSeparationId < 1) { throw new ArgumentNullException(nameof(employeeSeparationId), "The required parameter [employeeSeparationId] is missing or has in invalid value."); }
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            decimal totalOwed = 0.00M;
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT COALESCE(SUM(o.amt_blc), 0 ) total  ");
            sb.Append("FROM public.erm_spx_outstdns o ");
            sb.Append("WHERE (o.emp_spx_id = @emp_spx_id) ");
            sb.Append("AND (o.outstdn_typ_id = 0);");

            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var emp_spx_id = cmd.Parameters.Add("@emp_spx_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                emp_spx_id.Value = employeeSeparationId;
                var resultObj =  await cmd.ExecuteScalarAsync();
                totalOwed = (decimal)resultObj;
            }
            await conn.CloseAsync();
            return totalOwed;
        }

        public async Task<decimal> GetBalanceIndebtedBySeparationIdAsync(int employeeSeparationId)
        {
            if (employeeSeparationId < 1) { throw new ArgumentNullException(nameof(employeeSeparationId), "The required parameter [employeeSeparationId] is missing or has in invalid value."); }
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            decimal totalIndebted = 0.00M;
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT COALESCE(SUM(o.amt_blc), 0 ) total  ");
            sb.Append("FROM public.erm_spx_outstdns o ");
            sb.Append("WHERE (o.emp_spx_id = @emp_spx_id) ");
            sb.Append("AND (o.outstdn_typ_id = 1);");

            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var emp_spx_id = cmd.Parameters.Add("@emp_spx_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                emp_spx_id.Value = employeeSeparationId;
                var resultObj = await cmd.ExecuteScalarAsync();
                totalIndebted = (decimal)resultObj;
            }
            await conn.CloseAsync();
            return totalIndebted;
        }

        #endregion

        #region Employee Separation Types Action Methods
        public async Task<bool> AddSeparationTypeAsync(EmployeeSeparationType e)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.erm_spx_typs(spx_typ_ds) ");
            sb.Append("VALUES (@spx_typ_ds);");

            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var spx_typ_ds = cmd.Parameters.Add("@spx_typ_ds", NpgsqlDbType.Text);
                cmd.Prepare();
                spx_typ_ds.Value = e.Description;
                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<bool> EditSeparationTypeAsync(EmployeeSeparationType e)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.erm_spx_typs SET spx_typ_ds=@spx_typ_ds ");
            sb.Append("WHERE (spx_typ_id=@spx_typ_id);");

            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var spx_typ_id = cmd.Parameters.Add("@spx_typ_id", NpgsqlDbType.Integer);
                var spx_typ_ds = cmd.Parameters.Add("@spx_typ_ds", NpgsqlDbType.Text);
                cmd.Prepare();
                spx_typ_id.Value = e.Id;
                spx_typ_ds.Value = e.Description;

                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<bool> DeleteSeparationTypeAsync(int id)
        {
            if (id < 1) { return false; }
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("DELETE FROM public.erm_spx_typs ");
            sb.Append("WHERE (spx_typ_id=@spx_typ_id); ");
            string query = sb.ToString();

            await conn.OpenAsync();
            //Delete data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var spx_typ_id = cmd.Parameters.Add("@spx_typ_id", NpgsqlDbType.Integer);
                cmd.Prepare();
                spx_typ_id.Value = id;

                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<EmployeeSeparationType> GetSeparationTypeByIdAsync(int id)
        {
            EmployeeSeparationType e = new EmployeeSeparationType();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT spx_typ_id, spx_typ_ds FROM public.erm_spx_typs ");
            sb.Append("WHERE (spx_typ_id=@spx_typ_id);");

            query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var spx_typ_id = cmd.Parameters.Add("@spx_typ_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                spx_typ_id.Value = id;
                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        e.Id = reader["spx_typ_id"] == DBNull.Value ? 0 : (int)reader["spx_typ_id"];
                        e.Description = reader["spx_typ_ds"] == DBNull.Value ? string.Empty : reader["spx_typ_ds"].ToString();
                    }
            }
            await conn.CloseAsync();
            return e;
        }

        public async Task<List<EmployeeSeparationType>> GetSeparationTypesbyNameAsync(string name)
        {
            List<EmployeeSeparationType> employeeSeparationTypeList = new List<EmployeeSeparationType>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT spx_typ_id, spx_typ_ds FROM public.erm_spx_typs ");
            sb.Append("WHERE LOWER(spx_typ_ds) = LOWER(@spx_typ_ds) ");
            sb.Append("ORDER BY spx_typ_ds;");

            query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var spx_typ_ds = cmd.Parameters.Add("@spx_typ_ds", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                spx_typ_ds.Value = name;

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        employeeSeparationTypeList.Add(
                          new EmployeeSeparationType
                          {
                              Id = reader["spx_typ_id"] == DBNull.Value ? 0 : (int)reader["spx_typ_id"],
                              Description = reader["spx_typ_ds"] == DBNull.Value ? string.Empty : reader["spx_typ_ds"].ToString()
                          });
                    }
            }
            await conn.CloseAsync();
            return employeeSeparationTypeList;
        }

        public async Task<List<EmployeeSeparationType>> GetAllSeparationTypesAsync()
        {
            List<EmployeeSeparationType> employeeSeparationTypeList = new List<EmployeeSeparationType>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT spx_typ_id, spx_typ_ds FROM public.erm_spx_typs ");
            sb.Append("ORDER BY spx_typ_ds;");

            query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                await cmd.PrepareAsync();

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        employeeSeparationTypeList.Add(
                          new EmployeeSeparationType
                          {
                              Id = reader["spx_typ_id"] == DBNull.Value ? 0 : (int)reader["spx_typ_id"],
                              Description = reader["spx_typ_ds"] == DBNull.Value ? string.Empty : reader["spx_typ_ds"].ToString()
                          });
                    }
            }
            await conn.CloseAsync();
            return employeeSeparationTypeList;
        }

        #endregion

        #region Employee Separation Reasons Action Methods
        public async Task<bool> AddSeparationReasonAsync(EmployeeSeparationReason e)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.erm_spx_rsns(spx_rsn_ds) ");
            sb.Append("VALUES (@spx_rsn_ds);");

            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var spx_rsn_ds = cmd.Parameters.Add("@spx_rsn_ds", NpgsqlDbType.Text);
                cmd.Prepare();
                spx_rsn_ds.Value = e.Description;
                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<bool> EditSeparationReasonAsync(EmployeeSeparationReason e)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.erm_spx_rsns SET spx_rsn_ds=@spx_rsn_ds ");
            sb.Append("WHERE (spx_rsn_id=@spx_rsn_id);");

            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var spx_rsn_id = cmd.Parameters.Add("@spx_rsn_id", NpgsqlDbType.Integer);
                var spx_rsn_ds = cmd.Parameters.Add("@spx_rsn_ds", NpgsqlDbType.Text);
                cmd.Prepare();
                spx_rsn_id.Value = e.Id;
                spx_rsn_ds.Value = e.Description;

                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<bool> DeleteSeparationReasonAsync(int id)
        {
            if (id < 1) { return false; }
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("DELETE FROM public.erm_spx_rsns ");
            sb.Append("WHERE (spx_rsn_id=@spx_rsn_id); ");
            string query = sb.ToString();

            await conn.OpenAsync();
            //Delete data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var spx_rsn_id = cmd.Parameters.Add("@spx_rsn_id", NpgsqlDbType.Integer);
                cmd.Prepare();
                spx_rsn_id.Value = id;

                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<EmployeeSeparationReason> GetSeparationReasonByIdAsync(int id)
        {
            EmployeeSeparationReason e = new EmployeeSeparationReason();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT spx_rsn_id, spx_rsn_ds FROM public.erm_spx_rsns ");
            sb.Append("WHERE (spx_rsn_id=@spx_rsn_id);");

            query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var spx_rsn_id = cmd.Parameters.Add("@spx_rsn_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                spx_rsn_id.Value = id;
                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        e.Id = reader["spx_rsn_id"] == DBNull.Value ? 0 : (int)reader["spx_rsn_id"];
                        e.Description = reader["spx_rsn_ds"] == DBNull.Value ? string.Empty : reader["spx_rsn_ds"].ToString();
                    }
            }
            await conn.CloseAsync();
            return e;
        }

        public async Task<List<EmployeeSeparationReason>> GetSeparationReasonsbyNameAsync(string name)
        {
            List<EmployeeSeparationReason> employeeSeparationReasonList = new List<EmployeeSeparationReason>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT spx_rsn_id, spx_rsn_ds FROM public.erm_spx_rsns ");
            sb.Append("WHERE LOWER(spx_rsn_ds) = LOWER(@spx_rsn_ds) ");
            sb.Append("ORDER BY spx_rsn_ds;");

            query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var spx_rsn_ds = cmd.Parameters.Add("@spx_rsn_ds", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                spx_rsn_ds.Value = name;

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        employeeSeparationReasonList.Add(
                          new EmployeeSeparationReason
                          {
                              Id = reader["spx_rsn_id"] == DBNull.Value ? 0 : (int)reader["spx_rsn_id"],
                              Description = reader["spx_rsn_ds"] == DBNull.Value ? string.Empty : reader["spx_rsn_ds"].ToString()
                          });
                    }
            }
            await conn.CloseAsync();
            return employeeSeparationReasonList;
        }

        public async Task<List<EmployeeSeparationReason>> GetAllSeparationReasonsAsync()
        {
            List<EmployeeSeparationReason> employeeSeparationReasonList = new List<EmployeeSeparationReason>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT spx_rsn_id, spx_rsn_ds FROM public.erm_spx_rsns ");
            sb.Append("ORDER BY spx_rsn_ds;");

            query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                await cmd.PrepareAsync();

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        employeeSeparationReasonList.Add(
                          new EmployeeSeparationReason
                          {
                              Id = reader["spx_rsn_id"] == DBNull.Value ? 0 : (int)reader["spx_rsn_id"],
                              Description = reader["spx_rsn_ds"] == DBNull.Value ? string.Empty : reader["spx_rsn_ds"].ToString()
                          });
                    }
            }
            await conn.CloseAsync();
            return employeeSeparationReasonList;
        }

        #endregion
    }
}
