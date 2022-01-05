using IntranetPortal.Base.Models.GlobalSettingsModels;
using IntranetPortal.Base.Repositories.GlobalSettingsRepositories;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Data.Repositories.GlobalSettingsRepositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        public IConfiguration _config { get; }
        public DepartmentRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        //============= Department Action Methods =====================================//
        #region Department Action Methods

        public async Task<bool> AddDepartmentAsync(Department department)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = "INSERT INTO public.gst_depts(deptname, depthd1, depthd2) VALUES (@deptname, @depthd1, @depthd2);";
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var deptname = cmd.Parameters.Add("@deptname", NpgsqlDbType.Text);
                    var depthd1 = cmd.Parameters.Add("@depthd1", NpgsqlDbType.Text);
                    var depthd2 = cmd.Parameters.Add("@depthd2", NpgsqlDbType.Text);
                    cmd.Prepare();
                    deptname.Value = department.DepartmentName;
                    depthd1.Value = department.DepartmentHeadID1 ?? (object)DBNull.Value;
                    depthd2.Value = department.DepartmentHeadID2 ?? (object)DBNull.Value;
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

        public async Task<bool> DeleteDepartmentAsync(int Id)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = $"DELETE FROM public.gst_depts WHERE (deptqk = @deptqk);;";
            try
            {
                await conn.OpenAsync();
                //Delete data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var deptqk = cmd.Parameters.Add("@deptqk", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    deptqk.Value = Id;
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

        public async Task<bool> EditDepartmentAsync(Department department)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = $"UPDATE public.gst_depts SET deptname=@deptname, depthd1=@depthd1, depthd2=@depthd2 WHERE (deptqk=@deptqk);";
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var deptqk = cmd.Parameters.Add("@deptqk", NpgsqlDbType.Integer);
                    var deptname = cmd.Parameters.Add("@deptname", NpgsqlDbType.Text);
                    var depthd1 = cmd.Parameters.Add("@depthd1", NpgsqlDbType.Text);
                    var depthd2 = cmd.Parameters.Add("@depthd2", NpgsqlDbType.Text);
                    cmd.Prepare();
                    deptqk.Value = department.DepartmentID;
                    deptname.Value = department.DepartmentName;
                    depthd1.Value = department.DepartmentHeadID1 ?? (object)DBNull.Value;
                    depthd2.Value = department.DepartmentHeadID2 ?? (object)DBNull.Value;
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

        public async Task<Department> GetDepartmentByIdAsync(int Id)
        {
            Department department = new Department();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            if (Id < 1) { return null; }
            sb.Append($"SELECT deptname, depthd1, depthd2, deptqk, p1.fullname AS depthd1_name, p2.fullname AS depthd2_name ");
            sb.Append($"FROM public.gst_depts d LEFT OUTER JOIN gst_prsns p1 ON d.depthd1 = p1.id LEFT OUTER JOIN gst_prsns p2 ");
            sb.Append($"ON d.depthd2 = p2.id WHERE deptqk = @deptqk;");
            query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var deptqk = cmd.Parameters.Add("@deptqk", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    deptqk.Value = Id;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            department.DepartmentID = reader["deptqk"] == DBNull.Value ? 0 : (int)(reader["deptqk"]);
                            department.DepartmentName = reader["deptname"] == DBNull.Value ? String.Empty : reader["deptname"].ToString();
                            department.DepartmentHeadID1 = reader["depthd1"] == DBNull.Value ? String.Empty : reader["depthd1"].ToString();
                            department.DepartmentHeadID2 = reader["depthd2"] == DBNull.Value ? String.Empty : reader["depthd2"].ToString();
                            department.DepartmentHeadName1 = reader["depthd1_name"] == DBNull.Value ? String.Empty : reader["depthd1_name"].ToString();
                            department.DepartmentHeadName2 = reader["depthd2_name"] == DBNull.Value ? String.Empty : reader["depthd2_name"].ToString();
                        }
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message);
            }
            return department;
        }

        public async Task<IList<Department>> GetDepartmentsAsync()
        {
            List<Department> deptList = new List<Department>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append($"SELECT deptname, depthd1, depthd2, deptqk, p1.fullname AS depthd1_name, p2.fullname AS depthd2_name ");
            sb.Append($"FROM public.gst_depts d LEFT OUTER JOIN gst_prsns p1 ON d.depthd1 = p1.id LEFT OUTER JOIN gst_prsns p2 ");
            sb.Append($"ON d.depthd2 = p2.id ORDER BY deptname ASC;");
            query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    await cmd.PrepareAsync();
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        deptList.Add(new Department()
                        {
                            DepartmentID = reader["deptqk"] == DBNull.Value ? 0 : (int)(reader["deptqk"]),
                        DepartmentName = reader["deptname"] == DBNull.Value ? String.Empty : reader["deptname"].ToString(),
                        DepartmentHeadID1 = reader["depthd1"] == DBNull.Value ? String.Empty : reader["depthd1"].ToString(),
                        DepartmentHeadID2 = reader["depthd2"] == DBNull.Value ? String.Empty : reader["depthd2"].ToString(),
                        DepartmentHeadName1 = reader["depthd1_name"] == DBNull.Value ? String.Empty : reader["depthd1_name"].ToString(),
                        DepartmentHeadName2 = reader["depthd2_name"] == DBNull.Value ? String.Empty : reader["depthd2_name"].ToString(),
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
            return deptList;
        }


        #endregion
    }
}
