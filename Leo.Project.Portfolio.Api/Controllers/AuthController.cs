using Leo.Project.Portfolio.Api.Model;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Leo.Project.Portfolio.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public IActionResult Login([FromQuery] string Username, [FromQuery] string Password)
    {
        try
        {
            var user = _context.Users.SingleOrDefault(u => u.Username == Username);

            if (user == null || user.Password != Password)
            {
                return Unauthorized("Authentication failed.");
            }

            var claims = new[]
            {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(int.Parse(_configuration["Jwt:ExpiryMinutes"])),
                signingCredentials: creds);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }
        catch (Exception ex)
        {
            // Log the exception (e.g., to file or Azure Application Insights)
            return StatusCode(500, "Internal Server Error: " + ex.Message);
        }
    }


    // Helper method for password verification
    public static bool VerifyPassword(string enteredPassword, string storedHash, string storedSalt)
    {
        // Decode the stored salt
        var saltBytes = Convert.FromBase64String(storedSalt);

        // Combine the entered password and the stored salt
        var passwordBytes = Encoding.UTF8.GetBytes(enteredPassword);
        var passwordWithSaltBytes = new byte[passwordBytes.Length + saltBytes.Length];
        Buffer.BlockCopy(saltBytes, 0, passwordWithSaltBytes, 0, saltBytes.Length);
        Buffer.BlockCopy(passwordBytes, 0, passwordWithSaltBytes, saltBytes.Length, passwordBytes.Length);

        // Hash the combination
        using (var sha256 = SHA256.Create())
        {
            var hash = sha256.ComputeHash(passwordWithSaltBytes);
            var enteredHash = Convert.ToBase64String(hash);

            // Compare the stored hash with the computed hash
            return storedHash == enteredHash;
        }
    }
}
