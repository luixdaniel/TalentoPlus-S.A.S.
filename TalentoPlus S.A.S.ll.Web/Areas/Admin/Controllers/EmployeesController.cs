using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TalentoPlus_S.A.S.ll.Web.Data.Entities;
using TalentoPlus_S.A.S.ll.Web.Services;

namespace TalentoPlus_S.A.S.ll.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class EmployeesController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IDepartmentService _departmentService;

        public EmployeesController(IEmployeeService employeeService, IDepartmentService departmentService)
        {
            _employeeService = employeeService;
            _departmentService = departmentService;
        }

        // GET: Admin/Employees
        public async Task<IActionResult> Index()
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            return View(employees);
        }

        // GET: Admin/Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _employeeService.GetEmployeeByIdAsync(id.Value);
            
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Admin/Employees/Create
        public async Task<IActionResult> Create()
        {
            await LoadDepartments();
            return View();
        }

        // POST: Admin/Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee)
        {
            if (!ModelState.IsValid)
            {
                await LoadDepartments();
                return View(employee);
            }

            try
            {
                await _employeeService.CreateEmployeeAsync(employee);
                TempData["Success"] = "Empleado creado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al crear el empleado: {ex.Message}");
                await LoadDepartments();
                return View(employee);
            }
        }

        // GET: Admin/Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _employeeService.GetEmployeeByIdAsync(id.Value);
            
            if (employee == null)
            {
                return NotFound();
            }

            await LoadDepartments();
            return View(employee);
        }

        // POST: Admin/Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Employee employee)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                await LoadDepartments();
                return View(employee);
            }

            try
            {
                await _employeeService.UpdateEmployeeAsync(employee);
                TempData["Success"] = "Empleado actualizado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al actualizar el empleado: {ex.Message}");
                await LoadDepartments();
                return View(employee);
            }
        }

        // GET: Admin/Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _employeeService.GetEmployeeByIdAsync(id.Value);
            
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Admin/Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _employeeService.DeleteEmployeeAsync(id);
                TempData["Success"] = "Empleado eliminado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al eliminar el empleado: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Admin/Employees/Import
        public IActionResult Import()
        {
            return View();
        }

        // POST: Admin/Employees/Import
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length == 0)
            {
                ModelState.AddModelError("excelFile", "Por favor selecciona un archivo Excel");
                return View();
            }

            // Validate file extension
            var extension = Path.GetExtension(excelFile.FileName).ToLower();
            if (extension != ".xlsx" && extension != ".xls")
            {
                ModelState.AddModelError("excelFile", "Solo se permiten archivos Excel (.xlsx, .xls)");
                return View();
            }

            // Validate file size (max 10 MB)
            if (excelFile.Length > 10 * 1024 * 1024)
            {
                ModelState.AddModelError("excelFile", "El archivo no debe superar los 10 MB");
                return View();
            }

            try
            {
                using var stream = new MemoryStream();
                await excelFile.CopyToAsync(stream);
                stream.Position = 0;

                var importService = HttpContext.RequestServices.GetRequiredService<IExcelImportService>();
                var result = await importService.ImportEmployeesFromExcelAsync(stream);

                if (result.Success)
                {
                    TempData["Success"] = result.FailedImports == 0
                        ? $"Importación completada: {result.SuccessfulImports} nuevos, {result.UpdatedRecords} actualizados"
                        : $"Importación parcial: {result.SuccessfulImports} exitosos, {result.FailedImports} fallidos";
                }

                ViewBag.ImportResult = result;
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error inesperado: {ex.Message}");
                return View();
            }
        }

        private async Task LoadDepartments()
        {
            var departments = await _departmentService.GetAllDepartmentsAsync();
            ViewBag.Departments = new SelectList(departments, "Id", "Name");
        }
    }
}

