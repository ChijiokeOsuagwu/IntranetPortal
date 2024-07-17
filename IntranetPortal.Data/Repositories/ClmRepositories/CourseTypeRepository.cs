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
    public class CourseTypeRepository: ICourseTypeRepository
    {
        public IConfiguration _config { get; }
        public CourseTypeRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        #region Course Type Read Action Methods
        public async Task<IList<CourseType>> GetAllAsync()
        {
            List<CourseType> courseTypesList = new List<CourseType>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT crs_typ_id, crs_typ_ds ");
            sb.Append("FROM public.clm_crs_typs ");
            sb.Append("ORDER BY crs_typ_ds;");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                await cmd.PrepareAsync();
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    courseTypesList.Add(new CourseType()
                    {
                        CourseTypeId = reader["crs_typ_id"] == DBNull.Value ? 0 : (int)(reader["crs_typ_id"]),
                        CourseTypeDescription = reader["crs_typ_ds"] == DBNull.Value ? string.Empty : (reader["crs_typ_ds"]).ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return courseTypesList;
        }

        public async Task<IList<CourseType>> GetByIdAsync(int courseTypeId)
        {
            List<CourseType> courseTypesList = new List<CourseType>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT crs_typ_id, crs_typ_ds ");
            sb.Append("FROM public.clm_crs_typs ");
            sb.Append("WHERE crs_typ_id = @crs_typ_id;");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var crs_typ_id = cmd.Parameters.Add("@crs_typ_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                crs_typ_id.Value = courseTypeId;
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    courseTypesList.Add(new CourseType()
                    {
                        CourseTypeId = reader["crs_typ_id"] == DBNull.Value ? 0 : (int)(reader["crs_typ_id"]),
                        CourseTypeDescription = reader["crs_typ_ds"] == DBNull.Value ? string.Empty : (reader["crs_typ_ds"]).ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return courseTypesList;
        }

        public async Task<IList<CourseType>> GetByDescriptionAsync(string courseTypeDescription)
        {
            List<CourseType> courseTypesList = new List<CourseType>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT crs_typ_id, crs_typ_ds ");
            sb.Append("FROM public.clm_crs_typs ");
            sb.Append("WHERE LOWER(crs_typ_ds) = LOWER(@crs_typ_ds);");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var crs_typ_ds = cmd.Parameters.Add("@crs_typ_ds", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                crs_typ_ds.Value = courseTypeDescription;
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    courseTypesList.Add(new CourseType()
                    {
                        CourseTypeId = reader["crs_typ_id"] == DBNull.Value ? 0 : (int)(reader["crs_typ_id"]),
                        CourseTypeDescription = reader["crs_typ_ds"] == DBNull.Value ? string.Empty : (reader["crs_typ_ds"]).ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return courseTypesList;
        }

        #endregion

        #region Course Type Write Action Methods
        public async Task<bool> AddAsync(CourseType courseType)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.clm_crs_typs(crs_typ_ds) ");
            sb.Append("VALUES (@crs_typ_ds);");
            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var crs_typ_ds = cmd.Parameters.Add("@crs_typ_ds", NpgsqlDbType.Text);
                cmd.Prepare();
                crs_typ_ds.Value = courseType.CourseTypeDescription;
                rows = await cmd.ExecuteNonQueryAsync();
                await conn.CloseAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<bool> UpdateAsync(CourseType courseType)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.clm_crs_typs SET crs_typ_ds=@crs_typ_ds ");
            sb.Append("WHERE (crs_typ_id=@crs_typ_id);");
            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var crs_typ_ds = cmd.Parameters.Add("@crs_typ_ds", NpgsqlDbType.Text);
                var crs_typ_id = cmd.Parameters.Add("@crs_typ_id", NpgsqlDbType.Integer);
                cmd.Prepare();
                crs_typ_ds.Value = courseType.CourseTypeDescription;
                crs_typ_id.Value = courseType.CourseTypeId;

                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int courseTypeId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));

            string query = "DELETE FROM public.clm_crs_typs WHERE(crs_typ_id = @crs_typ_id);";

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var crs_typ_id = cmd.Parameters.Add("@crs_typ_id", NpgsqlDbType.Integer);
                cmd.Prepare();
                crs_typ_id.Value = courseTypeId;

                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }
        #endregion
    }
}
