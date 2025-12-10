namespace TalentoPlus_S.A.S.ll.Web.Areas.Admin.Models
{
    /// <summary>
    /// Configuración para importación masiva de empleados
    /// Permite personalizar el comportamiento de la importación
    /// </summary>
    public class ImportacionMasiva
    {
        /// <summary>
        /// Actualizar empleados existentes si se encuentran por email
        /// </summary>
        public bool ActualizarExistentes { get; set; } = true;

        /// <summary>
        /// Detener la importación al primer error
        /// </summary>
        public bool DetenerEnError { get; set; } = false;

        /// <summary>
        /// Validar emails duplicados antes de importar
        /// </summary>
        public bool ValidarDuplicados { get; set; } = true;

        /// <summary>
        /// Enviar notificación por email al completar
        /// </summary>
        public bool NotificarAlCompletar { get; set; } = false;

        /// <summary>
        /// Número máximo de filas a procesar (0 = sin límite)
        /// </summary>
        public int MaximoFilas { get; set; } = 0;

        /// <summary>
        /// Columna que identifica al empleado (para actualizaciones)
        /// </summary>
        public string ColumnaIdentificador { get; set; } = "Email";

        /// <summary>
        /// Mapeo personalizado de columnas (nombre en Excel -> nombre esperado)
        /// </summary>
        public Dictionary<string, string> MapeoColumnas { get; set; } = new();

        /// <summary>
        /// Fila donde comienzan los datos (por defecto fila 2, después de headers)
        /// </summary>
        public int FilaInicioDatos { get; set; } = 2;

        /// <summary>
        /// Nombre del archivo siendo importado
        /// </summary>
        public string? NombreArchivo { get; set; }

        /// <summary>
        /// Validaciones adicionales personalizadas
        /// </summary>
        public List<string> ValidacionesAdicionales { get; set; } = new();
    }
}

