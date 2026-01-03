
using System.Net.Http.Headers;

namespace FluxForgeApi.Common.http
{
    public class GitHubHttpClient : IGitHubHttpClient
    {
        private readonly HttpClient _httpClient;

        public GitHubHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private HttpRequestMessage CreateRequest(HttpMethod method, string url, string accessToken, object? body = null)
        {
            var request = new HttpRequestMessage(method, url);

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            request.Headers.Add("User-Agent", "FluxForge-App");

            request.Headers.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));

            if (body != null)
            {
                request.Content = JsonContent.Create(body);
            }

            return request;
        }

        public Task<T> GetAsync<T>(string url, string accessToken)
        {
            using var request = CreateRequest(HttpMethod.Get, url, accessToken);

            var response = _httpClient.SendAsync(request).Result;

            return response.Content.ReadFromJsonAsync<T>();
        }

        public async Task<T> PostAsync<T>(string url, object body, string accessToken)
        {
            using var request = CreateRequest(HttpMethod.Post, url, accessToken, body);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<T>();
        }
    }
}
