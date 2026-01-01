namespace FluxForgeApi.Entity.AuthEntity
{
    public class AuthMainEntity
    {
        public AuthMainEntity()
        {
            AuthMainEntityDefaultValue();
        }

        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public DateTime CreatedAt { get; set; }

        public void AuthMainEntityDefaultValue()
        {
            DisplayName = "";
            Email = "";
            PasswordHash = "";
            CreatedAt = DateTime.UtcNow;
        }
    }

    public class AuthEntity : AuthMainEntity
    {
        public AuthEntity()
        {
            AuthEntityDefaultValue();
        }
        public string GithubId { get; set; }
        public string GithubToken { get; set; }

        public void AuthEntityDefaultValue()
        {
            GithubId = "";
            GithubToken = "";
        }
    }
}
