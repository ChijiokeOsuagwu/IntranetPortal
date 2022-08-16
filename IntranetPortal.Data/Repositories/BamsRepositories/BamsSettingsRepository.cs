using IntranetPortal.Base.Models.BamsModels;
using IntranetPortal.Base.Repositories.BamsRepositories;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Data.Repositories.BamsRepositories
{
    public class BamsSettingsRepository : IBamsSettingsRepository
    {
        public IConfiguration _config { get; }
        public BamsSettingsRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        //============== AssignmentEventType Action Methods =====================================================//
        #region AssignmentEventType Action Methods
        public async Task<IList<AssignmentEventType>> GetAllAssignmentEventTypesAsync()
        {
            IList<AssignmentEventType> assignmentEventTypes = new List<AssignmentEventType>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT evnt_typ_id, evnt_typ_ds FROM public.bam_evnt_typs ");
            sb.Append("ORDER BY evnt_typ_id ASC  ");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    await cmd.PrepareAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assignmentEventTypes.Add(new AssignmentEventType
                            {
                                ID = reader["evnt_typ_id"] == DBNull.Value ? 0 : (int)reader["evnt_typ_id"],
                                Description = reader["evnt_typ_ds"] == DBNull.Value ? string.Empty : reader["evnt_typ_ds"].ToString(),
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
            return assignmentEventTypes;
        }
        #endregion

        //============== AssignmentStatus Action Methods =====================================================//
        #region AssignmentStatus Action Methods
        public async Task<IList<AssignmentStatus>> GetAssignmentStatusByTypeAsync(string statusType)
        {
            IList<AssignmentStatus> assignmentStatusList = new List<AssignmentStatus>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT stts_id, stts_nm, stts_tp FROM public.bam_stt_stts ");
            sb.Append("WHERE (stts_tp = @stts_tp) ORDER BY stts_id ASC;");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var stts_tp = cmd.Parameters.Add("@stts_tp", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    stts_tp.Value = statusType;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assignmentStatusList.Add(new AssignmentStatus
                            {
                                ID = reader["stts_id"] == DBNull.Value ? 0 : (int)reader["stts_id"],
                                Description = reader["stts_nm"] == DBNull.Value ? string.Empty : reader["stts_nm"].ToString(),
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
            return assignmentStatusList;
        }
        #endregion
    }
}
