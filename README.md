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
  - `username`: Required, string.
  - `email`: Required, string.
  - `password`: Required, string.
    ```json
    {
        "username": "string",
        "email": "string",
        "password": "string"
    }
    ```
- **Response:**
  - `id`: The ID of the newly created user.
    ```json
    {
        "id": "string"
    }
    ```

#### `POST /api/users/login`
- **Description:** Authenticates a user and returns a JWT token.
- **Request Body:**
  - `usernameOrEmail`: Required, string (either username or email).
  - `password`: Required, string.
    ```json
    {
        "usernameOrEmail": "string",
        "password": "string"
    }
    ```
- **Response:**
  - `username`: The username of the authenticated user.
  - `email`: The email of the authenticated user.
  - `token`: JWT token for authentication.
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
  - `title`: Required, string.
  - `description`: Optional, string.
  - `status`: Required, enum value, one of ["Pending", "InProgress", "Completed"].
  - `priority`: Required, enum value, one of ["Low", "Medium", "High"].
  - `dueDate`: Optional, string, in ISO 8601 format.
    ```json
    {
        "title": "string",
        "description": "string",
        "status": "string",
        "priority": "string",
        "dueDate": "string"
    }
    ```
- **Response:**
  - Returns the task details, including the `id`, `createdAt`, and `updatedAt` fields.
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
  - `status`: Optional, filter by task status (enum: ["Pending", "InProgress", "Completed"]).
  - `priority`: Optional, filter by task priority (enum: ["Low", "Medium", "High"]).
  - `dueDate`: Optional, filter by due date (ISO 8601 format).
  - `sortBy`: Optional, sort by `DueDate`, `Priority`, or `CreatedAt`.
  - `page`: Required, integer, the page number for pagination.
  - `pageSize`: Required, integer, number of tasks per page.
- **Response:**
  - Returns a paginated list of tasks.
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
        ]
    }
    ```

#### `GET /api/tasks/{id}`
- **Description:** Retrieves a specific task by ID. Requires JWT authentication.
- **Response:**
  - Returns the task details.
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
  - `title`: Required, string.
  - `description`: Optional, string.
  - `status`: Required, enum value, one of ["Pending", "InProgress", "Completed"].
  - `priority`: Required, enum value, one of ["Low", "Medium", "High"].
  - `dueDate`: Optional, string, in ISO 8601 format.
    ```json
    {
        "title": "string",
        "description": "string",
        "status": "string",
        "priority": "string",
        "dueDate": "string"
    }
    ```
- **Response:**
  - Returns the updated task details.
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
- **Response:**
  - `204 No Content`: If the task was successfully deleted.
  - `404 Not Found`: If the task with the provided ID does not exist.

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