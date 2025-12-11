using System.Text;
using System.Text.Json;
using TalentoPlus_S.A.S.ll.Web.Data.Entities;
using TalentoPlus_S.A.S.ll.Web.Models.ViewModels;

namespace TalentoPlus_S.A.S.ll.Web.Services
{
    public class GeminiAiService : IAiQueryService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IEmployeeService _employeeService;
        private readonly IDepartmentService _departmentService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<GeminiAiService> _logger;

        public GeminiAiService(
            IHttpClientFactory httpClientFactory,
            IEmployeeService employeeService,
            IDepartmentService departmentService,
            IConfiguration configuration,
            ILogger<GeminiAiService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _employeeService = employeeService;
            _departmentService = departmentService;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<AiQueryResponse> ProcessQueryAsync(string query)
        {
            try
            {
                // Obtener datos actuales
                var employees = await _employeeService.GetAllEmployeesAsync();
                var departments = await _departmentService.GetAllDepartmentsAsync();

                // Procesar consulta localmente (modo demostración)
                var result = ProcessQueryLocally(query, employees, departments);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error procesando consulta de IA");
                return new AiQueryResponse
                {
                    Success = false,
                    Error = "Error al procesar la consulta. Por favor, intenta de nuevo.",
                    Answer = "Lo siento, no pude procesar tu consulta en este momento."
                };
            }
        }

        private AiQueryResponse ProcessQueryLocally(string query, IEnumerable<Employee> employees, IEnumerable<Department> departments)
        {
            var queryLower = query.ToLower();
            var answer = "";
            object? additionalData = null;

            // Estadísticas generales
            var totalEmployees = employees.Count();
            var activeCount = employees.Count(e => e.Status == EmployeeStatus.Active);
            var inactiveCount = employees.Count(e => e.Status == EmployeeStatus.Inactive);
            var vacationCount = employees.Count(e => e.Status == EmployeeStatus.Vacation);

            // Consultas sobre total de empleados
            if (queryLower.Contains("cuántos empleados") && (queryLower.Contains("total") || queryLower.Contains("hay")))
            {
                answer = $"Actualmente hay {totalEmployees} empleados registrados en el sistema.";
            }
            // Consultas sobre empleados activos
            else if (queryLower.Contains("activo"))
            {
                answer = $"Hay {activeCount} empleados activos en el sistema.";
            }
            // Consultas sobre empleados inactivos
            else if (queryLower.Contains("inactivo"))
            {
                answer = $"Hay {inactiveCount} empleados inactivos en el sistema.";
            }
            // Consultas sobre empleados en vacaciones
            else if (queryLower.Contains("vacacion"))
            {
                answer = $"Hay {vacationCount} empleados en vacaciones actualmente.";
            }
            // Consultas por departamento
            else
            {
                foreach (var dept in departments)
                {
                    if (queryLower.Contains(dept.Name.ToLower()))
                    {
                        var deptEmployees = employees.Where(e => e.DepartmentId == dept.Id).ToList();
                        answer = $"El departamento de {dept.Name} tiene {deptEmployees.Count} empleados.";
                        
                        additionalData = new
                        {
                            DepartmentName = dept.Name,
                            EmployeeCount = deptEmployees.Count,
                            Employees = deptEmployees.Select(e => new
                            {
                                e.FirstName,
                                e.LastName,
                                e.Position,
                                Status = e.Status.ToString()
                            }).ToList()
                        };
                        break;
                    }
                }
            }

            // Consultas por cargo/posición
            if (string.IsNullOrEmpty(answer))
            {
                var positionStats = employees
                    .GroupBy(e => e.Position.ToLower())
                    .Select(g => new { Position = g.First().Position, Count = g.Count() })
                    .ToList();

                foreach (var pos in positionStats)
                {
                    if (queryLower.Contains(pos.Position.ToLower()))
                    {
                        answer = $"Hay {pos.Count} {pos.Position}(s) en la plataforma.";
                        break;
                    }
                }
            }

            // Respuesta por defecto si no se encontró coincidencia
            if (string.IsNullOrEmpty(answer))
            {
                answer = $@"Basándome en los datos actuales del sistema:
- Total de empleados: {totalEmployees}
- Empleados activos: {activeCount}
- Empleados inactivos: {inactiveCount}
- Empleados en vacaciones: {vacationCount}

Puedes preguntarme sobre departamentos específicos, cargos, o estados de empleados.";
            }

            return new AiQueryResponse
            {
                Success = true,
                Answer = answer,
                Data = additionalData
            };
        }

        private string BuildContext(IEnumerable<Employee> employees, IEnumerable<Department> departments)
        {
            var employeeCount = employees.Count();
            var activeCount = employees.Count(e => e.Status == EmployeeStatus.Active);
            var inactiveCount = employees.Count(e => e.Status == EmployeeStatus.Inactive);
            var vacationCount = employees.Count(e => e.Status == EmployeeStatus.Vacation);

            var departmentStats = departments.Select(d => new
            {
                Name = d.Name,
                Count = employees.Count(e => e.DepartmentId == d.Id)
            }).ToList();

            var positionStats = employees
                .GroupBy(e => e.Position)
                .Select(g => new { Position = g.Key, Count = g.Count() })
                .ToList();

            return $@"
Contexto de la base de datos de empleados:

ESTADÍSTICAS GENERALES:
- Total de empleados: {employeeCount}
- Empleados activos: {activeCount}
- Empleados inactivos: {inactiveCount}
- Empleados en vacaciones: {vacationCount}

DEPARTAMENTOS:
{string.Join("\n", departmentStats.Select(d => $"- {d.Name}: {d.Count} empleados"))}

CARGOS/POSICIONES:
{string.Join("\n", positionStats.Select(p => $"- {p.Position}: {p.Count} empleados"))}

ESTADOS POSIBLES:
- Active (Activo)
- Inactive (Inactivo)
- Vacation (Vacaciones)

NIVELES EDUCATIVOS POSIBLES:
- Professional (Profesional)
- Technical (Técnico)
- Technologist (Tecnólogo)
- Master (Maestría)
- Specialization (Especialización)
";
        }

        private async Task<string> CallGeminiApiAsync(string query, string context)
        {
            var apiKey = _configuration["Gemini:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new InvalidOperationException("Gemini API Key no configurada");
            }

            var client = _httpClientFactory.CreateClient();
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={apiKey}";

            var prompt = $@"
Eres un asistente de análisis de datos para un sistema de gestión de empleados.

{context}

INSTRUCCIONES:
1. Analiza la pregunta del usuario
2. Basándote ÚNICAMENTE en los datos proporcionados arriba, responde la pregunta
3. NO inventes datos
4. Si la pregunta no se puede responder con los datos disponibles, di que no tienes esa información
5. Sé conciso y directo
6. Usa números exactos de los datos proporcionados

PREGUNTA DEL USUARIO:
{query}

RESPUESTA:";

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(url, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Error de Gemini API: {responseContent}");
                throw new Exception($"Error al llamar a Gemini API: {response.StatusCode}");
            }

            // Parsear respuesta de Gemini
            using var doc = JsonDocument.Parse(responseContent);
            var text = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            return text ?? "No se pudo obtener respuesta";
        }

        private async Task<AiQueryResponse> ProcessGeminiResponse(
            string geminiResponse,
            IEnumerable<Employee> employees,
            IEnumerable<Department> departments)
        {
            // Extraer datos adicionales si es necesario
            object? additionalData = null;

            // Detectar si la pregunta es sobre un departamento específico
            var queryLower = geminiResponse.ToLower();
            foreach (var dept in departments)
            {
                if (queryLower.Contains(dept.Name.ToLower()))
                {
                    var deptEmployees = employees.Where(e => e.DepartmentId == dept.Id).ToList();
                    additionalData = new
                    {
                        DepartmentName = dept.Name,
                        EmployeeCount = deptEmployees.Count,
                        Employees = deptEmployees.Select(e => new
                        {
                            e.FirstName,
                            e.LastName,
                            e.Position,
                            Status = e.Status.ToString()
                        }).ToList()
                    };
                    break;
                }
            }

            return new AiQueryResponse
            {
                Success = true,
                Answer = geminiResponse,
                Data = additionalData
            };
        }
    }
}
