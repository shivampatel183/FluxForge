using FluxForgeApi.Common;
using FluxForgeApi.Repository.AuthRepository;
using Microsoft.AspNetCore.Mvc;
using FluxForgeApi.Entity.AuthEntity;

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
        public async Task<IActionResult> GitHubCallback([FromQuery] string code)
        {
            try
            {
                var accressToken = await gitAuthRepository.GetAccessTokenAsync(code);
                var user = await gitAuthRepository.GetUserAsync(accressToken);

                var existingUser = await authRepository.Login(new AuthMainEntity
                {
                    Email = user.Email
                });
                if (existingUser.Email == "")
                {
                    await authRepository.Registration(new AuthEntity
                    {
                        DisplayName = user.Name,
                        Email = user.Email,
                        GithubId = (user.Id).ToString(),
                        GithubToken = accressToken,
                        PasswordHash = Guid.NewGuid().ToString(),
                        AvatarUrl = user.AvatarUrl
                    });
                }
                var token = authRepository.GenerateToken(new AuthMainEntity{ Email = user.Email });
                SetCookie("FluxForgeJwt", token, httpOnly: false);
                SetCookie("UserName", user.Name, httpOnly: false);
                SetCookie("UserEmail", user.Email, httpOnly: false);
                SetCookie("UserAvatar", user.AvatarUrl, httpOnly: false);

                return Redirect("http://localhost:4200/github-redirect");
            }
            catch (Exception ex)
            {
                return Redirect("http://localhost:4200/login");
            }
        }

        #region Private Methods

        private void SetCookie(string key, string value, bool httpOnly)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = httpOnly,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddDays(7)
            };

            Response.Cookies.Append(key, value, cookieOptions);
        }

        #endregion
    }
}
