using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PulseSystem.Application.DTOs.requests;
using PulseSystem.Application.Services.Implementations;
using PulseSystem.Domain.Entities;

namespace PulseSystem.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class AuthController : ControllerBase
{
    private readonly LoginService _loginService;
    private readonly IConfiguration _configuration;

    public AuthController(LoginService loginService, IConfiguration configuration)
    {
        _loginService = loginService;
        _configuration = configuration;
    }

    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
    {
        var employee = await _loginService.ValidatedUserAsync(loginRequest.Email, loginRequest.Password);
        if (employee == null)
            return Unauthorized("Email ou senha inválidos");

        if (!_loginService.IsGestor(employee))
            return Unauthorized("Acesso negado. Apenas gestores podem acessar este recurso");

        var token = GenerateJwtToken(employee);
        return Ok(new { token });
    }

    private string GenerateJwtToken(Employee employee)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, employee.Email),
            new Claim(ClaimTypes.Role, employee.Role)

        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(jwtSettings.GetValue<int>("ExpireMinutes")),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
