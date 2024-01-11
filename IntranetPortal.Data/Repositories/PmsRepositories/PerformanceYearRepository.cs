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
    public class PerformanceYearRepository : IPerformanceYearRepository
    {

        public IConfiguration _config { get; }
        public PerformanceYearRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        public async Task<IList<PerformanceYear>> GetAllAsync()
        {
            List<PerformanceYear> performanceYearsList = new List<PerformanceYear>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT pms_yr_id, pms_yr_nm, yr_str_dt, yr_ndt_dt, yr_ctt, yr_ctb, ");
            sb.Append("yr_mdt, yr_mdb FROM public.pmssttyrs ORDER BY yr_str_dt DESC; ");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                await cmd.PrepareAsync();
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    performanceYearsList.Add(new PerformanceYear()
                    {
                        Id = reader["pms_yr_id"] == DBNull.Value ? 0 : (int)(reader["pms_yr_id"]),
                        Name = reader["pms_yr_nm"] == DBNull.Value ? string.Empty : (reader["pms_yr_nm"]).ToString(),
                        StartDate = reader["yr_str_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["yr_str_dt"],
                        EndDate = reader["yr_ndt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["yr_ndt_dt"],
                        LastModifiedBy = reader["yr_mdb"] == DBNull.Value ? string.Empty : (reader["yr_mdb"]).ToString(),
                        LastModifiedTime = reader["yr_mdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["yr_mdt"],
                        CreatedBy = reader["yr_ctb"] == DBNull.Value ? string.Empty : (reader["yr_ctb"]).ToString(),
                        CreatedTime = reader["yr_ctt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["yr_ctt"],
                    });
                }
            }
            await conn.CloseAsync();
            return performanceYearsList;
        }

        public async Task<IList<PerformanceYear>> GetByIdAsync(int performanceYearId)
        {
            List<PerformanceYear> performanceYearsList = new List<PerformanceYear>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT pms_yr_id, pms_yr_nm, yr_str_dt, yr_ndt_dt, yr_ctt, yr_ctb, ");
            sb.Append("yr_mdt, yr_mdb FROM public.pmssttyrs WHERE (pms_yr_id = @pms_yr_id ); ");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var pms_yr_id = cmd.Parameters.Add("@pms_yr_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                pms_yr_id.Value = performanceYearId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    performanceYearsList.Add(new PerformanceYear()
                    {
                        Id = reader["pms_yr_id"] == DBNull.Value ? 0 : (int)(reader["pms_yr_id"]),
                        Name = reader["pms_yr_nm"] == DBNull.Value ? string.Empty : (reader["pms_yr_nm"]).ToString(),
                        StartDate = reader["yr_str_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["yr_str_dt"],
                        EndDate = reader["yr_ndt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["yr_ndt_dt"],
                        LastModifiedBy = reader["yr_mdb"] == DBNull.Value ? string.Empty : (reader["yr_mdb"]).ToString(),
                        LastModifiedTime = reader["yr_mdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["yr_mdt"],
                        CreatedBy = reader["yr_ctb"] == DBNull.Value ? string.Empty : (reader["yr_ctb"]).ToString(),
                        CreatedTime = reader["yr_ctt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["yr_ctt"],
                    });
                }
            }
            await conn.CloseAsync();
            return performanceYearsList;
        }

        public async Task<IList<PerformanceYear>> GetByNameAsync(string performanceYearName)
        {
            List<PerformanceYear> performanceYearsList = new List<PerformanceYear>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT pms_yr_id, pms_yr_nm, yr_str_dt, yr_ndt_dt, yr_ctt, yr_ctb, ");
            sb.Append("yr_mdt, yr_mdb FROM public.pmssttyrs WHERE (pms_yr_nm = @pms_yr_nm); ");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var pms_yr_nm = cmd.Parameters.Add("@pms_yr_nm", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                pms_yr_nm.Value = performanceYearName;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    performanceYearsList.Add(new PerformanceYear()
                    {
                        Id = reader["pms_yr_id"] == DBNull.Value ? 0 : (int)(reader["pms_yr_id"]),
                        Name = reader["pms_yr_nm"] == DBNull.Value ? string.Empty : (reader["pms_yr_nm"]).ToString(),
                        StartDate = reader["yr_str_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["yr_str_dt"],
                        EndDate = reader["yr_ndt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["yr_ndt_dt"],
                        LastModifiedBy = reader["yr_mdb"] == DBNull.Value ? string.Empty : (reader["yr_mdb"]).ToString(),
                        LastModifiedTime = reader["yr_mdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["yr_mdt"],
                        CreatedBy = reader["yr_ctb"] == DBNull.Value ? string.Empty : (reader["yr_ctb"]).ToString(),
                        CreatedTime = reader["yr_ctt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["yr_ctt"],
                    });
                }
            }
            await conn.CloseAsync();
            return performanceYearsList;
        }

        public async Task<IList<PerformanceYear>> GetByOverlappingDatesAsync(DateTime startDate, DateTime endDate)
        {
            List<PerformanceYear> performanceYearsList = new List<PerformanceYear>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT pms_yr_id, pms_yr_nm, yr_str_dt, yr_ndt_dt, yr_ctt, yr_ctb, ");
            sb.Append("yr_mdt, yr_mdb FROM public.pmssttyrs  ");
            sb.Append("WHERE (yr_str_dt = @yr_str_dt OR yr_str_dt < @yr_str_dt) ");
            sb.Append("AND (yr_ndt_dt = @yr_ndt_dt OR yr_ndt_dt > @yr_ndt_dt); ");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var yr_str_dt = cmd.Parameters.Add("@yr_str_dt", NpgsqlDbType.Date);
                var yr_ndt_dt = cmd.Parameters.Add("@yr_ndt_dt", NpgsqlDbType.Date);
                await cmd.PrepareAsync();
                yr_str_dt.Value = startDate;
                yr_ndt_dt.Value = endDate;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    performanceYearsList.Add(new PerformanceYear()
                    {
                        Id = reader["pms_yr_id"] == DBNull.Value ? 0 : (int)(reader["pms_yr_id"]),
                        Name = reader["pms_yr_nm"] == DBNull.Value ? string.Empty : (reader["pms_yr_nm"]).ToString(),
                        StartDate = reader["yr_str_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["yr_str_dt"],
                        EndDate = reader["yr_ndt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["yr_ndt_dt"],
                        LastModifiedBy = reader["yr_mdb"] == DBNull.Value ? string.Empty : (reader["yr_mdb"]).ToString(),
                        LastModifiedTime = reader["yr_mdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["yr_mdt"],
                        CreatedBy = reader["yr_ctb"] == DBNull.Value ? string.Empty : (reader["yr_ctb"]).ToString(),
                        CreatedTime = reader["yr_ctt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["yr_ctt"],
                    });
                }
            }
            await conn.CloseAsync();
            return performanceYearsList;
        }


        public async Task<bool> AddAsync(PerformanceYear performanceYear)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.pmssttyrs(pms_yr_nm, yr_str_dt, yr_ndt_dt, ");
            sb.Append("yr_ctt, yr_ctb, yr_mdt, yr_mdb) VALUES (@pms_yr_nm, @yr_str_dt, ");
            sb.Append("@yr_ndt_dt, @yr_ctt, @yr_ctb, @yr_mdt, @yr_mdb); ");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var pms_yr_nm = cmd.Parameters.Add("@pms_yr_nm", NpgsqlDbType.Text);
                    var yr_str_dt = cmd.Parameters.Add("@yr_str_dt", NpgsqlDbType.Date);
                    var yr_ndt_dt = cmd.Parameters.Add("@yr_ndt_dt", NpgsqlDbType.Date);
                    var yr_ctb = cmd.Parameters.Add("@yr_ctb", NpgsqlDbType.Text);
                    var yr_ctt = cmd.Parameters.Add("@yr_ctt", NpgsqlDbType.TimestampTz);
                    var yr_mdb = cmd.Parameters.Add("@yr_mdb", NpgsqlDbType.Text);
                    var yr_mdt = cmd.Parameters.Add("@yr_mdt", NpgsqlDbType.TimestampTz);

                    cmd.Prepare();

                    pms_yr_nm.Value = performanceYear.Name;
                    yr_str_dt.Value = performanceYear.StartDate ?? DateTime.UtcNow.Date;
                    yr_ndt_dt.Value = performanceYear.EndDate ?? DateTime.UtcNow.Date;
                    yr_ctb.Value = performanceYear.CreatedBy ?? (object)DBNull.Value;
                    yr_ctt.Value = performanceYear.CreatedTime ?? (object)DBNull.Value;
                    yr_mdb.Value = performanceYear.CreatedBy ?? (object)DBNull.Value;
                    yr_mdt.Value = performanceYear.CreatedTime ?? (object)DBNull.Value;

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

        public async Task<bool> UpdateAsync(PerformanceYear performanceYear)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));

            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.pmssttyrs SET pms_yr_nm=@pms_yr_nm,  ");
            sb.Append("yr_str_dt=@yr_str_dt, yr_ndt_dt=@yr_ndt_dt, yr_mdt=@yr_mdt, ");
            sb.Append("yr_mdb=@yr_mdb WHERE (pms_yr_id = @pms_yr_id ); ");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var pms_yr_nm = cmd.Parameters.Add("@pms_yr_nm", NpgsqlDbType.Text);
                    var yr_str_dt = cmd.Parameters.Add("@yr_str_dt", NpgsqlDbType.Date);
                    var yr_ndt_dt = cmd.Parameters.Add("@yr_ndt_dt", NpgsqlDbType.Date);
                    var yr_mdb = cmd.Parameters.Add("@yr_mdb", NpgsqlDbType.Text);
                    var yr_mdt = cmd.Parameters.Add("@yr_mdt", NpgsqlDbType.TimestampTz);
                    var pms_yr_id = cmd.Parameters.Add("@pms_yr_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    pms_yr_nm.Value = performanceYear.Name;
                    yr_str_dt.Value = performanceYear.StartDate ?? DateTime.UtcNow.Date;
                    yr_ndt_dt.Value = performanceYear.EndDate ?? DateTime.UtcNow.Date;
                    yr_mdb.Value = performanceYear.CreatedBy ?? (object)DBNull.Value;
                    yr_mdt.Value = performanceYear.CreatedTime ?? (object)DBNull.Value;
                    pms_yr_id.Value = performanceYear.Id;

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

        public async Task<bool> DeleteAsync(int performanceYearId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));

            string query = "DELETE FROM public.pmssttyrs WHERE (pms_yr_id = @pms_yr_id );";
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var pms_yr_id = cmd.Parameters.Add("@pms_yr_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    pms_yr_id.Value = performanceYearId;

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
    }
}
