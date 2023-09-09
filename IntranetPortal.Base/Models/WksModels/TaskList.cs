using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.WksModels
{
    public class TaskList
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsArchived { get; set; }
        public bool IsLocked { get; set; }
        public string OwnerId { get; set; }
        public string OwnerName { get; set; }
        public DateTime? CreatedTime { get; set; }
        public string CreatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedTime { get; set; }
        public string DeletedBy { get; set; }
    }
}
