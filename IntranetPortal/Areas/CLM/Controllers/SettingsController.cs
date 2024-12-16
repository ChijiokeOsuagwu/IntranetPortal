using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IntranetPortal.Areas.CLM.Models;
using IntranetPortal.Base.Models.BaseModels;
using IntranetPortal.Base.Models.ClmModels;
using IntranetPortal.Base.Services;
using IntranetPortal.Configurations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System.IO;
using IntranetPortal.Helpers;

namespace IntranetPortal.Areas.CLM.Controllers
{
    [Area("CLM")]
    public class SettingsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IGlobalSettingsService _globalSettingsService;
        private readonly IClmService _clmService;
        private readonly IBaseModelService _baseModelService;
        private readonly IDataProtector _dataProtector;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public SettingsController(IConfiguration configuration,
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

        public IActionResult Index()
        {
            return View();
        }

        #region Course Type Controller Methods

        [Authorize(Roles = "CLMSTTMGA, XYALLACCZ")]
        public async Task<IActionResult> CourseTypes()
        {
            CourseTypesListViewModel model = new CourseTypesListViewModel();
            var entities = await _clmService.GetCourseTypesAsync();
            if (entities != null) { model.CourseTypesList = entities; }
            return View(model);
        }

        [Authorize(Roles = "CLMSTTMGA, XYALLACCZ")]
        public async Task<IActionResult> ManageCourseType(int id)
        {
            CourseTypeViewModel model = new CourseTypeViewModel();
            if (id > 0)
            {
                CourseType courseType = await _clmService.GetCourseTypeAsync(id);
                if (courseType != null && !string.IsNullOrWhiteSpace(courseType.CourseTypeDescription))
                {
                    model.CourseTypeId = courseType.CourseTypeId;
                    model.CourseTypeDescription = courseType.CourseTypeDescription;
                }
            }
            return View(model);
        }

        [Authorize(Roles = "CLMSTTMGA, XYALLACCZ")]
        [HttpPost]
        public async Task<IActionResult> ManageCourseType(CourseTypeViewModel model)
        {
            try
            {
                CourseType courseType = new CourseType();
                if (ModelState.IsValid)
                {
                    courseType.CourseTypeId = model.CourseTypeId;
                    courseType.CourseTypeDescription = model.CourseTypeDescription;

                    if (courseType.CourseTypeId < 1)
                    {
                        bool IsAdded = await _clmService.AddCourseTypeAsync(courseType);
                        if (IsAdded)
                        {
                            return RedirectToAction("CourseTypes");
                        }
                        else
                        {
                            model.ViewModelSuccessMessage = "Sorry, an error was encountered. Please try again.";
                        }
                    }
                    else
                    {
                        bool IsUpdated = await _clmService.EditCourseTypeAsync(courseType);
                        if (IsUpdated)
                        {
                            model.OperationIsSuccessful = true;
                            model.ViewModelSuccessMessage = "Course Type was updated successfully!";
                        }
                        else
                        {
                            model.ViewModelSuccessMessage = "Sorry, an error was encountered. Please try again.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        [Authorize(Roles = "CLMSTTMGA, XYALLACCZ")]
        public async Task<IActionResult> DeleteCourseType(int id)
        {
            CourseTypeViewModel model = new CourseTypeViewModel();
            if (id > 0)
            {
                CourseType courseType = await _clmService.GetCourseTypeAsync(id);
                if (courseType != null && !string.IsNullOrWhiteSpace(courseType.CourseTypeDescription))
                {
                    model.CourseTypeId = courseType.CourseTypeId;
                    model.CourseTypeDescription = courseType.CourseTypeDescription;
                }
            }
            return View(model);
        }

        [Authorize(Roles = "CLMSTTMGA, XYALLACCZ")]
        [HttpPost]
        public async Task<IActionResult> DeleteCourseType(CourseTypeViewModel model)
        {
            try
            {
                if (model.CourseTypeId > 0)
                {
                    bool IsDeleted = await _clmService.DeleteCourseTypeAsync(model.CourseTypeId);
                    if (IsDeleted)
                    {
                        return RedirectToAction("CourseTypes");
                    }
                    else
                    {
                        model.ViewModelErrorMessage = "Sorry, an error was encountered. Delete operation failed.";
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        #endregion

        #region Subject Area Controller Methods

        [Authorize(Roles = "CLMSTTMGA, XYALLACCZ")]
        public async Task<IActionResult> SubjectAreas()
        {
            SubjectAreasListViewModel model = new SubjectAreasListViewModel();
            var entities = await _clmService.GetSubjectAreasAsync();
            if (entities != null) { model.SubjectAreaList = entities; }
            return View(model);
        }

        [Authorize(Roles = "CLMSTTMGA, XYALLACCZ")]
        public async Task<IActionResult> ManageSubjectArea(int id)
        {
            SubjectAreaViewModel model = new SubjectAreaViewModel();
            if (id > 0)
            {
                SubjectArea subjectArea = await _clmService.GetSubjectAreaAsync(id);
                if (subjectArea != null && !string.IsNullOrWhiteSpace(subjectArea.SubjectAreaDescription))
                {
                    model.SubjectAreaId = subjectArea.SubjectAreaId;
                    model.SubjectAreaDescription = subjectArea.SubjectAreaDescription;
                }
            }
            return View(model);
        }

        [Authorize(Roles = "CLMSTTMGA, XYALLACCZ")]
        [HttpPost]
        public async Task<IActionResult> ManageSubjectArea(SubjectAreaViewModel model)
        {
            try
            {
                SubjectArea subjectArea = new SubjectArea();
                if (ModelState.IsValid)
                {
                    subjectArea.SubjectAreaId = model.SubjectAreaId;
                    subjectArea.SubjectAreaDescription = model.SubjectAreaDescription;

                    if (subjectArea.SubjectAreaId < 1)
                    {
                        bool IsAdded = await _clmService.AddSubjectAreaAsync(subjectArea);
                        if (IsAdded)
                        {
                            return RedirectToAction("SubjectAreas");
                        }
                        else
                        {
                            model.ViewModelSuccessMessage = "Sorry, an error was encountered. Please try again.";
                        }
                    }
                    else
                    {
                        bool IsUpdated = await _clmService.EditSubjectAreaAsync(subjectArea);
                        if (IsUpdated)
                        {
                            model.OperationIsSuccessful = true;
                            model.ViewModelSuccessMessage = "Subject Area was updated successfully!";
                        }
                        else
                        {
                            model.ViewModelSuccessMessage = "Sorry, an error was encountered. Please try again.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        [Authorize(Roles = "CLMSTTMGA, XYALLACCZ")]
        public async Task<IActionResult> DeleteSubjectArea(int id)
        {
            SubjectAreaViewModel model = new SubjectAreaViewModel();
            if (id > 0)
            {
                SubjectArea subjectArea = await _clmService.GetSubjectAreaAsync(id);
                if (subjectArea != null && !string.IsNullOrWhiteSpace(subjectArea.SubjectAreaDescription))
                {
                    model.SubjectAreaId = subjectArea.SubjectAreaId;
                    model.SubjectAreaDescription = subjectArea.SubjectAreaDescription;
                }
            }
            return View(model);
        }

        [Authorize(Roles = "CLMSTTMGA, XYALLACCZ")]
        [HttpPost]
        public async Task<IActionResult> DeleteSubjectArea(SubjectAreaViewModel model)
        {
            try
            {
                if (model.SubjectAreaId > 0)
                {
                    bool IsDeleted = await _clmService.DeleteSubjectAreaAsync(model.SubjectAreaId);
                    if (IsDeleted)
                    {
                        return RedirectToAction("SubjectAreas");
                    }
                    else
                    {
                        model.ViewModelErrorMessage = "Sorry, an error was encountered. Delete operation failed.";
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        #endregion

        #region Courses Controller Methods

        [Authorize(Roles = "CLMSTTMGA, XYALLACCZ")]
        public async Task<IActionResult> Courses(int? tp = null, int? sa = null, int? lv = null, string nm = null)
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
                ViewBag.CourseTypesList = new SelectList(course_types, "CourseTypeId", "CourseTypeDescription", model.tp);
            }

            List<SubjectArea> subject_areas = await _clmService.GetSubjectAreasAsync();
            if (subject_areas != null && subject_areas.Count > 0)
            {
                ViewBag.SubjectAreasList = new SelectList(subject_areas, "SubjectAreaId", "SubjectAreaDescription", model.sa);
            }

            return View(model);
        }

        [Authorize(Roles = "CLMSTTMGA, XYALLACCZ")]
        public async Task<IActionResult> ManageCourse(int id)
        {
            CourseViewModel model = new CourseViewModel();
            if (id > 0)
            {
                Course course = await _clmService.GetCourseAsync(id);
                if (course != null && !string.IsNullOrWhiteSpace(course.CourseTitle))
                {
                    model = model.ExtractFromCourse(course);
                }
            }

            List<CourseType> course_types = await _clmService.GetCourseTypesAsync();
            if (course_types != null && course_types.Count > 0)
            {
                ViewBag.CourseTypesList = new SelectList(course_types, "CourseTypeId", "CourseTypeDescription", model.CourseTypeId);
            }

            List<SubjectArea> subject_areas = await _clmService.GetSubjectAreasAsync();
            if (subject_areas != null && subject_areas.Count > 0)
            {
                ViewBag.SubjectAreasList = new SelectList(subject_areas, "SubjectAreaId", "SubjectAreaDescription", model.SubjectAreaId);
            }

            return View(model);
        }

        [Authorize(Roles = "CLMSTTMGA, XYALLACCZ")]
        [HttpPost]
        public async Task<IActionResult> ManageCourse(CourseViewModel model)
        {
            try
            {
                Course course = new Course();
                if (ModelState.IsValid)
                {
                    course = model.ConvertToCourse();

                    if (course.CourseId > 0)
                    {
                        bool IsUpdated = await _clmService.UpdateCourseAsync(course);
                        if (IsUpdated)
                        {
                            model.OperationIsSuccessful = true;
                            model.ViewModelSuccessMessage = "Course was updated successfully!";
                        }
                        else
                        {
                            model.ViewModelSuccessMessage = "Sorry, an error was encountered. Please try again.";
                        }
                    }
                    else
                    {
                        course.UploadedTime = DateTime.UtcNow;
                        course.UploadedBy = HttpContext.User.Identity.Name;
                        bool IsAdded = await _clmService.CreateCourseAsync(course);
                        if (IsAdded)
                        {
                            return RedirectToAction("Courses");
                        }
                        else
                        {
                            model.ViewModelSuccessMessage = "Sorry, an error was encountered. Please try again.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        [Authorize(Roles = "CLMSTTMGA, XYALLACCZ")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            CourseViewModel model = new CourseViewModel();
            if (id > 0)
            {
                Course course = await _clmService.GetCourseAsync(id);
                if (course != null && !string.IsNullOrWhiteSpace(course.CourseTitle))
                {
                    model = model.ExtractFromCourse(course);
                }
            }
            return View(model);
        }

        [Authorize(Roles = "CLMSTTMGA, XYALLACCZ")]
        [HttpPost]
        public async Task<IActionResult> DeleteCourse(CourseViewModel model)
        {
            try
            {
                if (model.CourseId > 0)
                {
                    bool IsDeleted = await _clmService.DeleteCourseAsync(model.CourseId);
                    if (IsDeleted)
                    {
                        return RedirectToAction("Courses");
                    }
                    else
                    {
                        model.ViewModelErrorMessage = "Sorry, an error was encountered. Delete operation failed.";
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        public async Task<IActionResult> CourseInfo(int id)
        {
            CourseViewModel model = new CourseViewModel();
            if (id > 0)
            {
                Course course = await _clmService.GetCourseAsync(id);
                if (course != null && !string.IsNullOrWhiteSpace(course.CourseTitle))
                {
                    model = model.ExtractFromCourse(course);
                }
            }
            return View(model);
        }

        #endregion

        #region Course Contents Controller Methods

        [Authorize(Roles = "CLMSTTMGA, XYALLACCZ")]
        public async Task<IActionResult> CourseContents(int id, int? fm = null)
        {
            CourseContentListViewModel model = new CourseContentListViewModel();
            model.id = id;
            model.fm = fm;

            var entities = await _clmService.FindCourseContentsAsync(model.id, model.fm);
            if (entities != null) { model.CourseContentsList = entities; }

            return View(model);
        }

        [Authorize(Roles = "CLMSTTMGA, XYALLACCZ")]
        public async Task<IActionResult> ManageCourseContent(int id, long? cd = 0)
        {
            CourseContentViewModel model = new CourseContentViewModel();
            model.CourseId = id;
            if (cd > 0)
            {
                CourseContent courseContent = await _clmService.GetCourseContentAsync(cd.Value);
                if (courseContent != null && !string.IsNullOrWhiteSpace(courseContent.CourseTitle))
                {
                    model = model.ExtractFromCourseContent(courseContent);
                }
            }
            return View(model);
        }

        [Authorize(Roles = "CLMSTTMGA, XYALLACCZ")]
        [HttpPost]
        public async Task<IActionResult> ManageCourseContent(CourseContentViewModel model)
        {
            try
            {
                CourseContent courseContent = new CourseContent();
                if (ModelState.IsValid)
                {
                    courseContent = model.ConvertToCourseContent();

                    if (courseContent.CourseContentId > 0)
                    {
                        bool IsUpdated = await _clmService.UpdateCourseContentAsync(courseContent);
                        if (IsUpdated)
                        {
                            model.OperationIsSuccessful = true;
                            model.ViewModelSuccessMessage = "Course Content was updated successfully!";
                        }
                        else
                        {
                            model.ViewModelSuccessMessage = "Sorry, an error was encountered. Please try again.";
                        }
                    }
                    else
                    {
                        courseContent.ContentUploadTime = DateTime.UtcNow;
                        courseContent.ContentUploadedBy = HttpContext.User.Identity.Name;
                        bool IsAdded = await _clmService.CreateCourseContentAsync(courseContent);
                        if (IsAdded)
                        {
                            model.OperationIsSuccessful = true;
                            model.ViewModelSuccessMessage = "Course Content was added successfully!";
                        }
                        else
                        {
                            model.ViewModelSuccessMessage = "Sorry, an error was encountered. Please try again.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        [Authorize(Roles = "CLMSTTMGA, XYALLACCZ")]
        public async Task<IActionResult> DeleteCourseContent(long id)
        {
            CourseContentViewModel model = new CourseContentViewModel();
            if (id > 0)
            {
                CourseContent courseContent = await _clmService.GetCourseContentAsync(id);
                if (courseContent != null && !string.IsNullOrWhiteSpace(courseContent.ContentTitle))
                {
                    model = model.ExtractFromCourseContent(courseContent);
                }
            }
            return View(model);
        }

        [Authorize(Roles = "CLMSTTMGA, XYALLACCZ")]
        [HttpPost]
        public async Task<IActionResult> DeleteCourseContent(CourseContentViewModel model)
        {
            try
            {
                string contentFilePath = null;
                if (model.CourseContentId > 0)
                {
                    CourseContent courseContent = await _clmService.GetCourseContentAsync(model.CourseContentId);
                    if (courseContent != null && !string.IsNullOrWhiteSpace(courseContent.ContentFullPath))
                    {
                        contentFilePath = courseContent.ContentFullPath;
                        bool IsDeleted = await _clmService.DeleteCourseContentAsync(model.CourseContentId);
                        if (IsDeleted)
                        {
                            FileInfo file = new FileInfo(contentFilePath);
                            if (file.Exists)
                            {
                                if (!file.IsFileOpen())
                                {
                                    await Task.Run(() => {
                                        file.Delete();
                                    });
                                }
                            }
                            return RedirectToAction("CourseContents", new { id = model.CourseId, fm = 0 });
                        }
                        else
                        {
                            model.ViewModelErrorMessage = "Sorry, an error was encountered. Delete operation failed.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        public async Task<IActionResult> CourseContentInfo(long id)
        {
            CourseContentViewModel model = new CourseContentViewModel();
            if (id > 0)
            {
                CourseContent courseContent = await _clmService.GetCourseContentAsync(id);
                if (courseContent != null && !string.IsNullOrWhiteSpace(courseContent.ContentTitle))
                {
                    model = model.ExtractFromCourseContent(courseContent);
                }
            }
            return View(model);
        }

        [Authorize(Roles = "CLMSTTMGA, XYALLACCZ")]
        public async Task<IActionResult> EditTextContent(long id)
        {
            CourseContent courseContent = new CourseContent();
            EditTextContentViewModel model = new EditTextContentViewModel();
            var c_entity = await _clmService.GetCourseContentAsync(id);
            if (c_entity != null)
            {
                model.ContentHeading = c_entity.ContentHeading;
                model.ContentTitle = c_entity.ContentTitle;
                model.CourseContentID = c_entity.CourseContentId;
                model.CourseID = c_entity.CourseId;
                model.RawTextContent = c_entity.ContentBody;
            }
            return View(model);
        }

        [Authorize(Roles = "CLMSTTMGA, XYALLACCZ")]
        [HttpPost]
        public async Task<IActionResult> EditTextContent(EditTextContentViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool IsUpdated = await _clmService.UpdateContentTextAsync(model.CourseContentID, model.RawTextContent);
                    if (IsUpdated)
                    {
                        model.OperationIsSuccessful = true;
                        model.ViewModelSuccessMessage = "Content was updated successfully!";
                    }
                    else
                    {
                        model.ViewModelSuccessMessage = "Sorry, an error was encountered. Please try again.";
                    }
                }
                else
                {
                    model.ViewModelSuccessMessage = "Sorry, some important form fields are missing. Please try again.";
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        public async Task<IActionResult> UploadCourseContent(long id)
        {
            CourseContent courseContent = new CourseContent();
            UploadContentViewModel model = new UploadContentViewModel();
            var c_entity = await _clmService.GetCourseContentAsync(id);
            if (c_entity != null)
            {
                model.ContentHeading = c_entity.ContentHeading;
                model.ContentTitle = c_entity.ContentTitle;
                model.CourseContentID = c_entity.CourseContentId;
                model.CourseID = c_entity.CourseId;
                model.OldContentUrl = c_entity.ContentLink;
                model.FormatID = c_entity.ContentFormatId;
                model.Format = (ContentFormat)c_entity.ContentFormatId;
            }
            return View(model);
        }

        [Authorize(Roles = "CLMSTTMGA, XYALLACCZ")]
        [HttpPost]
        [RequestSizeLimit(268435456)]
        [RequestFormLimits(MultipartBodyLengthLimit = 268435456)]
        public async Task<IActionResult> UploadCourseContent(UploadContentViewModel model)
        {
            string previousFilePath = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    if (!string.IsNullOrWhiteSpace(model.OldContentUrl))
                    {
                        var courseContent = await _clmService.GetCourseContentAsync(model.CourseContentID);
                        if (courseContent != null && !string.IsNullOrWhiteSpace(courseContent.ContentFullPath))
                        {
                            previousFilePath = courseContent.ContentFullPath;
                        }
                    }

                    if (model.ContentFile != null && model.ContentFile.Length > 0)
                    {
                        string uploadDirectory = string.Empty;
                        ContentFormat contentFormat = (ContentFormat)model.FormatID;
                        switch (contentFormat)
                        {
                            case ContentFormat.Text:
                                break;
                            case ContentFormat.Image:
                                uploadDirectory = @"uploads/clm/imgs";
                                break;
                            case ContentFormat.Video:
                                uploadDirectory = @"uploads/clm/vids";
                                break;
                            case ContentFormat.Pdf:
                                uploadDirectory = @"uploads/clm/docs";
                                break;
                            default:
                                break;
                        }

                        string fileName = Guid.NewGuid().ToString() + "_" + model.ContentFile.FileName;
                        var filePath = Path.Combine(_webHostEnvironment.WebRootPath, uploadDirectory, fileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.ContentFile.CopyToAsync(fileStream);
                        }

                        string fileUrl = "/" + uploadDirectory + "/" + fileName;
                        if (await _clmService.UpdateContentLinkAsync(model.CourseContentID, fileUrl, filePath))
                        {
                            if (!string.IsNullOrWhiteSpace(previousFilePath))
                            {
                                FileInfo file = new FileInfo(previousFilePath);
                                if (file.Exists)
                                {
                                    if (!file.IsFileOpen())
                                    {
                                        await Task.Run(() => {
                                            file.Delete();
                                        });
                                    }
                                }
                            }
                            model.ViewModelSuccessMessage = $"Congratulations! File was uploaded successfully.";
                        }
                        else
                        {
                            FileInfo file = new FileInfo(filePath);
                            if (file.Exists)
                            {
                                if (!file.IsFileOpen())
                                {
                                    await Task.Run(() => {
                                        file.Delete();
                                    });
                                }
                            }
                            model.ViewModelErrorMessage = $"Error! An error was encountered. File upload failed.";
                        }
                    }
                }
                else
                {
                    model.ViewModelSuccessMessage = "Sorry, it appears the maximum file size is exceeded or some important form fields are missing. Please try again.";
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        #endregion

    }
}