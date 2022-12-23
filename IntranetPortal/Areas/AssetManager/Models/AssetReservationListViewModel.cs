using IntranetPortal.Base.Models.AssetManagerModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.AssetManager.Models
{
    public class AssetReservationListViewModel : BaseListViewModel
    {
        public int? yr { get; set; }
        public int? mn { get; set; }
        public string AssetID { get; set; }
        public List<AssetReservation> AssetReservationList { get; set; }
    }
}
