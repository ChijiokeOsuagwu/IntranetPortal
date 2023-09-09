using IntranetPortal.Base.Models.GlobalSettingsModels;
using IntranetPortal.Base.Enums;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Globalization;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IntranetPortal.Base.Repositories.GlobalSettingsRepositories;

namespace IntranetPortal.Data.Repositories.GlobalSettingsRepositories
{
    public class ProgramRepository : IProgramRepository
    {
        public IConfiguration _config { get; }
        public ProgramRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        #region Program Write Action Methods

        public async Task<bool> AddProgramAsync(Programme program)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.gst_prgms(prgm_ttl, prgm_ds, ");
            sb.Append("prgm_frq, prgm_sts, prgm_no, loc_id, start_time, ");
            sb.Append("end_time, platform, prgm_belt, prgm_typ) ");
            sb.Append("VALUES (@prgm_ttl, @prgm_ds, @prgm_frq, @prgm_sts, ");
            sb.Append("@prgm_no, @loc_id, @start_time, @end_time, ");
            sb.Append("@platform, @prgm_belt, @prgm_typ); ");
            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var prgm_ttl = cmd.Parameters.Add("@prgm_ttl", NpgsqlDbType.Text);
                var prgm_ds = cmd.Parameters.Add("@prgm_ds", NpgsqlDbType.Text);
                var prgm_frq = cmd.Parameters.Add("@prgm_frq", NpgsqlDbType.Integer);
                var prgm_sts = cmd.Parameters.Add("@prgm_sts", NpgsqlDbType.Integer);
                var prgm_no = cmd.Parameters.Add("@prgm_no", NpgsqlDbType.Text);
                var loc_id = cmd.Parameters.Add("@loc_id", NpgsqlDbType.Integer);
                var start_time = cmd.Parameters.Add("@start_time", NpgsqlDbType.Text);
                var end_time = cmd.Parameters.Add("@end_time", NpgsqlDbType.Text);
                var platform = cmd.Parameters.Add("@platform", NpgsqlDbType.Text);
                var prgm_belt = cmd.Parameters.Add("@prgm_belt", NpgsqlDbType.Text);
                var prgm_typ = cmd.Parameters.Add("@prgm_typ", NpgsqlDbType.Text);
                cmd.Prepare();
                prgm_ttl.Value = program.Title;
                prgm_ds.Value = program.Description ?? (object)DBNull.Value;
                prgm_frq.Value = (int)program.Frequency;
                prgm_sts.Value = (int)program.Status;
                prgm_no.Value = program.Code;
                loc_id.Value = program.HostStationId ?? (object)DBNull.Value;
                start_time.Value = program.StartTime ?? (object)DBNull.Value;
                end_time.Value = program.EndTime ?? (object)DBNull.Value;
                platform.Value = program.Platform ?? (object)DBNull.Value;
                prgm_belt.Value = program.ProgramBelt ?? (object)DBNull.Value;
                prgm_typ.Value = program.ProgramType ?? (object)DBNull.Value;

                //if(program.StartTime == null) { start_time.Value = (object)DBNull.Value; }
                //else { start_time.Value = TimeSpan.ParseExact(program.StartTime.Value.ToShortTimeString(), "HH:mm:ss", CultureInfo.InvariantCulture); }

                //else { start_time.Value = program.StartTime.Value.ToUniversalTime(); }
                //else {start_time.Value = DateTime.ParseExact(program.StartTime.ToString(), "HH:mm:ss", CultureInfo.InvariantCulture); }

                //if (program.EndTime == null) { end_time.Value = (object)DBNull.Value; }
                //else { end_time.Value = TimeSpan.ParseExact(program.EndTime.Value.ToShortTimeString(),"HH:mm:ss", CultureInfo.InvariantCulture); }

                //else { end_time.Value = program.EndTime.Value.ToUniversalTime(); }
                //else { end_time.Value = DateTime.ParseExact(program.EndTime.ToString(), "HH:mm:ss", CultureInfo.InvariantCulture); }

                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<bool> DeleteProgramAsync(int Id)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = $"DELETE FROM public.gst_prgms WHERE(prgm_id = @prgm_id);";

            await conn.OpenAsync();
            //Delete data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var prgm_id = cmd.Parameters.Add("@prgm_id", NpgsqlDbType.Integer);
                cmd.Prepare();
                prgm_id.Value = Id;
                rows = await cmd.ExecuteNonQueryAsync();
                await conn.CloseAsync();
            }

            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<bool> EditProgramAsync(Programme program)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.gst_prgms SET prgm_ttl=@prgm_ttl, ");
            sb.Append("prgm_ds=@prgm_ds, prgm_frq=@prgm_frq, prgm_sts=@prgm_sts, ");
            sb.Append("prgm_no=@prgm_no, loc_id=@loc_id, start_time=@start_time, ");
            sb.Append("end_time=@end_time, platform=@platform, prgm_belt=@prgm_belt, ");
            sb.Append("prgm_typ=@prgm_typ  WHERE(prgm_id = @prgm_id);");
            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var prgm_id = cmd.Parameters.Add("@prgm_id", NpgsqlDbType.Integer);
                var prgm_ttl = cmd.Parameters.Add("@prgm_ttl", NpgsqlDbType.Text);
                var prgm_ds = cmd.Parameters.Add("@prgm_ds", NpgsqlDbType.Text);
                var prgm_frq = cmd.Parameters.Add("@prgm_frq", NpgsqlDbType.Integer);
                var prgm_sts = cmd.Parameters.Add("@prgm_sts", NpgsqlDbType.Integer);
                var prgm_no = cmd.Parameters.Add("@prgm_no", NpgsqlDbType.Text);
                var loc_id = cmd.Parameters.Add("@loc_id", NpgsqlDbType.Integer);
                var start_time = cmd.Parameters.Add("@start_time", NpgsqlDbType.Text);
                var end_time = cmd.Parameters.Add("@end_time", NpgsqlDbType.Text);
                var platform = cmd.Parameters.Add("@platform", NpgsqlDbType.Text);
                var prgm_belt = cmd.Parameters.Add("@prgm_belt", NpgsqlDbType.Text);
                var prgm_typ = cmd.Parameters.Add("@prgm_typ", NpgsqlDbType.Text);
                cmd.Prepare();
                prgm_id.Value = program.Id;
                prgm_ttl.Value = program.Title;
                prgm_ds.Value = program.Description ?? (object)DBNull.Value;
                prgm_frq.Value = (int)program.Frequency;
                prgm_sts.Value = (int)program.Status;
                prgm_no.Value = program.Code;
                loc_id.Value = program.HostStationId ?? (object)DBNull.Value;
                start_time.Value = program.StartTime ?? (object)DBNull.Value;
                end_time.Value = program.EndTime ?? (object)DBNull.Value;
                platform.Value = program.Platform ?? (object)DBNull.Value;
                prgm_belt.Value = program.ProgramBelt ?? (object)DBNull.Value;
                prgm_typ.Value = program.ProgramType ?? (object)DBNull.Value;

