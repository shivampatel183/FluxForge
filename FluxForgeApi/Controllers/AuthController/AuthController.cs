using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using FluxForgeApi.Common;
using FluxForgeApi.Repository.AuthRepository;
using FluxForgeApi.Entity.AuthEntity;
using Azure.Core;

namespace FluxForgeApi.Controllers.AuthController
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

        

        [HttpPost("Registration")]
        public async Task<ApiResponse<string>> RegisterUser(AuthEntity user)
        {
            try
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

                Guid? Id = await authRepository.Registration(user);

                if (Id == null)
                {
                    return ApiResponse<string>.Fail("User Already Exists");
                }

                return ApiResponse<string>.Ok(null,"Registration Successful");
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.Fail($"Registration failed: {ex.Message}");
            }
        }

        [HttpPost("Login")]
        public async Task<ApiResponse<string>> LoginUser(AuthMainEntity user)
        {
            try
            {
                var ValidateUser = await authRepository.Login(user);

                if (ValidateUser.Email == "")
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
            return ApiResponse<string>.Ok(null,"Token is valid");
        }

    }
}