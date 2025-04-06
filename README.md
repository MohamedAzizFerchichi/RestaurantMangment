# RestaurantManagement

---

## Overview

RestaurantManagement is a Blazor WebAssembly application designed to streamline restaurant operations. It provides distinct interfaces for clients and administrators, facilitating tasks such as user authentication, menu viewing, order placement (implied), and administrative oversight.

---

## Features

*   **User Authentication:** Secure login and registration for clients and administrators.
*   **Role-Based Access:** Separate dashboards and functionalities for different user roles (Client, Admin).
*   **Client Dashboard:** Interface for clients (details to be added based on implementation).
*   **Admin Dashboard:** Interface for administrators (details to be added based on implementation).
*   **(Potential) Menu Management:** Based on `MenuItem.cs` model (details to be added).
*   **(Potential) Order Management:** Based on `Order.cs` and `OrderItem.cs` models (details to be added).

---

## Technologies Used

*   **Frontend:** Blazor WebAssembly (.NET 8)
*   **UI Component Library:** MudBlazor
*   **Authentication:** Custom Authentication State Provider, JWT (implied from typical Blazor auth patterns)
*   **Data Persistence:** Entity Framework Core (implied from `Data/RestaurantDbContext.cs`)
*   **Local Storage:** Used for client-side storage (e.g., tokens).

---

## Project Structure Highlights

*   `Models/`: Contains the core data structures (User, MenuItem, Order, etc.).
*   `Services/`: Houses business logic and services like Authentication and Local Storage interaction.
*   `Pages/`: Contains the Blazor components representing different views/pages (Login, Register, Dashboards).
*   `Layout/`: Defines the main application layout and navigation (`NavMenu.razor`).
*   `Data/`: Contains the `DbContext` for database interactions (`RestaurantDbContext.cs`).
*   `Program.cs`: Application entry point and service configuration.
*   `wwwroot/`: Static assets like CSS, images, and `index.html`.

---

## Getting Started

1.  **Prerequisites:**
    *   .NET 8 SDK installed.
    *   (Optional) SQL Server or other database configured if using EF Core migrations.
2.  **Clone the repository:**
    ```bash
    git clone <repository-url>
    cd RestaurantManagement
    ```
3.  **Install EF Core Tools (if needed):**
    *   The Entity Framework Core tools are required for database migrations. Install them as a local tool:
        ```bash
        dotnet new tool-manifest # Run if .config/dotnet-tools.json doesn't exist
        dotnet tool install dotnet-ef
        ```
4.  **Database Setup (if applicable):**
    *   Update connection string in `appsettings.json` (if present) or `Program.cs`.
    *   Create and apply database migrations (using the local tool):
        ```bash
        dotnet ef migrations add InitialCreate # Or your desired migration name
        dotnet ef database update
        ```
5.  **Run the application:**
    ```bash
    dotnet run
    ```
    The application will be accessible at the URL provided in the console output (usually `https://localhost:xxxx` or `http://localhost:xxxx`).

---

## Contributing

Contributions are welcome! Please follow standard fork-and-pull-request workflow. Ensure code adheres to existing project style and includes relevant tests if applicable.
