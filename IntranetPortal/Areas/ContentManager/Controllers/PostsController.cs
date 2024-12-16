using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IntranetPortal.Areas.ContentManager.Models;
using IntranetPortal.Base.Enums;
using IntranetPortal.Base.Models.ContentManagerModels;
using IntranetPortal.Base.Services;
using IntranetPortal.Configurations;
using IntranetPortal.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace IntranetPortal.Areas.ContentManager.Controllers
{
    [Area("ContentManager")]
    [Authorize]
    public class PostsController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;
        private readonly IContentManagerService _contentManager;
        private readonly IDataProtector _dataProtector;
        public PostsController(IWebHostEnvironment webHostingEnvironment, IConfiguration configuration,
                                    IContentManagerService contentManager, IDataProtectionProvider dataProtectionProvider,
                                    DataProtectionEncryptionStrings dataProtectionEncryptionStrings)
        {
            _webHostEnvironment = webHostingEnvironment;
            _configuration = configuration;
            _contentManager = contentManager;
            _dataProtector = dataProtectionProvider.CreateProtector(dataProtectionEncryptionStrings.RouteValuesEncryptionCode);
        }

        [Authorize(Roles = "PCMMGACNT, XYALLACCZ")]
        public async Task<IActionResult> List(string ss)
        {
            PostListViewModel model = new PostListViewModel();
            IList<Post> entities = new List<Post>();

            if(!string.IsNullOrWhiteSpace(ss))
            {
                model.ss = ss;
                entities = await _contentManager.SearchPostsByTitle(ss, (int)PostType.Article);
            }
            else
            {
                entities = await _contentManager.GetPostsByPostTypeId((int)PostType.Article);
            }

            //if (entities != null) {
            //    foreach( var i in entities)
            //    {
            //        i.PostTypeName = i.PostTypeId switch
            //        {
            //            0 => "Banner",
            //            1 => "Celebrant",
            //            2 => "Article",
            //            3 => "Announcement",
            //            4 => "Photos",
            //            _ => "Post",
            //        };
            //    }
            //     
            //}
            model.PostList = entities.ToList();
            return View(model);
        }

        [Authorize(Roles = "PCMMGACNT, XYALLACCZ")]
        public IActionResult Add()
        {
            PostViewModel model = new PostViewModel();
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "PCMMGACNT, XYALLACCZ")]
        public async Task<IActionResult> Add(PostViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uploadsFolder = null;
                string absoluteFilePath = null;

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    var supportedTypes = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    FileInfo fileInfo = new FileInfo(model.ImageFile.FileName);
                    var fileExt = fileInfo.Extension;
                    if (!supportedTypes.Contains(fileExt))
                    {
                        model.ViewModelErrorMessage = "Sorry, invalid image format. Only images of type jpg, jpeg, png, gif are permitted.";
                        return View(model);
                    }

                    //if(fileInfo.Length / (1048576) > 1)
                    //{
                    //    model.ViewModelErrorMessage = "Sorry, this image is too large. Image size must not exceed 1MB.";
                    //    return View(model);
                    //}

                    uploadsFolder = "uploads/cms/" + Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
                    absoluteFilePath = Path.Combine(_webHostEnvironment.WebRootPath, uploadsFolder);
                    await model.ImageFile.CopyToAsync(new FileStream(absoluteFilePath, FileMode.Create));
                }
                Post post = new Post
                {
                    PostTitle = model.PostTitle,
                    ImagePath = "/" + uploadsFolder,
                    ImageFullPath = absoluteFilePath,
                    PostDetails = model.PostDetails,
                    PostDetailsRaw = model.PostDetailsRaw,
                    EnableComment = model.EnableComments,
                    IsHidden = model.IsHidden,
                    PostSummary = model.PostSummary,
                    PostTypeId = model.PostTypeId,
                    ModifiedDate = DateTime.UtcNow,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedBy = HttpContext.User.Identity.Name ?? string.Empty,
                    CreatedBy = HttpContext.User.Identity.Name ?? string.Empty
                };

                if (await _contentManager.CreatePostAsync(post))
                {
                    return RedirectToAction("List", "Posts");
                }
                else
                {
                    FileInfo file = new FileInfo(absoluteFilePath);
                    if (file.Exists)
                    {
                        if (!file.IsFileOpen())
                        {
                            await Task.Run(() => {
                                file.Delete();
                            });
                        }
                    }
                    model.ViewModelErrorMessage = "Sorry, an error was encountered. New Post could not be added.";
                }
            }
            return View(model);
        }

        [Authorize(Roles = "PCMMGACNT, XYALLACCZ")]
        public async Task<IActionResult> Delete(string id)
        {
            int PostId = Convert.ToInt32(_dataProtector.Unprotect(id));
            PostViewModel model = new PostViewModel();
            var post = await _contentManager.GetPostByIdAsync(PostId);
            model.PostId = post.PostId;
            model.ImagePath = post.ImagePath;
            model.PostSummary = post.PostSummary;
            model.PostTitle = post.PostTitle;
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "PCMMGACNT, XYALLACCZ")]
        public async Task<IActionResult> Delete(PostViewModel model)
        {
            if (model == null || model.PostId < 1)
            {
                model.ViewModelErrorMessage = $"Error! Sorry, an error was encountered. Article could not be deleted.";
                return View(model);
            }
            var entity = await _contentManager.GetPostByIdAsync(model.PostId.Value);
            long PostId = model.PostId.Value;
            string filePath = string.Empty;
            //if (!string.IsNullOrEmpty(entity.ImageFullPath))
            //{
            //    filePath = Path.Combine(_webHostEnvironment.WebRootPath, entity.ImageFullPath);
            //}
            FileInfo file = new FileInfo(entity.ImageFullPath);
            if (file.Exists)
            {
                if (!file.IsFileOpen())
                {
                    await Task.Run(() => {
                        file.Delete();
                    });
                }
            }
            var result = await _contentManager.DeletePostAsync(PostId);
            if (!result)
            {
                model.ViewModelErrorMessage = $"Error! Sorry, an error was encountered. Post could not be deleted.";
                return View(model);
            }
            TempData["DeleteSuccessMessage"] = "The Post was successfully deleted!";
            return RedirectToAction("Index", "Articles");
        }

        [Authorize(Roles = "PCMMGACNT, XYALLACCZ")]
        public async Task<IActionResult> Edit(long id)
        {
            long PostId = 0;
            if (id > 0) { PostId = id; }
            PostEditViewModel model = new PostEditViewModel();
            var post = await _contentManager.GetPostByIdAsync(PostId);
            model.PostId = post.PostId;
            model.ImagePath = post.ImagePath;
            model.OldImagePath = post.ImagePath;
            model.PostSummary = post.PostSummary;
            model.PostTitle = post.PostTitle;
            model.EnableComments = post.EnableComment;
            model.IsHidden = post.IsHidden;
            model.PostTypeId = (int)post.PostTypeId;
            model.PostDetailsRaw = post.PostDetailsRaw;
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "PCMMGACNT, XYALLACCZ")]
        public async Task<IActionResult> Edit(PostEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                Post post = new Post();
                string absoluteFilePath = string.Empty;
                string uploadedFilePath = string.Empty;

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    var supportedTypes = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    FileInfo fileInfo = new FileInfo(model.ImageFile.FileName);
                    var fileExt = fileInfo.Extension;
                    if (!supportedTypes.Contains(fileExt))
                    {
                        model.ViewModelErrorMessage = "Sorry, invalid image format. Only images of type jpg, jpeg, png, gif are permitted.";
                        return View(model);
                    }

                    uploadedFilePath = "uploads/cms/" + Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
                    absoluteFilePath = Path.Combine(_webHostEnvironment.WebRootPath, uploadedFilePath);
                    await model.ImageFile.CopyToAsync(new FileStream(absoluteFilePath, FileMode.Create));

                    var entity = await _contentManager.GetPostByIdAsync(model.PostId.Value);
                    if(entity != null)
                    {
                        FileInfo oldFile = new FileInfo(entity.ImageFullPath); //(Path.Combine(_webHostEnvironment.WebRootPath, model.OldImagePath));
                        if (oldFile.Exists)
                        {
                            if (!oldFile.IsFileOpen())
                            {
                                await Task.Run(() => {
                                    oldFile.Delete();
                                });
                            }
                        }
                    }
                }

                post.IsHidden = model.IsHidden;
                post.PostSummary = model.PostSummary;
                post.PostDetails = model.PostDetails;
                post.PostTypeId = model.PostTypeId;
                post.ModifiedDate = DateTime.UtcNow;
                post.ModifiedBy = HttpContext.User.Identity.Name ?? string.Empty;
                post.PostTitle = model.PostTitle;
                post.ImagePath = "/"+ uploadedFilePath;
                post.ImageFullPath = absoluteFilePath;
                post.EnableComment = model.EnableComments;
                post.PostDetailsRaw = model.PostDetailsRaw;
                post.PostId = model.PostId.Value;

                if (await _contentManager.UpdatePostAsync(post))
                {
                    return RedirectToAction("List", "Posts");
                }
                else
                {
                    FileInfo file = new FileInfo(absoluteFilePath);
                    if (file.Exists)
                    {
                        if (!file.IsFileOpen())
                        {
                            await Task.Run(() => {
                                file.Delete();
                            });
                        }
                    }
                    model.ViewModelErrorMessage = "Sorry, an error was encountered. Post Could not be added. Please try again.";
                }
            }
            return View(model);
        }

        [Authorize(Roles = "PCMMGACNT, XYALLACCZ")]
        public async Task<IActionResult> Announcement()
        {
            AnnouncementViewModel model = new AnnouncementViewModel();
            model.PostTypeId = 3;

            var entities = await _contentManager.GetAllAnnouncementsAsync();
            if(entities != null && entities.Count > 0)
            {
                var post = entities.FirstOrDefault();
                model.PostId = post.PostId;
                model.PostDetails = post.PostDetails;
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "PCMMGACNT, XYALLACCZ")]
        public async Task<IActionResult> Announcement(AnnouncementViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Post post = new Post();
                    post.PostDetails = model.PostDetails;
                    post.PostTypeId = model.PostTypeId;
                    post.ModifiedDate = DateTime.UtcNow;
                    post.ModifiedBy = HttpContext.User.Identity.Name ?? string.Empty;
                    post.PostId = model.PostId ?? 0;

                    bool IsSuccessful = false;
                    if (post.PostId > 0)
                    {
                        IsSuccessful = await _contentManager.UpdatePostAsync(post);
                    }
                    else
                    {
                        IsSuccessful = await _contentManager.CreatePostAsync(post);
                    }

                    if (IsSuccessful)
                    {
                        model.OperationIsCompleted = true;
                        model.OperationIsSuccessful = true;
                        model.ViewModelSuccessMessage = "Announcement updated successfully!";
                    }
                    else
                    {
                        model.ViewModelErrorMessage = "Announcement was not updated. An error was encountered. Please try again.";
                    }
                }
                catch(Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }
            return View(model);
        }

        #region Helper Controller Action Methods
        public string DeletePost(int id)
        {
            if (id < 1) { return "parameter error"; }
            string actionBy = HttpContext.User.Identity.Name;
            try
            {
                if(_contentManager.DeletePostAsync(id).Result)
                {
                    return "success";
                }
                else
                {
                    return "method failure";
                }
            }
            catch
            {
                return "service error";
            }
        }
        #endregion
    }
}