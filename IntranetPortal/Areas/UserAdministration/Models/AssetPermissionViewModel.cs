using IntranetPortal.Base.Models.AssetManagerModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.UserAdministration.Models
{
    public class AssetPermissionViewModel:BaseViewModel
    {
        public int AssetPermissionID { get; set; }

        [Required]
        public string UserID { get; set; }

        [Display(Name="Full Name")]
        public string UserFullName { get; set; }

        [Required(ErrorMessage = "Control Unit is required.")]
        [Display(Name = "Control Unit")]
        public int AssetDivisionID { get; set; }

        [Display(Name = "Control Unit")]
        public string AssetDivisionName { get; set; }

        public AssetPermission ConvertToAssetPermission()
        {
            return new AssetPermission
            {
                ID = AssetPermissionID,
                AssetDivisionID = AssetDivisionID,
                AssetDivisionName = AssetDivisionName,
                PermissionType = Base.Enums.EntityPermissionType.AssetDivision,
                UserID = UserID,
            };
        }
    }
}
