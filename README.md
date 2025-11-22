# ToDo Full-Stack Application

TODO list application built using Angular 20 and .NET 8 Web API.

## Repository

https://github.com/ryan-davari/ToDo

## Tech Stack

### Frontend

- Angular 20
- Angular Material
- SCSS styling
- Karma + Jasmine unit tests
- HttpClient + HttpClientTestingModule

### Backend

- .NET 8 Web API
- layering: Controller → Service → Repository
- In-memory data store (no database required)
- AutoMapper
- xUnit + Moq unit tests

## Features

- View TODO list
- Add new tasks
- Edit tasks
- Delete tasks
- Mark complete/incomplete
- Show/hide completed tasks
- In-memory backend storage
- Unit tests (backend + frontend)

# Running the Application

## 1. Clone the repository

- git clone https://github.com/ryan-davari/ToDo.git
- cd <repo-folder>

# Backend (.NET 8 API)

## Install dependencies

- cd ToDo.Api
- dotnet restore

## Run the API

- dotnet run

API will run at:

- https://localhost:7083
- http://localhost:5137

# Frontend (Angular UI)

- Angular CLI: 20.3.10
- Node: 22.19.0
- Package Manager: npm 10.9.3
- OS: win32 x64
- Angular: 20.3.12

## Install dependencies

- cd todo-ui
- npm install

## Run the UI

- npm start

# or

- ng serve

- Runs at:
- http://localhost:4200

- Backend API URL is located in:
- src/environments/environment.ts

# Running Tests

## Backend Tests

- cd ToDo.Api.Tests
- dotnet test

## Frontend Tests

- cd todo-ui
- ng test

# API Endpoints

- GET /api/TaskItem → Get all tasks
- GET /api/TaskItem/{id} → Get task by Id
- POST /api/TaskItem → Create a task
- PUT /api/TaskItem/{id} → Update a task
- DELETE /api/TaskItem/{id} → Delete a task

# Contact

- Ryan Davari
- Email: ryan.davari@gmail.com
- Location: Melbourne, Australia
