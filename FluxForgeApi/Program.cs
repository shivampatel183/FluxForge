
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FluxForgeApi.Business.Auth;
using FluxForgeApi.Repository.AuthRepository;
using FluxForgeApi.Business.AuthBusiness;

namespace FluxForgeApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularUI",
                    policy =>
                    {
                        policy.WithOrigins("http://localhost:65517", "http://localhost:4200")
                              .AllowAnyMethod()
                              .AllowAnyHeader();
                    });
            });
            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddHttpClient();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddScoped<IAuthRepository, AuthBusiness>();
            builder.Services.AddScoped<IGithubAuthRepository, GithubAuthBusiness>();

            builder.Services.AddScoped<IDbConnection>(Sp =>
            {
                var connection = builder.Configuration.GetConnectionString("DefaultConnection");
                return new SqlConnection(connection);
            });

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                        ValidAudience = builder.Configuration["JwtSettings:Audience"],

                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"])
                        )
                    };
                });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowAngularUI");
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
