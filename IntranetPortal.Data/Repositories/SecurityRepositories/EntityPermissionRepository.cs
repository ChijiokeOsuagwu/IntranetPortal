using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IntranetPortal.Base.Enums;
using IntranetPortal.Base.Models.AssetManagerModels;
using IntranetPortal.Base.Repositories.SecurityRepositories;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;

namespace IntranetPortal.Data.Repositories.SecurityRepositories
{
    public class EntityPermissionRepository : IEntityPermissionRepository
    {
        public IConfiguration _config { get; }
        public EntityPermissionRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        #region AssetPermission Action Methods

        public async Task<IList<AssetPermission>> GetAssetPermissionsByUserIdAsync(string userId)
        {
            List<AssetPermission> assetPermissionList = new List<AssetPermission>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT n.ntt_pms_id, n.usr_acct_id, n.asst_dvsn_id, ");
            sb.Append("n.ntt_typ, (SELECT asst_dvsn_nm FROM public.asm_stt_dvsn ");
            sb.Append("WHERE asst_dvsn_id = n.asst_dvsn_id) as asst_dvsn_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns ");
            sb.Append("WHERE id = n.usr_acct_id) as usr_fullname ");
            sb.Append("FROM public.sct_ntt_pms n ");
            sb.Append("WHERE (n.usr_acct_id = @usr_acct_id) AND (n.ntt_typ = @ntt_typ) ");
            sb.Append("ORDER BY asst_dvsn_nm;");
            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var ntt_typ = cmd.Parameters.Add("@ntt_typ", NpgsqlDbType.Integer);
                var usr_acct_id = cmd.Parameters.Add("@usr_acct_id", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                ntt_typ.Value = (int)EntityPermissionType.AssetDivision;
                usr_acct_id.Value = userId;
                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        assetPermissionList.Add(new AssetPermission()
                        {
                            ID = reader["ntt_pms_id"] == DBNull.Value ? 0 : (int)reader["ntt_pms_id"],
                            AssetDivisionID = reader["asst_dvsn_id"] == DBNull.Value ? 0 : (int)reader["asst_dvsn_id"],
                            AssetDivisionName = reader["asst_dvsn_nm"] == DBNull.Value ? string.Empty : reader["asst_dvsn_nm"].ToString(),
                            UserID = reader["usr_acct_id"] == DBNull.Value ? string.Empty : reader["usr_acct_id"].ToString(),
                            UserFullName = reader["usr_fullname"] == DBNull.Value ? string.Empty : reader["usr_fullname"].ToString(),
                        });
                    }
            }
            await conn.CloseAsync();
            return assetPermissionList;
        }

        public async Task<bool> AddAssetPermissionAsync(AssetPermission assetPermission)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.sct_ntt_pms(usr_acct_id, ");
            sb.Append("asst_dvsn_id, ntt_typ) VALUES ");
            sb.Append("(@usr_acct_id, @asst_dvsn_id, @ntt_typ);");
            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var usr_acct_id = cmd.Parameters.Add("@usr_acct_id", NpgsqlDbType.Text);
                var ntt_typ = cmd.Parameters.Add("@ntt_typ", NpgsqlDbType.Integer);
                var asst_dvsn_id = cmd.Parameters.Add("@asst_dvsn_id", NpgsqlDbType.Integer);
                cmd.Prepare();
                usr_acct_id.Value = assetPermission.UserID;
                ntt_typ.Value = (int)assetPermission.PermissionType;
                asst_dvsn_id.Value = assetPermission.AssetDivisionID;

                rows = await cmd.ExecuteNonQueryAsync();
                await conn.CloseAsync();
            }
            return rows > 0;
        }

        public async Task<bool> DeleteAssetPermissionAsync(int assetPermissionId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("DELETE FROM public.sct_ntt_pms WHERE (ntt_pms_id = @ntt_pms_id); ");
            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var ntt_pms_id = cmd.Parameters.Add("@ntt_pms_id", NpgsqlDbType.Integer);
                cmd.Prepare();
                ntt_pms_id.Value = assetPermissionId;
                rows = await cmd.ExecuteNonQueryAsync();
                await conn.CloseAsync();
            }

            return rows > 0;
        }

        #endregion
    }
}
