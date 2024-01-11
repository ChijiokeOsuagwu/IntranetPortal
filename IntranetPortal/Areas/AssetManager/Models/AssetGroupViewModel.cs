using IntranetPortal.Base.Models.AssetManagerModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.AssetManager.Models
{
    public class AssetGroupViewModel:BaseViewModel
    {
        public int? GroupID { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "Name must not exceed 100 characters.")]
        public string GroupName { get; set; }

        [Required]
        [Display(Name = "Class")]
        public int ClassID { get; set; }

        [Display(Name = "Class")]
        public string ClassName { get; set; }

        [Display(Name = "Category")]
        public int CategoryID { get; set; }

        [Display(Name = "Category")]
        public string CategoryName { get; set; }

        public AssetGroup ConvertToAssetGroup()
        {
            return new AssetGroup
            {
                GroupID = GroupID,
                GroupName = GroupName,
                CategoryID = CategoryID,
                ClassID = ClassID
            };
        }
    }
}
