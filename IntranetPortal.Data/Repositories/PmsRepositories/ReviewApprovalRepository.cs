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
    public class ReviewApprovalRepository : IReviewApprovalRepository
    {
        public IConfiguration _config { get; }
        public ReviewApprovalRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        #region Review Approvals Read Action Methods
        public async Task<IList<ReviewApproval>> GetAllAsync()
        {
            List<ReviewApproval> reviewApprovalsList = new List<ReviewApproval>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT a.rvw_aprv_id, a.aprv_typ_id, a.rvw_hdr_id, ");
            sb.Append("a.aprv_emp_id, a.rvw_emp_id, a.is_aprvd, a.rvw_aprv_dt, ");
            sb.Append("a.rvw_aprv_rmk, a.aprv_rl_id, a.rvw_mtrc_id, r.aprv_rl_nm, ");
            sb.Append("r.mst_aprv_con, r.mst_aprv_eva, e.fullname as rvw_emp_nm, ");
            sb.Append("f.fullname as aprv_emp_nm, t.aprv_typ_nm ");
            sb.Append("FROM public.pmsrvwaprvs a ");
            sb.Append("LEFT JOIN public.pmsaprvrls r ON r.aprv_rl_id = a.aprv_rl_id ");
            sb.Append("LEFT JOIN public.gst_prsns e ON e.id = a.rvw_emp_id ");
            sb.Append("LEFT JOIN public.gst_prsns f ON f.id = a.aprv_emp_id ");
            sb.Append("LEFT JOIN public.pmsaprvtyps t ON t.aprv_typ_id = a.aprv_typ_id ");
            sb.Append("ORDER BY a.rvw_aprv_id DESC;");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                await cmd.PrepareAsync();
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    reviewApprovalsList.Add(new ReviewApproval()
                    {
                        ReviewApprovalId = reader["rvw_aprv_id"] == DBNull.Value ? 0 : (int)(reader["rvw_aprv_id"]),
                        ApprovalTypeId = reader["aprv_typ_id"] == DBNull.Value ? 0 : (int)reader["aprv_typ_id"],
                        ApprovalTypeDescription = reader["aprv_typ_nm"] == DBNull.Value ? string.Empty : (reader["aprv_typ_nm"]).ToString(),
                        ReviewHeaderId = reader["rvw_hdr_id"] == DBNull.Value ? 0 : (int)reader["rvw_hdr_id"],
                        ApproverId = reader["aprv_emp_id"] == DBNull.Value ? string.Empty : reader["aprv_emp_id"].ToString(),
                        ApproverName = reader["aprv_emp_nm"] == DBNull.Value ? string.Empty : reader["aprv_emp_nm"].ToString(),
                        AppraiseeId = reader["rvw_emp_id"] == DBNull.Value ? string.Empty : reader["rvw_emp_id"].ToString(),
                        AppraiseeName = reader["rvw_emp_nm"] == DBNull.Value ? string.Empty : reader["rvw_emp_id"].ToString(),
                        IsApproved = reader["is_aprvd"] == DBNull.Value ? false : (bool)reader["is_aprvd"],
                        ApprovedTime = reader["rvw_aprv_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rvw_aprv_dt"],
                        ApprovedComments = reader["rvw_aprv_rmk"] == DBNull.Value ? string.Empty : reader["rvw_aprv_rmk"].ToString(),
                        ApproverRoleId = reader["aprv_rl_id"] == DBNull.Value ? 0 : (int)reader["aprv_rl_id"],
                        ApproverRoleDescription = reader["aprv_rl_nm"] == DBNull.Value ? string.Empty : reader["aprv_rl_nm"].ToString(),
                        ReviewMetricId = reader["rvw_mtrc_id"] == DBNull.Value ? 0 : (int)reader["rvw_mtrc_id"],
                        MustApproveContract = reader["mst_aprv_con"] == DBNull.Value ? false : (bool)reader["mst_aprv_con"],
                        MustApproveEvaluation = reader["mst_aprv_eva"] == DBNull.Value ? false : (bool)reader["mst_aprv_eva"],
                    });
                }
            }
            await conn.CloseAsync();
            return reviewApprovalsList;
        }

        public async Task<IList<ReviewApproval>> GetByIdAsync(int reviewApprovalId)
        {
            List<ReviewApproval> reviewApprovalsList = new List<ReviewApproval>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT a.rvw_aprv_id, a.aprv_typ_id, a.rvw_hdr_id, ");
            sb.Append("a.aprv_emp_id, a.rvw_emp_id, a.is_aprvd, a.rvw_aprv_dt, ");
            sb.Append("a.rvw_aprv_rmk, a.aprv_rl_id, a.rvw_mtrc_id, r.aprv_rl_nm, ");
            sb.Append("r.mst_aprv_con, r.mst_aprv_eva, e.fullname as rvw_emp_nm, ");
            sb.Append("f.fullname as aprv_emp_nm, t.aprv_typ_nm ");
            sb.Append("FROM public.pmsrvwaprvs a ");
            sb.Append("LEFT JOIN public.pmsaprvrls r ON r.aprv_rl_id = a.aprv_rl_id ");
            sb.Append("LEFT JOIN public.gst_prsns e ON e.id = a.rvw_emp_id ");
            sb.Append("LEFT JOIN public.gst_prsns f ON f.id = a.aprv_emp_id ");
            sb.Append("LEFT JOIN public.pmsaprvtyps t ON t.aprv_typ_id = a.aprv_typ_id ");
            sb.Append("WHERE (a.rvw_aprv_id = @rvw_aprv_id);");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_aprv_id = cmd.Parameters.Add("@rvw_aprv_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                rvw_aprv_id.Value = reviewApprovalId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    reviewApprovalsList.Add(new ReviewApproval()
                    {
                        ReviewApprovalId = reader["rvw_aprv_id"] == DBNull.Value ? 0 : (int)(reader["rvw_aprv_id"]),
                        ApprovalTypeId = reader["aprv_typ_id"] == DBNull.Value ? 0 : (int)reader["aprv_typ_id"],
                        ApprovalTypeDescription = reader["aprv_typ_nm"] == DBNull.Value ? string.Empty : (reader["aprv_typ_nm"]).ToString(),
                        ReviewHeaderId = reader["rvw_hdr_id"] == DBNull.Value ? 0 : (int)reader["rvw_hdr_id"],
                        ApproverId = reader["aprv_emp_id"] == DBNull.Value ? string.Empty : reader["aprv_emp_id"].ToString(),
                        ApproverName = reader["aprv_emp_nm"] == DBNull.Value ? string.Empty : reader["aprv_emp_nm"].ToString(),
                        AppraiseeId = reader["rvw_emp_id"] == DBNull.Value ? string.Empty : reader["rvw_emp_id"].ToString(),
                        AppraiseeName = reader["rvw_emp_nm"] == DBNull.Value ? string.Empty : reader["rvw_emp_id"].ToString(),
                        IsApproved = reader["is_aprvd"] == DBNull.Value ? false : (bool)reader["is_aprvd"],
                        ApprovedTime = reader["rvw_aprv_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rvw_aprv_dt"],
                        ApprovedComments = reader["rvw_aprv_rmk"] == DBNull.Value ? string.Empty : reader["rvw_aprv_rmk"].ToString(),
                        ApproverRoleId = reader["aprv_rl_id"] == DBNull.Value ? 0 : (int)reader["aprv_rl_id"],
                        ApproverRoleDescription = reader["aprv_rl_nm"] == DBNull.Value ? string.Empty : reader["aprv_rl_nm"].ToString(),
                        ReviewMetricId = reader["rvw_mtrc_id"] == DBNull.Value ? 0 : (int)reader["rvw_mtrc_id"],
                        MustApproveContract = reader["mst_aprv_con"] == DBNull.Value ? false : (bool)reader["mst_aprv_con"],
                        MustApproveEvaluation = reader["mst_aprv_eva"] == DBNull.Value ? false : (bool)reader["mst_aprv_eva"],
                    });
                }
            }
            await conn.CloseAsync();
            return reviewApprovalsList;
        }

        //=== Get All Approvals ====//
        public async Task<IList<ReviewApproval>> GetByReviewHeaderIdAsync(int reviewHeaderId)
        {
            List<ReviewApproval> reviewApprovalsList = new List<ReviewApproval>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT a.rvw_aprv_id, a.aprv_typ_id, a.rvw_hdr_id, ");
            sb.Append("a.aprv_emp_id, a.rvw_emp_id, a.is_aprvd, a.rvw_aprv_dt, ");
            sb.Append("a.rvw_aprv_rmk, a.aprv_rl_id, a.rvw_mtrc_id, r.aprv_rl_nm, ");
            sb.Append("r.mst_aprv_con, r.mst_aprv_eva, e.fullname as rvw_emp_nm, ");
            sb.Append("f.fullname as aprv_emp_nm, t.aprv_typ_nm ");
            sb.Append("FROM public.pmsrvwaprvs a ");
            sb.Append("LEFT JOIN public.pmsaprvrls r ON r.aprv_rl_id = a.aprv_rl_id ");
            sb.Append("LEFT JOIN public.gst_prsns e ON e.id = a.rvw_emp_id ");
            sb.Append("LEFT JOIN public.gst_prsns f ON f.id = a.aprv_emp_id ");
            sb.Append("LEFT JOIN public.pmsaprvtyps t ON t.aprv_typ_id = a.aprv_typ_id ");
            sb.Append("WHERE (a.rvw_hdr_id = @rvw_hdr_id) ");
            sb.Append("ORDER BY a.rvw_aprv_id DESC;");
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
                    reviewApprovalsList.Add(new ReviewApproval()
                    {
                        ReviewApprovalId = reader["rvw_aprv_id"] == DBNull.Value ? 0 : (int)(reader["rvw_aprv_id"]),
                        ApprovalTypeId = reader["aprv_typ_id"] == DBNull.Value ? 0 : (int)reader["aprv_typ_id"],
                        ApprovalTypeDescription = reader["aprv_typ_nm"] == DBNull.Value ? string.Empty : (reader["aprv_typ_nm"]).ToString(),
                        ReviewHeaderId = reader["rvw_hdr_id"] == DBNull.Value ? 0 : (int)reader["rvw_hdr_id"],
                        ApproverId = reader["aprv_emp_id"] == DBNull.Value ? string.Empty : reader["aprv_emp_id"].ToString(),
                        ApproverName = reader["aprv_emp_nm"] == DBNull.Value ? string.Empty : reader["aprv_emp_nm"].ToString(),
                        AppraiseeId = reader["rvw_emp_id"] == DBNull.Value ? string.Empty : reader["rvw_emp_id"].ToString(),
                        AppraiseeName = reader["rvw_emp_nm"] == DBNull.Value ? string.Empty : reader["rvw_emp_id"].ToString(),
                        IsApproved = reader["is_aprvd"] == DBNull.Value ? false : (bool)reader["is_aprvd"],
                        ApprovedTime = reader["rvw_aprv_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rvw_aprv_dt"],
                        ApprovedComments = reader["rvw_aprv_rmk"] == DBNull.Value ? string.Empty : reader["rvw_aprv_rmk"].ToString(),
                        ApproverRoleId = reader["aprv_rl_id"] == DBNull.Value ? 0 : (int)reader["aprv_rl_id"],
                        ApproverRoleDescription = reader["aprv_rl_nm"] == DBNull.Value ? string.Empty : reader["aprv_rl_nm"].ToString(),
                        ReviewMetricId = reader["rvw_mtrc_id"] == DBNull.Value ? 0 : (int)reader["rvw_mtrc_id"],
                        MustApproveContract = reader["mst_aprv_con"] == DBNull.Value ? false : (bool)reader["mst_aprv_con"],
                        MustApproveEvaluation = reader["mst_aprv_eva"] == DBNull.Value ? false : (bool)reader["mst_aprv_eva"],
                    });
                }
            }
            await conn.CloseAsync();
            return reviewApprovalsList;
        }

        public async Task<IList<ReviewApproval>> GetByReviewHeaderIdAsync(int reviewHeaderId, int approvalTypeId)
        {
            List<ReviewApproval> reviewApprovalsList = new List<ReviewApproval>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT a.rvw_aprv_id, a.aprv_typ_id, a.rvw_hdr_id, ");
            sb.Append("a.aprv_emp_id, a.rvw_emp_id, a.is_aprvd, a.rvw_aprv_dt, ");
            sb.Append("a.rvw_aprv_rmk, a.aprv_rl_id, a.rvw_mtrc_id, r.aprv_rl_nm, ");
            sb.Append("r.mst_aprv_con, r.mst_aprv_eva, e.fullname as rvw_emp_nm, ");
            sb.Append("f.fullname as aprv_emp_nm, t.aprv_typ_nm ");
            sb.Append("FROM public.pmsrvwaprvs a ");
            sb.Append("LEFT JOIN public.pmsaprvrls r ON r.aprv_rl_id = a.aprv_rl_id ");
            sb.Append("LEFT JOIN public.gst_prsns e ON e.id = a.rvw_emp_id ");
            sb.Append("LEFT JOIN public.gst_prsns f ON f.id = a.aprv_emp_id ");
            sb.Append("LEFT JOIN public.pmsaprvtyps t ON t.aprv_typ_id = a.aprv_typ_id ");
            sb.Append("WHERE (a.rvw_hdr_id = @rvw_hdr_id) ");
            sb.Append("AND (a.aprv_typ_id = @aprv_typ_id) ");
            sb.Append("ORDER BY a.rvw_aprv_id DESC;");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_hdr_id = cmd.Parameters.Add("@rvw_hdr_id", NpgsqlDbType.Integer);
                var aprv_typ_id = cmd.Parameters.Add("@aprv_typ_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                rvw_hdr_id.Value = reviewHeaderId;
                aprv_typ_id.Value = approvalTypeId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    reviewApprovalsList.Add(new ReviewApproval()
                    {
                        ReviewApprovalId = reader["rvw_aprv_id"] == DBNull.Value ? 0 : (int)(reader["rvw_aprv_id"]),
                        ApprovalTypeId = reader["aprv_typ_id"] == DBNull.Value ? 0 : (int)reader["aprv_typ_id"],
                        ApprovalTypeDescription = reader["aprv_typ_nm"] == DBNull.Value ? string.Empty : (reader["aprv_typ_nm"]).ToString(),
                        ReviewHeaderId = reader["rvw_hdr_id"] == DBNull.Value ? 0 : (int)reader["rvw_hdr_id"],
                        ApproverId = reader["aprv_emp_id"] == DBNull.Value ? string.Empty : reader["aprv_emp_id"].ToString(),
                        ApproverName = reader["aprv_emp_nm"] == DBNull.Value ? string.Empty : reader["aprv_emp_nm"].ToString(),
                        AppraiseeId = reader["rvw_emp_id"] == DBNull.Value ? string.Empty : reader["rvw_emp_id"].ToString(),
                        AppraiseeName = reader["rvw_emp_nm"] == DBNull.Value ? string.Empty : reader["rvw_emp_id"].ToString(),
                        IsApproved = reader["is_aprvd"] == DBNull.Value ? false : (bool)reader["is_aprvd"],
                        ApprovedTime = reader["rvw_aprv_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rvw_aprv_dt"],
                        ApprovedComments = reader["rvw_aprv_rmk"] == DBNull.Value ? string.Empty : reader["rvw_aprv_rmk"].ToString(),
                        ApproverRoleId = reader["aprv_rl_id"] == DBNull.Value ? 0 : (int)reader["aprv_rl_id"],
                        ApproverRoleDescription = reader["aprv_rl_nm"] == DBNull.Value ? string.Empty : reader["aprv_rl_nm"].ToString(),
                        ReviewMetricId = reader["rvw_mtrc_id"] == DBNull.Value ? 0 : (int)reader["rvw_mtrc_id"],
                        MustApproveContract = reader["mst_aprv_con"] == DBNull.Value ? false : (bool)reader["mst_aprv_con"],
                        MustApproveEvaluation = reader["mst_aprv_eva"] == DBNull.Value ? false : (bool)reader["mst_aprv_eva"],
                    });
                }
            }
            await conn.CloseAsync();
            return reviewApprovalsList;
        }

        public async Task<IList<ReviewApproval>> GetByReviewHeaderIdAsync(int reviewHeaderId, int approvalTypeId, int approverRoleId)
        {
            List<ReviewApproval> reviewApprovalsList = new List<ReviewApproval>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT a.rvw_aprv_id, a.aprv_typ_id, a.rvw_hdr_id, ");
            sb.Append("a.aprv_emp_id, a.rvw_emp_id, a.is_aprvd, a.rvw_aprv_dt, ");
            sb.Append("a.rvw_aprv_rmk, a.aprv_rl_id, a.rvw_mtrc_id, r.aprv_rl_nm, ");
            sb.Append("r.mst_aprv_con, r.mst_aprv_eva, e.fullname as rvw_emp_nm, ");
            sb.Append("f.fullname as aprv_emp_nm, t.aprv_typ_nm ");
            sb.Append("FROM public.pmsrvwaprvs a ");
            sb.Append("LEFT JOIN public.pmsaprvrls r ON r.aprv_rl_id = a.aprv_rl_id ");
            sb.Append("LEFT JOIN public.gst_prsns e ON e.id = a.rvw_emp_id ");
            sb.Append("LEFT JOIN public.gst_prsns f ON f.id = a.aprv_emp_id ");
            sb.Append("LEFT JOIN public.pmsaprvtyps t ON t.aprv_typ_id = a.aprv_typ_id ");
            sb.Append("WHERE (a.rvw_hdr_id = @rvw_hdr_id) ");
            sb.Append("AND (a.aprv_typ_id = @aprv_typ_id) ");
            sb.Append("AND (a.aprv_rl_id = @aprv_rl_id) ");
            sb.Append("ORDER BY a.rvw_aprv_id DESC;");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_hdr_id = cmd.Parameters.Add("@rvw_hdr_id", NpgsqlDbType.Integer);
                var aprv_typ_id = cmd.Parameters.Add("@aprv_typ_id", NpgsqlDbType.Integer);
                var aprv_rl_id = cmd.Parameters.Add("@aprv_rl_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                rvw_hdr_id.Value = reviewHeaderId;
                aprv_typ_id.Value = approvalTypeId;
                aprv_rl_id.Value = approverRoleId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    reviewApprovalsList.Add(new ReviewApproval()
                    {
                        ReviewApprovalId = reader["rvw_aprv_id"] == DBNull.Value ? 0 : (int)(reader["rvw_aprv_id"]),
                        ApprovalTypeId = reader["aprv_typ_id"] == DBNull.Value ? 0 : (int)reader["aprv_typ_id"],
                        ApprovalTypeDescription = reader["aprv_typ_nm"] == DBNull.Value ? string.Empty : (reader["aprv_typ_nm"]).ToString(),
                        ReviewHeaderId = reader["rvw_hdr_id"] == DBNull.Value ? 0 : (int)reader["rvw_hdr_id"],
                        ApproverId = reader["aprv_emp_id"] == DBNull.Value ? string.Empty : reader["aprv_emp_id"].ToString(),
                        ApproverName = reader["aprv_emp_nm"] == DBNull.Value ? string.Empty : reader["aprv_emp_nm"].ToString(),
                        AppraiseeId = reader["rvw_emp_id"] == DBNull.Value ? string.Empty : reader["rvw_emp_id"].ToString(),
                        AppraiseeName = reader["rvw_emp_nm"] == DBNull.Value ? string.Empty : reader["rvw_emp_id"].ToString(),
                        IsApproved = reader["is_aprvd"] == DBNull.Value ? false : (bool)reader["is_aprvd"],
                        ApprovedTime = reader["rvw_aprv_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rvw_aprv_dt"],
                        ApprovedComments = reader["rvw_aprv_rmk"] == DBNull.Value ? string.Empty : reader["rvw_aprv_rmk"].ToString(),
                        ApproverRoleId = reader["aprv_rl_id"] == DBNull.Value ? 0 : (int)reader["aprv_rl_id"],
                        ApproverRoleDescription = reader["aprv_rl_nm"] == DBNull.Value ? string.Empty : reader["aprv_rl_nm"].ToString(),
                        ReviewMetricId = reader["rvw_mtrc_id"] == DBNull.Value ? 0 : (int)reader["rvw_mtrc_id"],
                        MustApproveContract = reader["mst_aprv_con"] == DBNull.Value ? false : (bool)reader["mst_aprv_con"],
                        MustApproveEvaluation = reader["mst_aprv_eva"] == DBNull.Value ? false : (bool)reader["mst_aprv_eva"],
                    });
                }
            }
            await conn.CloseAsync();
            return reviewApprovalsList;
        }


        //======= Get Only Approved =======//
        public async Task<IList<ReviewApproval>> GetApprovedByReviewHeaderIdAsync(int reviewHeaderId)
        {
            List<ReviewApproval> reviewApprovalsList = new List<ReviewApproval>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT a.rvw_aprv_id, a.aprv_typ_id, a.rvw_hdr_id, ");
            sb.Append("a.aprv_emp_id, a.rvw_emp_id, a.is_aprvd, a.rvw_aprv_dt, ");
            sb.Append("a.rvw_aprv_rmk, a.aprv_rl_id, a.rvw_mtrc_id, r.aprv_rl_nm, ");
            sb.Append("r.mst_aprv_con, r.mst_aprv_eva, e.fullname as rvw_emp_nm, ");
            sb.Append("f.fullname as aprv_emp_nm, t.aprv_typ_nm ");
            sb.Append("FROM public.pmsrvwaprvs a ");
            sb.Append("LEFT JOIN public.pmsaprvrls r ON r.aprv_rl_id = a.aprv_rl_id ");
            sb.Append("LEFT JOIN public.gst_prsns e ON e.id = a.rvw_emp_id ");
            sb.Append("LEFT JOIN public.gst_prsns f ON f.id = a.aprv_emp_id ");
            sb.Append("LEFT JOIN public.pmsaprvtyps t ON t.aprv_typ_id = a.aprv_typ_id ");
            sb.Append("WHERE (a.rvw_hdr_id = @rvw_hdr_id) ");
            sb.Append("AND (a.is_aprvd = true) ");
            sb.Append("ORDER BY a.rvw_aprv_id DESC;");
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
                    reviewApprovalsList.Add(new ReviewApproval()
                    {
                        ReviewApprovalId = reader["rvw_aprv_id"] == DBNull.Value ? 0 : (int)(reader["rvw_aprv_id"]),
                        ApprovalTypeId = reader["aprv_typ_id"] == DBNull.Value ? 0 : (int)reader["aprv_typ_id"],
                        ApprovalTypeDescription = reader["aprv_typ_nm"] == DBNull.Value ? string.Empty : (reader["aprv_typ_nm"]).ToString(),
                        ReviewHeaderId = reader["rvw_hdr_id"] == DBNull.Value ? 0 : (int)reader["rvw_hdr_id"],
                        ApproverId = reader["aprv_emp_id"] == DBNull.Value ? string.Empty : reader["aprv_emp_id"].ToString(),
                        ApproverName = reader["aprv_emp_nm"] == DBNull.Value ? string.Empty : reader["aprv_emp_nm"].ToString(),
                        AppraiseeId = reader["rvw_emp_id"] == DBNull.Value ? string.Empty : reader["rvw_emp_id"].ToString(),
                        AppraiseeName = reader["rvw_emp_nm"] == DBNull.Value ? string.Empty : reader["rvw_emp_id"].ToString(),
                        IsApproved = reader["is_aprvd"] == DBNull.Value ? false : (bool)reader["is_aprvd"],
                        ApprovedTime = reader["rvw_aprv_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rvw_aprv_dt"],
                        ApprovedComments = reader["rvw_aprv_rmk"] == DBNull.Value ? string.Empty : reader["rvw_aprv_rmk"].ToString(),
                        ApproverRoleId = reader["aprv_rl_id"] == DBNull.Value ? 0 : (int)reader["aprv_rl_id"],
                        ApproverRoleDescription = reader["aprv_rl_nm"] == DBNull.Value ? string.Empty : reader["aprv_rl_nm"].ToString(),
                        ReviewMetricId = reader["rvw_mtrc_id"] == DBNull.Value ? 0 : (int)reader["rvw_mtrc_id"],
                        MustApproveContract = reader["mst_aprv_con"] == DBNull.Value ? false : (bool)reader["mst_aprv_con"],
                        MustApproveEvaluation = reader["mst_aprv_eva"] == DBNull.Value ? false : (bool)reader["mst_aprv_eva"],
                    });
                }
            }
            await conn.CloseAsync();
            return reviewApprovalsList;
        }

        public async Task<IList<ReviewApproval>> GetApprovedByReviewHeaderIdAsync(int reviewHeaderId, int approvalTypeId)
        {
            List<ReviewApproval> reviewApprovalsList = new List<ReviewApproval>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT a.rvw_aprv_id, a.aprv_typ_id, a.rvw_hdr_id, ");
            sb.Append("a.aprv_emp_id, a.rvw_emp_id, a.is_aprvd, a.rvw_aprv_dt, ");
            sb.Append("a.rvw_aprv_rmk, a.aprv_rl_id, a.rvw_mtrc_id, r.aprv_rl_nm, ");
            sb.Append("r.mst_aprv_con, r.mst_aprv_eva, e.fullname as rvw_emp_nm, ");
            sb.Append("f.fullname as aprv_emp_nm, t.aprv_typ_nm ");
            sb.Append("FROM public.pmsrvwaprvs a ");
            sb.Append("LEFT JOIN public.pmsaprvrls r ON r.aprv_rl_id = a.aprv_rl_id ");
            sb.Append("LEFT JOIN public.gst_prsns e ON e.id = a.rvw_emp_id ");
            sb.Append("LEFT JOIN public.gst_prsns f ON f.id = a.aprv_emp_id ");
            sb.Append("LEFT JOIN public.pmsaprvtyps t ON t.aprv_typ_id = a.aprv_typ_id ");
            sb.Append("WHERE (a.rvw_hdr_id = @rvw_hdr_id) ");
            sb.Append("AND (a.aprv_typ_id = @aprv_typ_id) ");
            sb.Append("AND (a.is_aprvd = true) ");
            sb.Append("ORDER BY a.rvw_aprv_id DESC;");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_hdr_id = cmd.Parameters.Add("@rvw_hdr_id", NpgsqlDbType.Integer);
                var aprv_typ_id = cmd.Parameters.Add("@aprv_typ_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                rvw_hdr_id.Value = reviewHeaderId;
                aprv_typ_id.Value = approvalTypeId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    reviewApprovalsList.Add(new ReviewApproval()
                    {
                        ReviewApprovalId = reader["rvw_aprv_id"] == DBNull.Value ? 0 : (int)(reader["rvw_aprv_id"]),
                        ApprovalTypeId = reader["aprv_typ_id"] == DBNull.Value ? 0 : (int)reader["aprv_typ_id"],
                        ApprovalTypeDescription = reader["aprv_typ_nm"] == DBNull.Value ? string.Empty : (reader["aprv_typ_nm"]).ToString(),
                        ReviewHeaderId = reader["rvw_hdr_id"] == DBNull.Value ? 0 : (int)reader["rvw_hdr_id"],
                        ApproverId = reader["aprv_emp_id"] == DBNull.Value ? string.Empty : reader["aprv_emp_id"].ToString(),
                        ApproverName = reader["aprv_emp_nm"] == DBNull.Value ? string.Empty : reader["aprv_emp_nm"].ToString(),
                        AppraiseeId = reader["rvw_emp_id"] == DBNull.Value ? string.Empty : reader["rvw_emp_id"].ToString(),
                        AppraiseeName = reader["rvw_emp_nm"] == DBNull.Value ? string.Empty : reader["rvw_emp_id"].ToString(),
                        IsApproved = reader["is_aprvd"] == DBNull.Value ? false : (bool)reader["is_aprvd"],
                        ApprovedTime = reader["rvw_aprv_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rvw_aprv_dt"],
                        ApprovedComments = reader["rvw_aprv_rmk"] == DBNull.Value ? string.Empty : reader["rvw_aprv_rmk"].ToString(),
                        ApproverRoleId = reader["aprv_rl_id"] == DBNull.Value ? 0 : (int)reader["aprv_rl_id"],
                        ApproverRoleDescription = reader["aprv_rl_nm"] == DBNull.Value ? string.Empty : reader["aprv_rl_nm"].ToString(),
                        ReviewMetricId = reader["rvw_mtrc_id"] == DBNull.Value ? 0 : (int)reader["rvw_mtrc_id"],
                        MustApproveContract = reader["mst_aprv_con"] == DBNull.Value ? false : (bool)reader["mst_aprv_con"],
                        MustApproveEvaluation = reader["mst_aprv_eva"] == DBNull.Value ? false : (bool)reader["mst_aprv_eva"],
                    });
                }
            }
            await conn.CloseAsync();
            return reviewApprovalsList;
        }

        public async Task<IList<ReviewApproval>> GetApprovedByReviewHeaderIdAsync(int reviewHeaderId, int approvalTypeId, int approverRoleId)
        {
            List<ReviewApproval> reviewApprovalsList = new List<ReviewApproval>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT a.rvw_aprv_id, a.aprv_typ_id, a.rvw_hdr_id, ");
            sb.Append("a.aprv_emp_id, a.rvw_emp_id, a.is_aprvd, a.rvw_aprv_dt, ");
            sb.Append("a.rvw_aprv_rmk, a.aprv_rl_id, a.rvw_mtrc_id, r.aprv_rl_nm, ");
            sb.Append("r.mst_aprv_con, r.mst_aprv_eva, e.fullname as rvw_emp_nm, ");
            sb.Append("f.fullname as aprv_emp_nm, t.aprv_typ_nm ");
            sb.Append("FROM public.pmsrvwaprvs a ");
            sb.Append("LEFT JOIN public.pmsaprvrls r ON r.aprv_rl_id = a.aprv_rl_id ");
            sb.Append("LEFT JOIN public.gst_prsns e ON e.id = a.rvw_emp_id ");
            sb.Append("LEFT JOIN public.gst_prsns f ON f.id = a.aprv_emp_id ");
            sb.Append("LEFT JOIN public.pmsaprvtyps t ON t.aprv_typ_id = a.aprv_typ_id ");
            sb.Append("WHERE (a.rvw_hdr_id = @rvw_hdr_id) ");
            sb.Append("AND (a.aprv_typ_id = @aprv_typ_id) ");
            sb.Append("AND (a.aprv_rl_id = @aprv_rl_id) ");
            sb.Append("AND (a.is_aprvd = true) ");
            sb.Append("ORDER BY a.rvw_aprv_id DESC;");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_hdr_id = cmd.Parameters.Add("@rvw_hdr_id", NpgsqlDbType.Integer);
                var aprv_typ_id = cmd.Parameters.Add("@aprv_typ_id", NpgsqlDbType.Integer);
                var aprv_rl_id = cmd.Parameters.Add("@aprv_rl_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                rvw_hdr_id.Value = reviewHeaderId;
                aprv_typ_id.Value = approvalTypeId;
                aprv_rl_id.Value = approverRoleId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    reviewApprovalsList.Add(new ReviewApproval()
                    {
                        ReviewApprovalId = reader["rvw_aprv_id"] == DBNull.Value ? 0 : (int)(reader["rvw_aprv_id"]),
                        ApprovalTypeId = reader["aprv_typ_id"] == DBNull.Value ? 0 : (int)reader["aprv_typ_id"],
                        ApprovalTypeDescription = reader["aprv_typ_nm"] == DBNull.Value ? string.Empty : (reader["aprv_typ_nm"]).ToString(),
                        ReviewHeaderId = reader["rvw_hdr_id"] == DBNull.Value ? 0 : (int)reader["rvw_hdr_id"],
                        ApproverId = reader["aprv_emp_id"] == DBNull.Value ? string.Empty : reader["aprv_emp_id"].ToString(),
                        ApproverName = reader["aprv_emp_nm"] == DBNull.Value ? string.Empty : reader["aprv_emp_nm"].ToString(),
                        AppraiseeId = reader["rvw_emp_id"] == DBNull.Value ? string.Empty : reader["rvw_emp_id"].ToString(),
                        AppraiseeName = reader["rvw_emp_nm"] == DBNull.Value ? string.Empty : reader["rvw_emp_id"].ToString(),
                        IsApproved = reader["is_aprvd"] == DBNull.Value ? false : (bool)reader["is_aprvd"],
                        ApprovedTime = reader["rvw_aprv_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rvw_aprv_dt"],
                        ApprovedComments = reader["rvw_aprv_rmk"] == DBNull.Value ? string.Empty : reader["rvw_aprv_rmk"].ToString(),
                        ApproverRoleId = reader["aprv_rl_id"] == DBNull.Value ? 0 : (int)reader["aprv_rl_id"],
                        ApproverRoleDescription = reader["aprv_rl_nm"] == DBNull.Value ? string.Empty : reader["aprv_rl_nm"].ToString(),
                        ReviewMetricId = reader["rvw_mtrc_id"] == DBNull.Value ? 0 : (int)reader["rvw_mtrc_id"],
                        MustApproveContract = reader["mst_aprv_con"] == DBNull.Value ? false : (bool)reader["mst_aprv_con"],
                        MustApproveEvaluation = reader["mst_aprv_eva"] == DBNull.Value ? false : (bool)reader["mst_aprv_eva"],
                    });
                }
            }
            await conn.CloseAsync();
            return reviewApprovalsList;
        }


        #endregion

        #region Review Approvals Write Action Methods
        public async Task<bool> AddAsync(ReviewApproval reviewApproval)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.pmsrvwaprvs(aprv_typ_id, rvw_hdr_id, ");
            sb.Append("aprv_emp_id, rvw_emp_id, is_aprvd, rvw_aprv_dt, rvw_aprv_rmk, ");
            sb.Append("aprv_rl_id, rvw_mtrc_id) VALUES (@aprv_typ_id, @rvw_hdr_id, ");
            sb.Append("@aprv_emp_id, @rvw_emp_id, @is_aprvd, @rvw_aprv_dt, ");
            sb.Append("@rvw_aprv_rmk, @aprv_rl_id, @rvw_mtrc_id);");
            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var aprv_typ_id = cmd.Parameters.Add("@aprv_typ_id", NpgsqlDbType.Integer);
                var rvw_hdr_id = cmd.Parameters.Add("@rvw_hdr_id", NpgsqlDbType.Integer);
                var aprv_emp_id = cmd.Parameters.Add("@aprv_emp_id", NpgsqlDbType.Text);
                var rvw_emp_id = cmd.Parameters.Add("@rvw_emp_id", NpgsqlDbType.Text);
                var is_aprvd = cmd.Parameters.Add("@is_aprvd", NpgsqlDbType.Boolean);
                var rvw_aprv_dt = cmd.Parameters.Add("@rvw_aprv_dt", NpgsqlDbType.TimestampTz);
                var rvw_aprv_rmk = cmd.Parameters.Add("@rvw_aprv_rmk", NpgsqlDbType.Text);
                var aprv_rl_id = cmd.Parameters.Add("@aprv_rl_id", NpgsqlDbType.Integer);
                var rvw_mtrc_id = cmd.Parameters.Add("@rvw_mtrc_id", NpgsqlDbType.Integer);
                cmd.Prepare();
                aprv_typ_id.Value = reviewApproval.ApprovalTypeId;
                rvw_hdr_id.Value = reviewApproval.ReviewHeaderId;
                aprv_emp_id.Value = reviewApproval.ApproverId;
                rvw_emp_id.Value = reviewApproval.AppraiseeId ?? (object)DBNull.Value;
                is_aprvd.Value = reviewApproval.IsApproved;
                rvw_aprv_dt.Value = reviewApproval.ApprovedTime;
                rvw_aprv_rmk.Value = reviewApproval.ApprovedComments ?? (object)DBNull.Value;
                aprv_rl_id.Value = reviewApproval.ApproverRoleId;
                rvw_mtrc_id.Value = reviewApproval.ReviewMetricId ?? (object)DBNull.Value;

                rows = await cmd.ExecuteNonQueryAsync();
                await conn.CloseAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int reviewApprovalId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));

            string query = "DELETE FROM public.pmsrvwaprvs WHERE(rvw_aprv_id=@rvw_aprv_id);";

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_aprv_id = cmd.Parameters.Add("@rvw_aprv_id", NpgsqlDbType.Integer);
                cmd.Prepare();
                rvw_aprv_id.Value = reviewApprovalId;

                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(ReviewApproval reviewApproval)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("DELETE FROM public.pmsrvwaprvs ");
            sb.Append("WHERE(aprv_typ_id=@aprv_typ_id) ");
            sb.Append("AND (rvw_hdr_id=@rvw_hdr_id) ");
            sb.Append("AND (aprv_emp_id=@aprv_emp_id) ");
            sb.Append("AND (aprv_rl_id=@aprv_rl_id); ");

            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var aprv_typ_id = cmd.Parameters.Add("@aprv_typ_id", NpgsqlDbType.Integer);
                var rvw_hdr_id = cmd.Parameters.Add("@rvw_hdr_id", NpgsqlDbType.Integer);
                var aprv_emp_id = cmd.Parameters.Add("@aprv_emp_id", NpgsqlDbType.Integer);
                var aprv_rl_id = cmd.Parameters.Add("@aprv_rl_id", NpgsqlDbType.Integer);
                cmd.Prepare();
                aprv_typ_id.Value = reviewApproval.ApprovalTypeId;
                rvw_hdr_id.Value = reviewApproval.ReviewHeaderId;
                aprv_emp_id.Value = reviewApproval.ApproverId;
                aprv_rl_id.Value = reviewApproval.ApproverRoleId;

                rows = await cmd.ExecuteNonQueryAsync();
                await conn.CloseAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        #endregion
    }
}
