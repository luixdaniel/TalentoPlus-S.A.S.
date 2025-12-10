using TalentoPlus_S.A.S.ll.Web.Data.Entities;
using TalentoPlus_S.A.S.ll.Web.Models.ViewModels;

namespace TalentoPlus_S.A.S.ll.Web.Extensions
{
    public static class EmployeeExtensions
    {
        public static EmployeeViewModel ToViewModel(this Employee employee)
        {
            if (employee == null) return null!;

            return new EmployeeViewModel
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
                Status = employee.Status,
                EducationLevel = employee.EducationLevel,
                ProfessionalProfile = employee.ProfessionalProfile,
                DepartmentId = employee.DepartmentId,
                DepartmentName = employee.Department?.Name
            };
        }

        public static Employee ToEntity(this EmployeeViewModel viewModel)
        {
            if (viewModel == null) return null!;

            return new Employee
            {
                Id = viewModel.Id,
                DocumentNumber = viewModel.DocumentNumber,
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                BirthDate = viewModel.BirthDate,
                Address = viewModel.Address,
                Phone = viewModel.Phone,
                Email = viewModel.Email,
                Position = viewModel.Position,
                Salary = viewModel.Salary,
                HireDate = viewModel.HireDate,
                Status = viewModel.Status,
                EducationLevel = viewModel.EducationLevel,
                ProfessionalProfile = viewModel.ProfessionalProfile,
                DepartmentId = viewModel.DepartmentId
            };
        }

        public static void UpdateEntity(this Employee employee, EmployeeViewModel viewModel)
        {
            if (employee == null || viewModel == null) return;

            employee.DocumentNumber = viewModel.DocumentNumber;
            employee.FirstName = viewModel.FirstName;
            employee.LastName = viewModel.LastName;
            employee.BirthDate = viewModel.BirthDate;
            employee.Address = viewModel.Address;
            employee.Phone = viewModel.Phone;
            employee.Email = viewModel.Email;
            employee.Position = viewModel.Position;
            employee.Salary = viewModel.Salary;
            employee.HireDate = viewModel.HireDate;
            employee.Status = viewModel.Status;
            employee.EducationLevel = viewModel.EducationLevel;
            employee.ProfessionalProfile = viewModel.ProfessionalProfile;
            employee.DepartmentId = viewModel.DepartmentId;
        }

        public static List<EmployeeViewModel> ToViewModelList(this IEnumerable<Employee> employees)
        {
            return employees?.Select(e => e.ToViewModel()).ToList() ?? new List<EmployeeViewModel>();
        }
    }
}

