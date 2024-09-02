using IntranetPortal.Base.Models.SecurityModels;
using IntranetPortal.Base.Repositories.SecurityRepositories;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Data.Repositories.SecurityRepositories
{
    public class EmployeeUserRepository : IEmployeeUserRepository
    {
        public IConfiguration _config { get; }
        public EmployeeUserRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        public async Task<IList<EmployeeUser>> GetAllAsync()
        {
            List<EmployeeUser> employeeUserList = new List<EmployeeUser>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            //string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT u.usr_id, p.fullname, p.sex, p.phone1, p.phone2, e.official_email, ");
            sb.Append("p.email, u.usr_nm, u.usr_typ, u.usr_afc, u.usr_ccs, u.usr_mlc, ");
            sb.Append("u.lck_enb, u.lck_end, u.usr_pwh, u.usr_stp, u.usr_tfe, u.coy_cd, u.usr_cb, ");
            sb.Append("u.usr_cd, u.usr_md, u.usr_mb, e.emp_id, e.emp_no_1, e.emp_no_2, e.loc_id, ");
            sb.Append("l.locname, e.dept_id, d.deptname, e.unit_id, t.unitname, e.coy_id, c.coy_name ");
            sb.Append("FROM public.sct_usr_acct u INNER JOIN public.erm_emp_inf e ON u.usr_id = e.emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p ON e.emp_id = p.id ");
            sb.Append("INNER JOIN public.gst_locs l ON e.loc_id = l.locqk ");
            sb.Append("INNER JOIN public.gst_depts d ON e.dept_id = d.deptqk ");
            sb.Append("INNER JOIN public.gst_units t ON e.unit_id = t.unitqk ");
            sb.Append("INNER JOIN public.gst_coys c ON e.coy_id = c.coy_code ");
            sb.Append("WHERE (e.is_dx = false AND p.is_dx = false ");
            sb.Append("AND u.is_dx = false) ORDER BY p.fullname ASC;");
            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                await cmd.PrepareAsync();
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    employeeUserList.Add(new EmployeeUser()
                    {
                        UserID = reader["usr_id"] == DBNull.Value ? string.Empty : (reader["usr_id"]).ToString(),
                        UserName = reader["usr_nm"] == DBNull.Value ? string.Empty : (reader["usr_nm"]).ToString(),
                        UserType = reader["usr_typ"] == DBNull.Value ? string.Empty : (reader["usr_typ"]).ToString(),
                        AccessFailCount = reader["usr_afc"] == DBNull.Value ? 0 : (int)reader["usr_afc"],
                        CompanyCode = reader["coy_id"] == DBNull.Value ? string.Empty : (reader["coy_id"]).ToString(),
                        ConcurrencyString = reader["usr_ccs"] == DBNull.Value ? string.Empty : (reader["usr_ccs"]).ToString(),
                        EmailIsConfirmed = reader["usr_mlc"] == DBNull.Value ? false : (bool)reader["usr_mlc"],
                        LockEnabled = reader["lck_enb"] == DBNull.Value ? false : (bool)reader["lck_enb"],
                        LockEndDate = reader["lck_end"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["lck_end"]),
                        PasswordHash = reader["usr_pwh"] == DBNull.Value ? string.Empty : reader["usr_pwh"].ToString(),
                        SecurityStamp = reader["usr_stp"] == DBNull.Value ? string.Empty : reader["usr_stp"].ToString(),
                        TwoFactorEnabled = reader["usr_tfe"] == DBNull.Value ? false : (bool)reader["usr_tfe"],
                        EmployeeNo1 = reader["emp_no_1"] == DBNull.Value ? string.Empty : (reader["emp_no_1"]).ToString(),
                        EmployeeNo2 = reader["emp_no_2"] == DBNull.Value ? string.Empty : (reader["emp_no_2"]).ToString(),
                        CompanyID = reader["coy_id"] == DBNull.Value ? string.Empty : (reader["coy_id"]).ToString(),
                        CompanyName = reader["coy_name"] == DBNull.Value ? string.Empty : (reader["coy_name"]).ToString(),
                        DepartmentID = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                        DepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : (reader["deptname"]).ToString(),
                        UnitID = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                        UnitName = reader["unitname"] == DBNull.Value ? string.Empty : (reader["unitname"]).ToString(),
                        LocationID = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                        LocationName = reader["locname"] == DBNull.Value ? string.Empty : (reader["locname"]).ToString(),
                        OfficialEmail = reader["official_email"] == DBNull.Value ? String.Empty : reader["official_email"].ToString(),
                        PersonalEmail = reader["email"] == DBNull.Value ? String.Empty : reader["email"].ToString(),
                        FullName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                        Sex = reader["sex"] == DBNull.Value ? string.Empty : reader["sex"].ToString(),
                        PhoneNo1 = reader["phone1"] == DBNull.Value ? string.Empty : reader["phone1"].ToString(),
                        PhoneNo2 = reader["phone2"] == DBNull.Value ? string.Empty : reader["phone2"].ToString(),
                        ModifiedBy = reader["usr_mb"] == DBNull.Value ? string.Empty : reader["usr_mb"].ToString(),
                        ModifiedTime = reader["usr_md"] == DBNull.Value ? string.Empty : reader["usr_md"].ToString(),
                        CreatedTime = reader["usr_cd"] == DBNull.Value ? string.Empty : reader["usr_cd"].ToString(),
                        CreatedBy = reader["usr_cb"] == DBNull.Value ? string.Empty : reader["usr_cb"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return employeeUserList;
        }

        public async Task<IList<EmployeeUser>> SearchByNameAsync(string fullName)
        {
            if (String.IsNullOrEmpty(fullName)) { return null; }
            List<EmployeeUser> employeeUserList = new List<EmployeeUser>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT u.usr_id, p.fullname, p.sex, p.phone1, p.phone2, e.official_email, ");
            sb.Append("p.email, u.usr_nm, u.usr_typ, u.usr_afc, u.usr_ccs, u.usr_mlc, ");
            sb.Append("u.lck_enb, u.lck_end, u.usr_pwh, u.usr_stp, u.usr_tfe, u.coy_cd, u.usr_cb, ");
            sb.Append("u.usr_cd, u.usr_md, u.usr_mb, e.emp_id, e.emp_no_1, e.emp_no_2, e.loc_id, ");
            sb.Append("l.locname, e.dept_id, d.deptname, e.unit_id, t.unitname, e.coy_id, c.coy_name ");
            sb.Append("FROM public.sct_usr_acct u INNER JOIN public.erm_emp_inf e ON u.usr_id = e.emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p ON e.emp_id = p.id ");
            sb.Append("INNER JOIN public.gst_locs l ON e.loc_id = l.locqk ");
            sb.Append("INNER JOIN public.gst_depts d ON e.dept_id = d.deptqk ");
            sb.Append("INNER JOIN public.gst_units t ON e.unit_id = t.unitqk ");
            sb.Append("INNER JOIN public.gst_coys c ON e.coy_id = c.coy_code ");
            sb.Append("WHERE (e.is_dx = false AND p.is_dx = false ");
            sb.Append("AND u.is_dx = false) ");
            sb.Append("AND (LOWER(p.fullname) LIKE '%'||LOWER(@fullname)||'%') ");
            sb.Append("ORDER BY p.fullname;");
            query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var employeeName = cmd.Parameters.Add("@fullname", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    employeeName.Value = fullName;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        employeeUserList.Add(new EmployeeUser()
                        {
                            UserID = reader["usr_id"] == DBNull.Value ? string.Empty : (reader["usr_id"]).ToString(),
                            UserName = reader["usr_nm"] == DBNull.Value ? string.Empty : (reader["usr_nm"]).ToString(),
                            UserType = reader["usr_typ"] == DBNull.Value ? string.Empty : (reader["usr_typ"]).ToString(),
                            AccessFailCount = reader["usr_afc"] == DBNull.Value ? 0 : (int)reader["usr_afc"],
                            CompanyCode = reader["coy_id"] == DBNull.Value ? string.Empty : (reader["coy_id"]).ToString(),
                            ConcurrencyString = reader["usr_ccs"] == DBNull.Value ? string.Empty : (reader["usr_ccs"]).ToString(),
                            EmailIsConfirmed = reader["usr_mlc"] == DBNull.Value ? false : (bool)reader["usr_mlc"],
                            LockEnabled = reader["lck_enb"] == DBNull.Value ? false : (bool)reader["lck_enb"],
                            LockEndDate = reader["lck_end"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["lck_end"]),
                            PasswordHash = reader["usr_pwh"] == DBNull.Value ? string.Empty : reader["usr_pwh"].ToString(),
                            SecurityStamp = reader["usr_stp"] == DBNull.Value ? string.Empty : reader["usr_stp"].ToString(),
                            TwoFactorEnabled = reader["usr_tfe"] == DBNull.Value ? false : (bool)reader["usr_tfe"],
                            EmployeeNo1 = reader["emp_no_1"] == DBNull.Value ? string.Empty : (reader["emp_no_1"]).ToString(),
                            EmployeeNo2 = reader["emp_no_2"] == DBNull.Value ? string.Empty : (reader["emp_no_2"]).ToString(),
                            CompanyID = reader["coy_id"] == DBNull.Value ? string.Empty : (reader["coy_id"]).ToString(),
                            CompanyName = reader["coy_name"] == DBNull.Value ? string.Empty : (reader["coy_name"]).ToString(),
                            DepartmentID = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                            DepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : (reader["deptname"]).ToString(),
                            UnitID = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                            UnitName = reader["unitname"] == DBNull.Value ? string.Empty : (reader["unitname"]).ToString(),
                            LocationID = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                            LocationName = reader["locname"] == DBNull.Value ? string.Empty : (reader["locname"]).ToString(),
                            OfficialEmail = reader["official_email"] == DBNull.Value ? String.Empty : reader["official_email"].ToString(),
                            PersonalEmail = reader["email"] == DBNull.Value ? String.Empty : reader["email"].ToString(),
                            FullName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                            Sex = reader["sex"] == DBNull.Value ? string.Empty : reader["sex"].ToString(),
                            PhoneNo1 = reader["phone1"] == DBNull.Value ? string.Empty : reader["phone1"].ToString(),
                            PhoneNo2 = reader["phone2"] == DBNull.Value ? string.Empty : reader["phone2"].ToString(),
                            ModifiedBy = reader["usr_mb"] == DBNull.Value ? string.Empty : reader["usr_mb"].ToString(),
                            ModifiedTime = reader["usr_md"] == DBNull.Value ? string.Empty : reader["usr_md"].ToString(),
                            CreatedTime = reader["usr_cd"] == DBNull.Value ? string.Empty : reader["usr_cd"].ToString(),
                            CreatedBy = reader["usr_cb"] == DBNull.Value ? string.Empty : reader["usr_cb"].ToString(),
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
            return employeeUserList;
        }

        public async Task<IList<EmployeeUser>> GetByNameAsync(string fullName)
        {
            if (String.IsNullOrEmpty(fullName)) { return null; }
            List<EmployeeUser> employeeUserList = new List<EmployeeUser>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT u.usr_id, p.fullname, p.sex, p.phone1, p.phone2, e.official_email, ");
            sb.Append("p.email, u.usr_nm, u.usr_typ, u.usr_afc, u.usr_ccs, u.usr_mlc, ");
            sb.Append("u.lck_enb, u.lck_end, u.usr_pwh, u.usr_stp, u.usr_tfe, u.coy_cd, u.usr_cb, ");
            sb.Append("u.usr_cd, u.usr_md, u.usr_mb, e.emp_id, e.emp_no_1, e.emp_no_2, e.loc_id, ");
            sb.Append("l.locname, e.dept_id, d.deptname, e.unit_id, t.unitname, e.coy_id, c.coy_name ");
            sb.Append("FROM public.sct_usr_acct u INNER JOIN public.erm_emp_inf e ON u.usr_id = e.emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p ON e.emp_id = p.id ");
            sb.Append("INNER JOIN public.gst_locs l ON e.loc_id = l.locqk ");
            sb.Append("INNER JOIN public.gst_depts d ON e.dept_id = d.deptqk ");
            sb.Append("INNER JOIN public.gst_units t ON e.unit_id = t.unitqk ");
            sb.Append("INNER JOIN public.gst_coys c ON e.coy_id = c.coy_code ");
            sb.Append("WHERE (e.is_dx = false AND p.is_dx = false ");
            sb.Append("AND u.is_dx = false) ");
            sb.Append("AND (LOWER(p.fullname) = LOWER(@fullname)) ");
            sb.Append("ORDER BY p.fullname;");
            query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var employeeName = cmd.Parameters.Add("@fullname", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                employeeName.Value = fullName;
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    employeeUserList.Add(new EmployeeUser()
                    {
                        UserID = reader["usr_id"] == DBNull.Value ? string.Empty : (reader["usr_id"]).ToString(),
                        UserName = reader["usr_nm"] == DBNull.Value ? string.Empty : (reader["usr_nm"]).ToString(),
                        UserType = reader["usr_typ"] == DBNull.Value ? string.Empty : (reader["usr_typ"]).ToString(),
                        AccessFailCount = reader["usr_afc"] == DBNull.Value ? 0 : (int)reader["usr_afc"],
                        CompanyCode = reader["coy_cd"] == DBNull.Value ? string.Empty : (reader["coy_cd"]).ToString(),
                        ConcurrencyString = reader["usr_ccs"] == DBNull.Value ? string.Empty : (reader["usr_ccs"]).ToString(),
                        EmailIsConfirmed = reader["usr_mlc"] == DBNull.Value ? false : (bool)reader["usr_mlc"],
                        LockEnabled = reader["lck_enb"] == DBNull.Value ? false : (bool)reader["lck_enb"],
                        LockEndDate = reader["lck_end"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["lck_end"]),
                        PasswordHash = reader["usr_pwh"] == DBNull.Value ? string.Empty : reader["usr_pwh"].ToString(),
                        SecurityStamp = reader["usr_stp"] == DBNull.Value ? string.Empty : reader["usr_stp"].ToString(),
                        TwoFactorEnabled = reader["usr_tfe"] == DBNull.Value ? false : (bool)reader["usr_tfe"],
                        EmployeeNo1 = reader["emp_no_1"] == DBNull.Value ? string.Empty : (reader["emp_no_1"]).ToString(),
                        EmployeeNo2 = reader["emp_no_2"] == DBNull.Value ? string.Empty : (reader["emp_no_2"]).ToString(),
                        CompanyID = reader["coy_id"] == DBNull.Value ? string.Empty : (reader["coy_id"]).ToString(),
                        CompanyName = reader["coy_name"] == DBNull.Value ? string.Empty : (reader["coy_name"]).ToString(),
                        DepartmentID = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                        DepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : (reader["deptname"]).ToString(),
                        UnitID = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                        UnitName = reader["unitname"] == DBNull.Value ? string.Empty : (reader["unitname"]).ToString(),
                        LocationID = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                        LocationName = reader["locname"] == DBNull.Value ? string.Empty : (reader["locname"]).ToString(),
                        OfficialEmail = reader["official_email"] == DBNull.Value ? String.Empty : reader["official_email"].ToString(),
                        PersonalEmail = reader["email"] == DBNull.Value ? String.Empty : reader["email"].ToString(),
                        FullName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                        Sex = reader["sex"] == DBNull.Value ? string.Empty : reader["sex"].ToString(),
                        PhoneNo1 = reader["phone1"] == DBNull.Value ? string.Empty : reader["phone1"].ToString(),
                        PhoneNo2 = reader["phone2"] == DBNull.Value ? string.Empty : reader["phone2"].ToString(),
                        ModifiedBy = reader["usr_mb"] == DBNull.Value ? string.Empty : reader["usr_mb"].ToString(),
                        ModifiedTime = reader["usr_md"] == DBNull.Value ? string.Empty : reader["usr_md"].ToString(),
                        CreatedTime = reader["usr_cd"] == DBNull.Value ? string.Empty : reader["usr_cd"].ToString(),
                        CreatedBy = reader["usr_cb"] == DBNull.Value ? string.Empty : reader["usr_cb"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return employeeUserList;
        }
    }
}
