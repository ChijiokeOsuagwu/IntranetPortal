using IntranetPortal.Base.Models.GlobalSettingsModels;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.GlobalSettingsRepositories
{
    public interface IProgramRepository
    {
        IConfiguration _config { get; }

        #region Programme Write Action Methods
        Task<bool> AddProgramAsync(Programme program);
        Task<bool> DeleteProgramAsync(int Id);
        Task<bool> EditProgramAsync(Programme program);
        #endregion

        #region Programme Read Action Methods
        Task<Programme> GetByIdAsync(int Id);
        Task<Programme> GetByCodeAsync(string programCode);
        Task<IList<Programme>> SearchByTitleAsync(string programTitle);
        Task<IList<Programme>> GetByTitleAsync(string programTitle);
        Task<IList<Programme>> GetAllAsync();
        Task<IList<Programme>> GetByProgramTypeAsync(string programType);
        Task<IList<Programme>> GetByProgramBeltAsync(string programBelt);
        Task<IList<Programme>> GetByProgramTypeAndProgramBeltAsync(string programType, string programBelt);
        #endregion

        #region Programme Belt Read Action Methods
        Task<IList<ProgrammeBelt>> GetAllProgrammeBeltsAsync();
        #endregion

        #region Program Platform Read Action Methods
        Task<IList<ProgramPlatform>> GetAllProgramPlatformsAsync();
        #endregion
    }
}
