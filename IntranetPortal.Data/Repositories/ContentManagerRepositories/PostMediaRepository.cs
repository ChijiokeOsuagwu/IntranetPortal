using IntranetPortal.Base.Enums;
using IntranetPortal.Base.Models.ContentManagerModels;
using IntranetPortal.Base.Repositories.ContentManagerRepositories;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Data.Repositories.ContentManagerRepositories
{
    public class PostMediaRepository : IPostMediaRepository
    {
        public IConfiguration _config { get; }
        public PostMediaRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        //======= Post Media Write Action Methods ==========//
        #region Post Media Write Action Methods
        public async Task<bool> AddPostMediaAsync(PostMedia postMedia)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.pcm_pstm (typ_id, locp, mpid, caption, mfpth) ");
            sb.Append("VALUES (@typ_id, @locp, @mpid, @caption, @mfpth); ");

            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var typ_id = cmd.Parameters.Add("@typ_id", NpgsqlDbType.Integer);
                var locp = cmd.Parameters.Add("@locp", NpgsqlDbType.Text);
                var mpid = cmd.Parameters.Add("@mpid", NpgsqlDbType.Bigint);
                var caption = cmd.Parameters.Add("@caption", NpgsqlDbType.Text);
                var mfpth = cmd.Parameters.Add("@mfpth", NpgsqlDbType.Text);
                cmd.Prepare();
                typ_id.Value = (int)postMedia.MediaType;
                locp.Value = postMedia.MediaLocationPath;
                mpid.Value = postMedia.MasterPostId;
                caption.Value = postMedia.Caption ?? string.Empty;
                mfpth.Value = postMedia.MediaLocationFullPath ?? string.Empty;

                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }
        public async Task<bool> DeletePostMediaAsync(long id)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = "DELETE FROM public.pcm_pstm	WHERE (id = @id);";

            await conn.OpenAsync();
            //Delete data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var postid = cmd.Parameters.Add("@id", NpgsqlDbType.Bigint);
                cmd.Prepare();
                postid.Value = id;
                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }
        #endregion

        //====== Post Media Read Action Methods ============//
        #region Post Media Read Action Methods
        public async Task<PostMedia> GetByIdAsync(long postMediaId)
        {
            if (postMediaId < 1) { return null; }
            PostMedia media = new PostMedia();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = string.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT id, locp, mpid, typ_id, caption, mfpth, ");
            sb.Append("CASE WHEN typ_id = 0 THEN 'Image' ");
            sb.Append("WHEN typ_id = 1 THEN 'Video' ");
            sb.Append("WHEN typ_id = 2 THEN 'Audio' END typ_ds ");
            sb.Append("FROM public.pcm_pstm ");
            sb.Append("WHERE id = @id;");
            query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var id = cmd.Parameters.Add("@id", NpgsqlDbType.Bigint);
                await cmd.PrepareAsync();
                id.Value = postMediaId;

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        media.PostMediaId = reader["id"] == DBNull.Value ? 0 : (long)reader["id"];
                        media.Caption = reader["caption"] == DBNull.Value ? string.Empty : reader["caption"].ToString();
                        media.MediaLocationPath = reader["locp"] == DBNull.Value ? string.Empty : reader["locp"].ToString();
                        media.MediaLocationFullPath = reader["mfpth"] == DBNull.Value ? string.Empty : reader["mfpth"].ToString();
                        media.MasterPostId = reader["mpid"] == DBNull.Value ? 0 : (long)reader["mpid"];
                        media.MediaType = reader["typ_id"] == DBNull.Value ? 0 : (MediaType)reader["typ_id"];
                        media.MediaTypeDescription = reader["typ_ds"] == DBNull.Value ? string.Empty : reader["typ_ds"].ToString();
                    }
            }
            await conn.CloseAsync();
            return media;
        }

        public async Task<IList<PostMedia>> GetByMasterPostIdAsync(long masterPostId)
        {
            List<PostMedia> medialist = new List<PostMedia>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = string.Empty;

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT id, locp, mpid, typ_id, caption, mfpth, ");
            sb.Append("CASE WHEN typ_id = 0 THEN 'Image' ");
            sb.Append("WHEN typ_id = 1 THEN 'Video' ");
            sb.Append("WHEN typ_id = 2 THEN 'Audio' END typ_ds ");
            sb.Append("FROM public.pcm_pstm ");
            sb.Append("WHERE mpid = @mpid;");
            query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var mpid = cmd.Parameters.Add("@mpid", NpgsqlDbType.Bigint);
                await cmd.PrepareAsync();
                mpid.Value = masterPostId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    medialist.Add(new PostMedia()
                    {
                        PostMediaId = reader["id"] == DBNull.Value ? 0 : (long)(reader["id"]),
                        Caption = reader["caption"] == DBNull.Value ? string.Empty : reader["caption"].ToString(),
                        MediaLocationPath = reader["locp"] == DBNull.Value ? string.Empty : reader["locp"].ToString(),
                        MasterPostId = reader["mpid"] == DBNull.Value ? 0 : (long)reader["mpid"],
                        MediaLocationFullPath = reader["mfpth"] == DBNull.Value ? string.Empty : reader["mfpth"].ToString(),
                        MediaType = reader["typ_id"] == DBNull.Value ? 0 : (MediaType)reader["typ_id"],
                        MediaTypeDescription = reader["typ_ds"] == DBNull.Value ? string.Empty : reader["typ_ds"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return medialist;
        }

        #endregion
    }
}
