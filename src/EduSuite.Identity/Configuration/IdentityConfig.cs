using Duende.IdentityServer.Models;

namespace EduSuite.Identity.Configuration;

public static class IdentityConfig
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email(),
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new List<ApiScope>
        {
            new ApiScope("edusuite.api", "EduSuite API"),
            new ApiScope("edusuite.admin", "EduSuite Administration")
        };

    public static IEnumerable<Client> Clients =>
        new List<Client>
        {
            // Blazor Server Client
            new Client
            {
                ClientId = "edusuite.web",
                ClientName = "EduSuite Web Application",
                AllowedGrantTypes = GrantTypes.Code,
                RequirePkce = true,
                RequireClientSecret = true,
                ClientSecrets = { new Secret("your-secret-here".Sha256()) },

                RedirectUris = { "https://localhost:5001/signin-oidc" },
                PostLogoutRedirectUris = { "https://localhost:5001/signout-callback-oidc" },
                FrontChannelLogoutUri = "https://localhost:5001/signout-oidc",

                AllowOfflineAccess = true,
                AllowedScopes = 
                {
                    "openid",
                    "profile",
                    "email",
                    "edusuite.api"
                }
            },

            // API Client for service-to-service communication
            new Client
            {
                ClientId = "edusuite.api.client",
                ClientName = "EduSuite API Client",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = { new Secret("api-secret".Sha256()) },
                AllowedScopes = { "edusuite.api", "edusuite.admin" }
            }
        };
} 