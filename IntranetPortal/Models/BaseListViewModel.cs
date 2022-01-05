using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Models
{
    public class BaseListViewModel : BaseViewModel
    {
        public string SearchParameter { get; set; }
        public string SearchParameterValue { get; set; }
        public int RecordCount { get; set; }
        public int RecordsPerPage { get; set; }
    }
}
