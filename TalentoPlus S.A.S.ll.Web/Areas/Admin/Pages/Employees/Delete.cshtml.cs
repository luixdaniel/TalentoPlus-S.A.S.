using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TalentoPlus_S.A.S.ll.Web.Data.Entities;
using TalentoPlus_S.A.S.ll.Web.Services;

namespace TalentoPlus_S.A.S.ll.Web.Areas.Admin.Pages.Employees
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly IEmployeeService _employeeService;

        public DeleteModel(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [BindProperty]
        public Employee Employee { get; set; } = null!;

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

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _employeeService.DeleteEmployeeAsync(Employee.Id);
            TempData["Success"] = "Empleado eliminado exitosamente";
            return RedirectToPage("./Index");
        }
    }
}

