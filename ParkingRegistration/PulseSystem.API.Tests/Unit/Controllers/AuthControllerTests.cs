using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using PulseSystem.Controllers;
using PulseSystem.Application.Services.Implementations;
using PulseSystem.Application.DTOs.requests;
using PulseSystem.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace PulseSystem.API.Tests.Unit.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<LoginService> _loginServiceMock;
        private readonly IConfiguration _configuration;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            // Criar Mock do LoginService
            // Certifique-se que os métodos que você quer mockar são "virtual"
            _loginServiceMock = new Mock<LoginService>(null) { CallBase = true };

            // Criar IConfiguration fake com valores de JWT
            var inMemorySettings = new Dictionary<string, string>
            {
                {"Jwt:Key", "QmZr8eL3p2Xy9rN7uA1vG5cT8sK4hJ6oL9zM2fB7qD1yE3wR5tP0nU8iC4xO6aS"},
                {"Jwt:Issuer", "PulseAuthAPI"},
                {"Jwt:Audience", "PulseSystem"},
                {"Jwt:ExpireMinutes", "60"}
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _controller = new AuthController(_loginServiceMock.Object, _configuration);
        }

        [Fact]
        public async Task Login_ShouldReturnToken_WhenUserIsGestor()
        {
            // Arrange
            var loginRequest = new LoginRequest { Email = "teste@teste.com", Password = "1234" };
            var employee = new Employee { Email = "teste@teste.com", Role = "GESTOR" };

            _loginServiceMock
                .Setup(s => s.ValidatedUserAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(employee);

            _loginServiceMock
                .Setup(s => s.IsGestor(employee))
                .Returns(true);

            // Act
            var result = await _controller.Login(loginRequest);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;

            okResult!.Value.Should().BeAssignableTo<object>();

            // Captura o token dinamicamente
            var tokenProperty = okResult.Value.GetType().GetProperty("token");
            tokenProperty.Should().NotBeNull();
            var tokenValue = tokenProperty!.GetValue(okResult.Value)?.ToString();
            tokenValue.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenUserInvalid()
        {
            // Arrange
            var loginRequest = new LoginRequest { Email = "inv@teste.com", Password = "1234" };

            _loginServiceMock
                .Setup(s => s.ValidatedUserAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((Employee?)null);

            // Act
            var result = await _controller.Login(loginRequest);

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
            var unauthorized = result as UnauthorizedObjectResult;
            unauthorized!.Value.Should().Be("Email ou senha inválidos");
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenUserNotGestor()
        {
            // Arrange
            var loginRequest = new LoginRequest { Email = "user@teste.com", Password = "1234" };
            var employee = new Employee { Email = "user@teste.com", Role = "FUNCIONARIO" };

            _loginServiceMock
                .Setup(s => s.ValidatedUserAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(employee);

            _loginServiceMock
                .Setup(s => s.IsGestor(employee))
                .Returns(false);

            // Act
            var result = await _controller.Login(loginRequest);

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
            var unauthorized = result as UnauthorizedObjectResult;
            unauthorized!.Value.Should().Be("Acesso negado. Apenas gestores podem acessar este recurso");
        }
    }
}
