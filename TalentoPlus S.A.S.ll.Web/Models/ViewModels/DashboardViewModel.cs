namespace TalentoPlus_S.A.S.ll.Web.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalEmployees { get; set; }
        public int EmployeesOnVacation { get; set; }
        public int ActiveEmployees { get; set; }
        public int InactiveEmployees { get; set; }
        public Dictionary<string, int> EmployeesByDepartment { get; set; } = new();
        public List<DepartmentStatistic> DepartmentStatistics { get; set; } = new();
    }

    public class DepartmentStatistic
    {
        public string DepartmentName { get; set; } = string.Empty;
        public int EmployeeCount { get; set; }
        public decimal Percentage { get; set; }
    }
}
