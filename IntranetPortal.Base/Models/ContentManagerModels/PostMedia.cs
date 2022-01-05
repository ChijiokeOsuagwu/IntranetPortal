using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.ContentManagerModels
{
    public class PostMedia
    {
        public int PostMediaId { get; set; }
        public string MediaType { get; set; }
        public string MediaLocationPath { get; set; }
        public int PostId { get; set; }
    }
}
