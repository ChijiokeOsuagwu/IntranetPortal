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
    public class EmployeesRepository : IEmployeesRepository
    {
        public IConfiguration _config { get; }
        public EmployeesRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        #region Employees Read Action Methods
        public async Task<Employee> GetEmployeeByIdAsync(string employeeId)
        {
            if (String.IsNullOrEmpty(employeeId)) { throw new ArgumentNullException(nameof(employeeId), "The required parameter [employeeId] is missing or has in invalid value."); }
            Employee employee = new Employee();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT e.emp_id, e.emp_no_1, e.emp_no_2, e.start_up_date, ");
            sb.Append("e.yrs_of_experience, e.start_up_designation, e.place_of_engagement, ");
            sb.Append("e.confirmation_date, e.current_designation, e.job_grade, e.employment_status, ");
            sb.Append("e.date_of_last_promotion, e.official_email, e.state_of_origin, e.dx_by, ");
            sb.Append("e.lga_of_origin, e.religion, e.geo_political_region, e.next_of_kin_name, ");
            sb.Append("e.next_of_kin_relationship, e.modified_by, e.modified_date, e.is_dx, e.dx_time, ");
            sb.Append("e.created_by, e.created_date, e.next_of_kin_address, e.next_of_kin_phone, ");
            sb.Append("e.next_of_kin_email, e.dept_id, e.unit_id, e.loc_id, e.coy_id, p.id, p.title, ");
            sb.Append("p.sname, p.fname, p.oname, p.fullname, p.sex, p.phone1, p.phone2,");
            sb.Append("p.email AS personal_email, p.address, p.mdb, p.mdt, p.ctb, p.ctt, ");
            sb.Append("p.imgp, p.birthday, p.birthmonth, p.birthyear, p.maritalstatus, l.locname, ");
            sb.Append("l.loctype, l.lochq1, l.lochq2, l.locmb, l.locmd, l.loccb, l.loccd, l.locctr, ");
            sb.Append("l.locst, l.locqk, c.coy_code, c.coy_name, d.deptname, d.depthd1, d.depthd2, ");
            sb.Append("d.deptqk, u.unitname, u.unithd1, u.unithd2, u.unitqk ");
            sb.Append("FROM erm_emp_inf e JOIN gst_prsns p ON e.emp_id = p.id ");
            sb.Append("LEFT JOIN gst_locs l ON e.loc_id = l.locqk ");
            sb.Append("LEFT JOIN gst_coys c ON e.coy_id = c.coy_code ");
            sb.Append("LEFT JOIN gst_depts d ON e.dept_id = d.deptqk ");
            sb.Append("LEFT JOIN gst_units u ON e.unit_id = u.unitqk ");
            sb.Append("WHERE(LOWER(e.emp_id) = LOWER(@emp_id)) AND (e.is_dx = false);");
            query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    emp_id.Value = employeeId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            employee.EmployeeID = reader["emp_id"] == DBNull.Value ? string.Empty : (reader["emp_id"]).ToString();
                            employee.EmployeeNo1 = reader["emp_no_1"] == DBNull.Value ? string.Empty : (reader["emp_no_1"]).ToString();
                            employee.EmployeeNo2 = reader["emp_no_2"] == DBNull.Value ? string.Empty : (reader["emp_no_2"]).ToString();
                            employee.StartUpDate = reader["start_up_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["start_up_date"];
                            employee.YearsOfExperience = reader["yrs_of_experience"] == DBNull.Value ? 0 : (int)reader["yrs_of_experience"];
                            employee.StartUpDesignation = reader["start_up_designation"] == DBNull.Value ? String.Empty : reader["start_up_designation"].ToString();
                            employee.PlaceOfEngagement = reader["place_of_engagement"] == DBNull.Value ? String.Empty : reader["place_of_engagement"].ToString();
                            employee.ConfirmationDate = reader["confirmation_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["confirmation_date"];
                            employee.CurrentDesignation = reader["current_designation"] == DBNull.Value ? String.Empty : reader["current_designation"].ToString();
                            employee.JobGrade = reader["job_grade"] == DBNull.Value ? String.Empty : reader["job_grade"].ToString();
                            employee.EmploymentStatus = reader["employment_status"] == DBNull.Value ? String.Empty : reader["employment_status"].ToString();
                            employee.DateOfLastPromotion = reader["date_of_last_promotion"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["date_of_last_promotion"];
                            employee.LengthOfService = reader["start_up_date"] == DBNull.Value ? 0 : (int)(DateTime.Now - (DateTime)reader["start_up_date"]).TotalDays;
                            employee.OfficialEmail = reader["official_email"] == DBNull.Value ? String.Empty : reader["official_email"].ToString();
                            employee.StateOfOrigin = reader["state_of_origin"] == DBNull.Value ? String.Empty : reader["state_of_origin"].ToString();
                            employee.LgaOfOrigin = reader["lga_of_origin"] == DBNull.Value ? String.Empty : reader["lga_of_origin"].ToString();
                            employee.Religion = reader["religion"] == DBNull.Value ? String.Empty : reader["religion"].ToString();
                            employee.GeoPoliticalRegion = reader["geo_political_region"] == DBNull.Value ? String.Empty : reader["geo_political_region"].ToString();
                            employee.NextOfKinName = reader["next_of_kin_name"] == DBNull.Value ? String.Empty : reader["next_of_kin_name"].ToString();
                            employee.NextOfKinRelationship = reader["next_of_kin_relationship"] == DBNull.Value ? String.Empty : reader["next_of_kin_relationship"].ToString();
                            employee.NextOfKinAddress = reader["next_of_kin_address"] == DBNull.Value ? String.Empty : reader["next_of_kin_address"].ToString();
                            employee.NextOfKinPhone = reader["next_of_kin_phone"] == DBNull.Value ? String.Empty : reader["next_of_kin_phone"].ToString();
                            employee.NextOfKinEmail = reader["next_of_kin_email"] == DBNull.Value ? String.Empty : reader["next_of_kin_email"].ToString();
                            employee.CompanyID = reader["coy_id"] == DBNull.Value ? string.Empty : (reader["coy_id"]).ToString();
                            employee.DepartmentID = reader["dept_id"] == DBNull.Value ? 0 : (int)(reader["dept_id"]);
                            employee.UnitID = reader["unit_id"] == DBNull.Value ? 0 : (int)(reader["unit_id"]);
                            employee.LocationID = reader["loc_id"] == DBNull.Value ? 0 : (int)(reader["loc_id"]);
                            employee.EmployeeModifiedBy = reader["modified_by"] == DBNull.Value ? string.Empty : reader["modified_by"].ToString();
                            employee.EmployeeModifiedDate = reader["modified_date"] == DBNull.Value ? string.Empty : reader["modified_date"].ToString();
                            employee.EmployeeCreatedBy = reader["created_by"] == DBNull.Value ? string.Empty : reader["created_by"].ToString();
                            employee.EmployeeCreatedDate = reader["created_date"] == DBNull.Value ? string.Empty : reader["created_date"].ToString();
                            employee.IsDeactivated = reader["is_dx"] == DBNull.Value ? true : (bool)reader["is_dx"];
                            employee.DeactivationTime = reader["dx_time"] == DBNull.Value ? string.Empty : reader["dx_time"].ToString();
                            employee.DeactivatedBy = reader["dx_by"] == DBNull.Value ? string.Empty : reader["dx_by"].ToString();

                            employee.PersonID = reader["id"] == DBNull.Value ? String.Empty : reader["id"].ToString();
                            employee.Title = reader["title"] == DBNull.Value ? string.Empty : reader["title"].ToString();
                            employee.Surname = reader["sname"] == DBNull.Value ? string.Empty : reader["sname"].ToString();
                            employee.FirstName = reader["fname"] == DBNull.Value ? string.Empty : reader["fname"].ToString();
                            employee.OtherNames = reader["oname"] == DBNull.Value ? string.Empty : reader["oname"].ToString();
                            employee.FullName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString();
                            employee.Sex = reader["sex"] == DBNull.Value ? string.Empty : reader["sex"].ToString();
                            employee.MaritalStatus = reader["maritalstatus"] == DBNull.Value ? String.Empty : reader["maritalstatus"].ToString();
                            employee.BirthDay = reader["birthday"] == DBNull.Value ? (int?)null : (int)reader["birthday"];
                            employee.BirthMonth = reader["birthmonth"] == DBNull.Value ? (int?)null : (int)reader["birthmonth"];
                            employee.BirthYear = reader["birthyear"] == DBNull.Value ? (int?)null : (int)reader["birthyear"];
                            employee.PhoneNo1 = reader["phone1"] == DBNull.Value ? string.Empty : reader["phone1"].ToString();
                            employee.PhoneNo2 = reader["phone2"] == DBNull.Value ? string.Empty : reader["phone2"].ToString();
                            employee.Email = reader["personal_email"] == DBNull.Value ? string.Empty : reader["personal_email"].ToString();
                            employee.Address = reader["address"] == DBNull.Value ? string.Empty : reader["address"].ToString();
                            employee.ImagePath = reader["imgp"] == DBNull.Value ? string.Empty : reader["imgp"].ToString();
                            employee.ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString();
                            employee.ModifiedTime = reader["mdt"] == DBNull.Value ? string.Empty : reader["mdt"].ToString();
                            employee.CreatedTime = reader["ctt"] == DBNull.Value ? string.Empty : reader["ctt"].ToString();
                            employee.CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString();

                            employee.LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString();
                            employee.LocationType = reader["loctype"] == DBNull.Value ? string.Empty : reader["loctype"].ToString();
                            employee.LocationHead1 = reader["lochq1"] == DBNull.Value ? string.Empty : reader["lochq1"].ToString();
                            employee.LocationHead2 = reader["lochq2"] == DBNull.Value ? string.Empty : reader["lochq2"].ToString();
                            employee.LocationCountry = reader["locctr"] == DBNull.Value ? string.Empty : reader["locctr"].ToString();
                            employee.LocationState = reader["locst"] == DBNull.Value ? string.Empty : reader["locst"].ToString();
                            employee.CompanyName = reader["coy_name"] == DBNull.Value ? string.Empty : reader["coy_name"].ToString();
                            employee.DepartmentHead1 = reader["depthd1"] == DBNull.Value ? string.Empty : reader["depthd1"].ToString();
                            employee.DepartmentHead2 = reader["depthd2"] == DBNull.Value ? string.Empty : reader["depthd2"].ToString();
                            employee.DepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString();
                            employee.UnitHead1 = reader["unithd1"] == DBNull.Value ? string.Empty : reader["unithd1"].ToString();
                            employee.UnitHead2 = reader["unithd2"] == DBNull.Value ? string.Empty : reader["unithd2"].ToString();
                            employee.UnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString();

                        }
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message);
            }
            return employee;
        }

        public async Task<Employee> GetEmployeeByNameAsync(string employeeName)
        {
            if (String.IsNullOrEmpty(employeeName)) { throw new ArgumentNullException(nameof(employeeName), "The required parameter [EmployeeName] is missing or has in invalid value."); }
            Employee employee = new Employee();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT e.emp_id, e.emp_no_1, e.emp_no_2, e.start_up_date, ");
            sb.Append("e.yrs_of_experience, e.start_up_designation, e.place_of_engagement, ");
            sb.Append("e.confirmation_date, e.current_designation, e.job_grade, e.employment_status, ");
            sb.Append("e.date_of_last_promotion, e.official_email, e.state_of_origin, e.is_dx, ");
            sb.Append("e.lga_of_origin, e.religion, e.geo_political_region, e.next_of_kin_name, ");
            sb.Append("e.next_of_kin_relationship, e.modified_by, e.modified_date, e.dx_time, e.dx_by, ");
            sb.Append("e.created_by, e.created_date, e.next_of_kin_address, e.next_of_kin_phone, ");
            sb.Append("e.next_of_kin_email, e.dept_id, e.unit_id, e.loc_id, e.coy_id, p.id, p.title, ");
            sb.Append("p.sname, p.fname, p.oname, p.fullname, p.sex, p.phone1, p.phone2,");
            sb.Append("p.email AS personal_email, p.address, p.mdb, p.mdt, p.ctb, p.ctt, ");
            sb.Append("p.imgp, p.birthday, p.birthmonth, p.birthyear, p.maritalstatus, l.locname, ");
            sb.Append("l.loctype, l.lochq1, l.lochq2, l.locmb, l.locmd, l.loccb, l.loccd, l.locctr, ");
            sb.Append("l.locst, l.locqk, c.coy_code, c.coy_name, d.deptname, d.depthd1, d.depthd2, ");
            sb.Append("d.deptqk, u.unitname, u.unithd1, u.unithd2, u.unitqk ");
            sb.Append("FROM erm_emp_inf e JOIN gst_prsns p ON e.emp_id = p.id ");
            sb.Append("AND e.is_dx = false ");
            sb.Append("LEFT JOIN gst_locs l ON e.loc_id = l.locqk ");
            sb.Append("LEFT JOIN gst_coys c ON e.coy_id = c.coy_code ");
            sb.Append("LEFT JOIN gst_depts d ON e.dept_id = d.deptqk ");
            sb.Append("LEFT JOIN gst_units u ON e.unit_id = u.unitqk ");
            sb.Append("WHERE(LOWER(p.fullname) = LOWER(@fullname)) AND (e.is_dx = false);");
            query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var fullname = cmd.Parameters.Add("@fullname", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    fullname.Value = employeeName;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            employee.EmployeeID = reader["emp_id"] == DBNull.Value ? string.Empty : (reader["emp_id"]).ToString();
                            employee.EmployeeNo1 = reader["emp_no_1"] == DBNull.Value ? string.Empty : (reader["emp_no_1"]).ToString();
                            employee.EmployeeNo2 = reader["emp_no_2"] == DBNull.Value ? string.Empty : (reader["emp_no_2"]).ToString();
                            employee.StartUpDate = reader["start_up_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["start_up_date"];
                            employee.YearsOfExperience = reader["yrs_of_experience"] == DBNull.Value ? 0 : (int)reader["yrs_of_experience"];
                            employee.StartUpDesignation = reader["start_up_designation"] == DBNull.Value ? String.Empty : reader["start_up_designation"].ToString();
                            employee.PlaceOfEngagement = reader["place_of_engagement"] == DBNull.Value ? String.Empty : reader["place_of_engagement"].ToString();
                            employee.ConfirmationDate = reader["confirmation_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["confirmation_date"];
                            employee.CurrentDesignation = reader["current_designation"] == DBNull.Value ? String.Empty : reader["current_designation"].ToString();
                            employee.JobGrade = reader["job_grade"] == DBNull.Value ? String.Empty : reader["job_grade"].ToString();
                            employee.EmploymentStatus = reader["employment_status"] == DBNull.Value ? String.Empty : reader["employment_status"].ToString();
                            employee.DateOfLastPromotion = reader["date_of_last_promotion"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["date_of_last_promotion"];
                            employee.LengthOfService = reader["start_up_date"] == DBNull.Value ? 0 : (int)((DateTime.Now - (DateTime)reader["start_up_date"]).TotalDays);

                            employee.OfficialEmail = reader["official_email"] == DBNull.Value ? String.Empty : reader["official_email"].ToString();
                            employee.StateOfOrigin = reader["state_of_origin"] == DBNull.Value ? String.Empty : reader["state_of_origin"].ToString();
                            employee.LgaOfOrigin = reader["lga_of_origin"] == DBNull.Value ? String.Empty : reader["lga_of_origin"].ToString();
                            employee.Religion = reader["religion"] == DBNull.Value ? String.Empty : reader["religion"].ToString();
                            employee.GeoPoliticalRegion = reader["geo_political_region"] == DBNull.Value ? String.Empty : reader["geo_political_region"].ToString();
                            employee.NextOfKinName = reader["next_of_kin_name"] == DBNull.Value ? String.Empty : reader["next_of_kin_name"].ToString();
                            employee.NextOfKinRelationship = reader["next_of_kin_relationship"] == DBNull.Value ? String.Empty : reader["next_of_kin_relationship"].ToString();
                            employee.NextOfKinAddress = reader["next_of_kin_address"] == DBNull.Value ? String.Empty : reader["next_of_kin_address"].ToString();
                            employee.NextOfKinPhone = reader["next_of_kin_phone"] == DBNull.Value ? String.Empty : reader["next_of_kin_phone"].ToString();
                            employee.NextOfKinEmail = reader["next_of_kin_email"] == DBNull.Value ? String.Empty : reader["next_of_kin_email"].ToString();
                            employee.CompanyID = reader["coy_id"] == DBNull.Value ? string.Empty : (reader["coy_id"]).ToString();
                            employee.DepartmentID = reader["dept_id"] == DBNull.Value ? 0 : (int)(reader["dept_id"]);
                            employee.UnitID = reader["unit_id"] == DBNull.Value ? 0 : (int)(reader["unit_id"]);
                            employee.LocationID = reader["loc_id"] == DBNull.Value ? 0 : (int)(reader["loc_id"]);
                            employee.EmployeeModifiedBy = reader["modified_by"] == DBNull.Value ? string.Empty : reader["modified_by"].ToString();
                            employee.EmployeeModifiedDate = reader["modified_date"] == DBNull.Value ? string.Empty : reader["modified_date"].ToString();
                            employee.EmployeeCreatedBy = reader["created_by"] == DBNull.Value ? string.Empty : reader["created_by"].ToString();
                            employee.EmployeeCreatedDate = reader["created_date"] == DBNull.Value ? string.Empty : reader["created_date"].ToString();
                            employee.IsDeactivated = reader["is_dx"] == DBNull.Value ? true : (bool)reader["is_dx"];
                            employee.DeactivationTime = reader["dx_time"] == DBNull.Value ? string.Empty : reader["dx_time"].ToString();
                            employee.DeactivatedBy = reader["dx_by"] == DBNull.Value ? string.Empty : reader["dx_by"].ToString();


                            employee.PersonID = reader["id"] == DBNull.Value ? String.Empty : reader["id"].ToString();
                            employee.Title = reader["title"] == DBNull.Value ? string.Empty : reader["title"].ToString();
                            employee.Surname = reader["sname"] == DBNull.Value ? string.Empty : reader["sname"].ToString();
                            employee.FirstName = reader["fname"] == DBNull.Value ? string.Empty : reader["fname"].ToString();
                            employee.OtherNames = reader["oname"] == DBNull.Value ? string.Empty : reader["oname"].ToString();
                            employee.FullName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString();
                            employee.Sex = reader["sex"] == DBNull.Value ? string.Empty : reader["sex"].ToString();
                            employee.MaritalStatus = reader["maritalstatus"] == DBNull.Value ? String.Empty : reader["maritalstatus"].ToString();
                            employee.BirthDay = reader["birthday"] == DBNull.Value ? 0 : (int)reader["birthday"];
                            employee.BirthMonth = reader["birthmonth"] == DBNull.Value ? 0 : (int)reader["birthmonth"];
                            employee.BirthYear = reader["birthyear"] == DBNull.Value ? 0 : (int)reader["birthyear"];
                            employee.PhoneNo1 = reader["phone1"] == DBNull.Value ? string.Empty : reader["phone1"].ToString();
                            employee.PhoneNo2 = reader["phone2"] == DBNull.Value ? string.Empty : reader["phone2"].ToString();
                            employee.Email = reader["personal_email"] == DBNull.Value ? string.Empty : reader["personal_email"].ToString();
                            employee.Address = reader["address"] == DBNull.Value ? string.Empty : reader["address"].ToString();
                            employee.ImagePath = reader["imgp"] == DBNull.Value ? string.Empty : reader["imgp"].ToString();
                            employee.ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString();
                            employee.ModifiedTime = reader["mdt"] == DBNull.Value ? string.Empty : reader["mdt"].ToString();
                            employee.CreatedTime = reader["ctt"] == DBNull.Value ? string.Empty : reader["ctt"].ToString();
                            employee.CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString();

                            employee.LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString();
                            employee.LocationType = reader["loctype"] == DBNull.Value ? string.Empty : reader["loctype"].ToString();
                            employee.LocationHead1 = reader["lochq1"] == DBNull.Value ? string.Empty : reader["lochq1"].ToString();
                            employee.LocationHead2 = reader["lochq2"] == DBNull.Value ? string.Empty : reader["lochq2"].ToString();
                            employee.LocationCountry = reader["locctr"] == DBNull.Value ? string.Empty : reader["locctr"].ToString();
                            employee.LocationState = reader["locst"] == DBNull.Value ? string.Empty : reader["locst"].ToString();
                            employee.CompanyName = reader["coy_name"] == DBNull.Value ? string.Empty : reader["coy_name"].ToString();
                            employee.DepartmentHead1 = reader["depthd1"] == DBNull.Value ? string.Empty : reader["depthd1"].ToString();
                            employee.DepartmentHead2 = reader["depthd2"] == DBNull.Value ? string.Empty : reader["depthd2"].ToString();
                            employee.DepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString();
                            employee.UnitHead1 = reader["unithd1"] == DBNull.Value ? string.Empty : reader["unithd1"].ToString();
                            employee.UnitHead2 = reader["unithd2"] == DBNull.Value ? string.Empty : reader["unithd2"].ToString();
                            employee.UnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString();
                        }
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                return null;
            }
            return employee;
        }

        public async Task<IList<Employee>> GetEmployeesAsync()
        {
            List<Employee> employeeList = new List<Employee>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT e.emp_id, e.emp_no_1, e.emp_no_2, e.start_up_date, ");
            sb.Append("e.yrs_of_experience, e.start_up_designation, e.place_of_engagement, ");
            sb.Append("e.confirmation_date, e.current_designation, e.job_grade, e.employment_status, ");
            sb.Append("e.date_of_last_promotion, e.official_email, e.state_of_origin, e.is_dx, ");
            sb.Append("e.lga_of_origin, e.religion, e.geo_political_region, e.next_of_kin_name, ");
            sb.Append("e.next_of_kin_relationship, e.modified_by, e.modified_date, e.dx_time, e.dx_by, ");
            sb.Append("e.created_by, e.created_date, e.next_of_kin_address, e.next_of_kin_phone, ");
            sb.Append("e.next_of_kin_email, e.dept_id, e.unit_id, e.loc_id, e.coy_id, p.id, p.title, ");
            sb.Append("p.sname, p.fname, p.oname, p.fullname, p.sex, p.phone1, p.phone2,");
            sb.Append("p.email AS personal_email, p.address, p.mdb, p.mdt, p.ctb, p.ctt, ");
            sb.Append("p.imgp, p.birthday, p.birthmonth, p.birthyear, p.maritalstatus, l.locname, ");
            sb.Append("l.loctype, l.lochq1, l.lochq2, l.locmb, l.locmd, l.loccb, l.loccd, l.locctr, ");
            sb.Append("l.locst, l.locqk, c.coy_code, c.coy_name, d.deptname, d.depthd1, d.depthd2, ");
            sb.Append("d.deptqk, u.unitname, u.unithd1, u.unithd2, u.unitqk ");
            sb.Append("FROM erm_emp_inf e JOIN gst_prsns p ON e.emp_id = p.id ");
            sb.Append("AND e.is_dx = false ");
            sb.Append("LEFT JOIN gst_locs l ON e.loc_id = l.locqk ");
            sb.Append("LEFT JOIN gst_coys c ON e.coy_id = c.coy_code ");
            sb.Append("LEFT JOIN gst_depts d ON e.dept_id = d.deptqk ");
            sb.Append("LEFT JOIN gst_units u ON e.unit_id = u.unitqk ");
            sb.Append("WHERE (e.is_dx = false) ORDER BY p.fullname;");
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
                        employeeList.Add(new Employee()
                        {
                            EmployeeID = reader["emp_id"] == DBNull.Value ? string.Empty : (reader["emp_id"]).ToString(),
                            EmployeeNo1 = reader["emp_no_1"] == DBNull.Value ? string.Empty : (reader["emp_no_1"]).ToString(),
                            EmployeeNo2 = reader["emp_no_2"] == DBNull.Value ? string.Empty : (reader["emp_no_2"]).ToString(),
                            StartUpDate = reader["start_up_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["start_up_date"],
                            YearsOfExperience = reader["yrs_of_experience"] == DBNull.Value ? 0 : (int)reader["yrs_of_experience"],
                            StartUpDesignation = reader["start_up_designation"] == DBNull.Value ? String.Empty : reader["start_up_designation"].ToString(),
                            PlaceOfEngagement = reader["place_of_engagement"] == DBNull.Value ? String.Empty : reader["place_of_engagement"].ToString(),
                            ConfirmationDate = reader["confirmation_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["confirmation_date"],
                            CurrentDesignation = reader["current_designation"] == DBNull.Value ? String.Empty : reader["current_designation"].ToString(),
                            JobGrade = reader["job_grade"] == DBNull.Value ? String.Empty : reader["job_grade"].ToString(),
                            EmploymentStatus = reader["employment_status"] == DBNull.Value ? String.Empty : reader["employment_status"].ToString(),
                            DateOfLastPromotion = reader["date_of_last_promotion"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["date_of_last_promotion"],
                            LengthOfService = reader["start_up_date"] == DBNull.Value ? 0 : (int)((DateTime.Now - (DateTime)reader["start_up_date"]).TotalDays),

                            OfficialEmail = reader["official_email"] == DBNull.Value ? String.Empty : reader["official_email"].ToString(),
                            StateOfOrigin = reader["state_of_origin"] == DBNull.Value ? String.Empty : reader["state_of_origin"].ToString(),
                            LgaOfOrigin = reader["lga_of_origin"] == DBNull.Value ? String.Empty : reader["lga_of_origin"].ToString(),
                            Religion = reader["religion"] == DBNull.Value ? String.Empty : reader["religion"].ToString(),
                            GeoPoliticalRegion = reader["geo_political_region"] == DBNull.Value ? String.Empty : reader["geo_political_region"].ToString(),
                            NextOfKinName = reader["next_of_kin_name"] == DBNull.Value ? String.Empty : reader["next_of_kin_name"].ToString(),
                            NextOfKinRelationship = reader["next_of_kin_relationship"] == DBNull.Value ? String.Empty : reader["next_of_kin_relationship"].ToString(),
                            NextOfKinAddress = reader["next_of_kin_address"] == DBNull.Value ? String.Empty : reader["next_of_kin_address"].ToString(),
                            NextOfKinPhone = reader["next_of_kin_phone"] == DBNull.Value ? String.Empty : reader["next_of_kin_phone"].ToString(),
                            NextOfKinEmail = reader["next_of_kin_email"] == DBNull.Value ? String.Empty : reader["next_of_kin_email"].ToString(),
                            CompanyID = reader["coy_id"] == DBNull.Value ? string.Empty : (reader["coy_id"]).ToString(),
                            DepartmentID = reader["dept_id"] == DBNull.Value ? 0 : (int)(reader["dept_id"]),
                            UnitID = reader["unit_id"] == DBNull.Value ? 0 : (int)(reader["unit_id"]),
                            LocationID = reader["loc_id"] == DBNull.Value ? 0 : (int)(reader["loc_id"]),
                            EmployeeModifiedBy = reader["modified_by"] == DBNull.Value ? string.Empty : reader["modified_by"].ToString(),
                            EmployeeModifiedDate = reader["modified_date"] == DBNull.Value ? string.Empty : reader["modified_date"].ToString(),
                            EmployeeCreatedBy = reader["created_by"] == DBNull.Value ? string.Empty : reader["created_by"].ToString(),
                            EmployeeCreatedDate = reader["created_date"] == DBNull.Value ? string.Empty : reader["created_date"].ToString(),
                            IsDeactivated = reader["is_dx"] == DBNull.Value ? true : (bool)reader["is_dx"],
                            DeactivationTime = reader["dx_time"] == DBNull.Value ? string.Empty : reader["dx_time"].ToString(),
                            DeactivatedBy = reader["dx_by"] == DBNull.Value ? string.Empty : reader["dx_by"].ToString(),

                            PersonID = reader["id"] == DBNull.Value ? String.Empty : reader["id"].ToString(),
                            Title = reader["title"] == DBNull.Value ? string.Empty : reader["title"].ToString(),
                            Surname = reader["sname"] == DBNull.Value ? string.Empty : reader["sname"].ToString(),
                            FirstName = reader["fname"] == DBNull.Value ? string.Empty : reader["fname"].ToString(),
                            OtherNames = reader["oname"] == DBNull.Value ? string.Empty : reader["oname"].ToString(),
                            FullName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                            Sex = reader["sex"] == DBNull.Value ? string.Empty : reader["sex"].ToString(),
                            MaritalStatus = reader["maritalstatus"] == DBNull.Value ? String.Empty : reader["maritalstatus"].ToString(),
                            BirthDay = reader["birthday"] == DBNull.Value ? 0 : (int)reader["birthday"],
                            BirthMonth = reader["birthmonth"] == DBNull.Value ? 0 : (int)reader["birthmonth"],
                            BirthYear = reader["birthyear"] == DBNull.Value ? 0 : (int)reader["birthyear"],
                            PhoneNo1 = reader["phone1"] == DBNull.Value ? string.Empty : reader["phone1"].ToString(),
                            PhoneNo2 = reader["phone2"] == DBNull.Value ? string.Empty : reader["phone2"].ToString(),
                            Email = reader["personal_email"] == DBNull.Value ? string.Empty : reader["personal_email"].ToString(),
                            Address = reader["address"] == DBNull.Value ? string.Empty : reader["address"].ToString(),
                            ImagePath = reader["imgp"] == DBNull.Value ? string.Empty : reader["imgp"].ToString(),
                            ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                            ModifiedTime = reader["mdt"] == DBNull.Value ? string.Empty : reader["mdt"].ToString(),
                            CreatedTime = reader["ctt"] == DBNull.Value ? string.Empty : reader["ctt"].ToString(),
                            CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),

                            LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            LocationType = reader["loctype"] == DBNull.Value ? string.Empty : reader["loctype"].ToString(),
                            LocationHead1 = reader["lochq1"] == DBNull.Value ? string.Empty : reader["lochq1"].ToString(),
                            LocationHead2 = reader["lochq2"] == DBNull.Value ? string.Empty : reader["lochq2"].ToString(),
                            LocationCountry = reader["locctr"] == DBNull.Value ? string.Empty : reader["locctr"].ToString(),
                            LocationState = reader["locst"] == DBNull.Value ? string.Empty : reader["locst"].ToString(),
                            CompanyName = reader["coy_name"] == DBNull.Value ? string.Empty : reader["coy_name"].ToString(),
                            DepartmentHead1 = reader["depthd1"] == DBNull.Value ? string.Empty : reader["depthd1"].ToString(),
                            DepartmentHead2 = reader["depthd2"] == DBNull.Value ? string.Empty : reader["depthd2"].ToString(),
                            DepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                            UnitHead1 = reader["unithd1"] == DBNull.Value ? string.Empty : reader["unithd1"].ToString(),
                            UnitHead2 = reader["unithd2"] == DBNull.Value ? string.Empty : reader["unithd2"].ToString(),
                            UnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
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
            return employeeList;
        }

        public async Task<IList<Employee>> GetEmployeesByNameAsync(string employeeName)
        {
            List<Employee> employeeList = new List<Employee>();
            if (string.IsNullOrWhiteSpace(employeeName)) { throw new ArgumentNullException(nameof(employeeName)); }
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT e.emp_id, e.emp_no_1, e.emp_no_2, e.start_up_date, ");
            sb.Append("e.yrs_of_experience, e.start_up_designation, e.place_of_engagement, ");
            sb.Append("e.confirmation_date, e.current_designation, e.job_grade, e.employment_status, ");
            sb.Append("e.date_of_last_promotion, e.official_email, e.state_of_origin, e.is_dx, ");
            sb.Append("e.lga_of_origin, e.religion, e.geo_political_region, e.next_of_kin_name, ");
            sb.Append("e.next_of_kin_relationship, e.modified_by, e.modified_date, e.dx_time, e.dx_by, ");
            sb.Append("e.created_by, e.created_date, e.next_of_kin_address, e.next_of_kin_phone, ");
            sb.Append("e.next_of_kin_email, e.dept_id, e.unit_id, e.loc_id, e.coy_id, p.id, p.title, ");
            sb.Append("p.sname, p.fname, p.oname, p.fullname, p.sex, p.phone1, p.phone2,");
            sb.Append("p.email AS personal_email, p.address, p.mdb, p.mdt, p.ctb, p.ctt, ");
            sb.Append("p.imgp, p.birthday, p.birthmonth, p.birthyear, p.maritalstatus, l.locname, ");
            sb.Append("l.loctype, l.lochq1, l.lochq2, l.locmb, l.locmd, l.loccb, l.loccd, l.locctr, ");
            sb.Append("l.locst, l.locqk, c.coy_code, c.coy_name, d.deptname, d.depthd1, d.depthd2, ");
            sb.Append("d.deptqk, u.unitname, u.unithd1, u.unithd2, u.unitqk ");
            sb.Append("FROM erm_emp_inf e JOIN gst_prsns p ON e.emp_id = p.id ");
            sb.Append("AND e.is_dx = false ");
            sb.Append("LEFT JOIN gst_locs l ON e.loc_id = l.locqk ");
            sb.Append("LEFT JOIN gst_coys c ON e.coy_id = c.coy_code ");
            sb.Append("LEFT JOIN gst_depts d ON e.dept_id = d.deptqk ");
            sb.Append("LEFT JOIN gst_units u ON e.unit_id = u.unitqk ");
            sb.Append("WHERE(LOWER(p.fullname) LIKE '%'||LOWER(@name)||'%') ");
            //sb.Append("OR (LOWER(p.sname) LIKE '%'||LOWER(@name)||'%') ");
            //sb.Append("OR (LOWER(p.oname) LIKE '%'||LOWER(@name)||'%') ");
            //sb.Append("OR (LOWER(p.fullname) LIKE '%'||LOWER(@name)||'%') ");
            sb.Append("AND (e.is_dx = false OR p.is_dx = false) ");
            sb.Append("ORDER BY p.fullname;");
            query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var name = cmd.Parameters.Add("@name", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    name.Value = employeeName;

                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        employeeList.Add(new Employee()
                        {
                            EmployeeID = reader["emp_id"] == DBNull.Value ? string.Empty : (reader["emp_id"]).ToString(),
                            EmployeeNo1 = reader["emp_no_1"] == DBNull.Value ? string.Empty : (reader["emp_no_1"]).ToString(),
                            EmployeeNo2 = reader["emp_no_2"] == DBNull.Value ? string.Empty : (reader["emp_no_2"]).ToString(),
                            StartUpDate = reader["start_up_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["start_up_date"],
                            YearsOfExperience = reader["yrs_of_experience"] == DBNull.Value ? 0 : (int)reader["yrs_of_experience"],
                            StartUpDesignation = reader["start_up_designation"] == DBNull.Value ? String.Empty : reader["start_up_designation"].ToString(),
                            PlaceOfEngagement = reader["place_of_engagement"] == DBNull.Value ? String.Empty : reader["place_of_engagement"].ToString(),
                            ConfirmationDate = reader["confirmation_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["confirmation_date"],
                            CurrentDesignation = reader["current_designation"] == DBNull.Value ? String.Empty : reader["current_designation"].ToString(),
                            JobGrade = reader["job_grade"] == DBNull.Value ? String.Empty : reader["job_grade"].ToString(),
                            EmploymentStatus = reader["employment_status"] == DBNull.Value ? String.Empty : reader["employment_status"].ToString(),
                            DateOfLastPromotion = reader["date_of_last_promotion"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["date_of_last_promotion"],
                            LengthOfService = reader["start_up_date"] == DBNull.Value ? 0 : (int)((DateTime.Now - (DateTime)reader["start_up_date"]).TotalDays),

                            OfficialEmail = reader["official_email"] == DBNull.Value ? String.Empty : reader["official_email"].ToString(),
                            StateOfOrigin = reader["state_of_origin"] == DBNull.Value ? String.Empty : reader["state_of_origin"].ToString(),
                            LgaOfOrigin = reader["lga_of_origin"] == DBNull.Value ? String.Empty : reader["lga_of_origin"].ToString(),
                            Religion = reader["religion"] == DBNull.Value ? String.Empty : reader["religion"].ToString(),
                            GeoPoliticalRegion = reader["geo_political_region"] == DBNull.Value ? String.Empty : reader["geo_political_region"].ToString(),
                            NextOfKinName = reader["next_of_kin_name"] == DBNull.Value ? String.Empty : reader["next_of_kin_name"].ToString(),
                            NextOfKinRelationship = reader["next_of_kin_relationship"] == DBNull.Value ? String.Empty : reader["next_of_kin_relationship"].ToString(),
                            NextOfKinAddress = reader["next_of_kin_address"] == DBNull.Value ? String.Empty : reader["next_of_kin_address"].ToString(),
                            NextOfKinPhone = reader["next_of_kin_phone"] == DBNull.Value ? String.Empty : reader["next_of_kin_phone"].ToString(),
                            NextOfKinEmail = reader["next_of_kin_email"] == DBNull.Value ? String.Empty : reader["next_of_kin_email"].ToString(),
                            CompanyID = reader["coy_id"] == DBNull.Value ? string.Empty : (reader["coy_id"]).ToString(),
                            DepartmentID = reader["dept_id"] == DBNull.Value ? 0 : (int)(reader["dept_id"]),
                            UnitID = reader["unit_id"] == DBNull.Value ? 0 : (int)(reader["unit_id"]),
                            LocationID = reader["loc_id"] == DBNull.Value ? 0 : (int)(reader["loc_id"]),
                            EmployeeModifiedBy = reader["modified_by"] == DBNull.Value ? string.Empty : reader["modified_by"].ToString(),
                            EmployeeModifiedDate = reader["modified_date"] == DBNull.Value ? string.Empty : reader["modified_date"].ToString(),
                            EmployeeCreatedBy = reader["created_by"] == DBNull.Value ? string.Empty : reader["created_by"].ToString(),
                            EmployeeCreatedDate = reader["created_date"] == DBNull.Value ? string.Empty : reader["created_date"].ToString(),
                            IsDeactivated = reader["is_dx"] == DBNull.Value ? true : (bool)reader["is_dx"],
                            DeactivationTime = reader["dx_time"] == DBNull.Value ? string.Empty : reader["dx_time"].ToString(),
                            DeactivatedBy = reader["dx_by"] == DBNull.Value ? string.Empty : reader["dx_by"].ToString(),

                            PersonID = reader["id"] == DBNull.Value ? String.Empty : reader["id"].ToString(),
                            Title = reader["title"] == DBNull.Value ? string.Empty : reader["title"].ToString(),
                            Surname = reader["sname"] == DBNull.Value ? string.Empty : reader["sname"].ToString(),
                            FirstName = reader["fname"] == DBNull.Value ? string.Empty : reader["fname"].ToString(),
                            OtherNames = reader["oname"] == DBNull.Value ? string.Empty : reader["oname"].ToString(),
                            FullName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                            Sex = reader["sex"] == DBNull.Value ? string.Empty : reader["sex"].ToString(),
                            MaritalStatus = reader["maritalstatus"] == DBNull.Value ? String.Empty : reader["maritalstatus"].ToString(),
                            BirthDay = reader["birthday"] == DBNull.Value ? 0 : (int)reader["birthday"],
                            BirthMonth = reader["birthmonth"] == DBNull.Value ? 0 : (int)reader["birthmonth"],
                            BirthYear = reader["birthyear"] == DBNull.Value ? 0 : (int)reader["birthyear"],
                            PhoneNo1 = reader["phone1"] == DBNull.Value ? string.Empty : reader["phone1"].ToString(),
                            PhoneNo2 = reader["phone2"] == DBNull.Value ? string.Empty : reader["phone2"].ToString(),
                            Email = reader["personal_email"] == DBNull.Value ? string.Empty : reader["personal_email"].ToString(),
                            Address = reader["address"] == DBNull.Value ? string.Empty : reader["address"].ToString(),
                            ImagePath = reader["imgp"] == DBNull.Value ? string.Empty : reader["imgp"].ToString(),
                            ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                            ModifiedTime = reader["mdt"] == DBNull.Value ? string.Empty : reader["mdt"].ToString(),
                            CreatedTime = reader["ctt"] == DBNull.Value ? string.Empty : reader["ctt"].ToString(),
                            CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),

                            LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            LocationType = reader["loctype"] == DBNull.Value ? string.Empty : reader["loctype"].ToString(),
                            LocationHead1 = reader["lochq1"] == DBNull.Value ? string.Empty : reader["lochq1"].ToString(),
                            LocationHead2 = reader["lochq2"] == DBNull.Value ? string.Empty : reader["lochq2"].ToString(),
                            LocationCountry = reader["locctr"] == DBNull.Value ? string.Empty : reader["locctr"].ToString(),
                            LocationState = reader["locst"] == DBNull.Value ? string.Empty : reader["locst"].ToString(),
                            CompanyName = reader["coy_name"] == DBNull.Value ? string.Empty : reader["coy_name"].ToString(),
                            DepartmentHead1 = reader["depthd1"] == DBNull.Value ? string.Empty : reader["depthd1"].ToString(),
                            DepartmentHead2 = reader["depthd2"] == DBNull.Value ? string.Empty : reader["depthd2"].ToString(),
                            DepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                            UnitHead1 = reader["unithd1"] == DBNull.Value ? string.Empty : reader["unithd1"].ToString(),
                            UnitHead2 = reader["unithd2"] == DBNull.Value ? string.Empty : reader["unithd2"].ToString(),
                            UnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
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
            return employeeList;
        }

        public async Task<IList<Employee>> GetOtherEmployeesByNameAsync(string employeeId, string otherEmployeeName)
        {
            List<Employee> employeeList = new List<Employee>();
            if (string.IsNullOrWhiteSpace(otherEmployeeName)) { throw new ArgumentNullException(nameof(otherEmployeeName)); }
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT e.emp_id, e.emp_no_1, e.emp_no_2, e.start_up_date, ");
            sb.Append("e.yrs_of_experience, e.start_up_designation, e.place_of_engagement, ");
            sb.Append("e.confirmation_date, e.current_designation, e.job_grade, e.employment_status, ");
            sb.Append("e.date_of_last_promotion, e.official_email, e.state_of_origin, e.is_dx, ");
            sb.Append("e.lga_of_origin, e.religion, e.geo_political_region, e.next_of_kin_name, ");
            sb.Append("e.next_of_kin_relationship, e.modified_by, e.modified_date, e.dx_time, e.dx_by, ");
            sb.Append("e.created_by, e.created_date, e.next_of_kin_address, e.next_of_kin_phone, ");
            sb.Append("e.next_of_kin_email, e.dept_id, e.unit_id, e.loc_id, e.coy_id, p.id, p.title, ");
            sb.Append("p.sname, p.fname, p.oname, p.fullname, p.sex, p.phone1, p.phone2,");
            sb.Append("p.email AS personal_email, p.address, p.mdb, p.mdt, p.ctb, p.ctt, ");
            sb.Append("p.imgp, p.birthday, p.birthmonth, p.birthyear, p.maritalstatus, l.locname, ");
            sb.Append("l.loctype, l.lochq1, l.lochq2, l.locmb, l.locmd, l.loccb, l.loccd, l.locctr, ");
            sb.Append("l.locst, l.locqk, c.coy_code, c.coy_name, d.deptname, d.depthd1, d.depthd2, ");
            sb.Append("d.deptqk, u.unitname, u.unithd1, u.unithd2, u.unitqk ");
            sb.Append("FROM erm_emp_inf e JOIN gst_prsns p ON e.emp_id = p.id ");
            sb.Append("AND e.is_dx = false ");
            sb.Append("LEFT JOIN gst_locs l ON e.loc_id = l.locqk ");
            sb.Append("LEFT JOIN gst_coys c ON e.coy_id = c.coy_code ");
            sb.Append("LEFT JOIN gst_depts d ON e.dept_id = d.deptqk ");
            sb.Append("LEFT JOIN gst_units u ON e.unit_id = u.unitqk ");
            sb.Append("WHERE (LOWER(e.emp_id) != LOWER(@emp_id)) ");
            sb.Append("AND ((LOWER(p.fname) LIKE '%'||LOWER(@name)||'%') ");
            sb.Append("OR (LOWER(p.sname) LIKE '%'||LOWER(@name)||'%') ");
            sb.Append("OR (LOWER(p.oname) LIKE '%'||LOWER(@name)||'%') ");
            sb.Append("OR (LOWER(p.fullname) LIKE '%'||LOWER(@name)||'%')) ");
            sb.Append("AND (e.is_dx = false) ORDER BY p.fullname;");
            query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                    var name = cmd.Parameters.Add("@name", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    emp_id.Value = employeeId;
                    name.Value = otherEmployeeName;

                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        employeeList.Add(new Employee()
                        {
                            EmployeeID = reader["emp_id"] == DBNull.Value ? string.Empty : (reader["emp_id"]).ToString(),
                            EmployeeNo1 = reader["emp_no_1"] == DBNull.Value ? string.Empty : (reader["emp_no_1"]).ToString(),
                            EmployeeNo2 = reader["emp_no_2"] == DBNull.Value ? string.Empty : (reader["emp_no_2"]).ToString(),
                            StartUpDate = reader["start_up_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["start_up_date"],
                            YearsOfExperience = reader["yrs_of_experience"] == DBNull.Value ? 0 : (int)reader["yrs_of_experience"],
                            StartUpDesignation = reader["start_up_designation"] == DBNull.Value ? String.Empty : reader["start_up_designation"].ToString(),
                            PlaceOfEngagement = reader["place_of_engagement"] == DBNull.Value ? String.Empty : reader["place_of_engagement"].ToString(),
                            ConfirmationDate = reader["confirmation_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["confirmation_date"],
                            CurrentDesignation = reader["current_designation"] == DBNull.Value ? String.Empty : reader["current_designation"].ToString(),
                            JobGrade = reader["job_grade"] == DBNull.Value ? String.Empty : reader["job_grade"].ToString(),
                            EmploymentStatus = reader["employment_status"] == DBNull.Value ? String.Empty : reader["employment_status"].ToString(),
                            DateOfLastPromotion = reader["date_of_last_promotion"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["date_of_last_promotion"],
                            LengthOfService = reader["start_up_date"] == DBNull.Value ? 0 : (int)((DateTime.Now - (DateTime)reader["start_up_date"]).TotalDays),

                            OfficialEmail = reader["official_email"] == DBNull.Value ? String.Empty : reader["official_email"].ToString(),
                            StateOfOrigin = reader["state_of_origin"] == DBNull.Value ? String.Empty : reader["state_of_origin"].ToString(),
                            LgaOfOrigin = reader["lga_of_origin"] == DBNull.Value ? String.Empty : reader["lga_of_origin"].ToString(),
                            Religion = reader["religion"] == DBNull.Value ? String.Empty : reader["religion"].ToString(),
                            GeoPoliticalRegion = reader["geo_political_region"] == DBNull.Value ? String.Empty : reader["geo_political_region"].ToString(),
                            NextOfKinName = reader["next_of_kin_name"] == DBNull.Value ? String.Empty : reader["next_of_kin_name"].ToString(),
                            NextOfKinRelationship = reader["next_of_kin_relationship"] == DBNull.Value ? String.Empty : reader["next_of_kin_relationship"].ToString(),
                            NextOfKinAddress = reader["next_of_kin_address"] == DBNull.Value ? String.Empty : reader["next_of_kin_address"].ToString(),
                            NextOfKinPhone = reader["next_of_kin_phone"] == DBNull.Value ? String.Empty : reader["next_of_kin_phone"].ToString(),
                            NextOfKinEmail = reader["next_of_kin_email"] == DBNull.Value ? String.Empty : reader["next_of_kin_email"].ToString(),
                            CompanyID = reader["coy_id"] == DBNull.Value ? string.Empty : (reader["coy_id"]).ToString(),
                            DepartmentID = reader["dept_id"] == DBNull.Value ? 0 : (int)(reader["dept_id"]),
                            UnitID = reader["unit_id"] == DBNull.Value ? 0 : (int)(reader["unit_id"]),
                            LocationID = reader["loc_id"] == DBNull.Value ? 0 : (int)(reader["loc_id"]),
                            EmployeeModifiedBy = reader["modified_by"] == DBNull.Value ? string.Empty : reader["modified_by"].ToString(),
                            EmployeeModifiedDate = reader["modified_date"] == DBNull.Value ? string.Empty : reader["modified_date"].ToString(),
                            EmployeeCreatedBy = reader["created_by"] == DBNull.Value ? string.Empty : reader["created_by"].ToString(),
                            EmployeeCreatedDate = reader["created_date"] == DBNull.Value ? string.Empty : reader["created_date"].ToString(),
                            IsDeactivated = reader["is_dx"] == DBNull.Value ? true : (bool)reader["is_dx"],
                            DeactivationTime = reader["dx_time"] == DBNull.Value ? string.Empty : reader["dx_time"].ToString(),
                            DeactivatedBy = reader["dx_by"] == DBNull.Value ? string.Empty : reader["dx_by"].ToString(),

                            PersonID = reader["id"] == DBNull.Value ? String.Empty : reader["id"].ToString(),
                            Title = reader["title"] == DBNull.Value ? string.Empty : reader["title"].ToString(),
                            Surname = reader["sname"] == DBNull.Value ? string.Empty : reader["sname"].ToString(),
                            FirstName = reader["fname"] == DBNull.Value ? string.Empty : reader["fname"].ToString(),
                            OtherNames = reader["oname"] == DBNull.Value ? string.Empty : reader["oname"].ToString(),
                            FullName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                            Sex = reader["sex"] == DBNull.Value ? string.Empty : reader["sex"].ToString(),
                            MaritalStatus = reader["maritalstatus"] == DBNull.Value ? String.Empty : reader["maritalstatus"].ToString(),
                            BirthDay = reader["birthday"] == DBNull.Value ? 0 : (int)reader["birthday"],
                            BirthMonth = reader["birthmonth"] == DBNull.Value ? 0 : (int)reader["birthmonth"],
                            BirthYear = reader["birthyear"] == DBNull.Value ? 0 : (int)reader["birthyear"],
                            PhoneNo1 = reader["phone1"] == DBNull.Value ? string.Empty : reader["phone1"].ToString(),
                            PhoneNo2 = reader["phone2"] == DBNull.Value ? string.Empty : reader["phone2"].ToString(),
                            Email = reader["personal_email"] == DBNull.Value ? string.Empty : reader["personal_email"].ToString(),
                            Address = reader["address"] == DBNull.Value ? string.Empty : reader["address"].ToString(),
                            ImagePath = reader["imgp"] == DBNull.Value ? string.Empty : reader["imgp"].ToString(),
                            ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                            ModifiedTime = reader["mdt"] == DBNull.Value ? string.Empty : reader["mdt"].ToString(),
                            CreatedTime = reader["ctt"] == DBNull.Value ? string.Empty : reader["ctt"].ToString(),
                            CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),

                            LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            LocationType = reader["loctype"] == DBNull.Value ? string.Empty : reader["loctype"].ToString(),
                            LocationHead1 = reader["lochq1"] == DBNull.Value ? string.Empty : reader["lochq1"].ToString(),
                            LocationHead2 = reader["lochq2"] == DBNull.Value ? string.Empty : reader["lochq2"].ToString(),
                            LocationCountry = reader["locctr"] == DBNull.Value ? string.Empty : reader["locctr"].ToString(),
                            LocationState = reader["locst"] == DBNull.Value ? string.Empty : reader["locst"].ToString(),
                            CompanyName = reader["coy_name"] == DBNull.Value ? string.Empty : reader["coy_name"].ToString(),
                            DepartmentHead1 = reader["depthd1"] == DBNull.Value ? string.Empty : reader["depthd1"].ToString(),
                            DepartmentHead2 = reader["depthd2"] == DBNull.Value ? string.Empty : reader["depthd2"].ToString(),
                            DepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                            UnitHead1 = reader["unithd1"] == DBNull.Value ? string.Empty : reader["unithd1"].ToString(),
                            UnitHead2 = reader["unithd2"] == DBNull.Value ? string.Empty : reader["unithd2"].ToString(),
                            UnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
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
            return employeeList;
        }

        public async Task<IList<Employee>> GetEmployeesByCompanyCodeAsync(string companyCode)
        {
            List<Employee> employeeList = new List<Employee>();
            if (string.IsNullOrWhiteSpace(companyCode)) { throw new ArgumentNullException(nameof(companyCode)); }
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT e.emp_id, e.emp_no_1, e.emp_no_2, e.start_up_date, ");
            sb.Append("e.yrs_of_experience, e.start_up_designation, e.place_of_engagement, ");
            sb.Append("e.confirmation_date, e.current_designation, e.job_grade, e.employment_status, ");
            sb.Append("e.date_of_last_promotion, e.official_email, e.state_of_origin, e.is_dx, ");
            sb.Append("e.lga_of_origin, e.religion, e.geo_political_region, e.next_of_kin_name, ");
            sb.Append("e.next_of_kin_relationship, e.modified_by, e.modified_date, e.dx_time, e.dx_by, ");
            sb.Append("e.created_by, e.created_date, e.next_of_kin_address, e.next_of_kin_phone, ");
            sb.Append("e.next_of_kin_email, e.dept_id, e.unit_id, e.loc_id, e.coy_id, p.id, p.title, ");
            sb.Append("p.sname, p.fname, p.oname, p.fullname, p.sex, p.phone1, p.phone2,");
            sb.Append("p.email AS personal_email, p.address, p.mdb, p.mdt, p.ctb, p.ctt, ");
            sb.Append("p.imgp, p.birthday, p.birthmonth, p.birthyear, p.maritalstatus, l.locname, ");
            sb.Append("l.loctype, l.lochq1, l.lochq2, l.locmb, l.locmd, l.loccb, l.loccd, l.locctr, ");
            sb.Append("l.locst, l.locqk, c.coy_code, c.coy_name, d.deptname, d.depthd1, d.depthd2, ");
            sb.Append("d.deptqk, u.unitname, u.unithd1, u.unithd2, u.unitqk ");
            sb.Append("FROM erm_emp_inf e JOIN gst_prsns p ON e.emp_id = p.id ");
            sb.Append("AND e.is_dx = false ");
            sb.Append("LEFT JOIN gst_locs l ON e.loc_id = l.locqk ");
            sb.Append("LEFT JOIN gst_coys c ON e.coy_id = c.coy_code ");
            sb.Append("LEFT JOIN gst_depts d ON e.dept_id = d.deptqk ");
            sb.Append("LEFT JOIN gst_units u ON e.unit_id = u.unitqk ");
            sb.Append("WHERE(e.coy_id = @coy_id) ");
            sb.Append("AND (e.is_dx = false) ORDER BY p.fullname;");
            query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var coy_id = cmd.Parameters.Add("@coy_id", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    coy_id.Value = companyCode;

                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        employeeList.Add(new Employee()
                        {
                            EmployeeID = reader["emp_id"] == DBNull.Value ? string.Empty : (reader["emp_id"]).ToString(),
                            EmployeeNo1 = reader["emp_no_1"] == DBNull.Value ? string.Empty : (reader["emp_no_1"]).ToString(),
                            EmployeeNo2 = reader["emp_no_2"] == DBNull.Value ? string.Empty : (reader["emp_no_2"]).ToString(),
                            StartUpDate = reader["start_up_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["start_up_date"],
                            YearsOfExperience = reader["yrs_of_experience"] == DBNull.Value ? 0 : (int)reader["yrs_of_experience"],
                            StartUpDesignation = reader["start_up_designation"] == DBNull.Value ? String.Empty : reader["start_up_designation"].ToString(),
                            PlaceOfEngagement = reader["place_of_engagement"] == DBNull.Value ? String.Empty : reader["place_of_engagement"].ToString(),
                            ConfirmationDate = reader["confirmation_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["confirmation_date"],
                            CurrentDesignation = reader["current_designation"] == DBNull.Value ? String.Empty : reader["current_designation"].ToString(),
                            JobGrade = reader["job_grade"] == DBNull.Value ? String.Empty : reader["job_grade"].ToString(),
                            EmploymentStatus = reader["employment_status"] == DBNull.Value ? String.Empty : reader["employment_status"].ToString(),
                            DateOfLastPromotion = reader["date_of_last_promotion"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["date_of_last_promotion"],
                            LengthOfService = reader["start_up_date"] == DBNull.Value ? 0 : (int)((DateTime.Now - (DateTime)reader["start_up_date"]).TotalDays),

                            OfficialEmail = reader["official_email"] == DBNull.Value ? String.Empty : reader["official_email"].ToString(),
                            StateOfOrigin = reader["state_of_origin"] == DBNull.Value ? String.Empty : reader["state_of_origin"].ToString(),
                            LgaOfOrigin = reader["lga_of_origin"] == DBNull.Value ? String.Empty : reader["lga_of_origin"].ToString(),
                            Religion = reader["religion"] == DBNull.Value ? String.Empty : reader["religion"].ToString(),
                            GeoPoliticalRegion = reader["geo_political_region"] == DBNull.Value ? String.Empty : reader["geo_political_region"].ToString(),
                            NextOfKinName = reader["next_of_kin_name"] == DBNull.Value ? String.Empty : reader["next_of_kin_name"].ToString(),
                            NextOfKinRelationship = reader["next_of_kin_relationship"] == DBNull.Value ? String.Empty : reader["next_of_kin_relationship"].ToString(),
                            NextOfKinAddress = reader["next_of_kin_address"] == DBNull.Value ? String.Empty : reader["next_of_kin_address"].ToString(),
                            NextOfKinPhone = reader["next_of_kin_phone"] == DBNull.Value ? String.Empty : reader["next_of_kin_phone"].ToString(),
                            NextOfKinEmail = reader["next_of_kin_email"] == DBNull.Value ? String.Empty : reader["next_of_kin_email"].ToString(),
                            CompanyID = reader["coy_id"] == DBNull.Value ? string.Empty : (reader["coy_id"]).ToString(),
                            DepartmentID = reader["dept_id"] == DBNull.Value ? 0 : (int)(reader["dept_id"]),
                            UnitID = reader["unit_id"] == DBNull.Value ? 0 : (int)(reader["unit_id"]),
                            LocationID = reader["loc_id"] == DBNull.Value ? 0 : (int)(reader["loc_id"]),
                            EmployeeModifiedBy = reader["modified_by"] == DBNull.Value ? string.Empty : reader["modified_by"].ToString(),
                            EmployeeModifiedDate = reader["modified_date"] == DBNull.Value ? string.Empty : reader["modified_date"].ToString(),
                            EmployeeCreatedBy = reader["created_by"] == DBNull.Value ? string.Empty : reader["created_by"].ToString(),
                            EmployeeCreatedDate = reader["created_date"] == DBNull.Value ? string.Empty : reader["created_date"].ToString(),
                            IsDeactivated = reader["is_dx"] == DBNull.Value ? true : (bool)reader["is_dx"],
                            DeactivationTime = reader["dx_time"] == DBNull.Value ? string.Empty : reader["dx_time"].ToString(),
                            DeactivatedBy = reader["dx_by"] == DBNull.Value ? string.Empty : reader["dx_by"].ToString(),

                            PersonID = reader["id"] == DBNull.Value ? String.Empty : reader["id"].ToString(),
                            Title = reader["title"] == DBNull.Value ? string.Empty : reader["title"].ToString(),
                            Surname = reader["sname"] == DBNull.Value ? string.Empty : reader["sname"].ToString(),
                            FirstName = reader["fname"] == DBNull.Value ? string.Empty : reader["fname"].ToString(),
                            OtherNames = reader["oname"] == DBNull.Value ? string.Empty : reader["oname"].ToString(),
                            FullName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                            Sex = reader["sex"] == DBNull.Value ? string.Empty : reader["sex"].ToString(),
                            MaritalStatus = reader["maritalstatus"] == DBNull.Value ? String.Empty : reader["maritalstatus"].ToString(),
                            BirthDay = reader["birthday"] == DBNull.Value ? 0 : (int)reader["birthday"],
                            BirthMonth = reader["birthmonth"] == DBNull.Value ? 0 : (int)reader["birthmonth"],
                            BirthYear = reader["birthyear"] == DBNull.Value ? 0 : (int)reader["birthyear"],
                            PhoneNo1 = reader["phone1"] == DBNull.Value ? string.Empty : reader["phone1"].ToString(),
                            PhoneNo2 = reader["phone2"] == DBNull.Value ? string.Empty : reader["phone2"].ToString(),
                            Email = reader["personal_email"] == DBNull.Value ? string.Empty : reader["personal_email"].ToString(),
                            Address = reader["address"] == DBNull.Value ? string.Empty : reader["address"].ToString(),
                            ImagePath = reader["imgp"] == DBNull.Value ? string.Empty : reader["imgp"].ToString(),
                            ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                            ModifiedTime = reader["mdt"] == DBNull.Value ? string.Empty : reader["mdt"].ToString(),
                            CreatedTime = reader["ctt"] == DBNull.Value ? string.Empty : reader["ctt"].ToString(),
                            CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),

                            LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            LocationType = reader["loctype"] == DBNull.Value ? string.Empty : reader["loctype"].ToString(),
                            LocationHead1 = reader["lochq1"] == DBNull.Value ? string.Empty : reader["lochq1"].ToString(),
                            LocationHead2 = reader["lochq2"] == DBNull.Value ? string.Empty : reader["lochq2"].ToString(),
                            LocationCountry = reader["locctr"] == DBNull.Value ? string.Empty : reader["locctr"].ToString(),
                            LocationState = reader["locst"] == DBNull.Value ? string.Empty : reader["locst"].ToString(),
                            CompanyName = reader["coy_name"] == DBNull.Value ? string.Empty : reader["coy_name"].ToString(),
                            DepartmentHead1 = reader["depthd1"] == DBNull.Value ? string.Empty : reader["depthd1"].ToString(),
                            DepartmentHead2 = reader["depthd2"] == DBNull.Value ? string.Empty : reader["depthd2"].ToString(),
                            DepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                            UnitHead1 = reader["unithd1"] == DBNull.Value ? string.Empty : reader["unithd1"].ToString(),
                            UnitHead2 = reader["unithd2"] == DBNull.Value ? string.Empty : reader["unithd2"].ToString(),
                            UnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
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
            return employeeList;
        }

        public async Task<IList<Employee>> GetEmployeesByCompanyCodeAsync(string companyCode, int locationId)
        {
            List<Employee> employeeList = new List<Employee>();
            if (string.IsNullOrWhiteSpace(companyCode)) { throw new ArgumentNullException(nameof(companyCode)); }
            if (locationId < 1) { throw new ArgumentNullException(nameof(locationId)); }

            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT e.emp_id, e.emp_no_1, e.emp_no_2, e.start_up_date, ");
            sb.Append("e.yrs_of_experience, e.start_up_designation, e.place_of_engagement, ");
            sb.Append("e.confirmation_date, e.current_designation, e.job_grade, e.employment_status, ");
            sb.Append("e.date_of_last_promotion, e.official_email, e.state_of_origin, e.is_dx, ");
            sb.Append("e.lga_of_origin, e.religion, e.geo_political_region, e.next_of_kin_name, ");
            sb.Append("e.next_of_kin_relationship, e.modified_by, e.modified_date, e.dx_time, e.dx_by, ");
            sb.Append("e.created_by, e.created_date, e.next_of_kin_address, e.next_of_kin_phone, ");
            sb.Append("e.next_of_kin_email, e.dept_id, e.unit_id, e.loc_id, e.coy_id, p.id, p.title, ");
            sb.Append("p.sname, p.fname, p.oname, p.fullname, p.sex, p.phone1, p.phone2,");
            sb.Append("p.email AS personal_email, p.address, p.mdb, p.mdt, p.ctb, p.ctt, ");
            sb.Append("p.imgp, p.birthday, p.birthmonth, p.birthyear, p.maritalstatus, l.locname, ");
            sb.Append("l.loctype, l.lochq1, l.lochq2, l.locmb, l.locmd, l.loccb, l.loccd, l.locctr, ");
            sb.Append("l.locst, l.locqk, c.coy_code, c.coy_name, d.deptname, d.depthd1, d.depthd2, ");
            sb.Append("d.deptqk, u.unitname, u.unithd1, u.unithd2, u.unitqk ");
            sb.Append("FROM erm_emp_inf e JOIN gst_prsns p ON e.emp_id = p.id ");
            sb.Append("AND e.is_dx = false ");
            sb.Append("LEFT JOIN gst_locs l ON e.loc_id = l.locqk ");
            sb.Append("LEFT JOIN gst_coys c ON e.coy_id = c.coy_code ");
            sb.Append("LEFT JOIN gst_depts d ON e.dept_id = d.deptqk ");
            sb.Append("LEFT JOIN gst_units u ON e.unit_id = u.unitqk ");
            sb.Append("WHERE(e.coy_id = @coy_id) AND (e.loc_id = @loc_id) ");
            sb.Append("AND (e.is_dx = false) ORDER BY p.fullname;");
            query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var coy_id = cmd.Parameters.Add("@coy_id", NpgsqlDbType.Text);
                    var loc_id = cmd.Parameters.Add("@loc_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    coy_id.Value = companyCode;
                    loc_id.Value = locationId;

                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        employeeList.Add(new Employee()
                        {
                            EmployeeID = reader["emp_id"] == DBNull.Value ? string.Empty : (reader["emp_id"]).ToString(),
                            EmployeeNo1 = reader["emp_no_1"] == DBNull.Value ? string.Empty : (reader["emp_no_1"]).ToString(),
                            EmployeeNo2 = reader["emp_no_2"] == DBNull.Value ? string.Empty : (reader["emp_no_2"]).ToString(),
                            StartUpDate = reader["start_up_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["start_up_date"],
                            YearsOfExperience = reader["yrs_of_experience"] == DBNull.Value ? 0 : (int)reader["yrs_of_experience"],
                            StartUpDesignation = reader["start_up_designation"] == DBNull.Value ? String.Empty : reader["start_up_designation"].ToString(),
                            PlaceOfEngagement = reader["place_of_engagement"] == DBNull.Value ? String.Empty : reader["place_of_engagement"].ToString(),
                            ConfirmationDate = reader["confirmation_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["confirmation_date"],
                            CurrentDesignation = reader["current_designation"] == DBNull.Value ? String.Empty : reader["current_designation"].ToString(),
                            JobGrade = reader["job_grade"] == DBNull.Value ? String.Empty : reader["job_grade"].ToString(),
                            EmploymentStatus = reader["employment_status"] == DBNull.Value ? String.Empty : reader["employment_status"].ToString(),
                            DateOfLastPromotion = reader["date_of_last_promotion"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["date_of_last_promotion"],
                            LengthOfService = reader["start_up_date"] == DBNull.Value ? 0 : (int)((DateTime.Now - (DateTime)reader["start_up_date"]).TotalDays),

                            OfficialEmail = reader["official_email"] == DBNull.Value ? String.Empty : reader["official_email"].ToString(),
                            StateOfOrigin = reader["state_of_origin"] == DBNull.Value ? String.Empty : reader["state_of_origin"].ToString(),
                            LgaOfOrigin = reader["lga_of_origin"] == DBNull.Value ? String.Empty : reader["lga_of_origin"].ToString(),
                            Religion = reader["religion"] == DBNull.Value ? String.Empty : reader["religion"].ToString(),
                            GeoPoliticalRegion = reader["geo_political_region"] == DBNull.Value ? String.Empty : reader["geo_political_region"].ToString(),
                            NextOfKinName = reader["next_of_kin_name"] == DBNull.Value ? String.Empty : reader["next_of_kin_name"].ToString(),
                            NextOfKinRelationship = reader["next_of_kin_relationship"] == DBNull.Value ? String.Empty : reader["next_of_kin_relationship"].ToString(),
                            NextOfKinAddress = reader["next_of_kin_address"] == DBNull.Value ? String.Empty : reader["next_of_kin_address"].ToString(),
                            NextOfKinPhone = reader["next_of_kin_phone"] == DBNull.Value ? String.Empty : reader["next_of_kin_phone"].ToString(),
                            NextOfKinEmail = reader["next_of_kin_email"] == DBNull.Value ? String.Empty : reader["next_of_kin_email"].ToString(),
                            CompanyID = reader["coy_id"] == DBNull.Value ? string.Empty : (reader["coy_id"]).ToString(),
                            DepartmentID = reader["dept_id"] == DBNull.Value ? 0 : (int)(reader["dept_id"]),
                            UnitID = reader["unit_id"] == DBNull.Value ? 0 : (int)(reader["unit_id"]),
                            LocationID = reader["loc_id"] == DBNull.Value ? 0 : (int)(reader["loc_id"]),
                            EmployeeModifiedBy = reader["modified_by"] == DBNull.Value ? string.Empty : reader["modified_by"].ToString(),
                            EmployeeModifiedDate = reader["modified_date"] == DBNull.Value ? string.Empty : reader["modified_date"].ToString(),
                            EmployeeCreatedBy = reader["created_by"] == DBNull.Value ? string.Empty : reader["created_by"].ToString(),
                            EmployeeCreatedDate = reader["created_date"] == DBNull.Value ? string.Empty : reader["created_date"].ToString(),
                            IsDeactivated = reader["is_dx"] == DBNull.Value ? true : (bool)reader["is_dx"],
                            DeactivationTime = reader["dx_time"] == DBNull.Value ? string.Empty : reader["dx_time"].ToString(),
                            DeactivatedBy = reader["dx_by"] == DBNull.Value ? string.Empty : reader["dx_by"].ToString(),

                            PersonID = reader["id"] == DBNull.Value ? String.Empty : reader["id"].ToString(),
                            Title = reader["title"] == DBNull.Value ? string.Empty : reader["title"].ToString(),
                            Surname = reader["sname"] == DBNull.Value ? string.Empty : reader["sname"].ToString(),
                            FirstName = reader["fname"] == DBNull.Value ? string.Empty : reader["fname"].ToString(),
                            OtherNames = reader["oname"] == DBNull.Value ? string.Empty : reader["oname"].ToString(),
                            FullName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                            Sex = reader["sex"] == DBNull.Value ? string.Empty : reader["sex"].ToString(),
                            MaritalStatus = reader["maritalstatus"] == DBNull.Value ? String.Empty : reader["maritalstatus"].ToString(),
                            BirthDay = reader["birthday"] == DBNull.Value ? 0 : (int)reader["birthday"],
                            BirthMonth = reader["birthmonth"] == DBNull.Value ? 0 : (int)reader["birthmonth"],
                            BirthYear = reader["birthyear"] == DBNull.Value ? 0 : (int)reader["birthyear"],
                            PhoneNo1 = reader["phone1"] == DBNull.Value ? string.Empty : reader["phone1"].ToString(),
                            PhoneNo2 = reader["phone2"] == DBNull.Value ? string.Empty : reader["phone2"].ToString(),
                            Email = reader["personal_email"] == DBNull.Value ? string.Empty : reader["personal_email"].ToString(),
                            Address = reader["address"] == DBNull.Value ? string.Empty : reader["address"].ToString(),
                            ImagePath = reader["imgp"] == DBNull.Value ? string.Empty : reader["imgp"].ToString(),
                            ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                            ModifiedTime = reader["mdt"] == DBNull.Value ? string.Empty : reader["mdt"].ToString(),
                            CreatedTime = reader["ctt"] == DBNull.Value ? string.Empty : reader["ctt"].ToString(),
                            CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),

                            LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            LocationType = reader["loctype"] == DBNull.Value ? string.Empty : reader["loctype"].ToString(),
                            LocationHead1 = reader["lochq1"] == DBNull.Value ? string.Empty : reader["lochq1"].ToString(),
                            LocationHead2 = reader["lochq2"] == DBNull.Value ? string.Empty : reader["lochq2"].ToString(),
                            LocationCountry = reader["locctr"] == DBNull.Value ? string.Empty : reader["locctr"].ToString(),
                            LocationState = reader["locst"] == DBNull.Value ? string.Empty : reader["locst"].ToString(),
                            CompanyName = reader["coy_name"] == DBNull.Value ? string.Empty : reader["coy_name"].ToString(),
                            DepartmentHead1 = reader["depthd1"] == DBNull.Value ? string.Empty : reader["depthd1"].ToString(),
                            DepartmentHead2 = reader["depthd2"] == DBNull.Value ? string.Empty : reader["depthd2"].ToString(),
                            DepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                            UnitHead1 = reader["unithd1"] == DBNull.Value ? string.Empty : reader["unithd1"].ToString(),
                            UnitHead2 = reader["unithd2"] == DBNull.Value ? string.Empty : reader["unithd2"].ToString(),
                            UnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
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
            return employeeList;
        }

        public async Task<IList<Employee>> GetEmployeesByCompanyCodeAsync(string companyCode, int locationId, int departmentId)
        {
            List<Employee> employeeList = new List<Employee>();
            if (string.IsNullOrWhiteSpace(companyCode)) { throw new ArgumentNullException(nameof(companyCode)); }
            if (locationId < 1) { throw new ArgumentNullException(nameof(locationId)); }
            if (departmentId < 1) { throw new ArgumentNullException(nameof(departmentId)); }

            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT e.emp_id, e.emp_no_1, e.emp_no_2, e.start_up_date, ");
            sb.Append("e.yrs_of_experience, e.start_up_designation, e.place_of_engagement, ");
            sb.Append("e.confirmation_date, e.current_designation, e.job_grade, e.employment_status, ");
            sb.Append("e.date_of_last_promotion, e.official_email, e.state_of_origin, e.is_dx, ");
            sb.Append("e.lga_of_origin, e.religion, e.geo_political_region, e.next_of_kin_name, ");
            sb.Append("e.next_of_kin_relationship, e.modified_by, e.modified_date, e.dx_time, e.dx_by, ");
            sb.Append("e.created_by, e.created_date, e.next_of_kin_address, e.next_of_kin_phone, ");
            sb.Append("e.next_of_kin_email, e.dept_id, e.unit_id, e.loc_id, e.coy_id, p.id, p.title, ");
            sb.Append("p.sname, p.fname, p.oname, p.fullname, p.sex, p.phone1, p.phone2,");
            sb.Append("p.email AS personal_email, p.address, p.mdb, p.mdt, p.ctb, p.ctt, ");
            sb.Append("p.imgp, p.birthday, p.birthmonth, p.birthyear, p.maritalstatus, l.locname, ");
            sb.Append("l.loctype, l.lochq1, l.lochq2, l.locmb, l.locmd, l.loccb, l.loccd, l.locctr, ");
            sb.Append("l.locst, l.locqk, c.coy_code, c.coy_name, d.deptname, d.depthd1, d.depthd2, ");
            sb.Append("d.deptqk, u.unitname, u.unithd1, u.unithd2, u.unitqk ");
            sb.Append("FROM erm_emp_inf e JOIN gst_prsns p ON e.emp_id = p.id ");
            sb.Append("AND e.is_dx = false ");
            sb.Append("LEFT JOIN gst_locs l ON e.loc_id = l.locqk ");
            sb.Append("LEFT JOIN gst_coys c ON e.coy_id = c.coy_code ");
            sb.Append("LEFT JOIN gst_depts d ON e.dept_id = d.deptqk ");
            sb.Append("LEFT JOIN gst_units u ON e.unit_id = u.unitqk ");
            sb.Append("WHERE(e.coy_id = @coy_id) AND (e.loc_id = @loc_id) ");
            sb.Append("AND (e.dept_id = @dept_id) ");
            sb.Append("AND (e.is_dx = false) ORDER BY p.fullname;");
            query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var coy_id = cmd.Parameters.Add("@coy_id", NpgsqlDbType.Text);
                    var loc_id = cmd.Parameters.Add("@loc_id", NpgsqlDbType.Integer);
                    var dept_id = cmd.Parameters.Add("@dept_id", NpgsqlDbType.Integer);

                    await cmd.PrepareAsync();
                    coy_id.Value = companyCode;
                    loc_id.Value = locationId;
                    dept_id.Value = departmentId;

                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        employeeList.Add(new Employee()
                        {
                            EmployeeID = reader["emp_id"] == DBNull.Value ? string.Empty : (reader["emp_id"]).ToString(),
                            EmployeeNo1 = reader["emp_no_1"] == DBNull.Value ? string.Empty : (reader["emp_no_1"]).ToString(),
                            EmployeeNo2 = reader["emp_no_2"] == DBNull.Value ? string.Empty : (reader["emp_no_2"]).ToString(),
                            StartUpDate = reader["start_up_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["start_up_date"],
                            YearsOfExperience = reader["yrs_of_experience"] == DBNull.Value ? 0 : (int)reader["yrs_of_experience"],
                            StartUpDesignation = reader["start_up_designation"] == DBNull.Value ? String.Empty : reader["start_up_designation"].ToString(),
                            PlaceOfEngagement = reader["place_of_engagement"] == DBNull.Value ? String.Empty : reader["place_of_engagement"].ToString(),
                            ConfirmationDate = reader["confirmation_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["confirmation_date"],
                            CurrentDesignation = reader["current_designation"] == DBNull.Value ? String.Empty : reader["current_designation"].ToString(),
                            JobGrade = reader["job_grade"] == DBNull.Value ? String.Empty : reader["job_grade"].ToString(),
                            EmploymentStatus = reader["employment_status"] == DBNull.Value ? String.Empty : reader["employment_status"].ToString(),
                            DateOfLastPromotion = reader["date_of_last_promotion"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["date_of_last_promotion"],
                            LengthOfService = reader["start_up_date"] == DBNull.Value ? 0 : (int)((DateTime.Now - (DateTime)reader["start_up_date"]).TotalDays),

                            OfficialEmail = reader["official_email"] == DBNull.Value ? String.Empty : reader["official_email"].ToString(),
                            StateOfOrigin = reader["state_of_origin"] == DBNull.Value ? String.Empty : reader["state_of_origin"].ToString(),
                            LgaOfOrigin = reader["lga_of_origin"] == DBNull.Value ? String.Empty : reader["lga_of_origin"].ToString(),
                            Religion = reader["religion"] == DBNull.Value ? String.Empty : reader["religion"].ToString(),
                            GeoPoliticalRegion = reader["geo_political_region"] == DBNull.Value ? String.Empty : reader["geo_political_region"].ToString(),
                            NextOfKinName = reader["next_of_kin_name"] == DBNull.Value ? String.Empty : reader["next_of_kin_name"].ToString(),
                            NextOfKinRelationship = reader["next_of_kin_relationship"] == DBNull.Value ? String.Empty : reader["next_of_kin_relationship"].ToString(),
                            NextOfKinAddress = reader["next_of_kin_address"] == DBNull.Value ? String.Empty : reader["next_of_kin_address"].ToString(),
                            NextOfKinPhone = reader["next_of_kin_phone"] == DBNull.Value ? String.Empty : reader["next_of_kin_phone"].ToString(),
                            NextOfKinEmail = reader["next_of_kin_email"] == DBNull.Value ? String.Empty : reader["next_of_kin_email"].ToString(),
                            CompanyID = reader["coy_id"] == DBNull.Value ? string.Empty : (reader["coy_id"]).ToString(),
                            DepartmentID = reader["dept_id"] == DBNull.Value ? 0 : (int)(reader["dept_id"]),
                            UnitID = reader["unit_id"] == DBNull.Value ? 0 : (int)(reader["unit_id"]),
                            LocationID = reader["loc_id"] == DBNull.Value ? 0 : (int)(reader["loc_id"]),
                            EmployeeModifiedBy = reader["modified_by"] == DBNull.Value ? string.Empty : reader["modified_by"].ToString(),
                            EmployeeModifiedDate = reader["modified_date"] == DBNull.Value ? string.Empty : reader["modified_date"].ToString(),
                            EmployeeCreatedBy = reader["created_by"] == DBNull.Value ? string.Empty : reader["created_by"].ToString(),
                            EmployeeCreatedDate = reader["created_date"] == DBNull.Value ? string.Empty : reader["created_date"].ToString(),
                            IsDeactivated = reader["is_dx"] == DBNull.Value ? true : (bool)reader["is_dx"],
                            DeactivationTime = reader["dx_time"] == DBNull.Value ? string.Empty : reader["dx_time"].ToString(),
                            DeactivatedBy = reader["dx_by"] == DBNull.Value ? string.Empty : reader["dx_by"].ToString(),

                            PersonID = reader["id"] == DBNull.Value ? String.Empty : reader["id"].ToString(),
                            Title = reader["title"] == DBNull.Value ? string.Empty : reader["title"].ToString(),
                            Surname = reader["sname"] == DBNull.Value ? string.Empty : reader["sname"].ToString(),
                            FirstName = reader["fname"] == DBNull.Value ? string.Empty : reader["fname"].ToString(),
                            OtherNames = reader["oname"] == DBNull.Value ? string.Empty : reader["oname"].ToString(),
                            FullName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                            Sex = reader["sex"] == DBNull.Value ? string.Empty : reader["sex"].ToString(),
                            MaritalStatus = reader["maritalstatus"] == DBNull.Value ? String.Empty : reader["maritalstatus"].ToString(),
                            BirthDay = reader["birthday"] == DBNull.Value ? 0 : (int)reader["birthday"],
                            BirthMonth = reader["birthmonth"] == DBNull.Value ? 0 : (int)reader["birthmonth"],
                            BirthYear = reader["birthyear"] == DBNull.Value ? 0 : (int)reader["birthyear"],
                            PhoneNo1 = reader["phone1"] == DBNull.Value ? string.Empty : reader["phone1"].ToString(),
                            PhoneNo2 = reader["phone2"] == DBNull.Value ? string.Empty : reader["phone2"].ToString(),
                            Email = reader["personal_email"] == DBNull.Value ? string.Empty : reader["personal_email"].ToString(),
                            Address = reader["address"] == DBNull.Value ? string.Empty : reader["address"].ToString(),
                            ImagePath = reader["imgp"] == DBNull.Value ? string.Empty : reader["imgp"].ToString(),
                            ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                            ModifiedTime = reader["mdt"] == DBNull.Value ? string.Empty : reader["mdt"].ToString(),
                            CreatedTime = reader["ctt"] == DBNull.Value ? string.Empty : reader["ctt"].ToString(),
                            CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),

                            LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            LocationType = reader["loctype"] == DBNull.Value ? string.Empty : reader["loctype"].ToString(),
                            LocationHead1 = reader["lochq1"] == DBNull.Value ? string.Empty : reader["lochq1"].ToString(),
                            LocationHead2 = reader["lochq2"] == DBNull.Value ? string.Empty : reader["lochq2"].ToString(),
                            LocationCountry = reader["locctr"] == DBNull.Value ? string.Empty : reader["locctr"].ToString(),
                            LocationState = reader["locst"] == DBNull.Value ? string.Empty : reader["locst"].ToString(),
                            CompanyName = reader["coy_name"] == DBNull.Value ? string.Empty : reader["coy_name"].ToString(),
                            DepartmentHead1 = reader["depthd1"] == DBNull.Value ? string.Empty : reader["depthd1"].ToString(),
                            DepartmentHead2 = reader["depthd2"] == DBNull.Value ? string.Empty : reader["depthd2"].ToString(),
                            DepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                            UnitHead1 = reader["unithd1"] == DBNull.Value ? string.Empty : reader["unithd1"].ToString(),
                            UnitHead2 = reader["unithd2"] == DBNull.Value ? string.Empty : reader["unithd2"].ToString(),
                            UnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
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
            return employeeList;
        }

        public async Task<IList<Employee>> GetEmployeesByCompanyCodeAndUnitAsync(string companyCode, int locationId, int unitId)
        {
            List<Employee> employeeList = new List<Employee>();
            if (string.IsNullOrWhiteSpace(companyCode)) { throw new ArgumentNullException(nameof(companyCode)); }
            if (locationId < 1) { throw new ArgumentNullException(nameof(locationId)); }
            if (unitId < 1) { throw new ArgumentNullException(nameof(unitId)); }

            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT e.emp_id, e.emp_no_1, e.emp_no_2, e.start_up_date, ");
            sb.Append("e.yrs_of_experience, e.start_up_designation, e.place_of_engagement, ");
            sb.Append("e.confirmation_date, e.current_designation, e.job_grade, e.employment_status, ");
            sb.Append("e.date_of_last_promotion, e.official_email, e.state_of_origin, e.is_dx, ");
            sb.Append("e.lga_of_origin, e.religion, e.geo_political_region, e.next_of_kin_name, ");
            sb.Append("e.next_of_kin_relationship, e.modified_by, e.modified_date, e.dx_time, e.dx_by, ");
            sb.Append("e.created_by, e.created_date, e.next_of_kin_address, e.next_of_kin_phone, ");
            sb.Append("e.next_of_kin_email, e.dept_id, e.unit_id, e.loc_id, e.coy_id, p.id, p.title, ");
            sb.Append("p.sname, p.fname, p.oname, p.fullname, p.sex, p.phone1, p.phone2,");
            sb.Append("p.email AS personal_email, p.address, p.mdb, p.mdt, p.ctb, p.ctt, ");
            sb.Append("p.imgp, p.birthday, p.birthmonth, p.birthyear, p.maritalstatus, l.locname, ");
            sb.Append("l.loctype, l.lochq1, l.lochq2, l.locmb, l.locmd, l.loccb, l.loccd, l.locctr, ");
            sb.Append("l.locst, l.locqk, c.coy_code, c.coy_name, d.deptname, d.depthd1, d.depthd2, ");
            sb.Append("d.deptqk, u.unitname, u.unithd1, u.unithd2, u.unitqk ");
            sb.Append("FROM erm_emp_inf e JOIN gst_prsns p ON e.emp_id = p.id ");
            sb.Append("AND e.is_dx = false ");
            sb.Append("LEFT JOIN gst_locs l ON e.loc_id = l.locqk ");
            sb.Append("LEFT JOIN gst_coys c ON e.coy_id = c.coy_code ");
            sb.Append("LEFT JOIN gst_depts d ON e.dept_id = d.deptqk ");
            sb.Append("LEFT JOIN gst_units u ON e.unit_id = u.unitqk ");
            sb.Append("WHERE(e.coy_id = @coy_id) AND (e.loc_id = @loc_id) ");
            sb.Append("AND (e.unit_id = @unit_id) ");
            sb.Append("AND (e.is_dx = false) ORDER BY p.fullname;");
            query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var coy_id = cmd.Parameters.Add("@coy_id", NpgsqlDbType.Text);
                    var loc_id = cmd.Parameters.Add("@loc_id", NpgsqlDbType.Integer);
                    var unit_id = cmd.Parameters.Add("@unit_id", NpgsqlDbType.Integer);

                    await cmd.PrepareAsync();
                    coy_id.Value = companyCode;
                    loc_id.Value = locationId;
                    unit_id.Value = unitId;

                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        employeeList.Add(new Employee()
                        {
                            EmployeeID = reader["emp_id"] == DBNull.Value ? string.Empty : (reader["emp_id"]).ToString(),
                            EmployeeNo1 = reader["emp_no_1"] == DBNull.Value ? string.Empty : (reader["emp_no_1"]).ToString(),
                            EmployeeNo2 = reader["emp_no_2"] == DBNull.Value ? string.Empty : (reader["emp_no_2"]).ToString(),
                            StartUpDate = reader["start_up_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["start_up_date"],
                            YearsOfExperience = reader["yrs_of_experience"] == DBNull.Value ? 0 : (int)reader["yrs_of_experience"],
                            StartUpDesignation = reader["start_up_designation"] == DBNull.Value ? String.Empty : reader["start_up_designation"].ToString(),
                            PlaceOfEngagement = reader["place_of_engagement"] == DBNull.Value ? String.Empty : reader["place_of_engagement"].ToString(),
                            ConfirmationDate = reader["confirmation_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["confirmation_date"],
                            CurrentDesignation = reader["current_designation"] == DBNull.Value ? String.Empty : reader["current_designation"].ToString(),
                            JobGrade = reader["job_grade"] == DBNull.Value ? String.Empty : reader["job_grade"].ToString(),
                            EmploymentStatus = reader["employment_status"] == DBNull.Value ? String.Empty : reader["employment_status"].ToString(),
                            DateOfLastPromotion = reader["date_of_last_promotion"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["date_of_last_promotion"],
                            LengthOfService = reader["start_up_date"] == DBNull.Value ? 0 : (int)((DateTime.Now - (DateTime)reader["start_up_date"]).TotalDays),

                            OfficialEmail = reader["official_email"] == DBNull.Value ? String.Empty : reader["official_email"].ToString(),
                            StateOfOrigin = reader["state_of_origin"] == DBNull.Value ? String.Empty : reader["state_of_origin"].ToString(),
                            LgaOfOrigin = reader["lga_of_origin"] == DBNull.Value ? String.Empty : reader["lga_of_origin"].ToString(),
                            Religion = reader["religion"] == DBNull.Value ? String.Empty : reader["religion"].ToString(),
                            GeoPoliticalRegion = reader["geo_political_region"] == DBNull.Value ? String.Empty : reader["geo_political_region"].ToString(),
                            NextOfKinName = reader["next_of_kin_name"] == DBNull.Value ? String.Empty : reader["next_of_kin_name"].ToString(),
                            NextOfKinRelationship = reader["next_of_kin_relationship"] == DBNull.Value ? String.Empty : reader["next_of_kin_relationship"].ToString(),
                            NextOfKinAddress = reader["next_of_kin_address"] == DBNull.Value ? String.Empty : reader["next_of_kin_address"].ToString(),
                            NextOfKinPhone = reader["next_of_kin_phone"] == DBNull.Value ? String.Empty : reader["next_of_kin_phone"].ToString(),
                            NextOfKinEmail = reader["next_of_kin_email"] == DBNull.Value ? String.Empty : reader["next_of_kin_email"].ToString(),
                            CompanyID = reader["coy_id"] == DBNull.Value ? string.Empty : (reader["coy_id"]).ToString(),
                            DepartmentID = reader["dept_id"] == DBNull.Value ? 0 : (int)(reader["dept_id"]),
                            UnitID = reader["unit_id"] == DBNull.Value ? 0 : (int)(reader["unit_id"]),
                            LocationID = reader["loc_id"] == DBNull.Value ? 0 : (int)(reader["loc_id"]),
                            EmployeeModifiedBy = reader["modified_by"] == DBNull.Value ? string.Empty : reader["modified_by"].ToString(),
                            EmployeeModifiedDate = reader["modified_date"] == DBNull.Value ? string.Empty : reader["modified_date"].ToString(),
                            EmployeeCreatedBy = reader["created_by"] == DBNull.Value ? string.Empty : reader["created_by"].ToString(),
                            EmployeeCreatedDate = reader["created_date"] == DBNull.Value ? string.Empty : reader["created_date"].ToString(),
                            IsDeactivated = reader["is_dx"] == DBNull.Value ? true : (bool)reader["is_dx"],
                            DeactivationTime = reader["dx_time"] == DBNull.Value ? string.Empty : reader["dx_time"].ToString(),
                            DeactivatedBy = reader["dx_by"] == DBNull.Value ? string.Empty : reader["dx_by"].ToString(),

                            PersonID = reader["id"] == DBNull.Value ? String.Empty : reader["id"].ToString(),
                            Title = reader["title"] == DBNull.Value ? string.Empty : reader["title"].ToString(),
                            Surname = reader["sname"] == DBNull.Value ? string.Empty : reader["sname"].ToString(),
                            FirstName = reader["fname"] == DBNull.Value ? string.Empty : reader["fname"].ToString(),
                            OtherNames = reader["oname"] == DBNull.Value ? string.Empty : reader["oname"].ToString(),
                            FullName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                            Sex = reader["sex"] == DBNull.Value ? string.Empty : reader["sex"].ToString(),
                            MaritalStatus = reader["maritalstatus"] == DBNull.Value ? String.Empty : reader["maritalstatus"].ToString(),
                            BirthDay = reader["birthday"] == DBNull.Value ? 0 : (int)reader["birthday"],
                            BirthMonth = reader["birthmonth"] == DBNull.Value ? 0 : (int)reader["birthmonth"],
                            BirthYear = reader["birthyear"] == DBNull.Value ? 0 : (int)reader["birthyear"],
                            PhoneNo1 = reader["phone1"] == DBNull.Value ? string.Empty : reader["phone1"].ToString(),
                            PhoneNo2 = reader["phone2"] == DBNull.Value ? string.Empty : reader["phone2"].ToString(),
                            Email = reader["personal_email"] == DBNull.Value ? string.Empty : reader["personal_email"].ToString(),
                            Address = reader["address"] == DBNull.Value ? string.Empty : reader["address"].ToString(),
                            ImagePath = reader["imgp"] == DBNull.Value ? string.Empty : reader["imgp"].ToString(),
                            ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                            ModifiedTime = reader["mdt"] == DBNull.Value ? string.Empty : reader["mdt"].ToString(),
                            CreatedTime = reader["ctt"] == DBNull.Value ? string.Empty : reader["ctt"].ToString(),
                            CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),

                            LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            LocationType = reader["loctype"] == DBNull.Value ? string.Empty : reader["loctype"].ToString(),
                            LocationHead1 = reader["lochq1"] == DBNull.Value ? string.Empty : reader["lochq1"].ToString(),
                            LocationHead2 = reader["lochq2"] == DBNull.Value ? string.Empty : reader["lochq2"].ToString(),
                            LocationCountry = reader["locctr"] == DBNull.Value ? string.Empty : reader["locctr"].ToString(),
                            LocationState = reader["locst"] == DBNull.Value ? string.Empty : reader["locst"].ToString(),
                            CompanyName = reader["coy_name"] == DBNull.Value ? string.Empty : reader["coy_name"].ToString(),
                            DepartmentHead1 = reader["depthd1"] == DBNull.Value ? string.Empty : reader["depthd1"].ToString(),
                            DepartmentHead2 = reader["depthd2"] == DBNull.Value ? string.Empty : reader["depthd2"].ToString(),
                            DepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                            UnitHead1 = reader["unithd1"] == DBNull.Value ? string.Empty : reader["unithd1"].ToString(),
                            UnitHead2 = reader["unithd2"] == DBNull.Value ? string.Empty : reader["unithd2"].ToString(),
                            UnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
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
            return employeeList;
        }

        public async Task<IList<Employee>> GetEmployeesByCompanyCodeAndUnitAsync(string companyCode, int unitId)
        {
            List<Employee> employeeList = new List<Employee>();
            if (string.IsNullOrWhiteSpace(companyCode)) { throw new ArgumentNullException(nameof(companyCode)); }
            if (unitId < 1) { throw new ArgumentNullException(nameof(unitId)); }

            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT e.emp_id, e.emp_no_1, e.emp_no_2, e.start_up_date, ");
            sb.Append("e.yrs_of_experience, e.start_up_designation, e.place_of_engagement, ");
            sb.Append("e.confirmation_date, e.current_designation, e.job_grade, e.employment_status, ");
            sb.Append("e.date_of_last_promotion, e.official_email, e.state_of_origin, e.is_dx, ");
            sb.Append("e.lga_of_origin, e.religion, e.geo_political_region, e.next_of_kin_name, ");
            sb.Append("e.next_of_kin_relationship, e.modified_by, e.modified_date, e.dx_time, e.dx_by, ");
            sb.Append("e.created_by, e.created_date, e.next_of_kin_address, e.next_of_kin_phone, ");
            sb.Append("e.next_of_kin_email, e.dept_id, e.unit_id, e.loc_id, e.coy_id, p.id, p.title, ");
            sb.Append("p.sname, p.fname, p.oname, p.fullname, p.sex, p.phone1, p.phone2,");
            sb.Append("p.email AS personal_email, p.address, p.mdb, p.mdt, p.ctb, p.ctt, ");
            sb.Append("p.imgp, p.birthday, p.birthmonth, p.birthyear, p.maritalstatus, l.locname, ");
            sb.Append("l.loctype, l.lochq1, l.lochq2, l.locmb, l.locmd, l.loccb, l.loccd, l.locctr, ");
            sb.Append("l.locst, l.locqk, c.coy_code, c.coy_name, d.deptname, d.depthd1, d.depthd2, ");
            sb.Append("d.deptqk, u.unitname, u.unithd1, u.unithd2, u.unitqk ");
            sb.Append("FROM erm_emp_inf e JOIN gst_prsns p ON e.emp_id = p.id ");
            sb.Append("AND e.is_dx = false ");
            sb.Append("LEFT JOIN gst_locs l ON e.loc_id = l.locqk ");
            sb.Append("LEFT JOIN gst_coys c ON e.coy_id = c.coy_code ");
            sb.Append("LEFT JOIN gst_depts d ON e.dept_id = d.deptqk ");
            sb.Append("LEFT JOIN gst_units u ON e.unit_id = u.unitqk ");
            sb.Append("WHERE(e.coy_id = @coy_id) ");
            sb.Append("AND (e.unit_id = @unit_id) ");
            sb.Append("AND (e.is_dx = false) ORDER BY p.fullname;");
            query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var coy_id = cmd.Parameters.Add("@coy_id", NpgsqlDbType.Text);
                    var unit_id = cmd.Parameters.Add("@unit_id", NpgsqlDbType.Integer);

                    await cmd.PrepareAsync();
                    coy_id.Value = companyCode;
                    unit_id.Value = unitId;

                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        employeeList.Add(new Employee()
                        {
                            EmployeeID = reader["emp_id"] == DBNull.Value ? string.Empty : (reader["emp_id"]).ToString(),
                            EmployeeNo1 = reader["emp_no_1"] == DBNull.Value ? string.Empty : (reader["emp_no_1"]).ToString(),
                            EmployeeNo2 = reader["emp_no_2"] == DBNull.Value ? string.Empty : (reader["emp_no_2"]).ToString(),
                            StartUpDate = reader["start_up_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["start_up_date"],
                            YearsOfExperience = reader["yrs_of_experience"] == DBNull.Value ? 0 : (int)reader["yrs_of_experience"],
                            StartUpDesignation = reader["start_up_designation"] == DBNull.Value ? String.Empty : reader["start_up_designation"].ToString(),
                            PlaceOfEngagement = reader["place_of_engagement"] == DBNull.Value ? String.Empty : reader["place_of_engagement"].ToString(),
                            ConfirmationDate = reader["confirmation_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["confirmation_date"],
                            CurrentDesignation = reader["current_designation"] == DBNull.Value ? String.Empty : reader["current_designation"].ToString(),
                            JobGrade = reader["job_grade"] == DBNull.Value ? String.Empty : reader["job_grade"].ToString(),
                            EmploymentStatus = reader["employment_status"] == DBNull.Value ? String.Empty : reader["employment_status"].ToString(),
                            DateOfLastPromotion = reader["date_of_last_promotion"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["date_of_last_promotion"],
                            LengthOfService = reader["start_up_date"] == DBNull.Value ? 0 : (int)((DateTime.Now - (DateTime)reader["start_up_date"]).TotalDays),

                            OfficialEmail = reader["official_email"] == DBNull.Value ? String.Empty : reader["official_email"].ToString(),
                            StateOfOrigin = reader["state_of_origin"] == DBNull.Value ? String.Empty : reader["state_of_origin"].ToString(),
                            LgaOfOrigin = reader["lga_of_origin"] == DBNull.Value ? String.Empty : reader["lga_of_origin"].ToString(),
                            Religion = reader["religion"] == DBNull.Value ? String.Empty : reader["religion"].ToString(),
                            GeoPoliticalRegion = reader["geo_political_region"] == DBNull.Value ? String.Empty : reader["geo_political_region"].ToString(),
                            NextOfKinName = reader["next_of_kin_name"] == DBNull.Value ? String.Empty : reader["next_of_kin_name"].ToString(),
                            NextOfKinRelationship = reader["next_of_kin_relationship"] == DBNull.Value ? String.Empty : reader["next_of_kin_relationship"].ToString(),
                            NextOfKinAddress = reader["next_of_kin_address"] == DBNull.Value ? String.Empty : reader["next_of_kin_address"].ToString(),
                            NextOfKinPhone = reader["next_of_kin_phone"] == DBNull.Value ? String.Empty : reader["next_of_kin_phone"].ToString(),
                            NextOfKinEmail = reader["next_of_kin_email"] == DBNull.Value ? String.Empty : reader["next_of_kin_email"].ToString(),
                            CompanyID = reader["coy_id"] == DBNull.Value ? string.Empty : (reader["coy_id"]).ToString(),
                            DepartmentID = reader["dept_id"] == DBNull.Value ? 0 : (int)(reader["dept_id"]),
                            UnitID = reader["unit_id"] == DBNull.Value ? 0 : (int)(reader["unit_id"]),
                            LocationID = reader["loc_id"] == DBNull.Value ? 0 : (int)(reader["loc_id"]),
                            EmployeeModifiedBy = reader["modified_by"] == DBNull.Value ? string.Empty : reader["modified_by"].ToString(),
                            EmployeeModifiedDate = reader["modified_date"] == DBNull.Value ? string.Empty : reader["modified_date"].ToString(),
                            EmployeeCreatedBy = reader["created_by"] == DBNull.Value ? string.Empty : reader["created_by"].ToString(),
                            EmployeeCreatedDate = reader["created_date"] == DBNull.Value ? string.Empty : reader["created_date"].ToString(),
                            IsDeactivated = reader["is_dx"] == DBNull.Value ? true : (bool)reader["is_dx"],
                            DeactivationTime = reader["dx_time"] == DBNull.Value ? string.Empty : reader["dx_time"].ToString(),
                            DeactivatedBy = reader["dx_by"] == DBNull.Value ? string.Empty : reader["dx_by"].ToString(),

                            PersonID = reader["id"] == DBNull.Value ? String.Empty : reader["id"].ToString(),
                            Title = reader["title"] == DBNull.Value ? string.Empty : reader["title"].ToString(),
                            Surname = reader["sname"] == DBNull.Value ? string.Empty : reader["sname"].ToString(),
                            FirstName = reader["fname"] == DBNull.Value ? string.Empty : reader["fname"].ToString(),
                            OtherNames = reader["oname"] == DBNull.Value ? string.Empty : reader["oname"].ToString(),
                            FullName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                            Sex = reader["sex"] == DBNull.Value ? string.Empty : reader["sex"].ToString(),
                            MaritalStatus = reader["maritalstatus"] == DBNull.Value ? String.Empty : reader["maritalstatus"].ToString(),
                            BirthDay = reader["birthday"] == DBNull.Value ? 0 : (int)reader["birthday"],
                            BirthMonth = reader["birthmonth"] == DBNull.Value ? 0 : (int)reader["birthmonth"],
                            BirthYear = reader["birthyear"] == DBNull.Value ? 0 : (int)reader["birthyear"],
                            PhoneNo1 = reader["phone1"] == DBNull.Value ? string.Empty : reader["phone1"].ToString(),
                            PhoneNo2 = reader["phone2"] == DBNull.Value ? string.Empty : reader["phone2"].ToString(),
                            Email = reader["personal_email"] == DBNull.Value ? string.Empty : reader["personal_email"].ToString(),
                            Address = reader["address"] == DBNull.Value ? string.Empty : reader["address"].ToString(),
                            ImagePath = reader["imgp"] == DBNull.Value ? string.Empty : reader["imgp"].ToString(),
                            ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                            ModifiedTime = reader["mdt"] == DBNull.Value ? string.Empty : reader["mdt"].ToString(),
                            CreatedTime = reader["ctt"] == DBNull.Value ? string.Empty : reader["ctt"].ToString(),
                            CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),

                            LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            LocationType = reader["loctype"] == DBNull.Value ? string.Empty : reader["loctype"].ToString(),
                            LocationHead1 = reader["lochq1"] == DBNull.Value ? string.Empty : reader["lochq1"].ToString(),
                            LocationHead2 = reader["lochq2"] == DBNull.Value ? string.Empty : reader["lochq2"].ToString(),
                            LocationCountry = reader["locctr"] == DBNull.Value ? string.Empty : reader["locctr"].ToString(),
                            LocationState = reader["locst"] == DBNull.Value ? string.Empty : reader["locst"].ToString(),
                            CompanyName = reader["coy_name"] == DBNull.Value ? string.Empty : reader["coy_name"].ToString(),
                            DepartmentHead1 = reader["depthd1"] == DBNull.Value ? string.Empty : reader["depthd1"].ToString(),
                            DepartmentHead2 = reader["depthd2"] == DBNull.Value ? string.Empty : reader["depthd2"].ToString(),
                            DepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                            UnitHead1 = reader["unithd1"] == DBNull.Value ? string.Empty : reader["unithd1"].ToString(),
                            UnitHead2 = reader["unithd2"] == DBNull.Value ? string.Empty : reader["unithd2"].ToString(),
                            UnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
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
            return employeeList;
        }

        public async Task<IList<Employee>> GetEmployeesByCompanyCodeAndDeptAsync(string companyCode, int deptId)
        {
            List<Employee> employeeList = new List<Employee>();
            if (string.IsNullOrWhiteSpace(companyCode)) { throw new ArgumentNullException(nameof(companyCode)); }
            if (deptId < 1) { throw new ArgumentNullException(nameof(deptId)); }

            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT e.emp_id, e.emp_no_1, e.emp_no_2, e.start_up_date, ");
            sb.Append("e.yrs_of_experience, e.start_up_designation, e.place_of_engagement, ");
            sb.Append("e.confirmation_date, e.current_designation, e.job_grade, e.employment_status, ");
            sb.Append("e.date_of_last_promotion, e.official_email, e.state_of_origin, e.is_dx, ");
            sb.Append("e.lga_of_origin, e.religion, e.geo_political_region, e.next_of_kin_name, ");
            sb.Append("e.next_of_kin_relationship, e.modified_by, e.modified_date, e.dx_time, e.dx_by, ");
            sb.Append("e.created_by, e.created_date, e.next_of_kin_address, e.next_of_kin_phone, ");
            sb.Append("e.next_of_kin_email, e.dept_id, e.unit_id, e.loc_id, e.coy_id, p.id, p.title, ");
            sb.Append("p.sname, p.fname, p.oname, p.fullname, p.sex, p.phone1, p.phone2,");
            sb.Append("p.email AS personal_email, p.address, p.mdb, p.mdt, p.ctb, p.ctt, ");
            sb.Append("p.imgp, p.birthday, p.birthmonth, p.birthyear, p.maritalstatus, l.locname, ");
            sb.Append("l.loctype, l.lochq1, l.lochq2, l.locmb, l.locmd, l.loccb, l.loccd, l.locctr, ");
            sb.Append("l.locst, l.locqk, c.coy_code, c.coy_name, d.deptname, d.depthd1, d.depthd2, ");
            sb.Append("d.deptqk, u.unitname, u.unithd1, u.unithd2, u.unitqk ");
            sb.Append("FROM erm_emp_inf e JOIN gst_prsns p ON e.emp_id = p.id ");
            sb.Append("AND e.is_dx = false ");
            sb.Append("LEFT JOIN gst_locs l ON e.loc_id = l.locqk ");
            sb.Append("LEFT JOIN gst_coys c ON e.coy_id = c.coy_code ");
            sb.Append("LEFT JOIN gst_depts d ON e.dept_id = d.deptqk ");
            sb.Append("LEFT JOIN gst_units u ON e.unit_id = u.unitqk ");
            sb.Append("WHERE(e.coy_id = @coy_id) ");
            sb.Append("AND (e.unit_id = @unit_id) ");
            sb.Append("AND (e.is_dx = false) ORDER BY p.fullname;");
            query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var coy_id = cmd.Parameters.Add("@coy_id", NpgsqlDbType.Text);
                    var dept_id = cmd.Parameters.Add("@dept_id", NpgsqlDbType.Integer);

                    await cmd.PrepareAsync();
                    coy_id.Value = companyCode;
                    dept_id.Value = deptId;

                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        employeeList.Add(new Employee()
                        {
                            EmployeeID = reader["emp_id"] == DBNull.Value ? string.Empty : (reader["emp_id"]).ToString(),
                            EmployeeNo1 = reader["emp_no_1"] == DBNull.Value ? string.Empty : (reader["emp_no_1"]).ToString(),
                            EmployeeNo2 = reader["emp_no_2"] == DBNull.Value ? string.Empty : (reader["emp_no_2"]).ToString(),
                            StartUpDate = reader["start_up_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["start_up_date"],
                            YearsOfExperience = reader["yrs_of_experience"] == DBNull.Value ? 0 : (int)reader["yrs_of_experience"],
                            StartUpDesignation = reader["start_up_designation"] == DBNull.Value ? String.Empty : reader["start_up_designation"].ToString(),
                            PlaceOfEngagement = reader["place_of_engagement"] == DBNull.Value ? String.Empty : reader["place_of_engagement"].ToString(),
                            ConfirmationDate = reader["confirmation_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["confirmation_date"],
                            CurrentDesignation = reader["current_designation"] == DBNull.Value ? String.Empty : reader["current_designation"].ToString(),
                            JobGrade = reader["job_grade"] == DBNull.Value ? String.Empty : reader["job_grade"].ToString(),
                            EmploymentStatus = reader["employment_status"] == DBNull.Value ? String.Empty : reader["employment_status"].ToString(),
                            DateOfLastPromotion = reader["date_of_last_promotion"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["date_of_last_promotion"],
                            LengthOfService = reader["start_up_date"] == DBNull.Value ? 0 : (int)((DateTime.Now - (DateTime)reader["start_up_date"]).TotalDays),

                            OfficialEmail = reader["official_email"] == DBNull.Value ? String.Empty : reader["official_email"].ToString(),
                            StateOfOrigin = reader["state_of_origin"] == DBNull.Value ? String.Empty : reader["state_of_origin"].ToString(),
                            LgaOfOrigin = reader["lga_of_origin"] == DBNull.Value ? String.Empty : reader["lga_of_origin"].ToString(),
                            Religion = reader["religion"] == DBNull.Value ? String.Empty : reader["religion"].ToString(),
                            GeoPoliticalRegion = reader["geo_political_region"] == DBNull.Value ? String.Empty : reader["geo_political_region"].ToString(),
                            NextOfKinName = reader["next_of_kin_name"] == DBNull.Value ? String.Empty : reader["next_of_kin_name"].ToString(),
                            NextOfKinRelationship = reader["next_of_kin_relationship"] == DBNull.Value ? String.Empty : reader["next_of_kin_relationship"].ToString(),
                            NextOfKinAddress = reader["next_of_kin_address"] == DBNull.Value ? String.Empty : reader["next_of_kin_address"].ToString(),
                            NextOfKinPhone = reader["next_of_kin_phone"] == DBNull.Value ? String.Empty : reader["next_of_kin_phone"].ToString(),
                            NextOfKinEmail = reader["next_of_kin_email"] == DBNull.Value ? String.Empty : reader["next_of_kin_email"].ToString(),
                            CompanyID = reader["coy_id"] == DBNull.Value ? string.Empty : (reader["coy_id"]).ToString(),
                            DepartmentID = reader["dept_id"] == DBNull.Value ? 0 : (int)(reader["dept_id"]),
                            UnitID = reader["unit_id"] == DBNull.Value ? 0 : (int)(reader["unit_id"]),
                            LocationID = reader["loc_id"] == DBNull.Value ? 0 : (int)(reader["loc_id"]),
                            EmployeeModifiedBy = reader["modified_by"] == DBNull.Value ? string.Empty : reader["modified_by"].ToString(),
                            EmployeeModifiedDate = reader["modified_date"] == DBNull.Value ? string.Empty : reader["modified_date"].ToString(),
                            EmployeeCreatedBy = reader["created_by"] == DBNull.Value ? string.Empty : reader["created_by"].ToString(),
                            EmployeeCreatedDate = reader["created_date"] == DBNull.Value ? string.Empty : reader["created_date"].ToString(),
                            IsDeactivated = reader["is_dx"] == DBNull.Value ? true : (bool)reader["is_dx"],
                            DeactivationTime = reader["dx_time"] == DBNull.Value ? string.Empty : reader["dx_time"].ToString(),
                            DeactivatedBy = reader["dx_by"] == DBNull.Value ? string.Empty : reader["dx_by"].ToString(),

                            PersonID = reader["id"] == DBNull.Value ? String.Empty : reader["id"].ToString(),
                            Title = reader["title"] == DBNull.Value ? string.Empty : reader["title"].ToString(),
                            Surname = reader["sname"] == DBNull.Value ? string.Empty : reader["sname"].ToString(),
                            FirstName = reader["fname"] == DBNull.Value ? string.Empty : reader["fname"].ToString(),
                            OtherNames = reader["oname"] == DBNull.Value ? string.Empty : reader["oname"].ToString(),
                            FullName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                            Sex = reader["sex"] == DBNull.Value ? string.Empty : reader["sex"].ToString(),
                            MaritalStatus = reader["maritalstatus"] == DBNull.Value ? String.Empty : reader["maritalstatus"].ToString(),
                            BirthDay = reader["birthday"] == DBNull.Value ? 0 : (int)reader["birthday"],
                            BirthMonth = reader["birthmonth"] == DBNull.Value ? 0 : (int)reader["birthmonth"],
                            BirthYear = reader["birthyear"] == DBNull.Value ? 0 : (int)reader["birthyear"],
                            PhoneNo1 = reader["phone1"] == DBNull.Value ? string.Empty : reader["phone1"].ToString(),
                            PhoneNo2 = reader["phone2"] == DBNull.Value ? string.Empty : reader["phone2"].ToString(),
                            Email = reader["personal_email"] == DBNull.Value ? string.Empty : reader["personal_email"].ToString(),
                            Address = reader["address"] == DBNull.Value ? string.Empty : reader["address"].ToString(),
                            ImagePath = reader["imgp"] == DBNull.Value ? string.Empty : reader["imgp"].ToString(),
                            ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                            ModifiedTime = reader["mdt"] == DBNull.Value ? string.Empty : reader["mdt"].ToString(),
                            CreatedTime = reader["ctt"] == DBNull.Value ? string.Empty : reader["ctt"].ToString(),
                            CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),

                            LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            LocationType = reader["loctype"] == DBNull.Value ? string.Empty : reader["loctype"].ToString(),
                            LocationHead1 = reader["lochq1"] == DBNull.Value ? string.Empty : reader["lochq1"].ToString(),
                            LocationHead2 = reader["lochq2"] == DBNull.Value ? string.Empty : reader["lochq2"].ToString(),
                            LocationCountry = reader["locctr"] == DBNull.Value ? string.Empty : reader["locctr"].ToString(),
                            LocationState = reader["locst"] == DBNull.Value ? string.Empty : reader["locst"].ToString(),
                            CompanyName = reader["coy_name"] == DBNull.Value ? string.Empty : reader["coy_name"].ToString(),
                            DepartmentHead1 = reader["depthd1"] == DBNull.Value ? string.Empty : reader["depthd1"].ToString(),
                            DepartmentHead2 = reader["depthd2"] == DBNull.Value ? string.Empty : reader["depthd2"].ToString(),
                            DepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                            UnitHead1 = reader["unithd1"] == DBNull.Value ? string.Empty : reader["unithd1"].ToString(),
                            UnitHead2 = reader["unithd2"] == DBNull.Value ? string.Empty : reader["unithd2"].ToString(),
                            UnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
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
            return employeeList;
        }

        public async Task<IList<Employee>> GetEmployeesByLocationAsync(int locationId)
        {
            List<Employee> employeeList = new List<Employee>();
            if (locationId < 1) { throw new ArgumentNullException(nameof(locationId)); }
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT e.emp_id, e.emp_no_1, e.emp_no_2, e.start_up_date, ");
            sb.Append("e.yrs_of_experience, e.start_up_designation, e.place_of_engagement, ");
            sb.Append("e.confirmation_date, e.current_designation, e.job_grade, e.employment_status, ");
            sb.Append("e.date_of_last_promotion, e.official_email, e.state_of_origin, e.is_dx, ");
            sb.Append("e.lga_of_origin, e.religion, e.geo_political_region, e.next_of_kin_name, ");
            sb.Append("e.next_of_kin_relationship, e.modified_by, e.modified_date, e.dx_time, e.dx_by, ");
            sb.Append("e.created_by, e.created_date, e.next_of_kin_address, e.next_of_kin_phone, ");
            sb.Append("e.next_of_kin_email, e.dept_id, e.unit_id, e.loc_id, e.coy_id, p.id, p.title, ");
            sb.Append("p.sname, p.fname, p.oname, p.fullname, p.sex, p.phone1, p.phone2,");
            sb.Append("p.email AS personal_email, p.address, p.mdb, p.mdt, p.ctb, p.ctt, ");
            sb.Append("p.imgp, p.birthday, p.birthmonth, p.birthyear, p.maritalstatus, l.locname, ");
            sb.Append("l.loctype, l.lochq1, l.lochq2, l.locmb, l.locmd, l.loccb, l.loccd, l.locctr, ");
            sb.Append("l.locst, l.locqk, c.coy_code, c.coy_name, d.deptname, d.depthd1, d.depthd2, ");
            sb.Append("d.deptqk, u.unitname, u.unithd1, u.unithd2, u.unitqk ");
            sb.Append("FROM erm_emp_inf e JOIN gst_prsns p ON e.emp_id = p.id ");
            sb.Append("AND e.is_dx = false ");
            sb.Append("LEFT JOIN gst_locs l ON e.loc_id = l.locqk ");
            sb.Append("LEFT JOIN gst_coys c ON e.coy_id = c.coy_code ");
            sb.Append("LEFT JOIN gst_depts d ON e.dept_id = d.deptqk ");
            sb.Append("LEFT JOIN gst_units u ON e.unit_id = u.unitqk ");
            sb.Append("WHERE(e.loc_id = @loc_id) AND (e.is_dx = false) ");
            sb.Append("ORDER BY p.fullname;");
            query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var loc_id = cmd.Parameters.Add("@loc_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    loc_id.Value = locationId;

                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        employeeList.Add(new Employee()
                        {
                            EmployeeID = reader["emp_id"] == DBNull.Value ? string.Empty : (reader["emp_id"]).ToString(),
                            EmployeeNo1 = reader["emp_no_1"] == DBNull.Value ? string.Empty : (reader["emp_no_1"]).ToString(),
                            EmployeeNo2 = reader["emp_no_2"] == DBNull.Value ? string.Empty : (reader["emp_no_2"]).ToString(),
                            StartUpDate = reader["start_up_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["start_up_date"],
                            YearsOfExperience = reader["yrs_of_experience"] == DBNull.Value ? 0 : (int)reader["yrs_of_experience"],
                            StartUpDesignation = reader["start_up_designation"] == DBNull.Value ? String.Empty : reader["start_up_designation"].ToString(),
                            PlaceOfEngagement = reader["place_of_engagement"] == DBNull.Value ? String.Empty : reader["place_of_engagement"].ToString(),
                            ConfirmationDate = reader["confirmation_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["confirmation_date"],
                            CurrentDesignation = reader["current_designation"] == DBNull.Value ? String.Empty : reader["current_designation"].ToString(),
                            JobGrade = reader["job_grade"] == DBNull.Value ? String.Empty : reader["job_grade"].ToString(),
                            EmploymentStatus = reader["employment_status"] == DBNull.Value ? String.Empty : reader["employment_status"].ToString(),
                            DateOfLastPromotion = reader["date_of_last_promotion"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["date_of_last_promotion"],
                            LengthOfService = reader["start_up_date"] == DBNull.Value ? 0 : (int)((DateTime.Now - (DateTime)reader["start_up_date"]).TotalDays),

                            OfficialEmail = reader["official_email"] == DBNull.Value ? String.Empty : reader["official_email"].ToString(),
                            StateOfOrigin = reader["state_of_origin"] == DBNull.Value ? String.Empty : reader["state_of_origin"].ToString(),
                            LgaOfOrigin = reader["lga_of_origin"] == DBNull.Value ? String.Empty : reader["lga_of_origin"].ToString(),
                            Religion = reader["religion"] == DBNull.Value ? String.Empty : reader["religion"].ToString(),
                            GeoPoliticalRegion = reader["geo_political_region"] == DBNull.Value ? String.Empty : reader["geo_political_region"].ToString(),
                            NextOfKinName = reader["next_of_kin_name"] == DBNull.Value ? String.Empty : reader["next_of_kin_name"].ToString(),
                            NextOfKinRelationship = reader["next_of_kin_relationship"] == DBNull.Value ? String.Empty : reader["next_of_kin_relationship"].ToString(),
                            NextOfKinAddress = reader["next_of_kin_address"] == DBNull.Value ? String.Empty : reader["next_of_kin_address"].ToString(),
                            NextOfKinPhone = reader["next_of_kin_phone"] == DBNull.Value ? String.Empty : reader["next_of_kin_phone"].ToString(),
                            NextOfKinEmail = reader["next_of_kin_email"] == DBNull.Value ? String.Empty : reader["next_of_kin_email"].ToString(),
                            CompanyID = reader["coy_id"] == DBNull.Value ? string.Empty : (reader["coy_id"]).ToString(),
                            DepartmentID = reader["dept_id"] == DBNull.Value ? 0 : (int)(reader["dept_id"]),
                            UnitID = reader["unit_id"] == DBNull.Value ? 0 : (int)(reader["unit_id"]),
                            LocationID = reader["loc_id"] == DBNull.Value ? 0 : (int)(reader["loc_id"]),
                            EmployeeModifiedBy = reader["modified_by"] == DBNull.Value ? string.Empty : reader["modified_by"].ToString(),
                            EmployeeModifiedDate = reader["modified_date"] == DBNull.Value ? string.Empty : reader["modified_date"].ToString(),
                            EmployeeCreatedBy = reader["created_by"] == DBNull.Value ? string.Empty : reader["created_by"].ToString(),
                            EmployeeCreatedDate = reader["created_date"] == DBNull.Value ? string.Empty : reader["created_date"].ToString(),
                            IsDeactivated = reader["is_dx"] == DBNull.Value ? true : (bool)reader["is_dx"],
                            DeactivationTime = reader["dx_time"] == DBNull.Value ? string.Empty : reader["dx_time"].ToString(),
                            DeactivatedBy = reader["dx_by"] == DBNull.Value ? string.Empty : reader["dx_by"].ToString(),

                            PersonID = reader["id"] == DBNull.Value ? String.Empty : reader["id"].ToString(),
                            Title = reader["title"] == DBNull.Value ? string.Empty : reader["title"].ToString(),
                            Surname = reader["sname"] == DBNull.Value ? string.Empty : reader["sname"].ToString(),
                            FirstName = reader["fname"] == DBNull.Value ? string.Empty : reader["fname"].ToString(),
                            OtherNames = reader["oname"] == DBNull.Value ? string.Empty : reader["oname"].ToString(),
                            FullName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                            Sex = reader["sex"] == DBNull.Value ? string.Empty : reader["sex"].ToString(),
                            MaritalStatus = reader["maritalstatus"] == DBNull.Value ? String.Empty : reader["maritalstatus"].ToString(),
                            BirthDay = reader["birthday"] == DBNull.Value ? 0 : (int)reader["birthday"],
                            BirthMonth = reader["birthmonth"] == DBNull.Value ? 0 : (int)reader["birthmonth"],
                            BirthYear = reader["birthyear"] == DBNull.Value ? 0 : (int)reader["birthyear"],
                            PhoneNo1 = reader["phone1"] == DBNull.Value ? string.Empty : reader["phone1"].ToString(),
                            PhoneNo2 = reader["phone2"] == DBNull.Value ? string.Empty : reader["phone2"].ToString(),
                            Email = reader["personal_email"] == DBNull.Value ? string.Empty : reader["personal_email"].ToString(),
                            Address = reader["address"] == DBNull.Value ? string.Empty : reader["address"].ToString(),
                            ImagePath = reader["imgp"] == DBNull.Value ? string.Empty : reader["imgp"].ToString(),
                            ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                            ModifiedTime = reader["mdt"] == DBNull.Value ? string.Empty : reader["mdt"].ToString(),
                            CreatedTime = reader["ctt"] == DBNull.Value ? string.Empty : reader["ctt"].ToString(),
                            CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),

                            LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            LocationType = reader["loctype"] == DBNull.Value ? string.Empty : reader["loctype"].ToString(),
                            LocationHead1 = reader["lochq1"] == DBNull.Value ? string.Empty : reader["lochq1"].ToString(),
                            LocationHead2 = reader["lochq2"] == DBNull.Value ? string.Empty : reader["lochq2"].ToString(),
                            LocationCountry = reader["locctr"] == DBNull.Value ? string.Empty : reader["locctr"].ToString(),
                            LocationState = reader["locst"] == DBNull.Value ? string.Empty : reader["locst"].ToString(),
                            CompanyName = reader["coy_name"] == DBNull.Value ? string.Empty : reader["coy_name"].ToString(),
                            DepartmentHead1 = reader["depthd1"] == DBNull.Value ? string.Empty : reader["depthd1"].ToString(),
                            DepartmentHead2 = reader["depthd2"] == DBNull.Value ? string.Empty : reader["depthd2"].ToString(),
                            DepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                            UnitHead1 = reader["unithd1"] == DBNull.Value ? string.Empty : reader["unithd1"].ToString(),
                            UnitHead2 = reader["unithd2"] == DBNull.Value ? string.Empty : reader["unithd2"].ToString(),
                            UnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
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
            return employeeList;
        }

        public async Task<IList<Employee>> GetEmployeesByLocationAsync(int locationId, int deptId)
        {
            List<Employee> employeeList = new List<Employee>();
            if (locationId < 1) { throw new ArgumentNullException(nameof(locationId)); }
            if (deptId < 1) { throw new ArgumentNullException(nameof(deptId)); }
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT e.emp_id, e.emp_no_1, e.emp_no_2, e.start_up_date, ");
            sb.Append("e.yrs_of_experience, e.start_up_designation, e.place_of_engagement, ");
            sb.Append("e.confirmation_date, e.current_designation, e.job_grade, e.employment_status, ");
            sb.Append("e.date_of_last_promotion, e.official_email, e.state_of_origin, e.is_dx, ");
            sb.Append("e.lga_of_origin, e.religion, e.geo_political_region, e.next_of_kin_name, ");
            sb.Append("e.next_of_kin_relationship, e.modified_by, e.modified_date, e.dx_time, e.dx_by, ");
            sb.Append("e.created_by, e.created_date, e.next_of_kin_address, e.next_of_kin_phone, ");
            sb.Append("e.next_of_kin_email, e.dept_id, e.unit_id, e.loc_id, e.coy_id, p.id, p.title, ");
            sb.Append("p.sname, p.fname, p.oname, p.fullname, p.sex, p.phone1, p.phone2,");
            sb.Append("p.email AS personal_email, p.address, p.mdb, p.mdt, p.ctb, p.ctt, ");
            sb.Append("p.imgp, p.birthday, p.birthmonth, p.birthyear, p.maritalstatus, l.locname, ");
            sb.Append("l.loctype, l.lochq1, l.lochq2, l.locmb, l.locmd, l.loccb, l.loccd, l.locctr, ");
            sb.Append("l.locst, l.locqk, c.coy_code, c.coy_name, d.deptname, d.depthd1, d.depthd2, ");
            sb.Append("d.deptqk, u.unitname, u.unithd1, u.unithd2, u.unitqk ");
            sb.Append("FROM erm_emp_inf e JOIN gst_prsns p ON e.emp_id = p.id ");
            sb.Append("AND e.is_dx = false ");
            sb.Append("LEFT JOIN gst_locs l ON e.loc_id = l.locqk ");
            sb.Append("LEFT JOIN gst_coys c ON e.coy_id = c.coy_code ");
            sb.Append("LEFT JOIN gst_depts d ON e.dept_id = d.deptqk ");
            sb.Append("LEFT JOIN gst_units u ON e.unit_id = u.unitqk ");
            sb.Append("WHERE(e.loc_id = @loc_id) AND (e.dept_id = @dept_id) ");
            sb.Append("AND (e.is_dx = false) ORDER BY p.fullname;");
            query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var loc_id = cmd.Parameters.Add("@loc_id", NpgsqlDbType.Integer);
                    var dept_id = cmd.Parameters.Add("@dept_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    loc_id.Value = locationId;
                    dept_id.Value = deptId;

                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        employeeList.Add(new Employee()
                        {
                            EmployeeID = reader["emp_id"] == DBNull.Value ? string.Empty : (reader["emp_id"]).ToString(),
                            EmployeeNo1 = reader["emp_no_1"] == DBNull.Value ? string.Empty : (reader["emp_no_1"]).ToString(),
                            EmployeeNo2 = reader["emp_no_2"] == DBNull.Value ? string.Empty : (reader["emp_no_2"]).ToString(),
                            StartUpDate = reader["start_up_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["start_up_date"],
                            YearsOfExperience = reader["yrs_of_experience"] == DBNull.Value ? 0 : (int)reader["yrs_of_experience"],
                            StartUpDesignation = reader["start_up_designation"] == DBNull.Value ? String.Empty : reader["start_up_designation"].ToString(),
                            PlaceOfEngagement = reader["place_of_engagement"] == DBNull.Value ? String.Empty : reader["place_of_engagement"].ToString(),
                            ConfirmationDate = reader["confirmation_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["confirmation_date"],
                            CurrentDesignation = reader["current_designation"] == DBNull.Value ? String.Empty : reader["current_designation"].ToString(),
                            JobGrade = reader["job_grade"] == DBNull.Value ? String.Empty : reader["job_grade"].ToString(),
                            EmploymentStatus = reader["employment_status"] == DBNull.Value ? String.Empty : reader["employment_status"].ToString(),
                            DateOfLastPromotion = reader["date_of_last_promotion"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["date_of_last_promotion"],
                            LengthOfService = reader["start_up_date"] == DBNull.Value ? 0 : (int)((DateTime.Now - (DateTime)reader["start_up_date"]).TotalDays),

                            OfficialEmail = reader["official_email"] == DBNull.Value ? String.Empty : reader["official_email"].ToString(),
                            StateOfOrigin = reader["state_of_origin"] == DBNull.Value ? String.Empty : reader["state_of_origin"].ToString(),
                            LgaOfOrigin = reader["lga_of_origin"] == DBNull.Value ? String.Empty : reader["lga_of_origin"].ToString(),
                            Religion = reader["religion"] == DBNull.Value ? String.Empty : reader["religion"].ToString(),
                            GeoPoliticalRegion = reader["geo_political_region"] == DBNull.Value ? String.Empty : reader["geo_political_region"].ToString(),
                            NextOfKinName = reader["next_of_kin_name"] == DBNull.Value ? String.Empty : reader["next_of_kin_name"].ToString(),
                            NextOfKinRelationship = reader["next_of_kin_relationship"] == DBNull.Value ? String.Empty : reader["next_of_kin_relationship"].ToString(),
                            NextOfKinAddress = reader["next_of_kin_address"] == DBNull.Value ? String.Empty : reader["next_of_kin_address"].ToString(),
                            NextOfKinPhone = reader["next_of_kin_phone"] == DBNull.Value ? String.Empty : reader["next_of_kin_phone"].ToString(),
                            NextOfKinEmail = reader["next_of_kin_email"] == DBNull.Value ? String.Empty : reader["next_of_kin_email"].ToString(),
                            CompanyID = reader["coy_id"] == DBNull.Value ? string.Empty : (reader["coy_id"]).ToString(),
                            DepartmentID = reader["dept_id"] == DBNull.Value ? 0 : (int)(reader["dept_id"]),
                            UnitID = reader["unit_id"] == DBNull.Value ? 0 : (int)(reader["unit_id"]),
                            LocationID = reader["loc_id"] == DBNull.Value ? 0 : (int)(reader["loc_id"]),
                            EmployeeModifiedBy = reader["modified_by"] == DBNull.Value ? string.Empty : reader["modified_by"].ToString(),
                            EmployeeModifiedDate = reader["modified_date"] == DBNull.Value ? string.Empty : reader["modified_date"].ToString(),
                            EmployeeCreatedBy = reader["created_by"] == DBNull.Value ? string.Empty : reader["created_by"].ToString(),
                            EmployeeCreatedDate = reader["created_date"] == DBNull.Value ? string.Empty : reader["created_date"].ToString(),
                            IsDeactivated = reader["is_dx"] == DBNull.Value ? true : (bool)reader["is_dx"],
                            DeactivationTime = reader["dx_time"] == DBNull.Value ? string.Empty : reader["dx_time"].ToString(),
                            DeactivatedBy = reader["dx_by"] == DBNull.Value ? string.Empty : reader["dx_by"].ToString(),

                            PersonID = reader["id"] == DBNull.Value ? String.Empty : reader["id"].ToString(),
                            Title = reader["title"] == DBNull.Value ? string.Empty : reader["title"].ToString(),
                            Surname = reader["sname"] == DBNull.Value ? string.Empty : reader["sname"].ToString(),
                            FirstName = reader["fname"] == DBNull.Value ? string.Empty : reader["fname"].ToString(),
                            OtherNames = reader["oname"] == DBNull.Value ? string.Empty : reader["oname"].ToString(),
                            FullName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                            Sex = reader["sex"] == DBNull.Value ? string.Empty : reader["sex"].ToString(),
                            MaritalStatus = reader["maritalstatus"] == DBNull.Value ? String.Empty : reader["maritalstatus"].ToString(),
                            BirthDay = reader["birthday"] == DBNull.Value ? 0 : (int)reader["birthday"],
                            BirthMonth = reader["birthmonth"] == DBNull.Value ? 0 : (int)reader["birthmonth"],
                            BirthYear = reader["birthyear"] == DBNull.Value ? 0 : (int)reader["birthyear"],
                            PhoneNo1 = reader["phone1"] == DBNull.Value ? string.Empty : reader["phone1"].ToString(),
                            PhoneNo2 = reader["phone2"] == DBNull.Value ? string.Empty : reader["phone2"].ToString(),
                            Email = reader["personal_email"] == DBNull.Value ? string.Empty : reader["personal_email"].ToString(),
                            Address = reader["address"] == DBNull.Value ? string.Empty : reader["address"].ToString(),
                            ImagePath = reader["imgp"] == DBNull.Value ? string.Empty : reader["imgp"].ToString(),
                            ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                            ModifiedTime = reader["mdt"] == DBNull.Value ? string.Empty : reader["mdt"].ToString(),
                            CreatedTime = reader["ctt"] == DBNull.Value ? string.Empty : reader["ctt"].ToString(),
                            CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),

                            LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            LocationType = reader["loctype"] == DBNull.Value ? string.Empty : reader["loctype"].ToString(),
                            LocationHead1 = reader["lochq1"] == DBNull.Value ? string.Empty : reader["lochq1"].ToString(),
                            LocationHead2 = reader["lochq2"] == DBNull.Value ? string.Empty : reader["lochq2"].ToString(),
                            LocationCountry = reader["locctr"] == DBNull.Value ? string.Empty : reader["locctr"].ToString(),
                            LocationState = reader["locst"] == DBNull.Value ? string.Empty : reader["locst"].ToString(),
                            CompanyName = reader["coy_name"] == DBNull.Value ? string.Empty : reader["coy_name"].ToString(),
                            DepartmentHead1 = reader["depthd1"] == DBNull.Value ? string.Empty : reader["depthd1"].ToString(),
                            DepartmentHead2 = reader["depthd2"] == DBNull.Value ? string.Empty : reader["depthd2"].ToString(),
                            DepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                            UnitHead1 = reader["unithd1"] == DBNull.Value ? string.Empty : reader["unithd1"].ToString(),
                            UnitHead2 = reader["unithd2"] == DBNull.Value ? string.Empty : reader["unithd2"].ToString(),
                            UnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
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
            return employeeList;
        }

        public async Task<IList<Employee>> GetEmployeesByLocationAsync(int locationId, int deptId, int unitId)
        {
            List<Employee> employeeList = new List<Employee>();
            if (locationId < 1) { throw new ArgumentNullException(nameof(locationId)); }
            if (deptId < 1) { throw new ArgumentNullException(nameof(deptId)); }
            if (unitId < 1) { throw new ArgumentNullException(nameof(unitId)); }
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT e.emp_id, e.emp_no_1, e.emp_no_2, e.start_up_date, ");
            sb.Append("e.yrs_of_experience, e.start_up_designation, e.place_of_engagement, ");
            sb.Append("e.confirmation_date, e.current_designation, e.job_grade, e.employment_status, ");
            sb.Append("e.date_of_last_promotion, e.official_email, e.state_of_origin, e.is_dx, ");
            sb.Append("e.lga_of_origin, e.religion, e.geo_political_region, e.next_of_kin_name, ");
            sb.Append("e.next_of_kin_relationship, e.modified_by, e.modified_date, e.dx_time, e.dx_by, ");
            sb.Append("e.created_by, e.created_date, e.next_of_kin_address, e.next_of_kin_phone, ");
            sb.Append("e.next_of_kin_email, e.dept_id, e.unit_id, e.loc_id, e.coy_id, p.id, p.title, ");
            sb.Append("p.sname, p.fname, p.oname, p.fullname, p.sex, p.phone1, p.phone2,");
            sb.Append("p.email AS personal_email, p.address, p.mdb, p.mdt, p.ctb, p.ctt, ");
            sb.Append("p.imgp, p.birthday, p.birthmonth, p.birthyear, p.maritalstatus, l.locname, ");
            sb.Append("l.loctype, l.lochq1, l.lochq2, l.locmb, l.locmd, l.loccb, l.loccd, l.locctr, ");
            sb.Append("l.locst, l.locqk, c.coy_code, c.coy_name, d.deptname, d.depthd1, d.depthd2, ");
            sb.Append("d.deptqk, u.unitname, u.unithd1, u.unithd2, u.unitqk ");
            sb.Append("FROM erm_emp_inf e JOIN gst_prsns p ON e.emp_id = p.id ");
            sb.Append("AND e.is_dx = false ");
            sb.Append("LEFT JOIN gst_locs l ON e.loc_id = l.locqk ");
            sb.Append("LEFT JOIN gst_coys c ON e.coy_id = c.coy_code ");
            sb.Append("LEFT JOIN gst_depts d ON e.dept_id = d.deptqk ");
            sb.Append("LEFT JOIN gst_units u ON e.unit_id = u.unitqk ");
            sb.Append("WHERE(e.loc_id = @loc_id) AND (e.dept_id = @dept_id) ");
            sb.Append("AND (e.unit_id = @unit_id) AND (e.is_dx = false) ");
            sb.Append("ORDER BY p.fullname;");
            query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var loc_id = cmd.Parameters.Add("@loc_id", NpgsqlDbType.Integer);
                    var dept_id = cmd.Parameters.Add("@dept_id", NpgsqlDbType.Integer);
                    var unit_id = cmd.Parameters.Add("@unit_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    loc_id.Value = locationId;
                    dept_id.Value = deptId;
                    unit_id.Value = unitId;

                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        employeeList.Add(new Employee()
                        {
                            EmployeeID = reader["emp_id"] == DBNull.Value ? string.Empty : (reader["emp_id"]).ToString(),
                            EmployeeNo1 = reader["emp_no_1"] == DBNull.Value ? string.Empty : (reader["emp_no_1"]).ToString(),
                            EmployeeNo2 = reader["emp_no_2"] == DBNull.Value ? string.Empty : (reader["emp_no_2"]).ToString(),
                            StartUpDate = reader["start_up_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["start_up_date"],
                            YearsOfExperience = reader["yrs_of_experience"] == DBNull.Value ? 0 : (int)reader["yrs_of_experience"],
                            StartUpDesignation = reader["start_up_designation"] == DBNull.Value ? String.Empty : reader["start_up_designation"].ToString(),
                            PlaceOfEngagement = reader["place_of_engagement"] == DBNull.Value ? String.Empty : reader["place_of_engagement"].ToString(),
                            ConfirmationDate = reader["confirmation_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["confirmation_date"],
                            CurrentDesignation = reader["current_designation"] == DBNull.Value ? String.Empty : reader["current_designation"].ToString(),
                            JobGrade = reader["job_grade"] == DBNull.Value ? String.Empty : reader["job_grade"].ToString(),
                            EmploymentStatus = reader["employment_status"] == DBNull.Value ? String.Empty : reader["employment_status"].ToString(),
                            DateOfLastPromotion = reader["date_of_last_promotion"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["date_of_last_promotion"],
                            LengthOfService = reader["start_up_date"] == DBNull.Value ? 0 : (int)((DateTime.Now - (DateTime)reader["start_up_date"]).TotalDays),

                            OfficialEmail = reader["official_email"] == DBNull.Value ? String.Empty : reader["official_email"].ToString(),
                            StateOfOrigin = reader["state_of_origin"] == DBNull.Value ? String.Empty : reader["state_of_origin"].ToString(),
                            LgaOfOrigin = reader["lga_of_origin"] == DBNull.Value ? String.Empty : reader["lga_of_origin"].ToString(),
                            Religion = reader["religion"] == DBNull.Value ? String.Empty : reader["religion"].ToString(),
                            GeoPoliticalRegion = reader["geo_political_region"] == DBNull.Value ? String.Empty : reader["geo_political_region"].ToString(),
                            NextOfKinName = reader["next_of_kin_name"] == DBNull.Value ? String.Empty : reader["next_of_kin_name"].ToString(),
                            NextOfKinRelationship = reader["next_of_kin_relationship"] == DBNull.Value ? String.Empty : reader["next_of_kin_relationship"].ToString(),
                            NextOfKinAddress = reader["next_of_kin_address"] == DBNull.Value ? String.Empty : reader["next_of_kin_address"].ToString(),
                            NextOfKinPhone = reader["next_of_kin_phone"] == DBNull.Value ? String.Empty : reader["next_of_kin_phone"].ToString(),
                            NextOfKinEmail = reader["next_of_kin_email"] == DBNull.Value ? String.Empty : reader["next_of_kin_email"].ToString(),
                            CompanyID = reader["coy_id"] == DBNull.Value ? string.Empty : (reader["coy_id"]).ToString(),
                            DepartmentID = reader["dept_id"] == DBNull.Value ? 0 : (int)(reader["dept_id"]),
                            UnitID = reader["unit_id"] == DBNull.Value ? 0 : (int)(reader["unit_id"]),
                            LocationID = reader["loc_id"] == DBNull.Value ? 0 : (int)(reader["loc_id"]),
                            EmployeeModifiedBy = reader["modified_by"] == DBNull.Value ? string.Empty : reader["modified_by"].ToString(),
                            EmployeeModifiedDate = reader["modified_date"] == DBNull.Value ? string.Empty : reader["modified_date"].ToString(),
                            EmployeeCreatedBy = reader["created_by"] == DBNull.Value ? string.Empty : reader["created_by"].ToString(),
                            EmployeeCreatedDate = reader["created_date"] == DBNull.Value ? string.Empty : reader["created_date"].ToString(),
                            IsDeactivated = reader["is_dx"] == DBNull.Value ? true : (bool)reader["is_dx"],
                            DeactivationTime = reader["dx_time"] == DBNull.Value ? string.Empty : reader["dx_time"].ToString(),
                            DeactivatedBy = reader["dx_by"] == DBNull.Value ? string.Empty : reader["dx_by"].ToString(),

                            PersonID = reader["id"] == DBNull.Value ? String.Empty : reader["id"].ToString(),
                            Title = reader["title"] == DBNull.Value ? string.Empty : reader["title"].ToString(),
                            Surname = reader["sname"] == DBNull.Value ? string.Empty : reader["sname"].ToString(),
                            FirstName = reader["fname"] == DBNull.Value ? string.Empty : reader["fname"].ToString(),
                            OtherNames = reader["oname"] == DBNull.Value ? string.Empty : reader["oname"].ToString(),
                            FullName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                            Sex = reader["sex"] == DBNull.Value ? string.Empty : reader["sex"].ToString(),
                            MaritalStatus = reader["maritalstatus"] == DBNull.Value ? String.Empty : reader["maritalstatus"].ToString(),
                            BirthDay = reader["birthday"] == DBNull.Value ? 0 : (int)reader["birthday"],
                            BirthMonth = reader["birthmonth"] == DBNull.Value ? 0 : (int)reader["birthmonth"],
                            BirthYear = reader["birthyear"] == DBNull.Value ? 0 : (int)reader["birthyear"],
                            PhoneNo1 = reader["phone1"] == DBNull.Value ? string.Empty : reader["phone1"].ToString(),
                            PhoneNo2 = reader["phone2"] == DBNull.Value ? string.Empty : reader["phone2"].ToString(),
                            Email = reader["personal_email"] == DBNull.Value ? string.Empty : reader["personal_email"].ToString(),
                            Address = reader["address"] == DBNull.Value ? string.Empty : reader["address"].ToString(),
                            ImagePath = reader["imgp"] == DBNull.Value ? string.Empty : reader["imgp"].ToString(),
                            ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                            ModifiedTime = reader["mdt"] == DBNull.Value ? string.Empty : reader["mdt"].ToString(),
                            CreatedTime = reader["ctt"] == DBNull.Value ? string.Empty : reader["ctt"].ToString(),
                            CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),

                            LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            LocationType = reader["loctype"] == DBNull.Value ? string.Empty : reader["loctype"].ToString(),
                            LocationHead1 = reader["lochq1"] == DBNull.Value ? string.Empty : reader["lochq1"].ToString(),
                            LocationHead2 = reader["lochq2"] == DBNull.Value ? string.Empty : reader["lochq2"].ToString(),
                            LocationCountry = reader["locctr"] == DBNull.Value ? string.Empty : reader["locctr"].ToString(),
                            LocationState = reader["locst"] == DBNull.Value ? string.Empty : reader["locst"].ToString(),
                            CompanyName = reader["coy_name"] == DBNull.Value ? string.Empty : reader["coy_name"].ToString(),
                            DepartmentHead1 = reader["depthd1"] == DBNull.Value ? string.Empty : reader["depthd1"].ToString(),
                            DepartmentHead2 = reader["depthd2"] == DBNull.Value ? string.Empty : reader["depthd2"].ToString(),
                            DepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                            UnitHead1 = reader["unithd1"] == DBNull.Value ? string.Empty : reader["unithd1"].ToString(),
                            UnitHead2 = reader["unithd2"] == DBNull.Value ? string.Empty : reader["unithd2"].ToString(),
                            UnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
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
            return employeeList;
        }

        public async Task<IList<Employee>> GetEmployeesByLocationAndUnitAsync(int locationId, int unitId)
        {
            List<Employee> employeeList = new List<Employee>();
            if (locationId < 1) { throw new ArgumentNullException(nameof(locationId)); }
            if (unitId < 1) { throw new ArgumentNullException(nameof(unitId)); }
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT e.emp_id, e.emp_no_1, e.emp_no_2, e.start_up_date, ");
            sb.Append("e.yrs_of_experience, e.start_up_designation, e.place_of_engagement, ");
            sb.Append("e.confirmation_date, e.current_designation, e.job_grade, e.employment_status, ");
            sb.Append("e.date_of_last_promotion, e.official_email, e.state_of_origin, e.is_dx, ");
            sb.Append("e.lga_of_origin, e.religion, e.geo_political_region, e.next_of_kin_name, ");
            sb.Append("e.next_of_kin_relationship, e.modified_by, e.modified_date, e.dx_time, e.dx_by, ");
            sb.Append("e.created_by, e.created_date, e.next_of_kin_address, e.next_of_kin_phone, ");
            sb.Append("e.next_of_kin_email, e.dept_id, e.unit_id, e.loc_id, e.coy_id, p.id, p.title, ");
            sb.Append("p.sname, p.fname, p.oname, p.fullname, p.sex, p.phone1, p.phone2,");
            sb.Append("p.email AS personal_email, p.address, p.mdb, p.mdt, p.ctb, p.ctt, ");
            sb.Append("p.imgp, p.birthday, p.birthmonth, p.birthyear, p.maritalstatus, l.locname, ");
            sb.Append("l.loctype, l.lochq1, l.lochq2, l.locmb, l.locmd, l.loccb, l.loccd, l.locctr, ");
            sb.Append("l.locst, l.locqk, c.coy_code, c.coy_name, d.deptname, d.depthd1, d.depthd2, ");
            sb.Append("d.deptqk, u.unitname, u.unithd1, u.unithd2, u.unitqk ");
            sb.Append("FROM erm_emp_inf e JOIN gst_prsns p ON e.emp_id = p.id ");
            sb.Append("AND e.is_dx = false ");
            sb.Append("LEFT JOIN gst_locs l ON e.loc_id = l.locqk ");
            sb.Append("LEFT JOIN gst_coys c ON e.coy_id = c.coy_code ");
            sb.Append("LEFT JOIN gst_depts d ON e.dept_id = d.deptqk ");
            sb.Append("LEFT JOIN gst_units u ON e.unit_id = u.unitqk ");
            sb.Append("WHERE(e.loc_id = @loc_id) AND (e.unit_id = @unit_id) ");
            sb.Append("AND (e.is_dx = false) ORDER BY p.fullname;");
            query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var loc_id = cmd.Parameters.Add("@loc_id", NpgsqlDbType.Integer);
                    var unit_id = cmd.Parameters.Add("@unit_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    loc_id.Value = locationId;
                    unit_id.Value = unitId;

                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        employeeList.Add(new Employee()
                        {
                            EmployeeID = reader["emp_id"] == DBNull.Value ? string.Empty : (reader["emp_id"]).ToString(),
                            EmployeeNo1 = reader["emp_no_1"] == DBNull.Value ? string.Empty : (reader["emp_no_1"]).ToString(),
                            EmployeeNo2 = reader["emp_no_2"] == DBNull.Value ? string.Empty : (reader["emp_no_2"]).ToString(),
                            StartUpDate = reader["start_up_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["start_up_date"],
                            YearsOfExperience = reader["yrs_of_experience"] == DBNull.Value ? 0 : (int)reader["yrs_of_experience"],
                            StartUpDesignation = reader["start_up_designation"] == DBNull.Value ? String.Empty : reader["start_up_designation"].ToString(),
                            PlaceOfEngagement = reader["place_of_engagement"] == DBNull.Value ? String.Empty : reader["place_of_engagement"].ToString(),
                            ConfirmationDate = reader["confirmation_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["confirmation_date"],
                            CurrentDesignation = reader["current_designation"] == DBNull.Value ? String.Empty : reader["current_designation"].ToString(),
                            JobGrade = reader["job_grade"] == DBNull.Value ? String.Empty : reader["job_grade"].ToString(),
                            EmploymentStatus = reader["employment_status"] == DBNull.Value ? String.Empty : reader["employment_status"].ToString(),
                            DateOfLastPromotion = reader["date_of_last_promotion"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["date_of_last_promotion"],
                            LengthOfService = reader["start_up_date"] == DBNull.Value ? 0 : (int)((DateTime.Now - (DateTime)reader["start_up_date"]).TotalDays),

                            OfficialEmail = reader["official_email"] == DBNull.Value ? String.Empty : reader["official_email"].ToString(),
                            StateOfOrigin = reader["state_of_origin"] == DBNull.Value ? String.Empty : reader["state_of_origin"].ToString(),
                            LgaOfOrigin = reader["lga_of_origin"] == DBNull.Value ? String.Empty : reader["lga_of_origin"].ToString(),
                            Religion = reader["religion"] == DBNull.Value ? String.Empty : reader["religion"].ToString(),
                            GeoPoliticalRegion = reader["geo_political_region"] == DBNull.Value ? String.Empty : reader["geo_political_region"].ToString(),
                            NextOfKinName = reader["next_of_kin_name"] == DBNull.Value ? String.Empty : reader["next_of_kin_name"].ToString(),
                            NextOfKinRelationship = reader["next_of_kin_relationship"] == DBNull.Value ? String.Empty : reader["next_of_kin_relationship"].ToString(),
                            NextOfKinAddress = reader["next_of_kin_address"] == DBNull.Value ? String.Empty : reader["next_of_kin_address"].ToString(),
                            NextOfKinPhone = reader["next_of_kin_phone"] == DBNull.Value ? String.Empty : reader["next_of_kin_phone"].ToString(),
                            NextOfKinEmail = reader["next_of_kin_email"] == DBNull.Value ? String.Empty : reader["next_of_kin_email"].ToString(),
                            CompanyID = reader["coy_id"] == DBNull.Value ? string.Empty : (reader["coy_id"]).ToString(),
                            DepartmentID = reader["dept_id"] == DBNull.Value ? 0 : (int)(reader["dept_id"]),
                            UnitID = reader["unit_id"] == DBNull.Value ? 0 : (int)(reader["unit_id"]),
                            LocationID = reader["loc_id"] == DBNull.Value ? 0 : (int)(reader["loc_id"]),
                            EmployeeModifiedBy = reader["modified_by"] == DBNull.Value ? string.Empty : reader["modified_by"].ToString(),
                            EmployeeModifiedDate = reader["modified_date"] == DBNull.Value ? string.Empty : reader["modified_date"].ToString(),
                            EmployeeCreatedBy = reader["created_by"] == DBNull.Value ? string.Empty : reader["created_by"].ToString(),
                            EmployeeCreatedDate = reader["created_date"] == DBNull.Value ? string.Empty : reader["created_date"].ToString(),
                            IsDeactivated = reader["is_dx"] == DBNull.Value ? true : (bool)reader["is_dx"],
                            DeactivationTime = reader["dx_time"] == DBNull.Value ? string.Empty : reader["dx_time"].ToString(),
                            DeactivatedBy = reader["dx_by"] == DBNull.Value ? string.Empty : reader["dx_by"].ToString(),

                            PersonID = reader["id"] == DBNull.Value ? String.Empty : reader["id"].ToString(),
                            Title = reader["title"] == DBNull.Value ? string.Empty : reader["title"].ToString(),
                            Surname = reader["sname"] == DBNull.Value ? string.Empty : reader["sname"].ToString(),
                            FirstName = reader["fname"] == DBNull.Value ? string.Empty : reader["fname"].ToString(),
                            OtherNames = reader["oname"] == DBNull.Value ? string.Empty : reader["oname"].ToString(),
                            FullName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                            Sex = reader["sex"] == DBNull.Value ? string.Empty : reader["sex"].ToString(),
                            MaritalStatus = reader["maritalstatus"] == DBNull.Value ? String.Empty : reader["maritalstatus"].ToString(),
                            BirthDay = reader["birthday"] == DBNull.Value ? 0 : (int)reader["birthday"],
                            BirthMonth = reader["birthmonth"] == DBNull.Value ? 0 : (int)reader["birthmonth"],
                            BirthYear = reader["birthyear"] == DBNull.Value ? 0 : (int)reader["birthyear"],
                            PhoneNo1 = reader["phone1"] == DBNull.Value ? string.Empty : reader["phone1"].ToString(),
                            PhoneNo2 = reader["phone2"] == DBNull.Value ? string.Empty : reader["phone2"].ToString(),
                            Email = reader["personal_email"] == DBNull.Value ? string.Empty : reader["personal_email"].ToString(),
                            Address = reader["address"] == DBNull.Value ? string.Empty : reader["address"].ToString(),
                            ImagePath = reader["imgp"] == DBNull.Value ? string.Empty : reader["imgp"].ToString(),
                            ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                            ModifiedTime = reader["mdt"] == DBNull.Value ? string.Empty : reader["mdt"].ToString(),
                            CreatedTime = reader["ctt"] == DBNull.Value ? string.Empty : reader["ctt"].ToString(),
                            CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),

                            LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            LocationType = reader["loctype"] == DBNull.Value ? string.Empty : reader["loctype"].ToString(),
                            LocationHead1 = reader["lochq1"] == DBNull.Value ? string.Empty : reader["lochq1"].ToString(),
                            LocationHead2 = reader["lochq2"] == DBNull.Value ? string.Empty : reader["lochq2"].ToString(),
                            LocationCountry = reader["locctr"] == DBNull.Value ? string.Empty : reader["locctr"].ToString(),
                            LocationState = reader["locst"] == DBNull.Value ? string.Empty : reader["locst"].ToString(),
                            CompanyName = reader["coy_name"] == DBNull.Value ? string.Empty : reader["coy_name"].ToString(),
                            DepartmentHead1 = reader["depthd1"] == DBNull.Value ? string.Empty : reader["depthd1"].ToString(),
                            DepartmentHead2 = reader["depthd2"] == DBNull.Value ? string.Empty : reader["depthd2"].ToString(),
                            DepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                            UnitHead1 = reader["unithd1"] == DBNull.Value ? string.Empty : reader["unithd1"].ToString(),
                            UnitHead2 = reader["unithd2"] == DBNull.Value ? string.Empty : reader["unithd2"].ToString(),
                            UnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
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
            return employeeList;
        }

        public async Task<IList<Employee>> GetEmployeesByUnitAsync(int unitId)
        {
            List<Employee> employeeList = new List<Employee>();
            if (unitId < 1) { throw new ArgumentNullException(nameof(unitId)); }
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT e.emp_id, e.emp_no_1, e.emp_no_2, e.start_up_date, ");
            sb.Append("e.yrs_of_experience, e.start_up_designation, e.place_of_engagement, ");
            sb.Append("e.confirmation_date, e.current_designation, e.job_grade, e.employment_status, ");
            sb.Append("e.date_of_last_promotion, e.official_email, e.state_of_origin, e.is_dx, ");
            sb.Append("e.lga_of_origin, e.religion, e.geo_political_region, e.next_of_kin_name, ");
            sb.Append("e.next_of_kin_relationship, e.modified_by, e.modified_date, e.dx_time, e.dx_by, ");
            sb.Append("e.created_by, e.created_date, e.next_of_kin_address, e.next_of_kin_phone, ");
            sb.Append("e.next_of_kin_email, e.dept_id, e.unit_id, e.loc_id, e.coy_id, p.id, p.title, ");
            sb.Append("p.sname, p.fname, p.oname, p.fullname, p.sex, p.phone1, p.phone2,");
            sb.Append("p.email AS personal_email, p.address, p.mdb, p.mdt, p.ctb, p.ctt, ");
            sb.Append("p.imgp, p.birthday, p.birthmonth, p.birthyear, p.maritalstatus, l.locname, ");
            sb.Append("l.loctype, l.lochq1, l.lochq2, l.locmb, l.locmd, l.loccb, l.loccd, l.locctr, ");
            sb.Append("l.locst, l.locqk, c.coy_code, c.coy_name, d.deptname, d.depthd1, d.depthd2, ");
            sb.Append("d.deptqk, u.unitname, u.unithd1, u.unithd2, u.unitqk ");
            sb.Append("FROM erm_emp_inf e JOIN gst_prsns p ON e.emp_id = p.id ");
            sb.Append("AND e.is_dx = false ");
            sb.Append("LEFT JOIN gst_locs l ON e.loc_id = l.locqk ");
            sb.Append("LEFT JOIN gst_coys c ON e.coy_id = c.coy_code ");
            sb.Append("LEFT JOIN gst_depts d ON e.dept_id = d.deptqk ");
            sb.Append("LEFT JOIN gst_units u ON e.unit_id = u.unitqk ");
            sb.Append("WHERE (e.unit_id = @unit_id) ");
            sb.Append("AND (e.is_dx = false) ORDER BY p.fullname;");
            query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var unit_id = cmd.Parameters.Add("@unit_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    unit_id.Value = unitId;

                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        employeeList.Add(new Employee()
                        {
                            EmployeeID = reader["emp_id"] == DBNull.Value ? string.Empty : (reader["emp_id"]).ToString(),
                            EmployeeNo1 = reader["emp_no_1"] == DBNull.Value ? string.Empty : (reader["emp_no_1"]).ToString(),
                            EmployeeNo2 = reader["emp_no_2"] == DBNull.Value ? string.Empty : (reader["emp_no_2"]).ToString(),
                            StartUpDate = reader["start_up_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["start_up_date"],
                            YearsOfExperience = reader["yrs_of_experience"] == DBNull.Value ? 0 : (int)reader["yrs_of_experience"],
                            StartUpDesignation = reader["start_up_designation"] == DBNull.Value ? String.Empty : reader["start_up_designation"].ToString(),
                            PlaceOfEngagement = reader["place_of_engagement"] == DBNull.Value ? String.Empty : reader["place_of_engagement"].ToString(),
                            ConfirmationDate = reader["confirmation_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["confirmation_date"],
                            CurrentDesignation = reader["current_designation"] == DBNull.Value ? String.Empty : reader["current_designation"].ToString(),
                            JobGrade = reader["job_grade"] == DBNull.Value ? String.Empty : reader["job_grade"].ToString(),
                            EmploymentStatus = reader["employment_status"] == DBNull.Value ? String.Empty : reader["employment_status"].ToString(),
                            DateOfLastPromotion = reader["date_of_last_promotion"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["date_of_last_promotion"],
                            LengthOfService = reader["start_up_date"] == DBNull.Value ? 0 : (int)((DateTime.Now - (DateTime)reader["start_up_date"]).TotalDays),

                            OfficialEmail = reader["official_email"] == DBNull.Value ? String.Empty : reader["official_email"].ToString(),
                            StateOfOrigin = reader["state_of_origin"] == DBNull.Value ? String.Empty : reader["state_of_origin"].ToString(),
                            LgaOfOrigin = reader["lga_of_origin"] == DBNull.Value ? String.Empty : reader["lga_of_origin"].ToString(),
                            Religion = reader["religion"] == DBNull.Value ? String.Empty : reader["religion"].ToString(),
                            GeoPoliticalRegion = reader["geo_political_region"] == DBNull.Value ? String.Empty : reader["geo_political_region"].ToString(),
                            NextOfKinName = reader["next_of_kin_name"] == DBNull.Value ? String.Empty : reader["next_of_kin_name"].ToString(),
                            NextOfKinRelationship = reader["next_of_kin_relationship"] == DBNull.Value ? String.Empty : reader["next_of_kin_relationship"].ToString(),
                            NextOfKinAddress = reader["next_of_kin_address"] == DBNull.Value ? String.Empty : reader["next_of_kin_address"].ToString(),
                            NextOfKinPhone = reader["next_of_kin_phone"] == DBNull.Value ? String.Empty : reader["next_of_kin_phone"].ToString(),
                            NextOfKinEmail = reader["next_of_kin_email"] == DBNull.Value ? String.Empty : reader["next_of_kin_email"].ToString(),
                            CompanyID = reader["coy_id"] == DBNull.Value ? string.Empty : (reader["coy_id"]).ToString(),
                            DepartmentID = reader["dept_id"] == DBNull.Value ? 0 : (int)(reader["dept_id"]),
                            UnitID = reader["unit_id"] == DBNull.Value ? 0 : (int)(reader["unit_id"]),
                            LocationID = reader["loc_id"] == DBNull.Value ? 0 : (int)(reader["loc_id"]),
                            EmployeeModifiedBy = reader["modified_by"] == DBNull.Value ? string.Empty : reader["modified_by"].ToString(),
                            EmployeeModifiedDate = reader["modified_date"] == DBNull.Value ? string.Empty : reader["modified_date"].ToString(),
                            EmployeeCreatedBy = reader["created_by"] == DBNull.Value ? string.Empty : reader["created_by"].ToString(),
                            EmployeeCreatedDate = reader["created_date"] == DBNull.Value ? string.Empty : reader["created_date"].ToString(),
                            IsDeactivated = reader["is_dx"] == DBNull.Value ? true : (bool)reader["is_dx"],
                            DeactivationTime = reader["dx_time"] == DBNull.Value ? string.Empty : reader["dx_time"].ToString(),
                            DeactivatedBy = reader["dx_by"] == DBNull.Value ? string.Empty : reader["dx_by"].ToString(),

                            PersonID = reader["id"] == DBNull.Value ? String.Empty : reader["id"].ToString(),
                            Title = reader["title"] == DBNull.Value ? string.Empty : reader["title"].ToString(),
                            Surname = reader["sname"] == DBNull.Value ? string.Empty : reader["sname"].ToString(),
                            FirstName = reader["fname"] == DBNull.Value ? string.Empty : reader["fname"].ToString(),
                            OtherNames = reader["oname"] == DBNull.Value ? string.Empty : reader["oname"].ToString(),
                            FullName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                            Sex = reader["sex"] == DBNull.Value ? string.Empty : reader["sex"].ToString(),
                            MaritalStatus = reader["maritalstatus"] == DBNull.Value ? String.Empty : reader["maritalstatus"].ToString(),
                            BirthDay = reader["birthday"] == DBNull.Value ? 0 : (int)reader["birthday"],
                            BirthMonth = reader["birthmonth"] == DBNull.Value ? 0 : (int)reader["birthmonth"],
                            BirthYear = reader["birthyear"] == DBNull.Value ? 0 : (int)reader["birthyear"],
                            PhoneNo1 = reader["phone1"] == DBNull.Value ? string.Empty : reader["phone1"].ToString(),
                            PhoneNo2 = reader["phone2"] == DBNull.Value ? string.Empty : reader["phone2"].ToString(),
                            Email = reader["personal_email"] == DBNull.Value ? string.Empty : reader["personal_email"].ToString(),
                            Address = reader["address"] == DBNull.Value ? string.Empty : reader["address"].ToString(),
                            ImagePath = reader["imgp"] == DBNull.Value ? string.Empty : reader["imgp"].ToString(),
                            ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                            ModifiedTime = reader["mdt"] == DBNull.Value ? string.Empty : reader["mdt"].ToString(),
                            CreatedTime = reader["ctt"] == DBNull.Value ? string.Empty : reader["ctt"].ToString(),
                            CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),

                            LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            LocationType = reader["loctype"] == DBNull.Value ? string.Empty : reader["loctype"].ToString(),
                            LocationHead1 = reader["lochq1"] == DBNull.Value ? string.Empty : reader["lochq1"].ToString(),
                            LocationHead2 = reader["lochq2"] == DBNull.Value ? string.Empty : reader["lochq2"].ToString(),
                            LocationCountry = reader["locctr"] == DBNull.Value ? string.Empty : reader["locctr"].ToString(),
                            LocationState = reader["locst"] == DBNull.Value ? string.Empty : reader["locst"].ToString(),
                            CompanyName = reader["coy_name"] == DBNull.Value ? string.Empty : reader["coy_name"].ToString(),
                            DepartmentHead1 = reader["depthd1"] == DBNull.Value ? string.Empty : reader["depthd1"].ToString(),
                            DepartmentHead2 = reader["depthd2"] == DBNull.Value ? string.Empty : reader["depthd2"].ToString(),
                            DepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                            UnitHead1 = reader["unithd1"] == DBNull.Value ? string.Empty : reader["unithd1"].ToString(),
                            UnitHead2 = reader["unithd2"] == DBNull.Value ? string.Empty : reader["unithd2"].ToString(),
                            UnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
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
            return employeeList;
        }

        public async Task<IList<Employee>> GetEmployeesByDeptAsync(int deptId)
        {
            List<Employee> employeeList = new List<Employee>();
            if (deptId < 1) { throw new ArgumentNullException(nameof(deptId)); }
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT e.emp_id, e.emp_no_1, e.emp_no_2, e.start_up_date, ");
            sb.Append("e.yrs_of_experience, e.start_up_designation, e.place_of_engagement, ");
            sb.Append("e.confirmation_date, e.current_designation, e.job_grade, e.employment_status, ");
            sb.Append("e.date_of_last_promotion, e.official_email, e.state_of_origin, e.is_dx, ");
            sb.Append("e.lga_of_origin, e.religion, e.geo_political_region, e.next_of_kin_name, ");
            sb.Append("e.next_of_kin_relationship, e.modified_by, e.modified_date, e.dx_time, e.dx_by, ");
            sb.Append("e.created_by, e.created_date, e.next_of_kin_address, e.next_of_kin_phone, ");
            sb.Append("e.next_of_kin_email, e.dept_id, e.unit_id, e.loc_id, e.coy_id, p.id, p.title, ");
            sb.Append("p.sname, p.fname, p.oname, p.fullname, p.sex, p.phone1, p.phone2,");
            sb.Append("p.email AS personal_email, p.address, p.mdb, p.mdt, p.ctb, p.ctt, ");
            sb.Append("p.imgp, p.birthday, p.birthmonth, p.birthyear, p.maritalstatus, l.locname, ");
            sb.Append("l.loctype, l.lochq1, l.lochq2, l.locmb, l.locmd, l.loccb, l.loccd, l.locctr, ");
            sb.Append("l.locst, l.locqk, c.coy_code, c.coy_name, d.deptname, d.depthd1, d.depthd2, ");
            sb.Append("d.deptqk, u.unitname, u.unithd1, u.unithd2, u.unitqk ");
            sb.Append("FROM erm_emp_inf e JOIN gst_prsns p ON e.emp_id = p.id ");
            sb.Append("AND e.is_dx = false ");
            sb.Append("LEFT JOIN gst_locs l ON e.loc_id = l.locqk ");
            sb.Append("LEFT JOIN gst_coys c ON e.coy_id = c.coy_code ");
            sb.Append("LEFT JOIN gst_depts d ON e.dept_id = d.deptqk ");
            sb.Append("LEFT JOIN gst_units u ON e.unit_id = u.unitqk ");
            sb.Append("WHERE (e.dept_id = @dept_id) ");
            sb.Append("AND (e.is_dx = false) ORDER BY p.fullname;");
            query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var dept_id = cmd.Parameters.Add("@dept_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    dept_id.Value = deptId;

                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        employeeList.Add(new Employee()
                        {
                            EmployeeID = reader["emp_id"] == DBNull.Value ? string.Empty : (reader["emp_id"]).ToString(),
                            EmployeeNo1 = reader["emp_no_1"] == DBNull.Value ? string.Empty : (reader["emp_no_1"]).ToString(),
                            EmployeeNo2 = reader["emp_no_2"] == DBNull.Value ? string.Empty : (reader["emp_no_2"]).ToString(),
                            StartUpDate = reader["start_up_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["start_up_date"],
                            YearsOfExperience = reader["yrs_of_experience"] == DBNull.Value ? 0 : (int)reader["yrs_of_experience"],
                            StartUpDesignation = reader["start_up_designation"] == DBNull.Value ? String.Empty : reader["start_up_designation"].ToString(),
                            PlaceOfEngagement = reader["place_of_engagement"] == DBNull.Value ? String.Empty : reader["place_of_engagement"].ToString(),
                            ConfirmationDate = reader["confirmation_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["confirmation_date"],
                            CurrentDesignation = reader["current_designation"] == DBNull.Value ? String.Empty : reader["current_designation"].ToString(),
                            JobGrade = reader["job_grade"] == DBNull.Value ? String.Empty : reader["job_grade"].ToString(),
                            EmploymentStatus = reader["employment_status"] == DBNull.Value ? String.Empty : reader["employment_status"].ToString(),
                            DateOfLastPromotion = reader["date_of_last_promotion"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["date_of_last_promotion"],
                            LengthOfService = reader["start_up_date"] == DBNull.Value ? 0 : (int)((DateTime.Now - (DateTime)reader["start_up_date"]).TotalDays),

                            OfficialEmail = reader["official_email"] == DBNull.Value ? String.Empty : reader["official_email"].ToString(),
                            StateOfOrigin = reader["state_of_origin"] == DBNull.Value ? String.Empty : reader["state_of_origin"].ToString(),
                            LgaOfOrigin = reader["lga_of_origin"] == DBNull.Value ? String.Empty : reader["lga_of_origin"].ToString(),
                            Religion = reader["religion"] == DBNull.Value ? String.Empty : reader["religion"].ToString(),
                            GeoPoliticalRegion = reader["geo_political_region"] == DBNull.Value ? String.Empty : reader["geo_political_region"].ToString(),
                            NextOfKinName = reader["next_of_kin_name"] == DBNull.Value ? String.Empty : reader["next_of_kin_name"].ToString(),
                            NextOfKinRelationship = reader["next_of_kin_relationship"] == DBNull.Value ? String.Empty : reader["next_of_kin_relationship"].ToString(),
                            NextOfKinAddress = reader["next_of_kin_address"] == DBNull.Value ? String.Empty : reader["next_of_kin_address"].ToString(),
                            NextOfKinPhone = reader["next_of_kin_phone"] == DBNull.Value ? String.Empty : reader["next_of_kin_phone"].ToString(),
                            NextOfKinEmail = reader["next_of_kin_email"] == DBNull.Value ? String.Empty : reader["next_of_kin_email"].ToString(),
                            CompanyID = reader["coy_id"] == DBNull.Value ? string.Empty : (reader["coy_id"]).ToString(),
                            DepartmentID = reader["dept_id"] == DBNull.Value ? 0 : (int)(reader["dept_id"]),
                            UnitID = reader["unit_id"] == DBNull.Value ? 0 : (int)(reader["unit_id"]),
                            LocationID = reader["loc_id"] == DBNull.Value ? 0 : (int)(reader["loc_id"]),
                            EmployeeModifiedBy = reader["modified_by"] == DBNull.Value ? string.Empty : reader["modified_by"].ToString(),
                            EmployeeModifiedDate = reader["modified_date"] == DBNull.Value ? string.Empty : reader["modified_date"].ToString(),
                            EmployeeCreatedBy = reader["created_by"] == DBNull.Value ? string.Empty : reader["created_by"].ToString(),
                            EmployeeCreatedDate = reader["created_date"] == DBNull.Value ? string.Empty : reader["created_date"].ToString(),
                            IsDeactivated = reader["is_dx"] == DBNull.Value ? true : (bool)reader["is_dx"],
                            DeactivationTime = reader["dx_time"] == DBNull.Value ? string.Empty : reader["dx_time"].ToString(),
                            DeactivatedBy = reader["dx_by"] == DBNull.Value ? string.Empty : reader["dx_by"].ToString(),

                            PersonID = reader["id"] == DBNull.Value ? String.Empty : reader["id"].ToString(),
                            Title = reader["title"] == DBNull.Value ? string.Empty : reader["title"].ToString(),
                            Surname = reader["sname"] == DBNull.Value ? string.Empty : reader["sname"].ToString(),
                            FirstName = reader["fname"] == DBNull.Value ? string.Empty : reader["fname"].ToString(),
                            OtherNames = reader["oname"] == DBNull.Value ? string.Empty : reader["oname"].ToString(),
                            FullName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                            Sex = reader["sex"] == DBNull.Value ? string.Empty : reader["sex"].ToString(),
                            MaritalStatus = reader["maritalstatus"] == DBNull.Value ? String.Empty : reader["maritalstatus"].ToString(),
                            BirthDay = reader["birthday"] == DBNull.Value ? 0 : (int)reader["birthday"],
                            BirthMonth = reader["birthmonth"] == DBNull.Value ? 0 : (int)reader["birthmonth"],
                            BirthYear = reader["birthyear"] == DBNull.Value ? 0 : (int)reader["birthyear"],
                            PhoneNo1 = reader["phone1"] == DBNull.Value ? string.Empty : reader["phone1"].ToString(),
                            PhoneNo2 = reader["phone2"] == DBNull.Value ? string.Empty : reader["phone2"].ToString(),
                            Email = reader["personal_email"] == DBNull.Value ? string.Empty : reader["personal_email"].ToString(),
                            Address = reader["address"] == DBNull.Value ? string.Empty : reader["address"].ToString(),
                            ImagePath = reader["imgp"] == DBNull.Value ? string.Empty : reader["imgp"].ToString(),
                            ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                            ModifiedTime = reader["mdt"] == DBNull.Value ? string.Empty : reader["mdt"].ToString(),
                            CreatedTime = reader["ctt"] == DBNull.Value ? string.Empty : reader["ctt"].ToString(),
                            CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),

                            LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            LocationType = reader["loctype"] == DBNull.Value ? string.Empty : reader["loctype"].ToString(),
                            LocationHead1 = reader["lochq1"] == DBNull.Value ? string.Empty : reader["lochq1"].ToString(),
                            LocationHead2 = reader["lochq2"] == DBNull.Value ? string.Empty : reader["lochq2"].ToString(),
                            LocationCountry = reader["locctr"] == DBNull.Value ? string.Empty : reader["locctr"].ToString(),
                            LocationState = reader["locst"] == DBNull.Value ? string.Empty : reader["locst"].ToString(),
                            CompanyName = reader["coy_name"] == DBNull.Value ? string.Empty : reader["coy_name"].ToString(),
                            DepartmentHead1 = reader["depthd1"] == DBNull.Value ? string.Empty : reader["depthd1"].ToString(),
                            DepartmentHead2 = reader["depthd2"] == DBNull.Value ? string.Empty : reader["depthd2"].ToString(),
                            DepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                            UnitHead1 = reader["unithd1"] == DBNull.Value ? string.Empty : reader["unithd1"].ToString(),
                            UnitHead2 = reader["unithd2"] == DBNull.Value ? string.Empty : reader["unithd2"].ToString(),
                            UnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
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
            return employeeList;
        }

        public async Task<IList<Employee>> GetEmployeesByBirthMonthAsync(int birthMonth)
        {
            List<Employee> employeeList = new List<Employee>();
            if (birthMonth < 1) { throw new ArgumentNullException(nameof(birthMonth)); }
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT e.emp_id, e.emp_no_1, e.emp_no_2, e.start_up_date, ");
            sb.Append("e.yrs_of_experience, e.start_up_designation, e.place_of_engagement, ");
            sb.Append("e.confirmation_date, e.current_designation, e.job_grade, e.employment_status, ");
            sb.Append("e.date_of_last_promotion, e.official_email, e.state_of_origin, e.is_dx, ");
            sb.Append("e.lga_of_origin, e.religion, e.geo_political_region, e.next_of_kin_name, ");
            sb.Append("e.next_of_kin_relationship, e.modified_by, e.modified_date, e.dx_time, e.dx_by, ");
            sb.Append("e.created_by, e.created_date, e.next_of_kin_address, e.next_of_kin_phone, ");
            sb.Append("e.next_of_kin_email, e.dept_id, e.unit_id, e.loc_id, e.coy_id, p.id, p.title, ");
            sb.Append("p.sname, p.fname, p.oname, p.fullname, p.sex, p.phone1, p.phone2,");
            sb.Append("p.email AS personal_email, p.address, p.mdb, p.mdt, p.ctb, p.ctt, ");
            sb.Append("p.imgp, p.birthday, p.birthmonth, p.birthyear, p.maritalstatus, l.locname, ");
            sb.Append("l.loctype, l.lochq1, l.lochq2, l.locmb, l.locmd, l.loccb, l.loccd, l.locctr, ");
            sb.Append("l.locst, l.locqk, c.coy_code, c.coy_name, d.deptname, d.depthd1, d.depthd2, ");
            sb.Append("d.deptqk, u.unitname, u.unithd1, u.unithd2, u.unitqk ");
            sb.Append("FROM erm_emp_inf e JOIN gst_prsns p ON e.emp_id = p.id ");
            sb.Append("AND e.is_dx = false ");
            sb.Append("LEFT JOIN gst_locs l ON e.loc_id = l.locqk ");
            sb.Append("LEFT JOIN gst_coys c ON e.coy_id = c.coy_code ");
            sb.Append("LEFT JOIN gst_depts d ON e.dept_id = d.deptqk ");
            sb.Append("LEFT JOIN gst_units u ON e.unit_id = u.unitqk ");
            sb.Append("WHERE(p.birthmonth = @birthmonth) ");
            sb.Append("AND (e.is_dx = false) ORDER BY p.birthday;");
            query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var birthmonth = cmd.Parameters.Add("@birthmonth", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    birthmonth.Value = birthMonth;

                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        employeeList.Add(new Employee()
                        {
                            EmployeeID = reader["emp_id"] == DBNull.Value ? string.Empty : (reader["emp_id"]).ToString(),
                            EmployeeNo1 = reader["emp_no_1"] == DBNull.Value ? string.Empty : (reader["emp_no_1"]).ToString(),
                            EmployeeNo2 = reader["emp_no_2"] == DBNull.Value ? string.Empty : (reader["emp_no_2"]).ToString(),
                            StartUpDate = reader["start_up_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["start_up_date"],
                            YearsOfExperience = reader["yrs_of_experience"] == DBNull.Value ? 0 : (int)reader["yrs_of_experience"],
                            StartUpDesignation = reader["start_up_designation"] == DBNull.Value ? String.Empty : reader["start_up_designation"].ToString(),
                            PlaceOfEngagement = reader["place_of_engagement"] == DBNull.Value ? String.Empty : reader["place_of_engagement"].ToString(),
                            ConfirmationDate = reader["confirmation_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["confirmation_date"],
                            CurrentDesignation = reader["current_designation"] == DBNull.Value ? String.Empty : reader["current_designation"].ToString(),
                            JobGrade = reader["job_grade"] == DBNull.Value ? String.Empty : reader["job_grade"].ToString(),
                            EmploymentStatus = reader["employment_status"] == DBNull.Value ? String.Empty : reader["employment_status"].ToString(),
                            DateOfLastPromotion = reader["date_of_last_promotion"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["date_of_last_promotion"],
                            LengthOfService = reader["start_up_date"] == DBNull.Value ? 0 : (int)((DateTime.Now - (DateTime)reader["start_up_date"]).TotalDays),

                            OfficialEmail = reader["official_email"] == DBNull.Value ? String.Empty : reader["official_email"].ToString(),
                            StateOfOrigin = reader["state_of_origin"] == DBNull.Value ? String.Empty : reader["state_of_origin"].ToString(),
                            LgaOfOrigin = reader["lga_of_origin"] == DBNull.Value ? String.Empty : reader["lga_of_origin"].ToString(),
                            Religion = reader["religion"] == DBNull.Value ? String.Empty : reader["religion"].ToString(),
                            GeoPoliticalRegion = reader["geo_political_region"] == DBNull.Value ? String.Empty : reader["geo_political_region"].ToString(),
                            NextOfKinName = reader["next_of_kin_name"] == DBNull.Value ? String.Empty : reader["next_of_kin_name"].ToString(),
                            NextOfKinRelationship = reader["next_of_kin_relationship"] == DBNull.Value ? String.Empty : reader["next_of_kin_relationship"].ToString(),
                            NextOfKinAddress = reader["next_of_kin_address"] == DBNull.Value ? String.Empty : reader["next_of_kin_address"].ToString(),
                            NextOfKinPhone = reader["next_of_kin_phone"] == DBNull.Value ? String.Empty : reader["next_of_kin_phone"].ToString(),
                            NextOfKinEmail = reader["next_of_kin_email"] == DBNull.Value ? String.Empty : reader["next_of_kin_email"].ToString(),
                            CompanyID = reader["coy_id"] == DBNull.Value ? string.Empty : (reader["coy_id"]).ToString(),
                            DepartmentID = reader["dept_id"] == DBNull.Value ? 0 : (int)(reader["dept_id"]),
                            UnitID = reader["unit_id"] == DBNull.Value ? 0 : (int)(reader["unit_id"]),
                            LocationID = reader["loc_id"] == DBNull.Value ? 0 : (int)(reader["loc_id"]),
                            EmployeeModifiedBy = reader["modified_by"] == DBNull.Value ? string.Empty : reader["modified_by"].ToString(),
                            EmployeeModifiedDate = reader["modified_date"] == DBNull.Value ? string.Empty : reader["modified_date"].ToString(),
                            EmployeeCreatedBy = reader["created_by"] == DBNull.Value ? string.Empty : reader["created_by"].ToString(),
                            EmployeeCreatedDate = reader["created_date"] == DBNull.Value ? string.Empty : reader["created_date"].ToString(),
                            IsDeactivated = reader["is_dx"] == DBNull.Value ? true : (bool)reader["is_dx"],
                            DeactivationTime = reader["dx_time"] == DBNull.Value ? string.Empty : reader["dx_time"].ToString(),
                            DeactivatedBy = reader["dx_by"] == DBNull.Value ? string.Empty : reader["dx_by"].ToString(),

                            PersonID = reader["id"] == DBNull.Value ? String.Empty : reader["id"].ToString(),
                            Title = reader["title"] == DBNull.Value ? string.Empty : reader["title"].ToString(),
                            Surname = reader["sname"] == DBNull.Value ? string.Empty : reader["sname"].ToString(),
                            FirstName = reader["fname"] == DBNull.Value ? string.Empty : reader["fname"].ToString(),
                            OtherNames = reader["oname"] == DBNull.Value ? string.Empty : reader["oname"].ToString(),
                            FullName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                            Sex = reader["sex"] == DBNull.Value ? string.Empty : reader["sex"].ToString(),
                            MaritalStatus = reader["maritalstatus"] == DBNull.Value ? String.Empty : reader["maritalstatus"].ToString(),
                            BirthDay = reader["birthday"] == DBNull.Value ? 0 : (int)reader["birthday"],
                            BirthMonth = reader["birthmonth"] == DBNull.Value ? 0 : (int)reader["birthmonth"],
                            BirthYear = reader["birthyear"] == DBNull.Value ? 0 : (int)reader["birthyear"],
                            PhoneNo1 = reader["phone1"] == DBNull.Value ? string.Empty : reader["phone1"].ToString(),
                            PhoneNo2 = reader["phone2"] == DBNull.Value ? string.Empty : reader["phone2"].ToString(),
                            Email = reader["personal_email"] == DBNull.Value ? string.Empty : reader["personal_email"].ToString(),
                            Address = reader["address"] == DBNull.Value ? string.Empty : reader["address"].ToString(),
                            ImagePath = reader["imgp"] == DBNull.Value ? string.Empty : reader["imgp"].ToString(),
                            ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                            ModifiedTime = reader["mdt"] == DBNull.Value ? string.Empty : reader["mdt"].ToString(),
                            CreatedTime = reader["ctt"] == DBNull.Value ? string.Empty : reader["ctt"].ToString(),
                            CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),

                            LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            LocationType = reader["loctype"] == DBNull.Value ? string.Empty : reader["loctype"].ToString(),
                            LocationHead1 = reader["lochq1"] == DBNull.Value ? string.Empty : reader["lochq1"].ToString(),
                            LocationHead2 = reader["lochq2"] == DBNull.Value ? string.Empty : reader["lochq2"].ToString(),
                            LocationCountry = reader["locctr"] == DBNull.Value ? string.Empty : reader["locctr"].ToString(),
                            LocationState = reader["locst"] == DBNull.Value ? string.Empty : reader["locst"].ToString(),
                            CompanyName = reader["coy_name"] == DBNull.Value ? string.Empty : reader["coy_name"].ToString(),
                            DepartmentHead1 = reader["depthd1"] == DBNull.Value ? string.Empty : reader["depthd1"].ToString(),
                            DepartmentHead2 = reader["depthd2"] == DBNull.Value ? string.Empty : reader["depthd2"].ToString(),
                            DepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                            UnitHead1 = reader["unithd1"] == DBNull.Value ? string.Empty : reader["unithd1"].ToString(),
                            UnitHead2 = reader["unithd2"] == DBNull.Value ? string.Empty : reader["unithd2"].ToString(),
                            UnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
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
            return employeeList;
        }

        public async Task<IList<Employee>> GetEmployeesByBirthMonthAndBirthDayAsync(int birthMonth, int birthDay)
        {
            List<Employee> employeeList = new List<Employee>();
            if (birthMonth < 1) { throw new ArgumentNullException(nameof(birthMonth)); }
            if (birthDay < 1) { throw new ArgumentNullException(nameof(birthDay)); }

            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT e.emp_id, e.emp_no_1, e.emp_no_2, e.start_up_date, ");
            sb.Append("e.yrs_of_experience, e.start_up_designation, e.place_of_engagement, ");
            sb.Append("e.confirmation_date, e.current_designation, e.job_grade, e.employment_status, ");
            sb.Append("e.date_of_last_promotion, e.official_email, e.state_of_origin, e.is_dx, ");
            sb.Append("e.lga_of_origin, e.religion, e.geo_political_region, e.next_of_kin_name, ");
            sb.Append("e.next_of_kin_relationship, e.modified_by, e.modified_date, e.dx_time, e.dx_by, ");
            sb.Append("e.created_by, e.created_date, e.next_of_kin_address, e.next_of_kin_phone, ");
            sb.Append("e.next_of_kin_email, e.dept_id, e.unit_id, e.loc_id, e.coy_id, p.id, p.title, ");
            sb.Append("p.sname, p.fname, p.oname, p.fullname, p.sex, p.phone1, p.phone2,");
            sb.Append("p.email AS personal_email, p.address, p.mdb, p.mdt, p.ctb, p.ctt, ");
            sb.Append("p.imgp, p.birthday, p.birthmonth, p.birthyear, p.maritalstatus, l.locname, ");
            sb.Append("l.loctype, l.lochq1, l.lochq2, l.locmb, l.locmd, l.loccb, l.loccd, l.locctr, ");
            sb.Append("l.locst, l.locqk, c.coy_code, c.coy_name, d.deptname, d.depthd1, d.depthd2, ");
            sb.Append("d.deptqk, u.unitname, u.unithd1, u.unithd2, u.unitqk ");
            sb.Append("FROM erm_emp_inf e JOIN gst_prsns p ON e.emp_id = p.id ");
            sb.Append("AND e.is_dx = false ");
            sb.Append("LEFT JOIN gst_locs l ON e.loc_id = l.locqk ");
            sb.Append("LEFT JOIN gst_coys c ON e.coy_id = c.coy_code ");
            sb.Append("LEFT JOIN gst_depts d ON e.dept_id = d.deptqk ");
            sb.Append("LEFT JOIN gst_units u ON e.unit_id = u.unitqk ");
            sb.Append("WHERE(p.birthmonth = @birthmonth) And (p.birthday = @birthday) ");
            sb.Append("AND (e.is_dx = false) ORDER BY p.fullname;");
            query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var birthmonth = cmd.Parameters.Add("@birthmonth", NpgsqlDbType.Integer);
                    var birthday = cmd.Parameters.Add("@birthday", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    birthmonth.Value = birthMonth;
                    birthday.Value = birthDay;

                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        employeeList.Add(new Employee()
                        {
                            EmployeeID = reader["emp_id"] == DBNull.Value ? string.Empty : (reader["emp_id"]).ToString(),
                            EmployeeNo1 = reader["emp_no_1"] == DBNull.Value ? string.Empty : (reader["emp_no_1"]).ToString(),
                            EmployeeNo2 = reader["emp_no_2"] == DBNull.Value ? string.Empty : (reader["emp_no_2"]).ToString(),
                            StartUpDate = reader["start_up_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["start_up_date"],
                            YearsOfExperience = reader["yrs_of_experience"] == DBNull.Value ? 0 : (int)reader["yrs_of_experience"],
                            StartUpDesignation = reader["start_up_designation"] == DBNull.Value ? String.Empty : reader["start_up_designation"].ToString(),
                            PlaceOfEngagement = reader["place_of_engagement"] == DBNull.Value ? String.Empty : reader["place_of_engagement"].ToString(),
                            ConfirmationDate = reader["confirmation_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["confirmation_date"],
                            CurrentDesignation = reader["current_designation"] == DBNull.Value ? String.Empty : reader["current_designation"].ToString(),
                            JobGrade = reader["job_grade"] == DBNull.Value ? String.Empty : reader["job_grade"].ToString(),
                            EmploymentStatus = reader["employment_status"] == DBNull.Value ? String.Empty : reader["employment_status"].ToString(),
                            DateOfLastPromotion = reader["date_of_last_promotion"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["date_of_last_promotion"],
                            LengthOfService = reader["start_up_date"] == DBNull.Value ? 0 : (int)((DateTime.Now - (DateTime)reader["start_up_date"]).TotalDays),

                            OfficialEmail = reader["official_email"] == DBNull.Value ? String.Empty : reader["official_email"].ToString(),
                            StateOfOrigin = reader["state_of_origin"] == DBNull.Value ? String.Empty : reader["state_of_origin"].ToString(),
                            LgaOfOrigin = reader["lga_of_origin"] == DBNull.Value ? String.Empty : reader["lga_of_origin"].ToString(),
                            Religion = reader["religion"] == DBNull.Value ? String.Empty : reader["religion"].ToString(),
                            GeoPoliticalRegion = reader["geo_political_region"] == DBNull.Value ? String.Empty : reader["geo_political_region"].ToString(),
                            NextOfKinName = reader["next_of_kin_name"] == DBNull.Value ? String.Empty : reader["next_of_kin_name"].ToString(),
                            NextOfKinRelationship = reader["next_of_kin_relationship"] == DBNull.Value ? String.Empty : reader["next_of_kin_relationship"].ToString(),
                            NextOfKinAddress = reader["next_of_kin_address"] == DBNull.Value ? String.Empty : reader["next_of_kin_address"].ToString(),
                            NextOfKinPhone = reader["next_of_kin_phone"] == DBNull.Value ? String.Empty : reader["next_of_kin_phone"].ToString(),
                            NextOfKinEmail = reader["next_of_kin_email"] == DBNull.Value ? String.Empty : reader["next_of_kin_email"].ToString(),
                            CompanyID = reader["coy_id"] == DBNull.Value ? string.Empty : (reader["coy_id"]).ToString(),
                            DepartmentID = reader["dept_id"] == DBNull.Value ? 0 : (int)(reader["dept_id"]),
                            UnitID = reader["unit_id"] == DBNull.Value ? 0 : (int)(reader["unit_id"]),
                            LocationID = reader["loc_id"] == DBNull.Value ? 0 : (int)(reader["loc_id"]),
                            EmployeeModifiedBy = reader["modified_by"] == DBNull.Value ? string.Empty : reader["modified_by"].ToString(),
                            EmployeeModifiedDate = reader["modified_date"] == DBNull.Value ? string.Empty : reader["modified_date"].ToString(),
                            EmployeeCreatedBy = reader["created_by"] == DBNull.Value ? string.Empty : reader["created_by"].ToString(),
                            EmployeeCreatedDate = reader["created_date"] == DBNull.Value ? string.Empty : reader["created_date"].ToString(),
                            IsDeactivated = reader["is_dx"] == DBNull.Value ? true : (bool)reader["is_dx"],
                            DeactivationTime = reader["dx_time"] == DBNull.Value ? string.Empty : reader["dx_time"].ToString(),
                            DeactivatedBy = reader["dx_by"] == DBNull.Value ? string.Empty : reader["dx_by"].ToString(),

                            PersonID = reader["id"] == DBNull.Value ? String.Empty : reader["id"].ToString(),
                            Title = reader["title"] == DBNull.Value ? string.Empty : reader["title"].ToString(),
                            Surname = reader["sname"] == DBNull.Value ? string.Empty : reader["sname"].ToString(),
                            FirstName = reader["fname"] == DBNull.Value ? string.Empty : reader["fname"].ToString(),
                            OtherNames = reader["oname"] == DBNull.Value ? string.Empty : reader["oname"].ToString(),
                            FullName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                            Sex = reader["sex"] == DBNull.Value ? string.Empty : reader["sex"].ToString(),
                            MaritalStatus = reader["maritalstatus"] == DBNull.Value ? String.Empty : reader["maritalstatus"].ToString(),
                            BirthDay = reader["birthday"] == DBNull.Value ? 0 : (int)reader["birthday"],
                            BirthMonth = reader["birthmonth"] == DBNull.Value ? 0 : (int)reader["birthmonth"],
                            BirthYear = reader["birthyear"] == DBNull.Value ? 0 : (int)reader["birthyear"],
                            PhoneNo1 = reader["phone1"] == DBNull.Value ? string.Empty : reader["phone1"].ToString(),
                            PhoneNo2 = reader["phone2"] == DBNull.Value ? string.Empty : reader["phone2"].ToString(),
                            Email = reader["personal_email"] == DBNull.Value ? string.Empty : reader["personal_email"].ToString(),
                            Address = reader["address"] == DBNull.Value ? string.Empty : reader["address"].ToString(),
                            ImagePath = reader["imgp"] == DBNull.Value ? string.Empty : reader["imgp"].ToString(),
                            ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                            ModifiedTime = reader["mdt"] == DBNull.Value ? string.Empty : reader["mdt"].ToString(),
                            CreatedTime = reader["ctt"] == DBNull.Value ? string.Empty : reader["ctt"].ToString(),
                            CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),

                            LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            LocationType = reader["loctype"] == DBNull.Value ? string.Empty : reader["loctype"].ToString(),
                            LocationHead1 = reader["lochq1"] == DBNull.Value ? string.Empty : reader["lochq1"].ToString(),
                            LocationHead2 = reader["lochq2"] == DBNull.Value ? string.Empty : reader["lochq2"].ToString(),
                            LocationCountry = reader["locctr"] == DBNull.Value ? string.Empty : reader["locctr"].ToString(),
                            LocationState = reader["locst"] == DBNull.Value ? string.Empty : reader["locst"].ToString(),
                            CompanyName = reader["coy_name"] == DBNull.Value ? string.Empty : reader["coy_name"].ToString(),
                            DepartmentHead1 = reader["depthd1"] == DBNull.Value ? string.Empty : reader["depthd1"].ToString(),
                            DepartmentHead2 = reader["depthd2"] == DBNull.Value ? string.Empty : reader["depthd2"].ToString(),
                            DepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                            UnitHead1 = reader["unithd1"] == DBNull.Value ? string.Empty : reader["unithd1"].ToString(),
                            UnitHead2 = reader["unithd2"] == DBNull.Value ? string.Empty : reader["unithd2"].ToString(),
                            UnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
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
            return employeeList;
        }

        public async Task<int> GetEmployeesCountByStartUpDateAsync(int startUpYear, int startUpMonth, int startUpDay)
        {
            int employeeCount = 0;
            if (startUpYear < 1) { throw new ArgumentNullException(nameof(startUpYear)); }
            if (startUpMonth < 1) { throw new ArgumentNullException(nameof(startUpMonth)); }
            if (startUpDay < 1) { throw new ArgumentNullException(nameof(startUpDay)); }

            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT COUNT (emp_id) as no_count ");
            sb.Append("FROM erm_emp_inf ");
            sb.Append("WHERE (date_part('year', start_up_date) = @start_up_year) ");
            sb.Append("AND (date_part('month', start_up_date) = @start_up_month) ");
            sb.Append("AND (date_part('day', start_up_date) = @start_up_day); ");

            query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var start_up_year = cmd.Parameters.Add("@start_up_year", NpgsqlDbType.Integer);
                var start_up_month = cmd.Parameters.Add("@start_up_month", NpgsqlDbType.Integer);
                var start_up_day = cmd.Parameters.Add("@start_up_day", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                start_up_year.Value = startUpYear;
                start_up_month.Value = startUpMonth;
                start_up_day.Value = startUpDay;

                var no_count = await cmd.ExecuteScalarAsync();
                employeeCount = Convert.ToInt32(no_count);
            }
            await conn.CloseAsync();
            return employeeCount;
        }

        public async Task<IList<Employee>> GetAllEmployeesWithoutUserAccountsAsync()
        {
            List<Employee> employeeList = new List<Employee>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT e.emp_id, e.emp_no_1, e.emp_no_2, e.official_email, e.dept_id,  ");
            sb.Append("e.unit_id, e.loc_id, e.coy_id, e.is_dx, e.dx_time, e.dx_by, p.id, ");
            sb.Append("p.fullname, p.sex, p.phone1, p.phone2, p.is_dx, p.dx_by, p.dx_time, ");
            sb.Append("p.imgp, a.usr_id, a.usr_nm, a.usr_typ, u.unitname, d.deptname, ");
            sb.Append("l.locname FROM public.erm_emp_inf e ");
            sb.Append("INNER JOIN public.gst_prsns p ON e.emp_id = p.id ");
            sb.Append("AND e.is_dx = false ");
            sb.Append("INNER JOIN public.gst_units u ON e.unit_id = u.unitqk ");
            sb.Append("INNER JOIN public.gst_depts d ON e.dept_id = d.deptqk ");
            sb.Append("INNER JOIN public.gst_locs l ON e.loc_id = l.locqk ");
            sb.Append("LEFT OUTER JOIN public.sct_usr_acct a ON p.id = a.usr_id ");
            sb.Append("WHERE (e.is_dx = false OR p.is_dx = false) AND (a.usr_id IS NULL) ");
            sb.Append("ORDER BY p.fullname;");

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
                        employeeList.Add(new Employee()
                        {
                            EmployeeID = reader["emp_id"] == DBNull.Value ? string.Empty : (reader["emp_id"]).ToString(),
                            EmployeeNo1 = reader["emp_no_1"] == DBNull.Value ? string.Empty : (reader["emp_no_1"]).ToString(),
                            EmployeeNo2 = reader["emp_no_2"] == DBNull.Value ? string.Empty : (reader["emp_no_2"]).ToString(),

                            OfficialEmail = reader["official_email"] == DBNull.Value ? String.Empty : reader["official_email"].ToString(),
                            CompanyID = reader["coy_id"] == DBNull.Value ? string.Empty : (reader["coy_id"]).ToString(),
                            DepartmentID = reader["dept_id"] == DBNull.Value ? 0 : (int)(reader["dept_id"]),
                            UnitID = reader["unit_id"] == DBNull.Value ? 0 : (int)(reader["unit_id"]),
                            LocationID = reader["loc_id"] == DBNull.Value ? 0 : (int)(reader["loc_id"]),

                            PersonID = reader["id"] == DBNull.Value ? String.Empty : reader["id"].ToString(),
                            FullName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                            Sex = reader["sex"] == DBNull.Value ? string.Empty : reader["sex"].ToString(),
                            PhoneNo1 = reader["phone1"] == DBNull.Value ? string.Empty : reader["phone1"].ToString(),
                            PhoneNo2 = reader["phone2"] == DBNull.Value ? string.Empty : reader["phone2"].ToString(),
                            ImagePath = reader["imgp"] == DBNull.Value ? string.Empty : reader["imgp"].ToString(),

                            LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            DepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                            UnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
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
            return employeeList;
        }

        public async Task<IList<Employee>> GetEmployeesWithoutUserAccountsByNameAsync(string employeeName)
        {
            List<Employee> employeeList = new List<Employee>();
            if (string.IsNullOrWhiteSpace(employeeName)) { throw new ArgumentNullException(nameof(employeeName)); }
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT e.emp_id, e.emp_no_1, e.emp_no_2, e.official_email, e.dept_id, ");
            sb.Append("e.unit_id, e.loc_id, e.coy_id, e.is_dx, e.dx_time, e.dx_by, p.id, ");
            sb.Append("p.fullname, p.sex, p.phone1, p.phone2, p.is_dx, p.dx_by, p.dx_time, ");
            sb.Append("p.fname, p.sname, p.oname, p.imgp, a.usr_id, a.usr_nm, a.usr_typ, ");
            sb.Append("u.unitname, d.deptname, l.locname FROM public.erm_emp_inf e ");
            sb.Append("INNER JOIN public.gst_prsns p ON e.emp_id = p.id ");
            sb.Append("AND e.is_dx = false ");
            sb.Append("INNER JOIN public.gst_units u ON e.unit_id = u.unitqk ");
            sb.Append("INNER JOIN public.gst_depts d ON e.dept_id = d.deptqk ");
            sb.Append("INNER JOIN public.gst_locs l ON e.loc_id = l.locqk ");
            sb.Append("LEFT OUTER JOIN public.sct_usr_acct a ON p.id = a.usr_id ");
            sb.Append("WHERE (e.is_dx = false OR p.is_dx = false) AND (a.usr_id IS NULL) ");
            sb.Append("AND ((LOWER(p.fname) LIKE '%'||LOWER(@name)||'%') ");
            sb.Append("OR (LOWER(p.sname) LIKE '%'||LOWER(@name)||'%') ");
            sb.Append("OR (LOWER(p.oname) LIKE '%'||LOWER(@name)||'%') ");
            sb.Append("OR (LOWER(p.fullname) LIKE '%'||LOWER(@name)||'%')) ");
            sb.Append("ORDER BY p.fullname;");
            query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var name = cmd.Parameters.Add("@name", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    name.Value = employeeName;

                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        employeeList.Add(new Employee()
                        {
                            EmployeeID = reader["emp_id"] == DBNull.Value ? string.Empty : (reader["emp_id"]).ToString(),
                            EmployeeNo1 = reader["emp_no_1"] == DBNull.Value ? string.Empty : (reader["emp_no_1"]).ToString(),
                            EmployeeNo2 = reader["emp_no_2"] == DBNull.Value ? string.Empty : (reader["emp_no_2"]).ToString(),

                            OfficialEmail = reader["official_email"] == DBNull.Value ? String.Empty : reader["official_email"].ToString(),
                            CompanyID = reader["coy_id"] == DBNull.Value ? string.Empty : (reader["coy_id"]).ToString(),
                            DepartmentID = reader["dept_id"] == DBNull.Value ? 0 : (int)(reader["dept_id"]),
                            UnitID = reader["unit_id"] == DBNull.Value ? 0 : (int)(reader["unit_id"]),
                            LocationID = reader["loc_id"] == DBNull.Value ? 0 : (int)(reader["loc_id"]),

                            PersonID = reader["id"] == DBNull.Value ? String.Empty : reader["id"].ToString(),
                            FullName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                            Sex = reader["sex"] == DBNull.Value ? string.Empty : reader["sex"].ToString(),
                            PhoneNo1 = reader["phone1"] == DBNull.Value ? string.Empty : reader["phone1"].ToString(),
                            PhoneNo2 = reader["phone2"] == DBNull.Value ? string.Empty : reader["phone2"].ToString(),
                            ImagePath = reader["imgp"] == DBNull.Value ? string.Empty : reader["imgp"].ToString(),

                            LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            DepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                            UnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
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
            return employeeList;
        }

        #endregion

        #region Employee Write Action Methods
        public async Task<bool> AddEmployeeAsync(Employee employee)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.erm_emp_inf(emp_id, emp_no_1, emp_no_2, start_up_date, ");
            sb.Append("start_up_designation, place_of_engagement, confirmation_date, current_designation, ");
            sb.Append("job_grade, employment_status, date_of_last_promotion, official_email, state_of_origin, ");
            sb.Append("lga_of_origin, religion, geo_political_region, next_of_kin_name, next_of_kin_relationship, ");
            sb.Append("modified_by, modified_date, created_by, created_date, next_of_kin_address, next_of_kin_phone, ");
            sb.Append("next_of_kin_email, dept_id, unit_id, loc_id, coy_id, yrs_of_experience, is_dx, dx_time, dx_by) ");
            sb.Append("VALUES (@emp_id, @emp_no_1, @emp_no_2, @start_up_date, @start_up_designation, ");
            sb.Append("@place_of_engagement, @confirmation_date, @current_designation, @job_grade,  ");
            sb.Append("@employment_status, @date_of_last_promotion, @official_email, @state_of_origin, ");
            sb.Append("@lga_of_origin, @religion, @geo_political_region, @next_of_kin_name,  ");
            sb.Append("@next_of_kin_relationship, @modified_by, @modified_date, @created_by, @created_date, ");
            sb.Append("@next_of_kin_address, @next_of_kin_phone, @next_of_kin_email, @dept_id, ");
            sb.Append("@unit_id, @loc_id, @coy_id, @yrs_of_experience, @is_dx, @dx_time, @dx_by); ");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var employeeId = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                    var employeeNo1 = cmd.Parameters.Add("@emp_no_1", NpgsqlDbType.Text);
                    var employeeNo2 = cmd.Parameters.Add("@emp_no_2", NpgsqlDbType.Text);
                    var startUpDate = cmd.Parameters.Add("@start_up_date", NpgsqlDbType.Date);
                    var startUpDesignation = cmd.Parameters.Add("@start_up_designation", NpgsqlDbType.Text);
                    var placeOfEngagement = cmd.Parameters.Add("@place_of_engagement", NpgsqlDbType.Text);
                    var confirmationDate = cmd.Parameters.Add("@confirmation_date", NpgsqlDbType.Date);
                    var currentDesignation = cmd.Parameters.Add("@current_designation", NpgsqlDbType.Text);
                    var jobGrade = cmd.Parameters.Add("@job_grade", NpgsqlDbType.Text);
                    var employmentStatus = cmd.Parameters.Add("@employment_status", NpgsqlDbType.Text);
                    var dateOfLastPromotion = cmd.Parameters.Add("@date_of_last_promotion", NpgsqlDbType.Date);
                    var officialEmail = cmd.Parameters.Add("@official_email", NpgsqlDbType.Text);
                    var stateOfOrigin = cmd.Parameters.Add("@state_of_origin", NpgsqlDbType.Text);
                    var lgaOfOrigin = cmd.Parameters.Add("@lga_of_origin", NpgsqlDbType.Text);
                    var religion = cmd.Parameters.Add("@religion", NpgsqlDbType.Text);
                    var geoPoliticalRegion = cmd.Parameters.Add("@geo_political_region", NpgsqlDbType.Text);
                    var nextOfKinName = cmd.Parameters.Add("@next_of_kin_name", NpgsqlDbType.Text);
                    var nextOfKinRelationship = cmd.Parameters.Add("@next_of_kin_relationship", NpgsqlDbType.Text);
                    var nextOfKinAddress = cmd.Parameters.Add("@next_of_kin_address", NpgsqlDbType.Text);
                    var nextOfKinPhone = cmd.Parameters.Add("@next_of_kin_phone", NpgsqlDbType.Text);
                    var nextOfKinEmail = cmd.Parameters.Add("@next_of_kin_email", NpgsqlDbType.Text);
                    var modifiedBy = cmd.Parameters.Add("@modified_by", NpgsqlDbType.Text);
                    var modifiedDate = cmd.Parameters.Add("@modified_date", NpgsqlDbType.Text);
                    var createdBy = cmd.Parameters.Add("@created_by", NpgsqlDbType.Text);
                    var createdDate = cmd.Parameters.Add("@created_date", NpgsqlDbType.Text);
                    var companyId = cmd.Parameters.Add("@coy_id", NpgsqlDbType.Text);
                    var departmentId = cmd.Parameters.Add("@dept_id", NpgsqlDbType.Integer);
                    var unitId = cmd.Parameters.Add("unit_id", NpgsqlDbType.Integer);
                    var locationId = cmd.Parameters.Add("@loc_id", NpgsqlDbType.Integer);
                    var yearsOfExperience = cmd.Parameters.Add("@yrs_of_experience", NpgsqlDbType.Integer);
                    var isDeactivated = cmd.Parameters.Add("@is_dx", NpgsqlDbType.Boolean);
                    var deactivatedTime = cmd.Parameters.Add("@dx_time", NpgsqlDbType.Text);
                    var deactivatedBy = cmd.Parameters.Add("@dx_by", NpgsqlDbType.Text);

                    cmd.Prepare();

                    employeeId.Value = employee.EmployeeID;
                    employeeNo1.Value = employee.EmployeeNo1 ?? (object)DBNull.Value;
                    employeeNo2.Value = employee.EmployeeNo2 ?? (object)DBNull.Value;
                    companyId.Value = employee.CompanyID ?? (object)DBNull.Value;
                    departmentId.Value = employee.DepartmentID ?? (object)DBNull.Value;
                    unitId.Value = employee.UnitID ?? (object)DBNull.Value;
                    locationId.Value = employee.LocationID ?? (object)DBNull.Value;
                    startUpDate.Value = employee.StartUpDate ?? (object)DBNull.Value;
                    yearsOfExperience.Value = employee.YearsOfExperience ?? (object)DBNull.Value;
                    startUpDesignation.Value = employee.StartUpDesignation ?? (object)DBNull.Value;
                    placeOfEngagement.Value = employee.PlaceOfEngagement ?? (object)DBNull.Value;
                    confirmationDate.Value = employee.ConfirmationDate ?? (object)DBNull.Value;
                    currentDesignation.Value = employee.CurrentDesignation ?? (object)DBNull.Value;
                    jobGrade.Value = employee.JobGrade ?? (object)DBNull.Value;
                    employmentStatus.Value = employee.EmploymentStatus ?? (object)DBNull.Value;
                    dateOfLastPromotion.Value = employee.DateOfLastPromotion ?? (object)DBNull.Value;
                    officialEmail.Value = employee.OfficialEmail ?? (object)DBNull.Value;
                    stateOfOrigin.Value = employee.StateOfOrigin ?? (object)DBNull.Value;
                    lgaOfOrigin.Value = employee.LgaOfOrigin ?? (object)DBNull.Value;
                    religion.Value = employee.Religion ?? (object)DBNull.Value;
                    geoPoliticalRegion.Value = employee.GeoPoliticalRegion ?? (object)DBNull.Value;
                    nextOfKinAddress.Value = employee.NextOfKinAddress ?? (object)DBNull.Value;
                    nextOfKinEmail.Value = employee.NextOfKinEmail ?? (object)DBNull.Value;
                    nextOfKinName.Value = employee.NextOfKinName ?? (object)DBNull.Value;
                    nextOfKinPhone.Value = employee.NextOfKinPhone ?? (object)DBNull.Value;
                    nextOfKinRelationship.Value = employee.NextOfKinRelationship ?? (object)DBNull.Value;
                    modifiedBy.Value = employee.ModifiedBy ?? (object)DBNull.Value;
                    modifiedDate.Value = employee.ModifiedTime ?? (object)DBNull.Value;
                    createdBy.Value = employee.CreatedBy ?? (object)DBNull.Value;
                    createdDate.Value = employee.CreatedTime ?? (object)DBNull.Value;
                    isDeactivated.Value = employee.IsDeactivated;
                    deactivatedTime.Value = employee.DeactivationTime ?? (object)DBNull.Value;
                    deactivatedBy.Value = employee.DeactivatedBy ?? (object)DBNull.Value;

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

        public async Task<bool> UpdateEmployeeSeparationAsync(string empId, string recordedBy, string recordedTime)
        {
            if (string.IsNullOrEmpty(empId)) { return false; }
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.erm_emp_inf SET is_dx = true, dx_time = @dx_time, ");
            sb.Append("dx_by = @dx_by WHERE (emp_id = @emp_id);");
            string query = sb.ToString();
  
                await conn.OpenAsync();
                //Delete data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                    var dx_time = cmd.Parameters.Add("@dx_time", NpgsqlDbType.Text);
                    var dx_by = cmd.Parameters.Add("@dx_by", NpgsqlDbType.Text);
                    cmd.Prepare();
                    emp_id.Value = empId;
                    dx_time.Value = recordedTime;
                    dx_by.Value = recordedBy;

                    rows = await cmd.ExecuteNonQueryAsync();
                }
            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<bool> DeleteEmployeeAsync(string Id, string deletedBy, string deletedTime)
        {
            if (string.IsNullOrEmpty(Id)) { return false; }
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.erm_emp_inf SET is_dx = true, dx_time = @dx_time, ");
            sb.Append("dx_by = @dx_by WHERE (emp_id = @emp_id);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Delete data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                    var dx_time = cmd.Parameters.Add("@dx_time", NpgsqlDbType.Text);
                    var dx_by = cmd.Parameters.Add("@dx_by", NpgsqlDbType.Text);
                    cmd.Prepare();
                    emp_id.Value = Id;
                    dx_time.Value = deletedTime;
                    dx_by.Value = deletedBy;

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

        public async Task<bool> EditEmployeeAsync(Employee employee)
        {
            if (employee == null) { throw new ArgumentNullException(nameof(employee), "The required parameter [Employee] is missing or has an invalid value."); }
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.erm_emp_inf SET emp_no_2 = @emp_no_2, ");
            sb.Append("start_up_date = @start_up_date, start_up_designation = @start_up_designation, ");
            sb.Append("place_of_engagement = @place_of_engagement, confirmation_date = @confirmation_date, ");
            sb.Append("current_designation = @current_designation, job_grade = @job_grade, ");
            sb.Append("employment_status = @employment_status, date_of_last_promotion = @date_of_last_promotion, ");
            sb.Append("official_email = @official_email, state_of_origin = @state_of_origin, ");
            sb.Append("lga_of_origin = @lga_of_origin, religion = @religion, geo_political_region = @geo_political_region, ");
            sb.Append("modified_by = @modified_by, modified_date = @modified_date, ");
            sb.Append("dept_id = @dept_id, unit_id = @unit_id, loc_id = @loc_id, coy_id = @coy_id, ");
            sb.Append("yrs_of_experience = @yrs_of_experience WHERE (emp_id = @emp_id);");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var employeeId = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                    var employeeNo2 = cmd.Parameters.Add("@emp_no_2", NpgsqlDbType.Text);
                    var startUpDate = cmd.Parameters.Add("@start_up_date", NpgsqlDbType.Date);
                    var startUpDesignation = cmd.Parameters.Add("@start_up_designation", NpgsqlDbType.Text);
                    var placeOfEngagement = cmd.Parameters.Add("@place_of_engagement", NpgsqlDbType.Text);
                    var confirmationDate = cmd.Parameters.Add("@confirmation_date", NpgsqlDbType.Date);
                    var currentDesignation = cmd.Parameters.Add("@current_designation", NpgsqlDbType.Text);
                    var jobGrade = cmd.Parameters.Add("@job_grade", NpgsqlDbType.Text);
                    var employmentStatus = cmd.Parameters.Add("@employment_status", NpgsqlDbType.Text);
                    var dateOfLastPromotion = cmd.Parameters.Add("@date_of_last_promotion", NpgsqlDbType.Date);
                    var officialEmail = cmd.Parameters.Add("@official_email", NpgsqlDbType.Text);
                    var stateOfOrigin = cmd.Parameters.Add("@state_of_origin", NpgsqlDbType.Text);
                    var lgaOfOrigin = cmd.Parameters.Add("@lga_of_origin", NpgsqlDbType.Text);
                    var religion = cmd.Parameters.Add("@religion", NpgsqlDbType.Text);
                    var geoPoliticalRegion = cmd.Parameters.Add("@geo_political_region", NpgsqlDbType.Text);
                    var modifiedBy = cmd.Parameters.Add("@modified_by", NpgsqlDbType.Text);
                    var modifiedDate = cmd.Parameters.Add("@modified_date", NpgsqlDbType.Text);
                    var createdBy = cmd.Parameters.Add("@created_by", NpgsqlDbType.Text);
                    var createdDate = cmd.Parameters.Add("@created_date", NpgsqlDbType.Text);
                    var companyId = cmd.Parameters.Add("@coy_id", NpgsqlDbType.Text);
                    var departmentId = cmd.Parameters.Add("@dept_id", NpgsqlDbType.Integer);
                    var unitId = cmd.Parameters.Add("unit_id", NpgsqlDbType.Integer);
                    var locationId = cmd.Parameters.Add("@loc_id", NpgsqlDbType.Integer);
                    var yearsOfExperience = cmd.Parameters.Add("@yrs_of_experience", NpgsqlDbType.Integer);

                    cmd.Prepare();

                    employeeId.Value = employee.EmployeeID;
                    employeeNo2.Value = employee.EmployeeNo2 ?? (object)DBNull.Value;
                    companyId.Value = employee.CompanyID ?? (object)DBNull.Value;
                    departmentId.Value = employee.DepartmentID ?? (object)DBNull.Value;
                    unitId.Value = employee.UnitID ?? (object)DBNull.Value;
                    locationId.Value = employee.LocationID ?? (object)DBNull.Value;
                    startUpDate.Value = employee.StartUpDate ?? (object)DBNull.Value;
                    yearsOfExperience.Value = employee.YearsOfExperience ?? (object)DBNull.Value;
                    startUpDesignation.Value = employee.StartUpDesignation ?? (object)DBNull.Value;
                    placeOfEngagement.Value = employee.PlaceOfEngagement ?? (object)DBNull.Value;
                    confirmationDate.Value = employee.ConfirmationDate ?? (object)DBNull.Value;
                    currentDesignation.Value = employee.CurrentDesignation ?? (object)DBNull.Value;
                    jobGrade.Value = employee.JobGrade ?? (object)DBNull.Value;
                    employmentStatus.Value = employee.EmploymentStatus ?? (object)DBNull.Value;
                    dateOfLastPromotion.Value = employee.DateOfLastPromotion ?? (object)DBNull.Value;
                    officialEmail.Value = employee.OfficialEmail ?? (object)DBNull.Value;
                    stateOfOrigin.Value = employee.StateOfOrigin ?? (object)DBNull.Value;
                    lgaOfOrigin.Value = employee.LgaOfOrigin ?? (object)DBNull.Value;
                    religion.Value = employee.Religion ?? (object)DBNull.Value;
                    geoPoliticalRegion.Value = employee.GeoPoliticalRegion ?? (object)DBNull.Value;
                    modifiedBy.Value = employee.ModifiedBy ?? (object)DBNull.Value;
                    modifiedDate.Value = employee.ModifiedTime ?? (object)DBNull.Value;
                    createdBy.Value = employee.CreatedBy ?? (object)DBNull.Value;
                    createdDate.Value = employee.CreatedTime ?? (object)DBNull.Value;

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


        #endregion

        #region Employee Report Line Action Methods
        public async Task<bool> AddEmployeeReportLineAsync(EmployeeReportLine employeeReportLine)
        {
            int rows = 0;
            DateTime _startDate = DateTime.Now.Date;
            DateTime _endDate = new DateTime(2060, 12, 31);
            if(employeeReportLine.ReportStartDate != null)
            {
                _startDate = employeeReportLine.ReportStartDate.Value;
            }

            if (employeeReportLine.ReportEndDate != null)
            {
                _endDate = employeeReportLine.ReportEndDate.Value;
            }

            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.erm_emp_rpts(emp_id, rpt_emp_id,");
            sb.Append("rpt_sts, rpt_nds, team_id,  rpt_emp_rl, ");
            sb.Append("unit_id, dept_id, mdb, mdt, ctb, ctt, rpt_typ) ");
            sb.Append("VALUES (@emp_id, @rpt_emp_id, @rpt_sts, @rpt_nds, ");
            sb.Append("@team_id, @rpt_emp_rl, @unit_id, @dept_id, ");
            sb.Append("@mdb, @mdt, @ctb, @ctt, @rpt_typ); ");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                    var rpt_emp_id = cmd.Parameters.Add("@rpt_emp_id", NpgsqlDbType.Text);
                    var rpt_emp_rl = cmd.Parameters.Add("@rpt_emp_rl", NpgsqlDbType.Text);
                    var rpt_sts = cmd.Parameters.Add("@rpt_sts", NpgsqlDbType.Date);
                    var rpt_nds = cmd.Parameters.Add("@rpt_nds", NpgsqlDbType.Date);
                    var team_id = cmd.Parameters.Add("@team_id", NpgsqlDbType.Text);
                    var unit_id = cmd.Parameters.Add("@unit_id", NpgsqlDbType.Integer);
                    var dept_id = cmd.Parameters.Add("@dept_id", NpgsqlDbType.Integer);
                    var mdb = cmd.Parameters.Add("@mdb", NpgsqlDbType.Text);
                    var mdt = cmd.Parameters.Add("@mdt", NpgsqlDbType.Text);
                    var ctb = cmd.Parameters.Add("@ctb", NpgsqlDbType.Text);
                    var ctt = cmd.Parameters.Add("@ctt", NpgsqlDbType.Text);
                    var rpt_typ = cmd.Parameters.Add("@rpt_typ", NpgsqlDbType.Text);
                    cmd.Prepare();
                    emp_id.Value = employeeReportLine.EmployeeID;
                    rpt_emp_id.Value = employeeReportLine.ReportsToEmployeeID;
                    rpt_emp_rl.Value = employeeReportLine.ReportsToEmployeeRole ?? (object)DBNull.Value;
                    rpt_sts.Value = _startDate.Date;
                    rpt_nds.Value = _endDate.Date;
                    team_id.Value = employeeReportLine.TeamID ?? (object)DBNull.Value;
                    unit_id.Value = employeeReportLine.UnitID ?? (object)DBNull.Value;
                    dept_id.Value = employeeReportLine.DepartmentID ?? (object)DBNull.Value;
                    mdb.Value = employeeReportLine.ModifiedBy ?? (object)DBNull.Value;
                    mdt.Value = employeeReportLine.ModifiedTime ?? (object)DBNull.Value;
                    ctb.Value = employeeReportLine.CreatedBy ?? (object)DBNull.Value;
                    ctt.Value = employeeReportLine.CreatedTime ?? (object)DBNull.Value;
                    rpt_typ.Value = employeeReportLine.ReportingLineType ?? "SUPERVISOR";

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

        public async Task<bool> DeleteEmployeeReportLineAsync(int employeeReportId)
        {
            if (employeeReportId < 1) { throw new ArgumentNullException("Required parameter [EmployeeReportId] has an invalid value."); }
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = $"DELETE FROM public.erm_emp_rpts WHERE (rpt_id = @rpt_id);";
            try
            {
                await conn.OpenAsync();
                //Delete data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var rpt_id = cmd.Parameters.Add("@rpt_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    rpt_id.Value = employeeReportId;
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

        public async Task<bool> EditEmployeeReportLineAsync(EmployeeReportLine employeeReportLine)
        {
            if (employeeReportLine == null) { throw new ArgumentNullException(nameof(employeeReportLine), "The required parameter [EmployeeReportLine] is missing or has an invalid value."); }
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append($"UPDATE public.erm_emp_rpts	SET emp_id = @emp_id, rpt_emp_id = @rpt_emp_id, ");
            sb.Append($"rpt_emp_rl = @rpt_emp_rl, rpt_sts = @rpt_sts, rpt_nds = @rpt_nds, ");
            sb.Append($"team_id = @team_id, unit_id = @unit_id, dept_id = @dept_id, ");
            sb.Append($"mdb = @mdb, mdt = @mdt, rpt_typ = @rpt_typ ");
            sb.Append("WHERE (rpt_id = @rpt_id);  ");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var rpt_id = cmd.Parameters.Add("@rpt_id", NpgsqlDbType.Integer);
                    var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                    var rpt_emp_id = cmd.Parameters.Add("@rpt_emp_id", NpgsqlDbType.Text);
                    var rpt_emp_rl = cmd.Parameters.Add("@rpt_emp_rl", NpgsqlDbType.Text);
                    var rpt_sts = cmd.Parameters.Add("@rpt_sts", NpgsqlDbType.Date);
                    var rpt_nds = cmd.Parameters.Add("@rpt_nds", NpgsqlDbType.Date);
                    var team_id = cmd.Parameters.Add("@team_id", NpgsqlDbType.Text);
                    var unit_id = cmd.Parameters.Add("unit_id", NpgsqlDbType.Integer);
                    var dept_id = cmd.Parameters.Add("@dept_id", NpgsqlDbType.Integer);
                    var mdb = cmd.Parameters.Add("@mdb", NpgsqlDbType.Text);
                    var mdt = cmd.Parameters.Add("@mdt", NpgsqlDbType.Text);
                    var rpt_typ = cmd.Parameters.Add("@rpt_typ", NpgsqlDbType.Text);
                    cmd.Prepare();
                    rpt_id.Value = employeeReportLine.ReportingLineID;
                    emp_id.Value = employeeReportLine.EmployeeID;
                    rpt_emp_id.Value = employeeReportLine.ReportsToEmployeeID;
                    rpt_emp_rl.Value = employeeReportLine.ReportsToEmployeeRole;
                    rpt_sts.Value = employeeReportLine.ReportStartDate ?? (object)DBNull.Value;
                    rpt_nds.Value = employeeReportLine.ReportEndDate ?? (object)DBNull.Value;
                    team_id.Value = employeeReportLine.TeamID ?? (object)DBNull.Value;
                    unit_id.Value = employeeReportLine.UnitID ?? (object)DBNull.Value;
                    dept_id.Value = employeeReportLine.DepartmentID ?? (object)DBNull.Value;
                    mdb.Value = employeeReportLine.ModifiedBy ?? (object)DBNull.Value;
                    mdt.Value = employeeReportLine.ModifiedTime ?? (object)DBNull.Value;
                    rpt_typ.Value = employeeReportLine.ReportingLineType ?? "SUPERVISOR";

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

        public async Task<EmployeeReportLine> GetEmployeeReportLineByIdAsync(int employeeReportLineId)
        {
            if (employeeReportLineId < 1) { throw new ArgumentNullException(nameof(employeeReportLineId), "The required parameter [EmployeeReportLineId] is missing or has in invalid value."); }
            EmployeeReportLine employeeReportLine = new EmployeeReportLine();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append($"SELECT r.rpt_id, r.emp_id, r.rpt_emp_id, r.rpt_emp_rl, r.rpt_sts, r.rpt_typ, ");
            sb.Append($"r.rpt_nds, r.team_id, r.unit_id, r.dept_id, r.mdb, r.mdt, r.ctb, r.ctt, ");
            sb.Append($"p1.fullname AS emp_name, p2.fullname AS rpt_emp_name, t.tm_nm, u.unitname, ");
            sb.Append($"d.deptname FROM public.erm_emp_rpts r ");
            sb.Append($"INNER JOIN public.gst_prsns p1 ON r.emp_id = p1.id ");
            sb.Append($"INNER JOIN public.gst_prsns p2 ON r.rpt_emp_id = p2.id ");
            sb.Append($"LEFT OUTER JOIN public.gst_tms t ON r.team_id = t.tm_id ");
            sb.Append($"LEFT OUTER JOIN public.gst_units u ON r.unit_id = u.unitqk ");
            sb.Append($"LEFT OUTER JOIN public.gst_depts d ON r.dept_id = d.deptqk ");
            sb.Append($"WHERE (r.rpt_id = @rpt_id);");
            query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var rpt_id = cmd.Parameters.Add("@rpt_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    rpt_id.Value = employeeReportLineId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            employeeReportLine.ReportingLineID = reader["rpt_id"] == DBNull.Value ? 0 : (int)reader["rpt_id"];
                            employeeReportLine.ReportingLineType = reader["rpt_typ"] == DBNull.Value ? string.Empty : reader["rpt_typ"].ToString();
                            employeeReportLine.EmployeeID = reader["emp_id"] == DBNull.Value ? string.Empty : (reader["emp_id"]).ToString();
                            employeeReportLine.ReportsToEmployeeID = reader["rpt_emp_id"] == DBNull.Value ? string.Empty : (reader["rpt_emp_id"]).ToString();
                            employeeReportLine.ReportsToEmployeeRole = reader["rpt_emp_rl"] == DBNull.Value ? string.Empty : (reader["rpt_emp_rl"]).ToString();
                            employeeReportLine.ReportStartDate = reader["rpt_sts"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rpt_sts"];
                            employeeReportLine.ReportEndDate = reader["rpt_nds"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rpt_nds"];
                            employeeReportLine.TeamID = reader["team_id"] == DBNull.Value ? string.Empty : reader["team_id"].ToString();
                            employeeReportLine.UnitID = reader["unit_id"] == DBNull.Value ? 0 : (int)(reader["unit_id"]);
                            employeeReportLine.DepartmentID = reader["dept_id"] == DBNull.Value ? 0 : (int)(reader["dept_id"]);
                            employeeReportLine.ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString();
                            employeeReportLine.ModifiedTime = reader["mdt"] == DBNull.Value ? string.Empty : reader["mdt"].ToString();
                            employeeReportLine.CreatedTime = reader["ctt"] == DBNull.Value ? string.Empty : reader["ctt"].ToString();
                            employeeReportLine.CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString();
                            employeeReportLine.EmployeeName = reader["emp_name"] == DBNull.Value ? string.Empty : reader["emp_name"].ToString();
                            employeeReportLine.ReportsToEmployeeName = reader["rpt_emp_name"] == DBNull.Value ? string.Empty : reader["rpt_emp_name"].ToString();
                            employeeReportLine.TeamName = reader["tm_nm"] == DBNull.Value ? string.Empty : reader["tm_nm"].ToString();
                            employeeReportLine.UnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString();
                            employeeReportLine.DepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString();
                        }
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message, ex.InnerException);
            }
            return employeeReportLine;
        }

        public async Task<IList<EmployeeReportLine>> GetEmployeeReportLinesByEmployeeIdAsync(string employeeId)
        {
            if (String.IsNullOrEmpty(employeeId)) { throw new ArgumentNullException("The required parameter [EmployeeID] has an invalid value."); }
            List<EmployeeReportLine> employeeReportLineList = new List<EmployeeReportLine>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append($"SELECT r.rpt_id, r.emp_id, r.rpt_emp_id, r.rpt_emp_rl, r.rpt_sts, r.rpt_typ, ");
            sb.Append($"r.rpt_nds, r.team_id, r.unit_id, r.dept_id, r.mdb, r.mdt, r.ctb, r.ctt, ");
            sb.Append($"p1.fullname AS emp_name, p2.fullname AS rpt_emp_name, t.tm_nm, u.unitname, ");
            sb.Append($"d.deptname FROM public.erm_emp_rpts r ");
            sb.Append($"INNER JOIN public.gst_prsns p1 ON r.emp_id = p1.id ");
            sb.Append($"INNER JOIN public.gst_prsns p2 ON r.rpt_emp_id = p2.id ");
            sb.Append($"LEFT OUTER JOIN public.gst_tms t ON r.team_id = t.tm_id ");
            sb.Append($"LEFT OUTER JOIN public.gst_units u ON r.unit_id = u.unitqk ");
            sb.Append($"LEFT OUTER JOIN public.gst_depts d ON r.dept_id = d.deptqk ");
            sb.Append($"WHERE LOWER(r.emp_id) = LOWER(@emp_id);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    emp_id.Value = employeeId;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        employeeReportLineList.Add(new EmployeeReportLine()
                        {
                            ReportingLineID = reader["rpt_id"] == DBNull.Value ? 0 : (int)reader["rpt_id"],
                            EmployeeID = reader["emp_id"] == DBNull.Value ? string.Empty : (reader["emp_id"]).ToString(),
                            ReportingLineType = reader["rpt_typ"] == DBNull.Value ? string.Empty : reader["rpt_typ"].ToString(),
                            ReportsToEmployeeID = reader["rpt_emp_id"] == DBNull.Value ? string.Empty : (reader["rpt_emp_id"]).ToString(),
                            ReportsToEmployeeRole = reader["rpt_emp_rl"] == DBNull.Value ? string.Empty : (reader["rpt_emp_rl"]).ToString(),
                            ReportStartDate = reader["rpt_sts"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rpt_sts"],
                            ReportEndDate = reader["rpt_nds"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rpt_nds"],
                            TeamID = reader["team_id"] == DBNull.Value ? string.Empty : reader["team_id"].ToString(),
                            UnitID = reader["unit_id"] == DBNull.Value ? 0 : (int)(reader["unit_id"]),
                            DepartmentID = reader["dept_id"] == DBNull.Value ? 0 : (int)(reader["dept_id"]),
                            ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                            ModifiedTime = reader["mdt"] == DBNull.Value ? string.Empty : reader["mdt"].ToString(),
                            CreatedTime = reader["ctt"] == DBNull.Value ? string.Empty : reader["ctt"].ToString(),
                            CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                            EmployeeName = reader["emp_name"] == DBNull.Value ? string.Empty : reader["emp_name"].ToString(),
                            ReportsToEmployeeName = reader["rpt_emp_name"] == DBNull.Value ? string.Empty : reader["rpt_emp_name"].ToString(),
                            TeamName = reader["tm_nm"] == DBNull.Value ? string.Empty : reader["tm_nm"].ToString(),
                            UnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                            DepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                        });
                    }
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message, ex.InnerException);
            }
            return employeeReportLineList;
        }

        public async Task<IList<EmployeeReportLine>> GetActiveEmployeeReportLinesByEmployeeIdAsync(string employeeId)
        {
            if (String.IsNullOrEmpty(employeeId)) { throw new ArgumentNullException("The required parameter [EmployeeID] has an invalid value."); }
            List<EmployeeReportLine> employeeReportLineList = new List<EmployeeReportLine>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT r.rpt_id, r.emp_id, r.rpt_emp_id, r.rpt_emp_rl, ");
            sb.Append("r.rpt_sts, r.rpt_typ, r.rpt_nds, r.team_id, ");
            sb.Append("r.unit_id, r.dept_id, r.mdb, r.mdt, r.ctb, r.ctt, ");
            sb.Append("p1.fullname AS emp_name, p2.fullname AS rpt_emp_name, ");
            sb.Append("t.tm_nm, u.unitname, d.deptname FROM public.erm_emp_rpts r ");
            sb.Append("INNER JOIN public.gst_prsns p1 ON r.emp_id = p1.id ");
            sb.Append("INNER JOIN public.gst_prsns p2 ON r.rpt_emp_id = p2.id ");
            sb.Append("LEFT OUTER JOIN public.gst_tms t ON r.team_id = t.tm_id ");
            sb.Append("LEFT OUTER JOIN public.gst_units u ON r.unit_id = u.unitqk ");
            sb.Append("LEFT OUTER JOIN public.gst_depts d ON r.dept_id = d.deptqk ");
            sb.Append("WHERE LOWER(r.emp_id) = LOWER(@emp_id) ");
            sb.Append("AND (r.rpt_nds >= CURRENT_DATE); ");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    emp_id.Value = employeeId;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        employeeReportLineList.Add(new EmployeeReportLine()
                        {
                            ReportingLineID = reader["rpt_id"] == DBNull.Value ? 0 : (int)reader["rpt_id"],
                            EmployeeID = reader["emp_id"] == DBNull.Value ? string.Empty : (reader["emp_id"]).ToString(),
                            ReportingLineType = reader["rpt_typ"] == DBNull.Value ? string.Empty : reader["rpt_typ"].ToString(),
                            ReportsToEmployeeID = reader["rpt_emp_id"] == DBNull.Value ? string.Empty : (reader["rpt_emp_id"]).ToString(),
                            ReportsToEmployeeRole = reader["rpt_emp_rl"] == DBNull.Value ? string.Empty : (reader["rpt_emp_rl"]).ToString(),
                            ReportStartDate = reader["rpt_sts"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rpt_sts"],
                            ReportEndDate = reader["rpt_nds"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rpt_nds"],
                            TeamID = reader["team_id"] == DBNull.Value ? string.Empty : reader["team_id"].ToString(),
                            UnitID = reader["unit_id"] == DBNull.Value ? 0 : (int)(reader["unit_id"]),
                            DepartmentID = reader["dept_id"] == DBNull.Value ? 0 : (int)(reader["dept_id"]),
                            ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                            ModifiedTime = reader["mdt"] == DBNull.Value ? string.Empty : reader["mdt"].ToString(),
                            CreatedTime = reader["ctt"] == DBNull.Value ? string.Empty : reader["ctt"].ToString(),
                            CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                            EmployeeName = reader["emp_name"] == DBNull.Value ? string.Empty : reader["emp_name"].ToString(),
                            ReportsToEmployeeName = reader["rpt_emp_name"] == DBNull.Value ? string.Empty : reader["rpt_emp_name"].ToString(),
                            TeamName = reader["tm_nm"] == DBNull.Value ? string.Empty : reader["tm_nm"].ToString(),
                            UnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                            DepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                        });
                    }
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message, ex.InnerException);
            }
            return employeeReportLineList;
        }

        public async Task<IList<EmployeeReportLine>> GetEmployeeReportsByReportsToEmployeeIdAsync(string reportsToEmployeeId)
        {
            if (String.IsNullOrEmpty(reportsToEmployeeId)) { throw new ArgumentNullException("The required parameter [ReportsToEmployeeID] has an invalid value."); }
            List<EmployeeReportLine> employeeReportLineList = new List<EmployeeReportLine>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append($"SELECT r.rpt_id, r.emp_id, r.rpt_emp_id, r.rpt_emp_rl, r.rpt_sts, r.rpt_typ, ");
            sb.Append($"r.rpt_nds, r.team_id, r.unit_id, r.dept_id, r.mdb, r.mdt, r.ctb, r.ctt, ");
            sb.Append($"p1.fullname AS emp_name, p2.fullname AS rpt_emp_name, t.tm_nm, u.unitname, ");
            sb.Append($"d.deptname FROM public.erm_emp_rpts r ");
            sb.Append($"INNER JOIN public.gst_prsns p1 ON r.emp_id = p1.id ");
            sb.Append($"INNER JOIN public.gst_prsns p2 ON r.rpt_emp_id = p2.id ");
            sb.Append($"LEFT OUTER JOIN public.gst_tms t ON r.team_id = t.tm_id ");
            sb.Append($"LEFT OUTER JOIN public.gst_units u ON r.unit_id = u.unitqk ");
            sb.Append($"LEFT OUTER JOIN public.gst_depts d ON r.dept_id = d.deptqk ");
            sb.Append($"WHERE LOWER(r.rpt_emp_id) = LOWER(@rpt_emp_id);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var rpt_emp_id = cmd.Parameters.Add("@rpt_emp_id", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    rpt_emp_id.Value = reportsToEmployeeId;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        employeeReportLineList.Add(new EmployeeReportLine()
                        {
                            ReportingLineID = reader["rpt_id"] == DBNull.Value ? 0 : (int)reader["rpt_id"],
                            EmployeeID = reader["emp_id"] == DBNull.Value ? string.Empty : (reader["emp_id"]).ToString(),
                            ReportingLineType = reader["rpt_typ"] == DBNull.Value ? string.Empty : reader["rpt_typ"].ToString(),
                            ReportsToEmployeeID = reader["rpt_emp_id"] == DBNull.Value ? string.Empty : (reader["rpt_emp_id"]).ToString(),
                            ReportsToEmployeeRole = reader["rpt_emp_rl"] == DBNull.Value ? string.Empty : (reader["rpt_emp_rl"]).ToString(),
                            ReportStartDate = reader["rpt_sts"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rpt_sts"],
                            ReportEndDate = reader["rpt_nds"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rpt_nds"],
                            TeamID = reader["team_id"] == DBNull.Value ? string.Empty : reader["team_id"].ToString(),
                            UnitID = reader["unit_id"] == DBNull.Value ? 0 : (int)(reader["unit_id"]),
                            DepartmentID = reader["dept_id"] == DBNull.Value ? 0 : (int)(reader["dept_id"]),
                            ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                            ModifiedTime = reader["mdt"] == DBNull.Value ? string.Empty : reader["mdt"].ToString(),
                            CreatedTime = reader["ctt"] == DBNull.Value ? string.Empty : reader["ctt"].ToString(),
                            CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                            EmployeeName = reader["emp_name"] == DBNull.Value ? string.Empty : reader["emp_name"].ToString(),
                            ReportsToEmployeeName = reader["rpt_emp_name"] == DBNull.Value ? string.Empty : reader["rpt_emp_name"].ToString(),
                            TeamName = reader["tm_nm"] == DBNull.Value ? string.Empty : reader["tm_nm"].ToString(),
                            UnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                            DepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                        });
                    }
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message, ex.InnerException);
            }
            return employeeReportLineList;
        }

        public async Task<IList<EmployeeReportLine>> GetActiveEmployeeReportsByReportsToEmployeeIdAsync(string reportsToEmployeeId)
        {
            if (String.IsNullOrEmpty(reportsToEmployeeId)) { throw new ArgumentNullException("The required parameter [ReportsToEmployeeID] has an invalid value."); }
            List<EmployeeReportLine> employeeReportLineList = new List<EmployeeReportLine>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append($"SELECT r.rpt_id, r.emp_id, r.rpt_emp_id, r.rpt_emp_rl, r.rpt_sts, r.rpt_typ, ");
            sb.Append($"r.rpt_nds, r.team_id, r.unit_id, r.dept_id, r.mdb, r.mdt, r.ctb, r.ctt, ");
            sb.Append($"p1.fullname AS emp_name, p2.fullname AS rpt_emp_name, t.tm_nm, u.unitname, ");
            sb.Append($"d.deptname FROM public.erm_emp_rpts r ");
            sb.Append($"INNER JOIN public.gst_prsns p1 ON r.emp_id = p1.id ");
            sb.Append($"INNER JOIN public.gst_prsns p2 ON r.rpt_emp_id = p2.id ");
            sb.Append($"LEFT OUTER JOIN public.gst_tms t ON r.team_id = t.tm_id ");
            sb.Append($"LEFT OUTER JOIN public.gst_units u ON r.unit_id = u.unitqk ");
            sb.Append($"LEFT OUTER JOIN public.gst_depts d ON r.dept_id = d.deptqk ");
            sb.Append($"WHERE LOWER(r.rpt_emp_id) = LOWER(@rpt_emp_id) AND (r.rpt_nds IS NULL);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var rpt_emp_id = cmd.Parameters.Add("@rpt_emp_id", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    rpt_emp_id.Value = reportsToEmployeeId;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        employeeReportLineList.Add(new EmployeeReportLine()
                        {
                            ReportingLineID = reader["rpt_id"] == DBNull.Value ? 0 : (int)reader["rpt_id"],
                            EmployeeID = reader["emp_id"] == DBNull.Value ? string.Empty : (reader["emp_id"]).ToString(),
                            ReportingLineType = reader["rpt_typ"] == DBNull.Value ? string.Empty : reader["rpt_typ"].ToString(),
                            ReportsToEmployeeID = reader["rpt_emp_id"] == DBNull.Value ? string.Empty : (reader["rpt_emp_id"]).ToString(),
                            ReportsToEmployeeRole = reader["rpt_emp_rl"] == DBNull.Value ? string.Empty : (reader["rpt_emp_rl"]).ToString(),
                            ReportStartDate = reader["rpt_sts"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rpt_sts"],
                            ReportEndDate = reader["rpt_nds"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rpt_nds"],
                            TeamID = reader["team_id"] == DBNull.Value ? string.Empty : reader["team_id"].ToString(),
                            UnitID = reader["unit_id"] == DBNull.Value ? 0 : (int)(reader["unit_id"]),
                            DepartmentID = reader["dept_id"] == DBNull.Value ? 0 : (int)(reader["dept_id"]),
                            ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                            ModifiedTime = reader["mdt"] == DBNull.Value ? string.Empty : reader["mdt"].ToString(),
                            CreatedTime = reader["ctt"] == DBNull.Value ? string.Empty : reader["ctt"].ToString(),
                            CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                            EmployeeName = reader["emp_name"] == DBNull.Value ? string.Empty : reader["emp_name"].ToString(),
                            ReportsToEmployeeName = reader["rpt_emp_name"] == DBNull.Value ? string.Empty : reader["rpt_emp_name"].ToString(),
                            TeamName = reader["tm_nm"] == DBNull.Value ? string.Empty : reader["tm_nm"].ToString(),
                            UnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                            DepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                        });
                    }
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message, ex.InnerException);
            }
            return employeeReportLineList;
        }

        #endregion
    }
}
