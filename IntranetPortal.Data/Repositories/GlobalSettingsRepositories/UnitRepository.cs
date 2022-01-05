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
    public class UnitRepository : IUnitRepository
    {
        public IConfiguration _config { get; }
        public UnitRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        //============= Unit Action Methods =====================================//
        #region Unit Action Methods

        public async Task<bool> AddUnitAsync(Unit unit)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = $"INSERT INTO public.gst_units(unitname, unithd1, unithd2, deptqk) VALUES (@unitname, @unithd1, @unithd2, @deptqk);";
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var unitname = cmd.Parameters.Add("@unitname", NpgsqlDbType.Text);
                    var unithd1 = cmd.Parameters.Add("@unithd1", NpgsqlDbType.Text);
                    var unithd2 = cmd.Parameters.Add("@unithd2", NpgsqlDbType.Text);
                    var deptqk = cmd.Parameters.Add("@deptqk", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    unitname.Value = unit.UnitName;
                    unithd1.Value = unit.UnitHeadID1 ?? (object)DBNull.Value;
                    unithd2.Value = unit.UnitHeadID2 ?? (object)DBNull.Value;
                    deptqk.Value = unit.DepartmentID;
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

        public async Task<bool> DeleteUnitAsync(int Id)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = $"DELETE FROM public.gst_units WHERE (unitqk = @unitqk);;";
            try
            {
                await conn.OpenAsync();
                //Delete data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var unitqk = cmd.Parameters.Add("@unitqk", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    unitqk.Value = Id;
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

        public async Task<bool> EditUnitAsync(Unit unit)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = $"UPDATE public.gst_units	SET unitname=@unitname, unithd1=@unithd1, unithd2=@unithd2, deptqk=@deptqk WHERE (unitqk=@unitqk)";
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var unitqk = cmd.Parameters.Add("@unitqk", NpgsqlDbType.Integer);
                    var deptqk = cmd.Parameters.Add("@deptqk", NpgsqlDbType.Integer);
                    var unitname = cmd.Parameters.Add("@unitname", NpgsqlDbType.Text);
                    var unithd1 = cmd.Parameters.Add("@unithd1", NpgsqlDbType.Text);
                    var unithd2 = cmd.Parameters.Add("@unithd2", NpgsqlDbType.Text);
                    cmd.Prepare();
                    unitqk.Value = unit.UnitID;
                    deptqk.Value = unit.DepartmentID;
                    unitname.Value = unit.UnitName;
                    unithd1.Value = unit.UnitHeadID1 ?? (object)DBNull.Value;
                    unithd2.Value = unit.UnitHeadID2 ?? (object)DBNull.Value;
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

        public async Task<Unit> GetUnitByIdAsync(int Id)
        {
            Unit unit = new Unit();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            if (Id < 1) { return null; }
            sb.Append($"SELECT unitname, unithd1, unithd2, unitqk, u.deptqk, d.deptname, p1.fullname AS unithd1_name, p2.fullname AS unithd2_name ");
            sb.Append($"FROM public.gst_units u LEFT OUTER JOIN gst_prsns p1 ON u.unithd1 = p1.id LEFT OUTER JOIN gst_prsns p2 ON u.unithd2 = p2.id ");
            sb.Append($"LEFT OUTER JOIN gst_depts d ON u.deptqk = d.deptqk WHERE (unitqk = @unitqk);");
            query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var unitqk = cmd.Parameters.Add("@unitqk", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    unitqk.Value = Id;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            unit.UnitID = reader["unitqk"] == DBNull.Value ? 0 : (int)(reader["unitqk"]);
                            unit.UnitName = reader["unitname"] == DBNull.Value ? String.Empty : reader["unitname"].ToString();
                            unit.DepartmentID = reader["deptqk"] == DBNull.Value ? 0 : (int)(reader["deptqk"]);
                            unit.DepartmentName = reader["deptname"] == DBNull.Value ? String.Empty : reader["deptname"].ToString();
                            unit.UnitHeadID1 = reader["unithd1"] == DBNull.Value ? String.Empty : reader["unithd1"].ToString();
                            unit.UnitHeadID2 = reader["unithd2"] == DBNull.Value ? String.Empty : reader["unithd2"].ToString();
                            unit.UnitHeadName1 = reader["unithd1_name"] == DBNull.Value ? String.Empty : reader["unithd1_name"].ToString();
                            unit.UnitHeadName2 = reader["unithd2_name"] == DBNull.Value ? String.Empty : reader["unithd2_name"].ToString();
                        }
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message);
            }
            return unit;
        }

        public async Task<IList<Unit>> GetUnitsAsync()
        {
            List<Unit> unitList = new List<Unit>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append($"SELECT unitname, unithd1, unithd2, unitqk, u.deptqk, d.deptname, p1.fullname AS unithd1_name, p2.fullname AS unithd2_name ");
            sb.Append($"FROM public.gst_units u LEFT OUTER JOIN gst_prsns p1 ON u.unithd1 = p1.id LEFT OUTER JOIN gst_prsns p2 ON u.unithd2 = p2.id ");
            sb.Append($"LEFT OUTER JOIN gst_depts d ON u.deptqk = d.deptqk ORDER BY d.deptname, unitname ASC;");
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
                        unitList.Add(new Unit()
                        {
                            UnitID = reader["unitqk"] == DBNull.Value ? 0 : (int)(reader["unitqk"]),
                        UnitName = reader["unitname"] == DBNull.Value ? String.Empty : reader["unitname"].ToString(),
                        DepartmentID = reader["deptqk"] == DBNull.Value ? 0 : (int)(reader["deptqk"]),
                        DepartmentName = reader["deptname"] == DBNull.Value ? String.Empty : reader["deptname"].ToString(),
                        UnitHeadID1 = reader["unithd1"] == DBNull.Value ? String.Empty : reader["unithd1"].ToString(),
                        UnitHeadID2 = reader["unithd2"] == DBNull.Value ? String.Empty : reader["unithd2"].ToString(),
                        UnitHeadName1 = reader["unithd1_name"] == DBNull.Value ? String.Empty : reader["unithd1_name"].ToString(),
                        UnitHeadName2 = reader["unithd2_name"] == DBNull.Value ? String.Empty : reader["unithd2_name"].ToString(),
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
            return unitList;
        }
        #endregion

    }
}
