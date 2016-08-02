namespace MyHomework.WebApp.Middlewares
{
    using Authorizations;
    using Basic;
    using Configurations;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using System.Security.Claims;
    using WeChat;

    public static class AuthorizationExtensions
    {
        public static IServiceCollection UseCustomAuthorization(this IServiceCollection services, IOptions<AppOptions> appOptions)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(Globals.AuthorizePolicySystemAdmin, 
                    policy => policy.RequireClaim(ClaimTypes.Role, UserType.Admin.ToString(), UserType.Creator.ToString(), UserType.ExternalSystemAdmin.ToString(), UserType.InternalSystemAdmin.ToString()));
                options.AddPolicy(Globals.AuthorizePolicyHomeworkPublisher,
                    policy =>
                    {
                        //TOTO load deparment id from configuration
                        policy.Requirements.Add(new DepartmentRequirements(appOptions.Value.WeChatOptions.HomeworkPublisherDepartmentId));
                    });
                options.AddPolicy(Globals.AuthorizePolicyMember,
                    policy => policy.RequireAuthenticatedUser());

            });

            services.AddSingleton<IAuthorizationHandler, DepartmentHandler>();

            return services;
        }
    }
}
