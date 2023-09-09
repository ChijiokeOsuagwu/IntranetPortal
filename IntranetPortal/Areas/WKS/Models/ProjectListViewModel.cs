using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IntranetPortal.Base.Models.WksModels;
using IntranetPortal.Models;

namespace IntranetPortal.Areas.WKS.Models
{
    public class ProjectListViewModel : BaseListViewModel
    {
        public string OwnerID { get; set; }
        public int? id { get; set; }
        public string Source { get; set; }
        public List<Project> ProjectList { get; set; }
    }
}
