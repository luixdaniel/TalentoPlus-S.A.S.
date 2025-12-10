# 📁 Carpeta Extensions - Explicación Completa

## 🎯 ¿Qué es la carpeta Extensions?

La carpeta **Extensions** contiene **métodos de extensión** (Extension Methods) que actúan como **mappers** o **conversores** entre las **Entidades de la Base de Datos** y los **ViewModels**.

---

## 🤔 ¿Qué son los Extension Methods?

Los **Extension Methods** son métodos estáticos que permiten "agregar" funcionalidad a tipos existentes sin modificar su código original.

### Sintaxis:
```csharp
public static class MiClaseExtensions
{
    // El primer parámetro con 'this' indica el tipo que se extiende
    public static TipoRetorno MetodoExtension(this TipoAExtender objeto)
    {
        // Lógica del método
    }
}
```

### Uso:
```csharp
// En lugar de llamar: MiClaseExtensions.MetodoExtension(objeto)
// Puedes llamar:
objeto.MetodoExtension();
```

---

## 📂 Archivos en la Carpeta

### 1. **EmployeeExtensions.cs**
Contiene métodos de extensión para convertir entre `Employee` (entidad) y `EmployeeViewModel`.

### 2. **DepartmentExtensions.cs**
Contiene métodos de extensión para convertir entre `Department` (entidad) y `DepartmentViewModel`.

---

## 🔄 EmployeeExtensions.cs - Métodos Disponibles

### 1️⃣ **ToViewModel()** - Entidad → ViewModel
```csharp
public static EmployeeViewModel ToViewModel(this Employee employee)
```

**¿Qué hace?**
- Convierte una entidad `Employee` de la base de datos a un `EmployeeViewModel` para la vista.

**Ejemplo de uso:**
```csharp
// En el Controller
var employee = await _employeeService.GetEmployeeByIdAsync(5);
var viewModel = employee.ToViewModel(); // ✅ Usa el extension method
return View(viewModel);
```

**¿Por qué es importante?**
- ✅ Evita exponer la entidad de BD directamente en las vistas
- ✅ Solo envía los datos necesarios
- ✅ Incluye propiedades calculadas como `DepartmentName`

---

### 2️⃣ **ToEntity()** - ViewModel → Entidad
```csharp
public static Employee ToEntity(this EmployeeViewModel viewModel)
```

**¿Qué hace?**
- Convierte un `EmployeeViewModel` (desde la vista) a una entidad `Employee` para guardar en BD.

**Ejemplo de uso:**
```csharp
// En el Controller - Método Create
[HttpPost]
public async Task<IActionResult> Create(EmployeeViewModel viewModel)
{
    var employee = viewModel.ToEntity(); // ✅ Convierte ViewModel a Entity
    await _employeeService.CreateEmployeeAsync(employee);
    return RedirectToAction(nameof(Index));
}
```

**¿Por qué es importante?**
- ✅ Permite recibir ViewModels en el Controller
- ✅ Convierte a entidad para guardar en BD
- ✅ Mantiene la separación de capas

---

### 3️⃣ **UpdateEntity()** - Actualizar Entidad desde ViewModel
```csharp
public static void UpdateEntity(this Employee employee, EmployeeViewModel viewModel)
```

**¿Qué hace?**
- Actualiza los valores de una entidad `Employee` existente con los datos del `EmployeeViewModel`.

**Ejemplo de uso:**
```csharp
// En el Controller - Método Edit
[HttpPost]
public async Task<IActionResult> Edit(int id, EmployeeViewModel viewModel)
{
    var employee = await _employeeService.GetEmployeeByIdAsync(id);
    employee.UpdateEntity(viewModel); // ✅ Actualiza la entidad
    await _employeeService.UpdateEmployeeAsync(employee);
    return RedirectToAction(nameof(Index));
}
```

**¿Por qué es importante?**
- ✅ No crea una nueva entidad, actualiza la existente
- ✅ Mantiene el ID y otras propiedades de tracking de EF Core
- ✅ Evita problemas de concurrencia

---

### 4️⃣ **ToViewModelList()** - Lista de Entidades → Lista de ViewModels
```csharp
public static List<EmployeeViewModel> ToViewModelList(this IEnumerable<Employee> employees)
```

**¿Qué hace?**
- Convierte una lista/colección de `Employee` a una lista de `EmployeeViewModel`.

**Ejemplo de uso:**
```csharp
// En el Controller - Método Index
public async Task<IActionResult> Index()
{
    var employees = await _employeeService.GetAllEmployeesAsync();
    var viewModels = employees.ToViewModelList(); // ✅ Convierte lista completa
    return View(viewModels);
}
```

**¿Por qué es importante?**
- ✅ Convierte colecciones completas de una vez
- ✅ Código más limpio y legible
- ✅ Manejo de nulos automático

---

## 🔄 DepartmentExtensions.cs - Métodos Disponibles

### 1️⃣ **ToViewModel()** - Entidad → ViewModel
```csharp
public static DepartmentViewModel ToViewModel(this Department department, int employeeCount = 0)
```

