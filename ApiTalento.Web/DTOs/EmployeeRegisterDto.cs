using System.ComponentModel.DataAnnotations;

namespace ApiTalento.Web.DTOs
{
    public class EmployeeRegisterDto
    {
        [Required(ErrorMessage = "El número de documento es requerido")]
        public string DocumentNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es requerido")]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de nacimiento es requerida")]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "La dirección es requerida")]
        [StringLength(200)]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es requerido")]
        [Phone]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "El cargo es requerido")]
        public string Position { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nivel educativo es requerido")]
        public int EducationLevel { get; set; }

        public string? ProfessionalProfile { get; set; }

        [Required(ErrorMessage = "El departamento es requerido")]
        public int DepartmentId { get; set; }
    }
}

