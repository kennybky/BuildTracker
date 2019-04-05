
using BuildTrackerApi.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildTrackerApi.Services.Authorization
{
    public class DenyNotConfirmed : Attribute, IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            
            if (!context.Filters.Any(item => item is AllowNotConfirmed || item is AllowAnonymousFilter))
            {
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new ForbidResult();
                return;
            }
            try
            {
                  var confirmed =  context.HttpContext.User.HasClaim("AccountConfirmed", true.ToString());

                    //Make a call to database to ensure Consistency
                    if (!confirmed)
                    {

                        BuildTrackerContext dbContext = ServiceProviderServiceExtensions.GetRequiredService<BuildTrackerContext>(context.HttpContext.RequestServices);
                        var id = int.Parse(context.HttpContext.User.Identity?.Name);
                        var user = await dbContext.FindAsync<User>(id);
                        if (user == null)
                        {
                            context.Result = new UnauthorizedResult();
                            return;
                        }
                        else if (!user.AccountConfirmed)
                        {
                            var dic = new Dictionary<string, string>();
                            dic.Add("Reason", "Account not Confirmes");
                            dic.Add("AuthUrl", "api/users/confirm/" + user.Id);
                            AuthenticationProperties props = new AuthenticationProperties(dic);
                            var result = JsonConvert.SerializeObject(new { error = "Account not confirmed" });
                            await context.HttpContext.Response.WriteAsync(result);
                            context.Result = new ForbidResult(props);
                            return;
                        }
                    }
                }
            catch
            {
                context.Result = new ForbidResult();
                return;
            }
        } 
        }
    }
}