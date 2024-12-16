using IntranetPortal.Base.Models.ContentManagerModels;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.ContentManagerRepositories
{
    public interface IPostMediaRepository
    {
        IConfiguration _config { get; }

        #region Post Media Write Action Interfaces
        Task<bool> AddPostMediaAsync(PostMedia postMedia);
        Task<bool> DeletePostMediaAsync(long id);
        #endregion

        #region Post Media Read Action Interfaces
        Task<PostMedia> GetByIdAsync(long postMediaId);
        Task<IList<PostMedia>> GetByMasterPostIdAsync(long masterPostId);
        #endregion
    }
}