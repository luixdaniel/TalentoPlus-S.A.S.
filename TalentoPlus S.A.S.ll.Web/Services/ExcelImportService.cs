using OfficeOpenXml;
using TalentoPlus_S.A.S.ll.Web.Data.Entities;
using TalentoPlus_S.A.S.ll.Web.Models.ImportExcel;
using TalentoPlus_S.A.S.ll.Web.Repositories;

namespace TalentoPlus_S.A.S.ll.Web.Services
{
    public class ExcelImportService : IExcelImportService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly ILogger<ExcelImportService> _logger;


        public ExcelImportService(
            IEmployeeRepository employeeRepository,
            IDepartmentRepository departmentRepository,
            ILogger<ExcelImportService> logger)
        {
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRepository;
            _logger = logger;
            
            // Set EPPlus license context
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public Task<ImportResult> ValidateExcelStructureAsync(Stream fileStream)
        {
            var result = new ImportResult { Success = true };

            try
            {
                using var package = new ExcelPackage(fileStream);
                var worksheet = package.Workbook.Worksheets.FirstOrDefault();

                if (worksheet == null)
                {
                    result.Success = false;
                    result.Errors.Add("El archivo Excel no contiene hojas de trabajo");
                    return Task.FromResult(result);
                }

                // Read headers from row 1 and create mapping directly from worksheet
                var maxCol = worksheet.Dimension?.Columns ?? 0;
                var mapeo = MapeoColumnasExcel.CrearDesdeWorksheet(worksheet, maxCol);
                
                // Count only non-empty headers for reporting
                int headerCount = 0;
                for (int col = 1; col <= maxCol; col++)
                {
                    var headerValue = worksheet.Cells[1, col].Value?.ToString()?.Trim();
                    if (!string.IsNullOrWhiteSpace(headerValue))
                    {
                        headerCount++;
                    }
                }
                result.Warnings.Add($"Se detectaron {headerCount} columnas en el archivo");

                // Validate that all required columns are present
                if (!mapeo.EsValido())
                {
                    result.Success = false;
                    var faltantes = mapeo.ObtenerColumnasFaltantes();
                    result.Errors.Add($"Faltan columnas requeridas: {string.Join(", ", faltantes)}");
                    return Task.FromResult(result);
                }

                result.TotalRows = worksheet.Dimension?.Rows - 1 ?? 0; // Exclude header row

                if (result.TotalRows == 0)
                {
                    result.Success = false;
                    result.Errors.Add("El archivo Excel no contiene datos de empleados");
                }
                // El conteo de columnas ya se hace en el mapeo
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Errors.Add($"Error al validar el archivo: {ex.Message}");
                _logger.LogError(ex, "Error validating Excel file");
            }

            return Task.FromResult(result);
        }

        public async Task<ImportResult> ImportEmployeesFromExcelAsync(Stream fileStream)
        {
            var result = new ImportResult();

            try
            {
                // First validate structure
                fileStream.Position = 0;
                var validationResult = await ValidateExcelStructureAsync(fileStream);
                
                if (!validationResult.Success)
                {
                    return validationResult;
                }

                result.Warnings = validationResult.Warnings;

                // Load departments for mapping
                var departments = (await _departmentRepository.GetAllAsync()).ToList();
                var departmentDict = departments.ToDictionary(d => d.Name.ToLower(), d => d);

                fileStream.Position = 0;
                using var package = new ExcelPackage(fileStream);
                var worksheet = package.Workbook.Worksheets.First();

                // Create column mapping directly from worksheet
                var maxCol = worksheet.Dimension?.Columns ?? 0;
                var mapeo = MapeoColumnasExcel.CrearDesdeWorksheet(worksheet, maxCol);

                var rowCount = worksheet.Dimension.Rows;
                result.TotalRows = rowCount - 1; // Exclude header

                // Process each row (starting from row 2, row 1 is headers)
                for (int row = 2; row <= rowCount; row++)
                {
                    try
                    {
                        var employee = await ProcessEmployeeRowAsync(worksheet, row, mapeo, departmentDict);
                        
                        if (employee != null)
                        {
                            // Check if employee exists by email
                            var existingEmployee = await _employeeRepository.GetAllAsync();
                            var existing = existingEmployee.FirstOrDefault(e => e.Email.Equals(employee.Email, StringComparison.OrdinalIgnoreCase));

                            if (existing != null)
                            {
                                // Update existing employee
                                existing.FirstName = employee.FirstName;
                                existing.LastName = employee.LastName;
                                existing.Phone = employee.Phone;
                                existing.Address = employee.Address;
                                existing.BirthDate = employee.BirthDate;
                                existing.HireDate = employee.HireDate;
                                existing.Position = employee.Position;
                                existing.Salary = employee.Salary;
                                existing.Status = employee.Status;
                                existing.EducationLevel = employee.EducationLevel;
                                existing.DepartmentId = employee.DepartmentId;
                                existing.ProfessionalProfile = employee.ProfessionalProfile;

                                await _employeeRepository.UpdateAsync(existing);
                                result.UpdatedRecords++;
                                result.ImportedEmployees.Add(existing);
                            }
                            else
                            {
                                // Create new employee
                                await _employeeRepository.AddAsync(employee);
                                result.SuccessfulImports++;
                                result.ImportedEmployees.Add(employee);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        result.FailedImports++;
                        result.Errors.Add($"Fila {row}: {ex.Message}");
                        _logger.LogError(ex, $"Error processing row {row}");
                    }
                }

                // Save all changes
                await _employeeRepository.SaveChangesAsync();
                result.Success = result.FailedImports == 0 || result.SuccessfulImports > 0 || result.UpdatedRecords > 0;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Errors.Add($"Error general al importar: {ex.Message}");
                _logger.LogError(ex, "Error importing Excel file");
            }

            return result;
        }

        private Task<Employee?> ProcessEmployeeRowAsync(
            ExcelWorksheet worksheet, 
            int row, 
            MapeoColumnasExcel mapeo,
            Dictionary<string, Department> departmentDict)
        {
            var employee = new Employee();

            // Optional Document Number
            if (mapeo.ColumnaDocumento.HasValue)
            {
                employee.DocumentNumber = GetCellValue(worksheet, row, mapeo.ColumnaDocumento.Value)?.Trim();
            }

            // Required fields usando el mapeo dinámico
            employee.FirstName = GetCellValue(worksheet, row, mapeo.ColumnaNombres!.Value)?.Trim() 
                ?? throw new Exception("El campo 'Nombres' es obligatorio");
            
            employee.LastName = GetCellValue(worksheet, row, mapeo.ColumnaApellidos!.Value)?.Trim() 
                ?? throw new Exception("El campo 'Apellidos' es obligatorio");
            
            employee.Email = GetCellValue(worksheet, row, mapeo.ColumnaEmail!.Value)?.Trim() 
                ?? throw new Exception("El campo 'Email' es obligatorio");
            
            // Validate email format
            if (!IsValidEmail(employee.Email))
            {
                throw new Exception($"Email inválido: {employee.Email}");
            }

            employee.Phone = GetCellValue(worksheet, row, mapeo.ColumnaTelefono!.Value)?.Trim() 
                ?? throw new Exception("El campo 'Teléfono' es obligatorio");
            
            employee.Address = GetCellValue(worksheet, row, mapeo.ColumnaDireccion!.Value)?.Trim() 
                ?? throw new Exception("El campo 'Dirección' es obligatoria");

            // Dates
            employee.BirthDate = ParseDate(worksheet, row, mapeo.ColumnaFechaNacimiento!.Value, "Fecha de Nacimiento");
            employee.HireDate = ParseDate(worksheet, row, mapeo.ColumnaFechaIngreso!.Value, "Fecha de Ingreso");

            employee.Position = GetCellValue(worksheet, row, mapeo.ColumnaCargo!.Value)?.Trim() 
                ?? throw new Exception("El campo 'Cargo' es obligatorio");

            // Salary
            var salaryStr = GetCellValue(worksheet, row, mapeo.ColumnaSalario!.Value);
            if (string.IsNullOrEmpty(salaryStr) || !decimal.TryParse(salaryStr.Replace("$", "").Replace(",", ""), out var salary))
            {
                throw new Exception("El campo 'Salario' debe ser un número válido");
            }
            employee.Salary = salary;

            // Status
            var statusStr = GetCellValue(worksheet, row, mapeo.ColumnaEstado!.Value)?.Trim().ToLower();
            employee.Status = statusStr switch
            {
                "activo" => EmployeeStatus.Active,
                "inactivo" => EmployeeStatus.Inactive,
                "vacaciones" => EmployeeStatus.Vacation,
                _ => throw new Exception($"Estado inválido: {statusStr}. Debe ser 'Activo', 'Inactivo' o 'Vacaciones'")
            };

            // Education Level
            var educationStr = GetCellValue(worksheet, row, mapeo.ColumnaNivelEducativo!.Value)?.Trim().ToLower();
            employee.EducationLevel = educationStr switch
            {
                "profesional" => EducationLevel.Professional,
                "técnico" or "tecnico" => EducationLevel.Technical,
                "tecnólogo" or "tecnologo" => EducationLevel.Technologist,
                "maestría" or "maestria" => EducationLevel.Master,
                "especialización" or "especializacion" => EducationLevel.Specialization,
                _ => throw new Exception($"Nivel educativo inválido: {educationStr}")
            };

            // Department
            var departmentName = GetCellValue(worksheet, row, mapeo.ColumnaDepartamento!.Value)?.Trim().ToLower();
            if (string.IsNullOrEmpty(departmentName))
            {
                throw new Exception("El campo 'Departamento' es obligatorio");
            }

            if (!departmentDict.TryGetValue(departmentName, out var department))
            {
                throw new Exception($"Departamento no encontrado: {departmentName}");
            }
            employee.DepartmentId = department.Id;

            // Optional field
            if (mapeo.ColumnaPerfilProfesional.HasValue)
            {
                employee.ProfessionalProfile = GetCellValue(worksheet, row, mapeo.ColumnaPerfilProfesional.Value)?.Trim();
            }

            return Task.FromResult<Employee?>(employee);
        }

        private string? GetCellValue(ExcelWorksheet worksheet, int row, int col)
        {
            return worksheet.Cells[row, col].Value?.ToString();
        }

        private DateTime ParseDate(ExcelWorksheet worksheet, int row, int col, string fieldName)
        {
            var cellValue = worksheet.Cells[row, col].Value;
            
            if (cellValue == null)
            {
                throw new Exception($"El campo '{fieldName}' es obligatorio");
            }

            // Try to parse as DateTime directly (Excel stores dates as numbers)
            if (cellValue is DateTime dateTime)
            {
                return dateTime;
            }

            // Try to parse as double (Excel serial date)
            if (cellValue is double doubleValue)
            {
                return DateTime.FromOADate(doubleValue);
            }

            // Try to parse as string
            var stringValue = cellValue.ToString();
            if (DateTime.TryParse(stringValue, out var parsedDate))
            {
                return parsedDate;
            }

            throw new Exception($"El campo '{fieldName}' no tiene un formato de fecha válido");
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}

