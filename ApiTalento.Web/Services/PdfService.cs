using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ApiTalento.Web.DTOs;

namespace ApiTalento.Web.Services
{
    public class PdfService : IPdfService
    {
        public PdfService()
        {
            // Configuración de licencia para uso comunitario (gratuita)
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public byte[] GenerateEmployeeResumePdf(EmployeeDto employee)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.Letter);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                    // Header
                    page.Header().Element(ComposeHeader);

                    // Content
                    page.Content().Element(c => ComposeContent(c, employee));

                    // Footer
                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("Generado el: ");
                        text.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm")).SemiBold();
                        text.Span(" | TalentoPlus S.A.S. - Sistema de Gestión de RRHH");
                    });
                });
            });

            return document.GeneratePdf();
        }

        private void ComposeHeader(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text("HOJA DE VIDA").FontSize(24).Bold().FontColor(Colors.Blue.Darken2);
                    column.Item().Text("TalentoPlus S.A.S.").FontSize(12).FontColor(Colors.Grey.Darken1);
                });

                row.ConstantItem(100).Height(50).Placeholder();
            });
        }

        private void ComposeContent(IContainer container, EmployeeDto employee)
        {
            container.Column(column =>
            {
                column.Spacing(10);

                // Datos Personales
                column.Item().Element(c => ComposeSectionTitle(c, "DATOS PERSONALES"));
                column.Item().Element(c => ComposePersonalData(c, employee));

                // Información Laboral
                column.Item().PaddingTop(15).Element(c => ComposeSectionTitle(c, "INFORMACIÓN LABORAL"));
                column.Item().Element(c => ComposeWorkData(c, employee));

                // Nivel Educativo
                column.Item().PaddingTop(15).Element(c => ComposeSectionTitle(c, "FORMACIÓN ACADÉMICA"));
                column.Item().Element(c => ComposeEducationData(c, employee));

                // Perfil Profesional
                if (!string.IsNullOrWhiteSpace(employee.ProfessionalProfile))
                {
                    column.Item().PaddingTop(15).Element(c => ComposeSectionTitle(c, "PERFIL PROFESIONAL"));
                    column.Item().Element(c => ComposeProfessionalProfile(c, employee));
                }

                // Datos de Contacto
                column.Item().PaddingTop(15).Element(c => ComposeSectionTitle(c, "DATOS DE CONTACTO"));
                column.Item().Element(c => ComposeContactData(c, employee));
            });
        }

        private void ComposeSectionTitle(IContainer container, string title)
        {
            container.Background(Colors.Blue.Lighten3)
                .Padding(8)
                .Text(title)
                .FontSize(14)
                .Bold()
                .FontColor(Colors.Blue.Darken3);
        }

        private void ComposePersonalData(IContainer container, EmployeeDto employee)
        {
            container.Column(column =>
            {
                column.Item().Row(row =>
                {
                    row.RelativeItem().Element(c => ComposeField(c, "Nombre Completo:", employee.FullName));
                    row.RelativeItem().Element(c => ComposeField(c, "Documento:", employee.DocumentNumber ?? "N/A"));
                });

                column.Item().PaddingTop(5).Row(row =>
                {
                    row.RelativeItem().Element(c => ComposeField(c, "Fecha de Nacimiento:", employee.BirthDate.ToString("dd/MM/yyyy")));
                    var edad = DateTime.Now.Year - employee.BirthDate.Year;
                    if (employee.BirthDate.Date > DateTime.Now.AddYears(-edad)) edad--;
                    row.RelativeItem().Element(c => ComposeField(c, "Edad:", $"{edad} años"));
                });
            });
        }

        private void ComposeWorkData(IContainer container, EmployeeDto employee)
        {
            container.Column(column =>
            {
                column.Item().Row(row =>
                {
                    row.RelativeItem().Element(c => ComposeField(c, "Cargo:", employee.Position));
                    row.RelativeItem().Element(c => ComposeField(c, "Departamento:", employee.DepartmentName ?? "N/A"));
                });

                column.Item().PaddingTop(5).Row(row =>
                {
                    row.RelativeItem().Element(c => ComposeField(c, "Fecha de Ingreso:", employee.HireDate.ToString("dd/MM/yyyy")));
                    row.RelativeItem().Element(c => ComposeField(c, "Estado:", employee.Status));
                });

                column.Item().PaddingTop(5).Row(row =>
                {
                    row.RelativeItem().Element(c => ComposeField(c, "Salario:", $"${employee.Salary:N2}"));
                    var antiguedad = DateTime.Now.Year - employee.HireDate.Year;
                    row.RelativeItem().Element(c => ComposeField(c, "Antigüedad:", $"{antiguedad} año(s)"));
                });
            });
        }

        private void ComposeEducationData(IContainer container, EmployeeDto employee)
        {
            container.Element(c => ComposeField(c, "Nivel Educativo:", employee.EducationLevel));
        }

        private void ComposeProfessionalProfile(IContainer container, EmployeeDto employee)
        {
            container.Padding(10)
                .Background(Colors.Grey.Lighten4)
                .Text(employee.ProfessionalProfile ?? "")
                .FontSize(10)
                .Justify();
        }

        private void ComposeContactData(IContainer container, EmployeeDto employee)
        {
            container.Column(column =>
            {
                column.Item().Element(c => ComposeField(c, "Dirección:", employee.Address));
                
                column.Item().PaddingTop(5).Row(row =>
                {
                    row.RelativeItem().Element(c => ComposeField(c, "Teléfono:", employee.Phone));
                    row.RelativeItem().Element(c => ComposeField(c, "Email:", employee.Email));
                });
            });
        }

        private void ComposeField(IContainer container, string label, string value)
        {
            container.Row(row =>
            {
                row.ConstantItem(120).Text(label).Bold().FontSize(10);
                row.RelativeItem().Text(value).FontSize(10);
            });
        }
    }
}

