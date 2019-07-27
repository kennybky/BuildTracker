using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BuildTrackerApi.Helpers;
using BuildTrackerApi.Middleware;
using BuildTrackerApi.Models;
using BuildTrackerApi.Services;
using BuildTrackerApi.Services.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;

namespace BuildTrackerApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddDbContext<BuildTrackerContext>();


            services.AddMvc()
                .AddJsonOptions(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore)
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddAutoMapper();

            // configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            // configure jwt authentication
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
           
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
                        var userId = int.Parse(context.Principal.Identity.Name);
                        var user = userService.GetById(userId);
                        if (user == null)
                        {
                            // return unauthorized if user no longer exists
                            context.Fail("User No Longer Exists");
                        }
                        return Task.CompletedTask;
                    }

                };
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddDefaultIdentity<User>()
       .AddEntityFrameworkStores<BuildTrackerContext>();

            services.AddDbContext<BuildTrackerContext>(options =>
        options.UseSqlServer(appSettings.ConnectionString));

           

            // configure DI for application services
            services.AddScoped<IUserService, UserService>();




            services.AddScoped<IAuthorizationHandler, UserAuthorizationHandler>();

      


            services.Configure<IdentityOptions>(options =>
            {
                // Password Requirements
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;
                options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultProvider;

                // Default Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info() { Title = "Build Tracker API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseCors(x => x
              .AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());




            app.UseAuthentication();



            app.UseMiddleware<RestExceptionHandler>();

            app.UseDefaultFiles();

            app.UseStaticFiles();

            //app.UseHttpsRedirection();
            app.UseMvc(routes=>
            {
                routes.MapSpaFallbackRoute()
            });
            

            AddSampleData(serviceProvider).Wait();

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("v1/swagger.json", "Build Tracker API v1"));
        }

        public async Task AddSampleData(IServiceProvider serviceProvider)
        {
            var user = new User()
            {
                UserName = "johndoe",
                FirstName = "John",
                LastName = "Doe",
                Role = Role.ADMIN,
                Email = "johndoe@email.com",
                LockoutEnabled = true,
                AccountConfirmed = true
            };
            var userService = serviceProvider.GetRequiredService<IUserService>();
            try
            {
                if((await userService.GetByUserName(user.UserName)) == null)
                {
                    var result = await userService.Create(user, "P@ssw0rd");
                }
            } catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }

        }
    }
}
