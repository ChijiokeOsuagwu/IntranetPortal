using IntranetPortal.Base.Enums;
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
    public class AppraisalGradeRepository : IAppraisalGradeRepository
    {
        public IConfiguration _config { get; }
        public AppraisalGradeRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        #region Appraisal Grade Read Action Methods
        public async Task<IList<AppraisalGrade>> GetAllAsync()
        {
            List<AppraisalGrade> appraisalGradesList = new List<AppraisalGrade>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT g.rvw_grd_id, g.rvw_grd_ds, g.rvw_sxn_id, g.grd_typ_id, ");
            sb.Append("g.lwr_band, g.upr_band, g.grd_ctt, g.grd_ctb, g.grd_mdt, g.grd_mdb, ");
            sb.Append("g.grd_rnk, s.rvw_sxn_nm, CASE g.grd_rnk  WHEN 1 THEN '1st' ");
            sb.Append("WHEN 2 THEN '2nd' WHEN 3 THEN '3rd' WHEN 4 THEN '4th' ");
            sb.Append("WHEN 5 THEN '5th'  WHEN 6 THEN '6th'  WHEN 7 THEN '7th' ");
            sb.Append("WHEN 8 THEN '8th' WHEN 9 THEN '9th'   WHEN 10 THEN '10th' ");
            sb.Append("END grd_rnk_ds FROM public.pmsrvwgrds g INNER JOIN ");
            sb.Append("public.pmsrvwsxns s ON g.rvw_sxn_id = s.rvw_sxn_id ");
            sb.Append("ORDER BY g.grd_typ_id, g.upr_band DESC;");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                await cmd.PrepareAsync();
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    appraisalGradesList.Add(new AppraisalGrade()
                    {
                        AppraisalGradeId = reader["rvw_grd_id"] == DBNull.Value ? 0 : (int)(reader["rvw_grd_id"]),
                        AppraisalGradeDescription = reader["rvw_grd_ds"] == DBNull.Value ? string.Empty : (reader["rvw_grd_ds"]).ToString(),
                        ReviewSessionId = reader["rvw_sxn_id"] == DBNull.Value ? 0 : (int)reader["rvw_sxn_id"],
                        ReviewSessionName = reader["rvw_sxn_nm"] == DBNull.Value ? string.Empty : reader["rvw_sxn_nm"].ToString(),

                        GradeType = reader["grd_typ_id"] == DBNull.Value ? ReviewGradeType.Performance : (ReviewGradeType)reader["grd_typ_id"],
                        LowerBandScore = reader["lwr_band"] == DBNull.Value ? 0.00M : (decimal)reader["lwr_band"],
                        UpperBandScore = reader["upr_band"] == DBNull.Value ? 0.00M : (decimal)reader["upr_band"],
                        GradeRank = reader["grd_rnk"] == DBNull.Value ? 0 : Convert.ToInt16(reader["grd_rnk"]),
                        GradeRankDescription = reader["grd_rnk_ds"] == DBNull.Value ? string.Empty : (reader["grd_rnk_ds"]).ToString(),
                        LastModifiedBy = reader["grd_mdb"] == DBNull.Value ? string.Empty : (reader["grd_mdb"]).ToString(),
                        LastModifiedTime = reader["grd_mdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["grd_mdt"],
                        CreatedBy = reader["grd_ctb"] == DBNull.Value ? string.Empty : (reader["grd_ctb"]).ToString(),
                        CreatedTime = reader["grd_ctt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["grd_ctt"],
                    });
                }
            }
            await conn.CloseAsync();
            return appraisalGradesList;
        }

        public async Task<IList<AppraisalGrade>> GetByIdAsync(int appraisalGradeId)
        {
            List<AppraisalGrade> appraisalGradesList = new List<AppraisalGrade>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT g.rvw_grd_id, g.rvw_grd_ds, g.rvw_sxn_id, g.grd_typ_id, ");
            sb.Append("g.lwr_band, g.upr_band, g.grd_ctt, g.grd_ctb, g.grd_mdt, g.grd_mdb, ");
            sb.Append("g.grd_rnk, s.rvw_sxn_nm, CASE g.grd_rnk  WHEN 1 THEN '1st' ");
            sb.Append("WHEN 2 THEN '2nd' WHEN 3 THEN '3rd' WHEN 4 THEN '4th' ");
            sb.Append("WHEN 5 THEN '5th'  WHEN 6 THEN '6th'  WHEN 7 THEN '7th' ");
            sb.Append("WHEN 8 THEN '8th' WHEN 9 THEN '9th'   WHEN 10 THEN '10th' ");
            sb.Append("END grd_rnk_ds FROM public.pmsrvwgrds g INNER JOIN ");
            sb.Append("public.pmsrvwsxns s ON g.rvw_sxn_id = s.rvw_sxn_id ");
            sb.Append("WHERE (g.rvw_grd_id = @rvw_grd_id) ");
            sb.Append("ORDER BY g.upr_band DESC;");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_grd_id = cmd.Parameters.Add("@rvw_grd_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                rvw_grd_id.Value = appraisalGradeId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    appraisalGradesList.Add(new AppraisalGrade()
                    {
                        AppraisalGradeId = reader["rvw_grd_id"] == DBNull.Value ? 0 : (int)(reader["rvw_grd_id"]),
                        AppraisalGradeDescription = reader["rvw_grd_ds"] == DBNull.Value ? string.Empty : (reader["rvw_grd_ds"]).ToString(),
                        ReviewSessionId = reader["rvw_sxn_id"] == DBNull.Value ? 0 : (int)reader["rvw_sxn_id"],
                        ReviewSessionName = reader["rvw_sxn_nm"] == DBNull.Value ? string.Empty : reader["rvw_sxn_nm"].ToString(),

                        GradeType = reader["grd_typ_id"] == DBNull.Value ? ReviewGradeType.Performance : (ReviewGradeType)reader["grd_typ_id"],
                        LowerBandScore = reader["lwr_band"] == DBNull.Value ? 0.00M : (decimal)reader["lwr_band"],
                        UpperBandScore = reader["upr_band"] == DBNull.Value ? 0.00M : (decimal)reader["upr_band"],
                        GradeRank = reader["grd_rnk"] == DBNull.Value ? 0 : Convert.ToInt16(reader["grd_rnk"]),
                        GradeRankDescription = reader["grd_rnk_ds"] == DBNull.Value ? string.Empty : (reader["grd_rnk_ds"]).ToString(),
                        LastModifiedBy = reader["grd_mdb"] == DBNull.Value ? string.Empty : (reader["grd_mdb"]).ToString(),
                        LastModifiedTime = reader["grd_mdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["grd_mdt"],
                        CreatedBy = reader["grd_ctb"] == DBNull.Value ? string.Empty : (reader["grd_ctb"]).ToString(),
                        CreatedTime = reader["grd_ctt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["grd_ctt"],
                    });
                }
            }
            await conn.CloseAsync();
            return appraisalGradesList;
        }

        public async Task<IList<AppraisalGrade>> GetByNameAsync(string appraisalGradeName)
        {
            List<AppraisalGrade> appraisalGradesList = new List<AppraisalGrade>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT g.rvw_grd_id, g.rvw_grd_ds, g.rvw_sxn_id, g.grd_typ_id, ");
            sb.Append("g.lwr_band, g.upr_band, g.grd_ctt, g.grd_ctb, g.grd_mdt, g.grd_mdb, ");
            sb.Append("g.grd_rnk, s.rvw_sxn_nm, CASE g.grd_rnk  WHEN 1 THEN '1st' ");
            sb.Append("WHEN 2 THEN '2nd' WHEN 3 THEN '3rd' WHEN 4 THEN '4th' ");
            sb.Append("WHEN 5 THEN '5th'  WHEN 6 THEN '6th'  WHEN 7 THEN '7th' ");
            sb.Append("WHEN 8 THEN '8th' WHEN 9 THEN '9th'   WHEN 10 THEN '10th' ");
            sb.Append("END grd_rnk_ds FROM public.pmsrvwgrds g INNER JOIN ");
            sb.Append("public.pmsrvwsxns s ON g.rvw_sxn_id = s.rvw_sxn_id ");
            sb.Append("WHERE (g.rvw_grd_ds = @rvw_grd_ds) ");
            sb.Append("ORDER BY g.upr_band DESC;");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_grd_ds = cmd.Parameters.Add("@rvw_grd_ds", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                rvw_grd_ds.Value = appraisalGradeName;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    appraisalGradesList.Add(new AppraisalGrade()
                    {
                        AppraisalGradeId = reader["rvw_grd_id"] == DBNull.Value ? 0 : (int)(reader["rvw_grd_id"]),
                        AppraisalGradeDescription = reader["rvw_grd_ds"] == DBNull.Value ? string.Empty : (reader["rvw_grd_ds"]).ToString(),
                        ReviewSessionId = reader["rvw_sxn_id"] == DBNull.Value ? 0 : (int)reader["rvw_sxn_id"],
                        ReviewSessionName = reader["rvw_sxn_nm"] == DBNull.Value ? string.Empty : reader["rvw_sxn_nm"].ToString(),

                        GradeType = reader["grd_typ_id"] == DBNull.Value ? ReviewGradeType.Performance : (ReviewGradeType)reader["grd_typ_id"],
                        LowerBandScore = reader["lwr_band"] == DBNull.Value ? 0.00M : (decimal)reader["lwr_band"],
                        UpperBandScore = reader["upr_band"] == DBNull.Value ? 0.00M : (decimal)reader["upr_band"],
                        GradeRank = reader["grd_rnk"] == DBNull.Value ? 0 : Convert.ToInt16(reader["grd_rnk"]),
                        GradeRankDescription = reader["grd_rnk_ds"] == DBNull.Value ? string.Empty : (reader["grd_rnk_ds"]).ToString(),
                        LastModifiedBy = reader["grd_mdb"] == DBNull.Value ? string.Empty : (reader["grd_mdb"]).ToString(),
                        LastModifiedTime = reader["grd_mdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["grd_mdt"],
                        CreatedBy = reader["grd_ctb"] == DBNull.Value ? string.Empty : (reader["grd_ctb"]).ToString(),
                        CreatedTime = reader["grd_ctt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["grd_ctt"],
                    });
                }
            }
            await conn.CloseAsync();
            return appraisalGradesList;
        }

        public async Task<IList<AppraisalGrade>> GetByReviewSessionIdAsync(int reviewSessionId)
        {
            List<AppraisalGrade> appraisalGradesList = new List<AppraisalGrade>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT g.rvw_grd_id, g.rvw_grd_ds, g.rvw_sxn_id, g.grd_typ_id, ");
            sb.Append("g.lwr_band, g.upr_band, g.grd_ctt, g.grd_ctb, g.grd_mdt, g.grd_mdb, ");
            sb.Append("g.grd_rnk, s.rvw_sxn_nm, CASE g.grd_rnk  WHEN 1 THEN '1st' ");
            sb.Append("WHEN 2 THEN '2nd' WHEN 3 THEN '3rd' WHEN 4 THEN '4th' ");
            sb.Append("WHEN 5 THEN '5th' WHEN 6 THEN '6th' WHEN 7 THEN '7th' ");
            sb.Append("WHEN 8 THEN '8th' WHEN 9 THEN '9th' WHEN 10 THEN '10th' ");
            sb.Append("END grd_rnk_ds FROM public.pmsrvwgrds g INNER JOIN ");
            sb.Append("public.pmsrvwsxns s ON g.rvw_sxn_id = s.rvw_sxn_id ");
            sb.Append("WHERE (g.rvw_sxn_id = @rvw_sxn_id) ");
            sb.Append("ORDER BY g.grd_typ_id, g.upr_band DESC;");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_sxn_id = cmd.Parameters.Add("@rvw_sxn_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                rvw_sxn_id.Value = reviewSessionId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    appraisalGradesList.Add(new AppraisalGrade()
                    {
                        AppraisalGradeId = reader["rvw_grd_id"] == DBNull.Value ? 0 : (int)(reader["rvw_grd_id"]),
                        AppraisalGradeDescription = reader["rvw_grd_ds"] == DBNull.Value ? string.Empty : (reader["rvw_grd_ds"]).ToString(),
                        ReviewSessionId = reader["rvw_sxn_id"] == DBNull.Value ? 0 : (int)reader["rvw_sxn_id"],
                        ReviewSessionName = reader["rvw_sxn_nm"] == DBNull.Value ? string.Empty : reader["rvw_sxn_nm"].ToString(),

                        GradeType = reader["grd_typ_id"] == DBNull.Value ? ReviewGradeType.Performance : (ReviewGradeType)reader["grd_typ_id"],
                        LowerBandScore = reader["lwr_band"] == DBNull.Value ? 0.00M : (decimal)reader["lwr_band"],
                        UpperBandScore = reader["upr_band"] == DBNull.Value ? 0.00M : (decimal)reader["upr_band"],
                        GradeRank = reader["grd_rnk"] == DBNull.Value ? 0 : Convert.ToInt16(reader["grd_rnk"]),
                        GradeRankDescription = reader["grd_rnk_ds"] == DBNull.Value ? string.Empty : (reader["grd_rnk_ds"]).ToString(),
                        LastModifiedBy = reader["grd_mdb"] == DBNull.Value ? string.Empty : (reader["grd_mdb"]).ToString(),
                        LastModifiedTime = reader["grd_mdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["grd_mdt"],
                        CreatedBy = reader["grd_ctb"] == DBNull.Value ? string.Empty : (reader["grd_ctb"]).ToString(),
                        CreatedTime = reader["grd_ctt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["grd_ctt"],
                    });
                }
            }
            await conn.CloseAsync();
            return appraisalGradesList;
        }

        public async Task<IList<AppraisalGrade>> GetByReviewSessionIdAsync(int reviewSessionId, ReviewGradeType gradeType)
        {
            List<AppraisalGrade> appraisalGradesList = new List<AppraisalGrade>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT g.rvw_grd_id, g.rvw_grd_ds, g.rvw_sxn_id, g.grd_typ_id, ");
            sb.Append("g.lwr_band, g.upr_band, g.grd_ctt, g.grd_ctb, g.grd_mdt, g.grd_mdb, ");
            sb.Append("g.grd_rnk, s.rvw_sxn_nm, CASE g.grd_rnk  WHEN 1 THEN '1st' ");
            sb.Append("WHEN 2 THEN '2nd' WHEN 3 THEN '3rd' WHEN 4 THEN '4th' ");
            sb.Append("WHEN 5 THEN '5th'  WHEN 6 THEN '6th'  WHEN 7 THEN '7th' ");
            sb.Append("WHEN 8 THEN '8th' WHEN 9 THEN '9th'   WHEN 10 THEN '10th' ");
            sb.Append("END grd_rnk_ds FROM public.pmsrvwgrds g INNER JOIN ");
            sb.Append("public.pmsrvwsxns s ON g.rvw_sxn_id = s.rvw_sxn_id ");
            sb.Append("WHERE (g.rvw_sxn_id = @rvw_sxn_id) ");
            sb.Append("AND (g.grd_typ_id = @grd_typ_id) ");
            sb.Append("ORDER BY g.grd_typ_id, g.upr_band DESC; ");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_sxn_id = cmd.Parameters.Add("@rvw_sxn_id", NpgsqlDbType.Integer);
                var grd_typ_id = cmd.Parameters.Add("@grd_typ_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                rvw_sxn_id.Value = reviewSessionId;
                grd_typ_id.Value = (int)gradeType;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    appraisalGradesList.Add(new AppraisalGrade()
                    {
                        AppraisalGradeId = reader["rvw_grd_id"] == DBNull.Value ? 0 : (int)(reader["rvw_grd_id"]),
                        AppraisalGradeDescription = reader["rvw_grd_ds"] == DBNull.Value ? string.Empty : (reader["rvw_grd_ds"]).ToString(),
                        ReviewSessionId = reader["rvw_sxn_id"] == DBNull.Value ? 0 : (int)reader["rvw_sxn_id"],
                        ReviewSessionName = reader["rvw_sxn_nm"] == DBNull.Value ? string.Empty : reader["rvw_sxn_nm"].ToString(),

                        GradeType = reader["grd_typ_id"] == DBNull.Value ? ReviewGradeType.Performance : (ReviewGradeType)reader["grd_typ_id"],
                        LowerBandScore = reader["lwr_band"] == DBNull.Value ? 0.00M : (decimal)reader["lwr_band"],
                        UpperBandScore = reader["upr_band"] == DBNull.Value ? 0.00M : (decimal)reader["upr_band"],
                        GradeRank = reader["grd_rnk"] == DBNull.Value ? 0 : Convert.ToInt16(reader["grd_rnk"]),
                        GradeRankDescription = reader["grd_rnk_ds"] == DBNull.Value ? string.Empty : (reader["grd_rnk_ds"]).ToString(),
                        LastModifiedBy = reader["grd_mdb"] == DBNull.Value ? string.Empty : (reader["grd_mdb"]).ToString(),
                        LastModifiedTime = reader["grd_mdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["grd_mdt"],
                        CreatedBy = reader["grd_ctb"] == DBNull.Value ? string.Empty : (reader["grd_ctb"]).ToString(),
                        CreatedTime = reader["grd_ctt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["grd_ctt"],
                    });
                }
            }
            await conn.CloseAsync();
            return appraisalGradesList;
        }

        public async Task<IList<AppraisalGrade>> GetByReviewSessionIdAndGradeScoreAsync(int reviewSessionId, ReviewGradeType gradeType, decimal gradeScore)
        {
            List<AppraisalGrade> appraisalGradesList = new List<AppraisalGrade>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT g.rvw_grd_id, g.rvw_grd_ds, g.rvw_sxn_id, g.grd_typ_id, ");
            sb.Append("g.lwr_band, g.upr_band, g.grd_ctt, g.grd_ctb, g.grd_mdt, g.grd_mdb, ");
            sb.Append("g.grd_rnk, s.rvw_sxn_nm, CASE g.grd_rnk  WHEN 1 THEN '1st' ");
            sb.Append("WHEN 2 THEN '2nd' WHEN 3 THEN '3rd' WHEN 4 THEN '4th' ");
            sb.Append("WHEN 5 THEN '5th'  WHEN 6 THEN '6th'  WHEN 7 THEN '7th' ");
            sb.Append("WHEN 8 THEN '8th' WHEN 9 THEN '9th'   WHEN 10 THEN '10th' ");
            sb.Append("END grd_rnk_ds FROM public.pmsrvwgrds g INNER JOIN ");
            sb.Append("public.pmsrvwsxns s ON g.rvw_sxn_id = s.rvw_sxn_id ");
            sb.Append("WHERE (g.rvw_sxn_id = @rvw_sxn_id) ");
            sb.Append("AND (g.grd_typ_id = @grd_typ_id) ");
            sb.Append("AND (@grade_score BETWEEN g.lwr_band AND g.upr_band) ");
            sb.Append("ORDER BY g.grd_typ_id, g.upr_band DESC; ");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_sxn_id = cmd.Parameters.Add("@rvw_sxn_id", NpgsqlDbType.Integer);
                var grd_typ_id = cmd.Parameters.Add("@grd_typ_id", NpgsqlDbType.Integer);
                var grade_score = cmd.Parameters.Add("@grade_score", NpgsqlDbType.Numeric);
                await cmd.PrepareAsync();
                rvw_sxn_id.Value = reviewSessionId;
                grd_typ_id.Value = (int)gradeType;
                grade_score.Value = gradeScore;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    appraisalGradesList.Add(new AppraisalGrade()
                    {
                        AppraisalGradeId = reader["rvw_grd_id"] == DBNull.Value ? 0 : (int)(reader["rvw_grd_id"]),
                        AppraisalGradeDescription = reader["rvw_grd_ds"] == DBNull.Value ? string.Empty : (reader["rvw_grd_ds"]).ToString(),
                        ReviewSessionId = reader["rvw_sxn_id"] == DBNull.Value ? 0 : (int)reader["rvw_sxn_id"],
                        ReviewSessionName = reader["rvw_sxn_nm"] == DBNull.Value ? string.Empty : reader["rvw_sxn_nm"].ToString(),

                        GradeType = reader["grd_typ_id"] == DBNull.Value ? ReviewGradeType.Performance : (ReviewGradeType)reader["grd_typ_id"],
                        LowerBandScore = reader["lwr_band"] == DBNull.Value ? 0.00M : (decimal)reader["lwr_band"],
                        UpperBandScore = reader["upr_band"] == DBNull.Value ? 0.00M : (decimal)reader["upr_band"],
                        GradeRank = reader["grd_rnk"] == DBNull.Value ? 0 : Convert.ToInt16(reader["grd_rnk"]),
                        GradeRankDescription = reader["grd_rnk_ds"] == DBNull.Value ? string.Empty : (reader["grd_rnk_ds"]).ToString(),
                        LastModifiedBy = reader["grd_mdb"] == DBNull.Value ? string.Empty : (reader["grd_mdb"]).ToString(),
                        LastModifiedTime = reader["grd_mdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["grd_mdt"],
                        CreatedBy = reader["grd_ctb"] == DBNull.Value ? string.Empty : (reader["grd_ctb"]).ToString(),
                        CreatedTime = reader["grd_ctt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["grd_ctt"],
                    });
                }
            }
            await conn.CloseAsync();
            return appraisalGradesList;
        }
        #endregion

        #region Review Grade Write Action Methods
        public async Task<bool> AddAsync(AppraisalGrade appraisalGrade)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.pmsrvwgrds(rvw_grd_ds, rvw_sxn_id, grd_typ_id, ");
            sb.Append("lwr_band, upr_band, grd_ctt, grd_ctb, grd_mdt, grd_mdb, grd_rnk)");
            sb.Append("VALUES (@rvw_grd_ds, @rvw_sxn_id, @grd_typ_id, @lwr_band, @upr_band, ");
            sb.Append("@grd_ctt, @grd_ctb, @grd_mdt, @grd_mdb, @grd_rnk);");
            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_grd_ds = cmd.Parameters.Add("@rvw_grd_ds", NpgsqlDbType.Text);
                var rvw_sxn_id = cmd.Parameters.Add("@rvw_sxn_id", NpgsqlDbType.Integer);
                var grd_typ_id = cmd.Parameters.Add("@grd_typ_id", NpgsqlDbType.Integer);
                var lwr_band = cmd.Parameters.Add("@lwr_band", NpgsqlDbType.Numeric);
                var upr_band = cmd.Parameters.Add("@upr_band", NpgsqlDbType.Numeric);
                var grd_rnk = cmd.Parameters.Add("@grd_rnk", NpgsqlDbType.Integer);
                var grd_ctb = cmd.Parameters.Add("@grd_ctb", NpgsqlDbType.Text);
                var grd_ctt = cmd.Parameters.Add("@grd_ctt", NpgsqlDbType.TimestampTz);
                var grd_mdb = cmd.Parameters.Add("@grd_mdb", NpgsqlDbType.Text);
                var grd_mdt = cmd.Parameters.Add("@grd_mdt", NpgsqlDbType.TimestampTz);
                cmd.Prepare();
                rvw_grd_ds.Value = appraisalGrade.AppraisalGradeDescription;
                rvw_sxn_id.Value = appraisalGrade.ReviewSessionId;
                grd_typ_id.Value = (int)appraisalGrade.GradeType;
                lwr_band.Value = appraisalGrade.LowerBandScore;
                upr_band.Value = appraisalGrade.UpperBandScore;
                grd_rnk.Value = appraisalGrade.GradeRank;
                grd_ctb.Value = appraisalGrade.CreatedBy ?? (object)DBNull.Value;
                grd_ctt.Value = appraisalGrade.CreatedTime ?? (object)DBNull.Value;
                grd_mdb.Value = appraisalGrade.CreatedBy ?? (object)DBNull.Value;
                grd_mdt.Value = appraisalGrade.CreatedTime ?? (object)DBNull.Value;

                rows = await cmd.ExecuteNonQueryAsync();
                await conn.CloseAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<bool> CopyAsync(string copiedBy, int reviewSessionId, int gradeTemplateId, ReviewGradeType gradeType)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.pmsrvwgrds (rvw_grd_ds, rvw_sxn_id, ");
            sb.Append("grd_typ_id, lwr_band, upr_band, grd_ctt, grd_ctb, grd_mdt, ");
            sb.Append("grd_mdb, grd_rnk) SELECT sxn_grd_ds, @rvw_sxn_id, ");
            sb.Append("grd_typ_id, lwr_band, upr_band, @grd_ctt, @grd_ctb, @grd_ctt, ");
            sb.Append("@grd_ctb, grd_rnk FROM  public.pmsgrddtls ");
            sb.Append("WHERE grd_hdr_id = @grd_hdr_id AND grd_typ_id = @grd_typ_id;");
            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_sxn_id = cmd.Parameters.Add("@rvw_sxn_id", NpgsqlDbType.Integer);
                var grd_hdr_id = cmd.Parameters.Add("@grd_hdr_id", NpgsqlDbType.Integer);
                var grd_typ_id = cmd.Parameters.Add("@grd_typ_id", NpgsqlDbType.Integer);
                var grd_ctb = cmd.Parameters.Add("@grd_ctb", NpgsqlDbType.Text);
                var grd_ctt = cmd.Parameters.Add("@grd_ctt", NpgsqlDbType.TimestampTz);
                cmd.Prepare();
                rvw_sxn_id.Value = reviewSessionId;
                grd_typ_id.Value = (int)gradeType;
                grd_ctb.Value = copiedBy ?? (object)DBNull.Value;
                grd_ctt.Value = DateTime.UtcNow;

                rows = await cmd.ExecuteNonQueryAsync();
                await conn.CloseAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<bool> CopyAsync(string copiedBy, int reviewSessionId, int gradeTemplateId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.pmsrvwgrds (rvw_grd_ds, rvw_sxn_id, ");
            sb.Append("grd_typ_id, lwr_band, upr_band, grd_ctt, grd_ctb, grd_mdt, ");
            sb.Append("grd_mdb, grd_rnk) SELECT sxn_grd_ds, @rvw_sxn_id, ");
            sb.Append("grd_typ_id, lwr_band, upr_band, @grd_ctt, @grd_ctb, @grd_ctt, ");
            sb.Append("@grd_ctb, grd_rnk FROM  public.pmsgrddtls ");
            sb.Append("WHERE grd_hdr_id = @grd_hdr_id;");
            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_sxn_id = cmd.Parameters.Add("@rvw_sxn_id", NpgsqlDbType.Integer);
                var grd_hdr_id = cmd.Parameters.Add("@grd_hdr_id", NpgsqlDbType.Integer);
                var grd_ctb = cmd.Parameters.Add("@grd_ctb", NpgsqlDbType.Text);
                var grd_ctt = cmd.Parameters.Add("@grd_ctt", NpgsqlDbType.TimestampTz);
                cmd.Prepare();
                rvw_sxn_id.Value = reviewSessionId;
                grd_hdr_id.Value = gradeTemplateId;
                grd_ctb.Value = copiedBy ?? (object)DBNull.Value;
                grd_ctt.Value = DateTime.UtcNow;

                rows = await cmd.ExecuteNonQueryAsync();
                await conn.CloseAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }


        public async Task<bool> UpdateAsync(AppraisalGrade appraisalGrade)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));

            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.pmsrvwgrds SET rvw_grd_ds=@rvw_grd_ds,  ");
            sb.Append("rvw_sxn_id=@rvw_sxn_id, grd_typ_id=@grd_typ_id, ");
            sb.Append("lwr_band=@lwr_band, upr_band=@upr_band, grd_mdt=@grd_mdt, ");
            sb.Append("grd_mdb=@grd_mdb, grd_rnk=@grd_rnk ");
            sb.Append("WHERE (rvw_grd_id=@rvw_grd_id); ");
            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_grd_ds = cmd.Parameters.Add("@rvw_grd_ds", NpgsqlDbType.Text);
                var rvw_sxn_id = cmd.Parameters.Add("@rvw_sxn_id", NpgsqlDbType.Integer);
                var grd_typ_id = cmd.Parameters.Add("@grd_typ_id", NpgsqlDbType.Integer);
                var lwr_band = cmd.Parameters.Add("@lwr_band", NpgsqlDbType.Numeric);
                var upr_band = cmd.Parameters.Add("@upr_band", NpgsqlDbType.Numeric);
                var grd_rnk = cmd.Parameters.Add("@grd_rnk", NpgsqlDbType.Integer);
                var grd_mdt = cmd.Parameters.Add("@grd_mdt", NpgsqlDbType.TimestampTz);
                var grd_mdb = cmd.Parameters.Add("@grd_mdb", NpgsqlDbType.Text);
                var rvw_grd_id = cmd.Parameters.Add("@rvw_grd_id", NpgsqlDbType.Integer);
                cmd.Prepare();
                rvw_grd_ds.Value = appraisalGrade.AppraisalGradeDescription;
                rvw_sxn_id.Value = appraisalGrade.ReviewSessionId;
                grd_typ_id.Value = (int)appraisalGrade.GradeType;
                lwr_band.Value = appraisalGrade.LowerBandScore;
                upr_band.Value = appraisalGrade.UpperBandScore;
                grd_rnk.Value = appraisalGrade.GradeRank;
                grd_mdt.Value = appraisalGrade.LastModifiedTime ?? (object)DBNull.Value;
                grd_mdb.Value = appraisalGrade.LastModifiedBy ?? (object)DBNull.Value;
                rvw_grd_id.Value = appraisalGrade.AppraisalGradeId;

                rows = await cmd.ExecuteNonQueryAsync();
                await conn.CloseAsync();
            }
            await conn.CloseAsync();
            return rows > 0;

        }

        public async Task<bool> DeleteAsync(int appraisalGradeId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));

            string query = "DELETE FROM public.pmsrvwgrds WHERE(rvw_grd_id=@rvw_grd_id);";

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_grd_id = cmd.Parameters.Add("@rvw_grd_id", NpgsqlDbType.Integer);
                cmd.Prepare();
                rvw_grd_id.Value = appraisalGradeId;

                rows = await cmd.ExecuteNonQueryAsync();
                await conn.CloseAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        #endregion


    }
}
