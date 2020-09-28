using FrostAura.Services.Identity.Data.Enums;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace FrostAura.Services.Identity.Models
{
    /// <summary>
    /// Login request structure.
    /// </summary>
    public class LoginViewModel
    {
        /// <summary>
        /// User email address.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "A valid email address is required.")]
        public string Email { get; set; }
        /// <summary>
        /// Account password.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "A valid password is required.")]
        public string Password { get; set; }
        /// <summary>
        /// OpenId Connect return URL.
        /// </summary>
        public string ReturnUrl { get; set; }
        /// <summary>
        /// Client app name to use.
        /// </summary>
        public string ClientName { get; set; } = "FrostAura Identity";
        /// <summary>
        /// Client app icon to use.
        /// </summary>
        public string ClientIcon { get; set; } = "/vectors/icons/fa.client.logo.svg";
        /// <summary>
        /// Client app icon to use.
        /// </summary>
        public string ClientCustomStyleSheetUrl { get; set; }

        /// <summary>
        /// Resovle the client claims, if any, from the configuration database.
        /// </summary>
        /// <param name="configurationDbContext">Configuration database.</param>
        /// <returns>Chainable this instance.</returns>
        public LoginViewModel SetClientClaimsFromConfigDb(Data.ConfigurationDbContext configurationDbContext)
        {
            var clientContext = GetClientFromReturnUrl(ReturnUrl, configurationDbContext);
            var claims = clientContext?
                .Claims
                .Select(c => new ClientClaim
                {
                    Type = c.Type,
                    Value = c.Value
                })
                .ToList();

            if (!claims.Any()) return this;

            ClientName = claims
                .FirstOrDefault(c => c.Type == ClaimKeys.FA_CLIENT_NAME)?
                .Value ?? ClientName;
            ClientIcon = claims
                .FirstOrDefault(c => c.Type == ClaimKeys.FA_CLIENT_CUSTOM_LOGO_SVG_URL)?
                .Value ?? ClientIcon;
            ClientCustomStyleSheetUrl = claims
                .FirstOrDefault(c => c.Type == ClaimKeys.FA_CLIENT_CUSTOM_CSS_URL)?
                .Value ?? ClientCustomStyleSheetUrl;

            return this;
        }


        /// <summary>
        /// Attempt to get the client context for a given return url.
        /// </summary>
        /// <param name="returnUrl">Given return url.</param>
        /// <param name="configurationDbContext">Configuration database.</param>
        /// <returns>Client context, if any.</returns>
        private IdentityServer4.EntityFramework.Entities.Client GetClientFromReturnUrl(string returnUrl, Data.ConfigurationDbContext configurationDbContext)
        {
            if (string.IsNullOrWhiteSpace(returnUrl)) return default;

            var clientId = returnUrl
                .Split("client_id=")
                .Last()
                .Split("&")
                .First();
            var client = configurationDbContext
                .Clients
                .Include(c => c.RedirectUris)
                .Include(c => c.Claims)
                .FirstOrDefault(c => c.ClientId == clientId);

            return client;
        }
    }
}
