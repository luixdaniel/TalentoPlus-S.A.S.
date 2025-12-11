using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using ApiTalento.Web.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace ApiTalento.Tests.IntegrationTests
{
    public class AuthEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public AuthEndpointsTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsToken()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                DocumentNumber = "1045228184",
                Email = "ceraluis4@gmail.com"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(loginDto),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await _client.PostAsync("/api/Auth/login", content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
            Assert.NotNull(loginResponse);
            Assert.NotNull(loginResponse.Token);
            Assert.NotEmpty(loginResponse.Token);
            Assert.Equal("ceraluis4@gmail.com", loginResponse.Email);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                DocumentNumber = "9999999999",
                Email = "invalid@example.com"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(loginDto),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await _client.PostAsync("/api/Auth/login", content);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
