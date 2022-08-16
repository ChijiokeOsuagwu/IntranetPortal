using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Models
{
    public class BaseListViewModel : BaseViewModel
    {
        public string sp { get; set; }
        public string SearchParameterValue { get; set; }
        public int rc { get; set; }
        public int pg { get; set; }
    }
}
