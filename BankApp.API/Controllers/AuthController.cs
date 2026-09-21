using Microsoft.AspNetCore.Mvc;
using BankApp.Services;
using BankApp.Models;
using BankApp.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace BankApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase {
    private readonly AuthService authService;
    private readonly IConfiguration configuration;

    public AuthController(IConfiguration configuration) {
        var context = new AppDbContext();
        authService = new AuthService(context);
        this.configuration = configuration;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request) {
        if (!authService.CardExists(request.CardNumber))
            return Unauthorized("Карта не найдена");

        if (authService.IsCardBlocked(request.CardNumber))
            return Unauthorized("Карта заблокирована");
        
        var user = authService.GetUserByCard(request.CardNumber);
        if (user == null)
            return Unauthorized("Пользователь не найден");
        
        if (user.IsFirstLogin)
            return Unauthorized("Требуется активация карты");

        if (!authService.ValidatePin(request.CardNumber, request.Pin)) {
            authService.IncrementFailedAttempts(request.CardNumber);
            return Unauthorized("Неверный PIN-код");
        }

        authService.ResetFailedAttempts(request.CardNumber);

        var token = GenerateJwtToken(user);

        return Ok(new {
            Message = "Успешный вход",
            UserName = user.FullName,
            UserId = user.Id,
            Token = token
        });
    }

    private string GenerateJwtToken(User user) {
        var jwtSettings = configuration.GetSection("Jwt");
        var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

        var claims = new[] {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.FullName)
        };

        var tokenDescriptor = new SecurityTokenDescriptor {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"],
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            )
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);

    }

    [HttpPost("activate")]
    public IActionResult Activate([FromBody] ActivateRequest request) {
        if (!authService.CardExists(request.CardNumber))
            return BadRequest("Карта не найдена");

        var success = authService.ActivateCard(request.CardNumber, request.Pin);
        if (!success)
            return BadRequest("Не удалось активировать карту");
        return Ok(new {Message = "Карта активирована"});
    }
}

public class LoginRequest {
    public string CardNumber {get; set;}
    public string Pin {get; set;}
}

public class ActivateRequest {
    public string CardNumber {get; set;}
    public string Pin {get; set;}
}