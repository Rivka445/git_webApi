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

````markdown
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

## � Technical Features & Highlights

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

## �📂 Folder Structure

```text
├── DressRental.API          # Controllers, Middlewares, AppSettings
├── DressRental.Services     # Business Logic, Interfaces, AutoMapper Profiles, DTOs
├── DressRental.Repositories # DB Context, Entities (EF), Repository Implementations
└── DressRental.Tests        # Unit & Integration Tests1   

````

## 🐳 Docker

This repository includes a production-ready multi-stage `Dockerfile` (targeting .NET 8) and an optimized `.dockerignore` to keep the build context small.

Quick commands to build and run the image locally:

Build the image:

```powershell
docker build -t eventdressrental:latest .
```

Run the container (map port 5000 locally to container port 80):

```powershell
docker run -e "ASPNETCORE_URLS=http://+:80" -p 5000:80 --rm eventdressrental:latest
```

Notes:
- The Dockerfile publishes for `linux-x64` and trims unused dependencies. If you deploy to ARM (for example on Raspberry Pi or some cloud instances), change the RID accordingly.
- Do NOT include development configuration or secrets in images. Use environment variables or a secure secret store for production settings (for example the Redis connection string).
- The `.dockerignore` excludes `bin/`, `obj/`, `.vs/`, `appsettings.Development.json`, and logs to reduce image build size.

Redis and caching:
- The service uses `IDistributedCache` and the codebase includes optional support for StackExchange.Redis. Provide the Redis connection string via environment variables or `appsettings.Production.json` at deploy time.
- If you run locally and want Redis for caching, you can start a Redis container:

```powershell
docker run -d --name redis -p 6379:6379 redis:7
```

Then pass the connection string to the API container at run time, e.g. `-e "Redis__ConnectionString=host.docker.internal:6379"` (or use the redis container hostname when using Docker Compose).
