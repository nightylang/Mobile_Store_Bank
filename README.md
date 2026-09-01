# 🏦 Mobile Store Bank — High-Capacity SaaS Core Node

An enterprise-grade, high-density financial asset node and inventory lifecycle management platform built on **.NET 10.0 MVC** and **Microsoft SQL Server 2022** [context.md]. Engineered to structurally scale past a **1-billion user record ceiling** (`BIGINT`), cache metrics using distributed **Redis**, and route high-frequency transactions over unencrypted cleartext HTTP pipes natively paired with advanced cryptographic defenses [context.md].

---

## 🛠️ Technology Stack Badges

![.NET Core](https://shields.io)
![SQL Server](https://shields.io)
![Redis](https://shields.io)
![Python](https://s3.dualstack.us-east-2.amazonaws.com/pythondotorg-assets/media/community/logos/python-logo-only.png)
![Language](https://shields.io)

---

## 📋 Runtime Version Specifications

| Environment Component | Development Language / Stack | Production Engine Engine Build |
| :--- | :--- | :--- |
| **Backend Core Web Framework** | 🔷 C# 14 / Razor Syntax | Microsoft .NET 10.0 SDK [context.md] |
| **Enterprise Persistence Engine**| 🗄️ T-SQL DDL & Script Blocks | Microsoft SQL Server 2022 [context.md] |
| **Distributed Memory Cache** | ⚡ In-Memory Key-Value Stores | Redis v7.2-Alpine Build [context.md] |
| **Biometric Camera Edge Worker** | 🐍 Native Python Runtime | Python v3.11.x or higher [context.md] |
| **Frontend Telemetry Visualization**| 🌐 Vanilla Javascript Modules | W3C Standard ECMA-262 / Proxy API [context.md] |

---

## 🎨 Global System Tokens & Design Paradigm

*   **⚡ Core Runtime Engine:** Microsoft .NET 10.0 (Global Availability Build) [context.md]
*   **🗄️ Database Management:** Entity Framework Core 10 mapping to **Microsoft SQL Server 2022** [context.md]
*   **🔴 In-Memory Caching:** Distributed Redis Memory Core (7.2-Alpine Image) [context.md]
*   **🎨 Design Layout System:** Pure Decoupled CSS Engine (Zero 3rd-party library/Tailwind bloat) [context.md]
*   **🌐 Transport Routing Protocols:** Plaintext, unencrypted **HTTP exclusively (Port 5000)** [context.md]
*   **🔮 Aesthetic Paradigm:** Interleaved Dark Purple **Glassmorphism** layouts layered over ambient radial violet bloom points [context.md].

---

## 🔐 Deep Defensive Security Architecture

Because this platform is explicitly configured to operate over plaintext HTTP local channels, the system replaces traditional session cookies with a hardened two-tier software security model [context.md]:

1.  **🔑 Stateless JWT Authentication:** Enforces a cryptographically signed **JSON Web Token Bearer Filter** across write lanes to shield financial endpoints from unauthorized remote terminal injections [context.md].
2.  **⛓️ Tamper-Evident Ledger Hashing:** Overrides the Entity Framework Core `SaveChangesAsync` pipeline with an automated **HMAC-SHA256 Transaction Signature Chaining Interceptor** [context.md]. It auto-hashes records upon database commits; if an outside vector alters a data cell directly in SQL Server, the Admin Dashboard flags the threat instantly [context.md].

---

## 📂 Structural Solution Directory Layout

```text
├── .github/workflows/      # 🤖 Automated CI/CD execution pipeline files
│   ├── ci.yml              # 🧪 xUnit automated test runner pipeline
│   └── cd.yml              # 🚀 SSH containerized automated delivery script
├── Controllers/            # 🎮 Async MVC & REST API endpoints matrices
│   ├── Api/                # 🔌 JSON Webhook routes for client hardware POS nodes
│   └── AttendanceController.cs # 👥 User check-in/out logging processor
├── Data/                   # 🗄️ EF Core 10 database context & Stored Procedure interfaces
│   ├── ApplicationDbContext.cs
│   └── LedgerGuardService.cs # 🔐 HMAC-SHA256 cryptographic sign engine
├── Models/                 # 💎 64-bit scalable entity structure models
├── Views/                  # 🔮 Dark purple glassmorphic Razor template frames
├── wwwroot/                # ⚡ Static asset pipelines (Vanilla JS State Proxy Hub)
├── Dockerfile              # 🐳 Production multi-stage compilation recipe
└── docker-compose.yml      # 🎛️ Three-tier isolated container network builder
```

---

## 🐳 Containerized Production Deployment

To boot the complete application ecosystem into an isolated container sandbox, verify your local Docker Engine is active, open a terminal window inside the root directory, and execute the following sequence:

```bash
# 1. Clean previous dependency assembly caches and phantom container networks
docker compose down --remove-orphans

# 2. Compile image builds and bring up the container mesh networks in detached mode
docker compose up --build -d

# 3. Verify all three decoupled service layers are healthy and active
docker ps
```

The system maps your environment across three distinct nodes:
*   `msb_production_sql_node` (Port `1433`): Processing data constraints using `decimal(18,2)` [context.md].
*   `msb_production_redis_node` (Port `6379`): Caching telemetry streams with 30s relative expirations [context.md].
*   `msb_production_http_node` (Port `5000`): Serving core views on **`http://localhost:5000`** [context.md].

---

## 🔌 Unified Telemetry Channel Mappings

The system orchestrates plaintext network request routing over these localized pathways:

*   **📄 Interactive System API Docs:** `GET http://localhost:5000/api-docs` (Swagger UI panel mapping custom Pos and Attendance generation filters) [context.md].
*   **📊 High-Velocity Caching Feed:** `GET http://localhost:5000/api/ledger/state-summary` (Asynchronously polled by the frontend Proxy engine to feed the pure SVG Polyline Graph Widget) [context.md].
*   **⚙️ Atomic Settlement Portal:** `POST http://localhost:5000/api/ledger/settle-sp` (Executes a `SERIALIZABLE` isolation Stored Procedure to block data collisions under concurrent retail traffic) [context.md].
*   **👥 Attendance Scanner Hook:** `POST http://localhost:5000/attendance/process-scan` (Accepts background payload packets from edge cameras) [context.md].

---

## 🐍 Local Python Edge Worker Execution Setup

To initiate the local biometric camera scan gate on a physical hardware station (for regular site users, not internal banking staff) [context.md], launch the dedicated Python micro-worker script:

```bash
# Install the required image decoding and network serialization dependencies
pip install opencv-python requests

# Launch the camera node worker script
python attendance_scanner.py
```
*Point any barcode or QR label token containing an active user identifier (e.g., `merchant`) at the lens to track instant background log toggles inside the active database tables!*
