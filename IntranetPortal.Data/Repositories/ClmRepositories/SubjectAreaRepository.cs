using IntranetPortal.Base.Models.ClmModels;
using IntranetPortal.Base.Repositories.ClmRepositories;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Data.Repositories.ClmRepositories
{
    public class SubjectAreaRepository:ISubjectAreaRepository
    {
        public IConfiguration _config { get; }
        public SubjectAreaRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        #region Subject Area Read Action Methods
        public async Task<IList<SubjectArea>> GetAllAsync()
        {
            List<SubjectArea> subjectAreasList = new List<SubjectArea>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT crs_sbjs_id, crs_sbjs_ds ");
            sb.Append("FROM public.clm_crs_sbjs ");
            sb.Append("ORDER BY crs_sbjs_ds;");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                await cmd.PrepareAsync();
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    subjectAreasList.Add(new SubjectArea()
                    {
                        SubjectAreaId = reader["crs_sbjs_id"] == DBNull.Value ? 0 : (int)(reader["crs_sbjs_id"]),
                        SubjectAreaDescription = reader["crs_sbjs_ds"] == DBNull.Value ? string.Empty : (reader["crs_sbjs_ds"]).ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return subjectAreasList;
        }

        public async Task<IList<SubjectArea>> GetByIdAsync(int subjectAreaId)
        {
            List<SubjectArea> subjectAreaList = new List<SubjectArea>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT crs_sbjs_id, crs_sbjs_ds ");
            sb.Append("FROM public.clm_crs_sbjs ");
            sb.Append("WHERE crs_sbjs_id = @crs_sbjs_id;");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var crs_sbjs_id = cmd.Parameters.Add("@crs_sbjs_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                crs_sbjs_id.Value = subjectAreaId;
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    subjectAreaList.Add(new SubjectArea()
                    {
                        SubjectAreaId = reader["crs_sbjs_id"] == DBNull.Value ? 0 : (int)(reader["crs_sbjs_id"]),
                        SubjectAreaDescription = reader["crs_sbjs_ds"] == DBNull.Value ? string.Empty : (reader["crs_sbjs_ds"]).ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return subjectAreaList;
        }

        public async Task<IList<SubjectArea>> GetByDescriptionAsync(string subjectAreaDescription)
        {
            List<SubjectArea> subjectAreasList = new List<SubjectArea>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT crs_sbjs_id, crs_sbjs_ds ");
            sb.Append("FROM public.clm_crs_sbjs ");
            sb.Append("WHERE LOWER(crs_sbjs_ds) = LOWER(@crs_sbjs_ds);");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var crs_sbjs_ds = cmd.Parameters.Add("@crs_sbjs_ds", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                crs_sbjs_ds.Value = subjectAreaDescription;
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    subjectAreasList.Add(new SubjectArea()
                    {
                        SubjectAreaId = reader["crs_sbjs_id"] == DBNull.Value ? 0 : (int)(reader["crs_sbjs_id"]),
                        SubjectAreaDescription = reader["crs_sbjs_ds"] == DBNull.Value ? string.Empty : (reader["crs_sbjs_ds"]).ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return subjectAreasList;
        }

        #endregion

        #region Subject Area Write Action Methods
        public async Task<bool> AddAsync(SubjectArea subjectArea)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.clm_crs_sbjs(crs_sbjs_ds) ");
            sb.Append("VALUES (@crs_sbjs_ds);");
            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var crs_sbjs_ds = cmd.Parameters.Add("@crs_sbjs_ds", NpgsqlDbType.Text);
                cmd.Prepare();
                crs_sbjs_ds.Value = subjectArea.SubjectAreaDescription;
                rows = await cmd.ExecuteNonQueryAsync();
                await conn.CloseAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<bool> UpdateAsync(SubjectArea subjectArea)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.clm_crs_sbjs SET crs_sbjs_ds=@crs_sbjs_ds ");
            sb.Append("WHERE (crs_sbjs_id=@crs_sbjs_id);");
            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var crs_sbjs_ds = cmd.Parameters.Add("@crs_sbjs_ds", NpgsqlDbType.Text);
                var crs_sbjs_id = cmd.Parameters.Add("@crs_sbjs_id", NpgsqlDbType.Integer);
                cmd.Prepare();
                crs_sbjs_ds.Value = subjectArea.SubjectAreaDescription;
                crs_sbjs_id.Value = subjectArea.SubjectAreaId;

                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int subjectAreaId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));

            string query = "DELETE FROM public.clm_crs_sbjs WHERE(crs_sbjs_id = @crs_sbjs_id);";

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var crs_sbjs_id = cmd.Parameters.Add("@crs_sbjs_id", NpgsqlDbType.Integer);
                cmd.Prepare();
                crs_sbjs_id.Value = subjectAreaId;

                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }
        #endregion
    }
}
