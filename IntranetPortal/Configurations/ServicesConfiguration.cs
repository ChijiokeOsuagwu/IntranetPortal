using IntranetPortal.Base.Repositories.AssetManagerRepositories;
using IntranetPortal.Base.Repositories.BamsRepositories;
using IntranetPortal.Base.Repositories.BaseRepositories;
using IntranetPortal.Base.Repositories.BusinessManagerRepositories;
using IntranetPortal.Base.Repositories.ContentManagerRepositories;
using IntranetPortal.Base.Repositories.ErmRepository;
using IntranetPortal.Base.Repositories.GlobalSettingsRepositories;
using IntranetPortal.Base.Repositories.SecurityRepositories;
using IntranetPortal.Base.Services;
using IntranetPortal.Data.Repositories.AssetManagerRepositories;
using IntranetPortal.Data.Repositories.BamsRepositories;
using IntranetPortal.Data.Repositories.BaseRepositories;
using IntranetPortal.Data.Repositories.BusinessManagerRepositories;
using IntranetPortal.Data.Repositories.ContentManagerRepositories;
using IntranetPortal.Data.Repositories.ErmRepositories;
using IntranetPortal.Data.Repositories.GlobalSettingsRepositories;
using IntranetPortal.Data.Repositories.SecurityRepositories;
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
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<ILocationRepository, LocationRepository>();
            services.AddScoped<IEmployeesRepository, EmployeesRepository>();
            services.AddScoped<IPersonRepository, PersonRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<IUnitRepository, UnitRepository>();
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<IUtilityRepository, UtilityRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IEmployeeUserRepository, EmployeeUserRepository>();
            services.AddScoped<IUserPermissionRepository, UserPermissionRepository>();
            services.AddScoped<ITeamRepository, TeamRepository>();
            services.AddScoped<IAssetRepository, AssetRepository>();
            services.AddScoped<IAssetTypeRepository, AssetTypeRepository>();
            services.AddScoped<IAssetCategoryRepository, AssetCategoryRepository>();
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
        }

    }
}
