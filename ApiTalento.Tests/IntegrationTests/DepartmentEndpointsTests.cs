using System.Net;
using System.Net.Http.Json;
using ApiTalento.Web.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace ApiTalento.Tests.IntegrationTests
{
    public class DepartmentEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public DepartmentEndpointsTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task GetDepartments_ReturnsListOfDepartments()
        {
            // Arrange
            // Este endpoint es público, no requiere autenticación

            // Act
            var response = await _client.GetAsync("/api/Departments");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var departments = await response.Content.ReadFromJsonAsync<List<DepartmentDto>>();
            Assert.NotNull(departments);
            Assert.NotEmpty(departments);
            
            // Verificar que cada departamento tiene los datos correctos
            foreach (var dept in departments)
            {
                Assert.True(dept.Id > 0);
                Assert.False(string.IsNullOrEmpty(dept.Name));
            }
        }
    }
}
