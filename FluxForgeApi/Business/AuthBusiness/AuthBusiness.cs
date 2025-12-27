using System.Data;
using FluxForgeApi.Entity;
using Dapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Azure;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using FluxForgeApi.Repository.AuthRepository;
namespace FluxForgeApi.Business.Auth
{
    public class AuthBusiness : IAuthRepository
    {
        private readonly IConfiguration configuration;
        private readonly IDbConnection _db;
        public AuthBusiness(IConfiguration configuration, IDbConnection dbConnection)
        {
            this.configuration = configuration;
            _db = dbConnection;
        }
        
        public async Task<int> Registration(AuthMainEntity user)
        {
            return await _db.ExecuteAsync("User_Registration", user, commandType: CommandType.StoredProcedure);
        }

        public async Task<AuthEntity> Login(AuthMainEntity user)
        {
            return await _db.QueryFirstOrDefaultAsync<AuthEntity>("User_Login", user, commandType: CommandType.StoredProcedure);
        }

        public string GenerateToken(AuthMainEntity user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.DisplayName),
                new Claim(ClaimTypes.Email , user.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var token = new JwtSecurityToken(
                    issuer: configuration["JwtSettings:Issuer"],
                    audience: configuration["JwtSettings:Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(24),
                    signingCredentials: creds
                );

            string jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            return jwtToken;
        }
    }
}
