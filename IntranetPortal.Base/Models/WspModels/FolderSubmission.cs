﻿using IntranetPortal.Base.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.WspModels
{
    public class FolderSubmission
    {
        public long FolderSubmissionId { get; set; }
        public long FolderId { get; set; }
        public string FolderName { get; set; }
        public string FromEmployeeId { get; set; }
        public string FromEmployeeName { get; set; }
        public string FromEmployeeUnitName { get; set; }
        public string ToEmployeeId { get; set; }
        public string ToEmployeeName { get; set; }
        public WorkItemSubmissionType SubmissionType { get; set; }
        public DateTime? DateSubmitted { get; set; }
        public string Comment { get; set; }
        public bool IsActioned { get; set; }
        public DateTime? DateActioned { get; set; }

    }
}
