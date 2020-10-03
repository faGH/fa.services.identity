using FrostAura.Libraries.Core.Extensions.Validation;
using FrostAura.Services.Identity.Api.Configuration;
using FrostAura.Services.Identity.Data.Resources;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FrostAura.Services.Identity.Data.Extensions
{
    /// <summary>
    /// Application builder extensions.
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Initialize database context sync.
        /// </summary>
        /// <param name="app">Application builder.</param>
        /// <returns>Application builder.</returns>
        public static IApplicationBuilder UseFrostAuraResources<TCaller>(this IApplicationBuilder app)
        {
            var RESILIENT_ALLOWED_ATTEMPTS = 3;
            var RESILIENT_BACKOFF = TimeSpan.FromSeconds(5);

            for (int i = 1; i <= RESILIENT_ALLOWED_ATTEMPTS; i++)
            {
                try
                {
                    app = InitializeDatabasesAsync<TCaller>(app).GetAwaiter().GetResult();
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Database migration failed on try {i}.");
                    Thread.Sleep(RESILIENT_BACKOFF);
                }
            }

            return app;
        }

        /// <summary>
        /// Set signing credentials.
        /// </summary>
        /// <param name="app">Identity server builder.</param>
        /// <param name="configuration">App configuration.</param>
        /// <returns>Identity server builder.</returns>
        public static IIdentityServerBuilder AddFrostAuraSigningCredentials(this IIdentityServerBuilder app, IConfiguration configuration)
        {
            var expirationInDays = configuration.GetValue<int>("FrostAura:Identity:Jwt:ExpirationDays");
            var keyFileName = configuration.GetValue<string>("FrostAura:Identity:Jwt:KeyFilePath");
            var rsaResource = new RsaResource(TimeSpan.FromDays(expirationInDays), keyFileName);
            var key = rsaResource.GetKey();
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256)
            {
                CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }
            };

            return app
                .AddSigningCredential(signingCredentials);
        }

        /// <summary>
        /// Initialize database context async.
        /// </summary>
        /// <param name="app">Application builder.</param>
        /// <returns>Application builder.</returns>
        private static async Task<IApplicationBuilder> InitializeDatabasesAsync<TCaller>(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var logger = serviceScope
                    .ServiceProvider
                    .GetRequiredService<ILogger<TCaller>>()
                    .ThrowIfNull("Logger");

                // PersistedGrantDbContext
                var persistedGrantDbContext = serviceScope
                    .ServiceProvider
                    .GetRequiredService<FrostAura.Services.Identity.Data.PersistedGrantDbContext>();

                logger.LogInformation($"Migrating database '{nameof(persistedGrantDbContext)}' => '{persistedGrantDbContext.Database.GetDbConnection().ConnectionString}'.");

                persistedGrantDbContext
                    .Database
                    .Migrate();

                // ConfigurationDbContext
                var configDbContext = serviceScope
                    .ServiceProvider
                    .GetRequiredService<FrostAura.Services.Identity.Data.ConfigurationDbContext>();

                logger.LogInformation($"Migrating database '{nameof(configDbContext)}' => '{configDbContext.Database.GetDbConnection().ConnectionString}'.");

                configDbContext
                    .Database
                    .Migrate();

                var identityOptions = serviceScope
                    .ServiceProvider
                    .GetService<IOptions<Models.IdentityOptions>>()
                    .ThrowIfNull("identityOptions")
                    .Value
                    .ThrowIfNull("Value");

                // Seed clients.
                foreach (var seedClient in SeedingConfig
                    .GetStaticClients()
                    .Where(c => !(configDbContext.Clients.Any(nc => nc.ClientId == c.ClientId))))
                {
                    logger.LogInformation($"Seeding client '{seedClient.ClientId}.");

                    configDbContext.Clients.Add(new IdentityServer4.EntityFramework.Entities.Client
                    {
                        ClientId = seedClient.ClientId,
                        ClientSecrets = seedClient
                            .ClientSecrets
                            .Select(s => new IdentityServer4.EntityFramework.Entities.ClientSecret
                            {
                                Value = s.Value
                            })
                            .ToList(),
                        AllowedGrantTypes = seedClient
                            .AllowedGrantTypes
                            .Select(gt => new IdentityServer4.EntityFramework.Entities.ClientGrantType
                            {
                                GrantType = gt
                            })
                            .ToList(),
                        AllowedScopes = seedClient
                            .AllowedScopes
                            .Select(asc => new IdentityServer4.EntityFramework.Entities.ClientScope
                            {
                                Scope = asc
                            })
                            .ToList(),
                        RedirectUris = seedClient
                            .RedirectUris
                            .Select(ruri => new IdentityServer4.EntityFramework.Entities.ClientRedirectUri
                            {
                                RedirectUri = ruri
                            })
                            .ToList(),
                        Claims = seedClient
                            .Claims
                            .Select(c => new IdentityServer4.EntityFramework.Entities.ClientClaim
                            {
                                Type = c.Type,
                                Value = c.Value
                            })
                            .ToList(),
                        RequireConsent = seedClient.RequireConsent,
                        AlwaysIncludeUserClaimsInIdToken = seedClient.AlwaysIncludeUserClaimsInIdToken
                    });
                }

                await configDbContext.SaveChangesAsync();

                // Seed identity resources.
                foreach (var idr in SeedingConfig
                    .GetStaticIdentityResources()
                    .Where(rid => !(configDbContext.IdentityResources.Any(nrid => nrid.Name == rid.Name))))
                {
                    logger.LogInformation($"Seeding identity resource '{idr.Name}.");

                    configDbContext.IdentityResources.Add(new IdentityServer4.EntityFramework.Entities.IdentityResource
                    {
                        Name = idr.Name,
                        UserClaims = idr
                            .UserClaims
                            .Select(uc => new IdentityServer4.EntityFramework.Entities.IdentityResourceClaim
                            {
                                Type = uc
                            })
                            .ToList()
                    });
                }

                await configDbContext.SaveChangesAsync();

                // Seed api scopes.
                foreach (var apiScope in SeedingConfig
                    .GetStaticApiScopeResources()
                    .Where(s => !(configDbContext.ApiScopes.Any(ns => ns.Name == s.Name))))
                {
                    logger.LogInformation($"Seeding api scope '{apiScope.Name}.");

                    configDbContext.ApiScopes.Add(new IdentityServer4.EntityFramework.Entities.ApiScope
                    {
                        Name = apiScope.Name,
                        UserClaims = apiScope
                            .UserClaims
                            .Select(uc => new IdentityServer4.EntityFramework.Entities.ApiScopeClaim
                            {
                                Type = uc
                            })
                            .ToList()
                    });
                }

                await configDbContext.SaveChangesAsync();

                // Seed apis.
                foreach (var api in SeedingConfig
                    .GetStaticApiResources()
                    .Where(s => !(configDbContext.ApiResources.Any(ns => ns.Name == s.Name))))
                {
                    logger.LogInformation($"Seeding api resource '{api.Name}.");

                    configDbContext.ApiResources.Add(new IdentityServer4.EntityFramework.Entities.ApiResource
                    {
                        Name = api.Name,
                        Description = api.Description,
                        Scopes = api
                            .Scopes
                            .Select(s => new IdentityServer4.EntityFramework.Entities.ApiResourceScope
                            {
                                Scope = s
                            })
                            .ToList(),
                        UserClaims = api
                            .UserClaims
                            .Select(uc => new IdentityServer4.EntityFramework.Entities.ApiResourceClaim
                            {
                                Type = uc
                            })
                            .ToList()
                    });
                }

                await configDbContext.SaveChangesAsync();

                // IdentityDbContext
                var identityDbContext = serviceScope
                    .ServiceProvider
                    .GetRequiredService<FrostAura.Services.Identity.Data.IdentityDbContext>();

                logger.LogInformation($"Migrating database '{nameof(identityDbContext)}' => '{identityDbContext.Database.GetDbConnection().ConnectionString}'.");

                identityDbContext
                    .Database
                    .Migrate();
            }

            await SeedIdentityDatabase<TCaller>(app);

            return app;
        }

        /// <summary>
        /// Seed the identity database.
        /// </summary>
        /// <param name="serviceProvider">Application services provider.</param>
        /// <returns>Application services provider.</returns>
        public static async Task SeedIdentityDatabase<TCaller>(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var logger = serviceScope
                    .ServiceProvider
                    .GetRequiredService<ILogger<TCaller>>()
                    .ThrowIfNull("Logger");
                var identityConfiguration = serviceScope
                    .ServiceProvider
                    .GetService<IOptions<Data.Models.IdentityOptions>>()
                    .ThrowIfNull("identityConfiguration")
                    .Value
                    .ThrowIfNull("Value");
                var userManager = serviceScope
                    .ServiceProvider
                    .GetRequiredService<UserManager<IdentityUser>>()
                    .ThrowIfNull("userManager");

                if (!identityConfiguration.SeedUsers.Any())
                {
                    logger.LogInformation($"No user accounts to seed were found in the configuration.");

                    return;
                }

                logger.LogInformation($"Starting the seeding of {identityConfiguration.SeedUsers.Count} user accounts from the configuration. Accounts that already exist will be skipped.");

                foreach (var userToSeed in identityConfiguration
                    .SeedUsers
                    .Where(u => !userManager.Users.Any(dbu => dbu.UserName == u.UserName)))
                {
                    userToSeed.Email = userToSeed.UserName;

                    logger.LogInformation($"Seeding user '{userToSeed.UserName}'.");

                    // Task.WhenAll causes threading issues with the dbcontext.
                    await userManager.CreateAsync(userToSeed, userToSeed.Password);
                }

                logger.LogInformation("Seeding of user accounts succeeded.");
            }
        }
    }
}
