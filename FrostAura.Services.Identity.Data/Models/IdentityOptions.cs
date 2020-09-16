using System.Collections.Generic;

namespace FrostAura.Services.Identity.Data.Models
{
    /// <summary>
    /// FrostAura application options model.
    /// </summary>
    public class IdentityOptions
    {
        /// <summary>
        /// Cookie name to use for the identity server.
        /// </summary>
        public string CookieName { get; set; }
        /// <summary>
        /// Login endpoint to use for the identity server.
        /// </summary>
        public string LoginEndpoint { get; set; }
        /// <summary>
        /// List of users to seed.
        /// </summary>
        public List<UserToSeed> SeedUsers { get; set; } = new List<UserToSeed>();
    }
}
