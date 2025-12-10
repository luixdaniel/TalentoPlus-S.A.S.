using OfficeOpenXml;
using TalentoPlus_S.A.S.ll.Web.Data.Entities;
using TalentoPlus_S.A.S.ll.Web.Repositories;

namespace TalentoPlus_S.A.S.ll.Web.Services
{
    public class ExcelImportService : IExcelImportService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly ILogger<ExcelImportService> _logger;

        // Expected Excel column headers
        private readonly Dictionary<string, int> _columnMapping = new()
        {
            { "Nombres", 0 },
            { "Apellidos", 1 },
            { "Email", 2 },
            { "Teléfono", 3 },
            { "Dirección", 4 },
            { "Fecha de Nacimiento", 5 },
            { "Fecha de Ingreso", 6 },
            { "Cargo", 7 },
            { "Salario", 8 },
            { "Estado", 9 },
            { "Nivel Educativo", 10 },
            { "Departamento", 11 },
            { "Perfil Profesional", 12 }
        };

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

        public async Task<ImportResult> ValidateExcelStructureAsync(Stream fileStream)
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
                    return result;
                }

                // Validate headers (row 1)
                var expectedHeaders = _columnMapping.Keys.ToList();
                for (int col = 1; col <= expectedHeaders.Count; col++)
                {
                    var headerValue = worksheet.Cells[1, col].Value?.ToString()?.Trim();
                    var expectedHeader = expectedHeaders[col - 1];

                    if (string.IsNullOrEmpty(headerValue) || !headerValue.Equals(expectedHeader, StringComparison.OrdinalIgnoreCase))
                    {
                        result.Warnings.Add($"Columna {col}: Se esperaba '{expectedHeader}' pero se encontró '{headerValue}'");
                    }
                }

                result.TotalRows = worksheet.Dimension?.Rows - 1 ?? 0; // Exclude header row

                if (result.TotalRows == 0)
                {
                    result.Success = false;
                    result.Errors.Add("El archivo Excel no contiene datos de empleados");
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Errors.Add($"Error al validar el archivo: {ex.Message}");
                _logger.LogError(ex, "Error validating Excel file");
            }

            return result;
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

                var rowCount = worksheet.Dimension.Rows;
                result.TotalRows = rowCount - 1; // Exclude header

                // Process each row (starting from row 2, row 1 is headers)
                for (int row = 2; row <= rowCount; row++)
                {
                    try
                    {
                        var employee = await ProcessEmployeeRowAsync(worksheet, row, departmentDict);
                        
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

        private async Task<Employee?> ProcessEmployeeRowAsync(
            ExcelWorksheet worksheet, 
            int row, 
            Dictionary<string, Department> departmentDict)
        {
            var employee = new Employee();

            // Required fields
            employee.FirstName = GetCellValue(worksheet, row, 1)?.Trim() 
                ?? throw new Exception("El campo 'Nombres' es obligatorio");
            
            employee.LastName = GetCellValue(worksheet, row, 2)?.Trim() 
                ?? throw new Exception("El campo 'Apellidos' es obligatorio");
            
            employee.Email = GetCellValue(worksheet, row, 3)?.Trim() 
                ?? throw new Exception("El campo 'Email' es obligatorio");
            
            // Validate email format
            if (!IsValidEmail(employee.Email))
            {
                throw new Exception($"Email inválido: {employee.Email}");
            }

            employee.Phone = GetCellValue(worksheet, row, 4)?.Trim() 
                ?? throw new Exception("El campo 'Teléfono' es obligatorio");
            
            employee.Address = GetCellValue(worksheet, row, 5)?.Trim() 
                ?? throw new Exception("El campo 'Dirección' es obligatorio");

            // Dates
            employee.BirthDate = ParseDate(worksheet, row, 6, "Fecha de Nacimiento");
            employee.HireDate = ParseDate(worksheet, row, 7, "Fecha de Ingreso");

            employee.Position = GetCellValue(worksheet, row, 8)?.Trim() 
                ?? throw new Exception("El campo 'Cargo' es obligatorio");

            // Salary
            var salaryStr = GetCellValue(worksheet, row, 9);
            if (string.IsNullOrEmpty(salaryStr) || !decimal.TryParse(salaryStr.Replace("$", "").Replace(",", ""), out var salary))
            {
                throw new Exception("El campo 'Salario' debe ser un número válido");
            }
            employee.Salary = salary;

            // Status
            var statusStr = GetCellValue(worksheet, row, 10)?.Trim().ToLower();
            employee.Status = statusStr switch
            {
                "activo" => EmployeeStatus.Active,
                "inactivo" => EmployeeStatus.Inactive,
                "vacaciones" => EmployeeStatus.Vacation,
                _ => throw new Exception($"Estado inválido: {statusStr}. Debe ser 'Activo', 'Inactivo' o 'Vacaciones'")
            };

            // Education Level
            var educationStr = GetCellValue(worksheet, row, 11)?.Trim().ToLower();
            employee.EducationLevel = educationStr switch
            {
                "profesional" => EducationLevel.Professional,
                "tecnólogo" or "tecnologo" => EducationLevel.Technologist,
                "maestría" or "maestria" => EducationLevel.Master,
                "especialización" or "especializacion" => EducationLevel.Specialization,
                _ => throw new Exception($"Nivel educativo inválido: {educationStr}")
            };

            // Department
            var departmentName = GetCellValue(worksheet, row, 12)?.Trim().ToLower();
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
            employee.ProfessionalProfile = GetCellValue(worksheet, row, 13)?.Trim();

            return employee;
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

