# eShop — High-Throughput Order System

A production-minded e-commerce backend built with **.NET**, **Redis**, **RabbitMQ**, **SQL Server**, and **Docker** — designed around one real problem: what happens when a flash sale hits and thousands of users try to buy the same 10 items at once?

---

## The Problem: Overselling

Imagine you have one product — **iPhone 17 Pro** — with only **10 units in stock**. You run a flash offer: price drops from **$1,999 → $199**.

Hundreds of users rush to buy. That's great news for business. But here's what becomes a nightmare for the software:

> When thousands of requests hit the same resource at the same time, **race conditions happen** — two users read `stock = 1`, both think they can buy, both succeed, and now you've sold something you don't have.

That's overselling. This project handles it cleanly.

---

## How It's Solved — Step by Step

### 1. User hits the Place Order endpoint

A user sends a valid order request. Nothing fancy here — just clean input validation before anything else happens.

### 2. Skip the DB. Hit Redis first.

Instead of letting thousands of requests hammer SQL Server directly (massive I/O, locks, deadlocks), stock checks go through **Redis** — in-memory, microsecond-fast.

### 3. Atomic reservation via Lua Script

This is the core of the solution. For each item in the order:

```csharp
bool isReserved = await cacheService.ReserveStockAsync(cacheKey, item.Quantity);
```

Under the hood, `ReserveStockAsync` runs a **Lua script on Redis** — which means the check-and-decrement is a single atomic operation. No two requests can interleave here. Either you get the stock or you don't.

If an order has multiple items and one of them fails midway, everything already reserved gets **rolled back**:

```csharp
// Rollback previous allocations if a multi-item order fails midway
foreach (var key in reservedItems)
{
    await cacheService.IncrementStockAsync(key, item.Quantity);
}
```

### 4. Publish, don't wait

Once stock is reserved, an `OrderCreatedEvent` gets fired to **RabbitMQ**:

```csharp
await messageBus.PublishAsync(new OrderCreatedEvent(orderId, orderDto.UserId, items));
```

The request doesn't wait for the DB. The heavy work is offloaded to a consumer.

### 5. Return `202 Accepted` immediately

The client gets an instant response — stock was secured, order is being processed. On the frontend this is the moment to show something like *"Processing your order — don't close this screen"*.

The source of truth (SQL Server) hasn't been touched yet. That happens next.

### 6. Consumer picks it up and finishes the job

The `OrderCreatedConsumer` processes the event asynchronously:

- Fetches products and validates stock against the DB (second line of defense)
- Deducts inventory inside a **DB transaction**
- Uses **RowVersion** for optimistic concurrency — safe from concurrent updates at the DB level too
- Avoids **N+1 queries** by batching product lookups with a single `WHERE IN` query
- Saves the order, commits, then publishes `StockReservedEvent` to continue the chain (payment, email, etc.)

If a `DbUpdateConcurrencyException` is thrown — meaning another transaction modified the same row — it rolls back cleanly and surfaces a retryable error.

### 7. Client polls for status (pull strategy)

After receiving `202`, the frontend polls after 1–5 seconds to check order status and update the UI accordingly.

---

## Architecture

```
Client
  │
  ▼
[API Layer]  ──── Redis (atomic stock reservation)
  │
  └──► RabbitMQ ──► [OrderCreatedConsumer]
                          │
                          ├── SQL Server (orders + inventory)
                          ├── StockReservedEvent ──► (payment, email, ...)
                          └── DbUpdateConcurrencyException → rollback + retry signal
```

**Project structure follows clean layered architecture:**

```
├── API            → Controllers, middleware, entry point
├── Domain         → Entities, domain events, core models
├── Application    → Services, DTOs, interfaces
├── EF             → DbContext, migrations, configurations
└── Infrastructure → Redis, RabbitMQ, consumers, external integrations
```

---

## Tech Stack

| Layer | Technology |
|---|---|
| Backend | .NET |
| Database | SQL Server |
| Cache / Stock Lock | Redis + Lua scripting |
| Message Broker | RabbitMQ (via MassTransit) |
| Containerization | Docker |

---

## Key Engineering Decisions

**Why Redis for stock reservation?**
SQL Server under concurrent writes means locks, contention, and potential deadlocks. Redis with a Lua script gives us atomic check-and-decrement with no locking overhead — thousands of requests handled cleanly.

**Why async messaging instead of synchronous DB writes?**
The order placement path is now I/O-light and fast. The expensive work (DB writes, inventory updates, payment prep) runs asynchronously. This keeps the user experience snappy even under load.

**Why RowVersion in the consumer?**
Redis handles the cache layer. But the DB is the source of truth — and the consumer adds a second safety net using optimistic concurrency. If two consumers somehow process conflicting events, EF Core's concurrency token catches it.