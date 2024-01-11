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
    public class CompetencyRepository : ICompetencyRepository
    {
        public IConfiguration _config { get; }
        public CompetencyRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        #region Competency Read Action Methods
        public async Task<List<Competency>> GetByAllAsync()
        {
            List<Competency> competencyList = new List<Competency>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT c.pms_cmp_id, c.pms_cmp_nm, c.pms_cmp_ds, c.cmp_cat_id, ");
            sb.Append("c.cmp_lvl_id, t.cmp_cat_ds, ");
            sb.Append("l.cmp_lvl_ds FROM public.pmssttcmps c ");
            sb.Append("LEFT JOIN public.pmscmpcats t ON t.cmp_cat_id = c.cmp_cat_id ");
            sb.Append("LEFT JOIN public.pmscmplvls l ON l.cmp_lvl_id = c.cmp_lvl_id ");
            sb.Append("ORDER BY t.cmp_cat_id, c.cmp_lvl_id, c.pms_cmp_nm;");

            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                await cmd.PrepareAsync();

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    competencyList.Add(new Competency()
                    {
                        Id = reader["pms_cmp_id"] == DBNull.Value ? 0 : (int)reader["pms_cmp_id"],
                        Title = reader["pms_cmp_nm"] == DBNull.Value ? string.Empty : reader["pms_cmp_nm"].ToString(),
                        Description = reader["pms_cmp_ds"] == DBNull.Value ? string.Empty : reader["pms_cmp_ds"].ToString(),
                        CategoryId = reader["cmp_cat_id"] == DBNull.Value ? 0 : (int)reader["cmp_cat_id"],
                        CategoryDescription = reader["cmp_cat_ds"] == DBNull.Value ? string.Empty : reader["cmp_cat_ds"].ToString(),
                        LevelId = reader["cmp_lvl_id"] == DBNull.Value ? 0 : (int)reader["cmp_lvl_id"],
                        LevelDescription = reader["cmp_lvl_ds"] == DBNull.Value ? string.Empty : reader["cmp_lvl_ds"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return competencyList;
        }

        public async Task<List<Competency>> GetByIdAsync(int competencyId)
        {
            List<Competency> competencyList = new List<Competency>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT c.pms_cmp_id, c.pms_cmp_nm, c.pms_cmp_ds, c.cmp_cat_id, ");
            sb.Append("c.cmp_lvl_id, t.cmp_cat_ds, ");
            sb.Append("l.cmp_lvl_ds FROM public.pmssttcmps c ");
            sb.Append("LEFT JOIN public.pmscmpcats t ON t.cmp_cat_id = c.cmp_cat_id ");
            sb.Append("LEFT JOIN public.pmscmplvls l ON l.cmp_lvl_id = c.cmp_lvl_id ");
            sb.Append("WHERE (c.pms_cmp_id = @pms_cmp_id); ");

            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var pms_cmp_id = cmd.Parameters.Add("@pms_cmp_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                pms_cmp_id.Value = competencyId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    competencyList.Add(new Competency()
                    {
                        Id = reader["pms_cmp_id"] == DBNull.Value ? 0 : (int)reader["pms_cmp_id"],
                        Title = reader["pms_cmp_nm"] == DBNull.Value ? string.Empty : reader["pms_cmp_nm"].ToString(),
                        Description = reader["pms_cmp_ds"] == DBNull.Value ? string.Empty : reader["pms_cmp_ds"].ToString(),
                        CategoryId = reader["cmp_cat_id"] == DBNull.Value ? 0 : (int)reader["cmp_cat_id"],
                        CategoryDescription = reader["cmp_cat_ds"] == DBNull.Value ? string.Empty : reader["cmp_cat_ds"].ToString(),
                        LevelId = reader["cmp_lvl_id"] == DBNull.Value ? 0 : (int)reader["cmp_lvl_id"],
                        LevelDescription = reader["cmp_lvl_ds"] == DBNull.Value ? string.Empty : reader["cmp_lvl_ds"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return competencyList;
        }

        public async Task<List<Competency>> GetByCategoryIdAsync(int categoryId)
        {
            List<Competency> competencyList = new List<Competency>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT c.pms_cmp_id, c.pms_cmp_nm, c.pms_cmp_ds, c.cmp_cat_id, ");
            sb.Append("c.cmp_lvl_id, t.cmp_cat_ds, ");
            sb.Append("l.cmp_lvl_ds FROM public.pmssttcmps c ");
            sb.Append("LEFT JOIN public.pmscmpcats t ON t.cmp_cat_id = c.cmp_cat_id ");
            sb.Append("LEFT JOIN public.pmscmplvls l ON l.cmp_lvl_id = c.cmp_lvl_id ");
            sb.Append("WHERE (c.cmp_cat_id = @cmp_cat_id) ");
            sb.Append("ORDER BY t.cmp_cat_id, c.cmp_lvl_id, c.pms_cmp_nm;");

            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var cmp_cat_id = cmd.Parameters.Add("@cmp_cat_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                cmp_cat_id.Value = categoryId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    competencyList.Add(new Competency()
                    {
                        Id = reader["pms_cmp_id"] == DBNull.Value ? 0 : (int)reader["pms_cmp_id"],
                        Title = reader["pms_cmp_nm"] == DBNull.Value ? string.Empty : reader["pms_cmp_nm"].ToString(),
                        Description = reader["pms_cmp_ds"] == DBNull.Value ? string.Empty : reader["pms_cmp_ds"].ToString(),
                        CategoryId = reader["cmp_cat_id"] == DBNull.Value ? 0 : (int)reader["cmp_cat_id"],
                        CategoryDescription = reader["cmp_cat_ds"] == DBNull.Value ? string.Empty : reader["cmp_cat_ds"].ToString(),
                        LevelId = reader["cmp_lvl_id"] == DBNull.Value ? 0 : (int)reader["cmp_lvl_id"],
                        LevelDescription = reader["cmp_lvl_ds"] == DBNull.Value ? string.Empty : reader["cmp_lvl_ds"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return competencyList;
        }

        public async Task<List<Competency>> GetByLevelIdAsync(int levelId)
        {
            List<Competency> competencyList = new List<Competency>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT c.pms_cmp_id, c.pms_cmp_nm, c.pms_cmp_ds, c.cmp_cat_id, ");
            sb.Append("c.cmp_lvl_id, t.cmp_cat_ds, ");
            sb.Append("l.cmp_lvl_ds FROM public.pmssttcmps c ");
            sb.Append("LEFT JOIN public.pmscmpcats t ON t.cmp_cat_id = c.cmp_cat_id ");
            sb.Append("LEFT JOIN public.pmscmplvls l ON l.cmp_lvl_id = c.cmp_lvl_id ");
            sb.Append("WHERE (c.cmp_lvl_id = @cmp_lvl_id) ");
            sb.Append("ORDER BY t.cmp_cat_id, c.cmp_lvl_id, c.pms_cmp_nm;");

            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var cmp_lvl_id = cmd.Parameters.Add("@cmp_lvl_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                cmp_lvl_id.Value = levelId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    competencyList.Add(new Competency()
                    {
                        Id = reader["pms_cmp_id"] == DBNull.Value ? 0 : (int)reader["pms_cmp_id"],
                        Title = reader["pms_cmp_nm"] == DBNull.Value ? string.Empty : reader["pms_cmp_nm"].ToString(),
                        Description = reader["pms_cmp_ds"] == DBNull.Value ? string.Empty : reader["pms_cmp_ds"].ToString(),
                        CategoryId = reader["cmp_cat_id"] == DBNull.Value ? 0 : (int)reader["cmp_cat_id"],
                        CategoryDescription = reader["cmp_cat_ds"] == DBNull.Value ? string.Empty : reader["cmp_cat_ds"].ToString(),
                        LevelId = reader["cmp_lvl_id"] == DBNull.Value ? 0 : (int)reader["cmp_lvl_id"],
                        LevelDescription = reader["cmp_lvl_ds"] == DBNull.Value ? string.Empty : reader["cmp_lvl_ds"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return competencyList;
        }

        public async Task<List<Competency>> GetByCategoryIdAndLevelIdAsync(int categoryId, int levelId)
        {
            List<Competency> competencyList = new List<Competency>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT c.pms_cmp_id, c.pms_cmp_nm, c.pms_cmp_ds, c.cmp_cat_id, ");
            sb.Append("c.cmp_lvl_id, t.cmp_cat_ds, ");
            sb.Append("l.cmp_lvl_ds FROM public.pmssttcmps c ");
            sb.Append("LEFT JOIN public.pmscmpcats t ON t.cmp_cat_id = c.cmp_cat_id ");
            sb.Append("LEFT JOIN public.pmscmplvls l ON l.cmp_lvl_id = c.cmp_lvl_id ");
            sb.Append("WHERE (c.cmp_cat_id = @cmp_cat_id) AND (c.cmp_lvl_id = @cmp_lvl_id) ");
            sb.Append("ORDER BY t.cmp_cat_id, c.cmp_lvl_id, c.pms_cmp_nm;");

            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var cmp_cat_id = cmd.Parameters.Add("@cmp_cat_id", NpgsqlDbType.Integer);
                var cmp_lvl_id = cmd.Parameters.Add("@cmp_lvl_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                cmp_cat_id.Value = categoryId;
                cmp_lvl_id.Value = levelId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    competencyList.Add(new Competency()
                    {
                        Id = reader["pms_cmp_id"] == DBNull.Value ? 0 : (int)reader["pms_cmp_id"],
                        Title = reader["pms_cmp_nm"] == DBNull.Value ? string.Empty : reader["pms_cmp_nm"].ToString(),
                        Description = reader["pms_cmp_ds"] == DBNull.Value ? string.Empty : reader["pms_cmp_ds"].ToString(),
                        CategoryId = reader["cmp_cat_id"] == DBNull.Value ? 0 : (int)reader["cmp_cat_id"],
                        CategoryDescription = reader["cmp_cat_ds"] == DBNull.Value ? string.Empty : reader["cmp_cat_ds"].ToString(),
                        LevelId = reader["cmp_lvl_id"] == DBNull.Value ? 0 : (int)reader["cmp_lvl_id"],
                        LevelDescription = reader["cmp_lvl_ds"] == DBNull.Value ? string.Empty : reader["cmp_lvl_ds"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return competencyList;
        }

        #endregion

        #region Competency Write Action Methods
        public async Task<bool> AddAsync(Competency competency)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.pmssttcmps(pms_cmp_nm, pms_cmp_ds, ");
            sb.Append("cmp_cat_id, cmp_lvl_id ) VALUES (@pms_cmp_nm, ");
            sb.Append("@pms_cmp_ds, @cmp_cat_id, @cmp_lvl_id);");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var pms_cmp_nm = cmd.Parameters.Add("@pms_cmp_nm", NpgsqlDbType.Text);
                    var pms_cmp_ds = cmd.Parameters.Add("@pms_cmp_ds", NpgsqlDbType.Text);
                    var cmp_cat_id = cmd.Parameters.Add("@cmp_cat_id", NpgsqlDbType.Integer);
                    var cmp_lvl_id = cmd.Parameters.Add("@cmp_lvl_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    pms_cmp_nm.Value = competency.Title;
                    pms_cmp_ds.Value = competency.Description;
                    cmp_cat_id.Value = competency.CategoryId ?? (object)DBNull.Value;
                    cmp_lvl_id.Value = competency.LevelId ?? (object)DBNull.Value;
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

        public async Task<bool> UpdateAsync(Competency competency)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.pmssttcmps SET pms_cmp_nm=@pms_cmp_nm, ");
            sb.Append("pms_cmp_ds=@pms_cmp_ds, cmp_cat_id=@cmp_cat_id, ");
            sb.Append("cmp_lvl_id=@cmp_lvl_id ");
            sb.Append("WHERE (pms_cmp_id=@pms_cmp_id);");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var pms_cmp_nm = cmd.Parameters.Add("@pms_cmp_nm", NpgsqlDbType.Text);
                    var pms_cmp_ds = cmd.Parameters.Add("@pms_cmp_ds", NpgsqlDbType.Text);
                    var cmp_cat_id = cmd.Parameters.Add("@cmp_cat_id", NpgsqlDbType.Integer);
                    var cmp_lvl_id = cmd.Parameters.Add("@cmp_lvl_id", NpgsqlDbType.Integer);
                    var pms_cmp_id = cmd.Parameters.Add("@pms_cmp_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    pms_cmp_nm.Value = competency.Title;
                    pms_cmp_ds.Value = competency.Description;
                    cmp_cat_id.Value = competency.CategoryId ?? (object)DBNull.Value;
                    cmp_lvl_id.Value = competency.LevelId ?? (object)DBNull.Value;
                    pms_cmp_id.Value = competency.Id;

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

        public async Task<bool> DeleteAsync(int competencyId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("DELETE FROM public.pmssttcmps ");
            sb.Append("WHERE (pms_cmp_id = @pms_cmp_id);");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var pms_cmp_id = cmd.Parameters.Add("@pms_cmp_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    pms_cmp_id.Value = competencyId;

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
