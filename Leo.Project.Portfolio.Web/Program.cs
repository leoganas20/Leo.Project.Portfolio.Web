using Blazored.LocalStorage;
using Leo.Project.Portfolio.Web.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using MudBlazor.Services;

namespace Leo.Project.Portfolio.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            var env = builder.HostEnvironment;

            // Set the API base URL depending on the environment
            var apiBaseUrl = env.IsDevelopment()
                ? "https://localhost:7008" // Development URL
                : "https://portfolio-api-bce7cjdng7e3crfn.canadacentral-01.azurewebsites.net"; // Production URL

            // Add the custom HTTP message handler
            //builder.Services.AddScoped<AuthorizationMessageHandler>();

            // Add HttpClient with the custom handler
            builder.Services.AddScoped(sp => new HttpClient
            {
                BaseAddress = new Uri(apiBaseUrl)
            });

            // Register services
            builder.Services.AddBlazoredLocalStorage(); // For localStorage
            builder.Services.AddScoped<LogService>();   // Register the login service

            builder.Services.AddScoped<RequestEmailService>();
            builder.Services.AddMudServices();
            builder.Services.AddScoped<PortfolioApiService>();
            builder.Services.AddMudServices(config =>
            {
                config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopCenter;
                config.SnackbarConfiguration.PreventDuplicates = false;
                config.SnackbarConfiguration.NewestOnTop = false;
                config.SnackbarConfiguration.ShowCloseIcon = true;
                config.SnackbarConfiguration.VisibleStateDuration = 2000;
                config.SnackbarConfiguration.HideTransitionDuration = 500;
                config.SnackbarConfiguration.ShowTransitionDuration = 500;
                config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
            });

            // Build and run the Blazor app
            await builder.Build().RunAsync();
        }
    }
}
