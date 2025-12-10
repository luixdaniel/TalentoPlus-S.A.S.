using System.ComponentModel.DataAnnotations;

namespace ApiTalento.Web.DTOs
{
    public class LoginDto
    {
        [Required(ErrorMessage = "El documento es requerido")]
        public string DocumentNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}

