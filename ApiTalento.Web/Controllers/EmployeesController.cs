using System.Security.Claims;
using ApiTalento.Web.DTOs;
using ApiTalento.Web.Mappings;
using ApiTalento.Web.Models.ViewModels;
using ApiTalento.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ApiTalento.Web.Data.Entities;
using ApiTalento.Web.Repositories;

namespace ApiTalento.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IPdfService _pdfService;
        private readonly ILogger<EmployeesController> _logger;

        public EmployeesController(
            IEmployeeRepository employeeRepository,
            IPdfService pdfService,
            ILogger<EmployeesController> logger)
        {
            _employeeRepository = employeeRepository;
            _pdfService = pdfService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene la información del empleado autenticado
        /// </summary>
        /// <returns>Información del empleado</returns>
        [HttpGet("me")]
        [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EmployeeDto>> GetMyInfo()
        {
            try
            {
                // Obtener el ID del empleado desde el token JWT
                var employeeIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(employeeIdClaim) || !int.TryParse(employeeIdClaim, out int employeeId))
                {
                    return Unauthorized(new { message = "Token inválido" });
                }

                var employee = await _employeeRepository.GetByIdAsync(employeeId);
                if (employee == null)
                {
                    return NotFound(new { message = "Empleado no encontrado" });
                }

                var employeeDto = employee.ToDto();
                return Ok(employeeDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener información del empleado");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Descarga la hoja de vida en PDF del empleado autenticado
        /// </summary>
        /// <returns>Archivo PDF</returns>
        [HttpGet("me/resume")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DownloadMyResume()
        {
            try
            {
                // Obtener el ID del empleado desde el token JWT
                var employeeIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(employeeIdClaim) || !int.TryParse(employeeIdClaim, out int employeeId))
                {
                    return Unauthorized(new { message = "Token inválido" });
                }

                var employee = await _employeeRepository.GetByIdAsync(employeeId);
                if (employee == null)
                {
                    return NotFound(new { message = "Empleado no encontrado" });
                }

                // Convertir a DTO
                var employeeDto = employee.ToDto();

                // Generar PDF
                var pdfBytes = _pdfService.GenerateEmployeeResumePdf(employeeDto);
                var fileName = $"HV_{employee.FirstName}_{employee.LastName}_{DateTime.Now:yyyyMMdd}.pdf";

                _logger.LogInformation($"PDF generado para empleado {employee.Email}");

                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar PDF del empleado");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}

