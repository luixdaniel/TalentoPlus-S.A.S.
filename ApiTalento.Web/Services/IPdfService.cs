using ApiTalento.Web.DTOs;

namespace ApiTalento.Web.Services
{
    public interface IPdfService
    {
        byte[] GenerateEmployeeResumePdf(EmployeeDto employee);
    }
}
