namespace TalentoPlus_S.A.S.ll.Web.Models.ViewModels
{
    public class AiQueryResponse
    {
        public bool Success { get; set; }
        public string Answer { get; set; } = string.Empty;
        public string? Error { get; set; }
        public object? Data { get; set; }
    }
}
