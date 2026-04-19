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
├── DressRental.API          # Controllers, Middlewares, AppSettings
├── DressRental.Services     # Business Logic, Interfaces, AutoMapper Profiles, DTOs
├── DressRental.Repositories # DB Context, Entities (EF), Repository Implementations
└── DressRental.Tests        # Unit & Integration Tests1   
