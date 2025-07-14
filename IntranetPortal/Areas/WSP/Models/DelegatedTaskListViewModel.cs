using IntranetPortal.Base.Models.WspModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.WSP.Models
{
    public class DelegatedTaskListViewModel:BaseListViewModel
    {
        public string id { get; set; }
        public int? ps { get; set; }
        public DateTime? fd { get; set; }
        public DateTime? td { get; set; }
        public string ed { get; set; }
        public List<DelegatedTaskItem> TaskItems { get; set; }

    }
}
