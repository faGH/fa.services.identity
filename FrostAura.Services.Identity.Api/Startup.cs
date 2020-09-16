using FrostAura.Libraries.Core.Extensions.Validation;
using FrostAura.Services.Identity.Api.Configuration;
using FrostAura.Services.Identity.Data;
using FrostAura.Services.Identity.Data.Configuration;
using FrostAura.Services.Identity.Data.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace FrostAura.Services.Identity.Api
{
    /// <summary>
    /// Startup class for the application.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Application configuration.
        /// </summary>
        private readonly IConfiguration _configuration;
        /// <summary>
        /// Application logger.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Constructor to allow for apssing dependencies.
        /// </summary>
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration.ThrowIfNull(nameof(configuration));
        }

        /// <summary>
        /// Configure dependent serices.
        /// </summary>
        /// <param name="services">Application service collection.</param>
        /// <param name="configuration">Application configuration.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            var identitySection = _configuration.GetSection("FrostAura:Identity");
            var identityOptions = identitySection.Get<Data.Models.IdentityOptions>();
            var operationalDbConnectionString = _configuration.GetConnectionString("OperationalDbConnection");
            var configDbConnectionString = _configuration.GetConnectionString("ConfigurationDbConnection");
            var identityDbConnectionString = _configuration.GetConnectionString("IdentityDbConnection");
            var migrationsAssembly = typeof(IdentityDbContext).GetTypeInfo().Assembly.GetName().Name;

            _logger?.LogDebug($"Operational DB connection: '{operationalDbConnectionString}'");
            _logger?.LogDebug($"Configuration DB connection: '{configDbConnectionString}'");

            services.Configure<Data.Models.IdentityOptions>(identitySection);
            services.AddDbContext<Data.IdentityDbContext>(config =>
            {
                config.UseSqlServer(identityDbConnectionString);
            });
            services.AddDbContext<Data.PersistedGrantDbContext>(config =>
            {
                config.UseSqlServer(operationalDbConnectionString);
            });
            services.AddDbContext<Data.ConfigurationDbContext>(config =>
            {
                config.UseSqlServer(configDbConnectionString);
            });
            services
                .AddIdentity<IdentityUser, IdentityRole>(config => IdentityConfig.ConfigureIdentityOptions(config, identityOptions))
                .AddEntityFrameworkStores<IdentityDbContext>()
                .AddDefaultTokenProviders();
            services.ConfigureApplicationCookie(config => IdentityConfig.ConfigureCookieOptions(config, identityOptions));
            services
                .AddIdentityServer()
                .AddAspNetIdentity<IdentityUser>()
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseSqlServer(configDbConnectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseSqlServer(operationalDbConnectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                    options.EnableTokenCleanup = true;
                })
                .AddDeveloperSigningCredential();
            services.AddControllersWithViews();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">Application builder.</param>
        /// <param name="env">Environment context.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.InitializeDatabases<Startup>();
            app.UseRouting();
            app.UseIdentityServer();
            app.UseStaticFiles();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
