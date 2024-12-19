using Leo.Project.Portfolio.Api.Controllers;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NETCore.MailKit.Extensions;
using NETCore.MailKit.Infrastructure.Internal;

using Newtonsoft.Json;  // Make sure this is added
using Swashbuckle.AspNetCore.Swagger;
using System.IO;

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
                    // Development environment CORS
                    options.AddPolicy("AllowBlazor",
                        policy => policy.WithOrigins("https://localhost:7275") // Local development origin
                                        .AllowAnyMethod()
                                        .AllowAnyHeader());
                }
                else
                {
                    // Production environment CORS
                    options.AddPolicy("AllowBlazor",
                        policy => policy.WithOrigins("https://leoganas-developer.com") // Production origin
                                        .AllowAnyMethod()
                                        .AllowAnyHeader());
                }
            });
            // Add Swagger services
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Portfolio API", Version = "v1" });
            });

            // Add DbContext using SQL Server
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


            //MailKit
            var mailKitOptions = builder.Configuration.GetSection("SmtpSettings").Get<MailKitOptions>();

            // Register the MailKit Email service with the resolved options
            builder.Services.AddMailKit(config => config.UseMailKit(mailKitOptions));

            builder.Services.AddScoped<EmailService>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
            //    // Enable Swagger middleware
            //    app.UseSwagger();

            //    // Enable Swagger UI and add cache control headers
            //    app.UseSwaggerUI(c =>
            //    {
            //        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Portfolio API v1");
            //        c.RoutePrefix = string.Empty; // Optionally, make Swagger UI available at the root
            //    });
            //}
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Portfolio API v1");
                c.RoutePrefix = string.Empty;
            });
            // Enable CORS middleware
            app.UseCors("AllowBlazor");  // This applies the CORS policy to the app

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
