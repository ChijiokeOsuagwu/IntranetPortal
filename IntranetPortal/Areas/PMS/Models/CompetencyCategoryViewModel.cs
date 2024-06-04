using IntranetPortal.Base.Models.PmsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.PMS.Models
{
    public class CompetencyCategoryViewModel:BaseViewModel
    {
        public int? Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Description { get; set; }

        public CompetencyCategory ConvertToCompetencyCategory()
        {
            return new CompetencyCategory
            {
                Id = Id ?? 0,
                Description = Description,
            };
        }

        public CompetencyCategoryViewModel ExtractFromCompetencyCategory(CompetencyCategory competencyCategory)
        {
            return new CompetencyCategoryViewModel
            {
                Id = competencyCategory.Id,
                Description = competencyCategory.Description,
            };
        }
    }
}
