namespace MyHomework.WebApp.Authorizations
{
    using Microsoft.AspNetCore.Authorization;
    using Middlewares.WeChatAuthenticationMiddlewares;
    using System.Threading.Tasks;
    using System.Linq;
    using System;

    public class DepartmentHandler : AuthorizationHandler<DepartmentRequirements>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, DepartmentRequirements requirement)
        {
            if(context.User.HasClaim(c => c.Type == WeChatAuthenticationDefaults.ClaimType_Department))
            {
                var claim = context.User.Claims.First(c => c.Type == WeChatAuthenticationDefaults.ClaimType_Department);
                var departments = claim.Value;
                var departmentsArray = departments.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if(departmentsArray.Contains(requirement.DepartmentId))
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}
