# ApiTalento.Web - REST API

## Overview

REST API for the TalentoPlus Human Resources Management System. Provides public and protected endpoints for employee management, authentication, and document generation.

## Features

- **JWT Authentication** - Secure token-based authentication
- **Employee Self-Registration** - Public endpoint for employee registration
- **Employee Information** - Protected endpoints for authenticated employees
- **PDF Generation** - Automatic resume/CV generation
- **Email Notifications** - Welcome emails on registration
- **Department Management** - Public department listing

## Technology Stack

- ASP.NET Core 8.0
- Entity Framework Core
- PostgreSQL (Supabase)
- JWT Bearer Authentication
- QuestPDF (PDF generation)
- Swagger/OpenAPI

## API Endpoints

### Public Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/Departments` | List all departments |
| POST | `/api/Auth/register` | Employee self-registration |
| POST | `/api/Auth/login` | Login and get JWT token |

### Protected Endpoints (Require JWT)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/Employees/me` | Get authenticated employee info |
| GET | `/api/Employees/me/resume` | Download employee resume (PDF) |

## Configuration

### Required Environment Variables

```bash
ConnectionStrings__DefaultConnection=<PostgreSQL connection string>
Jwt__Key=<JWT secret key (min 32 chars)>
Jwt__Issuer=TalentoPlus.API
Jwt__Audience=TalentoPlus.Employees
Jwt__ExpirationHours=24
Email__SmtpServer=smtp.gmail.com
Email__SmtpPort=587
Email__Username=<email>
Email__Password=<app password>
```

### appsettings.json Example

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "User Id=postgres;Password=***;Server=***;Port=5432;Database=postgres"
  },
  "Jwt": {
    "Key": "Your_Secret_Key_Here_Min_32_Characters",
    "Issuer": "TalentoPlus.API",
    "Audience": "TalentoPlus.Employees",
    "ExpirationHours": 24
  },
  "Email": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "FromName": "TalentoPlus S.A.S.",
    "FromAddress": "noreply@talentoplussas.com",
    "Username": "your-email@gmail.com",
    "Password": "your-app-password"
  }
}
```

## Running Locally

### Prerequisites

- .NET 8.0 SDK
- PostgreSQL database

### Steps

```bash
cd ApiTalento.Web
dotnet restore
dotnet run
```

The API will be available at `http://localhost:5108`

## Running with Docker

```bash
# From project root
docker-compose -f docker-compose.simple.yml up -d api
```

The API will be available at `http://localhost:5109`

## Testing the API

### Using Swagger

Navigate to `http://localhost:5108/swagger` (or `5109` for Docker)

### Using curl

```bash
# Get departments (public)
curl http://localhost:5108/api/Departments

# Login
curl -X POST http://localhost:5108/api/Auth/login \
  -H "Content-Type: application/json" \
  -d '{"documentNumber":"1045228184","email":"ceraluis4@gmail.com"}'

# Get employee info (requires token)
curl http://localhost:5108/api/Employees/me \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"

# Download resume (requires token)
curl http://localhost:5108/api/Employees/me/resume \
  -H "Authorization: Bearer YOUR_TOKEN_HERE" \
  -o resume.pdf
```

## Project Structure

```
ApiTalento.Web/
├── Controllers/          # API controllers
│   ├── AuthController.cs
│   ├── DepartmentsController.cs
│   └── EmployeesController.cs
├── Services/            # Business logic
│   ├── JwtService.cs
│   ├── EmailService.cs
│   └── PdfService.cs
├── Repositories/        # Data access
│   ├── GenericRepository.cs
│   ├── EmployeeRepository.cs
│   └── DepartmentRepository.cs
├── DTOs/               # Data transfer objects
├── Data/               # Database context and entities
└── Program.cs          # Application entry point
```

## Authentication Flow

1. Employee registers via `/api/Auth/register`
2. System creates employee with "Inactive" status
3. System sends welcome email
4. Admin approves employee (changes status to "Active")
5. Employee logs in via `/api/Auth/login`
6. System returns JWT token
7. Employee uses token to access protected endpoints

## Security

- Passwords are not stored (document number + email used for authentication)
- JWT tokens expire after 24 hours
- Protected endpoints require valid JWT token
- CORS configured for cross-origin requests
- HTTPS recommended for production

## Error Handling

All endpoints return standard HTTP status codes:

- `200 OK` - Success
- `400 Bad Request` - Invalid input
- `401 Unauthorized` - Missing or invalid token
- `404 Not Found` - Resource not found
- `500 Internal Server Error` - Server error

## License

Private and confidential.
