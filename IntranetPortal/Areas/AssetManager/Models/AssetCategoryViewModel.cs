using IntranetPortal.Base.Models.AssetManagerModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.AssetManager.Models
{
    public class AssetCategoryViewModel : BaseViewModel
    {
        public int? ID { get; set; }

        [Required]
        [Display(Name = "Category Name")]
        [MaxLength(100, ErrorMessage = "Category Name must not exceed 100 characters.")]
        public string Name { get; set; }

        [Display(Name = "Category Description")]
        [MaxLength(250, ErrorMessage = "Category Name must not exceed 250 characters.")]
        public string Description { get; set; }

        public AssetCategory ConvertToAssetCategory()
        {
            return new AssetCategory
            {
                ID = ID,
                Name = Name,
                Description = Description
            };
    }
    }
}
