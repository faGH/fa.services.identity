using FrostAura.Services.Identity.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FrostAura.Services.Identity.Data
{
    /// <summary>
    /// Database context for entity framework that contains all identity tables.
    /// </summary>
    public class IdentityDbContext : Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityDbContext<FaUser>
    {
        /// <summary>
        /// Construct and allow for passing options.
        /// </summary>
        /// <param name="options">Db creation options.</param>
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
            : base(options)
        {
        }
    }
}
