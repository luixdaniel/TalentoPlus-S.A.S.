namespace TalentoPlus_S.A.S.ll.Web.Areas.Admin.ViewModels
{
    /// <summary>
    /// ViewModel para mostrar el resultado de la importación de Excel
    /// </summary>
    public class ImportResultViewModel
    {
        public bool Exitoso { get; set; }
        public int TotalFilas { get; set; }
        public int Importados { get; set; }
        public int Actualizados { get; set; }
        public int Fallidos { get; set; }
        public List<string> Errores { get; set; } = new();
        public List<string> Advertencias { get; set; } = new();
        public List<EmpleadoImportadoViewModel> EmpleadosImportados { get; set; } = new();
        public DateTime FechaImportacion { get; set; } = DateTime.Now;
        public string? NombreArchivo { get; set; }

        public string MensajeResumen => Exitoso
            ? $"Se importaron {Importados} empleados nuevos y se actualizaron {Actualizados} existentes"
            : $"La importación falló con {Errores.Count} errores";
    }

    /// <summary>
    /// ViewModel para mostrar información resumida de un empleado importado
    /// </summary>
    public class EmpleadoImportadoViewModel
    {
        public int? Id { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Cargo { get; set; } = string.Empty;
        public string Departamento { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public bool EsActualizacion { get; set; }
        public string TipoOperacion => EsActualizacion ? "Actualizado" : "Nuevo";
    }
}

