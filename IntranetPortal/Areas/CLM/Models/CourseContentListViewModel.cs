using IntranetPortal.Base.Models.ClmModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.CLM.Models
{
    public class CourseContentListViewModel:BaseListViewModel
    {
        public int id { get; set; }
        public int? fm { get; set; }
        public IList<CourseContent> CourseContentsList { get; set; }
    }
}
