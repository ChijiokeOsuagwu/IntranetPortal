using IntranetPortal.Base.Models.BaseModels;
using IntranetPortal.Base.Repositories.BaseRepositories;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Data.Repositories.BaseRepositories
{
    public class PersonRepository : IPersonRepository
    {
        public IConfiguration _config { get; }
        public PersonRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        //============= Persons Action Methods =====================================//
        #region Persons Action Methods

        public async Task<bool> AddPersonAsync(Person person)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append($"INSERT INTO public.gst_prsns(id, title, sname, fname, oname, fullname, sex, phone1,phone2, email, ");
            sb.Append($"address, mdb, mdt, ctb, ctt, imgp, birthday, birthmonth, birthyear, maritalstatus) VALUES (@id, ");
            sb.Append($"@title, @sname, @fname, @oname, @fullname, @sex, @phone1, @phone2, @email, @address, ");
            sb.Append($"@mdb, @mdt, @ctb, @ctt, @imgp, @birthday, @birthmonth, @birthyear, @maritalstatus); ");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var personId = cmd.Parameters.Add("@id", NpgsqlDbType.Text);
                    var title = cmd.Parameters.Add("@title", NpgsqlDbType.Text);
                    var surname = cmd.Parameters.Add("@sname", NpgsqlDbType.Text);
                    var firstName = cmd.Parameters.Add("@fname", NpgsqlDbType.Text);
                    var otherNames = cmd.Parameters.Add("@oname", NpgsqlDbType.Text);
                    var fullName = cmd.Parameters.Add("@fullname", NpgsqlDbType.Text);
                    var sex = cmd.Parameters.Add("@sex", NpgsqlDbType.Text);
                    var phone1 = cmd.Parameters.Add("@phone1", NpgsqlDbType.Text);
                    var phone2 = cmd.Parameters.Add("@phone2", NpgsqlDbType.Text);
                    var email = cmd.Parameters.Add("@email", NpgsqlDbType.Text);
                    var address = cmd.Parameters.Add("@address", NpgsqlDbType.Text);
                    var modifiedBy = cmd.Parameters.Add("@mdb", NpgsqlDbType.Text);
                    var modifiedDate = cmd.Parameters.Add("@mdt", NpgsqlDbType.Text);
                    var createdBy = cmd.Parameters.Add("@ctb", NpgsqlDbType.Text);
                    var createdDate = cmd.Parameters.Add("@ctt", NpgsqlDbType.Text);
                    var imagePath = cmd.Parameters.Add("@imgp", NpgsqlDbType.Text);
                    var birthday = cmd.Parameters.Add("@birthday", NpgsqlDbType.Integer);
                    var birthmonth = cmd.Parameters.Add("@birthmonth", NpgsqlDbType.Integer);
                    var birthyear = cmd.Parameters.Add("@birthyear", NpgsqlDbType.Integer);
                    var maritalstatus = cmd.Parameters.Add("@maritalstatus", NpgsqlDbType.Text);
                    cmd.Prepare();
                    personId.Value = person.PersonID;
                    title.Value = person.Title ?? (object)DBNull.Value;
                    surname.Value = person.Surname ?? (object)DBNull.Value;
                    firstName.Value = person.FirstName ?? (object)DBNull.Value;
                    otherNames.Value = person.OtherNames ?? (object)DBNull.Value;
                    fullName.Value = person.FullName;
                    sex.Value = person.Sex ?? (object)DBNull.Value;
                    phone1.Value = person.PhoneNo1 ?? (object)DBNull.Value;
                    phone2.Value = person.PhoneNo2 ?? (object)DBNull.Value;
                    email.Value = person.Email ?? (object)DBNull.Value;
                    address.Value = person.Address ?? (object)DBNull.Value;
                    modifiedBy.Value = person.ModifiedBy ?? (object)DBNull.Value;
                    modifiedDate.Value = person.ModifiedTime ?? (object)DBNull.Value;
                    createdBy.Value = person.CreatedBy ?? (object)DBNull.Value;
                    createdDate.Value = person.CreatedTime ?? (object)DBNull.Value;
                    imagePath.Value = person.ImagePath ?? (object)DBNull.Value;
                    birthday.Value = person.BirthDay ?? (object)DBNull.Value;
                    birthmonth.Value = person.BirthMonth ?? (object)DBNull.Value;
                    birthyear.Value = person.BirthYear ?? (object)DBNull.Value;
                    maritalstatus.Value = person.MaritalStatus ?? (object)DBNull.Value;
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

        public async Task<bool> DeletePersonAsync(string Id)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = $"DELETE FROM public.gst_prsns WHERE (LOWER(id) = LOWER(@id));";
            try
            {
                await conn.OpenAsync();
                //Delete data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var personId = cmd.Parameters.Add("@id", NpgsqlDbType.Text);
                    cmd.Prepare();
                    personId.Value = Id;
                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                //ErrorRepository errorRepository = new ErrorRepository(_config);
                //ErrorEntity errorEntity = new ErrorEntity();
                //errorEntity.ErrorMessage = ex.Message;
                //errorEntity.ErrorDetail = ex.ToString();
                //errorEntity.ErrorTime = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} (UTC)";
                //errorEntity.ErrorInnerSource = ex.Source;
                //errorEntity.ErrorSource = "ApplicationUserRepository_AddApplicationUserAsync";
                //errorRepository.AddError(errorEntity);
                await conn.CloseAsync();
                rows = -1;
            }
            return rows > 0;
        }

        public async Task<bool> EditPersonAsync(Person person)
        {
            if (person == null) { return false; }
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append($"UPDATE public.gst_prsns SET title=@title, sname=@sname, fname=@fname, oname=@oname, fullname=@fullname, ");
            sb.Append($"sex=@sex, phone1=@phone1, phone2=@phone2, email=@email, address=@address, mdb=@mdb, mdt=@mdt, imgp=@imgp, ");
            sb.Append($"birthday=@birthday, birthmonth=@birthmonth, birthyear=@birthyear, maritalstatus=@maritalstatus ");
            sb.Append($"WHERE (id=@id);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var personId = cmd.Parameters.Add("@id", NpgsqlDbType.Text);
                    var personTitle = cmd.Parameters.Add("@title", NpgsqlDbType.Text);
                    var personSurname = cmd.Parameters.Add("@sname", NpgsqlDbType.Text);
                    var personFirstName = cmd.Parameters.Add("@fname", NpgsqlDbType.Text);
                    var personOtherNames = cmd.Parameters.Add("@oname", NpgsqlDbType.Text);
                    var personFullName = cmd.Parameters.Add("@fullname", NpgsqlDbType.Text);
                    var personSex = cmd.Parameters.Add("@sex", NpgsqlDbType.Text);
                    var personPhone1 = cmd.Parameters.Add("@phone1", NpgsqlDbType.Text);
                    var personPhone2 = cmd.Parameters.Add("@phone2", NpgsqlDbType.Text);
                    var personEmail = cmd.Parameters.Add("@email", NpgsqlDbType.Text);
                    var personAddress = cmd.Parameters.Add("@address", NpgsqlDbType.Text);
                    var personModifiedBy = cmd.Parameters.Add("@mdb", NpgsqlDbType.Text);
                    var personModifiedTime = cmd.Parameters.Add("@mdt", NpgsqlDbType.Text);
                    var personImagePath = cmd.Parameters.Add("@imgp", NpgsqlDbType.Text);
                    var personBirthDay = cmd.Parameters.Add("@birthday", NpgsqlDbType.Integer);
                    var personBirthMonth = cmd.Parameters.Add("@birthmonth", NpgsqlDbType.Integer);
                    var personBirthYear = cmd.Parameters.Add("@birthyear", NpgsqlDbType.Integer);
                    var personMaritalStatus = cmd.Parameters.Add("@maritalstatus", NpgsqlDbType.Text);
                    cmd.Prepare();
                    personId.Value = person.PersonID;
                    personTitle.Value = person.Title ?? (object)DBNull.Value;
                    personSurname.Value = person.Surname ?? (object)DBNull.Value;
                    personFirstName.Value = person.FirstName ?? (object)DBNull.Value;
                    personOtherNames.Value = person.OtherNames ?? (object)DBNull.Value;
                    personFullName.Value = person.FullName;
                    personSex.Value = person.Sex ?? (object)DBNull.Value;
                    personPhone1.Value = person.PhoneNo1 ?? (object)DBNull.Value;
                    personPhone2.Value = person.PhoneNo2 ?? (object)DBNull.Value;
                    personEmail.Value = person.Email ?? (object)DBNull.Value;
                    personAddress.Value = person.Address ?? (object)DBNull.Value;
                    personImagePath.Value = person.ImagePath ?? (object)DBNull.Value;
                    personModifiedBy.Value = person.ModifiedBy ?? (object)DBNull.Value;
                    personModifiedTime.Value = person.ModifiedTime ?? (object)DBNull.Value;
                    personBirthDay.Value = person.BirthDay ?? (object)DBNull.Value;
                    personBirthMonth.Value = person.BirthMonth ?? (object)DBNull.Value;
                    personBirthYear.Value = person.BirthYear ?? (object)DBNull.Value;
                    personMaritalStatus.Value = person.MaritalStatus ?? (object)DBNull.Value;

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

        public async Task<Person> GetPersonByIdAsync(string Id)
        {
            Person person = new Person();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            if (String.IsNullOrEmpty(Id)) { return null; }
            sb.Append($"SELECT id, title, sname, fname, oname, fullname, sex, phone1, phone2, email, ");
            sb.Append($"address, mdb, mdt, ctb, ctt, imgp, birthday, birthmonth, birthyear, maritalstatus ");
            sb.Append($"FROM public.gst_prsns WHERE (id = @id); ");
            query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var personId = cmd.Parameters.Add("@id", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    personId.Value = Id;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            person.PersonID = reader["id"] == DBNull.Value ? string.Empty : (reader["id"]).ToString();
                            person.Title = reader["title"] == DBNull.Value ? String.Empty : reader["title"].ToString();
                            person.Surname = reader["sname"] == DBNull.Value ? String.Empty : reader["sname"].ToString();
                            person.FirstName = reader["fname"] == DBNull.Value ? String.Empty : reader["fname"].ToString();
                            person.OtherNames = reader["oname"] == DBNull.Value ? String.Empty : reader["oname"].ToString();
                            person.FullName = reader["fullname"] == DBNull.Value ? String.Empty : reader["fullname"].ToString();
                            person.Sex = reader["sex"] == DBNull.Value ? String.Empty : reader["sex"].ToString();
                            person.PhoneNo1 = reader["phone1"] == DBNull.Value ? String.Empty : reader["phone1"].ToString();
                            person.PhoneNo2 = reader["phone2"] == DBNull.Value ? String.Empty : reader["phone2"].ToString();
                            person.Email = reader["email"] == DBNull.Value ? String.Empty : reader["email"].ToString();
                            person.Address = reader["address"] == DBNull.Value ? String.Empty : reader["address"].ToString();
                            person.ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString();
                            person.ModifiedTime = reader["mdt"] == DBNull.Value ? string.Empty : reader["mdt"].ToString();
                            person.CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString();
                            person.CreatedTime = reader["ctt"] == DBNull.Value ? string.Empty : reader["ctt"].ToString();
                            person.BirthDay = reader["birthday"] == DBNull.Value ? 0 : (int)reader["birthday"];
                            person.BirthMonth = reader["birthmonth"] == DBNull.Value ? 0 : (int)reader["birthmonth"];
                            person.BirthYear = reader["birthyear"] == DBNull.Value ? 0 : (int)reader["birthyear"];
                            person.MaritalStatus = reader["maritalstatus"] == DBNull.Value ? string.Empty : reader["maritalstatus"].ToString();
                        }
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message);
            }
            return person;
        }

        public async Task<Person> GetPersonByNameAsync(string personName)
        {
            Person person = new Person();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            if (String.IsNullOrEmpty(personName)) { throw new ArgumentNullException("Required parameter PersonName cannot be null."); }
            sb.Append($"SELECT id, title, sname, fname, oname, fullname, sex, phone1, phone2, email, ");
            sb.Append($"address, mdb, mdt, ctb, ctt, imgp, birthday, birthmonth, birthyear, maritalstatus ");
            sb.Append($"FROM public.gst_prsns WHERE (fullname = @fullname); ");
            query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var fullName = cmd.Parameters.Add("@fullname", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    fullName.Value = personName;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            person.PersonID = reader["id"] == DBNull.Value ? string.Empty : (reader["id"]).ToString();
                            person.Title = reader["title"] == DBNull.Value ? String.Empty : reader["title"].ToString();
                            person.Surname = reader["sname"] == DBNull.Value ? String.Empty : reader["sname"].ToString();
                            person.FirstName = reader["fname"] == DBNull.Value ? String.Empty : reader["fname"].ToString();
                            person.OtherNames = reader["oname"] == DBNull.Value ? String.Empty : reader["oname"].ToString();
                            person.FullName = reader["fullname"] == DBNull.Value ? String.Empty : reader["fullname"].ToString();
                            person.Sex = reader["sex"] == DBNull.Value ? String.Empty : reader["sex"].ToString();
                            person.PhoneNo1 = reader["phone1"] == DBNull.Value ? String.Empty : reader["phone1"].ToString();
                            person.PhoneNo2 = reader["phone2"] == DBNull.Value ? String.Empty : reader["phone2"].ToString();
                            person.Email = reader["email"] == DBNull.Value ? String.Empty : reader["email"].ToString();
                            person.Address = reader["address"] == DBNull.Value ? String.Empty : reader["address"].ToString();
                            person.ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString();
                            person.ModifiedTime = reader["mdt"] == DBNull.Value ? string.Empty : reader["mdt"].ToString();
                            person.CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString();
                            person.CreatedTime = reader["ctt"] == DBNull.Value ? string.Empty : reader["ctt"].ToString();
                            person.BirthDay = reader["birthday"] == DBNull.Value ? 0 : (int)reader["birthday"];
                            person.BirthMonth = reader["birthmonth"] == DBNull.Value ? 0 : (int)reader["birthmonth"];
                            person.BirthYear = reader["birthyear"] == DBNull.Value ? 0 : (int)reader["birthyear"];
                            person.MaritalStatus = reader["maritalstatus"] == DBNull.Value ? string.Empty : reader["maritalstatus"].ToString();
                        }
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message);
            }
            return person;
        }

        public async Task<IList<Person>> GetPersonsAsync()
        {
            List<Person> personList = new List<Person>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append($"SELECT id, title, sname, fname, oname, fullname, sex, phone1, phone2, email, ");
            sb.Append($"address, mdb, mdt, ctb, ctt, imgp, birthday, birthmonth, birthyear, maritalstatus ");
            sb.Append($"FROM public.gst_prsns ORDER BY fullname ASC; ");
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
                        personList.Add(new Person()
                        {
                            PersonID = reader["id"] == DBNull.Value ? string.Empty : (reader["id"]).ToString(),
                        Title = reader["title"] == DBNull.Value ? String.Empty : reader["title"].ToString(),
                        Surname = reader["sname"] == DBNull.Value ? String.Empty : reader["sname"].ToString(),
                        FirstName = reader["fname"] == DBNull.Value ? String.Empty : reader["fname"].ToString(),
                        OtherNames = reader["oname"] == DBNull.Value ? String.Empty : reader["oname"].ToString(),
                        FullName = reader["fullname"] == DBNull.Value ? String.Empty : reader["fullname"].ToString(),
                        Sex = reader["sex"] == DBNull.Value ? String.Empty : reader["sex"].ToString(),
                        PhoneNo1 = reader["phone1"] == DBNull.Value ? String.Empty : reader["phone1"].ToString(),
                        PhoneNo2 = reader["phone2"] == DBNull.Value ? String.Empty : reader["phone2"].ToString(),
                        Email = reader["email"] == DBNull.Value ? String.Empty : reader["email"].ToString(),
                        Address = reader["address"] == DBNull.Value ? String.Empty : reader["address"].ToString(),
                        ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                        ModifiedTime = reader["mdt"] == DBNull.Value ? string.Empty : reader["mdt"].ToString(),
                        CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                        CreatedTime = reader["ctt"] == DBNull.Value ? string.Empty : reader["ctt"].ToString(),
                        BirthDay = reader["birthday"] == DBNull.Value ? 0 : (int)reader["birthday"],
                        BirthMonth = reader["birthmonth"] == DBNull.Value ? 0 : (int)reader["birthmonth"],
                        BirthYear = reader["birthyear"] == DBNull.Value ? 0 : (int)reader["birthyear"],
                        MaritalStatus = reader["maritalstatus"] == DBNull.Value ? string.Empty : reader["maritalstatus"].ToString(),

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
            return personList;
        }

        #endregion

    }
}
