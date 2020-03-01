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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

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


            services.AddControllersWithViews().
                AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore)
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddRazorPages();

            services.AddAutoMapper(typeof(AutoMapperProfile));

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
                .AddRoles<AppRole>()
                .AddEntityFrameworkStores<BuildTrackerContext>();

            services.AddDbContext<BuildTrackerContext>(options =>
        options.UseSqlServer(appSettings.ConnectionString));

           

            // configure DI for application services
            services.AddTransient<IUserService, UserService>();




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
                c.SwaggerDoc("v1", new OpenApiInfo() { Title = "Build Tracker API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
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




            

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<RestExceptionHandler>();

            app.UseDefaultFiles();

            app.UseStaticFiles();

            //app.UseHttpsRedirection();
            app.UseEndpoints(configure =>
            {
                configure.MapControllers();
                configure.MapRazorPages();
            });

            InitializeRoles(serviceProvider).Wait();
            AddSampleData(serviceProvider).Wait();

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("v1/swagger.json", "Build Tracker API v1"));
        }

        private async Task InitializeRoles(IServiceProvider serviceProvider)
        {
            RoleManager<AppRole> roleManager = serviceProvider.GetRequiredService<RoleManager<AppRole>>();
            IEnumerable<AppRole> roles = Models.ModelExtensions.GetRoles();
            foreach(var role in roles)
            {
                if (!(await roleManager.RoleExistsAsync(role.Name)))
                    await roleManager.CreateAsync(role);
            }
        }

        public async Task AddSampleData(IServiceProvider serviceProvider)
        {
            var user = new User()
            {
                UserName = "johndoe",
                FirstName = "John",
                LastName = "Doe",
                //Role = Role.ADMIN,
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
                    if (result != null)
                    {
                        await userService.AddRole(result, Role.ADMIN);
                    }
                }
            } catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }

        }
    }
}
