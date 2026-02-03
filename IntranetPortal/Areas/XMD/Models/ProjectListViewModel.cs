using IntranetPortal.Base.Models.WspModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.XMD.Models
{
    public class ProjectListViewModel:BaseListViewModel
    {
        public int tp { get; set; }
        public int? st { get; set; }
        public List<Project> ProjectList { get; set; }
    }
}
