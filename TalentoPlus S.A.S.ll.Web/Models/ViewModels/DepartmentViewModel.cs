using System.ComponentModel.DataAnnotations;

namespace TalentoPlus_S.A.S.ll.Web.Models.ViewModels
{
    public class DepartmentViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del departamento es obligatorio")]
        [Display(Name = "Departamento")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Total de Empleados")]
        public int EmployeeCount { get; set; }
    }
}

