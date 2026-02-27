# Microservices Architecture Plan for EventDressRental

## Executive Summary

This plan outlines the migration strategy for transforming the current monolithic EventDressRental application into a microservices architecture. The migration will be phased to minimize risk while maximizing the benefits of distributed systems.

---

## 1. Service Decomposition (Domain-Driven Design)

Based on analysis of the current codebase, I've identified the following bounded contexts and corresponding microservices:

### Core Microservices

| Service | Bounded Context | Current Components | Responsibilities |
|---------|-----------------|-------------------|------------------|
| **User Service** | Identity & Access | `UserService`, `UserPasswordService`, `UserRepository`, `UserPasswordRepository` | Authentication, authorization, user profiles, password management |
| **Catalog Service** | Product Catalog | `CategoryService`, `ModelService`, `DressService` + related repositories | Dress inventory, categories, model management, availability |
| **Order Service** | Order Management | `OrderService`, `OrderRepository` | Order lifecycle, order items, checkout process |
| **Rating Service** | Feedback & Reviews | `RatingService`, `RatingRepository` | Customer reviews, dress ratings, aggregated scores |

### Supporting Services

| Service | Responsibility |
|---------|---------------|
| **API Gateway** | Request routing, rate limiting, authentication passthrough |
| **Notification Service** | Email notifications, SMS alerts (future) |
| **Media Service** | Image storage/retrieval for dress photos (wwwroot migration) |

### Domain Model Mapping

```
┌─────────────────────────────────────────────────────────────────┐
│                        API GATEWAY                               │
└─────────────────────────────────────────────────────────────────┘
                              │
        ┌─────────────────────┼─────────────────────┐
        │                     │                     │
        ▼                     ▼                     ▼
┌───────────────┐    ┌───────────────┐    ┌───────────────┐
│  USER SERVICE │    │CATALOG SERVICE│    │ ORDER SERVICE │
│               │    │               │    │               │
│ • User        │    │ • Category    │    │ • Order       │
│ • UserPassword│    │ • Model       │    │ • OrderItem   │
│               │    │ • Dress       │    │ • Status      │
└───────────────┘    └───────────────┘    └───────────────┘
        │                     │                     │
        │                     ▼                     │
        │            ┌───────────────┐              │
        └───────────►│RATING SERVICE │◄─────────────┘
                     │               │
                     │ • Rating      │
                     └───────────────┘
```

---

## 2. Tech Stack Selection

### Service-Specific Recommendations

