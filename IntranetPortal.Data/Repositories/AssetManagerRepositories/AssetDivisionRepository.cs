using IntranetPortal.Base.Models.AssetManagerModels;
using IntranetPortal.Base.Repositories.AssetManagerRepositories;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Data.Repositories.AssetManagerRepositories
{
    public class AssetDivisionRepository : IAssetDivisionRepository
    {
        public IConfiguration _config { get; }
        public AssetDivisionRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        //============== Asset Division Action Methods =====================================================//
        #region AssetDivision Read Action Methods

        public async Task<AssetDivision> GetByIdAsync(int divisionId)
        {
            AssetDivision assetDivision = new AssetDivision();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT asst_dvsn_id, asst_dvsn_nm, asst_dvsn_ds ");
            sb.Append("FROM public.asm_stt_dvsn  ");
            sb.Append("WHERE asst_dvsn_id = @asst_dvsn_id;");
            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var division_id = cmd.Parameters.Add("@asst_dvsn_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                division_id.Value = divisionId;
                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        assetDivision.ID = reader["asst_dvsn_id"] == DBNull.Value ? (int?)null : (int)reader["asst_dvsn_id"];
                        assetDivision.Name = reader["asst_dvsn_nm"] == DBNull.Value ? String.Empty : reader["asst_dvsn_nm"].ToString();
                        assetDivision.Description = reader["asst_dvsn_ds"] == DBNull.Value ? String.Empty : reader["asst_dvsn_ds"].ToString();
                    }
            }
            return assetDivision;
        }

        public async Task<IList<AssetDivision>> GetByNameAsync(string divisionName)
        {
            List<AssetDivision> divisionList = new List<AssetDivision>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT asst_dvsn_id, asst_dvsn_nm, asst_dvsn_ds ");
            sb.Append("FROM public.asm_stt_dvsn  ");
            sb.Append("WHERE(LOWER(asst_dvsn_nm) LIKE '%'||LOWER(@asst_dvsn_nm)||'%') ");
            sb.Append("ORDER BY asst_dvsn_nm; ");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var division_name = cmd.Parameters.Add("@asst_dvsn_nm", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    division_name.Value = divisionName;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            divisionList.Add(new AssetDivision()
                            {
                                ID = reader["asst_dvsn_id"] == DBNull.Value ? (int?)null : (int)reader["asst_dvsn_id"],
                                Name = reader["asst_dvsn_nm"] == DBNull.Value ? String.Empty : reader["asst_dvsn_nm"].ToString(),
                                Description = reader["asst_dvsn_ds"] == DBNull.Value ? String.Empty : reader["asst_dvsn_ds"].ToString(),
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
            return divisionList;
        }

        public async Task<IList<AssetDivision>> GetAllAsync(string userId)
        {
            List<AssetDivision> divisionList = new List<AssetDivision>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT asst_dvsn_id, asst_dvsn_nm, asst_dvsn_ds ");
            sb.Append("FROM public.asm_stt_dvsn ");
            sb.Append("WHERE asst_dvsn_id IN (SELECT asst_dvsn_id FROM public.sct_ntt_pms ");
            sb.Append("WHERE ntt_typ=0 AND usr_acct_id = @usr_id) ");
            sb.Append("ORDER BY asst_dvsn_nm; ");

            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var usr_id = cmd.Parameters.Add("@usr_id", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                usr_id.Value = userId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    divisionList.Add(new AssetDivision()
                    {
                        ID = reader["asst_dvsn_id"] == DBNull.Value ? 0 : (int)reader["asst_dvsn_id"],
                        Name = reader["asst_dvsn_nm"] == DBNull.Value ? String.Empty : reader["asst_dvsn_nm"].ToString(),
                        Description = reader["asst_dvsn_ds"] == DBNull.Value ? String.Empty : reader["asst_dvsn_ds"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return divisionList;
        }

        public async Task<IList<AssetDivision>> GetAllAsync()
        {
            List<AssetDivision> divisionList = new List<AssetDivision>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT asst_dvsn_id, asst_dvsn_nm, asst_dvsn_ds ");
            sb.Append("FROM public.asm_stt_dvsn ");
            sb.Append("ORDER BY asst_dvsn_nm; ");
            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                await cmd.PrepareAsync();
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    divisionList.Add(new AssetDivision()
                    {
                        ID = reader["asst_dvsn_id"] == DBNull.Value ? 0 : (int)reader["asst_dvsn_id"],
                        Name = reader["asst_dvsn_nm"] == DBNull.Value ? String.Empty : reader["asst_dvsn_nm"].ToString(),
                        Description = reader["asst_dvsn_ds"] == DBNull.Value ? String.Empty : reader["asst_dvsn_ds"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return divisionList;
        }
        #endregion

        #region AssetDivision Write Action Methods

        public async Task<bool> AddAsync(AssetDivision assetDivision)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.asm_stt_dvsn ");
            sb.Append("(asst_dvsn_nm, asst_dvsn_ds) ");
            sb.Append("VALUES (@asst_dvsn_nm, @asst_dvsn_ds); ");
            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var asst_dvsn_nm = cmd.Parameters.Add("@asst_dvsn_nm", NpgsqlDbType.Text);
                var asst_dvsn_ds = cmd.Parameters.Add("@asst_dvsn_ds", NpgsqlDbType.Text);
                cmd.Prepare();
                asst_dvsn_nm.Value = assetDivision.Name;
                asst_dvsn_ds.Value = assetDivision.Description ?? (object)DBNull.Value;
                rows = await cmd.ExecuteNonQueryAsync();
                await conn.CloseAsync();
            }
            return rows > 0;
        }

        public async Task<bool> EditAsync(AssetDivision assetDivision)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.asm_stt_dvsn SET asst_dvsn_nm=@asst_dvsn_nm, ");
            sb.Append("asst_dvsn_ds=@asst_dvsn_ds ");
            sb.Append("WHERE asst_dvsn_id=@asst_dvsn_id; ");
            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var asst_dvsn_id = cmd.Parameters.Add("@asst_dvsn_id", NpgsqlDbType.Integer);
                var asst_dvsn_nm = cmd.Parameters.Add("@asst_dvsn_nm", NpgsqlDbType.Text);
                var asst_dvsn_ds = cmd.Parameters.Add("@asst_dvsn_ds", NpgsqlDbType.Text);
                cmd.Prepare();
                asst_dvsn_id.Value = assetDivision.ID;
                asst_dvsn_nm.Value = assetDivision.Name;
                asst_dvsn_ds.Value = assetDivision.Description ?? (object)DBNull.Value;

                rows = await cmd.ExecuteNonQueryAsync();
                await conn.CloseAsync();
            }
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int assetDivisionId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("DELETE FROM public.asm_stt_dvsn  ");
            sb.Append("WHERE (asst_dvsn_id = @asst_dvsn_id); ");
            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var asst_dvsn_id = cmd.Parameters.Add("@asst_dvsn_id", NpgsqlDbType.Integer);
                cmd.Prepare();
                asst_dvsn_id.Value = assetDivisionId;
                rows = await cmd.ExecuteNonQueryAsync();
                await conn.CloseAsync();
            }
            return rows > 0;
        }

        #endregion
    }
}
