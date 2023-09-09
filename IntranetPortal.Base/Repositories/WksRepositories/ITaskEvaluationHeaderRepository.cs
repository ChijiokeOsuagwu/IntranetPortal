using IntranetPortal.Base.Models.WksModels;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.WksRepositories
{
    public interface ITaskEvaluationHeaderRepository
    {
        IConfiguration _config { get; }

        #region Task Evaluation Header Write Action Interfaces
        Task<int> AddAsync(TaskEvaluationHeader taskEvaluationHeader);
        Task<bool> UpdateAsync(TaskEvaluationHeader taskEvaluationHeader);
        #endregion

        #region Task Evaluation Header Read Action Interfaces - By Task Owner ID
        Task<List<TaskEvaluationHeader>> GetByIdAsync(int taskEvaluationHeaderId);
        Task<List<TaskEvaluationHeader>> GetByTaskOwnerIdAsync(string taskOwnerId);
        Task<List<TaskEvaluationHeader>> GetByTaskOwnerIdAndDueYearAsync(string taskOwnerId, int dueYear);
        Task<List<TaskEvaluationHeader>> GetByTaskOwnerIdAndDueYearAndDueMonthAsync(string taskOwnerId, int dueYear, int dueMonth);
        #endregion

        #region Task Evaluation Header Read Action Interfaces - By Task List ID
        Task<List<TaskEvaluationHeader>> GetByTaskListIdAndEvaluatorIdAsync(int taskListId, string evaluatorId);
        Task<TaskEvaluationHeader> GetScoresByTaskListIdAndEvaluatorIdAsync(int taskListId, string evaluatorId);

        #endregion

        #region Task Evaluation Header Read Action Interfaces - By Reports To Employee ID
        //================= Read By Reports To Employee ID ===============================//
        Task<List<TaskEvaluationHeader>> GetByReportsToEmployeeIdAndDueYearAndDueMonthAsync(string reportsToEmployeeId, int dueYear, int dueMonth);
        Task<List<TaskEvaluationHeader>> GetByReportsToEmployeeIdAndDueYearAsync(string reportsToEmployeeId, int dueYear);
        Task<List<TaskEvaluationHeader>> GetByReportsToEmployeeIdAsync(string reportsToEmployeeId);

        #endregion

        #region Task Evaluation Header Read Action Interfaces - By Location, Department, Unit
        //================= Read By Location, Department and Unit ===============================//
        Task<List<TaskEvaluationHeader>> GetByUnitIdAndEvaluationYearAndEvaluationMonthAsync(int taskOwnerUnitId, int startYear, int endYear, int startMonth, int endMonth);
        Task<List<TaskEvaluationHeader>> GetByUnitIdAndEvaluationYearAsync(int taskOwnerUnitId, int startYear, int endYear);
        Task<List<TaskEvaluationHeader>> GetByUnitIdAndLocationIdAndEvaluationYearAndEvaluationMonthAsync(int taskOwnerUnitId, int taskOwnerLocationId, int startYear, int endYear, int startMonth, int endMonth);
        Task<List<TaskEvaluationHeader>> GetByUnitIdAndLocationIdAndEvaluationYearAsync(int taskOwnerUnitId, int taskOwnerLocationId, int startYear, int endYear);
        Task<List<TaskEvaluationHeader>> GetByDepartmentIdAndEvaluationYearAndEvaluationMonthAsync(int taskOwnerDepartmentId, int startYear, int endYear, int startMonth, int endMonth);
        Task<List<TaskEvaluationHeader>> GetByDepartmentIdAndEvaluationYearAsync(int taskOwnerDepartmentId, int startYear, int endYear);
        Task<List<TaskEvaluationHeader>> GetByDepartmentIdAndLocationIdAndEvaluationYearAndEvaluationMonthAsync(int taskOwnerDepartmentId, int taskOwnerLocationId, int startYear, int endYear, int startMonth, int endMonth);
        Task<List<TaskEvaluationHeader>> GetByDepartmentIdAndLocationIdAndEvaluationYearAsync(int taskOwnerDepartmentId, int taskOwnerLocationId, int startYear, int endYear);
        Task<List<TaskEvaluationHeader>> GetByLocationIdAndEvaluationYearAndEvaluationMonthAsync(int taskOwnerLocationId, int startYear, int endYear, int startMonth, int endMonth);
        Task<List<TaskEvaluationHeader>> GetByLocationIdAndEvaluationYearAsync(int taskOwnerLocationId, int startYear, int endYear);
        Task<List<TaskEvaluationHeader>> GetByEvaluationYearAndEvaluationMonthAsync(int startYear, int endYear, int startMonth, int endMonth);
        Task<List<TaskEvaluationHeader>> GetByEvaluationYearAsync(int startYear, int endYear);
        #endregion

        #region Task Evaluation Header Summary Report Read Action Interfaces - By Location, Department, Unit
        //================= Read By Location, Department and Unit ===============================//
        Task<List<TaskEvaluationHeaderSummary>> GetSummaryByUnitIdAndEvaluationYearAndEvaluationMonthAsync(int taskOwnerUnitId, int startYear, int endYear, int startMonth, int endMonth);
        Task<List<TaskEvaluationHeaderSummary>> GetSummaryByUnitIdAndEvaluationYearAsync(int taskOwnerUnitId, int startYear, int endYear);
        Task<List<TaskEvaluationHeaderSummary>> GetSummaryByUnitIdAndLocationIdAndEvaluationYearAndEvaluationMonthAsync(int taskOwnerUnitId, int taskOwnerLocationId, int startYear, int endYear, int startMonth, int endMonth);
        Task<List<TaskEvaluationHeaderSummary>> GetSummaryByUnitIdAndLocationIdAndEvaluationYearAsync(int taskOwnerUnitId, int taskOwnerLocationId, int startYear, int endYear);
        Task<List<TaskEvaluationHeaderSummary>> GetSummaryByDepartmentIdAndEvaluationYearAndEvaluationMonthAsync(int taskOwnerDepartmentId, int startYear, int endYear, int startMonth, int endMonth);
        Task<List<TaskEvaluationHeaderSummary>> GetSummaryByDepartmentIdAndEvaluationYearAsync(int taskOwnerDepartmentId, int startYear, int endYear);
        Task<List<TaskEvaluationHeaderSummary>> GetSummaryByDepartmentIdAndLocationIdAndEvaluationYearAndEvaluationMonthAsync(int taskOwnerDepartmentId, int taskOwnerLocationId, int startYear, int endYear, int startMonth, int endMonth);
        Task<List<TaskEvaluationHeaderSummary>> GetSummaryByDepartmentIdAndLocationIdAndEvaluationYearAsync(int taskOwnerDepartmentId, int taskOwnerLocationId, int startYear, int endYear);
        Task<List<TaskEvaluationHeaderSummary>> GetSummaryByLocationIdAndEvaluationYearAndEvaluationMonthAsync(int taskOwnerLocationId, int startYear, int endYear, int startMonth, int endMonth);
        Task<List<TaskEvaluationHeaderSummary>> GetSummaryByLocationIdAndEvaluationYearAsync(int taskOwnerLocationId, int startYear, int endYear);
        Task<List<TaskEvaluationHeaderSummary>> GetSummaryByEvaluationYearAndEvaluationMonthAsync(int startYear, int endYear, int startMonth, int endMonth);
        Task<List<TaskEvaluationHeaderSummary>> GetSummaryByEvaluationYearAsync(int startYear, int endYear);
        #endregion
    }
}
