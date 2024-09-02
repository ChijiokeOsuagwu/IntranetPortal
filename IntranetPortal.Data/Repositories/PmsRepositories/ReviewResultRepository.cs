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
    public class ReviewResultRepository : IReviewResultRepository
    {
        public IConfiguration _config { get; }
        public ReviewResultRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        #region Review Result Read Action Methods

        public async Task<IList<ReviewResult>> GetIntitalByThirdPartyAsync(int reviewHeaderId, string appraiserId, int metricTypeId)
        {
            List<ReviewResult> reviewResultsList = new List<ReviewResult>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT m.rvw_mtrc_id, m.rvw_hdr_id, m.rvw_sxn_id, m.rvw_yr_id, ");
            sb.Append("m.mtrc_typ_id, m.mtrc_ds, m.mtrc_kpi, m.mtrc_tgt, m.mtrc_wtg, ");
            sb.Append("h.rvw_emp_id, s.rvw_sxn_nm, y.pms_yr_nm, d.rvw_dtl_id, ");
            sb.Append("d.aprsr_rl_id, d.aprsr_typ_id, d.actl_achvmts, d.aprsr_score, ");
            sb.Append("d.score_time, d.aprsr_rmks, d.score_ds, d.score_val, ");
            sb.Append("d.rvw_aprsr_id, e.fullname as appraisee_nm, r.aprv_rl_nm, ");
            sb.Append("CASE m.mtrc_typ_id WHEN 0 THEN 'KPA' WHEN 1 THEN 'Competency' ");
            sb.Append("END mtrc_typ_ds, CASE d.aprsr_typ_id WHEN 0 THEN 'Self Appraiser' ");
            sb.Append("WHEN 1 THEN 'Principal Appraiser' WHEN 2 THEN '3rd Party Appraiser' ");
            sb.Append("END aprsr_typ_ds, f.fullname as appraiser_nm, ");

            sb.Append("(SELECT actl_achvmts as app_achvmts FROM public.pmsrvwrdtls ");
            sb.Append("WHERE rvw_hdr_id = @rvw_hdr_id AND rvw_mtric_id = m.rvw_mtrc_id ");
            sb.Append("AND rvw_aprsr_id = h.rvw_emp_id), ");
            sb.Append("(SELECT score_ds as app_score FROM public.pmsrvwrdtls ");
            sb.Append("WHERE rvw_hdr_id = @rvw_hdr_id AND rvw_mtric_id = m.rvw_mtrc_id ");
            sb.Append("AND rvw_aprsr_id = h.rvw_emp_id) ");

            sb.Append("FROM public.pmsrvwmtrcs m ");
            sb.Append("INNER JOIN public.pmsrvwhdrs h ON (h.rvw_hdr_id = m.rvw_hdr_id ");
            sb.Append("AND m.rvw_hdr_id = @rvw_hdr_id) ");
            sb.Append("INNER JOIN public.pmsrvwsxns s ON s.rvw_sxn_id = m.rvw_sxn_id ");
            sb.Append("INNER JOIN public.pmssttyrs y ON y.pms_yr_id = m.rvw_yr_id ");
            sb.Append("INNER JOIN public.gst_prsns e ON e.id = h.rvw_emp_id ");
            sb.Append("LEFT JOIN public.pmsrvwrdtls d ON ((d.rvw_mtric_id = m.rvw_mtrc_id) ");
            sb.Append("AND (m.rvw_hdr_id = @rvw_hdr_id) AND (d.rvw_aprsr_id = @rvw_aprsr_id)) ");
            sb.Append("LEFT JOIN public.gst_prsns f ON f.id = d.rvw_aprsr_id ");
            sb.Append("LEFT JOIN public.pmsaprvrls r ON r.aprv_rl_id = d.aprsr_rl_id ");
            sb.Append("WHERE (m.mtrc_typ_id = @mtrc_typ_id); ");

            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_hdr_id = cmd.Parameters.Add("@rvw_hdr_id", NpgsqlDbType.Integer);
                var mtrc_typ_id = cmd.Parameters.Add("@mtrc_typ_id", NpgsqlDbType.Integer);
                var rvw_aprsr_id = cmd.Parameters.Add("@rvw_aprsr_id", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                rvw_hdr_id.Value = reviewHeaderId;
                mtrc_typ_id.Value = metricTypeId;
                rvw_aprsr_id.Value = appraiserId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    reviewResultsList.Add(new ReviewResult()
                    {
                        ReviewMetricId = reader["rvw_mtrc_id"] == DBNull.Value ? 0 : (int)reader["rvw_mtrc_id"],
                        ReviewHeaderId = reader["rvw_hdr_id"] == DBNull.Value ? 0 : (int)reader["rvw_hdr_id"],
                        ReviewSessionId = reader["rvw_sxn_id"] == DBNull.Value ? 0 : (int)reader["rvw_sxn_id"],
                        ReviewSessionName = reader["rvw_sxn_nm"] == DBNull.Value ? string.Empty : reader["rvw_sxn_nm"].ToString(),

                        AppraiseeId = reader["rvw_emp_id"] == DBNull.Value ? string.Empty : reader["rvw_emp_id"].ToString(),
                        AppraiseeName = reader["appraisee_nm"] == DBNull.Value ? string.Empty : reader["appraisee_nm"].ToString(),

                        ReviewYearId = reader["rvw_yr_id"] == DBNull.Value ? 0 : (int)reader["rvw_yr_id"],
                        ReviewYearName = reader["pms_yr_nm"] == DBNull.Value ? string.Empty : reader["pms_yr_nm"].ToString(),

                        AppraiserId = reader["rvw_aprsr_id"] == DBNull.Value ? string.Empty : reader["rvw_aprsr_id"].ToString(),
                        AppraiserName = reader["appraiser_nm"] == DBNull.Value ? string.Empty : reader["appraiser_nm"].ToString(),

                        ReviewMetricTypeId = reader["mtrc_typ_id"] == DBNull.Value ? 0 : (int)reader["mtrc_typ_id"],
                        ReviewMetricTypeDescription = reader["mtrc_typ_ds"] == DBNull.Value ? string.Empty : reader["mtrc_typ_ds"].ToString(),
                        ReviewMetricDescription = reader["mtrc_ds"] == DBNull.Value ? string.Empty : reader["mtrc_ds"].ToString(),

                        ReviewMetricMeasurement = reader["mtrc_kpi"] == DBNull.Value ? string.Empty : reader["mtrc_kpi"].ToString(),
                        ReviewMetricTarget = reader["mtrc_tgt"] == DBNull.Value ? string.Empty : reader["mtrc_tgt"].ToString(),
                        ReviewMetricWeightage = reader["mtrc_wtg"] == DBNull.Value ? 0.00M : (decimal)reader["mtrc_wtg"],

                        ReviewResultId = reader["rvw_dtl_id"] == DBNull.Value ? 0 : (int)reader["rvw_dtl_id"],
                        AppraiserRoleId = reader["aprsr_rl_id"] == DBNull.Value ? (int?)null : (int)reader["aprsr_rl_id"],
                        AppraiserRoleName = reader["aprv_rl_nm"] == DBNull.Value ? "Appraisee" : reader["aprv_rl_nm"].ToString(),

                        AppraiserTypeId = reader["aprsr_typ_id"] == DBNull.Value ? 0 : (int)reader["aprsr_typ_id"],
                        AppraiserTypeDescription = reader["aprsr_typ_ds"] == DBNull.Value ? string.Empty : reader["aprsr_typ_ds"].ToString(),

                        ActualAchievement = reader["actl_achvmts"] == DBNull.Value ? string.Empty : reader["actl_achvmts"].ToString(),
                        AppraiserScore = reader["aprsr_score"] == DBNull.Value ? 0.00M : (decimal)reader["aprsr_score"],
                        AppraiserScoreDescription = reader["score_ds"] == DBNull.Value ? string.Empty : reader["score_ds"].ToString(),
                        AppraiserScoreValue = reader["score_val"] == DBNull.Value ? 0.00M : (decimal)reader["score_val"],

                        ScoreTime = reader["score_time"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["score_time"],
                        AppraiserComment = reader["aprsr_rmks"] == DBNull.Value ? string.Empty : reader["aprsr_rmks"].ToString(),

                        AppraiseeAchievement = reader["app_achvmts"] == DBNull.Value ? string.Empty : reader["app_achvmts"].ToString(),
                        AppraiseeScore = reader["app_score"] == DBNull.Value ? string.Empty : reader["app_score"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return reviewResultsList;
        }

        public async Task<IList<ReviewResult>> GetIntitalByMetricTypeIdAsync(int reviewHeaderId, string appraiserId, int metricTypeId)
        {
            List<ReviewResult> reviewResultsList = new List<ReviewResult>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT m.rvw_mtrc_id, m.rvw_hdr_id, m.rvw_sxn_id, m.rvw_yr_id, ");
            sb.Append("m.mtrc_typ_id, m.mtrc_ds, m.mtrc_kpi, m.mtrc_tgt, m.mtrc_wtg, ");
            sb.Append("h.rvw_emp_id, s.rvw_sxn_nm, y.pms_yr_nm, d.rvw_dtl_id, ");
            sb.Append("d.aprsr_rl_id, d.aprsr_typ_id, d.actl_achvmts, d.aprsr_score, ");
            sb.Append("d.score_time, d.aprsr_rmks, d.score_ds, d.score_val, ");
            sb.Append("d.rvw_aprsr_id, e.fullname as appraisee_nm, r.aprv_rl_nm, ");
            sb.Append("CASE m.mtrc_typ_id WHEN 0 THEN 'KPA' WHEN 1 THEN 'Competency' ");
            sb.Append("END mtrc_typ_ds, CASE d.aprsr_typ_id WHEN 0 THEN 'Self Appraiser' ");
            sb.Append("WHEN 1 THEN 'Principal Appraiser' WHEN 2 THEN '3rd Party Appraiser' ");
            sb.Append("END aprsr_typ_ds, f.fullname as appraiser_nm, ");

            sb.Append("(SELECT actl_achvmts as app_achvmts FROM public.pmsrvwrdtls ");
            sb.Append("WHERE rvw_hdr_id = @rvw_hdr_id AND rvw_mtric_id = m.rvw_mtrc_id ");
            sb.Append("AND rvw_aprsr_id = h.rvw_emp_id), ");
            sb.Append("(SELECT score_ds as app_score FROM public.pmsrvwrdtls ");
            sb.Append("WHERE rvw_hdr_id = @rvw_hdr_id AND rvw_mtric_id = m.rvw_mtrc_id ");
            sb.Append("AND rvw_aprsr_id = h.rvw_emp_id) ");

            sb.Append("FROM public.pmsrvwmtrcs m ");
            sb.Append("INNER JOIN public.pmsrvwhdrs h ON (h.rvw_hdr_id = m.rvw_hdr_id ");
            sb.Append("AND m.rvw_hdr_id = @rvw_hdr_id) ");
            sb.Append("INNER JOIN public.pmsrvwsxns s ON s.rvw_sxn_id = m.rvw_sxn_id ");
            sb.Append("INNER JOIN public.pmssttyrs y ON y.pms_yr_id = m.rvw_yr_id ");
            sb.Append("INNER JOIN public.gst_prsns e ON e.id = h.rvw_emp_id ");
            sb.Append("LEFT JOIN public.pmsrvwrdtls d ON ((d.rvw_mtric_id = m.rvw_mtrc_id) ");
            sb.Append("AND (m.rvw_hdr_id = @rvw_hdr_id) AND (d.rvw_aprsr_id = @rvw_aprsr_id)) ");
            sb.Append("LEFT JOIN public.gst_prsns f ON f.id = d.rvw_aprsr_id ");
            sb.Append("LEFT JOIN public.pmsaprvrls r ON r.aprv_rl_id = d.aprsr_rl_id ");
            sb.Append("WHERE (m.mtrc_typ_id = @mtrc_typ_id); ");
            //sb.Append("AND (d.rvw_aprsr_id = @rvw_aprsr_id OR d.rvw_aprsr_id IS NULL); ");

            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_hdr_id = cmd.Parameters.Add("@rvw_hdr_id", NpgsqlDbType.Integer);
                var rvw_aprsr_id = cmd.Parameters.Add("@rvw_aprsr_id", NpgsqlDbType.Text);
                var mtrc_typ_id = cmd.Parameters.Add("@mtrc_typ_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                rvw_hdr_id.Value = reviewHeaderId;
                rvw_aprsr_id.Value = appraiserId;
                mtrc_typ_id.Value = metricTypeId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    reviewResultsList.Add(new ReviewResult()
                    {
                        ReviewMetricId = reader["rvw_mtrc_id"] == DBNull.Value ? 0 : (int)reader["rvw_mtrc_id"],
                        ReviewHeaderId = reader["rvw_hdr_id"] == DBNull.Value ? 0 : (int)reader["rvw_hdr_id"],
                        ReviewSessionId = reader["rvw_sxn_id"] == DBNull.Value ? 0 : (int)reader["rvw_sxn_id"],
                        ReviewSessionName = reader["rvw_sxn_nm"] == DBNull.Value ? string.Empty : reader["rvw_sxn_nm"].ToString(),

                        AppraiseeId = reader["rvw_emp_id"] == DBNull.Value ? string.Empty : reader["rvw_emp_id"].ToString(),
                        AppraiseeName = reader["appraisee_nm"] == DBNull.Value ? string.Empty : reader["appraisee_nm"].ToString(),

                        ReviewYearId = reader["rvw_yr_id"] == DBNull.Value ? 0 : (int)reader["rvw_yr_id"],
                        ReviewYearName = reader["pms_yr_nm"] == DBNull.Value ? string.Empty : reader["pms_yr_nm"].ToString(),

                        AppraiserId = reader["rvw_aprsr_id"] == DBNull.Value ? string.Empty : reader["rvw_aprsr_id"].ToString(),
                        AppraiserName = reader["appraiser_nm"] == DBNull.Value ? string.Empty : reader["appraiser_nm"].ToString(),

                        ReviewMetricTypeId = reader["mtrc_typ_id"] == DBNull.Value ? 0 : (int)reader["mtrc_typ_id"],
                        ReviewMetricTypeDescription = reader["mtrc_typ_ds"] == DBNull.Value ? string.Empty : reader["mtrc_typ_ds"].ToString(),
                        ReviewMetricDescription = reader["mtrc_ds"] == DBNull.Value ? string.Empty : reader["mtrc_ds"].ToString(),

                        ReviewMetricMeasurement = reader["mtrc_kpi"] == DBNull.Value ? string.Empty : reader["mtrc_kpi"].ToString(),
                        ReviewMetricTarget = reader["mtrc_tgt"] == DBNull.Value ? string.Empty : reader["mtrc_tgt"].ToString(),
                        ReviewMetricWeightage = reader["mtrc_wtg"] == DBNull.Value ? 0.00M : (decimal)reader["mtrc_wtg"],

                        ReviewResultId = reader["rvw_dtl_id"] == DBNull.Value ? 0 : (int)reader["rvw_dtl_id"],
                        AppraiserRoleId = reader["aprsr_rl_id"] == DBNull.Value ? (int?)null : (int)reader["aprsr_rl_id"],
                        AppraiserRoleName = reader["aprv_rl_nm"] == DBNull.Value ? "Appraisee" : reader["aprv_rl_nm"].ToString(),

                        AppraiserTypeId = reader["aprsr_typ_id"] == DBNull.Value ? 0 : (int)reader["aprsr_typ_id"],
                        AppraiserTypeDescription = reader["aprsr_typ_ds"] == DBNull.Value ? string.Empty : reader["aprsr_typ_ds"].ToString(),

                        ActualAchievement = reader["actl_achvmts"] == DBNull.Value ? string.Empty : reader["actl_achvmts"].ToString(),
                        AppraiserScore = reader["aprsr_score"] == DBNull.Value ? 0.00M : (decimal)reader["aprsr_score"],
                        AppraiserScoreDescription = reader["score_ds"] == DBNull.Value ? string.Empty : reader["score_ds"].ToString(),
                        AppraiserScoreValue = reader["score_val"] == DBNull.Value ? 0.00M : (decimal)reader["score_val"],

                        ScoreTime = reader["score_time"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["score_time"],
                        AppraiserComment = reader["aprsr_rmks"] == DBNull.Value ? string.Empty : reader["aprsr_rmks"].ToString(),

                        AppraiseeAchievement = reader["app_achvmts"] == DBNull.Value ? string.Empty : reader["app_achvmts"].ToString(),
                        AppraiseeScore = reader["app_score"] == DBNull.Value ? string.Empty : reader["app_score"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return reviewResultsList;
        }

        public async Task<IList<ReviewResult>> GetIntitalByMetricIdAsync(int reviewHeaderId, string appraiserId, int metricId)
        {
            List<ReviewResult> reviewResultsList = new List<ReviewResult>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT m.rvw_mtrc_id, m.rvw_hdr_id, m.rvw_sxn_id, m.rvw_yr_id, ");
            sb.Append("m.mtrc_typ_id, m.mtrc_ds, m.mtrc_kpi, m.mtrc_tgt, m.mtrc_wtg, ");
            sb.Append("h.rvw_emp_id, s.rvw_sxn_nm, y.pms_yr_nm, d.rvw_dtl_id, ");
            sb.Append("d.aprsr_rl_id, d.aprsr_typ_id, d.actl_achvmts, d.aprsr_score, ");
            sb.Append("d.score_time, d.aprsr_rmks, d.score_ds, d.score_val, ");
            sb.Append("d.rvw_aprsr_id, e.fullname as appraisee_nm, r.aprv_rl_nm, ");
            sb.Append("CASE m.mtrc_typ_id WHEN 0 THEN 'KPA' WHEN 1 THEN 'Competency' ");
            sb.Append("END mtrc_typ_ds, CASE d.aprsr_typ_id WHEN 0 THEN 'Self Appraiser' ");
            sb.Append("WHEN 1 THEN 'Principal Appraiser' WHEN 2 THEN '3rd Party Appraiser' ");
            sb.Append("END aprsr_typ_ds, f.fullname as appraiser_nm,");

            sb.Append("(SELECT actl_achvmts as app_achvmts FROM public.pmsrvwrdtls ");
            sb.Append("WHERE rvw_hdr_id = @rvw_hdr_id AND rvw_mtric_id = m.rvw_mtrc_id ");
            sb.Append("AND rvw_aprsr_id = h.rvw_emp_id), ");
            sb.Append("(SELECT score_ds as app_score FROM public.pmsrvwrdtls ");
            sb.Append("WHERE rvw_hdr_id = @rvw_hdr_id AND rvw_mtric_id = m.rvw_mtrc_id ");
            sb.Append("AND rvw_aprsr_id = h.rvw_emp_id)  ");

            sb.Append("FROM public.pmsrvwmtrcs m ");
            sb.Append("INNER JOIN public.pmsrvwhdrs h ON (h.rvw_hdr_id = m.rvw_hdr_id ");
            sb.Append("AND m.rvw_hdr_id = @rvw_hdr_id) ");
            sb.Append("INNER JOIN public.pmsrvwsxns s ON s.rvw_sxn_id = m.rvw_sxn_id ");
            sb.Append("INNER JOIN public.pmssttyrs y ON y.pms_yr_id = m.rvw_yr_id ");
            sb.Append("INNER JOIN public.gst_prsns e ON e.id = h.rvw_emp_id ");
            sb.Append("LEFT JOIN public.pmsrvwrdtls d ON ((d.rvw_mtric_id = m.rvw_mtrc_id) ");
            sb.Append("AND (m.rvw_hdr_id = @rvw_hdr_id) AND (d.rvw_aprsr_id = @rvw_aprsr_id)) ");
            sb.Append("LEFT JOIN public.gst_prsns f ON f.id = d.rvw_aprsr_id ");
            sb.Append("LEFT JOIN public.pmsaprvrls r ON r.aprv_rl_id = d.aprsr_rl_id ");
            sb.Append("WHERE (m.rvw_mtrc_id = @rvw_mtrc_id); ");

            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_hdr_id = cmd.Parameters.Add("@rvw_hdr_id", NpgsqlDbType.Integer);
                var rvw_aprsr_id = cmd.Parameters.Add("@rvw_aprsr_id", NpgsqlDbType.Text);
                var rvw_mtrc_id = cmd.Parameters.Add("@rvw_mtrc_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                rvw_hdr_id.Value = reviewHeaderId;
                rvw_aprsr_id.Value = appraiserId;
                rvw_mtrc_id.Value = metricId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    reviewResultsList.Add(new ReviewResult()
                    {
                        ReviewMetricId = reader["rvw_mtrc_id"] == DBNull.Value ? 0 : (int)reader["rvw_mtrc_id"],
                        ReviewHeaderId = reader["rvw_hdr_id"] == DBNull.Value ? 0 : (int)reader["rvw_hdr_id"],
                        ReviewSessionId = reader["rvw_sxn_id"] == DBNull.Value ? 0 : (int)reader["rvw_sxn_id"],
                        ReviewSessionName = reader["rvw_sxn_nm"] == DBNull.Value ? string.Empty : reader["rvw_sxn_nm"].ToString(),

                        AppraiseeId = reader["rvw_emp_id"] == DBNull.Value ? string.Empty : reader["rvw_emp_id"].ToString(),
                        AppraiseeName = reader["appraisee_nm"] == DBNull.Value ? string.Empty : reader["appraisee_nm"].ToString(),

                        ReviewYearId = reader["rvw_yr_id"] == DBNull.Value ? 0 : (int)reader["rvw_yr_id"],
                        ReviewYearName = reader["pms_yr_nm"] == DBNull.Value ? string.Empty : reader["pms_yr_nm"].ToString(),

                        AppraiserId = reader["rvw_aprsr_id"] == DBNull.Value ? string.Empty : reader["rvw_aprsr_id"].ToString(),
                        AppraiserName = reader["appraiser_nm"] == DBNull.Value ? string.Empty : reader["appraiser_nm"].ToString(),

                        ReviewMetricTypeId = reader["mtrc_typ_id"] == DBNull.Value ? 0 : (int)reader["mtrc_typ_id"],
                        ReviewMetricTypeDescription = reader["mtrc_typ_ds"] == DBNull.Value ? string.Empty : reader["mtrc_typ_ds"].ToString(),
                        ReviewMetricDescription = reader["mtrc_ds"] == DBNull.Value ? string.Empty : reader["mtrc_ds"].ToString(),

                        ReviewMetricMeasurement = reader["mtrc_kpi"] == DBNull.Value ? string.Empty : reader["mtrc_kpi"].ToString(),
                        ReviewMetricTarget = reader["mtrc_tgt"] == DBNull.Value ? string.Empty : reader["mtrc_tgt"].ToString(),
                        ReviewMetricWeightage = reader["mtrc_wtg"] == DBNull.Value ? 0.00M : (decimal)reader["mtrc_wtg"],

                        ReviewResultId = reader["rvw_dtl_id"] == DBNull.Value ? 0 : (int)reader["rvw_dtl_id"],
                        AppraiserRoleId = reader["aprsr_rl_id"] == DBNull.Value ? (int?)null : (int)reader["aprsr_rl_id"],
                        AppraiserRoleName = reader["aprv_rl_nm"] == DBNull.Value ? "Appraisee" : reader["aprv_rl_nm"].ToString(),

                        AppraiserTypeId = reader["aprsr_typ_id"] == DBNull.Value ? 0 : (int)reader["aprsr_typ_id"],
                        AppraiserTypeDescription = reader["aprsr_typ_ds"] == DBNull.Value ? string.Empty : reader["aprsr_typ_ds"].ToString(),

                        ActualAchievement = reader["actl_achvmts"] == DBNull.Value ? string.Empty : reader["actl_achvmts"].ToString(),
                        AppraiserScore = reader["aprsr_score"] == DBNull.Value ? 0.00M : (decimal)reader["aprsr_score"],
                        AppraiserScoreDescription = reader["score_ds"] == DBNull.Value ? string.Empty : reader["score_ds"].ToString(),
                        AppraiserScoreValue = reader["score_val"] == DBNull.Value ? 0.00M : (decimal)reader["score_val"],

                        ScoreTime = reader["score_time"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["score_time"],
                        AppraiserComment = reader["aprsr_rmks"] == DBNull.Value ? string.Empty : reader["aprsr_rmks"].ToString(),

                        AppraiseeAchievement = reader["app_achvmts"] == DBNull.Value ? string.Empty : reader["app_achvmts"].ToString(),
                        AppraiseeScore = reader["app_score"] == DBNull.Value ? string.Empty : reader["app_score"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return reviewResultsList;
        }

        public async Task<IList<ReviewResult>> GetById(int reviewResultId)
        {
            List<ReviewResult> reviewResultsList = new List<ReviewResult>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT d.rvw_dtl_id, d.rvw_sxn_id, d.rvw_hdr_id, d.rvw_yr_id, ");
            sb.Append("d.rvw_emp_id, d.rvw_aprsr_id, d.aprsr_rl_id, d.aprsr_typ_id, ");
            sb.Append("d.rvw_mtric_id, d.actl_achvmts, d.aprsr_score, d.score_time, ");
            sb.Append("d.aprsr_rmks, m.mtrc_typ_id, m.mtrc_ds, m.mtrc_kpi, m.mtrc_tgt, ");
            sb.Append("m.mtrc_wtg, h.rvw_emp_id, s.rvw_sxn_nm, y.pms_yr_nm, ");
            sb.Append("e.fullname as appraisee_nm, r.aprv_rl_nm, d.score_ds, ");
            sb.Append("CASE m.mtrc_typ_id WHEN 0 THEN 'KPA' WHEN 1 THEN 'Competency' ");
            sb.Append("END mtrc_typ_ds, CASE d.aprsr_typ_id WHEN 0 THEN 'Self Appraiser' ");
            sb.Append("WHEN 1 THEN 'Principal Appraiser' WHEN 2 THEN '3rd Party Appraiser' ");
            sb.Append("END aprsr_typ_ds, f.fullname as appraiser_nm, d.score_val ");
            sb.Append("FROM public.pmsrvwrdtls d ");
            sb.Append("INNER JOIN public.pmsrvwmtrcs m ON m.rvw_mtrc_id = d.rvw_mtric_id ");
            sb.Append("INNER JOIN public.pmsrvwhdrs h ON h.rvw_hdr_id = m.rvw_hdr_id ");
            sb.Append("INNER JOIN public.pmsrvwsxns s ON s.rvw_sxn_id = m.rvw_sxn_id ");
            sb.Append("INNER JOIN public.pmssttyrs y ON y.pms_yr_id = m.rvw_yr_id ");
            sb.Append("INNER JOIN public.gst_prsns e ON e.id = h.rvw_emp_id ");
            sb.Append("LEFT JOIN public.gst_prsns f ON f.id = d.rvw_aprsr_id ");
            sb.Append("LEFT JOIN public.pmsaprvrls r ON r.aprv_rl_id = d.aprsr_rl_id ");
            sb.Append("WHERE (d.rvw_dtl_id = @rvw_dtl_id);");

            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_dtl_id = cmd.Parameters.Add("@rvw_dtl_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                rvw_dtl_id.Value = reviewResultId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    reviewResultsList.Add(new ReviewResult()
                    {
                        ReviewMetricId = reader["rvw_mtric_id"] == DBNull.Value ? 0 : (int)reader["rvw_mtric_id"],
                        ReviewHeaderId = reader["rvw_hdr_id"] == DBNull.Value ? 0 : (int)reader["rvw_hdr_id"],
                        ReviewSessionId = reader["rvw_sxn_id"] == DBNull.Value ? 0 : (int)reader["rvw_sxn_id"],
                        ReviewSessionName = reader["rvw_sxn_nm"] == DBNull.Value ? string.Empty : reader["rvw_sxn_nm"].ToString(),

                        AppraiseeId = reader["rvw_emp_id"] == DBNull.Value ? string.Empty : reader["rvw_emp_id"].ToString(),
                        AppraiseeName = reader["appraisee_nm"] == DBNull.Value ? string.Empty : reader["appraisee_nm"].ToString(),

                        ReviewYearId = reader["rvw_yr_id"] == DBNull.Value ? 0 : (int)reader["rvw_yr_id"],
                        ReviewYearName = reader["pms_yr_nm"] == DBNull.Value ? string.Empty : reader["pms_yr_nm"].ToString(),

                        AppraiserId = reader["rvw_aprsr_id"] == DBNull.Value ? string.Empty : reader["rvw_aprsr_id"].ToString(),
                        AppraiserName = reader["appraiser_nm"] == DBNull.Value ? string.Empty : reader["appraiser_nm"].ToString(),

                        ReviewMetricTypeId = reader["mtrc_typ_id"] == DBNull.Value ? 0 : (int)reader["mtrc_typ_id"],
                        ReviewMetricTypeDescription = reader["mtrc_typ_ds"] == DBNull.Value ? string.Empty : reader["mtrc_typ_ds"].ToString(),
                        ReviewMetricDescription = reader["mtrc_ds"] == DBNull.Value ? string.Empty : reader["mtrc_ds"].ToString(),

                        ReviewMetricMeasurement = reader["mtrc_kpi"] == DBNull.Value ? string.Empty : reader["mtrc_kpi"].ToString(),
                        ReviewMetricTarget = reader["mtrc_tgt"] == DBNull.Value ? string.Empty : reader["mtrc_tgt"].ToString(),
                        ReviewMetricWeightage = reader["mtrc_wtg"] == DBNull.Value ? 0.00M : (decimal)reader["mtrc_wtg"],

                        ReviewResultId = reader["rvw_dtl_id"] == DBNull.Value ? 0 : (int)reader["rvw_dtl_id"],
                        AppraiserRoleId = reader["aprsr_rl_id"] == DBNull.Value ? (int?)null : (int)reader["aprsr_rl_id"],
                        AppraiserRoleName = reader["aprv_rl_nm"] == DBNull.Value ? "Appraisee" : reader["aprv_rl_nm"].ToString(),

                        AppraiserTypeId = reader["aprsr_typ_id"] == DBNull.Value ? 0 : (int)reader["aprsr_typ_id"],
                        AppraiserTypeDescription = reader["aprsr_typ_ds"] == DBNull.Value ? string.Empty : reader["aprsr_typ_ds"].ToString(),

                        ActualAchievement = reader["actl_achvmts"] == DBNull.Value ? string.Empty : reader["actl_achvmts"].ToString(),
                        AppraiserScore = reader["aprsr_score"] == DBNull.Value ? 0.00M : (decimal)reader["aprsr_score"],
                        AppraiserScoreDescription = reader["score_ds"] == DBNull.Value ? string.Empty : reader["score_ds"].ToString(),
                        AppraiserScoreValue = reader["score_val"] == DBNull.Value ? 0.00M : (decimal)reader["score_val"],

                        ScoreTime = reader["score_time"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["score_time"],
                        AppraiserComment = reader["aprsr_rmks"] == DBNull.Value ? string.Empty : reader["aprsr_rmks"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return reviewResultsList;
        }

        public async Task<IList<ReviewResult>> GetByAppraiserIdAndMetricTypeId(int reviewHeaderId, string appraiserId, int reviewMetricTypeId)
        {
            List<ReviewResult> reviewResultsList = new List<ReviewResult>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT d.rvw_dtl_id, d.rvw_sxn_id, d.rvw_hdr_id, d.rvw_yr_id, ");
            sb.Append("d.rvw_emp_id, d.rvw_aprsr_id, d.aprsr_rl_id, d.aprsr_typ_id, ");
            sb.Append("d.rvw_mtric_id, d.actl_achvmts, d.aprsr_score, d.score_time, ");
            sb.Append("d.aprsr_rmks, m.mtrc_typ_id, m.mtrc_ds, m.mtrc_kpi, m.mtrc_tgt, ");
            sb.Append("m.mtrc_wtg, h.rvw_emp_id, s.rvw_sxn_nm, y.pms_yr_nm, ");
            sb.Append("e.fullname as appraisee_nm, r.aprv_rl_nm, d.score_ds, ");
            sb.Append("CASE m.mtrc_typ_id WHEN 0 THEN 'KPA' WHEN 1 THEN 'Competency' ");
            sb.Append("END mtrc_typ_ds, CASE d.aprsr_typ_id WHEN 0 THEN 'Self Appraiser' ");
            sb.Append("WHEN 1 THEN 'Principal Appraiser' WHEN 2 THEN '3rd Party Appraiser' ");
            sb.Append("END aprsr_typ_ds, f.fullname as appraiser_nm, d.score_val ");
            sb.Append("FROM public.pmsrvwrdtls d ");
            sb.Append("INNER JOIN public.pmsrvwmtrcs m ON m.rvw_mtrc_id = d.rvw_mtric_id ");
            sb.Append("INNER JOIN public.pmsrvwhdrs h ON (h.rvw_hdr_id = m.rvw_hdr_id ");
            sb.Append("AND m.rvw_hdr_id = @rvw_hdr_id) ");
            sb.Append("INNER JOIN public.pmsrvwsxns s ON s.rvw_sxn_id = m.rvw_sxn_id ");
            sb.Append("INNER JOIN public.pmssttyrs y ON y.pms_yr_id = m.rvw_yr_id ");
            sb.Append("INNER JOIN public.gst_prsns e ON e.id = h.rvw_emp_id ");
            sb.Append("LEFT JOIN public.gst_prsns f ON f.id = d.rvw_aprsr_id ");
            sb.Append("LEFT JOIN public.pmsaprvrls r ON r.aprv_rl_id = d.aprsr_rl_id ");
            sb.Append("WHERE (d.rvw_hdr_id = @rvw_hdr_id) ");
            sb.Append("AND (d.rvw_aprsr_id = @rvw_aprsr_id) ");
            sb.Append("AND (m.mtrc_typ_id = @mtrc_typ_id) ");
            sb.Append("ORDER BY d.rvw_dtl_id;");

            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_hdr_id = cmd.Parameters.Add("@rvw_hdr_id", NpgsqlDbType.Integer);
                var rvw_aprsr_id = cmd.Parameters.Add("@rvw_aprsr_id", NpgsqlDbType.Text);
                var mtrc_typ_id = cmd.Parameters.Add("@mtrc_typ_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                rvw_hdr_id.Value = reviewHeaderId;
                rvw_aprsr_id.Value = appraiserId;
                mtrc_typ_id.Value = reviewMetricTypeId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    reviewResultsList.Add(new ReviewResult()
                    {
                        ReviewMetricId = reader["rvw_mtric_id"] == DBNull.Value ? 0 : (int)reader["rvw_mtric_id"],
                        ReviewHeaderId = reader["rvw_hdr_id"] == DBNull.Value ? 0 : (int)reader["rvw_hdr_id"],
                        ReviewSessionId = reader["rvw_sxn_id"] == DBNull.Value ? 0 : (int)reader["rvw_sxn_id"],
                        ReviewSessionName = reader["rvw_sxn_nm"] == DBNull.Value ? string.Empty : reader["rvw_sxn_nm"].ToString(),

                        AppraiseeId = reader["rvw_emp_id"] == DBNull.Value ? string.Empty : reader["rvw_emp_id"].ToString(),
                        AppraiseeName = reader["appraisee_nm"] == DBNull.Value ? string.Empty : reader["appraisee_nm"].ToString(),

                        ReviewYearId = reader["rvw_yr_id"] == DBNull.Value ? 0 : (int)reader["rvw_yr_id"],
                        ReviewYearName = reader["pms_yr_nm"] == DBNull.Value ? string.Empty : reader["pms_yr_nm"].ToString(),

                        AppraiserId = reader["rvw_aprsr_id"] == DBNull.Value ? string.Empty : reader["rvw_aprsr_id"].ToString(),
                        AppraiserName = reader["appraiser_nm"] == DBNull.Value ? string.Empty : reader["appraiser_nm"].ToString(),

                        ReviewMetricTypeId = reader["mtrc_typ_id"] == DBNull.Value ? 0 : (int)reader["mtrc_typ_id"],
                        ReviewMetricTypeDescription = reader["mtrc_typ_ds"] == DBNull.Value ? string.Empty : reader["mtrc_typ_ds"].ToString(),
                        ReviewMetricDescription = reader["mtrc_ds"] == DBNull.Value ? string.Empty : reader["mtrc_ds"].ToString(),

                        ReviewMetricMeasurement = reader["mtrc_kpi"] == DBNull.Value ? string.Empty : reader["mtrc_kpi"].ToString(),
                        ReviewMetricTarget = reader["mtrc_tgt"] == DBNull.Value ? string.Empty : reader["mtrc_tgt"].ToString(),
                        ReviewMetricWeightage = reader["mtrc_wtg"] == DBNull.Value ? 0.00M : (decimal)reader["mtrc_wtg"],

                        ReviewResultId = reader["rvw_dtl_id"] == DBNull.Value ? 0 : (int)reader["rvw_dtl_id"],
                        AppraiserRoleId = reader["aprsr_rl_id"] == DBNull.Value ? (int?)null : (int)reader["aprsr_rl_id"],
                        AppraiserRoleName = reader["aprv_rl_nm"] == DBNull.Value ? "Appraisee" : reader["aprv_rl_nm"].ToString(),

                        AppraiserTypeId = reader["aprsr_typ_id"] == DBNull.Value ? 0 : (int)reader["aprsr_typ_id"],
                        AppraiserTypeDescription = reader["aprsr_typ_ds"] == DBNull.Value ? string.Empty : reader["aprsr_typ_ds"].ToString(),

                        ActualAchievement = reader["actl_achvmts"] == DBNull.Value ? string.Empty : reader["actl_achvmts"].ToString(),
                        AppraiserScore = reader["aprsr_score"] == DBNull.Value ? 0.00M : (decimal)reader["aprsr_score"],
                        AppraiserScoreDescription = reader["score_ds"] == DBNull.Value ? string.Empty : reader["score_ds"].ToString(),
                        AppraiserScoreValue = reader["score_val"] == DBNull.Value ? 0.00M : (decimal)reader["score_val"],

                        ScoreTime = reader["score_time"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["score_time"],
                        AppraiserComment = reader["aprsr_rmks"] == DBNull.Value ? string.Empty : reader["aprsr_rmks"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return reviewResultsList;
        }

        public async Task<int> GetMetricCountByAppraiserIdAndMetricTypeId(int reviewHeaderId, string appraiserId, int reviewMetricTypeId)
        {
            int total_metric_count = 0;
            List<ReviewResult> reviewResultsList = new List<ReviewResult>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT COUNT (d.rvw_dtl_id) as total ");
            sb.Append("FROM public.pmsrvwrdtls d ");
            sb.Append("WHERE (d.rvw_hdr_id = @rvw_hdr_id) ");
            sb.Append("AND (d.rvw_aprsr_id = @rvw_aprsr_id) ");
            //sb.Append("AND (m.mtrc_typ_id = @mtrc_typ_id); ");
            sb.Append("AND (d.rvw_mtric_id IN (SELECT ");
            sb.Append("rvw_mtrc_id FROM public.pmsrvwmtrcs ");
            sb.Append("WHERE rvw_hdr_id = d.rvw_hdr_id ");
            sb.Append("AND mtrc_typ_id = @mtrc_typ_id));");

            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_hdr_id = cmd.Parameters.Add("@rvw_hdr_id", NpgsqlDbType.Integer);
                var rvw_aprsr_id = cmd.Parameters.Add("@rvw_aprsr_id", NpgsqlDbType.Text);
                var mtrc_typ_id = cmd.Parameters.Add("@mtrc_typ_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                rvw_hdr_id.Value = reviewHeaderId;
                rvw_aprsr_id.Value = appraiserId;
                mtrc_typ_id.Value = reviewMetricTypeId;

                var wt = await cmd.ExecuteScalarAsync();
                total_metric_count = Convert.ToInt32(wt);
            }
            await conn.CloseAsync();
            return total_metric_count;
        }

        public async Task<IList<ReviewResult>> GetByAppraiserIdAndMetricId(int reviewHeaderId, string appraiserId, int reviewMetricId)
        {
            List<ReviewResult> reviewResultsList = new List<ReviewResult>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT d.rvw_dtl_id, d.rvw_sxn_id, d.rvw_hdr_id, d.rvw_yr_id, ");
            sb.Append("d.rvw_emp_id, d.rvw_aprsr_id, d.aprsr_rl_id, d.aprsr_typ_id, ");
            sb.Append("d.rvw_mtric_id, d.actl_achvmts, d.aprsr_score, d.score_time, ");
            sb.Append("d.aprsr_rmks, m.mtrc_typ_id, m.mtrc_ds, m.mtrc_kpi, m.mtrc_tgt, ");
            sb.Append("m.mtrc_wtg, h.rvw_emp_id, s.rvw_sxn_nm, y.pms_yr_nm, d.score_val, ");
            sb.Append("e.fullname as appraisee_nm, r.aprv_rl_nm, d.score_ds, ");
            sb.Append("CASE m.mtrc_typ_id WHEN 0 THEN 'KPA' WHEN 1 THEN 'Competency' ");
            sb.Append("END mtrc_typ_ds, CASE d.aprsr_typ_id WHEN 0 THEN 'Self Appraiser' ");
            sb.Append("WHEN 1 THEN 'Principal Appraiser' WHEN 2 THEN '3rd Party Appraiser' ");
            sb.Append("END aprsr_typ_ds, f.fullname as appraiser_nm ");
            sb.Append("FROM public.pmsrvwrdtls d ");
            sb.Append("INNER JOIN public.pmsrvwmtrcs m ON m.rvw_mtrc_id = d.rvw_mtric_id ");
            sb.Append("INNER JOIN public.pmsrvwhdrs h ON (h.rvw_hdr_id = m.rvw_hdr_id ");
            sb.Append("AND m.rvw_hdr_id = @rvw_hdr_id) ");
            sb.Append("INNER JOIN public.pmsrvwsxns s ON s.rvw_sxn_id = m.rvw_sxn_id ");
            sb.Append("INNER JOIN public.pmssttyrs y ON y.pms_yr_id = m.rvw_yr_id ");
            sb.Append("INNER JOIN public.gst_prsns e ON e.id = h.rvw_emp_id ");
            sb.Append("LEFT JOIN public.gst_prsns f ON f.id = d.rvw_aprsr_id ");
            sb.Append("LEFT JOIN public.pmsaprvrls r ON r.aprv_rl_id = d.aprsr_rl_id ");
            sb.Append("WHERE (d.rvw_hdr_id = @rvw_hdr_id) ");
            sb.Append("AND (d.rvw_aprsr_id = @rvw_aprsr_id) ");
            sb.Append("AND (d.rvw_mtric_id = @rvw_mtric_id) ");
            sb.Append("ORDER BY d.rvw_dtl_id;");

            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_hdr_id = cmd.Parameters.Add("@rvw_hdr_id", NpgsqlDbType.Integer);
                var rvw_aprsr_id = cmd.Parameters.Add("@rvw_aprsr_id", NpgsqlDbType.Text);
                var rvw_mtric_id = cmd.Parameters.Add("@rvw_mtric_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                rvw_hdr_id.Value = reviewHeaderId;
                rvw_aprsr_id.Value = appraiserId;
                rvw_mtric_id.Value = reviewMetricId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    reviewResultsList.Add(new ReviewResult()
                    {
                        ReviewMetricId = reader["rvw_mtric_id"] == DBNull.Value ? 0 : (int)reader["rvw_mtric_id"],
                        ReviewHeaderId = reader["rvw_hdr_id"] == DBNull.Value ? 0 : (int)reader["rvw_hdr_id"],
                        ReviewSessionId = reader["rvw_sxn_id"] == DBNull.Value ? 0 : (int)reader["rvw_sxn_id"],
                        ReviewSessionName = reader["rvw_sxn_nm"] == DBNull.Value ? string.Empty : reader["rvw_sxn_nm"].ToString(),

                        AppraiseeId = reader["rvw_emp_id"] == DBNull.Value ? string.Empty : reader["rvw_emp_id"].ToString(),
                        AppraiseeName = reader["appraisee_nm"] == DBNull.Value ? string.Empty : reader["appraisee_nm"].ToString(),

                        ReviewYearId = reader["rvw_yr_id"] == DBNull.Value ? 0 : (int)reader["rvw_yr_id"],
                        ReviewYearName = reader["pms_yr_nm"] == DBNull.Value ? string.Empty : reader["pms_yr_nm"].ToString(),

                        AppraiserId = reader["rvw_aprsr_id"] == DBNull.Value ? string.Empty : reader["rvw_aprsr_id"].ToString(),
                        AppraiserName = reader["appraiser_nm"] == DBNull.Value ? string.Empty : reader["appraiser_nm"].ToString(),

                        ReviewMetricTypeId = reader["mtrc_typ_id"] == DBNull.Value ? 0 : (int)reader["mtrc_typ_id"],
                        ReviewMetricTypeDescription = reader["mtrc_typ_ds"] == DBNull.Value ? string.Empty : reader["mtrc_typ_ds"].ToString(),
                        ReviewMetricDescription = reader["mtrc_ds"] == DBNull.Value ? string.Empty : reader["mtrc_ds"].ToString(),

                        ReviewMetricMeasurement = reader["mtrc_kpi"] == DBNull.Value ? string.Empty : reader["mtrc_kpi"].ToString(),
                        ReviewMetricTarget = reader["mtrc_tgt"] == DBNull.Value ? string.Empty : reader["mtrc_tgt"].ToString(),
                        ReviewMetricWeightage = reader["mtrc_wtg"] == DBNull.Value ? 0.00M : (decimal)reader["mtrc_wtg"],

                        ReviewResultId = reader["rvw_dtl_id"] == DBNull.Value ? 0 : (int)reader["rvw_dtl_id"],
                        AppraiserRoleId = reader["aprsr_rl_id"] == DBNull.Value ? (int?)null : (int)reader["aprsr_rl_id"],
                        AppraiserRoleName = reader["aprv_rl_nm"] == DBNull.Value ? "Appraisee" : reader["aprv_rl_nm"].ToString(),

                        AppraiserTypeId = reader["aprsr_typ_id"] == DBNull.Value ? 0 : (int)reader["aprsr_typ_id"],
                        AppraiserTypeDescription = reader["aprsr_typ_ds"] == DBNull.Value ? string.Empty : reader["aprsr_typ_ds"].ToString(),

                        ActualAchievement = reader["actl_achvmts"] == DBNull.Value ? string.Empty : reader["actl_achvmts"].ToString(),
                        AppraiserScore = reader["aprsr_score"] == DBNull.Value ? 0.00M : (decimal)reader["aprsr_score"],
                        AppraiserScoreDescription = reader["score_ds"] == DBNull.Value ? string.Empty : reader["score_ds"].ToString(),
                        AppraiserScoreValue = reader["score_val"] == DBNull.Value ? 0.00M : (decimal)reader["score_val"],

                        ScoreTime = reader["score_time"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["score_time"],
                        AppraiserComment = reader["aprsr_rmks"] == DBNull.Value ? string.Empty : reader["aprsr_rmks"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return reviewResultsList;
        }

        public async Task<IList<ReviewResult>> GetByAppraiserIdAndReviewHeaderId(int reviewHeaderId, string appraiserId)
        {
            List<ReviewResult> reviewResultsList = new List<ReviewResult>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT d.rvw_dtl_id, d.rvw_sxn_id, d.rvw_hdr_id, d.rvw_yr_id, ");
            sb.Append("d.rvw_emp_id, d.rvw_aprsr_id, d.aprsr_rl_id, d.aprsr_typ_id, ");
            sb.Append("d.rvw_mtric_id, d.actl_achvmts, d.aprsr_score, d.score_time, ");
            sb.Append("d.aprsr_rmks, m.mtrc_typ_id, m.mtrc_ds, m.mtrc_kpi, m.mtrc_tgt, ");
            sb.Append("m.mtrc_wtg, h.rvw_emp_id, s.rvw_sxn_nm, y.pms_yr_nm, d.score_val, ");
            sb.Append("e.fullname as appraisee_nm, r.aprv_rl_nm, d.score_ds, ");
            sb.Append("CASE m.mtrc_typ_id WHEN 0 THEN 'KPA' WHEN 1 THEN 'Competency' ");
            sb.Append("END mtrc_typ_ds, CASE d.aprsr_typ_id WHEN 0 THEN 'Self Appraiser' ");
            sb.Append("WHEN 1 THEN 'Principal Appraiser' WHEN 2 THEN '3rd Party Appraiser' ");
            sb.Append("END aprsr_typ_ds, f.fullname as appraiser_nm ");
            sb.Append("FROM public.pmsrvwrdtls d ");
            sb.Append("INNER JOIN public.pmsrvwmtrcs m ON m.rvw_mtrc_id = d.rvw_mtric_id ");
            sb.Append("INNER JOIN public.pmsrvwhdrs h ON (h.rvw_hdr_id = m.rvw_hdr_id ");
            sb.Append("AND m.rvw_hdr_id = @rvw_hdr_id) ");
            sb.Append("INNER JOIN public.pmsrvwsxns s ON s.rvw_sxn_id = m.rvw_sxn_id ");
            sb.Append("INNER JOIN public.pmssttyrs y ON y.pms_yr_id = m.rvw_yr_id ");
            sb.Append("INNER JOIN public.gst_prsns e ON e.id = h.rvw_emp_id ");
            sb.Append("LEFT JOIN public.gst_prsns f ON f.id = d.rvw_aprsr_id ");
            sb.Append("LEFT JOIN public.pmsaprvrls r ON r.aprv_rl_id = d.aprsr_rl_id ");
            sb.Append("WHERE (d.rvw_hdr_id = @rvw_hdr_id) ");
            sb.Append("AND (d.rvw_aprsr_id = @rvw_aprsr_id) ");
            sb.Append("ORDER BY d.rvw_dtl_id;");

            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_hdr_id = cmd.Parameters.Add("@rvw_hdr_id", NpgsqlDbType.Integer);
                var rvw_aprsr_id = cmd.Parameters.Add("@rvw_aprsr_id", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                rvw_hdr_id.Value = reviewHeaderId;
                rvw_aprsr_id.Value = appraiserId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    reviewResultsList.Add(new ReviewResult()
                    {
                        ReviewMetricId = reader["rvw_mtric_id"] == DBNull.Value ? 0 : (int)reader["rvw_mtric_id"],
                        ReviewHeaderId = reader["rvw_hdr_id"] == DBNull.Value ? 0 : (int)reader["rvw_hdr_id"],
                        ReviewSessionId = reader["rvw_sxn_id"] == DBNull.Value ? 0 : (int)reader["rvw_sxn_id"],
                        ReviewSessionName = reader["rvw_sxn_nm"] == DBNull.Value ? string.Empty : reader["rvw_sxn_nm"].ToString(),

                        AppraiseeId = reader["rvw_emp_id"] == DBNull.Value ? string.Empty : reader["rvw_emp_id"].ToString(),
                        AppraiseeName = reader["appraisee_nm"] == DBNull.Value ? string.Empty : reader["appraisee_nm"].ToString(),

                        ReviewYearId = reader["rvw_yr_id"] == DBNull.Value ? 0 : (int)reader["rvw_yr_id"],
                        ReviewYearName = reader["pms_yr_nm"] == DBNull.Value ? string.Empty : reader["pms_yr_nm"].ToString(),

                        AppraiserId = reader["rvw_aprsr_id"] == DBNull.Value ? string.Empty : reader["rvw_aprsr_id"].ToString(),
                        AppraiserName = reader["appraiser_nm"] == DBNull.Value ? string.Empty : reader["appraiser_nm"].ToString(),

                        ReviewMetricTypeId = reader["mtrc_typ_id"] == DBNull.Value ? 0 : (int)reader["mtrc_typ_id"],
                        ReviewMetricTypeDescription = reader["mtrc_typ_ds"] == DBNull.Value ? string.Empty : reader["mtrc_typ_ds"].ToString(),
                        ReviewMetricDescription = reader["mtrc_ds"] == DBNull.Value ? string.Empty : reader["mtrc_ds"].ToString(),

                        ReviewMetricMeasurement = reader["mtrc_kpi"] == DBNull.Value ? string.Empty : reader["mtrc_kpi"].ToString(),
                        ReviewMetricTarget = reader["mtrc_tgt"] == DBNull.Value ? string.Empty : reader["mtrc_tgt"].ToString(),
                        ReviewMetricWeightage = reader["mtrc_wtg"] == DBNull.Value ? 0.00M : (decimal)reader["mtrc_wtg"],

                        ReviewResultId = reader["rvw_dtl_id"] == DBNull.Value ? 0 : (int)reader["rvw_dtl_id"],
                        AppraiserRoleId = reader["aprsr_rl_id"] == DBNull.Value ? (int?)null : (int)reader["aprsr_rl_id"],
                        AppraiserRoleName = reader["aprv_rl_nm"] == DBNull.Value ? "Appraisee" : reader["aprv_rl_nm"].ToString(),

                        AppraiserTypeId = reader["aprsr_typ_id"] == DBNull.Value ? 0 : (int)reader["aprsr_typ_id"],
                        AppraiserTypeDescription = reader["aprsr_typ_ds"] == DBNull.Value ? string.Empty : reader["aprsr_typ_ds"].ToString(),

                        ActualAchievement = reader["actl_achvmts"] == DBNull.Value ? string.Empty : reader["actl_achvmts"].ToString(),
                        AppraiserScore = reader["aprsr_score"] == DBNull.Value ? 0.00M : (decimal)reader["aprsr_score"],
                        AppraiserScoreDescription = reader["score_ds"] == DBNull.Value ? string.Empty : reader["score_ds"].ToString(),
                        AppraiserScoreValue = reader["score_val"] == DBNull.Value ? 0.00M : (decimal)reader["score_val"],

                        ScoreTime = reader["score_time"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["score_time"],
                        AppraiserComment = reader["aprsr_rmks"] == DBNull.Value ? string.Empty : reader["aprsr_rmks"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return reviewResultsList;
        }

        public async Task<List<string>> GetAppraisersByReviewHeaderId(int reviewHeaderId)
        {
            List<string> appraisersList = new List<string>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT DISTINCT rvw_aprsr_id ");
            sb.Append("FROM public.pmsrvwrdtls ");
            sb.Append("WHERE (rvw_hdr_id = @rvw_hdr_id); ");

            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_hdr_id = cmd.Parameters.Add("@rvw_hdr_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                rvw_hdr_id.Value = reviewHeaderId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    appraisersList.Add(
                         reader["rvw_aprsr_id"] == DBNull.Value ? string.Empty : reader["rvw_aprsr_id"].ToString()
                       );
                }
            }
            await conn.CloseAsync();
            return appraisersList;
        }

        #endregion


        #region Review Score Read Action Methods
        public async Task<List<ReviewScore>> GetScoresByReviewHeaderIdAndAppraiserIdAsync(int reviewHeaderId, string appraiserId)
        {
            List<ReviewScore> reviewScoreList = new List<ReviewScore>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT d.rvw_hdr_id, d.rvw_aprsr_id, m.mtrc_typ_id, ");
            sb.Append("SUM(score_val) AS total_score FROM public.pmsrvwrdtls d ");
            sb.Append("INNER JOIN public.pmsrvwmtrcs m ON (m.rvw_mtrc_id = d.rvw_mtric_id ");
            sb.Append("AND m.rvw_hdr_id = @rvw_hdr_id) ");
            sb.Append("WHERE (d.rvw_hdr_id = @rvw_hdr_id) ");
            sb.Append("AND (d.rvw_aprsr_id = @rvw_aprsr_id) ");
            sb.Append("GROUP BY d.rvw_hdr_id, d.rvw_aprsr_id, m.mtrc_typ_id ");

            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_hdr_id = cmd.Parameters.Add("@rvw_hdr_id", NpgsqlDbType.Integer);
                var rvw_aprsr_id = cmd.Parameters.Add("@rvw_aprsr_id", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                rvw_hdr_id.Value = reviewHeaderId;
                rvw_aprsr_id.Value = appraiserId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    reviewScoreList.Add(new ReviewScore
                    {
                        ReviewMetricTypeId = reader["mtrc_typ_id"] == DBNull.Value ? 0 : (int)reader["mtrc_typ_id"],
                        ReviewHeaderId = reader["rvw_hdr_id"] == DBNull.Value ? 0 : (int)reader["rvw_hdr_id"],
                        AppraiserId = reader["rvw_aprsr_id"] == DBNull.Value ? string.Empty : reader["rvw_aprsr_id"].ToString(),
                        TotalScore = reader["total_score"] == DBNull.Value ? 0.00M : (decimal)reader["total_score"],
                    });
                }
            }
            await conn.CloseAsync();
            return reviewScoreList;
        }

        #endregion

        #region Review Result Write Action Methods
        public async Task<bool> AddAsync(ReviewResult reviewResult)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.pmsrvwrdtls( rvw_sxn_id, rvw_hdr_id, ");
            sb.Append("rvw_yr_id, rvw_emp_id, rvw_aprsr_id, aprsr_rl_id, ");
            sb.Append("aprsr_typ_id, rvw_mtric_id, actl_achvmts, aprsr_score, ");
            sb.Append("score_time, aprsr_rmks, score_ds, score_val) ");
            sb.Append("VALUES (@rvw_sxn_id, @rvw_hdr_id, @rvw_yr_id, ");
            sb.Append("@rvw_emp_id, @rvw_aprsr_id, @aprsr_rl_id, @aprsr_typ_id, ");
            sb.Append("@rvw_mtric_id, @actl_achvmts, @aprsr_score, ");
            sb.Append("@score_time, @aprsr_rmks, @score_ds, @score_val); ");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var rvw_sxn_id = cmd.Parameters.Add("@rvw_sxn_id", NpgsqlDbType.Integer);
                    var rvw_hdr_id = cmd.Parameters.Add("@rvw_hdr_id", NpgsqlDbType.Integer);
                    var rvw_yr_id = cmd.Parameters.Add("@rvw_yr_id", NpgsqlDbType.Integer);
                    var rvw_emp_id = cmd.Parameters.Add("@rvw_emp_id", NpgsqlDbType.Text);
                    var rvw_aprsr_id = cmd.Parameters.Add("@rvw_aprsr_id", NpgsqlDbType.Text);
                    var aprsr_rl_id = cmd.Parameters.Add("@aprsr_rl_id", NpgsqlDbType.Integer);
                    var aprsr_typ_id = cmd.Parameters.Add("@aprsr_typ_id", NpgsqlDbType.Integer);
                    var rvw_mtric_id = cmd.Parameters.Add("@rvw_mtric_id", NpgsqlDbType.Integer);
                    var actl_achvmts = cmd.Parameters.Add("@actl_achvmts", NpgsqlDbType.Text);
                    var aprsr_score = cmd.Parameters.Add("@aprsr_score", NpgsqlDbType.Numeric);
                    var score_time = cmd.Parameters.Add("@score_time", NpgsqlDbType.TimestampTz);
                    var aprsr_rmks = cmd.Parameters.Add("@aprsr_rmks", NpgsqlDbType.Text);
                    var score_ds = cmd.Parameters.Add("@score_ds", NpgsqlDbType.Text);
                    var score_val = cmd.Parameters.Add("@score_val", NpgsqlDbType.Numeric);
                    cmd.Prepare();
                    rvw_sxn_id.Value = reviewResult.ReviewSessionId;
                    rvw_emp_id.Value = reviewResult.AppraiseeId;
                    rvw_hdr_id.Value = reviewResult.ReviewHeaderId;
                    rvw_yr_id.Value = reviewResult.ReviewYearId;
                    rvw_aprsr_id.Value = reviewResult.AppraiserId;
                    aprsr_rl_id.Value = reviewResult.AppraiserRoleId ?? (object)DBNull.Value;
                    aprsr_typ_id.Value = reviewResult.AppraiserTypeId;
                    rvw_mtric_id.Value = reviewResult.ReviewMetricId;
                    actl_achvmts.Value = reviewResult.ActualAchievement ?? (object)DBNull.Value;
                    aprsr_score.Value = reviewResult.AppraiserScore;
                    score_time.Value = reviewResult.ScoreTime;
                    aprsr_rmks.Value = reviewResult.AppraiserComment ?? (object)DBNull.Value;
                    score_ds.Value = reviewResult.AppraiserScoreDescription ?? (object)DBNull.Value;
                    score_val.Value = reviewResult.AppraiserScoreValue;

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

        public async Task<bool> UpdateAsync(ReviewResult reviewResult)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.pmsrvwrdtls SET actl_achvmts=@actl_achvmts, ");
            sb.Append("aprsr_score=@aprsr_score, score_time=@score_time, ");
            sb.Append("aprsr_rmks=@aprsr_rmks, score_ds=@score_ds, ");
            sb.Append("score_val=@score_val ");
            sb.Append("WHERE (rvw_dtl_id=@rvw_dtl_id); ");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var rvw_dtl_id = cmd.Parameters.Add("@rvw_dtl_id", NpgsqlDbType.Integer);
                    var actl_achvmts = cmd.Parameters.Add("@actl_achvmts", NpgsqlDbType.Text);
                    var aprsr_score = cmd.Parameters.Add("@aprsr_score", NpgsqlDbType.Numeric);
                    var score_time = cmd.Parameters.Add("@score_time", NpgsqlDbType.TimestampTz);
                    var aprsr_rmks = cmd.Parameters.Add("@aprsr_rmks", NpgsqlDbType.Text);
                    var score_ds = cmd.Parameters.Add("@score_ds", NpgsqlDbType.Text);
                    var score_val = cmd.Parameters.Add("@score_val", NpgsqlDbType.Numeric);

                    cmd.Prepare();
                    rvw_dtl_id.Value = reviewResult.ReviewResultId;
                    actl_achvmts.Value = reviewResult.ActualAchievement ?? (object)DBNull.Value;
                    aprsr_score.Value = reviewResult.AppraiserScore;
                    score_time.Value = reviewResult.ScoreTime;
                    aprsr_rmks.Value = reviewResult.AppraiserComment ?? (object)DBNull.Value;
                    score_ds.Value = reviewResult.AppraiserScoreDescription ?? (object)DBNull.Value;
                    score_val.Value = reviewResult.AppraiserScoreValue;

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


        public async Task<bool> DeleteEvaluationsByReviewHeaderIdAsync(int reviewHeaderId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("DELETE FROM public.pmsrvwrdtls ");
            sb.Append("WHERE (rvw_hdr_id=@rvw_hdr_id); ");

            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_hdr_id = cmd.Parameters.Add("@rvw_hdr_id", NpgsqlDbType.Integer);
                cmd.Prepare();
                rvw_hdr_id.Value = reviewHeaderId;
                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<bool> DeleteEvaluationsExceptSelfEvaluationAsync(int reviewHeaderId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("DELETE FROM public.pmsrvwrdtls ");
            sb.Append("WHERE (rvw_hdr_id=@rvw_hdr_id) ");
            sb.Append("AND (aprsr_typ_id > 0);");

            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_hdr_id = cmd.Parameters.Add("@rvw_hdr_id", NpgsqlDbType.Integer);
                cmd.Prepare();
                rvw_hdr_id.Value = reviewHeaderId;
                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        #endregion

        #region Result Summary Read Action Methods
        public async Task<IList<ResultSummary>> GetSummaryByReviewSessionId(int reviewSessionId)
        {
            List<ResultSummary> resultSummaryList = new List<ResultSummary>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT r.rvw_smr_id, r.rvw_hdr_id, r.rvw_sxn_id, r.rvw_emp_id, ");
            sb.Append("r.rvw_aprsr_id, r.kpa_scr_obt, r.cmp_scr_obt, r.cmb_scr_obt, ");
            sb.Append("r.scr_rank, r.scr_rank_ds, r.pfm_rating, r.apr_typ_ds, ");
            sb.Append("r.aprsr_rl_ds, r.rvw_yr_id, s.rvw_sxn_nm,  r.is_main, ");
            sb.Append("(SELECT fullname FROM gst_prsns WHERE id = r.rvw_aprsr_id) as rvw_aprsr_nm, ");
            sb.Append("(SELECT fullname FROM gst_prsns WHERE id = r.rvw_emp_id) as rvw_emp_nm, ");
            sb.Append("y.pms_yr_nm, e.unit_id, e.dept_id, e.loc_id, e.emp_no_1, ");
            sb.Append("e.current_designation, d.deptname, u.unitname, l.locname, ");
            sb.Append("s.ttl_cmp_scr, s.ttl_kpa_scr, s.ttl_cmb_scr ");
            sb.Append("FROM public.pmsrvwsmry r ");
            sb.Append("INNER JOIN public.pmsrvwsxns s ON s.rvw_sxn_id = r.rvw_sxn_id ");
            sb.Append("INNER JOIN public.pmssttyrs y ON y.pms_yr_id = r.rvw_yr_id ");
            sb.Append("INNER JOIN public.erm_emp_inf e ON e.emp_id = r.rvw_emp_id ");
            sb.Append("INNER JOIN public.erm_emp_inf f ON f.emp_id = r.rvw_aprsr_id ");
            sb.Append("INNER JOIN public.gst_depts d ON d.deptqk = e.dept_id ");
            sb.Append("INNER JOIN public.gst_units u ON u.unitqk = e.unit_id ");
            sb.Append("INNER JOIN public.gst_locs l ON l.locqk = e.loc_id ");
            sb.Append("WHERE (r.is_main = true) ");
            sb.Append("AND (r.rvw_sxn_id = @rvw_sxn_id) ");
            sb.Append("ORDER BY d.deptname, u.unitname, rvw_emp_nm, rvw_aprsr_nm;");

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
                    resultSummaryList.Add(new ResultSummary()
                    {
                        ResultSummaryId = reader["rvw_smr_id"] == DBNull.Value ? 0 : (int)reader["rvw_smr_id"],
                        ReviewHeaderId = reader["rvw_hdr_id"] == DBNull.Value ? 0 : (int)reader["rvw_hdr_id"],
                        ReviewSessionId = reader["rvw_sxn_id"] == DBNull.Value ? 0 : (int)reader["rvw_sxn_id"],
                        ReviewSessionName = reader["rvw_sxn_nm"] == DBNull.Value ? string.Empty : reader["rvw_sxn_nm"].ToString(),

                        AppraiseeId = reader["rvw_emp_id"] == DBNull.Value ? string.Empty : reader["rvw_emp_id"].ToString(),
                        AppraiseeName = reader["rvw_emp_nm"] == DBNull.Value ? string.Empty : reader["rvw_emp_nm"].ToString(),

                        ReviewYearId = reader["rvw_yr_id"] == DBNull.Value ? 0 : (int)reader["rvw_yr_id"],
                        ReviewYearName = reader["pms_yr_nm"] == DBNull.Value ? string.Empty : reader["pms_yr_nm"].ToString(),

                        AppraiserId = reader["rvw_aprsr_id"] == DBNull.Value ? string.Empty : reader["rvw_aprsr_id"].ToString(),
                        AppraiserName = reader["rvw_aprsr_nm"] == DBNull.Value ? string.Empty : reader["rvw_aprsr_nm"].ToString(),

                        KpaScoreTotal = reader["ttl_kpa_scr"] == DBNull.Value ? 0.00M : (decimal)reader["ttl_kpa_scr"],
                        KpaScoreObtained = reader["kpa_scr_obt"] == DBNull.Value ? 0.00M : (decimal)reader["kpa_scr_obt"],
                        CompetencyScoreTotal = reader["ttl_cmp_scr"] == DBNull.Value ? 0.00M : (decimal)reader["ttl_cmp_scr"],
                        CompetencyScoreObtained = reader["cmp_scr_obt"] == DBNull.Value ? 0.00M : (decimal)reader["cmp_scr_obt"],
                        CombinedScoreTotal = reader["ttl_cmb_scr"] == DBNull.Value ? 0.00M : (decimal)reader["ttl_cmb_scr"],
                        CombinedScoreObtained = reader["cmb_scr_obt"] == DBNull.Value ? 0.00M : (decimal)reader["cmb_scr_obt"],

                        ScoreRank = reader["scr_rank"] == DBNull.Value ? 0 : (int)reader["scr_rank"],
                        ScoreRankDescription = reader["scr_rank_ds"] == DBNull.Value ? string.Empty : reader["scr_rank_ds"].ToString(),
                        PerformanceRating = reader["pfm_rating"] == DBNull.Value ? string.Empty : reader["pfm_rating"].ToString(),

                        AppraiserTypeDescription = reader["apr_typ_ds"] == DBNull.Value ? string.Empty : reader["apr_typ_ds"].ToString(),
                        AppraiserRoleDescription = reader["aprsr_rl_ds"] == DBNull.Value ? string.Empty : reader["aprsr_rl_ds"].ToString(),

                        UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                        UnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),

                        DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                        DepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),

                        LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                        LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),

                        EmployeeNo = reader["emp_no_1"] == DBNull.Value ? string.Empty : reader["emp_no_1"].ToString(),
                        CurrentDesignation = reader["current_designation"] == DBNull.Value ? string.Empty : reader["current_designation"].ToString(),
                        IsMain = reader["is_main"] == DBNull.Value ? false : (bool)reader["is_main"],
                    });
                }
            }
            await conn.CloseAsync();
            return resultSummaryList;
        }

        public async Task<IList<ResultSummary>> GetSummaryByReviewSessionIdAndDepartmentCode(int reviewSessionId, int departmentId)
        {
            List<ResultSummary> resultSummaryList = new List<ResultSummary>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT r.rvw_smr_id, r.rvw_hdr_id, r.rvw_sxn_id, r.rvw_emp_id, ");
            sb.Append("r.rvw_aprsr_id, r.kpa_scr_obt, r.cmp_scr_obt, r.cmb_scr_obt, ");
            sb.Append("r.scr_rank, r.scr_rank_ds, r.pfm_rating, r.apr_typ_ds, ");
            sb.Append("r.aprsr_rl_ds, r.rvw_yr_id, s.rvw_sxn_nm,  r.is_main, ");

            sb.Append("(SELECT fullname FROM gst_prsns WHERE id = r.rvw_emp_id) as rvw_emp_nm, ");
            sb.Append("(SELECT fullname FROM gst_prsns WHERE id = r.rvw_aprsr_id) as rvw_aprsr_nm, ");

            sb.Append("y.pms_yr_nm, e.unit_id, e.dept_id, e.loc_id, e.emp_no_1, ");
            sb.Append("e.current_designation, d.deptname, u.unitname, l.locname, ");
            sb.Append("s.ttl_cmp_scr, s.ttl_kpa_scr, s.ttl_cmb_scr ");
            sb.Append("FROM public.pmsrvwsmry r ");
            sb.Append("INNER JOIN public.pmsrvwsxns s ON s.rvw_sxn_id = r.rvw_sxn_id ");
            sb.Append("INNER JOIN public.pmssttyrs y ON y.pms_yr_id = r.rvw_yr_id ");
            sb.Append("INNER JOIN public.erm_emp_inf e ON e.emp_id = r.rvw_emp_id ");
            sb.Append("INNER JOIN public.erm_emp_inf f ON f.emp_id = r.rvw_aprsr_id ");
            sb.Append("INNER JOIN public.gst_depts d ON d.deptqk = e.dept_id ");
            sb.Append("INNER JOIN public.gst_units u ON u.unitqk = e.unit_id ");
            sb.Append("INNER JOIN public.gst_locs l ON l.locqk = e.loc_id ");
            sb.Append("WHERE (r.is_main = true) ");
            sb.Append("AND (r.rvw_sxn_id = @rvw_sxn_id) ");
            sb.Append("AND (e.dept_id = @dept_id) ");
            sb.Append("ORDER BY d.deptname, u.unitname, rvw_emp_nm;");

            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_sxn_id = cmd.Parameters.Add("@rvw_sxn_id", NpgsqlDbType.Integer);
                var dept_id = cmd.Parameters.Add("@dept_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                rvw_sxn_id.Value = reviewSessionId;
                dept_id.Value = departmentId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    resultSummaryList.Add(new ResultSummary()
                    {
                        ResultSummaryId = reader["rvw_smr_id"] == DBNull.Value ? 0 : (int)reader["rvw_smr_id"],
                        ReviewHeaderId = reader["rvw_hdr_id"] == DBNull.Value ? 0 : (int)reader["rvw_hdr_id"],
                        ReviewSessionId = reader["rvw_sxn_id"] == DBNull.Value ? 0 : (int)reader["rvw_sxn_id"],
                        ReviewSessionName = reader["rvw_sxn_nm"] == DBNull.Value ? string.Empty : reader["rvw_sxn_nm"].ToString(),

                        AppraiseeId = reader["rvw_emp_id"] == DBNull.Value ? string.Empty : reader["rvw_emp_id"].ToString(),
                        AppraiseeName = reader["rvw_emp_nm"] == DBNull.Value ? string.Empty : reader["rvw_emp_nm"].ToString(),

                        ReviewYearId = reader["rvw_yr_id"] == DBNull.Value ? 0 : (int)reader["rvw_yr_id"],
                        ReviewYearName = reader["pms_yr_nm"] == DBNull.Value ? string.Empty : reader["pms_yr_nm"].ToString(),

                        AppraiserId = reader["rvw_aprsr_id"] == DBNull.Value ? string.Empty : reader["rvw_aprsr_id"].ToString(),
                        AppraiserName = reader["rvw_aprsr_nm"] == DBNull.Value ? string.Empty : reader["rvw_aprsr_nm"].ToString(),

                        KpaScoreTotal = reader["ttl_kpa_scr"] == DBNull.Value ? 0.00M : (decimal)reader["ttl_kpa_scr"],
                        KpaScoreObtained = reader["kpa_scr_obt"] == DBNull.Value ? 0.00M : (decimal)reader["kpa_scr_obt"],
                        CompetencyScoreTotal = reader["ttl_cmp_scr"] == DBNull.Value ? 0.00M : (decimal)reader["ttl_cmp_scr"],
                        CompetencyScoreObtained = reader["cmp_scr_obt"] == DBNull.Value ? 0.00M : (decimal)reader["cmp_scr_obt"],
                        CombinedScoreTotal = reader["ttl_cmb_scr"] == DBNull.Value ? 0.00M : (decimal)reader["ttl_cmb_scr"],
                        CombinedScoreObtained = reader["cmb_scr_obt"] == DBNull.Value ? 0.00M : (decimal)reader["cmb_scr_obt"],

                        ScoreRank = reader["scr_rank"] == DBNull.Value ? 0 : (int)reader["scr_rank"],
                        ScoreRankDescription = reader["scr_rank_ds"] == DBNull.Value ? string.Empty : reader["scr_rank_ds"].ToString(),
                        PerformanceRating = reader["pfm_rating"] == DBNull.Value ? string.Empty : reader["pfm_rating"].ToString(),

                        AppraiserTypeDescription = reader["apr_typ_ds"] == DBNull.Value ? string.Empty : reader["apr_typ_ds"].ToString(),
                        AppraiserRoleDescription = reader["aprsr_rl_ds"] == DBNull.Value ? string.Empty : reader["aprsr_rl_ds"].ToString(),

                        UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                        UnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),

                        DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                        DepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),

                        LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                        LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),

                        EmployeeNo = reader["emp_no_1"] == DBNull.Value ? string.Empty : reader["emp_no_1"].ToString(),
                        CurrentDesignation = reader["current_designation"] == DBNull.Value ? string.Empty : reader["current_designation"].ToString(),
                        IsMain = reader["is_main"] == DBNull.Value ? false : (bool)reader["is_main"],
                    });
                }
            }
            await conn.CloseAsync();
            return resultSummaryList;
        }

        public async Task<IList<ResultSummary>> GetSummaryByReviewSessionIdAndUnitCode(int reviewSessionId, int unitId)
        {
            List<ResultSummary> resultSummaryList = new List<ResultSummary>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT r.rvw_smr_id, r.rvw_hdr_id, r.rvw_sxn_id, r.rvw_emp_id, ");
            sb.Append("r.rvw_aprsr_id, r.kpa_scr_obt, r.cmp_scr_obt, r.cmb_scr_obt, ");
            sb.Append("r.scr_rank, r.scr_rank_ds, r.pfm_rating, r.apr_typ_ds, ");
            sb.Append("r.aprsr_rl_ds, r.rvw_yr_id, s.rvw_sxn_nm,  r.is_main, ");

            sb.Append("(SELECT fullname FROM gst_prsns WHERE id = r.rvw_emp_id) as rvw_emp_nm, ");
            sb.Append("(SELECT fullname FROM gst_prsns WHERE id = r.rvw_aprsr_id) as rvw_aprsr_nm, ");

            sb.Append("y.pms_yr_nm, e.unit_id, e.dept_id, e.loc_id, e.emp_no_1, ");
            sb.Append("e.current_designation, d.deptname, u.unitname, l.locname, ");
            sb.Append("s.ttl_cmp_scr, s.ttl_kpa_scr, s.ttl_cmb_scr ");
            sb.Append("FROM public.pmsrvwsmry r ");
            sb.Append("INNER JOIN public.pmsrvwsxns s ON s.rvw_sxn_id = r.rvw_sxn_id ");
            sb.Append("INNER JOIN public.pmssttyrs y ON y.pms_yr_id = r.rvw_yr_id ");
            sb.Append("INNER JOIN public.erm_emp_inf e ON e.emp_id = r.rvw_emp_id ");
            sb.Append("INNER JOIN public.erm_emp_inf f ON f.emp_id = r.rvw_aprsr_id ");
            sb.Append("INNER JOIN public.gst_depts d ON d.deptqk = e.dept_id ");
            sb.Append("INNER JOIN public.gst_units u ON u.unitqk = e.unit_id ");
            sb.Append("INNER JOIN public.gst_locs l ON l.locqk = e.loc_id ");
            sb.Append("WHERE (r.is_main = true) ");
            sb.Append("AND (r.rvw_sxn_id = @rvw_sxn_id) ");
            sb.Append("AND (e.unit_id = @unit_cd) ");
            sb.Append("ORDER BY d.deptname, u.unitname, e.fullname;");

            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_sxn_id = cmd.Parameters.Add("@rvw_sxn_id", NpgsqlDbType.Integer);
                var unit_cd = cmd.Parameters.Add("@unit_cd", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                rvw_sxn_id.Value = reviewSessionId;
                unit_cd.Value = unitId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    resultSummaryList.Add(new ResultSummary()
                    {
                        ResultSummaryId = reader["rvw_smr_id"] == DBNull.Value ? 0 : (int)reader["rvw_smr_id"],
                        ReviewHeaderId = reader["rvw_hdr_id"] == DBNull.Value ? 0 : (int)reader["rvw_hdr_id"],
                        ReviewSessionId = reader["rvw_sxn_id"] == DBNull.Value ? 0 : (int)reader["rvw_sxn_id"],
                        ReviewSessionName = reader["rvw_sxn_nm"] == DBNull.Value ? string.Empty : reader["rvw_sxn_nm"].ToString(),

                        AppraiseeId = reader["rvw_emp_id"] == DBNull.Value ? string.Empty : reader["rvw_emp_id"].ToString(),
                        AppraiseeName = reader["rvw_emp_nm"] == DBNull.Value ? string.Empty : reader["rvw_emp_nm"].ToString(),

                        ReviewYearId = reader["rvw_yr_id"] == DBNull.Value ? 0 : (int)reader["rvw_yr_id"],
                        ReviewYearName = reader["pms_yr_nm"] == DBNull.Value ? string.Empty : reader["pms_yr_nm"].ToString(),

                        AppraiserId = reader["rvw_aprsr_id"] == DBNull.Value ? string.Empty : reader["rvw_aprsr_id"].ToString(),
                        AppraiserName = reader["rvw_aprsr_nm"] == DBNull.Value ? string.Empty : reader["rvw_aprsr_nm"].ToString(),

                        KpaScoreTotal = reader["ttl_kpa_scr"] == DBNull.Value ? 0.00M : (decimal)reader["ttl_kpa_scr"],
                        KpaScoreObtained = reader["kpa_scr_obt"] == DBNull.Value ? 0.00M : (decimal)reader["kpa_scr_obt"],
                        CompetencyScoreTotal = reader["ttl_cmp_scr"] == DBNull.Value ? 0.00M : (decimal)reader["ttl_cmp_scr"],
                        CompetencyScoreObtained = reader["cmp_scr_obt"] == DBNull.Value ? 0.00M : (decimal)reader["cmp_scr_obt"],
                        CombinedScoreTotal = reader["ttl_cmb_scr"] == DBNull.Value ? 0.00M : (decimal)reader["ttl_cmb_scr"],
                        CombinedScoreObtained = reader["cmb_scr_obt"] == DBNull.Value ? 0.00M : (decimal)reader["cmb_scr_obt"],

                        ScoreRank = reader["scr_rank"] == DBNull.Value ? 0 : (int)reader["scr_rank"],
                        ScoreRankDescription = reader["scr_rank_ds"] == DBNull.Value ? string.Empty : reader["scr_rank_ds"].ToString(),
                        PerformanceRating = reader["pfm_rating"] == DBNull.Value ? string.Empty : reader["pfm_rating"].ToString(),

                        AppraiserTypeDescription = reader["apr_typ_ds"] == DBNull.Value ? string.Empty : reader["apr_typ_ds"].ToString(),
                        AppraiserRoleDescription = reader["aprsr_rl_ds"] == DBNull.Value ? string.Empty : reader["aprsr_rl_ds"].ToString(),

                        UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                        UnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),

                        DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                        DepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),

                        LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                        LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),

                        EmployeeNo = reader["emp_no_1"] == DBNull.Value ? string.Empty : reader["emp_no_1"].ToString(),
                        CurrentDesignation = reader["current_designation"] == DBNull.Value ? string.Empty : reader["current_designation"].ToString(),
                        IsMain = reader["is_main"] == DBNull.Value ? false : (bool)reader["is_main"],
                    });
                }
            }
            await conn.CloseAsync();
            return resultSummaryList;
        }

        public async Task<IList<ResultSummary>> GetSummaryByReviewSessionIdAndAppraiseeName(int reviewSessionId, string appraiseeName)
        {
            List<ResultSummary> resultSummaryList = new List<ResultSummary>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT r.rvw_smr_id, r.rvw_hdr_id, r.rvw_sxn_id, r.rvw_emp_id, ");
            sb.Append("r.rvw_aprsr_id, r.kpa_scr_obt, r.cmp_scr_obt, r.cmb_scr_obt, ");
            sb.Append("r.scr_rank, r.scr_rank_ds, r.pfm_rating, r.apr_typ_ds, ");
            sb.Append("r.aprsr_rl_ds, r.rvw_yr_id, s.rvw_sxn_nm,  r.is_main, ");

            sb.Append("(SELECT fullname FROM gst_prsns WHERE id = r.rvw_emp_id) as rvw_emp_nm, ");
            sb.Append("(SELECT fullname FROM gst_prsns WHERE id = r.rvw_aprsr_id) as rvw_aprsr_nm, ");

            sb.Append("y.pms_yr_nm, e.unit_id, e.dept_id, e.loc_id, e.emp_no_1, ");
            sb.Append("e.current_designation, d.deptname, u.unitname, l.locname, ");
            sb.Append("s.ttl_cmp_scr, s.ttl_kpa_scr, s.ttl_cmb_scr ");
            sb.Append("FROM public.pmsrvwsmry r ");
            sb.Append("INNER JOIN public.pmsrvwsxns s ON s.rvw_sxn_id = r.rvw_sxn_id ");
            sb.Append("INNER JOIN public.pmssttyrs y ON y.pms_yr_id = r.rvw_yr_id ");
            sb.Append("INNER JOIN public.erm_emp_inf e ON e.emp_id = r.rvw_emp_id ");
            sb.Append("INNER JOIN public.erm_emp_inf f ON f.emp_id = r.rvw_aprsr_id ");
            sb.Append("INNER JOIN public.gst_depts d ON d.deptqk = e.dept_id ");
            sb.Append("INNER JOIN public.gst_units u ON u.unitqk = e.unit_id ");
            sb.Append("INNER JOIN public.gst_locs l ON l.locqk = e.loc_id ");
            sb.Append("WHERE (r.is_main = true) ");
            sb.Append("AND (r.rvw_sxn_id = @rvw_sxn_id) ");
            sb.Append("AND ((SELECT fullname FROM gst_prsns WHERE id = r.rvw_emp_id) = @appraisee_name) ");
            sb.Append("ORDER BY d.deptname, u.unitname, rvw_emp_nm;");

            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_sxn_id = cmd.Parameters.Add("@rvw_sxn_id", NpgsqlDbType.Integer);
                var appraisee_name = cmd.Parameters.Add("@appraisee_name", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                rvw_sxn_id.Value = reviewSessionId;
                appraisee_name.Value = appraiseeName;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    resultSummaryList.Add(new ResultSummary()
                    {
                        ResultSummaryId = reader["rvw_smr_id"] == DBNull.Value ? 0 : (int)reader["rvw_smr_id"],
                        ReviewHeaderId = reader["rvw_hdr_id"] == DBNull.Value ? 0 : (int)reader["rvw_hdr_id"],
                        ReviewSessionId = reader["rvw_sxn_id"] == DBNull.Value ? 0 : (int)reader["rvw_sxn_id"],
                        ReviewSessionName = reader["rvw_sxn_nm"] == DBNull.Value ? string.Empty : reader["rvw_sxn_nm"].ToString(),

                        AppraiseeId = reader["rvw_emp_id"] == DBNull.Value ? string.Empty : reader["rvw_emp_id"].ToString(),
                        AppraiseeName = reader["rvw_emp_nm"] == DBNull.Value ? string.Empty : reader["rvw_emp_nm"].ToString(),

                        ReviewYearId = reader["rvw_yr_id"] == DBNull.Value ? 0 : (int)reader["rvw_yr_id"],
                        ReviewYearName = reader["pms_yr_nm"] == DBNull.Value ? string.Empty : reader["pms_yr_nm"].ToString(),

                        AppraiserId = reader["rvw_aprsr_id"] == DBNull.Value ? string.Empty : reader["rvw_aprsr_id"].ToString(),
                        AppraiserName = reader["rvw_aprsr_nm"] == DBNull.Value ? string.Empty : reader["rvw_aprsr_nm"].ToString(),

                        KpaScoreTotal = reader["ttl_kpa_scr"] == DBNull.Value ? 0.00M : (decimal)reader["ttl_kpa_scr"],
                        KpaScoreObtained = reader["kpa_scr_obt"] == DBNull.Value ? 0.00M : (decimal)reader["kpa_scr_obt"],
                        CompetencyScoreTotal = reader["ttl_cmp_scr"] == DBNull.Value ? 0.00M : (decimal)reader["ttl_cmp_scr"],
                        CompetencyScoreObtained = reader["cmp_scr_obt"] == DBNull.Value ? 0.00M : (decimal)reader["cmp_scr_obt"],
                        CombinedScoreTotal = reader["ttl_cmb_scr"] == DBNull.Value ? 0.00M : (decimal)reader["ttl_cmb_scr"],
                        CombinedScoreObtained = reader["cmb_scr_obt"] == DBNull.Value ? 0.00M : (decimal)reader["cmb_scr_obt"],

                        ScoreRank = reader["scr_rank"] == DBNull.Value ? 0 : (int)reader["scr_rank"],
                        ScoreRankDescription = reader["scr_rank_ds"] == DBNull.Value ? string.Empty : reader["scr_rank_ds"].ToString(),
                        PerformanceRating = reader["pfm_rating"] == DBNull.Value ? string.Empty : reader["pfm_rating"].ToString(),

                        AppraiserTypeDescription = reader["apr_typ_ds"] == DBNull.Value ? string.Empty : reader["apr_typ_ds"].ToString(),
                        AppraiserRoleDescription = reader["aprsr_rl_ds"] == DBNull.Value ? string.Empty : reader["aprsr_rl_ds"].ToString(),

                        UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                        UnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),

                        DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                        DepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),

                        LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                        LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),

                        EmployeeNo = reader["emp_no_1"] == DBNull.Value ? string.Empty : reader["emp_no_1"].ToString(),
                        CurrentDesignation = reader["current_designation"] == DBNull.Value ? string.Empty : reader["current_designation"].ToString(),
                        IsMain = reader["is_main"] == DBNull.Value ? false : (bool)reader["is_main"],
                    });
                }
            }
            await conn.CloseAsync();
            return resultSummaryList;
        }

        public async Task<IList<ResultSummary>> GetSummaryByAppraiserIdAndReviewHeaderId(int reviewHeaderId, string appraiserId)
        {
            List<ResultSummary> resultSummaryList = new List<ResultSummary>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT r.rvw_smr_id, r.rvw_hdr_id, r.rvw_sxn_id, r.rvw_emp_id, ");
            sb.Append("r.rvw_aprsr_id, r.kpa_scr_obt, r.cmp_scr_obt, r.cmb_scr_obt, ");
            sb.Append("r.scr_rank, r.scr_rank_ds, r.pfm_rating, r.apr_typ_ds, ");
            sb.Append("r.aprsr_rl_ds, r.rvw_yr_id, s.rvw_sxn_nm,  r.is_main, ");

            sb.Append("(SELECT fullname FROM gst_prsns WHERE id = r.rvw_emp_id) as rvw_emp_nm, ");
            sb.Append("(SELECT fullname FROM gst_prsns WHERE id = r.rvw_aprsr_id) as rvw_aprsr_nm, ");

            sb.Append("y.pms_yr_nm, e.unit_id, e.dept_id, e.loc_id, e.emp_no_1, ");
            sb.Append("e.current_designation, d.deptname, u.unitname, l.locname, ");
            sb.Append("s.ttl_cmp_scr, s.ttl_kpa_scr, s.ttl_cmb_scr ");
            sb.Append("FROM public.pmsrvwsmry r ");
            sb.Append("INNER JOIN public.pmsrvwsxns s ON s.rvw_sxn_id = r.rvw_sxn_id ");
            sb.Append("INNER JOIN public.pmssttyrs y ON y.pms_yr_id = r.rvw_yr_id ");
            sb.Append("INNER JOIN public.erm_emp_inf e ON e.emp_id = r.rvw_emp_id ");
            sb.Append("INNER JOIN public.erm_emp_inf f ON f.emp_id = r.rvw_aprsr_id ");
            sb.Append("INNER JOIN public.gst_depts d ON d.deptqk = e.dept_id ");
            sb.Append("INNER JOIN public.gst_units u ON u.unitqk = e.unit_id ");
            sb.Append("INNER JOIN public.gst_locs l ON l.locqk = e.loc_id ");
            sb.Append("WHERE (r.rvw_hdr_id = @rvw_hdr_id) ");
            sb.Append("AND (r.rvw_aprsr_id = @rvw_aprsr_id) ");
            sb.Append("ORDER BY r.rvw_smr_id;");

            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_hdr_id = cmd.Parameters.Add("@rvw_hdr_id", NpgsqlDbType.Integer);
                var rvw_aprsr_id = cmd.Parameters.Add("@rvw_aprsr_id", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                rvw_hdr_id.Value = reviewHeaderId;
                rvw_aprsr_id.Value = appraiserId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    resultSummaryList.Add(new ResultSummary()
                    {
                        ResultSummaryId = reader["rvw_smr_id"] == DBNull.Value ? 0 : (int)reader["rvw_smr_id"],
                        ReviewHeaderId = reader["rvw_hdr_id"] == DBNull.Value ? 0 : (int)reader["rvw_hdr_id"],
                        ReviewSessionId = reader["rvw_sxn_id"] == DBNull.Value ? 0 : (int)reader["rvw_sxn_id"],
                        ReviewSessionName = reader["rvw_sxn_nm"] == DBNull.Value ? string.Empty : reader["rvw_sxn_nm"].ToString(),

                        AppraiseeId = reader["rvw_emp_id"] == DBNull.Value ? string.Empty : reader["rvw_emp_id"].ToString(),
                        AppraiseeName = reader["rvw_emp_nm"] == DBNull.Value ? string.Empty : reader["rvw_emp_nm"].ToString(),

                        ReviewYearId = reader["rvw_yr_id"] == DBNull.Value ? 0 : (int)reader["rvw_yr_id"],
                        ReviewYearName = reader["pms_yr_nm"] == DBNull.Value ? string.Empty : reader["pms_yr_nm"].ToString(),

                        AppraiserId = reader["rvw_aprsr_id"] == DBNull.Value ? string.Empty : reader["rvw_aprsr_id"].ToString(),
                        AppraiserName = reader["rvw_aprsr_nm"] == DBNull.Value ? string.Empty : reader["rvw_aprsr_nm"].ToString(),

                        KpaScoreTotal = reader["ttl_kpa_scr"] == DBNull.Value ? 0.00M : (decimal)reader["ttl_kpa_scr"],
                        KpaScoreObtained = reader["kpa_scr_obt"] == DBNull.Value ? 0.00M : (decimal)reader["kpa_scr_obt"],
                        CompetencyScoreTotal = reader["ttl_cmp_scr"] == DBNull.Value ? 0.00M : (decimal)reader["ttl_cmp_scr"],
                        CompetencyScoreObtained = reader["cmp_scr_obt"] == DBNull.Value ? 0.00M : (decimal)reader["cmp_scr_obt"],
                        CombinedScoreTotal = reader["ttl_cmb_scr"] == DBNull.Value ? 0.00M : (decimal)reader["ttl_cmb_scr"],
                        CombinedScoreObtained = reader["cmb_scr_obt"] == DBNull.Value ? 0.00M : (decimal)reader["cmb_scr_obt"],

                        ScoreRank = reader["scr_rank"] == DBNull.Value ? 0 : (int)reader["scr_rank"],
                        ScoreRankDescription = reader["scr_rank_ds"] == DBNull.Value ? string.Empty : reader["scr_rank_ds"].ToString(),
                        PerformanceRating = reader["pfm_rating"] == DBNull.Value ? string.Empty : reader["pfm_rating"].ToString(),

                        AppraiserTypeDescription = reader["apr_typ_ds"] == DBNull.Value ? string.Empty : reader["apr_typ_ds"].ToString(),
                        AppraiserRoleDescription = reader["aprsr_rl_ds"] == DBNull.Value ? string.Empty : reader["aprsr_rl_ds"].ToString(),

                        UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                        UnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),

                        DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                        DepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),

                        LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                        LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),

                        EmployeeNo = reader["emp_no_1"] == DBNull.Value ? string.Empty : reader["emp_no_1"].ToString(),
                        CurrentDesignation = reader["current_designation"] == DBNull.Value ? string.Empty : reader["current_designation"].ToString(),
                        IsMain = reader["is_main"] == DBNull.Value ? false : (bool)reader["is_main"],
                    });
                }
            }
            await conn.CloseAsync();
            return resultSummaryList;
        }

        public async Task<List<AppraiserDetail>> GetAppraisersDetailsByReviewHeaderId(int reviewHeaderId)
        {
            List<AppraiserDetail> appraiserDetailList = new List<AppraiserDetail>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT r.rvw_smr_id, r.rvw_hdr_id, r.rvw_sxn_id, r.rvw_aprsr_id, ");
            sb.Append("r.apr_typ_ds, r.aprsr_rl_ds, f.current_designation, f.emp_no_1, ");
            sb.Append("d.deptname, u.unitname, (SELECT fullname FROM gst_prsns ");
            sb.Append("WHERE id = r.rvw_aprsr_id) as rvw_aprsr_nm, ");
           // sb.Append("(SELECT rvw_aprsr_nm ||' ('||r.apr_typ_ds||')') AS aprsr_ds, ");
            sb.Append("l.locname FROM public.pmsrvwsmry r ");
            sb.Append("INNER JOIN public.erm_emp_inf f ON f.emp_id = r.rvw_aprsr_id ");
            sb.Append("INNER JOIN public.gst_depts d ON d.deptqk = f.dept_id ");
            sb.Append("INNER JOIN public.gst_units u ON u.unitqk = f.unit_id ");
            sb.Append("INNER JOIN public.gst_locs l ON l.locqk = f.loc_id ");
            sb.Append("WHERE (r.rvw_hdr_id = @rvw_hdr_id) ");
            sb.Append("ORDER BY r.rvw_smr_id;");

            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_hdr_id = cmd.Parameters.Add("@rvw_hdr_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                rvw_hdr_id.Value = reviewHeaderId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    appraiserDetailList.Add(new AppraiserDetail()
                    {
                        ResultSummaryId = reader["rvw_smr_id"] == DBNull.Value ? 0 : (int)reader["rvw_smr_id"],
                        ReviewHeaderId = reader["rvw_hdr_id"] == DBNull.Value ? 0 : (int)reader["rvw_hdr_id"],
                        ReviewSessionId = reader["rvw_sxn_id"] == DBNull.Value ? 0 : (int)reader["rvw_sxn_id"],
                        AppraiserId = reader["rvw_aprsr_id"] == DBNull.Value ? string.Empty : reader["rvw_aprsr_id"].ToString(),
                        AppraiserName = reader["rvw_aprsr_nm"] == DBNull.Value ? string.Empty : reader["rvw_aprsr_nm"].ToString(),
                        AppraiserTypeDescription = reader["apr_typ_ds"] == DBNull.Value ? string.Empty : reader["apr_typ_ds"].ToString(),
                        AppraiserRoleDescription = reader["aprsr_rl_ds"] == DBNull.Value ? string.Empty : reader["aprsr_rl_ds"].ToString(),
                        AppraiserUnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                        AppraiserDepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                        AppraiserLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                        AppraiserNo = reader["emp_no_1"] == DBNull.Value ? string.Empty : reader["emp_no_1"].ToString(),
                        AppraiserDesignation = reader["current_designation"] == DBNull.Value ? string.Empty : reader["current_designation"].ToString(),
                       // AppraiserFullDescription = reader["aprsr_ds"] == DBNull.Value ? string.Empty : reader["aprsr_ds"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return appraiserDetailList;
        }

        public async Task<IList<ResultSummary>> GetSummaryByReportToId(string reportToId, int reviewSessionId)
        {
            List<ResultSummary> resultSummaryList = new List<ResultSummary>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT r.emp_id, e.emp_no_1, e.current_designation, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = r.emp_id) AS fullname, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = s.rvw_aprsr_id) AS rvw_aprsr_nm, ");
            sb.Append("u.unitname, s.kpa_scr_obt, s.cmp_scr_obt, s.cmb_scr_obt, ");
            sb.Append("s.rvw_hdr_id, s.rvw_aprsr_id, s.apr_typ_ds, s.aprsr_rl_ds, ");
            sb.Append("s.pfm_rating, s.is_main FROM public.erm_emp_rpts r ");
            sb.Append("INNER JOIN public.erm_emp_inf e ON e.emp_id = r.emp_id ");
            sb.Append("INNER JOIN public.gst_units u ON u.unitqk = e.unit_id ");
            sb.Append("LEFT JOIN public.pmsrvwsmry s ON s.rvw_emp_id = r.emp_id ");
            sb.Append("LEFT JOIN public.erm_emp_inf f ON f.emp_id = s.rvw_aprsr_id ");
            sb.Append("WHERE (r.rpt_nds IS NULL OR r.rpt_nds > CURRENT_DATE) ");
            sb.Append("AND (r.rpt_emp_id = @rpt_emp_id AND s.rvw_sxn_id = @rvw_sxn_id) ");
            //sb.Append("AND (s.is_main = true) ");
            sb.Append("ORDER BY u.unitname, s.cmb_scr_obt DESC;");

            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_sxn_id = cmd.Parameters.Add("@rvw_sxn_id", NpgsqlDbType.Integer);
                var rpt_emp_id = cmd.Parameters.Add("@rpt_emp_id", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                rvw_sxn_id.Value = reviewSessionId;
                rpt_emp_id.Value = reportToId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    resultSummaryList.Add(new ResultSummary()
                    {
                        ReviewHeaderId = reader["rvw_hdr_id"] == DBNull.Value ? 0 : (int)reader["rvw_hdr_id"],
                        AppraiseeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
                        AppraiseeName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                        AppraiserId = reader["rvw_aprsr_id"] == DBNull.Value ? string.Empty : reader["rvw_aprsr_id"].ToString(),
                        AppraiserName = reader["rvw_aprsr_nm"] == DBNull.Value ? string.Empty : reader["rvw_aprsr_nm"].ToString(),
                        AppraiserRoleDescription = reader["aprsr_rl_ds"] == DBNull.Value ? string.Empty : reader["aprsr_rl_ds"].ToString(),
                        AppraiserTypeDescription = reader["apr_typ_ds"] == DBNull.Value ? string.Empty : reader["apr_typ_ds"].ToString(),
                        IsMain = reader["is_main"] == DBNull.Value ? false : (bool)reader["is_main"],
                        KpaScoreObtained = reader["kpa_scr_obt"] == DBNull.Value ? 0.00M : (decimal)reader["kpa_scr_obt"],
                        CompetencyScoreObtained = reader["cmp_scr_obt"] == DBNull.Value ? 0.00M : (decimal)reader["cmp_scr_obt"],
                        CombinedScoreObtained = reader["cmb_scr_obt"] == DBNull.Value ? 0.00M : (decimal)reader["cmb_scr_obt"],
                        PerformanceRating = reader["pfm_rating"] == DBNull.Value ? string.Empty : reader["pfm_rating"].ToString(),
                        UnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                        EmployeeNo = reader["emp_no_1"] == DBNull.Value ? string.Empty : reader["emp_no_1"].ToString(),
                        CurrentDesignation = reader["current_designation"] == DBNull.Value ? string.Empty : reader["current_designation"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return resultSummaryList;
        }

        public async Task<IList<ResultSummary>> GetSummaryByReportToIdAndAppraiseeId(string reportToId, int reviewSessionId, string appraiseeId)
        {
            List<ResultSummary> resultSummaryList = new List<ResultSummary>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT r.emp_id, e.emp_no_1, e.current_designation, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = r.emp_id) AS fullname, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = s.rvw_aprsr_id) AS rvw_aprsr_nm, ");
            sb.Append("u.unitname, s.kpa_scr_obt, s.cmp_scr_obt, s.cmb_scr_obt, ");
            sb.Append("s.rvw_hdr_id, s.rvw_aprsr_id, s.apr_typ_ds, s.aprsr_rl_ds, ");
            sb.Append("s.pfm_rating, s.is_main FROM public.erm_emp_rpts r ");
            sb.Append("INNER JOIN public.erm_emp_inf e ON e.emp_id = r.emp_id ");
            sb.Append("INNER JOIN public.gst_units u ON u.unitqk = e.unit_id ");
            sb.Append("LEFT JOIN public.pmsrvwsmry s ON s.rvw_emp_id = r.emp_id ");
            sb.Append("LEFT JOIN public.erm_emp_inf f ON f.emp_id = s.rvw_aprsr_id ");
            sb.Append("WHERE (r.rpt_nds IS NULL OR r.rpt_nds > CURRENT_DATE) ");
            sb.Append("AND (r.rpt_emp_id = @rpt_emp_id AND s.rvw_sxn_id = @rvw_sxn_id ");
            sb.Append("AND r.emp_id = @emp_id) ");
            sb.Append("ORDER BY u.unitname, s.cmb_scr_obt DESC;");

            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_sxn_id = cmd.Parameters.Add("@rvw_sxn_id", NpgsqlDbType.Integer);
                var rpt_emp_id = cmd.Parameters.Add("@rpt_emp_id", NpgsqlDbType.Text);
                var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                rvw_sxn_id.Value = reviewSessionId;
                rpt_emp_id.Value = reportToId;
                emp_id.Value = appraiseeId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    resultSummaryList.Add(new ResultSummary()
                    {
                        ReviewHeaderId = reader["rvw_hdr_id"] == DBNull.Value ? 0 : (int)reader["rvw_hdr_id"],
                        AppraiseeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
                        AppraiseeName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                        AppraiserId = reader["rvw_aprsr_id"] == DBNull.Value ? string.Empty : reader["rvw_aprsr_id"].ToString(),
                        AppraiserName = reader["rvw_aprsr_nm"] == DBNull.Value ? string.Empty : reader["rvw_aprsr_nm"].ToString(),
                        AppraiserRoleDescription = reader["aprsr_rl_ds"] == DBNull.Value ? string.Empty : reader["aprsr_rl_ds"].ToString(),
                        AppraiserTypeDescription = reader["apr_typ_ds"] == DBNull.Value ? string.Empty : reader["apr_typ_ds"].ToString(),
                        IsMain = reader["is_main"] == DBNull.Value ? false : (bool)reader["is_main"],
                        KpaScoreObtained = reader["kpa_scr_obt"] == DBNull.Value ? 0.00M : (decimal)reader["kpa_scr_obt"],
                        CompetencyScoreObtained = reader["cmp_scr_obt"] == DBNull.Value ? 0.00M : (decimal)reader["cmp_scr_obt"],
                        CombinedScoreObtained = reader["cmb_scr_obt"] == DBNull.Value ? 0.00M : (decimal)reader["cmb_scr_obt"],
                        PerformanceRating = reader["pfm_rating"] == DBNull.Value ? string.Empty : reader["pfm_rating"].ToString(),
                        UnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                        EmployeeNo = reader["emp_no_1"] == DBNull.Value ? string.Empty : reader["emp_no_1"].ToString(),
                        CurrentDesignation = reader["current_designation"] == DBNull.Value ? string.Empty : reader["current_designation"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return resultSummaryList;
        }

        #endregion

        #region Result Summary Write Action Methods
        public async Task<bool> AddSummaryAsync(ResultSummary resultSummary)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.pmsrvwsmry(rvw_hdr_id, ");
            sb.Append("rvw_sxn_id, rvw_emp_id, rvw_aprsr_id, ");
            sb.Append("kpa_scr_obt, cmp_scr_obt, cmb_scr_obt, ");
            sb.Append("scr_rank, scr_rank_ds, pfm_rating, ");
            sb.Append("apr_typ_ds, aprsr_rl_ds, rvw_yr_id, is_main)  ");
            sb.Append("VALUES (@rvw_hdr_id, @rvw_sxn_id, @rvw_emp_id, ");
            sb.Append("@rvw_aprsr_id, @kpa_scr_obt, @cmp_scr_obt, ");
            sb.Append("@cmb_scr_obt, @scr_rank, @scr_rank_ds, @pfm_rating, ");
            sb.Append("@apr_typ_ds, @aprsr_rl_ds, @rvw_yr_id, @is_main); ");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var rvw_sxn_id = cmd.Parameters.Add("@rvw_sxn_id", NpgsqlDbType.Integer);
                    var rvw_hdr_id = cmd.Parameters.Add("@rvw_hdr_id", NpgsqlDbType.Integer);
                    var rvw_yr_id = cmd.Parameters.Add("@rvw_yr_id", NpgsqlDbType.Integer);
                    var rvw_emp_id = cmd.Parameters.Add("@rvw_emp_id", NpgsqlDbType.Text);
                    var rvw_aprsr_id = cmd.Parameters.Add("@rvw_aprsr_id", NpgsqlDbType.Text);
                    var kpa_scr_obt = cmd.Parameters.Add("@kpa_scr_obt", NpgsqlDbType.Numeric);
                    var cmp_scr_obt = cmd.Parameters.Add("@cmp_scr_obt", NpgsqlDbType.Numeric);
                    var cmb_scr_obt = cmd.Parameters.Add("@cmb_scr_obt", NpgsqlDbType.Numeric);
                    var scr_rank = cmd.Parameters.Add("@scr_rank", NpgsqlDbType.Integer);
                    var scr_rank_ds = cmd.Parameters.Add("@scr_rank_ds", NpgsqlDbType.Text);
                    var pfm_rating = cmd.Parameters.Add("@pfm_rating", NpgsqlDbType.Text);
                    var apr_typ_ds = cmd.Parameters.Add("@apr_typ_ds", NpgsqlDbType.Text);
                    var aprsr_rl_ds = cmd.Parameters.Add("@aprsr_rl_ds", NpgsqlDbType.Text);
                    var is_main = cmd.Parameters.Add("@is_main", NpgsqlDbType.Boolean);
                    cmd.Prepare();
                    rvw_sxn_id.Value = resultSummary.ReviewSessionId;
                    rvw_emp_id.Value = resultSummary.AppraiseeId;
                    rvw_hdr_id.Value = resultSummary.ReviewHeaderId;
                    rvw_yr_id.Value = resultSummary.ReviewYearId;
                    rvw_aprsr_id.Value = resultSummary.AppraiserId;
                    aprsr_rl_ds.Value = resultSummary.AppraiserRoleDescription;
                    apr_typ_ds.Value = resultSummary.AppraiserTypeDescription;
                    kpa_scr_obt.Value = resultSummary.KpaScoreObtained;
                    cmp_scr_obt.Value = resultSummary.CompetencyScoreObtained;
                    cmb_scr_obt.Value = resultSummary.CombinedScoreObtained;
                    scr_rank.Value = resultSummary.ScoreRank;
                    scr_rank_ds.Value = resultSummary.ScoreRankDescription;
                    pfm_rating.Value = resultSummary.PerformanceRating;
                    is_main.Value = resultSummary.IsMain;

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

        public async Task<bool> UpdateSummaryAsync(ResultSummary resultSummary)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.pmsrvwsmry SET kpa_scr_obt=@kpa_scr_obt, ");
            sb.Append("cmp_scr_obt=@cmp_scr_obt, cmb_scr_obt=@cmb_scr_obt, ");
            sb.Append("scr_rank=@scr_rank, scr_rank_ds=@scr_rank_ds, ");
            sb.Append("pfm_rating=@pfm_rating, apr_typ_ds=@apr_typ_ds, ");
            sb.Append("aprsr_rl_ds=@aprsr_rl_ds, is_main=@is_main ");
            sb.Append("WHERE (rvw_smr_id=@rvw_smr_id);");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Update data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var rvw_smr_id = cmd.Parameters.Add("@rvw_smr_id", NpgsqlDbType.Integer);
                    var kpa_scr_obt = cmd.Parameters.Add("@kpa_scr_obt", NpgsqlDbType.Numeric);
                    var cmp_scr_obt = cmd.Parameters.Add("@cmp_scr_obt", NpgsqlDbType.Numeric);
                    var cmb_scr_obt = cmd.Parameters.Add("@cmb_scr_obt", NpgsqlDbType.Numeric);
                    var scr_rank = cmd.Parameters.Add("@scr_rank", NpgsqlDbType.Integer);
                    var scr_rank_ds = cmd.Parameters.Add("@scr_rank_ds", NpgsqlDbType.Text);
                    var pfm_rating = cmd.Parameters.Add("@pfm_rating", NpgsqlDbType.Text);
                    var apr_typ_ds = cmd.Parameters.Add("@apr_typ_ds", NpgsqlDbType.Text);
                    var aprsr_rl_ds = cmd.Parameters.Add("@aprsr_rl_ds", NpgsqlDbType.Text);
                    var is_main = cmd.Parameters.Add("@is_main", NpgsqlDbType.Boolean);
                    cmd.Prepare();
                    rvw_smr_id.Value = resultSummary.ResultSummaryId;
                    aprsr_rl_ds.Value = resultSummary.AppraiserRoleDescription;
                    apr_typ_ds.Value = resultSummary.AppraiserTypeDescription;
                    kpa_scr_obt.Value = resultSummary.KpaScoreObtained;
                    cmp_scr_obt.Value = resultSummary.CompetencyScoreObtained;
                    cmb_scr_obt.Value = resultSummary.CombinedScoreObtained;
                    scr_rank.Value = resultSummary.ScoreRank;
                    scr_rank_ds.Value = resultSummary.ScoreRankDescription;
                    pfm_rating.Value = resultSummary.PerformanceRating;
                    is_main.Value = resultSummary.IsMain;

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

        public async Task<bool> DeleteSummaryAsync(int reviewHeaderId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("DELETE FROM public.pmsrvwsmry ");
            sb.Append("WHERE (rvw_hdr_id=@rvw_hdr_id);");

            string query = sb.ToString();

            await conn.OpenAsync();
            //Update data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_hdr_id = cmd.Parameters.Add("@rvw_hdr_id", NpgsqlDbType.Integer);
                cmd.Prepare();
                rvw_hdr_id.Value = reviewHeaderId;

                rows = await cmd.ExecuteNonQueryAsync();
            }

            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<bool> DeleteSummaryExceptSelfEvaluationAsync(int reviewHeaderId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("DELETE FROM public.pmsrvwsmry ");
            sb.Append("WHERE (rvw_hdr_id=@rvw_hdr_id) ");
            sb.Append("AND (aprsr_rl_ds != 'Appraisee');");

            string query = sb.ToString();

            await conn.OpenAsync();
            //Update data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_hdr_id = cmd.Parameters.Add("@rvw_hdr_id", NpgsqlDbType.Integer);
                cmd.Prepare();
                rvw_hdr_id.Value = reviewHeaderId;

                rows = await cmd.ExecuteNonQueryAsync();
            }

            await conn.CloseAsync();
            return rows > 0;
        }


        #endregion

        #region Result Details Read

        public async Task<IList<ResultDetail>> GetPrincipalResultDetailByReviewSessionIdAsync(int reviewSessionId)
        {
            List<ResultDetail> resultDetailList = new List<ResultDetail>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT r.rvw_smr_id, r.rvw_hdr_id, r.rvw_sxn_id, r.rvw_emp_id, ");
            sb.Append("r.rvw_aprsr_id, r.kpa_scr_obt, r.cmp_scr_obt, r.cmb_scr_obt, ");
            sb.Append("r.scr_rank, r.scr_rank_ds, r.pfm_rating, r.apr_typ_ds, ");
            sb.Append("r.aprsr_rl_ds, r.rvw_yr_id, s.rvw_sxn_nm,  r.is_main, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = r.rvw_emp_id) as rvw_emp_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = r.rvw_aprsr_id) as rvw_aprsr_nm, ");
            sb.Append("y.pms_yr_nm, e.unit_id, e.dept_id, e.loc_id, e.emp_no_1, ");
            sb.Append("e.current_designation, d.deptname, u.unitname, l.locname, ");
            sb.Append("f.current_designation as rvw_aprsr_dsg, ");
            sb.Append("s.ttl_cmp_scr, s.ttl_kpa_scr, s.ttl_cmb_scr, ");
            sb.Append("h.fbk_probs, h.fbk_solns, h.lm_rmk, h.uh_rmk, h.dh_rmk, ");
            sb.Append("h.hr_rmk, h.mgt_rmk, h.is_flg, h.rvw_gls, h.lm_nm, ");
            sb.Append("h.uh_nm, h.dh_nm, h.hr_nm, h.mgt_nm, h.lm_rec, ");
            sb.Append("h.uh_rec, h.dh_rec, h.hr_rec, h.mgt_dec ");
            sb.Append("FROM public.pmsrvwsmry r ");
            sb.Append("INNER JOIN public.pmsrvwhdrs h ON (h.rvw_hdr_id = r.rvw_hdr_id ");
            sb.Append("AND h.pry_apr_id = r.rvw_aprsr_id) ");
            sb.Append("INNER JOIN public.pmsrvwsxns s ON s.rvw_sxn_id = r.rvw_sxn_id ");
            sb.Append("INNER JOIN public.pmssttyrs y ON y.pms_yr_id = r.rvw_yr_id ");
            sb.Append("INNER JOIN public.erm_emp_inf e ON e.emp_id = r.rvw_emp_id ");
            sb.Append("INNER JOIN public.erm_emp_inf f ON f.emp_id = r.rvw_aprsr_id ");
            sb.Append("INNER JOIN public.gst_depts d ON d.deptqk = e.dept_id ");
            sb.Append("INNER JOIN public.gst_units u ON u.unitqk = e.unit_id ");
            sb.Append("INNER JOIN public.gst_locs l ON l.locqk = e.loc_id ");
            sb.Append("WHERE (s.rvw_sxn_id = @rvw_sxn_id) ");
            sb.Append("ORDER BY r.rvw_smr_id;");

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
                    resultDetailList.Add(new ResultDetail()
                    {
                        ResultSummaryId = reader["rvw_smr_id"] == DBNull.Value ? 0 : (int)reader["rvw_smr_id"],
                        ReviewHeaderId = reader["rvw_hdr_id"] == DBNull.Value ? 0 : (int)reader["rvw_hdr_id"],
                        ReviewSessionId = reader["rvw_sxn_id"] == DBNull.Value ? 0 : (int)reader["rvw_sxn_id"],
                        ReviewSessionName = reader["rvw_sxn_nm"] == DBNull.Value ? string.Empty : reader["rvw_sxn_nm"].ToString(),

                        AppraiseeId = reader["rvw_emp_id"] == DBNull.Value ? string.Empty : reader["rvw_emp_id"].ToString(),
                        AppraiseeName = reader["rvw_emp_nm"] == DBNull.Value ? string.Empty : reader["rvw_emp_nm"].ToString(),

                        ReviewYearId = reader["rvw_yr_id"] == DBNull.Value ? 0 : (int)reader["rvw_yr_id"],
                        ReviewYearName = reader["pms_yr_nm"] == DBNull.Value ? string.Empty : reader["pms_yr_nm"].ToString(),

                        AppraiserId = reader["rvw_aprsr_id"] == DBNull.Value ? string.Empty : reader["rvw_aprsr_id"].ToString(),
                        AppraiserName = reader["rvw_aprsr_nm"] == DBNull.Value ? string.Empty : reader["rvw_aprsr_nm"].ToString(),
                        AppraiserDesignation = reader["rvw_aprsr_dsg"] == DBNull.Value ? string.Empty : reader["rvw_aprsr_dsg"].ToString(),


                        KpaScoreTotal = reader["ttl_kpa_scr"] == DBNull.Value ? 0.00M : (decimal)reader["ttl_kpa_scr"],
                        KpaScoreObtained = reader["kpa_scr_obt"] == DBNull.Value ? 0.00M : (decimal)reader["kpa_scr_obt"],
                        CompetencyScoreTotal = reader["ttl_cmp_scr"] == DBNull.Value ? 0.00M : (decimal)reader["ttl_cmp_scr"],
                        CompetencyScoreObtained = reader["cmp_scr_obt"] == DBNull.Value ? 0.00M : (decimal)reader["cmp_scr_obt"],
                        CombinedScoreTotal = reader["ttl_cmb_scr"] == DBNull.Value ? 0.00M : (decimal)reader["ttl_cmb_scr"],
                        CombinedScoreObtained = reader["cmb_scr_obt"] == DBNull.Value ? 0.00M : (decimal)reader["cmb_scr_obt"],

                        ScoreRank = reader["scr_rank"] == DBNull.Value ? 0 : (int)reader["scr_rank"],
                        ScoreRankDescription = reader["scr_rank_ds"] == DBNull.Value ? string.Empty : reader["scr_rank_ds"].ToString(),
                        PerformanceRating = reader["pfm_rating"] == DBNull.Value ? string.Empty : reader["pfm_rating"].ToString(),

                        AppraiserTypeDescription = reader["apr_typ_ds"] == DBNull.Value ? string.Empty : reader["apr_typ_ds"].ToString(),
                        AppraiserRoleDescription = reader["aprsr_rl_ds"] == DBNull.Value ? string.Empty : reader["aprsr_rl_ds"].ToString(),

                        UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                        UnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),

                        DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                        DepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),

                        LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                        LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),

                        EmployeeNo = reader["emp_no_1"] == DBNull.Value ? string.Empty : reader["emp_no_1"].ToString(),
                        IsMain = reader["is_main"] == DBNull.Value ? false : (bool)reader["is_main"],

                        AppraiseeDesignation = reader["current_designation"] == DBNull.Value ? string.Empty : reader["current_designation"].ToString(),
                        DepartmentHeadComments = reader["dh_rmk"] == DBNull.Value ? string.Empty : reader["dh_rmk"].ToString(),
                        DepartmentHeadName = reader["dh_nm"] == DBNull.Value ? string.Empty : reader["dh_nm"].ToString(),
                        DepartmentHeadRecommendation = reader["dh_rec"] == DBNull.Value ? string.Empty : reader["dh_rec"].ToString(),
                        FeedbackProblems = reader["fbk_probs"] == DBNull.Value ? string.Empty : reader["fbk_probs"].ToString(),
                        FeedbackSolutions = reader["fbk_solns"] == DBNull.Value ? string.Empty : reader["fbk_solns"].ToString(),
                        HrComments = reader["hr_rmk"] == DBNull.Value ? string.Empty : reader["hr_rmk"].ToString(),
                        HrName = reader["hr_nm"] == DBNull.Value ? string.Empty : reader["hr_nm"].ToString(),
                        HrRecommendation = reader["hr_rec"] == DBNull.Value ? string.Empty : reader["hr_rec"].ToString(),
                        IsFlagged = reader["is_flg"] == DBNull.Value ? false : (bool)reader["is_flg"],
                        LineManagerComments = reader["lm_rmk"] == DBNull.Value ? string.Empty : reader["lm_rmk"].ToString(),
                        LineManagerName = reader["lm_nm"] == DBNull.Value ? string.Empty : reader["lm_nm"].ToString(),
                        LineManagerRecommendation = reader["lm_rec"] == DBNull.Value ? string.Empty : reader["lm_rec"].ToString(),
                        ManagementComments = reader["mgt_rmk"] == DBNull.Value ? string.Empty : reader["mgt_rmk"].ToString(),
                        ManagementDecision = reader["mgt_dec"] == DBNull.Value ? string.Empty : reader["mgt_dec"].ToString(),
                        ManagementName = reader["mgt_nm"] == DBNull.Value ? string.Empty : reader["mgt_nm"].ToString(),
                        PerformanceGoal = reader["rvw_gls"] == DBNull.Value ? string.Empty : reader["rvw_gls"].ToString(),
                        UnitHeadComments = reader["uh_rmk"] == DBNull.Value ? string.Empty : reader["uh_rmk"].ToString(),
                        UnitHeadName = reader["uh_nm"] == DBNull.Value ? string.Empty : reader["uh_nm"].ToString(),
                        UnitHeadRecommendation = reader["uh_rec"] == DBNull.Value ? string.Empty : reader["uh_rec"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return resultDetailList;
        }

        public async Task<IList<ResultDetail>> GetPrincipalResultDetailByLocationIdAndReviewSessionIdAsync(int reviewSessionId, int locationId)
        {
            List<ResultDetail> resultDetailList = new List<ResultDetail>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT r.rvw_smr_id, r.rvw_hdr_id, r.rvw_sxn_id, r.rvw_emp_id, ");
            sb.Append("r.rvw_aprsr_id, r.kpa_scr_obt, r.cmp_scr_obt, r.cmb_scr_obt, ");
            sb.Append("r.scr_rank, r.scr_rank_ds, r.pfm_rating, r.apr_typ_ds, ");
            sb.Append("r.aprsr_rl_ds, r.rvw_yr_id, s.rvw_sxn_nm,  r.is_main, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = r.rvw_emp_id) as rvw_emp_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = r.rvw_aprsr_id) as rvw_aprsr_nm, ");
            sb.Append("y.pms_yr_nm, e.unit_id, e.dept_id, e.loc_id, e.emp_no_1, ");
            sb.Append("e.current_designation, d.deptname, u.unitname, l.locname, ");
            sb.Append("f.current_designation as rvw_aprsr_dsg, ");
            sb.Append("s.ttl_cmp_scr, s.ttl_kpa_scr, s.ttl_cmb_scr, ");
            sb.Append("h.fbk_probs, h.fbk_solns, h.lm_rmk, h.uh_rmk, h.dh_rmk, ");
            sb.Append("h.hr_rmk, h.mgt_rmk, h.is_flg, h.rvw_gls, h.lm_nm, ");
            sb.Append("h.uh_nm, h.dh_nm, h.hr_nm, h.mgt_nm, h.lm_rec, ");
            sb.Append("h.uh_rec, h.dh_rec, h.hr_rec, h.mgt_dec ");
            sb.Append("FROM public.pmsrvwsmry r ");
            sb.Append("INNER JOIN public.pmsrvwhdrs h ON (h.rvw_hdr_id = r.rvw_hdr_id ");
            sb.Append("AND h.pry_apr_id = r.rvw_aprsr_id) ");
            sb.Append("INNER JOIN public.pmsrvwsxns s ON s.rvw_sxn_id = r.rvw_sxn_id ");
            sb.Append("INNER JOIN public.pmssttyrs y ON y.pms_yr_id = r.rvw_yr_id ");
            sb.Append("INNER JOIN public.erm_emp_inf e ON e.emp_id = r.rvw_emp_id ");
            sb.Append("INNER JOIN public.erm_emp_inf f ON f.emp_id = r.rvw_aprsr_id ");
            sb.Append("INNER JOIN public.gst_depts d ON d.deptqk = e.dept_id ");
            sb.Append("INNER JOIN public.gst_units u ON u.unitqk = e.unit_id ");
            sb.Append("INNER JOIN public.gst_locs l ON l.locqk = e.loc_id ");
            sb.Append("WHERE (s.rvw_sxn_id = @rvw_sxn_id) ");
            sb.Append("AND (e.loc_id = @loc_id) ");
            sb.Append("ORDER BY r.rvw_smr_id;");

            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_sxn_id = cmd.Parameters.Add("@rvw_sxn_id", NpgsqlDbType.Integer);
                var loc_id = cmd.Parameters.Add("@loc_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                rvw_sxn_id.Value = reviewSessionId;
                loc_id.Value = locationId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    resultDetailList.Add(new ResultDetail()
                    {
                        ResultSummaryId = reader["rvw_smr_id"] == DBNull.Value ? 0 : (int)reader["rvw_smr_id"],
                        ReviewHeaderId = reader["rvw_hdr_id"] == DBNull.Value ? 0 : (int)reader["rvw_hdr_id"],
                        ReviewSessionId = reader["rvw_sxn_id"] == DBNull.Value ? 0 : (int)reader["rvw_sxn_id"],
                        ReviewSessionName = reader["rvw_sxn_nm"] == DBNull.Value ? string.Empty : reader["rvw_sxn_nm"].ToString(),

                        AppraiseeId = reader["rvw_emp_id"] == DBNull.Value ? string.Empty : reader["rvw_emp_id"].ToString(),
                        AppraiseeName = reader["rvw_emp_nm"] == DBNull.Value ? string.Empty : reader["rvw_emp_nm"].ToString(),

                        ReviewYearId = reader["rvw_yr_id"] == DBNull.Value ? 0 : (int)reader["rvw_yr_id"],
                        ReviewYearName = reader["pms_yr_nm"] == DBNull.Value ? string.Empty : reader["pms_yr_nm"].ToString(),

                        AppraiserId = reader["rvw_aprsr_id"] == DBNull.Value ? string.Empty : reader["rvw_aprsr_id"].ToString(),
                        AppraiserName = reader["rvw_aprsr_nm"] == DBNull.Value ? string.Empty : reader["rvw_aprsr_nm"].ToString(),
                        AppraiserDesignation = reader["rvw_aprsr_dsg"] == DBNull.Value ? string.Empty : reader["rvw_aprsr_dsg"].ToString(),

                        KpaScoreTotal = reader["ttl_kpa_scr"] == DBNull.Value ? 0.00M : (decimal)reader["ttl_kpa_scr"],
                        KpaScoreObtained = reader["kpa_scr_obt"] == DBNull.Value ? 0.00M : (decimal)reader["kpa_scr_obt"],
                        CompetencyScoreTotal = reader["ttl_cmp_scr"] == DBNull.Value ? 0.00M : (decimal)reader["ttl_cmp_scr"],
                        CompetencyScoreObtained = reader["cmp_scr_obt"] == DBNull.Value ? 0.00M : (decimal)reader["cmp_scr_obt"],
                        CombinedScoreTotal = reader["ttl_cmb_scr"] == DBNull.Value ? 0.00M : (decimal)reader["ttl_cmb_scr"],
                        CombinedScoreObtained = reader["cmb_scr_obt"] == DBNull.Value ? 0.00M : (decimal)reader["cmb_scr_obt"],

                        ScoreRank = reader["scr_rank"] == DBNull.Value ? 0 : (int)reader["scr_rank"],
                        ScoreRankDescription = reader["scr_rank_ds"] == DBNull.Value ? string.Empty : reader["scr_rank_ds"].ToString(),
                        PerformanceRating = reader["pfm_rating"] == DBNull.Value ? string.Empty : reader["pfm_rating"].ToString(),

                        AppraiserTypeDescription = reader["apr_typ_ds"] == DBNull.Value ? string.Empty : reader["apr_typ_ds"].ToString(),
                        AppraiserRoleDescription = reader["aprsr_rl_ds"] == DBNull.Value ? string.Empty : reader["aprsr_rl_ds"].ToString(),

                        UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                        UnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),

                        DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                        DepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),

                        LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                        LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),

                        EmployeeNo = reader["emp_no_1"] == DBNull.Value ? string.Empty : reader["emp_no_1"].ToString(),
                        IsMain = reader["is_main"] == DBNull.Value ? false : (bool)reader["is_main"],

                        AppraiseeDesignation = reader["current_designation"] == DBNull.Value ? string.Empty : reader["current_designation"].ToString(),
                        DepartmentHeadComments = reader["dh_rmk"] == DBNull.Value ? string.Empty : reader["dh_rmk"].ToString(),
                        DepartmentHeadName = reader["dh_nm"] == DBNull.Value ? string.Empty : reader["dh_nm"].ToString(),
                        DepartmentHeadRecommendation = reader["dh_rec"] == DBNull.Value ? string.Empty : reader["dh_rec"].ToString(),
                        FeedbackProblems = reader["fbk_probs"] == DBNull.Value ? string.Empty : reader["fbk_probs"].ToString(),
                        FeedbackSolutions = reader["fbk_solns"] == DBNull.Value ? string.Empty : reader["fbk_solns"].ToString(),
                        HrComments = reader["hr_rmk"] == DBNull.Value ? string.Empty : reader["hr_rmk"].ToString(),
                        HrName = reader["hr_nm"] == DBNull.Value ? string.Empty : reader["hr_nm"].ToString(),
                        HrRecommendation = reader["hr_rec"] == DBNull.Value ? string.Empty : reader["hr_rec"].ToString(),
                        IsFlagged = reader["is_flg"] == DBNull.Value ? false : (bool)reader["is_flg"],
                        LineManagerComments = reader["lm_rmk"] == DBNull.Value ? string.Empty : reader["lm_rmk"].ToString(),
                        LineManagerName = reader["lm_nm"] == DBNull.Value ? string.Empty : reader["lm_nm"].ToString(),
                        LineManagerRecommendation = reader["lm_rec"] == DBNull.Value ? string.Empty : reader["lm_rec"].ToString(),
                        ManagementComments = reader["mgt_rmk"] == DBNull.Value ? string.Empty : reader["mgt_rmk"].ToString(),
                        ManagementDecision = reader["mgt_dec"] == DBNull.Value ? string.Empty : reader["mgt_dec"].ToString(),
                        ManagementName = reader["mgt_nm"] == DBNull.Value ? string.Empty : reader["mgt_nm"].ToString(),
                        PerformanceGoal = reader["rvw_gls"] == DBNull.Value ? string.Empty : reader["rvw_gls"].ToString(),
                        UnitHeadComments = reader["uh_rmk"] == DBNull.Value ? string.Empty : reader["uh_rmk"].ToString(),
                        UnitHeadName = reader["uh_nm"] == DBNull.Value ? string.Empty : reader["uh_nm"].ToString(),
                        UnitHeadRecommendation = reader["uh_rec"] == DBNull.Value ? string.Empty : reader["uh_rec"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return resultDetailList;
        }

        public async Task<IList<ResultDetail>> GetPrincipalResultDetailByDepartmentCodeAndReviewSessionIdAsync(int reviewSessionId, int departmentId)
        {
            List<ResultDetail> resultDetailList = new List<ResultDetail>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT r.rvw_smr_id, r.rvw_hdr_id, r.rvw_sxn_id, r.rvw_emp_id, ");
            sb.Append("r.rvw_aprsr_id, r.kpa_scr_obt, r.cmp_scr_obt, r.cmb_scr_obt, ");
            sb.Append("r.scr_rank, r.scr_rank_ds, r.pfm_rating, r.apr_typ_ds, ");
            sb.Append("r.aprsr_rl_ds, r.rvw_yr_id, s.rvw_sxn_nm,  r.is_main, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = r.rvw_emp_id) as rvw_emp_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = r.rvw_aprsr_id) as rvw_aprsr_nm, ");
            sb.Append("y.pms_yr_nm, e.unit_id, e.dept_id, e.loc_id, e.emp_no_1, ");
            sb.Append("e.current_designation, d.deptname, u.unitname, l.locname, ");
            sb.Append("f.current_designation as rvw_aprsr_dsg, ");
            sb.Append("s.ttl_cmp_scr, s.ttl_kpa_scr, s.ttl_cmb_scr, ");
            sb.Append("h.fbk_probs, h.fbk_solns, h.lm_rmk, h.uh_rmk, h.dh_rmk, ");
            sb.Append("h.hr_rmk, h.mgt_rmk, h.is_flg, h.rvw_gls, h.lm_nm, ");
            sb.Append("h.uh_nm, h.dh_nm, h.hr_nm, h.mgt_nm, h.lm_rec, ");
            sb.Append("h.uh_rec, h.dh_rec, h.hr_rec, h.mgt_dec ");
            sb.Append("FROM public.pmsrvwsmry r ");
            sb.Append("INNER JOIN public.pmsrvwhdrs h ON (h.rvw_hdr_id = r.rvw_hdr_id ");
            sb.Append("AND h.pry_apr_id = r.rvw_aprsr_id) ");
            sb.Append("INNER JOIN public.pmsrvwsxns s ON s.rvw_sxn_id = r.rvw_sxn_id ");
            sb.Append("INNER JOIN public.pmssttyrs y ON y.pms_yr_id = r.rvw_yr_id ");
            sb.Append("INNER JOIN public.erm_emp_inf e ON e.emp_id = r.rvw_emp_id ");
            sb.Append("INNER JOIN public.erm_emp_inf f ON f.emp_id = r.rvw_aprsr_id ");
            sb.Append("INNER JOIN public.gst_depts d ON d.deptqk = e.dept_id ");
            sb.Append("INNER JOIN public.gst_units u ON u.unitqk = e.unit_id ");
            sb.Append("INNER JOIN public.gst_locs l ON l.locqk = e.loc_id ");
            sb.Append("WHERE (s.rvw_sxn_id = @rvw_sxn_id) ");
            sb.Append("AND (e.dept_id = @dept_cd) ");
            sb.Append("ORDER BY r.rvw_smr_id;");

            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_sxn_id = cmd.Parameters.Add("@rvw_sxn_id", NpgsqlDbType.Integer);
                var dept_cd = cmd.Parameters.Add("@dept_cd", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                rvw_sxn_id.Value = reviewSessionId;
                dept_cd.Value = departmentId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    resultDetailList.Add(new ResultDetail()
                    {
                        ResultSummaryId = reader["rvw_smr_id"] == DBNull.Value ? 0 : (int)reader["rvw_smr_id"],
                        ReviewHeaderId = reader["rvw_hdr_id"] == DBNull.Value ? 0 : (int)reader["rvw_hdr_id"],
                        ReviewSessionId = reader["rvw_sxn_id"] == DBNull.Value ? 0 : (int)reader["rvw_sxn_id"],
                        ReviewSessionName = reader["rvw_sxn_nm"] == DBNull.Value ? string.Empty : reader["rvw_sxn_nm"].ToString(),

                        AppraiseeId = reader["rvw_emp_id"] == DBNull.Value ? string.Empty : reader["rvw_emp_id"].ToString(),
                        AppraiseeName = reader["rvw_emp_nm"] == DBNull.Value ? string.Empty : reader["rvw_emp_nm"].ToString(),

                        ReviewYearId = reader["rvw_yr_id"] == DBNull.Value ? 0 : (int)reader["rvw_yr_id"],
                        ReviewYearName = reader["pms_yr_nm"] == DBNull.Value ? string.Empty : reader["pms_yr_nm"].ToString(),

                        AppraiserId = reader["rvw_aprsr_id"] == DBNull.Value ? string.Empty : reader["rvw_aprsr_id"].ToString(),
                        AppraiserName = reader["rvw_aprsr_nm"] == DBNull.Value ? string.Empty : reader["rvw_aprsr_nm"].ToString(),
                        AppraiserDesignation = reader["rvw_aprsr_dsg"] == DBNull.Value ? string.Empty : reader["rvw_aprsr_dsg"].ToString(),

                        KpaScoreTotal = reader["ttl_kpa_scr"] == DBNull.Value ? 0.00M : (decimal)reader["ttl_kpa_scr"],
                        KpaScoreObtained = reader["kpa_scr_obt"] == DBNull.Value ? 0.00M : (decimal)reader["kpa_scr_obt"],
                        CompetencyScoreTotal = reader["ttl_cmp_scr"] == DBNull.Value ? 0.00M : (decimal)reader["ttl_cmp_scr"],
                        CompetencyScoreObtained = reader["cmp_scr_obt"] == DBNull.Value ? 0.00M : (decimal)reader["cmp_scr_obt"],
                        CombinedScoreTotal = reader["ttl_cmb_scr"] == DBNull.Value ? 0.00M : (decimal)reader["ttl_cmb_scr"],
                        CombinedScoreObtained = reader["cmb_scr_obt"] == DBNull.Value ? 0.00M : (decimal)reader["cmb_scr_obt"],

                        ScoreRank = reader["scr_rank"] == DBNull.Value ? 0 : (int)reader["scr_rank"],
                        ScoreRankDescription = reader["scr_rank_ds"] == DBNull.Value ? string.Empty : reader["scr_rank_ds"].ToString(),
                        PerformanceRating = reader["pfm_rating"] == DBNull.Value ? string.Empty : reader["pfm_rating"].ToString(),

                        AppraiserTypeDescription = reader["apr_typ_ds"] == DBNull.Value ? string.Empty : reader["apr_typ_ds"].ToString(),
                        AppraiserRoleDescription = reader["aprsr_rl_ds"] == DBNull.Value ? string.Empty : reader["aprsr_rl_ds"].ToString(),

                        UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                        UnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),

                        DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                        DepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),

                        LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                        LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),

                        EmployeeNo = reader["emp_no_1"] == DBNull.Value ? string.Empty : reader["emp_no_1"].ToString(),
                        IsMain = reader["is_main"] == DBNull.Value ? false : (bool)reader["is_main"],

                        AppraiseeDesignation = reader["current_designation"] == DBNull.Value ? string.Empty : reader["current_designation"].ToString(),
                        DepartmentHeadComments = reader["dh_rmk"] == DBNull.Value ? string.Empty : reader["dh_rmk"].ToString(),
                        DepartmentHeadName = reader["dh_nm"] == DBNull.Value ? string.Empty : reader["dh_nm"].ToString(),
                        DepartmentHeadRecommendation = reader["dh_rec"] == DBNull.Value ? string.Empty : reader["dh_rec"].ToString(),
                        FeedbackProblems = reader["fbk_probs"] == DBNull.Value ? string.Empty : reader["fbk_probs"].ToString(),
                        FeedbackSolutions = reader["fbk_solns"] == DBNull.Value ? string.Empty : reader["fbk_solns"].ToString(),
                        HrComments = reader["hr_rmk"] == DBNull.Value ? string.Empty : reader["hr_rmk"].ToString(),
                        HrName = reader["hr_nm"] == DBNull.Value ? string.Empty : reader["hr_nm"].ToString(),
                        HrRecommendation = reader["hr_rec"] == DBNull.Value ? string.Empty : reader["hr_rec"].ToString(),
                        IsFlagged = reader["is_flg"] == DBNull.Value ? false : (bool)reader["is_flg"],
                        LineManagerComments = reader["lm_rmk"] == DBNull.Value ? string.Empty : reader["lm_rmk"].ToString(),
                        LineManagerName = reader["lm_nm"] == DBNull.Value ? string.Empty : reader["lm_nm"].ToString(),
                        LineManagerRecommendation = reader["lm_rec"] == DBNull.Value ? string.Empty : reader["lm_rec"].ToString(),
                        ManagementComments = reader["mgt_rmk"] == DBNull.Value ? string.Empty : reader["mgt_rmk"].ToString(),
                        ManagementDecision = reader["mgt_dec"] == DBNull.Value ? string.Empty : reader["mgt_dec"].ToString(),
                        ManagementName = reader["mgt_nm"] == DBNull.Value ? string.Empty : reader["mgt_nm"].ToString(),
                        PerformanceGoal = reader["rvw_gls"] == DBNull.Value ? string.Empty : reader["rvw_gls"].ToString(),
                        UnitHeadComments = reader["uh_rmk"] == DBNull.Value ? string.Empty : reader["uh_rmk"].ToString(),
                        UnitHeadName = reader["uh_nm"] == DBNull.Value ? string.Empty : reader["uh_nm"].ToString(),
                        UnitHeadRecommendation = reader["uh_rec"] == DBNull.Value ? string.Empty : reader["uh_rec"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return resultDetailList;
        }

        public async Task<IList<ResultDetail>> GetPrincipalResultDetailByUnitCodeAndReviewSessionIdAsync(int reviewSessionId, int unitId)
        {
            List<ResultDetail> resultDetailList = new List<ResultDetail>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT r.rvw_smr_id, r.rvw_hdr_id, r.rvw_sxn_id, r.rvw_emp_id, ");
            sb.Append("r.rvw_aprsr_id, r.kpa_scr_obt, r.cmp_scr_obt, r.cmb_scr_obt, ");
            sb.Append("r.scr_rank, r.scr_rank_ds, r.pfm_rating, r.apr_typ_ds, ");
            sb.Append("r.aprsr_rl_ds, r.rvw_yr_id, s.rvw_sxn_nm,  r.is_main, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = r.rvw_emp_id) as rvw_emp_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = r.rvw_aprsr_id) as rvw_aprsr_nm, ");

            sb.Append("y.pms_yr_nm, e.unit_id, e.dept_id, e.loc_id, e.emp_no_1, ");
            sb.Append("e.current_designation, d.deptname, u.unitname, l.locname, ");
            sb.Append("f.current_designation as rvw_aprsr_dsg, ");
            sb.Append("s.ttl_cmp_scr, s.ttl_kpa_scr, s.ttl_cmb_scr, ");
            sb.Append("h.fbk_probs, h.fbk_solns, h.lm_rmk, h.uh_rmk, h.dh_rmk, ");
            sb.Append("h.hr_rmk, h.mgt_rmk, h.is_flg, h.rvw_gls, h.lm_nm, ");
            sb.Append("h.uh_nm, h.dh_nm, h.hr_nm, h.mgt_nm, h.lm_rec, ");
            sb.Append("h.uh_rec, h.dh_rec, h.hr_rec, h.mgt_dec ");
            sb.Append("FROM public.pmsrvwsmry r ");
            sb.Append("INNER JOIN public.pmsrvwhdrs h ON (h.rvw_hdr_id = r.rvw_hdr_id ");
            sb.Append("AND h.pry_apr_id = r.rvw_aprsr_id) ");
            sb.Append("INNER JOIN public.pmsrvwsxns s ON s.rvw_sxn_id = r.rvw_sxn_id ");
            sb.Append("INNER JOIN public.pmssttyrs y ON y.pms_yr_id = r.rvw_yr_id ");
            sb.Append("INNER JOIN public.erm_emp_inf e ON e.emp_id = r.rvw_emp_id ");
            sb.Append("INNER JOIN public.erm_emp_inf f ON f.emp_id = r.rvw_aprsr_id ");
            sb.Append("INNER JOIN public.gst_depts d ON d.deptqk = e.dept_id ");
            sb.Append("INNER JOIN public.gst_units u ON u.unitqk = e.unit_id ");
            sb.Append("INNER JOIN public.gst_locs l ON l.locqk = e.loc_id ");
            sb.Append("WHERE (s.rvw_sxn_id = @rvw_sxn_id) ");
            sb.Append("AND (e.unit_id = @unit_cd) ");
            sb.Append("ORDER BY r.rvw_smr_id;");

            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_sxn_id = cmd.Parameters.Add("@rvw_sxn_id", NpgsqlDbType.Integer);
                var unit_cd = cmd.Parameters.Add("@unit_cd", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                rvw_sxn_id.Value = reviewSessionId;
                unit_cd.Value = unitId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    resultDetailList.Add(new ResultDetail()
                    {
                        ResultSummaryId = reader["rvw_smr_id"] == DBNull.Value ? 0 : (int)reader["rvw_smr_id"],
                        ReviewHeaderId = reader["rvw_hdr_id"] == DBNull.Value ? 0 : (int)reader["rvw_hdr_id"],
                        ReviewSessionId = reader["rvw_sxn_id"] == DBNull.Value ? 0 : (int)reader["rvw_sxn_id"],
                        ReviewSessionName = reader["rvw_sxn_nm"] == DBNull.Value ? string.Empty : reader["rvw_sxn_nm"].ToString(),

                        AppraiseeId = reader["rvw_emp_id"] == DBNull.Value ? string.Empty : reader["rvw_emp_id"].ToString(),
                        AppraiseeName = reader["rvw_emp_nm"] == DBNull.Value ? string.Empty : reader["rvw_emp_nm"].ToString(),

                        ReviewYearId = reader["rvw_yr_id"] == DBNull.Value ? 0 : (int)reader["rvw_yr_id"],
                        ReviewYearName = reader["pms_yr_nm"] == DBNull.Value ? string.Empty : reader["pms_yr_nm"].ToString(),

                        AppraiserId = reader["rvw_aprsr_id"] == DBNull.Value ? string.Empty : reader["rvw_aprsr_id"].ToString(),
                        AppraiserName = reader["rvw_aprsr_nm"] == DBNull.Value ? string.Empty : reader["rvw_aprsr_nm"].ToString(),
                        AppraiserDesignation = reader["rvw_aprsr_dsg"] == DBNull.Value ? string.Empty : reader["rvw_aprsr_dsg"].ToString(),

                        KpaScoreTotal = reader["ttl_kpa_scr"] == DBNull.Value ? 0.00M : (decimal)reader["ttl_kpa_scr"],
                        KpaScoreObtained = reader["kpa_scr_obt"] == DBNull.Value ? 0.00M : (decimal)reader["kpa_scr_obt"],
                        CompetencyScoreTotal = reader["ttl_cmp_scr"] == DBNull.Value ? 0.00M : (decimal)reader["ttl_cmp_scr"],
                        CompetencyScoreObtained = reader["cmp_scr_obt"] == DBNull.Value ? 0.00M : (decimal)reader["cmp_scr_obt"],
                        CombinedScoreTotal = reader["ttl_cmb_scr"] == DBNull.Value ? 0.00M : (decimal)reader["ttl_cmb_scr"],
                        CombinedScoreObtained = reader["cmb_scr_obt"] == DBNull.Value ? 0.00M : (decimal)reader["cmb_scr_obt"],

                        ScoreRank = reader["scr_rank"] == DBNull.Value ? 0 : (int)reader["scr_rank"],
                        ScoreRankDescription = reader["scr_rank_ds"] == DBNull.Value ? string.Empty : reader["scr_rank_ds"].ToString(),
                        PerformanceRating = reader["pfm_rating"] == DBNull.Value ? string.Empty : reader["pfm_rating"].ToString(),

                        AppraiserTypeDescription = reader["apr_typ_ds"] == DBNull.Value ? string.Empty : reader["apr_typ_ds"].ToString(),
                        AppraiserRoleDescription = reader["aprsr_rl_ds"] == DBNull.Value ? string.Empty : reader["aprsr_rl_ds"].ToString(),

                        UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                        UnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),

                        DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                        DepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),

                        LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                        LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),

                        EmployeeNo = reader["emp_no_1"] == DBNull.Value ? string.Empty : reader["emp_no_1"].ToString(),
                        IsMain = reader["is_main"] == DBNull.Value ? false : (bool)reader["is_main"],

                        AppraiseeDesignation = reader["current_designation"] == DBNull.Value ? string.Empty : reader["current_designation"].ToString(),
                        DepartmentHeadComments = reader["dh_rmk"] == DBNull.Value ? string.Empty : reader["dh_rmk"].ToString(),
                        DepartmentHeadName = reader["dh_nm"] == DBNull.Value ? string.Empty : reader["dh_nm"].ToString(),
                        DepartmentHeadRecommendation = reader["dh_rec"] == DBNull.Value ? string.Empty : reader["dh_rec"].ToString(),
                        FeedbackProblems = reader["fbk_probs"] == DBNull.Value ? string.Empty : reader["fbk_probs"].ToString(),
                        FeedbackSolutions = reader["fbk_solns"] == DBNull.Value ? string.Empty : reader["fbk_solns"].ToString(),
                        HrComments = reader["hr_rmk"] == DBNull.Value ? string.Empty : reader["hr_rmk"].ToString(),
                        HrName = reader["hr_nm"] == DBNull.Value ? string.Empty : reader["hr_nm"].ToString(),
                        HrRecommendation = reader["hr_rec"] == DBNull.Value ? string.Empty : reader["hr_rec"].ToString(),
                        IsFlagged = reader["is_flg"] == DBNull.Value ? false : (bool)reader["is_flg"],
                        LineManagerComments = reader["lm_rmk"] == DBNull.Value ? string.Empty : reader["lm_rmk"].ToString(),
                        LineManagerName = reader["lm_nm"] == DBNull.Value ? string.Empty : reader["lm_nm"].ToString(),
                        LineManagerRecommendation = reader["lm_rec"] == DBNull.Value ? string.Empty : reader["lm_rec"].ToString(),
                        ManagementComments = reader["mgt_rmk"] == DBNull.Value ? string.Empty : reader["mgt_rmk"].ToString(),
                        ManagementDecision = reader["mgt_dec"] == DBNull.Value ? string.Empty : reader["mgt_dec"].ToString(),
                        ManagementName = reader["mgt_nm"] == DBNull.Value ? string.Empty : reader["mgt_nm"].ToString(),
                        PerformanceGoal = reader["rvw_gls"] == DBNull.Value ? string.Empty : reader["rvw_gls"].ToString(),
                        UnitHeadComments = reader["uh_rmk"] == DBNull.Value ? string.Empty : reader["uh_rmk"].ToString(),
                        UnitHeadName = reader["uh_nm"] == DBNull.Value ? string.Empty : reader["uh_nm"].ToString(),
                        UnitHeadRecommendation = reader["uh_rec"] == DBNull.Value ? string.Empty : reader["uh_rec"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return resultDetailList;
        }


        //===== Rejected Evaluation Results =====//
        public async Task<IList<ResultDetail>> GetRejectedPrincipalResultDetailByReviewSessionIdAsync(int reviewSessionId)
        {
            List<ResultDetail> resultDetailList = new List<ResultDetail>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT r.rvw_smr_id, r.rvw_hdr_id, r.rvw_sxn_id, r.rvw_emp_id, ");
            sb.Append("r.rvw_aprsr_id, r.kpa_scr_obt, r.cmp_scr_obt, r.cmb_scr_obt, ");
            sb.Append("r.scr_rank, r.scr_rank_ds, r.pfm_rating, r.apr_typ_ds, ");
            sb.Append("r.aprsr_rl_ds, r.rvw_yr_id, s.rvw_sxn_nm,  r.is_main, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = r.rvw_emp_id) as rvw_emp_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = r.rvw_aprsr_id) as rvw_aprsr_nm, ");
            sb.Append("y.pms_yr_nm, e.unit_id, e.dept_id, e.loc_id, e.emp_no_1, ");
            sb.Append("e.current_designation, d.deptname, u.unitname, l.locname, ");
            sb.Append("f.current_designation as rvw_aprsr_dsg, s.ttl_cmp_scr, ");
            sb.Append("s.ttl_kpa_scr, s.ttl_cmb_scr, h.is_flg, h.flg_rsn, h.flg_dt, ");
            sb.Append("h.fbk_probs, h.fbk_solns, h.lm_rmk, h.uh_rmk, h.dh_rmk, ");
            sb.Append("h.hr_rmk, h.mgt_rmk, h.rvw_gls, h.lm_nm, h.uh_nm, h.dh_nm, ");
            sb.Append("h.hr_nm, h.mgt_nm, h.lm_rec, h.uh_rec, h.dh_rec, h.hr_rec, ");
            sb.Append("h.mgt_dec FROM public.pmsrvwsmry r ");
            sb.Append("INNER JOIN public.pmsrvwhdrs h ON (h.rvw_hdr_id = r.rvw_hdr_id ");
            sb.Append("AND h.pry_apr_id = r.rvw_aprsr_id) ");
            sb.Append("INNER JOIN public.pmsrvwsxns s ON s.rvw_sxn_id = r.rvw_sxn_id ");
            sb.Append("INNER JOIN public.pmssttyrs y ON y.pms_yr_id = r.rvw_yr_id ");
            sb.Append("INNER JOIN public.erm_emp_inf e ON e.emp_id = r.rvw_emp_id ");
            sb.Append("INNER JOIN public.erm_emp_inf f ON f.emp_id = r.rvw_aprsr_id ");
            sb.Append("INNER JOIN public.gst_depts d ON d.deptqk = e.dept_id ");
            sb.Append("INNER JOIN public.gst_units u ON u.unitqk = e.unit_id ");
            sb.Append("INNER JOIN public.gst_locs l ON l.locqk = e.loc_id ");
            sb.Append("WHERE (s.rvw_sxn_id = @rvw_sxn_id) AND (h.is_flg = true) ");
            sb.Append("ORDER BY r.rvw_smr_id;");

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
                    resultDetailList.Add(new ResultDetail()
                    {
                        ResultSummaryId = reader["rvw_smr_id"] == DBNull.Value ? 0 : (int)reader["rvw_smr_id"],
                        ReviewHeaderId = reader["rvw_hdr_id"] == DBNull.Value ? 0 : (int)reader["rvw_hdr_id"],
                        ReviewSessionId = reader["rvw_sxn_id"] == DBNull.Value ? 0 : (int)reader["rvw_sxn_id"],
                        ReviewSessionName = reader["rvw_sxn_nm"] == DBNull.Value ? string.Empty : reader["rvw_sxn_nm"].ToString(),

                        AppraiseeId = reader["rvw_emp_id"] == DBNull.Value ? string.Empty : reader["rvw_emp_id"].ToString(),
                        AppraiseeName = reader["rvw_emp_nm"] == DBNull.Value ? string.Empty : reader["rvw_emp_nm"].ToString(),

                        ReviewYearId = reader["rvw_yr_id"] == DBNull.Value ? 0 : (int)reader["rvw_yr_id"],
                        ReviewYearName = reader["pms_yr_nm"] == DBNull.Value ? string.Empty : reader["pms_yr_nm"].ToString(),

                        AppraiserId = reader["rvw_aprsr_id"] == DBNull.Value ? string.Empty : reader["rvw_aprsr_id"].ToString(),
                        AppraiserName = reader["rvw_aprsr_nm"] == DBNull.Value ? string.Empty : reader["rvw_aprsr_nm"].ToString(),
                        AppraiserDesignation = reader["rvw_aprsr_dsg"] == DBNull.Value ? string.Empty : reader["rvw_aprsr_dsg"].ToString(),


                        KpaScoreTotal = reader["ttl_kpa_scr"] == DBNull.Value ? 0.00M : (decimal)reader["ttl_kpa_scr"],
                        KpaScoreObtained = reader["kpa_scr_obt"] == DBNull.Value ? 0.00M : (decimal)reader["kpa_scr_obt"],
                        CompetencyScoreTotal = reader["ttl_cmp_scr"] == DBNull.Value ? 0.00M : (decimal)reader["ttl_cmp_scr"],
                        CompetencyScoreObtained = reader["cmp_scr_obt"] == DBNull.Value ? 0.00M : (decimal)reader["cmp_scr_obt"],
                        CombinedScoreTotal = reader["ttl_cmb_scr"] == DBNull.Value ? 0.00M : (decimal)reader["ttl_cmb_scr"],
                        CombinedScoreObtained = reader["cmb_scr_obt"] == DBNull.Value ? 0.00M : (decimal)reader["cmb_scr_obt"],

                        ScoreRank = reader["scr_rank"] == DBNull.Value ? 0 : (int)reader["scr_rank"],
                        ScoreRankDescription = reader["scr_rank_ds"] == DBNull.Value ? string.Empty : reader["scr_rank_ds"].ToString(),
                        PerformanceRating = reader["pfm_rating"] == DBNull.Value ? string.Empty : reader["pfm_rating"].ToString(),

                        AppraiserTypeDescription = reader["apr_typ_ds"] == DBNull.Value ? string.Empty : reader["apr_typ_ds"].ToString(),
                        AppraiserRoleDescription = reader["aprsr_rl_ds"] == DBNull.Value ? string.Empty : reader["aprsr_rl_ds"].ToString(),

                        UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                        UnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),

                        DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                        DepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),

                        LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                        LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),

                        EmployeeNo = reader["emp_no_1"] == DBNull.Value ? string.Empty : reader["emp_no_1"].ToString(),
                        IsMain = reader["is_main"] == DBNull.Value ? false : (bool)reader["is_main"],

                        AppraiseeDesignation = reader["current_designation"] == DBNull.Value ? string.Empty : reader["current_designation"].ToString(),
                        DepartmentHeadComments = reader["dh_rmk"] == DBNull.Value ? string.Empty : reader["dh_rmk"].ToString(),
                        DepartmentHeadName = reader["dh_nm"] == DBNull.Value ? string.Empty : reader["dh_nm"].ToString(),
                        DepartmentHeadRecommendation = reader["dh_rec"] == DBNull.Value ? string.Empty : reader["dh_rec"].ToString(),
                        FeedbackProblems = reader["fbk_probs"] == DBNull.Value ? string.Empty : reader["fbk_probs"].ToString(),
                        FeedbackSolutions = reader["fbk_solns"] == DBNull.Value ? string.Empty : reader["fbk_solns"].ToString(),
                        HrComments = reader["hr_rmk"] == DBNull.Value ? string.Empty : reader["hr_rmk"].ToString(),
                        HrName = reader["hr_nm"] == DBNull.Value ? string.Empty : reader["hr_nm"].ToString(),
                        HrRecommendation = reader["hr_rec"] == DBNull.Value ? string.Empty : reader["hr_rec"].ToString(),
                        IsFlagged = reader["is_flg"] == DBNull.Value ? false : (bool)reader["is_flg"],
                        FlaggedReason = reader["flg_rsn"] == DBNull.Value ? string.Empty : reader["flg_rsn"].ToString(),
                        FlaggedTime = reader["flg_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["flg_dt"],

                        LineManagerComments = reader["lm_rmk"] == DBNull.Value ? string.Empty : reader["lm_rmk"].ToString(),
                        LineManagerName = reader["lm_nm"] == DBNull.Value ? string.Empty : reader["lm_nm"].ToString(),
                        LineManagerRecommendation = reader["lm_rec"] == DBNull.Value ? string.Empty : reader["lm_rec"].ToString(),
                        ManagementComments = reader["mgt_rmk"] == DBNull.Value ? string.Empty : reader["mgt_rmk"].ToString(),
                        ManagementDecision = reader["mgt_dec"] == DBNull.Value ? string.Empty : reader["mgt_dec"].ToString(),
                        ManagementName = reader["mgt_nm"] == DBNull.Value ? string.Empty : reader["mgt_nm"].ToString(),
                        PerformanceGoal = reader["rvw_gls"] == DBNull.Value ? string.Empty : reader["rvw_gls"].ToString(),
                        UnitHeadComments = reader["uh_rmk"] == DBNull.Value ? string.Empty : reader["uh_rmk"].ToString(),
                        UnitHeadName = reader["uh_nm"] == DBNull.Value ? string.Empty : reader["uh_nm"].ToString(),
                        UnitHeadRecommendation = reader["uh_rec"] == DBNull.Value ? string.Empty : reader["uh_rec"].ToString(),
                    }) ;
                }
            }
            await conn.CloseAsync();
            return resultDetailList;
        }

        public async Task<IList<ResultDetail>> GetRejectedPrincipalResultDetailByLocationIdAndReviewSessionIdAsync(int reviewSessionId, int locationId)
        {
            List<ResultDetail> resultDetailList = new List<ResultDetail>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT r.rvw_smr_id, r.rvw_hdr_id, r.rvw_sxn_id, r.rvw_emp_id, ");
            sb.Append("r.rvw_aprsr_id, r.kpa_scr_obt, r.cmp_scr_obt, r.cmb_scr_obt, ");
            sb.Append("r.scr_rank, r.scr_rank_ds, r.pfm_rating, r.apr_typ_ds, ");
            sb.Append("r.aprsr_rl_ds, r.rvw_yr_id, s.rvw_sxn_nm,  r.is_main, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = r.rvw_emp_id) as rvw_emp_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = r.rvw_aprsr_id) as rvw_aprsr_nm, ");
            sb.Append("y.pms_yr_nm, e.unit_id, e.dept_id, e.loc_id, e.emp_no_1, ");
            sb.Append("e.current_designation, d.deptname, u.unitname, l.locname, ");
            sb.Append("f.current_designation as rvw_aprsr_dsg, h.flg_rsn, h.flg_dt, ");
            sb.Append("s.ttl_cmp_scr, s.ttl_kpa_scr, s.ttl_cmb_scr, ");
            sb.Append("h.fbk_probs, h.fbk_solns, h.lm_rmk, h.uh_rmk, h.dh_rmk, ");
            sb.Append("h.hr_rmk, h.mgt_rmk, h.is_flg, h.rvw_gls, h.lm_nm, ");
            sb.Append("h.uh_nm, h.dh_nm, h.hr_nm, h.mgt_nm, h.lm_rec, ");
            sb.Append("h.uh_rec, h.dh_rec, h.hr_rec, h.mgt_dec ");
            sb.Append("FROM public.pmsrvwsmry r ");
            sb.Append("INNER JOIN public.pmsrvwhdrs h ON (h.rvw_hdr_id = r.rvw_hdr_id ");
            sb.Append("AND h.pry_apr_id = r.rvw_aprsr_id) ");
            sb.Append("INNER JOIN public.pmsrvwsxns s ON s.rvw_sxn_id = r.rvw_sxn_id ");
            sb.Append("INNER JOIN public.pmssttyrs y ON y.pms_yr_id = r.rvw_yr_id ");
            sb.Append("INNER JOIN public.erm_emp_inf e ON e.emp_id = r.rvw_emp_id ");
            sb.Append("INNER JOIN public.erm_emp_inf f ON f.emp_id = r.rvw_aprsr_id ");
            sb.Append("INNER JOIN public.gst_depts d ON d.deptqk = e.dept_id ");
            sb.Append("INNER JOIN public.gst_units u ON u.unitqk = e.unit_id ");
            sb.Append("INNER JOIN public.gst_locs l ON l.locqk = e.loc_id ");
            sb.Append("WHERE (s.rvw_sxn_id = @rvw_sxn_id) ");
            sb.Append("AND (e.loc_id = @loc_id) AND (h.is_flg = true) ");
            sb.Append("ORDER BY r.rvw_smr_id;");

            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_sxn_id = cmd.Parameters.Add("@rvw_sxn_id", NpgsqlDbType.Integer);
                var loc_id = cmd.Parameters.Add("@loc_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                rvw_sxn_id.Value = reviewSessionId;
                loc_id.Value = locationId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    resultDetailList.Add(new ResultDetail()
                    {
                        ResultSummaryId = reader["rvw_smr_id"] == DBNull.Value ? 0 : (int)reader["rvw_smr_id"],
                        ReviewHeaderId = reader["rvw_hdr_id"] == DBNull.Value ? 0 : (int)reader["rvw_hdr_id"],
                        ReviewSessionId = reader["rvw_sxn_id"] == DBNull.Value ? 0 : (int)reader["rvw_sxn_id"],
                        ReviewSessionName = reader["rvw_sxn_nm"] == DBNull.Value ? string.Empty : reader["rvw_sxn_nm"].ToString(),

                        AppraiseeId = reader["rvw_emp_id"] == DBNull.Value ? string.Empty : reader["rvw_emp_id"].ToString(),
                        AppraiseeName = reader["rvw_emp_nm"] == DBNull.Value ? string.Empty : reader["rvw_emp_nm"].ToString(),

                        ReviewYearId = reader["rvw_yr_id"] == DBNull.Value ? 0 : (int)reader["rvw_yr_id"],
                        ReviewYearName = reader["pms_yr_nm"] == DBNull.Value ? string.Empty : reader["pms_yr_nm"].ToString(),

                        AppraiserId = reader["rvw_aprsr_id"] == DBNull.Value ? string.Empty : reader["rvw_aprsr_id"].ToString(),
                        AppraiserName = reader["rvw_aprsr_nm"] == DBNull.Value ? string.Empty : reader["rvw_aprsr_nm"].ToString(),
                        AppraiserDesignation = reader["rvw_aprsr_dsg"] == DBNull.Value ? string.Empty : reader["rvw_aprsr_dsg"].ToString(),

                        KpaScoreTotal = reader["ttl_kpa_scr"] == DBNull.Value ? 0.00M : (decimal)reader["ttl_kpa_scr"],
                        KpaScoreObtained = reader["kpa_scr_obt"] == DBNull.Value ? 0.00M : (decimal)reader["kpa_scr_obt"],
                        CompetencyScoreTotal = reader["ttl_cmp_scr"] == DBNull.Value ? 0.00M : (decimal)reader["ttl_cmp_scr"],
                        CompetencyScoreObtained = reader["cmp_scr_obt"] == DBNull.Value ? 0.00M : (decimal)reader["cmp_scr_obt"],
                        CombinedScoreTotal = reader["ttl_cmb_scr"] == DBNull.Value ? 0.00M : (decimal)reader["ttl_cmb_scr"],
                        CombinedScoreObtained = reader["cmb_scr_obt"] == DBNull.Value ? 0.00M : (decimal)reader["cmb_scr_obt"],

                        ScoreRank = reader["scr_rank"] == DBNull.Value ? 0 : (int)reader["scr_rank"],
                        ScoreRankDescription = reader["scr_rank_ds"] == DBNull.Value ? string.Empty : reader["scr_rank_ds"].ToString(),
                        PerformanceRating = reader["pfm_rating"] == DBNull.Value ? string.Empty : reader["pfm_rating"].ToString(),

                        AppraiserTypeDescription = reader["apr_typ_ds"] == DBNull.Value ? string.Empty : reader["apr_typ_ds"].ToString(),
                        AppraiserRoleDescription = reader["aprsr_rl_ds"] == DBNull.Value ? string.Empty : reader["aprsr_rl_ds"].ToString(),

                        UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                        UnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),

                        DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                        DepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),

                        LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                        LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),

                        EmployeeNo = reader["emp_no_1"] == DBNull.Value ? string.Empty : reader["emp_no_1"].ToString(),
                        IsMain = reader["is_main"] == DBNull.Value ? false : (bool)reader["is_main"],

                        AppraiseeDesignation = reader["current_designation"] == DBNull.Value ? string.Empty : reader["current_designation"].ToString(),
                        DepartmentHeadComments = reader["dh_rmk"] == DBNull.Value ? string.Empty : reader["dh_rmk"].ToString(),
                        DepartmentHeadName = reader["dh_nm"] == DBNull.Value ? string.Empty : reader["dh_nm"].ToString(),
                        DepartmentHeadRecommendation = reader["dh_rec"] == DBNull.Value ? string.Empty : reader["dh_rec"].ToString(),
                        FeedbackProblems = reader["fbk_probs"] == DBNull.Value ? string.Empty : reader["fbk_probs"].ToString(),
                        FeedbackSolutions = reader["fbk_solns"] == DBNull.Value ? string.Empty : reader["fbk_solns"].ToString(),
                        HrComments = reader["hr_rmk"] == DBNull.Value ? string.Empty : reader["hr_rmk"].ToString(),
                        HrName = reader["hr_nm"] == DBNull.Value ? string.Empty : reader["hr_nm"].ToString(),
                        HrRecommendation = reader["hr_rec"] == DBNull.Value ? string.Empty : reader["hr_rec"].ToString(),
                        
                        IsFlagged = reader["is_flg"] == DBNull.Value ? false : (bool)reader["is_flg"],
                        FlaggedReason = reader["flg_rsn"] == DBNull.Value ? string.Empty : reader["flg_rsn"].ToString(),
                        FlaggedTime = reader["flg_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["flg_dt"],

                        LineManagerComments = reader["lm_rmk"] == DBNull.Value ? string.Empty : reader["lm_rmk"].ToString(),
                        LineManagerName = reader["lm_nm"] == DBNull.Value ? string.Empty : reader["lm_nm"].ToString(),
                        LineManagerRecommendation = reader["lm_rec"] == DBNull.Value ? string.Empty : reader["lm_rec"].ToString(),
                        ManagementComments = reader["mgt_rmk"] == DBNull.Value ? string.Empty : reader["mgt_rmk"].ToString(),
                        ManagementDecision = reader["mgt_dec"] == DBNull.Value ? string.Empty : reader["mgt_dec"].ToString(),
                        ManagementName = reader["mgt_nm"] == DBNull.Value ? string.Empty : reader["mgt_nm"].ToString(),
                        PerformanceGoal = reader["rvw_gls"] == DBNull.Value ? string.Empty : reader["rvw_gls"].ToString(),
                        UnitHeadComments = reader["uh_rmk"] == DBNull.Value ? string.Empty : reader["uh_rmk"].ToString(),
                        UnitHeadName = reader["uh_nm"] == DBNull.Value ? string.Empty : reader["uh_nm"].ToString(),
                        UnitHeadRecommendation = reader["uh_rec"] == DBNull.Value ? string.Empty : reader["uh_rec"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return resultDetailList;
        }

        #endregion
    }
}
