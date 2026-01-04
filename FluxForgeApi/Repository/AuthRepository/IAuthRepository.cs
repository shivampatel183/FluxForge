using FluxForgeApi.Entity.AuthEntity;

namespace FluxForgeApi.Repository.AuthRepository
{
    public interface IAuthRepository
    {
        public Task<Guid?> Registration(AuthEntity Auth);
        public Task<AuthEntity> Login(AuthMainEntity Auth);
        public string GenerateToken(AuthMainEntity Auth);
    }
}
