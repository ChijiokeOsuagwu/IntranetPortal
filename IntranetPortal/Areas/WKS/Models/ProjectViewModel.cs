using IntranetPortal.Base.Enums;
using IntranetPortal.Base.Models.WksModels;
using IntranetPortal.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace IntranetPortal.Areas.WKS.Models
{
    public class ProjectViewModel : BaseViewModel
    {
        public string Code { get; set; }
        public string ProgressStatusDescription { get; set; }
        public string Instructions { get; set; }

        [Display(Name="In Folder")]
        public int FolderID { get; set; }

        [Display(Name = "In Folder")]
        public string FolderTitle { get; set; }
        public int WorkspaceID { get; set; }
        public string WorkspaceTitle { get; set; }


        public int ID { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }
        public string Description { get; set; }
        public string OwnerID { get; set; }
        public string OwnerName { get; set; }

        [Display(Name="Assign To")]
        public string AssignedToID { get; set; }

        [Display(Name="Assign To")]
        public string AssignedToName { get; set; }
        public DateTime? AssignedTime { get; set; }
        public WorkItemProgressStatus ProgressStatus { get; set; }

        [Display(Name="Start Date")]
        [DataType(DataType.Date)]
        public DateTime? ExpectedStartTime { get; set; }

        [Display(Name="Due Date")]
        [DataType(DataType.Date)]
        public DateTime? ExpectedDueTime { get; set; }
        public long ExpectedDurationInMinutes { get; set; }
        public long ExpectedDurationInHours { get; set; }
        public int ExpectedDurationInDays { get; set; }
        public long TotalExpectedDurationInMinutes { get; set; }
        public long TotalActualDurationInMinutes { get; set; }
        public long ActualDurationInHours { get; set; }
        public int ActualDurationInDays { get; set; }
        public long ActualDurationInMinutes { get; set; }
        public WorkItemPriority Priority { get; set; }
        public long EstimatedCostForeignCurrency { get; set; }
        public long ActualCostForeignCurrency { get; set; }
        public string ForeignCurrencyCode { get; set; }
        public long EstimatedCostLocalCurrency { get; set; }
        public long ActualCostLocalCurrency { get; set; }
        public int PercentageCompleted { get; set; }
        public string Deliverables { get; set; }

        [Display(Name="Unit")]
        public int? UnitID { get; set; }

        [Display(Name = "Unit")]
        public string UnitName { get; set; }

        [Display(Name = "Department")]
        public int? DepartmentID { get; set; }

        [Display(Name = "Department")]
        public string DepartmentName { get; set; }

        [Display(Name="Location")]
        public int? LocationID { get; set; }

        [Display(Name = "Location")]
        public string LocationName { get; set; }
        public int? MasterWorkItemID { get; set; }
        public string MasterWorkItemTitle { get; set; }
        public string CreatedTime { get; set; }
        public string CreatedBy { get; set; }
        public string LastModifiedTime { get; set; }
        public string LastModifiedBy { get; set; }

        public Project ConvertToProject()
        {
            return new Project
            {
                ActualCostForeignCurrency = this.ActualCostForeignCurrency,
                ActualDurationInDays = this.ActualDurationInDays,
                ActualCostLocalCurrency = this.ActualCostLocalCurrency,
                ActualDurationInHours = this.ActualDurationInHours,
                ActualDurationInMinutes = this.ActualDurationInMinutes,
                AssignedTime = this.AssignedTime,
                AssignedToID = this.AssignedToID,
                AssignedToName = this.AssignedToName,
                Number = this.Code,
                CreatedBy = this.CreatedBy,
                CreatedTime = this.CreatedTime,
                Deliverables = this.Deliverables,
                DepartmentID = this.DepartmentID,
                DepartmentName = this.DepartmentName,
                Description = this.Description,
                EstimatedCostForeignCurrency = this.EstimatedCostForeignCurrency,
                EstimatedCostLocalCurrency = this.EstimatedCostLocalCurrency,
                ExpectedDueTime = this.ExpectedDueTime,
                ExpectedDurationInDays = this.ExpectedDurationInDays,
                ExpectedDurationInHours = this.ExpectedDurationInHours,
                ExpectedDurationInMinutes = this.ExpectedDurationInMinutes,
                ExpectedStartTime = this.ExpectedStartTime,
                FolderID = this.FolderID,
                FolderTitle = this.FolderTitle,
                ForeignCurrencyCode = this.ForeignCurrencyCode,
                ID = ID,
                Instructions = this.Instructions,
                LastModifiedBy = this.LastModifiedBy,
                LastModifiedTime = this.LastModifiedTime,
                LocationID = this.LocationID,
                LocationName = this.LocationName,
                OwnerID = this.OwnerID,
                OwnerName = this.OwnerName,
                PercentageCompleted = this.PercentageCompleted,
                Priority = this.Priority,
                ProgressStatus = this.ProgressStatus,
                ProgressStatusDescription = this.ProgressStatusDescription,
                Title = this.Title,
                TotalActualDurationInMinutes = this.TotalActualDurationInMinutes,
                TotalExpectedDurationInMinutes = this.TotalExpectedDurationInMinutes,
                UnitID = this.UnitID,
                UnitName = this.UnitName,
                WorkspaceID = this.WorkspaceID,
                WorkspaceTitle = this.WorkspaceTitle,
                MasterWorkItemID = this.MasterWorkItemID,
                MasterWorkItemTitle = this.MasterWorkItemTitle,
                ItemType = WorkItemType.Project
            };
        }

        public ProjectViewModel ExtractViewModel(Project project)
        {
            return new ProjectViewModel
            {
                ActualCostForeignCurrency = project.ActualCostForeignCurrency,
                ActualDurationInDays = project.ActualDurationInDays,
                ActualCostLocalCurrency = project.ActualCostLocalCurrency,
                ActualDurationInHours = project.ActualDurationInHours,
                ActualDurationInMinutes = project.ActualDurationInMinutes,
                AssignedTime = project.AssignedTime,
                AssignedToID = project.AssignedToID,
                AssignedToName = project.AssignedToName,
                Code = project.Number,
                CreatedBy = project.CreatedBy,
                CreatedTime = project.CreatedTime,
                Deliverables = project.Deliverables,
                DepartmentID = project.DepartmentID,
                DepartmentName = project.DepartmentName,
                Description = project.Description,
                EstimatedCostForeignCurrency = project.EstimatedCostForeignCurrency,
                EstimatedCostLocalCurrency = project.EstimatedCostLocalCurrency,
                ExpectedDueTime = project.ExpectedDueTime,
                ExpectedDurationInDays = project.ExpectedDurationInDays,
                ExpectedDurationInHours = project.ExpectedDurationInHours,
                ExpectedDurationInMinutes = project.ExpectedDurationInMinutes,
                ExpectedStartTime = project.ExpectedStartTime,
                FolderID = project.FolderID,
                FolderTitle = project.FolderTitle,
                ForeignCurrencyCode = project.ForeignCurrencyCode,
                ID = project.ID,
                Instructions = project.Instructions,
                LastModifiedBy = project.LastModifiedBy,
                LastModifiedTime = project.LastModifiedTime,
                LocationID = project.LocationID,
                LocationName = project.LocationName,
                OwnerID = project.OwnerID,
                OwnerName = project.OwnerName,
                PercentageCompleted = project.PercentageCompleted,
                Priority = project.Priority,
                ProgressStatus = project.ProgressStatus,
                ProgressStatusDescription = project.ProgressStatusDescription,
                Title = project.Title,
                TotalActualDurationInMinutes = project.TotalActualDurationInMinutes,
                TotalExpectedDurationInMinutes = project.TotalExpectedDurationInMinutes,
                UnitID = project.UnitID,
                UnitName = project.UnitName,
                WorkspaceID = project.WorkspaceID,
                WorkspaceTitle = project.WorkspaceTitle
            };
        }

    }
}
