using IntranetPortal.Base.Models.BaseModels;
using IntranetPortal.Base.Repositories.BusinessManagerRepositories;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Data.Repositories.BusinessManagerRepositories
{
    public class BusinessContactRepository : IBusinessContactRepository
    {
        public IConfiguration _config { get; }
        public BusinessContactRepository(IConfiguration configuration)
        {
            _config = configuration;
        }
        public async Task<bool> AddAsync(BusinessContact businessContact)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append($"INSERT INTO public.gst_prsn_bzns(bzns_id, prsn_id, prsn_rl) ");
            sb.Append($"VALUES (@bzns_id, @prsn_id, @prsn_rl)");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var bzns_id = cmd.Parameters.Add("@bzns_id", NpgsqlDbType.Text);
                    var prsn_id = cmd.Parameters.Add("@prsn_id", NpgsqlDbType.Text);
                    var prsn_rl = cmd.Parameters.Add("@prsn_rl", NpgsqlDbType.Text);
                    cmd.Prepare();
                    bzns_id.Value = businessContact.BusinessID;
                    prsn_id.Value = businessContact.PersonID;
                    prsn_rl.Value = businessContact.PersonRole;

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

        public async Task<bool> EditAsync(BusinessContact businessContact)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = $"UPDATE public.gst_prsn_bzns SET prsn_rl = @prsn_rl WHERE (prsn_bzns_id = @prsn_bzns_id);";
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var prsn_rl = cmd.Parameters.Add("@prsn_rl", NpgsqlDbType.Text);
                    var prsn_bzns_id = cmd.Parameters.Add("@prsn_bzns_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    prsn_rl.Value = businessContact.PersonRole;
                    prsn_bzns_id.Value = businessContact.BusinessContactID;
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

        public async Task<bool> DeleteAsync(int businessContactId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = $"DELETE FROM public.gst_prsn_bzns WHERE (prsn_bzns_id = @prsn_bzns_id);";
            try
            {
                await conn.OpenAsync();
                //Delete data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var prsn_bzns_id = cmd.Parameters.Add("@prsn_bzns_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    prsn_bzns_id.Value = businessContactId;
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

        public async Task<bool> DeleteByBusinessIdAsync(string businessId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = $"DELETE FROM public.gst_prsn_bzns WHERE (bzns_id = @bzns_id);";
            try
            {
                await conn.OpenAsync();
                //Delete data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var bzns_id = cmd.Parameters.Add("@bzns_id", NpgsqlDbType.Text);
                    cmd.Prepare();
                    bzns_id.Value = businessId;
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

        public async Task<BusinessContact> GetByIdAsync(int businessContactId)
        {
            BusinessContact person = new BusinessContact();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            if (businessContactId < 1) { throw new ArgumentNullException("Required parameter [businessContactId] cannot be null."); }
            sb.Append($"SELECT c.prsn_bzns_id, c.bzns_id, c.prsn_id, c.prsn_rl, p.title, p.sname, ");
            sb.Append($"p.fname, p.oname, p.fullname, p.sex, p.phone1, p.phone2, p.email, p.address, ");
            sb.Append($"p.mdb, p.mdt, p.ctb, p.ctt, p.imgp, p.birthday, p.birthmonth, p.birthyear, ");
            sb.Append($"p.maritalstatus, b.bzns_name, b.station_id, b.bzns_no, l.locname ");
            sb.Append($"FROM public.gst_prsn_bzns c ");
            sb.Append($"INNER JOIN public.gst_prsns p ON c.prsn_id = p.id ");
            sb.Append($"INNER JOIN public.gst_bzns b ON b.bzns_id = c.bzns_id ");
            sb.Append($"INNER JOIN public.gst_locs l ON b.station_id = l.locqk ");
            sb.Append("WHERE (c.prsn_bzns_id = @prsn_bzns_id); ");
            query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var prsn_bzns_id = cmd.Parameters.Add("@prsn_bzns_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    prsn_bzns_id.Value = businessContactId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            person.BusinessContactID = reader["prsn_bzns_id"] == DBNull.Value ? 0 : (int)reader["prsn_bzns_id"];
                            person.BusinessName = reader["bzns_name"] == DBNull.Value ? String.Empty : reader["bzns_name"].ToString();
                            person.BusinessID = reader["bzns_id"] == DBNull.Value ? string.Empty : reader["bzns_id"].ToString();
                            person.BusinessNo = reader["bzns_no"] == DBNull.Value ? string.Empty : reader["bzns_no"].ToString();
                            person.StationID = reader["station_id"] == DBNull.Value ? string.Empty : reader["station_id"].ToString();
                            person.StationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString();

                            person.PersonRole = reader["prsn_rl"] == DBNull.Value ? string.Empty : reader["prsn_rl"].ToString();
                            person.PersonID = reader["prsn_id"] == DBNull.Value ? string.Empty : (reader["prsn_id"]).ToString();
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
                            person.BirthDay = reader["birthday"] == DBNull.Value ? (int?)null : (int)reader["birthday"];
                            person.BirthMonth = reader["birthmonth"] == DBNull.Value ? (int?)null : (int)reader["birthmonth"];
                            person.BirthYear = reader["birthyear"] == DBNull.Value ? (int?)null : (int)reader["birthyear"];
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

        public async Task<IList<BusinessContact>> GetAllAsync()
        {
            List<BusinessContact> businessContactList = new List<BusinessContact>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append($"SELECT c.prsn_bzns_id, c.bzns_id, c.prsn_id, c.prsn_rl, p.title, p.sname, ");
            sb.Append($"p.fname, p.oname, p.fullname, p.sex, p.phone1, p.phone2, p.email, p.address, ");
            sb.Append($"p.mdb, p.mdt, p.ctb, p.ctt, p.imgp, p.birthday, p.birthmonth, p.birthyear, ");
            sb.Append($"p.maritalstatus, b.bzns_name, b.station_id, b.bzns_no, l.locname ");
            sb.Append($"FROM public.gst_prsn_bzns c ");
            sb.Append($"INNER JOIN public.gst_prsns p ON c.prsn_id = p.id ");
            sb.Append($"INNER JOIN public.gst_bzns b ON b.bzns_id = c.bzns_id ");
            sb.Append($"INNER JOIN public.gst_locs l ON b.station_id = l.locqk ");
            sb.Append($"ORDER BY p.fullname");
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
                        businessContactList.Add(new BusinessContact()
                        {
                            BusinessContactID = reader["prsn_bzns_id"] == DBNull.Value ? 0 : (int)reader["prsn_bzns_id"],
                            BusinessName = reader["bzns_name"] == DBNull.Value ? String.Empty : reader["bzns_name"].ToString(),
                            BusinessID = reader["bzns_id"] == DBNull.Value ? string.Empty : reader["bzns_id"].ToString(),
                            BusinessNo = reader["bzns_no"] == DBNull.Value ? string.Empty : reader["bzns_no"].ToString(),
                            StationID = reader["station_id"] == DBNull.Value ? string.Empty : reader["station_id"].ToString(),
                            StationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),

                            PersonRole = reader["prsn_rl"] == DBNull.Value ? string.Empty : reader["prsn_rl"].ToString(),
                            PersonID = reader["prsn_id"] == DBNull.Value ? string.Empty : (reader["prsn_id"]).ToString(),
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
                            BirthDay = reader["birthday"] == DBNull.Value ? (int?)null : (int)reader["birthday"],
                            BirthMonth = reader["birthmonth"] == DBNull.Value ? (int?)null : (int)reader["birthmonth"],
                            BirthYear = reader["birthyear"] == DBNull.Value ? (int?)null : (int)reader["birthyear"],
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
            return businessContactList;
        }

        public async Task<IList<BusinessContact>> GetByBusinessIdAsync(string businessId)
        {
            List<BusinessContact> businessContactList = new List<BusinessContact>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append($"SELECT c.prsn_bzns_id, c.bzns_id, c.prsn_id, c.prsn_rl, p.title, p.sname, ");
            sb.Append($"p.fname, p.oname, p.fullname, p.sex, p.phone1, p.phone2, p.email, p.address, ");
            sb.Append($"p.mdb, p.mdt, p.ctb, p.ctt, p.imgp, p.birthday, p.birthmonth, p.birthyear, ");
            sb.Append($"p.maritalstatus, b.bzns_name, b.station_id, b.bzns_no, l.locname ");
            sb.Append($"FROM public.gst_prsn_bzns c ");
            sb.Append($"INNER JOIN public.gst_prsns p ON c.prsn_id = p.id ");
            sb.Append($"INNER JOIN public.gst_bzns b ON b.bzns_id = c.bzns_id ");
            sb.Append($"INNER JOIN public.gst_locs l ON b.station_id = l.locqk ");
            sb.Append($"WHERE (LOWER(c.bzns_id) = LOWER(@bzns_id)) ORDER BY p.fullname");
            query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var bzns_id = cmd.Parameters.Add("@bzns_id", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    bzns_id.Value = businessId;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        businessContactList.Add(new BusinessContact()
                        {
                            BusinessContactID = reader["prsn_bzns_id"] == DBNull.Value ? 0 : (int)reader["prsn_bzns_id"],
                        BusinessName = reader["bzns_name"] == DBNull.Value ? String.Empty : reader["bzns_name"].ToString(),
                        BusinessID = reader["bzns_id"] == DBNull.Value ? string.Empty : reader["bzns_id"].ToString(),
                        BusinessNo = reader["bzns_no"] == DBNull.Value ? string.Empty : reader["bzns_no"].ToString(),
                        StationID = reader["station_id"] == DBNull.Value ? string.Empty : reader["station_id"].ToString(),
                        StationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),

                        PersonRole = reader["prsn_rl"] == DBNull.Value ? string.Empty : reader["prsn_rl"].ToString(),
                        PersonID = reader["prsn_id"] == DBNull.Value ? string.Empty : (reader["prsn_id"]).ToString(),
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
                        BirthDay = reader["birthday"] == DBNull.Value ? (int?)null : (int)reader["birthday"],
                        BirthMonth = reader["birthmonth"] == DBNull.Value ? (int?)null : (int)reader["birthmonth"],
                        BirthYear = reader["birthyear"] == DBNull.Value ? (int?)null : (int)reader["birthyear"],
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
            return businessContactList;
        }
    }
}
