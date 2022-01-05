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
    public class CompanyRepository : ICompanyRepository
    {
        public IConfiguration _config { get; }
        public CompanyRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        //============= Company Action Methods =====================================//
        #region Company Action Methods

        public async Task<Company> GetCompanyByCodeAsync(string companyCode)
        {
            Company company = new Company();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = $"SELECT coy_code, coy_name, coy_desc FROM public.gst_coys WHERE (coy_code = @coy_code);";
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var coy_code = cmd.Parameters.Add("@coy_code", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    coy_code.Value = companyCode;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            company.CompanyCode = reader["coy_code"] == DBNull.Value ? String.Empty : reader["coy_code"].ToString();
                            company.CompanyName = reader["coy_name"] == DBNull.Value ? String.Empty : reader["coy_name"].ToString();
                            company.CompanyDescription = reader["coy_desc"] == DBNull.Value ? String.Empty : reader["coy_desc"].ToString();
                        }
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message);
            }
            return company;
        }

        public async Task<IList<Company>> GetCompaniesAsync()
        {
            List<Company> companyList = new List<Company>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = $"SELECT coy_code, coy_name, coy_desc FROM public.gst_coys ORDER BY coy_name ASC;";
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
                        companyList.Add(new Company()
                        {
                            CompanyCode = reader["coy_code"] == DBNull.Value ? String.Empty : reader["coy_code"].ToString(),
                        CompanyName = reader["coy_name"] == DBNull.Value ? String.Empty : reader["coy_name"].ToString(),
                        CompanyDescription = reader["coy_desc"] == DBNull.Value ? String.Empty : reader["coy_desc"].ToString()
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
            return companyList;
        }
        #endregion

    }
}
