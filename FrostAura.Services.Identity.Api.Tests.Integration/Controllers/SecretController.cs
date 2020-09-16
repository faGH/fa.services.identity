using IdentityModel.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

namespace FrostAura.Services.Identity.Api.Tests.Integration.Controllers
{
    public class SecretController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        public SecretController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        [Route("/secret")]
        [Authorize]
        public IActionResult Index()
        {
            return Ok("Secret message from FrostAura.Services.Identity.Tests.Integration");
        }

        [Route("/")]
        public async Task<IActionResult> AuthClient()
        {
            // Install identity model package for models.
            var httpClient = httpClientFactory.CreateClient();
            var discoveryDocument = await httpClient.GetDiscoveryDocumentAsync("https://localhost:5001");
            var tokenResponse = await httpClient.RequestClientCredentialsTokenAsync(
                new ClientCredentialsTokenRequest
                {
                    Address = discoveryDocument.TokenEndpoint,
                    ClientId = "FrostAura.Services.Identity.Api.Tests.Integration",
                    ClientSecret = "Password1234$",
                    Scope = "FrostAura.AllowConnection"
                });
            var apiHttpClient = httpClientFactory.CreateClient();

            apiHttpClient.SetBearerToken(tokenResponse.AccessToken);

            var response = await apiHttpClient.GetAsync("https://localhost:5002/secret");
            var content = await response.Content.ReadAsStringAsync();

            return Ok(new
            {
                access_token = tokenResponse.AccessToken,
                message = content
            });
        }
    }
}
