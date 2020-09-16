
using IdentityServer4.EntityFramework.Extensions;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;

namespace FrostAura.Services.Identity.Data
{
    /// <summary>
    /// Database context for entity framework that contains all configuration tables.
    /// </summary>
    public class ConfigurationDbContext : IdentityServer4.EntityFramework.DbContexts.ConfigurationDbContext
    {
        /// <summary>
        /// Store options.
        /// </summary>
        private readonly ConfigurationStoreOptions _storeOptions;

        /// <summary>
        /// Construct and allow for passing options.
        /// </summary>
        /// <param name="options">Db creation options.</param>
        public ConfigurationDbContext(DbContextOptions<IdentityServer4.EntityFramework.DbContexts.ConfigurationDbContext> options, ConfigurationStoreOptions storeOptions)
            : base(options, storeOptions)
        {
            _storeOptions = storeOptions;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ConfigureClientContext(_storeOptions);
            modelBuilder.ConfigureResourcesContext(_storeOptions);

            base.OnModelCreating(modelBuilder);
        }
    }
}
