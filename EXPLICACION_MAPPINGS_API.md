# 📁 Carpeta Mappings - API REST

## 🎯 ¿Por qué existe la carpeta Mappings?

La carpeta **Mappings** contiene **métodos de conversión (mappers)** que transforman:
- **Entidades de BD** (Employee, Department) → **DTOs** (Data Transfer Objects)

---

## 🤔 ¿Por qué no usar Extensions como en el proyecto Web?

### **Proyecto Web:**
```
Entidades ↔️ ViewModels (para vistas Razor)
Extensions/ (compartidos entre vistas y controllers)
```

### **Proyecto API:**
```
Entidades → DTOs (para respuestas JSON)
Mappings/ (específicos para la API)
```

**Razón:** 
- Los **ViewModels** están diseñados para vistas HTML/Razor
- Los **DTOs** están diseñados para respuestas JSON de la API
- Son propósitos diferentes, requieren conversiones diferentes

---

## 📂 Archivos en Mappings/

### 1. **EmployeeMapper.cs**

Convierte `Employee` (BD) → `EmployeeDto` (JSON)

```csharp
public static EmployeeDto ToDto(this Employee employee)
{
    return new EmployeeDto
    {
        Id = employee.Id,
        FirstName = employee.FirstName,
        Status = GetStatusDisplay(employee.Status), // Convierte enum a texto
        // ... más propiedades
    };
}
```

**Características especiales:**
- ✅ Convierte enums a strings legibles (ej: `EmployeeStatus.Active` → "Activo")
- ✅ Maneja valores nulos de forma segura
- ✅ Incluye propiedades calculadas (ej: `FullName`)
- ✅ Oculta propiedades sensibles si es necesario

---

### 2. **DepartmentMapper.cs**

Convierte `Department` (BD) → `DepartmentDto` (JSON)

```csharp
public static DepartmentDto ToDto(this Department department)
{
    return new DepartmentDto
    {
        Id = department.Id,
        Name = department.Name
    };
}
```

Más simple porque Department tiene pocos campos.

---

## 🔄 Flujo de Uso en la API

### **Ejemplo 1: Endpoint público - Listar Departamentos**

```
Cliente hace GET /api/departments
    ↓
Controller obtiene List<Department> del Repository
    ↓
Usa mapper: departments.Select(d => d.ToDto())
    ↓
Devuelve List<DepartmentDto> como JSON
    ↓
Cliente recibe: [{"id": 1, "name": "Logística"}, ...]
```

**Código:**
```csharp
[HttpGet]
public async Task<ActionResult<IEnumerable<DepartmentDto>>> GetDepartments()
{
    var departments = await _departmentRepository.GetAllAsync();
    var dtos = departments.Select(d => d.ToDto()).ToList(); // ✅ Mapper
    return Ok(dtos);
}
```

---

### **Ejemplo 2: Endpoint protegido - Mi Información**

```
Cliente hace GET /api/employees/me con JWT
    ↓
Middleware valida JWT y extrae employeeId
    ↓
Controller obtiene Employee del Repository
    ↓
Usa mapper: employee.ToDto()
    ↓
Devuelve EmployeeDto como JSON
    ↓
Cliente recibe todos sus datos
```

**Código:**
```csharp
[HttpGet("me")]
[Authorize] // ✅ Requiere JWT
public async Task<ActionResult<EmployeeDto>> GetMyInfo()
{
    var employeeId = GetEmployeeIdFromToken();
    var employee = await _employeeRepository.GetByIdAsync(employeeId);
    var dto = employee.ToDto(); // ✅ Mapper
    return Ok(dto);
}
```

---

## 🆚 Diferencia: Extensions vs Mappings

| Aspecto | Extensions (Web) | Mappings (API) |
|---------|------------------|----------------|
| **Origen** | Entidades (Employee) | Entidades (Employee) |
| **Destino** | ViewModels | DTOs |
| **Propósito** | Vistas Razor HTML | Respuestas JSON |
| **Ubicación** | `Extensions/` | `Mappings/` |
| **Bidireccional** | Sí (ToViewModel/ToEntity) | No (solo ToDto) |
| **Ejemplos** | `employee.ToViewModel()` | `employee.ToDto()` |

---

## ✅ Ventajas de tener Mappings separados

### 1. **Seguridad**
```csharp
// ❌ Sin mapper (expone todo)
return Ok(employee); // Incluye campos privados, IDs internos, etc.

// ✅ Con mapper (control total)
return Ok(employee.ToDto()); // Solo campos públicos definidos en DTO
```

### 2. **Flexibilidad**
```csharp
// Puedes transformar datos antes de enviarlos
Status = GetStatusDisplay(employee.Status) // "Active" → "Activo"
EducationLevel = GetEducationLevelDisplay(employee.EducationLevel)
```

