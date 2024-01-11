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
    public class AssetGroupRepository : IAssetGroupRepository
    {

        public IConfiguration _config { get; }
        public AssetGroupRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        //============== Asset Group Action Methods =======================//
        #region AssetGroup Action Methods

        public async Task<AssetGroup> GetByIdAsync(int assetGroupId)
        {
            AssetGroup assetGroup = new AssetGroup();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT g.grp_id, g.grp_nm, g.ctg_id, ");
            sb.Append("g.clss_id, l.clss_nm, c.asst_ctgs_nm  ");
            sb.Append("FROM public.asm_stt_grps g ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ");
            sb.Append("ON g.ctg_id = c.asst_ctgs_id  ");
            sb.Append("LEFT JOIN public.asm_stt_clss l ");
            sb.Append("ON l.clss_id = g.clss_id  ");
            sb.Append("WHERE (g.grp_id = @grp_id);");
            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var grp_id = cmd.Parameters.Add("@grp_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                grp_id.Value = assetGroupId;
                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        assetGroup.GroupID = reader["grp_id"] == DBNull.Value ? 0 : (int)reader["grp_id"];
                        assetGroup.GroupName = reader["grp_nm"] == DBNull.Value ? String.Empty : reader["grp_nm"].ToString();
                        assetGroup.ClassID = reader["clss_id"] == DBNull.Value ? (int?)null : (int)reader["clss_id"];
                        assetGroup.ClassName = reader["clss_nm"] == DBNull.Value ? String.Empty : reader["clss_nm"].ToString();
                        assetGroup.CategoryID = reader["ctg_id"] == DBNull.Value ? (int?)null : (int)reader["ctg_id"];
                        assetGroup.CategoryName = reader["asst_ctgs_nm"] == DBNull.Value ? String.Empty : reader["asst_ctgs_nm"].ToString();
                    }
            }
            await conn.CloseAsync();

            return assetGroup;
        }

        public async Task<IList<AssetGroup>> GetByNameAsync(string groupName)
        {
            List<AssetGroup> groupList = new List<AssetGroup>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT g.grp_id, g.grp_nm, g.ctg_id, ");
            sb.Append("g.clss_id, l.clss_nm, c.asst_ctgs_nm  ");
            sb.Append("FROM public.asm_stt_grps g ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ");
            sb.Append("ON g.ctg_id = c.asst_ctgs_id  ");
            sb.Append("LEFT JOIN public.asm_stt_clss l ");
            sb.Append("ON l.clss_id = g.clss_id  ");
            sb.Append("WHERE(LOWER(g.grp_nm) LIKE '%'||LOWER(@grp_nm)||'%') ");
            sb.Append("ORDER BY grp_nm;");
            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var grp_nm = cmd.Parameters.Add("@grp_nm", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                grp_nm.Value = groupName;
                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        groupList.Add(new AssetGroup()
                        {
                            GroupID = reader["grp_id"] == DBNull.Value ? 0 : (int)reader["grp_id"],
                            GroupName = reader["grp_nm"] == DBNull.Value ? String.Empty : reader["grp_nm"].ToString(),
                            ClassID = reader["clss_id"] == DBNull.Value ? (int?)null : (int)reader["clss_id"],
                            ClassName = reader["clss_nm"] == DBNull.Value ? String.Empty : reader["clss_nm"].ToString(),
                            CategoryID = reader["ctg_id"] == DBNull.Value ? (int?)null : (int)reader["ctg_id"],
                            CategoryName = reader["asst_ctgs_nm"] == DBNull.Value ? String.Empty : reader["asst_ctgs_nm"].ToString(),
                        });
                    }
            }
            await conn.CloseAsync();
            return groupList;
        }

        public async Task<IList<AssetGroup>> GetByCategoryIdAsync(int assetCategoryId)
        {
            List<AssetGroup> assetGroupList = new List<AssetGroup>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT g.grp_id, g.grp_nm, g.ctg_id, ");
            sb.Append("g.clss_id, l.clss_nm, c.asst_ctgs_nm  ");
            sb.Append("FROM public.asm_stt_grps g ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ");
            sb.Append("ON g.ctg_id = c.asst_ctgs_id ");
            sb.Append("LEFT JOIN public.asm_stt_clss l ");
            sb.Append("ON l.clss_id = g.clss_id ");
            sb.Append("WHERE (g.ctg_id = @ctg_id);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var ctg_id = cmd.Parameters.Add("@ctg_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    ctg_id.Value = assetCategoryId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assetGroupList.Add(new AssetGroup()
                            {
                                GroupID = reader["grp_id"] == DBNull.Value ? 0 : (int)reader["grp_id"],
                                GroupName = reader["grp_nm"] == DBNull.Value ? String.Empty : reader["grp_nm"].ToString(),
                                ClassID = reader["clss_id"] == DBNull.Value ? (int?)null : (int)reader["clss_id"],
                                ClassName = reader["clss_nm"] == DBNull.Value ? String.Empty : reader["clss_nm"].ToString(),
                                CategoryID = reader["ctg_id"] == DBNull.Value ? (int?)null : (int)reader["ctg_id"],
                                CategoryName = reader["asst_ctgs_nm"] == DBNull.Value ? String.Empty : reader["asst_ctgs_nm"].ToString(),
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
            return assetGroupList;
        }

        public async Task<IList<AssetGroup>> GetByClassIdAsync(int assetClassId)
        {
            List<AssetGroup> assetGroupList = new List<AssetGroup>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT g.grp_id, g.grp_nm, g.ctg_id, ");
            sb.Append("g.clss_id, l.clss_nm, c.asst_ctgs_nm  ");
            sb.Append("FROM public.asm_stt_grps g ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ");
            sb.Append("ON g.ctg_id = c.asst_ctgs_id  ");
            sb.Append("LEFT JOIN public.asm_stt_clss l ");
            sb.Append("ON l.clss_id = g.clss_id  ");
            sb.Append("WHERE (g.clss_id = @clss_id);");
            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var clss_id = cmd.Parameters.Add("@clss_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                clss_id.Value = assetClassId;
                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        assetGroupList.Add(new AssetGroup()
                        {
                            GroupID = reader["grp_id"] == DBNull.Value ? 0 : (int)reader["grp_id"],
                            GroupName = reader["grp_nm"] == DBNull.Value ? String.Empty : reader["grp_nm"].ToString(),
                            ClassID = reader["clss_id"] == DBNull.Value ? (int?)null : (int)reader["clss_id"],
                            ClassName = reader["clss_nm"] == DBNull.Value ? String.Empty : reader["clss_nm"].ToString(),
                            CategoryID = reader["ctg_id"] == DBNull.Value ? (int?)null : (int)reader["ctg_id"],
                            CategoryName = reader["asst_ctgs_nm"] == DBNull.Value ? String.Empty : reader["asst_ctgs_nm"].ToString(),
                        });
                    }
            }
            await conn.CloseAsync();
            return assetGroupList;
        }

        public async Task<IList<AssetGroup>> GetAllAsync()
        {
            List<AssetGroup> groupsList = new List<AssetGroup>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT g.grp_id, g.grp_nm, g.ctg_id, ");
            sb.Append("g.clss_id, l.clss_nm, c.asst_ctgs_nm  ");
            sb.Append("FROM public.asm_stt_grps g ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ");
            sb.Append("ON g.ctg_id = c.asst_ctgs_id  ");
            sb.Append("LEFT JOIN public.asm_stt_clss l ");
            sb.Append("ON l.clss_id = g.clss_id  ");
            sb.Append("ORDER BY c.asst_ctgs_nm, l.clss_nm, g.grp_nm;");
            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                await cmd.PrepareAsync();
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    groupsList.Add(new AssetGroup()
                    {
                        GroupID = reader["grp_id"] == DBNull.Value ? 0 : (int)reader["grp_id"],
                        GroupName = reader["grp_nm"] == DBNull.Value ? string.Empty : reader["grp_nm"].ToString(),
                        ClassID = reader["clss_id"] == DBNull.Value ? (int?)null : (int)reader["clss_id"],
                        ClassName = reader["clss_nm"] == DBNull.Value ? String.Empty : reader["clss_nm"].ToString(),
                        CategoryID = reader["ctg_id"] == DBNull.Value ? (int?)null : (int)reader["ctg_id"],
                        CategoryName = reader["asst_ctgs_nm"] == DBNull.Value ? String.Empty : reader["asst_ctgs_nm"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return groupsList;
        }

        public async Task<bool> AddAsync(AssetGroup assetGroup)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.asm_stt_grps( ");
            sb.Append("grp_nm, ctg_id, clss_id) ");
            sb.Append("VALUES (@grp_nm, @ctg_id, @clss_id);");
            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var grp_nm = cmd.Parameters.Add("@grp_nm", NpgsqlDbType.Text);
                var ctg_id = cmd.Parameters.Add("@ctg_id", NpgsqlDbType.Integer);
                var clss_id = cmd.Parameters.Add("@clss_id", NpgsqlDbType.Integer);
                cmd.Prepare();
                grp_nm.Value = assetGroup.GroupName;
                ctg_id.Value = assetGroup.CategoryID;
                clss_id.Value = assetGroup.ClassID;

                rows = await cmd.ExecuteNonQueryAsync();

            }
            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<bool> EditAsync(AssetGroup assetGroup)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.asm_stt_grps SET ");
            sb.Append("grp_nm=@grp_nm, ctg_id=@ctg_id, clss_id=@clss_id ");
            sb.Append("WHERE (grp_id = @grp_id); ");

            string query = sb.ToString();
            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var grp_id = cmd.Parameters.Add("@grp_id", NpgsqlDbType.Integer);
                var ctg_id = cmd.Parameters.Add("@ctg_id", NpgsqlDbType.Integer);
                var grp_nm = cmd.Parameters.Add("@grp_nm", NpgsqlDbType.Text);
                var clss_id = cmd.Parameters.Add("@clss_id", NpgsqlDbType.Integer);
                cmd.Prepare();
                grp_id.Value = assetGroup.GroupID;
                grp_nm.Value = assetGroup.GroupName;
                ctg_id.Value = assetGroup.CategoryID;
                clss_id.Value = assetGroup.ClassID;
                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int assetGroupId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = $"DELETE FROM public.asm_stt_grps WHERE (grp_id = @grp_id);";

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var grp_id = cmd.Parameters.Add("@grp_id", NpgsqlDbType.Integer);
                cmd.Prepare();
                grp_id.Value = assetGroupId;
                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        #endregion

    }
}
