using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using southosting.Logic;
using Microsoft.EntityFrameworkCore;
using southosting.Models;
using southosting.Data;
using southosting.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using MySql.Data.EntityFrameworkCore;
using MySql.Data.EntityFrameworkCore.Extensions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.ResponseCompression;



namespace southosting
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;

            if (env == null) throw new ArgumentNullException(nameof(env));
            Environment = env;
        }

        public IConfiguration Configuration { get; }

        private IHostingEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddOptions();
            services.Configure<BlobStorage>(Configuration.GetSection("BlobStorage"));

            services.AddMvc(config => {
                var policy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // use lowercase routes
            services.AddRouting(options => 
            {
                options.LowercaseUrls = true;
            });

            services.Configure<AppSettings>(Configuration);

            // setup relevant database
            services.AddDbContext<SouthostingContext>(options =>
            {
               // options.UseMySQL(Configuration.GetConnectionString("defaultConnection"));
                options.UseSqlServer(Configuration.GetConnectionString("defaultConnection"));
            });

            services.AddAuthentication();
            services.AddAuthorization(options => 
            {
                options.AddPolicy("ElevatedRights", policy => policy.RequireRole(Constants.AdministratorRole,
                                                                                 Constants.LandlordRole,
                                                                                 Constants.AccommodationOfficerRole));
            });

            services.AddIdentity<SouthostingUser, IdentityRole>()
                    .AddEntityFrameworkStores<SouthostingContext>()
                    .AddDefaultUI()
                    .AddDefaultTokenProviders();

            // auth handlers
            services.AddScoped<IAuthorizationHandler, IsAdvertOwnerHandler>();
            services.AddSingleton<IAuthorizationHandler, IsNotStudentHandler>();

            // for seeding data from https://randomuser.me
            services.AddHttpClient();

            // enable text compression
            services.Configure<GzipCompressionProviderOptions>(options => options.Level = System.IO.Compression.CompressionLevel.Fastest);
            services.AddResponseCompression();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseResponseCompression();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error/Error");
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
