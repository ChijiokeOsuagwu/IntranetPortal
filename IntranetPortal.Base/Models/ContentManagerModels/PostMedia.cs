using IntranetPortal.Base.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.ContentManagerModels
{
    public class PostMedia
    {
        public long PostMediaId { get; set; }
        public MediaType MediaType { get; set; }
        public string MediaTypeDescription { get; set; }
        public string MediaLocationPath { get; set; }
        public long MasterPostId { get; set; }
        public string Caption { get; set; }
        public string MediaLocationFullPath { get; set; }
    }
}
