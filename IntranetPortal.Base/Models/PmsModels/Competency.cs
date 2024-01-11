using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.PmsModels
{
    public class Competency
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? CategoryId { get; set; }
        public string CategoryDescription { get; set; }
        public int? LevelId { get; set; }
        public string LevelDescription { get; set; }
        public int? IndustryId { get; set; }
        public string IndustryDescription { get; set; }
    }
}
