using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.SrmModels
{
    public class ServiceCenterConfiguration
    {
        public long Id { get; set; }
        public int ServiceCenterId { get; set; }
        public string ServiceCenterName { get; set; }
        public int ServiceTypeId { get; set; }
        public string ServiceTypeName { get; set; }
        public int? ServiceSystemId { get; set; }
        public string ServiceSystemName { get; set; }
    }
}
