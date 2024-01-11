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
    public class ApprovalRoleRepository : IApprovalRoleRepository
    {
        public IConfiguration _config { get; }
        public ApprovalRoleRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        #region Appraisal Grade Read Action Methods
        public async Task<IList<ApprovalRole>> GetAllAsync()
        {
            List<ApprovalRole> approvalRolesList = new List<ApprovalRole>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT aprv_rl_id, aprv_rl_nm, mst_aprv_con, ");
            sb.Append("mst_aprv_eva FROM public.pmsaprvrls ");
            sb.Append("ORDER BY aprv_rl_id;");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                await cmd.PrepareAsync();
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    approvalRolesList.Add(new ApprovalRole()
                    {
                        ApprovalRoleId = reader["aprv_rl_id"] == DBNull.Value ? 0 : (int)(reader["aprv_rl_id"]),
                        ApprovalRoleName = reader["aprv_rl_nm"] == DBNull.Value ? string.Empty : (reader["aprv_rl_nm"]).ToString(),
                        MustApproveContract = reader["mst_aprv_con"] == DBNull.Value ? false : (bool)reader["mst_aprv_con"],
                        MustApproveEvaluation = reader["mst_aprv_eva"] == DBNull.Value ? false : (bool)reader["mst_aprv_eva"],
                    });
                }
            }
            await conn.CloseAsync();
            return approvalRolesList;
        }

        public async Task<IList<ApprovalRole>> GetByIdAsync(int approvalRoleId)
        {
            List<ApprovalRole> approvalRolesList = new List<ApprovalRole>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT aprv_rl_id, aprv_rl_nm, mst_aprv_con, ");
            sb.Append("mst_aprv_eva FROM public.pmsaprvrls ");
            sb.Append("WHERE aprv_rl_id = @aprv_rl_id;");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var aprv_rl_id = cmd.Parameters.Add("@aprv_rl_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                aprv_rl_id.Value = approvalRoleId;
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    approvalRolesList.Add(new ApprovalRole()
                    {
                        ApprovalRoleId = reader["aprv_rl_id"] == DBNull.Value ? 0 : (int)(reader["aprv_rl_id"]),
                        ApprovalRoleName = reader["aprv_rl_nm"] == DBNull.Value ? string.Empty : (reader["aprv_rl_nm"]).ToString(),
                        MustApproveContract = reader["mst_aprv_con"] == DBNull.Value ? false : (bool)reader["mst_aprv_con"],
                        MustApproveEvaluation = reader["mst_aprv_eva"] == DBNull.Value ? false : (bool)reader["mst_aprv_eva"],
                    });
                }
            }
            await conn.CloseAsync();
            return approvalRolesList;
        }

        public async Task<IList<ApprovalRole>> GetByNameAsync(string approvalRoleName)
        {
            List<ApprovalRole> approvalRolesList = new List<ApprovalRole>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT aprv_rl_id, aprv_rl_nm, mst_aprv_con, ");
            sb.Append("mst_aprv_eva FROM public.pmsaprvrls ");
            sb.Append("WHERE LOWER(aprv_rl_nm) = LOWER(@aprv_rl_nm);");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var aprv_rl_nm = cmd.Parameters.Add("@aprv_rl_nm", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                aprv_rl_nm.Value = approvalRoleName;
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    approvalRolesList.Add(new ApprovalRole()
                    {
                        ApprovalRoleId = reader["aprv_rl_id"] == DBNull.Value ? 0 : (int)(reader["aprv_rl_id"]),
                        ApprovalRoleName = reader["aprv_rl_nm"] == DBNull.Value ? string.Empty : (reader["aprv_rl_nm"]).ToString(),
                        MustApproveContract = reader["mst_aprv_con"] == DBNull.Value ? false : (bool)reader["mst_aprv_con"],
                        MustApproveEvaluation = reader["mst_aprv_eva"] == DBNull.Value ? false : (bool)reader["mst_aprv_eva"],
                    });
                }
            }
            await conn.CloseAsync();
            return approvalRolesList;
        }

        public async Task<IList<ApprovalRole>> GetMustApproveContractsAsync()
        {
            List<ApprovalRole> approvalRolesList = new List<ApprovalRole>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT aprv_rl_id, aprv_rl_nm, mst_aprv_con, ");
            sb.Append("mst_aprv_eva FROM public.pmsaprvrls ");
            sb.Append("WHERE mst_aprv_con = true ");
            sb.Append("ORDER BY aprv_rl_id;");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                await cmd.PrepareAsync();
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    approvalRolesList.Add(new ApprovalRole()
                    {
                        ApprovalRoleId = reader["aprv_rl_id"] == DBNull.Value ? 0 : (int)(reader["aprv_rl_id"]),
                        ApprovalRoleName = reader["aprv_rl_nm"] == DBNull.Value ? string.Empty : (reader["aprv_rl_nm"]).ToString(),
                        MustApproveContract = reader["mst_aprv_con"] == DBNull.Value ? false : (bool)reader["mst_aprv_con"],
                        MustApproveEvaluation = reader["mst_aprv_eva"] == DBNull.Value ? false : (bool)reader["mst_aprv_eva"],
                    });
                }
            }
            await conn.CloseAsync();
            return approvalRolesList;
        }

        public async Task<IList<ApprovalRole>> GetMustApproveEvaluationsAsync()
        {
            List<ApprovalRole> approvalRolesList = new List<ApprovalRole>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT aprv_rl_id, aprv_rl_nm, mst_aprv_con, ");
            sb.Append("mst_aprv_eva FROM public.pmsaprvrls ");
            sb.Append("WHERE mst_aprv_eva = true ");
            sb.Append("ORDER BY aprv_rl_id;");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                await cmd.PrepareAsync();
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    approvalRolesList.Add(new ApprovalRole()
                    {
                        ApprovalRoleId = reader["aprv_rl_id"] == DBNull.Value ? 0 : (int)(reader["aprv_rl_id"]),
                        ApprovalRoleName = reader["aprv_rl_nm"] == DBNull.Value ? string.Empty : (reader["aprv_rl_nm"]).ToString(),
                        MustApproveContract = reader["mst_aprv_con"] == DBNull.Value ? false : (bool)reader["mst_aprv_con"],
                        MustApproveEvaluation = reader["mst_aprv_eva"] == DBNull.Value ? false : (bool)reader["mst_aprv_eva"],
                    });
                }
            }
            await conn.CloseAsync();
            return approvalRolesList;
        }

        #endregion

        #region Review Grade Write Action Methods
        public async Task<bool> AddAsync(ApprovalRole approvalRole)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.pmsaprvrls(aprv_rl_nm, mst_aprv_con, mst_aprv_eva) ");
            sb.Append("VALUES (@aprv_rl_nm, @mst_aprv_con, @mst_aprv_eva);");
            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var aprv_rl_nm = cmd.Parameters.Add("@aprv_rl_nm", NpgsqlDbType.Text);
                var mst_aprv_con = cmd.Parameters.Add("@mst_aprv_con", NpgsqlDbType.Boolean);
                var mst_aprv_eva = cmd.Parameters.Add("@mst_aprv_eva", NpgsqlDbType.Boolean);
                cmd.Prepare();
                aprv_rl_nm.Value = approvalRole.ApprovalRoleName;
                mst_aprv_con.Value = approvalRole.MustApproveContract;
                mst_aprv_eva.Value = approvalRole.MustApproveEvaluation;
                rows = await cmd.ExecuteNonQueryAsync();
                await conn.CloseAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<bool> UpdateAsync(ApprovalRole approvalRole)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.pmsaprvrls SET aprv_rl_nm=@aprv_rl_nm, mst_aprv_con=@mst_aprv_con, ");
            sb.Append("mst_aprv_eva=@mst_aprv_eva WHERE (aprv_rl_id=@aprv_rl_id);");
            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var aprv_rl_nm = cmd.Parameters.Add("@aprv_rl_nm", NpgsqlDbType.Text);
                var mst_aprv_con = cmd.Parameters.Add("@mst_aprv_con", NpgsqlDbType.Boolean);
                var mst_aprv_eva = cmd.Parameters.Add("@mst_aprv_eva", NpgsqlDbType.Boolean);
                var aprv_rl_id = cmd.Parameters.Add("@aprv_rl_id", NpgsqlDbType.Integer);
                cmd.Prepare();
                aprv_rl_nm.Value = approvalRole.ApprovalRoleName;
                mst_aprv_con.Value = approvalRole.MustApproveContract;
                mst_aprv_eva.Value = approvalRole.MustApproveEvaluation;
                aprv_rl_id.Value = approvalRole.ApprovalRoleId;

                rows = await cmd.ExecuteNonQueryAsync();
                await conn.CloseAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int approvalRoleId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));

            string query = "DELETE FROM public.pmsaprvrls WHERE(aprv_rl_id=@aprv_rl_id);";

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var aprv_rl_id = cmd.Parameters.Add("@aprv_rl_id", NpgsqlDbType.Integer);
                cmd.Prepare();
                aprv_rl_id.Value = approvalRoleId;

                rows = await cmd.ExecuteNonQueryAsync();
                await conn.CloseAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }
        #endregion

    }
}
