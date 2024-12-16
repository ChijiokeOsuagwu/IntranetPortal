using IntranetPortal.Base.Models.EmployeeRecordModels;
using IntranetPortal.Base.Repositories.ErmRepositories;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Data.Repositories.ErmRepositories
{
    public class EmployeeOptionsRepository : IEmployeeOptionsRepository
    {
        public IConfiguration _config { get; }
        public EmployeeOptionsRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        public async Task<EmployeeOptions> GetAllAsync(string id)
        {
            EmployeeOptions e = new EmployeeOptions();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT e.emp_id, e.lvs_pfl_id, (SELECT fullname FROM ");
            sb.Append("public.gst_prsns WHERE id = e.emp_id) as emp_nm, ");
            sb.Append("(SELECT lvs_pfl_nm FROM public.lms_lvs_pfls ");
            sb.Append("WHERE lvs_pfl_id = e.lvs_pfl_id) as lvs_pfl_nm  ");
            sb.Append("FROM erm_emp_inf e ");
            sb.Append("WHERE(LOWER(e.emp_id) = LOWER(@emp_id)) AND (e.is_dx = false);");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                emp_id.Value = id;
                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        e.EmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString();
                        e.EmployeeFullName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString();
                        e.LeaveProfileId = reader["lvs_pfl_id"] == DBNull.Value ? 0 : (int)reader["lvs_pfl_id"];
                        e.LeaveProfileName = reader["lvs_pfl_nm"] == DBNull.Value ? string.Empty : reader["lvs_pfl_nm"].ToString();
                    }
            }
            await conn.CloseAsync();
            return e;
        }

        public async Task<bool> EditAsync(EmployeeOptions e)
        {
            if (e == null) { throw new ArgumentNullException(nameof(e), "The required parameter [Employee Options] is missing or has an invalid value."); }
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.erm_emp_inf SET lvs_pfl_id = @lvs_pfl_id ");
            sb.Append("WHERE (emp_id = @emp_id);");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                    var lvs_pfl_id = cmd.Parameters.Add("@lvs_pfl_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    emp_id.Value = e.EmployeeId;
                    lvs_pfl_id.Value = e.LeaveProfileId == 0 ? (object)DBNull.Value : e.LeaveProfileId;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
    }
}
