using OfficeOpenXml;

namespace TalentoPlus_S.A.S.ll.Web.Models.ImportExcel
{
    /// <summary>
    /// Mapeo dinámico de columnas del Excel
    /// Detecta automáticamente el orden de las columnas basándose en los headers
    /// </summary>
    public class MapeoColumnasExcel
    {
        // Índices de columnas (base 1, como en Excel)
        public int? ColumnaDocumento { get; set; }
        public int? ColumnaNombres { get; set; }
        public int? ColumnaApellidos { get; set; }
        public int? ColumnaEmail { get; set; }
        public int? ColumnaTelefono { get; set; }
        public int? ColumnaDireccion { get; set; }
        public int? ColumnaFechaNacimiento { get; set; }
        public int? ColumnaFechaIngreso { get; set; }
        public int? ColumnaCargo { get; set; }
        public int? ColumnaSalario { get; set; }
        public int? ColumnaEstado { get; set; }
        public int? ColumnaNivelEducativo { get; set; }
        public int? ColumnaDepartamento { get; set; }
        public int? ColumnaPerfilProfesional { get; set; }

        /// <summary>
        /// Valida que todas las columnas requeridas estén mapeadas
        /// </summary>
        public bool EsValido()
        {
            return ColumnaNombres.HasValue
                && ColumnaApellidos.HasValue
                && ColumnaEmail.HasValue
                && ColumnaTelefono.HasValue
                && ColumnaDireccion.HasValue
                && ColumnaFechaNacimiento.HasValue
                && ColumnaFechaIngreso.HasValue
                && ColumnaCargo.HasValue
                && ColumnaSalario.HasValue
                && ColumnaEstado.HasValue
                && ColumnaNivelEducativo.HasValue
                && ColumnaDepartamento.HasValue;
            // ColumnaPerfilProfesional es opcional
        }

        /// <summary>
        /// Obtiene lista de columnas faltantes
        /// </summary>
        public List<string> ObtenerColumnasFaltantes()
        {
            var faltantes = new List<string>();

            if (!ColumnaNombres.HasValue) faltantes.Add("Nombres");
            if (!ColumnaApellidos.HasValue) faltantes.Add("Apellidos");
            if (!ColumnaEmail.HasValue) faltantes.Add("Email");
            if (!ColumnaTelefono.HasValue) faltantes.Add("Teléfono");
            if (!ColumnaDireccion.HasValue) faltantes.Add("Dirección");
            if (!ColumnaFechaNacimiento.HasValue) faltantes.Add("Fecha de Nacimiento");
            if (!ColumnaFechaIngreso.HasValue) faltantes.Add("Fecha de Ingreso");
            if (!ColumnaCargo.HasValue) faltantes.Add("Cargo");
            if (!ColumnaSalario.HasValue) faltantes.Add("Salario");
            if (!ColumnaEstado.HasValue) faltantes.Add("Estado");
            if (!ColumnaNivelEducativo.HasValue) faltantes.Add("Nivel Educativo");
            if (!ColumnaDepartamento.HasValue) faltantes.Add("Departamento");

            return faltantes;
        }

        /// <summary>
        /// Crea el mapeo directamente desde el worksheet de Excel
        /// Detecta automáticamente el orden de las columnas sin crear lista intermedia
        /// </summary>
        public static MapeoColumnasExcel CrearDesdeWorksheet(OfficeOpenXml.ExcelWorksheet worksheet, int maxCol)
        {
            var mapeo = new MapeoColumnasExcel();

            for (int col = 1; col <= maxCol; col++)
            {
                var headerValue = worksheet.Cells[1, col].Value?.ToString()?.Trim();
                if (string.IsNullOrWhiteSpace(headerValue))
                    continue;

                var header = NormalizarHeader(headerValue);

                if (CoincideHeader(header, "documento", "document", "identificacion", "cedula", "dni") && !header.Contains("apellido"))
                    mapeo.ColumnaDocumento = col;
                else if (CoincideHeader(header, "nombres", "nombre", "first name", "firstname"))
                    mapeo.ColumnaNombres = col;
                else if (CoincideHeader(header, "apellidos", "apellido", "last name", "lastname"))
                    mapeo.ColumnaApellidos = col;
                else if (CoincideHeader(header, "email", "correo", "correo electronico", "e-mail", "mail"))
                    mapeo.ColumnaEmail = col;
                else if (CoincideHeader(header, "telefono", "teléfono", "phone", "celular", "movil"))
                    mapeo.ColumnaTelefono = col;
                else if (CoincideHeader(header, "direccion", "dirección", "address", "domicilio"))
                    mapeo.ColumnaDireccion = col;
                else if (CoincideHeader(header, "fechanacimiento", "fechadenacimiento", "fechanac", "nacimiento", "birthdate"))
                    mapeo.ColumnaFechaNacimiento = col;
                else if (CoincideHeader(header, "fechaingreso", "fechadeingreso", "ingreso", "hiredate"))
                    mapeo.ColumnaFechaIngreso = col;
                else if (CoincideHeader(header, "cargo", "position", "puesto", "rol", "posicion"))
                    mapeo.ColumnaCargo = col;
                else if (CoincideHeader(header, "salario", "salary", "sueldo", "remuneracion", "pago"))
                    mapeo.ColumnaSalario = col;
                else if (CoincideHeader(header, "estado", "status", "state", "estatus"))
                    mapeo.ColumnaEstado = col;
                else if (CoincideHeader(header, "niveleducativo", "educacion", "education", "nivel"))
                    mapeo.ColumnaNivelEducativo = col;
                else if (CoincideHeader(header, "departamento", "department", "area", "área", "seccion"))
                    mapeo.ColumnaDepartamento = col;
                else if (CoincideHeader(header, "perfilprofesional", "perfil", "profile", "descripcion"))
                    mapeo.ColumnaPerfilProfesional = col;
            }

            return mapeo;
        }

