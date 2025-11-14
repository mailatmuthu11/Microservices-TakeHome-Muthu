# .NET Core Microservices Take-Home Project


This project is a small **microservices-based Order Processing System** built with **.NET 8**, **MassTransit**, and **RabbitMQ**. 
It demonstrates an **event-driven architecture** with clean separation of concerns.

---

## Project Structure

```
/OrderService.API
/PaymentService.API
/NotificationService.API
/docker-compose.yml
/README.md
```

Each service is organized following **Clean Architecture principles**:
- **API Layer:** Controllers, Swagger endpoints
- **Application Layer:** Business logic and interfaces
- **Domain Layer:** Entities and events
- **Infrastructure Layer:** Repositories and messaging
- **Middleware:** Global error handling

---

## Services Overview

| OrderService           | Creates orders and publishes `OrderCreatedEvent` | 8080 |
| PaymentService         | Processes orders by consuming `OrderCreatedEvent` and publishing `PaymentSucceededEvent` | 8081 |
| NotificationService    | Sends notifications by consuming `PaymentSucceededEvent` | 8082 |
| RabbitMQ               | Message broker for asynchronous communication   | 5672/15672 |

---

## Running the Services with Docker

### Prerequisites
- Docker and Docker Compose installed
- Ports 8080–8082 and 5672/15672 available

### Build and Start Services

From the root folder, run:
```bash
docker-compose up --build
```

### Access Services
- **OrderService Swagger:** http://localhost:8080/swagger
- **PaymentService Swagger:** http://localhost:8081/swagger
- **NotificationService Swagger:** http://localhost:8082/swagger
- **RabbitMQ Management UI:** http://localhost:15672 (guest/guest)

### Testing the Flow
1. Create a new order using OrderService Swagger.
2. PaymentService automatically processes the order.
3. NotificationService logs the notification to the console.

---

## 🏗 Architecture Overview
- **Event-driven:** Services communicate via RabbitMQ events.
- **Clean Architecture Layers:** Controllers (HTTP), Services (business logic), Repositories (data), Messaging (RabbitMQ).
- **Global Exception Middleware:** Handles errors consistently.
- **Swagger:** Enabled in Docker environment.

---

## 🔁 Event Flow
```
[OrderService] --(OrderCreatedEvent)--> [PaymentService] --(PaymentSucceededEvent)--> [NotificationService]
```

---

## Design Decisions & Assumptions
- Used **MassTransit** with RabbitMQ for messaging.
- **In-memory storage** to focus on event-driven architecture.
- Each service can run independently in Docker.
- Environment `Docker` ensures Swagger works inside containers.

---

## Known Limitations
- No database (in-memory only).
- Notifications are simulated with console logging.
- No authentication or authorization implemented.


---

## Unit Tests
- Each service has an xUnit + Moq test project.
- Run all tests:
```bash
dotnet test
```