# 🏢 Employee Management System (EMS)

A role-based **Employee Management System** REST API built with **ASP.NET Core Web API (.NET 8 LTS)** and **MySQL**.

It manages employees, departments, designations, roles, personal details, documents, leave approval workflows, and a complete recruitment module (job postings and candidate applications), with JWT security, email notifications, file uploads, and generic search/filter/sort/pagination.

---

## 📑 Table of Contents

- [Project Overview](#-project-overview)
- [Key Features](#-key-features)
- [Tech Stack](#️-tech-stack)
- [Architecture](#️-architecture)
- [Database Design](#️-database-design)
- [Core Modules](#-core-modules)
- [API Endpoints](#-api-endpoints)
- [Authentication & Authorization](#-authentication--authorization)
- [Email Notifications](#-email-notifications)
- [File Upload](#-file-upload)
- [Search, Filter, Sort & Pagination](#-search-filter-sort--pagination)
- [Global Exception Handling](#-global-exception-handling)
- [Logging](#-logging)
- [Security Practices](#-security-practices)
- [Getting Started](#️-getting-started)
- [Configuration](#-configuration)
- [Deployment](#-deployment)
- [Roadmap](#-roadmap)
- [Author](#-author)

---

## 🚀 Project Overview

EMS manages the complete employee and recruitment workflow:

```text
User Registration / Login
        ↓
Employee Profile Creation
        ↓
Department / Designation / Role / Manager Assignment
        ↓
Personal Details & Document Upload
        ↓
Leave Application
        ↓
Manager Approval / Rejection (Email Notification)
```

Recruitment workflow:

```text
HR creates Job Posting
        ↓
Candidate applies (Resume + Photo + Education)
        ↓
HR reviews applications (Filter / Sort / Paginate)
```

---

## ✨ Key Features

- 🔐 JWT authentication with role-based authorization (Admin, HR, Manager, Employee)
- 📧 OTP-based forgot / reset password via email (MailKit SMTP)
- 👥 Employee management with Personal Details (1:1) and Documents (1:N)
- 🗂️ Department, Designation, and Role management
- 🌴 Leave management with manager-scoped approval workflow
- 📢 Recruitment module: Job Postings and Candidate Applications
- 📁 File upload (resume, photo, documents) with validation and orphan-file cleanup
- 🔁 Generic Repository + Unit of Work for transactional, multi-entity operations
- 🗑️ Cascading soft-delete across related entities
- 🛡️ Global exception-handling middleware with standardized API responses
- 📝 Structured logging with Serilog (console + rolling file)
- 🔎 Reusable generic filter, sort, and pagination extensions

---

## 🛠️ Tech Stack

### Backend

| Technology | Purpose |
|---|---|
| ASP.NET Core Web API (.NET 8 LTS) | REST API framework |
| Entity Framework Core (DB-First) | ORM with scaffolded entities |
| Pomelo.EntityFrameworkCore.MySql | MySQL provider |
| MySQL | Relational database |
| JWT Bearer | Authentication |
| BCrypt.Net-Next | Password hashing |
| MailKit / MimeKit | SMTP email |
| Serilog | Structured logging |
| Swagger / OpenAPI | API documentation |

### Tools & Hosting

- Visual Studio 2022
- MySQL Workbench
- Postman
- Git / GitHub
- Aiven (cloud MySQL)
- Render (API hosting)

---

## 🏗️ Architecture

The project follows a **layered architecture**:

```text
Controller  →  Service  →  Repository / Unit of Work  →  Database
   (API)      (Business)        (Data Access)            (MySQL)
```

### Solution Structure

```text
EmployeeManagementSystem
│
├── EMS.API
│   ├── Controllers
│   ├── Middleware          (Global exception handler)
│   ├── Properties
│   ├── wwwroot             (Uploaded files)
│   └── Program.cs
│
├── EMS.Service
│   ├── DTOs
│   ├── Interfaces
│   ├── Services
│   └── Filters             (Per-entity filter extensions)
│
├── EMS.Data
│   ├── Models              (Scaffolded entities)
│   ├── EmsDbContext
│   ├── Repositories        (Generic Repository<T>)
│   └── UnitOfWork
│
├── EMS.Core
│   ├── Interfaces
│   └── Enums
│
└── EMS.Common
    ├── Helpers             (JWT, Email, File storage, URL helper)
    ├── Settings            (EmailSettings, FileStorageSettings)
    ├── Extensions          (Sort / Paging)
    └── Wrappers            (ApiResponse<T>)
```

### Design Patterns Used

- **Repository Pattern** (generic `Repository<T>`)
- **Unit of Work Pattern** (single `SaveChanges`, explicit transactions)
- **DTO Pattern** (manual mapping between entities and DTOs)
- **Dependency Injection**
- **Middleware Pipeline** (global exception handling)

---

## 🗄️ Database Design

### Entity Relationships

```text
User 1 : 1 Employee
Department 1 : N Employee
Designation 1 : N Employee
Role 1 : N Employee
Employee 1 : 1 EmployeePersonalDetail
Employee 1 : N EmployeeDocument
Employee N : 1 Employee (Manager, via ManagerId)
Employee 1 : N LeaveRequest
LeaveType 1 : N LeaveRequest
JobPosting 1 : N Career (Application)
Career 1 : N CareerEducation
Career 1 : N CareerDocument
```

### Main Tables

| Table | Description |
|---|---|
| Users | Login credentials, role, OTP fields |
| Roles | Admin, HR, Manager, Employee |
| Departments | Department master |
| Designations | Job titles master |
| Employees | Core employee profile |
| EmployeePersonalDetails | Personal, family, bank, education details |
| EmployeeDocuments | Uploaded employee documents |
| LeaveTypes | Casual, Sick, Earned, etc. |
| LeaveRequests | Leave applications and approval status |
| JobPostings | Open vacancies |
| Careers | Candidate applications |
| CareerEducations | Candidate education history |

### Common Audit Columns

Every table follows the same audit pattern:

```text
CreatedDate, CreatedBy
ModifiedDate, ModifiedBy
IsDeleted, DeletedDate, DeletedBy   (soft delete)
```

---

## 📦 Core Modules

## 1. Authentication & User Management

- User registration
- Login with JWT
- Role-based authorization
- Forgot password (OTP via email)
- Reset password
- Account activation / deactivation
- BCrypt password hashing

### User Roles

```text
Admin
HR
Manager
Employee
```

---

## 2. Department / Designation / Role Management

- Create, Get, Update, Soft Delete
- Unique name validation
- Used as dropdown lookups when creating employees

---

## 3. Employee Management

- Create employee profile (linked to an existing registered user via email)
- Get by Id (with Department, Designation, Role names)
- Get all with filter, sort, and pagination
- Managers lookup (for the reporting-manager dropdown)
- Soft delete with cascade

Cascade behaviour on delete:

```text
Employee
  ├── EmployeePersonalDetail   (soft deleted)
  ├── EmployeeDocuments        (soft deleted)
  └── User                     (soft deleted + deactivated)
```

---

## 4. Employee Personal Details

A one-to-one extension of the employee profile:

- Marital status, nationality, Aadhar / PAN
- Emergency contact details
- Bank details
- Address (district, state, pincode)
- Family details (father, mother, siblings, family income)
- Education history (SSLC, HSC / Diploma, UG, PG)

---

## 5. Employee Documents

An employee can have multiple documents.

### Document Types

```text
Resume
IDProof
HSCCertificate
SSLCCertificate
UGDegree
PGDegree
OfferLetter
```

Uploading a new document of the same type replaces the previous active one.

---

## 6. Leave Management

### Leave Workflow

```text
Employee
   ↓
Apply Leave
   ↓
Validation (dates, leave type, overlap)
   ↓
Pending
   ↓
Reporting Manager Review
   ↓
Approved / Rejected
```

### Leave Status

```text
Pending
Approved
Rejected
```

### Leave Validation Rules

Before a leave request is created:

- Employee profile must exist
- Leave type must exist
- End date must not be earlier than start date
- Start date must not be in the past
- No overlapping Pending / Approved leave for the same employee
- Rejected leaves do not block re-applying for the same dates

### Approval Rules

- Only the employee's **reporting manager** can approve or reject
- A request that is already actioned cannot be actioned again
- The acting manager is always taken from the JWT token, never from client input

---

## 7. Recruitment: Job Postings

- Create, update, soft delete
- Public listing (open postings only)
- Admin / HR listing (all statuses)
- Filter by department, employment type, work mode, salary, experience

### Employment Type

```text
FullTime
PartTime
Contract
Internship
```

### Work Mode

```text
Onsite
Remote
Hybrid
```

### Job Posting Status

```text
Open
Closed
OnHold
```

---

## 8. Recruitment: Career Applications

Candidates apply against a job posting.

```text
JobPosting 1 : N Career
Career 1 : N CareerEducation
```

Application includes:

- Personal and contact details
- Current designation, company, salary, notice period
- Skill set, LinkedIn and GitHub URLs
- Photo and resume upload
- Multiple education records

### Validation Rules

- At least one education record is required
- Education `ToDate` must not be earlier than `FromDate`
- Duplicate application (same email + same position) is blocked
- Uploaded files are deleted if the database save fails

---

## 🔌 API Endpoints

> Exact routes may vary slightly. Full documentation is available in Swagger UI.

### Auth

| Method | Endpoint | Access |
|---|---|---|
| POST | `/api/auth/register` | Public |
| POST | `/api/auth/login` | Public |
| POST | `/api/auth/forgot-password` | Public |
| POST | `/api/auth/reset-password` | Public |

### Employee

| Method | Endpoint | Access |
|---|---|---|
| POST | `/api/employee` | Admin, HR |
| GET | `/api/employee` | Authenticated (filter / sort / paginate) |
| GET | `/api/employee/{id}` | Authenticated |
| GET | `/api/employee/managers-lookup` | Admin, HR |
| DELETE | `/api/employee/delete-employee/{id}` | Admin, HR |

### Employee Documents

| Method | Endpoint | Access |
|---|---|---|
| POST | `/api/employeedocument/upload` | Admin, HR |

### Department / Designation / Role

| Method | Endpoint | Access |
|---|---|---|
| GET | `/api/department`, `/api/designation`, `/api/role` | Authenticated |
| POST / PUT / DELETE | Same routes | Admin, HR |

### Leave

| Method | Endpoint | Access |
|---|---|---|
| GET / POST | `/api/leavetype` | Authenticated / Admin, HR |
| POST | `/api/leaverequest/apply` | Authenticated |
| GET | `/api/leaverequest/my-leaves` | Authenticated |
| GET | `/api/leaverequest/pending-approvals` | Manager |
| PUT | `/api/leaverequest/{id}/approve` | Manager |
| PUT | `/api/leaverequest/{id}/reject` | Manager |

### Recruitment

| Method | Endpoint | Access |
|---|---|---|
| POST / PUT / DELETE | `/api/jobposting` | Admin, HR |
| GET | `/api/jobposting` | Public (open postings) |
| POST | `/api/career` | Public (candidate apply) |
| GET | `/api/career` | Admin, HR (filter / sort / paginate) |
| DELETE | `/api/career/{id}` | Admin, HR |

---

## 🔐 Authentication & Authorization

```text
Login (email + password)
        ↓
Password verified with BCrypt
        ↓
JWT generated (UserId, Email, Role, Name)
        ↓
Client sends: Authorization: Bearer <token>
        ↓
JWT middleware validates signature and expiry
        ↓
[Authorize(Roles = "...")] enforces role access
```

Important rule used across the project:

```text
"WHO is doing the action"  → always from JWT token
"WHICH record is targeted" → from URL / request body
```

---

## 📧 Email Notifications

HTML emails are sent through SMTP (MailKit) for:

- OTP for password reset
- New leave request (to the reporting manager)
- Leave approved / rejected (to the employee)

Email failure never rolls back the main operation. It is logged and the request continues.

---

## 📁 File Upload

Employee documents are stored under `wwwroot`:

```text
wwwroot/UploadedDocuments/{EmployeeId}/{DocumentType}/{guid}_{filename}
```

Validation:

- Maximum file size (configurable)
- Allowed extensions: `.pdf .jpg .jpeg .png .doc .docx` (configurable)
- Unique GUID-based file names
- Relative path stored in DB (portable across environments)
- Full public URL built at response time
- Orphaned files deleted automatically if the DB save fails

---

## 🔎 Search, Filter, Sort & Pagination

Built once as generic `IQueryable<T>` extensions and reused across modules.

```text
Filter  →  Sort (asc / desc via sort-map)  →  Pagination
```

Examples:

```http
GET /api/employee?pageNumber=1&pageSize=10
GET /api/employee?name=Arun&departmentId=1
GET /api/career?sortBy=experience&sortDescending=true
```

Paged response shape:

```json
{
  "items": [],
  "pageNumber": 1,
  "pageSize": 10,
  "totalRecords": 42
}
```

---

## 🛡️ Global Exception Handling

A custom middleware converts exceptions into a consistent response shape and keeps controllers free of try / catch blocks.

| Exception | HTTP Status |
|---|---|
| `KeyNotFoundException` | 404 Not Found |
| `InvalidOperationException` | 409 Conflict |
| `ArgumentException` | 400 Bad Request |
| `UnauthorizedAccessException` | 401 Unauthorized |
| Any other exception | 500 Internal Server Error |

Standard response:

```json
{
  "success": false,
  "message": "Employee with Id 99 not found.",
  "data": null,
  "statusCode": 404,
  "traceId": "0HN...",
  "timestamp": "2026-10-24T10:00:00Z"
}
```

Stack traces are shown only in the Development environment.

---

## 📝 Logging

- Serilog with console and rolling daily file sinks (`Logs/log-.txt`)
- Structured logging with message templates (no string interpolation)
- Business events logged: leave applied / approved / rejected, employee created / deleted, document uploaded
- Errors logged centrally by the exception middleware

---

## 🔒 Security Practices

- Passwords hashed with BCrypt
- Actor identity (`CreatedBy`, `ModifiedBy`, `DeletedBy`) always derived from the JWT
- Role is forced to `Employee` on registration; it is never accepted from client input
- Generic "Invalid email or password" message on failed login
- OTP is single-use with a 10-minute expiry
- Secrets are kept out of source control (User Secrets locally, environment variables in production)
- GitHub push protection enabled to block accidental secret commits

---

## ⚙️ Getting Started

### Prerequisites

- .NET 8 SDK
- MySQL Server (local) or a cloud MySQL such as Aiven
- Visual Studio 2022 (or VS Code)

### 1. Clone the repository

```bash
git clone https://github.com/<your-username>/EmployeeManagementSystem.git
cd EmployeeManagementSystem
```

### 2. Create the database

Create the MySQL database and run the table scripts (Users, Roles, Departments, Designations, Employees, EmployeePersonalDetails, EmployeeDocuments, LeaveTypes, LeaveRequests, JobPostings, Careers, CareerEducations).

### 3. Scaffold entities (DB-First)

```powershell
Scaffold-DbContext "<connection-string>" Pomelo.EntityFrameworkCore.MySql `
    -OutputDir Models -ContextDir . -Context EmsDbContext `
    -Project EMS.Data -StartupProject EMS.API -Force
```

> ⚠️ After every scaffold, open `EmsDbContext.cs` and **delete the generated `OnConfiguring` method**. It contains a hard-coded connection string that overrides your configured one.

### 4. Configure local secrets

```bash
cd EMS.API
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "server=localhost;port=3306;database=EmployeeManagementSystemDB;user=root;password=YOUR_PASSWORD"
dotnet user-secrets set "JwtSettings:SecretKey" "YOUR_LONG_RANDOM_SECRET_KEY"
dotnet user-secrets set "EmailSettings:SenderEmail" "your-email@gmail.com"
dotnet user-secrets set "EmailSettings:Username" "your-email@gmail.com"
dotnet user-secrets set "EmailSettings:Password" "your-gmail-app-password"
```

### 5. Run the application

```bash
dotnet run --project EMS.API
```

### 6. Open Swagger

```text
https://localhost:7196/swagger
```

---

## 🔧 Configuration

`appsettings.json` contains only non-sensitive structure. Sensitive values are left empty:

```json
{
  "ConnectionStrings": { "DefaultConnection": "" },
  "JwtSettings": {
    "SecretKey": "",
    "Issuer": "EMS.API",
    "Audience": "EMS.Client",
    "ExpiryMinutes": 60
  },
  "EmailSettings": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "SenderName": "Employee Management System",
    "SenderEmail": "",
    "Username": "",
    "Password": "",
    "EnableSsl": true
  },
  "FileStorageSettings": {
    "BasePath": "UploadedDocuments",
    "MaxFileSizeInMB": 5,
    "AllowedExtensions": [ ".pdf", ".jpg", ".jpeg", ".png", ".doc", ".docx" ]
  }
}
```

Configuration priority:

```text
appsettings.json  →  appsettings.{Environment}.json  →  Environment Variables
(lowest)                                                  (highest)
```

---

## 🌐 Deployment

```text
GitHub (source)
      ↓
Render (API hosting)
      ↓
Aiven (cloud MySQL)
```

Set these **Environment Variables** in the Render dashboard:

| Key | Description |
|---|---|
| `ASPNETCORE_ENVIRONMENT` | `Production` |
| `ConnectionStrings__DefaultConnection` | Aiven MySQL connection string (with SSL) |
| `JwtSettings__SecretKey` | JWT signing key |
| `EmailSettings__SenderEmail` | SMTP sender email |
| `EmailSettings__Username` | SMTP username |
| `EmailSettings__Password` | SMTP app password |

> Double underscore `__` maps to nested JSON keys in ASP.NET Core configuration.


---

## 🗺️ Roadmap

- [ ] Salary management module (table is ready, APIs pending)
- [ ] Leave balance tracking per leave type
- [ ] Employee Update API and self-service profile update
- [ ] Authenticated document download endpoint (instead of public `wwwroot` URLs)
- [ ] Cloud file storage (Azure Blob / S3) instead of local disk
- [ ] Refresh tokens
- [ ] Unit and integration tests (xUnit, Moq)
- [ ] Docker support and CI/CD pipeline

---

## 👨‍💻 Author

**Sivasubramanian S**

- GitHub: https://github.com/sivasubramanian2004
- LinkedIn: https://www.linkedin.com/in/sivasubramanian-s-988389325/
- Email: shivasubramanian3332@gmail.com

---

## 📄 License

This project is built for learning and portfolio purposes.
