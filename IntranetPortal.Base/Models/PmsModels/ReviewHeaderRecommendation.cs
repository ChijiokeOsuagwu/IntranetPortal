using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.PmsModels
{
    public class ReviewHeaderRecommendation
    {
        public int ReviewHeaderId { get; set; }
        public string RecommendationType { get; set; }
        public string RecommendedAction { get; set; }
        public string RecommendedByName { get; set; }
        public string RecommendationRemarks { get; set; }
    }
}
