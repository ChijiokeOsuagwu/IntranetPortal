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
    public class ReviewMetricRepository : IReviewMetricRepository
    {
        public IConfiguration _config { get; }
        public ReviewMetricRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        #region Review Metric Read Action Methods
        public async Task<List<ReviewMetric>> GetByReviewHeaderIdAsync(int reviewHeaderId)
        {
            List<ReviewMetric> reviewMetricList = new List<ReviewMetric>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT m.rvw_mtrc_id, m.rvw_hdr_id, m.rvw_sxn_id, m.rvw_yr_id, ");
            sb.Append("m.mtrc_typ_id, m.mtrc_ds, m.mtrc_kpi, m.mtrc_tgt, m.mtrc_wtg, ");
            sb.Append("h.rvw_emp_id, h.pry_apr_id, s.rvw_sxn_nm, ");
            sb.Append("f.fullname AS pry_apr_nm, a.fullname AS rvw_emp_nm, ");
            sb.Append("CASE m.mtrc_typ_id WHEN 0 THEN 'KPA' WHEN 1 THEN 'Competency' ");
            sb.Append("END mtrc_typ_ds FROM public.pmsrvwmtrcs m ");
            sb.Append("INNER JOIN public.pmsrvwhdrs h ON h.rvw_hdr_id = m.rvw_hdr_id ");
            sb.Append("INNER JOIN public.pmsrvwsxns s ON s.rvw_sxn_id = m.rvw_sxn_id ");
            sb.Append("INNER JOIN public.gst_prsns a ON a.id = h.rvw_emp_id ");
            sb.Append("LEFT JOIN public.gst_prsns f ON f.id = h.pry_apr_id ");
            sb.Append("WHERE (m.rvw_hdr_id = @rvw_hdr_id) ");
            sb.Append("ORDER BY m.rvw_mtrc_id; ");
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
                    reviewMetricList.Add(new ReviewMetric()
                    {
                        ReviewMetricId = reader["rvw_mtrc_id"] == DBNull.Value ? 0 : (int)reader["rvw_mtrc_id"],
                        ReviewHeaderId = reader["rvw_hdr_id"] == DBNull.Value ? 0 : (int)reader["rvw_hdr_id"],
                        ReviewSessionId = reader["rvw_sxn_id"] == DBNull.Value ? 0 : (int)reader["rvw_sxn_id"],
                        ReviewYearId = reader["rvw_yr_id"] == DBNull.Value ? 0 : (int)reader["rvw_yr_id"],
                        ReviewMetricTypeId = reader["mtrc_typ_id"] == DBNull.Value ? 0 : (int)reader["mtrc_typ_id"],
                        ReviewMetricTypeDescription = reader["mtrc_typ_ds"] == DBNull.Value ? string.Empty : reader["mtrc_typ_ds"].ToString(),
                        ReviewMetricDescription = reader["mtrc_ds"] == DBNull.Value ? string.Empty : reader["mtrc_ds"].ToString(),
                        ReviewMetricKpi = reader["mtrc_kpi"] == DBNull.Value ? string.Empty : reader["mtrc_kpi"].ToString(),
                        ReviewMetricTarget = reader["mtrc_tgt"] == DBNull.Value ? string.Empty : reader["mtrc_tgt"].ToString(),
                        ReviewMetricWeightage = reader["mtrc_wtg"] == DBNull.Value ? 0.00M : (decimal)reader["mtrc_wtg"],
                        AppraiseeId = reader["rvw_emp_id"] == DBNull.Value ? string.Empty : reader["rvw_emp_id"].ToString(),
                        PrimaryAppraiserId = reader["pry_apr_id"] == DBNull.Value ? string.Empty : reader["pry_apr_id"].ToString(),
                        ReviewSessionDescription = reader["rvw_sxn_nm"] == DBNull.Value ? string.Empty : reader["rvw_sxn_nm"].ToString(),
                        PrimaryAppraiserName = reader["pry_apr_nm"] == DBNull.Value ? string.Empty : reader["pry_apr_nm"].ToString(),
                        AppraiseeName = reader["rvw_emp_nm"] == DBNull.Value ? string.Empty : reader["rvw_emp_nm"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return reviewMetricList;
        }

        public async Task<List<ReviewMetric>> GetByMetricDescriptionAsync(int reviewHeaderId, string metricDescription)
        {
            List<ReviewMetric> reviewMetricList = new List<ReviewMetric>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT m.rvw_mtrc_id, m.rvw_hdr_id, m.rvw_sxn_id, m.rvw_yr_id, ");
            sb.Append("m.mtrc_typ_id, m.mtrc_ds, m.mtrc_kpi, m.mtrc_tgt, m.mtrc_wtg, ");
            sb.Append("h.rvw_emp_id, h.pry_apr_id, s.rvw_sxn_nm, ");
            sb.Append("f.fullname AS pry_apr_nm, a.fullname AS rvw_emp_nm, ");
            sb.Append("CASE m.mtrc_typ_id WHEN 0 THEN 'KPA' WHEN 1 THEN 'Competency' ");
            sb.Append("END mtrc_typ_ds FROM public.pmsrvwmtrcs m ");
            sb.Append("INNER JOIN public.pmsrvwhdrs h ON h.rvw_hdr_id = m.rvw_hdr_id ");
            sb.Append("INNER JOIN public.pmsrvwsxns s ON s.rvw_sxn_id = m.rvw_sxn_id ");
            sb.Append("INNER JOIN public.gst_prsns a ON a.id = h.rvw_emp_id ");
            sb.Append("LEFT JOIN public.gst_prsns f ON f.id = h.pry_apr_id ");
            sb.Append("WHERE (m.rvw_hdr_id = @rvw_hdr_id) ");
            sb.Append("AND (LOWER(m.mtrc_ds) = LOWER(@mtrc_ds)) ");
            sb.Append("ORDER BY m.rvw_mtrc_id; ");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_hdr_id = cmd.Parameters.Add("@rvw_hdr_id", NpgsqlDbType.Integer);
                var mtrc_ds = cmd.Parameters.Add("@mtrc_ds", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                rvw_hdr_id.Value = reviewHeaderId;
                mtrc_ds.Value = metricDescription;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    reviewMetricList.Add(new ReviewMetric()
                    {
                        ReviewMetricId = reader["rvw_mtrc_id"] == DBNull.Value ? 0 : (int)reader["rvw_mtrc_id"],
                        ReviewHeaderId = reader["rvw_hdr_id"] == DBNull.Value ? 0 : (int)reader["rvw_hdr_id"],
                        ReviewSessionId = reader["rvw_sxn_id"] == DBNull.Value ? 0 : (int)reader["rvw_sxn_id"],
                        ReviewYearId = reader["rvw_yr_id"] == DBNull.Value ? 0 : (int)reader["rvw_yr_id"],
                        ReviewMetricTypeId = reader["mtrc_typ_id"] == DBNull.Value ? 0 : (int)reader["mtrc_typ_id"],
                        ReviewMetricTypeDescription = reader["mtrc_typ_ds"] == DBNull.Value ? string.Empty : reader["mtrc_typ_ds"].ToString(),
                        ReviewMetricDescription = reader["mtrc_ds"] == DBNull.Value ? string.Empty : reader["mtrc_ds"].ToString(),
                        ReviewMetricKpi = reader["mtrc_kpi"] == DBNull.Value ? string.Empty : reader["mtrc_kpi"].ToString(),
                        ReviewMetricTarget = reader["mtrc_tgt"] == DBNull.Value ? string.Empty : reader["mtrc_tgt"].ToString(),
                        ReviewMetricWeightage = reader["mtrc_wtg"] == DBNull.Value ? 0.00M : (decimal)reader["mtrc_wtg"],
                        AppraiseeId = reader["rvw_emp_id"] == DBNull.Value ? string.Empty : reader["rvw_emp_id"].ToString(),
                        PrimaryAppraiserId = reader["pry_apr_id"] == DBNull.Value ? string.Empty : reader["pry_apr_id"].ToString(),
                        ReviewSessionDescription = reader["rvw_sxn_nm"] == DBNull.Value ? string.Empty : reader["rvw_sxn_nm"].ToString(),
                        PrimaryAppraiserName = reader["pry_apr_nm"] == DBNull.Value ? string.Empty : reader["pry_apr_nm"].ToString(),
                        AppraiseeName = reader["rvw_emp_nm"] == DBNull.Value ? string.Empty : reader["rvw_emp_nm"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return reviewMetricList;
        }

        public async Task<List<ReviewMetric>> GetKpasByReviewHeaderIdAsync(int reviewHeaderId)
        {
            List<ReviewMetric> reviewMetricList = new List<ReviewMetric>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT m.rvw_mtrc_id, m.rvw_hdr_id, m.rvw_sxn_id, m.rvw_yr_id, ");
            sb.Append("m.mtrc_typ_id, m.mtrc_ds, m.mtrc_kpi, m.mtrc_tgt, m.mtrc_wtg, ");
            sb.Append("h.rvw_emp_id, h.pry_apr_id, s.rvw_sxn_nm, ");
            sb.Append("f.fullname AS pry_apr_nm, a.fullname AS rvw_emp_nm, ");
            sb.Append("CASE m.mtrc_typ_id WHEN 0 THEN 'KPA' WHEN 1 THEN 'Competency' ");
            sb.Append("END mtrc_typ_ds FROM public.pmsrvwmtrcs m ");
            sb.Append("INNER JOIN public.pmsrvwhdrs h ON h.rvw_hdr_id = m.rvw_hdr_id ");
            sb.Append("INNER JOIN public.pmsrvwsxns s ON s.rvw_sxn_id = m.rvw_sxn_id ");
            sb.Append("INNER JOIN public.gst_prsns a ON a.id = h.rvw_emp_id ");
            sb.Append("LEFT JOIN public.gst_prsns f ON f.id = h.pry_apr_id ");
            sb.Append("WHERE (m.rvw_hdr_id = @rvw_hdr_id) ");
            sb.Append("AND (m.mtrc_typ_id = 0) ");
            sb.Append("ORDER BY m.rvw_mtrc_id; ");

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
                    reviewMetricList.Add(new ReviewMetric()
                    {
                        ReviewMetricId = reader["rvw_mtrc_id"] == DBNull.Value ? 0 : (int)reader["rvw_mtrc_id"],
                        ReviewHeaderId = reader["rvw_hdr_id"] == DBNull.Value ? 0 : (int)reader["rvw_hdr_id"],
                        ReviewSessionId = reader["rvw_sxn_id"] == DBNull.Value ? 0 : (int)reader["rvw_sxn_id"],
                        ReviewYearId = reader["rvw_yr_id"] == DBNull.Value ? 0 : (int)reader["rvw_yr_id"],
                        ReviewMetricTypeId = reader["mtrc_typ_id"] == DBNull.Value ? 0 : (int)reader["mtrc_typ_id"],
                        ReviewMetricTypeDescription = reader["mtrc_typ_ds"] == DBNull.Value ? string.Empty : reader["mtrc_typ_ds"].ToString(),
                        ReviewMetricDescription = reader["mtrc_ds"] == DBNull.Value ? string.Empty : reader["mtrc_ds"].ToString(),
                        ReviewMetricKpi = reader["mtrc_kpi"] == DBNull.Value ? string.Empty : reader["mtrc_kpi"].ToString(),
                        ReviewMetricTarget = reader["mtrc_tgt"] == DBNull.Value ? string.Empty : reader["mtrc_tgt"].ToString(),
                        ReviewMetricWeightage = reader["mtrc_wtg"] == DBNull.Value ? 0.00M : (decimal)reader["mtrc_wtg"],
                        AppraiseeId = reader["rvw_emp_id"] == DBNull.Value ? string.Empty : reader["rvw_emp_id"].ToString(),
                        PrimaryAppraiserId = reader["pry_apr_id"] == DBNull.Value ? string.Empty : reader["pry_apr_id"].ToString(),
                        ReviewSessionDescription = reader["rvw_sxn_nm"] == DBNull.Value ? string.Empty : reader["rvw_sxn_nm"].ToString(),
                        PrimaryAppraiserName = reader["pry_apr_nm"] == DBNull.Value ? string.Empty : reader["pry_apr_nm"].ToString(),
                        AppraiseeName = reader["rvw_emp_nm"] == DBNull.Value ? string.Empty : reader["rvw_emp_nm"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return reviewMetricList;
        }

        public async Task<List<ReviewMetric>> GetCmpsByReviewHeaderIdAsync(int reviewHeaderId)
        {
            List<ReviewMetric> reviewMetricList = new List<ReviewMetric>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT m.rvw_mtrc_id, m.rvw_hdr_id, m.rvw_sxn_id, m.rvw_yr_id, ");
            sb.Append("m.mtrc_typ_id, m.mtrc_ds, m.mtrc_kpi, m.mtrc_tgt, m.mtrc_wtg, ");
            sb.Append("h.rvw_emp_id, h.pry_apr_id, s.rvw_sxn_nm, ");
            sb.Append("f.fullname AS pry_apr_nm, a.fullname AS rvw_emp_nm, ");
            sb.Append("CASE m.mtrc_typ_id WHEN 0 THEN 'KPA' WHEN 1 THEN 'Competency' ");
            sb.Append("END mtrc_typ_ds FROM public.pmsrvwmtrcs m ");
            sb.Append("INNER JOIN public.pmsrvwhdrs h ON h.rvw_hdr_id = m.rvw_hdr_id ");
            sb.Append("INNER JOIN public.pmsrvwsxns s ON s.rvw_sxn_id = m.rvw_sxn_id ");
            sb.Append("INNER JOIN public.gst_prsns a ON a.id = h.rvw_emp_id ");
            sb.Append("LEFT JOIN public.gst_prsns f ON f.id = h.pry_apr_id ");
            sb.Append("WHERE (m.rvw_hdr_id = @rvw_hdr_id) ");
            sb.Append("AND (m.mtrc_typ_id = 1) ");
            sb.Append("ORDER BY m.rvw_mtrc_id; ");

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
                    reviewMetricList.Add(new ReviewMetric()
                    {
                        ReviewMetricId = reader["rvw_mtrc_id"] == DBNull.Value ? 0 : (int)reader["rvw_mtrc_id"],
                        ReviewHeaderId = reader["rvw_hdr_id"] == DBNull.Value ? 0 : (int)reader["rvw_hdr_id"],
                        ReviewSessionId = reader["rvw_sxn_id"] == DBNull.Value ? 0 : (int)reader["rvw_sxn_id"],
                        ReviewYearId = reader["rvw_yr_id"] == DBNull.Value ? 0 : (int)reader["rvw_yr_id"],
                        ReviewMetricTypeId = reader["mtrc_typ_id"] == DBNull.Value ? 0 : (int)reader["mtrc_typ_id"],
                        ReviewMetricTypeDescription = reader["mtrc_typ_ds"] == DBNull.Value ? string.Empty : reader["mtrc_typ_ds"].ToString(),
                        ReviewMetricDescription = reader["mtrc_ds"] == DBNull.Value ? string.Empty : reader["mtrc_ds"].ToString(),
                        ReviewMetricKpi = reader["mtrc_kpi"] == DBNull.Value ? string.Empty : reader["mtrc_kpi"].ToString(),
                        ReviewMetricTarget = reader["mtrc_tgt"] == DBNull.Value ? string.Empty : reader["mtrc_tgt"].ToString(),
                        ReviewMetricWeightage = reader["mtrc_wtg"] == DBNull.Value ? 0.00M : (decimal)reader["mtrc_wtg"],
                        AppraiseeId = reader["rvw_emp_id"] == DBNull.Value ? string.Empty : reader["rvw_emp_id"].ToString(),
                        PrimaryAppraiserId = reader["pry_apr_id"] == DBNull.Value ? string.Empty : reader["pry_apr_id"].ToString(),
                        ReviewSessionDescription = reader["rvw_sxn_nm"] == DBNull.Value ? string.Empty : reader["rvw_sxn_nm"].ToString(),
                        PrimaryAppraiserName = reader["pry_apr_nm"] == DBNull.Value ? string.Empty : reader["pry_apr_nm"].ToString(),
                        AppraiseeName = reader["rvw_emp_nm"] == DBNull.Value ? string.Empty : reader["rvw_emp_nm"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return reviewMetricList;
        }

        public async Task<List<ReviewMetric>> GetByIdAsync(int reviewMetricId)
        {
            List<ReviewMetric> reviewMetricList = new List<ReviewMetric>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT m.rvw_mtrc_id, m.rvw_hdr_id, m.rvw_sxn_id, m.rvw_yr_id, ");
            sb.Append("m.mtrc_typ_id, m.mtrc_ds, m.mtrc_kpi, m.mtrc_tgt, m.mtrc_wtg, ");
            sb.Append("h.rvw_emp_id, h.pry_apr_id, s.rvw_sxn_nm, ");
            sb.Append("f.fullname AS pry_apr_nm, a.fullname AS rvw_emp_nm, ");
            sb.Append("CASE m.mtrc_typ_id WHEN 0 THEN 'KPA' WHEN 1 THEN 'Competency' ");
            sb.Append("END mtrc_typ_ds FROM public.pmsrvwmtrcs m ");
            sb.Append("INNER JOIN public.pmsrvwhdrs h ON h.rvw_hdr_id = m.rvw_hdr_id ");
            sb.Append("INNER JOIN public.pmsrvwsxns s ON s.rvw_sxn_id = m.rvw_sxn_id ");
            sb.Append("INNER JOIN public.gst_prsns a ON a.id = h.rvw_emp_id ");
            sb.Append("LEFT JOIN public.gst_prsns f ON f.id = h.pry_apr_id ");
            sb.Append("WHERE (m.rvw_mtrc_id = @rvw_mtrc_id) ");
            sb.Append("ORDER BY m.rvw_mtrc_id; ");

            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_mtrc_id = cmd.Parameters.Add("@rvw_mtrc_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                rvw_mtrc_id.Value = reviewMetricId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    reviewMetricList.Add(new ReviewMetric()
                    {
                        ReviewMetricId = reader["rvw_mtrc_id"] == DBNull.Value ? 0 : (int)reader["rvw_mtrc_id"],
                        ReviewHeaderId = reader["rvw_hdr_id"] == DBNull.Value ? 0 : (int)reader["rvw_hdr_id"],
                        ReviewSessionId = reader["rvw_sxn_id"] == DBNull.Value ? 0 : (int)reader["rvw_sxn_id"],
                        ReviewYearId = reader["rvw_yr_id"] == DBNull.Value ? 0 : (int)reader["rvw_yr_id"],
                        ReviewMetricTypeId = reader["mtrc_typ_id"] == DBNull.Value ? 0 : (int)reader["mtrc_typ_id"],
                        ReviewMetricTypeDescription = reader["mtrc_typ_ds"] == DBNull.Value ? string.Empty : reader["mtrc_typ_ds"].ToString(),
                        ReviewMetricDescription = reader["mtrc_ds"] == DBNull.Value ? string.Empty : reader["mtrc_ds"].ToString(),
                        ReviewMetricKpi = reader["mtrc_kpi"] == DBNull.Value ? string.Empty : reader["mtrc_kpi"].ToString(),
                        ReviewMetricTarget = reader["mtrc_tgt"] == DBNull.Value ? string.Empty : reader["mtrc_tgt"].ToString(),
                        ReviewMetricWeightage = reader["mtrc_wtg"] == DBNull.Value ? 0.00M : (decimal)reader["mtrc_wtg"],
                        AppraiseeId = reader["rvw_emp_id"] == DBNull.Value ? string.Empty : reader["rvw_emp_id"].ToString(),
                        PrimaryAppraiserId = reader["pry_apr_id"] == DBNull.Value ? string.Empty : reader["pry_apr_id"].ToString(),
                        ReviewSessionDescription = reader["rvw_sxn_nm"] == DBNull.Value ? string.Empty : reader["rvw_sxn_nm"].ToString(),
                        PrimaryAppraiserName = reader["pry_apr_nm"] == DBNull.Value ? string.Empty : reader["pry_apr_nm"].ToString(),
                        AppraiseeName = reader["rvw_emp_nm"] == DBNull.Value ? string.Empty : reader["rvw_emp_nm"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return reviewMetricList;
        }

        public async Task<decimal> GetTotalWeightageByReviewHeaderIdAsync(int reviewHeaderId)
        {
            decimal total_weightage = 0.00M;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT  SUM(mtrc_wtg) as total ");
            sb.Append("FROM public.pmsrvwmtrcs ");
            sb.Append("WHERE (rvw_hdr_id = @rvw_hdr_id); ");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_hdr_id = cmd.Parameters.Add("@rvw_hdr_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                rvw_hdr_id.Value = reviewHeaderId;

                var wt = await cmd.ExecuteScalarAsync();
                total_weightage = (decimal)wt;
            }
            await conn.CloseAsync();
            return total_weightage;
        }

        public async Task<decimal> GetTotalKpaWeightageByReviewHeaderIdAsync(int reviewHeaderId)
        {
            decimal total_weightage = 0.00M;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT SUM(mtrc_wtg) as total ");
            sb.Append("FROM public.pmsrvwmtrcs ");
            sb.Append("WHERE (rvw_hdr_id = @rvw_hdr_id)  ");
            sb.Append("AND (mtrc_typ_id = 0); ");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_hdr_id = cmd.Parameters.Add("@rvw_hdr_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                rvw_hdr_id.Value = reviewHeaderId;

                var wt = await cmd.ExecuteScalarAsync();
                if (wt != null && wt != DBNull.Value)
                {
                    total_weightage = (decimal)wt;
                }
            }
            await conn.CloseAsync();
            return total_weightage;
        }

        public async Task<decimal> GetTotalCmpWeightageByReviewHeaderIdAsync(int reviewHeaderId)
        {
            decimal total_weightage = 0.00M;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT  SUM(mtrc_wtg) as total ");
            sb.Append("FROM public.pmsrvwmtrcs ");
            sb.Append("WHERE (rvw_hdr_id = @rvw_hdr_id)  ");
            sb.Append("AND (mtrc_typ_id = 1); ");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_hdr_id = cmd.Parameters.Add("@rvw_hdr_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                rvw_hdr_id.Value = reviewHeaderId;

                var wt = await cmd.ExecuteScalarAsync();
                total_weightage = (decimal)wt;
            }
            await conn.CloseAsync();
            return total_weightage;
        }

        public async Task<int> GetCmpCountByReviewHeaderIdAsync(int reviewHeaderId)
        {
            int total_weightage = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT COUNT(rvw_mtrc_id) as total ");
            sb.Append("FROM public.pmsrvwmtrcs ");
            sb.Append("WHERE (rvw_hdr_id = @rvw_hdr_id)  ");
            sb.Append("AND (mtrc_typ_id = 1); ");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_hdr_id = cmd.Parameters.Add("@rvw_hdr_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                rvw_hdr_id.Value = reviewHeaderId;

                var wt = await cmd.ExecuteScalarAsync();
                total_weightage = Convert.ToInt32(wt);
            }
            await conn.CloseAsync();
            return total_weightage;
        }

        public async Task<IList<ReviewMetric>> GetUnevaluatedByMetricTypeIdAsync(int reviewHeaderId, string appraiserId, int metricTypeId)
        {
            List<ReviewMetric> reviewMetricList = new List<ReviewMetric>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT m.rvw_mtrc_id, m.rvw_hdr_id, m.rvw_sxn_id, m.rvw_yr_id, ");
            sb.Append("m.mtrc_typ_id, m.mtrc_ds, m.mtrc_kpi, m.mtrc_tgt, m.mtrc_wtg ");
            sb.Append("FROM public.pmsrvwmtrcs m ");
            sb.Append("WHERE (m.rvw_hdr_id = @rvw_hdr_id) AND (m.mtrc_typ_id = @mtrc_typ_id) ");
            sb.Append("AND (m.rvw_mtrc_id NOT IN (SELECT rvw_mtric_id ");
            sb.Append("FROM public.pmsrvwrdtls d WHERE d.rvw_hdr_id = @rvw_hdr_id ");
            sb.Append("AND d.rvw_aprsr_id = @rvw_aprsr_id)) ");
            sb.Append("ORDER BY m.rvw_mtrc_id; ");


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
                    reviewMetricList.Add(new ReviewMetric()
                    {
                        ReviewMetricId = reader["rvw_mtrc_id"] == DBNull.Value ? 0 : (int)reader["rvw_mtrc_id"],
                        ReviewHeaderId = reader["rvw_hdr_id"] == DBNull.Value ? 0 : (int)reader["rvw_hdr_id"],
                        ReviewSessionId = reader["rvw_sxn_id"] == DBNull.Value ? 0 : (int)reader["rvw_sxn_id"],
                        ReviewYearId = reader["rvw_yr_id"] == DBNull.Value ? 0 : (int)reader["rvw_yr_id"],
                        ReviewMetricTypeId = reader["mtrc_typ_id"] == DBNull.Value ? 0 : (int)reader["mtrc_typ_id"],
                        ReviewMetricDescription = reader["mtrc_ds"] == DBNull.Value ? string.Empty : reader["mtrc_ds"].ToString(),
                        ReviewMetricKpi = reader["mtrc_kpi"] == DBNull.Value ? string.Empty : reader["mtrc_kpi"].ToString(),
                        ReviewMetricTarget = reader["mtrc_tgt"] == DBNull.Value ? string.Empty : reader["mtrc_tgt"].ToString(),
                        ReviewMetricWeightage = reader["mtrc_wtg"] == DBNull.Value ? 0.00M : (decimal)reader["mtrc_wtg"],
                    });
                }
            }
            await conn.CloseAsync();
            return reviewMetricList;
        }


        #endregion

        #region Review Metric Write Action Methods
        public async Task<bool> AddAsync(ReviewMetric reviewMetric)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.pmsrvwmtrcs (rvw_hdr_id, rvw_sxn_id, ");
            sb.Append("rvw_yr_id, mtrc_typ_id, mtrc_ds, mtrc_kpi, mtrc_tgt, mtrc_wtg) ");
            sb.Append("VALUES (@rvw_hdr_id, @rvw_sxn_id, @rvw_yr_id, @mtrc_typ_id, ");
            sb.Append("@mtrc_ds, @mtrc_kpi, @mtrc_tgt, @mtrc_wtg); ");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var rvw_hdr_id = cmd.Parameters.Add("@rvw_hdr_id", NpgsqlDbType.Integer);
                    var rvw_sxn_id = cmd.Parameters.Add("@rvw_sxn_id", NpgsqlDbType.Integer);
                    var rvw_yr_id = cmd.Parameters.Add("@rvw_yr_id", NpgsqlDbType.Integer);
                    var mtrc_typ_id = cmd.Parameters.Add("@mtrc_typ_id", NpgsqlDbType.Integer);
                    var mtrc_ds = cmd.Parameters.Add("@mtrc_ds", NpgsqlDbType.Text);
                    var mtrc_kpi = cmd.Parameters.Add("@mtrc_kpi", NpgsqlDbType.Text);
                    var mtrc_tgt = cmd.Parameters.Add("@mtrc_tgt", NpgsqlDbType.Text);
                    var mtrc_wtg = cmd.Parameters.Add("mtrc_wtg", NpgsqlDbType.Numeric);
                    cmd.Prepare();
                    rvw_hdr_id.Value = reviewMetric.ReviewHeaderId;
                    rvw_sxn_id.Value = reviewMetric.ReviewSessionId;
                    rvw_yr_id.Value = reviewMetric.ReviewYearId;
                    mtrc_typ_id.Value = reviewMetric.ReviewMetricTypeId;
                    mtrc_ds.Value = reviewMetric.ReviewMetricDescription;
                    mtrc_kpi.Value = reviewMetric.ReviewMetricKpi ?? (object)DBNull.Value;
                    mtrc_tgt.Value = reviewMetric.ReviewMetricTarget ?? (object)DBNull.Value;
                    mtrc_wtg.Value = reviewMetric.ReviewMetricWeightage;

                    rows = await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                await conn.CloseAsync();
            }
            return rows > 0;
        }

        public async Task<bool> UpdateAsync(ReviewMetric reviewMetric)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.pmsrvwmtrcs SET mtrc_ds=@mtrc_ds, ");
            sb.Append("mtrc_kpi=@mtrc_kpi, mtrc_tgt=@mtrc_tgt, mtrc_wtg=@mtrc_wtg ");
            sb.Append("WHERE (rvw_mtrc_id = @rvw_mtrc_id);");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var mtrc_ds = cmd.Parameters.Add("@mtrc_ds", NpgsqlDbType.Text);
                    var mtrc_kpi = cmd.Parameters.Add("@mtrc_kpi", NpgsqlDbType.Text);
                    var mtrc_tgt = cmd.Parameters.Add("@mtrc_tgt", NpgsqlDbType.Text);
                    var mtrc_wtg = cmd.Parameters.Add("mtrc_wtg", NpgsqlDbType.Numeric);
                    var rvw_mtrc_id = cmd.Parameters.Add("@rvw_mtrc_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    rvw_mtrc_id.Value = reviewMetric.ReviewMetricId;
                    mtrc_ds.Value = reviewMetric.ReviewMetricDescription;
                    mtrc_kpi.Value = reviewMetric.ReviewMetricKpi ?? (object)DBNull.Value;
                    mtrc_tgt.Value = reviewMetric.ReviewMetricTarget ?? (object)DBNull.Value;
                    mtrc_wtg.Value = reviewMetric.ReviewMetricWeightage;

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

        public async Task<bool> DeleteAsync(int reviewMetricId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("DELETE FROM public.pmsrvwmtrcs ");
            sb.Append("WHERE (rvw_mtrc_id = @rvw_mtrc_id);");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var rvw_mtrc_id = cmd.Parameters.Add("@rvw_mtrc_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    rvw_mtrc_id.Value = reviewMetricId;

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

    }
}
