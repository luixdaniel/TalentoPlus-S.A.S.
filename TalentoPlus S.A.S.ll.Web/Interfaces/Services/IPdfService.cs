using TalentoPlus_S.A.S.ll.Web.Models.ViewModels;

namespace TalentoPlus_S.A.S.ll.Web.Services
{
    public interface IPdfService
    {
        byte[] GenerateEmployeeResumePdf(EmployeeViewModel employee);
    }
}

