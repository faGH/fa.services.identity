using Microsoft.AspNetCore.Identity;

namespace FrostAura.Services.Identity.Data.Models
{
    /// <summary>
    /// FrostAura application options model.
    /// </summary>
    public class UserToSeed : FaUser
    {
        /// <summary>
        /// Account password.
        /// </summary>
        public string Password { get; set; }
    }
}
