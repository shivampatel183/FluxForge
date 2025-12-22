namespace FluxForgeApi.Entity
{
    public class UserMainEntity
    {
        public UserMainEntity()
        {
            UserMainEntityDefaultValue();
        }

        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public DateTime CreatedAt { get; set; }

        public void UserMainEntityDefaultValue()
        {
            DisplayName = "";
            Email = "";
            PasswordHash = "";
            CreatedAt = DateTime.UtcNow;
        }
    }

    public class UserEntity : UserMainEntity
    {
        public UserEntity()
        {
            UserEntityDefaultValue();
        }
        public string GithubId { get; set; }
        public string GithubToken { get; set; }

        public void UserEntityDefaultValue()
        {
            GithubId = "";
            GithubToken = "";
        }
    }
}
