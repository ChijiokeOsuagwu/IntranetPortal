using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.CLM.Models
{
    public class EditTextContentViewModel:BaseViewModel
    {
        public long CourseContentID { get; set; }
        public int CourseID { get; set; }
        public string ContentHeading { get; set; }
        public string ContentTitle { get; set; }
        public string RawTextContent { get; set; }
    }
}
