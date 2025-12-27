using FluxForgeApi.Entity;

namespace FluxForgeApi.Repository.AuthRepository
{
    public interface IGithubAuthRepository
    {
        public string GetAuthUrl();
        public Task<string> GetAccessTokenAsync(string code);
        public Task<GithubUserEntity> GetUserAsync(string accessToken);
    }
}
