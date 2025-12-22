using FluxForgeApi.Entity;
using FluxForgeApi.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;

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
        public async Task<IActionResult> GitHubCallback([FromQuery] string code)
        {
            if (string.IsNullOrEmpty(code))
                return BadRequest("Missing code");

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
                return StatusCode((int)response.StatusCode, payload);

            var json = JsonDocument.Parse(payload);
            var accessToken = json.RootElement.GetProperty("access_token").GetString();
            return Ok(new { accessToken });
        }

        [HttpPost("Registration")]
        public async Task<IActionResult> RegisterUser(UserMainEntity user)
        {
            try
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
                int Id = await authRepository.Registration(user);
                if (Id == null)
                {
                    return Ok("User Already Exists");
                }
                else
                {
                    return Ok("Registration Successful");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Registration failed");
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> LoginUser(UserMainEntity user)

        {
            try
            {
                var ValidateUser = await authRepository.Login(user);
                if (ValidateUser != null)
                {
                    bool isValid = BCrypt.Net.BCrypt.Verify(user.PasswordHash, ValidateUser.PasswordHash);
                    if (!isValid)
                    {
                        return BadRequest("Invalid Credentials");
                    }
                    string token = authRepository.GenerateToken(user);
                    return Ok(token);
                }
                else
                {
                    return BadRequest("User Not Found");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Login Failed");
            }
        }

        [HttpPost("validateToken")]
        [Authorize]
        public IActionResult ValidateToken()
        {
            return Ok("Token is valid");
        }
    }
}