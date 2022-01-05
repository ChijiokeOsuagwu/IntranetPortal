using IntranetPortal.Base.Models.EmployeeRecordModels;
using IntranetPortal.Base.Repositories.EmployeeRecordRepositories;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Data.Repositories.EmployeeRecordRepositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        public IConfiguration _config { get; }
        public EmployeeRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        //=============================== Employee Action Methods =====================================//
        #region Employee Action Methods
        public async Task<bool> AddEmployeeAsync(Employee employee)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append($"INSERT INTO public.erm_emps(emp_id, emp_no_1, emp_no_2, dept_id, unit_id, loc_id, start_up_date, ");
            sb.Append($"yrs_of_experience, length_of_service, start_up_designation, place_of_engagement, confirmation_date, ");
            sb.Append($"current_designation, job_grade, reports_to_1, reports_to_2, reports_to_3, employment_status, date_of_last_promotion, ");
            sb.Append($"official_email, state_of_origin, lga_of_origin, religion, geo_political_region, coy_id, ");
            sb.Append($"next_of_kin_name, next_of_kin_relationship, modified_by, modified_date, created_by, created_date) ");
            sb.Append($"VALUES (@emp_id, @emp_no_1, @emp_no_2, @dept_id, @unit_id, @loc_id, @start_up_date, @yrs_of_experience, ");
            sb.Append($"@length_of_service, @start_up_designation, @place_of_engagement, @confirmation_date, @current_designation, ");
            sb.Append($"@job_grade, @reports_to_1, @reports_to_2, @reports_to_3, @employment_status, @date_of_last_promotion, ");
            sb.Append($"@official_email, @state_of_origin, @lga_of_origin, @religion, @geo_political_region, @coy_id, ");
            sb.Append($"@next_of_kin_name, @next_of_kin_relationship, @modified_by, @modified_date, @created_by, @created_date);");
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
                    var companyId = cmd.Parameters.Add("@coy_id", NpgsqlDbType.Text);
                    var departmentId = cmd.Parameters.Add("@dept_id", NpgsqlDbType.Text);
                    var unitId = cmd.Parameters.Add("unit_id", NpgsqlDbType.Text);
                    var locationId = cmd.Parameters.Add("@loc_id", NpgsqlDbType.Text);
                    var startUpDate = cmd.Parameters.Add("@start_up_date", NpgsqlDbType.Date);
                    var yearsOfExperience = cmd.Parameters.Add("@yrs_of_experience", NpgsqlDbType.Integer);
                    var lengthOfService = cmd.Parameters.Add("@length_of_service", NpgsqlDbType.Integer);
                    var startUpDesignation = cmd.Parameters.Add("@start_up_designation", NpgsqlDbType.Text);
                    var placeOfEngagement = cmd.Parameters.Add("@place_of_engagement", NpgsqlDbType.Text);
                    var confirmationDate = cmd.Parameters.Add("@confirmation_date", NpgsqlDbType.Date);
                    var currentDesignation = cmd.Parameters.Add("@current_designation", NpgsqlDbType.Text);
                    var jobGrade = cmd.Parameters.Add("@job_grade", NpgsqlDbType.Text);
                    var reportsTo1 = cmd.Parameters.Add("@reports_to_1", NpgsqlDbType.Text);
                    var reportsTo2 = cmd.Parameters.Add("@reports_to_2", NpgsqlDbType.Text);
                    var reportsTo3 = cmd.Parameters.Add("@reports_to_3", NpgsqlDbType.Text);
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
                    lengthOfService.Value = employee.LengthOfService ?? (object)DBNull.Value;
                    startUpDesignation.Value = employee.StartUpDesignation ?? (object)DBNull.Value;
                    placeOfEngagement.Value = employee.PlaceOfEngagement ?? (object)DBNull.Value;
                    confirmationDate.Value = employee.ConfirmationDate ?? (object)DBNull.Value;
                    currentDesignation.Value = employee.CurrentDesignation ?? (object)DBNull.Value;
                    jobGrade.Value = employee.JobGrade ?? (object)DBNull.Value;
                    reportsTo1.Value = employee.ReportsTo1_EmployeeID ?? (object)DBNull.Value;
                    reportsTo2.Value = employee.ReportsTo2_EmployeeID ?? (object)DBNull.Value;
                    reportsTo3.Value = employee.ReportsTo3_EmployeeID ?? (object)DBNull.Value;
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

        public async Task<bool> DeleteEmployeeAsync(string Id)
        {
            if (string.IsNullOrEmpty(Id)) { return false; }
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = $"DELETE FROM public.erm_emps WHERE (emp_id = @emp_id);";
            try
            {
                await conn.OpenAsync();
                //Delete data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var employeeId = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                    cmd.Prepare();
                    employeeId.Value = Id;
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
            sb.Append($"UPDATE public.erm_emps SET emp_no_2=@emp_no_2, dept_id=@dept_id, unit_id=@unit_id, coy_id=@coy_id");
            sb.Append($"loc_id=@loc_id, start_up_date=@start_up_date, yrs_of_experience=@yrs_of_experience, length_of_service=@length_of_service, ");
            sb.Append($"start_up_designation=@start_up_designation, place_of_engagement=@place_of_engagement, confirmation_date=@confirmation_date, ");
            sb.Append($"current_designation=@current_designation, job_grade=@job_grade, reports_to_1=@reports_to_1, reports_to_2=@reports_to_2, ");
            sb.Append($"reports_to_3=@reports_to_3, employment_status=@employment_status, date_of_last_promotion=@date_of_last_promotion, ");
            sb.Append($"official_email=@official_email, state_of_origin=@state_of_origin, lga_of_origin=@lga_of_origin, ");
            sb.Append($"religion=@religion, geo_political_region=@geo_political_region, next_of_kin_name=@next_of_kin_name, ");
            sb.Append($"next_of_kin_relationship=@next_of_kin_relationship, modified_by=@modified_by, modified_date=@modified_date, ");
            sb.Append($"next_of_kin_address=@next_of_kin_address, next_of_kin_phone=@next_of_kin_phone, next_of_kin_email=@next_of_kin_email ");
            sb.Append($"WHERE (emp_id = @emp_id);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var employeeId = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                    var employeeNo2 = cmd.Parameters.Add("@emp_no_2", NpgsqlDbType.Text);
                    var companyId = cmd.Parameters.Add("@coy_id", NpgsqlDbType.Text);
                    var departmentId = cmd.Parameters.Add("@dept_id", NpgsqlDbType.Integer);
                    var unitId = cmd.Parameters.Add("unit_id", NpgsqlDbType.Integer);
                    var locationId = cmd.Parameters.Add("@loc_id", NpgsqlDbType.Integer);
                    var startUpDate = cmd.Parameters.Add("@start_up_date", NpgsqlDbType.Date);
                    var yearsOfExperience = cmd.Parameters.Add("@yrs_of_experience", NpgsqlDbType.Integer);
                    var lengthOfService = cmd.Parameters.Add("@length_of_service", NpgsqlDbType.Integer);
                    var startUpDesignation = cmd.Parameters.Add("@start_up_designation", NpgsqlDbType.Text);
                    var placeOfEngagement = cmd.Parameters.Add("@place_of_engagement", NpgsqlDbType.Text);
                    var confirmationDate = cmd.Parameters.Add("@confirmation_date", NpgsqlDbType.Date);
                    var currentDesignation = cmd.Parameters.Add("@current_designation", NpgsqlDbType.Text);
                    var jobGrade = cmd.Parameters.Add("@job_grade", NpgsqlDbType.Text);
                    var reportsTo1 = cmd.Parameters.Add("@reports_to_1", NpgsqlDbType.Text);
                    var reportsTo2 = cmd.Parameters.Add("@reports_to_2", NpgsqlDbType.Text);
                    var reportsTo3 = cmd.Parameters.Add("@reports_to_3", NpgsqlDbType.Text);
                    var employmentStatus = cmd.Parameters.Add("@employment_status", NpgsqlDbType.Text);
                    var dateOfLastPromotion = cmd.Parameters.Add("@date_of_last_promotion", NpgsqlDbType.Date);
                    var dateOfBirth = cmd.Parameters.Add("@date_of_birth", NpgsqlDbType.Date);
                    var officialEmail = cmd.Parameters.Add("@official_email", NpgsqlDbType.Text);
                    var maritalStatus = cmd.Parameters.Add("@marital_status", NpgsqlDbType.Text);
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

                    cmd.Prepare();
                    employeeId.Value = employee.EmployeeID;
                    employeeNo2.Value = employee.EmployeeNo2 ?? (object)DBNull.Value;
                    companyId.Value = employee.CompanyID ?? (object)DBNull.Value;
                    departmentId.Value = employee.DepartmentID ?? (object)DBNull.Value;
                    unitId.Value = employee.UnitID ?? (object)DBNull.Value;
                    locationId.Value = employee.LocationID ?? (object)DBNull.Value;
                    startUpDate.Value = employee.StartUpDate ?? (object)DBNull.Value;
                    yearsOfExperience.Value = employee.YearsOfExperience ?? (object)DBNull.Value;
                    lengthOfService.Value = employee.LengthOfService ?? (object)DBNull.Value;
                    startUpDesignation.Value = employee.StartUpDesignation ?? (object)DBNull.Value;
                    placeOfEngagement.Value = employee.PlaceOfEngagement ?? (object)DBNull.Value;
                    confirmationDate.Value = employee.ConfirmationDate ?? (object)DBNull.Value;
                    currentDesignation.Value = employee.CurrentDesignation ?? (object)DBNull.Value;
                    jobGrade.Value = employee.JobGrade ?? (object)DBNull.Value;
                    reportsTo1.Value = employee.ReportsTo1_EmployeeID ?? (object)DBNull.Value;
                    reportsTo2.Value = employee.ReportsTo2_EmployeeID ?? (object)DBNull.Value;
                    reportsTo3.Value = employee.ReportsTo3_EmployeeID ?? (object)DBNull.Value;
                    employmentStatus.Value = employee.EmploymentStatus ?? (object)DBNull.Value;
                    dateOfLastPromotion.Value = employee.DateOfLastPromotion ?? (object)DBNull.Value;
                    dateOfBirth.Value = employee.DateOfBirth ?? (object)DBNull.Value;
                    officialEmail.Value = employee.OfficialEmail ?? (object)DBNull.Value;
                    maritalStatus.Value = employee.MaritalStatus ?? (object)DBNull.Value;
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

        public async Task<Employee> GetEmployeeByIdAsync(string employeeId)
        {
            if (String.IsNullOrEmpty(employeeId)) { throw new ArgumentNullException(nameof(employeeId), "The required parameter [employeeId] is missing or has in invalid value."); }
            Employee employee = new Employee();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append($"SELECT emp_id, emp_no_1, emp_no_2, coy_id, dept_id, unit_id, loc_id, start_up_date, yrs_of_experience, ");
            sb.Append($"length_of_service, start_up_designation, place_of_engagement, confirmation_date, current_designation, job_grade, ");
            sb.Append($"reports_to_1, reports_to_2, reports_to_3, employment_status, date_of_last_promotion, official_email, ");
            sb.Append($"state_of_origin, lga_of_origin, religion, geo_political_region, next_of_kin_name, next_of_kin_relationship, ");
            sb.Append($"modified_by, modified_date, created_by, created_date, next_of_kin_address, next_of_kin_phone, next_of_kin_email, ");
            sb.Append($"id, title, sname, fname, oname, fullname, sex, phone1, phone2, email, address, mdb, mdt, ctb, ctt, imgp, ");
            sb.Append($"birthday, birthmonth, birthyear, maritalstatus FROM public.erm_emps e ");
            sb.Append($"INNER JOIN gst_prsns p ON e.emp_id = p.id WHERE(LOWER(emp_id) = LOWER(@emp_id));");
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
                            employee.CompanyID = reader["coy_id"] == DBNull.Value ? string.Empty : (reader["coy_id"]).ToString();
                            employee.DepartmentID = reader["dept_id"] == DBNull.Value ? 0 : (int)(reader["dept_id"]);
                            employee.UnitID = reader["unit_id"] == DBNull.Value ? 0 : (int)(reader["unit_id"]);
                            employee.LocationID = reader["loc_id"] == DBNull.Value ? 0 : (int)(reader["loc_id"]);
                            employee.StartUpDate = reader["start_up_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["start_up_date"];
                            employee.YearsOfExperience = reader["yrs_of_experience"] == DBNull.Value ? 0 : (int)reader["yrs_of_experience"];
                            employee.LengthOfService = reader["length_of_service"] == DBNull.Value ? 0 : (int)reader["length_of_service"];
                            employee.StartUpDesignation = reader["start_up_designation"] == DBNull.Value ? String.Empty : reader["start_up_designation"].ToString();
                            employee.PlaceOfEngagement = reader["place_of_engagement"] == DBNull.Value ? String.Empty : reader["place_of_engagement"].ToString();
                            employee.ConfirmationDate = reader["confirmation_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["confirmation_date"];
                            employee.CurrentDesignation = reader["current_designation"] == DBNull.Value ? String.Empty : reader["current_designation"].ToString();
                            employee.JobGrade = reader["job_grade"] == DBNull.Value ? String.Empty : reader["job_grade"].ToString();
                            employee.ReportsTo1_EmployeeID = reader["reports_to_1"] == DBNull.Value ? String.Empty : reader["reports_to_1"].ToString();
                            employee.ReportsTo2_EmployeeID = reader["reports_to_2"] == DBNull.Value ? String.Empty : reader["reports_to_2"].ToString();
                            employee.ReportsTo3_EmployeeID = reader["reports_to_3"] == DBNull.Value ? String.Empty : reader["reports_to_3"].ToString();
                            employee.EmploymentStatus = reader["employment_status"] == DBNull.Value ? String.Empty : reader["employment_status"].ToString();
                            employee.DateOfLastPromotion = reader["date_of_last_promotion"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["date_of_last_promotion"];
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
                            employee.EmployeeModifiedBy = reader["modified_by"] == DBNull.Value ? string.Empty : reader["modified_by"].ToString();
                            employee.EmployeeModifiedDate = reader["modified_date"] == DBNull.Value ? string.Empty : reader["modified_date"].ToString();
                            employee.EmployeeCreatedBy = reader["created_by"] == DBNull.Value ? string.Empty : reader["created_by"].ToString();
                            employee.EmployeeCreatedDate = reader["created_date"] == DBNull.Value ? string.Empty : reader["created_date"].ToString();

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
                            employee.Email = reader["email"] == DBNull.Value ? string.Empty : reader["email"].ToString();
                            employee.Address = reader["address"] == DBNull.Value ? string.Empty : reader["address"].ToString();
                            employee.ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString();
                            employee.ModifiedTime = reader["mdt"] == DBNull.Value ? string.Empty : reader["mdt"].ToString();
                            employee.CreatedTime = reader["ctt"] == DBNull.Value ? string.Empty : reader["ctt"].ToString();
                            employee.CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString();
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

            sb.Append($"SELECT emp_id, emp_no_1, emp_no_2, coy_id, dept_id, unit_id, loc_id, start_up_date, yrs_of_experience, ");
            sb.Append($"length_of_service, start_up_designation, place_of_engagement, confirmation_date, current_designation, job_grade, ");
            sb.Append($"reports_to_1, reports_to_2, reports_to_3, employment_status, date_of_last_promotion, official_email, ");
            sb.Append($"state_of_origin, lga_of_origin, religion, geo_political_region, next_of_kin_name, next_of_kin_relationship, ");
            sb.Append($"modified_by, modified_date, created_by, created_date, next_of_kin_address, next_of_kin_phone, next_of_kin_email, ");
            sb.Append($"id, title, sname, fname, oname, fullname, sex, phone1, phone2, email, address, mdb, mdt, ctb, ctt, imgp, ");
            sb.Append($"birthday, birthmonth, birthyear, maritalstatus FROM public.erm_emps e ");
            sb.Append($"INNER JOIN gst_prsns p ON e.emp_id = p.id ORDER BY fname;");
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
                            CompanyID = reader["coy_id"] == DBNull.Value ? string.Empty : (reader["coy_id"]).ToString(),
                            EmployeeNo2 = reader["emp_no_2"] == DBNull.Value ? string.Empty : (reader["emp_no_2"]).ToString(),
                            DepartmentID = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                            UnitID = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                            LocationID = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                            StartUpDate = reader["start_up_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["start_up_date"],
                            YearsOfExperience = reader["yrs_of_experience"] == DBNull.Value ? 0 : (int)reader["yrs_of_experience"],
                            LengthOfService = reader["length_of_service"] == DBNull.Value ? 0 : (int)reader["length_of_service"],
                            StartUpDesignation = reader["start_up_designation"] == DBNull.Value ? String.Empty : reader["start_up_designation"].ToString(),
                            PlaceOfEngagement = reader["place_of_engagement"] == DBNull.Value ? String.Empty : reader["place_of_engagement"].ToString(),
                            ConfirmationDate = reader["confirmation_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["confirmation_date"],
                            CurrentDesignation = reader["current_designation"] == DBNull.Value ? String.Empty : reader["current_designation"].ToString(),
                            JobGrade = reader["job_grade"] == DBNull.Value ? String.Empty : reader["job_grade"].ToString(),
                            ReportsTo1_EmployeeID = reader["reports_to_1"] == DBNull.Value ? String.Empty : reader["reports_to_1"].ToString(),
                            ReportsTo2_EmployeeID = reader["reports_to_2"] == DBNull.Value ? String.Empty : reader["reports_to_2"].ToString(),
                            ReportsTo3_EmployeeID = reader["reports_to_3"] == DBNull.Value ? String.Empty : reader["reports_to_3"].ToString(),
                            EmploymentStatus = reader["employment_status"] == DBNull.Value ? String.Empty : reader["employment_status"].ToString(),
                            DateOfLastPromotion = reader["date_of_last_promotion"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["date_of_last_promotion"],
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
                            EmployeeModifiedBy = reader["modified_by"] == DBNull.Value ? string.Empty : reader["modified_by"].ToString(),
                            EmployeeModifiedDate = reader["modified_date"] == DBNull.Value ? string.Empty : reader["modified_date"].ToString(),
                            EmployeeCreatedBy = reader["created_by"] == DBNull.Value ? string.Empty : reader["created_by"].ToString(),
                            EmployeeCreatedDate = reader["created_date"] == DBNull.Value ? string.Empty : reader["created_date"].ToString(),

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
                            Email = reader["email"] == DBNull.Value ? string.Empty : reader["email"].ToString(),
                            Address = reader["address"] == DBNull.Value ? string.Empty : reader["address"].ToString(),
                            ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                            ModifiedTime = reader["mdt"] == DBNull.Value ? string.Empty : reader["mdt"].ToString(),
                            CreatedTime = reader["ctt"] == DBNull.Value ? string.Empty : reader["ctt"].ToString(),
                            CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
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
            if (String.IsNullOrEmpty(employeeName)) { return null; }
            List<Employee> employeeList = new List<Employee>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append($"SELECT emp_id, emp_no_1, emp_no_2, dept_id, unit_id, loc_id, coy_id, start_up_date, yrs_of_experience, ");
            sb.Append($"length_of_service, start_up_designation, place_of_engagement, confirmation_date, current_designation, job_grade, ");
            sb.Append($"reports_to_1, reports_to_2, reports_to_3, employment_status, date_of_last_promotion, official_email, ");
            sb.Append($"state_of_origin, lga_of_origin, religion, geo_political_region, next_of_kin_name, next_of_kin_relationship, ");
            sb.Append($"modified_by, modified_date, created_by, created_date, next_of_kin_address, next_of_kin_phone, next_of_kin_email, ");
            sb.Append($"id, title, sname, fname, oname, fullname, sex, phone1, phone2, email, address, mdb, mdt, ctb, ctt, imgp, ");
            sb.Append($"birthday, birthmonth, birthyear, maritalstatus FROM public.erm_emps e ");
            sb.Append($"INNER JOIN gst_prsns p ON e.emp_id = p.id  WHERE(LOWER(fullname) LIKE '%'||LOWER(@fullname)||'%') ORDER BY fname;");
            query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var fullName = cmd.Parameters.Add("@fullname", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    fullName.Value = employeeName;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        employeeList.Add(new Employee()
                        {
                            EmployeeID = reader["emp_id"] == DBNull.Value ? string.Empty : (reader["emp_id"]).ToString(),
                            EmployeeNo1 = reader["emp_no_1"] == DBNull.Value ? string.Empty : (reader["emp_no_1"]).ToString(),
                            EmployeeNo2 = reader["emp_no_2"] == DBNull.Value ? string.Empty : (reader["emp_no_2"]).ToString(),
                            CompanyID = reader["coy_id"] == DBNull.Value ? string.Empty : (reader["coy_id"]).ToString(),
                            DepartmentID = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                            UnitID = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                            LocationID = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                            StartUpDate = reader["start_up_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["start_up_date"],
                            YearsOfExperience = reader["yrs_of_experience"] == DBNull.Value ? 0 : (int)reader["yrs_of_experience"],
                            LengthOfService = reader["length_of_service"] == DBNull.Value ? 0 : (int)reader["length_of_service"],
                            StartUpDesignation = reader["start_up_designation"] == DBNull.Value ? String.Empty : reader["start_up_designation"].ToString(),
                            PlaceOfEngagement = reader["place_of_engagement"] == DBNull.Value ? String.Empty : reader["place_of_engagement"].ToString(),
                            ConfirmationDate = reader["confirmation_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["confirmation_date"],
                            CurrentDesignation = reader["current_designation"] == DBNull.Value ? String.Empty : reader["current_designation"].ToString(),
                            JobGrade = reader["job_grade"] == DBNull.Value ? String.Empty : reader["job_grade"].ToString(),
                            ReportsTo1_EmployeeID = reader["reports_to_1"] == DBNull.Value ? String.Empty : reader["reports_to_1"].ToString(),
                            ReportsTo2_EmployeeID = reader["reports_to_2"] == DBNull.Value ? String.Empty : reader["reports_to_2"].ToString(),
                            ReportsTo3_EmployeeID = reader["reports_to_3"] == DBNull.Value ? String.Empty : reader["reports_to_3"].ToString(),
                            EmploymentStatus = reader["employment_status"] == DBNull.Value ? String.Empty : reader["employment_status"].ToString(),
                            DateOfLastPromotion = reader["date_of_last_promotion"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["date_of_last_promotion"],
                            OfficialEmail = reader["official_email"] == DBNull.Value ? String.Empty : reader["official_email"].ToString(),
                            MaritalStatus = reader["marital_status"] == DBNull.Value ? String.Empty : reader["marital_status"].ToString(),
                            StateOfOrigin = reader["state_of_origin"] == DBNull.Value ? String.Empty : reader["state_of_origin"].ToString(),
                            LgaOfOrigin = reader["lga_of_origin"] == DBNull.Value ? String.Empty : reader["lga_of_origin"].ToString(),
                            Religion = reader["religion"] == DBNull.Value ? String.Empty : reader["religion"].ToString(),
                            GeoPoliticalRegion = reader["geo_political_region"] == DBNull.Value ? String.Empty : reader["geo_political_region"].ToString(),
                            NextOfKinName = reader["next_of_kin_name"] == DBNull.Value ? String.Empty : reader["next_of_kin_name"].ToString(),
                            NextOfKinRelationship = reader["next_of_kin_relationship"] == DBNull.Value ? String.Empty : reader["next_of_kin_relationship"].ToString(),
                            NextOfKinAddress = reader["next_of_kin_address"] == DBNull.Value ? String.Empty : reader["next_of_kin_address"].ToString(),
                            NextOfKinPhone = reader["next_of_kin_phone"] == DBNull.Value ? String.Empty : reader["next_of_kin_phone"].ToString(),
                            NextOfKinEmail = reader["next_of_kin_email"] == DBNull.Value ? String.Empty : reader["next_of_kin_email"].ToString(),
                            EmployeeModifiedBy = reader["modified_by"] == DBNull.Value ? string.Empty : reader["modified_by"].ToString(),
                            EmployeeModifiedDate = reader["modified_date"] == DBNull.Value ? string.Empty : reader["modified_date"].ToString(),
                            EmployeeCreatedBy = reader["created_by"] == DBNull.Value ? string.Empty : reader["created_by"].ToString(),
                            EmployeeCreatedDate = reader["created_date"] == DBNull.Value ? string.Empty : reader["created_date"].ToString(),

                            PersonID = reader["id"] == DBNull.Value ? String.Empty : reader["id"].ToString(),
                            Title = reader["title"] == DBNull.Value ? string.Empty : reader["title"].ToString(),
                            Surname = reader["sname"] == DBNull.Value ? string.Empty : reader["sname"].ToString(),
                            FirstName = reader["fname"] == DBNull.Value ? string.Empty : reader["fname"].ToString(),
                            OtherNames = reader["oname"] == DBNull.Value ? string.Empty : reader["oname"].ToString(),
                            FullName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                            Sex = reader["sex"] == DBNull.Value ? string.Empty : reader["sex"].ToString(),
                            PhoneNo1 = reader["phone1"] == DBNull.Value ? string.Empty : reader["phone1"].ToString(),
                            PhoneNo2 = reader["phone2"] == DBNull.Value ? string.Empty : reader["phone2"].ToString(),
                            Email = reader["email"] == DBNull.Value ? string.Empty : reader["email"].ToString(),
                            Address = reader["address"] == DBNull.Value ? string.Empty : reader["address"].ToString(),
                            ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                            ModifiedTime = reader["mdt"] == DBNull.Value ? string.Empty : reader["mdt"].ToString(),
                            CreatedTime = reader["ctt"] == DBNull.Value ? string.Empty : reader["ctt"].ToString(),
                            CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                        });
                    }
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                employeeList = null;
            }
            return employeeList;
        }

        #endregion

        //================================ EmployeeBasicInfo Action Methods ===========================//
        #region Employee Basic Info Action Methods
        public async Task<bool> AddEmployeeBasicInfoAsync(EmployeeBasicInfo employeeBasicInfo)
        {
            if(employeeBasicInfo == null) { throw new ArgumentNullException(nameof(employeeBasicInfo), "The required parameter [EmployeeBasicInfo] is missing or has an invalid value."); }
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append($"INSERT INTO public.erm_emps(emp_id, emp_no_1, emp_no_2, dept_id, unit_id, loc_id, start_up_date, ");
            sb.Append($"start_up_designation, job_grade, reports_to_1, reports_to_2, reports_to_3, employment_status, ");
            sb.Append($"official_email, state_of_origin, lga_of_origin, religion, geo_political_region, ");
            sb.Append($"modified_by, modified_date, created_by, created_date, coy_id) ");
            sb.Append($"VALUES (@emp_id, @emp_no_1, @emp_no_2, @dept_id, @unit_id, @loc_id, @start_up_date, @start_up_designation, ");
            sb.Append($"@job_grade, @reports_to_1, @reports_to_2, @reports_to_3, @employment_status, ");
            sb.Append($"@official_email, @state_of_origin, @lga_of_origin, @religion, @geo_political_region, ");
            sb.Append($"@modified_by, @modified_date, @created_by, @created_date, @coy_id);");
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
                    var companyCode = cmd.Parameters.Add("@coy_id", NpgsqlDbType.Text);
                    var departmentId = cmd.Parameters.Add("@dept_id", NpgsqlDbType.Integer);
                    var unitId = cmd.Parameters.Add("unit_id", NpgsqlDbType.Integer);
                    var locationId = cmd.Parameters.Add("@loc_id", NpgsqlDbType.Integer);
                    var startUpDate = cmd.Parameters.Add("@start_up_date", NpgsqlDbType.Date);
                    var startUpDesignation = cmd.Parameters.Add("@start_up_designation", NpgsqlDbType.Text);
                    var jobGrade = cmd.Parameters.Add("@job_grade", NpgsqlDbType.Text);
                    var reportsTo1 = cmd.Parameters.Add("@reports_to_1", NpgsqlDbType.Text);
                    var reportsTo2 = cmd.Parameters.Add("@reports_to_2", NpgsqlDbType.Text);
                    var reportsTo3 = cmd.Parameters.Add("@reports_to_3", NpgsqlDbType.Text);
                    var employmentStatus = cmd.Parameters.Add("@employment_status", NpgsqlDbType.Text);
                    var officialEmail = cmd.Parameters.Add("@official_email", NpgsqlDbType.Text);
                    var stateOfOrigin = cmd.Parameters.Add("@state_of_origin", NpgsqlDbType.Text);
                    var lgaOfOrigin = cmd.Parameters.Add("@lga_of_origin", NpgsqlDbType.Text);
                    var religion = cmd.Parameters.Add("@religion", NpgsqlDbType.Text);
                    var geoPoliticalRegion = cmd.Parameters.Add("@geo_political_region", NpgsqlDbType.Text);
                    var modifiedBy = cmd.Parameters.Add("@modified_by", NpgsqlDbType.Text);
                    var modifiedDate = cmd.Parameters.Add("@modified_date", NpgsqlDbType.Text);
                    var createdBy = cmd.Parameters.Add("@created_by", NpgsqlDbType.Text);
                    var createdDate = cmd.Parameters.Add("@created_date", NpgsqlDbType.Text);

                    cmd.Prepare();
                    employeeId.Value = employeeBasicInfo.EmployeeID;
                    employeeNo1.Value = employeeBasicInfo.EmployeeNo1 ?? (object)DBNull.Value;
                    employeeNo2.Value = employeeBasicInfo.EmployeeNo2 ?? (object)DBNull.Value;
                    companyCode.Value = employeeBasicInfo.CompanyCode ?? (object)DBNull.Value;
                    departmentId.Value = employeeBasicInfo.DepartmentID ?? (object)DBNull.Value;
                    unitId.Value = employeeBasicInfo.UnitID ?? (object)DBNull.Value;
                    locationId.Value = employeeBasicInfo.LocationID ?? (object)DBNull.Value;
                    startUpDate.Value = employeeBasicInfo.StartUpDate ?? (object)DBNull.Value;
                    startUpDesignation.Value = employeeBasicInfo.StartUpDesignation ?? (object)DBNull.Value;
                    jobGrade.Value = employeeBasicInfo.JobGrade ?? (object)DBNull.Value;
                    reportsTo1.Value = employeeBasicInfo.ReportsTo1_EmployeeID ?? (object)DBNull.Value;
                    reportsTo2.Value = employeeBasicInfo.ReportsTo2_EmployeeID ?? (object)DBNull.Value;
                    reportsTo3.Value = employeeBasicInfo.ReportsTo3_EmployeeID ?? (object)DBNull.Value;
                    employmentStatus.Value = employeeBasicInfo.EmploymentStatus ?? (object)DBNull.Value;
                    officialEmail.Value = employeeBasicInfo.OfficialEmail ?? (object)DBNull.Value;
                    stateOfOrigin.Value = employeeBasicInfo.StateOfOrigin ?? (object)DBNull.Value;
                    lgaOfOrigin.Value = employeeBasicInfo.LgaOfOrigin ?? (object)DBNull.Value;
                    religion.Value = employeeBasicInfo.Religion ?? (object)DBNull.Value;
                    geoPoliticalRegion.Value = employeeBasicInfo.GeoPoliticalRegion ?? (object)DBNull.Value;
                    modifiedBy.Value = employeeBasicInfo.EmployeeModifiedBy ?? (object)DBNull.Value;
                    modifiedDate.Value = employeeBasicInfo.EmployeeModifiedDate ?? (object)DBNull.Value;
                    createdBy.Value = employeeBasicInfo.EmployeeCreatedBy ?? (object)DBNull.Value;
                    createdDate.Value = employeeBasicInfo.EmployeeCreatedDate ?? (object)DBNull.Value;

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

        public async Task<bool> EditEmployeeBasicInfoAsync(EmployeeBasicInfo employeeBasicInfo)
        {
            if (employeeBasicInfo == null) { throw new ArgumentNullException(nameof(employeeBasicInfo),"The require parameter [Employee Basic Info Model] is missing."); }
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append($"UPDATE public.erm_emps SET emp_no_2=@emp_no_2, dept_id=@dept_id, unit_id=@unit_id, ");
            sb.Append($"loc_id=@loc_id, start_up_date=@start_up_date, start_up_designation=@start_up_designation, ");
            sb.Append($"job_grade=@job_grade, reports_to_1=@reports_to_1, reports_to_2=@reports_to_2, ");
            sb.Append($"reports_to_3=@reports_to_3, employment_status=@employment_status, coy_id=@coy_id, ");
            sb.Append($"official_email=@official_email, state_of_origin=@state_of_origin, lga_of_origin=@lga_of_origin, ");
            sb.Append($"religion=@religion, geo_political_region=@geo_political_region, ");
            sb.Append($"modified_by=@modified_by, modified_date=@modified_date ");
            sb.Append($"WHERE (LOWER(emp_id) = LOWER(@emp_id));");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var employeeId = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                    var employeeNo2 = cmd.Parameters.Add("@emp_no_2", NpgsqlDbType.Text);
                    var companyCode = cmd.Parameters.Add("@coy_id", NpgsqlDbType.Text);
                    var departmentId = cmd.Parameters.Add("@dept_id", NpgsqlDbType.Integer);
                    var unitId = cmd.Parameters.Add("unit_id", NpgsqlDbType.Integer);
                    var locationId = cmd.Parameters.Add("@loc_id", NpgsqlDbType.Integer);
                    var startUpDate = cmd.Parameters.Add("@start_up_date", NpgsqlDbType.Date);
                    var startUpDesignation = cmd.Parameters.Add("@start_up_designation", NpgsqlDbType.Text);
                    var jobGrade = cmd.Parameters.Add("@job_grade", NpgsqlDbType.Text);
                    var reportsTo1 = cmd.Parameters.Add("@reports_to_1", NpgsqlDbType.Text);
                    var reportsTo2 = cmd.Parameters.Add("@reports_to_2", NpgsqlDbType.Text);
                    var reportsTo3 = cmd.Parameters.Add("@reports_to_3", NpgsqlDbType.Text);
                    var employmentStatus = cmd.Parameters.Add("@employment_status", NpgsqlDbType.Text);
                    var officialEmail = cmd.Parameters.Add("@official_email", NpgsqlDbType.Text);
                    var stateOfOrigin = cmd.Parameters.Add("@state_of_origin", NpgsqlDbType.Text);
                    var lgaOfOrigin = cmd.Parameters.Add("@lga_of_origin", NpgsqlDbType.Text);
                    var religion = cmd.Parameters.Add("@religion", NpgsqlDbType.Text);
                    var geoPoliticalRegion = cmd.Parameters.Add("@geo_political_region", NpgsqlDbType.Text);
                    var modifiedBy = cmd.Parameters.Add("@modified_by", NpgsqlDbType.Text);
                    var modifiedDate = cmd.Parameters.Add("@modified_date", NpgsqlDbType.Text);

                    cmd.Prepare();
                    employeeId.Value = employeeBasicInfo.EmployeeID;
                    employeeNo2.Value = employeeBasicInfo.EmployeeNo2 ?? (object)DBNull.Value;
                    companyCode.Value = employeeBasicInfo.CompanyCode ?? (object)DBNull.Value;
                    departmentId.Value = employeeBasicInfo.DepartmentID ?? (object)DBNull.Value;
                    unitId.Value = employeeBasicInfo.UnitID ?? (object)DBNull.Value;
                    locationId.Value = employeeBasicInfo.LocationID ?? (object)DBNull.Value;
                    startUpDate.Value = employeeBasicInfo.StartUpDate ?? (object)DBNull.Value;
                    startUpDesignation.Value = employeeBasicInfo.StartUpDesignation ?? (object)DBNull.Value;
                    jobGrade.Value = employeeBasicInfo.JobGrade ?? (object)DBNull.Value;
                    reportsTo1.Value = employeeBasicInfo.ReportsTo1_EmployeeID ?? (object)DBNull.Value;
                    reportsTo2.Value = employeeBasicInfo.ReportsTo2_EmployeeID ?? (object)DBNull.Value;
                    reportsTo3.Value = employeeBasicInfo.ReportsTo3_EmployeeID ?? (object)DBNull.Value;
                    employmentStatus.Value = employeeBasicInfo.EmploymentStatus ?? (object)DBNull.Value;
                    officialEmail.Value = employeeBasicInfo.OfficialEmail ?? (object)DBNull.Value;
                    stateOfOrigin.Value = employeeBasicInfo.StateOfOrigin ?? (object)DBNull.Value;
                    lgaOfOrigin.Value = employeeBasicInfo.LgaOfOrigin ?? (object)DBNull.Value;
                    religion.Value = employeeBasicInfo.Religion ?? (object)DBNull.Value;
                    geoPoliticalRegion.Value = employeeBasicInfo.GeoPoliticalRegion ?? (object)DBNull.Value;
                    modifiedBy.Value = employeeBasicInfo.EmployeeModifiedBy ?? (object)DBNull.Value;
                    modifiedDate.Value = employeeBasicInfo.EmployeeModifiedDate ?? (object)DBNull.Value;

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

        //=========================== Employee Next Of Kin Info Action Methods ============================// 
        #region Employee Next Of Kin Info Action Methods 
        public async Task<bool> EditEmployeeNextOfKinInfoAsync(EmployeeNextOfKinInfo employeeNextOfKinInfo)
        {
            if (employeeNextOfKinInfo == null) { throw new ArgumentNullException(nameof(employeeNextOfKinInfo), "The require parameter [Employee Next Of Kin Info Model] is missing."); }
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append($"UPDATE public.erm_emps SET  next_of_kin_name=@next_of_kin_name, next_of_kin_relationship=@next_of_kin_relationship, ");
            sb.Append($"modified_by=@modified_by, modified_date=@modified_date, next_of_kin_address=@next_of_kin_address, ");
            sb.Append($"next_of_kin_phone=@next_of_kin_phone, next_of_kin_email=@next_of_kin_email  WHERE emp_id=@emp_id;");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var employeeId = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                    var nextOfKinName = cmd.Parameters.Add("@next_of_kin_name", NpgsqlDbType.Text);
                    var nextOfKinRelationship = cmd.Parameters.Add("@next_of_kin_relationship", NpgsqlDbType.Text);
                    var nextOfKinAddresss = cmd.Parameters.Add("@next_of_kin_address", NpgsqlDbType.Text);
                    var nextOfKinPhone = cmd.Parameters.Add("@next_of_kin_phone", NpgsqlDbType.Text);
                    var nextOfKinEmail = cmd.Parameters.Add("@next_of_kin_email", NpgsqlDbType.Text);
                    var modifiedBy = cmd.Parameters.Add("@modified_by", NpgsqlDbType.Text);
                    var modifiedDate = cmd.Parameters.Add("@modified_date", NpgsqlDbType.Text);
                    cmd.Prepare();
                    employeeId.Value = employeeNextOfKinInfo.EmployeeID;
                    nextOfKinName.Value = employeeNextOfKinInfo.NextOfKinName ?? (object)DBNull.Value;
                    nextOfKinRelationship.Value = employeeNextOfKinInfo.NextOfKinRelationship ?? (object)DBNull.Value;
                    nextOfKinAddresss.Value = employeeNextOfKinInfo.NextOfKinAddress ?? (object)DBNull.Value;
                    nextOfKinPhone.Value = employeeNextOfKinInfo.NextOfKinPhone ?? (object)DBNull.Value;
                    nextOfKinEmail.Value = employeeNextOfKinInfo.NextOfKinEmail ?? (object)DBNull.Value;
                    modifiedBy.Value = employeeNextOfKinInfo.EmployeeModifiedBy ?? (object)DBNull.Value;
                    modifiedDate.Value = employeeNextOfKinInfo.EmployeeModifiedDate ?? (object)DBNull.Value;

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

        //=========================== Employee History Info Action Methods ============================// 
        #region Employee History Info Action Methods
        public async Task<bool> EditEmployeeHistoryInfoAsync(EmployeeHistoryInfo employeeHistoryInfo)
        {
            if (employeeHistoryInfo == null) { throw new ArgumentNullException(nameof(employeeHistoryInfo), "The require parameter [Employee History Info Model] is missing."); }
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append($"UPDATE public.erm_emps SET yrs_of_experience=@yrs_of_experience, length_of_service=@length_of_service, ");
            sb.Append($"place_of_engagement=@place_of_engagement, confirmation_date=@confirmation_date, ");
            sb.Append($"current_designation=@current_designation, date_of_last_promotion=@date_of_last_promotion, ");
            sb.Append($"modified_by=@modified_by, modified_date=@modified_date  WHERE LOWER(emp_id)=LOWER(@emp_id);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var employeeId = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                    var yearsOfExperience = cmd.Parameters.Add("@yrs_of_experience", NpgsqlDbType.Integer);
                    var lengthOfService = cmd.Parameters.Add("@length_of_service", NpgsqlDbType.Integer);
                    var placeOfEngagement = cmd.Parameters.Add("@place_of_engagement", NpgsqlDbType.Text);
                    var confirmationDate = cmd.Parameters.Add("@confirmation_date", NpgsqlDbType.Date);
                    var currentDesignation = cmd.Parameters.Add("@current_designation", NpgsqlDbType.Text);
                    var dateOfLastPromotion = cmd.Parameters.Add("@date_of_last_promotion", NpgsqlDbType.Date);
                    var modifiedBy = cmd.Parameters.Add("@modified_by", NpgsqlDbType.Text);
                    var modifiedDate = cmd.Parameters.Add("@modified_date", NpgsqlDbType.Text);
                    cmd.Prepare();
                    employeeId.Value = employeeHistoryInfo.EmployeeID;
                    yearsOfExperience.Value = employeeHistoryInfo.YearsOfExperience ?? (object)DBNull.Value;
                    lengthOfService.Value = employeeHistoryInfo.LengthOfService ?? (object)DBNull.Value;
                    placeOfEngagement.Value = employeeHistoryInfo.PlaceOfEngagement ?? (object)DBNull.Value;
                    confirmationDate.Value = employeeHistoryInfo.ConfirmationDate ?? (object)DBNull.Value;
                    currentDesignation.Value = employeeHistoryInfo.CurrentDesignation ?? (object)DBNull.Value;
                    dateOfLastPromotion.Value = employeeHistoryInfo.DateOfLastPromotion ?? (object)DBNull.Value;
                    modifiedBy.Value = employeeHistoryInfo.EmployeeModifiedBy ?? (object)DBNull.Value;
                    modifiedDate.Value = employeeHistoryInfo.EmployeeModifiedDate ?? (object)DBNull.Value;

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

    }
}
