using IntranetPortal.Base.Models.PmsModels;
using IntranetPortal.Base.Repositories.PmsRepositories;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Data.Repositories.PmsRepositories
{
    public class PmsSystemRepository : IPmsSystemRepository
    {
        public IConfiguration _config { get; }
        public PmsSystemRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        public async Task<List<CompetencyCategory>> GetAllCompetencyCategoriesAsync()
        {
            List<CompetencyCategory> categoryList = new List<CompetencyCategory>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = "SELECT cmp_cat_id, cmp_cat_ds FROM public.pmscmpcats; ";
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                await cmd.PrepareAsync();

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    categoryList.Add(new CompetencyCategory()
                    {
                        Id = reader["cmp_cat_id"] == DBNull.Value ? 0 : (int)reader["cmp_cat_id"],
                        Description = reader["cmp_cat_ds"] == DBNull.Value ? string.Empty : reader["cmp_cat_ds"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return categoryList;
        }

        public async Task<List<CompetencyLevel>> GetAllCompetencyLevelsAsync()
        {
            List<CompetencyLevel> levelList = new List<CompetencyLevel>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = "SELECT cmp_lvl_id, cmp_lvl_ds FROM public.pmscmplvls;";
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                await cmd.PrepareAsync();

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    levelList.Add(new CompetencyLevel()
                    {
                        Id = reader["cmp_lvl_id"] == DBNull.Value ? 0 : (int)reader["cmp_lvl_id"],
                        Description = reader["cmp_lvl_ds"] == DBNull.Value ? string.Empty : reader["cmp_lvl_ds"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return levelList;
        }

    }
}
