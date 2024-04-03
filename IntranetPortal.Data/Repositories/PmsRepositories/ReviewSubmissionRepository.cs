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
    public class ReviewSubmissionRepository : IReviewSubmissionRepository
    {
        public IConfiguration _config { get; }
        public ReviewSubmissionRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        #region Review Submission Read Action Methods
        public async Task<List<ReviewSubmission>> GetByReviewerIdAsync(string reviewerId)
        {
            List<ReviewSubmission> reviewSubmissionList = new List<ReviewSubmission>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.rvw_sbm_id, s.rvw_hdr_id, s.frm_emp_id, s.to_emp_id, ");
            sb.Append("s.sbm_typ_id, s.sbm_dt, s.sbm_msg, s.is_xtn, s.dt_xtn, ");
            sb.Append("e.fullname AS frm_emp_nm, f.fullname AS to_emp_nm, h.rvw_sxn_id, ");
            sb.Append("h.rvw_stg_id, s.apvr_rl_id, a.aprv_rl_nm, ");
            sb.Append("CASE sbm_typ_id WHEN 1 THEN 'Performance Contract Approval' ");
            sb.Append("WHEN 2 THEN 'Final Evaluation' WHEN 3 THEN 'Result Approval' ");
            sb.Append("ELSE 'Not Sure' END sbm_typ_ds ");
            sb.Append("FROM public.pmsrvwsbms s ");
            sb.Append("INNER JOIN public.pmsrvwhdrs h ON h.rvw_hdr_id = s.rvw_hdr_id ");
            sb.Append("INNER JOIN public.pmsaprvrls a ON a.aprv_rl_id = s.apvr_rl_id ");
            sb.Append("INNER JOIN public.gst_prsns e ON e.id = s.frm_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns f ON f.id = s.to_emp_id ");
            sb.Append("WHERE (s.to_emp_id = @to_emp_id) AND (s.is_del = false) ");
            sb.Append("ORDER BY s.sbm_dt DESC;");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var to_emp_id = cmd.Parameters.Add("@to_emp_id", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                to_emp_id.Value = reviewerId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    reviewSubmissionList.Add(new ReviewSubmission()
                    {
                        ReviewSubmissionId = reader["rvw_sbm_id"] == DBNull.Value ? 0 : (int)reader["rvw_sbm_id"],
                        ReviewHeaderId = reader["rvw_hdr_id"] == DBNull.Value ? 0 : (int)reader["rvw_hdr_id"],
                        ReviewSessionId = reader["rvw_sxn_id"] == DBNull.Value ? 0 : (int)reader["rvw_sxn_id"],
                        FromEmployeeId = reader["frm_emp_id"] == DBNull.Value ? string.Empty : reader["frm_emp_id"].ToString(),
                        FromEmployeeName = reader["frm_emp_nm"] == DBNull.Value ? string.Empty : reader["frm_emp_nm"].ToString(),
                        ToEmployeeId = reader["to_emp_id"] == DBNull.Value ? string.Empty : reader["to_emp_id"].ToString(),
                        ToEmployeeName = reader["to_emp_nm"] == DBNull.Value ? string.Empty : reader["to_emp_nm"].ToString(),
                        ToEmployeeRoleId = reader["apvr_rl_id"] == DBNull.Value ? 0 : (int)reader["apvr_rl_id"],
                        ToEmployeeRoleName = reader["aprv_rl_nm"] == DBNull.Value ? string.Empty : reader["aprv_rl_nm"].ToString(),
                        SubmissionPurposeId = reader["sbm_typ_id"] == DBNull.Value ? 0 : (int)reader["sbm_typ_id"],
                        SubmissionPurposeDescription = reader["sbm_typ_ds"] == DBNull.Value ? string.Empty : reader["sbm_typ_ds"].ToString(),
                        TimeSubmitted = reader["sbm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["sbm_dt"],
                        SubmissionMessage = reader["sbm_msg"] == DBNull.Value ? string.Empty : reader["sbm_msg"].ToString(),
                        IsActioned = reader["is_xtn"] == DBNull.Value ? false : (bool)reader["is_xtn"],
                        TimeActioned = reader["dt_xtn"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dt_xtn"],
                    });
                }
            }
            await conn.CloseAsync();
            return reviewSubmissionList;
        }
        public async Task<List<ReviewSubmission>> GetByReviewerIdAndReviewSessionIdAsync(string reviewerId, int reviewSessionId)
        {
            List<ReviewSubmission> reviewSubmissionList = new List<ReviewSubmission>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.rvw_sbm_id, s.rvw_hdr_id, s.frm_emp_id, s.to_emp_id, ");
            sb.Append("s.sbm_typ_id, s.sbm_dt, s.sbm_msg, s.is_xtn, s.dt_xtn, ");
            sb.Append("e.fullname AS frm_emp_nm, f.fullname AS to_emp_nm, h.rvw_sxn_id, ");
            sb.Append("h.rvw_stg_id, s.apvr_rl_id, a.aprv_rl_nm, ");
            sb.Append("CASE sbm_typ_id WHEN 1 THEN 'Performance Contract Approval' ");
            sb.Append("WHEN 2 THEN 'Final Evaluation' WHEN 3 THEN 'Result Approval' ");
            sb.Append("ELSE 'Not Sure' END sbm_typ_ds ");
            sb.Append("FROM public.pmsrvwsbms s ");
            sb.Append("INNER JOIN public.pmsrvwhdrs h ON h.rvw_hdr_id = s.rvw_hdr_id ");
            sb.Append("INNER JOIN public.pmsaprvrls a ON a.aprv_rl_id = s.apvr_rl_id ");
            sb.Append("INNER JOIN public.gst_prsns e ON e.id = s.frm_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns f ON f.id = s.to_emp_id ");
            sb.Append("WHERE (s.to_emp_id = @to_emp_id) AND (h.rvw_sxn_id = @rvw_sxn_id) ");
            sb.Append("AND (s.is_del = false) ORDER BY s.sbm_dt DESC;");

            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var to_emp_id = cmd.Parameters.Add("@to_emp_id", NpgsqlDbType.Text);
                var rvw_sxn_id = cmd.Parameters.Add("@rvw_sxn_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                to_emp_id.Value = reviewerId;
                rvw_sxn_id.Value = reviewSessionId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    reviewSubmissionList.Add(new ReviewSubmission()
                    {
                        ReviewSubmissionId = reader["rvw_sbm_id"] == DBNull.Value ? 0 : (int)reader["rvw_sbm_id"],
                        ReviewHeaderId = reader["rvw_hdr_id"] == DBNull.Value ? 0 : (int)reader["rvw_hdr_id"],
                        ReviewSessionId = reader["rvw_sxn_id"] == DBNull.Value ? 0 : (int)reader["rvw_sxn_id"],
                        FromEmployeeId = reader["frm_emp_id"] == DBNull.Value ? string.Empty : reader["frm_emp_id"].ToString(),
                        FromEmployeeName = reader["frm_emp_nm"] == DBNull.Value ? string.Empty : reader["frm_emp_nm"].ToString(),
                        ToEmployeeId = reader["to_emp_id"] == DBNull.Value ? string.Empty : reader["to_emp_id"].ToString(),
                        ToEmployeeName = reader["to_emp_nm"] == DBNull.Value ? string.Empty : reader["to_emp_nm"].ToString(),
                        ToEmployeeRoleId = reader["apvr_rl_id"] == DBNull.Value ? 0 : (int)reader["apvr_rl_id"],
                        ToEmployeeRoleName = reader["aprv_rl_nm"] == DBNull.Value ? string.Empty : reader["aprv_rl_nm"].ToString(),
                        SubmissionPurposeId = reader["sbm_typ_id"] == DBNull.Value ? 0 : (int)reader["sbm_typ_id"],
                        SubmissionPurposeDescription = reader["sbm_typ_ds"] == DBNull.Value ? string.Empty : reader["sbm_typ_ds"].ToString(),
                        TimeSubmitted = reader["sbm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["sbm_dt"],
                        SubmissionMessage = reader["sbm_msg"] == DBNull.Value ? string.Empty : reader["sbm_msg"].ToString(),
                        IsActioned = reader["is_xtn"] == DBNull.Value ? false : (bool)reader["is_xtn"],
                        TimeActioned = reader["dt_xtn"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dt_xtn"],
                    });
                }
            }
            await conn.CloseAsync();
            return reviewSubmissionList;
        }
        public async Task<List<ReviewSubmission>> GetByReviewHeaderIdAsync(int reviewHeaderId)
        {
            List<ReviewSubmission> reviewSubmissionList = new List<ReviewSubmission>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.rvw_sbm_id, s.rvw_hdr_id, s.frm_emp_id, s.to_emp_id, ");
            sb.Append("s.sbm_typ_id, s.sbm_dt, s.sbm_msg, s.is_xtn, s.dt_xtn, ");
            sb.Append("e.fullname AS frm_emp_nm, f.fullname AS to_emp_nm, h.rvw_sxn_id, ");
            sb.Append("h.rvw_stg_id, s.apvr_rl_id, a.aprv_rl_nm, ");
            sb.Append("CASE sbm_typ_id WHEN 1 THEN 'Performance Contract Approval' ");
            sb.Append("WHEN 2 THEN 'Final Evaluation' WHEN 3 THEN 'Result Approval' ");
            sb.Append("ELSE 'Not Sure' END sbm_typ_ds ");
            sb.Append("FROM public.pmsrvwsbms s ");
            sb.Append("INNER JOIN public.pmsrvwhdrs h ON h.rvw_hdr_id = s.rvw_hdr_id ");
            sb.Append("INNER JOIN public.pmsaprvrls a ON a.aprv_rl_id = s.apvr_rl_id ");
            sb.Append("INNER JOIN public.gst_prsns e ON e.id = s.frm_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns f ON f.id = s.to_emp_id ");
            sb.Append("WHERE (s.rvw_hdr_id = @rvw_hdr_id) ORDER BY s.sbm_dt DESC; ");
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
                    reviewSubmissionList.Add(new ReviewSubmission()
                    {
                        ReviewSubmissionId = reader["rvw_sbm_id"] == DBNull.Value ? 0 : (int)reader["rvw_sbm_id"],
                        ReviewHeaderId = reader["rvw_hdr_id"] == DBNull.Value ? 0 : (int)reader["rvw_hdr_id"],
                        ReviewSessionId = reader["rvw_sxn_id"] == DBNull.Value ? 0 : (int)reader["rvw_sxn_id"],
                        FromEmployeeId = reader["frm_emp_id"] == DBNull.Value ? string.Empty : reader["frm_emp_id"].ToString(),
                        FromEmployeeName = reader["frm_emp_nm"] == DBNull.Value ? string.Empty : reader["frm_emp_nm"].ToString(),
                        ToEmployeeId = reader["to_emp_id"] == DBNull.Value ? string.Empty : reader["to_emp_id"].ToString(),
                        ToEmployeeName = reader["to_emp_nm"] == DBNull.Value ? string.Empty : reader["to_emp_nm"].ToString(),
                        ToEmployeeRoleId = reader["apvr_rl_id"] == DBNull.Value ? 0 : (int)reader["apvr_rl_id"],
                        ToEmployeeRoleName = reader["aprv_rl_nm"] == DBNull.Value ? string.Empty : reader["aprv_rl_nm"].ToString(),
                        SubmissionPurposeId = reader["sbm_typ_id"] == DBNull.Value ? 0 : (int)reader["sbm_typ_id"],
                        SubmissionPurposeDescription = reader["sbm_typ_ds"] == DBNull.Value ? string.Empty : reader["sbm_typ_ds"].ToString(),
                        TimeSubmitted = reader["sbm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["sbm_dt"],
                        SubmissionMessage = reader["sbm_msg"] == DBNull.Value ? string.Empty : reader["sbm_msg"].ToString(),
                        IsActioned = reader["is_xtn"] == DBNull.Value ? false : (bool)reader["is_xtn"],
                        TimeActioned = reader["dt_xtn"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dt_xtn"],
                    });
                }
            }
            await conn.CloseAsync();
            return reviewSubmissionList;
        }
        public async Task<List<ReviewSubmission>> GetByReviewHeaderIdAndSubmissionPurposeIdAsync(int reviewHeaderId, int submissionPurposeId)
        {
            List<ReviewSubmission> reviewSubmissionList = new List<ReviewSubmission>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.rvw_sbm_id, s.rvw_hdr_id, s.frm_emp_id, s.to_emp_id, ");
            sb.Append("s.sbm_typ_id, s.sbm_dt, s.sbm_msg, s.is_xtn, s.dt_xtn, ");
            sb.Append("e.fullname AS frm_emp_nm, f.fullname AS to_emp_nm, h.rvw_sxn_id, ");
            sb.Append("h.rvw_stg_id, s.apvr_rl_id, a.aprv_rl_nm, ");
            sb.Append("CASE sbm_typ_id WHEN 1 THEN 'Performance Contract Approval' ");
            sb.Append("WHEN 2 THEN 'Final Evaluation' WHEN 3 THEN 'Result Approval' ");
            sb.Append("ELSE 'Not Sure' END sbm_typ_ds ");
            sb.Append("FROM public.pmsrvwsbms s ");
            sb.Append("INNER JOIN public.pmsrvwhdrs h ON h.rvw_hdr_id = s.rvw_hdr_id ");
            sb.Append("INNER JOIN public.pmsaprvrls a ON a.aprv_rl_id = s.apvr_rl_id ");
            sb.Append("INNER JOIN public.gst_prsns e ON e.id = s.frm_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns f ON f.id = s.to_emp_id ");
            sb.Append("WHERE (s.rvw_hdr_id = @rvw_hdr_id) ");
            sb.Append("AND (s.sbm_typ_id = @sbm_typ_id) ");
            sb.Append("ORDER BY s.sbm_dt DESC;");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_hdr_id = cmd.Parameters.Add("@rvw_hdr_id", NpgsqlDbType.Integer);
                var sbm_typ_id = cmd.Parameters.Add("@sbm_typ_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                rvw_hdr_id.Value = reviewHeaderId;
                sbm_typ_id.Value = submissionPurposeId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    reviewSubmissionList.Add(new ReviewSubmission()
                    {
                        ReviewSubmissionId = reader["rvw_sbm_id"] == DBNull.Value ? 0 : (int)reader["rvw_sbm_id"],
                        ReviewHeaderId = reader["rvw_hdr_id"] == DBNull.Value ? 0 : (int)reader["rvw_hdr_id"],
                        ReviewSessionId = reader["rvw_sxn_id"] == DBNull.Value ? 0 : (int)reader["rvw_sxn_id"],
                        FromEmployeeId = reader["frm_emp_id"] == DBNull.Value ? string.Empty : reader["frm_emp_id"].ToString(),
                        FromEmployeeName = reader["frm_emp_nm"] == DBNull.Value ? string.Empty : reader["frm_emp_nm"].ToString(),
                        ToEmployeeId = reader["to_emp_id"] == DBNull.Value ? string.Empty : reader["to_emp_id"].ToString(),
                        ToEmployeeName = reader["to_emp_nm"] == DBNull.Value ? string.Empty : reader["to_emp_nm"].ToString(),
                        ToEmployeeRoleId = reader["apvr_rl_id"] == DBNull.Value ? 0 : (int)reader["apvr_rl_id"],
                        ToEmployeeRoleName = reader["aprv_rl_nm"] == DBNull.Value ? string.Empty : reader["aprv_rl_nm"].ToString(),
                        SubmissionPurposeId = reader["sbm_typ_id"] == DBNull.Value ? 0 : (int)reader["sbm_typ_id"],
                        SubmissionPurposeDescription = reader["sbm_typ_ds"] == DBNull.Value ? string.Empty : reader["sbm_typ_ds"].ToString(),
                        TimeSubmitted = reader["sbm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["sbm_dt"],
                        SubmissionMessage = reader["sbm_msg"] == DBNull.Value ? string.Empty : reader["sbm_msg"].ToString(),
                        IsActioned = reader["is_xtn"] == DBNull.Value ? false : (bool)reader["is_xtn"],
                        TimeActioned = reader["dt_xtn"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dt_xtn"],
                    });
                }
            }
            await conn.CloseAsync();
            return reviewSubmissionList;
        }
        public async Task<List<ReviewSubmission>> GetByReviewHeaderIdAndSubmissionPurposeIdAsync(int reviewHeaderId, int submissionPurposeId, string appraiserId)
        {
            List<ReviewSubmission> reviewSubmissionList = new List<ReviewSubmission>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.rvw_sbm_id, s.rvw_hdr_id, s.frm_emp_id, s.to_emp_id, ");
            sb.Append("s.sbm_typ_id, s.sbm_dt, s.sbm_msg, s.is_xtn, s.dt_xtn, ");
            sb.Append("e.fullname AS frm_emp_nm, f.fullname AS to_emp_nm, h.rvw_sxn_id, ");
            sb.Append("h.rvw_stg_id, s.apvr_rl_id, a.aprv_rl_nm, ");
            sb.Append("CASE sbm_typ_id WHEN 1 THEN 'Performance Contract Approval' ");
            sb.Append("WHEN 2 THEN 'Final Evaluation' WHEN 3 THEN 'Result Approval' ");
            sb.Append("ELSE 'Not Sure' END sbm_typ_ds ");
            sb.Append("FROM public.pmsrvwsbms s ");
            sb.Append("INNER JOIN public.pmsrvwhdrs h ON h.rvw_hdr_id = s.rvw_hdr_id ");
            sb.Append("INNER JOIN public.pmsaprvrls a ON a.aprv_rl_id = s.apvr_rl_id ");
            sb.Append("INNER JOIN public.gst_prsns e ON e.id = s.frm_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns f ON f.id = s.to_emp_id ");
            sb.Append("WHERE (s.rvw_hdr_id = @rvw_hdr_id) ");
            sb.Append("AND (s.sbm_typ_id = @sbm_typ_id) ");
            sb.Append("AND (s.to_emp_id = @to_emp_id) ");
            sb.Append("ORDER BY s.sbm_dt DESC;");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_hdr_id = cmd.Parameters.Add("@rvw_hdr_id", NpgsqlDbType.Integer);
                var sbm_typ_id = cmd.Parameters.Add("@sbm_typ_id", NpgsqlDbType.Integer);
                var to_emp_id = cmd.Parameters.Add("@to_emp_id", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                rvw_hdr_id.Value = reviewHeaderId;
                sbm_typ_id.Value = submissionPurposeId;
                to_emp_id.Value = appraiserId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    reviewSubmissionList.Add(new ReviewSubmission()
                    {
                        ReviewSubmissionId = reader["rvw_sbm_id"] == DBNull.Value ? 0 : (int)reader["rvw_sbm_id"],
                        ReviewHeaderId = reader["rvw_hdr_id"] == DBNull.Value ? 0 : (int)reader["rvw_hdr_id"],
                        ReviewSessionId = reader["rvw_sxn_id"] == DBNull.Value ? 0 : (int)reader["rvw_sxn_id"],
                        FromEmployeeId = reader["frm_emp_id"] == DBNull.Value ? string.Empty : reader["frm_emp_id"].ToString(),
                        FromEmployeeName = reader["frm_emp_nm"] == DBNull.Value ? string.Empty : reader["frm_emp_nm"].ToString(),
                        ToEmployeeId = reader["to_emp_id"] == DBNull.Value ? string.Empty : reader["to_emp_id"].ToString(),
                        ToEmployeeName = reader["to_emp_nm"] == DBNull.Value ? string.Empty : reader["to_emp_nm"].ToString(),
                        ToEmployeeRoleId = reader["apvr_rl_id"] == DBNull.Value ? 0 : (int)reader["apvr_rl_id"],
                        ToEmployeeRoleName = reader["aprv_rl_nm"] == DBNull.Value ? string.Empty : reader["aprv_rl_nm"].ToString(),
                        SubmissionPurposeId = reader["sbm_typ_id"] == DBNull.Value ? 0 : (int)reader["sbm_typ_id"],
                        SubmissionPurposeDescription = reader["sbm_typ_ds"] == DBNull.Value ? string.Empty : reader["sbm_typ_ds"].ToString(),
                        TimeSubmitted = reader["sbm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["sbm_dt"],
                        SubmissionMessage = reader["sbm_msg"] == DBNull.Value ? string.Empty : reader["sbm_msg"].ToString(),
                        IsActioned = reader["is_xtn"] == DBNull.Value ? false : (bool)reader["is_xtn"],
                        TimeActioned = reader["dt_xtn"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dt_xtn"],
                    });
                }
            }
            await conn.CloseAsync();
            return reviewSubmissionList;
        }
        public async Task<List<ReviewSubmission>> GetByIdAsync(int reviewSubmissionId)
        {
            List<ReviewSubmission> reviewSubmissionList = new List<ReviewSubmission>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.rvw_sbm_id, s.rvw_hdr_id, s.frm_emp_id, s.to_emp_id, ");
            sb.Append("s.sbm_typ_id, s.sbm_dt, s.sbm_msg, s.is_xtn, s.dt_xtn, ");
            sb.Append("e.fullname AS frm_emp_nm, f.fullname AS to_emp_nm, h.rvw_sxn_id, ");
            sb.Append("h.rvw_stg_id, s.apvr_rl_id, a.aprv_rl_nm, ");
            sb.Append("CASE sbm_typ_id WHEN 1 THEN 'Performance Contract Approval' ");
            sb.Append("WHEN 2 THEN 'Final Evaluation' WHEN 3 THEN 'Result Approval' ");
            sb.Append("ELSE 'Not Sure' END sbm_typ_ds ");
            sb.Append("FROM public.pmsrvwsbms s ");
            sb.Append("INNER JOIN public.pmsrvwhdrs h ON h.rvw_hdr_id = s.rvw_hdr_id ");
            sb.Append("INNER JOIN public.pmsaprvrls a ON a.aprv_rl_id = s.apvr_rl_id ");
            sb.Append("INNER JOIN public.gst_prsns e ON e.id = s.frm_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns f ON f.id = s.to_emp_id ");
            sb.Append("WHERE (s.rvw_sbm_id = @rvw_sbm_id); ");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_sbm_id = cmd.Parameters.Add("@rvw_sbm_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                rvw_sbm_id.Value = reviewSubmissionId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    reviewSubmissionList.Add(new ReviewSubmission()
                    {
                        ReviewSubmissionId = reader["rvw_sbm_id"] == DBNull.Value ? 0 : (int)reader["rvw_sbm_id"],
                        ReviewHeaderId = reader["rvw_hdr_id"] == DBNull.Value ? 0 : (int)reader["rvw_hdr_id"],
                        ReviewSessionId = reader["rvw_sxn_id"] == DBNull.Value ? 0 : (int)reader["rvw_sxn_id"],
                        FromEmployeeId = reader["frm_emp_id"] == DBNull.Value ? string.Empty : reader["frm_emp_id"].ToString(),
                        FromEmployeeName = reader["frm_emp_nm"] == DBNull.Value ? string.Empty : reader["frm_emp_nm"].ToString(),
                        ToEmployeeId = reader["to_emp_id"] == DBNull.Value ? string.Empty : reader["to_emp_id"].ToString(),
                        ToEmployeeName = reader["to_emp_nm"] == DBNull.Value ? string.Empty : reader["to_emp_nm"].ToString(),
                        ToEmployeeRoleId = reader["apvr_rl_id"] == DBNull.Value ? 0 : (int)reader["apvr_rl_id"],
                        ToEmployeeRoleName = reader["aprv_rl_nm"] == DBNull.Value ? string.Empty : reader["aprv_rl_nm"].ToString(),
                        SubmissionPurposeId = reader["sbm_typ_id"] == DBNull.Value ? 0 : (int)reader["sbm_typ_id"],
                        SubmissionPurposeDescription = reader["sbm_typ_ds"] == DBNull.Value ? string.Empty : reader["sbm_typ_ds"].ToString(),
                        TimeSubmitted = reader["sbm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["sbm_dt"],
                        SubmissionMessage = reader["sbm_msg"] == DBNull.Value ? string.Empty : reader["sbm_msg"].ToString(),
                        IsActioned = reader["is_xtn"] == DBNull.Value ? false : (bool)reader["is_xtn"],
                        TimeActioned = reader["dt_xtn"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dt_xtn"],
                    });
                }
            }
            await conn.CloseAsync();
            return reviewSubmissionList;
        }
        #endregion

        #region Review Submission Write Action Methods
        public async Task<bool> AddAsync(ReviewSubmission reviewSubmission)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.pmsrvwsbms(rvw_hdr_id, frm_emp_id, ");
            sb.Append("to_emp_id, sbm_typ_id, sbm_dt, sbm_msg, is_xtn, dt_xtn, ");
            sb.Append("apvr_rl_id) VALUES (@rvw_hdr_id, @frm_emp_id, @to_emp_id, ");
            sb.Append("@sbm_typ_id, @sbm_dt, @sbm_msg, @is_xtn, @dt_xtn, @apvr_rl_id);");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var rvw_hdr_id = cmd.Parameters.Add("@rvw_hdr_id", NpgsqlDbType.Integer);
                    var frm_emp_id = cmd.Parameters.Add("@frm_emp_id", NpgsqlDbType.Text);
                    var to_emp_id = cmd.Parameters.Add("@to_emp_id", NpgsqlDbType.Text);
                    var sbm_typ_id = cmd.Parameters.Add("@sbm_typ_id", NpgsqlDbType.Integer);
                    var sbm_dt = cmd.Parameters.Add("@sbm_dt", NpgsqlDbType.TimestampTz);
                    var sbm_msg = cmd.Parameters.Add("@sbm_msg", NpgsqlDbType.Text);
                    var is_xtn = cmd.Parameters.Add("@is_xtn", NpgsqlDbType.Boolean);
                    var dt_xtn = cmd.Parameters.Add("@dt_xtn", NpgsqlDbType.TimestampTz);
                    var apvr_rl_id = cmd.Parameters.Add("@apvr_rl_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    rvw_hdr_id.Value = reviewSubmission.ReviewHeaderId;
                    frm_emp_id.Value = reviewSubmission.FromEmployeeId;
                    to_emp_id.Value = reviewSubmission.ToEmployeeId;
                    sbm_typ_id.Value = reviewSubmission.SubmissionPurposeId;
                    sbm_dt.Value = reviewSubmission.TimeSubmitted ?? (object)DBNull.Value;
                    sbm_msg.Value = reviewSubmission.SubmissionMessage ?? (object)DBNull.Value;
                    is_xtn.Value = reviewSubmission.IsActioned;
                    dt_xtn.Value = reviewSubmission.TimeActioned ?? (object)DBNull.Value;
                    apvr_rl_id.Value = reviewSubmission.ToEmployeeRoleId;

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

        public async Task<bool> UpdateAsync(int reviewSubmissionId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.pmsrvwsbms SET is_xtn=true, dt_xtn=@dt_xtn ");
            sb.Append("WHERE (rvw_sbm_id=@rvw_sbm_id); ");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var rvw_sbm_id = cmd.Parameters.Add("@rvw_sbm_id", NpgsqlDbType.Integer);
                    var is_xtn = cmd.Parameters.Add("@is_xtn", NpgsqlDbType.Boolean);
                    var dt_xtn = cmd.Parameters.Add("@dt_xtn", NpgsqlDbType.TimestampTz);

                    cmd.Prepare();
                    rvw_sbm_id.Value = reviewSubmissionId;
                    is_xtn.Value = true;
                    dt_xtn.Value = DateTime.UtcNow;

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

        public async Task<bool> UpdateAsync(int reviewHeaderId, string toEmployeeId, int submissionPurposeId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.pmsrvwsbms SET is_xtn=true, dt_xtn=@dt_xtn ");
            sb.Append("WHERE (rvw_hdr_id=@rvw_hdr_id) ");
            sb.Append("AND (to_emp_id = @to_emp_id) ");
            sb.Append("AND (sbm_typ_id = @sbm_typ_id);");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var rvw_hdr_id = cmd.Parameters.Add("@rvw_hdr_id", NpgsqlDbType.Integer);
                    var to_emp_id = cmd.Parameters.Add("@to_emp_id", NpgsqlDbType.Text);
                    var sbm_typ_id = cmd.Parameters.Add("@sbm_typ_id", NpgsqlDbType.Integer);

                    var is_xtn = cmd.Parameters.Add("@is_xtn", NpgsqlDbType.Boolean);
                    var dt_xtn = cmd.Parameters.Add("@dt_xtn", NpgsqlDbType.TimestampTz);

                    cmd.Prepare();
                    rvw_hdr_id.Value = reviewHeaderId;
                    to_emp_id.Value = toEmployeeId;
                    sbm_typ_id.Value = submissionPurposeId;
                    is_xtn.Value = true;
                    dt_xtn.Value = DateTime.UtcNow;

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

        public async Task<bool> DeleteAsync(int reviewSubmissionId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.pmsrvwsbms SET is_del=true, dt_del=@dt_del ");
            sb.Append("WHERE (rvw_sbm_id=@rvw_sbm_id) ");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var rvw_sbm_id = cmd.Parameters.Add("@rvw_sbm_id", NpgsqlDbType.Integer);
                    var dt_del = cmd.Parameters.Add("@dt_del", NpgsqlDbType.TimestampTz);
                    cmd.Prepare();
                    rvw_sbm_id.Value = reviewSubmissionId;
                    dt_del.Value = DateTime.UtcNow;

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
        #endregion

    }
}
