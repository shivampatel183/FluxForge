namespace FluxForgeApi.Common.http
{
    public interface IGitHubHttpClient
    {
        Task<T> GetAsync<T>(string url, string accessToken);
        Task<T> PostAsync<T>(string url, object body, string accessToken);
    }
}
