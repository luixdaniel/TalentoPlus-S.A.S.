using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TalentoPlus_S.A.S.ll.Web.Services;

namespace TalentoPlus_S.A.S.ll.Web.Areas.Admin.Pages.Employees
{
    [Authorize]
    public class ImportModel : PageModel
    {
        private readonly IExcelImportService _excelImportService;
        private readonly ILogger<ImportModel> _logger;

        public ImportModel(IExcelImportService excelImportService, ILogger<ImportModel> logger)
        {
            _excelImportService = excelImportService;
            _logger = logger;
        }

        public ImportResult? ImportResult { get; set; }

        public void OnGet()
        {
            // Just show the form
        }

        public async Task<IActionResult> OnPostAsync(IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length == 0)
            {
                ModelState.AddModelError("excelFile", "Por favor selecciona un archivo Excel");
                return Page();
            }

            // Validate file extension
            var extension = Path.GetExtension(excelFile.FileName).ToLower();
            if (extension != ".xlsx" && extension != ".xls")
            {
                ModelState.AddModelError("excelFile", "Solo se permiten archivos Excel (.xlsx, .xls)");
                return Page();
            }

            // Validate file size (max 10 MB)
            if (excelFile.Length > 10 * 1024 * 1024)
            {
                ModelState.AddModelError("excelFile", "El archivo no debe superar los 10 MB");
                return Page();
            }

            try
            {
                using var stream = new MemoryStream();
                await excelFile.CopyToAsync(stream);
                stream.Position = 0;

                _logger.LogInformation("Starting Excel import from file: {FileName}", excelFile.FileName);

                ImportResult = await _excelImportService.ImportEmployeesFromExcelAsync(stream);

                if (ImportResult.Success)
                {
                    _logger.LogInformation(
                        "Excel import completed. Successful: {Success}, Updated: {Updated}, Failed: {Failed}",
                        ImportResult.SuccessfulImports,
                        ImportResult.UpdatedRecords,
                        ImportResult.FailedImports);

                    if (ImportResult.FailedImports == 0)
                    {
                        TempData["Success"] = $"Importación completada: {ImportResult.SuccessfulImports} nuevos, {ImportResult.UpdatedRecords} actualizados";
                    }
                }
                else
                {
                    _logger.LogWarning("Excel import failed with {ErrorCount} errors", ImportResult.Errors.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during Excel import");
                ImportResult = new ImportResult
                {
                    Success = false,
                    Errors = new List<string> { $"Error inesperado: {ex.Message}" }
                };
            }

            return Page();
        }
    }
}

