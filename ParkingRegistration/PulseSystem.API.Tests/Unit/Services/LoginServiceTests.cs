using Xunit;
using Moq;
using FluentAssertions;
using PulseSystem.Application.Services.Implementations;
using PulseSystem.Domain.Entities;
using PulseSystem.Infraestructure.Repositories.interfaces;
using System.Threading.Tasks;

namespace PulseSystem.API.Tests.Unit.Services
{
    public class LoginServiceTests
    {
        private readonly Mock<IEmployeeRepository> _employeeRepoMock;
        private readonly LoginService _service;

        public LoginServiceTests()
        {
            _employeeRepoMock = new Mock<IEmployeeRepository>();
            _service = new LoginService(_employeeRepoMock.Object);
        }

        [Fact]
        public async Task ValidatedUserAsync_ShouldReturnEmployee_WhenCredentialsAreCorrect()
        {
            // Arrange
            var password = "1234";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var employeeFromRepo = new Employee
            {
                Email = "teste@teste.com",
                Password = hashedPassword,
                Role = "GESTOR"
            };

            _employeeRepoMock.Setup(r => r.GetByEmailAsync("teste@teste.com"))
                .ReturnsAsync(employeeFromRepo);

            // Act
            var result = await _service.ValidatedUserAsync("teste@teste.com", password);

            // Assert
            result.Should().NotBeNull();
            result!.Email.Should().Be("teste@teste.com");
            result.Role.Should().Be("GESTOR");
        }

        [Fact]
        public async Task ValidatedUserAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            _employeeRepoMock.Setup(r => r.GetByEmailAsync("inv@teste.com"))
                .ReturnsAsync((Employee?)null);

            // Act
            var result = await _service.ValidatedUserAsync("inv@teste.com", "1234");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task ValidatedUserAsync_ShouldReturnNull_WhenPasswordIsIncorrect()
        {
            // Arrange
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("1234");

            var employeeFromRepo = new Employee
            {
                Email = "teste@teste.com",
                Password = hashedPassword,
                Role = "GESTOR"
            };

            _employeeRepoMock.Setup(r => r.GetByEmailAsync("teste@teste.com"))
                .ReturnsAsync(employeeFromRepo);

            // Act
            var result = await _service.ValidatedUserAsync("teste@teste.com", "wrongpassword");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void IsGestor_ShouldReturnTrue_WhenRoleIsGestor()
        {
            // Arrange
            var employee = new Employee { Role = "GESTOR" };

            // Act
            var result = _service.IsGestor(employee);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsGestor_ShouldReturnFalse_WhenRoleIsNotGestor()
        {
            // Arrange
            var employee = new Employee { Role = "FUNCIONARIO" };

            // Act
            var result = _service.IsGestor(employee);

            // Assert
            result.Should().BeFalse();
        }
    }
}
