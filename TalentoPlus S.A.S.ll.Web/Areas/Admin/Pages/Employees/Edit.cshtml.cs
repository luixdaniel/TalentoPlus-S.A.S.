using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TalentoPlus_S.A.S.ll.Web.Data.Entities;
using TalentoPlus_S.A.S.ll.Web.Services;

namespace TalentoPlus_S.A.S.ll.Web.Areas.Admin.Pages.Employees
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly IEmployeeService _employeeService;
        private readonly IDepartmentService _departmentService;

        public EditModel(IEmployeeService employeeService, IDepartmentService departmentService)
        {
            _employeeService = employeeService;
            _departmentService = departmentService;
        }

        [BindProperty]
        public Employee Employee { get; set; } = null!;
        
        public SelectList Departments { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Employee = await _employeeService.GetEmployeeByIdAsync(id.Value);
            
            if (Employee == null)
            {
                return NotFound();
            }

            await LoadDepartmentsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadDepartmentsAsync();
                return Page();
            }

            await _employeeService.UpdateEmployeeAsync(Employee);
            TempData["Success"] = "Empleado actualizado exitosamente";
            return RedirectToPage("./Index");
        }

        private async Task LoadDepartmentsAsync()
        {
            var departments = await _departmentService.GetAllDepartmentsAsync();
            Departments = new SelectList(departments, "Id", "Name");
        }
    }
}

