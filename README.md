# Memorizz

Memorizz is a web application built with ASP.NET Core and Entity Framework Core. It includes identity management and various other features.

## Features

- User authentication and authorization
- Role management
- API endpoints for identity management
- Custom middleware for exception handling
- Integration with MediatR for CQRS pattern
- Fluent validation for request validation
- Swagger for API documentation

## Getting Started

### Prerequisites

- .NET 6.0 SDK or later
- PostgreSQL

### Installation

1. Clone the repository:
    ```sh
    git clone https://github.com/DrKrusto/Memorizz.git
    cd Memorizz
    ```

2. Set up the database:
    - Update the connection string in `appsettings.json`:
        ```json
        "ConnectionStrings": {
            "DefaultConnection": "Host=localhost;Database=memorizz;Username=yourusername;Password=yourpassword"
        }
        ```

3. Apply migrations:
    ```sh
    dotnet ef database update
    ```

4. Run the application:
    ```sh
    dotnet run --project Memorizz.Host
    ```

### Usage

- Access the API documentation at `https://localhost:5001/swagger`
- Use the provided endpoints for user and role management