| Service | Language/Framework | Database | Justification |
|---------|-------------------|----------|---------------|
| **User Service** | **C# / .NET 9** | **SQL Server** | Security-critical; leverage existing .NET security libraries, Identity integration, strong typing for sensitive data |
| **Catalog Service** | **C# / .NET 9** | **PostgreSQL + Redis Cache** | Read-heavy workload; Redis for caching dress availability; maintain team expertise |
| **Order Service** | **C# / .NET 9** | **SQL Server** | ACID compliance critical for transactions; complex business rules benefit from C# type system |
| **Rating Service** | **Node.js / Express** or **Go** | **MongoDB** | High I/O, simple CRUD operations; document model fits review data; Go if expecting high concurrency |
| **API Gateway** | **YARP** (C#) or **Kong** | N/A | YARP maintains .NET ecosystem; Kong for advanced features |
| **Notification Service** | **Node.js** | **Redis (queue)** | Event-driven, I/O intensive, excellent async handling |
| **Media Service** | **Go** | **Azure Blob/S3 + metadata in PostgreSQL** | High throughput for binary data, excellent concurrency |

### Technical Justifications

1. **C# / .NET 9 for Core Services**
   - Maintains team expertise and existing code patterns
   - Excellent async/await support already in codebase
   - Strong EF Core integration for SQL operations
   - Native gRPC support for inter-service communication

2. **Node.js for Rating/Notification Services**
   - Event-loop model perfect for I/O-bound operations
   - Lower overhead for simple CRUD operations
   - Rich ecosystem for notification providers (SendGrid, Twilio)

3. **Go for Media Service**
   - Superior concurrency model for file handling
   - Low memory footprint for high-throughput scenarios
   - Compiled binary simplifies container deployment

---

## 3. Inter-Service Communication Strategy

### Communication Matrix

| Producer | Consumer | Pattern | Protocol | Justification |
|----------|----------|---------|----------|---------------|
| API Gateway | All Services | Sync | **REST/HTTP** | Standard request/response for client-facing APIs |
| Order Service | Catalog Service | Sync | **gRPC** | Low latency stock checks during checkout |
| Order Service | User Service | Sync | **gRPC** | Validate user during order creation |
| Order Service | Notification Service | Async | **RabbitMQ** | Fire-and-forget order confirmations |
| Catalog Service | Rating Service | Async | **RabbitMQ** | Rating aggregation updates |
| User Service | Notification Service | Async | **RabbitMQ** | Welcome emails, password resets |

### Communication Architecture

```
                    ┌─────────────────────────────────────┐
                    │           MESSAGE BROKER            │
                    │          (RabbitMQ/Kafka)           │
                    │                                     │
                    │  Exchanges:                         │
                    │  • order.events                     │
                    │  • user.events                      │
                    │  • notification.queue               │
                    └─────────────────────────────────────┘
                           ▲         ▲         ▲
                           │         │         │
         ┌─────────────────┴─────────┴─────────┴──────────┐
         │                                                 │
    ┌────┴────┐    ┌──────────┐    ┌────────┐    ┌────────┴───┐
    │  ORDER  │───►│ CATALOG  │    │  USER  │    │NOTIFICATION│
    │ SERVICE │gRPC│ SERVICE  │    │SERVICE │    │  SERVICE   │
    └─────────┘    └──────────┘    └────────┘    └────────────┘
```

### Protocol Selection Rationale

- **REST**: External-facing APIs via gateway; industry standard, easy debugging
- **gRPC**: Internal service-to-service; binary protocol, code generation, streaming support
- **RabbitMQ over Kafka**: Current scale doesn't require Kafka's throughput; RabbitMQ offers simpler operations, better .NET integration via MassTransit

---

## 4. Database Strategy

### Recommendation: **Database per Service**

Each microservice owns its data exclusively. This ensures:
- Independent deployability
- Technology flexibility per service
- Fault isolation
- Clear ownership boundaries

### Database Allocation

| Service | Database | Type | Schema Migration Tool |
|---------|----------|------|----------------------|
| **User Service** | `eventdress_users` | SQL Server | EF Core Migrations |
| **Catalog Service** | `eventdress_catalog` | PostgreSQL | EF Core Migrations |
| **Order Service** | `eventdress_orders` | SQL Server | EF Core Migrations |
| **Rating Service** | `eventdress_ratings` | MongoDB | Mongoose/Native |
| **Notification Service** | Redis | Key-Value | N/A |

### Data Ownership & Synchronization

```
┌─────────────────────────────────────────────────────────────┐
│                    DATA OWNERSHIP MAP                        │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│  USER SERVICE          CATALOG SERVICE      ORDER SERVICE    │
│  ┌──────────────┐     ┌──────────────┐     ┌──────────────┐ │
│  │ Users        │     │ Categories   │     │ Orders       │ │
│  │ UserPasswords│     │ Models       │     │ OrderItems   │ │
│  │              │     │ Dresses      │     │ Status       │ │
│  └──────────────┘     └──────────────┘     └──────────────┘ │
│         │                    │                    │          │
│         └────────────────────┼────────────────────┘          │
│                              ▼                               │
│                    ┌──────────────────┐                      │
│                    │  RATING SERVICE  │                      │
│                    │  ┌────────────┐  │                      │
│                    │  │ Ratings    │  │                      │
│                    │  │ (denorm    │  │                      │
│                    │  │  user_id,  │  │                      │
│                    │  │  dress_id) │  │                      │
│                    │  └────────────┘  │                      │
│                    └──────────────────┘                      │
└─────────────────────────────────────────────────────────────┘
```

### Handling Cross-Service Queries

| Scenario | Solution |
|----------|----------|
| Order needs User details | Store `userId` reference; fetch via gRPC on demand |
| Rating needs Dress name | Denormalize `dressName` at write time; eventual consistency |
| Catalog availability check | Saga pattern for rental reservations |

### Saga Pattern for Order Processing

```
Order Created → Reserve Dress → Validate User → Confirm Payment → Complete
     │              │               │               │              │
     └──── Compensating transactions if any step fails ◄──────────┘
```

---

## 5. Infrastructure & Deployment Strategy

### Container Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                     KUBERNETES CLUSTER                           │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  ┌─────────────────────────────────────────────────────────┐    │
│  │                    INGRESS CONTROLLER                    │    │
│  │                    (NGINX / Traefik)                     │    │
│  └─────────────────────────────────────────────────────────┘    │
│                              │                                   │
│  ┌─────────────────────────────────────────────────────────┐    │
│  │                      API GATEWAY                         │    │
│  │              (YARP / Kong / Ocelot)                      │    │
│  │   • Rate Limiting  • Auth  • Routing  • Load Balancing   │    │
│  └─────────────────────────────────────────────────────────┘    │
│                              │                                   │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │                   SERVICE MESH (optional)                 │   │
│  │                   (Istio / Linkerd)                       │   │
│  └──────────────────────────────────────────────────────────┘   │
│                              │                                   │
│  ┌────────────┬────────────┬────────────┬────────────────────┐  │
│  │   USER     │  CATALOG   │   ORDER    │   RATING           │  │
│  │  SERVICE   │  SERVICE   │  SERVICE   │   SERVICE          │  │
│  │  (2 pods)  │  (3 pods)  │  (2 pods)  │   (2 pods)         │  │
│  └────────────┴────────────┴────────────┴────────────────────┘  │
│                                                                  │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │                    INFRASTRUCTURE                         │   │
│  │  ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌──────────┐  │   │
│  │  │ RabbitMQ │  │  Redis   │  │SQL Server│  │ MongoDB  │  │   │
│  │  │ (HA)     │  │ Cluster  │  │ (HA)     │  │ Replica  │  │   │
│  │  └──────────┘  └──────────┘  └──────────┘  └──────────┘  │   │
│  └──────────────────────────────────────────────────────────┘   │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

### Service Discovery

**Recommendation**: Kubernetes-native service discovery

- Each service registered as K8s Service
- DNS-based discovery: `user-service.eventdress.svc.cluster.local`
- Health checks via liveness/readiness probes
- Alternative: Consul for hybrid/multi-cloud scenarios

### API Gateway Configuration

```yaml
# Gateway Routing Schema (conceptual)
routes:
  - path: /api/users/**
    service: user-service
    auth: required
    rateLimit: 100/min
    
  - path: /api/catalog/**
    service: catalog-service
    auth: optional
    cache: 5min
    
  - path: /api/orders/**
    service: order-service
    auth: required
    rateLimit: 50/min
    
  - path: /api/ratings/**
    service: rating-service
    auth: optional
```

### Docker Strategy

Each service will have:
```dockerfile
# Multi-stage build pattern
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
# Build stage

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
# Runtime stage - minimal image
```

### CI/CD Pipeline Stages

```
┌────────┐   ┌────────┐   ┌────────┐   ┌────────┐   ┌────────┐
│  Code  │──►│ Build  │──►│  Test  │──►│ Deploy │──►│Monitor │
│ Commit │   │& Scan  │   │        │   │  Staging   │        │
└────────┘   └────────┘   └────────┘   └────────┘   └────────┘
                                            │
                                            ▼
                                       ┌────────┐
                                       │  Prod  │
                                       │ Deploy │
                                       └────────┘
```

---

## 6. Step-by-Step Migration Roadmap

### Phase 1: Foundation (Weeks 1-3)

| Step | Task | Deliverable |
|------|------|-------------|
| 1.1 | Set up Git monorepo structure | `/services/{user,catalog,order,rating}` |
| 1.2 | Create shared libraries project | `EventDressRental.Shared` (DTOs, contracts) |
| 1.3 | Docker Compose for local development | `docker-compose.yml` with all dependencies |
| 1.4 | API Gateway scaffolding (YARP) | Basic routing to monolith |

### Phase 2: Strangler Fig Pattern - User Service (Weeks 4-6)

| Step | Task | Deliverable |
|------|------|-------------|
| 2.1 | Extract User entities & migrations | Separate database schema |
| 2.2 | Create User Service API | REST + gRPC endpoints |
| 2.3 | Implement authentication middleware | JWT validation |
| 2.4 | Route `/api/users` through gateway | Monolith still handles other routes |
| 2.5 | Integration tests | Verify existing functionality |

### Phase 3: Catalog Service (Weeks 7-9)

| Step | Task | Deliverable |
|------|------|-------------|
| 3.1 | Extract Category, Model, Dress | PostgreSQL migration |
| 3.2 | Implement Redis caching layer | Dress availability cache |
| 3.3 | gRPC contracts for Order Service | Stock check protocol |
| 3.4 | Route `/api/categories`, `/api/models`, `/api/dresses` | Gateway routing |

### Phase 4: Order Service (Weeks 10-12)

| Step | Task | Deliverable |
|------|------|-------------|
| 4.1 | Extract Order, OrderItem, Status | SQL Server schema |
| 4.2 | Implement Saga orchestrator | Order processing workflow |
| 4.3 | RabbitMQ integration | Event publishing |
| 4.4 | Compensating transactions | Rollback handlers |

### Phase 5: Rating & Notification Services (Weeks 13-15)

| Step | Task | Deliverable |
|------|------|-------------|
| 5.1 | Create Rating Service (Node.js/Go) | MongoDB integration |
| 5.2 | Create Notification Service | Email/event handlers |
| 5.3 | Event consumers | RabbitMQ subscriptions |

### Phase 6: Infrastructure Hardening (Weeks 16-18)

| Step | Task | Deliverable |
|------|------|-------------|
| 6.1 | Kubernetes manifests | Deployments, Services, ConfigMaps |
| 6.2 | Helm charts per service | Parameterized deployments |
| 6.3 | Observability stack | Prometheus, Grafana, Jaeger |
| 6.4 | Centralized logging | ELK/Loki (migrate from NLog files) |

### Phase 7: Production Deployment (Weeks 19-20)

| Step | Task | Deliverable |
|------|------|-------------|
| 7.1 | Staging environment validation | Full E2E tests |
| 7.2 | Blue-green deployment setup | Zero-downtime strategy |
| 7.3 | Production rollout | Incremental traffic shift |
| 7.4 | Decommission monolith | Archive old codebase |

---

## 7. Observability & Cross-Cutting Concerns

### Distributed Tracing

```
Request ID propagation across all services:
  Gateway → User Service → Order Service → Notification
     │          │              │              │
     └──────────┴──────────────┴──────────────┘
                       │
              ┌────────┴────────┐
              │     JAEGER      │
              │  Trace Storage  │
              └─────────────────┘
```

### Logging Strategy

| Component | Current | Target |
|-----------|---------|--------|
| File logging | NLog → files | Structured JSON to stdout |
| Aggregation | Manual | Loki/Elasticsearch |
| Correlation | None | Distributed trace IDs |

### Health Checks

Each service exposes:
- `/health/live` - Liveness (is the process running?)
- `/health/ready` - Readiness (can accept traffic?)
- `/health/startup` - Startup probe (initial dependencies)

---

## 8. Risk Mitigation

| Risk | Mitigation Strategy |
|------|---------------------|
| Data consistency across services | Saga pattern + idempotency keys |
| Network partition | Circuit breakers (Polly), retry policies |
| Service discovery failure | DNS caching, fallback endpoints |
| Database migration errors | Blue-green database migrations, feature flags |
| Team learning curve | Phased rollout, maintain C# for core services |

---

## 9. Success Metrics

| Metric | Current State | Target |
|--------|---------------|--------|
| Deployment frequency | Manual | Multiple per day per service |
| Mean time to recovery | Hours | Minutes |
| Service independence | 0% | 100% (independent deployments) |
| Horizontal scalability | Vertical only | Auto-scaling per service |

---

## Summary

This plan transforms EventDressRental from a monolith into four core microservices (User, Catalog, Order, Rating) plus supporting infrastructure. The migration uses the Strangler Fig pattern to minimize risk, maintains C# expertise for core services while introducing polyglot options for supporting services, and establishes a robust Kubernetes-based deployment platform.

**Estimated Timeline**: 20 weeks (5 months)  
**Team Size Recommendation**: 3-4 developers + 1 DevOps engineer

---

*Document created: February 27, 2026*
