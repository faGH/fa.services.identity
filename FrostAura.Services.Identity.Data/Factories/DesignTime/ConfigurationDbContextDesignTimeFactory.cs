using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace FrostAura.Services.Identity.Data.Factories.DesignTime
{
    /// <summary>
    /// DB context factory for running migrations in design time.
    /// This allows for running migrations in the .Data project independently.
    /// </summary>
    public sealed class ConfigurationDbContextDesignTimeFactory : IDesignTimeDbContextFactory<ConfigurationDbContext>
    {
        /// <summary>
        /// Factory method for producing the design time db context
        /// </summary>
        /// <param name="args"></param>
        /// <returns>Database context.</returns>
        public ConfigurationDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Development.json")
                .Build();
            var builder = new DbContextOptionsBuilder<IdentityServer4.EntityFramework.DbContexts.ConfigurationDbContext>();
            var connectionString = configuration
                .GetConnectionString("ConfigurationDbConnection");

            builder.UseSqlServer(connectionString);

            Console.WriteLine($"Used connection string for configuration db: {connectionString}");

            return new ConfigurationDbContext(builder.Options, new ConfigurationStoreOptions());
        }
    }
}
