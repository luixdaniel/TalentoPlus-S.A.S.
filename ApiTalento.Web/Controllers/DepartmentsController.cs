using ApiTalento.Web.DTOs;
using ApiTalento.Web.Mappings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ApiTalento.Web.Repositories;

namespace ApiTalento.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly ILogger<DepartmentsController> _logger;

        public DepartmentsController(
            IDepartmentRepository departmentRepository,
            ILogger<DepartmentsController> logger)
        {
            _departmentRepository = departmentRepository;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene la lista de todos los departamentos
        /// </summary>
        /// <returns>Lista de departamentos</returns>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<DepartmentDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<DepartmentDto>>> GetDepartments()
        {
            try
            {
                var departments = await _departmentRepository.GetAllAsync();
                var departmentDtos = departments.Select(d => d.ToDto()).ToList();

                return Ok(departmentDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener departamentos");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}

