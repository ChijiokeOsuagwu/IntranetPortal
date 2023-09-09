using IntranetPortal.Base.Models.WksModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;

namespace IntranetPortal.Areas.WKS.Models
{
    public class TaskListViewModel:BaseViewModel
    {
        public string id { get; set; }
        public int st { get; set; }
        public int cy { get; set; }
        public int cm { get; set; }
        public List<TaskList> TaskLists { get; set; }
    }
}