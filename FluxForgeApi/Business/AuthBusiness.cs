using System.Data;
using FluxForgeApi.Entity;
using FluxForgeApi.Repository;
using Dapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Azure;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
namespace FluxForgeApi.Business
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
        public string GetAuthUrl()
        {
            var github = configuration.GetSection("GithubAuth");
            var ClientId = github["ClientId"];

            var url = $"https://github.com/login/oauth/authorize?client_id={ClientId}&scope=repo,user:email";
            return url;
        }

        public async Task<int> Registration(UserMainEntity user)
        {
            return await _db.ExecuteAsync("User_Registration", user, commandType: CommandType.StoredProcedure);
        }

        public async Task<UserEntity> Login(UserMainEntity user)
        {
            return await _db.QueryFirstOrDefaultAsync<UserEntity>("User_Login", user, commandType: CommandType.StoredProcedure);
        }

        public String GenerateToken(UserMainEntity user)
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
