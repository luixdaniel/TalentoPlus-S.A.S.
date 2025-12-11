using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using ApiTalento.Web.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace ApiTalento.Tests.IntegrationTests
{
    public class EmployeeEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public EmployeeEndpointsTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        private async Task<string> GetAuthTokenAsync()
        {
            var loginDto = new LoginDto
            {
                DocumentNumber = "1045228184",
                Email = "ceraluis4@gmail.com"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(loginDto),
                Encoding.UTF8,
                "application/json");

            var response = await _client.PostAsync("/api/Auth/login", content);
            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
            
            return loginResponse?.Token ?? throw new Exception("No se pudo obtener el token");
        }

        [Fact]
        public async Task GetMyInfo_WithValidToken_ReturnsEmployeeData()
        {
            // Arrange
            var token = await GetAuthTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.GetAsync("/api/Employees/me");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var employeeDto = await response.Content.ReadFromJsonAsync<EmployeeDto>();
            Assert.NotNull(employeeDto);
            Assert.Equal("ceraluis4@gmail.com", employeeDto.Email);
            Assert.Equal("1045228184", employeeDto.DocumentNumber);
        }

        [Fact]
        public async Task GetMyInfo_WithoutToken_ReturnsUnauthorized()
        {
            // Arrange
            // No se agrega token de autenticación

            // Act
            var response = await _client.GetAsync("/api/Employees/me");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task DownloadMyResume_WithValidToken_ReturnsPdf()
        {
            // Arrange
            var token = await GetAuthTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.GetAsync("/api/Employees/me/resume");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("application/pdf", response.Content.Headers.ContentType?.MediaType);

            var pdfBytes = await response.Content.ReadAsByteArrayAsync();
            Assert.NotEmpty(pdfBytes);
            
            // Verificar que es un PDF válido (comienza con %PDF)
            var pdfHeader = Encoding.ASCII.GetString(pdfBytes.Take(4).ToArray());
            Assert.Equal("%PDF", pdfHeader);
        }
    }
}
