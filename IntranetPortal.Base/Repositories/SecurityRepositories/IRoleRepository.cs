using IntranetPortal.Base.Models.SecurityModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.SecurityRepositories
{
    public interface IRoleRepository
    {
        Task<IList<ApplicationRole>> GetUnGrantedRolesByUserIdAsync(string userId);
        Task<IList<ApplicationRole>> GetUnGrantedRolesByUserIdAndApplicationIdAsync(string userId, string applicationId);
    }
}
