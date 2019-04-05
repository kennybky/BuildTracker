using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuildTrackerApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace BuildTrackerApi.Services.Authorization
{
    //
    // Summary:
    //     Specifies that the class or method that this attribute is applied to
    // allows users to access API without confirming account
    public class AllowNotConfirmed : Attribute, IAsyncAuthorizationFilter
    {
        public Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new ForbidResult();
            }
            return Task.CompletedTask;
        }

       
    }
}
