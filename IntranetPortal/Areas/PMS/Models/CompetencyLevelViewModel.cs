using IntranetPortal.Base.Models.PmsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.PMS.Models
{
    public class CompetencyLevelViewModel:BaseViewModel
    {
        public int? Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Description { get; set; }

        public CompetencyLevel ConvertToCompetencyLevel()
        {
            return new CompetencyLevel
            {
                Id = Id ?? 0,
                Description = Description,
            };
        }

        public CompetencyLevelViewModel ExtractFromCompetencyLevel(CompetencyLevel competencyLevel)
        {
            return new CompetencyLevelViewModel
            {
                Id = competencyLevel.Id,
                Description = competencyLevel.Description,
            };
        }
    }
}
