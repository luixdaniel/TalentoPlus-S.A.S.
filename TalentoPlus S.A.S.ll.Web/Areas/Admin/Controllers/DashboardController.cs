using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentoPlus_S.A.S.ll.Web.Data.Entities;
using TalentoPlus_S.A.S.ll.Web.Models.ViewModels;
using TalentoPlus_S.A.S.ll.Web.Services;

namespace TalentoPlus_S.A.S.ll.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IDepartmentService _departmentService;
        private readonly IAiQueryService _aiQueryService;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(
            IEmployeeService employeeService,
            IDepartmentService departmentService,
            IAiQueryService aiQueryService,
            ILogger<DashboardController> logger)
        {
            _employeeService = employeeService;
            _departmentService = departmentService;
            _aiQueryService = aiQueryService;
            _logger = logger;
        }

        // GET: Admin/Dashboard
        public async Task<IActionResult> Index()
        {
            try
            {
                var employees = await _employeeService.GetAllEmployeesAsync();
                var departments = await _departmentService.GetAllDepartmentsAsync();

                var viewModel = new DashboardViewModel
                {
                    TotalEmployees = employees.Count(),
                    EmployeesOnVacation = employees.Count(e => e.Status == EmployeeStatus.Vacation),
                    ActiveEmployees = employees.Count(e => e.Status == EmployeeStatus.Active),
                    InactiveEmployees = employees.Count(e => e.Status == EmployeeStatus.Inactive),
                    DepartmentStatistics = departments.Select(d =>
                    {
                        var count = employees.Count(e => e.DepartmentId == d.Id);
                        return new DepartmentStatistic
                        {
                            DepartmentName = d.Name,
                            EmployeeCount = count,
                            Percentage = employees.Any() ? (decimal)count / employees.Count() * 100 : 0
                        };
                    }).OrderByDescending(d => d.EmployeeCount).ToList()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar el dashboard");
                TempData["Error"] = "Error al cargar las estadísticas del dashboard";
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Admin/Dashboard/AskAi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AskAi([FromBody] AiQueryRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Query))
                {
                    return Json(new AiQueryResponse
                    {
                        Success = false,
                        Error = "La consulta no puede estar vacía",
                        Answer = "Por favor, escribe una pregunta."
                    });
                }

                var response = await _aiQueryService.ProcessQueryAsync(request.Query);
                return Json(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar consulta de IA");
                return Json(new AiQueryResponse
                {
                    Success = false,
                    Error = ex.Message,
                    Answer = "Lo siento, ocurrió un error al procesar tu consulta."
                });
            }
        }
    }
}
