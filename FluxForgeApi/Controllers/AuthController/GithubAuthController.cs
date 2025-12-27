using FluxForgeApi.Common;
using FluxForgeApi.Repository.AuthRepository;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using FluxForgeApi.Entity;

namespace FluxForgeApi.Controllers.AuthController
{
    [Route("api/[controller]")]
    [ApiController]
    public class GithubAuthController : ControllerBase
    {
        private readonly IGithubAuthRepository gitAuthRepository;
        private readonly IAuthRepository authRepository;

        public GithubAuthController(IGithubAuthRepository githubAuthRepository, IAuthRepository authRepository){
            this.authRepository = authRepository;
            this.gitAuthRepository = githubAuthRepository;
        }

        [HttpGet("github")]
        public IActionResult GitHubLogin()
        {
            var url = gitAuthRepository.GetAuthUrl();
            return Redirect(url);
        }

        [HttpGet("callback")]
        public async Task<ApiResponse<string>> GitHubCallback([FromQuery] string code)
        {
            try
            {
                if (string.IsNullOrEmpty(code))
                    return ApiResponse<string>.Fail("Missing code");

                var accressToken = await gitAuthRepository.GetAccessTokenAsync(code);
                var user = await gitAuthRepository.GetUserAsync(accressToken);

                var existingUser = await authRepository.Login(new AuthMainEntity
                {
                    Email = user.Email
                });
                if (existingUser == null)
                {
                    await authRepository.Registration(new AuthMainEntity
                    {
                        DisplayName = user.Name,
                        Email = user.Email,
                        PasswordHash = Guid.NewGuid().ToString()
                    });
                }
                
                return ApiResponse<string>.Fail("Could not retrieve access token from GitHub");
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.Fail(ex.Message);
            }
        }
    }
}
