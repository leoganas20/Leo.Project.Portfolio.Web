using Leo.Project.Portfolio.Api.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NETCore.MailKit.Extensions;
using NETCore.MailKit.Infrastructure.Internal;
using System.Text;

namespace Leo.Project.Portfolio.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            var env = builder.Environment;

            builder.Services.AddCors(options =>
            {
                if (env.IsDevelopment())
                {
                    options.AddPolicy("AllowBlazor",
                        policy => policy.WithOrigins("https://localhost:7275", "https://localhost:7008")
                                        .AllowAnyMethod()
                                        .AllowAnyHeader());
                }
                else
                {
                    options.AddPolicy("AllowBlazor",
                        policy => policy.WithOrigins("https://leoganas-developer.com", "https://leoprojectportfolioapi20241218212239.azurewebsites.net")
                                        .AllowAnyMethod()
                                        .AllowAnyHeader());
                }
            });

            builder.Services.AddLogging(logging =>
            {
                logging.AddConsole();
                logging.AddDebug();
            });

            // Configure JWT Authentication
            var jwtKey = builder.Configuration["Jwt:SecretKey"];
            var key = Encoding.ASCII.GetBytes(jwtKey);
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            // Add Swagger services
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Portfolio API", Version = "v1" });

                // Add JWT Authentication to Swagger
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            // Add DbContext using SQL Server
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // MailKit
            var mailKitOptions = builder.Configuration.GetSection("SmtpSettings").Get<MailKitOptions>();
            builder.Services.AddMailKit(config => config.UseMailKit(mailKitOptions));

            builder.Services.AddScoped<EmailService>();
            builder.Services.AddScoped<AuthController>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Portfolio API v1");
                c.RoutePrefix = string.Empty;
            });

            app.UseCors("AllowBlazor");
            app.UseStaticFiles();
            app.UseHttpsRedirection();

            // Add Authentication Middleware
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
