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
    public class UtilityRepository : IUtilityRepository
    {
        public IConfiguration _config { get; }
        public UtilityRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        //================================ Auto Number Action Methods ============================================//
        #region Auto Number Action Methods
        public async Task<string> GetAutoNumberAsync(string numberType)
        {
            string codeNumber = string.Empty;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            if (String.IsNullOrEmpty(numberType)) { throw new ArgumentNullException(nameof(numberType), "The required parameter [Number Type] is missing or has an invalid value."); }
            string query = $"SELECT LPAD(next_no::text, no_length, '0') AS code_no FROM gst_auto_no WHERE (no_type = @no_type)";
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var noType = cmd.Parameters.Add("@no_type", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    noType.Value = numberType;
                    var result = await cmd.ExecuteScalarAsync();
                    codeNumber = result.ToString();
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message);
            }
            return codeNumber;

        }

        public async Task<bool> IncrementAutoNumberAsync(string numberType)
        {
            if (String.IsNullOrEmpty(numberType)) { throw new ArgumentNullException(nameof(numberType), "The required parameter [Number Type] is missing or has an invalid value."); }
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = $"UPDATE public.gst_auto_no SET next_no=next_no + 1	WHERE no_type=@no_type;";
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var no_type = cmd.Parameters.Add("@no_type", NpgsqlDbType.Text);
                    cmd.Prepare();
                    no_type.Value = numberType;
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
