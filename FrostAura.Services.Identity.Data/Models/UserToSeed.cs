using Microsoft.AspNetCore.Identity;

namespace FrostAura.Services.Identity.Data.Models
{
    /// <summary>
    /// FrostAura application options model.
    /// </summary>
    public class UserToSeed : IdentityUser
    {
        /// <summary>
        /// Account password.
        /// </summary>
        public string Password { get; set; }
    }
}
