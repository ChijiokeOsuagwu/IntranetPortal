using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.WKS.Models
{
    public class AddTaskNoteViewModel:BaseViewModel
    {
        public long TaskItemID { get; set; }
        public long TaskNoteID { get; set; }
        public DateTime? NoteTime { get; set; }
        [Required]
        public string NoteText { get; set; }
        public string WrittenBy { get; set; }
        public int TaskListID { get; set; }
        public bool IsTaskNote { get; set; }
        public string Source { get; set; }
        public string OwnerID { get; set; }
    }
}
