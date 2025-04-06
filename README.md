# Restaurant Management System

A full-stack web application for managing restaurant operations, built with .NET 8, Blazor WebAssembly, and PostgreSQL.

## Features

- **User Management**
  - User registration and authentication
  - Role-based access control (Admin/User)
  - User profile management

- **Menu Management**
  - Add, edit, and delete menu items
  - Categorize items (Appetizers, Main Courses, Desserts, etc.)
  - Set prices and availability
  - Upload and manage item images

- **Order Management**
  - Create and track orders
  - Manage order status
  - View order history
  - Process payments

- **Plate Management**
  - Add, edit, and delete plates
  - Categorize plates
  - Set prices and availability
  - Upload and manage plate images

## Tech Stack

- **Frontend**
  - Blazor WebAssembly
  - MudBlazor UI Components
  - HTML5/CSS3
  - JavaScript/TypeScript

- **Backend**
  - .NET 8
  - ASP.NET Core Web API
  - Entity Framework Core
  - PostgreSQL Database
  - JWT Authentication

- **Development Tools**
  - Visual Studio 2022
  - Visual Studio Code
  - Git
  - PostgreSQL

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) or [Visual Studio Code](https://code.visualstudio.com/)
- [PostgreSQL](https://www.postgresql.org/download/)
- [Git](https://git-scm.com/downloads)

### Installation

1. Clone the repository:
```bash
git clone https://github.com/yourusername/RestaurantManagement.git
cd RestaurantManagement
```

2. Configure the database:
- Create a PostgreSQL database named `restaurant`
- Update the connection string in `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Database=restaurant;Username=your_username;Password=your_password"
}
```

3. Apply database migrations:
```bash
cd RestaurantManagement.Server
dotnet ef database update
```

4. Run the application:
```bash
dotnet run
```

The application will be available at:
- Frontend: https://localhost:5001
- Backend API: https://localhost:5239

## Project Structure

```
RestaurantManagement/
├── RestaurantManagement.Client/         # Blazor WebAssembly frontend
├── RestaurantManagement.Server/         # ASP.NET Core Web API backend
├── RestaurantManagement.Shared/         # Shared models and utilities
└── README.md
```

## Development

### Running the Application

1. Start the backend server:
```bash
cd RestaurantManagement.Server
dotnet run
```

2. Start the frontend client:
```bash
cd RestaurantManagement.Client
dotnet run
```

### Database Migrations

To create a new migration:
```bash
cd RestaurantManagement.Server
dotnet ef migrations add MigrationName
```

To apply migrations:
```bash
dotnet ef database update
```

## Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contact

Your Name - your.email@example.com

Project Link: [https://github.com/yourusername/RestaurantManagement](https://github.com/yourusername/RestaurantManagement)
