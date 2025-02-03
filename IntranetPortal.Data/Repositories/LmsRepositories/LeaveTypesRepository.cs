using IntranetPortal.Base.Models.LmsModels;
using IntranetPortal.Base.Repositories.LmsRepositories;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Data.Repositories.LmsRepositories
{
    public class LeaveTypesRepository : ILeaveTypesRepository
    {
        public IConfiguration _config { get; }
        public LeaveTypesRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        //====== Leave Types Action Methods =======//
        #region Leave Types Action Methods

        public async Task<bool> AddAsync(LeaveType leaveType)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.lms_lvs_typs(lvs_typ_cd, ");
            sb.Append("lvs_typ_nm, lvs_typ_ds) VALUES (@lvs_typ_cd, ");
            sb.Append("@lvs_typ_nm, @lvs_typ_ds); ");
            string query = sb.ToString();

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_typ_cd = cmd.Parameters.Add("@lvs_typ_cd", NpgsqlDbType.Text);
                    var lvs_typ_nm = cmd.Parameters.Add("@lvs_typ_nm", NpgsqlDbType.Text);
                    var lvs_typ_ds = cmd.Parameters.Add("@lvs_typ_ds", NpgsqlDbType.Text);
                    cmd.Prepare();
                    lvs_typ_cd.Value = leaveType.Code;
                    lvs_typ_nm.Value = leaveType.Name;
                    lvs_typ_ds.Value = leaveType.Description ?? (object)DBNull.Value;
                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(string code)
        {
            int rows = 0;
            string query = "DELETE FROM public.lms_lvs_typs WHERE (lvs_typ_cd = @lvs_typ_cd);";
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Delete data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_typ_cd = cmd.Parameters.Add("@lvs_typ_cd", NpgsqlDbType.Text);
                    cmd.Prepare();
                    lvs_typ_cd.Value = code;
                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            return rows > 0;
        }

        public async Task<bool> EditAsync(LeaveType leaveType)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.lms_lvs_typs SET lvs_typ_nm=@lvs_typ_nm, ");
            sb.Append("lvs_typ_ds=@lvs_typ_ds WHERE (lvs_typ_cd=@lvs_typ_cd); ");
            string query = sb.ToString();

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_typ_cd = cmd.Parameters.Add("@lvs_typ_cd", NpgsqlDbType.Text);
                    var lvs_typ_nm = cmd.Parameters.Add("@lvs_typ_nm", NpgsqlDbType.Text);
                    var lvs_typ_ds = cmd.Parameters.Add("@lvs_typ_ds", NpgsqlDbType.Text);
                    cmd.Prepare();
                    lvs_typ_cd.Value = leaveType.Code;
                    lvs_typ_nm.Value = leaveType.Name;
                    lvs_typ_ds.Value = leaveType.Description ?? (object)DBNull.Value;
                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            return rows > 0;
        }

        public async Task<LeaveType> GetByCodeAsync(string code)
        {
            LeaveType leaveType = new LeaveType();
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            if (string.IsNullOrWhiteSpace(code)) { return null; }
            sb.Append("SELECT lvs_typ_cd, lvs_typ_nm, lvs_typ_ds ");
            sb.Append("FROM public.lms_lvs_typs ");
            sb.Append("WHERE (lvs_typ_cd = @lvs_typ_cd);");
            query = sb.ToString();

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_typ_cd = cmd.Parameters.Add("@lvs_typ_cd", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    lvs_typ_cd.Value = code;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            leaveType.Code = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : (reader["lvs_typ_cd"]).ToString();
                            leaveType.Name = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString();
                            leaveType.Description = reader["lvs_typ_ds"] == DBNull.Value ? string.Empty : reader["lvs_typ_ds"].ToString();
                        }
                }
                await conn.CloseAsync();
            }
            return leaveType;
        }

        public async Task<LeaveType> GetByNameAsync(string name)
        {
            LeaveType leaveType = new LeaveType();
            string query = string.Empty;
            StringBuilder sb = new StringBuilder();
            if (string.IsNullOrWhiteSpace(name)) { return null; }
            sb.Append("SELECT lvs_typ_cd, lvs_typ_nm, lvs_typ_ds ");
            sb.Append("FROM public.lms_lvs_typs ");
            sb.Append("WHERE LOWER(lvs_typ_nm) = LOWER(@lvs_typ_nm);");
            query = sb.ToString();

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_typ_nm = cmd.Parameters.Add("@lvs_typ_nm", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    lvs_typ_nm.Value = name;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            leaveType.Code = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : (reader["lvs_typ_cd"]).ToString();
                            leaveType.Name = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString();
                            leaveType.Description = reader["lvs_typ_ds"] == DBNull.Value ? string.Empty : reader["lvs_typ_ds"].ToString();
                        }
                }
                await conn.CloseAsync();
            }
            return leaveType;
        }

        public async Task<List<LeaveType>> GetAllAsync()
        {
            List<LeaveType> leaveTypes = new List<LeaveType>();
            string query = string.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT lvs_typ_cd, lvs_typ_nm, lvs_typ_ds ");
            sb.Append("FROM public.lms_lvs_typs ");
            sb.Append("ORDER BY lvs_typ_nm; ");
            query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    await cmd.PrepareAsync();
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        leaveTypes.Add(new LeaveType()
                        {
                            Code = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : (reader["lvs_typ_cd"]).ToString(),
                            Name = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
                            Description = reader["lvs_typ_ds"] == DBNull.Value ? string.Empty : reader["lvs_typ_ds"].ToString()
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveTypes;
        }

        public async Task<List<LeaveType>> GetAllExcludingSystemAsync()
        {
            List<LeaveType> leaveTypes = new List<LeaveType>();
            string query = string.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT lvs_typ_cd, lvs_typ_nm, lvs_typ_ds ");
            sb.Append("FROM public.lms_lvs_typs WHERE lvs_typ_sy = false ");
            sb.Append("ORDER BY lvs_typ_nm; ");
            query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    await cmd.PrepareAsync();
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        leaveTypes.Add(new LeaveType()
                        {
                            Code = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : (reader["lvs_typ_cd"]).ToString(),
                            Name = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
                            Description = reader["lvs_typ_ds"] == DBNull.Value ? string.Empty : reader["lvs_typ_ds"].ToString()
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveTypes;
        }
        #endregion

    }
}
