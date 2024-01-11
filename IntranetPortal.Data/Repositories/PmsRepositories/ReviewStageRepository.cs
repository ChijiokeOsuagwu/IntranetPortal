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
    public class ReviewStageRepository : IReviewStageRepository
    {
        public IConfiguration _config { get; }
        public ReviewStageRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        #region Review Stage Read Action Methods
        public async Task<IList<ReviewStage>> GetAllAsync()
        {
            List<ReviewStage> reviewStagesList = new List<ReviewStage>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT rvw_stg_id, rvw_stg_nm, stg_xtn_ds, stg_hlp_ds, ");
            sb.Append("stg_phs_ds FROM public.pmssttstgs ");
            sb.Append("ORDER BY rvw_stg_id;");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                await cmd.PrepareAsync();
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    reviewStagesList.Add(new ReviewStage()
                    {
                        ReviewStageId = reader["rvw_stg_id"] == DBNull.Value ? 0 : (int)(reader["rvw_stg_id"]),
                        ReviewStageName = reader["rvw_stg_nm"] == DBNull.Value ? string.Empty : reader["rvw_stg_nm"].ToString(),
                        ActionDescription = reader["stg_xtn_ds"] == DBNull.Value ? string.Empty : reader["stg_xtn_ds"].ToString(),
                        PhaseDescription = reader["stg_phs_ds"] == DBNull.Value ? string.Empty : reader["stg_phs_ds"].ToString(),
                        HelpInstruction = reader["stg_hlp_ds"] == DBNull.Value ? string.Empty : reader["stg_hlp_ds"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return reviewStagesList;
        }

        public async Task<IList<ReviewStage>> GetAllPreviousAsync(int currentStageId)
        {
            List<ReviewStage> reviewStagesList = new List<ReviewStage>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT rvw_stg_id, rvw_stg_nm, stg_xtn_ds, stg_hlp_ds, ");
            sb.Append("stg_phs_ds FROM public.pmssttstgs ");
            sb.Append("WHERE rvw_stg_id <= @rvw_stg_id ");
            sb.Append("ORDER BY rvw_stg_id;");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_stg_id = cmd.Parameters.Add("@rvw_stg_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                rvw_stg_id.Value = currentStageId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    reviewStagesList.Add(new ReviewStage()
                    {
                        ReviewStageId = reader["rvw_stg_id"] == DBNull.Value ? 0 : (int)(reader["rvw_stg_id"]),
                        ReviewStageName = reader["rvw_stg_nm"] == DBNull.Value ? string.Empty : reader["rvw_stg_nm"].ToString(),
                        ActionDescription = reader["stg_xtn_ds"] == DBNull.Value ? string.Empty : reader["stg_xtn_ds"].ToString(),
                        PhaseDescription = reader["stg_phs_ds"] == DBNull.Value ? string.Empty : reader["stg_phs_ds"].ToString(),
                        HelpInstruction = reader["stg_hlp_ds"] == DBNull.Value ? string.Empty : reader["stg_hlp_ds"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return reviewStagesList;
        }
        #endregion
    }
}
