# Frontend 

Interfaz de usuario para la calculadora de tarifas de envio internacional.

## Tecnologias

- React
- Vite
- Bootstrap + React-Bootstrap
- Axios
- Framer Motion
- Lucide React

## Estructura principal

```text
src/
├── api/                 # Cliente HTTP, manejo de errores y servicios
├── features/calculator/ # Formulario y resultado de cotizacion
├── layouts/             # Navbar y Footer
└── utils/               # Logging y utilidades compartidas
```

## Configuracion

El frontend espera la variable `VITE_API_BASE_URL`.

Ejemplo en `.env`:

```env
VITE_API_BASE_URL=http://localhost:5235/api
```

## Ejecucion

```bash
npm install
npm run dev
```

## Build

```bash
npm run build
```

## Integracion con backend

Este frontend consume los siguientes endpoints:

- `GET /api/countries`
- `POST /api/shipment-quotes`
- `GET /api/shipment-quotes/{id}`

## Notas de calidad

- El cliente HTTP centraliza configuracion, trazabilidad y traduccion de errores.
- Los servicios desacoplan los componentes del detalle de Axios.
- La UI muestra errores funcionales sin cambiar el diseno visual base.
