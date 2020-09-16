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
    public sealed class IdentityDbContextDesignTimeFactory : IDesignTimeDbContextFactory<IdentityDbContext>
    {
        /// <summary>
        /// Factory method for producing the design time db context
        /// </summary>
        /// <param name="args"></param>
        /// <returns>Database context.</returns>
        public IdentityDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Migrations.json")
                .Build();
            var builder = new DbContextOptionsBuilder<IdentityDbContext>();

            string connectionString = configuration
                .GetConnectionString("IdentityDbConnection");

            builder.UseSqlServer(connectionString);

            Console.WriteLine($"Used connection string for identity db: {connectionString}");

            return new IdentityDbContext(builder.Options);
        }
    }
}
