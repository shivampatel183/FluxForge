using FluxForgeApi.Entity;
using FluxForgeApi.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using FluxForgeApi.Common;

namespace FluxForgeApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository authRepository;
        private readonly IConfiguration configuration;

        public AuthController(IAuthRepository authRepository, IConfiguration configuration)
        {
            this.authRepository = authRepository;
            this.configuration = configuration;
        }

        [HttpGet("github")]
        public IActionResult GitHubLogin()
        {
            var url = authRepository.GetAuthUrl();
            return Redirect(url);
        }

        [HttpGet("callback")]
        public async Task<ApiResponse<string>> GitHubCallback([FromQuery] string code)
        {
            try
            {
                if (string.IsNullOrEmpty(code))
                    return ApiResponse<string>.Fail("Missing code");

                var github = configuration.GetSection("GithubAuth");
                var clientId = github["ClientId"];
                var clientSecret = github["ClientSecret"];

                using var httpClient = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, "https://github.com/login/oauth/access_token");
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("client_id", clientId),
                    new KeyValuePair<string, string>("client_secret", clientSecret),
                    new KeyValuePair<string, string>("code", code)
                });

                var response = await httpClient.SendAsync(request);
                var payload = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return ApiResponse<string>.Fail($"GitHub Error: {payload}");

                using var json = JsonDocument.Parse(payload);

                if (json.RootElement.TryGetProperty("access_token", out var tokenElement))
                {
                    var accessToken = tokenElement.GetString();
                    return ApiResponse<string>.Ok(accessToken);
                }

                return ApiResponse<string>.Fail("Could not retrieve access token from GitHub");
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.Fail(ex.Message);
            }
        }

        [HttpPost("Registration")]
        public async Task<ApiResponse<string>> RegisterUser(UserMainEntity user)
        {
            try
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

                int Id = await authRepository.Registration(user);

                if (Id <= 0)
                {
                    return ApiResponse<string>.Fail("User Already Exists or Registration Failed");
                }

                return ApiResponse<string>.Ok("Registration Successful");
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.Fail($"Registration failed: {ex.Message}");
            }
        }

        [HttpPost("Login")]
        public async Task<ApiResponse<string>> LoginUser(UserMainEntity user)
        {
            try
            {
                var ValidateUser = await authRepository.Login(user);

                if (ValidateUser == null)
                {
                    return ApiResponse<string>.Fail("User Not Found");
                }

                bool isValid = BCrypt.Net.BCrypt.Verify(user.PasswordHash, ValidateUser.PasswordHash);

                if (!isValid)
                {
                    return ApiResponse<string>.Fail("Invalid Credentials");
                }

                string token = authRepository.GenerateToken(ValidateUser); 
                return ApiResponse<string>.Ok(token);
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.Fail($"Invalid Request: {ex.Message}");
            }
        }

        [HttpPost("validateToken")]
        [Authorize]
        public ApiResponse<string> ValidateToken()
        {
            return ApiResponse<string>.Ok("Token is valid");
        }
    }
}