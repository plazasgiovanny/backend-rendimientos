# Backend Rendimientos — Actividad Sumativa U3 (Arquitectura Frontend)

Backend mínimo (C# / .NET 9, minimal API) que simula el servidor real detrás del módulo **Rendimientos**. Existe para que el prototipo de React (`prototipo-rendimientos/`, repo hermano) haga una llamada HTTP real en vez de leer datos mock en memoria — así el patrón Flux se implementa fiel al ejemplo oficial de Facebook (`flux-async`), donde el Store reacciona a datos que llegan de un servidor real, no de una función local.

## Cómo correrlo

```bash
dotnet run
```

Queda escuchando en `http://localhost:5193`. CORS está habilitado solo para `http://localhost:5173` (el dev server de Vite del frontend).

## Endpoints

| Método | Ruta | Respuesta |
|---|---|---|
| `GET` | `/api/cuenta` | `{ "saldoTotal": 10000, "tasaAnualPorcentaje": 33 }` |
| `GET` | `/api/historial?dias=7` | `[{ "fechaHora": "2026-07-31T10:03:00-05:00", "monto": 47 }, ...]` (7 días que suman $321) |

`fechaHora` lleva el offset de Colombia (`-05:00`) explícito, no una fecha suelta — así el cliente nunca tiene que adivinar en qué zona horaria interpretar el dato.

Nota: `/api/cuenta` **no** devuelve `rendimientoAcumulado` — ese valor lo calcula el Store del frontend a partir del historial recibido, para mantener la consistencia maestro-detalle por diseño (ver `prototipo-rendimientos/src/stores/rendimientosStore.ts`).

## Alcance

Mínimo a propósito (dado el plazo de la entrega): solo estos dos endpoints, sin base de datos, sin simulación de latencia/errores de red (a diferencia del ejemplo oficial `flux-async`, que sí las incluye). Los datos son los mismos 7 días fijos que ya estaban en `rendimientosService.ts` del frontend.
