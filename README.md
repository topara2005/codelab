# DemoApp - Docker Setup

A containerized application with ASP.NET Core API, React frontend, and Nginx reverse proxy with basic authentication.

## Prerequisites

- Docker Desktop (Windows/Mac) or Docker Engine (Linux)
- Docker Compose

## Quick Start

1. **Clone and navigate to the project:**
   ```bash
   cd c:\git\demo
   ```

2. **Build and run all containers:**
   ```bash
   docker-compose up -d --build
   ```

3. **Access the application:**
   - URL: http://localhost:8000
   - Username: `admin`
   - Password: `admin`

## Architecture

- **Nginx** (Port 8000) - Reverse proxy with Basic Auth
- **API** (Internal: 8080) - ASP.NET Core backend
- **UI** (Internal: 80) - React frontend


## Health Check

- Health endpoint (no auth required): http://localhost:8000/health

## Change Default Password

1. Install `htpasswd` utility
2. Generate new password:
   ```bash
   htpasswd -c nginx/.htpasswd yourusername
   ```
3. Rebuild nginx:
   ```bash
   docker-compose up -d --build nginx
   ```


## Notes
- All containers run on Linux
- API and UI are in separate containers
- Nginx handles authentication for both services
- EventSource (SSE) is used for real-time data streaming
