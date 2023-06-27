using IntranetPortal.Base.Enums;
using IntranetPortal.Base.Models.WksModels;
using IntranetPortal.Base.Repositories.WksRepositories;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Data.Repositories.WksRepositories
{
    public class WorkItemRepository : IWorkItemRepository
    {
        public IConfiguration _config { get; }
        public WorkItemRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        #region Work Item Write Actions
        public async Task<bool> AddAsync(WorkItem workItem)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.wkm_prj_inf (prj_cd, prj_ttl, prj_dsc, prj_owner_id, ");
            sb.Append("prj_assigned_to, assigned_dt, prg_stts, at_risk, exp_start_dt, exp_due_dt, ");
            sb.Append("exp_dur_min, act_dur_min, prj_prrty, est_cost_fc, act_cost_fc, curr_cd_fc, ");
            sb.Append("est_cost_lc, act_cost_lc, pct_com, deliverables, instructions, fldr_id, ");
            sb.Append("wksp_id, has_team, is_dlt, dlt_dt, dlt_by, mod_by, mod_dt, crt_by, crt_dt, ");
            sb.Append("unit_id, dept_id, loc_id, mstr_id, typ_id, mstr_prj_id) ");
            sb.Append("VALUES (@prj_cd, @prj_ttl, @prj_dsc, @prj_owner_id, @prj_assigned_to,");
            sb.Append("@assigned_dt, @prg_stts, @at_risk, @exp_start_dt, @exp_due_dt, ");
            sb.Append("@exp_dur_min, @act_dur_min, @prj_prrty, @est_cost_fc, @act_cost_fc, ");
            sb.Append("@curr_cd_fc, @est_cost_lc, @act_cost_lc, @pct_com, @deliverables, ");
            sb.Append("@instructions, @fldr_id, @wksp_id, @has_team, @is_dlt, @dlt_dt, @dlt_by, ");
            sb.Append("@mod_by, @mod_dt, @crt_by, @crt_dt, @unit_id, @dept_id, @loc_id, ");
            sb.Append("@mstr_id, @typ_id, @mstr_prj_id); ");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var prj_cd = cmd.Parameters.Add("@prj_cd", NpgsqlDbType.Text);
                    var prj_ttl = cmd.Parameters.Add("@prj_ttl", NpgsqlDbType.Text);
                    var prj_dsc = cmd.Parameters.Add("@prj_dsc", NpgsqlDbType.Text);
                    var prj_owner_id = cmd.Parameters.Add("@prj_owner_id", NpgsqlDbType.Text);
                    var prj_assigned_to = cmd.Parameters.Add("@prj_assigned_to", NpgsqlDbType.Text);
                    var assigned_dt = cmd.Parameters.Add("@assigned_dt", NpgsqlDbType.TimestampTz);
                    var prg_stts = cmd.Parameters.Add("@prg_stts", NpgsqlDbType.Integer);
                    var at_risk = cmd.Parameters.Add("@at_risk", NpgsqlDbType.Boolean);
                    var exp_start_dt = cmd.Parameters.Add("@exp_start_dt", NpgsqlDbType.TimestampTz);
                    var exp_due_dt = cmd.Parameters.Add("@exp_due_dt", NpgsqlDbType.TimestampTz);
                    var exp_dur_min = cmd.Parameters.Add("@exp_dur_min", NpgsqlDbType.Integer);
                    var act_dur_min = cmd.Parameters.Add("@act_dur_min", NpgsqlDbType.Integer);
                    var prj_prrty = cmd.Parameters.Add("@prj_prrty", NpgsqlDbType.Integer);
                    var est_cost_fc = cmd.Parameters.Add("@est_cost_fc", NpgsqlDbType.Bigint);
                    var act_cost_fc = cmd.Parameters.Add("@act_cost_fc", NpgsqlDbType.Bigint);
                    var curr_cd_fc = cmd.Parameters.Add("@curr_cd_fc", NpgsqlDbType.Text);
                    var est_cost_lc = cmd.Parameters.Add("@est_cost_lc", NpgsqlDbType.Bigint);
                    var act_cost_lc = cmd.Parameters.Add("@act_cost_lc", NpgsqlDbType.Bigint);
                    var pct_com = cmd.Parameters.Add("@pct_com", NpgsqlDbType.Integer);
                    var deliverables = cmd.Parameters.Add("@deliverables", NpgsqlDbType.Text);
                    var instructions = cmd.Parameters.Add("@instructions", NpgsqlDbType.Text);
                    var fldr_id = cmd.Parameters.Add("@fldr_id", NpgsqlDbType.Integer);
                    var wksp_id = cmd.Parameters.Add("@wksp_id", NpgsqlDbType.Integer);
                    var has_team = cmd.Parameters.Add("@has_team", NpgsqlDbType.Boolean);
                    var is_dlt = cmd.Parameters.Add("@is_dlt", NpgsqlDbType.Boolean);
                    var dlt_dt = cmd.Parameters.Add("@dlt_dt", NpgsqlDbType.TimestampTz);
                    var dlt_by = cmd.Parameters.Add("@dlt_by", NpgsqlDbType.Text);
                    var mod_by = cmd.Parameters.Add("@mod_by", NpgsqlDbType.Text);
                    var mod_dt = cmd.Parameters.Add("@mod_dt", NpgsqlDbType.Text);
                    var crt_by = cmd.Parameters.Add("@crt_by", NpgsqlDbType.Text);
                    var crt_dt = cmd.Parameters.Add("@crt_dt", NpgsqlDbType.Text);
                    var unit_id = cmd.Parameters.Add("@unit_id", NpgsqlDbType.Integer);
                    var dept_id = cmd.Parameters.Add("@dept_id", NpgsqlDbType.Integer);
                    var loc_id = cmd.Parameters.Add("@loc_id", NpgsqlDbType.Integer);
                    var mstr_id = cmd.Parameters.Add("@mstr_id", NpgsqlDbType.Integer);
                    var typ_id = cmd.Parameters.Add("@typ_id", NpgsqlDbType.Integer);
                    var mstr_prj_id = cmd.Parameters.Add("@mstr_prj_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    prj_cd.Value = workItem.Code;
                    prj_ttl.Value = workItem.Title;
                    prj_dsc.Value = workItem.Description ?? (object)DBNull.Value;
                    prj_owner_id.Value = workItem.OwnerID;
                    prj_assigned_to.Value = workItem.AssignedToID;
                    assigned_dt.Value = workItem.AssignedTime ?? DateTime.UtcNow;
                    prg_stts.Value = (int)workItem.ProgressStatus;
                    at_risk.Value = workItem.IsAtRisk;
                    exp_start_dt.Value = workItem.ExpectedStartTime ?? DateTime.UtcNow;
                    exp_due_dt.Value = workItem.ExpectedDueTime ?? DateTime.UtcNow;
                    exp_dur_min.Value = workItem.TotalExpectedDurationInMinutes;
                    act_dur_min.Value = workItem.TotalActualDurationInMinutes;
                    prj_prrty.Value = (int)workItem.Priority;
                    est_cost_fc.Value = workItem.EstimatedCostForeignCurrency;
                    act_cost_fc.Value = workItem.ActualCostForeignCurrency;
                    curr_cd_fc.Value = workItem.ForeignCurrencyCode ?? (object)DBNull.Value;
                    est_cost_lc.Value = workItem.EstimatedCostLocalCurrency;
                    act_cost_lc.Value = workItem.ActualCostForeignCurrency;
                    pct_com.Value = workItem.PercentageCompleted;
                    deliverables.Value = workItem.Deliverables ?? (object)DBNull.Value;
                    instructions.Value = workItem.Instructions ?? (object)DBNull.Value;
                    fldr_id.Value = workItem.FolderID;
                    wksp_id.Value = workItem.WorkspaceID;
                    has_team.Value = workItem.HasTeam;
                    is_dlt.Value = workItem.IsDeleted;
                    dlt_dt.Value = workItem.DeletedTime ?? (object)DBNull.Value;
                    dlt_by.Value = workItem.DeletedBy ?? (object)DBNull.Value;
                    mod_by.Value = workItem.LastModifiedBy ?? (object)DBNull.Value;
                    mod_dt.Value = workItem.LastModifiedTime ?? (object)DBNull.Value;
                    crt_by.Value = workItem.CreatedBy ?? (object)DBNull.Value;
                    crt_dt.Value = workItem.CreatedTime ?? (object)DBNull.Value;
                    unit_id.Value = workItem.UnitID ?? (object)DBNull.Value;
                    dept_id.Value = workItem.DepartmentID ?? (object)DBNull.Value;
                    loc_id.Value = workItem.LocationID ?? (object)DBNull.Value;
                    mstr_id.Value = workItem.MasterWorkItemID ?? (object)DBNull.Value;
                    typ_id.Value = (int)workItem.ItemType;
                    mstr_prj_id.Value = workItem.MasterProjectID ?? (object)DBNull.Value;

                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message);
            }
            return rows > 0;
        }

        public async Task<bool> AddWorkItemAsync(WorkItem workItem)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.wkm_wki_inf (wki_cd, wki_ttl, wki_dsc, wki_owner_id, ");
            sb.Append("wki_assigned_to, assigned_dt, prg_stts, at_risk, exp_start_dt, exp_due_dt, ");
            sb.Append("exp_dur_min, act_dur_min, wki_prrty, est_cost_fc, act_cost_fc, curr_cd_fc, ");
            sb.Append("est_cost_lc, act_cost_lc, pct_com, deliverables, instructions, fldr_id, ");
            sb.Append("wksp_id, has_team, is_dlt, dlt_dt, dlt_by, mod_by, mod_dt, crt_by, crt_dt, ");
            sb.Append("unit_id, dept_id, loc_id, mstr_id, typ_id, mstr_prj_id) ");
            sb.Append("VALUES (@wki_cd, @wki_ttl, @wki_dsc, @wki_owner_id, @wki_assigned_to,");
            sb.Append("@assigned_dt, @prg_stts, @at_risk, @exp_start_dt, @exp_due_dt, ");
            sb.Append("@exp_dur_min, @act_dur_min, @wki_prrty, @est_cost_fc, @act_cost_fc, ");
            sb.Append("@curr_cd_fc, @est_cost_lc, @act_cost_lc, @pct_com, @deliverables, ");
            sb.Append("@instructions, @fldr_id, @wksp_id, @has_team, @is_dlt, @dlt_dt, @dlt_by, ");
            sb.Append("@mod_by, @mod_dt, @crt_by, @crt_dt, @unit_id, @dept_id, @loc_id, ");
            sb.Append("@mstr_id, @typ_id, @mstr_prj_id); ");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var wki_cd = cmd.Parameters.Add("@wki_cd", NpgsqlDbType.Text);
                    var wki_ttl = cmd.Parameters.Add("@wki_ttl", NpgsqlDbType.Text);
                    var wki_dsc = cmd.Parameters.Add("@wki_dsc", NpgsqlDbType.Text);
                    var wki_owner_id = cmd.Parameters.Add("@wki_owner_id", NpgsqlDbType.Text);
                    var wki_assigned_to = cmd.Parameters.Add("@wki_assigned_to", NpgsqlDbType.Text);
                    var assigned_dt = cmd.Parameters.Add("@assigned_dt", NpgsqlDbType.TimestampTz);
                    var prg_stts = cmd.Parameters.Add("@prg_stts", NpgsqlDbType.Integer);
                    var at_risk = cmd.Parameters.Add("@at_risk", NpgsqlDbType.Boolean);
                    var exp_start_dt = cmd.Parameters.Add("@exp_start_dt", NpgsqlDbType.TimestampTz);
                    var exp_due_dt = cmd.Parameters.Add("@exp_due_dt", NpgsqlDbType.TimestampTz);
                    var exp_dur_min = cmd.Parameters.Add("@exp_dur_min", NpgsqlDbType.Integer);
                    var act_dur_min = cmd.Parameters.Add("@act_dur_min", NpgsqlDbType.Integer);
                    var wki_prrty = cmd.Parameters.Add("@wki_prrty", NpgsqlDbType.Integer);
                    var est_cost_fc = cmd.Parameters.Add("@est_cost_fc", NpgsqlDbType.Bigint);
                    var act_cost_fc = cmd.Parameters.Add("@act_cost_fc", NpgsqlDbType.Bigint);
                    var curr_cd_fc = cmd.Parameters.Add("@curr_cd_fc", NpgsqlDbType.Text);
                    var est_cost_lc = cmd.Parameters.Add("@est_cost_lc", NpgsqlDbType.Bigint);
                    var act_cost_lc = cmd.Parameters.Add("@act_cost_lc", NpgsqlDbType.Bigint);
                    var pct_com = cmd.Parameters.Add("@pct_com", NpgsqlDbType.Integer);
                    var deliverables = cmd.Parameters.Add("@deliverables", NpgsqlDbType.Text);
                    var instructions = cmd.Parameters.Add("@instructions", NpgsqlDbType.Text);
                    var fldr_id = cmd.Parameters.Add("@fldr_id", NpgsqlDbType.Integer);
                    var wksp_id = cmd.Parameters.Add("@wksp_id", NpgsqlDbType.Integer);
                    var has_team = cmd.Parameters.Add("@has_team", NpgsqlDbType.Boolean);
                    var is_dlt = cmd.Parameters.Add("@is_dlt", NpgsqlDbType.Boolean);
                    var dlt_dt = cmd.Parameters.Add("@dlt_dt", NpgsqlDbType.TimestampTz);
                    var dlt_by = cmd.Parameters.Add("@dlt_by", NpgsqlDbType.Text);
                    var mod_by = cmd.Parameters.Add("@mod_by", NpgsqlDbType.Text);
                    var mod_dt = cmd.Parameters.Add("@mod_dt", NpgsqlDbType.Text);
                    var crt_by = cmd.Parameters.Add("@crt_by", NpgsqlDbType.Text);
                    var crt_dt = cmd.Parameters.Add("@crt_dt", NpgsqlDbType.Text);
                    var unit_id = cmd.Parameters.Add("@unit_id", NpgsqlDbType.Integer);
                    var dept_id = cmd.Parameters.Add("@dept_id", NpgsqlDbType.Integer);
                    var loc_id = cmd.Parameters.Add("@loc_id", NpgsqlDbType.Integer);
                    var mstr_id = cmd.Parameters.Add("@mstr_id", NpgsqlDbType.Integer);
                    var typ_id = cmd.Parameters.Add("@typ_id", NpgsqlDbType.Integer);
                    var mstr_prj_id = cmd.Parameters.Add("@mstr_prj_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    wki_cd.Value = workItem.Code;
                    wki_ttl.Value = workItem.Title;
                    wki_dsc.Value = workItem.Description ?? (object)DBNull.Value;
                    wki_owner_id.Value = workItem.OwnerID;
                    wki_assigned_to.Value = workItem.AssignedToID;
                    assigned_dt.Value = workItem.AssignedTime ?? DateTime.UtcNow;
                    prg_stts.Value = (int)workItem.ProgressStatus;
                    at_risk.Value = workItem.IsAtRisk;
                    exp_start_dt.Value = workItem.ExpectedStartTime ?? DateTime.UtcNow;
                    exp_due_dt.Value = workItem.ExpectedDueTime ?? DateTime.UtcNow;
                    exp_dur_min.Value = workItem.TotalExpectedDurationInMinutes;
                    act_dur_min.Value = workItem.TotalActualDurationInMinutes;
                    wki_prrty.Value = (int)workItem.Priority;
                    est_cost_fc.Value = workItem.EstimatedCostForeignCurrency;
                    act_cost_fc.Value = workItem.ActualCostForeignCurrency;
                    curr_cd_fc.Value = workItem.ForeignCurrencyCode ?? (object)DBNull.Value;
                    est_cost_lc.Value = workItem.EstimatedCostLocalCurrency;
                    act_cost_lc.Value = workItem.ActualCostForeignCurrency;
                    pct_com.Value = workItem.PercentageCompleted;
                    deliverables.Value = workItem.Deliverables ?? (object)DBNull.Value;
                    instructions.Value = workItem.Instructions ?? (object)DBNull.Value;
                    fldr_id.Value = workItem.FolderID;
                    wksp_id.Value = workItem.WorkspaceID;
                    has_team.Value = workItem.HasTeam;
                    is_dlt.Value = workItem.IsDeleted;
                    dlt_dt.Value = workItem.DeletedTime ?? (object)DBNull.Value;
                    dlt_by.Value = workItem.DeletedBy ?? (object)DBNull.Value;
                    mod_by.Value = workItem.LastModifiedBy ?? (object)DBNull.Value;
                    mod_dt.Value = workItem.LastModifiedTime ?? (object)DBNull.Value;
                    crt_by.Value = workItem.CreatedBy ?? (object)DBNull.Value;
                    crt_dt.Value = workItem.CreatedTime ?? (object)DBNull.Value;
                    unit_id.Value = workItem.UnitID ?? (object)DBNull.Value;
                    dept_id.Value = workItem.DepartmentID ?? (object)DBNull.Value;
                    loc_id.Value = workItem.LocationID ?? (object)DBNull.Value;
                    mstr_id.Value = workItem.MasterWorkItemID ?? (object)DBNull.Value;
                    typ_id.Value = (int)workItem.ItemType;
                    mstr_prj_id.Value = workItem.MasterProjectID ?? (object)DBNull.Value;

                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message);
            }
            return rows > 0;
        }


        public async Task<bool> UpdateAsync(WorkItem workItem)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.wkm_wki_inf SET wki_ttl=@wki_ttl, wki_dsc=@wki_dsc, ");
            sb.Append("prg_stts=@prg_stts, at_risk=@at_risk, wki_prrty=@wki_prrty, ");
            sb.Append("pct_com=@pct_com, deliverables=@deliverables, instructions=@instructions, ");
            sb.Append("mod_by=@mod_by, mod_dt=@mod_dt, mstr_id=@mstr_id, mstr_prj_id=@mstr_prj_id ");
            sb.Append("WHERE (wki_id=@wki_id);");
       
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var wki_ttl = cmd.Parameters.Add("@wki_ttl", NpgsqlDbType.Text);
                    var wki_dsc = cmd.Parameters.Add("@wki_dsc", NpgsqlDbType.Text);
                    var prg_stts = cmd.Parameters.Add("@prg_stts", NpgsqlDbType.Integer);
                    var at_risk = cmd.Parameters.Add("@at_risk", NpgsqlDbType.Boolean);
                    var wki_prrty = cmd.Parameters.Add("@wki_prrty", NpgsqlDbType.Integer);
                    var pct_com = cmd.Parameters.Add("@pct_com", NpgsqlDbType.Integer);
                    var deliverables = cmd.Parameters.Add("@deliverables", NpgsqlDbType.Text);
                    var instructions = cmd.Parameters.Add("@instructions", NpgsqlDbType.Text);
                    var mod_by = cmd.Parameters.Add("@mod_by", NpgsqlDbType.Text);
                    var mod_dt = cmd.Parameters.Add("@mod_dt", NpgsqlDbType.TimestampTz);
                    var mstr_id = cmd.Parameters.Add("@mstr_id", NpgsqlDbType.Integer);
                    var mstr_prj_id = cmd.Parameters.Add("@mstr_prj_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    wki_ttl.Value = workItem.Title;
                    wki_dsc.Value = workItem.Description ?? (object)DBNull.Value;
                    prg_stts.Value = (int)workItem.ProgressStatus;
                    at_risk.Value = workItem.IsAtRisk;
                    wki_prrty.Value = (int)workItem.Priority;
                    pct_com.Value = workItem.PercentageCompleted;
                    deliverables.Value = workItem.Deliverables ?? (object)DBNull.Value;
                    instructions.Value = workItem.Instructions ?? (object)DBNull.Value;
                    mod_by.Value = workItem.LastModifiedBy ?? (object)DBNull.Value;
                    mod_dt.Value = workItem.LastModifiedTime ?? (object)DBNull.Value;
                    mstr_id.Value = workItem.MasterWorkItemID ?? (object)DBNull.Value;

                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message);
            }
            return rows > 0;
        }


        public async Task<bool> UpdateToDeletedAsync(int workItemId, string deletedBy)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.wkm_wki_inf SET is_dlt=true, dlt_by=@dlt_by, ");
            sb.Append("dlt_on=@dlt_on WHERE (wki_id = @wki_id);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var wki_id = cmd.Parameters.Add("@wki_id", NpgsqlDbType.Integer);
                    var dlt_by = cmd.Parameters.Add("@dlt_by", NpgsqlDbType.Text);
                    var dlt_on = cmd.Parameters.Add("@dlt_on", NpgsqlDbType.TimestampTz);
                    cmd.Prepare();
                    wki_id.Value = workItemId;
                    dlt_by.Value = deletedBy;
                    dlt_on.Value = DateTime.UtcNow;

                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message);
            }
            return rows > 0;
        }
        #endregion

        #region Work Item Read Actions

        public async Task<IList<WorkItem>> GetByIdAsync(int workItemId)
        {
            IList<WorkItem> workItems = new List<WorkItem>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT p.wki_id, p.wki_cd, p.wki_ttl, p.wki_dsc, p.wki_owner_id, ");
            sb.Append("p.wki_assigned_to, p.assigned_dt, p.prg_stts, p.at_risk, ");
            sb.Append("p.exp_start_dt, p.exp_due_dt, p.exp_dur_min, p.act_dur_min, ");
            sb.Append("p.wki_prrty, p.est_cost_fc, p.act_cost_fc, p.curr_cd_fc, p.est_cost_lc, ");
            sb.Append("p.act_cost_lc, p.pct_com, p.deliverables, p.instructions, p.fldr_id, ");
            sb.Append("p.wksp_id, p.has_team, p.is_dlt, p.dlt_dt, p.dlt_by, p.unit_id, ");
            sb.Append("p.dept_id, p.loc_id, u.unitname, d.deptname, l.locname, s.fullname, ");
            sb.Append("f.title AS fldr_title, w.title AS wksp_title, p.mod_by, ");
            sb.Append("p.mod_dt, p.crt_by, p.crt_dt FROM public.wkm_wki_inf p ");
            sb.Append("INNER JOIN public.wkm_fdr_inf f ON f.id = p.fldr_id ");
            sb.Append("INNER JOIN public.wkm_wsp_inf w ON w.id = p.wksp_id ");
            sb.Append("LEFT OUTER JOIN public.gst_units u ON u.unitqk = p.unit_id ");
            sb.Append("LEFT OUTER JOIN public.gst_depts d ON d.deptqk = p.dept_id ");
            sb.Append("LEFT OUTER JOIN public.gst_locs l ON l.locqk = p.loc_id ");
            sb.Append("LEFT OUTER JOIN public.gst_prsns s ON s.id = p.prj_assigned_to ");
            sb.Append("WHERE (p.wki_id = @wki_id) ");
            sb.Append("ORDER BY p.wki_id;");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var wki_id = cmd.Parameters.Add("@wki_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    wki_id.Value = workItemId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            workItems.Add(new WorkItem
                            {
                                ID = reader["wki_id"] == DBNull.Value ? 0 : (int)reader["wki_id"],
                                Code = reader["wki_cd"] == DBNull.Value ? string.Empty : reader["wki_cd"].ToString(),
                                Title = reader["wki_ttl"] == DBNull.Value ? string.Empty : reader["wki_ttl"].ToString(),
                                Description = reader["wki_dsc"] == DBNull.Value ? string.Empty : reader["wki_dsc"].ToString(),
                                OwnerID = reader["wki_owner_id"] == DBNull.Value ? string.Empty : reader["wki_owner_id"].ToString(),
                                AssignedToID = reader["wki_assigned_to"] == DBNull.Value ? string.Empty : reader["wki_assigned_to"].ToString(),
                                AssignedToName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                                AssignedTime = reader["assigned_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["assigned_dt"],
                                ProgressStatus = reader["prg_stts"] == DBNull.Value ? WorkItemStatus.NotStarted : (WorkItemStatus)reader["prg_stts"],
                                IsAtRisk = reader["at_risk"] == DBNull.Value ? false : (bool)reader["at_risk"],
                                ExpectedStartTime = reader["exp_start_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["exp_start_dt"],
                                ExpectedDueTime = reader["exp_due_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["exp_due_dt"],
                                TotalExpectedDurationInMinutes = reader["exp_dur_min"] == DBNull.Value ? 0 : (long)reader["exp_dur_min"],
                                TotalActualDurationInMinutes = reader["act_dur_min"] == DBNull.Value ? 0 : (long)reader["act_dur_min"],
                                Priority = reader["wki_prrty"] == DBNull.Value ? WorkItemPriority.Normal : (WorkItemPriority)reader["wki_prrty"],
                                EstimatedCostForeignCurrency = reader["est_cost_fc"] == DBNull.Value ? 0 : (long)reader["est_cost_fc"],
                                ActualCostForeignCurrency = reader["act_cost_fc"] == DBNull.Value ? 0 : (long)reader["act_cost_fc"],
                                ForeignCurrencyCode = reader["curr_cd_fc"] == DBNull.Value ? string.Empty : reader["curr_cd_fc"].ToString(),
                                EstimatedCostLocalCurrency = reader["est_cost_lc"] == DBNull.Value ? 0 : (long)reader["est_cost_lc"],
                                ActualCostLocalCurrency = reader["act_cost_lc"] == DBNull.Value ? 0 : (long)reader["act_cost_lc"],
                                PercentageCompleted = reader["pct_com"] == DBNull.Value ? 0 : (int)reader["pct_com"],
                                Deliverables = reader["deliverables"] == DBNull.Value ? string.Empty : reader["deliverables"].ToString(),
                                Instructions = reader["instructions"] == DBNull.Value ? string.Empty : reader["deliverables"].ToString(),
                                FolderID = reader["fldr_id"] == DBNull.Value ? 0 : (int)reader["fldr_id"],
                                FolderTitle = reader["fldr_title"] == DBNull.Value ? string.Empty : reader["fldr_title"].ToString(),
                                WorkspaceID = reader["wksp_id"] == DBNull.Value ? 0 : (int)reader["wksp_id"],
                                WorkspaceTitle = reader["wksp_title"] == DBNull.Value ? string.Empty : reader["wksp_title"].ToString(),
                                HasTeam = reader["has_team"] == DBNull.Value ? false : (bool)reader["has_team"],
                                UnitID = reader["unit_id"] == DBNull.Value ? (int?)null : (int)reader["unit_id"],
                                UnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                                DepartmentID = reader["dept_id"] == DBNull.Value ? (int?)null : (int)reader["dept_id"],
                                DepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                                LocationID = reader["loc_id"] == DBNull.Value ? (int?)null : (int)reader["loc_id"],
                                LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                                IsDeleted = reader["is_dlt"] == DBNull.Value ? false : (bool)reader["is_dlt"],
                                DeletedTime = reader["dlt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dlt_dt"],
                                DeletedBy = reader["dlt_by"] == DBNull.Value ? string.Empty : reader["dlt_by"].ToString(),
                                LastModifiedTime = reader["mod_dt"] == DBNull.Value ? string.Empty : reader["mod_dt"].ToString(),
                                LastModifiedBy = reader["mod_by"] == DBNull.Value ? string.Empty : reader["mod_by"].ToString(),
                                CreatedTime = reader["crt_dt"] == DBNull.Value ? string.Empty : reader["crt_dt"].ToString(),
                                CreatedBy = reader["crt_by"] == DBNull.Value ? string.Empty : reader["crt_by"].ToString(),
                            });
                        }
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message);
            }
            return workItems;
        }

        public async Task<IList<WorkItem>> GetByFolderIdAsync(int folderId)
        {
            IList<WorkItem> workItems = new List<WorkItem>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT p.prj_id, p.prj_cd, p.prj_ttl, p.prj_dsc, p.prj_owner_id, ");
            sb.Append("p.prj_assigned_to, p.assigned_dt, p.prg_stts, p.at_risk, ");
            sb.Append("p.exp_start_dt, p.exp_due_dt, p.exp_dur_min, p.act_dur_min, ");
            sb.Append("p.prj_prrty, p.est_cost_fc, p.act_cost_fc, p.curr_cd_fc, p.est_cost_lc, ");
            sb.Append("p.act_cost_lc, p.pct_com, p.deliverables, p.instructions, p.fldr_id, ");
            sb.Append("p.wksp_id, p.has_team, p.is_dlt, p.dlt_dt, p.dlt_by, p.unit_id, ");
            sb.Append("p.dept_id, p.loc_id, u.unitname, d.deptname, l.locname, s.fullname, ");
            sb.Append("f.title AS fldr_title, w.title AS wksp_title, p.mod_by, ");
            sb.Append("p.mod_dt, p.crt_by, p.crt_dt FROM public.wkm_prj_inf p ");
            sb.Append("INNER JOIN public.wkm_fdr_inf f ON f.id = p.fldr_id ");
            sb.Append("INNER JOIN public.wkm_wsp_inf w ON w.id = p.wksp_id ");
            sb.Append("LEFT OUTER JOIN public.gst_units u ON u.unitqk = p.unit_id ");
            sb.Append("LEFT OUTER JOIN public.gst_depts d ON d.deptqk = p.dept_id ");
            sb.Append("LEFT OUTER JOIN public.gst_locs l ON l.locqk = p.loc_id ");
            sb.Append("LEFT OUTER JOIN public.gst_prsns s ON s.id = p.prj_assigned_to ");
            sb.Append("WHERE (p.fldr_id = @fldr_id) ");
            sb.Append("ORDER BY p.prj_id;");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var fldr_id = cmd.Parameters.Add("@fldr_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    fldr_id.Value = folderId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            workItems.Add(new WorkItem
                            {
                                ID = reader["prj_id"] == DBNull.Value ? 0 : (int)reader["prj_id"],
                                Code = reader["prj_cd"] == DBNull.Value ? string.Empty : reader["prj_cd"].ToString(),
                                Title = reader["prj_ttl"] == DBNull.Value ? string.Empty : reader["prj_ttl"].ToString(),
                                Description = reader["prj_dsc"] == DBNull.Value ? string.Empty : reader["prj_dsc"].ToString(),
                                OwnerID = reader["prj_owner_id"] == DBNull.Value ? string.Empty : reader["prj_owner_id"].ToString(),
                                AssignedToID = reader["prj_assigned_to"] == DBNull.Value ? string.Empty : reader["prj_assigned_to"].ToString(),
                                AssignedToName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                                AssignedTime = reader["assigned_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["assigned_dt"],
                                ProgressStatus = reader["prg_stts"] == DBNull.Value ? WorkItemStatus.NotStarted : (WorkItemStatus)reader["prg_stts"],
                                IsAtRisk = reader["at_risk"] == DBNull.Value ? false : (bool)reader["at_risk"],
                                ExpectedStartTime = reader["exp_start_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["exp_start_dt"],
                                ExpectedDueTime = reader["exp_due_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["exp_due_dt"],
                                TotalExpectedDurationInMinutes = reader["exp_dur_min"] == DBNull.Value ? 0 : (long)reader["exp_dur_min"],
                                TotalActualDurationInMinutes = reader["act_dur_min"] == DBNull.Value ? 0 : (long)reader["act_dur_min"],
                                Priority = reader["prj_prrty"] == DBNull.Value ? WorkItemPriority.Normal : (WorkItemPriority)reader["prj_prrty"],
                                EstimatedCostForeignCurrency = reader["est_cost_fc"] == DBNull.Value ? 0 : (long)reader["est_cost_fc"],
                                ActualCostForeignCurrency = reader["act_cost_fc"] == DBNull.Value ? 0 : (long)reader["act_cost_fc"],
                                ForeignCurrencyCode = reader["curr_cd_fc"] == DBNull.Value ? string.Empty : reader["curr_cd_fc"].ToString(),
                                EstimatedCostLocalCurrency = reader["est_cost_lc"] == DBNull.Value ? 0 : (long)reader["est_cost_lc"],
                                ActualCostLocalCurrency = reader["act_cost_lc"] == DBNull.Value ? 0 : (long)reader["act_cost_lc"],
                                PercentageCompleted = reader["pct_com"] == DBNull.Value ? 0 : (int)reader["pct_com"],
                                Deliverables = reader["deliverables"] == DBNull.Value ? string.Empty : reader["deliverables"].ToString(),
                                Instructions = reader["instructions"] == DBNull.Value ? string.Empty : reader["instructions"].ToString(),
                                FolderID = reader["fldr_id"] == DBNull.Value ? 0 : (int)reader["fldr_id"],
                                FolderTitle = reader["fldr_title"] == DBNull.Value ? string.Empty : reader["fldr_title"].ToString(),
                                WorkspaceID = reader["wksp_id"] == DBNull.Value ? 0 : (int)reader["wksp_id"],
                                WorkspaceTitle = reader["wksp_title"] == DBNull.Value ? string.Empty : reader["wksp_title"].ToString(),
                                HasTeam = reader["has_team"] == DBNull.Value ? false : (bool)reader["has_team"],
                                UnitID = reader["unit_id"] == DBNull.Value ? (int?)null : (int)reader["unit_id"],
                                UnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                                DepartmentID = reader["dept_id"] == DBNull.Value ? (int?)null : (int)reader["dept_id"],
                                DepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                                LocationID = reader["loc_id"] == DBNull.Value ? (int?)null : (int)reader["loc_id"],
                                LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                                IsDeleted = reader["is_dlt"] == DBNull.Value ? false : (bool)reader["is_dlt"],
                                DeletedTime = reader["dlt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dlt_dt"],
                                DeletedBy = reader["dlt_by"] == DBNull.Value ? string.Empty : reader["dlt_by"].ToString(),
                                LastModifiedTime = reader["mod_dt"] == DBNull.Value ? string.Empty : reader["mod_dt"].ToString(),
                                LastModifiedBy = reader["mod_by"] == DBNull.Value ? string.Empty : reader["mod_by"].ToString(),
                                CreatedTime = reader["crt_dt"] == DBNull.Value ? string.Empty : reader["crt_dt"].ToString(),
                                CreatedBy = reader["crt_by"] == DBNull.Value ? string.Empty : reader["crt_by"].ToString(),
                            });
                        }
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message);
            }
            return workItems;
        }

        public async Task<IList<WorkItem>> GetByOwnerIdAsync(string ownerId)
        {
            IList<WorkItem> workItems = new List<WorkItem>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT p.prj_id, p.prj_cd, p.prj_ttl, p.prj_dsc, p.prj_owner_id, ");
            sb.Append("p.prj_assigned_to, p.assigned_dt, p.prg_stts, p.at_risk, ");
            sb.Append("p.exp_start_dt, p.exp_due_dt, p.exp_dur_min, p.act_dur_min, ");
            sb.Append("p.prj_prrty, p.est_cost_fc, p.act_cost_fc, p.curr_cd_fc, p.est_cost_lc, ");
            sb.Append("p.act_cost_lc, p.pct_com, p.deliverables, p.instructions, p.fldr_id, ");
            sb.Append("p.wksp_id, p.has_team, p.is_dlt, p.dlt_dt, p.dlt_by, p.unit_id, ");
            sb.Append("p.dept_id, p.loc_id, u.unitname, d.deptname, l.locname, s.fullname, ");
            sb.Append("f.title AS fldr_title, w.title AS wksp_title, p.mod_by, ");
            sb.Append("p.mod_dt, p.crt_by, p.crt_dt FROM public.wkm_prj_inf p ");
            sb.Append("INNER JOIN public.wkm_fdr_inf f ON f.id = p.fldr_id ");
            sb.Append("INNER JOIN public.wkm_wsp_inf w ON w.id = p.wksp_id ");
            sb.Append("LEFT OUTER JOIN public.gst_units u ON u.unitqk = p.unit_id ");
            sb.Append("LEFT OUTER JOIN public.gst_depts d ON d.deptqk = p.dept_id ");
            sb.Append("LEFT OUTER JOIN public.gst_locs l ON l.locqk = p.loc_id ");
            sb.Append("LEFT OUTER JOIN public.gst_prsns s ON s.id = p.prj_assigned_to ");
            sb.Append("WHERE (p.prj_owner_id = @owner_id) AND (p.is_dlt = false) ");
            sb.Append("ORDER BY p.prj_id;");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var owner_id = cmd.Parameters.Add("@owner_id", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    owner_id.Value = ownerId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            workItems.Add(new WorkItem
                            {
                                ID = reader["prj_id"] == DBNull.Value ? 0 : (int)reader["prj_id"],
                                Code = reader["prj_cd"] == DBNull.Value ? string.Empty : reader["prj_cd"].ToString(),
                                Title = reader["prj_ttl"] == DBNull.Value ? string.Empty : reader["prj_ttl"].ToString(),
                                Description = reader["prj_dsc"] == DBNull.Value ? string.Empty : reader["prj_dsc"].ToString(),
                                OwnerID = reader["prj_owner_id"] == DBNull.Value ? string.Empty : reader["prj_owner_id"].ToString(),
                                AssignedToID = reader["prj_assigned_to"] == DBNull.Value ? string.Empty : reader["prj_assigned_to"].ToString(),
                                AssignedToName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                                AssignedTime = reader["assigned_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["assigned_dt"],
                                ProgressStatus = reader["prg_stts"] == DBNull.Value ? WorkItemStatus.NotStarted : (WorkItemStatus)reader["prg_stts"],
                                IsAtRisk = reader["at_risk"] == DBNull.Value ? false : (bool)reader["at_risk"],
                                ExpectedStartTime = reader["exp_start_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["exp_start_dt"],
                                ExpectedDueTime = reader["exp_due_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["exp_due_dt"],
                                TotalExpectedDurationInMinutes = reader["exp_dur_min"] == DBNull.Value ? 0 : (long)reader["exp_dur_min"],
                                TotalActualDurationInMinutes = reader["act_dur_min"] == DBNull.Value ? 0 : (long)reader["act_dur_min"],
                                Priority = reader["prj_prrty"] == DBNull.Value ? WorkItemPriority.Normal : (WorkItemPriority)reader["prj_prrty"],
                                EstimatedCostForeignCurrency = reader["est_cost_fc"] == DBNull.Value ? 0 : (long)reader["est_cost_fc"],
                                ActualCostForeignCurrency = reader["act_cost_fc"] == DBNull.Value ? 0 : (long)reader["act_cost_fc"],
                                ForeignCurrencyCode = reader["curr_cd_fc"] == DBNull.Value ? string.Empty : reader["curr_cd_fc"].ToString(),
                                EstimatedCostLocalCurrency = reader["est_cost_lc"] == DBNull.Value ? 0 : (long)reader["est_cost_lc"],
                                ActualCostLocalCurrency = reader["act_cost_lc"] == DBNull.Value ? 0 : (long)reader["act_cost_lc"],
                                PercentageCompleted = reader["pct_com"] == DBNull.Value ? 0 : (int)reader["pct_com"],
                                Deliverables = reader["deliverables"] == DBNull.Value ? string.Empty : reader["deliverables"].ToString(),
                                Instructions = reader["instructions"] == DBNull.Value ? string.Empty : reader["deliverables"].ToString(),
                                FolderID = reader["fldr_id"] == DBNull.Value ? 0 : (int)reader["fldr_id"],
                                FolderTitle = reader["fldr_title"] == DBNull.Value ? string.Empty : reader["fldr_title"].ToString(),
                                WorkspaceID = reader["wksp_id"] == DBNull.Value ? 0 : (int)reader["wksp_id"],
                                WorkspaceTitle = reader["wksp_title"] == DBNull.Value ? string.Empty : reader["wksp_title"].ToString(),
                                UnitID = reader["unit_id"] == DBNull.Value ? (int?)null : (int)reader["unit_id"],
                                UnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                                DepartmentID = reader["dept_id"] == DBNull.Value ? (int?)null : (int)reader["dept_id"],
                                DepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                                LocationID = reader["loc_id"] == DBNull.Value ? (int?)null : (int)reader["loc_id"],
                                LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                                HasTeam = reader["has_team"] == DBNull.Value ? false : (bool)reader["has_team"],
                                IsDeleted = reader["is_dlt"] == DBNull.Value ? false : (bool)reader["is_dlt"],
                                DeletedTime = reader["dlt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dlt_dt"],
                                DeletedBy = reader["dlt_by"] == DBNull.Value ? string.Empty : reader["dlt_by"].ToString(),
                                LastModifiedTime = reader["mod_dt"] == DBNull.Value ? string.Empty : reader["mod_dt"].ToString(),
                                LastModifiedBy = reader["mod_by"] == DBNull.Value ? string.Empty : reader["mod_by"].ToString(),
                                CreatedTime = reader["crt_dt"] == DBNull.Value ? string.Empty : reader["crt_dt"].ToString(),
                                CreatedBy = reader["crt_by"] == DBNull.Value ? string.Empty : reader["crt_by"].ToString(),
                            });
                        }
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message);
            }
            return workItems;
        }

        public async Task<IList<WorkItem>> GetByTitleAsync(string workItemTitle)
        {
            IList<WorkItem> workItems = new List<WorkItem>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT p.prj_id, p.prj_cd, p.prj_ttl, p.prj_dsc, p.prj_owner_id, ");
            sb.Append("p.prj_assigned_to, p.assigned_dt, p.prg_stts, p.at_risk, ");
            sb.Append("p.exp_start_dt, p.exp_due_dt, p.exp_dur_min, p.act_dur_min, ");
            sb.Append("p.prj_prrty, p.est_cost_fc, p.act_cost_fc, p.curr_cd_fc, p.est_cost_lc, ");
            sb.Append("p.act_cost_lc, p.pct_com, p.deliverables, p.instructions, p.fldr_id, ");
            sb.Append("p.wksp_id, p.has_team, p.is_dlt, p.dlt_dt, p.dlt_by, p.unit_id, ");
            sb.Append("p.dept_id, p.loc_id, u.unitname, d.deptname, l.locname, s.fullname, ");
            sb.Append("f.title AS fldr_title, w.title AS wksp_title, p.mod_by, ");
            sb.Append("p.mod_dt, p.crt_by, p.crt_dt FROM public.wkm_prj_inf p ");
            sb.Append("INNER JOIN public.wkm_fdr_inf f ON f.id = p.fldr_id ");
            sb.Append("INNER JOIN public.wkm_wsp_inf w ON w.id = p.wksp_id ");
            sb.Append("LEFT OUTER JOIN public.gst_units u ON u.unitqk = p.unit_id ");
            sb.Append("LEFT OUTER JOIN public.gst_depts d ON d.deptqk = p.dept_id ");
            sb.Append("LEFT OUTER JOIN public.gst_locs l ON l.locqk = p.loc_id ");
            sb.Append("LEFT OUTER JOIN public.gst_prsns s ON s.id = p.prj_assigned_to ");
            sb.Append("WHERE (LOWER(prj_ttl) = LOWER(@prj_ttl) AND p.is_dlt = false) ");
            sb.Append("ORDER BY p.prj_id;");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var prj_ttl = cmd.Parameters.Add("@prj_ttl", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    prj_ttl.Value = workItemTitle;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            workItems.Add(new WorkItem
                            {
                                ID = reader["prj_id"] == DBNull.Value ? 0 : (int)reader["prj_id"],
                                Code = reader["prj_cd"] == DBNull.Value ? string.Empty : reader["prj_cd"].ToString(),
                                Title = reader["prj_ttl"] == DBNull.Value ? string.Empty : reader["prj_ttl"].ToString(),
                                Description = reader["prj_dsc"] == DBNull.Value ? string.Empty : reader["prj_dsc"].ToString(),
                                OwnerID = reader["prj_owner_id"] == DBNull.Value ? string.Empty : reader["prj_owner_id"].ToString(),
                                AssignedToID = reader["prj_assigned_to"] == DBNull.Value ? string.Empty : reader["prj_assigned_to"].ToString(),
                                AssignedToName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                                AssignedTime = reader["assigned_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["assigned_dt"],
                                ProgressStatus = reader["prg_stts"] == DBNull.Value ? WorkItemStatus.NotStarted : (WorkItemStatus)reader["prg_stts"],
                                IsAtRisk = reader["at_risk"] == DBNull.Value ? false : (bool)reader["at_risk"],
                                ExpectedStartTime = reader["exp_start_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["exp_start_dt"],
                                ExpectedDueTime = reader["exp_due_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["exp_due_dt"],
                                TotalExpectedDurationInMinutes = reader["exp_dur_min"] == DBNull.Value ? 0 : (long)reader["exp_dur_min"],
                                TotalActualDurationInMinutes = reader["act_dur_min"] == DBNull.Value ? 0 : (long)reader["act_dur_min"],
                                Priority = reader["prj_prrty"] == DBNull.Value ? WorkItemPriority.Normal : (WorkItemPriority)reader["prj_prrty"],
                                EstimatedCostForeignCurrency = reader["est_cost_fc"] == DBNull.Value ? 0 : (long)reader["est_cost_fc"],
                                ActualCostForeignCurrency = reader["act_cost_fc"] == DBNull.Value ? 0 : (long)reader["act_cost_fc"],
                                ForeignCurrencyCode = reader["curr_cd_fc"] == DBNull.Value ? string.Empty : reader["curr_cd_fc"].ToString(),
                                EstimatedCostLocalCurrency = reader["est_cost_lc"] == DBNull.Value ? 0 : (long)reader["est_cost_lc"],
                                ActualCostLocalCurrency = reader["act_cost_lc"] == DBNull.Value ? 0 : (long)reader["act_cost_lc"],
                                PercentageCompleted = reader["pct_com"] == DBNull.Value ? 0 : (int)reader["pct_com"],
                                Deliverables = reader["deliverables"] == DBNull.Value ? string.Empty : reader["deliverables"].ToString(),
                                Instructions = reader["instructions"] == DBNull.Value ? string.Empty : reader["deliverables"].ToString(),
                                FolderID = reader["fldr_id"] == DBNull.Value ? 0 : (int)reader["fldr_id"],
                                FolderTitle = reader["fldr_title"] == DBNull.Value ? string.Empty : reader["fldr_title"].ToString(),
                                WorkspaceID = reader["wksp_id"] == DBNull.Value ? 0 : (int)reader["wksp_id"],
                                WorkspaceTitle = reader["wksp_title"] == DBNull.Value ? string.Empty : reader["wksp_title"].ToString(),
                                HasTeam = reader["has_team"] == DBNull.Value ? false : (bool)reader["has_team"],
                                UnitID = reader["unit_id"] == DBNull.Value ? (int?)null : (int)reader["unit_id"],
                                UnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                                DepartmentID = reader["dept_id"] == DBNull.Value ? (int?)null : (int)reader["dept_id"],
                                DepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                                LocationID = reader["loc_id"] == DBNull.Value ? (int?)null : (int)reader["loc_id"],
                                LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                                IsDeleted = reader["is_dlt"] == DBNull.Value ? false : (bool)reader["is_dlt"],
                                DeletedTime = reader["dlt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dlt_dt"],
                                DeletedBy = reader["dlt_by"] == DBNull.Value ? string.Empty : reader["dlt_by"].ToString(),
                                LastModifiedTime = reader["mod_dt"] == DBNull.Value ? string.Empty : reader["mod_dt"].ToString(),
                                LastModifiedBy = reader["mod_by"] == DBNull.Value ? string.Empty : reader["mod_by"].ToString(),
                                CreatedTime = reader["crt_dt"] == DBNull.Value ? string.Empty : reader["crt_dt"].ToString(),
                                CreatedBy = reader["crt_by"] == DBNull.Value ? string.Empty : reader["crt_by"].ToString(),
                            });
                        }
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message);
            }
            return workItems;
        }

        public async Task<IList<WorkItem>> SearchByFolderIdAndTitleAsync(int folderId, string workItemTitle)
        {
            IList<WorkItem> workItems = new List<WorkItem>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT p.prj_id, p.prj_cd, p.prj_ttl, p.prj_dsc, p.prj_owner_id, ");
            sb.Append("p.prj_assigned_to, p.assigned_dt, p.prg_stts, p.at_risk, ");
            sb.Append("p.exp_start_dt, p.exp_due_dt, p.exp_dur_min, p.act_dur_min, ");
            sb.Append("p.prj_prrty, p.est_cost_fc, p.act_cost_fc, p.curr_cd_fc, p.est_cost_lc, ");
            sb.Append("p.act_cost_lc, p.pct_com, p.deliverables, p.instructions, p.fldr_id, ");
            sb.Append("p.wksp_id, p.has_team, p.is_dlt, p.dlt_dt, p.dlt_by, p.unit_id, ");
            sb.Append("p.dept_id, p.loc_id, u.unitname, d.deptname, l.locname, s.fullname, ");
            sb.Append("f.title AS fldr_title, w.title AS wksp_title, p.mod_by, ");
            sb.Append("p.mod_dt, p.crt_by, p.crt_dt FROM public.wkm_prj_inf p ");
            sb.Append("INNER JOIN public.wkm_fdr_inf f ON f.id = p.fldr_id ");
            sb.Append("INNER JOIN public.wkm_wsp_inf w ON w.id = p.wksp_id ");
            sb.Append("LEFT OUTER JOIN public.gst_units u ON u.unitqk = p.unit_id ");
            sb.Append("LEFT OUTER JOIN public.gst_depts d ON d.deptqk = p.dept_id ");
            sb.Append("LEFT OUTER JOIN public.gst_locs l ON l.locqk = p.loc_id ");
            sb.Append("LEFT OUTER JOIN public.gst_prsns s ON s.id = p.prj_assigned_to ");
            sb.Append("WHERE (fldr_id = @fldr_id AND p.is_dlt = false) ");
            sb.Append("AND LOWER(prj_ttl) LIKE '%'||LOWER(@prj_ttl)||'%') ");
            sb.Append("ORDER BY p.prj_id;");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var fldr_id = cmd.Parameters.Add("@fldr_id", NpgsqlDbType.Integer);
                    var prj_ttl = cmd.Parameters.Add("@prj_ttl", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    fldr_id.Value = folderId;
                    prj_ttl.Value = workItemTitle;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            workItems.Add(new WorkItem
                            {
                                ID = reader["prj_id"] == DBNull.Value ? 0 : (int)reader["prj_id"],
                                Code = reader["prj_cd"] == DBNull.Value ? string.Empty : reader["prj_cd"].ToString(),
                                Title = reader["prj_ttl"] == DBNull.Value ? string.Empty : reader["prj_ttl"].ToString(),
                                Description = reader["prj_dsc"] == DBNull.Value ? string.Empty : reader["prj_dsc"].ToString(),
                                OwnerID = reader["prj_owner_id"] == DBNull.Value ? string.Empty : reader["prj_owner_id"].ToString(),
                                AssignedToID = reader["prj_assigned_to"] == DBNull.Value ? string.Empty : reader["prj_assigned_to"].ToString(),
                                AssignedToName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                                AssignedTime = reader["assigned_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["assigned_dt"],
                                ProgressStatus = reader["prg_stts"] == DBNull.Value ? WorkItemStatus.NotStarted : (WorkItemStatus)reader["prg_stts"],
                                IsAtRisk = reader["at_risk"] == DBNull.Value ? false : (bool)reader["at_risk"],
                                ExpectedStartTime = reader["exp_start_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["exp_start_dt"],
                                ExpectedDueTime = reader["exp_due_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["exp_due_dt"],
                                TotalExpectedDurationInMinutes = reader["exp_dur_min"] == DBNull.Value ? 0 : (long)reader["exp_dur_min"],
                                TotalActualDurationInMinutes = reader["act_dur_min"] == DBNull.Value ? 0 : (long)reader["act_dur_min"],
                                Priority = reader["prj_prrty"] == DBNull.Value ? WorkItemPriority.Normal : (WorkItemPriority)reader["prj_prrty"],
                                EstimatedCostForeignCurrency = reader["est_cost_fc"] == DBNull.Value ? 0 : (long)reader["est_cost_fc"],
                                ActualCostForeignCurrency = reader["act_cost_fc"] == DBNull.Value ? 0 : (long)reader["act_cost_fc"],
                                ForeignCurrencyCode = reader["curr_cd_fc"] == DBNull.Value ? string.Empty : reader["curr_cd_fc"].ToString(),
                                EstimatedCostLocalCurrency = reader["est_cost_lc"] == DBNull.Value ? 0 : (long)reader["est_cost_lc"],
                                ActualCostLocalCurrency = reader["act_cost_lc"] == DBNull.Value ? 0 : (long)reader["act_cost_lc"],
                                PercentageCompleted = reader["pct_com"] == DBNull.Value ? 0 : (int)reader["pct_com"],
                                Deliverables = reader["deliverables"] == DBNull.Value ? string.Empty : reader["deliverables"].ToString(),
                                Instructions = reader["instructions"] == DBNull.Value ? string.Empty : reader["deliverables"].ToString(),
                                FolderID = reader["fldr_id"] == DBNull.Value ? 0 : (int)reader["fldr_id"],
                                FolderTitle = reader["fldr_title"] == DBNull.Value ? string.Empty : reader["fldr_title"].ToString(),
                                WorkspaceID = reader["wksp_id"] == DBNull.Value ? 0 : (int)reader["wksp_id"],
                                WorkspaceTitle = reader["wksp_title"] == DBNull.Value ? string.Empty : reader["wksp_title"].ToString(),
                                HasTeam = reader["has_team"] == DBNull.Value ? false : (bool)reader["has_team"],
                                UnitID = reader["unit_id"] == DBNull.Value ? (int?)null : (int)reader["unit_id"],
                                UnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                                DepartmentID = reader["dept_id"] == DBNull.Value ? (int?)null : (int)reader["dept_id"],
                                DepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                                LocationID = reader["loc_id"] == DBNull.Value ? (int?)null : (int)reader["loc_id"],
                                LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                                IsDeleted = reader["is_dlt"] == DBNull.Value ? false : (bool)reader["is_dlt"],
                                DeletedTime = reader["dlt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dlt_dt"],
                                DeletedBy = reader["dlt_by"] == DBNull.Value ? string.Empty : reader["dlt_by"].ToString(),
                                LastModifiedTime = reader["mod_dt"] == DBNull.Value ? string.Empty : reader["mod_dt"].ToString(),
                                LastModifiedBy = reader["mod_by"] == DBNull.Value ? string.Empty : reader["mod_by"].ToString(),
                                CreatedTime = reader["crt_dt"] == DBNull.Value ? string.Empty : reader["crt_dt"].ToString(),
                                CreatedBy = reader["crt_by"] == DBNull.Value ? string.Empty : reader["crt_by"].ToString(),
                            });
                        }
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message);
            }
            return workItems;
        }

        public async Task<IList<WorkItem>> SearchByTitleAsync(string workItemTitle)
        {
            IList<WorkItem> workItems = new List<WorkItem>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT p.prj_id, p.prj_cd, p.prj_ttl, p.prj_dsc, p.prj_owner_id, ");
            sb.Append("p.prj_assigned_to, p.assigned_dt, p.prg_stts, p.at_risk, ");
            sb.Append("p.exp_start_dt, p.exp_due_dt, p.exp_dur_min, p.act_dur_min, ");
            sb.Append("p.prj_prrty, p.est_cost_fc, p.act_cost_fc, p.curr_cd_fc, p.est_cost_lc, ");
            sb.Append("p.act_cost_lc, p.pct_com, p.deliverables, p.instructions, p.fldr_id, ");
            sb.Append("p.wksp_id, p.has_team, p.is_dlt, p.dlt_dt, p.dlt_by, p.unit_id, ");
            sb.Append("p.dept_id, p.loc_id, u.unitname, d.deptname, l.locname, s.fullname, ");
            sb.Append("f.title AS fldr_title, w.title AS wksp_title, p.mod_by, ");
            sb.Append("p.mod_dt, p.crt_by, p.crt_dt FROM public.wkm_prj_inf p ");
            sb.Append("INNER JOIN public.wkm_fdr_inf f ON f.id = p.fldr_id ");
            sb.Append("INNER JOIN public.wkm_wsp_inf w ON w.id = p.wksp_id ");
            sb.Append("LEFT OUTER JOIN public.gst_units u ON u.unitqk = p.unit_id ");
            sb.Append("LEFT OUTER JOIN public.gst_depts d ON d.deptqk = p.dept_id ");
            sb.Append("LEFT OUTER JOIN public.gst_locs l ON l.locqk = p.loc_id ");
            sb.Append("LEFT OUTER JOIN public.gst_prsns s ON s.id = p.prj_assigned_to ");
            sb.Append("WHERE (p.is_dlt = false) ");
            sb.Append("AND LOWER(prj_ttl) LIKE '%'||LOWER(@prj_ttl)||'%') ");
            sb.Append("ORDER BY p.prj_id;");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var prj_ttl = cmd.Parameters.Add("@prj_ttl", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    prj_ttl.Value = workItemTitle;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            workItems.Add(new WorkItem
                            {
                                ID = reader["prj_id"] == DBNull.Value ? 0 : (int)reader["prj_id"],
                                Code = reader["prj_cd"] == DBNull.Value ? string.Empty : reader["prj_cd"].ToString(),
                                Title = reader["prj_ttl"] == DBNull.Value ? string.Empty : reader["prj_ttl"].ToString(),
                                Description = reader["prj_dsc"] == DBNull.Value ? string.Empty : reader["prj_dsc"].ToString(),
                                OwnerID = reader["prj_owner_id"] == DBNull.Value ? string.Empty : reader["prj_owner_id"].ToString(),
                                AssignedToID = reader["prj_assigned_to"] == DBNull.Value ? string.Empty : reader["prj_assigned_to"].ToString(),
                                AssignedToName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                                AssignedTime = reader["assigned_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["assigned_dt"],
                                ProgressStatus = reader["prg_stts"] == DBNull.Value ? WorkItemStatus.NotStarted : (WorkItemStatus)reader["prg_stts"],
                                IsAtRisk = reader["at_risk"] == DBNull.Value ? false : (bool)reader["at_risk"],
                                ExpectedStartTime = reader["exp_start_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["exp_start_dt"],
                                ExpectedDueTime = reader["exp_due_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["exp_due_dt"],
                                TotalExpectedDurationInMinutes = reader["exp_dur_min"] == DBNull.Value ? 0 : (long)reader["exp_dur_min"],
                                TotalActualDurationInMinutes = reader["act_dur_min"] == DBNull.Value ? 0 : (long)reader["act_dur_min"],
                                Priority = reader["prj_prrty"] == DBNull.Value ? WorkItemPriority.Normal : (WorkItemPriority)reader["prj_prrty"],
                                EstimatedCostForeignCurrency = reader["est_cost_fc"] == DBNull.Value ? 0 : (long)reader["est_cost_fc"],
                                ActualCostForeignCurrency = reader["act_cost_fc"] == DBNull.Value ? 0 : (long)reader["act_cost_fc"],
                                ForeignCurrencyCode = reader["curr_cd_fc"] == DBNull.Value ? string.Empty : reader["curr_cd_fc"].ToString(),
                                EstimatedCostLocalCurrency = reader["est_cost_lc"] == DBNull.Value ? 0 : (long)reader["est_cost_lc"],
                                ActualCostLocalCurrency = reader["act_cost_lc"] == DBNull.Value ? 0 : (long)reader["act_cost_lc"],
                                PercentageCompleted = reader["pct_com"] == DBNull.Value ? 0 : (int)reader["pct_com"],
                                Deliverables = reader["deliverables"] == DBNull.Value ? string.Empty : reader["deliverables"].ToString(),
                                Instructions = reader["instructions"] == DBNull.Value ? string.Empty : reader["deliverables"].ToString(),
                                FolderID = reader["fldr_id"] == DBNull.Value ? 0 : (int)reader["fldr_id"],
                                FolderTitle = reader["fldr_title"] == DBNull.Value ? string.Empty : reader["fldr_title"].ToString(),
                                WorkspaceID = reader["wksp_id"] == DBNull.Value ? 0 : (int)reader["wksp_id"],
                                WorkspaceTitle = reader["wksp_title"] == DBNull.Value ? string.Empty : reader["wksp_title"].ToString(),
                                HasTeam = reader["has_team"] == DBNull.Value ? false : (bool)reader["has_team"],
                                UnitID = reader["unit_id"] == DBNull.Value ? (int?)null : (int)reader["unit_id"],
                                UnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                                DepartmentID = reader["dept_id"] == DBNull.Value ? (int?)null : (int)reader["dept_id"],
                                DepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                                LocationID = reader["loc_id"] == DBNull.Value ? (int?)null : (int)reader["loc_id"],
                                LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                                IsDeleted = reader["is_dlt"] == DBNull.Value ? false : (bool)reader["is_dlt"],
                                DeletedTime = reader["dlt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dlt_dt"],
                                DeletedBy = reader["dlt_by"] == DBNull.Value ? string.Empty : reader["dlt_by"].ToString(),
                                LastModifiedTime = reader["mod_dt"] == DBNull.Value ? string.Empty : reader["mod_dt"].ToString(),
                                LastModifiedBy = reader["mod_by"] == DBNull.Value ? string.Empty : reader["mod_by"].ToString(),
                                CreatedTime = reader["crt_dt"] == DBNull.Value ? string.Empty : reader["crt_dt"].ToString(),
                                CreatedBy = reader["crt_by"] == DBNull.Value ? string.Empty : reader["crt_by"].ToString(),
                            });
                        }
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message);
            }
            return workItems;
        }
        #endregion

    }
}
