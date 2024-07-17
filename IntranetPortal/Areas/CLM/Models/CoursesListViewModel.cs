using IntranetPortal.Base.Models.ClmModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.CLM.Models
{
    public class CoursesListViewModel:BaseListViewModel
    {
        public int tp { get; set; }
        public int sa { get; set; }
        public int lv { get; set; }
        public string nm { get; set; }
        public IList<Course> CoursesList { get; set; }
    }
}
