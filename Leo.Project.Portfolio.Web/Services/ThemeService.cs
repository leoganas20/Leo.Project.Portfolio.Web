using Blazored.LocalStorage;
using Microsoft.JSInterop;

namespace Leo.Project.Portfolio.Web.Services;

public class ThemeService
{
    private readonly ILocalStorageService _localStorage;
    private readonly IJSRuntime _jsRuntime;
    private bool _isDarkMode = true;

    public bool IsDarkMode
    {
        get => _isDarkMode;
        private set
        {
            _isDarkMode = value;
            OnThemeChanged?.Invoke();
        }
    }

    public event Action? OnThemeChanged;

    public ThemeService(ILocalStorageService localStorage, IJSRuntime jsRuntime)
    {
        _localStorage = localStorage;
        _jsRuntime = jsRuntime;
    }

    public async Task InitializeAsync()
    {
        var savedTheme = await _localStorage.GetItemAsync<string>("theme");
        if (savedTheme != null)
        {
            IsDarkMode = savedTheme == "dark";
        }
        else
        {
            // Default to dark
            IsDarkMode = true;
        }
        await UpdateBodyAttribute();
    }

    public async Task ToggleThemeAsync()
    {
        IsDarkMode = !IsDarkMode;
        await _localStorage.SetItemAsync("theme", IsDarkMode ? "dark" : "light");
        await UpdateBodyAttribute();
    }

    private async Task UpdateBodyAttribute()
    {
        await _jsRuntime.InvokeVoidAsync("document.documentElement.setAttribute", "data-theme", IsDarkMode ? "dark" : "light");
    }
}
