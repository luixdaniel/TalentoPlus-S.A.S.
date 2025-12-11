# 🏢 TalentoPlus S.A.S. - Human Resources Management System

Complete HR management system with web admin panel, REST API, and AI-powered analytics dashboard.

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)](https://dotnet.microsoft.com/)
[![Docker](https://img.shields.io/badge/Docker-Ready-2496ED)](https://www.docker.com/)
[![Tests](https://img.shields.io/badge/Tests-9%2F9%20Passing-success)](./ApiTalento.Tests)
[![License](https://img.shields.io/badge/License-Private-red)](LICENSE)

---

## 📋 Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Architecture](#architecture)
- [Quick Start](#quick-start)
- [Projects](#projects)
- [Documentation](#documentation)
- [Technology Stack](#technology-stack)
- [Deployment](#deployment)
- [Testing](#testing)
- [License](#license)

---

## 🎯 Overview

TalentoPlus is a comprehensive Human Resources Management System designed to streamline employee management, provide self-service capabilities, and deliver AI-powered insights.

**Key Components:**
- **Admin Web Panel** - Full-featured management interface
- **REST API** - Public and protected endpoints for employee services
- **AI Dashboard** - Natural language queries and analytics
- **Integration Tests** - Comprehensive test coverage

---

## ✨ Features

### Admin Panel
- ✅ **Employee Management** - CRUD operations, search, filtering
- ✅ **Department Management** - Organizational structure
- ✅ **AI Dashboard** - Statistics and natural language queries
- ✅ **Excel Import** - Bulk employee data import
- ✅ **PDF Export** - Professional resume generation
- ✅ **Secure Authentication** - ASP.NET Identity

### REST API
- ✅ **JWT Authentication** - Secure token-based auth
- ✅ **Employee Self-Service** - Registration and information access
- ✅ **Document Generation** - Automatic PDF resumes
- ✅ **Email Notifications** - Welcome emails
- ✅ **Swagger Documentation** - Interactive API docs

### AI Features
- ✅ **Natural Language Queries** - Ask questions in plain English
- ✅ **Real-time Analytics** - Live data from database
- ✅ **Department Statistics** - Visual breakdowns
- ✅ **Employee Insights** - Status and distribution analysis

---

## 🏗️ Architecture

```
┌─────────────────────────────────────────────────────────┐
│                    TalentoPlus System                    │
├─────────────────────────────────────────────────────────┤
│                                                          │
│  ┌──────────────────┐         ┌──────────────────┐     │
│  │   Admin Panel    │         │     REST API     │     │
│  │   ASP.NET MVC    │         │   ASP.NET Core   │     │
│  │   Port: 5041     │         │   Port: 5109     │     │
│  │                  │         │                  │     │
│  │  - Employees     │         │  - Auth (JWT)    │     │
│  │  - Departments   │         │  - Employees     │     │
│  │  - AI Dashboard  │         │  - Departments   │     │
│  │  - Excel Import  │         │  - PDF Export    │     │
│  │  - PDF Export    │         │                  │     │
│  └────────┬─────────┘         └────────┬─────────┘     │
│           │                            │                │
│           └────────────┬───────────────┘                │
│                        ↓                                 │
│              ┌──────────────────┐                       │
│              │   PostgreSQL     │                       │
│              │   (Supabase)     │                       │
│              └──────────────────┘                       │
└─────────────────────────────────────────────────────────┘
```

---

## 🚀 Quick Start

### Using Docker (Recommended)

```bash
# Clone the repository
cd "TalentoPlus S.A.S.l"

# Deploy with Docker Compose
docker-compose -f docker-compose.simple.yml up -d

# Access the applications
# Admin Panel: http://localhost:5041
# REST API: http://localhost:5109
# Swagger: http://localhost:5109/swagger
```

### Manual Setup

```bash
# API
cd ApiTalento.Web
dotnet run  # http://localhost:5108

# Admin Panel
cd "TalentoPlus S.A.S.ll.Web"
dotnet run  # http://localhost:5040

# Tests
cd ApiTalento.Tests
dotnet test
```

### Default Credentials

**Admin Panel:**
- Email: `admin@talento.com`
- Password: `Admin123!`

**Test Employee (API):**
- Document: `1045228184`
- Email: `ceraluis4@gmail.com`

---

## 📁 Projects

### 1. [ApiTalento.Web](./ApiTalento.Web) - REST API

REST API providing employee services and authentication.

**Key Features:**
- JWT authentication
- Employee self-registration
- PDF resume generation
- Email notifications
- Swagger documentation

**Endpoints:**
- `GET /api/Departments` - List departments (public)
- `POST /api/Auth/register` - Register employee (public)
- `POST /api/Auth/login` - Login (public)
- `GET /api/Employees/me` - Get employee info (protected)
- `GET /api/Employees/me/resume` - Download resume PDF (protected)

[📖 Full API Documentation](./ApiTalento.Web/README.md)

---

### 2. [TalentoPlus S.A.S.ll.Web](./TalentoPlus%20S.A.S.ll.Web) - Admin Panel

Web-based administration interface for HR management.

**Key Features:**
- Employee CRUD operations
- Department management
- AI-powered dashboard
- Excel bulk import
- PDF export
- Responsive design

**Main Modules:**
- Employee Management
- Department Management
- AI Dashboard with statistics
- Import/Export tools

[📖 Full Web Documentation](./TalentoPlus%20S.A.S.ll.Web/README.md)

---

### 3. [ApiTalento.Tests](./ApiTalento.Tests) - Integration Tests

Comprehensive test suite for API validation.

**Coverage:**
- 9 integration tests
- Authentication flow
- Protected endpoints
- Database operations
- PDF generation

**Test Results:** ✅ 9/9 Passing

[📖 Full Test Documentation](./ApiTalento.Tests/README.md)

---

## 📚 Documentation

- [API Documentation](./ApiTalento.Web/README.md)
- [Web Panel Documentation](./TalentoPlus%20S.A.S.ll.Web/README.md)
- [Test Documentation](./ApiTalento.Tests/README.md)
- [Docker Deployment Guide](./docker_deployment.md)
- [API Testing Guide](./guia_pruebas_api.md)

---

## 🛠️ Technology Stack

### Backend
- **Framework:** ASP.NET Core 8.0
- **ORM:** Entity Framework Core
- **Database:** PostgreSQL (Supabase)
- **Authentication:** JWT Bearer + ASP.NET Identity
- **API Documentation:** Swagger/OpenAPI

### Frontend
- **Framework:** ASP.NET MVC / Razor Pages
- **UI:** Bootstrap 5
- **JavaScript:** jQuery
- **Icons:** Font Awesome

### Services
- **PDF Generation:** QuestPDF
- **Excel Processing:** EPPlus
- **Email:** SMTP
- **AI Processing:** Local NLP

### DevOps
- **Containerization:** Docker
- **Orchestration:** Docker Compose
- **Testing:** xUnit
- **CI/CD:** Ready for GitHub Actions

---

## 🐳 Deployment

### Docker Deployment

#### Simple Deployment (No Tests)

```bash
docker-compose -f docker-compose.simple.yml up -d
```

**Services:**
- API: http://localhost:5109
- Web: http://localhost:5041

#### Full Deployment (With Tests)

```bash
./deploy.sh
```

This script:
1. Runs integration tests
2. Deploys API if tests pass
3. Deploys Web panel
4. Verifies services are healthy

### Manual Deployment

See individual project READMEs for detailed instructions.

---

## 🧪 Testing

### Run All Tests

```bash
cd ApiTalento.Tests
dotnet test
```

### Test Coverage

- **Authentication:** Login validation, token generation
- **Endpoints:** Public and protected API endpoints
- **Database:** CRUD operations, relationships
- **Documents:** PDF generation and validation

**Latest Results:**
```
Total Tests: 9
Passed: 9 ✅
Failed: 0
Duration: ~4.4s
```

---

## 📊 AI Dashboard

The AI-powered dashboard provides:

### Statistics Cards
- Total employees
- Employees on vacation
- Active employees
- Inactive employees

### Department Distribution
- Visual breakdown by department
- Percentage calculations
- Employee counts

### AI Assistant
Ask questions in natural language:
- "How many employees are in Technology?"
- "How many employees are on vacation?"
- "What's the total number of employees?"
- "How many developers do we have?"

---

## 🔐 Security

- ✅ JWT token authentication (24-hour expiration)
- ✅ ASP.NET Identity for admin panel
- ✅ Password hashing
- ✅ CSRF protection
- ✅ CORS configuration
- ✅ HTTPS ready

---

## 📝 Configuration

### Environment Variables

**API:**
```bash
ConnectionStrings__DefaultConnection=<PostgreSQL>
Jwt__Key=<Secret Key>
Email__Username=<Email>
Email__Password=<App Password>
```

**Web:**
```bash
ConnectionStrings__DefaultConnection=<PostgreSQL>
Gemini__ApiKey=<API Key> (optional)
```

### Database

PostgreSQL database hosted on Supabase. Connection string required in `appsettings.json`.

---

## 🚦 Service Status

Check service health:

```bash
# API Health
curl http://localhost:5109/api/Departments

# Web Health
curl http://localhost:5041

# Docker Status
docker-compose -f docker-compose.simple.yml ps
```

---

## 🐛 Troubleshooting

### Port Already in Use

Change ports in `docker-compose.simple.yml`:
```yaml
ports:
  - "5110:5108"  # API
  - "5042:5040"  # Web
```

### Database Connection Error

1. Verify connection string
2. Check Supabase accessibility
3. Validate credentials

### Docker Build Fails

```bash
# Clean Docker cache
docker system prune -a -f

# Rebuild
docker-compose -f docker-compose.simple.yml build --no-cache
```

---

## 📈 Future Enhancements

- [ ] Role-based access control
- [ ] Real-time notifications
- [ ] Audit logging
- [ ] Advanced reporting
- [ ] Mobile app
- [ ] Two-factor authentication
- [ ] Performance monitoring

---

## 📄 License

This project is private and confidential.

---

## 👥 Team

Developed for TalentoPlus S.A.S.

---

## 📞 Support

For issues or questions, please refer to the individual project documentation:
- [API Issues](./ApiTalento.Web/README.md#troubleshooting)
- [Web Issues](./TalentoPlus%20S.A.S.ll.Web/README.md#troubleshooting)
- [Test Issues](./ApiTalento.Tests/README.md#troubleshooting)

---

**Version:** 1.0.0  
**Last Updated:** December 2025  
**Status:** ✅ Production Ready
