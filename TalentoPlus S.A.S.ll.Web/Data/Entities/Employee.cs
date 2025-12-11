using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace TalentoPlus_S.A.S.ll.Web.Data.Entities
{
    public class Employee
    {
        public int Id { get; set; }
        
        [StringLength(50)]
        public string? DocumentNumber { get; set; }
        
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Los apellidos son obligatorios")]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;
        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
        [Required(ErrorMessage = "La dirección es obligatoria")]
        [StringLength(200)]
        public string Address { get; set; } = string.Empty;
        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "El cargo es obligatorio")]
        [StringLength(100)]
        public string Position { get; set; } = string.Empty;
        [Required(ErrorMessage = "El salario es obligatorio")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "El salario debe ser mayor a 0")]
        public decimal Salary { get; set; }
        [Required(ErrorMessage = "La fecha de ingreso es obligatoria")]
        [DataType(DataType.Date)]
        public DateTime HireDate { get; set; }
        [Required(ErrorMessage = "El estado es obligatorio")]
        public EmployeeStatus Status { get; set; }
        [Required(ErrorMessage = "El nivel educativo es obligatorio")]
        public EducationLevel EducationLevel { get; set; }
        [StringLength(500)]
        public string? ProfessionalProfile { get; set; }
        [Required(ErrorMessage = "El departamento es obligatorio")]
        public int DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public Department? Department { get; set; }
        public string FullName => $"{FirstName} {LastName}";
    }
}
