using BuildTrackerApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BuildTrackerApi.Services.Authorization
{
    public class UserAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, User>
    {
        private readonly BuildTrackerContext _context;

        public UserAuthorizationHandler(BuildTrackerContext context)
        {
            _context = context;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, User resource)
        {
            if (context.User.IsInRole(Role.ADMIN.ToString()))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
            if(requirement.Name == Operations.Read.Name || requirement.Name == Operations.Update.Name)
            {
                CanModifyOrReadUser(context, requirement, resource);
            }
            return Task.CompletedTask;
        }

        private void CanModifyOrReadUser(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, User resource)
        {
            if(IsUser(context.User, resource))
            {
                context.Succeed(requirement);
            }
        }

        private bool IsUser(ClaimsPrincipal user, User resource)
        {
            return int.Parse(user.Identity.Name) == resource.Id;
        }
    }
}
