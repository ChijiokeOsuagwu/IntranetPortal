using IntranetPortal.Base.Models.BaseModels;
using IntranetPortal.Base.Models.PartnerServicesModels;
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
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.bpm_bzns_prsns(contact_nm, sex, ");
            sb.Append("phone1, phone2, email1, email2, address, mdb, mdt, ctb, ");
            sb.Append("ctt, bzns_id, designation) ");
            sb.Append("VALUES (@contact_nm, @sex, @phone1, @phone2, ");
            sb.Append("@email1, @email2, @address, @mdb, @mdt, @ctb, @ctt, ");
            sb.Append("@bzns_id, @designation); ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var contact_nm = cmd.Parameters.Add("@contact_nm", NpgsqlDbType.Text);
                    var sex = cmd.Parameters.Add("@sex", NpgsqlDbType.Text);
                    var bzns_id = cmd.Parameters.Add("@bzns_id", NpgsqlDbType.Text);
                    var phone1 = cmd.Parameters.Add("@phone1", NpgsqlDbType.Text);
                    var phone2 = cmd.Parameters.Add("@phone2", NpgsqlDbType.Text);
                    var email1 = cmd.Parameters.Add("@email1", NpgsqlDbType.Text);
                    var email2 = cmd.Parameters.Add("@email2", NpgsqlDbType.Text);
                    var address = cmd.Parameters.Add("@address", NpgsqlDbType.Text);
                    var mdb = cmd.Parameters.Add("@mdb", NpgsqlDbType.Text);
                    var mdt = cmd.Parameters.Add("@mdt", NpgsqlDbType.Timestamp);
                    var ctb = cmd.Parameters.Add("@ctb", NpgsqlDbType.Text);
                    var ctt = cmd.Parameters.Add("@ctt", NpgsqlDbType.Timestamp);
                    var designation = cmd.Parameters.Add("@designation", NpgsqlDbType.Text);
                    cmd.Prepare();
                    contact_nm.Value = businessContact.ContactName;
                    sex.Value = businessContact.Sex ?? (object)DBNull.Value;
                    bzns_id.Value = businessContact.BusinessID;
                    phone1.Value = businessContact.ContactPhone1 ?? (object)DBNull.Value; 
                    phone2.Value = businessContact.ContactPhone2 ?? (object)DBNull.Value;
                    email1.Value = businessContact.ContactEmail1 ?? (object)DBNull.Value;
                    email2.Value = businessContact.ContactEmail2 ?? (object)DBNull.Value;
                    address.Value = businessContact.ContactAddress ?? (object)DBNull.Value;
                    mdb.Value = businessContact.ModifiedBy ?? (object)DBNull.Value;
                    mdt.Value = businessContact.ModifiedTime ?? DateTime.Now;
                    ctb.Value = businessContact.CreatedBy ?? (object)DBNull.Value;
                    ctt.Value = businessContact.CreatedTime ?? DateTime.Now;
                    designation.Value = businessContact.Designation ?? (object)DBNull.Value;

                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        public async Task<bool> EditAsync(BusinessContact businessContact)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.bpm_bzns_prsns SET contact_nm=@contact_nm,");
            sb.Append("sex=@sex, phone1=@phone1, phone2=@phone2, email1=@email1, ");
            sb.Append("email2=@email2, address=@address, mdb=@mdb, mdt=@mdt, ");
            sb.Append("designation=@designation ");
            sb.Append("WHERE(contact_id= @contact_id);");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var contact_id = cmd.Parameters.Add("@contact_id", NpgsqlDbType.Bigint);
                    var contact_nm = cmd.Parameters.Add("@contact_nm", NpgsqlDbType.Text);
                    var sex = cmd.Parameters.Add("@sex", NpgsqlDbType.Text);
                    var phone1 = cmd.Parameters.Add("@phone1", NpgsqlDbType.Text);
                    var phone2 = cmd.Parameters.Add("@phone2", NpgsqlDbType.Text);
                    var email1 = cmd.Parameters.Add("@email1", NpgsqlDbType.Text);
                    var email2 = cmd.Parameters.Add("@email2", NpgsqlDbType.Text);
                    var address = cmd.Parameters.Add("@address", NpgsqlDbType.Text);
                    var mdb = cmd.Parameters.Add("@mdb", NpgsqlDbType.Text);
                    var mdt = cmd.Parameters.Add("@mdt", NpgsqlDbType.Timestamp);
                    var designation = cmd.Parameters.Add("@designation", NpgsqlDbType.Text);
                    cmd.Prepare();
                    contact_id.Value = businessContact.ContactID;
                    contact_nm.Value = businessContact.ContactName;
                    sex.Value = businessContact.Sex ?? (object)DBNull.Value;
                    phone1.Value = businessContact.ContactPhone1 ?? (object)DBNull.Value;
                    phone2.Value = businessContact.ContactPhone2 ?? (object)DBNull.Value;
                    email1.Value = businessContact.ContactEmail1 ?? (object)DBNull.Value;
                    email2.Value = businessContact.ContactEmail2 ?? (object)DBNull.Value;
                    address.Value = businessContact.ContactAddress ?? (object)DBNull.Value;
                    mdb.Value = businessContact.ModifiedBy ?? (object)DBNull.Value;
                    mdt.Value = businessContact.ModifiedTime ?? DateTime.Now;
                    designation.Value = businessContact.Designation ?? (object)DBNull.Value;

                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        public async Task<bool> DeleteAsync(long businessContactId)
        {
            int rows = 0;
            string query = $"DELETE FROM public.bpm_bzns_prsns WHERE (contact_id = @contact_id);";
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Delete data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var contact_id = cmd.Parameters.Add("@contact_id", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    contact_id.Value = businessContactId;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        public async Task<bool> DeleteByBusinessIdAsync(string businessContactId)
        {
            int rows = 0;
            string query = $"DELETE FROM public.bpm_bzns_prsns WHERE (bzns_id = @bzns_id);";
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Delete data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var bzns_id = cmd.Parameters.Add("@bzns_id", NpgsqlDbType.Text);
                    cmd.Prepare();
                    bzns_id.Value = businessContactId;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        public async Task<BusinessContact> GetByIdAsync(long businessContactId)
        {
            BusinessContact person = new BusinessContact();
            StringBuilder sb = new StringBuilder();
            if (businessContactId < 1) { throw new ArgumentNullException("Required parameter [businessContactId] cannot be null."); }
            sb.Append("SELECT p.contact_id, p.contact_nm, p.sex, p.phone1, p.phone2,");
            sb.Append("p.email1, p.email2, p.address, p.mdb, p.mdt, p.ctb, p.ctt, p.bzns_id, ");
            sb.Append("(SELECT bzns_nm FROM public.bpm_bzns_inf WHERE bzns_id = p.bzns_id) as bzns_nm, ");
            sb.Append("p.designation FROM public.bpm_bzns_prsns p ");
            sb.Append("WHERE(p.is_dx = false) AND (p.contact_id = @contact_id);");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var contact_id = cmd.Parameters.Add("@contact_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    contact_id.Value = businessContactId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            person.ContactID = reader["contact_id"] == DBNull.Value ? 0L : (long)reader["contact_id"];
                            person.ContactName = reader["contact_nm"] == DBNull.Value ? string.Empty : reader["contact_nm"].ToString();
                            person.Sex = reader["sex"] == DBNull.Value ? string.Empty : reader["sex"].ToString();
                            person.ContactPhone1 = reader["phone1"] == DBNull.Value ? string.Empty : reader["phone1"].ToString();
                            person.ContactPhone2 = reader["phone2"] == DBNull.Value ? string.Empty : reader["phone2"].ToString();
                            person.ContactEmail1 = reader["email1"] == DBNull.Value ? string.Empty : reader["email1"].ToString();
                            person.ContactEmail2 = reader["email2"] == DBNull.Value ? string.Empty : reader["email2"].ToString();
                            person.ContactAddress = reader["address"] == DBNull.Value ? string.Empty : reader["address"].ToString();
                            person.BusinessID = reader["bzns_id"] == DBNull.Value ? string.Empty : reader["bzns_id"].ToString();
                            person.BusinessName = reader["bzns_nm"] == DBNull.Value ? string.Empty : reader["bzns_nm"].ToString();
                            person.Designation = reader["designation"] == DBNull.Value ? string.Empty : reader["designation"].ToString();
                            person.ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString();
                            person.ModifiedTime = reader["mdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mdt"];
                            person.CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString();
                            person.CreatedTime = reader["ctt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ctt"];
                        }
                }
                await conn.CloseAsync();
            }
            return person;
        }
        public async Task<List<BusinessContact>> GetAllAsync()
        {
            List<BusinessContact> businessContactList = new List<BusinessContact>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT p.contact_id, p.contact_nm, p.sex, p.phone1, p.phone2,");
            sb.Append("p.email1, p.email2, p.address, p.mdb, p.mdt, p.ctb, p.ctt, p.bzns_id, ");
            sb.Append("(SELECT bzns_nm FROM public.bpm_bzns_inf WHERE bzns_id = p.bzns_id) as bzns_nm, ");
            sb.Append("p.designation FROM public.bpm_bzns_prsns p ");
            sb.Append("WHERE(p.is_dx = false) ORDER BY p.contact_nm;");
            string query = sb.ToString();
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
                        businessContactList.Add(new BusinessContact()
                        {
                            ContactID = reader["contact_id"] == DBNull.Value ? 0L : (long)reader["contact_id"],
                            ContactName = reader["contact_nm"] == DBNull.Value ? string.Empty : reader["contact_nm"].ToString(),
                            Sex = reader["sex"] == DBNull.Value ? string.Empty : reader["sex"].ToString(),
                            ContactPhone1 = reader["phone1"] == DBNull.Value ? string.Empty : reader["phone1"].ToString(),
                            ContactPhone2 = reader["phone2"] == DBNull.Value ? string.Empty : reader["phone2"].ToString(),
                            ContactEmail1 = reader["email1"] == DBNull.Value ? string.Empty : reader["email1"].ToString(),
                            ContactEmail2 = reader["email2"] == DBNull.Value ? string.Empty : reader["email2"].ToString(),
                            ContactAddress = reader["address"] == DBNull.Value ? string.Empty : reader["address"].ToString(),
                            BusinessID = reader["bzns_id"] == DBNull.Value ? string.Empty : reader["bzns_id"].ToString(),
                            BusinessName = reader["bzns_nm"] == DBNull.Value ? string.Empty : reader["bzns_nm"].ToString(),
                            Designation = reader["designation"] == DBNull.Value ? string.Empty : reader["designation"].ToString(),
                            ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                            ModifiedTime = reader["mdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mdt"],
                            CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                            CreatedTime = reader["ctt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ctt"],
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return businessContactList;
        }
        public async Task<List<BusinessContact>> GetByBusinessIdAsync(string businessId)
        {
            List<BusinessContact> businessContactList = new List<BusinessContact>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT p.contact_id, p.contact_nm, p.sex, p.phone1, p.phone2,");
            sb.Append("p.email1, p.email2, p.address, p.mdb, p.mdt, p.ctb, p.ctt, p.bzns_id, ");
            sb.Append("(SELECT bzns_nm FROM public.bpm_bzns_inf WHERE bzns_id = p.bzns_id) as bzns_nm, ");
            sb.Append("p.designation FROM public.bpm_bzns_prsns p ");
            sb.Append("WHERE(p.is_dx = false) AND (p.bzns_id = @bzns_id) ");
            sb.Append("ORDER BY p.contact_nm;");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
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
                            ContactID = reader["contact_id"] == DBNull.Value ? 0L : (long)reader["contact_id"],
                            ContactName = reader["contact_nm"] == DBNull.Value ? string.Empty : reader["contact_nm"].ToString(),
                            Sex = reader["sex"] == DBNull.Value ? string.Empty : reader["sex"].ToString(),
                            ContactPhone1 = reader["phone1"] == DBNull.Value ? string.Empty : reader["phone1"].ToString(),
                            ContactPhone2 = reader["phone2"] == DBNull.Value ? string.Empty : reader["phone2"].ToString(),
                            ContactEmail1 = reader["email1"] == DBNull.Value ? string.Empty : reader["email1"].ToString(),
                            ContactEmail2 = reader["email2"] == DBNull.Value ? string.Empty : reader["email2"].ToString(),
                            ContactAddress = reader["address"] == DBNull.Value ? string.Empty : reader["address"].ToString(),
                            BusinessID = reader["bzns_id"] == DBNull.Value ? string.Empty : reader["bzns_id"].ToString(),
                            BusinessName = reader["bzns_nm"] == DBNull.Value ? string.Empty : reader["bzns_nm"].ToString(),
                            Designation = reader["designation"] == DBNull.Value ? string.Empty : reader["designation"].ToString(),
                            ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                            ModifiedTime = reader["mdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mdt"],
                            CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                            CreatedTime = reader["ctt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ctt"],
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return businessContactList;
        }
    }
}
