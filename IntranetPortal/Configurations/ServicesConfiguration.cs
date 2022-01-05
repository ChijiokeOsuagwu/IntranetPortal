using IntranetPortal.Base.Repositories.BaseRepositories;
using IntranetPortal.Base.Repositories.ContentManagerRepositories;
using IntranetPortal.Base.Repositories.EmployeeRecordRepositories;
using IntranetPortal.Base.Repositories.GlobalSettingsRepositories;
using IntranetPortal.Base.Services;
using IntranetPortal.Data.Repositories.BaseRepositories;
using IntranetPortal.Data.Repositories.ContentManagerRepositories;
using IntranetPortal.Data.Repositories.EmployeeRecordRepositories;
using IntranetPortal.Data.Repositories.GlobalSettingsRepositories;
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
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IPersonRepository, PersonRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<IUnitRepository, UnitRepository>();
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<IUtilityRepository, UtilityRepository>();
        }

        public static void ConfigureServiceManagers(this IServiceCollection services)
        {
            services.AddScoped<IContentManagerService, ContentManagerService>();
            services.AddScoped<IGlobalSettingsService, GlobalSettingsService>();
            services.AddScoped<IEmployeeRecordService, EmployeeRecordService>();
            services.AddScoped<IBaseModelService, BaseModelService>();
        }

    }
}
