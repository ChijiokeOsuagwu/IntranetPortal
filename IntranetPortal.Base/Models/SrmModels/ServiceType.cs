using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.SrmModels
{
    public class ServiceType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ServiceSystemId { get; set; }
        public string ServiceSystemName { get; set; }
    }
}
