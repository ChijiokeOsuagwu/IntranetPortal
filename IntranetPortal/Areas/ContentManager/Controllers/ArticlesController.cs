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
    public class ArticlesController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;
        private readonly IContentManagerService _contentManager;
        private readonly IDataProtector _dataProtector;
        public ArticlesController(IWebHostEnvironment webHostingEnvironment, IConfiguration configuration,
                                    IContentManagerService contentManager, IDataProtectionProvider dataProtectionProvider,
                                    DataProtectionEncryptionStrings dataProtectionEncryptionStrings)
        {
            _webHostEnvironment = webHostingEnvironment;
            _configuration = configuration;
            _contentManager = contentManager;
            _dataProtector = dataProtectionProvider.CreateProtector(dataProtectionEncryptionStrings.RouteValuesEncryptionCode);
        }

        [Authorize(Roles = "PCMMGACNT, XYALLACCZ")]
        public async Task<IActionResult> Index()
        {
            ArticlesListViewModel model = new ArticlesListViewModel();
            var records = await _contentManager.GetAllArticlesAsync();
            model.Articles = records.ToList();
            foreach (var article in model.Articles)
            {
                article.CodedPostId = _dataProtector.Protect(article.PostId.ToString()).ToString();
            }
            return View(model);
        }

        [Authorize(Roles = "PCMMGACNT, XYALLACCZ")]
        public IActionResult Add()
        {
            ArticlesAddViewModel model = new ArticlesAddViewModel();
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "PCMMGACNT, XYALLACCZ")]
        public async Task<IActionResult> Add(ArticlesAddViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uploadsFolder = null;
                string absoluteFilePath = null;

                if (model.ArticleImage != null)
                {
                    var supportedTypes = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    FileInfo fileInfo = new FileInfo(model.ArticleImage.FileName);
                    var fileExt = fileInfo.Extension;
                    if (!supportedTypes.Contains(fileExt))
                    {
                        model.ViewModelErrorMessage = "Sorry, invalid image format. Only images of type jpg, jpeg, png, gif are permitted.";
                        return View(model);
                    }
                    uploadsFolder = "uploads/cms/" + Guid.NewGuid().ToString() + "_" + model.ArticleImage.FileName;
                    absoluteFilePath = Path.Combine(_webHostEnvironment.WebRootPath, uploadsFolder);
                    //await model.ArticleImage.CopyToAsync(new FileStream(absoluteFilePath, FileMode.Create));
                    using (var fileStream = new FileStream(absoluteFilePath, FileMode.Create))
                    {
                        await model.ArticleImage.CopyToAsync(fileStream);
                    }
                }
                Post post = new Post
                {
                    PostTitle = model.Title,
                    ImagePath = "/" + uploadsFolder,
                    ImageFullPath = absoluteFilePath,
                    EnableComment = model.EnableComments,
                    IsHidden = model.IsHidden,
                    PostSummary = model.Summary,
                    PostTypeId = (int)PostType.Article,
                    ModifiedDate = DateTime.UtcNow,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedBy = HttpContext.User.Identity.Name ?? string.Empty,
                    CreatedBy = HttpContext.User.Identity.Name ?? string.Empty
                };

                if (await _contentManager.CreatePostAsync(post))
                {
                    model.ViewModelSuccessMessage = $"Congratulations! New Article was added successfully.";
                }
                else
                {
                    FileInfo file = new FileInfo(absoluteFilePath);
                    if (file.Exists)
                    {
                        if (!file.IsFileOpen())
                        {
                            await Task.Run(() =>
                            {
                                file.Delete();
                            });
                        }
                    }
                    model.ViewModelErrorMessage = "Sorry, an error was encountered. New article could not be added.";
                }
            }
            return View(model);
        }

        [Authorize(Roles = "PCMMGACNT, XYALLACCZ")]
        public async Task<IActionResult> Delete(string id)
        {
            int PostId = Convert.ToInt32(_dataProtector.Unprotect(id));
            ArticlesDeleteViewModel model = new ArticlesDeleteViewModel();
            var article = await _contentManager.GetPostByIdAsync(PostId);
            model.Id = article.PostId;
            model.ImagePath = article.ImagePath;
            model.Summary = article.PostSummary;
            model.Title = article.PostTitle;
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "PCMMGACNT, XYALLACCZ")]
        public async Task<IActionResult> Delete(ArticlesDeleteViewModel model)
        {
            if (model == null || model.Id < 1)
            {
                model.ViewModelErrorMessage = "Sorry, an error was encountered. Article could not be deleted.";
                return View(model);
            }
            long PostId = model.Id;
            string filePath = string.Empty;
            if (!string.IsNullOrEmpty(model.ImagePath))
            {
                filePath = Path.Combine(_webHostEnvironment.WebRootPath, model.ImagePath);
            }
            FileInfo file = new FileInfo(filePath);
            if (file.Exists)
            {
                if (!file.IsFileOpen())
                {
                    await Task.Run(() =>
                    {
                        file.Delete();
                    });
                }
            }
            var result = await _contentManager.DeletePostAsync(PostId);
            if (!result)
            {
                model.ViewModelErrorMessage = "Sorry, an error was encountered. Article could not be deleted.";
                return View(model);
            }
            TempData["DeleteSuccessMessage"] = "The Article was successfully deleted!";
            return RedirectToAction("Index", "Articles");
        }

        [Authorize(Roles = "PCMMGACNT, XYALLACCZ")]
        public async Task<IActionResult> Edit(string id)
        {
            long PostId = 0;
            if (!string.IsNullOrEmpty(id)) { PostId = Convert.ToInt32(_dataProtector.Unprotect(id)); }
            ArticleEditViewModel model = new ArticleEditViewModel();
            var article = await _contentManager.GetPostByIdAsync(PostId);
            model.Id = article.PostId;
            model.ImagePath = article.ImagePath;
            model.Summary = article.PostSummary;
            model.Title = article.PostTitle;
            model.EnableComments = article.EnableComment;
            model.IsHidden = article.IsHidden;
            model.Id = article.PostId;
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "PCMMGACNT, XYALLACCZ")]
        public async Task<IActionResult> Edit(ArticleEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                Post post = new Post();
                string absoluteFilePath = string.Empty;
                string uploadsFolder = string.Empty;
                string previousUploadsFolder = null;
                string previousAbsoluteFilePath = null;
                bool newFileUploadedSuccessfully = false;

                var entity = await _contentManager.GetPostByIdAsync(model.Id);
                if (entity != null)
                {
                    previousAbsoluteFilePath = entity.ImageFullPath;
                    previousUploadsFolder = entity.ImagePath;
                }

                if (model.ArticleImage != null &&  model.ArticleImage.Length > 0)
                {
                    var supportedTypes = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    FileInfo fileInfo = new FileInfo(model.ArticleImage.FileName);
                    var fileExt = fileInfo.Extension;
                    if (!supportedTypes.Contains(fileExt))
                    {
                        model.ViewModelErrorMessage = "Sorry, invalid image format. Only images of type jpg, jpeg, png, gif are permitted.";
                        return View(model);
                    }
                    uploadsFolder = "uploads/cms/" + Guid.NewGuid().ToString() + "_" + model.ArticleImage.FileName;
                    absoluteFilePath = Path.Combine(_webHostEnvironment.WebRootPath, uploadsFolder);
                    //await model.ArticleImage.CopyToAsync(new FileStream(absoluteFilePath, FileMode.Create));
                    using (var fileStream = new FileStream(absoluteFilePath, FileMode.Create))
                    {
                        await model.ArticleImage.CopyToAsync(fileStream);
                        newFileUploadedSuccessfully = true;
                    }
                }
                else
                {
                    uploadsFolder = previousUploadsFolder;
                    absoluteFilePath = previousAbsoluteFilePath;
                }

                post.IsHidden = model.IsHidden;
                post.PostSummary = model.Summary;
                post.PostTypeId = (int)PostType.Banner;
                post.ModifiedDate = DateTime.UtcNow;
                post.ModifiedBy = HttpContext.User.Identity.Name ?? "System Administrator";
                post.PostTitle = model.Title;
                post.ImagePath = "/"+uploadsFolder;
                post.ImageFullPath = absoluteFilePath;
                post.EnableComment = model.EnableComments;

                if (await _contentManager.UpdatePostAsync(post))
                {
                    if (newFileUploadedSuccessfully)
                    {
                        FileInfo file = new FileInfo(previousAbsoluteFilePath);
                        if (file.Exists)
                        {
                            if (!file.IsFileOpen())
                            {
                                await Task.Run(() =>
                                {
                                    file.Delete();
                                });
                            }
                        }
                    }
                    model.ViewModelSuccessMessage = "Congratulations! Article was updated successfully.";
                }
                else
                {
                    FileInfo file = new FileInfo(absoluteFilePath);
                    if (!file.IsFileOpen())
                    {
                        await Task.Run(() =>
                        {
                            file.Delete();
                        });
                    }
                    model.ViewModelErrorMessage = "Sorry, an error was encountered. Article could not be updated.";
                }
            }
            return View(model);
        }

    }
}