                //if (program.StartTime == null) { start_time.Value = (object)DBNull.Value; }
                //else { start_time.Value = DateTime.ParseExact(program.StartTime.ToString(), "HH:mm:ss", CultureInfo.InvariantCulture); }

                //if (program.EndTime == null) { end_time.Value = (object)DBNull.Value; }
                //else { end_time.Value = DateTime.ParseExact(program.EndTime.ToString(), "HH:mm:ss", CultureInfo.InvariantCulture); }

                rows = await cmd.ExecuteNonQueryAsync();
            }

            await conn.CloseAsync();
            return rows > 0;
        }
        #endregion

        #region Program Read Action Methods
        public async Task<Programme> GetByIdAsync(int Id)
        {
            Programme program = new Programme();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            if (Id > 0)
            {
                sb.Append("SELECT p.prgm_id, p.prgm_ttl, p.prgm_ds, p.prgm_frq, ");
                sb.Append("p.prgm_sts, p.prgm_no, p.loc_id, p.start_time, p.end_time, ");
                sb.Append("p.platform, p.prgm_belt, p.prgm_typ, l.locname  ");
                sb.Append("FROM public.gst_prgms p ");
                sb.Append("LEFT JOIN public.gst_locs l ON l.locqk = p.loc_id ");
                sb.Append("WHERE (p.prgm_id = @prgm_id) ORDER BY p.prgm_ttl;");
                query = sb.ToString();

                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var prgm_id = cmd.Parameters.Add("@prgm_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    prgm_id.Value = Id;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            program.Id = reader["prgm_id"] == DBNull.Value ? 0 : (int)reader["prgm_id"];
                            program.Description = reader["prgm_ds"] == DBNull.Value ? string.Empty : reader["prgm_ds"].ToString();
                            program.Code = reader["prgm_no"] == DBNull.Value ? string.Empty : reader["prgm_no"].ToString();
                            program.Title = reader["prgm_ttl"] == DBNull.Value ? string.Empty : reader["prgm_ttl"].ToString();
                            program.Frequency = reader["prgm_frq"] == DBNull.Value ? ProgramFrequency.None : (ProgramFrequency)reader["prgm_frq"];
                            program.Status = reader["prgm_sts"] == DBNull.Value ? ProgramStatus.Running : (ProgramStatus)reader["prgm_sts"];
                            program.HostStationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"];
                            program.HostStationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString();
                            program.StartTime = reader["start_time"] == DBNull.Value ? string.Empty : reader["start_time"].ToString();
                            program.EndTime = reader["end_time"] == DBNull.Value ? string.Empty : reader["end_time"].ToString();
                            program.Platform = reader["platform"] == DBNull.Value ? string.Empty : reader["platform"].ToString();
                            program.ProgramBelt = reader["prgm_belt"] == DBNull.Value ? string.Empty : reader["prgm_belt"].ToString();
                            program.ProgramType = reader["prgm_typ"] == DBNull.Value ? string.Empty : reader["prgm_typ"].ToString();

                        }
                }
            }
            await conn.CloseAsync();
            return program;
        }

        public async Task<Programme> GetByCodeAsync(string programCode)
        {
            Programme program = new Programme();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(programCode))
            {
                sb.Append("SELECT p.prgm_id, p.prgm_ttl, p.prgm_ds, p.prgm_frq, ");
                sb.Append("p.prgm_sts, p.prgm_no, p.loc_id, p.start_time, p.end_time, ");
                sb.Append("p.platform, p.prgm_belt, p.prgm_typ, l.locname  ");
                sb.Append("FROM public.gst_prgms p ");
                sb.Append("LEFT JOIN public.gst_locs l ON l.locqk = p.loc_id ");
                sb.Append("WHERE (p.prgm_no = @prgm_no) ORDER BY p.prgm_ttl;");
                query = sb.ToString();

                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var prgm_no = cmd.Parameters.Add("@prgm_no", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    prgm_no.Value = programCode;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            program.Id = reader["prgm_id"] == DBNull.Value ? 0 : (int)reader["prgm_id"];
                            program.Description = reader["prgm_ds"] == DBNull.Value ? string.Empty : reader["prgm_ds"].ToString();
                            program.Code = reader["prgm_no"] == DBNull.Value ? string.Empty : reader["prgm_no"].ToString();
                            program.Title = reader["prgm_ttl"] == DBNull.Value ? string.Empty : reader["prgm_ttl"].ToString();
                            program.Frequency = reader["prgm_frq"] == DBNull.Value ? ProgramFrequency.None : (ProgramFrequency)reader["prgm_frq"];
                            program.Status = reader["prgm_sts"] == DBNull.Value ? ProgramStatus.Running : (ProgramStatus)reader["prgm_sts"];
                            program.HostStationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"];
                            program.HostStationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString();
                            program.StartTime = reader["start_time"] == DBNull.Value ? string.Empty : reader["start_time"].ToString();
                            program.EndTime = reader["end_time"] == DBNull.Value ? string.Empty : reader["end_time"].ToString();
                            program.Platform = reader["platform"] == DBNull.Value ? string.Empty : reader["platform"].ToString();
                            program.ProgramBelt = reader["prgm_belt"] == DBNull.Value ? string.Empty : reader["prgm_belt"].ToString();
                            program.ProgramType = reader["prgm_typ"] == DBNull.Value ? string.Empty : reader["prgm_typ"].ToString();

                        }
                }
            }
            await conn.CloseAsync();
            return program;
        }


        public async Task<IList<Programme>> GetAllAsync()
        {
            List<Programme> programList = new List<Programme>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT p.prgm_id, p.prgm_ttl, p.prgm_ds, p.prgm_frq, ");
            sb.Append("p.prgm_sts, p.prgm_no, p.loc_id, p.start_time, p.end_time, ");
            sb.Append("p.platform, p.prgm_belt, p.prgm_typ, l.locname  ");
            sb.Append("FROM public.gst_prgms p ");
            sb.Append("LEFT JOIN public.gst_locs l ON l.locqk = p.loc_id ");
            sb.Append("ORDER BY p.prgm_ttl;");

            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                await cmd.PrepareAsync();
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    programList.Add(new Programme()
                    {
                        Id = reader["prgm_id"] == DBNull.Value ? 0 : (int)reader["prgm_id"],
                        Description = reader["prgm_ds"] == DBNull.Value ? string.Empty : reader["prgm_ds"].ToString(),
                        Code = reader["prgm_no"] == DBNull.Value ? string.Empty : reader["prgm_no"].ToString(),
                        Title = reader["prgm_ttl"] == DBNull.Value ? string.Empty : reader["prgm_ttl"].ToString(),
                        Frequency = reader["prgm_frq"] == DBNull.Value ? ProgramFrequency.None : (ProgramFrequency)reader["prgm_frq"],
                        Status = reader["prgm_sts"] == DBNull.Value ? ProgramStatus.Running : (ProgramStatus)reader["prgm_sts"],
                        HostStationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                        HostStationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                        StartTime = reader["start_time"] == DBNull.Value ? string.Empty : reader["start_time"].ToString(), 
                        EndTime = reader["end_time"] == DBNull.Value ? string.Empty : reader["end_time"].ToString(), 
                        Platform = reader["platform"] == DBNull.Value ? string.Empty : reader["platform"].ToString(),
                        ProgramBelt = reader["prgm_belt"] == DBNull.Value ? string.Empty : reader["prgm_belt"].ToString(),
                        ProgramType = reader["prgm_typ"] == DBNull.Value ? string.Empty : reader["prgm_typ"].ToString(),
                    });
                }
            }

            await conn.CloseAsync();
            return programList;
        }

        public async Task<IList<Programme>> SearchByTitleAsync(string programTitle)
        {
            IList<Programme> programs = new List<Programme>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT p.prgm_id, p.prgm_ttl, p.prgm_ds, p.prgm_frq, ");
            sb.Append("p.prgm_sts, p.prgm_no, p.loc_id, p.start_time, p.end_time, ");
            sb.Append("p.platform, p.prgm_belt, p.prgm_typ, l.locname  ");
            sb.Append("FROM public.gst_prgms p ");
            sb.Append("LEFT JOIN public.gst_locs l ON l.locqk = p.loc_id ");
            sb.Append("WHERE LOWER(prgm_ttl) LIKE '%'||LOWER(@prgm_ttl)||'%') ");
            sb.Append("ORDER BY p.prgm_ttl;");

            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var prgm_ttl = cmd.Parameters.Add("@prgm_ttl", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                prgm_ttl.Value = programTitle;
                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        programs.Add(new Programme()
                        {
                            Id = reader["prgm_id"] == DBNull.Value ? 0 : (int)reader["prgm_id"],
                            Description = reader["prgm_ds"] == DBNull.Value ? string.Empty : reader["prgm_ds"].ToString(),
                            Code = reader["prgm_no"] == DBNull.Value ? string.Empty : reader["prgm_no"].ToString(),
                            Title = reader["prgm_ttl"] == DBNull.Value ? string.Empty : reader["prgm_ttl"].ToString(),
                            Frequency = reader["prgm_frq"] == DBNull.Value ? ProgramFrequency.None : (ProgramFrequency)reader["prgm_frq"],
                            Status = reader["prgm_sts"] == DBNull.Value ? ProgramStatus.Running : (ProgramStatus)reader["prgm_sts"],
                            HostStationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                            HostStationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            StartTime = reader["start_time"] == DBNull.Value ? string.Empty : reader["start_time"].ToString(),
                            EndTime = reader["end_time"] == DBNull.Value ? string.Empty : reader["end_time"].ToString(),
                            Platform = reader["platform"] == DBNull.Value ? string.Empty : reader["platform"].ToString(),
                            ProgramBelt = reader["prgm_belt"] == DBNull.Value ? string.Empty : reader["prgm_belt"].ToString(),
                            ProgramType = reader["prgm_typ"] == DBNull.Value ? string.Empty : reader["prgm_typ"].ToString(),
                        });
                    }
            }
            await conn.CloseAsync();
            return programs;
        }

        public async Task<IList<Programme>> GetByTitleAsync(string programTitle)
        {
            IList<Programme> programs = new List<Programme>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT p.prgm_id, p.prgm_ttl, p.prgm_ds, p.prgm_frq, ");
            sb.Append("p.prgm_sts, p.prgm_no, p.loc_id, p.start_time, p.end_time, ");
            sb.Append("p.platform, p.prgm_belt, p.prgm_typ, l.locname  ");
            sb.Append("FROM public.gst_prgms p ");
            sb.Append("LEFT JOIN public.gst_locs l ON l.locqk = p.loc_id ");
            sb.Append("WHERE LOWER(prgm_ttl) = LOWER(@prgm_ttl) ");
            sb.Append("ORDER BY p.prgm_ttl;");

            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var prgm_ttl = cmd.Parameters.Add("@prgm_ttl", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                prgm_ttl.Value = programTitle;
                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        programs.Add(new Programme()
                        {
                            Id = reader["prgm_id"] == DBNull.Value ? 0 : (int)reader["prgm_id"],
                            Description = reader["prgm_ds"] == DBNull.Value ? string.Empty : reader["prgm_ds"].ToString(),
                            Code = reader["prgm_no"] == DBNull.Value ? string.Empty : reader["prgm_no"].ToString(),
                            Title = reader["prgm_ttl"] == DBNull.Value ? string.Empty : reader["prgm_ttl"].ToString(),
                            Frequency = reader["prgm_frq"] == DBNull.Value ? ProgramFrequency.None : (ProgramFrequency)reader["prgm_frq"],
                            Status = reader["prgm_sts"] == DBNull.Value ? ProgramStatus.Running : (ProgramStatus)reader["prgm_sts"],
                            HostStationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                            HostStationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            StartTime = reader["start_time"] == DBNull.Value ? string.Empty : reader["start_time"].ToString(),
                            EndTime = reader["end_time"] == DBNull.Value ? string.Empty : reader["end_time"].ToString(),
                            Platform = reader["platform"] == DBNull.Value ? string.Empty : reader["platform"].ToString(),
                            ProgramBelt = reader["prgm_belt"] == DBNull.Value ? string.Empty : reader["prgm_belt"].ToString(),
                            ProgramType = reader["prgm_typ"] == DBNull.Value ? string.Empty : reader["prgm_typ"].ToString(),
                        });
                    }
            }
            await conn.CloseAsync();
            return programs;
        }

        public async Task<IList<Programme>> GetByProgramTypeAsync(string programType)
        {
            List<Programme> programList = new List<Programme>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT p.prgm_id, p.prgm_ttl, p.prgm_ds, p.prgm_frq, ");
            sb.Append("p.prgm_sts, p.prgm_no, p.loc_id, p.start_time, p.end_time, ");
            sb.Append("p.platform, p.prgm_belt, p.prgm_typ, l.locname  ");
            sb.Append("FROM public.gst_prgms p ");
            sb.Append("LEFT JOIN public.gst_locs l ON l.locqk = p.loc_id ");
            sb.Append("WHERE (p.prgm_typ = @prgm_typ) ");
            sb.Append("ORDER BY p.prgm_ttl;");

            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var prgm_typ = cmd.Parameters.Add("@prgm_typ", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                prgm_typ.Value = programType;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    programList.Add(new Programme()
                    {
                        Id = reader["prgm_id"] == DBNull.Value ? 0 : (int)reader["prgm_id"],
                        Description = reader["prgm_ds"] == DBNull.Value ? string.Empty : reader["prgm_ds"].ToString(),
                        Code = reader["prgm_no"] == DBNull.Value ? string.Empty : reader["prgm_no"].ToString(),
                        Title = reader["prgm_ttl"] == DBNull.Value ? string.Empty : reader["prgm_ttl"].ToString(),
                        Frequency = reader["prgm_frq"] == DBNull.Value ? ProgramFrequency.None : (ProgramFrequency)reader["prgm_frq"],
                        Status = reader["prgm_sts"] == DBNull.Value ? ProgramStatus.Running : (ProgramStatus)reader["prgm_sts"],
                        HostStationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                        HostStationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                        StartTime = reader["start_time"] == DBNull.Value ? string.Empty : reader["start_time"].ToString(),
                        EndTime = reader["end_time"] == DBNull.Value ? string.Empty : reader["end_time"].ToString(),
                        Platform = reader["platform"] == DBNull.Value ? string.Empty : reader["platform"].ToString(),
                        ProgramBelt = reader["prgm_belt"] == DBNull.Value ? string.Empty : reader["prgm_belt"].ToString(),
                        ProgramType = reader["prgm_typ"] == DBNull.Value ? string.Empty : reader["prgm_typ"].ToString(),
                    });
                }
            }

            await conn.CloseAsync();
            return programList;
        }

        public async Task<IList<Programme>> GetByProgramBeltAsync(string programBelt)
        {
            List<Programme> programList = new List<Programme>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT p.prgm_id, p.prgm_ttl, p.prgm_ds, p.prgm_frq, ");
            sb.Append("p.prgm_sts, p.prgm_no, p.loc_id, p.start_time, p.end_time, ");
            sb.Append("p.platform, p.prgm_belt, p.prgm_typ, l.locname  ");
            sb.Append("FROM public.gst_prgms p ");
            sb.Append("LEFT JOIN public.gst_locs l ON l.locqk = p.loc_id ");
            sb.Append("WHERE (p.prgm_belt = @prgm_belt) ");
            sb.Append("ORDER BY p.prgm_ttl;");

            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var prgm_belt = cmd.Parameters.Add("@prgm_belt", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                prgm_belt.Value = programBelt;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    programList.Add(new Programme()
                    {
                        Id = reader["prgm_id"] == DBNull.Value ? 0 : (int)reader["prgm_id"],
                        Description = reader["prgm_ds"] == DBNull.Value ? string.Empty : reader["prgm_ds"].ToString(),
                        Code = reader["prgm_no"] == DBNull.Value ? string.Empty : reader["prgm_no"].ToString(),
                        Title = reader["prgm_ttl"] == DBNull.Value ? string.Empty : reader["prgm_ttl"].ToString(),
                        Frequency = reader["prgm_frq"] == DBNull.Value ? ProgramFrequency.None : (ProgramFrequency)reader["prgm_frq"],
                        Status = reader["prgm_sts"] == DBNull.Value ? ProgramStatus.Running : (ProgramStatus)reader["prgm_sts"],
                        HostStationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                        HostStationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                        StartTime = reader["start_time"] == DBNull.Value ? string.Empty : reader["start_time"].ToString(),
                        EndTime = reader["end_time"] == DBNull.Value ? string.Empty : reader["end_time"].ToString(),
                        Platform = reader["platform"] == DBNull.Value ? string.Empty : reader["platform"].ToString(),
                        ProgramBelt = reader["prgm_belt"] == DBNull.Value ? string.Empty : reader["prgm_belt"].ToString(),
                        ProgramType = reader["prgm_typ"] == DBNull.Value ? string.Empty : reader["prgm_typ"].ToString(),

                    });
                }
            }
            await conn.CloseAsync();
            return programList;
        }

        public async Task<IList<Programme>> GetByProgramTypeAndProgramBeltAsync(string programType, string programBelt)
        {
            List<Programme> programList = new List<Programme>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT p.prgm_id, p.prgm_ttl, p.prgm_ds, p.prgm_frq, ");
            sb.Append("p.prgm_sts, p.prgm_no, p.loc_id, p.start_time, p.end_time, ");
            sb.Append("p.platform, p.prgm_belt, p.prgm_typ, l.locname  ");
            sb.Append("FROM public.gst_prgms p ");
            sb.Append("LEFT JOIN public.gst_locs l ON l.locqk = p.loc_id ");
            sb.Append("WHERE (p.prgm_typ = @prgm_typ) AND (p.prgm_belt = @prgm_belt) ");
            sb.Append("ORDER BY p.prgm_ttl;");

            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var prgm_typ = cmd.Parameters.Add("@prgm_typ", NpgsqlDbType.Text);
                var prgm_belt = cmd.Parameters.Add("@prgm_belt", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                prgm_typ.Value = programType;
                prgm_belt.Value = programBelt;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    programList.Add(new Programme()
                    {
                        Id = reader["prgm_id"] == DBNull.Value ? 0 : (int)reader["prgm_id"],
                        Description = reader["prgm_ds"] == DBNull.Value ? string.Empty : reader["prgm_ds"].ToString(),
                        Code = reader["prgm_no"] == DBNull.Value ? string.Empty : reader["prgm_no"].ToString(),
                        Title = reader["prgm_ttl"] == DBNull.Value ? string.Empty : reader["prgm_ttl"].ToString(),
                        Frequency = reader["prgm_frq"] == DBNull.Value ? ProgramFrequency.None : (ProgramFrequency)reader["prgm_frq"],
                        Status = reader["prgm_sts"] == DBNull.Value ? ProgramStatus.Running : (ProgramStatus)reader["prgm_sts"],
                        HostStationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                        HostStationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                        StartTime = reader["start_time"] == DBNull.Value ? string.Empty : reader["start_time"].ToString(),
                        EndTime = reader["end_time"] == DBNull.Value ? string.Empty : reader["end_time"].ToString(),
                        Platform = reader["platform"] == DBNull.Value ? string.Empty : reader["platform"].ToString(),
                        ProgramBelt = reader["prgm_belt"] == DBNull.Value ? string.Empty : reader["prgm_belt"].ToString(),
                        ProgramType = reader["prgm_typ"] == DBNull.Value ? string.Empty : reader["prgm_typ"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return programList;
        }

        #endregion

        #region Programme Belt Action Methds
        public async Task<IList<ProgrammeBelt>> GetAllProgrammeBeltsAsync()
        {
            List<ProgrammeBelt> programBeltList = new List<ProgrammeBelt>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT belt_id, belt_nm, start_time, end_time ");
            sb.Append("FROM public.gst_prgm_belt ");
            sb.Append("ORDER BY start_time;");

            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                await cmd.PrepareAsync();
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    programBeltList.Add(new ProgrammeBelt()
                    {
                        Id = reader["belt_id"] == DBNull.Value ? string.Empty : reader["belt_id"].ToString(),
                        Name = reader["belt_nm"] == DBNull.Value ? string.Empty : reader["belt_nm"].ToString(),
                        StartTime = reader["start_time"] == DBNull.Value ? string.Empty : reader["start_time"].ToString(),
                        EndTime = reader["end_time"] == DBNull.Value ? string.Empty : reader["end_time"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return programBeltList;
        }
        #endregion

        #region Programme Platform Action Methds
        public async Task<IList<ProgramPlatform>> GetAllProgramPlatformsAsync()
        {
            List<ProgramPlatform> programPlatformList = new List<ProgramPlatform>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT pltfm_id, pltfm_nm, pltfm_ds ");
            sb.Append("FROM public.gst_prgm_pltfm ");
            sb.Append("ORDER BY pltfm_nm;");

            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                await cmd.PrepareAsync();
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    programPlatformList.Add(new ProgramPlatform()
                    {
                        Id = reader["pltfm_id"] == DBNull.Value ? string.Empty : reader["pltfm_id"].ToString(),
                        Name = reader["pltfm_nm"] == DBNull.Value ? string.Empty : reader["pltfm_nm"].ToString(),
                        Description = reader["pltfm_ds"] == DBNull.Value ? string.Empty : reader["pltfm_ds"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return programPlatformList;
        }
        #endregion
    }
}
