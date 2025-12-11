using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentoPlus_S.A.S.ll.Web.Services;
using TalentoPlus_S.A.S.ll.Web.Models.ViewModels;
using TalentoPlus_S.A.S.ll.Web.Extensions;

namespace TalentoPlus_S.A.S.ll.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class DepartmentsController : Controller
    {
        private readonly IDepartmentService _departmentService;
        private readonly IEmployeeService _employeeService;

        public DepartmentsController(IDepartmentService departmentService, IEmployeeService employeeService)
        {
            _departmentService = departmentService;
            _employeeService = employeeService;
        }

        // GET: Admin/Departments
        public async Task<IActionResult> Index()
        {
            var departments = await _departmentService.GetAllDepartmentsAsync();
            var employees = await _employeeService.GetAllEmployeesAsync();
            
            var viewModels = departments.Select(d => new DepartmentViewModel
            {
                Id = d.Id,
                Name = d.Name,
                EmployeeCount = employees.Count(e => e.DepartmentId == d.Id)
            }).ToList();
            
            return View(viewModels);
        }
    }
}

