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
    public class PerformanceSettingsRepository : IPerformanceSettingsRepository
    {
        public IConfiguration _config { get; }
        public PerformanceSettingsRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        public async Task<IList<ReviewType>> GetAllReviewTypesAsync()
        {
            List<ReviewType> reviewTypesList = new List<ReviewType>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT rvw_typ_id, rvw_typ_nm FROM public.pmsrvwtyps ");
            sb.Append("ORDER BY rvw_typ_id;");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                await cmd.PrepareAsync();
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    reviewTypesList.Add(new ReviewType()
                    {
                        ReviewTypeId = reader["rvw_typ_id"] == DBNull.Value ? 0 : (int)(reader["rvw_typ_id"]),
                        ReviewTypeName = reader["rvw_typ_nm"] == DBNull.Value ? string.Empty : (reader["rvw_typ_nm"]).ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return reviewTypesList;
        }

        public async Task<IList<AppraisalRecommendation>> GetAllRecommendationsAsync()
        {
            List<AppraisalRecommendation> recommendationsList = new List<AppraisalRecommendation>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT pms_rcmd_id, pms_rcmd_nm FROM public.pmssttrcmds ");
            sb.Append("ORDER BY pms_rcmd_id;");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                await cmd.PrepareAsync();
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    recommendationsList.Add(new AppraisalRecommendation()
                    {
                        Id = reader["pms_rcmd_id"] == DBNull.Value ? 0 : (int)(reader["pms_rcmd_id"]),
                        Description = reader["pms_rcmd_nm"] == DBNull.Value ? string.Empty : (reader["pms_rcmd_nm"]).ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return recommendationsList;
        }
    }
}
