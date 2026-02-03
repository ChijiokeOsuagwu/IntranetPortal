using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.WspModels
{
    public class ProjectType
    {
        public int ProjectTypeId { get; set; }
        public string ProjectTypeDescription { get; set; }
        public bool IsExecutiveManagementType { get; set; }
    }
}
