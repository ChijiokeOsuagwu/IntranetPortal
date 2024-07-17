using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IntranetPortal.Areas.CLM.Models;
using IntranetPortal.Base.Models.ClmModels;
using IntranetPortal.Base.Services;
using IntranetPortal.Configurations;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;

namespace IntranetPortal.Areas.CLM.Controllers
{
    [Area("CLM")]
    public class LibraryController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IGlobalSettingsService _globalSettingsService;
        private readonly IClmService _clmService;
        private readonly IBaseModelService _baseModelService;
        private readonly IDataProtector _dataProtector;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public LibraryController(IConfiguration configuration,
                                    IGlobalSettingsService globalSettingsService, IDataProtectionProvider dataProtectionProvider,
                                    DataProtectionEncryptionStrings dataProtectionEncryptionStrings, IClmService clmService,
                                    IBaseModelService baseModelService, IWebHostEnvironment webHostEnvironment)
        {
            _configuration = configuration;
            _globalSettingsService = globalSettingsService;
            _clmService = clmService;
            _baseModelService = baseModelService;
            _dataProtector = dataProtectionProvider.CreateProtector(dataProtectionEncryptionStrings.RouteValuesEncryptionCode);
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Search(int? tp = null, int? sa = null, int? lv = null, string nm = null)
        {
            CoursesListViewModel model = new CoursesListViewModel();
            model.lv = lv ?? 0;
            model.tp = tp ?? 0;
            model.sa = sa ?? 0;
            model.nm = nm;

            var entities = await _clmService.FindCoursesAsync(model.tp, model.sa, model.lv, model.nm);
            if (entities != null) { model.CoursesList = entities; }

            List<CourseType> course_types = await _clmService.GetCourseTypesAsync();
            if (course_types != null && course_types.Count > 0)
            {
                ViewBag.CourseTypesList = new SelectList(course_types, "CourseTypeId", "CourseTypeDescription", tp);
            }

            List<SubjectArea> subject_areas = await _clmService.GetSubjectAreasAsync();
            if (subject_areas != null && subject_areas.Count > 0)
            {
                ViewBag.SubjectAreasList = new SelectList(subject_areas, "SubjectAreaId", "SubjectAreaDescription", sa);
            }

            return View(model);
        }

        public async Task<IActionResult> CourseContents(int id)
        {
            CourseContentListViewModel model = new CourseContentListViewModel();
            model.id = id;
            model.fm = null;

            var entities = await _clmService.FindCourseContentsAsync(model.id, model.fm);
            if (entities != null) { model.CourseContentsList = entities; }

            return View(model);
        }

        public async Task<IActionResult> ReadTextContent(long id)
        {
            CourseContent courseContent = new CourseContent();
            EditTextContentViewModel model = new EditTextContentViewModel();
            var c_entity = await _clmService.GetCourseContentAsync(id);
            if(c_entity != null)
            {
                model.ContentHeading = c_entity.ContentHeading;
                model.ContentTitle = c_entity.ContentTitle;
                model.CourseContentID = c_entity.CourseContentId;
                model.CourseID = c_entity.CourseId;
                model.RawTextContent = c_entity.ContentBody;
            }
            return View(model);
        }

        public async Task<IActionResult> WatchVideoContent(long id)
        {
            CourseContent courseContent = new CourseContent();
            VideoContentViewModel model = new VideoContentViewModel();
            var c_entity = await _clmService.GetCourseContentAsync(id);
            if (c_entity != null)
            {
                model.ContentHeading = c_entity.ContentHeading;
                model.ContentTitle = c_entity.ContentTitle;
                model.CourseContentID = c_entity.CourseContentId;
                model.CourseID = c_entity.CourseId;
                model.ContentUrl = c_entity.ContentLink;
            }
            return View(model);
        }

        public async Task<IActionResult> LoadDocumentContent(long id)
        {
            CourseContent courseContent = new CourseContent();
            VideoContentViewModel model = new VideoContentViewModel();
            var c_entity = await _clmService.GetCourseContentAsync(id);
            if (c_entity != null)
            {
                model.ContentHeading = c_entity.ContentHeading;
                model.ContentTitle = c_entity.ContentTitle;
                model.CourseContentID = c_entity.CourseContentId;
                model.CourseID = c_entity.CourseId;
                model.ContentUrl = c_entity.ContentLink;
            }
            return View(model);
        }
    }
}