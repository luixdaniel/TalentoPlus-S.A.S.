namespace TalentoPlus_S.A.S.ll.Web.Areas.Admin.ViewModels
{
    /// <summary>
    /// ViewModel para el dashboard administrativo
    /// </summary>
    public class AdminDashboardViewModel
    {
        public int TotalEmpleados { get; set; }
        public int EmpleadosActivos { get; set; }
        public int EmpleadosInactivos { get; set; }
        public int EmpleadosVacaciones { get; set; }
        public int TotalDepartamentos { get; set; }
        
        public List<DepartamentoEstadistica> EstadisticasPorDepartamento { get; set; } = new();
        public List<EmpleadoReciente> EmpleadosRecientes { get; set; } = new();
        
        public decimal SalarioPromedio { get; set; }
        public decimal SalarioTotal { get; set; }
    }

    public class DepartamentoEstadistica
    {
        public string NombreDepartamento { get; set; } = string.Empty;
        public int CantidadEmpleados { get; set; }
        public decimal SalarioPromedio { get; set; }
    }

    public class EmpleadoReciente
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string Cargo { get; set; } = string.Empty;
        public string Departamento { get; set; } = string.Empty;
        public DateTime FechaIngreso { get; set; }
    }
}

