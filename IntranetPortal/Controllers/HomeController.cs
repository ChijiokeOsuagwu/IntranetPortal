using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using IntranetPortal.Models;
using Microsoft.Extensions.Configuration;
using IntranetPortal.Base.Services;
using IntranetPortal.Base.Models.ContentManagerModels;

namespace IntranetPortal.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IConfiguration _configuration;
        private readonly IContentManagerService _contentManager;
        public HomeController(IConfiguration configuration, IContentManagerService contentManager)
        {
            _configuration = configuration;
            _contentManager = contentManager;
        }

        public async Task<IActionResult> Index()
        {
            GeneralIndexViewModel model = new GeneralIndexViewModel();
            var bs = await _contentManager.GetUnhiddenBannersAsync();
            var ts = await _contentManager.GetUnhiddenArticlesAsync();

            model.Banners = bs.ToList();
            model.Articles = ts.ToList();
            return View(model);
        }

        public IActionResult Apps()
        {
            return View();
        }

        public async Task<IActionResult> Read( int? id)
        {
            Post post = new Post();
            if(id != null) { post = await _contentManager.GetPostByIdAsync(id.Value);}
            return View(post);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Blank()
        {
            return View();
        }

        public IActionResult Test()
        {
            return View();
        }

        public IActionResult BlueTest()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
