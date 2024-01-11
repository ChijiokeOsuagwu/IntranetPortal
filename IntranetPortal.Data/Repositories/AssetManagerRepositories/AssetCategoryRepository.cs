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
    public class AssetCategoryRepository : IAssetCategoryRepository
    {
        public IConfiguration _config { get; }
        public AssetCategoryRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        //============== Asset Category Action Methods =====================================================//
        #region AssetCategory Action Methods

        public async Task<AssetCategory> GetByIdAsync(int categoryId)
        {
            AssetCategory assetCategory = new AssetCategory();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = $"SELECT asst_ctgs_id, asst_ctgs_nm, asst_ctgs_ds FROM public.asm_stt_ctgs WHERE asst_ctgs_id = @asst_ctgs_id;";
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var category_id = cmd.Parameters.Add("@asst_ctgs_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    category_id.Value = categoryId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assetCategory.ID = reader["asst_ctgs_id"] == DBNull.Value ? 0 : (int)reader["asst_ctgs_id"];
                            assetCategory.Name = reader["asst_ctgs_nm"] == DBNull.Value ? String.Empty : reader["asst_ctgs_nm"].ToString();
                            assetCategory.Description = reader["asst_ctgs_ds"] == DBNull.Value ? String.Empty : reader["asst_ctgs_ds"].ToString();
                        }
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message);
            }
            return assetCategory;
        }

        public async Task<IList<AssetCategory>> GetByNameAsync(string categoryName)
        {
            List<AssetCategory> categoryList = new List<AssetCategory>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT asst_ctgs_id, asst_ctgs_nm, asst_ctgs_ds FROM public.asm_stt_ctgs ");
            sb.Append($"WHERE(LOWER(asst_ctgs_nm) LIKE '%'||LOWER(@asst_ctgs_nm)||'%') ORDER BY asst_ctgs_nm;");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var category_name = cmd.Parameters.Add("@asst_ctgs_nm", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    category_name.Value = categoryName;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            categoryList.Add(new AssetCategory()
                            {
                                ID = reader["asst_ctgs_id"] == DBNull.Value ? 0 : (int)reader["asst_ctgs_id"],
                                Name = reader["asst_ctgs_nm"] == DBNull.Value ? String.Empty : reader["asst_ctgs_nm"].ToString(),
                                Description = reader["asst_ctgs_ds"] == DBNull.Value ? String.Empty : reader["asst_ctgs_ds"].ToString(),
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
            return categoryList;
        }

        public async Task<IList<AssetCategory>> GetAllAsync()
        {
            List<AssetCategory> categoryList = new List<AssetCategory>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = $"SELECT asst_ctgs_id, asst_ctgs_nm, asst_ctgs_ds FROM public.asm_stt_ctgs ORDER BY asst_ctgs_nm";
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    await cmd.PrepareAsync();
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        categoryList.Add(new AssetCategory()
                        {
                            ID = reader["asst_ctgs_id"] == DBNull.Value ? 0 : (int)reader["asst_ctgs_id"],
                            Name = reader["asst_ctgs_nm"] == DBNull.Value ? String.Empty : reader["asst_ctgs_nm"].ToString(),
                            Description = reader["asst_ctgs_ds"] == DBNull.Value ? String.Empty : reader["asst_ctgs_ds"].ToString(),
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
            return categoryList;
        }

        public async Task<bool> AddAsync(AssetCategory assetCategory)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = $"INSERT INTO public.asm_stt_ctgs(asst_ctgs_nm, asst_ctgs_ds) VALUES (@asst_ctgs_nm, @asst_ctgs_ds);";
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var categoryName = cmd.Parameters.Add("@asst_ctgs_nm", NpgsqlDbType.Text);
                    var categoryDescription = cmd.Parameters.Add("@asst_ctgs_ds", NpgsqlDbType.Text);
                    cmd.Prepare();
                    categoryName.Value = assetCategory.Name;
                    categoryDescription.Value = assetCategory.Description ?? (object)DBNull.Value;
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

        public async Task<bool> EditAsync(AssetCategory assetCategory)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.asm_stt_ctgs SET asst_ctgs_nm=@asst_ctgs_nm, ");
            sb.Append("asst_ctgs_ds=@asst_ctgs_ds ");
            sb.Append("WHERE asst_ctgs_id=@asst_ctgs_id; ");
            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var categoryId = cmd.Parameters.Add("@asst_ctgs_id", NpgsqlDbType.Integer);
                var categoryName = cmd.Parameters.Add("@asst_ctgs_nm", NpgsqlDbType.Text);
                var categoryDescription = cmd.Parameters.Add("@asst_ctgs_ds", NpgsqlDbType.Text);
                cmd.Prepare();
                categoryId.Value = assetCategory.ID;
                categoryName.Value = assetCategory.Name;
                categoryDescription.Value = assetCategory.Description ?? (object)DBNull.Value;
                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int assetCategoryId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = $"DELETE FROM public.asm_stt_ctgs WHERE (asst_ctgs_id = @asst_ctgs_id);";
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var category_id = cmd.Parameters.Add("@asst_ctgs_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    category_id.Value = assetCategoryId;
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
