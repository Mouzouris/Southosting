using System.Threading.Tasks;
using southosting.Data;
using southosting.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace southosting.Authorization
{
    public class IsAdvertOwnerHandler
        : AuthorizationHandler<OperationAuthorizationRequirement, Advert>
    {
        UserManager<SouthostingUser> _userManager;

        public IsAdvertOwnerHandler(UserManager<SouthostingUser> userManager)
        {
            _userManager = userManager;
        }


        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                       OperationAuthorizationRequirement requirement,
                                                       Advert resource)
        {
            if (context.User == null || resource == null) {
                return Task.CompletedTask;
            }
            
            if (resource.LandlordID == _userManager.GetUserId(context.User))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}