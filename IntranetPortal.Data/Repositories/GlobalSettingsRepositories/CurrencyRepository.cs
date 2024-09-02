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
    public class CurrencyRepository : ICurrencyRepository
    {
        public IConfiguration _config { get; }
        public CurrencyRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        //============== Currency Action Methods ================//
        #region Currency Action Methods

        public async Task<Currency> GetByCodeAsync(string code)
        {
            Currency currency = new Currency();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT curr_cd, curr_nm, curr_sy ");
            sb.Append("FROM public.gst_currs ");
            sb.Append("WHERE (curr_cd = @curr_cd);");
            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var curr_cd = cmd.Parameters.Add("@curr_cd", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                curr_cd.Value = code;

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        currency.Code = reader["curr_cd"] == DBNull.Value ? string.Empty : reader["curr_cd"].ToString();
                        currency.Name = reader["curr_nm"] == DBNull.Value ? string.Empty : reader["curr_nm"].ToString();
                        currency.Symbol = reader["curr_sy"] == DBNull.Value ? string.Empty : reader["curr_sy"].ToString();
                    }
            }
            await conn.CloseAsync();
            return currency;
        }

        public async Task<IList<Currency>> GetCurrenciesAsync()
        {
            List<Currency> currencyList = new List<Currency>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT curr_cd, curr_nm, curr_sy ");
            sb.Append("FROM public.gst_currs ");
            sb.Append("ORDER BY curr_nm;");
            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                await cmd.PrepareAsync();
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    currencyList.Add(new Currency()
                    {
                        Code = reader["curr_cd"] == DBNull.Value ? string.Empty : reader["curr_cd"].ToString(),
                        Name = reader["curr_nm"] == DBNull.Value ? string.Empty : reader["curr_nm"].ToString(),
                        Symbol = reader["curr_sy"] == DBNull.Value ? string.Empty : reader["curr_sy"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return currencyList;
        }

        //public async Task<bool> AddTeamAsync(Team team)
        //{
        //    int rows = 0;
        //    var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
        //    StringBuilder sb = new StringBuilder();
        //    sb.Append("INSERT INTO public.gst_tms(tm_id, tm_nm, tm_ds, tm_loc, tm_mb, tm_md, tm_cb, tm_cd) ");
        //    sb.Append("VALUES (@tm_id, @tm_nm, @tm_ds, @tm_loc, @tm_mb, @tm_md, @tm_mb, @tm_md);");
        //    string query = sb.ToString();
        //    try
        //    {
        //        await conn.OpenAsync();
        //        //Insert data
        //        using (var cmd = new NpgsqlCommand(query, conn))
        //        {
        //            var teamId = cmd.Parameters.Add("@tm_id", NpgsqlDbType.Text);
        //            var teamName = cmd.Parameters.Add("@tm_nm", NpgsqlDbType.Text);
        //            var teamDescription = cmd.Parameters.Add("@tm_ds", NpgsqlDbType.Text);
        //            var teamLocationId = cmd.Parameters.Add("@tm_loc", NpgsqlDbType.Integer);
        //            var actionBy = cmd.Parameters.Add("@tm_mb", NpgsqlDbType.Text);
        //            var modifiedDate = cmd.Parameters.Add("@tm_md", NpgsqlDbType.Text);
        //            cmd.Prepare();
        //            teamId.Value = Guid.NewGuid().ToString();
        //            teamName.Value = team.TeamName;
        //            teamDescription.Value = team.TeamDescription ?? (object)DBNull.Value;
        //            teamLocationId.Value = team.TeamLocationID ?? (object)DBNull.Value;
        //            actionBy.Value = team.ModifiedBy ?? (object)DBNull.Value;
        //            modifiedDate.Value = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} GMT" ?? (object)DBNull.Value;
        //            rows = await cmd.ExecuteNonQueryAsync();
        //            await conn.CloseAsync();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        await conn.CloseAsync();
        //        throw new Exception(ex.Message);
        //    }
        //    return rows > 0;
        //}

        //public async Task<bool> EditTeamAsync(Team team)
        //{
        //    int rows = 0;
        //    var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
        //    StringBuilder sb = new StringBuilder();
        //    sb.Append("UPDATE public.gst_tms SET tm_nm=@tm_nm, tm_ds=@tm_ds, tm_loc=@tm_loc, tm_mb=@tm_mb, ");
        //    sb.Append("tm_md=@tm_md WHERE (tm_id=@tm_id);");
        //    string query = sb.ToString();
        //    try
        //    {
        //        await conn.OpenAsync();
        //        //Insert data
        //        using (var cmd = new NpgsqlCommand(query, conn))
        //        {
        //            var teamId = cmd.Parameters.Add("@tm_id", NpgsqlDbType.Text);
        //            var teamName = cmd.Parameters.Add("@tm_nm", NpgsqlDbType.Text);
        //            var teamDescription = cmd.Parameters.Add("@tm_ds", NpgsqlDbType.Text);
        //            var teamLocationId = cmd.Parameters.Add("@tm_loc", NpgsqlDbType.Integer);
        //            var actionBy = cmd.Parameters.Add("@tm_mb", NpgsqlDbType.Text);
        //            var modifiedDate = cmd.Parameters.Add("@tm_md", NpgsqlDbType.Text);
        //            cmd.Prepare();
        //            teamId.Value = team.TeamID;
        //            teamName.Value = team.TeamName;
        //            teamDescription.Value = team.TeamDescription ?? (object)DBNull.Value;
        //            teamLocationId.Value = team.TeamLocationID ?? (object)DBNull.Value;
        //            actionBy.Value = team.ModifiedBy ?? (object)DBNull.Value;
        //            modifiedDate.Value = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} GMT" ?? (object)DBNull.Value;
        //            rows = await cmd.ExecuteNonQueryAsync();
        //            await conn.CloseAsync();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        await conn.CloseAsync();
        //        throw new Exception(ex.Message);
        //    }
        //    return rows > 0;
        //}

        //public async Task<bool> DeleteTeamAsync(string teamId)
        //{
        //    int rows = 0;
        //    var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
        //    string query = $"DELETE FROM public.gst_tms WHERE (tm_id=@tm_id);";
        //    try
        //    {
        //        await conn.OpenAsync();
        //        //Insert data
        //        using (var cmd = new NpgsqlCommand(query, conn))
        //        {
        //            var team_id = cmd.Parameters.Add("@tm_id", NpgsqlDbType.Text);
        //            cmd.Prepare();
        //            team_id.Value = teamId;
        //            rows = await cmd.ExecuteNonQueryAsync();
        //            await conn.CloseAsync();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        await conn.CloseAsync();
        //        throw new Exception(ex.Message);
        //    }
        //    return rows > 0;
        //}

        #endregion

    }
}
