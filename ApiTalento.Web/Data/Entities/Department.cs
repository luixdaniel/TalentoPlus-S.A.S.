using System.ComponentModel.DataAnnotations;
namespace ApiTalento.Web.Data.Entities
{
    public class Department
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El nombre del departamento es obligatorio")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
