using System.Threading.Tasks;
using southosting.Data;
using southosting.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace southosting.Authorization
{
    public class UserIsAdminHandler : AuthorizationHandler<OperationAuthorizationRequirement, Advert>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                       OperationAuthorizationRequirement requirement,
                                                       Advert resource)
        {
            if (context.User == null || resource == null) return Task.CompletedTask;

            // Managers can approve or reject.
            if (context.User.IsInRole(Constants.AdministratorRole))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}