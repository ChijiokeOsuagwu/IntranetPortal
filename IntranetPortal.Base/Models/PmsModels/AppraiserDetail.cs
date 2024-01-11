using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.PmsModels
{
    public class AppraiserDetail
    {
        public int ResultSummaryId { get; set; }
        public int ReviewHeaderId { get; set; }
        public int ReviewSessionId { get; set; }
        public string AppraiserId { get; set; }
        public string AppraiserNo { get; set; }
        public string AppraiserName { get; set; }
        public string AppraiserFullDescription { get; set; }
        public string AppraiserRoleDescription { get; set; }
        public string AppraiserTypeDescription { get; set; }
        public string AppraiserDesignation { get; set; }
        public string AppraiserUnitName { get; set; }
        public string AppraiserDepartmentName { get; set; }
        public string AppraiserLocationName { get; set; }
    }
}
