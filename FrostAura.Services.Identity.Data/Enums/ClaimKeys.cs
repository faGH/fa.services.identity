namespace FrostAura.Services.Identity.Data.Enums
{
    /// <summary>
    /// Collection of custom claim keys.
    /// </summary>
    public class ClaimKeys
    {
        /// <summary>
        /// Claim for clients to override the CSS to use in their sign in flow.
        /// </summary>
        public const string FA_CLIENT_CUSTOM_CSS_URL = "FrostAura.Clients.CustomCssUrl";
        /// <summary>
        /// Claim for clients to override the logo to use in their sign in flow.
        /// </summary>
        public const string FA_CLIENT_CUSTOM_LOGO_SVG_URL = "FrostAura.Clients.CustomLogoSvgUrl";
        /// <summary>
        /// Claim for clients to override the name to use in their sign in flow.
        /// </summary>
        public const string FA_CLIENT_NAME = "FrostAura.Clients.Name";
    }
}
