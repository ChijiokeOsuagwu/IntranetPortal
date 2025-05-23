﻿using IntranetPortal.Base.Repositories.AssetManagerRepositories;
using IntranetPortal.Base.Repositories.BamsRepositories;
using IntranetPortal.Base.Repositories.BaseRepositories;
using IntranetPortal.Base.Repositories.BusinessManagerRepositories;
using IntranetPortal.Base.Repositories.ClmRepositories;
using IntranetPortal.Base.Repositories.ContentManagerRepositories;
using IntranetPortal.Base.Repositories.ErmRepositories;
using IntranetPortal.Base.Repositories.GlobalSettingsRepositories;
using IntranetPortal.Base.Repositories.LmsRepositories;
using IntranetPortal.Base.Repositories.PmsRepositories;
using IntranetPortal.Base.Repositories.SecurityRepositories;
using IntranetPortal.Base.Repositories.WspRepositories;
using IntranetPortal.Base.Services;
using IntranetPortal.Data.Repositories.AssetManagerRepositories;
using IntranetPortal.Data.Repositories.BamsRepositories;
using IntranetPortal.Data.Repositories.BaseRepositories;
using IntranetPortal.Data.Repositories.BusinessManagerRepositories;
using IntranetPortal.Data.Repositories.ClmRepositories;
using IntranetPortal.Data.Repositories.ContentManagerRepositories;
using IntranetPortal.Data.Repositories.ErmRepositories;
using IntranetPortal.Data.Repositories.GlobalSettingsRepositories;
using IntranetPortal.Data.Repositories.LmsRepositories;
using IntranetPortal.Data.Repositories.PmsRepositories;
using IntranetPortal.Data.Repositories.SecurityRepositories;
using IntranetPortal.Data.Repositories.WspRepositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Configurations
{
    public static class ServicesConfiguration
    {
        public static void ConfigureRepositories(this IServiceCollection services)
        {
            //======== User Administration Repositories =============//
            services.AddScoped<IPersonRepository, PersonRepository>();
            services.AddScoped<IUtilityRepository, UtilityRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IEmployeeUserRepository, EmployeeUserRepository>();
            services.AddScoped<IUserPermissionRepository, UserPermissionRepository>();
            services.AddScoped<IEntityPermissionRepository, EntityPermissionRepository>();

            //======= Content Management Repositories ===============//
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<IPostMediaRepository, PostMediaRepository>();

            //======= Global Settings Repositories ===================//
            services.AddScoped<ICurrencyRepository, CurrencyRepository>();
            services.AddScoped<ITeamRepository, TeamRepository>();
            services.AddScoped<ILocationRepository, LocationRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<IUnitRepository, UnitRepository>();
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<ILocationPermissionRepository, LocationPermissionRepository>();
            services.AddScoped<IPublicHolidayRepository, PublicHolidayRepository>();

            //======= Employee Records Management Repositories ========//
            services.AddScoped<IEmployeesRepository, EmployeesRepository>();
            services.AddScoped<IEmployeeSeparationRepository, EmployeeSeparationRepository>();
            services.AddScoped<IEmployeeSeparationOutstandingRepository, EmployeeSeparationOutstandingRepository>();
            services.AddScoped<IEmployeeOptionsRepository, EmployeeOptionsRepository>();

            //======== Asset Management System Repositories ===========//
            services.AddScoped<IAssetRepository, AssetRepository>();
            services.AddScoped<IAssetTypeRepository, AssetTypeRepository>();
            services.AddScoped<IAssetCategoryRepository, AssetCategoryRepository>();
            services.AddScoped<IAssetDivisionRepository, AssetDivisionRepository>();
            services.AddScoped<IAssetReservationRepository, AssetReservationRepository>();
            services.AddScoped<IAssetUsageRepository, AssetUsageRepository>();
            services.AddScoped<IAssetIncidentRepository, AssetIncidentRepository>();
            services.AddScoped<IAssetMaintenanceRepository, AssetMaintenanceRepository>();
            services.AddScoped<IAssetMovementRepository, AssetMovementRepository>();
            services.AddScoped<IBusinessRepository, BusinessRepository>();
            services.AddScoped<IBusinessContactRepository, BusinessContactRepository>();
            services.AddScoped<IAssignmentEventRepository, AssignmentEventRepository>();
            services.AddScoped<IAssignmentDeploymentRepository, AssignmentDeploymentRepository>();
            services.AddScoped<IBamsSettingsRepository, BamsSettingsRepository>();
            services.AddScoped<IDeploymentTeamMemberRepository, DeploymentTeamMemberRepository>();
            services.AddScoped<IDeploymentEquipmentRepository, DeploymentEquipmentRepository>();
            services.AddScoped<IAssignmentExtensionRepository, AssignmentExtensionRepository>();
            services.AddScoped<IAssignmentUpdatesRepository, AssignmentUpdatesRepository>();
            services.AddScoped<IEquipmentGroupsRepository, EquipmentGroupsRepository>();
            services.AddScoped<IAssetEquipmentGroupRepository, AssetEquipmentGroupRepository>();
            services.AddScoped<IUserLoginRepository, UserLoginRepository>();
            services.AddScoped<IAssetBinLocationRepository, AssetBinLocationRepository>();

            //======== Channels Workspaces System Repositories ===========//
            services.AddScoped<IAssetClassRepository, AssetClassRepository>();
            services.AddScoped<IAssetGroupRepository, AssetGroupRepository>();
            services.AddScoped<IProgramRepository, ProgramRepository>();
            services.AddScoped<IDeskspaceRepository, DeskspaceRepository>();
            
            //Performance Management System Repositories
            services.AddScoped<IPerformanceYearRepository, PerformanceYearRepository>();
            services.AddScoped<IPmsActivityHistoryRepository, PmsActivityHistoryRepository>();
            services.AddScoped<IPmsSystemRepository, PmsSystemRepository>();
            services.AddScoped<IReviewApprovalRepository, ReviewApprovalRepository>();
            services.AddScoped<IReviewCDGRepository, ReviewCDGRepository>();
            services.AddScoped<IReviewGradeRepository, ReviewGradeRepository>();
            services.AddScoped<IReviewHeaderRepository, ReviewHeaderRepository>();
            services.AddScoped<IReviewMessageRepository, ReviewMessageRepository>();
            services.AddScoped<IReviewMetricRepository, ReviewMetricRepository>();
            services.AddScoped<IReviewResultRepository, ReviewResultRepository>();
            services.AddScoped<IReviewSessionRepository, ReviewSessionRepository>();
            services.AddScoped<IReviewStageRepository, ReviewStageRepository>();
            services.AddScoped<IReviewSubmissionRepository, ReviewSubmissionRepository>();
            services.AddScoped<IAppraisalGradeRepository, AppraisalGradeRepository>();
            services.AddScoped<IApprovalRoleRepository, ApprovalRoleRepository>();
            services.AddScoped<ICompetencyRepository, CompetencyRepository>();
            services.AddScoped<IGradeHeaderRepository, GradeHeaderRepository>();
            services.AddScoped<ISessionScheduleRepository, SessionScheduleRepository>();
            services.AddScoped<IPerformanceSettingsRepository, PerformanceSettingsRepository>();
            
            //===== Channels Learning Management Repositories =====//
            services.AddScoped<ICourseTypeRepository, CourseTypeRepository>();
            services.AddScoped<ISubjectAreaRepository, SubjectAreaRepository>();
            services.AddScoped<ICourseRepository, CourseRepository>();
            services.AddScoped<ICourseContentRepository, CourseContentRepository>();

            //====== Leave Management Repositories ============//
            services.AddScoped<ILeaveTypesRepository, LeaveTypesRepository>();
            services.AddScoped<ILeaveProfileRepository, LeaveProfileRepository>();
            services.AddScoped<ILeaveProfileDetailRepository, LeaveProfileDetailRepository>();
            services.AddScoped<IEmployeeLeaveRepository, EmployeeLeaveRepository>();
        }

        public static void ConfigureServiceManagers(this IServiceCollection services)
        {
            services.AddScoped<ISecurityService, SecurityService>();
            services.AddScoped<IContentManagerService, ContentManagerService>();
            services.AddScoped<IGlobalSettingsService, GlobalSettingsService>();
            services.AddScoped<IBaseModelService, BaseModelService>();
            services.AddScoped<IAssetManagerService, AssetManagerService>();
            services.AddScoped<IBusinessManagerService, BusinessManagerService>();
            services.AddScoped<IBamsManagerService, BamsManagerService>();
            services.AddScoped<IErmService, ErmService>();
            services.AddScoped<IPerformanceService, PerformanceService>();
            services.AddScoped<IClmService, ClmService>();
            services.AddScoped<ILmsService, LmsService>();
            services.AddScoped<IWspService, WspService>();
        }

    }
}
