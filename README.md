# ToDo Full-Stack Application

A complete full-stack TODO list application built with **Angular 20** and **.NET 8 Web API**, featuring:

- SQL Server persistence via **Entity Framework Core**
- User authentication with **ASP.NET Core Identity**
- **JWT Bearer Authentication** (UI + API)
- Role-based authorization (Admin/User)
- User-specific TODO lists

---

## 📌 Repository

https://github.com/ryan-davari/ToDo-EF-SQL

Contains both backend (.NET) and frontend (Angular) projects.

---

# 🚀 Tech Stack

## 🖥️ Frontend

- **Angular 20**
- **Angular Material**
- **SCSS styling**
- **JWT-based authentication**
- Login & Register pages
- HTTP interceptor for attaching JWT tokens
- Route guards for protected routes
- Task CRUD UI
- Karma + Jasmine tests

---

## 🧩 Backend

- **.NET 8 Web API**
- Clean architecture: **Controller → Service → Repository → EF Core**
- **SQL Server**  
- **Entity Framework Core 8**  
  - Code-first migrations  
  - Fluent API configuration  
- **ASP.NET Core Identity**  
  - AppUser model  
  - Identity tables  
  - Role seeding (Admin / User)  
- **JWT Authentication**  
  - Login / Register  
  - Token generation (7-day expiry)  
- AutoMapper for DTOs  
- xUnit tests + mocking  

---

# 🔒 Authentication & Authorization

The API uses **JWT Bearer Auth** + **ASP.NET Core Identity**.

### Implemented

- Register new users  
- Login and receive JWT token  
- Store token in browser (UI)  
- Angular Auth Interceptor auto-attaches token  
- All `/api/TaskItem` endpoints require authentication  
- Each task belongs to a specific user  
- Users can **only access their own tasks**

### Main Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/Account/register` | Register |
| POST | `/api/Account/login` | Login & get JWT |
| GET | `/api/TaskItem` | Get tasks for current user |
| POST | `/api/TaskItem` | Create task |
| PUT | `/api/TaskItem/{id}` | Update task |
| DELETE | `/api/TaskItem/{id}` | Delete task |

---

# 🗄️ Database

### Technology

- SQL Server LocalDB (default)
- Configured in `appsettings.json`

### Apply all EF migrations

```bash
cd Backend/ToDo.DAL
dotnet ef database update --startup-project ../ToDo.Api
```

This creates:

- TaskItems table
- Identity tables:
  - AspNetUsers  
  - AspNetRoles  
  - AspNetUserRoles  
  - AspNetUserClaims  
  - etc.

---

# 📦 Project Structure

```
Backend/
 ├── ToDo.Api             → Controllers, Identity, DI, JWT, Swagger
 ├── ToDo.DAL             → DbContext, Entities, EF Core Migrations
 ├── ToDo.Api.Tests       → Unit tests

Frontend/
 └── todo-ui              → Angular 20 application
```

---

# ▶️ Running the Application

## 1️⃣ Clone the repository

```
git clone https://github.com/ryan-davari/ToDo-EF-SQL.git
cd ToDo-EF-SQL
```

---

# 🧑‍💻 Backend Setup (.NET 8)

### Install dependencies

```
cd Backend/ToDo.Api
dotnet restore
```

### Run the API

```
dotnet run
```

API will start at:

- https://localhost:7083  
- http://localhost:5137  

Swagger UI available at:  
`/swagger`

---

# 🖥️ Frontend Setup (Angular 20)

### Install dependencies

```
cd Frontend/todo-ui
npm install
```

### Run Angular

```
npm start
# or
ng serve
```

Runs at:  
**http://localhost:4200**

API base URL is in:  
`src/environments/environment.ts`

---

# 🧪 Running Tests

## Backend Tests

```
cd Backend/ToDo.Api.Tests
dotnet test
```

## Frontend Tests

```
cd Frontend/todo-ui
ng test
```

---

# ✨ Features Summary

### 🔧 Task Management  
- Add, edit, delete  
- Mark complete/incomplete  
- Show/hide completed items  
- User-specific data  

### 🔒 Security  
- JWT authentication  
- ASP.NET Identity  
- Roles (Admin/User)  
- Secured endpoints  
- Angular guards + interceptors  

### 💾 Persistence  
- SQL Server  
- EF Core migrations  
- Fluent configuration  
- UserId foreign key in TaskItems  

---

# 📬 Contact

**Ryan Davari**  
📧 ryan.davari@gmail.com  
📍 Melbourne, Australia
