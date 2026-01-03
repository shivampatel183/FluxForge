using System.Data;
using FluxForgeApi.Common;
using System.Net.Http.Headers;
using System.Text.Json;
using FluxForgeApi.Repository.AuthRepository;
using FluxForgeApi.Entity.AuthEntity;
using FluxForgeApi.Common.http;

namespace FluxForgeApi.Business.AuthBusiness
{
    public class GithubAuthBusiness : IGithubAuthRepository
    {
        private readonly IConfiguration configuration;
        private readonly HttpClient _httpClient;
        private readonly IGitHubHttpClient _gitHubHttp;

        public GithubAuthBusiness(HttpClient httpClient, IConfiguration configuration, IGitHubHttpClient _gitHubHttp)
        {
            this.configuration = configuration;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://api.github.com/");
            _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("FluxForge", "1.0"));
            _gitHubHttp = _gitHubHttp;
        }

        public string GetAuthUrl()
        {
            var github = configuration.GetSection("GithubAuth");
            var clientId = github["ClientId"];
            var redirectUri = github["RedirectUri"];

            return $"https://github.com/login/oauth/authorize" +
                   $"?client_id={clientId}" +
                   $"&redirect_uri={redirectUri}" +
                   $"&scope=user:email";
        }

        public async Task<string> GetAccessTokenAsync(string code) {

            var github = configuration.GetSection("GithubAuth");
            var clientId = github["ClientId"];
            var clientSecret = github["ClientSecrets"];

            using var httpClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://github.com/login/oauth/access_token");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret),
                new KeyValuePair<string, string>("code", code)
            });

            var response = await httpClient.SendAsync(request);
            var payload = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return null;

            using var json = JsonDocument.Parse(payload);

            if (json.RootElement.TryGetProperty("access_token", out var tokenElement))
            {
                return tokenElement.GetString(); ;
            }
            return null;
        }

        public Task<GithubUserEntity> GetUserAsync(string accessToken)
        { 
            return _gitHubHttp.GetAsync<GithubUserEntity>("user", accessToken);
        }

    }
}
