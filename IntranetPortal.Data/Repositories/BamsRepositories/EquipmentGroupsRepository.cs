using IntranetPortal.Base.Models.BamsModels;
using IntranetPortal.Base.Repositories.BamsRepositories;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Data.Repositories.BamsRepositories
{
    public class EquipmentGroupsRepository : IEquipmentGroupsRepository
    {
        public IConfiguration _config { get; }
        public EquipmentGroupsRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        //============== Equipment Groups Action Methods =====================================================//
        #region Equipment Groups Action Methods

        public async Task<EquipmentGroup> GetByIdAsync(int Id)
        {
            EquipmentGroup equipmentGroup = new EquipmentGroup();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT eqmt_grp_id, eqmt_grp_nm, eqmt_grp_ds, created_by, created_time ");
            sb.Append("FROM public.bam_eqmt_grps WHERE (eqmt_grp_id = @eqmt_grp_id); ");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var eqmt_grp_id = cmd.Parameters.Add("@eqmt_grp_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    eqmt_grp_id.Value = Id;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            equipmentGroup.EquipmentGroupID = reader["eqmt_grp_id"] == DBNull.Value ? 0 : (int)reader["eqmt_grp_id"];
                            equipmentGroup.EquipmentGroupName = reader["eqmt_grp_nm"] == DBNull.Value ? string.Empty : reader["eqmt_grp_nm"].ToString();
                            equipmentGroup.EquipmentGroupDescription = reader["eqmt_grp_ds"] == DBNull.Value ? string.Empty : reader["eqmt_grp_ds"].ToString();
                            equipmentGroup.CreatedBy = reader["created_by"] == DBNull.Value ? string.Empty : reader["created_by"].ToString();
                            equipmentGroup.CreatedTime = reader["created_time"] == DBNull.Value ? string.Empty : reader["created_time"].ToString();
                        }
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message);
            }
            return equipmentGroup;
        }

        public async Task<IList<EquipmentGroup>> GetAllAsync()
        {
            IList<EquipmentGroup> equipmentGroups = new List<EquipmentGroup>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT eqmt_grp_id, eqmt_grp_nm, eqmt_grp_ds, created_by, created_time ");
            sb.Append("FROM public.bam_eqmt_grps ORDER BY eqmt_grp_nm; ");
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
                            equipmentGroups.Add(new EquipmentGroup
                            {
                                EquipmentGroupID = reader["eqmt_grp_id"] == DBNull.Value ? 0 : (int)reader["eqmt_grp_id"],
                                EquipmentGroupName = reader["eqmt_grp_nm"] == DBNull.Value ? string.Empty : reader["eqmt_grp_nm"].ToString(),
                                EquipmentGroupDescription = reader["eqmt_grp_ds"] == DBNull.Value ? string.Empty : reader["eqmt_grp_ds"].ToString(),
                                CreatedBy = reader["created_by"] == DBNull.Value ? string.Empty : reader["created_by"].ToString(),
                                CreatedTime = reader["created_time"] == DBNull.Value ? string.Empty : reader["created_time"].ToString(),
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
            return equipmentGroups;
        }

        #endregion

        //============== Equipment Group CRUD Action Methods =================================================//
        #region Equipment Group CRUD Action Methods
        public async Task<bool> AddAsync(EquipmentGroup equipmentGroup)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.bam_eqmt_grps(eqmt_grp_nm, eqmt_grp_ds, ");
            sb.Append("created_by, created_time) VALUES (@eqmt_grp_nm, @eqmt_grp_ds, ");
            sb.Append(" @created_by, @created_time);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var eqmt_grp_nm = cmd.Parameters.Add("@eqmt_grp_nm", NpgsqlDbType.Text);
                    var eqmt_grp_ds = cmd.Parameters.Add("@eqmt_grp_ds", NpgsqlDbType.Text);
                    var created_by = cmd.Parameters.Add("@created_by", NpgsqlDbType.Text);
                    var created_time = cmd.Parameters.Add("@created_time", NpgsqlDbType.Text);
                    cmd.Prepare();
                    eqmt_grp_nm.Value = equipmentGroup.EquipmentGroupName;
                    eqmt_grp_ds.Value = equipmentGroup.EquipmentGroupDescription ?? (object)DBNull.Value;
                    created_by.Value = equipmentGroup.CreatedBy ?? (object)DBNull.Value;
                    created_time.Value = equipmentGroup.CreatedTime ?? (object)DBNull.Value;
                    
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

        public async Task<bool> EditAsync(EquipmentGroup equipmentGroup)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.bam_eqmt_grps SET eqmt_grp_nm = @eqmt_grp_nm, ");
            sb.Append("eqmt_grp_ds = @eqmt_grp_ds WHERE (eqmt_grp_id = @eqmt_grp_id); ");
            sb.Append(" ");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var eqmt_grp_id = cmd.Parameters.Add("@eqmt_grp_id", NpgsqlDbType.Integer);
                    var eqmt_grp_nm = cmd.Parameters.Add("@eqmt_grp_nm", NpgsqlDbType.Text);
                    var eqmt_grp_ds = cmd.Parameters.Add("@eqmt_grp_ds", NpgsqlDbType.Text);
                    var created_by = cmd.Parameters.Add("@created_by", NpgsqlDbType.Text);
                    var created_time = cmd.Parameters.Add("@created_time", NpgsqlDbType.Text);
                    cmd.Prepare();
                    eqmt_grp_id.Value = equipmentGroup.EquipmentGroupID;
                    eqmt_grp_nm.Value = equipmentGroup.EquipmentGroupName;
                    eqmt_grp_ds.Value = equipmentGroup.EquipmentGroupDescription ?? (object)DBNull.Value;
                    created_by.Value = equipmentGroup.CreatedBy ?? (object)DBNull.Value;
                    created_time.Value = equipmentGroup.CreatedTime ?? (object)DBNull.Value;

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

        public async Task<bool> DeleteAsync(int equipmentGroupId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = $"DELETE FROM public.bam_eqmt_grps WHERE (eqmt_grp_id = @eqmt_grp_id);";
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var eqmt_grp_id = cmd.Parameters.Add("@eqmt_grp_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    eqmt_grp_id.Value = equipmentGroupId;
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
