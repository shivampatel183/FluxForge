using FluxForgeApi.Entity;

namespace FluxForgeApi.Repository.AuthRepository
{
    public interface IAuthRepository
    {
        public Task<int> Registration(AuthMainEntity Auth);
        public Task<AuthEntity> Login(AuthMainEntity Auth);
        public string GenerateToken(AuthMainEntity Auth);
    }
}
