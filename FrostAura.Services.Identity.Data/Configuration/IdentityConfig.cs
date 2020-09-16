using FrostAura.Libraries.Core.Extensions.Validation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

namespace FrostAura.Services.Identity.Data.Configuration
{
    /// <summary>
    /// Static identity configuration provider.
    /// </summary>
    public class IdentityConfig
    {

        /// <summary>
        /// Configure identity options.
        /// </summary>
        /// <param name="config">Identity options.</param>
        /// <param name="options">Identity options.</param>
        /// <returns>Identity options.</returns>
        public static IdentityOptions ConfigureIdentityOptions(IdentityOptions config, Data.Models.IdentityOptions options)
        {
            config.Password.RequiredLength = 4;
            config.Password.RequireDigit = false;
            config.Password.RequireNonAlphanumeric = false;
            config.Password.RequireUppercase = false;

            return config;
        }

        /// <summary>
        /// Configure cookie options.
        /// </summary>
        /// <param name="config">Cookie options.</param>
        /// <param name="options">Identity options.</param>
        /// <returns>Cookie options.</returns>
        public static CookieAuthenticationOptions ConfigureCookieOptions(CookieAuthenticationOptions config, Data.Models.IdentityOptions options)
        {
            options.ThrowIfNull(nameof(options));

            config.Cookie.Name = options.CookieName.ThrowIfNullOrWhitespace(nameof(options.CookieName));
            config.LoginPath = options.LoginEndpoint.ThrowIfNullOrWhitespace(nameof(options.LoginEndpoint));

            return config;
        }
    }
}
