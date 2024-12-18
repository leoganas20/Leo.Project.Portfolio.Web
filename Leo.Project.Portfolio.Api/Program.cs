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

            // Enable CORS to allow your Blazor client (localhost:7275)
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowBlazor",
                    builder => builder.WithOrigins("https://localhost:7275")
                                      .AllowAnyMethod()
                                      .AllowAnyHeader());
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

            // Generate and save beautified swagger.json file
            if (app.Environment.IsDevelopment())
            {
                var swaggerProvider = app.Services.GetRequiredService<ISwaggerProvider>();
                var swaggerDoc = swaggerProvider.GetSwagger("v1");

                // Path to save the swagger.json file
                var swaggerJsonPath = Path.Combine(Directory.GetCurrentDirectory(), "swagger", "swagger.json");

                // Create directory if it doesn't exist
                Directory.CreateDirectory(Path.GetDirectoryName(swaggerJsonPath));

                // Serialize the Swagger doc with indentation and save it as a beautified JSON file
                var settings = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented // This makes the JSON pretty
                };

                var json = JsonConvert.SerializeObject(swaggerDoc, settings);

                // Write the beautified JSON to the file
                File.WriteAllText(swaggerJsonPath, json);
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                // Enable Swagger middleware
                app.UseSwagger();

                // Enable Swagger UI and add cache control headers
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Portfolio API v1");
                    c.RoutePrefix = string.Empty; // Optionally, make Swagger UI available at the root
                });
            }

            // Enable CORS middleware
            app.UseCors("AllowBlazor");  // This applies the CORS policy to the app

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
