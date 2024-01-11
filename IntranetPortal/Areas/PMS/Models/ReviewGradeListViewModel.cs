using IntranetPortal.Base.Models.PmsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.PMS.Models
{
        public class ReviewGradeListViewModel : BaseViewModel
        {
            public int? TemplateId { get; set; }
            public int? SessionId { get; set; }
            public List<ReviewGrade> ReviewGradeList { get; set; }
        }

    }
