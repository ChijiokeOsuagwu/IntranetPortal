using IntranetPortal.Base.Models.BamsModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.BamsRepositories
{
    public interface IBamsSettingsRepository
    {
        Task<IList<AssignmentEventType>> GetAllAssignmentEventTypesAsync();
        Task<IList<AssignmentStatus>> GetAssignmentStatusByTypeAsync(string statusType);
    }
}
