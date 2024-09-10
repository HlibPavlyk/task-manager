# Task Manager API

This project provides a backend API for managing tasks with user authentication. Built with .NET 8 and Entity Framework, the API allows users to create, update, delete, and retrieve tasks associated with their accounts. It supports JWT-based authentication and offers paginated task querying, with filtering and sorting options.

## Setup Instructions

### Prerequisites
- [Docker](https://www.docker.com/get-started)

### Running the Project Locally

1. **Clone the repository:**
    ```sh
    git clone https://github.com/HlibPavlyk/task-manager.git
    cd task-manager
    ```

2. **Build and run the Docker containers:**
    ```sh
    docker-compose up --build
    ```

3. **Access the API:**
    - The API will be available at `https://localhost:5000`

## API Documentation




### User Authentication

#### `POST /api/users/register`
- **Description:** Registers a new user in the system.
- **Request Body:**
    ```json
    {
        "username": "string",
        "email": "string",
        "password": "string"
    }
    ```
- **Response:**
    ```json
    {
        "id": "string",
    }
    ```

#### `POST /api/users/login`
- **Description:** Authenticates a user and returns a JWT token.
- **Request Body:**
    ```json
    {
        "usernameOrEmail": "string",
        "password": "string"
    }
    ```
- **Response:**
    ```json
    {
        "username": "string",
        "email": "string",
        "token": "string"
    }
    ```

### Task Management

#### `POST /api/tasks`
- **Description:** Creates a new task. Requires JWT authentication.
- **Request Body:**
    ```json
    {
        "title": "string", // Required
        "description": "string", // Optional
        "status": "string", // Required, Enum: ["Pending", "InProgress", "Completed"]
        "priority": "string", // Required, Enum: ["Low", "Medium", "High"]
        "dueDate": "string" // Optional, ISO 8601 format
    }
    ```
- **Response:**
    ```json
    {
        "id": "int",
        "title": "string",
        "description": "string",
        "status": "string",
        "priority": "string",
        "dueDate": "string",
        "createdAt": "string",
        "updatedAt": "string"
    }
    ```

#### `GET /api/tasks`
- **Description:** Retrieves a paginated list of tasks. Supports filtering and sorting. Requires JWT authentication.
- **Query Parameters:**
  - `status` (optional): Filter by task status. Enum: \["Pending", "InProgress", "Completed"\]
  - `priority` (optional): Filter by task priority. Enum: \["Low", "Medium", "High"\]
  - `dueDate` (optional): Filter by due date.
  - `sortBy` (optional): Sort by `DueDate`, `Priority`, or `CreatedAt` by default.
  - `page` (required): Page number for pagination.
  - `pageSize` (required): Number of tasks per page.
- **Response:**
    ```json
    {
        "totalPages": "int",
        "tasks": [
            {
                "id": "int",
                "title": "string",
                "description": "string",
                "status": "string",
                "priority": "string",
                "dueDate": "string",
                "createdAt": "string",
                "updatedAt": "string"
            }
        ],[...]
    }
    ```

#### `GET /api/tasks/{id}`
- **Description:** Retrieves a specific task by ID. Requires JWT authentication.
- **Response:**
    ```json
    {
        "id": "int",
        "title": "string",
        "description": "string",
        "status": "string",
        "priority": "string",
        "dueDate": "string",
        "createdAt": "string",
        "updatedAt": "string"
    }
    ```

#### `PUT /api/tasks/{id}`
- **Description:** Updates an existing task by ID. Requires JWT authentication.
- **Request Body:**
    ```json
    {
        "title": "string", // Required
        "description": "string", // Optional
        "status": "string", // Required, Enum: ["Pending", "InProgress", "Completed"]
        "priority": "string", // Required, Enum: ["Low", "Medium", "High"]
        "dueDate": "string" // Optional, ISO 8601 format
    }
    ```
- **Response:**
    ```json
    {
        "id": "int",
        "title": "string",
        "description": "string",
        "status": "string",
        "priority": "string",
        "dueDate": "string",
        "createdAt": "string",
        "updatedAt": "string"
    }
    ```

#### `DELETE /api/tasks/{id}`
- **Description:** Deletes a task by its ID (specified in the route). Requires JWT authentication.

- This updated description clarifies that the response for a successful deletion returns no content, as expected with a `204 No Content` status.

## Architecture and Design

### Architecture
The TaskManager API follows the **Clean Architecture** principles, which emphasize separation of concerns and clear boundaries between layers. This ensures maintainability, scalability, and testability by isolating the business logic from the infrastructure.

- **API Layer**: Handles incoming HTTP requests and maps them to services in the application layer.
- **Application Layer**: Contains business logic, application services, and DTOs. This layer is responsible for orchestrating use cases and business rules.
- **Domain Layer**: Represents the core business logic, including domain entities and business rules. This layer is isolated from any external dependencies, ensuring that it only focuses on the core domain of the system.
- **Infrastructure Layer**: Responsible for managing external concerns like databases (using Entity Framework Core), external services, and repository implementations. It interacts with the application layer to persist data.

### Design Choices
- **Dependency Injection (DI):** Used to manage dependencies and promote loose coupling. Services, repositories, and other dependencies are injected where needed.
- **Entity Framework Core (EF):** Used for data access and database migrations. It simplifies database interactions and ensures the database schema is in sync with the application models.
- **JWT Authentication:** Used for securing API endpoints. It provides a secure way to authenticate users and protect resources.
- **Swagger:** Used for API documentation and testing. It provides an interactive interface for exploring and testing the API endpoints.
- **Docker:** Used for containerization and easy deployment. It ensures the application runs consistently across different environments.
- **AutoMapper:** Used for object-object mapping to simplify data transfer between layers. It reduces the boilerplate code needed for mapping between domain models and DTOs.
- **Clean Architecture:** Ensures separation of concerns and promotes maintainability. Each layer has a distinct responsibility, making the codebase easier to manage and extend.
- **Unit Testing:** Ensures the reliability and correctness of the application. Key services and components are covered by unit tests to catch issues early in the development process.
- **DTOs (Data Transfer Objects):** Used to transfer data between layers. They encapsulate the data and ensure only necessary information is exposed.
- **Repositories:** Abstract the data access logic from the business logic. They provide a clean API for interacting with the database.
- **Unit of Work:** Manages transactions and ensures that a series of operations are completed successfully. It helps maintain data integrity and consistency.