        /// <summary>
        /// Crea el mapeo desde los headers de Excel
        /// Detecta automáticamente el orden de las columnas
        /// </summary>
        public static MapeoColumnasExcel CrearDesdeHeaders(List<string> headers)
        {
            var mapeo = new MapeoColumnasExcel();

            for (int i = 0; i < headers.Count; i++)
            {
                var header = NormalizarHeader(headers[i]);
                var columna = i + 1; // Excel usa base 1

                if (CoincideHeader(header, "documento", "document", "identificacion", "cedula", "dni") && !header.Contains("apellido"))
                    mapeo.ColumnaDocumento = columna;
                else if (CoincideHeader(header, "nombres", "nombre", "first name", "firstname"))
                    mapeo.ColumnaNombres = columna;
                else if (CoincideHeader(header, "apellidos", "apellido", "last name", "lastname"))
                    mapeo.ColumnaApellidos = columna;
                else if (CoincideHeader(header, "email", "correo", "correo electronico", "e-mail", "mail"))
                    mapeo.ColumnaEmail = columna;
                else if (CoincideHeader(header, "telefono", "teléfono", "phone", "celular", "movil"))
                    mapeo.ColumnaTelefono = columna;
                else if (CoincideHeader(header, "direccion", "dirección", "address", "domicilio"))
                    mapeo.ColumnaDireccion = columna;
                else if (CoincideHeader(header, "fechanacimiento", "fechadenacimiento", "nacimiento", "birthdate"))
                    mapeo.ColumnaFechaNacimiento = columna;
                else if (CoincideHeader(header, "fechaingreso", "fechadeingreso", "ingreso", "hiredate"))
                    mapeo.ColumnaFechaIngreso = columna;
                else if (CoincideHeader(header, "cargo", "position", "puesto", "rol", "posicion"))
                    mapeo.ColumnaCargo = columna;
                else if (CoincideHeader(header, "salario", "salary", "sueldo", "remuneracion", "pago"))
                    mapeo.ColumnaSalario = columna;
                else if (CoincideHeader(header, "estado", "status", "state", "estatus"))
                    mapeo.ColumnaEstado = columna;
                else if (CoincideHeader(header, "niveleducativo", "educacion", "education", "nivel"))
                    mapeo.ColumnaNivelEducativo = columna;
                else if (CoincideHeader(header, "departamento", "department", "area", "seccion"))
                    mapeo.ColumnaDepartamento = columna;
                else if (CoincideHeader(header, "perfilprofesional", "perfil", "profile", "descripcion"))
                    mapeo.ColumnaPerfilProfesional = columna;
            }

            return mapeo;
        }

        private static string NormalizarHeader(string header)
        {
            return header.ToLower().Trim()
                .Replace(" ", "")  // Quitar espacios
                .Replace("á", "a").Replace("é", "e").Replace("í", "i")
                .Replace("ó", "o").Replace("ú", "u").Replace("ñ", "n");
        }

        private static bool CoincideHeader(string headerNormalizado, params string[] patrones)
        {
            // El header ya viene normalizado, solo normalizar los patrones
            return patrones.Any(p => headerNormalizado.Contains(p.ToLower().Trim()
                .Replace("á", "a").Replace("é", "e").Replace("í", "i")
                .Replace("ó", "o").Replace("ú", "u").Replace("ñ", "n")));
        }
    }
}

