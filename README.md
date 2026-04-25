# Calculadora de tarifas de envío internacional

Aplicación web para calcular tarifas de envío internacional según el peso del paquete y el país de destino. La solución está construida con Clean Architecture en .NET para el backend, React con Vite para el frontend y SQL Server para la persistencia.

## Objetivo

Permitir que el usuario ingrese el peso del paquete, seleccione un país de destino y obtenga automáticamente el costo final del envío según las tarifas configuradas por país.

## Reglas de negocio iniciales

- India = peso x 5 USD
- Estados Unidos = peso x 8 USD
- Reino Unido = peso x 10 USD

## Tecnologías utilizadas

- Backend: ASP.NET Core Web API
- Frontend: React + Vite
- UI: Bootstrap + CSS
- Persistencia: EF Core + SQL Server
- Arquitectura: Clean Architecture
- Diagramas: imágenes y recursos UML dentro de `diagramas/`

## Estructura del proyecto

- `backend/`: solución .NET y proyectos organizados por capas
- `frontend/`: aplicación web React
- `scriptBD/`: scripts SQL para crear el esquema y cargar los datos iniciales
- `diagramas/`: diagramas generados de la solución

## Endpoints principales

- `GET /api/countries`: obtiene los países disponibles
- `POST /api/shipment-quotes`: calcula y registra una cotización
- `GET /api/shipment-quotes/{id}`: consulta una cotización registrada

## Requisitos previos

- .NET SDK 9
- Node.js y npm
- SQL Server

## Configuración de base de datos

1. Ejecuta `scriptBD/001_create_schema.sql`.
2. Ejecuta `scriptBD/002_seed_data.sql`.
3. Verifica la cadena de conexión en `backend/src/ShippingRates.Api/appsettings.Development.json`.

Ejemplo:

```json
{
  "ConnectionStrings": {
    "TarifasEnvioDb": "Server=DESKTOP-B307NU0;Database=TarifasEnvioDb;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

Nota: `backend/src/ShippingRates.Api/appsettings.json` queda sin cadena real para no acoplar la configuración general a una máquina específica. La conexión local debe configurarse en `appsettings.Development.json`.

## Ejecutar backend

```bash
dotnet run --project backend/src/ShippingRates.Api
```

La API queda disponible por defecto en:

- `http://localhost:5235`
- `https://localhost:7292`

## Ejecutar frontend

```bash
cd frontend
npm install
copy .env.example .env
npm run dev
```

## Configuración del frontend

Define `VITE_API_BASE_URL` en `frontend/.env`.

Ejemplo:

```env
VITE_API_BASE_URL=http://localhost:5235/api
```

## Documentación entregable

- `diagramas/diagrama-diseno-solucion.png`
- `diagramas/diagrama-capas.png`
- `diagramas/diagrama-componentes.png`

## Notas de implementación

- Las tarifas y países se consultan desde la base de datos para facilitar la escalabilidad.
- La solución permite agregar nuevos países y tarifas sin modificar la interfaz principal.
- El sistema registra cotizaciones para trazabilidad y crecimiento futuro del módulo.
