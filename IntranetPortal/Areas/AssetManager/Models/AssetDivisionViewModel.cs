using System;
using IntranetPortal.Base.Models.AssetManagerModels;
using IntranetPortal.Models;
using System.ComponentModel.DataAnnotations;

namespace IntranetPortal.Areas.AssetManager.Models
{
    public class AssetDivisionViewModel:BaseViewModel
    {
        public int? ID { get; set; }

        [Required]
        [Display(Name = "Name")]
        [MaxLength(100, ErrorMessage = "Name must not exceed 100 characters.")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        [MaxLength(250, ErrorMessage = "Description must not exceed 250 characters.")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Location")]
        public int LocationID { get; set; }

        public AssetDivision ConvertToAssetDivision()
        {
            return new AssetDivision
            {
                ID = ID,
                Name = Name,
                Description = Description,
                LocationID = LocationID,
            };
        }
    }
}
