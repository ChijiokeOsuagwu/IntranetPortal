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
    public class BusinessRepository : IBusinessRepository
    {
        public IConfiguration _config { get; }
        public BusinessRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        //====== Business Partner Action Methods ======//
        #region Business Partner Action Methods
        public async Task<List<string>> GetCodeNumbersByCreatedDateAsync(DateTime createdDate)
        {
            List<string> listOfCodeNumbers = new List<string>();
            string _newNumber = string.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT bzns_no FROM public.bpm_bzns_inf ");
            sb.Append("WHERE date_part('year', ctt) = date_part('year', @ctt) ");
            sb.Append("ORDER BY bzns_no DESC; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var ctt = cmd.Parameters.Add("@ctt", NpgsqlDbType.Timestamp);
                    await cmd.PrepareAsync();
                    ctt.Value = createdDate;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            _newNumber = reader["bzns_no"] == DBNull.Value ? string.Empty : reader["bzns_no"].ToString();
                            listOfCodeNumbers.Add(_newNumber);
                        }
                }
                await conn.CloseAsync();
            }
            return listOfCodeNumbers;
        }

        #endregion

        //====== Customers Action Methods =============//
        #region Customers Action Methods

        public async Task<Business> GetByIdAsync(string businessId)
        {
            Business business = new Business();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT b.bzns_id, b.bzns_no, b.bzns_nm, b.bzns_addr, b.bzns_stt, ");
            sb.Append("b.bzns_ctr, b.bzns_iscs, b.bzns_issp, b.bzns_isag, b.phone1, ");
            sb.Append("b.phone2, b.email1, b.email2, b.weblink1, b.weblink2, b.mdb, b.mdt, ");
            sb.Append("b.ctb, b.ctt, b.imgp, b.station_id, b.typ_id, b.ind_id, ");
            sb.Append("(SELECT locname FROM public.gst_locs WHERE locqk = b.station_id) as station_nm, ");
            sb.Append("(SELECT bzn_typ_nm FROM public.bpm_bzn_typs WHERE bzn_typ_id = b.typ_id) as typ_nm, ");
            sb.Append("(SELECT bzn_ind_nm FROM public.bpm_bzn_inds WHERE bzn_ind_id = b.ind_id) as ind_nm ");
            sb.Append("FROM public.bpm_bzns_inf b  ");
            sb.Append("WHERE (b.bzns_id = @bzns_id);");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
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
                            business.BusinessNumber = reader["bzns_no"] == DBNull.Value ? string.Empty : reader["bzns_no"].ToString();
                            business.BusinessName = reader["bzns_nm"] == DBNull.Value ? string.Empty : reader["bzns_nm"].ToString();
                            business.BusinessTypeId = reader["typ_id"] == DBNull.Value ? 0 : (int)reader["typ_id"];
                            business.BusinessType = reader["typ_nm"] == DBNull.Value ? string.Empty : reader["typ_nm"].ToString();
                            business.IndustrySectorId = reader["ind_id"] == DBNull.Value ? 0 : (int)reader["ind_id"];
                            business.IndustrySector = reader["ind_nm"] == DBNull.Value ? string.Empty : reader["ind_nm"].ToString();
                            business.BusinessAddress = reader["bzns_addr"] == DBNull.Value ? string.Empty : reader["bzns_addr"].ToString();
                            business.State = reader["bzns_stt"] == DBNull.Value ? string.Empty : reader["bzns_stt"].ToString();
                            business.Country = reader["bzns_ctr"] == DBNull.Value ? string.Empty : reader["bzns_ctr"].ToString();
                            business.IsCustomer = reader["bzns_iscs"] == DBNull.Value ? false : (bool)reader["bzns_iscs"];
                            business.IsSupplier = reader["bzns_issp"] == DBNull.Value ? false : (bool)reader["bzns_issp"];
                            business.IsAgent = reader["bzns_isag"] == DBNull.Value ? false : (bool)reader["bzns_isag"];
                            business.PhoneNo1 = reader["phone1"] == DBNull.Value ? string.Empty : reader["phone1"].ToString();
                            business.PhoneNo2 = reader["phone2"] == DBNull.Value ? string.Empty : reader["phone2"].ToString();
                            business.Email1 = reader["email1"] == DBNull.Value ? string.Empty : reader["email1"].ToString();
                            business.Email2 = reader["email2"] == DBNull.Value ? string.Empty : reader["email2"].ToString();
                            business.WebLink1 = reader["weblink1"] == DBNull.Value ? string.Empty : reader["weblink1"].ToString();
                            business.WebLink2 = reader["weblink2"] == DBNull.Value ? string.Empty : reader["weblink2"].ToString();
                            business.BusinessStationId = reader["station_id"] == DBNull.Value ? 0 : (int)reader["station_id"];
                            business.BusinessStationName = reader["station_nm"] == DBNull.Value ? string.Empty : reader["station_nm"].ToString();
                            business.ImagePath = reader["imgp"] == DBNull.Value ? string.Empty : reader["imgp"].ToString();
                            business.ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString();
                            business.ModifiedTime = reader["mdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mdt"];
                            business.CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString();
                            business.CreatedTime = reader["ctt"] == DBNull.Value ? (DateTime?)null : (DateTime?)reader["ctt"];
                        }
                }
                await conn.CloseAsync();
            }
            return business;
        }
        public async Task<Business> GetCustomerByNameAsync(string businessName)
        {
            Business business = new Business();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT b.bzns_id, b.bzns_no, b.bzns_nm, b.bzns_addr, b.bzns_stt, ");
            sb.Append("b.bzns_ctr, b.bzns_iscs, b.bzns_issp, b.bzns_isag, b.phone1, ");
            sb.Append("b.phone2, b.email1, b.email2, b.weblink1, b.weblink2, b.mdb, b.mdt, ");
            sb.Append("b.ctb, b.ctt, b.imgp, b.station_id, b.typ_id, b.ind_id, ");
            sb.Append("(SELECT locname FROM public.gst_locs WHERE locqk = b.station_id) as station_nm, ");
            sb.Append("(SELECT bzn_typ_nm FROM public.bpm_bzn_typs WHERE bzn_typ_id = b.typ_id) as typ_nm, ");
            sb.Append("(SELECT bzn_ind_nm FROM public.bpm_bzn_inds WHERE bzn_ind_id = b.ind_id) as ind_nm ");
            sb.Append("FROM public.bpm_bzns_inf b  ");
            sb.Append("WHERE (b.bzns_nm = @bzns_nm) AND (bzns_iscs = true);");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var bzns_nm = cmd.Parameters.Add("@bzns_nm", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    bzns_nm.Value = businessName;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            business.BusinessID = reader["bzns_id"] == DBNull.Value ? string.Empty : reader["bzns_id"].ToString();
                            business.BusinessNumber = reader["bzns_no"] == DBNull.Value ? string.Empty : reader["bzns_no"].ToString();
                            business.BusinessName = reader["bzns_nm"] == DBNull.Value ? string.Empty : reader["bzns_nm"].ToString();
                            business.BusinessTypeId = reader["typ_id"] == DBNull.Value ? 0 : (int)reader["typ_id"];
                            business.BusinessType = reader["typ_nm"] == DBNull.Value ? string.Empty : reader["typ_nm"].ToString();
                            business.IndustrySectorId = reader["ind_id"] == DBNull.Value ? 0 : (int)reader["ind_id"];
                            business.IndustrySector = reader["ind_nm"] == DBNull.Value ? string.Empty : reader["ind_nm"].ToString();
                            business.BusinessAddress = reader["bzns_addr"] == DBNull.Value ? string.Empty : reader["bzns_addr"].ToString();
                            business.State = reader["bzns_stt"] == DBNull.Value ? string.Empty : reader["bzns_stt"].ToString();
                            business.Country = reader["bzns_ctr"] == DBNull.Value ? string.Empty : reader["bzns_ctr"].ToString();
                            business.IsCustomer = reader["bzns_iscs"] == DBNull.Value ? false : (bool)reader["bzns_iscs"];
                            business.IsSupplier = reader["bzns_issp"] == DBNull.Value ? false : (bool)reader["bzns_issp"];
                            business.IsAgent = reader["bzns_isag"] == DBNull.Value ? false : (bool)reader["bzns_isag"];
                            business.PhoneNo1 = reader["phone1"] == DBNull.Value ? string.Empty : reader["phone1"].ToString();
                            business.PhoneNo2 = reader["phone2"] == DBNull.Value ? string.Empty : reader["phone2"].ToString();
                            business.Email1 = reader["email1"] == DBNull.Value ? string.Empty : reader["email1"].ToString();
                            business.Email2 = reader["email2"] == DBNull.Value ? string.Empty : reader["email2"].ToString();
                            business.WebLink1 = reader["weblink1"] == DBNull.Value ? string.Empty : reader["weblink1"].ToString();
                            business.WebLink2 = reader["weblink2"] == DBNull.Value ? string.Empty : reader["weblink2"].ToString();
                            business.BusinessStationId = reader["station_id"] == DBNull.Value ? 0 : (int)reader["station_id"];
                            business.BusinessStationName = reader["station_nm"] == DBNull.Value ? string.Empty : reader["station_nm"].ToString();
                            business.ImagePath = reader["imgp"] == DBNull.Value ? string.Empty : reader["imgp"].ToString();
                            business.ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString();
                            business.ModifiedTime = reader["mdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mdt"];
                            business.CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString();
                            business.CreatedTime = reader["ctt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ctt"];
                        }
                }
                await conn.CloseAsync();
            }
            return business;
        }
        public async Task<List<Business>> SearchCustomersByNameAsync(string businessName)
        {
            List<Business> businessList = new List<Business>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT b.bzns_id, b.bzns_no, b.bzns_nm, b.bzns_addr, b.bzns_stt, ");
            sb.Append("b.bzns_ctr, b.bzns_iscs, b.bzns_issp, b.bzns_isag, b.phone1, ");
            sb.Append("b.phone2, b.email1, b.email2, b.weblink1, b.weblink2, b.mdb, b.mdt, ");
            sb.Append("b.ctb, b.ctt, b.imgp, b.station_id, b.typ_id, b.ind_id, ");
            sb.Append("(SELECT locname FROM public.gst_locs WHERE locqk = b.station_id) as station_nm, ");
            sb.Append("(SELECT bzn_typ_nm FROM public.bpm_bzn_typs WHERE bzn_typ_id = b.typ_id) as typ_nm, ");
            sb.Append("(SELECT bzn_ind_nm FROM public.bpm_bzn_inds WHERE bzn_ind_id = b.ind_id) as ind_nm ");
            sb.Append("FROM public.bpm_bzns_inf b  ");
            sb.Append("WHERE(LOWER(b.bzns_nm) LIKE '%'||LOWER(@bzns_nm)||'%') ");
            sb.Append("AND (bzns_iscs = true) ORDER BY bzns_nm;");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var bzns_nm = cmd.Parameters.Add("@bzns_nm", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    bzns_nm.Value = businessName ?? string.Empty;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            businessList.Add(new Business()
                            {
                                BusinessID = reader["bzns_id"] == DBNull.Value ? string.Empty : reader["bzns_id"].ToString(),
                                BusinessNumber = reader["bzns_no"] == DBNull.Value ? string.Empty : reader["bzns_no"].ToString(),
                                BusinessName = reader["bzns_nm"] == DBNull.Value ? string.Empty : reader["bzns_nm"].ToString(),
                                BusinessTypeId = reader["typ_id"] == DBNull.Value ? 0 : (int)reader["typ_id"],
                                BusinessType = reader["typ_nm"] == DBNull.Value ? string.Empty : reader["typ_nm"].ToString(),
                                IndustrySectorId = reader["ind_id"] == DBNull.Value ? 0 : (int)reader["ind_id"],
                                IndustrySector = reader["ind_nm"] == DBNull.Value ? string.Empty : reader["ind_nm"].ToString(),
                                BusinessAddress = reader["bzns_addr"] == DBNull.Value ? string.Empty : reader["bzns_addr"].ToString(),
                                State = reader["bzns_stt"] == DBNull.Value ? string.Empty : reader["bzns_stt"].ToString(),
                                Country = reader["bzns_ctr"] == DBNull.Value ? string.Empty : reader["bzns_ctr"].ToString(),
                                IsCustomer = reader["bzns_iscs"] == DBNull.Value ? false : (bool)reader["bzns_iscs"],
                                IsSupplier = reader["bzns_issp"] == DBNull.Value ? false : (bool)reader["bzns_issp"],
                                IsAgent = reader["bzns_isag"] == DBNull.Value ? false : (bool)reader["bzns_isag"],
                                PhoneNo1 = reader["phone1"] == DBNull.Value ? string.Empty : reader["phone1"].ToString(),
                                PhoneNo2 = reader["phone2"] == DBNull.Value ? string.Empty : reader["phone2"].ToString(),
                                Email1 = reader["email1"] == DBNull.Value ? string.Empty : reader["email1"].ToString(),
                                Email2 = reader["email2"] == DBNull.Value ? string.Empty : reader["email2"].ToString(),
                                WebLink1 = reader["weblink1"] == DBNull.Value ? string.Empty : reader["weblink1"].ToString(),
                                WebLink2 = reader["weblink2"] == DBNull.Value ? string.Empty : reader["weblink2"].ToString(),
                                BusinessStationId = reader["station_id"] == DBNull.Value ? 0 : (int)reader["station_id"],
                                BusinessStationName = reader["station_nm"] == DBNull.Value ? string.Empty : reader["station_nm"].ToString(),
                                ImagePath = reader["imgp"] == DBNull.Value ? string.Empty : reader["imgp"].ToString(),
                                ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                                ModifiedTime = reader["mdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mdt"],
                                CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                                CreatedTime = reader["ctt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ctt"],
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return businessList;
        }
        public async Task<List<Business>> GetAllCustomersAsync()
        {
            List<Business> customerList = new List<Business>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT b.bzns_id, b.bzns_no, b.bzns_nm, b.bzns_addr, b.bzns_stt, ");
            sb.Append("b.bzns_ctr, b.bzns_iscs, b.bzns_issp, b.bzns_isag, b.phone1, ");
            sb.Append("b.phone2, b.email1, b.email2, b.weblink1, b.weblink2, b.mdb, b.mdt, ");
            sb.Append("b.ctb, b.ctt, b.imgp, b.station_id, b.typ_id, b.ind_id, ");
            sb.Append("(SELECT locname FROM public.gst_locs WHERE locqk = b.station_id) as station_nm, ");
            sb.Append("(SELECT bzn_typ_nm FROM public.bpm_bzn_typs WHERE bzn_typ_id = b.typ_id) as typ_nm, ");
            sb.Append("(SELECT bzn_ind_nm FROM public.bpm_bzn_inds WHERE bzn_ind_id = b.ind_id) as ind_nm ");
            sb.Append("FROM public.bpm_bzns_inf b  ");
            sb.Append("WHERE (bzns_iscs = true) ORDER BY bzns_nm;");
            string query = sb.ToString();

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
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
                                BusinessNumber = reader["bzns_no"] == DBNull.Value ? string.Empty : reader["bzns_no"].ToString(),
                                BusinessName = reader["bzns_nm"] == DBNull.Value ? string.Empty : reader["bzns_nm"].ToString(),
                                BusinessTypeId = reader["typ_id"] == DBNull.Value ? 0 : (int)reader["typ_id"],
                                BusinessType = reader["typ_nm"] == DBNull.Value ? string.Empty : reader["typ_nm"].ToString(),
                                IndustrySectorId = reader["ind_id"] == DBNull.Value ? 0 : (int)reader["ind_id"],
                                IndustrySector = reader["ind_nm"] == DBNull.Value ? string.Empty : reader["ind_nm"].ToString(),
                                BusinessAddress = reader["bzns_addr"] == DBNull.Value ? string.Empty : reader["bzns_addr"].ToString(),
                                State = reader["bzns_stt"] == DBNull.Value ? string.Empty : reader["bzns_stt"].ToString(),
                                Country = reader["bzns_ctr"] == DBNull.Value ? string.Empty : reader["bzns_ctr"].ToString(),
                                IsCustomer = reader["bzns_iscs"] == DBNull.Value ? false : (bool)reader["bzns_iscs"],
                                IsSupplier = reader["bzns_issp"] == DBNull.Value ? false : (bool)reader["bzns_issp"],
                                IsAgent = reader["bzns_isag"] == DBNull.Value ? false : (bool)reader["bzns_isag"],
                                PhoneNo1 = reader["phone1"] == DBNull.Value ? string.Empty : reader["phone1"].ToString(),
                                PhoneNo2 = reader["phone2"] == DBNull.Value ? string.Empty : reader["phone2"].ToString(),
                                Email1 = reader["email1"] == DBNull.Value ? string.Empty : reader["email1"].ToString(),
                                Email2 = reader["email2"] == DBNull.Value ? string.Empty : reader["email2"].ToString(),
                                WebLink1 = reader["weblink1"] == DBNull.Value ? string.Empty : reader["weblink1"].ToString(),
                                WebLink2 = reader["weblink2"] == DBNull.Value ? string.Empty : reader["weblink2"].ToString(),
                                BusinessStationId = reader["station_id"] == DBNull.Value ? 0 : (int)reader["station_id"],
                                BusinessStationName = reader["station_nm"] == DBNull.Value ? string.Empty : reader["station_nm"].ToString(),
                                ImagePath = reader["imgp"] == DBNull.Value ? string.Empty : reader["imgp"].ToString(),
                                ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                                ModifiedTime = reader["mdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mdt"],
                                CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                                CreatedTime = reader["ctt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ctt"],
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return customerList;
        }

        #endregion

        //====== Suppliers Action Methods =============//
        #region Suppliers Action Methods
        public async Task<Business> GetSupplierByNameAsync(string businessName)
        {
            Business business = new Business();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT b.bzns_id, b.bzns_no, b.bzns_nm, b.bzns_addr, b.bzns_stt, ");
            sb.Append("b.bzns_ctr, b.bzns_iscs, b.bzns_issp, b.bzns_isag, b.phone1, ");
            sb.Append("b.phone2, b.email1, b.email2, b.weblink1, b.weblink2, b.mdb, b.mdt, ");
            sb.Append("b.ctb, b.ctt, b.imgp, b.station_id, b.typ_id, b.ind_id, ");
            sb.Append("(SELECT locname FROM public.gst_locs WHERE locqk = b.station_id) as station_nm, ");
            sb.Append("(SELECT bzn_typ_nm FROM public.bpm_bzn_typs WHERE bzn_typ_id = b.typ_id) as typ_nm, ");
            sb.Append("(SELECT bzn_ind_nm FROM public.bpm_bzn_inds WHERE bzn_ind_id = b.ind_id) as ind_nm ");
            sb.Append("FROM public.bpm_bzns_inf b  ");
            sb.Append("WHERE (b.bzns_nm = @bzns_nm) AND (b.bzns_issp = true);");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var bzns_nm = cmd.Parameters.Add("@bzns_nm", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    bzns_nm.Value = businessName;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            business.BusinessID = reader["bzns_id"] == DBNull.Value ? string.Empty : reader["bzns_id"].ToString();
                            business.BusinessNumber = reader["bzns_no"] == DBNull.Value ? string.Empty : reader["bzns_no"].ToString();
                            business.BusinessName = reader["bzns_nm"] == DBNull.Value ? string.Empty : reader["bzns_nm"].ToString();
                            business.BusinessTypeId = reader["typ_id"] == DBNull.Value ? 0 : (int)reader["typ_id"];
                            business.BusinessType = reader["typ_nm"] == DBNull.Value ? string.Empty : reader["typ_nm"].ToString();
                            business.IndustrySectorId = reader["ind_id"] == DBNull.Value ? 0 : (int)reader["ind_id"];
                            business.IndustrySector = reader["ind_nm"] == DBNull.Value ? string.Empty : reader["ind_nm"].ToString();
                            business.BusinessAddress = reader["bzns_addr"] == DBNull.Value ? string.Empty : reader["bzns_addr"].ToString();
                            business.State = reader["bzns_stt"] == DBNull.Value ? string.Empty : reader["bzns_stt"].ToString();
                            business.Country = reader["bzns_ctr"] == DBNull.Value ? string.Empty : reader["bzns_ctr"].ToString();
                            business.IsCustomer = reader["bzns_iscs"] == DBNull.Value ? false : (bool)reader["bzns_iscs"];
                            business.IsSupplier = reader["bzns_issp"] == DBNull.Value ? false : (bool)reader["bzns_issp"];
                            business.IsAgent = reader["bzns_isag"] == DBNull.Value ? false : (bool)reader["bzns_isag"];
                            business.PhoneNo1 = reader["phone1"] == DBNull.Value ? string.Empty : reader["phone1"].ToString();
                            business.PhoneNo2 = reader["phone2"] == DBNull.Value ? string.Empty : reader["phone2"].ToString();
                            business.Email1 = reader["email1"] == DBNull.Value ? string.Empty : reader["email1"].ToString();
                            business.Email2 = reader["email2"] == DBNull.Value ? string.Empty : reader["email2"].ToString();
                            business.WebLink1 = reader["weblink1"] == DBNull.Value ? string.Empty : reader["weblink1"].ToString();
                            business.WebLink2 = reader["weblink2"] == DBNull.Value ? string.Empty : reader["weblink2"].ToString();
                            business.BusinessStationId = reader["station_id"] == DBNull.Value ? 0 : (int)reader["station_id"];
                            business.BusinessStationName = reader["station_nm"] == DBNull.Value ? string.Empty : reader["station_nm"].ToString();
                            business.ImagePath = reader["imgp"] == DBNull.Value ? string.Empty : reader["imgp"].ToString();
                            business.ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString();
                            business.ModifiedTime = reader["mdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mdt"];
                            business.CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString();
                            business.CreatedTime = reader["ctt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ctt"];
                        }
                }
                await conn.CloseAsync();
            }
            return business;
        }
        public async Task<List<Business>> SearchSuppliersByNameAsync(string businessName)
        {
            List<Business> businessList = new List<Business>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT b.bzns_id, b.bzns_no, b.bzns_nm, b.bzns_addr, b.bzns_stt, ");
            sb.Append("b.bzns_ctr, b.bzns_iscs, b.bzns_issp, b.bzns_isag, b.phone1, ");
            sb.Append("b.phone2, b.email1, b.email2, b.weblink1, b.weblink2, b.mdb, b.mdt, ");
            sb.Append("b.ctb, b.ctt, b.imgp, b.station_id, b.typ_id, b.ind_id, ");
            sb.Append("(SELECT locname FROM public.gst_locs WHERE locqk = b.station_id) as station_nm, ");
            sb.Append("(SELECT bzn_typ_nm FROM public.bpm_bzn_typs WHERE bzn_typ_id = b.typ_id) as typ_nm, ");
            sb.Append("(SELECT bzn_ind_nm FROM public.bpm_bzn_inds WHERE bzn_ind_id = b.ind_id) as ind_nm ");
            sb.Append("FROM public.bpm_bzns_inf b  ");
            sb.Append("WHERE(LOWER(b.bzns_nm) LIKE '%'||LOWER(@bzns_nm)||'%') ");
            sb.Append("AND (bzns_issp = true) ORDER BY bzns_nm;");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var bzns_nm = cmd.Parameters.Add("@bzns_nm", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    bzns_nm.Value = businessName ?? string.Empty;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            businessList.Add(new Business()
                            {
                                BusinessID = reader["bzns_id"] == DBNull.Value ? string.Empty : reader["bzns_id"].ToString(),
                                BusinessNumber = reader["bzns_no"] == DBNull.Value ? string.Empty : reader["bzns_no"].ToString(),
                                BusinessName = reader["bzns_nm"] == DBNull.Value ? string.Empty : reader["bzns_nm"].ToString(),
                                BusinessTypeId = reader["typ_id"] == DBNull.Value ? 0 : (int)reader["typ_id"],
                                BusinessType = reader["typ_nm"] == DBNull.Value ? string.Empty : reader["typ_nm"].ToString(),
                                IndustrySectorId = reader["ind_id"] == DBNull.Value ? 0 : (int)reader["ind_id"],
                                IndustrySector = reader["ind_nm"] == DBNull.Value ? string.Empty : reader["ind_nm"].ToString(),
                                BusinessAddress = reader["bzns_addr"] == DBNull.Value ? string.Empty : reader["bzns_addr"].ToString(),
                                State = reader["bzns_stt"] == DBNull.Value ? string.Empty : reader["bzns_stt"].ToString(),
                                Country = reader["bzns_ctr"] == DBNull.Value ? string.Empty : reader["bzns_ctr"].ToString(),
                                IsCustomer = reader["bzns_iscs"] == DBNull.Value ? false : (bool)reader["bzns_iscs"],
                                IsSupplier = reader["bzns_issp"] == DBNull.Value ? false : (bool)reader["bzns_issp"],
                                IsAgent = reader["bzns_isag"] == DBNull.Value ? false : (bool)reader["bzns_isag"],
                                PhoneNo1 = reader["phone1"] == DBNull.Value ? string.Empty : reader["phone1"].ToString(),
                                PhoneNo2 = reader["phone2"] == DBNull.Value ? string.Empty : reader["phone2"].ToString(),
                                Email1 = reader["email1"] == DBNull.Value ? string.Empty : reader["email1"].ToString(),
                                Email2 = reader["email2"] == DBNull.Value ? string.Empty : reader["email2"].ToString(),
                                WebLink1 = reader["weblink1"] == DBNull.Value ? string.Empty : reader["weblink1"].ToString(),
                                WebLink2 = reader["weblink2"] == DBNull.Value ? string.Empty : reader["weblink2"].ToString(),
                                BusinessStationId = reader["station_id"] == DBNull.Value ? 0 : (int)reader["station_id"],
                                BusinessStationName = reader["station_nm"] == DBNull.Value ? string.Empty : reader["station_nm"].ToString(),
                                ImagePath = reader["imgp"] == DBNull.Value ? string.Empty : reader["imgp"].ToString(),
                                ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                                ModifiedTime = reader["mdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mdt"],
                                CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                                CreatedTime = reader["ctt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ctt"],
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return businessList;
        }
        public async Task<List<Business>> GetAllSuppliersAsync()
        {
            List<Business> customerList = new List<Business>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT b.bzns_id, b.bzns_no, b.bzns_nm, b.bzns_addr, b.bzns_stt, ");
            sb.Append("b.bzns_ctr, b.bzns_iscs, b.bzns_issp, b.bzns_isag, b.phone1, ");
            sb.Append("b.phone2, b.email1, b.email2, b.weblink1, b.weblink2, b.mdb, b.mdt, ");
            sb.Append("b.ctb, b.ctt, b.imgp, b.station_id, b.typ_id, b.ind_id, ");
            sb.Append("(SELECT locname FROM public.gst_locs WHERE locqk = b.station_id) as station_nm, ");
            sb.Append("(SELECT bzn_typ_nm FROM public.bpm_bzn_typs WHERE bzn_typ_id = b.typ_id) as typ_nm, ");
            sb.Append("(SELECT bzn_ind_nm FROM public.bpm_bzn_inds WHERE bzn_ind_id = b.ind_id) as ind_nm ");
            sb.Append("FROM public.bpm_bzns_inf b  ");
            sb.Append("WHERE (bzns_issp = true) ORDER BY bzns_nm;");
            string query = sb.ToString();

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
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
                                BusinessNumber = reader["bzns_no"] == DBNull.Value ? string.Empty : reader["bzns_no"].ToString(),
                                BusinessName = reader["bzns_nm"] == DBNull.Value ? string.Empty : reader["bzns_nm"].ToString(),
                                BusinessTypeId = reader["typ_id"] == DBNull.Value ? 0 : (int)reader["typ_id"],
                                BusinessType = reader["typ_nm"] == DBNull.Value ? string.Empty : reader["typ_nm"].ToString(),
                                IndustrySectorId = reader["ind_id"] == DBNull.Value ? 0 : (int)reader["ind_id"],
                                IndustrySector = reader["ind_nm"] == DBNull.Value ? string.Empty : reader["ind_nm"].ToString(),
                                BusinessAddress = reader["bzns_addr"] == DBNull.Value ? string.Empty : reader["bzns_addr"].ToString(),
                                State = reader["bzns_stt"] == DBNull.Value ? string.Empty : reader["bzns_stt"].ToString(),
                                Country = reader["bzns_ctr"] == DBNull.Value ? string.Empty : reader["bzns_ctr"].ToString(),
                                IsCustomer = reader["bzns_iscs"] == DBNull.Value ? false : (bool)reader["bzns_iscs"],
                                IsSupplier = reader["bzns_issp"] == DBNull.Value ? false : (bool)reader["bzns_issp"],
                                IsAgent = reader["bzns_isag"] == DBNull.Value ? false : (bool)reader["bzns_isag"],
                                PhoneNo1 = reader["phone1"] == DBNull.Value ? string.Empty : reader["phone1"].ToString(),
                                PhoneNo2 = reader["phone2"] == DBNull.Value ? string.Empty : reader["phone2"].ToString(),
                                Email1 = reader["email1"] == DBNull.Value ? string.Empty : reader["email1"].ToString(),
                                Email2 = reader["email2"] == DBNull.Value ? string.Empty : reader["email2"].ToString(),
                                WebLink1 = reader["weblink1"] == DBNull.Value ? string.Empty : reader["weblink1"].ToString(),
                                WebLink2 = reader["weblink2"] == DBNull.Value ? string.Empty : reader["weblink2"].ToString(),
                                BusinessStationId = reader["station_id"] == DBNull.Value ? 0 : (int)reader["station_id"],
                                BusinessStationName = reader["station_nm"] == DBNull.Value ? string.Empty : reader["station_nm"].ToString(),
                                ImagePath = reader["imgp"] == DBNull.Value ? string.Empty : reader["imgp"].ToString(),
                                ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                                ModifiedTime = reader["mdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mdt"],
                                CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                                CreatedTime = reader["ctt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ctt"],
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return customerList;
        }

        #endregion

        //====== Business CRUD Action Methods ========//
        #region Business CRUD Action Methods
        public async Task<bool> AddAsync(Business business)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.bpm_bzns_inf(bzns_id, bzns_no, bzns_nm, ");
            sb.Append("bzns_addr, bzns_stt, bzns_ctr, bzns_iscs, bzns_issp, bzns_isag, ");
            sb.Append("phone1, phone2, email1, email2, weblink1, weblink2, mdb, ctb, ");
            sb.Append("imgp, station_id, mdt, ctt, typ_id, ind_id) ");
            sb.Append("VALUES (@bzns_id, @bzns_no, @bzns_nm, @bzns_addr, @bzns_stt, ");
            sb.Append("@bzns_ctr, @bzns_iscs, @bzns_issp, @bzns_isag, @phone1, @phone2, ");
            sb.Append("@email1, @email2, @weblink1, @weblink2, @mdb, @ctb, @imgp, ");
            sb.Append("@station_id, @mdt, @ctt, @typ_id, @ind_id); ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var bzns_id = cmd.Parameters.Add("@bzns_id", NpgsqlDbType.Text);
                    var bzns_no = cmd.Parameters.Add("@bzns_no", NpgsqlDbType.Text);
                    var bzns_nm = cmd.Parameters.Add("@bzns_nm", NpgsqlDbType.Text);
                    var typ_id = cmd.Parameters.Add("@typ_id", NpgsqlDbType.Integer);
                    var ind_id = cmd.Parameters.Add("@ind_id", NpgsqlDbType.Integer);
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
                    var station_id = cmd.Parameters.Add("@station_id", NpgsqlDbType.Integer);
                    var mdb = cmd.Parameters.Add("@mdb", NpgsqlDbType.Text);
                    var mdt = cmd.Parameters.Add("@mdt", NpgsqlDbType.Timestamp);
                    var ctb = cmd.Parameters.Add("@ctb", NpgsqlDbType.Text);
                    var ctt = cmd.Parameters.Add("@ctt", NpgsqlDbType.Timestamp);
                    cmd.Prepare();
                    bzns_id.Value = business.BusinessID ?? Guid.NewGuid().ToString();
                    bzns_no.Value = business.BusinessNumber ?? (object)DBNull.Value;
                    bzns_nm.Value = business.BusinessName.ToUpper();
                    typ_id.Value = business.BusinessTypeId ?? (object)DBNull.Value;
                    ind_id.Value = business.IndustrySectorId ?? (object)DBNull.Value;
                    bzns_addr.Value = business.BusinessAddress == string.Empty || business.BusinessAddress == null ? (object)DBNull.Value : business.BusinessAddress.ToUpper();
                    bzns_stt.Value = business.State == string.Empty || business.State == null ? (object)DBNull.Value : business.State.ToUpper();
                    bzns_ctr.Value = business.Country == string.Empty || business.Country == null ? (object)DBNull.Value : business.Country;
                    bzns_iscs.Value = business.IsCustomer;
                    bzns_issp.Value = business.IsSupplier;
                    bzns_isag.Value = business.IsAgent;
                    phone1.Value = business.PhoneNo1 ?? (object)DBNull.Value;
                    phone2.Value = business.PhoneNo2 ?? (object)DBNull.Value;
                    email1.Value = business.Email1 ?? (object)DBNull.Value;
                    email2.Value = business.Email2 ?? (object)DBNull.Value;
                    weblink1.Value = business.WebLink1 ?? (object)DBNull.Value;
                    weblink2.Value = business.WebLink2 ?? (object)DBNull.Value;
                    imgp.Value = business.ImagePath ?? (object)DBNull.Value;
                    station_id.Value = business.BusinessStationId ?? (object)DBNull.Value;
                    mdb.Value = business.CreatedBy ?? (object)DBNull.Value;
                    mdt.Value = business.CreatedTime ?? DateTime.Now;
                    ctb.Value = business.CreatedBy ?? (object)DBNull.Value;
                    ctt.Value = business.CreatedTime ?? DateTime.Now;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }

        public async Task<bool> EditAsync(Business business)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.bpm_bzns_inf SET bzns_no=@bzns_no, ");
            sb.Append("bzns_nm=@bzns_nm, bzns_addr=@bzns_addr, ");
            sb.Append("bzns_stt=@bzns_stt, bzns_issp=@bzns_issp, ");
            sb.Append("bzns_ctr=@bzns_ctr, bzns_iscs=@bzns_iscs, ");
            sb.Append("bzns_isag=@bzns_isag, phone1=@phone1, phone2=@phone2, ");
            sb.Append("email1=@email1, email2=@email2, weblink1=@weblink1, ");
            sb.Append("weblink2=@weblink2, mdb=@mdb, imgp=@imgp, ");
            sb.Append("station_id=@station_id, mdt=@mdt, ");
            sb.Append("typ_id=@typ_id, ind_id=@ind_id ");
            sb.Append("WHERE (bzns_id=@bzns_id); ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var bzns_id = cmd.Parameters.Add("@bzns_id", NpgsqlDbType.Text);
                    var bzns_no = cmd.Parameters.Add("@bzns_no", NpgsqlDbType.Text);
                    var bzns_nm = cmd.Parameters.Add("@bzns_nm", NpgsqlDbType.Text);
                    var typ_id = cmd.Parameters.Add("@typ_id", NpgsqlDbType.Integer);
                    var ind_id = cmd.Parameters.Add("@ind_id", NpgsqlDbType.Integer);
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
                    var station_id = cmd.Parameters.Add("@station_id", NpgsqlDbType.Integer);
                    var mdb = cmd.Parameters.Add("@mdb", NpgsqlDbType.Text);
                    var mdt = cmd.Parameters.Add("@mdt", NpgsqlDbType.Timestamp);
                    cmd.Prepare();
                    bzns_id.Value = business.BusinessID;
                    bzns_no.Value = business.BusinessNumber ?? (object)DBNull.Value;
                    bzns_nm.Value = business.BusinessName.ToUpper();
                    typ_id.Value = business.BusinessTypeId ?? (object)DBNull.Value;
                    ind_id.Value = business.IndustrySectorId ?? (object)DBNull.Value;
                    bzns_addr.Value = business.BusinessAddress == string.Empty || business.BusinessAddress == null ? (object)DBNull.Value : business.BusinessAddress.ToUpper();
                    bzns_stt.Value = business.State == string.Empty || business.State == null ? (object)DBNull.Value : business.State.ToUpper();
                    bzns_ctr.Value = business.Country == string.Empty || business.Country == null ? (object)DBNull.Value : business.Country;
                    bzns_iscs.Value = business.IsCustomer;
                    bzns_issp.Value = business.IsSupplier;
                    bzns_isag.Value = business.IsAgent;
                    phone1.Value = business.PhoneNo1 ?? (object)DBNull.Value;
                    phone2.Value = business.PhoneNo2 ?? (object)DBNull.Value;
                    email1.Value = business.Email1 ?? (object)DBNull.Value;
                    email2.Value = business.Email2 ?? (object)DBNull.Value;
                    weblink1.Value = business.WebLink1 ?? (object)DBNull.Value;
                    weblink2.Value = business.WebLink2 ?? (object)DBNull.Value;
                    imgp.Value = business.ImagePath ?? (object)DBNull.Value;
                    station_id.Value = business.BusinessStationId ?? (object)DBNull.Value;
                    mdb.Value = business.ModifiedBy ?? (object)DBNull.Value;
                    mdt.Value = business.ModifiedTime ?? (object)DBNull.Value;
                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(string businessId)
        {
            int rows = 0;
            string query = "DELETE FROM public.bpm_bzns_inf WHERE (bzns_id = @bzns_id);";
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var bzns_id = cmd.Parameters.Add("@bzns_id", NpgsqlDbType.Text);
                    cmd.Prepare();
                    bzns_id.Value = businessId;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        #endregion

    }
}
