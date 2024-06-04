using IntranetPortal.Base.Models.PmsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.PMS.Models
{
    public class ManageCompetencyViewModel:BaseViewModel
    {
        public int? Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required]
        [MaxLength(500)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Category")]
        public int? CategoryId { get; set; }

        [Display(Name = "Category")]
        public string CategoryDescription { get; set; }

        [Display(Name = "Level")]
        public int? LevelId { get; set; }

        [Display(Name = "Level")]
        public string LevelDescription { get; set; }


        public Competency ConvertToCompetency()
        {
            return new Competency
            {
                Id = Id ?? 0,
                Title = Title,
                Description = Description,
                CategoryId = CategoryId,
                CategoryDescription = CategoryDescription,
                LevelId = LevelId,
                LevelDescription = LevelDescription,
            };
        }

        public ManageCompetencyViewModel ExtractFromCompetency(Competency competency)
        {
            return new ManageCompetencyViewModel
            {
                Id = competency.Id,
                Title = competency.Title,
                Description = competency.Description,
                CategoryId = competency.CategoryId,
                CategoryDescription = competency.CategoryDescription,
                LevelId = competency.LevelId,
                LevelDescription = competency.LevelDescription,
            };
        }

    }
}
