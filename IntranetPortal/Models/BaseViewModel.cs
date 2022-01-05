using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Models
{
    public class BaseViewModel
    {
        public string ViewModelErrorMessage { get; set; }
        public string ViewModelSuccessMessage { get; set; }
        public string ViewModelWarningMessage { get; set; }
        public bool OperationIsCompleted { get; set; }
        public bool OperationIsSuccessful { get; set; }
    }
}
