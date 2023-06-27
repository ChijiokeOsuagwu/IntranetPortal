using IntranetPortal.Base.Models.AssetManagerModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.AssetManager.Models
{
    public class AssetBinLocationViewModel:BaseViewModel
    {
        public int? AssetBinLocationID { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "Name must not exceed 100 characters.")]
        [Display(Name="Name")]
        public string AssetBinLocationName { get; set; }

        [Required]
        [MaxLength(250, ErrorMessage = "Description must not exceed 250 characters.")]
        [Display(Name="Description")]
        public string AssetBinLocationDescription { get; set; }

        [Required]
        [Display(Name = "Location")]
        public int AssetLocationID { get; set; }

        [Display(Name = "Location")]
        public string AssetLocationName { get; set; }

        public AssetBinLocation ConvertToAssetBinLocation()
        {
            return new AssetBinLocation
            {
                AssetBinLocationID = AssetBinLocationID == null ? 0 : AssetBinLocationID.Value,
                AssetBinLocationName = AssetBinLocationName,
                AssetBinLocationDescription = AssetBinLocationDescription,
                AssetLocationID = AssetLocationID,
                AssetLocationName = AssetLocationName
            };
        }
    }
}
