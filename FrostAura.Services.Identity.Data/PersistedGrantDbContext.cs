using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;

namespace FrostAura.Services.Identity.Data
{
    /// <summary>
    /// Database context for entity framework that contains all configuration tables.
    /// </summary>
    public class PersistedGrantDbContext : IdentityServer4.EntityFramework.DbContexts.PersistedGrantDbContext
    {
        /// <summary>
        /// Construct and allow for passing options.
        /// </summary>
        /// <param name="options">Db creation options.</param>
        public PersistedGrantDbContext(DbContextOptions<IdentityServer4.EntityFramework.DbContexts.PersistedGrantDbContext> options, OperationalStoreOptions storeOptions)
            : base(options, storeOptions)
        {}
    }
}
