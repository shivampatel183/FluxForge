using FluxForgeApi.Entity;

namespace FluxForgeApi.Repository
{
    public interface IAuthRepository
    {
        public string GetAuthUrl();
        public Task<int> Registration(UserMainEntity user);
        public Task<UserEntity> Login(UserMainEntity user);
        public String GenerateToken(UserMainEntity user);
    }
}
