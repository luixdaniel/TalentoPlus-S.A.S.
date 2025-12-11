using ApiTalento.Web.DTOs;
using ApiTalento.Web.Data.Entities;

namespace ApiTalento.Web.Mappings
{
    public static class DepartmentMapper
    {
        public static DepartmentDto ToDto(this Department department)
        {
            if (department == null) return null!;

            return new DepartmentDto
            {
                Id = department.Id,
                Name = department.Name
            };
        }
    }
}

