using IntranetPortal.Base.Models.WspModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.WSP.Models
{
    public class ManageTaskFolderViewModel:BaseViewModel
    {
        public long Id { get; set; }

        [Required]
        [Display(Name = "Folder Title")]
        [MaxLength(150)]
        public string Title { get; set; }
        public long? WorkspaceId { get; set; }
        public bool IsArchived { get; set; }

        [Required]
        [Display(Name = "Owner")]
        public string OwnerId { get; set; }

        [Display(Name = "Owner")]
        [MaxLength(150)]
        public string OwnerName { get; set; }

        [Display(Name = "Start Date")]
        public DateTime? PeriodStartDate { get; set; }

        [Display(Name = "Due Date")]
        public DateTime? PeriodEndDate { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }
        public DateTime? CreatedTime { get; set; }
        [Display(Name = "Last Updated By")]
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public string WorkspaceTitle { get; set; }
        public bool IsLocked { get; set; }
        public bool IsReuseable { get; set; }
        public string SourcePage { get; set; }
        public WorkItemFolder Convert()
        {
            return new WorkItemFolder
            {
                CreatedBy = CreatedBy,
                CreatedTime = CreatedTime,
                UpdatedBy = UpdatedBy,
                UpdatedTime = UpdatedTime,
                Id = Id,
                IsArchived = IsArchived,
                IsLocked = IsLocked,
                IsReuseable = IsReuseable,
                OwnerId = OwnerId,
                PeriodEndDate = PeriodEndDate,
                PeriodStartDate = PeriodStartDate,
                Title = Title,
                WorkspaceId = WorkspaceId,
            };
        }
        public ManageTaskFolderViewModel Convert(WorkItemFolder folder)
        {
            return new ManageTaskFolderViewModel
            {
                CreatedBy = folder.CreatedBy,
                CreatedTime = folder.CreatedTime,
                Id = folder.Id,
                IsArchived = folder.IsArchived,
                IsLocked = folder.IsLocked,
                IsReuseable = folder.IsReuseable,
                OwnerId = folder.OwnerId,
                OwnerName = folder.OwnerName,
                PeriodEndDate = folder.PeriodEndDate,
                PeriodStartDate = folder.PeriodStartDate,
                Title = folder.Title,
                WorkspaceId = folder.WorkspaceId,
                WorkspaceTitle = folder.WorkspaceTitle,
            };
        }
    }
}
