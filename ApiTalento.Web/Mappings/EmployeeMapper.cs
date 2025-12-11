using ApiTalento.Web.DTOs;
using ApiTalento.Web.Data.Entities;

namespace ApiTalento.Web.Mappings
{
    public static class EmployeeMapper
    {
        public static EmployeeDto ToDto(this Employee employee)
        {
            if (employee == null) return null!;

            return new EmployeeDto
            {
                Id = employee.Id,
                DocumentNumber = employee.DocumentNumber,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                BirthDate = employee.BirthDate,
                Address = employee.Address,
                Phone = employee.Phone,
                Email = employee.Email,
                Position = employee.Position,
                Salary = employee.Salary,
                HireDate = employee.HireDate,
                Status = GetStatusDisplay(employee.Status),
                EducationLevel = GetEducationLevelDisplay(employee.EducationLevel),
                ProfessionalProfile = employee.ProfessionalProfile,
                DepartmentName = employee.Department?.Name ?? "N/A"
            };
        }

        private static string GetStatusDisplay(EmployeeStatus status)
        {
            return status switch
            {
                EmployeeStatus.Active => "Activo",
                EmployeeStatus.Inactive => "Inactivo",
                EmployeeStatus.Vacation => "Vacaciones",
                _ => "Desconocido"
            };
        }

        private static string GetEducationLevelDisplay(EducationLevel level)
        {
            return level switch
            {
                EducationLevel.Professional => "Profesional",
                EducationLevel.Technical => "Técnico",
                EducationLevel.Technologist => "Tecnólogo",
                EducationLevel.Master => "Maestría",
                EducationLevel.Specialization => "Especialización",
                _ => "Desconocido"
            };
        }
    }
}

