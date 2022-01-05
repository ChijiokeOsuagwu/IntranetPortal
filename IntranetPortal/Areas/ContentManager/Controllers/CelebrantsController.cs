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
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace IntranetPortal.Areas.ContentManager.Controllers
{
    [Area("ContentManager")]
    public class CelebrantsController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;
        private readonly IContentManagerService _contentManager;
        private readonly IDataProtector _dataProtector;
        public CelebrantsController(IWebHostEnvironment webHostingEnvironment, IConfiguration configuration,
                                    IContentManagerService contentManager, IDataProtectionProvider dataProtectionProvider,
                                    DataProtectionEncryptionStrings dataProtectionEncryptionStrings)
        {
            _webHostEnvironment = webHostingEnvironment;
            _configuration = configuration;
            _contentManager = contentManager;
            _dataProtector = dataProtectionProvider.CreateProtector(dataProtectionEncryptionStrings.RouteValuesEncryptionCode);
        }

        public IActionResult Index()
        {
            CelebrantsListViewModel model = new CelebrantsListViewModel();
            model.Celebrants = new List<Post>();
            return View(model);
        }

        public IActionResult Add()
        {
            CelebrantsAddViewModel model = new CelebrantsAddViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(CelebrantsAddViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.CelebrantImage != null)
                {
                    var supportedTypes = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    FileInfo fileInfo = new FileInfo(model.CelebrantImage.FileName);
                    var fileExt = fileInfo.Extension;
                    if (!supportedTypes.Contains(fileExt))
                    {
                        model.ViewModelErrorMessage = "Sorry, invalid image format. Only images of type jpg, jpeg, png, gif are permitted.";
                        return View(model);
                    }
                    string uploadsFolder = "uploads/cms/" + Guid.NewGuid().ToString() + "_" + model.CelebrantImage.FileName;
                    string absoluteFilePath = Path.Combine(_webHostEnvironment.WebRootPath, uploadsFolder);
                    await model.CelebrantImage.CopyToAsync(new FileStream(absoluteFilePath, FileMode.Create));

                    Post post = new Post
                    {
                        PostTitle = model.Title,
                        ImagePath = uploadsFolder,
                        EnableComment = model.EnableComments,
                        IsHidden = model.IsHidden,
                        PostSummary = model.Summary,
                        PostTypeId = (int)PostType.Celebrant,
                        ModifiedDate = DateTime.UtcNow,
                        CreatedDate = DateTime.UtcNow,
                        ModifiedBy = HttpContext.User.Identity.Name ?? "System Administrator",
                        CreatedBy = HttpContext.User.Identity.Name ?? "System Administrator"
                    };

                    if (await _contentManager.CreatePostAsync(post))
                    {
                        model.ViewModelSuccessMessage = $"Congratulations! New Celebrant was added successfully.";
                    }
                    else
                    {
                        FileInfo file = new FileInfo(absoluteFilePath);
                        if (file.Exists)
                        {
                            file.Delete();
                        }
                        model.ViewModelErrorMessage = $"Error! An error was encountered. New Celebrant could not be added.";
                    }
                }
            }
            return View(model);
        }


        public async Task<IActionResult> Delete(string id)
        {
            int PostId = Convert.ToInt32(_dataProtector.Unprotect(id));
            CelebrantsDeleteViewModel model = new CelebrantsDeleteViewModel();
            var article = await _contentManager.GetPostByIdAsync(PostId);
            model.Id = article.PostId;
            model.ImagePath = article.ImagePath;
            model.Summary = article.PostSummary;
            model.Title = article.PostTitle;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(CelebrantsDeleteViewModel model)
        {
            if (model == null || model.Id < 1)
            {
                model.ViewModelErrorMessage = $"Error! Sorry, an error was encountered. Celebrant could not be deleted.";
                return View(model);
            }
            int PostId = model.Id;
            string filePath = string.Empty;
            if (!string.IsNullOrEmpty(model.ImagePath))
            {
                filePath = Path.Combine(_webHostEnvironment.WebRootPath, model.ImagePath);
            }
            FileInfo file = new FileInfo(filePath);
            if (file.Exists)
            {
                file.Delete();
            }
            var result = await _contentManager.DeletePostAsync(PostId);
            if (!result)
            {
                model.ViewModelErrorMessage = $"Error! Sorry, an error was encountered. Celebrant could not be deleted.";
                return View(model);
            }
            TempData["DeleteSuccessMessage"] = "The Celebrant was successfully deleted!";
            return RedirectToAction("Index", "Celebrants");
        }

        public async Task<IActionResult> Edit(string id)
        {
            int PostId = 0;
            if (!string.IsNullOrEmpty(id)) { PostId = Convert.ToInt32(_dataProtector.Unprotect(id)); }
            CelebrantEditViewModel model = new CelebrantEditViewModel();
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
        public async Task<IActionResult> Edit(CelebrantEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                Post post = new Post();
                string absoluteFilePath = string.Empty;
                string uploadsFolder = string.Empty;

                if (model.CelebrantImage != null)
                {
                    var supportedTypes = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    FileInfo fileInfo = new FileInfo(model.CelebrantImage.FileName);
                    var fileExt = fileInfo.Extension;
                    if (!supportedTypes.Contains(fileExt))
                    {
                        model.ViewModelErrorMessage = "Sorry, invalid image format. Only images of type jpg, jpeg, png, gif are permitted.";
                        return View(model);
                    }
                    uploadsFolder = "uploads/cms/" + Guid.NewGuid().ToString() + "_" + model.CelebrantImage.FileName;
                    absoluteFilePath = Path.Combine(_webHostEnvironment.WebRootPath, uploadsFolder);
                    await model.CelebrantImage.CopyToAsync(new FileStream(absoluteFilePath, FileMode.Create));
                }

                post.IsHidden = model.IsHidden;
                post.PostSummary = model.Summary;
                post.PostTypeId = (int)PostType.Celebrant;
                post.ModifiedDate = DateTime.UtcNow;
                post.ModifiedBy = HttpContext.User.Identity.Name ?? "System Administrator";
                post.PostTitle = model.Title;
                post.ImagePath = uploadsFolder;
                post.EnableComment = model.EnableComments;

                if (await _contentManager.UpdatePostAsync(post))
                {
                    model.ViewModelSuccessMessage = $"Congratulations! Celebrant was updated successfully.";
                }
                else
                {
                    FileInfo file = new FileInfo(absoluteFilePath);
                    if (file.Exists)
                    {
                        file.Delete();
                    }
                    model.ViewModelErrorMessage = $"Error! An error was encountered. Celebrant could not be added.";
                }
            }
            return View(model);
        }

    }
}