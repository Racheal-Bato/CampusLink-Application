# CampusLink Application

##  Overview
CampusLink is a web-based application built with ASP.NET Core MVC for managing student registration and campus activities.

## Technologies Used
- ASP.NET Core MVC
- Entity Framework Core
- SQL Server
- Bootstrap 5
- Azure DevOps (for CI/CD)

##  Setup
1. Clone the repo
2. Update `appsettings.json` with your DB connection
3. Run migrations: `dotnet ef database update`
4. Run the app: `dotnet run`

##  Folder Structure
- `/Models` – Entity models
- `/Controllers` – Logic handlers
- `/Views` – Razor UI
- `/Data` – EF Core database context

##  License
No licence
