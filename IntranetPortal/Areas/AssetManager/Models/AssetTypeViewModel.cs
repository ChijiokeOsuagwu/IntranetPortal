using IntranetPortal.Base.Models.AssetManagerModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.AssetManager.Models
{
    public class AssetTypeViewModel : BaseViewModel
    {
        public int? ID { get; set; }

        [Required]
        [MaxLength(100,ErrorMessage ="Name must not exceed 100 characters.")]
        public string Name { get; set; }

        [Required]
        [MaxLength(250, ErrorMessage = "Description must not exceed 250 characters.")]
        public string Description { get; set; }

        [Required]
        [Display(Name="Category")]
        public int CategoryID { get; set; }

        [Display(Name = "Category")]
        public string CategoryName { get; set; }

        public AssetType ConvertToAssetType()
        {
            return new AssetType
            {
                ID = ID,
                Name = Name,
                Description = Description,
                CategoryID = CategoryID
            };
        }
    }
}
