using IntranetPortal.Base.Models.PmsModels;
using IntranetPortal.Base.Repositories.PmsRepositories;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Data.Repositories.PmsRepositories
{
    public class ReviewCDGRepository : IReviewCDGRepository
    {
        public IConfiguration _config { get; }
        public ReviewCDGRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        #region Review CDG Read Action Methods
        public async Task<List<ReviewCDG>> GetByReviewHeaderIdAsync(int reviewHeaderId)
        {
            List<ReviewCDG> reviewCDGList = new List<ReviewCDG>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT c.rvw_cdg_id, c.rvw_hdr_id, c.rvw_sxn_id, c.rvw_yr_id, ");
            sb.Append("c.rvw_cdg_ds, c.rvw_cdg_obj, c.rvw_cdg_xtn, c.rvw_emp_id, ");
            sb.Append("s.rvw_sxn_nm, e.fullname AS rvw_emp_nm ");
            sb.Append("FROM public.pmsrvwcdgs c ");
            sb.Append("INNER JOIN public.pmsrvwsxns s ON s.rvw_sxn_id = c.rvw_sxn_id ");
            sb.Append("INNER JOIN public.gst_prsns e ON e.id = c.rvw_emp_id ");
            sb.Append("WHERE (c.rvw_hdr_id = @rvw_hdr_id); ");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_hdr_id = cmd.Parameters.Add("@rvw_hdr_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                rvw_hdr_id.Value = reviewHeaderId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    reviewCDGList.Add(new ReviewCDG()
                    {
                        ReviewCdgId = reader["rvw_cdg_id"] == DBNull.Value ? 0 : (int)reader["rvw_cdg_id"],
                        ReviewHeaderId = reader["rvw_hdr_id"] == DBNull.Value ? 0 : (int)reader["rvw_hdr_id"],
                        ReviewSessionId = reader["rvw_sxn_id"] == DBNull.Value ? 0 : (int)reader["rvw_sxn_id"],
                        ReviewSessionName = reader["rvw_sxn_nm"] == DBNull.Value ? string.Empty : reader["rvw_sxn_nm"].ToString(),
                        ReviewYearId = reader["rvw_yr_id"] == DBNull.Value ? 0 : (int)reader["rvw_yr_id"],
                        ReviewCdgDescription = reader["rvw_cdg_ds"] == DBNull.Value ? string.Empty : reader["rvw_cdg_ds"].ToString(),
                        ReviewCdgObjective = reader["rvw_cdg_obj"] == DBNull.Value ? string.Empty : reader["rvw_cdg_obj"].ToString(),
                        ReviewCdgActionPlan = reader["rvw_cdg_xtn"] == DBNull.Value ? string.Empty : reader["rvw_cdg_xtn"].ToString(),
                        AppraiseeId = reader["rvw_emp_id"] == DBNull.Value ? string.Empty : reader["rvw_emp_id"].ToString(),
                        AppraiseeName = reader["rvw_emp_nm"] == DBNull.Value ? string.Empty : reader["rvw_emp_nm"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return reviewCDGList;
        }

        public async Task<List<ReviewCDG>> GetByIdAsync(int reviewCdgId)
        {
            List<ReviewCDG> reviewCDGList = new List<ReviewCDG>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT c.rvw_cdg_id, c.rvw_hdr_id, c.rvw_sxn_id, c.rvw_yr_id, ");
            sb.Append("c.rvw_cdg_ds, c.rvw_cdg_obj, c.rvw_cdg_xtn, c.rvw_emp_id, ");
            sb.Append("s.rvw_sxn_nm, e.fullname AS rvw_emp_nm ");
            sb.Append("FROM public.pmsrvwcdgs c ");
            sb.Append("INNER JOIN public.pmsrvwsxns s ON s.rvw_sxn_id = c.rvw_sxn_id ");
            sb.Append("INNER JOIN public.gst_prsns e ON e.id = c.rvw_emp_id ");
            sb.Append("WHERE (c.rvw_cdg_id = @rvw_cdg_id); ");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_cdg_id = cmd.Parameters.Add("@rvw_cdg_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                rvw_cdg_id.Value = reviewCdgId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    reviewCDGList.Add(new ReviewCDG()
                    {
                        ReviewCdgId = reader["rvw_cdg_id"] == DBNull.Value ? 0 : (int)reader["rvw_cdg_id"],
                        ReviewHeaderId = reader["rvw_hdr_id"] == DBNull.Value ? 0 : (int)reader["rvw_hdr_id"],
                        ReviewSessionId = reader["rvw_sxn_id"] == DBNull.Value ? 0 : (int)reader["rvw_sxn_id"],
                        ReviewSessionName = reader["rvw_sxn_nm"] == DBNull.Value ? string.Empty : reader["rvw_sxn_nm"].ToString(),
                        ReviewYearId = reader["rvw_yr_id"] == DBNull.Value ? 0 : (int)reader["rvw_yr_id"],
                        ReviewCdgDescription = reader["rvw_cdg_ds"] == DBNull.Value ? string.Empty : reader["rvw_cdg_ds"].ToString(),
                        ReviewCdgObjective = reader["rvw_cdg_obj"] == DBNull.Value ? string.Empty : reader["rvw_cdg_obj"].ToString(),
                        ReviewCdgActionPlan = reader["rvw_cdg_xtn"] == DBNull.Value ? string.Empty : reader["rvw_cdg_xtn"].ToString(),
                        AppraiseeId = reader["rvw_emp_id"] == DBNull.Value ? string.Empty : reader["rvw_emp_id"].ToString(),
                        AppraiseeName = reader["rvw_emp_nm"] == DBNull.Value ? string.Empty : reader["rvw_emp_nm"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return reviewCDGList;
        }

        #endregion

        #region Review CDG Write Action Methods
        public async Task<bool> AddAsync(ReviewCDG reviewCDG)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.pmsrvwcdgs(rvw_hdr_id, rvw_sxn_id, ");
            sb.Append("rvw_yr_id, rvw_cdg_ds, rvw_cdg_obj, rvw_cdg_xtn, rvw_emp_id) ");
            sb.Append("VALUES (@rvw_hdr_id, @rvw_sxn_id, @rvw_yr_id, @rvw_cdg_ds, ");
            sb.Append("@rvw_cdg_obj, @rvw_cdg_xtn, @rvw_emp_id);");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var rvw_hdr_id = cmd.Parameters.Add("@rvw_hdr_id", NpgsqlDbType.Integer);
                    var rvw_sxn_id = cmd.Parameters.Add("@rvw_sxn_id", NpgsqlDbType.Integer);
                    var rvw_yr_id = cmd.Parameters.Add("@rvw_yr_id", NpgsqlDbType.Integer);
                    var rvw_cdg_ds = cmd.Parameters.Add("@rvw_cdg_ds", NpgsqlDbType.Text);
                    var rvw_cdg_obj = cmd.Parameters.Add("@rvw_cdg_obj", NpgsqlDbType.Text);
                    var rvw_cdg_xtn = cmd.Parameters.Add("@rvw_cdg_xtn", NpgsqlDbType.Text);
                    var rvw_emp_id = cmd.Parameters.Add("rvw_emp_id", NpgsqlDbType.Text);
                    cmd.Prepare();
                    rvw_hdr_id.Value = reviewCDG.ReviewHeaderId;
                    rvw_sxn_id.Value = reviewCDG.ReviewSessionId;
                    rvw_yr_id.Value = reviewCDG.ReviewYearId;
                    rvw_cdg_ds.Value = reviewCDG.ReviewCdgDescription;
                    rvw_cdg_obj.Value = reviewCDG.ReviewCdgObjective ?? (object)DBNull.Value;
                    rvw_cdg_xtn.Value = reviewCDG.ReviewCdgActionPlan ?? (object)DBNull.Value;
                    rvw_emp_id.Value = reviewCDG.AppraiseeId;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                await conn.CloseAsync();
            }
            return rows > 0;
        }

        public async Task<bool> UpdateAsync(ReviewCDG reviewCdg)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.pmsrvwcdgs SET rvw_cdg_ds=@rvw_cdg_ds, ");
            sb.Append("rvw_cdg_obj=@rvw_cdg_obj, rvw_cdg_xtn=@rvw_cdg_xtn ");
            sb.Append("WHERE (rvw_cdg_id = @rvw_cdg_id);");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var rvw_cdg_ds = cmd.Parameters.Add("@rvw_cdg_ds", NpgsqlDbType.Text);
                    var rvw_cdg_obj = cmd.Parameters.Add("@rvw_cdg_obj", NpgsqlDbType.Text);
                    var rvw_cdg_xtn = cmd.Parameters.Add("@rvw_cdg_xtn", NpgsqlDbType.Text);
                    var rvw_cdg_id = cmd.Parameters.Add("@rvw_cdg_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    rvw_cdg_id.Value = reviewCdg.ReviewCdgId;
                    rvw_cdg_ds.Value = reviewCdg.ReviewCdgDescription;
                    rvw_cdg_obj.Value = reviewCdg.ReviewCdgObjective ?? (object)DBNull.Value;
                    rvw_cdg_xtn.Value = reviewCdg.ReviewCdgActionPlan ?? (object)DBNull.Value;

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

        public async Task<bool> DeleteAsync(int reviewCdgId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("DELETE FROM public.pmsrvwcdgs ");
            sb.Append("WHERE (rvw_cdg_id = @rvw_cdg_id);");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var rvw_cdg_id = cmd.Parameters.Add("@rvw_cdg_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    rvw_cdg_id.Value = reviewCdgId;

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
