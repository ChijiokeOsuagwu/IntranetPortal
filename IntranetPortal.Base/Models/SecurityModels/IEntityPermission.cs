using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.SecurityModels
{
    public interface IEntityPermission
    {
        string UserId { get; set; }
        int? LocationId { get; set; }
    }
}
