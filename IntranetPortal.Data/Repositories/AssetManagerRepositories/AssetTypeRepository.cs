using IntranetPortal.Base.Models.AssetManagerModels;
using IntranetPortal.Base.Models.SecurityModels;
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
    public class AssetTypeRepository : IAssetTypeRepository
    {
        public IConfiguration _config { get; }
        public AssetTypeRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        //======= AssetType Action Methods ===========//
        #region AssetType Read Action Methods

        public async Task<AssetType> GetByIdAsync(int assetTypeId)
        {
            AssetType assetType = new AssetType();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT t.typ_id, t.typ_nm, t.typ_ds, t.clss_id, l.clss_nm, ");
            sb.Append("t.ctg_id, c.asst_ctgs_nm, t.grp_id, g.grp_nm ");
            sb.Append("FROM public.asm_stt_typs t  ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ON t.ctg_id = c.asst_ctgs_id ");
            sb.Append("LEFT JOIN public.asm_stt_clss l ON l.clss_id = t.clss_id ");
            sb.Append("LEFT JOIN public.asm_stt_grps g ON g.grp_id = t.grp_id ");
            sb.Append("WHERE (typ_id = @typ_id);");
            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var asset_type_id = cmd.Parameters.Add("@typ_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                asset_type_id.Value = assetTypeId;
                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        assetType.ID = reader["typ_id"] == DBNull.Value ? 0 : (int)reader["typ_id"];
                        assetType.Name = reader["typ_nm"] == DBNull.Value ? String.Empty : reader["typ_nm"].ToString();
                        assetType.Description = reader["typ_ds"] == DBNull.Value ? String.Empty : reader["typ_ds"].ToString();
                        assetType.GroupID = reader["grp_id"] == DBNull.Value ? (int?)null : (int)reader["grp_id"];
                        assetType.GroupName = reader["grp_nm"] == DBNull.Value ? string.Empty : reader["grp_nm"].ToString();
                        assetType.ClassID = reader["clss_id"] == DBNull.Value ? 0 : (int)reader["clss_id"];
                        assetType.ClassName = reader["clss_nm"] == DBNull.Value ? String.Empty : reader["clss_nm"].ToString();
                        assetType.CategoryID = reader["ctg_id"] == DBNull.Value ? 0 : (int)reader["ctg_id"];
                        assetType.CategoryName = reader["asst_ctgs_nm"] == DBNull.Value ? String.Empty : reader["asst_ctgs_nm"].ToString();
                    }
            }
            await conn.CloseAsync();
            return assetType;
        }

        public async Task<IList<AssetType>> GetByNameAsync(string typeName)
        {
            List<AssetType> typeList = new List<AssetType>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT t.typ_id, t.typ_nm, t.typ_ds, t.clss_id, l.clss_nm, ");
            sb.Append("t.ctg_id, c.asst_ctgs_nm, t.grp_id, g.grp_nm ");
            sb.Append("FROM public.asm_stt_typs t ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ON t.ctg_id = c.asst_ctgs_id ");
            sb.Append("LEFT JOIN public.asm_stt_clss l ON l.clss_id = t.clss_id ");
            sb.Append("LEFT JOIN public.asm_stt_grps g ON g.grp_id = t.grp_id ");
            sb.Append("WHERE(LOWER(typ_nm) LIKE '%'||LOWER(@typ_nm)||'%') ");
            sb.Append("ORDER BY t.typ_nm;");
            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var type_name = cmd.Parameters.Add("@typ_nm", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                type_name.Value = typeName;
                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        typeList.Add(new AssetType()
                        {
                            ID = reader["typ_id"] == DBNull.Value ? 0 : (int)reader["typ_id"],
                            Name = reader["typ_nm"] == DBNull.Value ? String.Empty : reader["typ_nm"].ToString(),
                            Description = reader["typ_ds"] == DBNull.Value ? String.Empty : reader["typ_ds"].ToString(),
                            GroupID = reader["grp_id"] == DBNull.Value ? (int?)null : (int)reader["grp_id"],
                            GroupName = reader["grp_nm"] == DBNull.Value ? string.Empty : reader["grp_nm"].ToString(),
                            ClassID = reader["clss_id"] == DBNull.Value ? 0 : (int)reader["clss_id"],
                            ClassName = reader["clss_nm"] == DBNull.Value ? String.Empty : reader["clss_nm"].ToString(),
                            CategoryID = reader["ctg_id"] == DBNull.Value ? 0 : (int)reader["ctg_id"],
                            CategoryName = reader["asst_ctgs_nm"] == DBNull.Value ? String.Empty : reader["asst_ctgs_nm"].ToString(),
                        });
                    }
            }
            await conn.CloseAsync();
            return typeList;
        }

        public async Task<IList<AssetType>> GetByCategoryIdAsync(int assetCategoryId)
        {
            List<AssetType> assetTypeList = new List<AssetType>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT t.typ_id, t.typ_nm, t.typ_ds, t.clss_id, l.clss_nm, ");
            sb.Append("t.ctg_id, c.asst_ctgs_nm, t.grp_id, g.grp_nm ");
            sb.Append("FROM public.asm_stt_typs t  ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ON t.ctg_id = c.asst_ctgs_id ");
            sb.Append("LEFT JOIN public.asm_stt_clss l ON l.clss_id = t.clss_id ");
            sb.Append("LEFT JOIN public.asm_stt_grps g ON g.grp_id = t.grp_id ");
            sb.Append("WHERE (t.ctg_id = @ctg_id) ");
            sb.Append("ORDER BY t.typ_nm;");
            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var asset_category_id = cmd.Parameters.Add("@ctg_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                asset_category_id.Value = assetCategoryId;
                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        assetTypeList.Add(new AssetType()
                        {
                            ID = reader["typ_id"] == DBNull.Value ? 0 : (int)reader["typ_id"],
                            Name = reader["typ_nm"] == DBNull.Value ? String.Empty : reader["typ_nm"].ToString(),
                            Description = reader["typ_ds"] == DBNull.Value ? String.Empty : reader["typ_ds"].ToString(),
                            GroupID = reader["grp_id"] == DBNull.Value ? (int?)null : (int)reader["grp_id"],
                            GroupName = reader["grp_nm"] == DBNull.Value ? string.Empty : reader["grp_nm"].ToString(),
                            ClassID = reader["clss_id"] == DBNull.Value ? 0 : (int)reader["clss_id"],
                            ClassName = reader["clss_nm"] == DBNull.Value ? String.Empty : reader["clss_nm"].ToString(),
                            CategoryID = reader["ctg_id"] == DBNull.Value ? 0 : (int)reader["ctg_id"],
                            CategoryName = reader["asst_ctgs_nm"] == DBNull.Value ? String.Empty : reader["asst_ctgs_nm"].ToString(),
                        });
                    }
            }
            await conn.CloseAsync();
            return assetTypeList;
        }

        public async Task<IList<AssetType>> GetByClassIdAsync(int assetClassId)
        {
            List<AssetType> assetTypeList = new List<AssetType>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT t.typ_id, t.typ_nm, t.typ_ds, t.clss_id, l.clss_nm, ");
            sb.Append("t.ctg_id, c.asst_ctgs_nm, t.grp_id, g.grp_nm ");
            sb.Append("FROM public.asm_stt_typs t  ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ON t.ctg_id = c.asst_ctgs_id ");
            sb.Append("LEFT JOIN public.asm_stt_clss l ON l.clss_id = t.clss_id ");
            sb.Append("LEFT JOIN public.asm_stt_grps g ON g.grp_id = t.grp_id ");
            sb.Append("WHERE (t.clss_id = @clss_id OR t.clss_id IS NULL) ");
            sb.Append("ORDER BY t.typ_nm;");
            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var asset_class_id = cmd.Parameters.Add("@clss_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                asset_class_id.Value = assetClassId;
                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        assetTypeList.Add(new AssetType()
                        {
                            ID = reader["typ_id"] == DBNull.Value ? 0 : (int)reader["typ_id"],
                            Name = reader["typ_nm"] == DBNull.Value ? String.Empty : reader["typ_nm"].ToString(),
                            Description = reader["typ_ds"] == DBNull.Value ? String.Empty : reader["typ_ds"].ToString(),
                            GroupID = reader["grp_id"] == DBNull.Value ? (int?)null : (int)reader["grp_id"],
                            GroupName = reader["grp_nm"] == DBNull.Value ? string.Empty : reader["grp_nm"].ToString(),
                            ClassID = reader["clss_id"] == DBNull.Value ? 0 : (int)reader["clss_id"],
                            ClassName = reader["clss_nm"] == DBNull.Value ? String.Empty : reader["clss_nm"].ToString(),
                            CategoryID = reader["ctg_id"] == DBNull.Value ? 0 : (int)reader["ctg_id"],
                            CategoryName = reader["asst_ctgs_nm"] == DBNull.Value ? String.Empty : reader["asst_ctgs_nm"].ToString(),
                        });
                    }
            }
            await conn.CloseAsync();
            return assetTypeList;
        }

        public async Task<IList<AssetType>> GetByGroupIdAsync(int assetGroupId)
        {
            List<AssetType> assetTypeList = new List<AssetType>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT t.typ_id, t.typ_nm, t.typ_ds, t.clss_id, t.grp_id, ");
            sb.Append("l.clss_nm, t.ctg_id, c.asst_ctgs_nm, g.grp_nm ");
            sb.Append("FROM public.asm_stt_typs t  ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ON t.ctg_id = c.asst_ctgs_id ");
            sb.Append("LEFT JOIN public.asm_stt_clss l ON l.clss_id = t.clss_id ");
            sb.Append("LEFT JOIN public.asm_stt_grps g ON g.grp_id = t.grp_id ");
            sb.Append("WHERE (t.grp_id = @grp_id) ");
            sb.Append("ORDER BY t.typ_nm;");
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
                        assetTypeList.Add(new AssetType()
                        {
                            ID = reader["typ_id"] == DBNull.Value ? 0 : (int)reader["typ_id"],
                            Name = reader["typ_nm"] == DBNull.Value ? String.Empty : reader["typ_nm"].ToString(),
                            Description = reader["typ_ds"] == DBNull.Value ? String.Empty : reader["typ_ds"].ToString(),
                            GroupID = reader["grp_id"] == DBNull.Value ? (int?)null : (int)reader["grp_id"],
                            GroupName = reader["grp_nm"] == DBNull.Value ? string.Empty : reader["grp_nm"].ToString(),
                            ClassID = reader["clss_id"] == DBNull.Value ? 0 : (int)reader["clss_id"],
                            ClassName = reader["clss_nm"] == DBNull.Value ? String.Empty : reader["clss_nm"].ToString(),
                            CategoryID = reader["ctg_id"] == DBNull.Value ? 0 : (int)reader["ctg_id"],
                            CategoryName = reader["asst_ctgs_nm"] == DBNull.Value ? String.Empty : reader["asst_ctgs_nm"].ToString(),
                        });
                    }
            }
            await conn.CloseAsync();
            return assetTypeList;
        }

        public async Task<IList<AssetType>> GetAllAsync()
        {
            List<AssetType> typesList = new List<AssetType>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT t.typ_id, t.typ_nm, t.typ_ds, t.clss_id, ");
            sb.Append("l.clss_nm, t.ctg_id, c.asst_ctgs_nm, t.grp_id, ");
            sb.Append("g.grp_nm  FROM public.asm_stt_typs t ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ON t.ctg_id = c.asst_ctgs_id ");
            sb.Append("LEFT JOIN public.asm_stt_clss l ON l.clss_id = t.clss_id ");
            sb.Append("LEFT JOIN public.asm_stt_grps g ON g.grp_id = t.grp_id ");
            sb.Append("ORDER BY t.typ_nm;");
            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                await cmd.PrepareAsync();
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    typesList.Add(new AssetType()
                    {
                        ID = reader["typ_id"] == DBNull.Value ? 0 : (int)reader["typ_id"],
                        Name = reader["typ_nm"] == DBNull.Value ? string.Empty : reader["typ_nm"].ToString(),
                        Description = reader["typ_ds"] == DBNull.Value ? String.Empty : reader["typ_ds"].ToString(),
                        GroupID = reader["grp_id"] == DBNull.Value ? (int?)null : (int)reader["grp_id"],
                        GroupName = reader["grp_nm"] == DBNull.Value ? string.Empty : reader["grp_nm"].ToString(),
                        ClassID = reader["clss_id"] == DBNull.Value ? 0 : (int)reader["clss_id"],
                        ClassName = reader["clss_nm"] == DBNull.Value ? String.Empty : reader["clss_nm"].ToString(),
                        CategoryID = reader["ctg_id"] == DBNull.Value ? 0 : (int)reader["ctg_id"],
                        CategoryName = reader["asst_ctgs_nm"] == DBNull.Value ? String.Empty : reader["asst_ctgs_nm"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return typesList;
        }
        #endregion

        #region AssetType Write Action Methods
        public async Task<bool> AddAsync(AssetType assetType)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.asm_stt_typs(typ_nm, typ_ds, ");
            sb.Append("ctg_id, clss_id, grp_id) VALUES (@typ_nm, ");
            sb.Append("@typ_ds, @ctg_id, @clss_id, @grp_id); ");
            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var typeName = cmd.Parameters.Add("@typ_nm", NpgsqlDbType.Text);
                var typeDescription = cmd.Parameters.Add("@typ_ds", NpgsqlDbType.Text);
                var categoryId = cmd.Parameters.Add("@ctg_id", NpgsqlDbType.Integer);
                var clss_id = cmd.Parameters.Add("@clss_id", NpgsqlDbType.Integer);
                var grp_id = cmd.Parameters.Add("@grp_id", NpgsqlDbType.Integer);
                cmd.Prepare();
                typeName.Value = assetType.Name;
                typeDescription.Value = assetType.Description ?? (object)DBNull.Value;
                categoryId.Value = assetType.CategoryID;
                clss_id.Value = assetType.ClassID;
                grp_id.Value = assetType.GroupID;

                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<bool> EditAsync(AssetType assetType)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.asm_stt_typs SET typ_nm = @typ_nm, ");
            sb.Append("typ_ds = @typ_ds, clss_id = @clss_id, ctg_id = @ctg_id, ");
            sb.Append("grp_id = @grp_id  WHERE (typ_id = @typ_id);  ");

            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var typeId = cmd.Parameters.Add("@typ_id", NpgsqlDbType.Integer);
                var categoryId = cmd.Parameters.Add("@ctg_id", NpgsqlDbType.Integer);
                var typeName = cmd.Parameters.Add("@typ_nm", NpgsqlDbType.Text);
                var typeDescription = cmd.Parameters.Add("@typ_ds", NpgsqlDbType.Text);
                var clss_id = cmd.Parameters.Add("@clss_id", NpgsqlDbType.Integer);
                var grp_id = cmd.Parameters.Add("@grp_id", NpgsqlDbType.Integer);
                cmd.Prepare();
                typeId.Value = assetType.ID;
                typeName.Value = assetType.Name;
                typeDescription.Value = assetType.Description ?? (object)DBNull.Value;
                categoryId.Value = assetType.CategoryID;
                clss_id.Value = assetType.ClassID;
                grp_id.Value = assetType.GroupID;

                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int assetTypeId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = $"DELETE FROM public.asm_stt_typs WHERE (typ_id = @typ_id);";

                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var type_id = cmd.Parameters.Add("@typ_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    type_id.Value = assetTypeId;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
            await conn.CloseAsync();
            return rows > 0;
        }

        #endregion
    }
}
