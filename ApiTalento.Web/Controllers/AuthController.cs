using ApiTalento.Web.DTOs;
using ApiTalento.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentoPlus_S.A.S.ll.Web.Data.Entities;
using TalentoPlus_S.A.S.ll.Web.Repositories;

namespace ApiTalento.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IEmailService _emailService;
        private readonly IJwtService _jwtService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IEmployeeRepository employeeRepository,
            IDepartmentRepository departmentRepository,
            IEmailService emailService,
            IJwtService jwtService,
            ILogger<AuthController> logger)
        {
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRepository;
            _emailService = emailService;
            _jwtService = jwtService;
            _logger = logger;
        }

        /// <summary>
        /// Registra un nuevo empleado en el sistema
        /// </summary>
        /// <param name="registerDto">Datos del empleado a registrar</param>
        /// <returns>Confirmación del registro</returns>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Register([FromBody] EmployeeRegisterDto registerDto)
        {
            try
            {
                // Validar que el departamento existe
                var department = await _departmentRepository.GetByIdAsync(registerDto.DepartmentId);
                if (department == null)
                {
                    return BadRequest(new { message = "El departamento especificado no existe" });
                }

                // Validar que no existe otro empleado con el mismo documento
                var employees = await _employeeRepository.GetAllAsync();
                if (employees.Any(e => e.DocumentNumber == registerDto.DocumentNumber))
                {
                    return BadRequest(new { message = "Ya existe un empleado con ese número de documento" });
                }

                // Validar que no existe otro empleado con el mismo email
                if (employees.Any(e => e.Email == registerDto.Email))
                {
                    return BadRequest(new { message = "Ya existe un empleado con ese correo electrónico" });
                }

                // Crear el nuevo empleado
                var employee = new Employee
                {
                    DocumentNumber = registerDto.DocumentNumber,
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    BirthDate = registerDto.BirthDate,
                    Address = registerDto.Address,
                    Phone = registerDto.Phone,
                    Email = registerDto.Email,
                    Position = registerDto.Position,
                    Salary = 0, // Será asignado por el administrador
                    HireDate = DateTime.Now,
                    Status = EmployeeStatus.Inactive, // Inactivo hasta que el admin lo apruebe
                    EducationLevel = (EducationLevel)registerDto.EducationLevel,
                    ProfessionalProfile = registerDto.ProfessionalProfile,
                    DepartmentId = registerDto.DepartmentId
                };

                await _employeeRepository.AddAsync(employee);

                // Enviar correo de bienvenida
                try
                {
                    await _emailService.SendWelcomeEmailAsync(
                        employee.Email,
                        $"{employee.FirstName} {employee.LastName}"
                    );
                    _logger.LogInformation($"Correo de bienvenida enviado a {employee.Email}");
                }
                catch (Exception emailEx)
                {
                    _logger.LogError(emailEx, $"Error al enviar correo de bienvenida a {employee.Email}");
                    // No fallar el registro si el correo falla
                }

                return CreatedAtAction(
                    nameof(EmployeesController.GetMyInfo),
                    "Employees",
                    null,
                    new
                    {
                        message = "Registro exitoso. Se ha enviado un correo de confirmación.",
                        employeeId = employee.Id,
                        email = employee.Email
                    }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar empleado");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Autentica un empleado y devuelve un token JWT
        /// </summary>
        /// <param name="loginDto">Credenciales del empleado</param>
        /// <returns>Token JWT y datos del empleado</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var employees = await _employeeRepository.GetAllAsync();
                var employee = employees.FirstOrDefault(e =>
                    e.DocumentNumber == loginDto.DocumentNumber &&
                    e.Email == loginDto.Email);

                if (employee == null)
                {
                    return Unauthorized(new { message = "Credenciales inválidas" });
                }

                // Generar token JWT
                var token = _jwtService.GenerateToken(
                    employee.Id,
                    employee.Email,
                    employee.DocumentNumber ?? string.Empty
                );

                var response = new LoginResponseDto
                {
                    Token = token,
                    Email = employee.Email,
                    FullName = $"{employee.FirstName} {employee.LastName}",
                    ExpiresAt = DateTime.UtcNow.AddHours(24)
                };

                _logger.LogInformation($"Login exitoso para empleado {employee.Email}");

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al autenticar empleado");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}

