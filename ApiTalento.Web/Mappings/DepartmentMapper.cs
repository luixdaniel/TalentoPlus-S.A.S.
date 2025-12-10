using ApiTalento.Web.DTOs;
using TalentoPlus_S.A.S.ll.Web.Data.Entities;

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

