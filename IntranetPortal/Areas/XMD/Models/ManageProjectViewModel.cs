using IntranetPortal.Base.Models.WspModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.XMD.Models
{
    public class ManageProjectViewModel : BaseViewModel
    {
        public long ProjectId { get; set; }

        [Required]
        [Display(Name = "Project Title")]
        [MaxLength(100)]
        public string ProjectTitle { get; set; }

        [Required]
        [Display(Name = "Project Number")]
        [MaxLength(20)]
        public string ProjectCode { get; set; }

        [Display(Name = "Project Details")]
        [MaxLength(2500)]
        public string ProjectDetails { get; set; }

        [Required]
        [Display(Name = "Project Type")]
        public int? ProjectTypeId { get; set; }

        [Display(Name = "Project Type")]
        [MaxLength(50)]
        public string ProjectTypeName { get; set; }

        [Display(Name = "Progress Status*")]
        public int ProgressStatusId { get; set; }

        [Display(Name = "Progress Status")]
        [MaxLength(100)]
        public string ProgressStatusDescription { get; set; }

        [Required]
        [Display(Name = "Project Owner")]
        public string ProjectOwnerId { get; set; }

        [Display(Name = "Project Owner")]
        [MaxLength(150)]
        public string ProjectOwnerName { get; set; }

        [Display(Name = "Expected Start Date")]
        public DateTime? ExpectedStartTime { get; set; }

        [Display(Name = "Expected Due Date")]
        public DateTime? ExpectedEndTime { get; set; }

        [Display(Name = "Project Folder")]
        public long? ProjectDrawerId { get; set; }

        [Display(Name = "Project Folder")]
        public string ProjectDrawerTitle { get; set; }

        [Display(Name = "Unit")]
        public int? UnitId { get; set; }

        [Display(Name = "Unit")]
        public string UnitName { get; set; }

        [Display(Name = "Department")]
        public int? DepartmentId { get; set; }

        [Display(Name = "Department")]
        public string DepartmentName { get; set; }

        [Display(Name = "Location")]
        public int? LocationId { get; set; }

        [Display(Name = "Location")]
        public string LocationName { get; set; }

        public Project Convert()
        {
            return new Project()
            {
                DepartmentId = DepartmentId,
                DepartmentName = DepartmentName,
                ExpectedEndTime = ExpectedEndTime,
                ExpectedStartTime = ExpectedStartTime,
                LocationId = LocationId,
                LocationName = LocationName,
                ProjectCode = ProjectCode,
                ProjectDetails = ProjectDetails,
                ProjectDrawerId = ProjectDrawerId,
                ProjectDrawerTitle = ProjectDrawerTitle,
                ProjectId = ProjectId,
                ProjectOwnerId = ProjectOwnerId,
                ProjectOwnerName = ProjectOwnerName,
                ProjectTitle = ProjectTitle,
                ProjectTypeId = ProjectTypeId,
                ProjectTypeName = ProjectTypeName,
                UnitId = UnitId,
                UnitName = UnitName,
                ProgressStatusDescription = ProgressStatusDescription,
                ProgressStatusId = ProgressStatusId,
            };
        }

        public ManageProjectViewModel Convert(Project project)
        {
            return new ManageProjectViewModel()
            {
                DepartmentId = project.DepartmentId,
                DepartmentName = project.DepartmentName,
                ExpectedEndTime = project.ExpectedEndTime,
                ExpectedStartTime = project.ExpectedStartTime,
                LocationId = project.LocationId,
                LocationName = project.LocationName,
                ProjectCode = project.ProjectCode,
                ProjectDetails = project.ProjectDetails,
                ProjectDrawerId = project.ProjectDrawerId,
                ProjectDrawerTitle = project.ProjectDrawerTitle,
                ProjectId = project.ProjectId,
                ProjectOwnerId = project.ProjectOwnerId,
                ProjectOwnerName = project.ProjectOwnerName,
                ProjectTitle = project.ProjectTitle,
                ProjectTypeId = project.ProjectTypeId,
                ProjectTypeName = project.ProjectTypeName,
                UnitId = project.UnitId,
                UnitName = project.UnitName,
                ProgressStatusDescription = project.ProgressStatusDescription,
                ProgressStatusId = project.ProgressStatusId
            };
        }
    }
}
