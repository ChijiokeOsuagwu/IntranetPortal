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
    public class ReviewGradeRepository : IReviewGradeRepository
    {
        public IConfiguration _config { get; }
        public ReviewGradeRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        #region Review Grade Read Action Methods
        public async Task<IList<ReviewGrade>> GetAllAsync()
        {
            List<ReviewGrade> reviewGradesList = new List<ReviewGrade>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT g.sxn_grd_id, g.sxn_grd_ds, g.grd_hdr_id, g.grd_typ_id, ");
            sb.Append("g.lwr_band, g.upr_band, g.grd_ctt, g.grd_ctb, g.grd_mdt, g.grd_mdb, ");
            sb.Append("g.grd_rnk, h.grd_hdr_nm, CASE g.grd_rnk  WHEN 1 THEN '1st' ");
            sb.Append("WHEN 2 THEN '2nd' WHEN 3 THEN '3rd' WHEN 4 THEN '4th' ");
            sb.Append("WHEN 5 THEN '5th'  WHEN 6 THEN '6th'  WHEN 7 THEN '7th' ");
            sb.Append("WHEN 8 THEN '8th' WHEN 9 THEN '9th'   WHEN 10 THEN '10th' ");
            sb.Append("END grd_rnk_ds FROM public.pmsgrddtls g INNER JOIN ");
            sb.Append("public.pmsgrdhdrs h ON g.grd_hdr_id = h.grd_hdr_id ");
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
                    reviewGradesList.Add(new ReviewGrade()
                    {
                        ReviewGradeId = reader["sxn_grd_id"] == DBNull.Value ? 0 : (int)(reader["sxn_grd_id"]),
                        ReviewGradeDescription = reader["sxn_grd_ds"] == DBNull.Value ? string.Empty : (reader["sxn_grd_ds"]).ToString(),
                        GradeHeaderId = reader["grd_hdr_id"] == DBNull.Value ? 0 : (int)reader["grd_hdr_id"],
                        GradeHeaderName = reader["grd_hdr_nm"] == DBNull.Value ? string.Empty : reader["grd_hdr_nm"].ToString(),
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
            return reviewGradesList;
        }

        public async Task<IList<ReviewGrade>> GetByIdAsync(int reviewGradeId)
        {
            List<ReviewGrade> reviewGradesList = new List<ReviewGrade>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT g.sxn_grd_id, g.sxn_grd_ds, g.grd_hdr_id, g.grd_typ_id, ");
            sb.Append("g.lwr_band, g.upr_band, g.grd_ctt, g.grd_ctb, g.grd_mdt, g.grd_mdb, ");
            sb.Append("g.grd_rnk, h.grd_hdr_nm, CASE g.grd_rnk  WHEN 1 THEN '1st' ");
            sb.Append("WHEN 2 THEN '2nd' WHEN 3 THEN '3rd' WHEN 4 THEN '4th' ");
            sb.Append("WHEN 5 THEN '5th' WHEN 6 THEN '6th' WHEN 7 THEN '7th' ");
            sb.Append("WHEN 8 THEN '8th' WHEN 9 THEN '9th' WHEN 10 THEN '10th' ");
            sb.Append("END grd_rnk_ds FROM public.pmsgrddtls g INNER JOIN ");
            sb.Append("public.pmsgrdhdrs h ON g.grd_hdr_id = h.grd_hdr_id ");
            sb.Append("WHERE (g.sxn_grd_id = @sxn_grd_id); ");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var sxn_grd_id = cmd.Parameters.Add("@sxn_grd_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                sxn_grd_id.Value = reviewGradeId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    reviewGradesList.Add(new ReviewGrade()
                    {
                        ReviewGradeId = reader["sxn_grd_id"] == DBNull.Value ? 0 : (int)(reader["sxn_grd_id"]),
                        ReviewGradeDescription = reader["sxn_grd_ds"] == DBNull.Value ? string.Empty : (reader["sxn_grd_ds"]).ToString(),
                        GradeHeaderId = reader["grd_hdr_id"] == DBNull.Value ? 0 : (int)reader["grd_hdr_id"],
                        GradeHeaderName = reader["grd_hdr_nm"] == DBNull.Value ? string.Empty : reader["grd_hdr_nm"].ToString(),
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
            return reviewGradesList;
        }

        public async Task<IList<ReviewGrade>> GetByNameAsync(string reviewGradeName)
        {
            List<ReviewGrade> reviewGradesList = new List<ReviewGrade>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT g.sxn_grd_id, g.sxn_grd_ds, g.grd_hdr_id, g.grd_typ_id, ");
            sb.Append("g.lwr_band, g.upr_band, g.grd_ctt, g.grd_ctb, g.grd_mdt, g.grd_mdb, ");
            sb.Append("g.grd_rnk, h.grd_hdr_nm, CASE g.grd_rnk  WHEN 1 THEN '1st' ");
            sb.Append("WHEN 2 THEN '2nd' WHEN 3 THEN '3rd' WHEN 4 THEN '4th' ");
            sb.Append("WHEN 5 THEN '5th' WHEN 6 THEN '6th' WHEN 7 THEN '7th' ");
            sb.Append("WHEN 8 THEN '8th' WHEN 9 THEN '9th' WHEN 10 THEN '10th' ");
            sb.Append("END grd_rnk_ds FROM public.pmsgrddtls g INNER JOIN ");
            sb.Append("public.pmsgrdhdrs h ON g.grd_hdr_id = h.grd_hdr_id ");
            sb.Append("WHERE (g.sxn_grd_ds = @sxn_grd_ds) ");
            sb.Append("ORDER BY g.upr_band DESC;");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var sxn_grd_ds = cmd.Parameters.Add("@sxn_grd_ds", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                sxn_grd_ds.Value = reviewGradeName;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    reviewGradesList.Add(new ReviewGrade()
                    {
                        ReviewGradeId = reader["sxn_grd_id"] == DBNull.Value ? 0 : (int)(reader["sxn_grd_id"]),
                        ReviewGradeDescription = reader["sxn_grd_ds"] == DBNull.Value ? string.Empty : (reader["sxn_grd_ds"]).ToString(),
                        GradeHeaderId = reader["grd_hdr_id"] == DBNull.Value ? 0 : (int)reader["grd_hdr_id"],
                        GradeHeaderName = reader["grd_hdr_nm"] == DBNull.Value ? string.Empty : reader["grd_hdr_nm"].ToString(),
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
            return reviewGradesList;
        }

        public async Task<IList<ReviewGrade>> GetByGradeHeaderIdAsync(int gradeHeaderId)
        {
            List<ReviewGrade> reviewGradesList = new List<ReviewGrade>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT g.sxn_grd_id, g.sxn_grd_ds, g.grd_hdr_id, g.grd_typ_id, ");
            sb.Append("g.lwr_band, g.upr_band, g.grd_ctt, g.grd_ctb, g.grd_mdt, g.grd_mdb, ");
            sb.Append("g.grd_rnk, h.grd_hdr_nm, CASE g.grd_rnk  WHEN 1 THEN '1st' ");
            sb.Append("WHEN 2 THEN '2nd' WHEN 3 THEN '3rd' WHEN 4 THEN '4th' ");
            sb.Append("WHEN 5 THEN '5th' WHEN 6 THEN '6th' WHEN 7 THEN '7th' ");
            sb.Append("WHEN 8 THEN '8th' WHEN 9 THEN '9th' WHEN 10 THEN '10th' ");
            sb.Append("END grd_rnk_ds FROM public.pmsgrddtls g INNER JOIN ");
            sb.Append("public.pmsgrdhdrs h ON g.grd_hdr_id = h.grd_hdr_id ");
            sb.Append("WHERE (g.grd_hdr_id = @grd_hdr_id) ");
            sb.Append("ORDER BY g.grd_typ_id, g.upr_band DESC;");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var grd_hdr_id = cmd.Parameters.Add("@grd_hdr_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                grd_hdr_id.Value = gradeHeaderId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    reviewGradesList.Add(new ReviewGrade()
                    {
                        ReviewGradeId = reader["sxn_grd_id"] == DBNull.Value ? 0 : (int)(reader["sxn_grd_id"]),
                        ReviewGradeDescription = reader["sxn_grd_ds"] == DBNull.Value ? string.Empty : (reader["sxn_grd_ds"]).ToString(),
                        GradeHeaderId = reader["grd_hdr_id"] == DBNull.Value ? 0 : (int)reader["grd_hdr_id"],
                        GradeHeaderName = reader["grd_hdr_nm"] == DBNull.Value ? string.Empty : reader["grd_hdr_nm"].ToString(),
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
            return reviewGradesList;
        }

        public async Task<IList<ReviewGrade>> GetByGradeHeaderIdAsync(int gradeHeaderId, ReviewGradeType gradeType)
        {
            List<ReviewGrade> reviewGradesList = new List<ReviewGrade>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT g.sxn_grd_id, g.sxn_grd_ds, g.grd_hdr_id, g.grd_typ_id, ");
            sb.Append("g.lwr_band, g.upr_band, g.grd_ctt, g.grd_ctb, g.grd_mdt, g.grd_mdb, ");
            sb.Append("g.grd_rnk, h.grd_hdr_nm, CASE g.grd_rnk  WHEN 1 THEN '1st' ");
            sb.Append("WHEN 2 THEN '2nd' WHEN 3 THEN '3rd' WHEN 4 THEN '4th' ");
            sb.Append("WHEN 5 THEN '5th' WHEN 6 THEN '6th' WHEN 7 THEN '7th' ");
            sb.Append("WHEN 8 THEN '8th' WHEN 9 THEN '9th' WHEN 10 THEN '10th' ");
            sb.Append("END grd_rnk_ds FROM public.pmsgrddtls g INNER JOIN ");
            sb.Append("public.pmsgrdhdrs h ON g.grd_hdr_id = h.grd_hdr_id ");
            sb.Append("WHERE (g.grd_hdr_id = @grd_hdr_id) ");
            sb.Append("AND (g.grd_typ_id = @grd_typ_id) ");
            sb.Append("ORDER BY g.grd_typ_id, g.upr_band DESC; ");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var grd_hdr_id = cmd.Parameters.Add("@grd_hdr_id", NpgsqlDbType.Integer);
                var grd_typ_id = cmd.Parameters.Add("@grd_typ_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                grd_hdr_id.Value = gradeHeaderId;
                grd_typ_id.Value = (int)gradeType;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    reviewGradesList.Add(new ReviewGrade()
                    {
                        ReviewGradeId = reader["sxn_grd_id"] == DBNull.Value ? 0 : (int)(reader["sxn_grd_id"]),
                        ReviewGradeDescription = reader["sxn_grd_ds"] == DBNull.Value ? string.Empty : (reader["sxn_grd_ds"]).ToString(),
                        GradeHeaderId = reader["grd_hdr_id"] == DBNull.Value ? 0 : (int)reader["grd_hdr_id"],
                        GradeHeaderName = reader["grd_hdr_nm"] == DBNull.Value ? string.Empty : reader["grd_hdr_nm"].ToString(),
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
            return reviewGradesList;
        }

        #endregion

        #region Review Grade Write Action Methods
        public async Task<bool> AddAsync(ReviewGrade reviewGrade)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.pmsgrddtls(sxn_grd_ds, grd_hdr_id, grd_typ_id, ");
            sb.Append("lwr_band, upr_band, grd_ctt, grd_ctb, grd_mdt, grd_mdb, grd_rnk)");
            sb.Append("VALUES (@sxn_grd_ds, @grd_hdr_id, @grd_typ_id, @lwr_band, @upr_band, ");
            sb.Append("@grd_ctt, @grd_ctb, @grd_mdt, @grd_mdb, @grd_rnk);");
            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var sxn_grd_ds = cmd.Parameters.Add("@sxn_grd_ds", NpgsqlDbType.Text);
                var grd_hdr_id = cmd.Parameters.Add("@grd_hdr_id", NpgsqlDbType.Integer);
                var grd_typ_id = cmd.Parameters.Add("@grd_typ_id", NpgsqlDbType.Integer);
                var lwr_band = cmd.Parameters.Add("@lwr_band", NpgsqlDbType.Numeric);
                var upr_band = cmd.Parameters.Add("@upr_band", NpgsqlDbType.Numeric);
                var grd_rnk = cmd.Parameters.Add("@grd_rnk", NpgsqlDbType.Integer);
                var grd_ctb = cmd.Parameters.Add("@grd_ctb", NpgsqlDbType.Text);
                var grd_ctt = cmd.Parameters.Add("@grd_ctt", NpgsqlDbType.TimestampTz);
                var grd_mdb = cmd.Parameters.Add("@grd_mdb", NpgsqlDbType.Text);
                var grd_mdt = cmd.Parameters.Add("@grd_mdt", NpgsqlDbType.TimestampTz);
                cmd.Prepare();
                sxn_grd_ds.Value = reviewGrade.ReviewGradeDescription;
                grd_hdr_id.Value = reviewGrade.GradeHeaderId;
                grd_typ_id.Value = (int)reviewGrade.GradeType;
                lwr_band.Value = reviewGrade.LowerBandScore;
                upr_band.Value = reviewGrade.UpperBandScore;
                grd_rnk.Value = reviewGrade.GradeRank;
                grd_ctb.Value = reviewGrade.CreatedBy ?? (object)DBNull.Value;
                grd_ctt.Value = reviewGrade.CreatedTime ?? (object)DBNull.Value;
                grd_mdb.Value = reviewGrade.CreatedBy ?? (object)DBNull.Value;
                grd_mdt.Value = reviewGrade.CreatedTime ?? (object)DBNull.Value;

                rows = await cmd.ExecuteNonQueryAsync();
                await conn.CloseAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<bool> UpdateAsync(ReviewGrade reviewGrade)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));

            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.pmsgrddtls SET sxn_grd_ds=@sxn_grd_ds,  ");
            sb.Append("grd_hdr_id=@grd_hdr_id, grd_typ_id=@grd_typ_id, ");
            sb.Append("lwr_band=@lwr_band, upr_band=@upr_band, grd_mdt=@grd_mdt, ");
            sb.Append("grd_mdb=@grd_mdb, grd_rnk=@grd_rnk ");
            sb.Append("WHERE (sxn_grd_id=@sxn_grd_id); ");
            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var sxn_grd_ds = cmd.Parameters.Add("@sxn_grd_ds", NpgsqlDbType.Text);
                var grd_hdr_id = cmd.Parameters.Add("@grd_hdr_id", NpgsqlDbType.Integer);
                var grd_typ_id = cmd.Parameters.Add("@grd_typ_id", NpgsqlDbType.Integer);
                var lwr_band = cmd.Parameters.Add("@lwr_band", NpgsqlDbType.Numeric);
                var upr_band = cmd.Parameters.Add("@upr_band", NpgsqlDbType.Numeric);
                var grd_rnk = cmd.Parameters.Add("@grd_rnk", NpgsqlDbType.Integer);
                var grd_mdt = cmd.Parameters.Add("@grd_mdt", NpgsqlDbType.TimestampTz);
                var grd_mdb = cmd.Parameters.Add("@grd_mdb", NpgsqlDbType.Text);
                var sxn_grd_id = cmd.Parameters.Add("@sxn_grd_id", NpgsqlDbType.Integer);
                cmd.Prepare();
                sxn_grd_ds.Value = reviewGrade.ReviewGradeDescription;
                grd_hdr_id.Value = reviewGrade.GradeHeaderId;
                grd_typ_id.Value = (int)reviewGrade.GradeType;
                lwr_band.Value = reviewGrade.LowerBandScore;
                upr_band.Value = reviewGrade.UpperBandScore;
                grd_rnk.Value = reviewGrade.GradeRank;
                grd_mdt.Value = reviewGrade.LastModifiedTime ?? (object)DBNull.Value;
                grd_mdb.Value = reviewGrade.LastModifiedBy ?? (object)DBNull.Value;
                sxn_grd_id.Value = reviewGrade.ReviewGradeId;

                rows = await cmd.ExecuteNonQueryAsync();
                await conn.CloseAsync();
            }
            await conn.CloseAsync();
            return rows > 0;

        }

        public async Task<bool> DeleteAsync(int reviewGradeId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));

            string query = "DELETE FROM public.pmsgrddtls WHERE(sxn_grd_id=@sxn_grd_id);";

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var sxn_grd_id = cmd.Parameters.Add("@sxn_grd_id", NpgsqlDbType.Integer);
                cmd.Prepare();
                sxn_grd_id.Value = reviewGradeId;

                rows = await cmd.ExecuteNonQueryAsync();
                await conn.CloseAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        #endregion

    }
}
