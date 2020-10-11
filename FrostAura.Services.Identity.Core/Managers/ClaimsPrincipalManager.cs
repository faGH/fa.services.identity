using FrostAura.Services.Identity.Data.Models;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityOptions = Microsoft.AspNetCore.Identity.IdentityOptions;

namespace FrostAura.Services.Identity.Core.Managers
{
    /// <summary>
    /// Custom claims manager to allow for custom claims in a profile.
    /// </summary>
    public class ClaimsPrincipalManager : UserClaimsPrincipalFactory<FaUser>
    {
        /// <summary>
        /// Provide dependencies.
        /// </summary>
        /// <param name="userManager">User manager.</param>
        /// <param name="options">Identity options.</param>
        public ClaimsPrincipalManager(UserManager<FaUser> userManager, IOptions<IdentityOptions> options)
            :base(userManager, options)
        { }

        /// <summary>
        /// Build up custom claims identity.
        /// </summary>
        /// <param name="user">User context.</param>
        /// <returns>Custom claims identity.</returns>
        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(FaUser user)
        {
            var result = await base.GenerateClaimsAsync(user);

            // Add all FrostAura expected claims.
            result.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
            result.AddClaim(new Claim(ClaimTypes.GivenName, user.UserName));
            result.AddClaim(new Claim(ClaimTypes.Email, user.UserName));
            result.AddClaim(new Claim(ClaimTypes.Surname, user.UserName));
            result.AddClaim(new Claim(JwtClaimTypes.Picture, "https://placehold.it/256x256"));

            return result;
        }
    }
}
