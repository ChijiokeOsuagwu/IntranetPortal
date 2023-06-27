using IntranetPortal.Base.Models.SecurityModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.SecurityRepositories
{
    public interface IUserLoginRepository
    {
        Task<bool> AddAsync(UserLoginHistory userLoginHistory);
        Task<IList<UserLoginHistory>> GetByYearAndMonthAsync(int loginYear, int loginMonth);
        Task<IList<UserLoginHistory>> GetByYearAndMonthAndDayAsync(int loginYear, int loginMonth, int loginDay);

        Task<IList<UserLoginHistory>> GetByUserNameAndYearAsync(string loginUserName, int loginYear);
        Task<IList<UserLoginHistory>> GetByUserNameAndYearAndMonthAsync(string loginUserName, int loginYear, int loginMonth);
        Task<IList<UserLoginHistory>> GetByUserNameAndYearAndMonthAndDayAsync(string loginUserName, int loginYear, int loginMonth, int loginDay);
    }
}
