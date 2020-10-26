using FrostAura.Services.Identity.Data.Enums;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;
using System.Security.Claims;

namespace FrostAura.Services.Identity.Api.Configuration
{
    /// <summary>
    /// Static identity configuration provider.
    /// </summary>
    public class SeedingConfig
    {
        /// <summary>
        /// Collection of static identity api resources.
        /// 
        /// NOTE: APIs are resources that are accessible by other authenticated clients.
        /// If the resource at hand is in need of authenticating itself in order to speak to another resource, there needs to be a client defined for this.
        /// </summary>
        /// <returns>Collection of static identity api resources.</returns>
        public static IEnumerable<ApiResource> GetStaticApiResources()
        {
            return new List<ApiResource>
            {
                // Integration test API resource. We need this since this API speaks to itself (pretends to be another API and so requires auth).
                new ApiResource("FrostAura.Services.Identity.Tests.Integration")
                {
                    Scopes = new List<string>
                    {
                        "frostaura.scopes.api.devices"
                    }
                },
                new ApiResource("FrostAura.Services.Devices")
                {
                    Scopes = new List<string>
                    {
                        "frostaura.scopes.api.devices"
                    }
                }
            };
        }

        /// <summary>
        /// Collection of static identity api scopes.
        /// </summary>
        /// <returns>Collection of static identity api scopes.</returns>
        public static IEnumerable<ApiScope> GetStaticApiScopeResources()
        {
            return new List<ApiScope>
            {
                // Basic connection scope.
                new ApiScope("frostaura.scopes.api.devices", "Default FrostAura devices API scope.")
            };
        }

        /// <summary>
        /// Collection of static identity resources.
        /// </summary>
        /// <returns>Collection of static identity resources.</returns>
        public static IEnumerable<IdentityResource> GetStaticIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResource
                {
                    Name = "frostaura.scopes.default",
                    UserClaims = new List<string>
                    {
                        ClaimTypes.Name,
                        ClaimTypes.Email,
                        ClaimTypes.Surname,
                        JwtClaimTypes.Picture,
                        "offline_access"
                    }
                }
            };
        }

        /// <summary>
        /// Collection of static identity clients.
        /// </summary>
        /// <returns>Collection of static identity clients.</returns>
        public static IEnumerable<Client> GetStaticClients()
        {
            return new List<Client>
            {
                // Integration test client to allow that API to authenticate using an id and password.
                new Client
                {
                    ClientId = "FrostAura.Services.Identity.Api.Tests.Integration",
                    ClientSecrets = { new Secret("Password1234$".ToSha256()) },
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes =
                    {
                        "frostaura.scopes.default"
                    },
                    RequireConsent = false
                },
                // Auth flow test client to allow that app to sign in using the OpenIdConnect / OAuth2 flow.
                new Client
                {
                    ClientId = "FrostAura.Services.Identity.Api.Tests.AuthFlow",
                    ClientSecrets = { new Secret("Password1234$".ToSha256()) },
                    AllowedGrantTypes = GrantTypes.Code,
                    AllowedScopes =
                    {
                        "frostaura.scopes.default",
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile
                    },
                    Claims = new List<ClientClaim>
                    {
                        new ClientClaim(ClaimKeys.FA_CLIENT_CUSTOM_CSS_URL, "/css/layout.custom.demo.css"),
                        new ClientClaim(ClaimKeys.FA_CLIENT_CUSTOM_LOGO_SVG_URL, "https://freepik.cdnpk.net/img/avatars/01.svg"),
                        new ClientClaim(ClaimKeys.FA_CLIENT_NAME, "Auth Flow Demo App")
                    },
                    RedirectUris = new []{ "https://localhost:5006/signin-oidc" },
                    RequireConsent = false,
                    AlwaysIncludeUserClaimsInIdToken = true
                },
                // PivotPro client to allow that app to sign in using the OpenIdConnect / OAuth2 flow.
                new Client
                {
                    ClientId = "FrostAura.Clients.PivotPro",
                    ClientSecrets = { new Secret("P$v0tPR0iS@we40m3".ToSha256()) },
                    AllowedGrantTypes = GrantTypes.CodeAndClientCredentials,
                    AllowedScopes =
                    {
                        "frostaura.scopes.default",
                        "frostaura.scopes.api.devices",
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email
                    },
                    Claims = new List<ClientClaim>
                    {
                        new ClientClaim(ClaimKeys.FA_CLIENT_CUSTOM_CSS_URL, "/css/layout.custom.pivotpro.css"),
                        new ClientClaim(ClaimKeys.FA_CLIENT_CUSTOM_LOGO_SVG_URL, "/vectors/icons/fa.client.pivotpro.logo.svg"),
                        new ClientClaim(ClaimKeys.FA_CLIENT_NAME, "PivotPro")
                    },
                    RedirectUris = new []{ "https://localhost:5001/signin-oidc", "https://pivotpro.frostaura.net/signin-oidc", "https://localhost:8083/signin-oidc" },
                    RequireConsent = false,
                    AlwaysIncludeUserClaimsInIdToken = true
                },
                // Auth flow Northwood Crusaders client to allow that app to sign in using the OpenIdConnect / OAuth2 flow.
                new Client
                {
                    ClientId = "FrostAura.Clients.NorthwoodCrusaders",
                    ClientSecrets = { new Secret("NorthwoodIsAwesome".ToSha256()) },
                    AllowedGrantTypes = GrantTypes.CodeAndClientCredentials,
                    AllowedScopes =
                    {
                        "frostaura.scopes.default",
                        "frostaura.scopes.api.devices",
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email
                    },
                    Claims = new List<ClientClaim>
                    {
                        new ClientClaim(ClaimKeys.FA_CLIENT_CUSTOM_CSS_URL, "/css/layout.custom.northwood-crusaders.css"),
                        new ClientClaim(ClaimKeys.FA_CLIENT_CUSTOM_LOGO_SVG_URL, "/vectors/icons/fa.client.northwood-crusaders.logo.svg"),
                        new ClientClaim(ClaimKeys.FA_CLIENT_NAME, "Northwood Crusaders")
                    },
                    RedirectUris = new []{ "https://localhost:5006/signin-oidc", "https://crusaders.frostaura.net/signin-oidc", "https://localhost:8083/signin-oidc" },
                    RequireConsent = false,
                    AlwaysIncludeUserClaimsInIdToken = true
                },
                // Auth flow FrostAura Components client to allow that app to sign in using the OpenIdConnect / OAuth2 flow.
                new Client
                {
                    ClientId = "FrostAura.Clients.Components",
                    ClientSecrets = { new Secret("FrostAuraIsAwesome".ToSha256()) },
                    AllowedGrantTypes = GrantTypes.CodeAndClientCredentials,
                    AllowedScopes =
                    {
                        "frostaura.scopes.default",
                        "frostaura.scopes.api.devices",
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email
                    },
                    Claims = new List<ClientClaim>
                    {
                        new ClientClaim(ClaimKeys.FA_CLIENT_NAME, "FrostAura Components")
                    },
                    RedirectUris = new []{ "https://localhost:5006/signin-oidc", "https://components.frostaura.net/signin-oidc", "https://localhost:8083/signin-oidc" },
                    RequireConsent = false,
                    AlwaysIncludeUserClaimsInIdToken = true
                }
            };
        }
    }
}
