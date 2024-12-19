using Blazored.LocalStorage;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text;

namespace Leo.Project.Portfolio.Web.Services;

public class LogService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;

    public LogService(HttpClient httpClient, ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
    }

    public async Task<string> LoginAsync(string username, string password)
    {
        var loginData = new
        {
            username,
            password
        };

        var content = new StringContent(JsonSerializer.Serialize(loginData), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("login", content);

        if (response.IsSuccessStatusCode)
        {
            // Get the token from the response and store it
            var token = await response.Content.ReadAsStringAsync();
            await _localStorage.SetItemAsync("jwt_token", token);
            return token;
        }
        return null; // Handle failed login attempts
    }
}