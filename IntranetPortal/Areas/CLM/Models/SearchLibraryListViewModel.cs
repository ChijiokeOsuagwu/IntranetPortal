using IntranetPortal.Base.Models.ClmModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.CLM.Models
{
    public class SearchLibraryListViewModel:BaseListViewModel
    {

        public List<Course> CourseList { get; set; }
    }
}
