using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PulseSystem.Application.DTOs.requests;
using PulseSystem.Application.Services.interfaces;
using PulseSystem.Domain.Entities;

namespace PulseSystem.Controllers.v2
{
    [ApiController]
    [ApiExplorerSettings(GroupName = "v2")]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/auth")]
    public class AuthControllerV2 : ControllerBase
    {
        private readonly ILoginService _loginService;
        private readonly IConfiguration _configuration;

        public AuthControllerV2(ILoginService loginService, IConfiguration configuration)
        {
            _loginService = loginService;
            _configuration = configuration;
        }

        /// <summary>
        /// Autentica o usuário e retorna um token JWT.
        /// </summary>
        /// <param name="loginRequest">Credenciais do usuário (email e senha).</param>
        /// <returns>Token JWT para acesso à API.</returns>
        /// <response code="200">Retorna o token JWT.</response>
        /// <response code="401">Credenciais inválidas ou usuário sem permissão.</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(401)]
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

        /// <summary>
        /// Gera o token JWT com base nas informações do usuário.
        /// </summary>
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
}
