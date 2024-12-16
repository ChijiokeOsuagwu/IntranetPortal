using IntranetPortal.Base.Enums;
using IntranetPortal.Base.Models.ContentManagerModels;
using IntranetPortal.Base.Repositories.ContentManagerRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Services
{
    public class ContentManagerService : IContentManagerService
    {
        private readonly IPostRepository _postRepository;
        private readonly IPostMediaRepository _postMediaRepository;

        public ContentManagerService(IPostRepository postRepository, IPostMediaRepository postMediaRepository)
        {
            _postRepository = postRepository;
            _postMediaRepository = postMediaRepository;
        }

        #region Posts Write Service Methods
        public async Task<bool> CreatePostAsync(Post post)
        {
            if (post == null) { throw new ArgumentNullException(nameof(post), "Required parameter [post] is missing."); }
            return await _postRepository.AddPostAsync(post);
        }
        public async Task<bool> DeletePostAsync(long postId)
        {
            if (postId < 1) { throw new ArgumentNullException(nameof(postId), "Required parameter [postId] is missing."); }
            return await _postRepository.DeletePostAsync(postId);
        }
        public async Task<bool> UpdatePostAsync(Post post)
        {
            if (post == null) { throw new ArgumentNullException(nameof(post), "Required parameter [post] is missing."); }
            //post.PostTypeId = (int)PostType.Banner;
            if (!string.IsNullOrWhiteSpace(post.ImagePath))
            {
                return await _postRepository.EditPostAsync(post);
            }
            return await _postRepository.EditPostWithoutImageAsync(post);
        }
        public async Task<bool> HidePostAsync(long postId)
        {
            if (postId < 1) { throw new ArgumentNullException(nameof(postId), "Required parameter [postId] is missing."); }
            return await _postRepository.DeletePostAsync(postId);
        }
        public async Task<bool> UpdatePostDetailsAsync(long postId, string htmlContent, string modifiedBy, DateTime modifiedDate)
        {
            if (postId < 1) { throw new ArgumentNullException(nameof(postId), "Required parameter [postId] has invalid value."); }
            if (string.IsNullOrEmpty(htmlContent)) { throw new ArgumentNullException(nameof(htmlContent), "Required parameter [htmlContent] has invalid value."); }
            if (string.IsNullOrEmpty(modifiedBy)) { throw new ArgumentNullException(nameof(modifiedBy), "Required parameter [modifiedBy] has invalid value."); }
            if (modifiedDate == null) { throw new ArgumentNullException(nameof(modifiedDate), "Required parameter [modifiedDate] has invalid value."); }

            return await _postRepository.AddPostDetailAsync(postId, htmlContent, modifiedBy, modifiedDate);
        }

        #endregion

        #region Posts Read Service Methods

        public async Task<IList<Post>> GetAllPostsAsync()
        {
            return await _postRepository.GetAllAsync();
        }

        public async Task<Post> GetPostByIdAsync(long postId)
        {
            if (postId < 1) { throw new ArgumentNullException(nameof(postId), "Required parameter [postId] is missing."); }
            return await _postRepository.GetPostByIdAsync(postId);
        }
        
        public async Task<IList<Post>> GetPostsByPostTypeId(int postTypeId)
        {
            return await _postRepository.GetByTypeIdAsync(postTypeId);
        }
        
        public async Task<IList<Post>> SearchPostsByTitle(string postTitle, int postTypeId)
        {
            return await _postRepository.GetByTitleAsync(postTitle, postTypeId);
        }

        public async Task<IList<Post>> GetAllPostsExceptBannersnAnnouncementsAsync()
        {
            return await _postRepository.GetPostsWithoutBannersAndAnnouncementsAsync();
        }

        public async Task<IList<Post>> GetAllUnhiddenPostsExceptBannersnAnnouncementsAsync()
        {
            return await _postRepository.GetUnhiddenPostsWithoutBannersAndAnnouncementsAsync();
        }


        #endregion

        #region Articles Read Service Methods

        public async Task<IList<Post>> GetAllArticlesAsync()
        {
            return await _postRepository.GetByTypeIdAsync((int)PostType.Article);
        }

        public async Task<IList<Post>> GetUnhiddenArticlesAsync()
        {
            return await _postRepository.GetByTypeIdAsync((int)PostType.Article, false);
        }

        public async Task<IList<Post>> GetHiddenArticlesAsync()
        {
            return await _postRepository.GetByTypeIdAsync((int)PostType.Article, true);
        }

        #endregion

        #region Photos Read Service Methods
        public async Task<IList<Post>> GetAllPhotosAsync()
        {
            return await _postRepository.GetByTypeIdAsync((int)PostType.Photos);
        }

        public async Task<IList<Post>> GetUnhiddenPhotosAsync()
        {
            return await _postRepository.GetByTypeIdAsync((int)PostType.Photos, false);
        }

        public async Task<IList<Post>> GetHiddenPhotosAsync()
        {
            return await _postRepository.GetByTypeIdAsync((int)PostType.Photos, true);
        }

        #endregion

        #region Post Media Write Service Methods
        public async Task<bool> AddPostMediaAsync(PostMedia postMedia)
        {
            if (postMedia == null) { throw new ArgumentNullException(nameof(postMedia), "Required parameter [Post Media] is missing."); }
            return await _postMediaRepository.AddPostMediaAsync(postMedia);
        }
        public async Task<bool> DeletePostMediaAsync(long postMediaId)
        {
            if (postMediaId < 1) { throw new ArgumentNullException(nameof(postMediaId), "Required parameter [Post Media ID] is missing."); }
            return await _postMediaRepository.DeletePostMediaAsync(postMediaId);
        }
        #endregion

        #region Post Media Service Methods
        public async Task<PostMedia> GetPostMediaByIdAsync(long postMediaId)
        {
            if (postMediaId < 1) { throw new ArgumentNullException(nameof(postMediaId), "Required parameter [Post Media ID] is missing."); }
            return await _postMediaRepository.GetByIdAsync(postMediaId);
        }
        public async Task<IList<PostMedia>> GetPostMediasByMasterPostId(long masterPostId)
        {
            return await _postMediaRepository.GetByMasterPostIdAsync(masterPostId);
        }

        #endregion

        #region Banners Action Methods
        public async Task<IList<Post>> GetAllBannersAsync()
        {
            return await _postRepository.GetByTypeIdAsync((int)PostType.Banner);
        }

        public async Task<IList<Post>> GetUnhiddenBannersAsync()
        {
            return await _postRepository.GetByTypeIdAsync((int)PostType.Banner, false);
        }

        #endregion

        #region Announcements Action Methods
        public async Task<IList<Post>> GetAllAnnouncementsAsync()
        {
            return await _postRepository.GetByTypeIdAsync((int)PostType.Announcement);
        }

        public async Task<IList<Post>> GetUnhiddenAnnouncementsAsync()
        {
            return await _postRepository.GetByTypeIdAsync((int)PostType.Announcement, false);
        }

        #endregion

        


     
    }
}
