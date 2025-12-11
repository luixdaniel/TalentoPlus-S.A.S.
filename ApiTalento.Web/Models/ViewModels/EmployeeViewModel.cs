using System.ComponentModel.DataAnnotations;
using ApiTalento.Web.Data.Entities;

namespace ApiTalento.Web.Models.ViewModels
{
    public class EmployeeViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Número de Documento")]
        public string? DocumentNumber { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [Display(Name = "Nombres")]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los apellidos son obligatorios")]
        [Display(Name = "Apellidos")]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Display(Name = "Nombre Completo")]
        public string FullName => $"{FirstName} {LastName}";

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria")]
        [Display(Name = "Fecha de Nacimiento")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "La dirección es obligatoria")]
        [Display(Name = "Dirección")]
        [StringLength(200)]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [Display(Name = "Teléfono")]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        [Display(Name = "Email")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "El cargo es obligatorio")]
        [Display(Name = "Cargo")]
        [StringLength(100)]
        public string Position { get; set; } = string.Empty;

        [Required(ErrorMessage = "El salario es obligatorio")]
        [Display(Name = "Salario")]
        [DataType(DataType.Currency)]
        [Range(0, double.MaxValue, ErrorMessage = "El salario debe ser mayor a 0")]
        public decimal Salary { get; set; }

        [Required(ErrorMessage = "La fecha de ingreso es obligatoria")]
        [Display(Name = "Fecha de Ingreso")]
        [DataType(DataType.Date)]
        public DateTime HireDate { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio")]
        [Display(Name = "Estado")]
        public EmployeeStatus Status { get; set; }

        [Required(ErrorMessage = "El nivel educativo es obligatorio")]
        [Display(Name = "Nivel Educativo")]
        public EducationLevel EducationLevel { get; set; }

        [Display(Name = "Perfil Profesional")]
        [DataType(DataType.MultilineText)]
        public string? ProfessionalProfile { get; set; }

        [Required(ErrorMessage = "El departamento es obligatorio")]
        [Display(Name = "Departamento")]
        public int DepartmentId { get; set; }

        [Display(Name = "Departamento")]
        public string? DepartmentName { get; set; }

        // Helper para mostrar estado con formato
        public string StatusDisplay => Status switch
        {
            EmployeeStatus.Active => "Activo",
            EmployeeStatus.Inactive => "Inactivo",
            EmployeeStatus.Vacation => "Vacaciones",
            _ => "Desconocido"
        };

        // Helper para mostrar nivel educativo con formato
        public string EducationLevelDisplay => EducationLevel switch
        {
            EducationLevel.Professional => "Profesional",
            EducationLevel.Technical => "Técnico",
            EducationLevel.Technologist => "Tecnólogo",
            EducationLevel.Master => "Maestría",
            EducationLevel.Specialization => "Especialización",
            _ => "Desconocido"
        };
    }
}

