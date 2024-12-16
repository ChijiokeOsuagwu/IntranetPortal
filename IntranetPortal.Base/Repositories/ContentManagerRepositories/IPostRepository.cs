using IntranetPortal.Base.Models.ContentManagerModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.ContentManagerRepositories
{
    public interface IPostRepository
    {
        #region Posts Write Action Methods
        Task<bool> AddPostAsync(Post post);
        Task<bool> EditPostAsync(Post post);
        Task<bool> EditPostWithoutImageAsync(Post post);
        Task<bool> DeletePostAsync(long id);

        #endregion

        #region Posts Read Action Methods
        Task<IList<Post>> GetAllAsync();
        Task<IList<Post>> GetByTypeIdAsync(int typeId);
        Task<IList<Post>> GetByTypeIdAsync(int typeId, bool withHidden = false);
        Task<IList<Post>> GetByTitleAsync(string postTitle);
        Task<IList<Post>> GetByTitleAsync(string postTitle, int postTypeId);
        Task<Post> GetPostByIdAsync(long id);
        Task<IList<Post>> GetPostsWithoutBannersAndAnnouncementsAsync();
        Task<IList<Post>> GetUnhiddenPostsWithoutBannersAndAnnouncementsAsync();

        #endregion

        #region Post Details Read Action Methods

        Task<PostDetail> GetPostDetailsByIdAsync(long id);
        Task<bool> AddPostDetailAsync(long id, string htmlContent, string modifiedBy, DateTime modifiedDate);
        #endregion

    }
}
