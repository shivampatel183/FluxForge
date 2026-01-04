using System.Text.Json.Serialization;

namespace FluxForgeApi.Entity.AuthEntity
{
    public class GithubUserEntity
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("login")]
        public string Login { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("avatar_url")]
        public string AvatarUrl { get; set; }
    }

    public class GitHubEmailEntity
    {
        public string email { get; set; }
        public bool primary { get; set; }
        public bool verified { get; set; }
        public string visibility { get; set; }
    }
}
