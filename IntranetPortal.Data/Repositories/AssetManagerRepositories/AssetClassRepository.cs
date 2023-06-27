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
    public class AssetClassRepository : IAssetClassRepository
    {
        public IConfiguration _config { get; }
        public AssetClassRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        //============== Asset Class Action Methods =====================================================//
        #region AssetClass Action Methods

        public async Task<AssetClass> GetByIdAsync(int assetClassId)
        {
            AssetClass assetClass = new AssetClass();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT clss_id, clss_nm, clss_ds, ctg_id, g.asst_ctgs_nm ");
            sb.Append("FROM public.asm_stt_clss c INNER JOIN public.asm_stt_ctgs g ");
            sb.Append("ON g.asst_ctgs_id = c.ctg_id ");
            sb.Append("WHERE clss_id = @clss_id");
            string query = sb.ToString();
            try
            {
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
                            assetClass.ID = reader["clss_id"] == DBNull.Value ? 0 : (int)reader["clss_id"];
                            assetClass.Name = reader["clss_nm"] == DBNull.Value ? String.Empty : reader["clss_nm"].ToString();
                            assetClass.Description = reader["clss_ds"] == DBNull.Value ? String.Empty : reader["clss_ds"].ToString();
                            assetClass.CategoryID = reader["ctg_id"] == DBNull.Value ? 0 : (int)reader["ctg_id"];
                            assetClass.CategoryName = reader["asst_ctgs_nm"] == DBNull.Value ? String.Empty : reader["asst_ctgs_nm"].ToString();
                        }
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message);
            }
            return assetClass;
        }

        public async Task<IList<AssetClass>> GetByNameAsync(string className)
        {
            List<AssetClass> classList = new List<AssetClass>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT c.clss_id, c.clss_nm, c.clss_ds, c.ctg_id, g.asst_ctgs_nm ");
            sb.Append("FROM public.asm_stt_clss c INNER JOIN public.asm_stt_ctgs g ");
            sb.Append("ON g.asst_ctgs_id = c.ctg_id ");
            sb.Append("WHERE(LOWER(c.clss_nm) LIKE '%'||LOWER(@clss_nm)||'%')  ");
            sb.Append("ORDER BY c.clss_nm;");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var clss_nm = cmd.Parameters.Add("@clss_nm", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    clss_nm.Value = className;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            classList.Add(new AssetClass()
                            {
                                ID = reader["clss_id"] == DBNull.Value ? 0 : (int)reader["clss_id"],
                                Name = reader["clss_nm"] == DBNull.Value ? String.Empty : reader["clss_nm"].ToString(),
                                Description = reader["clss_ds"] == DBNull.Value ? String.Empty : reader["clss_ds"].ToString(),
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
            return classList;
        }

        public async Task<IList<AssetClass>> GetByNameAsync(string className, IEntityPermission entityPermission)
        {
            List<AssetClass> classList = new List<AssetClass>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT c.clss_id, c.clss_nm, c.clss_ds, c.ctg_id, g.asst_ctgs_nm ");
            sb.Append("FROM public.asm_stt_clss c INNER JOIN public.asm_stt_ctgs g ");
            sb.Append("ON g.asst_ctgs_id = c.ctg_id ");
            sb.Append("WHERE(LOWER(c.clss_nm) LIKE '%'||LOWER(@clss_nm)||'%')  ");
            sb.Append("AND c.ctg_id IN (SELECT ass_cat_id FROM  public.sct_entt_pms ");
            sb.Append("WHERE usr_id = @usr_id) ");
            sb.Append("ORDER BY c.clss_nm;");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var clss_nm = cmd.Parameters.Add("@clss_nm", NpgsqlDbType.Text);
                    var usr_id = cmd.Parameters.Add("@usr_id", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    clss_nm.Value = className;
                    usr_id.Value = entityPermission.UserId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            classList.Add(new AssetClass()
                            {
                                ID = reader["clss_id"] == DBNull.Value ? 0 : (int)reader["clss_id"],
                                Name = reader["clss_nm"] == DBNull.Value ? String.Empty : reader["clss_nm"].ToString(),
                                Description = reader["clss_ds"] == DBNull.Value ? String.Empty : reader["clss_ds"].ToString(),
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
            return classList;
        }

        public async Task<IList<AssetClass>> GetByCategoryIdAsync(int assetCategoryId)
        {
            List<AssetClass> assetClassList = new List<AssetClass>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT c.clss_id, c.clss_nm, c.clss_ds, c.ctg_id, g.asst_ctgs_nm ");
            sb.Append("FROM public.asm_stt_clss c INNER JOIN public.asm_stt_ctgs g ");
            sb.Append("ON g.asst_ctgs_id = c.ctg_id ");
            sb.Append("WHERE (c.ctg_id = @ctg_id) ");
            sb.Append("ORDER BY c.clss_nm;");

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
                            assetClassList.Add(new AssetClass()
                            {
                                ID = reader["clss_id"] == DBNull.Value ? 0 : (int)reader["clss_id"],
                                Name = reader["clss_nm"] == DBNull.Value ? String.Empty : reader["clss_nm"].ToString(),
                                Description = reader["clss_ds"] == DBNull.Value ? String.Empty : reader["clss_ds"].ToString(),
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
            return assetClassList;
        }

        public async Task<IList<AssetClass>> GetByCategoryIdAsync(int assetCategoryId, IEntityPermission entityPermission)
        {
            List<AssetClass> assetClassList = new List<AssetClass>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT c.clss_id, c.clss_nm, c.clss_ds, c.ctg_id, g.asst_ctgs_nm ");
            sb.Append("FROM public.asm_stt_clss c INNER JOIN public.asm_stt_ctgs g ");
            sb.Append("ON g.asst_ctgs_id = c.ctg_id ");
            sb.Append("WHERE (c.ctg_id = @ctg_id) ");
            sb.Append("AND c.ctg_id IN (SELECT ass_cat_id FROM  public.sct_entt_pms ");
            sb.Append("WHERE usr_id = @usr_id) ");
            sb.Append("ORDER BY c.clss_nm;");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var asset_category_id = cmd.Parameters.Add("@ctg_id", NpgsqlDbType.Integer);
                    var usr_id = cmd.Parameters.Add("@usr_id", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    asset_category_id.Value = assetCategoryId;
                    usr_id.Value = entityPermission.UserId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assetClassList.Add(new AssetClass()
                            {
                                ID = reader["clss_id"] == DBNull.Value ? 0 : (int)reader["clss_id"],
                                Name = reader["clss_nm"] == DBNull.Value ? String.Empty : reader["clss_nm"].ToString(),
                                Description = reader["clss_ds"] == DBNull.Value ? String.Empty : reader["clss_ds"].ToString(),
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
            return assetClassList;
        }

        public async Task<IList<AssetClass>> GetAllAsync()
        {
            List<AssetClass> classList = new List<AssetClass>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT c.clss_id, c.clss_nm, c.clss_ds, c.ctg_id, g.asst_ctgs_nm ");
            sb.Append("FROM public.asm_stt_clss c INNER JOIN public.asm_stt_ctgs g ");
            sb.Append("ON g.asst_ctgs_id = c.ctg_id ");
            sb.Append("ORDER BY g.asst_ctgs_nm, c.clss_nm;");
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
                        classList.Add(new AssetClass()
                        {
                            ID = reader["clss_id"] == DBNull.Value ? 0 : (int)reader["clss_id"],
                            Name = reader["clss_nm"] == DBNull.Value ? String.Empty : reader["clss_nm"].ToString(),
                            Description = reader["clss_ds"] == DBNull.Value ? String.Empty : reader["clss_ds"].ToString(),
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
            return classList;
        }

        public async Task<IList<AssetClass>> GetAllAsync(IEntityPermission entityPermission)
        {
            List<AssetClass> classList = new List<AssetClass>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT c.clss_id, c.clss_nm, c.clss_ds, c.ctg_id, g.asst_ctgs_nm ");
            sb.Append("FROM public.asm_stt_clss c INNER JOIN public.asm_stt_ctgs g ");
            sb.Append("ON g.asst_ctgs_id = c.ctg_id ");
            sb.Append("AND c.ctg_id IN (SELECT ass_cat_id FROM public.sct_entt_pms ");
            sb.Append("WHERE usr_id = @usr_id) ");
            sb.Append("ORDER BY g.asst_ctgs_nm, c.clss_nm;");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var usr_id = cmd.Parameters.Add("@usr_id", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    usr_id.Value = entityPermission.UserId;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        classList.Add(new AssetClass()
                        {
                            ID = reader["clss_id"] == DBNull.Value ? 0 : (int)reader["clss_id"],
                            Name = reader["clss_nm"] == DBNull.Value ? String.Empty : reader["clss_nm"].ToString(),
                            Description = reader["clss_ds"] == DBNull.Value ? String.Empty : reader["clss_ds"].ToString(),
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
            return classList;
        }

        public async Task<bool> AddAsync(AssetClass assetClass)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.asm_stt_clss(clss_nm, clss_ds, ctg_id) ");
            sb.Append(" VALUES (@clss_nm, @clss_ds, @ctg_id); ");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var clss_nm = cmd.Parameters.Add("@clss_nm", NpgsqlDbType.Text);
                    var clss_ds = cmd.Parameters.Add("@clss_ds", NpgsqlDbType.Text);
                    var ctg_id = cmd.Parameters.Add("@ctg_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    clss_nm.Value = assetClass.Name;
                    clss_ds.Value = assetClass.Description ?? (object)DBNull.Value;
                    ctg_id.Value = assetClass.CategoryID;

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

        public async Task<bool> EditAsync(AssetClass assetClass)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.asm_stt_clss SET clss_nm=@clss_nm, clss_ds=@clss_ds, ");
            sb.Append("ctg_id=@ctg_id WHERE(clss_id = @clss_id);");
            string query = sb.ToString(); try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var clss_nm = cmd.Parameters.Add("@clss_nm", NpgsqlDbType.Text);
                    var clss_ds = cmd.Parameters.Add("@clss_ds", NpgsqlDbType.Text);
                    var ctg_id = cmd.Parameters.Add("@ctg_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    clss_nm.Value = assetClass.Name;
                    clss_ds.Value = assetClass.Description ?? (object)DBNull.Value;
                    ctg_id.Value = assetClass.CategoryID;

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

        public async Task<bool> DeleteAsync(int assetClassId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = $"DELETE FROM public.asm_stt_clss WHERE (clss_id = @clss_id);";
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var clss_id = cmd.Parameters.Add("@clss_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    clss_id.Value = assetClassId;
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
