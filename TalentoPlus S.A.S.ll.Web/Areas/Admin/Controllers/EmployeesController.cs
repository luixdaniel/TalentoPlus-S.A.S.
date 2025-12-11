using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TalentoPlus_S.A.S.ll.Web.Data.Entities;
using TalentoPlus_S.A.S.ll.Web.Services;
using TalentoPlus_S.A.S.ll.Web.Models.ViewModels;
using TalentoPlus_S.A.S.ll.Web.Extensions;

namespace TalentoPlus_S.A.S.ll.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class EmployeesController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IDepartmentService _departmentService;
        private readonly IPdfService _pdfService;

        public EmployeesController(IEmployeeService employeeService, IDepartmentService departmentService, IPdfService pdfService)
        {
            _employeeService = employeeService;
            _departmentService = departmentService;
            _pdfService = pdfService;
        }

        // GET: Admin/Employees
        public async Task<IActionResult> Index()
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            var viewModels = employees.ToViewModelList();
            return View(viewModels);
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

            var viewModel = employee.ToViewModel();
            return View(viewModel);
        }

        // GET: Admin/Employees/Create
        public async Task<IActionResult> Create()
        {
            await LoadDepartments();
            return View(new EmployeeViewModel());
        }

        // POST: Admin/Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                await LoadDepartments();
                return View(viewModel);
            }

            try
            {
                var employee = viewModel.ToEntity();
                await _employeeService.CreateEmployeeAsync(employee);
                TempData["Success"] = "Empleado creado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al crear el empleado: {ex.Message}");
                await LoadDepartments();
                return View(viewModel);
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

            var viewModel = employee.ToViewModel();
            await LoadDepartments();
            return View(viewModel);
        }

        // POST: Admin/Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EmployeeViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                await LoadDepartments();
                return View(viewModel);
            }

            try
            {
                var employee = await _employeeService.GetEmployeeByIdAsync(id);
                if (employee == null)
                {
                    return NotFound();
                }

                employee.UpdateEntity(viewModel);
                await _employeeService.UpdateEmployeeAsync(employee);
                TempData["Success"] = "Empleado actualizado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al actualizar el empleado: {ex.Message}");
                await LoadDepartments();
                return View(viewModel);
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

            var viewModel = employee.ToViewModel();
            return View(viewModel);
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

        // GET: Admin/Employees/GenerateResumePdf/5
        public async Task<IActionResult> GenerateResumePdf(int? id)
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

            try
            {
                var viewModel = employee.ToViewModel();
                var pdfBytes = _pdfService.GenerateEmployeeResumePdf(viewModel);
                
                var fileName = $"HV_{employee.FirstName}_{employee.LastName}_{DateTime.Now:yyyyMMdd}.pdf";
                
                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al generar el PDF: {ex.Message}";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        private async Task LoadDepartments()
        {
            var departments = await _departmentService.GetAllDepartmentsAsync();
            ViewBag.Departments = new SelectList(departments, "Id", "Name");
        }
    }
}

