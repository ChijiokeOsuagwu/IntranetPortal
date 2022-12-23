using IntranetPortal.Base.Models.ContentManagerModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.ContentManagerRepositories
{
    public interface IPostRepository
    {
        #region Posts Action Methods
        Task<bool> AddPostAsync(Post post);
        Task<bool> EditPostAsync(Post post);
        Task<bool> DeletePostAsync(int id);
        Task<Post> GetPostByIdAsync(int id);
        Task<IList<Post>> GetAllAsync();
        Task<IList<Post>> GetPostsWithoutBannersAndAnnouncementsAsync();
        Task<IList<Post>> GetUnhiddenPostsWithoutBannersAndAnnouncementsAsync();
        Task<IList<Post>> GetByTypeIdAsync(int typeId);
        Task<IList<Post>> GetByTitleAsync(string postTitle);
        Task<IList<Post>> GetAllAnnouncementsAsync();
        Task<IList<Post>> GetUnhiddenAnnouncementsAsync();

        Task<PostDetail> GetPostDetailsByIdAsync(int id);
        Task<bool> AddPostDetailAsync(int id, string htmlContent, string modifiedBy, DateTime modifiedDate);
        #endregion

        #region Other Action Methods
        Task<IList<Post>> GetAllBannersAsync();
        Task<IList<Post>> GetUnhiddenBannersAsync();
        Task<IList<Post>> GetAllArticlesAsync();
        Task<IList<Post>> GetUnhiddenArticlesAsync();
        Task<IList<Post>> GetCelebrantsAsync();
        Task<IList<Post>> GetAnnouncementsAsync();
        Task<IList<Post>> GetEventsAsync();

        #endregion
    }
}
