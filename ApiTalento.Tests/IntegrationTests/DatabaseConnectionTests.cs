using ApiTalento.Web.Data;
using ApiTalento.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ApiTalento.Tests.IntegrationTests
{
    public class DatabaseConnectionTests
    {
        [Fact]
        public async Task DatabaseConnection_CanConnectAndQueryEmployees()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase_Employees")
                .Options;

            using var context = new ApplicationDbContext(options);

            // Seed test data
            context.Departments.Add(new Department { Id = 1, Name = "Test Department" });
            context.Employees.Add(new Employee
            {
                Id = 1,
                DocumentNumber = "123456789",
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                BirthDate = new DateTime(1990, 1, 1),
                Address = "Test Address",
                Phone = "1234567890",
                Position = "Test Position",
                Salary = 50000,
                HireDate = DateTime.Now,
                Status = EmployeeStatus.Active,
                EducationLevel = EducationLevel.Professional,
                DepartmentId = 1
            });
            await context.SaveChangesAsync();

            // Act
            var employees = await context.Employees.ToListAsync();

            // Assert
            Assert.NotNull(employees);
            Assert.Single(employees);
            Assert.Equal("Test", employees[0].FirstName);
            Assert.Equal("User", employees[0].LastName);
        }

        [Fact]
        public async Task DatabaseConnection_CanConnectAndQueryDepartments()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase_Departments")
                .Options;

            using var context = new ApplicationDbContext(options);

            // Seed test data
            context.Departments.AddRange(
                new Department { Id = 1, Name = "IT" },
                new Department { Id = 2, Name = "HR" },
                new Department { Id = 3, Name = "Finance" }
            );
            await context.SaveChangesAsync();

            // Act
            var departments = await context.Departments.ToListAsync();

            // Assert
            Assert.NotNull(departments);
            Assert.Equal(3, departments.Count);
            Assert.Contains(departments, d => d.Name == "IT");
            Assert.Contains(departments, d => d.Name == "HR");
            Assert.Contains(departments, d => d.Name == "Finance");
        }

        [Fact]
        public async Task DatabaseConnection_CanQueryEmployeesWithDepartments()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase_EmployeesWithDepts")
                .Options;

            using var context = new ApplicationDbContext(options);

            // Seed test data
            var department = new Department { Id = 1, Name = "Technology" };
            context.Departments.Add(department);
            
            context.Employees.Add(new Employee
            {
                Id = 1,
                DocumentNumber = "987654321",
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                BirthDate = new DateTime(1985, 5, 15),
                Address = "123 Main St",
                Phone = "5551234567",
                Position = "Developer",
                Salary = 75000,
                HireDate = DateTime.Now,
                Status = EmployeeStatus.Active,
                EducationLevel = EducationLevel.Professional,
                DepartmentId = 1
            });
            await context.SaveChangesAsync();

            // Act
            var employee = await context.Employees
                .Include(e => e.Department)
                .FirstOrDefaultAsync(e => e.DocumentNumber == "987654321");

            // Assert
            Assert.NotNull(employee);
            Assert.NotNull(employee.Department);
            Assert.Equal("Technology", employee.Department.Name);
            Assert.Equal("John", employee.FirstName);
        }
    }
}
