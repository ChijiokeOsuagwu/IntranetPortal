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
    public class AssetTypeRepository : IAssetTypeRepository
    {
        public IConfiguration _config { get; }
        public AssetTypeRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        //============== Asset Type Action Methods =====================================================//
        #region AssetType Action Methods

        public async Task<AssetType> GetByIdAsync(int assetTypeId)
        {
            AssetType assetType = new AssetType();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT t.typ_id, t.typ_nm, t.typ_ds, t.ctg_id, c.asst_ctgs_nm FROM public.asm_stt_typs t ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ON t.ctg_id = c.asst_ctgs_id WHERE (typ_id = @typ_id);");
            string query = sb.ToString();
            try
            {
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

                            assetType.CategoryID = reader["ctg_id"] == DBNull.Value ? 0 : (int)reader["ctg_id"];
                            assetType.CategoryName = reader["asst_ctgs_nm"] == DBNull.Value ? String.Empty : reader["asst_ctgs_nm"].ToString();
                        }
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message);
            }
            return assetType;
        }

        public async Task<IList<AssetType>> GetByNameAsync(string typeName)
        {
            List<AssetType> typeList = new List<AssetType>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT typ_id, typ_nm, typ_ds, ctg_id, asst_ctgs_nm FROM public.asm_stt_typs t ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ON t.ctg_id = c.asst_ctgs_id ");
            sb.Append($"WHERE(LOWER(typ_nm) LIKE '%'||LOWER(@typ_nm)||'%') ORDER BY typ_nm;");
            string query = sb.ToString();
            try
            {
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
                                CategoryID = reader["asst_ctgs_id"] == DBNull.Value ? 0 : (int)reader["asst_ctgs_id"],
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
            return typeList;
        }

        public async Task<IList<AssetType>> GetByCategoryIdAsync(int assetCategoryId)
        {
            List<AssetType> assetTypeList = new List<AssetType>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT typ_id, typ_nm, typ_ds, ctg_id, asst_ctgs_nm FROM public.asm_stt_typs t ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ON t.ctg_id = c.asst_ctgs_id WHERE (ctg_id = @ctg_id);");
            string query = sb.ToString();
            try
            {
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
                                CategoryID = reader["asst_ctgs_id"] == DBNull.Value ? 0 : (int)reader["asst_ctgs_id"],
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
            return assetTypeList;
        }

        public async Task<IList<AssetType>> GetAllAsync()
        {
            List<AssetType> typesList = new List<AssetType>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT typ_id, typ_nm, typ_ds, ctg_id, asst_ctgs_nm FROM public.asm_stt_typs t ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ON t.ctg_id = c.asst_ctgs_id ");
            sb.Append("ORDER BY asst_ctgs_nm, typ_nm;");
            string query = sb.ToString();
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
                        typesList.Add(new AssetType()
                        {
                            ID = reader["typ_id"] == DBNull.Value ? 0 : (int)reader["typ_id"],
                            Name = reader["typ_nm"] == DBNull.Value ? string.Empty : reader["typ_nm"].ToString(),
                            Description = reader["typ_ds"] == DBNull.Value ? String.Empty : reader["typ_ds"].ToString(),
                            CategoryID = reader["ctg_id"] == DBNull.Value ? 0 : (int)reader["ctg_id"],
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
            return typesList;
        }

        public async Task<bool> AddAsync(AssetType assetType)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = $"INSERT INTO public.asm_stt_typs(typ_nm, typ_ds, ctg_id) VALUES (@typ_nm, @typ_ds, @ctg_id);";
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var typeName = cmd.Parameters.Add("@typ_nm", NpgsqlDbType.Text);
                    var typeDescription = cmd.Parameters.Add("@typ_ds", NpgsqlDbType.Text);
                    var categoryId = cmd.Parameters.Add("@ctg_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    typeName.Value = assetType.Name;
                    typeDescription.Value = assetType.Description ?? (object)DBNull.Value;
                    categoryId.Value = assetType.CategoryID;

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

        public async Task<bool> EditAsync(AssetType assetType)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = $"UPDATE public.asm_stt_typs SET typ_nm = @typ_nm, typ_ds = @typ_ds, ctg_id = @ctg_id WHERE (typ_id = @typ_id);";
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var typeId = cmd.Parameters.Add("@typ_id", NpgsqlDbType.Integer);
                    var categoryId = cmd.Parameters.Add("@ctg_id", NpgsqlDbType.Integer);
                    var typeName = cmd.Parameters.Add("@typ_nm", NpgsqlDbType.Text);
                    var typeDescription = cmd.Parameters.Add("@typ_ds", NpgsqlDbType.Text);
                    cmd.Prepare();
                    typeId.Value = assetType.ID;
                    typeName.Value = assetType.Name;
                    typeDescription.Value = assetType.Description ?? (object)DBNull.Value;
                    categoryId.Value = assetType.CategoryID;
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

        public async Task<bool> DeleteAsync(int assetTypeId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = $"DELETE FROM public.asm_stt_typs WHERE (typ_id = @typ_id);";
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var type_id = cmd.Parameters.Add("@typ_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    type_id.Value = assetTypeId;
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
