using IntranetPortal.Base.Models.PmsModels;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.PmsRepositories
{
    public interface IApprovalRoleRepository
    {
        IConfiguration _config { get; }

        Task<bool> AddAsync(ApprovalRole approvalRole);
        Task<bool> DeleteAsync(int approvalRoleId);
        Task<IList<ApprovalRole>> GetAllAsync();
        Task<IList<ApprovalRole>> GetByIdAsync(int approvalRoleId);
        Task<IList<ApprovalRole>> GetByNameAsync(string approvalRoleName);
        Task<IList<ApprovalRole>> GetMustApproveContractsAsync();
        Task<IList<ApprovalRole>> GetMustApproveEvaluationsAsync();
        Task<bool> UpdateAsync(ApprovalRole approvalRole);
    }
}