# ApiTalento.Tests - Integration Tests

## Overview

Comprehensive integration test suite for the TalentoPlus API. Validates endpoints, database connections, and business logic using real database interactions.

## Features

- **9 Integration Tests** - Complete endpoint coverage
- **Real Database Testing** - Uses in-memory database for isolation
- **Authentication Testing** - JWT token validation
- **PDF Generation Testing** - Validates document generation
- **Database Connection Testing** - Verifies data access layer

## Technology Stack

- xUnit - Testing framework
- Microsoft.AspNetCore.Mvc.Testing - API testing
- Entity Framework InMemory - Test database
- .NET 8.0

## Test Coverage

### Authentication Tests (2 tests)

| Test | Description | Validates |
|------|-------------|-----------|
| `Login_WithValidCredentials_ReturnsToken` | Valid login returns JWT | Token generation, authentication flow |
| `Login_WithInvalidCredentials_ReturnsUnauthorized` | Invalid login returns 401 | Error handling, security |

### Employee Endpoint Tests (3 tests)

| Test | Description | Validates |
|------|-------------|-----------|
| `GetMyInfo_WithValidToken_ReturnsEmployeeData` | Authenticated access works | JWT validation, data retrieval |
| `GetMyInfo_WithoutToken_ReturnsUnauthorized` | Requires authentication | Authorization enforcement |
| `DownloadMyResume_WithValidToken_ReturnsPdf` | PDF generation works | File generation, content type |

### Department Tests (1 test)

| Test | Description | Validates |
|------|-------------|-----------|
| `GetDepartments_ReturnsListOfDepartments` | Public endpoint works | Public access, data retrieval |

### Database Tests (3 tests)

| Test | Description | Validates |
|------|-------------|-----------|
| `DatabaseConnection_CanConnectAndQueryEmployees` | Employee queries work | CRUD operations |
| `DatabaseConnection_CanConnectAndQueryDepartments` | Department queries work | Data access |
| `DatabaseConnection_CanQueryEmployeesWithDepartments` | Joins work correctly | Relationships, navigation |

## Running Tests

### Local Execution

```bash
cd ApiTalento.Tests
dotnet test
```

### With Detailed Output

```bash
dotnet test --verbosity normal
```

### Run Specific Test

```bash
dotnet test --filter "FullyQualifiedName~AuthEndpointsTests.Login_WithValidCredentials_ReturnsToken"
```

### Run Tests for Specific Class

```bash
dotnet test --filter "FullyQualifiedName~AuthEndpointsTests"
```

## Test Results

**Latest Run:**
- Total Tests: 9
- Passed: 9
- Failed: 0
- Duration: ~4.4 seconds

## Project Structure

```
ApiTalento.Tests/
├── IntegrationTests/
│   ├── AuthEndpointsTests.cs          # Authentication tests
│   ├── EmployeeEndpointsTests.cs      # Employee endpoint tests
│   ├── DepartmentEndpointsTests.cs    # Department tests
│   └── DatabaseConnectionTests.cs     # Database tests
├── ApiTalento.Tests.csproj
└── README.md
```

## Test Data

### Real Database Tests

Uses actual database with test employee:
- Document: `1045228184`
- Email: `ceraluis4@gmail.com`

### In-Memory Database Tests

Creates isolated test data for each test:
- Separate database per test
- Automatic cleanup
- No side effects

## Configuration

### WebApplicationFactory

Tests use `WebApplicationFactory<Program>` which:
- Creates in-memory API instance
- No need to run API separately
- Uses same configuration as production
- Isolated test environment

### Test Isolation

Each database test uses a unique database name:
- `TestDatabase_Employees`
- `TestDatabase_Departments`
- `TestDatabase_EmployeesWithDepts`

This ensures tests don't interfere with each other.

## Best Practices

✅ **Arrange-Act-Assert** - All tests follow AAA pattern  
✅ **Descriptive Names** - Test names clearly describe what's being tested  
✅ **Independent Tests** - Each test can run in isolation  
✅ **Multiple Assertions** - Validates multiple aspects per test  
✅ **Shared Fixtures** - Uses `IClassFixture` for efficiency  

## CI/CD Integration

### Docker

Tests run automatically before deployment:

```bash
# Build test image
docker build -f ApiTalento.Tests/Dockerfile -t talento-tests .

# Run tests
docker run --rm talento-tests
```

### GitHub Actions Example

```yaml
- name: Run Tests
  run: |
    cd ApiTalento.Tests
    dotnet test --logger "trx;LogFileName=test-results.trx"
```

## Troubleshooting

### Test Fails: "Cannot find Program"

**Solution:** Ensure `Program.cs` in ApiTalento.Web has:
```csharp
public partial class Program { }
```

### Test Fails: Connection Refused

**Solution:** Tests don't require API to be running. Check database configuration.

### Test Fails: Invalid Token

**Solution:** Verify test credentials exist in the database.

## Adding New Tests

### Example Test

```csharp
[Fact]
public async Task YourTest_Scenario_ExpectedResult()
{
    // Arrange
    var client = _factory.CreateClient();
    
    // Act
    var response = await client.GetAsync("/api/endpoint");
    
    // Assert
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
}
```

## Future Improvements

- [ ] Add performance tests
- [ ] Implement load testing
- [ ] Add code coverage reports
- [ ] Mock external services
- [ ] Add end-to-end tests

## License

Private and confidential.
