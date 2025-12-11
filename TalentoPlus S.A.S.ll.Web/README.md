# TalentoPlus S.A.S.ll.Web - Admin Panel

## Overview

Web-based administration panel for the TalentoPlus Human Resources Management System. Provides a complete interface for managing employees, departments, and viewing analytics with AI-powered insights.

## Features

- **Employee Management** - Full CRUD operations for employees
- **Department Management** - Manage organizational departments
- **AI-Powered Dashboard** - Natural language queries about employee data
- **Excel Import** - Bulk employee import from Excel files
- **PDF Export** - Generate employee resumes
- **Authentication** - Secure admin login with ASP.NET Identity
- **Responsive Design** - Modern, mobile-friendly interface

## Technology Stack

- ASP.NET Core 8.0 MVC
- ASP.NET Identity
- Entity Framework Core
- PostgreSQL (Supabase)
- Bootstrap 5
- jQuery
- QuestPDF (PDF generation)
- EPPlus (Excel import)

## Main Modules

### 1. Employee Management

- Create, read, update, delete employees
- Advanced search and filtering
- Bulk import from Excel
- Individual PDF resume generation
- Employee status management (Active, Inactive, Vacation)

### 2. Department Management

- CRUD operations for departments
- Employee assignment
- Department statistics

### 3. AI Dashboard

**Statistics Cards:**
- Total employees
- Employees on vacation
- Active employees
- Inactive employees

**Department Distribution:**
- Visual breakdown by department
- Percentage calculations
- Progress bars

**AI Assistant:**
- Natural language queries
- Real-time data analysis
- Example queries:
  - "How many employees are in Technology?"
  - "How many employees are on vacation?"
  - "What's the total number of employees?"

## Configuration

### Required Environment Variables

```bash
ConnectionStrings__DefaultConnection=<PostgreSQL connection string>
Gemini__ApiKey=<Google Gemini API key> (optional)
```

### appsettings.json Example

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "User Id=postgres;Password=***;Server=***;Port=5432;Database=postgres"
  },
  "Gemini": {
    "ApiKey": "YOUR_API_KEY_HERE"
  }
}
```

**Note:** The AI assistant works with local processing if no Gemini API key is provided.

## Running Locally

### Prerequisites

- .NET 8.0 SDK
- PostgreSQL database

### Steps

```bash
cd "TalentoPlus S.A.S.ll.Web"
dotnet restore
dotnet run
```

The application will be available at `http://localhost:5040`

## Running with Docker

```bash
# From project root
docker-compose -f docker-compose.simple.yml up -d web
```

The application will be available at `http://localhost:5041`

## Default Credentials

**Admin Account:**
- Email: `admin@talento.com`
- Password: `Admin123!`

## Project Structure

```
TalentoPlus S.A.S.ll.Web/
├── Areas/
│   ├── Admin/
│   │   ├── Controllers/      # MVC controllers
│   │   │   ├── DashboardController.cs
│   │   │   ├── EmployeesController.cs
│   │   │   └── DepartmentsController.cs
│   │   └── Views/           # Razor views
│   │       ├── Dashboard/
│   │       ├── Employees/
│   │       └── Departments/
│   └── Identity/
│       └── Pages/           # Identity pages
│           └── Account/
├── Services/               # Business logic
│   ├── EmployeeService.cs
│   ├── DepartmentService.cs
│   ├── GeminiAiService.cs
│   ├── ExcelImportService.cs
│   └── PdfService.cs
├── Data/                  # Database context
│   ├── ApplicationDbContext.cs
│   └── Entities/
├── Models/               # View models
│   └── ViewModels/
├── Views/               # Shared views
│   └── Shared/
└── Program.cs          # Application entry point
```

## Key Features

### Excel Import

Supports bulk employee import with the following columns:
- Document Number
- First Name
- Last Name
- Email
- Birth Date
- Address
- Phone
- Position
- Salary
- Hire Date
- Department
- Education Level
- Professional Profile

### PDF Generation

Automatically generates professional resumes including:
- Personal information
- Professional profile
- Work experience
- Education
- Contact details

### AI Dashboard

The AI assistant can answer questions like:
- "How many employees are in [Department]?"
- "How many employees are active/inactive?"
- "What's the total number of employees?"
- "How many [Position] do we have?"

## User Interface

- **Modern Design** - Clean, professional interface
- **Responsive Layout** - Works on desktop, tablet, and mobile
- **Bootstrap 5** - Modern UI components
- **Font Awesome Icons** - Professional iconography
- **Interactive Charts** - Visual data representation

## Security

- ASP.NET Identity authentication
- Role-based authorization
- Secure password hashing
- CSRF protection
- Session management

## Database Migrations

```bash
# Create migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update
```

## Troubleshooting

### Cannot login

- Verify database connection
- Check if admin user exists
- Reset password if needed

### Excel import fails

- Verify Excel file format
- Check column names match expected format
- Ensure data types are correct

### AI not responding

- Check Gemini API key configuration
- Verify internet connection
- Local processing will work without API key

## License

Private and confidential.
