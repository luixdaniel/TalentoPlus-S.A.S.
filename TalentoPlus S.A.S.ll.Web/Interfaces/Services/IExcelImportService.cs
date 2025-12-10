using TalentoPlus_S.A.S.ll.Web.Data.Entities;

namespace TalentoPlus_S.A.S.ll.Web.Services
{
    public interface IExcelImportService
    {
        Task<ImportResult> ImportEmployeesFromExcelAsync(Stream fileStream);
        Task<ImportResult> ValidateExcelStructureAsync(Stream fileStream);
    }

    public class ImportResult
    {
        public bool Success { get; set; }
        public int TotalRows { get; set; }
        public int SuccessfulImports { get; set; }
        public int UpdatedRecords { get; set; }
        public int FailedImports { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public List<string> Warnings { get; set; } = new List<string>();
        public List<Employee> ImportedEmployees { get; set; } = new List<Employee>();
    }
}