**¿Qué hace?**
- Convierte una entidad `Department` a `DepartmentViewModel`.
- Acepta un parámetro opcional `employeeCount` para agregar el conteo de empleados.

**Ejemplo de uso:**
```csharp
var department = await _departmentService.GetDepartmentByIdAsync(1);
var viewModel = department.ToViewModel(employeeCount: 25);
```

---

### 2️⃣ **ToViewModelList()** - Lista de Departamentos
```csharp
public static List<DepartmentViewModel> ToViewModelList(this IEnumerable<Department> departments)
```

**¿Qué hace?**
- Convierte una lista de `Department` a `DepartmentViewModel`.

**Ejemplo de uso:**
```csharp
var departments = await _departmentService.GetAllDepartmentsAsync();
var viewModels = departments.ToViewModelList();
```

---

## 🎯 Flujo Completo de Uso

### **Escenario 1: Mostrar Lista de Empleados (GET)**

```
Base de Datos (Employee entities)
    ↓
Service devuelve List<Employee>
    ↓
Controller usa: employees.ToViewModelList()
    ↓
Vista recibe List<EmployeeViewModel>
    ↓
Usuario ve los datos (sin exponer BD)
```

**Código:**
```csharp
// Controller
public async Task<IActionResult> Index()
{
    var employees = await _employeeService.GetAllEmployeesAsync(); // Entidades
    var viewModels = employees.ToViewModelList(); // ✅ Conversión
    return View(viewModels); // ViewModels
}
```

---

### **Escenario 2: Crear Empleado (POST)**

```
Usuario llena formulario
    ↓
Vista envía EmployeeViewModel
    ↓
Controller recibe EmployeeViewModel
    ↓
Controller usa: viewModel.ToEntity()
    ↓
Service guarda Employee (entidad)
    ↓
Base de Datos
```

**Código:**
```csharp
// Controller
[HttpPost]
public async Task<IActionResult> Create(EmployeeViewModel viewModel)
{
    var employee = viewModel.ToEntity(); // ✅ ViewModel → Entidad
    await _employeeService.CreateEmployeeAsync(employee);
    return RedirectToAction(nameof(Index));
}
```

---

### **Escenario 3: Editar Empleado (POST)**

```
Usuario edita formulario
    ↓
Vista envía EmployeeViewModel
    ↓
Controller obtiene Employee existente de BD
    ↓
Controller usa: employee.UpdateEntity(viewModel)
    ↓
Service actualiza en BD
```

**Código:**
```csharp
// Controller
[HttpPost]
public async Task<IActionResult> Edit(int id, EmployeeViewModel viewModel)
{
    var employee = await _employeeService.GetEmployeeByIdAsync(id);
    employee.UpdateEntity(viewModel); // ✅ Actualiza entidad
    await _employeeService.UpdateEmployeeAsync(employee);
    return RedirectToAction(nameof(Index));
}
```

---

## ✅ Ventajas de Usar Extensions (Mappers)

### 1. **Código Limpio y Legible**
```csharp
// ❌ Sin extension methods (código repetitivo)
var viewModel = new EmployeeViewModel
{
    Id = employee.Id,
    FirstName = employee.FirstName,
    LastName = employee.LastName,
    // ... 15 propiedades más
};

// ✅ Con extension methods (limpio)
var viewModel = employee.ToViewModel();
```

### 2. **Reutilizable**
- Se usa en todos los controllers que necesiten conversiones
- Evita duplicar código de mapeo

### 3. **Mantenible**
- Si cambia la estructura, solo actualizas el extension method
- Todos los lugares que lo usan se actualizan automáticamente

### 4. **Testeable**
- Fácil de crear tests unitarios
- Métodos estáticos simples de probar

### 5. **Sintaxis Fluida**
```csharp
// Sintaxis natural y fácil de leer
var viewModels = employees
    .Where(e => e.Status == EmployeeStatus.Active)
    .ToViewModelList(); // ✅ Se lee como inglés
```

---

## 🆚 Comparación: Con vs Sin Extensions

### **Sin Extension Methods** ❌
```csharp
// Controller
public async Task<IActionResult> Index()
{
    var employees = await _employeeService.GetAllEmployeesAsync();
    
    // Código repetitivo en cada controller
    var viewModels = employees.Select(e => new EmployeeViewModel
    {
        Id = e.Id,
        FirstName = e.FirstName,
        LastName = e.LastName,
        BirthDate = e.BirthDate,
        Address = e.Address,
        Phone = e.Phone,
        Email = e.Email,
        Position = e.Position,
        Salary = e.Salary,
        HireDate = e.HireDate,
        Status = e.Status,
        EducationLevel = e.EducationLevel,
        ProfessionalProfile = e.ProfessionalProfile,
        DepartmentId = e.DepartmentId,
        DepartmentName = e.Department?.Name
    }).ToList();
    
    return View(viewModels);
}
```

