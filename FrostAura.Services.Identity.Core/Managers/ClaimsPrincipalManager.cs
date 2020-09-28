using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FrostAura.Services.Identity.Core.Managers
{
    /// <summary>
    /// Custom claims manager to allow for custom claims in a profile.
    /// </summary>
    public class ClaimsPrincipalManager : UserClaimsPrincipalFactory<IdentityUser>
    {
        /// <summary>
        /// Provide dependencies.
        /// </summary>
        /// <param name="userManager">User manager.</param>
        /// <param name="options">Identity options.</param>
        public ClaimsPrincipalManager(UserManager<IdentityUser> userManager, IOptions<IdentityOptions> options)
            :base(userManager, options)
        { }

        /// <summary>
        /// Build up custom claims identity.
        /// </summary>
        /// <param name="user">User context.</param>
        /// <returns>Custom claims identity.</returns>
        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(IdentityUser user)
        {
            var result = await base.GenerateClaimsAsync(user);

            result.AddClaim(new Claim(ClaimTypes.Name, user.Email));
            result.AddClaim(new Claim(ClaimTypes.GivenName, user.Email));
            result.AddClaim(new Claim(ClaimTypes.Email, user.Email));

            return result;
        }
    }
}
