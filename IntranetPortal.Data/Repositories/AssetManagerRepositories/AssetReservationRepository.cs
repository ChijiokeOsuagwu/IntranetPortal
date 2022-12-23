using IntranetPortal.Base.Models.AssetManagerModels;
using IntranetPortal.Base.Repositories.AssetManagerRepositories;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Data.Repositories.AssetManagerRepositories
{
    public class AssetReservationRepository : IAssetReservationRepository
    {
        public IConfiguration _config { get; }
        public AssetReservationRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        //============== All Asset Reservation Action Methods Starts Here =====================================================//
        #region All Asset Reservation Action Methods

        public async Task<IList<AssetReservation>> GetAllAsync()
        {
            List<AssetReservation> assetReservations = new List<AssetReservation>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT r.ass_rsv_id, r.asset_id, r.evnt_str_dt, r.evnt_nd_dt, r.evnt_ds, r.rsv_by, r.rsv_dt, r.evnt_loc, ");
            sb.Append("r.rsv_stt, r.asset_type_id, r.ass_rsv_mb, r.ass_rsv_md, a.asst_nm, a.asst_ds, t.typ_nm ");
            sb.Append("FROM public.asm_ass_rsvs r INNER JOIN public.asm_stt_asst a ON r.asset_id = a.asst_id ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON r.asset_type_id = t.typ_id ");
            sb.Append("ORDER BY r.asset_type_id, r.asset_id, r.ass_rsv_id DESC;");
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
                            assetReservations.Add(new AssetReservation()
                            {
                                AssetReservationID = reader["ass_rsv_id"] == DBNull.Value ? 0 : (int)reader["ass_rsv_id"],
                                AssetID = reader["asset_id"] == DBNull.Value ? string.Empty : reader["asset_id"].ToString(),
                                AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                                AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                                EventStartTime = reader["evnt_str_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["evnt_str_dt"],
                                EventEndTime = reader["evnt_nd_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["evnt_nd_dt"],
                                EventDescription = reader["evnt_ds"] == DBNull.Value ? string.Empty : reader["evnt_ds"].ToString(),
                                EventLocation = reader["evnt_loc"] == DBNull.Value ? string.Empty : reader["evnt_loc"].ToString(),
                                ReservationStatus = reader["rsv_stt"] == DBNull.Value ? string.Empty : reader["rsv_stt"].ToString(),
                                ReservedBy = reader["rsv_by"] == DBNull.Value ? string.Empty : reader["rsv_by"].ToString(),
                                ReservedOn = reader["rsv_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rsv_dt"],
                                AssetTypeID = reader["asset_type_id"] == DBNull.Value ? 0 : (int)reader["asset_type_id"],
                                AssetTypeName = reader["typ_nm"] == DBNull.Value ? string.Empty : reader["typ_nm"].ToString(),
                                LastModifiedBy = reader["ass_rsv_mb"] == DBNull.Value ? string.Empty : reader["ass_rsv_mb"].ToString(),
                                LastModifiedTime = reader["ass_rsv_md"] == DBNull.Value ? string.Empty : reader["ass_rsv_md"].ToString(),
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
            return assetReservations;
        }

        public async Task<AssetReservation> GetByIdAsync(int assetReservationId)
        {
            AssetReservation assetReservation = new AssetReservation();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT r.ass_rsv_id, r.asset_id, r.evnt_str_dt, r.evnt_nd_dt, r.evnt_ds, r.rsv_by, r.rsv_dt, r.evnt_loc, ");
            sb.Append("r.rsv_stt, r.asset_type_id, r.ass_rsv_mb, r.ass_rsv_md, a.asst_nm, a.asst_ds, t.typ_nm ");
            sb.Append("FROM public.asm_ass_rsvs r INNER JOIN public.asm_stt_asst a ON r.asset_id = a.asst_id ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON r.asset_type_id = t.typ_id WHERE r.ass_rsv_id = @asst_rsv_id; ");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var asset_reservation_id = cmd.Parameters.Add("@asst_rsv_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    asset_reservation_id.Value = assetReservationId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assetReservation.AssetReservationID = reader["ass_rsv_id"] == DBNull.Value ? 0 : (int)reader["ass_rsv_id"];
                            assetReservation.AssetID = reader["asset_id"] == DBNull.Value ? string.Empty : reader["asset_id"].ToString();
                            assetReservation.AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString();
                            assetReservation.AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString();
                            assetReservation.EventStartTime = reader["evnt_str_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["evnt_str_dt"];
                            assetReservation.EventEndTime = reader["evnt_nd_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["evnt_nd_dt"];
                            assetReservation.EventDescription = reader["evnt_ds"] == DBNull.Value ? string.Empty : reader["evnt_ds"].ToString();
                            assetReservation.EventLocation = reader["evnt_loc"] == DBNull.Value ? string.Empty : reader["evnt_loc"].ToString();
                            assetReservation.ReservationStatus = reader["rsv_stt"] == DBNull.Value ? string.Empty : reader["rsv_stt"].ToString();
                            assetReservation.ReservedBy = reader["rsv_by"] == DBNull.Value ? string.Empty : reader["rsv_by"].ToString();
                            assetReservation.ReservedOn = reader["rsv_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rsv_dt"];
                            assetReservation.AssetTypeID = reader["asset_type_id"] == DBNull.Value ? 0 : (int)reader["asset_type_id"];
                            assetReservation.AssetTypeName = reader["typ_nm"] == DBNull.Value ? string.Empty : reader["typ_nm"].ToString();
                            assetReservation.LastModifiedBy = reader["ass_rsv_mb"] == DBNull.Value ? string.Empty : reader["ass_rsv_mb"].ToString();
                            assetReservation.LastModifiedTime = reader["ass_rsv_md"] == DBNull.Value ? string.Empty : reader["ass_rsv_md"].ToString();

                        }
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message);
            }
            return assetReservation;
        }

        public async Task<IList<AssetReservation>> GetByAssetIdAsync(string assetId)
        {
            List<AssetReservation> assetReservations = new List<AssetReservation>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT r.ass_rsv_id, r.asset_id, r.evnt_str_dt, r.evnt_nd_dt, r.evnt_ds, r.rsv_by, r.rsv_dt, r.evnt_loc, ");
            sb.Append("r.rsv_stt, r.asset_type_id, r.ass_rsv_mb, r.ass_rsv_md, a.asst_nm, a.asst_ds, t.typ_nm ");
            sb.Append("FROM public.asm_ass_rsvs r INNER JOIN public.asm_stt_asst a ON r.asset_id = a.asst_id ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON r.asset_type_id = t.typ_id WHERE r.asset_id = @asst_id ");
            sb.Append("ORDER BY r.ass_rsv_id DESC;");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var asset_id = cmd.Parameters.Add("@asst_id", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    asset_id.Value = assetId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assetReservations.Add(new AssetReservation()
                            {
                                AssetReservationID = reader["ass_rsv_id"] == DBNull.Value ? 0 : (int)reader["ass_rsv_id"],
                                AssetID = reader["asset_id"] == DBNull.Value ? string.Empty : reader["asset_id"].ToString(),
                                AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                                AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                                EventStartTime = reader["evnt_str_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["evnt_str_dt"],
                                EventEndTime = reader["evnt_nd_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["evnt_nd_dt"],
                                EventDescription = reader["evnt_ds"] == DBNull.Value ? string.Empty : reader["evnt_ds"].ToString(),
                                EventLocation = reader["evnt_loc"] == DBNull.Value ? string.Empty : reader["evnt_loc"].ToString(),
                                ReservationStatus = reader["rsv_stt"] == DBNull.Value ? string.Empty : reader["rsv_stt"].ToString(),
                                ReservedBy = reader["rsv_by"] == DBNull.Value ? string.Empty : reader["rsv_by"].ToString(),
                                ReservedOn = reader["rsv_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rsv_dt"],
                                AssetTypeID = reader["asset_type_id"] == DBNull.Value ? 0 : (int)reader["asset_type_id"],
                                AssetTypeName = reader["typ_nm"] == DBNull.Value ? string.Empty : reader["typ_nm"].ToString(),
                                LastModifiedBy = reader["ass_rsv_mb"] == DBNull.Value ? string.Empty : reader["ass_rsv_mb"].ToString(),
                                LastModifiedTime = reader["ass_rsv_md"] == DBNull.Value ? string.Empty : reader["ass_rsv_md"].ToString(),
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
            return assetReservations;
        }

        public async Task<IList<AssetReservation>> GetByAssetNameAsync(string assetName)
        {
            List<AssetReservation> assetReservations = new List<AssetReservation>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT r.ass_rsv_id, r.asset_id, r.evnt_str_dt, r.evnt_nd_dt, r.evnt_ds, r.rsv_by, r.rsv_dt, r.evnt_loc, ");
            sb.Append("r.rsv_stt, r.asset_type_id, r.ass_rsv_mb, r.ass_rsv_md, a.asst_nm, a.asst_ds, t.typ_nm ");
            sb.Append("FROM public.asm_ass_rsvs r INNER JOIN public.asm_stt_asst a ON r.asset_id = a.asst_id ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON r.asset_type_id = t.typ_id ");
            sb.Append($"WHERE(LOWER(asst_nm) LIKE '%'||LOWER(@asst_nm)||'%') ORDER BY asst_nm;");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var asset_name = cmd.Parameters.Add("@asst_nm", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    asset_name.Value = assetName;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assetReservations.Add(new AssetReservation()
                            {
                                AssetReservationID = reader["ass_rsv_id"] == DBNull.Value ? 0 : (int)reader["ass_rsv_id"],
                                AssetID = reader["asset_id"] == DBNull.Value ? string.Empty : reader["asset_id"].ToString(),
                                AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                                AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                                EventStartTime = reader["evnt_str_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["evnt_str_dt"],
                                EventEndTime = reader["evnt_nd_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["evnt_nd_dt"],
                                EventDescription = reader["evnt_ds"] == DBNull.Value ? string.Empty : reader["evnt_ds"].ToString(),
                                EventLocation = reader["evnt_loc"] == DBNull.Value ? string.Empty : reader["evnt_loc"].ToString(),
                                ReservationStatus = reader["rsv_stt"] == DBNull.Value ? string.Empty : reader["rsv_stt"].ToString(),
                                ReservedBy = reader["rsv_by"] == DBNull.Value ? string.Empty : reader["rsv_by"].ToString(),
                                ReservedOn = reader["rsv_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rsv_dt"],
                                AssetTypeID = reader["asset_type_id"] == DBNull.Value ? 0 : (int)reader["asset_type_id"],
                                AssetTypeName = reader["typ_nm"] == DBNull.Value ? string.Empty : reader["typ_nm"].ToString(),
                                LastModifiedBy = reader["ass_rsv_mb"] == DBNull.Value ? string.Empty : reader["ass_rsv_mb"].ToString(),
                                LastModifiedTime = reader["ass_rsv_md"] == DBNull.Value ? string.Empty : reader["ass_rsv_md"].ToString(),
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
            return assetReservations;
        }

        public async Task<IList<AssetReservation>> GetByAssetTypeIdAsync(int assetTypeId)
        {
            List<AssetReservation> assetReservations = new List<AssetReservation>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT r.ass_rsv_id, r.asset_id, r.evnt_str_dt, r.evnt_nd_dt, r.evnt_ds, r.rsv_by, r.rsv_dt, r.evnt_loc, ");
            sb.Append("r.rsv_stt, r.asset_type_id, r.ass_rsv_mb, r.ass_rsv_md, a.asst_nm, a.asst_ds, t.typ_nm ");
            sb.Append("FROM public.asm_ass_rsvs r INNER JOIN public.asm_stt_asst a ON r.asset_id = a.asst_id ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON r.asset_type_id = t.typ_id WHERE r.asset_type_id = @asst_type_id ");
            sb.Append("ORDER BY r.asset_id, r.ass_rsv_id DESC;");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var asset_type_id = cmd.Parameters.Add("@asst_type_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    asset_type_id.Value = assetTypeId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assetReservations.Add(new AssetReservation()
                            {
                                AssetReservationID = reader["ass_rsv_id"] == DBNull.Value ? 0 : (int)reader["ass_rsv_id"],
                                AssetID = reader["asset_id"] == DBNull.Value ? string.Empty : reader["asset_id"].ToString(),
                                AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                                AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                                EventStartTime = reader["evnt_str_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["evnt_str_dt"],
                                EventEndTime = reader["evnt_nd_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["evnt_nd_dt"],
                                EventDescription = reader["evnt_ds"] == DBNull.Value ? string.Empty : reader["evnt_ds"].ToString(),
                                EventLocation = reader["evnt_loc"] == DBNull.Value ? string.Empty : reader["evnt_loc"].ToString(),
                                ReservationStatus = reader["rsv_stt"] == DBNull.Value ? string.Empty : reader["rsv_stt"].ToString(),
                                ReservedBy = reader["rsv_by"] == DBNull.Value ? string.Empty : reader["rsv_by"].ToString(),
                                ReservedOn = reader["rsv_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rsv_dt"],
                                AssetTypeID = reader["asset_type_id"] == DBNull.Value ? 0 : (int)reader["asset_type_id"],
                                AssetTypeName = reader["typ_nm"] == DBNull.Value ? string.Empty : reader["typ_nm"].ToString(),
                                LastModifiedBy = reader["ass_rsv_mb"] == DBNull.Value ? string.Empty : reader["ass_rsv_mb"].ToString(),
                                LastModifiedTime = reader["ass_rsv_md"] == DBNull.Value ? string.Empty : reader["ass_rsv_md"].ToString(),
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
            return assetReservations;
        }

        public async Task<IList<AssetReservation>> GetByAssetIdAndYearAsync(string assetId, int reservedYear)
        {
            List<AssetReservation> assetReservations = new List<AssetReservation>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT r.ass_rsv_id, r.asset_id, r.evnt_str_dt, r.evnt_nd_dt, r.evnt_ds, r.rsv_by, r.rsv_dt, r.evnt_loc, ");
            sb.Append("r.rsv_stt, r.asset_type_id, r.ass_rsv_mb, r.ass_rsv_md, a.asst_nm, a.asst_ds, t.typ_nm ");
            sb.Append("FROM public.asm_ass_rsvs r INNER JOIN public.asm_stt_asst a ON r.asset_id = a.asst_id ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON r.asset_type_id = t.typ_id WHERE (r.asset_id = @asst_id) ");
            sb.Append("AND ((EXTRACT(YEAR FROM r.evnt_str_dt))::INTEGER = @yr) ");
            sb.Append("ORDER BY r.ass_rsv_id DESC;");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var asset_id = cmd.Parameters.Add("@asst_id", NpgsqlDbType.Text);
                    var yr = cmd.Parameters.Add("@yr", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    asset_id.Value = assetId;
                    yr.Value = reservedYear;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assetReservations.Add(new AssetReservation()
                            {
                                AssetReservationID = reader["ass_rsv_id"] == DBNull.Value ? 0 : (int)reader["ass_rsv_id"],
                                AssetID = reader["asset_id"] == DBNull.Value ? string.Empty : reader["asset_id"].ToString(),
                                AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                                AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                                EventStartTime = reader["evnt_str_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["evnt_str_dt"],
                                EventEndTime = reader["evnt_nd_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["evnt_nd_dt"],
                                EventDescription = reader["evnt_ds"] == DBNull.Value ? string.Empty : reader["evnt_ds"].ToString(),
                                EventLocation = reader["evnt_loc"] == DBNull.Value ? string.Empty : reader["evnt_loc"].ToString(),
                                ReservationStatus = reader["rsv_stt"] == DBNull.Value ? string.Empty : reader["rsv_stt"].ToString(),
                                ReservedBy = reader["rsv_by"] == DBNull.Value ? string.Empty : reader["rsv_by"].ToString(),
                                ReservedOn = reader["rsv_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rsv_dt"],
                                AssetTypeID = reader["asset_type_id"] == DBNull.Value ? 0 : (int)reader["asset_type_id"],
                                AssetTypeName = reader["typ_nm"] == DBNull.Value ? string.Empty : reader["typ_nm"].ToString(),
                                LastModifiedBy = reader["ass_rsv_mb"] == DBNull.Value ? string.Empty : reader["ass_rsv_mb"].ToString(),
                                LastModifiedTime = reader["ass_rsv_md"] == DBNull.Value ? string.Empty : reader["ass_rsv_md"].ToString(),
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
            return assetReservations;
        }

        public async Task<IList<AssetReservation>> GetByAssetIdAndYearAndMonthAsync(string assetId, int reservedYear, int reservedMonth)
        {
            List<AssetReservation> assetReservations = new List<AssetReservation>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT r.ass_rsv_id, r.asset_id, r.evnt_str_dt, r.evnt_nd_dt, r.evnt_ds, r.rsv_by, r.rsv_dt, r.evnt_loc, ");
            sb.Append("r.rsv_stt, r.asset_type_id, r.ass_rsv_mb, r.ass_rsv_md, a.asst_nm, a.asst_ds, t.typ_nm ");
            sb.Append("FROM public.asm_ass_rsvs r INNER JOIN public.asm_stt_asst a ON r.asset_id = a.asst_id ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON r.asset_type_id = t.typ_id WHERE (r.asset_id = @asst_id) ");
            sb.Append("AND ((EXTRACT(YEAR FROM r.evnt_str_dt))::INTEGER = @yr) ");
            sb.Append("AND ((EXTRACT(MONTH FROM r.evnt_str_dt))::INTEGER = @mn) ");
            sb.Append("ORDER BY r.ass_rsv_id DESC;");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var asset_id = cmd.Parameters.Add("@asst_id", NpgsqlDbType.Text);
                    var yr = cmd.Parameters.Add("@yr", NpgsqlDbType.Integer);
                    var mn = cmd.Parameters.Add("@mn", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    asset_id.Value = assetId;
                    yr.Value = reservedYear;
                    mn.Value = reservedMonth;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assetReservations.Add(new AssetReservation()
                            {
                                AssetReservationID = reader["ass_rsv_id"] == DBNull.Value ? 0 : (int)reader["ass_rsv_id"],
                                AssetID = reader["asset_id"] == DBNull.Value ? string.Empty : reader["asset_id"].ToString(),
                                AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                                AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                                EventStartTime = reader["evnt_str_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["evnt_str_dt"],
                                EventEndTime = reader["evnt_nd_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["evnt_nd_dt"],
                                EventDescription = reader["evnt_ds"] == DBNull.Value ? string.Empty : reader["evnt_ds"].ToString(),
                                EventLocation = reader["evnt_loc"] == DBNull.Value ? string.Empty : reader["evnt_loc"].ToString(),
                                ReservationStatus = reader["rsv_stt"] == DBNull.Value ? string.Empty : reader["rsv_stt"].ToString(),
                                ReservedBy = reader["rsv_by"] == DBNull.Value ? string.Empty : reader["rsv_by"].ToString(),
                                ReservedOn = reader["rsv_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rsv_dt"],
                                AssetTypeID = reader["asset_type_id"] == DBNull.Value ? 0 : (int)reader["asset_type_id"],
                                AssetTypeName = reader["typ_nm"] == DBNull.Value ? string.Empty : reader["typ_nm"].ToString(),
                                LastModifiedBy = reader["ass_rsv_mb"] == DBNull.Value ? string.Empty : reader["ass_rsv_mb"].ToString(),
                                LastModifiedTime = reader["ass_rsv_md"] == DBNull.Value ? string.Empty : reader["ass_rsv_md"].ToString(),
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
            return assetReservations;
        }


        #endregion

        //====================== Current Asset Reservations Methods Starts Here =============================================//
        #region Current Assets Reservations Methods
        public async Task<IList<AssetReservation>> GetAllCurrentAsync()
        {
            List<AssetReservation> assetReservations = new List<AssetReservation>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT r.ass_rsv_id, r.asset_id, r.evnt_str_dt, r.evnt_nd_dt, r.evnt_ds, r.rsv_by, r.rsv_dt, r.evnt_loc, ");
            sb.Append("r.rsv_stt, r.asset_type_id, r.ass_rsv_mb, r.ass_rsv_md, a.asst_nm, a.asst_ds, t.typ_nm ");
            sb.Append("FROM public.asm_ass_rsvs r INNER JOIN public.asm_stt_asst a ON r.asset_id = a.asst_id ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON r.asset_type_id = t.typ_id ");
            sb.Append("WHERE (r.evnt_str_dt >= CURRENT_TIMESTAMP - '2 day'::interval)");
            sb.Append("ORDER BY r.asset_type_id, r.asset_id, r.ass_rsv_id DESC;");
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
                            assetReservations.Add(new AssetReservation()
                            {
                                AssetReservationID = reader["ass_rsv_id"] == DBNull.Value ? 0 : (int)reader["ass_rsv_id"],
                                AssetID = reader["asset_id"] == DBNull.Value ? string.Empty : reader["asset_id"].ToString(),
                                AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                                AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                                EventStartTime = reader["evnt_str_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["evnt_str_dt"],
                                EventEndTime = reader["evnt_nd_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["evnt_nd_dt"],
                                EventDescription = reader["evnt_ds"] == DBNull.Value ? string.Empty : reader["evnt_ds"].ToString(),
                                EventLocation = reader["evnt_loc"] == DBNull.Value ? string.Empty : reader["evnt_loc"].ToString(),
                                ReservationStatus = reader["rsv_stt"] == DBNull.Value ? string.Empty : reader["rsv_stt"].ToString(),
                                ReservedBy = reader["rsv_by"] == DBNull.Value ? string.Empty : reader["rsv_by"].ToString(),
                                ReservedOn = reader["rsv_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rsv_dt"],
                                AssetTypeID = reader["asset_type_id"] == DBNull.Value ? 0 : (int)reader["asset_type_id"],
                                AssetTypeName = reader["typ_nm"] == DBNull.Value ? string.Empty : reader["typ_nm"].ToString(),
                                LastModifiedBy = reader["ass_rsv_mb"] == DBNull.Value ? string.Empty : reader["ass_rsv_mb"].ToString(),
                                LastModifiedTime = reader["ass_rsv_md"] == DBNull.Value ? string.Empty : reader["ass_rsv_md"].ToString(),
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
            return assetReservations;
        }

        public async Task<IList<AssetReservation>> GetCurrentByAssetIdAsync(string assetId)
        {
            List<AssetReservation> assetReservations = new List<AssetReservation>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT r.ass_rsv_id, r.asset_id, r.evnt_str_dt, r.evnt_nd_dt, r.evnt_ds, r.rsv_by, r.rsv_dt, r.evnt_loc, ");
            sb.Append("r.rsv_stt, r.asset_type_id, r.ass_rsv_mb, r.ass_rsv_md, a.asst_nm, a.asst_ds, t.typ_nm ");
            sb.Append("FROM public.asm_ass_rsvs r INNER JOIN public.asm_stt_asst a ON r.asset_id = a.asst_id ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON r.asset_type_id = t.typ_id WHERE (r.asset_id = @asst_id) ");
            sb.Append("AND (r.evnt_str_dt >= CURRENT_TIMESTAMP - '2 day'::interval) ORDER BY r.ass_rsv_id DESC;");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var asset_id = cmd.Parameters.Add("@asst_id", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    asset_id.Value = assetId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assetReservations.Add(new AssetReservation()
                            {
                                AssetReservationID = reader["ass_rsv_id"] == DBNull.Value ? 0 : (int)reader["ass_rsv_id"],
                                AssetID = reader["asset_id"] == DBNull.Value ? string.Empty : reader["asset_id"].ToString(),
                                AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                                AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                                EventStartTime = reader["evnt_str_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["evnt_str_dt"],
                                EventEndTime = reader["evnt_nd_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["evnt_nd_dt"],
                                EventDescription = reader["evnt_ds"] == DBNull.Value ? string.Empty : reader["evnt_ds"].ToString(),
                                EventLocation = reader["evnt_loc"] == DBNull.Value ? string.Empty : reader["evnt_loc"].ToString(),
                                ReservationStatus = reader["rsv_stt"] == DBNull.Value ? string.Empty : reader["rsv_stt"].ToString(),
                                ReservedBy = reader["rsv_by"] == DBNull.Value ? string.Empty : reader["rsv_by"].ToString(),
                                ReservedOn = reader["rsv_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rsv_dt"],
                                AssetTypeID = reader["asset_type_id"] == DBNull.Value ? 0 : (int)reader["asset_type_id"],
                                AssetTypeName = reader["typ_nm"] == DBNull.Value ? string.Empty : reader["typ_nm"].ToString(),
                                LastModifiedBy = reader["ass_rsv_mb"] == DBNull.Value ? string.Empty : reader["ass_rsv_mb"].ToString(),
                                LastModifiedTime = reader["ass_rsv_md"] == DBNull.Value ? string.Empty : reader["ass_rsv_md"].ToString(),
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
            return assetReservations;
        }

        public async Task<IList<AssetReservation>> GetCurrentByAssetNameAsync(string assetName)
        {
            List<AssetReservation> assetReservations = new List<AssetReservation>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT r.ass_rsv_id, r.asset_id, r.evnt_str_dt, r.evnt_nd_dt, r.evnt_ds, r.rsv_by, r.rsv_dt, r.evnt_loc, ");
            sb.Append("r.rsv_stt, r.asset_type_id, r.ass_rsv_mb, r.ass_rsv_md, a.asst_nm, a.asst_ds, t.typ_nm ");
            sb.Append("FROM public.asm_ass_rsvs r INNER JOIN public.asm_stt_asst a ON r.asset_id = a.asst_id ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON r.asset_type_id = t.typ_id ");
            sb.Append("WHERE (r.evnt_str_dt >= CURRENT_TIMESTAMP - '2 day'::interval) ");
            sb.Append($"AND (LOWER(asst_nm) LIKE '%'||LOWER(@asst_nm)||'%') ORDER BY asst_nm;");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var asset_name = cmd.Parameters.Add("@asst_nm", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    asset_name.Value = assetName;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assetReservations.Add(new AssetReservation()
                            {
                                AssetReservationID = reader["ass_rsv_id"] == DBNull.Value ? 0 : (int)reader["ass_rsv_id"],
                                AssetID = reader["asset_id"] == DBNull.Value ? string.Empty : reader["asset_id"].ToString(),
                                AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                                AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                                EventStartTime = reader["evnt_str_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["evnt_str_dt"],
                                EventEndTime = reader["evnt_nd_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["evnt_nd_dt"],
                                EventDescription = reader["evnt_ds"] == DBNull.Value ? string.Empty : reader["evnt_ds"].ToString(),
                                EventLocation = reader["evnt_loc"] == DBNull.Value ? string.Empty : reader["evnt_loc"].ToString(),
                                ReservationStatus = reader["rsv_stt"] == DBNull.Value ? string.Empty : reader["rsv_stt"].ToString(),
                                ReservedBy = reader["rsv_by"] == DBNull.Value ? string.Empty : reader["rsv_by"].ToString(),
                                ReservedOn = reader["rsv_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rsv_dt"],
                                AssetTypeID = reader["asset_type_id"] == DBNull.Value ? 0 : (int)reader["asset_type_id"],
                                AssetTypeName = reader["typ_nm"] == DBNull.Value ? string.Empty : reader["typ_nm"].ToString(),
                                LastModifiedBy = reader["ass_rsv_mb"] == DBNull.Value ? string.Empty : reader["ass_rsv_mb"].ToString(),
                                LastModifiedTime = reader["ass_rsv_md"] == DBNull.Value ? string.Empty : reader["ass_rsv_md"].ToString(),
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
            return assetReservations;
        }

        public async Task<IList<AssetReservation>> GetCurrentByAssetTypeIdAsync(int assetTypeId)
        {
            List<AssetReservation> assetReservations = new List<AssetReservation>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT r.ass_rsv_id, r.asset_id, r.evnt_str_dt, r.evnt_nd_dt, r.evnt_ds, r.rsv_by, r.rsv_dt, r.evnt_loc, ");
            sb.Append("r.rsv_stt, r.asset_type_id, r.ass_rsv_mb, r.ass_rsv_md, a.asst_nm, a.asst_ds, t.typ_nm ");
            sb.Append("FROM public.asm_ass_rsvs r INNER JOIN public.asm_stt_asst a ON r.asset_id = a.asst_id ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON r.asset_type_id = t.typ_id WHERE (r.asset_type_id = @asst_type_id) ");
            sb.Append("AND (r.evnt_str_dt >= CURRENT_TIMESTAMP - '2 day'::interval) ORDER BY r.asset_id, r.ass_rsv_id DESC;");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var asset_type_id = cmd.Parameters.Add("@asst_type_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    asset_type_id.Value = assetTypeId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assetReservations.Add(new AssetReservation()
                            {
                                AssetReservationID = reader["ass_rsv_id"] == DBNull.Value ? 0 : (int)reader["ass_rsv_id"],
                                AssetID = reader["asset_id"] == DBNull.Value ? string.Empty : reader["asset_id"].ToString(),
                                AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                                AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                                EventStartTime = reader["evnt_str_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["evnt_str_dt"],
                                EventEndTime = reader["evnt_nd_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["evnt_nd_dt"],
                                EventDescription = reader["evnt_ds"] == DBNull.Value ? string.Empty : reader["evnt_ds"].ToString(),
                                EventLocation = reader["evnt_loc"] == DBNull.Value ? string.Empty : reader["evnt_loc"].ToString(),
                                ReservationStatus = reader["rsv_stt"] == DBNull.Value ? string.Empty : reader["rsv_stt"].ToString(),
                                ReservedBy = reader["rsv_by"] == DBNull.Value ? string.Empty : reader["rsv_by"].ToString(),
                                ReservedOn = reader["rsv_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rsv_dt"],
                                AssetTypeID = reader["asset_type_id"] == DBNull.Value ? 0 : (int)reader["asset_type_id"],
                                AssetTypeName = reader["typ_nm"] == DBNull.Value ? string.Empty : reader["typ_nm"].ToString(),
                                LastModifiedBy = reader["ass_rsv_mb"] == DBNull.Value ? string.Empty : reader["ass_rsv_mb"].ToString(),
                                LastModifiedTime = reader["ass_rsv_md"] == DBNull.Value ? string.Empty : reader["ass_rsv_md"].ToString(),
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
            return assetReservations;
        }

        #endregion

        //====================== Asset Reservations CRUD Methods Starts Here =============================================//
        #region Assets Reservations CRUD Methods
        public async Task<bool> AddAsync(AssetReservation assetReservation)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.asm_ass_rsvs(asset_id, evnt_str_dt, evnt_nd_dt, evnt_ds, rsv_by, rsv_dt, evnt_loc, ");
            sb.Append("rsv_stt, asset_type_id, ass_rsv_mb, ass_rsv_md) VALUES (@asset_id, @evnt_str_dt, @evnt_nd_dt, @evnt_ds, ");
            sb.Append("@rsv_by, @rsv_dt, @evnt_loc, @rsv_stt, @asset_type_id, @ass_rsv_mb, @ass_rsv_md);");
           string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var asset_id = cmd.Parameters.Add("@asset_id", NpgsqlDbType.Text);
                    var evnt_str_dt = cmd.Parameters.Add("evnt_str_dt", NpgsqlDbType.TimestampTz);
                    var evnt_nd_dt = cmd.Parameters.Add("@evnt_nd_dt", NpgsqlDbType.TimestampTz);
                    var evnt_ds = cmd.Parameters.Add("@evnt_ds", NpgsqlDbType.Text);
                    var rsv_by = cmd.Parameters.Add("@rsv_by", NpgsqlDbType.Text);
                    var rsv_dt = cmd.Parameters.Add("@rsv_dt", NpgsqlDbType.TimestampTz);
                    var evnt_loc = cmd.Parameters.Add("@evnt_loc", NpgsqlDbType.Text);
                    var rsv_stt = cmd.Parameters.Add("@rsv_stt", NpgsqlDbType.Text);
                    var type_id = cmd.Parameters.Add("@asset_type_id", NpgsqlDbType.Integer);
                    var ass_rsv_mb = cmd.Parameters.Add("@ass_rsv_mb", NpgsqlDbType.Text);
                    var ass_rsv_md = cmd.Parameters.Add("@ass_rsv_md", NpgsqlDbType.Text);
                    cmd.Prepare();
                    asset_id.Value = assetReservation.AssetID;
                    evnt_str_dt.Value = assetReservation.EventStartTime ?? (object)DBNull.Value;
                    evnt_nd_dt.Value = assetReservation.EventEndTime ?? (object)DBNull.Value;
                    evnt_ds.Value = assetReservation.EventDescription ?? (object)DBNull.Value;
                    rsv_by.Value = assetReservation.ReservedBy;
                    rsv_dt.Value = assetReservation.ReservedOn;
                    evnt_loc.Value = assetReservation.EventLocation;
                    rsv_stt.Value = assetReservation.ReservationStatus;
                    type_id.Value = assetReservation.AssetTypeID;
                    ass_rsv_mb.Value = assetReservation.LastModifiedBy ?? (object)DBNull.Value;
                    ass_rsv_md.Value = assetReservation.LastModifiedTime ?? (object)DBNull.Value;

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

        public async Task<bool> EditAsync(AssetReservation assetReservation)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.asm_ass_rsvs SET asset_id=@asset_id, evnt_str_dt=@evnt_str_dt, evnt_nd_dt=@evnt_nd_dt, ");
            sb.Append("evnt_ds=@evnt_ds, rsv_by=@rsv_by, rsv_dt=@rsv_dt, evnt_loc=@evnt_loc, rsv_stt=@rsv_stt, ");
            sb.Append("asset_type_id=@asset_type_id, ass_rsv_mb=@ass_rsv_mb, ass_rsv_md=@ass_rsv_md ");
            sb.Append("WHERE (ass_rsv_id = @ass_rsv_id);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var asset_reservation_id = cmd.Parameters.Add("@ass_rsv_id", NpgsqlDbType.Integer);
                    var asset_id = cmd.Parameters.Add("@asset_id", NpgsqlDbType.Text);
                    var evnt_str_dt = cmd.Parameters.Add("evnt_str_dt", NpgsqlDbType.TimestampTz);
                    var evnt_nd_dt = cmd.Parameters.Add("@evnt_nd_dt", NpgsqlDbType.TimestampTz);
                    var evnt_ds = cmd.Parameters.Add("@evnt_ds", NpgsqlDbType.Text);
                    var rsv_by = cmd.Parameters.Add("@rsv_by", NpgsqlDbType.Text);
                    var rsv_dt = cmd.Parameters.Add("@rsv_dt", NpgsqlDbType.TimestampTz);
                    var evnt_loc = cmd.Parameters.Add("@evnt_loc", NpgsqlDbType.Text);
                    var rsv_stt = cmd.Parameters.Add("@rsv_stt", NpgsqlDbType.Text);
                    var type_id = cmd.Parameters.Add("@asset_type_id", NpgsqlDbType.Integer);
                    var ass_rsv_mb = cmd.Parameters.Add("@ass_rsv_mb", NpgsqlDbType.Text);
                    var ass_rsv_md = cmd.Parameters.Add("@ass_rsv_md", NpgsqlDbType.Text);
                    cmd.Prepare();
                    asset_reservation_id.Value = assetReservation.AssetReservationID;
                    asset_id.Value = assetReservation.AssetID;
                    evnt_str_dt.Value = assetReservation.EventStartTime ?? (object)DBNull.Value;
                    evnt_nd_dt.Value = assetReservation.EventEndTime ?? (object)DBNull.Value;
                    evnt_ds.Value = assetReservation.EventDescription ?? (object)DBNull.Value;
                    rsv_by.Value = assetReservation.ReservedBy;
                    rsv_dt.Value = assetReservation.ReservedOn;
                    evnt_loc.Value = assetReservation.EventLocation;
                    rsv_stt.Value = assetReservation.ReservationStatus;
                    type_id.Value = assetReservation.AssetTypeID;
                    ass_rsv_mb.Value = assetReservation.LastModifiedBy ?? (object)DBNull.Value;
                    ass_rsv_md.Value = assetReservation.LastModifiedTime ?? (object)DBNull.Value;

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

        public async Task<bool> DeleteAsync(int assetReservationId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = $"DELETE FROM public.asm_ass_rsvs WHERE ass_rsv_id = @ass_rsv_id;";
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var ass_rsv_id = cmd.Parameters.Add("@ass_rsv_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    ass_rsv_id.Value = assetReservationId;
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
