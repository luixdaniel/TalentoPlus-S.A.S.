namespace TalentoPlus_S.A.S.ll.Web.Models.ImportExcel
{
    /// <summary>
    /// DTO para desnormalizar datos del Excel independientemente del orden de columnas
    /// Permite importar archivos con columnas en cualquier orden
    /// </summary>
    public class DatosDesnormalizados
    {
        // Información Personal
        public string Nombres { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        
        // Fechas
        public string FechaNacimiento { get; set; } = string.Empty;
        public string FechaIngreso { get; set; } = string.Empty;
        
        // Información Laboral
        public string Cargo { get; set; } = string.Empty;
        public string Salario { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string NivelEducativo { get; set; } = string.Empty;
        public string Departamento { get; set; } = string.Empty;
        
        // Opcional
        public string? PerfilProfesional { get; set; }
        
        // Metadata
        public int NumeroFila { get; set; }
        public bool TieneDatos => !string.IsNullOrWhiteSpace(Email) || !string.IsNullOrWhiteSpace(Nombres);
    }
}

