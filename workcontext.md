# 📊 Operational Engineering Manifest: workscontext.md

## 🚀 Local Bootstrap & Verification Sequence
To instantiate the multi-container Release image infrastructure model locally on your workstation, ensure your Docker Engine is active, open a terminal window inside the root directory, and execute the following sequence:

```bash
# 1. Clean previous dependency assembly caches and phantom container networks
docker compose down --remove-orphans

# 2. Compile image builds and bring up the container mesh networks in detached mode
docker compose up --build -d

# 3. Verify all three decoupled service layers are healthy and active
docker ps
```

## 🔌 Core Telemetry Endpoint Mappings
The system routes transactions and background telemetry points across the following unencrypted HTTP channel pathways:

*   **Interactive System API Specs Docs:** `GET http://localhost:5000/api-docs` (Exposes custom Swagger Operation Filters documenting the JWT auth gates and payload constraints).
*   **High-Velocity Redis Memory Buffer Feed:** `GET http://localhost:5000/api/ledger/state-summary` (Serves cached JSON dashboard metrics, updating dynamically every 30 seconds).
*   **Asynchronous Attendance Gateway Channel:** `POST http://localhost:5000/attendance/process-scan` (Receives plaintext body payloads from the local Python camera scanner node).
*   **Atomic Stored Procedure Settlement Portal:** `POST http://localhost:5000/api/ledger/settle-sp` (Requires an 'Authorization: Bearer <TOKEN>' header to process remote mobile POS terminal transactions).

---

## 🐍 Local Python Edge Worker Execution Setup
To initiate your local video capture camera stream hardware layer at a physical scanning station, execute these terminal directives on your device:

```bash
# Install the required image decoding and network serialization dependencies
pip install opencv-python requests

# Launch the camera node worker script
python attendance_scanner.py
```
*Point any barcode or QR label token containing a registered username at your lens to trace real-time background logs inside your MS SQL Server instance database tables!*
