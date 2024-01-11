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
    public class GradeHeaderRepository : IGradeHeaderRepository
    {
        public IConfiguration _config { get; }
        public GradeHeaderRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        public async Task<IList<GradeHeader>> GetAllAsync()
        {
            List<GradeHeader> gradeHeadersList = new List<GradeHeader>();
            var conn = new Npgsql.NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = "SELECT grd_hdr_id, grd_hdr_nm, grd_hdr_ds FROM public.pmsgrdhdrs; ";
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                await cmd.PrepareAsync();
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    gradeHeadersList.Add(new GradeHeader()
                    {
                        GradeHeaderId = reader["grd_hdr_id"] == DBNull.Value ? 0 : (int)(reader["grd_hdr_id"]),
                        GradeHeaderName = reader["grd_hdr_nm"] == DBNull.Value ? string.Empty : (reader["grd_hdr_nm"]).ToString(),
                        GradeHeaderDescription = reader["grd_hdr_ds"] == DBNull.Value ? string.Empty : (reader["grd_hdr_ds"]).ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return gradeHeadersList;
        }

        public async Task<IList<GradeHeader>> GetByIdAsync(int gradeHeaderId)
        {
            List<GradeHeader> gradeHeadersList = new List<GradeHeader>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT grd_hdr_id, grd_hdr_nm, grd_hdr_ds FROM public.pmsgrdhdrs ");
            sb.Append("WHERE (grd_hdr_id = @grd_hdr_id ); ");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var grd_hdr_id = cmd.Parameters.Add("@grd_hdr_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                grd_hdr_id.Value = gradeHeaderId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    gradeHeadersList.Add(new GradeHeader()
                    {
                        GradeHeaderId = reader["grd_hdr_id"] == DBNull.Value ? 0 : (int)(reader["grd_hdr_id"]),
                        GradeHeaderName = reader["grd_hdr_nm"] == DBNull.Value ? string.Empty : (reader["grd_hdr_nm"]).ToString(),
                        GradeHeaderDescription = reader["grd_hdr_ds"] == DBNull.Value ? string.Empty : (reader["grd_hdr_ds"]).ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return gradeHeadersList;
        }

        public async Task<IList<GradeHeader>> GetByNameAsync(string gradeHeaderName)
        {
            List<GradeHeader> gradeHeadersList = new List<GradeHeader>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT grd_hdr_id, grd_hdr_nm, grd_hdr_ds FROM public.pmsgrdhdrs ");
            sb.Append("WHERE (LOWER(grd_hdr_nm) = LOWER(@grd_hdr_nm)); ");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var grd_hdr_nm = cmd.Parameters.Add("@grd_hdr_nm", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                grd_hdr_nm.Value = gradeHeaderName;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    gradeHeadersList.Add(new GradeHeader()
                    {
                        GradeHeaderId = reader["grd_hdr_id"] == DBNull.Value ? 0 : (int)(reader["grd_hdr_id"]),
                        GradeHeaderName = reader["grd_hdr_nm"] == DBNull.Value ? string.Empty : (reader["grd_hdr_nm"]).ToString(),
                        GradeHeaderDescription = reader["grd_hdr_ds"] == DBNull.Value ? string.Empty : (reader["grd_hdr_ds"]).ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return gradeHeadersList;
        }

        public async Task<bool> AddAsync(GradeHeader gradeHeader)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.pmsgrdhdrs(grd_hdr_nm, grd_hdr_ds) ");
            sb.Append("VALUES (@grd_hdr_nm, @grd_hdr_ds);");
            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var grd_hdr_nm = cmd.Parameters.Add("@grd_hdr_nm", NpgsqlDbType.Text);
                var grd_hdr_ds = cmd.Parameters.Add("@grd_hdr_ds", NpgsqlDbType.Text);
                cmd.Prepare();
                grd_hdr_nm.Value = gradeHeader.GradeHeaderName;
                grd_hdr_ds.Value = gradeHeader.GradeHeaderDescription;

                rows = await cmd.ExecuteNonQueryAsync();
                await conn.CloseAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<bool> UpdateAsync(GradeHeader gradeHeader)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));

            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.pmsgrdhdrs SET grd_hdr_nm=@grd_hdr_nm, ");
            sb.Append("grd_hdr_ds=@grd_hdr_ds WHERE (grd_hdr_id=@grd_hdr_id); ");

            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var grd_hdr_id = cmd.Parameters.Add("@grd_hdr_id", NpgsqlDbType.Integer);
                var grd_hdr_nm = cmd.Parameters.Add("@grd_hdr_nm", NpgsqlDbType.Text);
                var grd_hdr_ds = cmd.Parameters.Add("@grd_hdr_ds", NpgsqlDbType.Text);
                cmd.Prepare();
                grd_hdr_id.Value = gradeHeader.GradeHeaderId;
                grd_hdr_nm.Value = gradeHeader.GradeHeaderName;
                grd_hdr_ds.Value = gradeHeader.GradeHeaderDescription;

                rows = await cmd.ExecuteNonQueryAsync();
                await conn.CloseAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int gradeHeaderId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));

            string query = "DELETE FROM public.pmsgrdhdrs WHERE (grd_hdr_id = @grd_hdr_id );";

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var grd_hdr_id = cmd.Parameters.Add("@grd_hdr_id", NpgsqlDbType.Integer);
                cmd.Prepare();
                grd_hdr_id.Value = gradeHeaderId;

                rows = await cmd.ExecuteNonQueryAsync();
                await conn.CloseAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

    }
}