### **Con Extension Methods** ✅
```csharp
// Controller
public async Task<IActionResult> Index()
{
    var employees = await _employeeService.GetAllEmployeesAsync();
    var viewModels = employees.ToViewModelList(); // ✅ 1 línea
    return View(viewModels);
}
```

**Resultado:** Código 90% más corto y 100% más legible.

---

## 🧩 Relación con Otras Capas

```
┌─────────────────────────────────────────────┐
│          VISTA (View)                       │
│  Recibe/Envía: EmployeeViewModel           │
└──────────────┬──────────────────────────────┘
               │
               │ Extension Methods (Mappers)
               │ ↕️ ToViewModel() / ToEntity()
               │
┌──────────────▼──────────────────────────────┐
│       CONTROLLER                            │
│  Usa: Extensions para convertir            │
└──────────────┬──────────────────────────────┘
               │
               │
┌──────────────▼──────────────────────────────┐
│       SERVICE (Business Logic)              │
│  Trabaja con: Employee (Entidades)         │
└──────────────┬──────────────────────────────┘
               │
               │
┌──────────────▼──────────────────────────────┐
│       REPOSITORY (Data Access)              │
│  Accede a: Base de Datos (Employee)        │
└─────────────────────────────────────────────┘
```

---

## 🎓 Conceptos Importantes

### **Entidad (Entity)**
- Clase que representa una tabla en la base de datos
- Ejemplo: `Employee`, `Department`
- Ubicación: `Data/Entities/`
- Uso: Acceso a datos, EF Core

### **ViewModel**
- Clase que representa datos para la vista
- Ejemplo: `EmployeeViewModel`, `DepartmentViewModel`
- Ubicación: `Models/ViewModels/`
- Uso: Presentación, formularios

### **Mapper/Extension**
- Convierte entre Entidad ↔️ ViewModel
- Ubicación: `Extensions/`
- Uso: Controllers

---

## 📊 Ejemplo Real Completo

### **Archivo: EmployeesController.cs**

```csharp
using TalentoPlus_S.A.S.ll.Web.Extensions; // ✅ Importa extensions

public class EmployeesController : Controller
{
    private readonly IEmployeeService _employeeService;
    
    // GET: Listar empleados
    public async Task<IActionResult> Index()
    {
        var employees = await _employeeService.GetAllEmployeesAsync();
        return View(employees.ToViewModelList()); // ✅ Extension
    }
    
    // GET: Ver detalles
    public async Task<IActionResult> Details(int id)
    {
        var employee = await _employeeService.GetEmployeeByIdAsync(id);
        return View(employee.ToViewModel()); // ✅ Extension
    }
    
    // POST: Crear empleado
    [HttpPost]
    public async Task<IActionResult> Create(EmployeeViewModel viewModel)
    {
        var employee = viewModel.ToEntity(); // ✅ Extension
        await _employeeService.CreateEmployeeAsync(employee);
        return RedirectToAction(nameof(Index));
    }
    
    // POST: Editar empleado
    [HttpPost]
    public async Task<IActionResult> Edit(int id, EmployeeViewModel viewModel)
    {
        var employee = await _employeeService.GetEmployeeByIdAsync(id);
        employee.UpdateEntity(viewModel); // ✅ Extension
        await _employeeService.UpdateEmployeeAsync(employee);
        return RedirectToAction(nameof(Index));
    }
}
```

---

## 🚀 Alternativas

### **AutoMapper** (Librería Externa)
```csharp
// Con AutoMapper
var viewModel = _mapper.Map<EmployeeViewModel>(employee);
```

**Ventajas:**
- Configuración centralizada
- Mapeos complejos automáticos
- Popular en la industria

**Desventajas:**
- Dependencia externa
- Curva de aprendizaje
- Overhead de configuración

### **Extension Methods** (Tu Implementación)
```csharp
// Con extension methods
var viewModel = employee.ToViewModel();
```

**Ventajas:**
- ✅ Sin dependencias externas
- ✅ Control total del mapeo
- ✅ Fácil de entender y mantener
- ✅ Ideal para proyectos pequeños/medianos

---

## 📝 Resumen

| Aspecto | Descripción |
|---------|-------------|
| **Propósito** | Convertir entre Entidades y ViewModels |
| **Ubicación** | `Extensions/` |
| **Tipo** | Extension Methods (métodos estáticos) |
| **Uso** | `entity.ToViewModel()`, `viewModel.ToEntity()` |
| **Beneficio** | Código limpio, reutilizable y mantenible |
| **Patrón** | Mapper Pattern |

---

## 🎯 Conclusión

La carpeta **Extensions** es fundamental en tu arquitectura porque:

1. ✅ **Protege** la base de datos de ser expuesta directamente
2. ✅ **Simplifica** el código en los controllers
3. ✅ **Centraliza** la lógica de conversión
4. ✅ **Mejora** la mantenibilidad del proyecto
5. ✅ **Facilita** el testing

**Es la "traducción" entre lo que ve el usuario (ViewModels) y lo que está en la base de datos (Entities).**

---

**¡Los Extension Methods son el puente entre tu capa de presentación y tu capa de datos!** 🌉

