using IntranetPortal.Base.Models.PmsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.PMS.Models
{
    public class ReviewSessionsListViewModel:BaseViewModel
    {
        public int? Id { get; set; }
        public List<ReviewSession> ReviewSessionsList { get; set; }
    }
}