### 3. **Versionado de API**
```csharp
// Mappings/V1/EmployeeMapper.cs
public static EmployeeDtoV1 ToDto(...)

// Mappings/V2/EmployeeMapper.cs
public static EmployeeDtoV2 ToDto(...) // Estructura diferente
```

### 4. **Documentación clara**
```csharp
/// <summary>
/// Convierte Employee a EmployeeDto para respuestas JSON
/// </summary>
public static EmployeeDto ToDto(this Employee employee)
```

---

## 📊 Estructura Completa de la API

```
ApiTalento.Web/
├── Controllers/          # Endpoints de la API
│   ├── AuthController.cs       # Login, Register
│   ├── DepartmentsController.cs
│   └── EmployeesController.cs  # Me, Resume PDF
├── DTOs/                # Objetos para JSON
│   ├── DepartmentDto.cs
│   ├── EmployeeDto.cs
│   ├── LoginDto.cs
│   ├── LoginResponseDto.cs
│   └── EmployeeRegisterDto.cs
├── Mappings/            # ✅ Conversiones Entity → DTO
│   ├── DepartmentMapper.cs
│   └── EmployeeMapper.cs
├── Services/            # Lógica de negocio
│   ├── EmailService.cs       # SMTP real
│   ├── JwtService.cs         # Generación de tokens
│   └── PdfService.cs         # Generación de PDFs
├── Models/ViewModels/   # Para PDF (compartido)
│   └── EmployeeViewModel.cs
├── Data/                # Enlace simbólico al proyecto Web
├── Repositories/        # Enlace simbólico al proyecto Web
└── Program.cs           # Configuración JWT, Swagger, etc.
```

---

## 🎓 ¿Qué son los DTOs?

**DTO = Data Transfer Object** (Objeto de Transferencia de Datos)

### Características:
- ✅ **Solo datos**, sin lógica de negocio
- ✅ Diseñados para **serialización JSON**
- ✅ Controlan **qué se expone** al cliente
- ✅ Pueden ser diferentes de las entidades

### Ejemplo:

**Entidad (BD):**
```csharp
public class Employee
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public EmployeeStatus Status { get; set; } // Enum
    public decimal Salary { get; set; }
    public Department Department { get; set; } // Navegación
    // ... 15 propiedades más
}
```

**DTO (API):**
```csharp
public class EmployeeDto
{
    public int Id { get; set; }
    public string FullName { get; set; } // ✅ Calculado
    public string Status { get; set; } // ✅ String, no enum
    public decimal Salary { get; set; }
    public string DepartmentName { get; set; } // ✅ Solo nombre
    // Solo propiedades necesarias para el cliente
}
```

---

## 🔐 Seguridad con Mappings

### **Sin Mapper (Peligroso)** ❌
```csharp
[HttpGet("me")]
public async Task<ActionResult> GetMyInfo()
{
    var employee = await _repo.GetByIdAsync(id);
    return Ok(employee); // ❌ Expone TODO
}
```

**Respuesta JSON:**
```json
{
  "id": 5,
  "salary": 5000000, // ❌ Sensible
  "department": {    // ❌ Lazy loading
    "id": 1,
    "employees": [...] // ❌ Referencias circulares
  },
  // ... propiedades internas de EF Core
}
```

### **Con Mapper (Seguro)** ✅
```csharp
[HttpGet("me")]
public async Task<ActionResult> GetMyInfo()
{
    var employee = await _repo.GetByIdAsync(id);
    return Ok(employee.ToDto()); // ✅ Solo lo necesario
}
```

**Respuesta JSON:**
```json
{
  "id": 5,
  "fullName": "Juan López",
  "email": "juan@example.com",
  "position": "Desarrollador",
  "departmentName": "TI"
}
```

---

## 📝 Resumen

| Pregunta | Respuesta |
|----------|-----------|
| **¿Qué es Mappings?** | Carpeta con conversiones Entity → DTO |
| **¿Por qué existe?** | Para controlar qué datos se envían al cliente |
| **¿Es igual a Extensions?** | No, Extensions es para ViewModels (Web) |
| **¿Cuándo se usa?** | En todos los endpoints de la API |
| **¿Es obligatorio?** | No, pero es **muy recomendado** para seguridad |

---

## 🎯 Conclusión

**Mappings es la capa de "traducción" entre tu base de datos y el mundo exterior (clientes de la API).**

- ✅ Protege datos sensibles
- ✅ Transforma enums a strings
- ✅ Evita lazy loading accidental
- ✅ Controla la estructura JSON
- ✅ Facilita el versionado de la API

**¡Es como un filtro de seguridad para tus respuestas JSON!** 🛡️

