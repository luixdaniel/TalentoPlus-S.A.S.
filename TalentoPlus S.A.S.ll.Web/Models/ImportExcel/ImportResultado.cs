namespace TalentoPlus_S.A.S.ll.Web.Areas.Admin.Models
{
    /// <summary>
    /// Resultado de la importación masiva de empleados desde Excel
    /// </summary>
    public class ImportResultado
    {
        public bool Exitoso { get; set; }
        public int TotalFilas { get; set; }
        public int Importados { get; set; }
        public int Actualizados { get; set; }
        public int Fallidos { get; set; }
        public List<string> Errores { get; set; } = new();
        public List<string> Advertencias { get; set; } = new();
        public List<EmpleadoImportado> EmpleadosImportados { get; set; } = new();
        public DateTime FechaImportacion { get; set; } = DateTime.Now;
        public string? NombreArchivo { get; set; }
    }

    /// <summary>
    /// Información resumida de un empleado importado
    /// </summary>
    public class EmpleadoImportado
    {
        public int? Id { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Cargo { get; set; } = string.Empty;
        public string Departamento { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public bool EsActualizacion { get; set; }
    }
}

