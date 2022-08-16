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
    public class BusinessRepository : IBusinessRepository
    {
        public IConfiguration _config { get; }
        public BusinessRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        //============== Customers Action Methods =====================================================//
        #region Customers Action Methods

        public async Task<Business> GetByIdAsync(string businessId)
        {
            Business business = new Business();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT bzns_id, bzns_name, bzns_type, bzns_addr, bzns_stt, bzns_ctr, bzns_iscs, bzns_issp, ");
            sb.Append("phone1, phone2, email1, email2, weblink1, weblink2, mdb, mdt, ctb, ctt, imgp, bzns_isag, ");
            sb.Append("station_id, bzns_no, l.locname FROM public.gst_bzns b ");
            sb.Append("LEFT OUTER JOIN public.gst_locs l ON b.station_id = l.locqk ");
            sb.Append("WHERE (bzns_id = @bzns_id);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var business_id = cmd.Parameters.Add("@bzns_id", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    business_id.Value = businessId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            business.BusinessID = reader["bzns_id"] == DBNull.Value ? string.Empty : reader["bzns_id"].ToString();
                            business.BusinessName = reader["bzns_name"] == DBNull.Value ? string.Empty : reader["bzns_name"].ToString();
                            business.BusinessNumber = reader["bzns_no"] == DBNull.Value ? string.Empty : reader["bzns_no"].ToString();
                            business.BusinessType = reader["bzns_type"] == DBNull.Value ? string.Empty : reader["bzns_type"].ToString();
                            business.BusinessStationID = reader["station_id"] == DBNull.Value ? 0 : (int)reader["station_id"];
                            business.BusinessStationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString();
                            business.BusinessAddress = reader["bzns_addr"] == DBNull.Value ? string.Empty : reader["bzns_addr"].ToString();
                            business.State = reader["bzns_stt"] == DBNull.Value ? string.Empty : reader["bzns_stt"].ToString();
                            business.Country = reader["bzns_ctr"] == DBNull.Value ? string.Empty : reader["bzns_ctr"].ToString();
                            business.IsCustomer = reader["bzns_iscs"] == DBNull.Value ? false : (bool)reader["bzns_iscs"];
                            business.IsSupplier = reader["bzns_issp"] == DBNull.Value ? false : (bool)reader["bzns_issp"];
                            business.IsAgent = reader["bzns_isag"] == DBNull.Value ? false : (bool)reader["bzns_isag"];
                            business.ImagePath = reader["imgp"] == DBNull.Value ? string.Empty : reader["imgp"].ToString();
                            business.PhoneNo1 = reader["phone1"] == DBNull.Value ? string.Empty : reader["phone1"].ToString();
                            business.PhoneNo2 = reader["phone2"] == DBNull.Value ? string.Empty : reader["phone2"].ToString();
                            business.Email1 = reader["email1"] == DBNull.Value ? string.Empty : reader["email1"].ToString();
                            business.Email2 = reader["email2"] == DBNull.Value ? string.Empty : reader["email2"].ToString();
                            business.WebLink1 = reader["weblink1"] == DBNull.Value ? string.Empty : reader["weblink1"].ToString();
                            business.WebLink2 = reader["weblink2"] == DBNull.Value ? string.Empty : reader["weblink2"].ToString();
                            business.ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString();
                            business.ModifiedTime = reader["mdt"] == DBNull.Value ? string.Empty : reader["mdt"].ToString();
                            business.CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString();
                            business.CreatedTime = reader["ctt"] == DBNull.Value ? string.Empty : reader["ctt"].ToString();
                        }
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message);
            }
            return business;
        }

        public async Task<Business> GetCustomerByNameAsync(string businessName)
        {
            Business business = new Business();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT bzns_id, bzns_name, bzns_type, bzns_addr, bzns_stt, bzns_ctr, bzns_iscs, bzns_issp, ");
            sb.Append("phone1, phone2, email1, email2, weblink1, weblink2, mdb, mdt, ctb, ctt, imgp, bzns_isag, ");
            sb.Append("station_id, bzns_no, l.locname FROM public.gst_bzns b ");
            sb.Append("LEFT OUTER JOIN public.gst_locs l ON b.station_id = l.locqk ");
            sb.Append("WHERE (bzns_name = @bzns_name) AND  (bzns_iscs = true);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var business_name = cmd.Parameters.Add("@bzns_name", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    business_name.Value = businessName;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            business.BusinessID = reader["bzns_id"] == DBNull.Value ? string.Empty : reader["bzns_id"].ToString();
                            business.BusinessName = reader["bzns_name"] == DBNull.Value ? string.Empty : reader["bzns_name"].ToString();
                            business.BusinessNumber = reader["bzns_no"] == DBNull.Value ? string.Empty : reader["bzns_no"].ToString();
                            business.BusinessType = reader["bzns_type"] == DBNull.Value ? string.Empty : reader["bzns_type"].ToString();
                            business.BusinessStationID = reader["station_id"] == DBNull.Value ? 0 : (int)reader["station_id"];
                            business.BusinessStationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString();
                            business.BusinessAddress = reader["bzns_addr"] == DBNull.Value ? string.Empty : reader["bzns_addr"].ToString();
                            business.State = reader["bzns_stt"] == DBNull.Value ? string.Empty : reader["bzns_stt"].ToString();
                            business.Country = reader["bzns_ctr"] == DBNull.Value ? string.Empty : reader["bzns_ctr"].ToString();
                            business.IsCustomer = reader["bzns_iscs"] == DBNull.Value ? false : (bool)reader["bzns_iscs"];
                            business.IsSupplier = reader["bzns_issp"] == DBNull.Value ? false : (bool)reader["bzns_issp"];
                            business.IsAgent = reader["bzns_isag"] == DBNull.Value ? false : (bool)reader["bzns_isag"];
                            business.ImagePath = reader["imgp"] == DBNull.Value ? string.Empty : reader["imgp"].ToString();
                            business.PhoneNo1 = reader["phone1"] == DBNull.Value ? string.Empty : reader["phone1"].ToString();
                            business.PhoneNo2 = reader["phone2"] == DBNull.Value ? string.Empty : reader["phone2"].ToString();
                            business.Email1 = reader["email1"] == DBNull.Value ? string.Empty : reader["email1"].ToString();
                            business.Email2 = reader["email2"] == DBNull.Value ? string.Empty : reader["email2"].ToString();
                            business.WebLink1 = reader["weblink1"] == DBNull.Value ? string.Empty : reader["weblink1"].ToString();
                            business.WebLink2 = reader["weblink2"] == DBNull.Value ? string.Empty : reader["weblink2"].ToString();
                            business.ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString();
                            business.ModifiedTime = reader["mdt"] == DBNull.Value ? string.Empty : reader["mdt"].ToString();
                            business.CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString();
                            business.CreatedTime = reader["ctt"] == DBNull.Value ? string.Empty : reader["ctt"].ToString();
                        }
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message);
            }
            return business;
        }

        public async Task<IList<Business>> SearchCustomersByNameAsync(string businessName)
        {
            List<Business> businessList = new List<Business>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT bzns_id, bzns_name, bzns_type, bzns_addr, bzns_stt, bzns_ctr, bzns_iscs, bzns_issp, ");
            sb.Append("phone1, phone2, email1, email2, weblink1, weblink2, mdb, mdt, ctb, ctt, imgp, bzns_isag, ");
            sb.Append("station_id, bzns_no, l.locname FROM public.gst_bzns b ");
            sb.Append("LEFT OUTER JOIN public.gst_locs l ON b.station_id = l.locqk WHERE (bzns_iscs = true) ");
            sb.Append($"AND (LOWER(bzns_name) LIKE '%'||LOWER(@bzns_name)||'%') ORDER BY bzns_name;");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var bzns_name = cmd.Parameters.Add("@bzns_name", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    bzns_name.Value = businessName ?? string.Empty;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            businessList.Add(new Business()
                            {
                                BusinessID = reader["bzns_id"] == DBNull.Value ? string.Empty : reader["bzns_id"].ToString(),
                            BusinessName = reader["bzns_name"] == DBNull.Value ? string.Empty : reader["bzns_name"].ToString(),
                            BusinessNumber = reader["bzns_no"] == DBNull.Value ? string.Empty : reader["bzns_no"].ToString(),
                            BusinessType = reader["bzns_type"] == DBNull.Value ? string.Empty : reader["bzns_type"].ToString(),
                            BusinessStationID = reader["station_id"] == DBNull.Value ? 0 : (int)reader["station_id"],
                            BusinessStationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            BusinessAddress = reader["bzns_addr"] == DBNull.Value ? string.Empty : reader["bzns_addr"].ToString(),
                            State = reader["bzns_stt"] == DBNull.Value ? string.Empty : reader["bzns_stt"].ToString(),
                            Country = reader["bzns_ctr"] == DBNull.Value ? string.Empty : reader["bzns_ctr"].ToString(),
                            IsCustomer = reader["bzns_iscs"] == DBNull.Value ? false : (bool)reader["bzns_iscs"],
                            IsSupplier = reader["bzns_issp"] == DBNull.Value ? false : (bool)reader["bzns_issp"],
                            IsAgent = reader["bzns_isag"] == DBNull.Value ? false : (bool)reader["bzns_isag"],
                            ImagePath = reader["imgp"] == DBNull.Value ? string.Empty : reader["imgp"].ToString(),
                            PhoneNo1 = reader["phone1"] == DBNull.Value ? string.Empty : reader["phone1"].ToString(),
                            PhoneNo2 = reader["phone2"] == DBNull.Value ? string.Empty : reader["phone2"].ToString(),
                            Email1 = reader["email1"] == DBNull.Value ? string.Empty : reader["email1"].ToString(),
                            Email2 = reader["email2"] == DBNull.Value ? string.Empty : reader["email2"].ToString(),
                            WebLink1 = reader["weblink1"] == DBNull.Value ? string.Empty : reader["weblink1"].ToString(),
                            WebLink2 = reader["weblink2"] == DBNull.Value ? string.Empty : reader["weblink2"].ToString(),
                            ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                            ModifiedTime = reader["mdt"] == DBNull.Value ? string.Empty : reader["mdt"].ToString(),
                            CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                            CreatedTime = reader["ctt"] == DBNull.Value ? string.Empty : reader["ctt"].ToString(),
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
            return businessList;
        }

        public async Task<IList<Business>> GetAllCustomersAsync()
        {
            List<Business> customerList = new List<Business>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT bzns_id, bzns_name, bzns_type, bzns_addr, bzns_stt, bzns_ctr, bzns_iscs, bzns_issp, ");
            sb.Append("phone1, phone2, email1, email2, weblink1, weblink2, mdb, mdt, ctb, ctt, imgp, bzns_isag, ");
            sb.Append("station_id, bzns_no, l.locname FROM public.gst_bzns b ");
            sb.Append("LEFT OUTER JOIN public.gst_locs l ON b.station_id = l.locqk ");
            sb.Append("WHERE (bzns_iscs = true) ORDER BY bzns_name;");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    await cmd.PrepareAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            customerList.Add(new Business()
                            {
                                BusinessID = reader["bzns_id"] == DBNull.Value ? string.Empty : reader["bzns_id"].ToString(),
                                BusinessName = reader["bzns_name"] == DBNull.Value ? string.Empty : reader["bzns_name"].ToString(),
                                BusinessNumber = reader["bzns_no"] == DBNull.Value ? string.Empty : reader["bzns_no"].ToString(),
                                BusinessType = reader["bzns_type"] == DBNull.Value ? string.Empty : reader["bzns_type"].ToString(),
                                BusinessStationID = reader["station_id"] == DBNull.Value ? 0 : (int)reader["station_id"],
                                BusinessStationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                                BusinessAddress = reader["bzns_addr"] == DBNull.Value ? string.Empty : reader["bzns_addr"].ToString(),
                                State = reader["bzns_stt"] == DBNull.Value ? string.Empty : reader["bzns_stt"].ToString(),
                                Country = reader["bzns_ctr"] == DBNull.Value ? string.Empty : reader["bzns_ctr"].ToString(),
                                IsCustomer = reader["bzns_iscs"] == DBNull.Value ? false : (bool)reader["bzns_iscs"],
                                IsSupplier = reader["bzns_issp"] == DBNull.Value ? false : (bool)reader["bzns_issp"],
                                IsAgent = reader["bzns_isag"] == DBNull.Value ? false : (bool)reader["bzns_isag"],
                                ImagePath = reader["imgp"] == DBNull.Value ? string.Empty : reader["imgp"].ToString(),
                                PhoneNo1 = reader["phone1"] == DBNull.Value ? string.Empty : reader["phone1"].ToString(),
                                PhoneNo2 = reader["phone2"] == DBNull.Value ? string.Empty : reader["phone2"].ToString(),
                                Email1 = reader["email1"] == DBNull.Value ? string.Empty : reader["email1"].ToString(),
                                Email2 = reader["email2"] == DBNull.Value ? string.Empty : reader["email2"].ToString(),
                                WebLink1 = reader["weblink1"] == DBNull.Value ? string.Empty : reader["weblink1"].ToString(),
                                WebLink2 = reader["weblink2"] == DBNull.Value ? string.Empty : reader["weblink2"].ToString(),
                                ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                                ModifiedTime = reader["mdt"] == DBNull.Value ? string.Empty : reader["mdt"].ToString(),
                                CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                                CreatedTime = reader["ctt"] == DBNull.Value ? string.Empty : reader["ctt"].ToString(),
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
            return customerList;
        }

        #endregion


        //============== Suppliers Action Methods =====================================================//
        #region Suppliers Action Methods
        public async Task<Business> GetSupplierByNameAsync(string businessName)
        {
            Business business = new Business();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT bzns_id, bzns_name, bzns_type, bzns_addr, bzns_stt, bzns_ctr, bzns_iscs, bzns_issp, ");
            sb.Append("phone1, phone2, email1, email2, weblink1, weblink2, mdb, mdt, ctb, ctt, imgp, bzns_isag, ");
            sb.Append("station_id, bzns_no, l.locname FROM public.gst_bzns b ");
            sb.Append("LEFT OUTER JOIN public.gst_locs l ON b.station_id = l.locqk ");
            sb.Append("WHERE (bzns_name = @bzns_name) AND  (bzns_issp = true);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var business_name = cmd.Parameters.Add("@bzns_name", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    business_name.Value = businessName;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            business.BusinessID = reader["bzns_id"] == DBNull.Value ? string.Empty : reader["bzns_id"].ToString();
                            business.BusinessName = reader["bzns_name"] == DBNull.Value ? string.Empty : reader["bzns_name"].ToString();
                            business.BusinessNumber = reader["bzns_no"] == DBNull.Value ? string.Empty : reader["bzns_no"].ToString();
                            business.BusinessType = reader["bzns_type"] == DBNull.Value ? string.Empty : reader["bzns_type"].ToString();
                            business.BusinessStationID = reader["station_id"] == DBNull.Value ? 0 : (int)reader["station_id"];
                            business.BusinessStationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString();
                            business.BusinessAddress = reader["bzns_addr"] == DBNull.Value ? string.Empty : reader["bzns_addr"].ToString();
                            business.State = reader["bzns_stt"] == DBNull.Value ? string.Empty : reader["bzns_stt"].ToString();
                            business.Country = reader["bzns_ctr"] == DBNull.Value ? string.Empty : reader["bzns_ctr"].ToString();
                            business.IsCustomer = reader["bzns_iscs"] == DBNull.Value ? false : (bool)reader["bzns_iscs"];
                            business.IsSupplier = reader["bzns_issp"] == DBNull.Value ? false : (bool)reader["bzns_issp"];
                            business.IsAgent = reader["bzns_isag"] == DBNull.Value ? false : (bool)reader["bzns_isag"];
                            business.ImagePath = reader["imgp"] == DBNull.Value ? string.Empty : reader["imgp"].ToString();
                            business.PhoneNo1 = reader["phone1"] == DBNull.Value ? string.Empty : reader["phone1"].ToString();
                            business.PhoneNo2 = reader["phone2"] == DBNull.Value ? string.Empty : reader["phone2"].ToString();
                            business.Email1 = reader["email1"] == DBNull.Value ? string.Empty : reader["email1"].ToString();
                            business.Email2 = reader["email2"] == DBNull.Value ? string.Empty : reader["email2"].ToString();
                            business.WebLink1 = reader["weblink1"] == DBNull.Value ? string.Empty : reader["weblink1"].ToString();
                            business.WebLink2 = reader["weblink2"] == DBNull.Value ? string.Empty : reader["weblink2"].ToString();
                            business.ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString();
                            business.ModifiedTime = reader["mdt"] == DBNull.Value ? string.Empty : reader["mdt"].ToString();
                            business.CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString();
                            business.CreatedTime = reader["ctt"] == DBNull.Value ? string.Empty : reader["ctt"].ToString();
                        }
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message);
            }
            return business;
        }

        public async Task<IList<Business>> SearchSuppliersByNameAsync(string businessName)
        {
            List<Business> businessList = new List<Business>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT bzns_id, bzns_name, bzns_type, bzns_addr, bzns_stt, bzns_ctr, bzns_iscs, bzns_issp, ");
            sb.Append("phone1, phone2, email1, email2, weblink1, weblink2, mdb, mdt, ctb, ctt, imgp, bzns_isag, ");
            sb.Append("station_id, bzns_no, l.locname FROM public.gst_bzns b ");
            sb.Append("LEFT OUTER JOIN public.gst_locs l ON b.station_id = l.locqk WHERE (bzns_issp = true) ");
            sb.Append($"AND (LOWER(bzns_name) LIKE '%'||LOWER(@bzns_name)||'%') ORDER BY bzns_name;");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var bzns_name = cmd.Parameters.Add("@bzns_name", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    bzns_name.Value = businessName ?? string.Empty;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            businessList.Add(new Business()
                            {
                                BusinessID = reader["bzns_id"] == DBNull.Value ? string.Empty : reader["bzns_id"].ToString(),
                                BusinessName = reader["bzns_name"] == DBNull.Value ? string.Empty : reader["bzns_name"].ToString(),
                                BusinessNumber = reader["bzns_no"] == DBNull.Value ? string.Empty : reader["bzns_no"].ToString(),
                                BusinessType = reader["bzns_type"] == DBNull.Value ? string.Empty : reader["bzns_type"].ToString(),
                                BusinessStationID = reader["station_id"] == DBNull.Value ? 0 : (int)reader["station_id"],
                                BusinessStationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                                BusinessAddress = reader["bzns_addr"] == DBNull.Value ? string.Empty : reader["bzns_addr"].ToString(),
                                State = reader["bzns_stt"] == DBNull.Value ? string.Empty : reader["bzns_stt"].ToString(),
                                Country = reader["bzns_ctr"] == DBNull.Value ? string.Empty : reader["bzns_ctr"].ToString(),
                                IsCustomer = reader["bzns_iscs"] == DBNull.Value ? false : (bool)reader["bzns_iscs"],
                                IsSupplier = reader["bzns_issp"] == DBNull.Value ? false : (bool)reader["bzns_issp"],
                                IsAgent = reader["bzns_isag"] == DBNull.Value ? false : (bool)reader["bzns_isag"],
                                ImagePath = reader["imgp"] == DBNull.Value ? string.Empty : reader["imgp"].ToString(),
                                PhoneNo1 = reader["phone1"] == DBNull.Value ? string.Empty : reader["phone1"].ToString(),
                                PhoneNo2 = reader["phone2"] == DBNull.Value ? string.Empty : reader["phone2"].ToString(),
                                Email1 = reader["email1"] == DBNull.Value ? string.Empty : reader["email1"].ToString(),
                                Email2 = reader["email2"] == DBNull.Value ? string.Empty : reader["email2"].ToString(),
                                WebLink1 = reader["weblink1"] == DBNull.Value ? string.Empty : reader["weblink1"].ToString(),
                                WebLink2 = reader["weblink2"] == DBNull.Value ? string.Empty : reader["weblink2"].ToString(),
                                ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                                ModifiedTime = reader["mdt"] == DBNull.Value ? string.Empty : reader["mdt"].ToString(),
                                CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                                CreatedTime = reader["ctt"] == DBNull.Value ? string.Empty : reader["ctt"].ToString(),
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
            return businessList;
        }

        public async Task<IList<Business>> GetAllSuppliersAsync()
        {
            List<Business> customerList = new List<Business>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT bzns_id, bzns_name, bzns_type, bzns_addr, bzns_stt, bzns_ctr, bzns_iscs, bzns_issp, ");
            sb.Append("phone1, phone2, email1, email2, weblink1, weblink2, mdb, mdt, ctb, ctt, imgp, bzns_isag, ");
            sb.Append("station_id, bzns_no, l.locname FROM public.gst_bzns b ");
            sb.Append("LEFT OUTER JOIN public.gst_locs l ON b.station_id = l.locqk ");
            sb.Append("WHERE (bzns_issp = true) ORDER BY bzns_name;");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    await cmd.PrepareAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            customerList.Add(new Business()
                            {
                                BusinessID = reader["bzns_id"] == DBNull.Value ? string.Empty : reader["bzns_id"].ToString(),
                                BusinessName = reader["bzns_name"] == DBNull.Value ? string.Empty : reader["bzns_name"].ToString(),
                                BusinessNumber = reader["bzns_no"] == DBNull.Value ? string.Empty : reader["bzns_no"].ToString(),
                                BusinessType = reader["bzns_type"] == DBNull.Value ? string.Empty : reader["bzns_type"].ToString(),
                                BusinessStationID = reader["station_id"] == DBNull.Value ? 0 : (int)reader["station_id"],
                                BusinessStationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                                BusinessAddress = reader["bzns_addr"] == DBNull.Value ? string.Empty : reader["bzns_addr"].ToString(),
                                State = reader["bzns_stt"] == DBNull.Value ? string.Empty : reader["bzns_stt"].ToString(),
                                Country = reader["bzns_ctr"] == DBNull.Value ? string.Empty : reader["bzns_ctr"].ToString(),
                                IsCustomer = reader["bzns_iscs"] == DBNull.Value ? false : (bool)reader["bzns_iscs"],
                                IsSupplier = reader["bzns_issp"] == DBNull.Value ? false : (bool)reader["bzns_issp"],
                                IsAgent = reader["bzns_isag"] == DBNull.Value ? false : (bool)reader["bzns_isag"],
                                ImagePath = reader["imgp"] == DBNull.Value ? string.Empty : reader["imgp"].ToString(),
                                PhoneNo1 = reader["phone1"] == DBNull.Value ? string.Empty : reader["phone1"].ToString(),
                                PhoneNo2 = reader["phone2"] == DBNull.Value ? string.Empty : reader["phone2"].ToString(),
                                Email1 = reader["email1"] == DBNull.Value ? string.Empty : reader["email1"].ToString(),
                                Email2 = reader["email2"] == DBNull.Value ? string.Empty : reader["email2"].ToString(),
                                WebLink1 = reader["weblink1"] == DBNull.Value ? string.Empty : reader["weblink1"].ToString(),
                                WebLink2 = reader["weblink2"] == DBNull.Value ? string.Empty : reader["weblink2"].ToString(),
                                ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                                ModifiedTime = reader["mdt"] == DBNull.Value ? string.Empty : reader["mdt"].ToString(),
                                CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                                CreatedTime = reader["ctt"] == DBNull.Value ? string.Empty : reader["ctt"].ToString(),
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
            return customerList;
        }

        #endregion

        //============== Business CRUD Action Methods =================================================//
        #region Business CRUD Action Methods
        public async Task<bool> AddAsync(Business business)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.gst_bzns(bzns_id, bzns_name, bzns_type, bzns_addr, bzns_stt, ");
            sb.Append("bzns_ctr, bzns_iscs, bzns_issp, phone1, phone2, email1, email2, weblink1, weblink2, ");
            sb.Append("mdb, mdt, ctb, ctt, imgp, bzns_isag, station_id, bzns_no) VALUES (@bzns_id, @bzns_name, ");
            sb.Append("@bzns_type, @bzns_addr, @bzns_stt, @bzns_ctr, @bzns_iscs, @bzns_issp, @phone1, ");
            sb.Append("@phone2, @email1, @email2, @weblink1, @weblink2, @mdb, @mdt, @ctb, @ctt, @imgp, ");
            sb.Append("@bzns_isag, @station_id, @bzns_no);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var bzns_id = cmd.Parameters.Add("@bzns_id", NpgsqlDbType.Text);
                    var bzns_name = cmd.Parameters.Add("@bzns_name", NpgsqlDbType.Text);
                    var bzns_type = cmd.Parameters.Add("@bzns_type", NpgsqlDbType.Text);
                    var bzns_addr = cmd.Parameters.Add("@bzns_addr", NpgsqlDbType.Text);
                    var bzns_stt = cmd.Parameters.Add("@bzns_stt", NpgsqlDbType.Text);
                    var bzns_ctr = cmd.Parameters.Add("@bzns_ctr", NpgsqlDbType.Text);
                    var bzns_iscs = cmd.Parameters.Add("@bzns_iscs", NpgsqlDbType.Boolean);
                    var bzns_issp = cmd.Parameters.Add("@bzns_issp", NpgsqlDbType.Boolean);
                    var bzns_isag = cmd.Parameters.Add("@bzns_isag", NpgsqlDbType.Boolean);
                    var phone1 = cmd.Parameters.Add("@phone1", NpgsqlDbType.Text);
                    var phone2 = cmd.Parameters.Add("@phone2", NpgsqlDbType.Text);
                    var email1 = cmd.Parameters.Add("@email1", NpgsqlDbType.Text);
                    var email2 = cmd.Parameters.Add("@email2", NpgsqlDbType.Text);
                    var weblink1 = cmd.Parameters.Add("@weblink1", NpgsqlDbType.Text);
                    var weblink2 = cmd.Parameters.Add("@weblink2", NpgsqlDbType.Text);
                    var imgp = cmd.Parameters.Add("@imgp", NpgsqlDbType.Text);
                    var mdb = cmd.Parameters.Add("@mdb", NpgsqlDbType.Text);
                    var mdt = cmd.Parameters.Add("@mdt", NpgsqlDbType.Text);
                    var ctb = cmd.Parameters.Add("@ctb", NpgsqlDbType.Text);
                    var ctt = cmd.Parameters.Add("@ctt", NpgsqlDbType.Text);
                    var station_id = cmd.Parameters.Add("@station_id", NpgsqlDbType.Integer);
                    var bzns_no = cmd.Parameters.Add("@bzns_no", NpgsqlDbType.Text);
                    cmd.Prepare();
                    bzns_id.Value = business.BusinessID ?? Guid.NewGuid().ToString();
                    bzns_name.Value = business.BusinessName;
                    bzns_type.Value = business.BusinessType ?? (object)DBNull.Value;
                    bzns_addr.Value = business.BusinessAddress ?? (object)DBNull.Value;
                    bzns_stt.Value = business.State ?? (object)DBNull.Value;
                    bzns_ctr.Value = business.Country ?? (object)DBNull.Value;
                    bzns_iscs.Value = business.IsCustomer;
                    bzns_issp.Value = business.IsSupplier;
                    bzns_isag.Value = business.IsAgent;
                    phone1.Value = business.PhoneNo1 ?? (object)DBNull.Value;
                    phone2.Value = business.PhoneNo2 ?? (object)DBNull.Value;
                    email1.Value = business.Email1 ?? (object)DBNull.Value;
                    email2.Value = business.Email2 ?? (object)DBNull.Value;
                    weblink1.Value = business.WebLink1 ?? (object)DBNull.Value;
                    weblink2.Value = business.WebLink2 ?? (object)DBNull.Value;
                    mdb.Value = business.ModifiedBy ?? (object)DBNull.Value;
                    mdt.Value = business.ModifiedTime ?? (object)DBNull.Value;
                    ctb.Value = business.CreatedBy ?? (object)DBNull.Value;
                    ctt.Value = business.CreatedTime ?? (object)DBNull.Value;
                    imgp.Value = business.ImagePath ?? (object)DBNull.Value;
                    station_id.Value = business.BusinessStationID ?? (object)DBNull.Value;
                    bzns_no.Value = business.BusinessNumber ?? (object)DBNull.Value;
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

        public async Task<bool> EditAsync(Business business)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.gst_bzns SET  bzns_name = @bzns_name, bzns_type = @bzns_type, ");
            sb.Append("bzns_addr = @bzns_addr, bzns_stt = @bzns_stt, bzns_ctr = @bzns_ctr, ");
            sb.Append("phone1 = @phone1, phone2 = @phone2, email1 = @email1, email2 = @email2, ");
            sb.Append("weblink1 = @weblink1, weblink2 = @weblink2, mdb = @mdb, mdt = @mdt, ");
            sb.Append("imgp = @imgp, station_id = @station_id, bzns_no = @bzns_no  ");
            sb.Append("WHERE (bzns_id = @bzns_id);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var bzns_id = cmd.Parameters.Add("@bzns_id", NpgsqlDbType.Text);
                    var bzns_name = cmd.Parameters.Add("@bzns_name", NpgsqlDbType.Text);
                    var bzns_type = cmd.Parameters.Add("@bzns_type", NpgsqlDbType.Text);
                    var bzns_addr = cmd.Parameters.Add("@bzns_addr", NpgsqlDbType.Text);
                    var bzns_stt = cmd.Parameters.Add("@bzns_stt", NpgsqlDbType.Text);
                    var bzns_ctr = cmd.Parameters.Add("@bzns_ctr", NpgsqlDbType.Text);
                    var phone1 = cmd.Parameters.Add("@phone1", NpgsqlDbType.Text);
                    var phone2 = cmd.Parameters.Add("@phone2", NpgsqlDbType.Text);
                    var email1 = cmd.Parameters.Add("@email1", NpgsqlDbType.Text);
                    var email2 = cmd.Parameters.Add("@email2", NpgsqlDbType.Text);
                    var weblink1 = cmd.Parameters.Add("@weblink1", NpgsqlDbType.Text);
                    var weblink2 = cmd.Parameters.Add("@weblink2", NpgsqlDbType.Text);
                    var imgp = cmd.Parameters.Add("@imgp", NpgsqlDbType.Text);
                    var mdb = cmd.Parameters.Add("@mdb", NpgsqlDbType.Text);
                    var mdt = cmd.Parameters.Add("@mdt", NpgsqlDbType.Text);
                    var station_id = cmd.Parameters.Add("@station_id", NpgsqlDbType.Integer);
                    var bzns_no = cmd.Parameters.Add("@bzns_no", NpgsqlDbType.Text);
                    cmd.Prepare();
                    bzns_id.Value = business.BusinessID ?? Guid.NewGuid().ToString();
                    bzns_name.Value = business.BusinessName;
                    bzns_type.Value = business.BusinessType ?? (object)DBNull.Value;
                    bzns_addr.Value = business.BusinessAddress ?? (object)DBNull.Value;
                    bzns_stt.Value = business.State ?? (object)DBNull.Value;
                    bzns_ctr.Value = business.Country ?? (object)DBNull.Value;
                    phone1.Value = business.PhoneNo1 ?? (object)DBNull.Value;
                    phone2.Value = business.PhoneNo2 ?? (object)DBNull.Value;
                    email1.Value = business.Email1 ?? (object)DBNull.Value;
                    email2.Value = business.Email2 ?? (object)DBNull.Value;
                    weblink1.Value = business.WebLink1 ?? (object)DBNull.Value;
                    weblink2.Value = business.WebLink2 ?? (object)DBNull.Value;
                    mdb.Value = business.ModifiedBy ?? (object)DBNull.Value;
                    mdt.Value = business.ModifiedTime ?? (object)DBNull.Value;
                    imgp.Value = business.ImagePath ?? (object)DBNull.Value;
                    station_id.Value = business.BusinessStationID ?? (object)DBNull.Value;
                    bzns_no.Value = business.BusinessNumber ?? (object)DBNull.Value;

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

        public async Task<bool> DeleteAsync(string businessId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = $"DELETE FROM public.gst_bzns  WHERE (bzns_id = @bzns_id);";
            try
            {
                await conn.OpenAsync();
                //Insert data
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

        #endregion
    }
}
