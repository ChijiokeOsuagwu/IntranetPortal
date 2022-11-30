using IntranetPortal.Base.Models.BamsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.BAMS.Models
{
    public class EquipmentGroupsViewModel : BaseViewModel
    {
        public int? EquipmentGroupID { get; set; }

        [Display(Name = "Group Name")]
        [Required]
        public string EquipmentGroupName { get; set; }

        [Display(Name = "Group Description")]
        public string EquipmentGroupDescription { get; set; }

        public EquipmentGroup ConvertToEquipmentGroup()
        {
            return new EquipmentGroup
            {
                EquipmentGroupID = EquipmentGroupID,
                EquipmentGroupName = EquipmentGroupName,
                EquipmentGroupDescription = EquipmentGroupDescription,
            };
        }
    }
}
