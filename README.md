# 👗 Dress Rental System - Server Side (REST API)

A backend system for managing dress rentals, implemented as a modern **REST API** using **ASP.NET 9** and **C#**.  
The system is designed with a focus on high performance, scalability, and strict separation of concerns between business logic and data layers.

---

## 🏗 Architecture & Project Structure

The project is built using a **3-Layer Architecture**, which allows for easy maintenance, high-quality testing, and decoupled dependencies:

1. **Application Layer (Web API)** - Management of Controllers and Routing definitions.  
   - Implementation of Middlewares for handling HTTP requests and error management.  
   - Centralized **Dependency Injection** configuration.  

2. **Services Layer** - The business logic layer.  
   - Mediates between Controllers and Repositories.  
   - Performs data validation and processing.  
   - Executed **asynchronously** to release server resources.  

3. **Repositories Layer** - Data access using the **Repository Pattern**.  
   - Use of **Entity Framework Core** in a **Database First** approach.  
   - CRUD operations are performed **asynchronously** to improve performance and scalability.  

---

## 🛠 Technical Features & Highlights

### ⚡ Performance and Scalability
- **Asynchronous Programming:** Use of `async/await` in all layers to free up threads and allow for high scalability.  
- **Dependency Injection:** Use of **Dependency Injection (DI)** to create modular and flexible code.  

### 🔄 Data Management and Mapping
- **DTOs (Data Transfer Objects):** A DTO layer to prevent circular dependencies and separate the database from the API.  
- **C# Records:** DTOs are represented as `records` to ensure immutable objects and efficient data transfer.  
- **AutoMapper:** Automatic mapping between Entities and DTOs to maintain clean code.  

### 📊 Monitoring, Logs, and Error Handling
- **NLog:** Logging system operations and errors.  
- **Error Handling Middleware:** Uniform error handling and log redirection.  
- **Auditing:** All traffic and ratings are saved in the `Rating` table for analysis and tracking.  
- **Configuration:** Configurations are stored in `appsettings.json` files outside of the code.  

---

## 🧪 Testing

- **Unit Tests:** Isolated unit tests for the services.  
- **Integration Tests:** Integration tests to ensure synchronization between all layers and the database.  

---



## 📂 Folder Structure

```text
├── WebApiShop          # Controllers, Middlewares, AppSettings
├── Services            # Business Logic, Interfaces, DTOs
├── Repositories        # DB Context, Entities, Repository Implementations
└── Tests               # Unit & Integration Tests
```

---

## 🐳 Docker (minimal)

Build image:

```powershell
docker build -t eventdressrental:latest .
```

Run (example):

```powershell
docker run -e "ASPNETCORE_URLS=http://+:80" -e "Redis__ConnectionString=<your-redis>" -p 5000:80 --rm eventdressrental:latest
```

Short notes:
- Image built from `Dockerfile` using .NET 8.
- Pass secrets (Redis, DB) via env vars or a secret manager; do not bake them into the image.
- `.dockerignore` is included to reduce build context.
